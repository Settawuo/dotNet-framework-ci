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
    public class GetProfileQueryHandler : IQueryHandler<GetProfileQuery, GetProfileQueryModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_CFG_LOV> _cfgLov;

        public GetProfileQueryHandler(ILogger logger, IEntityRepository<FBB_INTERFACE_LOG> intfLog, IWBBUnitOfWork uow, IEntityRepository<FBB_CFG_LOV> cfgLov)
        {
            _logger = logger;
            _intfLog = intfLog;
            _uow = uow;
            _cfgLov = cfgLov;
        }

        public GetProfileQueryModel Handle(GetProfileQuery query)
        {
            var StatusCode = "";
            var StatusMessage = "";
            var result = new GetProfileQueryModel();
            InterfaceLogCommand log1 = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.authorization, "GetProfile", "GetProfileQueryHandler", "", "FBB", "");//log ids getProfile
            try
            {
                var urlconfig = _cfgLov.Get().Where(l => l.LOV_NAME == "IDS_Get_Profile_Url"
                                            && l.ACTIVEFLAG == "Y").Select(s => s.LOV_VAL1).FirstOrDefault();
                var client = new RestClient(urlconfig.ToString());
                var request = new RestRequest(Method.GET);
                request.AddHeader("Authorization", "Bearer " + query.authorization);
                var response = client.Execute(request);
                if (HttpStatusCode.OK.Equals(response.StatusCode))
                {
                    if (response.Content.Contains("asccode"))
                    {
                        response.Content = response.Content.Replace("asccode", "asc_code");
                    }
                    result = JsonConvert.DeserializeObject<GetProfileQueryModel>(response.Content) ?? new GetProfileQueryModel();
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log1, "Success", StatusMessage, "");
                }
                else
                {
                    StatusMessage = response.ErrorMessage;
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, response, log1, StatusCode, StatusMessage, "");
                }
            }
            catch (Exception ex)
            {
                StatusMessage += ex.Message;
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log1, StatusCode, StatusMessage, "");
            }
            return result;
        }
    }
}
