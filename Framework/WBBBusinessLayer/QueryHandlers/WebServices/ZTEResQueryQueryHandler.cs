using System;
using System.Linq;
using System.Net;
using System.Runtime.Caching;
using System.Text.RegularExpressions;
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
    //TODO: Splitter Management
    public class ZTEResQueryQueryHandler : IQueryHandler<ZTEResQueryQuery, ZTEResQueryModel>
    {
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<FBB_CFG_LOV> _cfgLovRepository;
        private readonly IEntityRepository<FBB_INTERFACE_LOG_3BB> _intfLog3bb;
        private readonly IQueryHandler<GetTokenFbbQuery, GetTokenFbbModel> _queryProcessor;
        private readonly MemoryCache cache = MemoryCache.Default;

        public ZTEResQueryQueryHandler(IWBBUnitOfWork uow,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IEntityRepository<FBB_CFG_LOV> cfgLovRepository,
            IEntityRepository<FBB_INTERFACE_LOG_3BB> intfLog3bb,
            IQueryHandler<GetTokenFbbQuery, GetTokenFbbModel> queryProcessor)
        {
            _uow = uow;
            _intfLog = intfLog;
            _cfgLovRepository = cfgLovRepository;
            _intfLog3bb = intfLog3bb;
            _queryProcessor = queryProcessor;
        }

        public ZTEResQueryModel Handle(ZTEResQueryQuery query)
        {
            int countException = 0;
            InterfaceLogCommand log = null;
            InterfaceLogCommand log2 = null;
            InterfaceLog3BBCommand log3bb = null;
            InterfaceLog3BBCommand log3bb2 = null;
            var zteReQueryModel = new ZTEResQueryModel();

            if (query.FullUrl == "3BB") log3bb = InterfaceLogServiceHelper.StartInterfaceLog3BB(_uow, _intfLog3bb, query, query.TRANSACTION_ID, "ResQuery", "ZTEResQueryQuery", null, "3BB", "");
            else log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.TRANSACTION_ID, "ResQuery", "ZTEResQueryQuery", null, "FBB", "");
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
                InterfaceLog3BBCommand logToken3BB = null;
                InterfaceLogCommand logToken = null;
                if (query.FullUrl == "3BB")
                {
                    logToken3BB = InterfaceLogServiceHelper.StartInterfaceLog3BB(_uow, _intfLog3bb, accessToken, query.TRANSACTION_ID, "ResQueryToken", "ZTEResQueryQueryToken", null, "3BB", "");
                }
                else
                {
                    logToken = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, accessToken, query.TRANSACTION_ID, "ResQueryToken", "ZTEResQueryQueryToken", null, "FBB", "");
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
                //Mock up
                //var resultLov = _cfgLovRepository.Get().FirstOrDefault(item => item.LOV_NAME == "TEST_RESQUERY_CASE") ?? new FBB_CFG_LOV();
                //var _resultCase = _cfgLovRepository.Get().FirstOrDefault(item => item.LOV_VAL1 == resultLov.LOV_VAL1) ?? new FBB_CFG_LOV();

                //switch (_resultCase.LOV_VAL2)
                //{
                //    case "1":
                //    case "2":
                //        zteReQueryModel.RETURN_CASECODE = _resultCase.LOV_VAL2;
                //        break;

                //    default:
                //        zteReQueryModel.RETURN_CASECODE = "3";
                //        break;
                //}

                //InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, zteReQueryModel, log, "Success", "", "");

                //return zteReQueryModel;

                // Release
                FBSSOrderServices.splitList[] listSpliter = null;
                if (query.LISTOFSPLITTER != null)
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

                FBSSOrderServices.dslamList[] listDslam = null;

                if (query.LISTOFDSLAM != null)
                {
                    listDslam = new FBSSOrderServices.dslamList[query.LISTOFDSLAM.Length];
                    var j = 0;

                    foreach (var dslam in query.LISTOFDSLAM.Select(q => new FBSSOrderServices.dslamList
                    {
                        DSLAM_NO = q.Dslam_Name.ToString(),
                        DSLAM_SEQ = (j + 1).ToString()
                    }))
                    {
                        listDslam[j] = dslam;
                        j++;
                    }
                }

                //R21.2 Endpoint FBSSOrderServices.OrderService
                var endpointFbssOrderServices = (from l in _cfgLovRepository.Get() where l.LOV_NAME == "FBSS_ORDER_SERVICES" select l.LOV_VAL1).ToList().FirstOrDefault();

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.ServerCertificateValidationCallback = (s, certificate, chain, sslPolicyErrors) => true;

                using (var service = new FBSSAccessToken.CustomOrderService(accessToken))
                {
                    if (!string.IsNullOrEmpty(endpointFbssOrderServices)) service.Url = endpointFbssOrderServices;

                    FBSSOrderServices.resultSplitList[] resultSplitLists;
                    FBSSOrderServices.resultDslamList[] resultDslamLists;
                    var resultResQuery = service.ResQuery(query.PRODUCT, listSpliter, listDslam, query.PHONE_FLAGE, query.ADDRESS_ID, out resultSplitLists, out resultDslamLists);
                    //Convert Case Return
                    var result = from lov in _cfgLovRepository.Get()
                                 where lov.LOV_NAME == "ResQuery" && lov.LOV_VAL1 == resultResQuery
                                 select lov;
                    var resultCase = result.FirstOrDefault() ?? new FBB_CFG_LOV();

                    if (resultCase != null && resultCase.LOV_VAL2 == "5")
                    {
                        if (query.PHONE_FLAGE == "Y")
                        {
                            query.PHONE_FLAGE = "N";
                        }
                        else
                        {
                            query.PHONE_FLAGE = "Y";
                        }

                        if (query.FullUrl == "3BB") log3bb2 = InterfaceLogServiceHelper.StartInterfaceLog3BB(_uow, _intfLog3bb, query, query.TRANSACTION_ID, "ResQuery2", "ZTEResQueryQuery2", null, "3BB", "");
                        else log2 = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.TRANSACTION_ID, "ResQuery2", "ZTEResQueryQuery2", null, "FBB", "");


                        resultResQuery = service.ResQuery(query.PRODUCT, listSpliter, listDslam, query.PHONE_FLAGE, query.ADDRESS_ID, out resultSplitLists, out resultDslamLists);


                        if (query.FullUrl == "3BB") InterfaceLogServiceHelper.EndInterfaceLog3BB(_uow, _intfLog3bb, resultResQuery, log3bb2, "Success", "", "");
                        else InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, resultResQuery, log2, "Success", "", "");

                        result = from lov in _cfgLovRepository.Get()
                                 where lov.LOV_NAME == "ResQuery" && lov.LOV_VAL1 == resultResQuery
                                 select lov;
                        resultCase = result.FirstOrDefault() ?? new FBB_CFG_LOV();
                    }

                    zteReQueryModel.RETURN_CASECODE = resultResQuery;

                    zteReQueryModel.RESULT_SPLITTERLIST = new ResultSplitList[resultSplitLists.Length];
                    int i = 0;
                    foreach (var spliter in resultSplitLists.Select(resultSpliter => new ResultSplitList
                    {
                        RESULT_CODE = resultSpliter.RESULT_CODE,
                        RESULT_DESCRIPTION = resultSpliter.RESULT_DESCRIPTION,
                        SPLITTER_NO = resultSpliter.SPLITTER_NO
                    }))
                    {
                        zteReQueryModel.RESULT_SPLITTERLIST[i] = spliter;
                        i++;
                    }

                    zteReQueryModel.RESULT_DSLAMLIST = new ResultDslamList[resultDslamLists.Length];
                    int j = 0;
                    foreach (var dslam in resultDslamLists.Select(resultDslam => new ResultDslamList
                    {
                        RESULT_CODE = resultDslam.RESULT_CODE,
                        RESULT_DESCRIPTION = resultDslam.RESULT_DESCRIPTION,
                        DSLAM_NO = resultDslam.DSLAM_NO
                    }))
                    {
                        zteReQueryModel.RESULT_DSLAMLIST[j] = dslam;
                        j++;
                    }

                    if (query.FullUrl == "3BB") InterfaceLogServiceHelper.EndInterfaceLog3BB(_uow, _intfLog3bb, zteReQueryModel, log3bb, "Success", "", "");
                    else InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, zteReQueryModel, log, "Success", "", "");

                    if (query.PRODUCT == "FTTH")
                    {
                        switch (resultCase.LOV_VAL2)
                        {
                            //case "1":
                            //case "2":
                            //    zteReQueryModel.RETURN_CASECODE = resultCase.LOV_VAL2;
                            //    break;

                            //default:
                            //    zteReQueryModel.RETURN_CASECODE = "3";
                            //    break;
                            case "1":
                            case "4":
                            case "2":
                                zteReQueryModel.RETURN_CASECODE = resultCase.LOV_VAL2;
                                break;
                                //case "4":
                                //    zteReQueryModel.RETURN_CASECODE = resultCase.LOV_VAL2;
                                //break;
                            default:
                                zteReQueryModel.RETURN_CASECODE = "3";
                                break;
                        }
                    }
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

                zteReQueryModel.RETURN_CASECODE = "3";

                if (query.FullUrl == "3BB") InterfaceLogServiceHelper.EndInterfaceLog3BB(_uow, _intfLog3bb, webEx.Message, log3bb, "Failed", webEx.Message, "");
                else InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, webEx.Message, log, "Failed", webEx.Message, "");
            }
            catch (Exception ex)
            {
                zteReQueryModel.RETURN_CASECODE = "3";

                if (query.FullUrl == "3BB") InterfaceLogServiceHelper.EndInterfaceLog3BB(_uow, _intfLog3bb, ex.Message, log3bb, "Failed", ex.Message, "");
                else InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex.Message, log, "Failed", ex.Message, "");
            }

            return zteReQueryModel;
        }
    }
}
