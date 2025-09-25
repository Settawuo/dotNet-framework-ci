using System;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using WBBContract;
using WBBContract.Queries.WebServices;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Models;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class IdentifyServiceRedirectQueryHandler : IQueryHandler<IdentifyServiceRedirectQuery, string>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_CFG_IDS> _cfIds;
        private readonly IEntityRepository<FBB_CFG_LOV> _cfLov;
        public IdentifyServiceRedirectQueryHandler(ILogger logger, IEntityRepository<FBB_CFG_IDS> cfgIds, IEntityRepository<FBB_CFG_LOV> cfLov)
        {
            _logger = logger;
            _cfIds = cfgIds;
            _cfLov = cfLov;
        }
        public string Handle(IdentifyServiceRedirectQuery query)
        {
            try
            {
                var urlconfig = _cfLov.Get().Where(l => l.LOV_NAME == "IDS_URL" && l.ACTIVEFLAG == "Y").Select(s => s.LOV_VAL1).FirstOrDefault();
                var ids_config = _cfIds.Get().Where(config => config.CHANNEL == query.CHANNEL && config.ACTIVE_FLAG == "Y").FirstOrDefault();
                _logger.Info("IDS Config Return Callback Url : " + ids_config.CALLBACK_URL);
                using (var http = new HttpClient())
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.ServerCertificateValidationCallback = (s, certificate, chain, sslPolicyErrors) => true;
                    var response = http.GetAsync($"{urlconfig}/oauth2/authorize?client_id={ids_config.CLIENT_ID}&scope=openid&response_type=code&redirect_uri={ids_config.CALLBACK_URL}").Result;
                    return response.RequestMessage.RequestUri.ToString();
                }
            }
            catch (Exception ex)
            {
                _logger.Info("Call IDS Service Redirect Url Error: " + ex.Message.ToString()); 
                throw ex;
            }
        }


    }
}
