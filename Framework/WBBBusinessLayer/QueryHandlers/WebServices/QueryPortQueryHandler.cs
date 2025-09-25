using System;
using System.Linq;
using System.Net;
using System.Runtime.Caching;
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
    public class QueryPortQueryHandler : IQueryHandler<QueryPortQuery, QueryPortModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<FBB_INTERFACE_LOG_3BB> _intfLog3bb;
        private readonly IEntityRepository<FBB_CFG_LOV> _lov;
        private readonly IWBBUnitOfWork _uow;
        private readonly IQueryHandler<GetTokenFbbQuery, GetTokenFbbModel> _queryProcessor;
        private readonly MemoryCache cache = MemoryCache.Default;

        public QueryPortQueryHandler(ILogger logger,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IEntityRepository<FBB_INTERFACE_LOG_3BB> intfLog3bb,
            IEntityRepository<FBB_CFG_LOV> lov,
            IWBBUnitOfWork uow,
            IQueryHandler<GetTokenFbbQuery, GetTokenFbbModel> queryProcessor)
        {
            _logger = logger;
            _intfLog = intfLog;
            _intfLog3bb = intfLog3bb;
            _lov = lov;
            _uow = uow;
            _queryProcessor = queryProcessor;
        }

        public QueryPortModel Handle(QueryPortQuery query)
        {
            int countException = 0;
            InterfaceLog3BBCommand log3bb = null;

            QueryPortModel result = new QueryPortModel();

            if (query.FullUrl == "3BB") log3bb = InterfaceLogServiceHelper.StartInterfaceLog3BB(_uow, _intfLog3bb, query, query.TRANSACTION_ID, "QueryPort", "QueryPortQueryHandler", "", "3BB", "");

            repeat:
            try
            {
                #region R24.10 Call Access Token FBSS
                string accessToken = string.Empty;
                string channel = FBSSAccessToken.channelFBB.ToUpper();
                accessToken = (string)cache.Get(channel); //Get cache

                if (string.IsNullOrEmpty(accessToken))
                {
                    string clientId = string.Empty;
                    string clientSecret = string.Empty;
                    string grantType = string.Empty;
                    var loveConfigList = _lov.Get(lov => lov.ACTIVEFLAG.Equals("Y") && lov.LOV_TYPE.Equals("FBSS_Authen")).ToList();
                    if (loveConfigList != null && loveConfigList.Count() > 0)
                    {
                        clientId = loveConfigList.FirstOrDefault(t => t.LOV_NAME == channel) != null ? loveConfigList.FirstOrDefault(t => t.LOV_NAME == channel).LOV_VAL1 : "";
                        clientSecret = loveConfigList.FirstOrDefault(t => t.LOV_NAME == channel) != null ? loveConfigList.FirstOrDefault(t => t.LOV_NAME == channel).LOV_VAL2 : "";
                        grantType = loveConfigList.FirstOrDefault(t => t.LOV_NAME == channel) != null ? loveConfigList.FirstOrDefault(t => t.LOV_NAME == channel).LOV_VAL4 : "";
                    }

                    var getToken = new GetTokenFbbQuery()
                    {
                        Channel = channel,
                        ParamGetoken = new ParametersGetoken()
                        {
                            client_id = clientId,
                            client_secret = clientSecret,
                            grant_type = grantType
                        }
                    };

                    var responseGetToken = _queryProcessor.Handle(getToken);
                    accessToken = (string)cache.Get(channel);
                }

                //log access token
                InterfaceLog3BBCommand logToken3BB = null;
                InterfaceLogCommand logToken = null;
                if (query.FullUrl == "3BB")
                {
                    logToken3BB = InterfaceLogServiceHelper.StartInterfaceLog3BB(_uow, _intfLog3bb, accessToken, query.TRANSACTION_ID, "QueryPortToken", "QueryPortQueryHandlerToken", "", "3BB", "");
                }
                else
                {
                    logToken = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, accessToken, query.TRANSACTION_ID, "QueryPortToken", "QueryPortQueryHandlerToken", "", "FBB", "");
                }

                if (!string.IsNullOrEmpty(accessToken))
                {
                    if (query.FullUrl == "3BB")
                    {
                        InterfaceLogServiceHelper.EndInterfaceLog3BB(_uow, _intfLog3bb, "", logToken3BB, "Success", "", "");
                    }
                    else
                    {
                        InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, "", logToken, "Success", "", "");
                    }
                }
                else
                {
                    if (query.FullUrl == "3BB")
                    {
                        InterfaceLogServiceHelper.EndInterfaceLog3BB(_uow, _intfLog3bb, "", logToken3BB, "Failed", "Access Token is Null", "");
                    }
                    else
                    {
                        InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, "", logToken, "Failed", "Access Token is Null", "");
                    }
                }
                #endregion

                QueryPortResponse Response = new QueryPortResponse();
                string RESULT_CODE = "";
                string RESULT_DESC = "";
                string RESOURCE_NO = query.RESOURCE_NO;
                string RESOURCE_NAME = "";
                string RESOURCE_ALIAS = "";
                string RESOURCE_SITE_NO = "";
                string RESOURCE_LATITUDE = "";
                string RESOURCE_LONGITUDE = "";
                FBSSOrderServices.portNoList[] PORT_NO_LIST;

                //R21.2 Endpoint FBSSOrderServices.OrderService
                var endpointFbssOrderServices = (from l in _lov.Get() where l.LOV_NAME == "FBSS_ORDER_SERVICES" select l.LOV_VAL1).ToList().FirstOrDefault();

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.ServerCertificateValidationCallback = (s, certificate, chain, sslPolicyErrors) => true;

                using (var service = new FBSSAccessToken.CustomOrderService(accessToken))
                {
                    if (!string.IsNullOrEmpty(endpointFbssOrderServices)) service.Url = endpointFbssOrderServices;

                    RESULT_CODE = service.QueryPort(
                            ref RESOURCE_NO,
                            query.RESOURCE_TYPE,
                            query.SERVICE_STATE,
                            out RESULT_DESC,
                            out RESOURCE_NAME,
                            out RESOURCE_ALIAS,
                            out RESOURCE_SITE_NO,
                            out RESOURCE_LATITUDE,
                            out RESOURCE_LONGITUDE,
                            out PORT_NO_LIST
                        );

                    Response.RESULT_CODE = RESULT_CODE.ToSafeString();
                    Response.RESULT_DESC = RESULT_DESC.ToSafeString();
                    Response.RESOURCE_NO = query.RESOURCE_NO.ToSafeString();
                    Response.RESOURCE_NAME = RESOURCE_NAME.ToSafeString();
                    Response.RESOURCE_ALIAS = RESOURCE_ALIAS.ToSafeString();
                    Response.RESOURCE_SITE_NO = RESOURCE_SITE_NO.ToSafeString();
                    Response.RESOURCE_LATITUDE = RESOURCE_LATITUDE.ToSafeString();
                    Response.RESOURCE_LONGITUDE = RESOURCE_LONGITUDE.ToSafeString();

                    if (PORT_NO_LIST != null && PORT_NO_LIST.Length > 0)
                    {
                        Response.QueryPortNoList = PORT_NO_LIST.Select(x => new QueryPortNo()
                        {
                            PORT_NO = x.PORT_NO.ToSafeString(),
                            PSTN = x.PSTN.ToSafeString(),
                            RELATE_DEVICE = x.RELATE_DEVICE.ToSafeString(),
                            MDF_VOICE = x.MDF_VOICE.ToSafeString(),
                            MDF_DATA = x.MDF_DATA.ToSafeString(),
                            SERVICE_STATE = x.SERVICE_STATE.ToSafeString(),
                            CUSTOMER_SERVICE_NO = x.CUSTOMER_SERVICE_NO.ToSafeString(),
                            REMARK = x.REMARK.ToSafeString()
                        }).ToList();
                    }
                    else
                    {
                        Response.QueryPortNoList = null;
                    }
                }

                result.Data = Response;
                result.return_code = "0";
                result.return_message = "Success";

                if (query.FullUrl == "3BB") InterfaceLogServiceHelper.EndInterfaceLog3BB(_uow, _intfLog3bb, result, log3bb, "Success", "", "");
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

                result.Data = null;
                result.return_code = "-1";
                result.return_message = webEx.Message;

                if (query.FullUrl == "3BB") InterfaceLogServiceHelper.EndInterfaceLog3BB(_uow, _intfLog3bb, webEx.Message, log3bb, "Failed", webEx.Message, "");
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.return_code = "-1";
                result.return_message = ex.Message;

                if (query.FullUrl == "3BB") InterfaceLogServiceHelper.EndInterfaceLog3BB(_uow, _intfLog3bb, ex.Message, log3bb, "Failed", ex.Message, "");
            }

            return result;
        }
    }
}