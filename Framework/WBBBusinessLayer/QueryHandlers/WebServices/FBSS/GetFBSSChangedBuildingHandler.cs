using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Caching;
using System.Text.RegularExpressions;
using System.Web.Caching;
using WBBBusinessLayer.Extension;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.WebServices;
using WBBContract.Queries.WebServices.FBSS;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class GetFBSSChangedBuildingHandler
        : IQueryHandler<GetFBSSChangedBuilding, List<FBSSChangedAddressInfo>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<FBB_CFG_LOV> _lov;
        private readonly IWBBUnitOfWork _uow;
        private readonly MemoryCache cache = MemoryCache.Default;
        private readonly IQueryHandler<GetTokenFbbQuery, GetTokenFbbModel> _queryProcessor;

        public GetFBSSChangedBuildingHandler(ILogger logger,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IEntityRepository<FBB_CFG_LOV> lov,
            IWBBUnitOfWork uow,
            IQueryHandler<GetTokenFbbQuery, GetTokenFbbModel> queryProcessor)
        {
            _logger = logger;
            _intfLog = intfLog;
            _lov = lov;
            _uow = uow;
            _queryProcessor = queryProcessor;
        }

        public List<FBSSChangedAddressInfo> Handle(GetFBSSChangedBuilding query)
        {
            int countException = 0;
            InterfaceLogCommand log = null;

            var resultCode = "";
            var resultDesc = "";

            var changedAddressInfoList = new List<FBSSChangedAddressInfo>();

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
                var logToken = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, accessToken, "", "queryChangedBuildingToken", "GetFBSSChangedBuildingHandlerToken", null, "FBB", "");
                
                if (!string.IsNullOrEmpty(accessToken))
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, "", logToken, "Success", "", "FBB");
                }
                else
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, "", logToken, "Failed", "Access Token is Null", "FBB");
                }
                #endregion
                var latestQueryDate = (from t in _lov.Get()
                                       where t.LOV_TYPE == "SCREEN"
                                        && t.LOV_NAME == "LASTDATE_BATCH_BV"
                                       select t).FirstOrDefault();

                if (null == latestQueryDate)
                {
                    _logger.Info("Config Not Found : BATCH LASTDATE_BATCH_BV");
                    return new List<FBSSChangedAddressInfo>();
                }

                query.StartDate = latestQueryDate.LOV_VAL2;
                query.EndDate = DateTime.Now.ToDisplayText(Constants.FBSSDateFormats.DateFormat);
                //log = FBSSExtensions.StartInterfaceFBSSLog(_uow, _intfLog, query,
                //"", "queryChangedBuilding", "GetFBSSChangedBuildingHandler");
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, "", "queryChangedBuilding", "GetFBSSChangedBuildingHandler", null, "FBB", "");

                //R21.2 Endpoint FBSSOrderServices.OrderService
                var endpointFbssOrderServices = (from l in _lov.Get() where l.LOV_NAME == "FBSS_ORDER_SERVICES" select l.LOV_VAL1).ToList().FirstOrDefault();

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.ServerCertificateValidationCallback = (s, certificate, chain, sslPolicyErrors) => true;

                using (var service = new FBSSAccessToken.CustomOrderService(accessToken))
                {
                    if (!string.IsNullOrEmpty(endpointFbssOrderServices)) service.Url = endpointFbssOrderServices;
                    changedAddressInfoList = service
                        .queryChangedBuilding(
                            query.StartDate,
                            query.EndDate,
                            out resultCode,
                            out resultDesc)
                        .Select(t => new FBSSChangedAddressInfo
                        {
                            AddressId = t.ADDRESS_ID.ToSafeString(),
                            AddressIdSpecified = t.ADDRESS_IDSpecified,
                            AddressType = t.ADDRESS_TYPE,//.ParseEnum<FBSSAddressType>(),
                            BuildingName = t.BUILDING_NAME,
                            BuildingNo = t.BUILDING_NO,
                            ChangedAction = t.ACTION,//.ParseEnum<FBSSChangeAddressAction>(),
                            Language = t.LANGUAGE,//.ParseEnum<FBSSLanguageFlag>(),
                            PostalCode = t.POSTAL_CODE,
                            SUBDISTRICT = t.SUB_DISTRICT,
                            ACCESS_MODE = t.ACCESS_MODE,
                            SITE_CODE = t.SITE_CODE,
                            PARTNER = t.PARTNER_NAME,
                            LATITUDE = t.LATITUDE,
                            LONGTITUDE = t.LONGTITUDE,

                            //R21.1
                            FTTR_FLAG = t.FTTR_FLAG,
                            SPECIFIC_TEAM_1 = t.SPECIFIC_TEAM_1,
                            SPECIFIC_TEAM_2 = t.SPECIFIC_TEAM_2

                        })
                        .Where(a => !string.IsNullOrEmpty(a.Language))
                        .ToList();
                }

                //FBSSExtensions.EndInterfaceFBSSLog(_uow, _intfLog, changedAddressInfoList, log,
                //        "Success", resultCode + " : " + resultDesc);
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, changedAddressInfoList, log, "Success", resultCode + " : " + resultDesc, "");

                // update lov
                latestQueryDate.LOV_VAL2 = DateTime.Now.ToDisplayText(Constants.FBSSDateFormats.DateFormat);
                _lov.Update(latestQueryDate);
                _uow.Persist();

                return changedAddressInfoList
                            .Where(t => !string.IsNullOrEmpty(t.Language)
                                            && !string.IsNullOrEmpty(t.AddressType)
                                            && !string.IsNullOrEmpty(t.PostalCode)).ToList();
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

                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, "", log, "Failed", webEx.Message, "");

                if (changedAddressInfoList.Any())
                {
                    return changedAddressInfoList;
                }

                throw webEx;
            }
            catch (Exception ex)
            {
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, "", log, "Failed", ex.Message, "");

                if (changedAddressInfoList.Any())
                {
                    return changedAddressInfoList;
                }

                throw ex;
            }
        }
    }
}