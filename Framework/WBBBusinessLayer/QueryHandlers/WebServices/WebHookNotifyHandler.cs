using System;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.WebServices;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.WebServiceModels;
using WBBEntity.PanelModels.WebServices;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class WebHookNotifyHandler : IQueryHandler<WebHookNotifyQuery, WebHookNotifyModel>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IQueryHandler<CheckNotiProductQuery, CheckNotiProductModel> _checkNotiProduct;
        private readonly IQueryHandler<NotifySuperDuperQuery, NotifySuperDuperModel> _notifySuperDuper;
        private readonly IQueryHandler<NotifyDeductionQuery, NotifyDeductionModel> _notifyDeduction;

        public WebHookNotifyHandler(ILogger logger, IWBBUnitOfWork uow, IEntityRepository<FBB_INTERFACE_LOG> intfLog, IQueryHandler<CheckNotiProductQuery, CheckNotiProductModel> checkNotiProduct, IQueryHandler<NotifySuperDuperQuery, NotifySuperDuperModel> notifySuperDuper, IQueryHandler<NotifyDeductionQuery, NotifyDeductionModel> notifyDeduction)
        {
            _logger = logger;
            _uow = uow;
            _intfLog = intfLog;
            _checkNotiProduct = checkNotiProduct;
            _notifySuperDuper = notifySuperDuper;
            _notifyDeduction = notifyDeduction;
        }

        public WebHookNotifyModel Handle(WebHookNotifyQuery query)
        {
            InterfaceLogCommand log = null;
            var result = new WebHookNotifyModel();
            try
            {
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.TransactionId, "WebHookNotify", "WebHookNotifyHandler", "", "FBB|" + query.FullUrl, "");

                var txn_id = query.DataResult.txn_id.ToSafeString();
                var order_transaction_id = query.DataResult.order_id.ToSafeString();

                var notiProduct = CheckNotiProduct(query.FullUrl, txn_id, order_transaction_id);
                if (notiProduct == "DEDUCTION")
                {
                    result = CallNotifyDeduction(query);
                }
                else if (notiProduct == "SUPER_DUPER")
                {
                    result = CallNotifySuperDuper(query);
                }
                else
                {
                    var msgNotFound = "WebHookNotifyHandler notiProduct is null, txn_id = " + txn_id;
                    result.RESULT_CODE = "-1";
                    result.RESULT_DESC = msgNotFound;
                    _logger.Info(msgNotFound);
                }
                return result;
            }
            catch (Exception ex)
            {
                result.RESULT_CODE = "-1";
                result.RESULT_DESC = ex.GetErrorMessage();
                _logger.Info("Error call WebHookNotifyHandler : " + ex.GetErrorMessage());
                return result;
            }
            finally
            {
                var resultLog = (result ?? new WebHookNotifyModel()).RESULT_CODE == "0" ? "Success" : "Failed";
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, resultLog, result.RESULT_DESC, "");
            }
        }

        private string CheckNotiProduct(string fullUrl, string txn_id, string order_id)
        {
            var result = string.Empty;
            var query = new CheckNotiProductQuery
            {
                FullUrl = fullUrl,
                transaction_id = txn_id,
                order_transaction_id = order_id,
            };
            var notiResult = _checkNotiProduct.Handle(query);
            if (notiResult != null)
            {
                result = notiResult.ret_product.ToSafeString();
            }
            return result;
        }

        private WebHookNotifyModel CallNotifyDeduction(WebHookNotifyQuery query)
        {
            var result = new WebHookNotifyModel();
            var queryDeduc = new NotifyDeductionQuery
            {
                FullUrl = query.FullUrl,
                OrderTransactionId = query.OrderTransactionId,
                TransactionId = query.TransactionId,
                DataResult = query.DataResult,
            };
            var resultDeduc = _notifyDeduction.Handle(queryDeduc);

            if (resultDeduc != null)
            {
                result.RESULT_CODE = resultDeduc.RESULT_CODE;
                result.RESULT_DESC = resultDeduc.RESULT_DESC;
            }
            return result;
        }

        private WebHookNotifyModel CallNotifySuperDuper(WebHookNotifyQuery query)
        {
            var result = new WebHookNotifyModel();
            var querySpdp = new NotifySuperDuperQuery
            {
                FullUrl = query.FullUrl,
                TransactionId = query.TransactionId,
                DataResult = query.DataResult
            };
            var resultSpdp = _notifySuperDuper.Handle(querySpdp);

            if (resultSpdp != null)
            {
                result.RESULT_CODE = resultSpdp.RESULT_CODE;
                result.RESULT_DESC = resultSpdp.RESULT_DESC;
            }
            return result;
        }
    }
}