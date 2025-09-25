using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Caching;
using WBBBusinessLayer.Extension;
using WBBBusinessLayer.FBSSOrderServices;
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
    public class GetFBSSFeasibilityCheckHandler
        : IQueryHandler<GetFBSSFeasibilityCheck, FBSSCoverageResult>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<FBB_CFG_LOV> _lov;
        private readonly IWBBUnitOfWork _uow;
        private readonly IQueryHandler<GetTokenFbbQuery, GetTokenFbbModel> _queryProcessor;
        private readonly MemoryCache cache = MemoryCache.Default;

        public GetFBSSFeasibilityCheckHandler(ILogger logger,
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

        public FBSSCoverageResult Handle(GetFBSSFeasibilityCheck query)
        {
            int countException = 0;
            var addressId = "";
            addressModeInfo[] accessmodeList = null;
            planSite planingSite = null;
            var isPartner = "";
            var partneName = "";
            var resultCode = "";
            var resultDesc = "";

            InterfaceLogCommand log = null;
            //log = FBSSExtensions.StartInterfaceFBSSLog(_uow, _intfLog, query,
            //    "", "feasibilityCheck", "GetFBSSFeasibilityCheckHandler");
            log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.TransactionId, "feasibilityCheck", "GetFBSSFeasibilityCheckHandler", null, "FBB|" + query.FullUrl, "");

            var coverageResult = new FBSSCoverageResult();

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
                var logToken = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, accessToken, query.TransactionId, "feasibilityCheckToken", "GetFBSSFeasibilityCheckHandlerToken", "FBB", "FBB", "FBB");
                
                if (!string.IsNullOrEmpty(accessToken))
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, "", logToken, "Success", "", "");
                }
                else
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, "", logToken, "Failed", "Access Token is Null", "");
                }
                #endregion

                //R21.2 Endpoint FBSSOrderServices.OrderService
                var endpointFbssOrderServices = (from l in _lov.Get() where l.LOV_NAME == "FBSS_ORDER_SERVICES" select l.LOV_VAL1).ToList().FirstOrDefault();

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.ServerCertificateValidationCallback = (s, certificate, chain, sslPolicyErrors) => true;

                //R24.10 Call Access Token FBSS
                using (var service = new FBSSAccessToken.CustomOrderService(accessToken))
                {
                    if (!string.IsNullOrEmpty(endpointFbssOrderServices)) service.Url = endpointFbssOrderServices;

                    var result = service.feasibilityCheck(
                        query.AddressType.ToSafeString().ParseEnum<addressType>(),
                        true,
                        query.PostalCode.ToSafeString(),
                        query.SubDistricName.ToSafeString(),
                        query.Language.ToSafeString().ParseEnum<language>(),
                        true,
                        query.BuildingName.ToSafeString().ToUpperInvariant(),
                        query.BuildingNo.ToSafeString().ToUpperInvariant(),
                        query.PhoneFlag.ToSafeString().ParseEnum<yn>(),
                        true,
                        query.FloorNo.ToSafeString(),
                        query.Latitude.ToSafeDecimal(), // changed
                        query.Latitude.ToSafeDecimal() > 0,
                        query.Longitude.ToSafeDecimal(), // changed
                        query.Longitude.ToSafeDecimal() > 0,
                        query.UnitNo.ToSafeString(),
                        out addressId,
                        out accessmodeList,
                        out planingSite,
                        out isPartner,
                        out partneName,
                        out resultCode,
                        out resultDesc);

                    //coverageResult = new FBSSCoverageResult
                    //{
                    //    AccessModeList = accessmodeList
                    //    .Select(t => new FBSSAccessModeInfo
                    //    {
                    //        AccessMode = (t == null ? "" : t.ACCESS_MODE),
                    //        ResourceList = null, 
                    //        InserviceDate = DateTime.Now,
                    //    }).ToList(),

                    //    AddressId = addressId,
                    //    Coverage = (string.IsNullOrEmpty(result) ? "NO" : result),
                    //    IsPartner = isPartner,
                    //    PartnerName = partneName,
                    //    PlanningSite = new FBSSAccessModeInfo
                    //    {
                    //        AccessMode = (planingSite == null ? "" : planingSite.ACCESS_MODE),
                    //        InserviceDate = (planingSite == null ? new DateTime() : planingSite.INSERVICE_DATE.ToFBSSDate()),
                    //    },
                    //};

                    /// new bind Data

                    coverageResult.AddressId = addressId;
                    coverageResult.Coverage = (string.IsNullOrEmpty(result) ? "NO" : result);
                    coverageResult.IsPartner = isPartner;
                    coverageResult.PartnerName = partneName;
                    coverageResult.PlanningSite = new FBSSAccessModeInfo
                    {
                        AccessMode = (planingSite == null ? "" : planingSite.ACCESS_MODE),
                        InserviceDate = (planingSite == null ? new DateTime() : planingSite.INSERVICE_DATE.ToFBSSDate()),
                    };
                    coverageResult.AccessModeList = new List<FBSSAccessModeInfo>();
                    if (accessmodeList != null && accessmodeList.Count() > 0)
                    {
                        foreach (var accessmodeitem in accessmodeList)
                        {
                            FBSSAccessModeInfo fBSSAccessModeInfo = new FBSSAccessModeInfo();
                            fBSSAccessModeInfo.AccessMode = accessmodeitem.ACCESS_MODE;
                            fBSSAccessModeInfo.InserviceDate = DateTime.Now;
                            fBSSAccessModeInfo.ResourceList = new List<ResourceInfo>();
                            if (accessmodeitem.LIST_OF_RESOURCES != null && accessmodeitem.LIST_OF_RESOURCES.Count() > 0)
                            {
                                foreach (var item in accessmodeitem.LIST_OF_RESOURCES)
                                {
                                    ResourceInfo Resource = new ResourceInfo();
                                    Resource.ResourceName = item.ResourceName;
                                    fBSSAccessModeInfo.ResourceList.Add(Resource);
                                }
                            }
                            coverageResult.AccessModeList.Add(fBSSAccessModeInfo);
                        }
                    }

                    //FBSSExtensions.EndInterfaceFBSSLog(_uow, _intfLog, coverageResult, log,
                    //  "Success", resultCode + ": " + resultDesc);
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, coverageResult, log, "Success", resultCode + ": " + resultDesc, "");

                    query.InterfaceLogId = log.OutInterfaceLogId;
                    coverageResult.InterfaceLogId = log.OutInterfaceLogId;

                    return coverageResult;
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

                if (null != log)
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, "", log, "Failed", webEx.Message, "");
                }

                throw webEx;
            }
            catch (Exception ex)
            {
                if (null != log)
                {
                    //FBSSExtensions.EndInterfaceFBSSLog(_uow, _intfLog, "", log,
                    //    "Failed", ex.Message);
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, "", log, "Failed", ex.Message, "");
                }

                throw ex;
            }
        }
    }
}
