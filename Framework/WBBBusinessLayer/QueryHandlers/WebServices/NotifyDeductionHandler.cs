using System;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Commands.WebServices;
using WBBContract.Queries.WebServices;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.WebServiceModels;
using WBBEntity.PanelModels.WebServices;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class NotifyDeductionHandler : IQueryHandler<NotifyDeductionQuery, NotifyDeductionModel>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IQueryHandler<ConfirmPaymentDeductionQuery, ConfirmPaymentDeductionModel> _confirmPaymentDeduction;
        private readonly IQueryHandler<GetRegisterPendingDeductionQuery, GetRegisterPendingDeductionModel> _getRegisterPendingDeduction;
        private readonly IQueryHandler<CheckOrderPendingDeductionQuery, CheckOrderPendingDeductionModel> _checkOrderPendingDeduction;
        private readonly IEntityRepository<string> _objService;

        public NotifyDeductionHandler(ILogger logger, IWBBUnitOfWork uow, IEntityRepository<FBB_INTERFACE_LOG> intfLog, IQueryHandler<ConfirmPaymentDeductionQuery, ConfirmPaymentDeductionModel> confirmPaymentDeduction, IQueryHandler<GetRegisterPendingDeductionQuery, GetRegisterPendingDeductionModel> getRegisterPendingDeduction, IQueryHandler<CheckOrderPendingDeductionQuery, CheckOrderPendingDeductionModel> checkOrderPendingDeduction, IEntityRepository<string> objService)
        {
            _logger = logger;
            _uow = uow;
            _intfLog = intfLog;
            _confirmPaymentDeduction = confirmPaymentDeduction;
            _getRegisterPendingDeduction = getRegisterPendingDeduction;
            _checkOrderPendingDeduction = checkOrderPendingDeduction;
            _objService = objService;
        }

        public NotifyDeductionModel Handle(NotifyDeductionQuery query)
        {
            InterfaceLogCommand log = null;
            var result = new NotifyDeductionModel();
            try
            {
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.TransactionId, "NotifyDeduction", "NotifyDeductionHandler", "", "FBB|" + query.FullUrl, "");
                var internetNo = string.Empty;
                var txn_id = query.DataResult.txn_id.ToSafeString();
                var fullUrl = query.FullUrl.ToSafeString();

                var channel = query.DataResult.channel_type.ToSafeString();
                var status = query.DataResult.status.ToSafeString();
                var status_code = query.DataResult.status_code.ToSafeString();
                var status_message = query.DataResult.status_message.ToSafeString();

                //TODO: 3.check order create หรือยัง
                if (CheckOrderDeduction(query))
                {
                    result.RESULT_CODE = "-1";
                    result.RESULT_DESC = "The order done.";
                    return result;
                }

                //Get Internet No
                internetNo = GetInternetNoByRegisPendingDeduction(fullUrl, txn_id);

                //TODO: 4.update payment status ว่าจ่ายหรือยัง
                var command = new SaveDeductionLogCommand()
                {
                    p_action = "New",
                    p_service_name = "WebHook Notify",
                    p_user_name = "WebHook Notify",
                    p_transaction_id = query.TransactionId,
                    p_order_transaction_id = query.OrderTransactionId,
                    p_enq_status = status,
                    p_enq_status_code = status_code,
                };
                InterfaceLogServiceHelper.DeductionLog(_objService, command, query);

                //TODO: 5.ช่วงตัดหนี้ตาม job(เอามารวม handler)
                //TODO: 6.update deduction status ว่าจ่ายหรือยัง
                var cmPmDeducQuery = new ConfirmPaymentDeductionQuery
                {
                    FullUrl = query.FullUrl,
                    txn_id = query.DataResult.txn_id.ToSafeString(),
                    non_mobile_no = internetNo.ToSafeString(),
                    order_transaction_id = query.OrderTransactionId.ToSafeString(),
                };
                var cmPmDeducResult = _confirmPaymentDeduction.Handle(cmPmDeducQuery);

                result.RESULT_CODE = "0";
                result.RESULT_DESC = "Success";
                return result;
            }
            catch (Exception ex)
            {
                result.RESULT_CODE = "-1";
                result.RESULT_DESC = ex.GetErrorMessage();
                _logger.Info("Error call NotifyDeductionHandler : " + ex.GetErrorMessage());
                return result;
            }
            finally
            {
                var resultLogStatus = (result ?? new NotifyDeductionModel()).RESULT_CODE == "0" ? "Success" : "Failed";
                var resultLogDesc = (result ?? new NotifyDeductionModel()).RESULT_CODE == "0" ? "" : result.RESULT_DESC;
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, resultLogStatus, resultLogDesc, "");
            }
        }

        private string GetInternetNoByRegisPendingDeduction(string fullUrl, string TransactionId)
        {
            var internetNo = string.Empty;
            var query = new GetRegisterPendingDeductionQuery()
            {
                Url = fullUrl,
                transaction_id = TransactionId,
            };
            var result = _getRegisterPendingDeduction.Handle(query);
            if (result != null && result.Data != null)
            {
                internetNo = result.Data.NON_MOBILE_NO.ToSafeString();
            }
            return internetNo;
        }


        private bool CheckOrderDeduction(NotifyDeductionQuery query)
        {
            var checkOrderDeduction = _checkOrderPendingDeduction.Handle(new CheckOrderPendingDeductionQuery()
            {
                OrderTransactionId = query.OrderTransactionId,
                TransactionId = query.TransactionId,
                FullUrl = query.FullUrl
            });
            if (checkOrderDeduction != null && checkOrderDeduction.OrderDeduction == "0")
            {
                return false;
            }
            else
            {
                return true;
            }
        }


    }
}
