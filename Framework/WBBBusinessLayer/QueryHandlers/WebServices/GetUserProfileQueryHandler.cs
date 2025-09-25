using WBBContract.Queries.WebServices;
using WBBContract;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Models;
using WBBContract.QueryModels.WebServices;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System;
using WBBContract.Commands;
using System.Linq;
using System.Threading.Tasks;
using RestSharp;
using WBBBusinessLayer.CommandHandlers;
using System.Security.Policy;
using WBBEntity.PanelModels.WebServiceModels;
using Inetlab.SMPP.Common;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class GetUserProfileQueryHandler : IQueryHandler<GetUserProfileQuery, GetUserProfileQueryModel>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<FBB_CFG_LOV> _lov;

        public GetUserProfileQueryHandler(ILogger logger,
            IWBBUnitOfWork uow,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IEntityRepository<FBB_CFG_LOV> lov)
        {
            _logger = logger;
            _uow = uow;
            _intfLog = intfLog;
            _lov = lov;
        }

        public GetUserProfileQueryModel Handle(GetUserProfileQuery query)
        {
            var StatusCode = "";
            var StatusMessage = "";
            InterfaceLogCommand log1 = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.authToken, "List<QueryUserProfileResponseList>", "GetUserProfileQueryHandler", "", "FBB", "");
            GetUserProfileQueryModel resultJsonConvert = new GetUserProfileQueryModel();
            GetAccessTokenUserProfileQueryModel accessTokenUserProfile = new GetAccessTokenUserProfileQueryModel();
            List<QueryUserProfileResponseList> queryUserProfileResponseList = new List<QueryUserProfileResponseList>();

            var url = _lov.Get().Where(w => w.LOV_TYPE == "FBB_OFFICER" && w.LOV_NAME == "URL_ACCESSTOKEN").FirstOrDefault();
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, url.LOV_VAL1);
            request.Headers.Add("Authorization", "Bearer " + query.authToken);
            var content = new StringContent("{\r\n    \"application_id\": \"" + url.LOV_VAL2 + "\",\r\n    \"login_name\":\"" + url.LOV_VAL3 + "\",\r\n    \"password\":\"" + url.LOV_VAL4 + "\"\r\n}", null, "application/json");
            request.Content = content;
            var response = client.SendAsync(request).Result;

            if (response.IsSuccessStatusCode)
            {
                response.EnsureSuccessStatusCode();
                var listcontent = response.Content.ReadAsStringAsync().Result;
                accessTokenUserProfile = JsonConvert.DeserializeObject<GetAccessTokenUserProfileQueryModel>(listcontent);
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, accessTokenUserProfile, log1, "Success", StatusMessage, "");
                //VO
                InterfaceLogCommand log2 = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.authToken, "CallVO", "GetUserProfileQueryHandler", "", "FBB", "");
                var userList = new List<Userprofile>();
                userList.Add(new Userprofile { Username = query.userName });
                var result = new GetProfileQueryRequest
                {
                    userprofile = userList
                };
                var urlVO = _lov.Get().Where(w => w.LOV_TYPE == "FBB_OFFICER" && w.LOV_NAME == "URL_VO").FirstOrDefault();
                var requestVO = new HttpRequestMessage(HttpMethod.Post, urlVO.LOV_VAL1);
                requestVO.Headers.Add("Authorization", "Bearer " + accessTokenUserProfile.access_token);
                var contentVO = new StringContent(JsonConvert.SerializeObject(result), null, "application/json");
                requestVO.Content = contentVO;
                var responseVO = client.SendAsync(requestVO).Result;
                if (responseVO.IsSuccessStatusCode)
                {
                    responseVO.EnsureSuccessStatusCode();
                    var listcontentVO = responseVO.Content.ReadAsStringAsync().Result;
                    resultJsonConvert = JsonConvert.DeserializeObject<GetUserProfileQueryModel>(listcontentVO);
                    queryUserProfileResponseList = resultJsonConvert.aisEmployeeHierarchy;
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, resultJsonConvert, log2, "Success", StatusMessage, "");
                }
                else
                {
                    Console.WriteLine($"HTTP request failed with status code: {response.StatusCode}");
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, resultJsonConvert, log2, "Failed", StatusMessage, "");
                }
            }
            else
            {
                Console.WriteLine($"HTTP request failed with status code: {response.StatusCode}");
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, resultJsonConvert, log1, "Failed", StatusMessage, "");
            }

            return resultJsonConvert;
        }

    }
}

