using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using WBBBusinessLayer.QueryHandlers;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Commands.WebServices;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;

namespace WBBBusinessLayer.CommandHandlers.WebServices
{
    public class SavePaymentSPDPLogHandler : ICommandHandler<SavePaymentSPDPLogCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<string> _objService;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;

        public SavePaymentSPDPLogHandler(ILogger logger,
            IEntityRepository<string> objService,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IWBBUnitOfWork uow)
        {
            _logger = logger;
            _objService = objService;
            _intfLog = intfLog;
            _uow = uow;
        }

        public void Handle(SavePaymentSPDPLogCommand command)
        {

            InterfaceLogCommand log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, command, command.p_order_id,
                    "SavePaymentSPDPLog", "SavePaymentSPDPLogHandler", null,
                    "FBB|", "WEB");
            try
            {
                var p_req_xml_param = new OracleParameter();
                p_req_xml_param.OracleDbType = OracleDbType.Clob;
                p_req_xml_param.ParameterName = "p_req_xml_param";
                p_req_xml_param.Direction = ParameterDirection.Input;

                var p_res_xml_param = new OracleParameter();
                p_res_xml_param.OracleDbType = OracleDbType.Clob;
                p_res_xml_param.ParameterName = "p_res_xml_param";
                p_res_xml_param.Direction = ParameterDirection.Input;

                var ret_code = new OracleParameter();
                ret_code.OracleDbType = OracleDbType.Varchar2;
                ret_code.ParameterName = "ret_code";
                ret_code.Size = 2000;
                ret_code.Direction = ParameterDirection.Output;

                var ret_message = new OracleParameter();
                ret_message.OracleDbType = OracleDbType.Varchar2;
                ret_message.ParameterName = "ret_message";
                ret_message.Size = 2000;
                ret_message.Direction = ParameterDirection.Output;

                var outp = new List<object>();
                var paramOut = outp.ToArray();

                var executeResult = _objService.ExecuteStoredProc("WBB.PKG_FBB_REGISTER_PAYMENT.PROC_INS_PAYMENT_LOG_SPDP",
                    out paramOut,
                    new
                    {
                        //in
                        p_action = command.p_action.ToSafeString(),
                        p_user_name = command.p_user_name.ToSafeString(),
                        p_non_mobile_no = command.p_non_mobile_no.ToSafeString(),
                        p_service_name = command.p_service_name.ToSafeString(),
                        p_endpoint = command.p_endpoint.ToSafeString(),
                        p_order_id = command.p_order_id.ToSafeString(),
                        p_txn_id = command.p_txn_id.ToSafeString(),
                        p_status = command.p_status.ToSafeString(),
                        p_status_code = command.p_status_code.ToSafeString(),
                        p_status_message = command.p_status_message.ToSafeString(),
                        p_channel = command.p_channel.ToSafeString(),
                        p_amount = command.p_amount.ToSafeString(),
                        p_req_xml_param = command.p_req_xml_param.ToSafeString(),
                        p_res_xml_param = command.p_res_xml_param.ToSafeString(),
                        p_order_transaction_id = command.p_order_transaction_id.ToSafeString(),
                        /// Out
                        ret_code = ret_code,
                        ret_message = ret_message

                    });
                command.ret_code = ret_code.Value.ToSafeString();
                command.ret_message = ret_message.Value.ToSafeString();

                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, command, log, "Success", "", "");

            }
            catch (Exception ex)
            {
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex, log, "Failed", ex.GetErrorMessage(), "");
                command.ret_code = "-1";
                command.ret_message = "Error SavePaymentSPDPLogHandler " + ex.Message;
            }
        }
    }

    public static class SavePaymentSPDPLogHelper
    {
        public static void Log(IEntityRepository<string> _objService, SavePaymentSPDPLogCommand command)
        {
            try
            {
                var p_req_xml_param = new OracleParameter();
                p_req_xml_param.OracleDbType = OracleDbType.Clob;
                p_req_xml_param.ParameterName = "p_req_xml_param";
                p_req_xml_param.Direction = ParameterDirection.Input;

                var p_res_xml_param = new OracleParameter();
                p_res_xml_param.OracleDbType = OracleDbType.Clob;
                p_res_xml_param.ParameterName = "p_res_xml_param";
                p_res_xml_param.Direction = ParameterDirection.Input;

                var ret_code = new OracleParameter();
                ret_code.OracleDbType = OracleDbType.Varchar2;
                ret_code.ParameterName = "ret_code";
                ret_code.Size = 2000;
                ret_code.Direction = ParameterDirection.Output;

                var ret_message = new OracleParameter();
                ret_message.OracleDbType = OracleDbType.Varchar2;
                ret_message.ParameterName = "ret_message";
                ret_message.Size = 2000;
                ret_message.Direction = ParameterDirection.Output;

                var outp = new List<object>();
                var paramOut = outp.ToArray();

                var executeResult = _objService.ExecuteStoredProc("WBB.PKG_FBB_REGISTER_PAYMENT.PROC_INS_PAYMENT_LOG_SPDP",
                    out paramOut,
                    new
                    {
                        //in
                        p_action = command.p_action.ToSafeString(),
                        p_user_name = command.p_user_name.ToSafeString(),
                        p_non_mobile_no = command.p_non_mobile_no.ToSafeString(),
                        p_service_name = command.p_service_name.ToSafeString(),
                        p_endpoint = command.p_endpoint.ToSafeString(),
                        p_order_id = command.p_order_id.ToSafeString(),
                        p_txn_id = command.p_txn_id.ToSafeString(),
                        p_status = command.p_status.ToSafeString(),
                        p_status_code = command.p_status_code.ToSafeString(),
                        p_status_message = command.p_status_message.ToSafeString(),
                        p_channel = command.p_channel.ToSafeString(),
                        p_amount = command.p_amount.ToSafeString(),
                        p_req_xml_param = command.p_req_xml_param.ToSafeString(),
                        p_res_xml_param = command.p_res_xml_param.ToSafeString(),
                        p_order_transaction_id = command.p_order_transaction_id.ToSafeString(),
                        /// Out
                        ret_code = ret_code,
                        ret_message = ret_message

                    });
                command.ret_code = ret_code.Value.ToSafeString();
                command.ret_message = ret_message.Value.ToSafeString();

            }
            catch (Exception ex)
            {
                command.ret_code = "-1";
                command.ret_message = "Error SavePaymentSPDPLogHelper " + ex.Message;
            }
        }
    }

}
