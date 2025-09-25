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
    public class RequestsGSSOServiceHandler : IQueryHandler<RequestsGSSOServiceQuery, RequestsGSSOServiceModel>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<string> _objService;
        private readonly IEntityRepository<FBB_CFG_LOV> _lov;

        public RequestsGSSOServiceHandler(ILogger logger,
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

        public RequestsGSSOServiceModel Handle(RequestsGSSOServiceQuery query)
        {
            InterfaceLogCommand log = null;
            RequestsGSSOServiceModel result = new RequestsGSSOServiceModel();
            result.GSSOContent = "";
            result.StatusCode = "Failed";
            result.StatusMessage = "Failed";

            try
            {
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.p_mobile_no,
                    "RequestsGSSOService", "RequestsGSSOServiceHandler", "", "FBB|" + query.FullUrl, "");

                RestClient client = new RestClient(query.p_endpoint);
                RestRequest request = new RestRequest();
                request.Method = Method.POST;
                request.AddHeader("Content-Type", "text/plain");
                request.AddParameter("application/json", query.p_bodyJsonStr, ParameterType.RequestBody);

                // execute the request
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                ServicePointManager.ServerCertificateValidationCallback =
                    (s, certificate, chain, sslPolicyErrors) => true;

                IRestResponse response = client.Execute(request);

                if (HttpStatusCode.OK.Equals(response.StatusCode))
                {
                    result.GSSOContent = response.Content;
                    result.StatusCode = "Success";
                    result.StatusMessage = "";
                }
                else
                {
                    result.StatusCode = "Failed";
                    result.StatusMessage = (!string.IsNullOrEmpty(response.ErrorMessage)
                            ? response.ErrorMessage
                            : response.Content).ToSafeString();
                }
            }
            catch (Exception ex)
            {
                result.StatusCode = "Failed";
                result.StatusMessage = ex.GetBaseException().ToString();
            }
            finally
            {
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, result.StatusCode, result.StatusMessage, "");
            }

            return result;
        }
    }
}
