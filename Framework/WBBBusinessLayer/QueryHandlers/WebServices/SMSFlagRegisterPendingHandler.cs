using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.WebServices;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class SMSFlagRegisterPendingHandler : IQueryHandler<SMSFlagRegisterPendingQuery, SMSFlagRegisterPendingModel>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<string> _objService;

        public SMSFlagRegisterPendingHandler(ILogger logger, IWBBUnitOfWork uow, IEntityRepository<FBB_INTERFACE_LOG> intfLog, IEntityRepository<string> objService)
        {
            _logger = logger;
            _uow = uow;
            _intfLog = intfLog;
            _objService = objService;
        }

        public SMSFlagRegisterPendingModel Handle(SMSFlagRegisterPendingQuery query)
        {
            InterfaceLogCommand log = null;
            var data = new SMSFlagRegisterPendingModel();
            var resultMessage = string.Empty;
            try
            {
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow,
                    _intfLog,
                    query,
                    string.IsNullOrEmpty(query.InternetNo) ? query.OrderId : query.InternetNo,
                    "CheckSMSFlagRegisterPending",
                    "CheckSMSFlagRegisterPendingHandler",
                    "",
                    "WBB",
                    "");

                if (query.Action == "Check")
                {

                    var p_order_id = new OracleParameter();
                    p_order_id.ParameterName = "p_order_id";
                    p_order_id.OracleDbType = OracleDbType.Varchar2;
                    p_order_id.Direction = ParameterDirection.Input;
                    p_order_id.Value = query.OrderId;

                    var p_status = new OracleParameter();
                    p_status.ParameterName = "p_status";
                    p_status.OracleDbType = OracleDbType.Varchar2;
                    p_status.Direction = ParameterDirection.Input;
                    p_status.Value = query.Status;

                    var p_mobile_no = new OracleParameter();
                    p_mobile_no.ParameterName = "p_mobile_no";
                    p_mobile_no.OracleDbType = OracleDbType.Varchar2;
                    p_mobile_no.Direction = ParameterDirection.Input;
                    p_mobile_no.Value = query.Mobile_No;

                    var p_option = new OracleParameter();
                    p_option.ParameterName = "P_OPTION";
                    p_option.OracleDbType = OracleDbType.Varchar2;
                    p_option.Direction = ParameterDirection.Input;
                    p_option.Value = query.Option.ToSafeString();

                    var ret_code = new OracleParameter();
                    ret_code.ParameterName = "ret_code";
                    ret_code.OracleDbType = OracleDbType.Varchar2;
                    ret_code.Size = 2000;
                    ret_code.Direction = ParameterDirection.Output;

                    var ret_message = new OracleParameter();
                    ret_message.ParameterName = "ret_message";
                    ret_message.OracleDbType = OracleDbType.Varchar2;
                    ret_message.Size = 2000;
                    ret_message.Direction = ParameterDirection.Output;

                    var ret_send_sms = new OracleParameter();
                    ret_send_sms.ParameterName = "ret_send_sms";
                    ret_send_sms.OracleDbType = OracleDbType.Varchar2;
                    ret_send_sms.Size = 2000;
                    ret_send_sms.Direction = ParameterDirection.Output;


                    var result = _objService.ExecuteStoredProc("WBB.PKG_FBB_REGISTER_PAYMENT.PROC_CHECK_SMS_FLAG",
                         new
                         {
                             p_order_id = p_order_id,
                             p_mobile_no = p_mobile_no,
                             p_status = p_status,
                             p_option = p_option,
                             //  return code
                             ret_code = ret_code,
                             ret_message = ret_message,
                             ret_send_sms = ret_send_sms

                         });

                    if (ret_code != null && ret_code.Value.ToSafeString() == "0")
                    {
                        data.SendSMS_Flag = ret_send_sms.Value.ToSafeString();
                    }
                    resultMessage = ret_message != null ? ret_message.Value.ToSafeString() : "";

                }
                else if (query.Action == "Update")
                {

                    var p_order_id = new OracleParameter();
                    p_order_id.ParameterName = "p_order_id";
                    p_order_id.OracleDbType = OracleDbType.Varchar2;
                    p_order_id.Direction = ParameterDirection.Input;
                    p_order_id.Value = query.OrderId;

                    var p_status = new OracleParameter();
                    p_status.ParameterName = "p_status";
                    p_status.OracleDbType = OracleDbType.Varchar2;
                    p_status.Direction = ParameterDirection.Input;
                    p_status.Value = query.Status;

                    var p_mobile_no = new OracleParameter();
                    p_mobile_no.ParameterName = "p_mobile_no";
                    p_mobile_no.OracleDbType = OracleDbType.Varchar2;
                    p_mobile_no.Direction = ParameterDirection.Input;
                    p_mobile_no.Value = query.Mobile_No;

                    var p_sms_flag = new OracleParameter();
                    p_sms_flag.ParameterName = "p_sms_flag";
                    p_sms_flag.OracleDbType = OracleDbType.Varchar2;
                    p_sms_flag.Direction = ParameterDirection.Input;
                    p_sms_flag.Value = query.SMS_Flag;

                    var p_update_by = new OracleParameter();
                    p_update_by.ParameterName = "p_update_by";
                    p_update_by.OracleDbType = OracleDbType.Varchar2;
                    p_update_by.Direction = ParameterDirection.Input;
                    p_update_by.Value = query.UpdateBy;

                    var p_option = new OracleParameter();
                    p_option.ParameterName = "P_OPTION";
                    p_option.OracleDbType = OracleDbType.Varchar2;
                    p_option.Direction = ParameterDirection.Input;
                    p_option.Value = query.Option.ToSafeString();

                    var ret_code = new OracleParameter();
                    ret_code.ParameterName = "ret_code";
                    ret_code.OracleDbType = OracleDbType.Varchar2;
                    ret_code.Size = 2000;
                    ret_code.Direction = ParameterDirection.Output;

                    var ret_message = new OracleParameter();
                    ret_message.ParameterName = "ret_message";
                    ret_message.OracleDbType = OracleDbType.Varchar2;
                    ret_message.Size = 2000;
                    ret_message.Direction = ParameterDirection.Output;

                    var result = _objService.ExecuteStoredProc("WBB.PKG_FBB_REGISTER_PAYMENT.PROC_UPD_SMS_FLAG",
                         new
                         {
                             p_order_id = p_order_id,
                             p_mobile_no = p_mobile_no,
                             p_status = p_status,
                             p_sms_flag = p_sms_flag,
                             p_update_by = p_update_by,
                             p_option = p_option,
                             //  return code
                             ret_code = ret_code,
                             ret_message = ret_message
                         });

                    resultMessage = ret_message != null ? ret_message.Value.ToSafeString() : "";
                }

                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, data, log, resultMessage, string.Empty, "");

            }
            catch (Exception ex)
            {
                _logger.Info("CheckSMSFlagRegisterPending : Error.");
                _logger.Info(ex.Message);

                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, "", log, "Error", ex.StackTrace, "");

                throw;
            }

            return data;
        }
    }
}
