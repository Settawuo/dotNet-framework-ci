using System;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.WebServices;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Models;
using WBBEntity.PanelModels.WebServiceModels;
using WBBEntity.PanelModels.WebServices;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class QueryOrderPendingDeductionHandler : IQueryHandler<QueryOrderPendingDeductionQuery, QueryOrderPendingDeductionModel>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IQueryHandler<GetPaymentEnquiryQuery, GetPaymentEnquiryModel> _getPaymentEnquiry;
        private readonly IQueryHandler<GetPaymentEnquiryToSuperDuperQuery, GetPaymentEnquiryToSuperDuperModel> _getPaymentEnquiryToSuperDuper;
        private readonly IQueryHandler<ConfirmPaymentDeductionQuery, ConfirmPaymentDeductionModel> _confirmPaymentDeduction;

        public QueryOrderPendingDeductionHandler(ILogger logger, IWBBUnitOfWork uow, IEntityRepository<FBB_INTERFACE_LOG> intfLog, IQueryHandler<GetPaymentEnquiryQuery, GetPaymentEnquiryModel> getPaymentEnquiry, IQueryHandler<GetPaymentEnquiryToSuperDuperQuery, GetPaymentEnquiryToSuperDuperModel> getPaymentEnquiryToSuperDuper, IQueryHandler<ConfirmPaymentDeductionQuery, ConfirmPaymentDeductionModel> confirmPaymentDeduction)
        {
            _logger = logger;
            _uow = uow;
            _intfLog = intfLog;
            _getPaymentEnquiry = getPaymentEnquiry;
            _getPaymentEnquiryToSuperDuper = getPaymentEnquiryToSuperDuper;
            _confirmPaymentDeduction = confirmPaymentDeduction;
        }

        public QueryOrderPendingDeductionModel Handle(QueryOrderPendingDeductionQuery query)
        {
            InterfaceLogCommand log = null;
            var resultMessage = "";
            var resultLog = "Success";
            var result = new QueryOrderPendingDeductionModel();

            try
            {
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, "", "QueryOrderPendingDeduction", "QueryOrderPendingDeductionHandler", "", "FBB|" + query.FullUrl, "");

                _logger.Info("GetPaymentEnquiry");

                string tranTime = DateTime.Now.ToString("ddMMyyyyHHmmss");

                GetPaymentEnquiryModel GetPaymentEnquiryResults = GetPaymentEnquiry(tranTime);
                if (GetPaymentEnquiryResults != null && GetPaymentEnquiryResults.OrderDeductEnquiryDatas != null && GetPaymentEnquiryResults.OrderDeductEnquiryDatas.Count > 0)
                {
                    _logger.Info("GetPaymentEnquiryResults HaveData.");

                    foreach (var PaymentEnquiryData in GetPaymentEnquiryResults.OrderDeductEnquiryDatas)
                    {
                        _logger.Info("PaymentEnquiry txn_id : " + PaymentEnquiryData.txn_id);

                        string Nonce = Guid.NewGuid().ToString();

                        PaymentEnquiryToSuperDuperBody body = new PaymentEnquiryToSuperDuperBody
                        {
                            txn_id = PaymentEnquiryData.txn_id
                        };

                        GetPaymentEnquiryToSuperDuperQuery getPaymentEnquiryToSuperDuperQuery = new GetPaymentEnquiryToSuperDuperQuery()
                        {
                            p_transaction_id = PaymentEnquiryData.txn_id,
                            p_mobile_no = PaymentEnquiryData.non_mobile_no,
                            User = !string.IsNullOrEmpty(query.FullUrl) ? query.FullUrl : "FBBQueryOrderPendingDeduction",
                            Url = PaymentEnquiryData.endpoint,
                            Secret = PaymentEnquiryData.channel_secret,
                            ContentType = "application/json",
                            MerchantID = PaymentEnquiryData.merchant_id,
                            Signature = "",
                            Nonce = Nonce,
                            Body = body
                        };
                        GetPaymentEnquiryToSuperDuperModel GetPaymentEnquiryToSuperDuperResults = GetPaymentEnquiryToSuperDuper(getPaymentEnquiryToSuperDuperQuery);

                        if (GetPaymentEnquiryToSuperDuperResults != null && GetPaymentEnquiryToSuperDuperResults.status.ToUpper() == "SUCCESS")
                        {
                            _logger.Info("GetPaymentEnquiryToSuperDuper Success.");

                            ConfirmPaymentDeductionQuery confirmPaymentDeductionQuery = new ConfirmPaymentDeductionQuery()
                            {
                                txn_id = PaymentEnquiryData.txn_id,
                                non_mobile_no = PaymentEnquiryData.non_mobile_no,
                                FullUrl = query.FullUrl,
                                update_by = query.FullUrl
                            };

                            var resultConfirmPaymentDeduction = _confirmPaymentDeduction.Handle(confirmPaymentDeductionQuery);
                        }
                        else
                        {
                            _logger.Info("GetPaymentEnquiryToSuperDuper FAIL.");
                        }
                    }
                }
                else
                {
                    _logger.Info("GetPaymentEnquiryResults NoData.");
                }
            }
            catch (Exception ex)
            {
                resultMessage = ex.Message;
                resultLog = "ERROR";
            }
            finally
            {
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, resultLog, resultMessage, "");
            }


            return result;
        }

        private GetPaymentEnquiryModel GetPaymentEnquiry(string tranTime)
        {
            GetPaymentEnquiryQuery query = new GetPaymentEnquiryQuery()
            {
                tranTime = tranTime
            };
            GetPaymentEnquiryModel Results = new GetPaymentEnquiryModel();
            try
            {
                Results = _getPaymentEnquiry.Handle(query);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }
            return Results;
        }

        private GetPaymentEnquiryToSuperDuperModel GetPaymentEnquiryToSuperDuper(GetPaymentEnquiryToSuperDuperQuery query)
        {
            GetPaymentEnquiryToSuperDuperModel Results = new GetPaymentEnquiryToSuperDuperModel();
            try
            {
                Results = _getPaymentEnquiryToSuperDuper.Handle(query);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }
            return Results;
        }
    }


}
