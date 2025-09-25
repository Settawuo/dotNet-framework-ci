using Newtonsoft.Json;
using RestSharp;
using System;
using System.Data;
using System.Linq;
using System.Net;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.WebServices;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Models;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class GetAccessTokenQueryHandler : IQueryHandler<GetAccessTokenQuery, GetAccessTokenQueryModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_CFG_LOV> _cfgLov;

        public GetAccessTokenQueryHandler(ILogger logger, IEntityRepository<FBB_INTERFACE_LOG> intfLog, IWBBUnitOfWork uow, IEntityRepository<FBB_CFG_LOV> cfgLov)
        {
            _logger = logger;
            _intfLog = intfLog;
            _uow = uow;
            _cfgLov = cfgLov;
        }

        public GetAccessTokenQueryModel Handle(GetAccessTokenQuery query)
        {
            var StatusCode = "Failed";
            var StatusMessage = "";
            var result = new GetAccessTokenQueryModel();
            InterfaceLogCommand log1 = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.code, "MenuOfficerAuthen", "GetAccessTokenQueryHandler", "", "FBB", "");//log ids code
            try
            {
                var urlconfig = _cfgLov.Get().Where(l => l.LOV_NAME == "IDS_Access_Token_Url"
                                         && l.ACTIVEFLAG == "Y").Select(s => s.LOV_VAL1).FirstOrDefault();
                var client = new RestClient(urlconfig.ToString());
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                request.AddHeader("Authorization", "Basic " + query.authorization);
                request.AddParameter("grant_type", "authorization_code");
                request.AddParameter("code", query.code);
                request.AddParameter("redirect_uri", query.redirect_uri);
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.ServerCertificateValidationCallback = (s, certificate, chain, sslPolicyErrors) => true;
                var response = client.Execute(request);

                if (HttpStatusCode.OK.Equals(response.StatusCode))
                {
                    result = JsonConvert.DeserializeObject<GetAccessTokenQueryModel>(response.Content) ?? new GetAccessTokenQueryModel();
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log1, "Success", StatusMessage, "");
                }
                else
                {
                    StatusMessage = JsonConvert.SerializeObject(response.ErrorMessage);
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, response, log1, StatusCode, StatusMessage, "");
                }
            }
            catch (Exception ex)
            {
                result.scope = ex.Message;//for check exception
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log1, StatusCode, ex.Message, "");
            }
            return result;
        }
    }
}
