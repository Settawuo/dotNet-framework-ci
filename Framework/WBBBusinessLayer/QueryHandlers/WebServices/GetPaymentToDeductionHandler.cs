using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Commands.WebServices;
using WBBContract.Queries.WebServices;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class GetPaymentToDeductionHandler : IQueryHandler<GetPaymentToDeductionQuery, GetPaymentToDeductionModel>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IQueryHandler<GetConfigReqPaymentQuery, GetConfigReqPaymentModel> _queryHandlerConfigEnquiry;
        private readonly ICommandHandler<SavePendingDeductionCommand> _savePendingDeduction;
        private readonly IEntityRepository<string> _objService;
        public GetPaymentToDeductionHandler(ILogger logger, IWBBUnitOfWork uow, IEntityRepository<FBB_INTERFACE_LOG> intfLog, IQueryHandler<GetConfigReqPaymentQuery, GetConfigReqPaymentModel> queryHandlerConfigEnquiry, ICommandHandler<SavePendingDeductionCommand> savePendingDeduction, IEntityRepository<string> objService)
        {
            _logger = logger;
            _uow = uow;
            _intfLog = intfLog;
            _queryHandlerConfigEnquiry = queryHandlerConfigEnquiry;
            _savePendingDeduction = savePendingDeduction;
            _objService = objService;
        }

        public GetPaymentToDeductionModel Handle(GetPaymentToDeductionQuery queryData)
        {
            InterfaceLogCommand log = null;
            var result = new GetPaymentToDeductionModel();
            string StatusCode = "Failed";
            string StatusMessage = "Failed";

            try
            {
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, queryData, queryData.InternetNo, "GetPaymentToDeduction", "GetPaymentToDeductionHandler", "", queryData.UpdateBy, "");

                //Get Config
                var configEnquiry = GetConfigEnquiry(queryData.ProductName, queryData.ServiceName);
                if (configEnquiry != null && configEnquiry.list_config_req_payment != null && configEnquiry.list_config_req_payment.Count > 0)
                {
                    var query = SetSuperDuperQuery(configEnquiry, queryData);
                    if (!string.IsNullOrEmpty(query.Url))
                    {
                        var p_order_id = query.Body.order_id;
                        var p_order_transaction_id = "P" + DateTime.Now.ToString("yyyyMMddHHmmssfffff");
                        query.Body.order_id = p_order_transaction_id;
                        if (query.Body.Eds != null)
                        {
                            query.Body.Eds.Eds_url_fail = SetPaymentToDeductionUrlFail(query.Body.Eds.Eds_url_fail, query.Body.order_id);
                        }

                        string BodyStr = JsonConvert.SerializeObject(query.Body);
                        query.BodyStr = BodyStr;

                        query.Signature = EncrypSuperDuperHelper.hmacsha256(query.Secret, query.Nonce, query.Body);

                        //Insert fbb_register_deduction_log
                        var regisDDlogInsert = SaveRegisterDeductionLog("New", query, result);

                        var client = new RestClient(query.Url);
                        var request = new RestRequest();
                        request.Method = Method.POST;
                        request.AddHeader("Content-Type", query.ContentType);
                        request.AddHeader("X-sdpg-merchant-id", query.MerchantID);
                        request.AddHeader("X-sdpg-signature", query.Signature);
                        request.AddHeader("X-sdpg-nonce", query.Nonce);
                        request.AddParameter("application/json", BodyStr, ParameterType.RequestBody);

                        // execute the request
                        ServicePointManager.Expect100Continue = true;
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                        ServicePointManager.ServerCertificateValidationCallback =
                            (s, certificate, chain, sslPolicyErrors) => true;

                        var response = client.Execute(request);

                        var content = response.Content; // raw content as string

                        if (HttpStatusCode.OK.Equals(response.StatusCode))
                        {
                            result = JsonConvert.DeserializeObject<GetPaymentToDeductionModel>(response.Content) ?? new GetPaymentToDeductionModel();
                            if (result != null)
                            {
                                if (result != null)
                                {
                                    StatusCode = "";
                                    StatusMessage = "";
                                }
                            }
                            else
                            {
                                StatusCode = "Failed";
                                StatusMessage = (!string.IsNullOrEmpty(response.ErrorMessage)
                                    ? response.ErrorMessage
                                    : response.Content).ToSafeString();
                            }
                        }
                        else
                        {
                            StatusCode = "Failed";
                            StatusMessage = (!string.IsNullOrEmpty(response.ErrorMessage)
                                    ? response.ErrorMessage
                                    : response.Content).ToSafeString();
                        }

                        //Insert fbb_register_pending_deduction
                        SaveRegisterPendingDeduction(queryData, query, result);

                        //Update fbb_register_deduction_log
                        var regisDDlogUpdate = SaveRegisterDeductionLog("Update", query, result);

                        StatusCode = string.IsNullOrEmpty(StatusCode) ? "Success" : "Failed";
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                StatusCode = "Failed";
                StatusMessage = ex.GetBaseException().ToString();
                _logger.Info("GetPaymentToDeductionHandler Exception " + ex.GetErrorMessage());
            }
            finally
            {
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, StatusCode, StatusMessage, "");
            }

            return result;
        }

        private GetConfigReqPaymentModel GetConfigEnquiry(string productName, string serviceName)
        {
            var resultConfig = _queryHandlerConfigEnquiry.Handle(new GetConfigReqPaymentQuery()
            {
                p_product_name = productName, //"ENQUIRY","FIBRE_PAYMENT",
                p_service_name = serviceName, //"Enquiry","Payment Order"
            });
            return resultConfig;
        }

        private GetPaymentToSuperDuperQuery SetSuperDuperQuery(GetConfigReqPaymentModel getConfigReqPaymentData, GetPaymentToDeductionQuery query)
        {
            //"amount": 100.50,
            //"currency": "THB",
            string ServiceName = query.ServiceName;
            string NonMobileNO = query.InternetNo;

            string Nonce = Guid.NewGuid().ToString();

            string endpoint = "";
            string channel_secret = "";
            string Content_Type = "";
            string X_sdpg_merchant_id = "";
            string order_id = "";
            string product_name = "";
            string service_id = "";
            string channel_type = "";
            string amount = "";
            string currency = "";
            string form_type = "";
            string is_remember = "";
            string ref_1 = "";
            string ref_2 = "";
            string payment_method_id = "";
            string cust_id = NonMobileNO;

            bool tmp_is_remember = false;

            List<ConfigReqPaymentData> configReqPaymentDatas = getConfigReqPaymentData.list_config_req_payment;
            // set config
            foreach (var item in configReqPaymentDatas)
            {

                if (item.attr_name == "endpoint")
                {
                    endpoint = item.attr_value.ToSafeString();
                }
                else if (item.attr_name == "channel_secret")
                {
                    channel_secret = item.attr_value.ToSafeString();
                }
                else if (item.attr_name == "Content-Type")
                {
                    Content_Type = item.attr_value.ToSafeString();
                }
                else if (item.attr_name == "X-sdpg-merchant-id")
                {
                    X_sdpg_merchant_id = item.attr_value.ToSafeString();
                }
                else if (item.attr_name == "order_id")
                {
                    order_id = item.attr_value.ToSafeString();
                }
                else if (item.attr_name == "product_name")
                {
                    product_name = item.attr_value.ToSafeString();
                }
                else if (item.attr_name == "service_id")
                {
                    service_id = item.attr_value.ToSafeString();
                }
                else if (item.attr_name == "channel_type")
                {
                    channel_type = item.attr_value.ToSafeString();
                }
                else if (item.attr_name == "amount")
                {
                    amount = item.attr_value.ToSafeString();
                }
                else if (item.attr_name == "currency")
                {
                    currency = item.attr_value.ToSafeString();
                }
                else if (item.attr_name == "form_type")
                {
                    form_type = item.attr_value.ToSafeString();
                }
                else if (item.attr_name == "is_remember")
                {
                    is_remember = item.attr_value.ToSafeString();
                }
                else if (item.attr_name == "ref_1")
                {
                    ref_1 = item.attr_value.ToSafeString();
                }
                else if (item.attr_name == "ref_2")
                {
                    ref_2 = item.attr_value.ToSafeString();
                }
                else if (item.attr_name == "payment_method_id")
                {
                    payment_method_id = item.attr_value.ToSafeString();
                }

            }

            //20.10 set ref1, ref12 (Ref1 = BA, Ref2 = Internet No)
            ref_1 = query.BillingNo.ToSafeString();
            ref_2 = query.InternetNo.ToSafeString();

            if (is_remember == "true")
            {
                tmp_is_remember = true;
            }

            string bank_code = "";
            string company_account_no = "";
            string company_account_name = "";
            string transaction_code = "";
            string billing_system = "";
            string merchant_type = "";
            string billing_account = "";
            string master_mobile_no = "";
            string batch_no = "";

            List<ConfigReqPaymentData> ReqPaymentMetadat = getConfigReqPaymentData.list_req_payment_metadata;
            // set config
            foreach (var item in ReqPaymentMetadat)
            {
                if (item.attr_name == "bank_code")
                {
                    bank_code = item.attr_value.ToSafeString();
                }
                else if (item.attr_name == "company_account_no")
                {
                    company_account_no = item.attr_value.ToSafeString();
                }
                else if (item.attr_name == "company_account_name")
                {
                    company_account_name = item.attr_value.ToSafeString();
                }
                else if (item.attr_name == "transaction_code")
                {
                    transaction_code = item.attr_value.ToSafeString();
                }
                else if (item.attr_name == "billing_system")
                {
                    billing_system = item.attr_value.ToSafeString();
                }
                else if (item.attr_name == "merchant_type")
                {
                    merchant_type = item.attr_value.ToSafeString();
                }
                else if (item.attr_name == "billing_account")
                {
                    billing_account = item.attr_value.ToSafeString();
                }
                else if (item.attr_name == "master_mobile_no")
                {
                    master_mobile_no = item.attr_value.ToSafeString();
                }
                else if (item.attr_name == "batch_no")
                {
                    batch_no = item.attr_value.ToSafeString();
                }
            }

            bool TMP_Eds_required = false;
            string Eds_required = "";
            string Eds_url_success = "";
            string Eds_url_fail = "";

            List<ConfigReqPaymentData> ReqPayment3ds = getConfigReqPaymentData.list_req_payment_3ds;
            // set config
            foreach (var item in ReqPayment3ds)
            {
                if (item.attr_name == "3ds_required")
                {
                    Eds_required = item.attr_value.ToSafeString();
                }
                else if (item.attr_name == "3ds_url_success")
                {
                    Eds_url_success = item.attr_value.ToSafeString();
                }
                else if (item.attr_name == "3ds_url_fail")
                {
                    Eds_url_fail = item.attr_value.ToSafeString();
                }
            }
            if (Eds_required == "true")
            {
                TMP_Eds_required = true;
            }

            amount = amount.ToSafeDouble() > 0 ? amount : query.Amount;

            CustomerMobileNo[] customer_mobile_no = new CustomerMobileNo[1];
            customer_mobile_no[0] = new CustomerMobileNo()
            {
                billing_account = query.BillingNo,
                mobile_no = NonMobileNO,
                amount = amount
            };

            MetaData metaData = new MetaData
            {
                bank_code = "",
                company_account_no = company_account_no,
                company_account_name = company_account_name,
                service_id = service_id,
                transaction_code = transaction_code,
                billing_system = billing_system,
                merchant_type = merchant_type,
                master_mobile_no = query.InternetNo,
                customer_mobile_no = customer_mobile_no,
                batch_no = batch_no
            };

            EdsData edsData = new EdsData
            {
                Eds_required = TMP_Eds_required,
                Eds_url_success = Eds_url_success,
                Eds_url_fail = Eds_url_fail
            };

            PaymentToSuperDuperBody body = new PaymentToSuperDuperBody
            {
                order_id = order_id,
                product_name = product_name,
                service_id = service_id,
                channel_type = channel_type,
                cust_id = cust_id,
                amount = amount,
                currency = currency,
                ref_1 = ref_1,
                ref_2 = ref_2,
                ref_3 = "",
                ref_4 = "",
                ref_5 = "",
                form_type = form_type,
                is_remember = tmp_is_remember,
                metadata = metaData,
                Eds = edsData
            };
            GetPaymentToSuperDuperQuery getPaymentInquiryToSuperDuperQuery = new GetPaymentToSuperDuperQuery
            {
                Url = endpoint,
                ProductName = product_name,
                ServiceName = ServiceName,
                p_mobile_no = NonMobileNO,
                Secret = channel_secret,
                payment_method_id = payment_method_id,

                ContentType = Content_Type,
                MerchantID = X_sdpg_merchant_id,
                Signature = "",
                Nonce = Nonce,
                Body = body
            };

            return getPaymentInquiryToSuperDuperQuery;
        }

        private SaveDeductionLogCommand SaveRegisterDeductionLog(string action, GetPaymentToSuperDuperQuery query, GetPaymentToDeductionModel result)
        {
            SaveDeductionLogCommand command;
            if (action == "New")
            {
                command = new SaveDeductionLogCommand()
                {
                    p_action = action,
                    p_user_name = query.ServiceName,
                    p_transaction_id = result.txn_id.ToSafeString(),
                    p_mobile_no = query.p_mobile_no.ToSafeString(),
                    p_service_name = query.ServiceName,
                    p_endpoint = query.Url,

                    p_pm_tux_code = "",
                    p_pm_receipt_num = "",
                    p_enq_status = !string.IsNullOrEmpty(result.status) ? result.status : "",
                    p_enq_status_code = result.status_code,
                    p_order_transaction_id = query.Body.order_id,

                    p_req_xml_param = "",
                    p_res_xml_param = ""
                };
                InterfaceLogServiceHelper.DeductionLog(_objService, command, query);
            }
            else
            {
                command = new SaveDeductionLogCommand()
                {
                    p_action = action,
                    p_user_name = query.ServiceName,
                    p_transaction_id = result.txn_id.ToSafeString(),
                    p_mobile_no = query.p_mobile_no.ToSafeString(),
                    p_service_name = query.ServiceName,
                    p_endpoint = query.Url,

                    p_pm_tux_code = "",
                    p_pm_receipt_num = "",
                    p_enq_status = !string.IsNullOrEmpty(result.status) ? result.status : "Failed",
                    p_enq_status_code = result.status_code,
                    p_order_transaction_id = query.Body.order_id,

                    p_req_xml_param = "",
                    p_res_xml_param = ""
                };
                InterfaceLogServiceHelper.DeductionLog(_objService, command, result);
            }
            return command;
        }

        private void SaveRegisterPendingDeduction(GetPaymentToDeductionQuery queryData, GetPaymentToSuperDuperQuery query, GetPaymentToDeductionModel result)
        {
            if (result != null && query.Body != null && !string.IsNullOrEmpty(result.form_url) && !string.IsNullOrEmpty(result.txn_id))
            {
                var command = new SavePendingDeductionCommand
                {
                    p_transaction_id = result.txn_id,
                    p_mobile_no = queryData.InternetNo,
                    p_ba_no = queryData.BillingNo,
                    p_paid_amt = query.Body.amount,
                    p_channel = queryData.ProductName,
                    p_merchant_id = query.MerchantID,
                    p_payment_method_id = query.payment_method_id,
                    p_order_transaction_id = query.Body.order_id,
                };
                _savePendingDeduction.Handle(command);
            }
        }

        private string SetPaymentToDeductionUrlFail(string urlFail, string orderTransactionId)
        {
            string urlResult;
            try
            {
                //urlFail = "http://localhost:50960/Process/MeshPaymentToSuperDuperFailResult{0}";

                var paraData = "OrderTransactionID=" + orderTransactionId;
                var paraResult = string.Format("?Data={0}", EncryptionUtility.Base64Encode(paraData));
                urlResult = urlFail.Replace("{0}", paraResult);
            }
            catch (Exception ex)
            {
                urlResult = urlFail;
                _logger.Info("GetPaymentToDeductionHandler SetPaymentToDeductionUrlFail Exception " + ex.GetErrorMessage());
            }
            return urlResult;
        }
    }
}