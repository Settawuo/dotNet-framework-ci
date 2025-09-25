using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries.SAPOnline;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers.SAPOnline
{
    class SubmitFOAReportQueryHandler : IQueryHandler<SubmitFOAReportQuery, List<SubmitFOAReport>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<SubmitFOAReport> _submitOrderLog;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBSS_FOA_SUBMIT_ORDER> _submitOrder;
        private readonly IEntityRepository<FBSS_FOA_SUBMIT_ORDER_DTL> _submitOrderDtl;
        private readonly IEntityRepository<FBSS_FIXED_ASSET_TRAN_LOG> _fixedSubmitOrderLog;
        private readonly IEntityRepository<SubmitFOAProduct> _submitProductLog;
        private readonly IEntityRepository<SubmitFOAInstall> _submitInstallLog;

        public SubmitFOAReportQueryHandler(
            ILogger logger,
            IEntityRepository<SubmitFOAReport> submitOrderLog,
            IWBBUnitOfWork uow, IEntityRepository<FBSS_FOA_SUBMIT_ORDER> submitOrder,
            IEntityRepository<FBSS_FOA_SUBMIT_ORDER_DTL> submitOrderDtl,
            IEntityRepository<FBSS_FIXED_ASSET_TRAN_LOG> fixedSubmitOrderLog,
            IEntityRepository<SubmitFOAProduct> submitProductLog,
            IEntityRepository<SubmitFOAInstall> submitInstallLog)
        {
            _logger = logger;
            _submitOrderLog = submitOrderLog;
            _submitOrder = submitOrder;
            _submitOrderDtl = submitOrderDtl;
            _fixedSubmitOrderLog = fixedSubmitOrderLog;
            _submitProductLog = submitProductLog;
            _submitInstallLog = submitInstallLog;
        }

        public List<SubmitFOAReport> Handle(SubmitFOAReportQuery query)
        {
            string[] subStr = query.dateFrom.Split('/');
            var dateFrom = new DateTime(subStr[2].ToSafeInteger(), subStr[1].ToSafeInteger(), subStr[0].ToSafeInteger());

            subStr = query.dateTo.Split('/');
            var dateTo = new DateTime(subStr[2].ToSafeInteger(), subStr[1].ToSafeInteger(), subStr[0].ToSafeInteger());

            var p_list_trans_log_cur = new OracleParameter();
            p_list_trans_log_cur.ParameterName = "p_list_trans_log_cur";
            p_list_trans_log_cur.OracleDbType = OracleDbType.RefCursor;
            p_list_trans_log_cur.Direction = ParameterDirection.Output;

            List<SubmitFOAReport> executeResult = _submitOrderLog.ExecuteReadStoredProc("WBB.PKG_FBB_FOA_ORDER_MANAGEMENT3.get_list_trans_log",
              new
              {
                  p_accessNo = query.accessNo.ToSafeString(),
                  p_orderNumber = query.orderNo.ToSafeString(),
                  p_status = query.status.ToSafeString(),
                  p_orderCreateFrom = dateFrom,
                  p_orderCreateTo = dateTo,
                  ret_list_trans_log = p_list_trans_log_cur
              }).ToList();

            foreach (var item in executeResult)
            {
                try
                {
                    var p_list_product_cur = new OracleParameter();
                    p_list_product_cur.ParameterName = "p_list_product_cur";
                    p_list_product_cur.OracleDbType = OracleDbType.RefCursor;
                    p_list_product_cur.Direction = ParameterDirection.Output;

                    var p_list_install_cur = new OracleParameter();
                    p_list_install_cur.ParameterName = "p_list_install_cur";
                    p_list_install_cur.OracleDbType = OracleDbType.RefCursor;
                    p_list_install_cur.Direction = ParameterDirection.Output;

                    //List<SubmitFOAProduct> _product = _submitProductLog.ExecuteReadStoredProc("WBB.PKG_FBB_FOA_ORDER_MANAGEMENT3.get_list_product_old",
                    //                     new
                    //                     {
                    //                         p_orderNo = item.ORDER_NO.ToSafeString(),
                    //                         p_accessNumber = item.ACCESS_NUMBER.ToSafeString(),
                    //                         ret_list_product = p_list_product_cur
                    //                     }).ToList();

                    //List<SubmitFOAInstall> _install = _submitInstallLog.ExecuteReadStoredProc("WBB.PKG_FBB_FOA_ORDER_MANAGEMENT3.get_list_install_old",
                    //                     new
                    //                     {
                    //                         p_orderNo = item.ORDER_NO.ToSafeString(),
                    //                         p_accessNumber = item.ACCESS_NUMBER.ToSafeString(),
                    //                         ret_list_install = p_list_install_cur
                    //                     }).ToList();

                    //item.InstallList = _install;
                    //item.ProductList = _product;

                    var _product = (from log in _fixedSubmitOrderLog.Get()
                                    join dtl in _submitOrderDtl.Get() on new { c1 = log.ORDER_NO, c2 = log.SERIAL_NO } equals new { c1 = dtl.ORDER_NO, c2 = dtl.SN }
                                    where log.ORDER_NO == item.ORDER_NO
                                    && log.INTERNET_NO == item.ACCESS_NUMBER
                                    && log.NEXT_TRAN_ID == null
                                    && log.PREV_TRAN_ID == null
                                    && log.REC_TYPE == "F"
                                    && log.MATERIAL_NO != "11999999"

                                    group new { log, dtl } by new
                                    {
                                        dtl.SN,
                                        dtl.MATERIAL_CODE,
                                        dtl.COMPANY_CODE,
                                        dtl.PLANT,
                                        dtl.STORAGE_LOCATION,
                                        dtl.SNPATTERN,
                                        dtl.MOVEMENT_TYPE,
                                        log.ERR_CODE,
                                        log.ERR_MSG
                                    } into grp
                                    select new SubmitFOAProduct
                                    {
                                        SerialNumber = grp.Key.SN,
                                        MaterialCode = grp.Key.MATERIAL_CODE,
                                        CompanyCode = grp.Key.COMPANY_CODE,
                                        Plant = grp.Key.PLANT,
                                        StorageLocation = grp.Key.STORAGE_LOCATION,
                                        SNPattern = grp.Key.SNPATTERN,
                                        MovementType = grp.Key.MOVEMENT_TYPE,
                                        ErrorCode = grp.Key.ERR_CODE,
                                        ErrorMassage = grp.Key.ERR_MSG
                                    }).ToList();

                    var myInClause = new string[] { "I", "E" };
                    var _install = (from log in _fixedSubmitOrderLog.Get()
                                    join ord in _submitOrder.Get() on new { c1 = log.INTERNET_NO, c2 = log.ORDER_NO } equals new { c1 = ord.ACCESS_NUMBER, c2 = ord.ORDER_NO }
                                    where log.ORDER_NO == item.ORDER_NO
                                    where log.INTERNET_NO == item.ACCESS_NUMBER
                                    where log.NEXT_TRAN_ID == null
                                    where log.PREV_TRAN_ID == null
                                    where myInClause.Contains(log.REC_TYPE)
                                    group new { log, ord } by new
                                    {
                                        MAIN_ASSET = log.MAIN_ASSET,
                                        SUB_NUMBER = log.SUB_NUMBER,
                                        SUBCONTRACT_CODE = ord.SUBCONTRACT_CODE,
                                        SUBCONTRACT_NAME = ord.SUBCONTRACT_NAME,
                                        COM_CODE = log.COM_CODE,
                                        INSTALLATION_COST = ord.INSTALLATION_COST,
                                        ERR_MSG = log.ERR_MSG
                                    } into grp
                                    select new SubmitFOAInstall
                                    {
                                        MAIN_ASSET = grp.Key.MAIN_ASSET,
                                        SUB_NUMBER = grp.Key.SUB_NUMBER,
                                        SUBCONTRACT_CODE = grp.Key.SUBCONTRACT_CODE,
                                        SUBCONTRACT_NAME = grp.Key.SUBCONTRACT_NAME,
                                        COM_CODE = grp.Key.COM_CODE,
                                        INSTALLATION_COST = grp.Key.INSTALLATION_COST,
                                        ERR_MSG = grp.Key.ERR_MSG
                                    }).ToList();

                    item.InstallList = _install;
                    item.ProductList = _product;

                }
                catch (Exception ex)
                {

                }

            }
            //var report = (from f in _fixedAsset.Get().OrderByDescending(p => p.TRANS_ID)
            //              join s in _submitOrder.Get() on f.INTERNET_NO equals s.ACCESS_NUMBER into s2
            //              from m in s2.DefaultIfEmpty()


            //             select new SubmitFOAReport()
            //                {
            //                    TranId = f.TRANS_ID,
            //                    AccessNo = m.ACCESS_NUMBER,
            //                    OrderNumber = m.ORDER_NO,
            //                    SubcontractorCode = m.SUBCONTRACT_CODE,
            //                    SubcontractorName = m.SUBCONTRACT_NAME,
            //                    ProductName = m.PRODUCT_NAME,
            //                    ServiceName = m.SERVICE_LIST,
            //                    OrderType = m.ORDER_TYPE,
            //                    submitFlag = m.SUBMIT_FLAG,
            //                    status = f.TRAN_STATUS,
            //                    remark = f.ERR_MSG
            //                }).ToList();

            //var sada = report.GroupBy(l => new { l.TranId, l.AccessNo })
            //    .Select(g => new SubmitFOAReport
            //    {
            //        TranId = g.Key.TranId,
            //        AccessNo = g.Key.AccessNo,
            //        status = string.Join(",", g.Select(i => i.status)),
            //        remark = g.Select(i => i.status == "ERROR" ? "ERROR" : "SUCCESS").ToString()
            //    }).ToList();


            return executeResult;
        }
    }
}
