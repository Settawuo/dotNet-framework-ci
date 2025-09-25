using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Caching;
using System.Web.Caching;
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
    public class GetFBSSQueryCPEPenaltyHandler : IQueryHandler<GetFBSSQueryCPEPenaltyQuery, List<FBSSQueryCPEPenaltyModel>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_CFG_LOV> _cfgLov;
        private readonly IQueryHandler<GetTokenFbbQuery, GetTokenFbbModel> _queryProcessor;
        private readonly MemoryCache cache = MemoryCache.Default;

        public GetFBSSQueryCPEPenaltyHandler(ILogger logger,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IWBBUnitOfWork uow,
            IEntityRepository<FBB_CFG_LOV> cfgLov,
            IQueryHandler<GetTokenFbbQuery, GetTokenFbbModel> queryProcessor)
        {
            _logger = logger;
            _intfLog = intfLog;
            _uow = uow;
            _cfgLov = cfgLov;
            _queryProcessor = queryProcessor;
        }
        public List<FBSSQueryCPEPenaltyModel> Handle(GetFBSSQueryCPEPenaltyQuery query)
        {
            int countException = 0;
            List<FBSSQueryCPEPenaltyModel> result = new List<FBSSQueryCPEPenaltyModel>();

            InterfaceLogCommand log = null;
            log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.TransactionId, "QueryCPEPenalty", "GetFBSSQueryCPEPenaltyHandler", "", "FBB|" + query.FullUrl, "WEB");

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
                var logToken = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, accessToken, query.TransactionId, "QueryCPEPenaltyToken", "GetFBSSQueryCPEPenaltyHandlerToken", "FBB", "FBB|" + query.FullUrl, "WEB");
                
                if (!string.IsNullOrEmpty(accessToken))
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, "", logToken, "Success", "", "");
                }
                else
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, "", logToken, "Failed", "Access Token is Null", "");
                }
                #endregion

                string result_desp = "";
                TTComplainSheet.cpePenalty[] CPE_INFO_LIST = null;

                //R21.2 Endpoint TTComplainSheet.TTComplainSheet
                var endpointFbssTTComplainsheet = (from l in _cfgLov.Get() where l.LOV_NAME == "FBSS_TT_COMPLAINSHEET" select l.LOV_VAL1).ToList().FirstOrDefault();

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.ServerCertificateValidationCallback = (s, certificate, chain, sslPolicyErrors) => true;

                //R24.10 Call Access Token FBSS
                using (var service = new FBSSAccessToken.CustomTTComplainSheet(accessToken))
                {
                    if (!string.IsNullOrEmpty(endpointFbssTTComplainsheet)) service.Url = endpointFbssTTComplainsheet;

                    var objResp = service.queryCPEPenalty(query.OPTION, query.FIBRENET_ID, query.SERIAL_NO, query.STATUS, query.MAC_ADDRESS,
                         out result_desp, out CPE_INFO_LIST);

                    if (CPE_INFO_LIST != null && CPE_INFO_LIST.Count() > 0)
                    {
                        foreach (var cpeInfo in CPE_INFO_LIST)
                        {
                            FBSSQueryCPEPenaltyModel CPEPenalty = new FBSSQueryCPEPenaltyModel();

                            CPEPenalty.SERIAL_NO = cpeInfo.SERIAL_NO;
                            CPEPenalty.STATUS = cpeInfo.STATUS;
                            CPEPenalty.STATUS_DESC = cpeInfo.STATUS_DESC;
                            CPEPenalty.CPE_TYPE = cpeInfo.CPE_TYPE;
                            CPEPenalty.CPE_MODEL_NAME = cpeInfo.CPE_MODEL_NAME;
                            CPEPenalty.COMPANY_CODE = cpeInfo.COMPANY_CODE;
                            CPEPenalty.PLANT = cpeInfo.PLANT;
                            CPEPenalty.STORAGE_LOCATION = cpeInfo.STORAGE_LOCATION;
                            CPEPenalty.MATERIAL_CODE = cpeInfo.MATERIAL_CODE;
                            CPEPenalty.PENALTY = cpeInfo.PENALTY;
                            CPEPenalty.PENALTY_PRODUCT_CODE = cpeInfo.PENALTY_PRODUCT_CODE;
                            CPEPenalty.OPER_DATE = cpeInfo.OPER_DATE;
                            CPEPenalty.CPE_BRAND_NAME = cpeInfo.CPE_BRAND_NAME;
                            CPEPenalty.CPE_GROUP_TYPE = cpeInfo.CPE_GROUP_TYPE;
                            CPEPenalty.REGISTER_DATE = cpeInfo.REGISTER_DATE;
                            CPEPenalty.CPE_MODEL_ID = cpeInfo.CPE_MODEL_ID;
                            CPEPenalty.FIBRENET_ID = cpeInfo.FIBRENET_ID;
                            CPEPenalty.SN_PATTERN = cpeInfo.SN_PATTERN;
                            CPEPenalty.SHIP_TO = cpeInfo.SHIP_TO;
                            CPEPenalty.WARRANTY_START_DATE = cpeInfo.WARRANTY_START_DATE;
                            CPEPenalty.WARRANTY_END_DATE = cpeInfo.WARRANTY_END_DATE;
                            CPEPenalty.MAC_ADDRESS = cpeInfo.MAC_ADDRESS;

                            result.Add(CPEPenalty);
                        }
                    }

                }

                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, "Success", string.Empty, "");
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

                _logger.Info("GetFBSSQueryCPEPenaltyHandler : Error.");
                _logger.Info(webEx.Message);
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, "", log, "Error", webEx.StackTrace, "");
                throw;
            }
            catch (Exception ex)
            {
                _logger.Info("GetFBSSQueryCPEPenaltyHandler : Error.");
                _logger.Info(ex.Message);
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, "", log, "Error", ex.StackTrace, "");
                throw;
            }

            return result;
        }

    }
}
