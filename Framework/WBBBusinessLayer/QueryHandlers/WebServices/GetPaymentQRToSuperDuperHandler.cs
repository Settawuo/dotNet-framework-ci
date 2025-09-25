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
    public class GetPaymentQRToSuperDuperHandler : IQueryHandler<GetPaymentQRToSuperDuperQuery, GetPaymentQRToSuperDuperModel>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<string> _objService;

        public GetPaymentQRToSuperDuperHandler(ILogger logger,
            IWBBUnitOfWork uow,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IEntityRepository<string> objService)
        {
            _logger = logger;
            _uow = uow;
            _intfLog = intfLog;
            _objService = objService;
        }

        public GetPaymentQRToSuperDuperModel Handle(GetPaymentQRToSuperDuperQuery query)
        {
            InterfaceLogCommand log = null;
            GetPaymentQRToSuperDuperModel result = new GetPaymentQRToSuperDuperModel();

            string content = "";
            string StatusCode = "Failed";
            string StatusMessage = "Failed";
            var p_order_id = "";
            var p_order_transaction_id = "";

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
                p_order_id = query.Body.order_id;
                p_order_transaction_id = "Q" + DateTime.Now.ToString("yyyyMMddHHmmssfffff");
                query.Body.order_id = p_order_transaction_id;

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
                    "GetPaymentQRToSuperDuper", "GetPaymentQRToSuperDuperHandler", "", "FBB|" + query.FullUrl, "");

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

                content = response.Content; // raw content as string

                if (HttpStatusCode.OK.Equals(response.StatusCode))
                {
                    content = content.Replace("\r", "\\r");
                    result = JsonConvert.DeserializeObject<GetPaymentQRToSuperDuperModel>(content) ?? new GetPaymentQRToSuperDuperModel();
                    if (result != null)
                    {
                        //StatusCode = result.status_code == "I0000" ? "Success" : "Failed";
                        //StatusMessage = result.status_message;
                        StatusCode = result.status == "PENDING" ? "Success" : "Failed";
                        StatusMessage = result.message;
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
                    p_status = "",
                    p_status_code = StatusCode,
                    p_status_message = "",
                    p_channel = "",
                    p_amount = query.Body.amount,
                    p_req_xml_param = "",
                    p_res_xml_param = "",
                    p_order_transaction_id = p_order_transaction_id,
                };
                InterfaceLogServiceHelper.SavePaymentSPDPLog(_objService, command2, content);

                StatusCode = string.IsNullOrEmpty(StatusCode) ? "Success" : "Failed";
            }
            catch (Exception ex)
            {
                StatusCode = "Failed";
                StatusMessage = ex.GetBaseException().ToString();
                _logger.Info(ex.GetErrorMessage());

                //result.status_message = StatusMessage;
                //result.status_code = StatusCode;
                result.message = StatusMessage;
                result.status = StatusCode;
            }
            finally
            {
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, StatusCode, StatusMessage, "");
            }

            return result;
        }

    }
}
