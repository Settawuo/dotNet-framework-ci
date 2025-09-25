using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Caching;
using System.Threading;
using System.Web.Caching;
using WBBBusinessLayer.Extension;
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
    public class DoServiceConfirmHandler : IQueryHandler<DoServiceConfirmQuery, DoServiceConfirmModel>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<string> _objService;
        private readonly IEntityRepository<FBB_CFG_LOV> _lov;
        private readonly MemoryCache cache = MemoryCache.Default;

        public DoServiceConfirmHandler(ILogger logger,
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

        public DoServiceConfirmModel Handle(DoServiceConfirmQuery query)
        {
            InterfaceLogCommand log = null;
            DoServiceConfirmModel result = new DoServiceConfirmModel();
            int deleyTimeSec = 15;
            int countException = 0;
        repeat:
            try
            {
                List<FBB_CFG_LOV> loveListUrl = null;
                string accessToken = string.Empty;
                string channel = FBSSAccessToken.channelFBB.ToUpper();
                accessToken = (string)cache.Get(channel); //Get cache
                var flag_check_auth = _lov
                    .Get(lov => lov.ACTIVEFLAG.Equals("Y") && lov.LOV_NAME.Equals("USE_TOKEN_WFM")).FirstOrDefault();
                    loveListUrl = _lov
                    .Get(lov => lov.ACTIVEFLAG.Equals("Y") && lov.LOV_NAME.Equals("URL_WFM")).ToList();

                if (loveListUrl != null && loveListUrl.Count() > 0)
                {
                    query.Url = loveListUrl.FirstOrDefault().LOV_VAL1.ToSafeString();
                    query.BodyData.STAFF_ID = loveListUrl.FirstOrDefault().LOV_VAL2.ToSafeString();
                    int.TryParse(loveListUrl.FirstOrDefault().LOV_VAL3.ToSafeString(), out deleyTimeSec);
                }

                List<FBB_CFG_LOV> loveList = null;

                loveList = _lov
                    .Get(lov => lov.ACTIVEFLAG.Equals("Y") && lov.LOV_NAME.Equals("REMARK_SERVICE_CONFIRM")).ToList();

                if (loveListUrl != null && loveListUrl.Count() > 0)
                {
                    query.BodyData.REMARK = loveList.FirstOrDefault().LOV_VAL1.ToSafeString();
                }

                string BodyStr = JsonConvert.SerializeObject(query.BodyData);
                query.BodyStr = BodyStr;

                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.BodyData.FIBRENET_ID,
                    "ServiceConfirmOrder", "ServiceConfirmOrder", "", "FBB|" + query.FullUrl, "");

                var client = new RestClient(query.Url);
                var request = new RestRequest();
                request.Method = Method.POST;
                //log access token
                var logToken = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, accessToken, query.BodyData.FIBRENET_ID, "ServiceConfirmOrderToken", "ServiceConfirmOrderHandleToken", null, "FBB", "");
                if (string.IsNullOrEmpty(accessToken)) // check token
                {
                    if (flag_check_auth != null) //check lov add token
                    {
                        request.AddHeader("Authorization", "Bearer " + accessToken);
                    }
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, "", logToken, "Success", "", "");
                }
                else
                {
                    if (flag_check_auth == null) //check lov add token
                    {
                        InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, "", logToken, "Success", "Access Token is Null", "");
                    } else
                    {
                        InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, "", logToken, "Fail", "Access Token is Null", "");
                    }
                }
                request.AddParameter("application/json", BodyStr, ParameterType.RequestBody);

                // execute the request
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                ServicePointManager.ServerCertificateValidationCallback =
                    (s, certificate, chain, sslPolicyErrors) => true;

                var response = client.Execute(request);

                var content = response.Content; // raw content as string

                if (HttpStatusCode.OK.Equals(response.StatusCode))
                {
                    result = JsonConvert.DeserializeObject<DoServiceConfirmModel>(response.Content) ?? new DoServiceConfirmModel();
                    if (result != null)
                    {
                        InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, content, log, "Success", "", "");
                        /// deleyTime            
                        Thread.Sleep(deleyTimeSec * 1000);
                    }
                    else
                    {
                        result.RESULT_CODE = "1";
                        result.RESULT_DESC = "result null";
                        InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, content, log, "Failed", "", "");
                    }
                }
                else
                {
                    result.RESULT_CODE = "1";
                    result.RESULT_DESC = response.StatusCode.ToString();
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, content, log, "Failed", "", "");
                }

            }
            catch (WebException webEx)
            {
                //R24.10 Call Access Token FBSS Exception 
                if ((webEx.Response is HttpWebResponse response && (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)) && (countException == 0))
                {
                    countException++;
                    cache.Remove(FBSSAccessToken.channelFBB.ToUpper());
                    webEx = null;
                    goto repeat;
                }

                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, webEx.Message, log, "Failed", webEx.Message, "");
                return new DoServiceConfirmModel();
            }
            catch (Exception ex)
            {
                result.RESULT_CODE = "1";
                result.RESULT_DESC = ex.Message;
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, "Failed", ex.Message, "");
            }

            return result;
        }
    }
}
