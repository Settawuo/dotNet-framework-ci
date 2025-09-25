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
    public class SaveDeductionLogHandler : ICommandHandler<SaveDeductionLogCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<string> _objService;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;

        public SaveDeductionLogHandler(ILogger logger,
            IEntityRepository<string> objService,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IWBBUnitOfWork uow)
        {
            _logger = logger;
            _objService = objService;
            _intfLog = intfLog;
            _uow = uow;
        }

        public void Handle(SaveDeductionLogCommand command)
        {
            InterfaceLogCommand log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, command,
                !string.IsNullOrEmpty(command.p_transaction_id) ? command.p_transaction_id : command.p_order_transaction_id,
                    "SaveDeductionLog", "SaveDeductionLogHandler", null,
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

                var executeResult = _objService.ExecuteStoredProc("WBB.PKG_FBB_REGISTER_PAYMENT.PROC_INS_DEDUCTION_LOG",
                    out paramOut,
                    new
                    {
                        //in
                        p_action = command.p_action.ToSafeString(),
                        p_user_name = command.p_user_name.ToSafeString(),
                        p_transaction_id = command.p_transaction_id.ToSafeString(),
                        p_mobile_no = command.p_mobile_no.ToSafeString(),
                        p_service_name = command.p_service_name.ToSafeString(),
                        p_endpoint = command.p_endpoint.ToSafeString(),
                        p_pm_tux_code = command.p_pm_tux_code.ToSafeString(),
                        p_pm_receipt_num = command.p_pm_receipt_num.ToSafeString(),
                        p_enq_status = command.p_enq_status.ToSafeString(),
                        p_enq_status_code = command.p_enq_status_code.ToSafeString(),
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
                command.ret_message = "Error SavePaymentLogCommandHandler " + ex.Message;
            }
        }
    }

    public static class DeductionLogHelper
    {
        public static void Log(IEntityRepository<string> _objService, SaveDeductionLogCommand command)
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

                var executeResult = _objService.ExecuteStoredProc("WBB.PKG_FBB_REGISTER_PAYMENT.PROC_INS_DEDUCTION_LOG",
                    out paramOut,
                    new
                    {
                        //in
                        p_action = command.p_action.ToSafeString(),
                        p_user_name = command.p_user_name.ToSafeString(),
                        p_transaction_id = command.p_transaction_id.ToSafeString(),
                        p_mobile_no = command.p_mobile_no.ToSafeString(),
                        p_service_name = command.p_service_name.ToSafeString(),
                        p_endpoint = command.p_endpoint.ToSafeString(),
                        p_pm_tux_code = command.p_pm_tux_code.ToSafeString(),
                        p_pm_receipt_num = command.p_pm_receipt_num.ToSafeString(),
                        p_enq_status = command.p_enq_status.ToSafeString(),
                        p_enq_status_code = command.p_enq_status_code.ToSafeString(),
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
                command.ret_message = "Error SavePaymentLogCommandHandler " + ex.Message;
            }
        }

        public static void ReceiptLog(IEntityRepository<string> _objService, SaveDeductionReceiptLogCommand command)
        {
            try
            {
                var p_transaction_id = new OracleParameter();
                p_transaction_id.OracleDbType = OracleDbType.Varchar2;
                p_transaction_id.ParameterName = "p_transaction_id";
                p_transaction_id.Direction = ParameterDirection.Input;
                p_transaction_id.Value = command.p_transaction_id;

                var p_pm_receipt_id = new OracleParameter();
                p_pm_receipt_id.OracleDbType = OracleDbType.Decimal;
                p_pm_receipt_id.ParameterName = "p_pm_receipt_id";
                p_pm_receipt_id.Direction = ParameterDirection.Input;
                p_pm_receipt_id.Value = command.p_pm_receipt_id;

                var p_pm_receipt_num = new OracleParameter();
                p_pm_receipt_num.OracleDbType = OracleDbType.Varchar2;
                p_pm_receipt_num.ParameterName = "p_pm_receipt_num";
                p_pm_receipt_num.Direction = ParameterDirection.Input;
                p_pm_receipt_num.Value = command.p_pm_receipt_num;

                var p_pm_billing_acc_num = new OracleParameter();
                p_pm_billing_acc_num.OracleDbType = OracleDbType.Varchar2;
                p_pm_billing_acc_num.ParameterName = "p_pm_billing_acc_num";
                p_pm_billing_acc_num.Direction = ParameterDirection.Input;
                p_pm_billing_acc_num.Value = command.p_pm_billing_acc_num;

                var p_pm_receipt_tot_mny = new OracleParameter();
                p_pm_receipt_tot_mny.OracleDbType = OracleDbType.Decimal;
                p_pm_receipt_tot_mny.ParameterName = "p_pm_receipt_tot_mny";
                p_pm_receipt_tot_mny.Direction = ParameterDirection.Input;
                p_pm_receipt_tot_mny.Value = command.p_pm_receipt_tot_mny;

                var p_pm_tax_mny = new OracleParameter();
                p_pm_tax_mny.OracleDbType = OracleDbType.Decimal;
                p_pm_tax_mny.ParameterName = "p_pm_tax_mny";
                p_pm_tax_mny.Direction = ParameterDirection.Input;
                p_pm_tax_mny.Value = command.p_pm_tax_mny;

                var p_user_name = new OracleParameter();
                p_user_name.OracleDbType = OracleDbType.Varchar2;
                p_user_name.ParameterName = "p_user_name";
                p_user_name.Direction = ParameterDirection.Input;
                p_user_name.Value = command.p_user_name;

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

                var executeResult = _objService.ExecuteStoredProc("WBB.PKG_FBB_REGISTER_PAYMENT.PROC_INS_DEDUCTION_RECEIPT_LOG",
                    out paramOut,
                    new
                    {
                        //in
                        p_transaction_id = p_transaction_id,
                        p_pm_receipt_id = p_pm_receipt_id,
                        p_pm_receipt_num = p_pm_receipt_num,
                        p_pm_billing_acc_num = p_pm_billing_acc_num,
                        p_pm_receipt_tot_mny = p_pm_receipt_tot_mny,
                        p_pm_tax_mny = p_pm_tax_mny,
                        p_user_name = p_user_name,
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
                command.ret_message = "Error SavePaymentLogCommandHandler " + ex.Message;
            }
        }
    }
}
