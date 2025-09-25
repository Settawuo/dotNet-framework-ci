using Newtonsoft.Json;
using RestSharp;
using System;
using System.Net;
using System.Security.Cryptography;
using System.Text;
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
    public class GetPaymentEnquiryToSuperDuperHandler : IQueryHandler<GetPaymentEnquiryToSuperDuperQuery, GetPaymentEnquiryToSuperDuperModel>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<string> _objService;

        public GetPaymentEnquiryToSuperDuperHandler(ILogger logger,
            IWBBUnitOfWork uow,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IEntityRepository<string> objService)
        {
            _logger = logger;
            _uow = uow;
            _intfLog = intfLog;
            _objService = objService;
        }

        public GetPaymentEnquiryToSuperDuperModel Handle(GetPaymentEnquiryToSuperDuperQuery query)
        {
            InterfaceLogCommand log = null;
            var result = new GetPaymentEnquiryToSuperDuperModel();
            try
            {
                query.Signature = EncrypSuperDuperHelper.hmacsha256(query.Secret, query.Nonce, query.Body);

                //Insert Log
                UpdateLog(query);

                string BodyStr = JsonConvert.SerializeObject(query.Body);
                query.BodyStr = BodyStr;

                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.Body.txn_id,
                    "GetPaymentEnquiryToSuperDuper", "GetPaymentEnquiryToSuperDuperHandler", "", "FBB", "");

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
                ServicePointManager.ServerCertificateValidationCallback = (s, certificate, chain, sslPolicyErrors) => true;

                var response = client.Execute(request);

                var content = response.Content; // raw content as string

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    try
                    {
                        result = JsonConvert.DeserializeObject<GetPaymentEnquiryToSuperDuperModel>(response.Content) ?? new GetPaymentEnquiryToSuperDuperModel();
                        InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, content, log,
                        result.status.ToUpper() == "SUCCESS" ? "Success" : "Failed", "", "");
                        //Update log Log
                        UpdateLog(query, result.status, response.Content);
                    }
                    catch
                    {
                        result.status = "Failed";
                        //Update log Log
                        UpdateLog(query, result.status, response.Content);

                        InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, response.Content, log,
                        "Failed", "", "");
                    }

                }
                else
                {
                    result.status = "Failed";
                    //Update log Log
                    UpdateLog(query, result.status, response.Content);

                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, "", log, "Failed", "Enquiry Failed", "");
                }
            }
            catch (Exception ex)
            {
                result.status = "Failed";
                //Update log Log
                UpdateLog(query, result.status, "");

                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, "", log, "Failed", ex.GetBaseException().ToString(), "");
                _logger.Info(ex.GetErrorMessage());
            }

            return result;
        }

        private void UpdateLog(GetPaymentEnquiryToSuperDuperQuery query, string status = "", string Content = "")
        {
            var action = string.IsNullOrEmpty(Content) ? "New" : "Update";
            var result = JsonConvert.DeserializeObject<GetPaymentEnquiryToSuperDuperModel>(Content) ?? new GetPaymentEnquiryToSuperDuperModel();
            var statusUpdate = !string.IsNullOrEmpty(result.status) ? result.status : status;

            if (query.p_transaction_id.ToSafeString() != "" && query.p_mobile_no.ToSafeString() != "")
            {
                SaveDeductionLogCommand command1 = new SaveDeductionLogCommand()
                {
                    p_action = action,
                    p_user_name = query.User,
                    p_transaction_id = query.p_transaction_id.ToSafeString(),
                    p_mobile_no = query.p_mobile_no.ToSafeString(),
                    p_service_name = "Enquiry",
                    p_endpoint = query.Url,

                    p_pm_tux_code = "",
                    p_pm_receipt_num = "",
                    p_enq_status = statusUpdate,
                    p_enq_status_code = result.status_code,

                    p_req_xml_param = "",
                    p_res_xml_param = ""
                };
                if (action == "New")
                {
                    InterfaceLogServiceHelper.DeductionLog(_objService, command1, query);
                }
                else
                {
                    InterfaceLogServiceHelper.DeductionLog(_objService, command1, Content);

                }
            }
            else
            {
                SavePaymentSPDPLogCommand command1 = new SavePaymentSPDPLogCommand()
                {
                    p_action = action,
                    p_user_name = query.User,
                    p_non_mobile_no = query.p_mobile_no,
                    p_service_name = "Enquiry",
                    p_endpoint = query.Url,
                    p_order_id = query.p_order_id,
                    p_txn_id = query.Body.txn_id,
                    p_status = statusUpdate,
                    p_status_code = result.status,
                    p_status_message = result.status_message,
                    //20.8 change channel -> channel_type
                    p_channel = result.channel_type,
                    p_amount = "",
                    p_req_xml_param = "",
                    p_res_xml_param = ""
                };
                if (action == "New")
                {
                    InterfaceLogServiceHelper.SavePaymentSPDPLog(_objService, command1, query);
                }
                else
                {
                    InterfaceLogServiceHelper.SavePaymentSPDPLog(_objService, command1, Content);

                }
            }


        }

    }

    public static class EncrypSuperDuperHelper
    {
        public static string hmacsha256<T>(string secret, string Nonce, T body)
        {
            string Body = JsonConvert.SerializeObject(body);
            string message = Body + Nonce;
            secret = secret ?? "";
            var encoding = new System.Text.ASCIIEncoding();
            byte[] keyByte = encoding.GetBytes(secret);
            byte[] messageBytes = encoding.GetBytes(message);
            using (var hmacsha256 = new HMACSHA256(keyByte))
            {
                byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
                return Hex(hashmessage);
            }
        }

        private static string Hex(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }
    }
}
