using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
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
    public class GetOnlineAuthenRequestTokenQueryHandler : IQueryHandler<GetOnlineAuthenRequestTokenQuery, GetOnlineAuthenRequestTokenQueryModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_CFG_LOV> _cfgLov;

        public GetOnlineAuthenRequestTokenQueryHandler(ILogger logger,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog, IWBBUnitOfWork uow, IEntityRepository<FBB_CFG_LOV> cfgLov)
        {
            _logger = logger;
            _intfLog = intfLog;
            _uow = uow;
            _cfgLov = cfgLov;
        }

        //R23.05 CheckFraud
        public GetOnlineAuthenRequestTokenQueryModel Handle(GetOnlineAuthenRequestTokenQuery query)
        {
            var lovConfigs = LoadLOVConfigToken();
            var url = GetConfig(lovConfigs, "requesttoken");
            var secertKey = GetConfig(lovConfigs, "Bearer");
            var client = new RestClient(url);
            var request = RequestBuilder(secertKey);
            var log = CreateOnlineAuthenLog(query);
            var response = client.Execute(request);
            var result = CheckStatus(response, log);
            return result;
        }

        private string GetConfig(List<string> lovConfigs, string target)
       => lovConfigs.FirstOrDefault(f => f.Contains(target));

        private List<string> LoadLOVConfigToken()
        => _cfgLov.Get(l => (l.LOV_NAME == "URL_ONLINE_AUTHEN" || l.LOV_NAME == "URL_ONLINE_AUTHEN_SECRET_KEY") && l.ACTIVEFLAG == "Y").Select(s => s.LOV_VAL1).ToList();

        private InterfaceLogCommand CreateOnlineAuthenLog(GetOnlineAuthenRequestTokenQuery query)
        => InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.Transaction_Id, "GetOnlineAuthenRequestToken", "GetOnlineAuthenRequestTokenQueryHandler", null, "FBB", "");

        private RestRequest RequestBuilder(string secertKey)
        {
            var request = new RestRequest();
            request.Method = Method.POST;
            request.AddHeader("Authorization", secertKey);
            request.AddParameter("application/json", "", ParameterType.RequestBody);

            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback =
                (s, certificate, chain, sslPolicyErrors) => true;

            return request;
        }

        private GetOnlineAuthenRequestTokenQueryModel CheckStatus(IRestResponse response, InterfaceLogCommand log)
        {
            var StatusCode = "Failed";
            var StatusMessage = "";
            var result = new GetOnlineAuthenRequestTokenQueryModel();
            try
            {
                result = JsonConvert.DeserializeObject<GetOnlineAuthenRequestTokenQueryModel>(response.Content) ?? new GetOnlineAuthenRequestTokenQueryModel();

                if (!HttpStatusCode.OK.Equals(response.StatusCode) || result == null)
                {
                    StatusMessage = (string.IsNullOrEmpty(response.ErrorMessage)
                            ? response.Content
                            : response.ErrorMessage).ToSafeString();
                }

                if (result.returnCode == "20000")
                {
                    StatusCode = "Success";
                    StatusMessage = result.returnMessage;
                }

                if (string.IsNullOrEmpty(result?.returnCode))
                {
                    result.returnCode = StatusCode;
                    result.returnMessage = StatusMessage;
                }
            }
            catch (Exception ex)
            {
                StatusMessage = ex.GetBaseException().ToString();
                _logger.Info("GetOnlineQueryMobileInfoHandler Exception " + ex.GetErrorMessage());
            }
            finally
            {
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, StatusCode, StatusMessage, "");
            }
            return result;
        }
    }
}
