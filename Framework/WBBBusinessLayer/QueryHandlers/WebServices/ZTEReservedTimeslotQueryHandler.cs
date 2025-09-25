using System;
using System.Linq;
using System.Net;
using System.Runtime.Caching;
using WBBBusinessLayer.Extension;
using WBBBusinessLayer.FBSSOrderServices;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.WebServices;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class ZTEReservedTimeslotQueryHandler : IQueryHandler<ZTEReservedTimeslotQuery, ZTEReservedTimeslotRespModel>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<FBB_CFG_LOV> _cfgLov;
        private readonly IQueryHandler<GetTokenFbbQuery, GetTokenFbbModel> _queryProcessor;
        private readonly MemoryCache cache = MemoryCache.Default;

        public ZTEReservedTimeslotQueryHandler(ILogger logger,
            IWBBUnitOfWork uow,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IEntityRepository<FBB_CFG_LOV> cfgLov,
            IQueryHandler<GetTokenFbbQuery, GetTokenFbbModel> queryProcessor)
        {
            _logger = logger;
            _uow = uow;
            _intfLog = intfLog;
            _cfgLov = cfgLov;
            _queryProcessor = queryProcessor;
        }

        public ZTEReservedTimeslotRespModel Handle(ZTEReservedTimeslotQuery query)
        {
            int countException = 0;
            InterfaceLogCommand log = null;
            ZTEReservedTimeslotRespModel result = new ZTEReservedTimeslotRespModel();

            log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.TRANSACTION_ID, "ZTEReservedTimeslot", "ZTEReservedTimeslotQueryHandler", query.ID_CARD_NO, "FBB|" + query.FullUrl, "");

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
                var logToken = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, accessToken, query.TRANSACTION_ID, "ZTEReservedTimeslotToken", "ZTEReservedTimeslotQueryHandlerToken", query.ID_CARD_NO, "FBB|" + query.FullUrl, "");
                
                if (!string.IsNullOrEmpty(accessToken))
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, "", logToken, "Success", "", "");
                }
                else
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, "", logToken, "Failed", "Access Token is Null", "");
                }
                #endregion

                string RESULT_DESC = "";
                string RESERVED_ID = "";
                if (query.TASK_TYPE == "MA")
                {
                    //R21.2 Endpoint TTComplainSheet.TTComplainSheet
                    var endpointFbssTTComplainsheet = (from l in _cfgLov.Get() where l.LOV_NAME == "FBSS_TT_COMPLAINSHEET" select l.LOV_VAL1).ToList().FirstOrDefault();

                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.ServerCertificateValidationCallback = (s, certificate, chain, sslPolicyErrors) => true;

                    //R24.10 Call Access Token FBSS
                    using (var service = new FBSSAccessToken.CustomTTComplainSheet(accessToken))
                    {
                        if (!string.IsNullOrEmpty(endpointFbssTTComplainsheet)) service.Url = endpointFbssTTComplainsheet;

                        string displayValue = "";
                        string SymptomCode = "";
                        if (query.PlayBoxCountOld == "1" && query.PlayBoxCountNew == "1" && query.ACCESS_MODE == "VDSL")
                        {
                            displayValue = "VDSL_EXT_1";
                        }
                        else if (query.PlayBoxCountOld == "1" && query.PlayBoxCountNew == "2" && query.ACCESS_MODE == "VDSL")
                        {
                            displayValue = "VDSL_EXT_2";
                        }
                        else if (query.PlayBoxCountOld == "2" && query.PlayBoxCountNew == "1" && query.ACCESS_MODE == "VDSL")
                        {
                            displayValue = "VDSL_EXT_1_2";
                        }
                        else if (query.PlayBoxCountOld == "1" && query.PlayBoxCountNew == "1" && query.ACCESS_MODE == "FTTH")
                        {
                            displayValue = "FTTH_EXT_1";
                        }
                        else if (query.PlayBoxCountOld == "1" && query.PlayBoxCountNew == "2" && query.ACCESS_MODE == "FTTH")
                        {
                            displayValue = "FTTH_EXT_2";
                        }
                        else if (query.PlayBoxCountOld == "2" && query.PlayBoxCountNew == "1" && query.ACCESS_MODE == "FTTH")
                        {
                            displayValue = "FTTH_EXT_1_2";
                        }

                        var LovConstant = from l in _cfgLov.Get()
                                          where l.DISPLAY_VAL == displayValue
                                          && l.LOV_TYPE == "FBB_CONSTANT"
                                          && l.LOV_NAME == "SYMPTOM_MULTIPLAYBOX"
                                          && l.ACTIVEFLAG == "Y"
                                          select l.LOV_VAL1;
                        if (LovConstant != null && LovConstant.Count() > 0)
                        {
                            SymptomCode = LovConstant.FirstOrDefault();
                        }

                        //R22.03 TopupReplace
                        if (query.PlayBoxCountOld == "0" && query.PlayBoxCountNew == "0")
                        {
                            SymptomCode = query.SymptonCodePBreplace;
                        }
                        //--------------------

                        string ServiceLevel = "";
                        if (query.ASSIGN_CONDITION_LIST != null && query.ASSIGN_CONDITION_LIST.Count > 0)
                        {
                            ServiceLevel = query.ASSIGN_CONDITION_LIST.FirstOrDefault(t => t.ATTR_NAME == "SERVICE_LEVEL").VALUE.ToSafeString();
                        }

                        result.RESULT = service.RESERVEDCAPACITY(
                            query.APPOINTMENT_DATE,
                            query.TIME_SLOT,
                            query.ACCESS_MODE,
                            query.PROD_SPEC_CODE,
                            query.ADDRESS_ID,
                            query.AisAirNumber,
                            query.AssignFlag,
                            SymptomCode,
                            ServiceLevel,
                            out RESULT_DESC,
                            out RESERVED_ID);
                    }
                }
                else
                {
                    //R21.2 Endpoint FBSSOrderServices.OrderService
                    var endpointFbssOrderServices = (from l in _cfgLov.Get() where l.LOV_NAME == "FBSS_ORDER_SERVICES" select l.LOV_VAL1).ToList().FirstOrDefault();

                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.ServerCertificateValidationCallback = (s, certificate, chain, sslPolicyErrors) => true;

                    //R24.10 Call Access Token FBSS
                    using (var service = new FBSSAccessToken.CustomOrderService(accessToken))
                    {
                        if (!string.IsNullOrEmpty(endpointFbssOrderServices)) service.Url = endpointFbssOrderServices;

                        //20.5 Service Level
                        assCondiAttr[] condiAttrArr = new assCondiAttr[query.ASSIGN_CONDITION_LIST.Count];
                        for (int i = 0; i < query.ASSIGN_CONDITION_LIST.Count; i++)
                        {
                            condiAttrArr[i] = new assCondiAttr()
                            {
                                ATTR_NAME = query.ASSIGN_CONDITION_LIST[i].ATTR_NAME,
                                VALUE = query.ASSIGN_CONDITION_LIST[i].VALUE
                            };
                        }

                        result.RESULT = service.ReservedTimeSlot(query.APPOINTMENT_DATE, query.TIME_SLOT, query.ACCESS_MODE, query.PROD_SPEC_CODE, query.ADDRESS_ID, query.LOCATION_CODE, query.SUBDISTRICT, query.POSTAL_CODE, query.ASSIGN_RULE, query.SUB_ACCESS_MODE, query.TASK_TYPE, condiAttrArr, out RESULT_DESC, out RESERVED_ID);
                    }
                }
                result.RESULT_DESC = RESULT_DESC;
                result.RESERVED_ID = RESERVED_ID;
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
