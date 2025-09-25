using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using WBBContract;
using WBBContract.Commands;
using WBBData.Repository;
using WBBEntity.Extensions;

namespace WBBBusinessLayer.CommandHandlers
{
    public class PaymentLogCommandHandler : ICommandHandler<PaymentLogCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<PaymentLogCommand> _paymentLogRepository;


        public PaymentLogCommandHandler(ILogger logger,
            IEntityRepository<PaymentLogCommand> paymentLogRepository)
        {
            _logger = logger;
            _paymentLogRepository = paymentLogRepository;
        }

        public void Handle(PaymentLogCommand command)
        {
            try
            {
                _logger.Info("START_WBB.PKG_FBB_PAYMENT.PROC_LOGGING_PAYMENT");

                var retCode = new OracleParameter
                {
                    OracleDbType = OracleDbType.Varchar2,
                    Size = 2000,
                    Direction = ParameterDirection.Output
                };

                var retMessage = new OracleParameter
                {
                    OracleDbType = OracleDbType.Varchar2,
                    Size = 2000,
                    Direction = ParameterDirection.Output
                };

                var retPaymentId = new OracleParameter
                {
                    OracleDbType = OracleDbType.Decimal,
                    Direction = ParameterDirection.Output
                };


                object[] paramOut;
                _paymentLogRepository.ExecuteStoredProc("WBB.PKG_FBB_PAYMENT.PROC_LOGGING_PAYMENT",
                    out paramOut,
                    new
                    {
                        //// return 
                        p_return_code = retCode,
                        p_return_message = retMessage,
                        p_return_Payment_Id = retPaymentId,

                        iPaymentId = command.PaymentId,
                        iSessionId = command.SessionId,
                        iBrowser = command.Browser,
                        iCustInternetNum = command.CustInternetNum,
                        iCustIdCardType = command.CustIdCardType,
                        iCustIdCardNum = command.CustIdCardNum,
                        iDueDate = command.DueDate,
                        iRequestParamMode = command.RequestParamMode,
                        iRequestParamPaymentMethod = command.RequestParamPaymentMethod,
                        iRequestParamMerchantId = command.RequestParamMerchantId,
                        iRequestParamServiceId = command.RequestParamServiceId,
                        iRequestParamRef1 = command.RequestParamRef1,
                        iRequestParamRef2 = command.RequestParamRef2,
                        iRequestParamAmount = command.RequestParamAmount,
                        iRequestParamUsername = command.RequestParamUsername,
                        iRequestParamPassword = command.RequestParamPassword,
                        iRequestUrl = command.RequestUrl,
                        iRequestDatetime = command.RequestDatetime,
                        iResponseMoblieNo = command.ResponseMoblieNo,
                        iResponseAmount = command.ResponseAmount,
                        iResponseReceiptNum = command.ResponseReceiptNum,
                        iResponseRef1 = command.ResponseRef1,
                        iResponseTxid = command.ResponseTxid,
                        iResponseUrlForward = command.ResponseUrlForward,
                        iResponseSessRef1 = command.ResponseSessRef1,
                        iResponseCode = command.ResponseCode,
                        iResponseMsg = command.ResponseMsg,
                        iResponseBalance = command.ResponseBalance,
                        iResponseValidity = command.ResponseValidity,
                        iResponseRef2 = command.ResponseRef2,
                        iResponseUrl = command.ResponseUrl,
                        iStatus = command.Status,
                        iResponseDatetime = command.ResponseDatetime
                    });

                command.PaymentId = (retPaymentId.Value != null ? Convert.ToDecimal(retPaymentId.Value.ToString()) : Convert.ToDecimal("0"));
                command.ReturnCode = retCode.Value != null ? retCode.Value.ToSafeString() : "-1";
                command.ReturnDesc = retMessage.Value != null ? retMessage.Value.ToSafeString() : "Error";

                _logger.Info("END_PROC_SAVE_SESSION_LOGIN");
            }
            catch (Exception ex)
            {
                _logger.Info(ex.GetErrorMessage());
                command.ReturnCode = "-1";
                command.ReturnDesc = "Error call PaymentLog Command handles : " + ex.GetErrorMessage();
            }
        }
    }
}
