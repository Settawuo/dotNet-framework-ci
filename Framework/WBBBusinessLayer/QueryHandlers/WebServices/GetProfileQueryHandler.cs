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
    public class GetProfileQueryFBBConfigHandler : IQueryHandler<GetUserProfileIDSQuery, GetProfileQueryModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_CFG_LOV> _cfLov;

        public GetProfileQueryFBBConfigHandler(ILogger logger, IEntityRepository<FBB_CFG_LOV> cfLov)
        {
            _logger = logger;
            _cfLov = cfLov;
        }
        public GetProfileQueryModel Handle(GetUserProfileIDSQuery query)
        {
            try
            {
                var urlconfig = _cfLov.Get().Where(l => l.LOV_NAME == "IDS_URL" && l.ACTIVEFLAG == "Y").Select(s => s.LOV_VAL1).FirstOrDefault();
                var client = new RestClient($"{urlconfig}/oauth2/userinfo?schema=openid");
                var request = new RestRequest(Method.GET);
                request.AddHeader("Authorization", "Bearer " + query.access_token);
                var response = client.Execute(request);
                return JsonConvert.DeserializeObject<GetProfileQueryModel>(response.Content) ?? new GetProfileQueryModel();
            }
            catch (Exception ex)
            {
                _logger.Info("Call IDS Service Get Profile User Error: " + ex.Message.ToString()); //for check exception
                throw ex;
            }
        }
    }
}
