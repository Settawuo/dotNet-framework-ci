using Newtonsoft.Json;
using RestSharp;
using System;
using System.Net;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.WebServices;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class GetPaymentInquiryToSuperDuperHandler : IQueryHandler<GetPaymentInquiryToSuperDuperQuery, GetPaymentInquiryToSuperDuperModel>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;

        public GetPaymentInquiryToSuperDuperHandler(ILogger logger,
            IWBBUnitOfWork uow,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog
            )
        {
            _logger = logger;
            _uow = uow;
            _intfLog = intfLog;
        }

        public GetPaymentInquiryToSuperDuperModel Handle(GetPaymentInquiryToSuperDuperQuery query)
        {
            InterfaceLogCommand log = null;
            var result = new GetPaymentInquiryToSuperDuperModel();

            try
            {
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.Body.service_id,
                    "GetPaymentInquiryToSuperDuper", "GetPaymentInquiryToSuperDuperHandler", "", "FBB", "");
                var client = new RestClient(query.Url);
                var request = new RestRequest(Method.POST);
                request.Method = Method.POST;
                request.AddHeader("Content-Type", query.ContentType);
                request.AddHeader("X-sdpg-merchant-id", query.MerchantID);
                request.AddHeader("X-sdpg-signature", query.Signature);
                request.AddHeader("X-sdpg-nonce", query.Nonce);
                request.AddJsonBody(query.Body);

                // execute the request
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                ServicePointManager.ServerCertificateValidationCallback =
                    (s, certificate, chain, sslPolicyErrors) => true;
                var response = client.Execute(request);

                var content = response.Content; // raw content as string

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    result = JsonConvert.DeserializeObject<GetPaymentInquiryToSuperDuperModel>(response.Content) ?? new GetPaymentInquiryToSuperDuperModel();

                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, content, log,
                        result.status_code == "I0000" ? "Success" : "Failed", "", "");
                }
                else
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, "", log, "Failed", response.ErrorMessage, "");
                }
            }
            catch (Exception ex)
            {
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, "", log, "Failed", ex.GetBaseException().ToString(), "");
                _logger.Info(ex.GetErrorMessage());
            }

            return result;
        }

    }
}
