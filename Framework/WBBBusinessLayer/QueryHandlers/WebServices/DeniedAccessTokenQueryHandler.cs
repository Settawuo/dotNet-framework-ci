using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using WBBContract;
using WBBContract.Queries.WebServices;
using WBBData.Repository;
using WBBEntity.Models;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class DeniedAccessTokenQueryHandler : IQueryHandler<DeniedAccessToken, string>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_CFG_LOV> _cfLov;

        public DeniedAccessTokenQueryHandler(ILogger logger, IEntityRepository<FBB_CFG_LOV> cfLov)
        {
            _logger = logger;
            _cfLov = cfLov;
        }
        public string Handle(DeniedAccessToken query)
        {
            try
            {
                var urlconfig = _cfLov.Get().Where(l => l.LOV_NAME == "IDS_URL" && l.ACTIVEFLAG == "Y").Select(s => s.LOV_VAL1).FirstOrDefault();
                using (var http = new HttpClient())
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.ServerCertificateValidationCallback = (s, certificate, chain, sslPolicyErrors) => true;
                    var response = http.GetAsync($"{urlconfig}/oidc/logout?id_token_hint={query.id_token}&post_logout_redirect_uri={query.redirect_uri}").Result;
                    return response.RequestMessage.RequestUri.ToString();
                }
            }
            catch (Exception ex)
            {
                _logger.Info("LOGOUT ERROR IDS");
                throw;
            }
        }
    }
}
