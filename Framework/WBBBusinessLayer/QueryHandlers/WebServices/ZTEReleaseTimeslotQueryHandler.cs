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
using WBBEntity.Models;
using WBBEntity.PanelModels;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class ZTEReleaseTimeslotQueryHandler : IQueryHandler<ZTEReleaseTimeslotQuery, ZTEReleaseTimeslotModel>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<FBB_CFG_LOV> _lov;
        private readonly IQueryHandler<GetTokenFbbQuery, GetTokenFbbModel> _queryProcessor;
        private readonly MemoryCache cache = MemoryCache.Default;

        public ZTEReleaseTimeslotQueryHandler(ILogger logger,
            IWBBUnitOfWork uow,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IEntityRepository<FBB_CFG_LOV> lov,
            IQueryHandler<GetTokenFbbQuery, GetTokenFbbModel> queryProcessor)
        {
            _logger = logger;
            _uow = uow;
            _intfLog = intfLog;
            _lov = lov;
            _queryProcessor = queryProcessor;
        }
        public ZTEReleaseTimeslotModel Handle(ZTEReleaseTimeslotQuery query)
        {
            int countException = 0;
            InterfaceLogCommand log = null;
            ZTEReleaseTimeslotModel result = new ZTEReleaseTimeslotModel();

            log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.RESERVED_ID, "ZTEReleaseTimeslot", "ZTEReleaseTimeslotQueryHandler", query.ID_CARD_NO, "FBB|" + query.FullUrl, "");
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
                var logToken = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, accessToken, query.RESERVED_ID, "ZTEReleaseTimeslotToken", "ZTEReleaseTimeslotQueryHandlerToken", query.ID_CARD_NO, "FBB|" + query.FullUrl, "");
                
                if (!string.IsNullOrEmpty(accessToken))
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, "", logToken, "Success", "", "");
                }
                else
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, "", logToken, "Failed", "Access Token is Null", "");
                }
                #endregion

                string RESULT_DESC;
                //R21.2 Endpoint FBSSOrderServices.OrderService
                var endpointFbssOrderServices = (from l in _lov.Get() where l.LOV_NAME == "FBSS_ORDER_SERVICES" select l.LOV_VAL1).ToList().FirstOrDefault();

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.ServerCertificateValidationCallback = (s, certificate, chain, sslPolicyErrors) => true;

                using (var service = new FBSSAccessToken.CustomOrderService(accessToken))
                {
                    if (!string.IsNullOrEmpty(endpointFbssOrderServices)) service.Url = endpointFbssOrderServices;

                    result.RESULT = service.ReleaseTimeSlot(query.RESERVED_ID, out RESULT_DESC);
                }
                result.RESULT_DESC = RESULT_DESC;
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, "Success", "", "");
                return result;
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

                result.RESULT = "-1";
                result.RESULT_DESC = webEx.Message;
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, webEx.Message, log, "Failed", webEx.Message, "");
                return result;
            }
            catch (Exception ex)
            {
                result.RESULT = "-1";
                result.RESULT_DESC = ex.Message;
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex.Message, log, "Failed", ex.Message, "");
                return result;
            }
        }
    }
}
