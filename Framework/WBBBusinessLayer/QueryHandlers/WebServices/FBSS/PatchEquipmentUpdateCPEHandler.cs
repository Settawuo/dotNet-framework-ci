using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Caching;
using System.Web.Caching;
using System.Web.Configuration;
using WBBBusinessLayer.Extension;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.PatchEquipment;
using WBBContract.Queries.WebServices;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Models;

namespace WBBBusinessLayer.QueryHandlers.WebServices.FBSS
{
    public class PatchEquipmentUpdateCPEHandler : IQueryHandler<UpdateCPE, UpdateCPEResponse>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_CFG_LOV> _cfgLov;
        private readonly MemoryCache cache = MemoryCache.Default;
        private readonly IQueryHandler<GetTokenFbbQuery, GetTokenFbbModel> _queryProcessor;

        public PatchEquipmentUpdateCPEHandler(ILogger logger
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
        public UpdateCPEResponse Handle(UpdateCPE query)
        {
            var accessToken = string.Empty;
            int countException = 0;
            UpdateCPEResponse result = new UpdateCPEResponse();

            InterfaceLogCommand log = null;

            var EndpointService = "";
            var endpointFbssOrderServices = (from l in _cfgLov.Get() where l.LOV_NAME == "FBSS_ORDER_SERVICES" select l.LOV_VAL1).ToList().FirstOrDefault();

            if (!string.IsNullOrEmpty(endpointFbssOrderServices)) EndpointService = endpointFbssOrderServices;

            log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, "0", "UpdateCPE", "PatchEquipmentUpdateCPEHandler", query.ACTION, "FBB|UpdateCPE", "FbbFbssInterface");
            
            repeat:
            try
            {
                #region Call Access Token FBSS
                var rollbackCallAPIToken = _cfgLov
                        .Get(lov => lov.ACTIVEFLAG.Equals("Y") && lov.LOV_TYPE.Equals("FBBPAYGPATCH_EQUIPMENT_BATCH") && lov.LOV_NAME.Equals("ROLLBACK_TOKEN"))
                        .FirstOrDefault();

                if (rollbackCallAPIToken?.DISPLAY_VAL != "Y")
                {
                    var channel = _cfgLov
                        .Get(lov => lov.ACTIVEFLAG.Equals("Y") && lov.LOV_TYPE.Equals("FBSS_Authen") && lov.LOV_NAME.Equals("PAYG"))
                        .FirstOrDefault()?.LOV_NAME;

                    accessToken = (string)cache.Get(channel);

                    if (string.IsNullOrEmpty(accessToken))
                    {
                        var configList = _cfgLov
                            .Get(lov => lov.ACTIVEFLAG.Equals("Y") && lov.LOV_TYPE.Equals("FBSS_Authen") && lov.LOV_NAME.Equals("PAYG"))
                            .ToList();

                        if (configList.Any())
                        {
                            var config = configList.First();
                            var clientId = config.LOV_VAL1;
                            var clientSecret = config.LOV_VAL2;

                            var getToken = new GetTokenFbbQuery()
                            {
                                Channel = channel,
                                ParamGetoken = new ParametersGetoken()
                                {
                                    client_id = clientId,
                                    client_secret = clientSecret,
                                    grant_type = null
                                }
                            };

                            var responseGetToken = _queryProcessor.Handle(getToken);
                            accessToken = (string)cache.Get(channel);
                        }
                    }
                }

                //log access token
                InterfaceLogCommand logGetToken = null;
                logGetToken = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, accessToken, "0", "UpdateCPECheckGetToken", "PatchEquipmentUpdateCPEHandler", "ROLLBACK_TOKEN is " + rollbackCallAPIToken?.DISPLAY_VAL, "FBB|UpdateCPE", "FbbFbssInterface");
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, "", logGetToken, "Success", "", "FbbFbssInterface");
                #endregion

                List<FBSSOrderServices.cpeList> CPE_LIST = new List<FBSSOrderServices.cpeList>();

                FBSSOrderServices.cpeList cpe = new FBSSOrderServices.cpeList();

                cpe.SERIAL_NO = query.CPE_LIST.SERIAL_NO;
                cpe.STATUS = query.CPE_LIST.STATUS;
                cpe.SN_PATTERN = query.CPE_LIST.SN_PATTERN;
                cpe.LOCATION = query.CPE_LIST.LOCATION;
                cpe.PLANT = query.CPE_LIST.PLANT;
                cpe.ACCESS_NO = query.CPE_LIST.ACCESS_NO;
                cpe.SERVICE_NAME = query.CPE_LIST.SERVICE_NAME;
                cpe.DEVICE_TYPE = query.CPE_LIST.DEVICE_TYPE;
                cpe.DESCRIPTION = query.CPE_LIST.DESCRIPTION;
                cpe.MATERIAL = query.CPE_LIST.MATERIAL;
                cpe.CREATE_BY = query.CPE_LIST.CREATE_BY;
                cpe.CREATE_DATE = query.CPE_LIST.CREATE_DATE;
                CPE_LIST.Add(cpe);

                FBSSOrderServices.resultList[] RET_CPE_LIST;

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.ServerCertificateValidationCallback = (s, certificate, chain, sslPolicyErrors) => true;

                using (var service = new FBSSAccessToken.CustomOrderService(accessToken))
                {
                    service.Url = EndpointService;

                    var objResp = service.UpdateCPE(query.ACTION, CPE_LIST.ToArray(), out RET_CPE_LIST);

                    result = RET_CPE_LIST.Select(item => new UpdateCPEResponse()
                    {
                        CASECODE = objResp,
                        RESULT_LIST = new RESULT_LIST()
                        {
                            SERIAL_NO = item.SERIAL_NO,
                            OPERATION_RESULT = item.OPERATION_RESULT,
                            RESULT_DESCRIPTION = item.RESULT_DESCRIPTION
                        }
                    }).FirstOrDefault();

                }
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, "Success", "", "FbbFbssInterface");
            }
            catch (WebException webEx)
            {
                //Call Access Token FBSS Exception 
                if ((webEx.Response is HttpWebResponse response && (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)) && (countException == 0))
                {
                   var channel = _cfgLov
                        .Get(lov => lov.ACTIVEFLAG.Equals("Y") && lov.LOV_TYPE.Equals("FBSS_Authen") && lov.LOV_NAME.Equals("PAYG"))
                        .FirstOrDefault()?.LOV_NAME;

                    countException++;
                    cache.Remove(channel);
                    webEx = null;
                    goto repeat;
                }
                result = null;
                if (log != null)
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, new SFFServices.SffRequest(), log, "Failed", webEx.Message, "FbbFbssInterface");
                }
            }
            catch (Exception ex)
            {
                result = null;
                if (null != log)
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, new SFFServices.SffRequest(), log, "Failed", ex.Message, "FbbFbssInterface");
                }
            }

            return result;
        }

    }
}
