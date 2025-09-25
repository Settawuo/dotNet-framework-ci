using System;
using System.Linq;
using System.Net;
using System.Runtime.Caching;
using System.Text.RegularExpressions;
using WBBBusinessLayer.Extension;
using WBBContract;
using WBBContract.Queries.WebServices;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Models;
using WBBEntity.PanelModels;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    //TODO: Splitter Management
    public class ZTEResReservedQueryHandler : IQueryHandler<ZTEResReservedQuery, ZTEResReservedModel>
    {
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<FBB_CFG_LOV> _cfgLovRepository;
        private readonly IQueryHandler<GetTokenFbbQuery, GetTokenFbbModel> _queryProcessor;
        private readonly MemoryCache cache = MemoryCache.Default;


        public ZTEResReservedQueryHandler(IWBBUnitOfWork uow,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog, IEntityRepository<FBB_CFG_LOV> cfgLovRepository, IQueryHandler<GetTokenFbbQuery, GetTokenFbbModel> queryProcessor)
        {
            _uow = uow;
            _intfLog = intfLog;
            _cfgLovRepository = cfgLovRepository;
            _queryProcessor = queryProcessor;

        }

        public ZTEResReservedModel Handle(ZTEResReservedQuery query)
        {
            int countException = 0;
            var rtnResQueryModel = new ZTEResReservedModel();
            var zteResReservedModel = new ZTEResReserved();

            var log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.TRANSACTION_ID, "ResReserved", "OrderService", null, "FBB", "");

            repeat:
            try
            {
                //Mock up
                //var resultLov = _cfgLovRepository.Get().FirstOrDefault(item => item.LOV_NAME == "TEST_RESERVED_CASE") ?? new FBB_CFG_LOV();
                //zteResReservedModel.RETURN_CODE = resultLov.LOV_VAL2;
                //zteResReservedModel.RESULT_DISCRIPTION = resultLov.LOV_VAL1;
                //zteResReservedModel.RES_RESERVATION_ID = resultLov.LOV_VAL3;

                //if (zteResReservedModel.RETURN_CODE == "0")
                //{
                //    rtnResQueryModel.RETURN_CASECODE = "1";
                //    rtnResQueryModel.RESERVATION_ID = zteResReservedModel.RES_RESERVATION_ID;
                //}
                //else
                //{
                //    var resultCase = _cfgLovRepository.Get().FirstOrDefault(item => item.LOV_VAL1 == zteResReservedModel.RESULT_DISCRIPTION) ?? new FBB_CFG_LOV();
                //    switch (resultCase.LOV_VAL2)
                //    {
                //        case "2":
                //            rtnResQueryModel.RETURN_CASECODE = resultCase.LOV_VAL2;
                //            break;

                //        default:
                //            rtnResQueryModel.RETURN_CASECODE = "3";
                //            break;
                //    }
                //}
                //InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, zteResReservedModel, log, "Success", "", "");

                //return rtnResQueryModel;

                // Release

                #region R24.10 Call Access Token FBSS
                string accessToken = string.Empty;
                string channel = FBSSAccessToken.channelFBB.ToUpper();
                accessToken = (string)cache.Get(channel); //Get cache

                if (string.IsNullOrEmpty(accessToken))
                {
                    string clientId = string.Empty;
                    string clientSecret = string.Empty;
                    string grantType = string.Empty;
                    var loveConfigList = _cfgLovRepository.Get(lov => lov.ACTIVEFLAG.Equals("Y") && lov.LOV_TYPE.Equals("FBSS_Authen")).ToList();
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
                var logToken = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, accessToken, query.TRANSACTION_ID, "ResReservedToken", "OrderServiceToken", null, "FBB", "");
                
                if (!string.IsNullOrEmpty(accessToken))
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, "", logToken, "Success", "", "");
                }
                else
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, "", logToken, "Failed", "Access Token is Null", "");
                }
                #endregion

                FBSSOrderServices.splitList[] listSpliter;
                if (query.LISTOFSPLITTER != null && query.LISTOFSPLITTER.Count() > 0)
                {
                    listSpliter = new FBSSOrderServices.splitList[query.LISTOFSPLITTER.Length];

                    var i = 0;

                    foreach (var spliter in query.LISTOFSPLITTER.Select(spliterquery => new FBSSOrderServices.splitList
                    {
                        SPLIT_DISTANCE = spliterquery.Distance.ToString(),
                        SPLIT_NO = spliterquery.Splitter_Name,
                        SPLIT_SEQ = (i + 1).ToString()
                    }))
                    {
                        listSpliter[i] = spliter;
                        i++;
                    }
                }
                else
                {
                    listSpliter = null;
                }

                FBSSOrderServices.dslamList[] listDslam;
                if (query.LISTOFDSLAM != null && query.LISTOFDSLAM.Count() > 0)
                {
                    listDslam = new FBSSOrderServices.dslamList[query.LISTOFDSLAM.Length];

                    var i = 0;

                    foreach (var dslam in query.LISTOFDSLAM.Select(dslamquery => new FBSSOrderServices.dslamList
                    {
                        DSLAM_PORT = "",
                        DSLAM_NO = dslamquery.Dslam_Name,
                        DSLAM_SEQ = (i + 1).ToString()
                    }))
                    {
                        listDslam[i] = dslam;
                        i++;
                    }
                }
                else
                {
                    listDslam = null;
                }

                //R21.2 Endpoint FBSSOrderServices.OrderService
                var endpointFbssOrderServices = (from l in _cfgLovRepository.Get() where l.LOV_NAME == "FBSS_ORDER_SERVICES" select l.LOV_VAL1).ToList().FirstOrDefault();

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.ServerCertificateValidationCallback = (s, certificate, chain, sslPolicyErrors) => true;

                using (var service = new FBSSAccessToken.CustomOrderService(accessToken))
                {
                    if (!string.IsNullOrEmpty(endpointFbssOrderServices)) service.Url = endpointFbssOrderServices;

                    string resultDiscription;
                    string resReservationId;
                    var resultResReserved = service.ResReserved(query.PRODUCT, listSpliter, listDslam, query.PHONE_FLAG, query.ADDRESS_ID, out resultDiscription, out resReservationId);

                    zteResReservedModel.RETURN_CODE = resultResReserved;
                    zteResReservedModel.RESULT_DISCRIPTION = resultDiscription;
                    zteResReservedModel.RES_RESERVATION_ID = resReservationId;

                    if (zteResReservedModel.RETURN_CODE == "0")
                    {
                        rtnResQueryModel.RETURN_CASECODE = "1";
                        rtnResQueryModel.RESERVATION_ID = zteResReservedModel.RES_RESERVATION_ID;
                    }
                    else
                    {
                        //var resultCase = _cfgLovRepository.Get().FirstOrDefault(item => item.LOV_NAME == "ResQuery" && item.LOV_VAL1 == zteResReservedModel.RESULT_DISCRIPTION) ?? new FBB_CFG_LOV();
                        var result = from lov in _cfgLovRepository.Get()
                                     where lov.LOV_NAME == "ResQuery" && lov.LOV_VAL1 == zteResReservedModel.RESULT_DISCRIPTION
                                     select lov;
                        var resultCase = result.FirstOrDefault() ?? new FBB_CFG_LOV();
                        switch (resultCase.LOV_VAL2)
                        {
                            case "2":
                                rtnResQueryModel.RETURN_CASECODE = resultCase.LOV_VAL2;
                                break;

                            default:
                                rtnResQueryModel.RETURN_CASECODE = "3";
                                break;
                        }
                    }
                    rtnResQueryModel.RETURN_CODE = zteResReservedModel.RETURN_CODE;
                }

                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, zteResReservedModel, log, "Success", "", "");
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
                rtnResQueryModel.RETURN_CASECODE = "3";
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, rtnResQueryModel.RETURN_CASECODE + "Error Mesage :" + webEx.Message, log, "Failed", webEx.Message, "");
            }
            catch (Exception ex)
            {
                rtnResQueryModel.RETURN_CASECODE = "3";
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, rtnResQueryModel.RETURN_CASECODE + "Error Mesage :"+ex.Message, log, "Failed", ex.Message, "");
            }

            return rtnResQueryModel;
        }
    }
}
