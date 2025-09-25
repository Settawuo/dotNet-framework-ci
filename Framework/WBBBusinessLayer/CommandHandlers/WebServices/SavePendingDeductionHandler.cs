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
    public class SavePendingDeductionHandler : ICommandHandler<SavePendingDeductionCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<string> _objService;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;

        public SavePendingDeductionHandler(ILogger logger,
            IEntityRepository<string> objService,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IWBBUnitOfWork uow)
        {
            _logger = logger;
            _objService = objService;
            _intfLog = intfLog;
            _uow = uow;
        }

        public void Handle(SavePendingDeductionCommand command)
        {

            InterfaceLogCommand log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, command, command.p_transaction_id,
                    "SavePendingDeduction", "SavePendingDeductionHandler", null,
                    "FBB|" + command.p_channel, "WEB");
            try
            {
                if (string.IsNullOrEmpty(command.p_order_transaction_id))
                {
                    command.p_order_transaction_id = command.p_payment_method_id == "147"
                        ? "P" + DateTime.Now.ToString("yyyyMMddHHmmssfffff")
                        : "Q" + DateTime.Now.ToString("yyyyMMddHHmmssfffff");
                }

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

                var executeResult = _objService.ExecuteStoredProc("WBB.PKG_FBB_REGISTER_PAYMENT.PROC_INS_PENDING_DEDUCTION",
                    out paramOut,
                    new
                    {
                        //in
                        p_transaction_id = command.p_transaction_id.ToSafeString(),
                        p_mobile_no = command.p_mobile_no.ToSafeString(),
                        p_ba_no = command.p_ba_no.ToSafeString(),
                        p_paid_amt = command.p_paid_amt.ToSafeString(),
                        p_channel = command.p_channel.ToSafeString(),
                        p_merchant_id = command.p_merchant_id.ToSafeString(),
                        p_payment_method_id = command.p_payment_method_id.ToSafeString(),
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
}
