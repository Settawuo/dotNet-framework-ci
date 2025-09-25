using Newtonsoft.Json;
using RestSharp;
using System;
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
    public class GetPaymentToSuperDuperHandler : IQueryHandler<GetPaymentToSuperDuperQuery, GetPaymentToSuperDuperModel>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<string> _objService;
        private readonly IEntityRepository<FBB_CFG_LOV> _lov;

        public GetPaymentToSuperDuperHandler(ILogger logger,
            IWBBUnitOfWork uow,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IEntityRepository<string> objService,
            IEntityRepository<FBB_CFG_LOV> lov)
        {
            _logger = logger;
            _uow = uow;
            _intfLog = intfLog;
            _objService = objService;
            _lov = lov;
        }

        public GetPaymentToSuperDuperModel Handle(GetPaymentToSuperDuperQuery query)
        {
            InterfaceLogCommand log = null;
            var result = new GetPaymentToSuperDuperModel();
            string content = "";
            string StatusCode = "Failed";
            string StatusMessage = "Failed";

            if (query.ProductName == "MESH")
            {
                query.FullUrl += "/MESH";
            }
            else if (query.ProductName == "BYOD")
            {
                query.FullUrl += "/SCPE";
            }

            try
            {
                var p_order_id = query.Body.order_id;
                var p_order_transaction_id = "P" + DateTime.Now.ToString("yyyyMMddHHmmssfffff");
                query.Body.order_id = p_order_transaction_id;
                if (query.Body.Eds != null)
                {
                    query.Body.Eds.Eds_url_fail = SetPaymentToSuperDuperUrlFail(query.Body.Eds.Eds_url_fail, query.Body.order_id);
                }

                string BodyStr = JsonConvert.SerializeObject(query.Body);
                query.BodyStr = BodyStr;

                query.Signature = EncrypSuperDuperHelper.hmacsha256(query.Secret, query.Nonce, query.Body);
                SavePaymentSPDPLogCommand command1 = new SavePaymentSPDPLogCommand()
                {
                    p_action = "New",
                    p_user_name = query.ProductName,
                    p_non_mobile_no = query.p_mobile_no,
                    p_service_name = query.ServiceName,
                    p_endpoint = query.Url,
                    p_order_id = p_order_id,
                    p_txn_id = "",
                    p_status = "",
                    p_status_code = "",
                    p_status_message = "",
                    p_channel = "",
                    p_amount = query.Body.amount,
                    p_req_xml_param = "",
                    p_res_xml_param = "",
                    p_order_transaction_id = p_order_transaction_id,
                };
                InterfaceLogServiceHelper.SavePaymentSPDPLog(_objService, command1, query);

                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.p_mobile_no,
                    "GetPaymentToSuperDuper", "GetPaymentToSuperDuperHandler", "", "FBB|" + query.FullUrl, "");

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
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                ServicePointManager.ServerCertificateValidationCallback =
                    (s, certificate, chain, sslPolicyErrors) => true;

                var response = client.Execute(request);

                content = response.Content; // raw content as string

                if (HttpStatusCode.OK.Equals(response.StatusCode))
                {
                    result = JsonConvert.DeserializeObject<GetPaymentToSuperDuperModel>(response.Content) ?? new GetPaymentToSuperDuperModel();
                    if (result != null)
                    {
                        if (result != null)
                        {
                            StatusCode = "";
                            StatusMessage = "";
                        }
                        InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, content, log,
                            result.status_code == "I0000" ? "Success" : "Failed", "", "");
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

                SavePaymentSPDPLogCommand command2 = new SavePaymentSPDPLogCommand()
                {
                    p_action = "Update",
                    p_user_name = query.ProductName,
                    p_non_mobile_no = query.p_mobile_no,
                    p_service_name = query.ServiceName,
                    p_endpoint = query.Url,
                    p_order_id = p_order_id,
                    p_txn_id = result.txn_id.ToSafeString(),
                    p_status = result.status,
                    p_status_code = result.status_code,
                    p_status_message = "",
                    p_channel = "",
                    p_amount = "",
                    p_req_xml_param = "",
                    p_res_xml_param = "",
                    p_order_transaction_id = p_order_transaction_id,
                };
                InterfaceLogServiceHelper.SavePaymentSPDPLog(_objService, command2, response.Content);

                StatusCode = string.IsNullOrEmpty(StatusCode) ? "Success" : "Failed";
            }
            catch (Exception ex)
            {
                StatusCode = "Failed";
                StatusMessage = ex.GetBaseException().ToString();
                _logger.Info("GetPaymentToSuperDuperHandler Exception " + ex.GetErrorMessage());
            }
            finally
            {
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, StatusCode, StatusMessage, "");
            }

            return result;
        }

        private string SetPaymentToSuperDuperUrlFail(string urlFail, string orderTransactionId)
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
                _logger.Info("GetPaymentToSuperDuperHandler SetPaymentToSuperDuperUrlFail Exception " + ex.GetErrorMessage());
            }
            return urlResult;
        }
    }
}
