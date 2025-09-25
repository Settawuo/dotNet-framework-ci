using Newtonsoft.Json;
using RestSharp;
using System;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using WBBContract;
using WBBContract.Queries.WebServices;
using WBBData.Repository;
using WBBEntity.Models;
using WBBEntity.PanelModels.WebServiceModels;
using static System.Net.WebRequestMethods;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class GetAccessTokenFBBConfigQueryHandler : IQueryHandler<GetAccessTokenFBBConfigQuery, GetAccessTokenQueryModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_CFG_IDS> _cfgIds;
        private readonly IEntityRepository<FBB_CFG_LOV> _cfLov;
        public GetAccessTokenFBBConfigQueryHandler(ILogger logger, IEntityRepository<FBB_CFG_IDS> cfgIds, IEntityRepository<FBB_CFG_LOV> cfLov)
        {
            _logger = logger;
            _cfgIds = cfgIds;
            _cfLov = cfLov;
        }

        public GetAccessTokenQueryModel Handle(GetAccessTokenFBBConfigQuery query)
        {
            var result = new GetAccessTokenQueryModel();
            try
            {
                var urlconfig = _cfLov.Get().Where(l => l.LOV_NAME == "IDS_URL" && l.ACTIVEFLAG == "Y").Select(s => s.LOV_VAL1).FirstOrDefault();
                var config = _cfgIds.Get().Where(config => config.CHANNEL == "FBBOFFICER" && config.ACTIVE_FLAG == "Y").FirstOrDefault();
                var client = new RestClient($"{urlconfig}/oauth2/token");

                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                request.AddHeader("Authorization", "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes($"{config.CLIENT_ID}:{config.CLIENT_SECRET}")));
                request.AddParameter("grant_type", "authorization_code");
                request.AddParameter("code", query.code);
                request.AddParameter("redirect_uri", query.redirect_uri);
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.ServerCertificateValidationCallback = (s, certificate, chain, sslPolicyErrors) => true;
                var response = client.Execute(request);
                result = JsonConvert.DeserializeObject<GetAccessTokenQueryModel>(response.Content);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Info("Call IDS Service Get Token Error: " + ex.Message.ToString()); //for check exception
                throw ex;
            }
        }
    }
}
