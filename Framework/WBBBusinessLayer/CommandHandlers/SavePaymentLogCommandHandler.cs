using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBBusinessLayer.QueryHandlers;
using WBBContract;
using WBBContract.Commands;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;

namespace WBBBusinessLayer.CommandHandlers
{
    public class SavePaymentLogCommandHandler : ICommandHandler<SavePaymentLogCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<string> _objService;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;

        public SavePaymentLogCommandHandler(ILogger logger,
            IEntityRepository<string> objService,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IWBBUnitOfWork uow)
        {
            _logger = logger;
            _objService = objService;
            _intfLog = intfLog;
            _uow = uow;
        }

        public void Handle(SavePaymentLogCommand command)
        {

            InterfaceLogCommand log = null;
            try
            {
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, command, command.Transaction_Id,
                    "SavePaymentLogCommandHandler", "SavePaymentLogCommandHandler", command.p_payment_order_id,
                    "FBB|" + command.FullUrl, "WEB");

                var return_code = new OracleParameter();
                return_code.OracleDbType = OracleDbType.Varchar2;
                return_code.ParameterName = "ret_code";
                return_code.Size = 2000;
                return_code.Direction = ParameterDirection.Output;

                var return_message = new OracleParameter();
                return_message.OracleDbType = OracleDbType.Varchar2;
                return_message.ParameterName = "ret_message";
                return_message.Size = 2000;
                return_message.Direction = ParameterDirection.Output;

                var outp = new List<object>();
                var paramOut = outp.ToArray();

                var executeResult = _objService.ExecuteStoredProc("WBB.PKG_FBB_REGISTER_PAYMENT.PROC_INS_PAYMENT_LOG",
                    out paramOut,
                    new
                    {
                        //in 
                        p_action = command.p_action.ToSafeString(),
                        p_payment_order_id = command.p_payment_order_id.ToSafeString(),
                        p_process_name = command.p_process_name.ToSafeString(),
                        p_endpoint = command.p_endpoint.ToSafeString(),
                        p_req_project_code = command.p_req_project_code.ToSafeString(),
                        p_req_command = command.p_req_command.ToSafeString(),
                        p_req_sid = command.p_req_sid.ToSafeString(),
                        p_req_redirect_url = command.p_req_redirect_url.ToSafeString(),
                        p_req_merchant_id = command.p_req_merchant_id.ToSafeString(),
                        p_req_order_id = command.p_req_order_id.ToSafeString(),
                        p_req_currency = command.p_req_currency.ToSafeString(),
                        p_req_purchase_amt = command.p_req_purchase_amt.ToSafeString(),
                        p_req_payment_method = command.p_req_payment_method.ToSafeString(),
                        p_req_product_desc = command.p_req_product_desc.ToSafeString(),
                        p_req_ref1 = command.p_req_ref1.ToSafeString(),
                        p_req_ref2 = command.p_req_ref2.ToSafeString(),
                        p_req_ref3 = command.p_req_ref3.ToSafeString(),
                        p_req_ref4 = command.p_req_ref4.ToSafeString(),
                        p_req_ref5 = command.p_req_ref5.ToSafeString(),
                        p_req_integrity_str = command.p_req_integrity_str.ToSafeString(),
                        p_req_sms_flag = command.p_req_sms_flag.ToSafeString(),
                        p_req_sms_mobile = command.p_req_sms_mobile.ToSafeString(),
                        p_req_mobile_no = command.p_req_mobile_no.ToSafeString(),
                        p_req_token_key = command.p_req_token_key.ToSafeString(),
                        p_req_order_expire = command.p_req_order_expire.ToSafeString(),
                        p_resp_status = command.p_resp_status.ToSafeString(),
                        p_resp_resp_code = command.p_resp_resp_code.ToSafeString(),
                        p_resp_resp_desc = command.p_resp_resp_desc.ToSafeString(),
                        p_resp_sale_id = command.p_resp_sale_id.ToSafeString(),
                        p_resp_endpoint_url = command.p_resp_endpoint_url.ToSafeString(),
                        p_resp_detail1 = command.p_resp_detail1.ToSafeString(),
                        p_resp_detail2 = command.p_resp_detail2.ToSafeString(),
                        p_resp_detail3 = command.p_resp_detail3.ToSafeString(),
                        p_post_status = command.p_post_status.ToSafeString(),
                        p_post_resp_code = command.p_post_resp_code.ToSafeString(),
                        p_post_resp_desc = command.p_post_resp_desc.ToSafeString(),
                        p_post_tran_id = command.p_post_tran_id.ToSafeString(),
                        p_post_sale_id = command.p_post_sale_id.ToSafeString(),
                        p_post_order_id = command.p_post_order_id.ToSafeString(),
                        p_post_currency = command.p_post_currency.ToSafeString(),
                        p_post_exchange_rate = command.p_post_exchange_rate.ToSafeString(),
                        p_post_purchase_amt = command.p_post_purchase_amt.ToSafeString(),
                        p_post_amount = command.p_post_amount.ToSafeString(),
                        p_post_inc_customer_fee = command.p_post_inc_customer_fee.ToSafeString(),
                        p_post_exc_customer_fee = command.p_post_exc_customer_fee.ToSafeString(),
                        p_post_payment_status = command.p_post_payment_status.ToSafeString(),
                        p_post_payment_code = command.p_post_payment_code.ToSafeString(),
                        p_post_order_expire_date = command.p_post_order_expire_date.ToSafeString(),
                        //18.10 : QR Code
                        p_req_app_id = command.p_req_app_id.ToSafeString(),
                        p_req_app_secret = command.p_req_app_secret.ToSafeString(),
                        p_req_channel = command.p_req_channel.ToSafeString(),
                        p_req_qr_type = command.p_req_qr_type.ToSafeString(),
                        p_req_terminal_id = command.p_req_terminal_id.ToSafeString(),
                        p_req_service_id = command.p_req_service_id.ToSafeString(),
                        p_req_location_name = command.p_req_location_name.ToSafeString(),
                        p_req_tran_id = command.p_req_tran_id.ToSafeString(),

                        p_resp_qr_format = command.p_resp_qr_format.ToSafeString(),
                        p_resp_qr_code_str = command.p_resp_qr_code_str.ToSafeString(),
                        p_resp_qr_code_validity = command.p_resp_qr_code_validity.ToSafeString(),
                        p_resp_reference = command.p_resp_reference.ToSafeString(),
                        p_resp_tran_dtm = command.p_resp_tran_dtm.ToSafeString(),
                        p_resp_tran_id = command.p_resp_tran_id.ToSafeString(),
                        p_resp_service_id = command.p_resp_service_id.ToSafeString(),
                        p_resp_terminal_id = command.p_resp_terminal_id.ToSafeString(),
                        p_resp_location_name = command.p_resp_location_name.ToSafeString(),
                        p_resp_amount = command.p_resp_amount.ToSafeString(),
                        p_resp_sof = command.p_resp_sof.ToSafeString(),
                        p_resp_qr_type = command.p_resp_qr_type.ToSafeString(),
                        p_resp_refund_dt = command.p_resp_refund_dt.ToSafeString(),
                        p_resp_dispute_id = command.p_resp_dispute_id.ToSafeString(),
                        p_resp_dispute_status = command.p_resp_dispute_status.ToSafeString(),
                        p_resp_dispute_reason_id = command.p_resp_dispute_reason_id.ToSafeString(),
                        p_resp_ref1 = command.p_resp_ref1.ToSafeString(),
                        p_resp_ref2 = command.p_resp_ref2.ToSafeString(),
                        p_resp_ref3 = command.p_resp_ref3.ToSafeString(),
                        p_resp_ref4 = command.p_resp_ref4.ToSafeString(),
                        p_resp_ref5 = command.p_resp_ref5.ToSafeString(),

                        p_post_tran_dtm = command.p_post_tran_dtm.ToSafeString(),
                        p_post_service_id = command.p_post_service_id.ToSafeString(),
                        p_post_terminal_id = command.p_post_terminal_id.ToSafeString(),
                        p_post_location_name = command.p_post_location_name.ToSafeString(),
                        p_post_sof = command.p_post_sof.ToSafeString(),
                        p_post_qr_type = command.p_post_qr_type.ToSafeString(),
                        p_post_ref1 = command.p_post_ref1.ToSafeString(),
                        p_post_ref2 = command.p_post_ref2.ToSafeString(),
                        p_post_ref3 = command.p_post_ref3.ToSafeString(),
                        p_post_ref4 = command.p_post_ref4.ToSafeString(),
                        p_post_ref5 = command.p_post_ref5.ToSafeString(),
                        /// Out
                        ret_code = return_code,
                        ret_message = return_message

                    });
                command.Return_Code = return_code.Value.ToSafeString();
                command.Return_Desc = return_message.Value.ToSafeString();

                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, return_code, log, "Success", "", "");

            }
            catch (Exception ex)
            {
                _logger.Info(ex.Message);

                if (null != log)
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex, log, "Failed", ex.GetErrorMessage(),
                        "");
                }

                command.Return_Code = "-1";
                command.Return_Desc = "Error SavePaymentLogCommandHandler " + ex.Message;
            }
        }
    }

    public class UpdatOrderPaymentStatusCommandHandler : ICommandHandler<UpdatOrderPaymentStatusCommand>
    {
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_CFG_LOV> _lovData;

        public UpdatOrderPaymentStatusCommandHandler(IWBBUnitOfWork uow, IEntityRepository<FBB_CFG_LOV> lovData)
        {
            _uow = uow;
            _lovData = lovData;
        }

        public void Handle(UpdatOrderPaymentStatusCommand command)
        {
            var lovDataInfo = _lovData.Get(t => t.LOV_NAME == "QUERY_ORDER_PAYMENT_STATUS_FLAG").FirstOrDefault();

            lovDataInfo.LOV_VAL1 = command.Status;
            lovDataInfo.UPDATED_DATE = DateTime.Now;
            lovDataInfo.UPDATED_BY = "BATCH";

            _lovData.Update(lovDataInfo);
            _uow.Persist();
        }

    }
}
