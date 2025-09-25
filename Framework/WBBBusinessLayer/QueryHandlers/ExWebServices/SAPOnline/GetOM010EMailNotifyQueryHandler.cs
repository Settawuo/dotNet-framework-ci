using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels;
namespace WBBBusinessLayer.QueryHandlers.ExWebServices.SAPOnline
{
    public class GetOM010EMailNotifyQueryHandler : IQueryHandler<GetOM010EMailNotifyQuery, ReturnFBSSOM010Notify>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_HISTORY_LOG> _historyLog;
        private readonly IEntityRepository<ReturnFBSSOM010Notify> _objService;
        private readonly IWBBUnitOfWork _uow;
        public GetOM010EMailNotifyQueryHandler(
               ILogger logger, IWBBUnitOfWork uow,
              IEntityRepository<ReturnFBSSOM010Notify> objService,
              IEntityRepository<FBB_HISTORY_LOG> historyLog)
        {
            _logger = logger;
            _uow = uow;
            _historyLog = historyLog;
            _objService = objService;


        }
        public ReturnFBSSOM010Notify Handle(GetOM010EMailNotifyQuery query)
        {
            var historyLog = new FBB_HISTORY_LOG();
            // ReturnFBSSOM010Notify Result = new ReturnFBSSOM010Notify();
            var returnForm = new ReturnFBSSOM010Notify();
            try
            {
                #region OracleParameter


                var ret_code = new OracleParameter();
                ret_code.OracleDbType = OracleDbType.Decimal;
                ret_code.Direction = ParameterDirection.Output;

                var msg = new OracleParameter();
                msg.OracleDbType = OracleDbType.Varchar2;
                msg.Size = 2000;
                msg.Direction = ParameterDirection.Output;

                #endregion
                //var result = _objService.ExecuteReadStoredProc(
                //      "WBB.PKG_FBBPAYG_LOAD_OM010.p_check_load_om",
                //      new
                //      {
                //          ret_code,
                //          msg,
                //      }).FirstOrDefault();




                //returnForm.ret_code = result.ret_code.ToSafeString();
                //returnForm.msg = result.msg.ToSafeString() != null ? msg.Value.ToString() : "";
                var executeResult = _objService.ExecuteStoredProc("WBB.PKG_FBBPAYG_LOAD_OM010.p_check_load_om",
                    new
                    {

                        /// return //////
                        ret_code = ret_code,
                        msg = msg

                    });
                if (executeResult != null)
                {
                    query.ret_code = ret_code.Value != null ? Convert.ToInt32(ret_code.Value.ToSafeString()) : -1;
                    query.msg = msg.Value.ToSafeString();
                    returnForm.ret_code = query.ret_code.ToSafeString();
                    returnForm.msg = query.msg.ToSafeString();
                }

                _logger.Info("Call WBB.PKG_FBBPAYG_LOAD_OM010.p_check_load_om: ret_code : " + ret_code + "msg:" + msg);


                return returnForm;
            }
            catch (Exception ex)
            {
                historyLog.HISTORY_ID = historyLog.HISTORY_ID;
                historyLog.ACTION = ActionHistory.ADD.ToString();
                historyLog.APPLICATION = "Batch FBSSOM010SendEMailNotification";
                historyLog.CREATED_BY = "BATCH FBSS";
                historyLog.CREATED_DATE = DateTime.Now;
                historyLog.DESCRIPTION = "ErrorFBSSOM010SendEMailNotificationHandler " + ex.GetErrorMessage();
                historyLog.REF_KEY = "FBSSOM010NotificationBatchJob";
                historyLog.REF_NAME = "NODEID";
                _historyLog.Create(historyLog);
                _uow.Persist();
                returnForm.ret_code = "-1";
                returnForm.msg = ex.GetErrorMessage();

                return returnForm;
            }



        }

    }
}
