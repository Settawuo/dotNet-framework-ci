using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries.SAPOnline;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.FBBWebConfigModels;

/* change history
 *ch0001 29/01/2020 --Get data revalue-- validate Accessno  แทนการ validate date from and date to
 */

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers.SAPOnline
{
    public class SubmitFOAEquipmentReportQueryHandler : IQueryHandler<SubmitFOAEquipmentReportQuery, SubmitFOAEquipmentListReturn>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        //private readonly IEntityRepository<SubmitFOAEquipmentReport> _submitOrderLog;
        private readonly IEntityRepository<SubmitFOAEquipment> _submitEquipmentLog;
        private readonly IEntityRepository<FBSS_CONFIG_TBL> _fbssConfigTBL;

        public SubmitFOAEquipmentReportQueryHandler(
            ILogger logger,
            //IEntityRepository<SubmitFOAEquipmentReport> submitOrderLog
            IEntityRepository<SubmitFOAEquipment> submitEquipmentLog,
            IEntityRepository<FBSS_CONFIG_TBL> fbssConfigTBL
            )
        {
            _logger = logger;
            //_submitOrderLog = submitOrderLog;
            _submitEquipmentLog = submitEquipmentLog;
            _fbssConfigTBL = fbssConfigTBL;
        }

        public SubmitFOAEquipmentListReturn Handle(SubmitFOAEquipmentReportQuery query)
        {
            //ch0001 start
            //string[] subStr = query.dateFrom.Split('/');
            //string dateFrom = subStr[0].ToSafeString() + subStr[1].ToSafeString() + subStr[2].ToSafeString();

            //subStr = query.dateTo.Split('/');
            //var dateTo = subStr[0].ToSafeString() + subStr[1].ToSafeString() + subStr[2].ToSafeString();

            string[] subStr = null;

            string dateFrom = "";
            string dateTo = "";

            if (query.dateFrom != "" && query.dateTo != "")
            {
                subStr = query.dateFrom.Split('/');
                dateFrom = subStr[0].ToSafeString() + subStr[1].ToSafeString() + subStr[2].ToSafeString();

                subStr = query.dateTo.Split('/');
                dateTo = subStr[0].ToSafeString() + subStr[1].ToSafeString() + subStr[2].ToSafeString();
            }
            //ch0001 end
            var p_ret_code = new OracleParameter();
            p_ret_code.ParameterName = "ret_code";
            p_ret_code.OracleDbType = OracleDbType.Decimal;
            p_ret_code.Direction = ParameterDirection.Output;

            var p_equip_cur = new OracleParameter();
            p_equip_cur.ParameterName = "cur";
            p_equip_cur.OracleDbType = OracleDbType.RefCursor;
            p_equip_cur.Direction = ParameterDirection.Output;
            var returnForm = new SubmitFOAEquipmentListReturn();

            #region R21.11 comment PAGESIZE and WBB.P_FBB_GET_EQUIP Only

            //var resuftlov = (from c in _fbssConfigTBL.Get()
            //                 where c.CON_TYPE == "RESEND_ORDER"
            //                 where c.CON_NAME == "PAGESIZE"
            //                 select new FbssConfigTBL
            //                 {
            //                     ACTIVEFLAG = c.ACTIVEFLAG == null ? "N" : c.ACTIVEFLAG
            //                 }).FirstOrDefault();
            //string StrPKG = "WBB.PKG_FBB_FOA_ORDER_MANAGEMENT.P_GET_EQUIP";
            //if (resuftlov != null)
            //    StrPKG = resuftlov.ACTIVEFLAG == "N" ? "WBB.PKG_FBB_FOA_ORDER_MANAGEMENT.P_GET_EQUIP" : "WBB.P_FBB_GET_EQUIP";

            #endregion

            try
            {
                var executeResult = _submitEquipmentLog.ExecuteReadStoredProc("WBB.P_FBB_GET_EQUIP",
                  new
                  {
                      p_ORDER_NO = query.orderNo.ToSafeString(),
                      p_INTERNET_NO = query.internetNo.ToSafeString(),
                      p_PRODUCT_NAME = query.productName.ToSafeString(),
                      p_SERVICE_NAME = query.serviceName.ToSafeString(),
                      p_ORDER_TYPE = query.orderType.ToSafeString(),
                      p_SUBCONT_CODE = query.subcontractorCode.ToSafeString(),
                      p_COM_CODE = query.companyCode.ToSafeString(),
                      p_MATERIAL_NO = query.materialCode.ToSafeString(),
                      p_PLANT = query.plant.ToSafeString(),
                      p_STORAGE = query.storLocation.ToSafeString(),
                      p_STATUS = query.status.ToSafeString(),
                      p_ERR_MSG = query.errormessage.ToSafeString(),
                      p_ORDER_FROM = dateFrom,
                      p_ORDER_TO = dateTo,
                      P_PRODUCT_OWNER = query.productOwner.ToSafeString(),
                      P_PAGE_INDEX = query.page_index,
                      P_PAGE_SIZE = query.page_size,
                      // return code
                      //0 = Success/Send, 1 = Error/Not Send, 2 = Success/Not Send
                      ret_code = p_ret_code,
                      cur = p_equip_cur
                  }).ToList();


                returnForm.ret_code = p_ret_code.Value != null ? p_ret_code.Value.ToSafeString() : "-1";
                returnForm.cur = executeResult;
                return returnForm;
            }
            catch (Exception ex)
            {
                _logger.Info(ex.GetErrorMessage());
                return null;
            }
        }
    }
}
