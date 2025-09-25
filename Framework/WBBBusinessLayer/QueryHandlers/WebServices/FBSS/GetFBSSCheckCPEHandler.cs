using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Caching;
using System.Web.Caching;
using System.Web.UI.WebControls;
using WBBBusinessLayer.Extension;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.WebServices;
using WBBContract.Queries.WebServices.FBSS;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Models;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.WebServices.FBSS
{
    public class GetFBSSCheckCPEHandler : IQueryHandler<GetFBSSCheckCPEQuery, List<FBSSCheckCPEModel>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_CFG_LOV> _cfgLov;
        private readonly IQueryHandler<GetTokenFbbQuery, GetTokenFbbModel> _queryProcessor;
        private readonly MemoryCache cache = MemoryCache.Default;

        public GetFBSSCheckCPEHandler(ILogger logger
            , IEntityRepository<FBB_INTERFACE_LOG> intfLog
            , IWBBUnitOfWork uow
            , IEntityRepository<FBB_CFG_LOV> cfgLov
            , IQueryHandler<GetTokenFbbQuery, GetTokenFbbModel> queryProcessor)
        {
            _logger = logger;
            _intfLog = intfLog;
            _uow = uow;
            _cfgLov = cfgLov;
            _queryProcessor = queryProcessor;
        }
        public List<FBSSCheckCPEModel> Handle(GetFBSSCheckCPEQuery query)
        {
            int countException = 0;
            List<FBSSCheckCPEModel> result = new List<FBSSCheckCPEModel>();

            InterfaceLogCommand log = null;

            //R21.2 Endpoint FBSSOrderServices.OrderService
            var endpointFbssOrderServices = (from l in _cfgLov.Get() where l.LOV_NAME == "FBSS_ORDER_SERVICES" select l.LOV_VAL1).ToList().FirstOrDefault();

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.ServerCertificateValidationCallback = (s, certificate, chain, sslPolicyErrors) => true;

            if (!string.IsNullOrEmpty(endpointFbssOrderServices)) query.EndpointService = endpointFbssOrderServices;

            log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.Transaction_Id, "CheckCPE", "GetFBSSCheckCPEHandler", query.IN_ID_CARD_NO, "FBB|" + query.FullUrl, "FbbFbssInterface");

            //log = FBSSExtensions.StartInterfaceFBSSLog(_uow, _intfLog, query, query.IN_ID_CARD_NO, "CheckCPE", "GetFBSSCheckCPEHandler");

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
                    var loveConfigList = _cfgLov.Get(lov => lov.ACTIVEFLAG.Equals("Y") && lov.LOV_TYPE.Equals("FBSS_Authen")).ToList();
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
                var logToken = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, accessToken, query.Transaction_Id, "CheckCPEToken", "GetFBSSCheckCPEHandlerToken", query.IN_ID_CARD_NO, "FBB|" + query.FullUrl, "FbbFbssInterface");
                
                if (!string.IsNullOrEmpty(accessToken))
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, "", logToken, "Success", "", "FbbFbssInterface");
                }
                else
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, "", logToken, "Failed", "Access Token is Null", "FbbFbssInterface");
                }
                #endregion

                List<FBSSOrderServices.CPE_SN_LIST> CPE_SN_LIST = new List<FBSSOrderServices.CPE_SN_LIST>();

                FBSSOrderServices.CPE_SN_LIST cpe = new FBSSOrderServices.CPE_SN_LIST() { SN = query.CPE };
                CPE_SN_LIST.Add(cpe);

                if (!string.IsNullOrEmpty(query.playbox))
                {
                    FBSSOrderServices.CPE_SN_LIST playbox = new FBSSOrderServices.CPE_SN_LIST() { SN = query.playbox };
                    CPE_SN_LIST.Add(playbox);
                }

                //R24.10 Call Access Token FBSS
                using (var service = new FBSSAccessToken.CustomOrderService(accessToken))
                {
                    //R21.2
                    service.Url = query.EndpointService;

                    var objResp = service.CheckCPE(CPE_SN_LIST.ToArray())
                        .Select(t => new FBSSCheckCPEModel
                        {
                            Check_Result_Code = t.CHECK_RESULT_CODE,
                            Check_Result_Desc = t.CHECK_RESULT_DESC,
                            SN = t.SN,
                            Status_ID = t.STATUS,
                            STATUS_DESC = t.STATUS_DESC,
                            CPE_MAC_ADDR = t.CPE_MAC_ADDR,
                            CPE_TYPE = t.CPE_TYPE,
                            CPE_COMPANY_CODE = t.CPE_COMPANY_CODE,
                            CPE_PLANT = t.CPE_PLANT,
                            CPE_STORAGE_LOCATION = t.CPE_STORAGE_LOCATION,
                            CPE_MATERIAL_CODE = t.CPE_MATERIAL_CODE,
                            SN_PATTERN = t.SN_PATTERN,

                            //R20.4 Update Field
                            CPE_MODEL_NAME = t.CPE_MODEL_NAME,
                            REGISTER_DATE = t.REGISTER_DATE,
                            FIBRENET_ID = t.FIBRENET_ID,
                            SHIP_TO = t.SHIP_TO,
                            WARRANTY_START_DATE = t.WARRANTY_START_DATE,
                            WARRANTY_END_DATE = t.WARRANTY_END_DATE,
                            MAC_ADDRESS = t.MAC_ADDRESS
                        }).ToList();
                    result = objResp;
                }


                //FBSSExtensions.EndInterfaceFBSSLog(_uow, _intfLog, result, log, "Success", "");
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, "Success", "", "FbbFbssInterface");
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

                if (log != null)
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, new SFFServices.SffRequest(), log, "Failed", webEx.Message, "FbbFbssInterface");
                }

                throw webEx;
            }
            catch (Exception ex)
            {
                if (null != log)
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, new SFFServices.SffRequest(), log, "Failed", ex.Message, "FbbFbssInterface");
                }

                throw ex;
            }

            return result;
        }

    }
}
