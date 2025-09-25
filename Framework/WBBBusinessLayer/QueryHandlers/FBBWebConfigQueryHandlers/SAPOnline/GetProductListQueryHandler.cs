using System;
using System.Collections.Generic;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBContract.Queries.FBBWebConfigQueries.SAPOnline;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Models;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers.SAPOnline
{
    public class GetProductListQueryHandler : IQueryHandler<GetProductListQuery, SubmitFOAResend>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBSS_FOA_SUBMIT_ORDER> _submitOrder;
        private readonly IEntityRepository<FBSS_FOA_SUBMIT_ORDER_DTL> _submitOrderDtl;
        private readonly IEntityRepository<FBSS_FIXED_ASSET_TRAN_LOG> _fixedSubmitOrderLog;
        private readonly IQueryHandler<GetFixedAssetConfigQuery, List<LovModel>> _queryProcessor;
        public GetProductListQueryHandler(
            ILogger logger,
            IQueryHandler<GetFixedAssetConfigQuery, List<LovModel>> queryProcessor,
            IEntityRepository<FBSS_FOA_SUBMIT_ORDER> submitOrder,
            IEntityRepository<FBSS_FOA_SUBMIT_ORDER_DTL> submitOrderDtl,
            IEntityRepository<FBSS_FIXED_ASSET_TRAN_LOG> fixedSubmitOrderLog,
            IWBBUnitOfWork uow)
        {
            _logger = logger;
            _submitOrder = submitOrder;
            _submitOrderDtl = submitOrderDtl;
            _fixedSubmitOrderLog = fixedSubmitOrderLog;
            _queryProcessor = queryProcessor;
            _uow = uow;
        }

        public SubmitFOAResend Handle(GetProductListQuery query)
        {

            var _main = (from m in _submitOrder.Get()
                         where m.ORDER_NO == query.OrderNo
                         where m.ACCESS_NUMBER == query.AccessNo

                         select new SubmitFOAResend
                         {
                             AccessNo = m.ACCESS_NUMBER,
                             OrderNumber = m.ORDER_NO,
                             SubcontractorCode = m.SUBCONTRACT_CODE,
                             SubcontractorName = m.SUBCONTRACT_NAME,
                             ProductName = m.PRODUCT_NAME,
                             ServiceName = m.SERVICE_LIST,
                             OrderType = m.ORDER_TYPE,
                             SubmitFlag = m.SUBMIT_FLAG,
                             RejectReason = m.REJECT_REASON,
                             FOA_Submit_date_value = m.FOA_SUBMIT_DATE,
                             OLT_NAME = m.OLT_NAME,
                             BUILDING_NAME = m.BUILDING_NAME,
                             Mobile_Contact = m.MOBILE_CONTACT,
                             ADDRESS_ID = m.ADDRESS_ID,
                             ORG_ID = m.ORG_ID,
                             REUSE_FLAG = m.REUSE_FLAG,
                             EVENT_FLOW_FLAG = m.EVENT_FLOW_FLAG,
                             SUBCONTRACT_TYPE = m.SUBCONTRACT_TYPE,
                             SUBCONTRACT_SUB_TYPE = m.SUBCONTRACT_SUB_TYPE,
                             REQUEST_SUB_FLAG = m.REQUEST_SUB_FLAG,
                             SUB_ACCESS_MODE = m.SUB_ACCESS_MODE,
                             PRODUCT_OWNER = m.PRODUCT_OWNER,
                             MAIN_PROMO_CODE = m.MAIN_PROMO_CODE,
                             TEAM_ID = m.TEAM_ID


                         }).First();


            var resultFixAssConfig = GET_FBSS_FIXED_ASSET_CONFIG("Flag_RollbackSAP").FirstOrDefault();

            if (resultFixAssConfig.DISPLAY_VAL == "Y")
            {


                var alloweREC_TYPE = new[] { "F", "M" };
                var _product = (from log in _fixedSubmitOrderLog.Get()
                                join dtl in _submitOrderDtl.Get() on new { c1 = log.ORDER_NO, c2 = log.SERIAL_NO } equals new { c1 = dtl.ORDER_NO, c2 = dtl.SN }
                                where log.ORDER_NO == query.OrderNo
                                where log.INTERNET_NO == query.AccessNo
                                where log.NEXT_TRAN_ID == null
                                where log.PREV_TRAN_ID == null
                                where alloweREC_TYPE.Contains(log.REC_TYPE)
                                where log.MATERIAL_NO != "11999999" || log.MATERIAL_NO == null
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
                                    log.ERR_MSG,
                                    log.TRAN_STATUS
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
                                    ErrorMassage = grp.Key.ERR_MSG,
                                    Status = grp.Key.TRAN_STATUS
                                }).ToList();

                _main.ProductList = _product;




                var myInClause = new string[] { "I", "E" };
                var _install = (from log in _fixedSubmitOrderLog.Get()
                                join ord in _submitOrder.Get() on new { c1 = log.INTERNET_NO, c2 = log.ORDER_NO } equals new { c1 = ord.ACCESS_NUMBER, c2 = ord.ORDER_NO }
                                where log.ORDER_NO == query.OrderNo
                                where log.INTERNET_NO == query.AccessNo
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

                _main.InstallList = _install;


            }
            else
            {

                var alloweREC_TYPE_S4 = new[] { "A", "C" };
                var _product_s4 = (from log in _fixedSubmitOrderLog.Get()
                                   join dtl in _submitOrderDtl.Get() on new { c1 = log.ORDER_NO, c2 = log.SERIAL_NO } equals new { c1 = dtl.ORDER_NO, c2 = dtl.SN }
                                   where log.ORDER_NO == query.OrderNo
                                   where log.INTERNET_NO == query.AccessNo
                                   where log.NEXT_TRAN_ID == null
                                   where log.PREV_TRAN_ID == null
                                   where alloweREC_TYPE_S4.Contains(log.REC_TYPE)
                                   where log.MATERIAL_NO != "11999999" || log.MATERIAL_NO == null
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
                                       log.ERR_MSG,
                                       log.TRAN_STATUS
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
                                       ErrorMassage = grp.Key.ERR_MSG,
                                       Status = grp.Key.TRAN_STATUS
                                   }).ToList();

                _main.ProductList = _product_s4;



                var myInClause_s4 = new string[] { "B" };
                var _install_s4 = (from log in _fixedSubmitOrderLog.Get()
                                   join ord in _submitOrder.Get() on new { c1 = log.INTERNET_NO, c2 = log.ORDER_NO } equals new { c1 = ord.ACCESS_NUMBER, c2 = ord.ORDER_NO }
                                   where log.ORDER_NO == query.OrderNo
                                   where log.INTERNET_NO == query.AccessNo
                                   where log.NEXT_TRAN_ID == null
                                   where log.PREV_TRAN_ID == null
                                   where myInClause_s4.Contains(log.REC_TYPE)
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

                _main.InstallList = _install_s4;

            }


            return _main;
        }



        public List<LovModel> GET_FBSS_FIXED_ASSET_CONFIG(string product_name)
        {
            var query = new GetFixedAssetConfigQuery()
            {
                Program = product_name
            };
            var _FbssConfig = _queryProcessor.Handle(query);

            return _FbssConfig;
        }

    }
}
