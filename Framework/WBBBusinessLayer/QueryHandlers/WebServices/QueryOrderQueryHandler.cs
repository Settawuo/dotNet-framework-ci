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
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class QueryOrderQueryHandler : IQueryHandler<QueryOrderQuery, QueryOrderModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<FBB_CFG_LOV> _lov;
        private readonly IWBBUnitOfWork _uow;
        private readonly IQueryHandler<GetTokenFbbQuery, GetTokenFbbModel> _queryProcessor;
        private readonly MemoryCache cache = MemoryCache.Default;

        public QueryOrderQueryHandler(ILogger logger,
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

        public QueryOrderModel Handle(QueryOrderQuery query)
        {
            int countException = 0;
            InterfaceLogCommand log = null;

            var in_transaction = "";

            if (query != null && query.FIBRENetID_List.Any())
            {
                in_transaction = query.FIBRENetID_List.FirstOrDefault().FIBRENET_ID;
            }

            log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, in_transaction, "QueryOrder", "QueryOrderQueryHandler", null, "FBB|" + query.FullUrl, "");

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
                var logToken = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, accessToken, in_transaction, "QueryOrderToken", "QueryOrderQueryHandlerToken", null, "FBB|" + query.FullUrl, "");
                
                if (!string.IsNullOrEmpty(accessToken))
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, "", logToken, "Success", "", "");
                }
                else
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, "", logToken, "Failed", "Access Token is Null", "");
                }
                #endregion

                FBSSOrderServices.fibernetId[] reqOrder = query.FIBRENetID_List.Select(x => new FBSSOrderServices.fibernetId()
                {
                    FIBRENET_ID = x.FIBRENET_ID,
                    START_DATE = x.START_DATE.ToString(),
                    END_DATE = x.END_DATE.ToString()
                }).ToArray();

                string RESULT_DESC;
                FBSSOrderServices.orderDetail[] respOrder;
                QueryOrderModel result = new QueryOrderModel();

                //R21.2 Endpoint FBSSOrderServices.OrderService
                var endpointFbssOrderServices = (from l in _lov.Get() where l.LOV_NAME == "FBSS_ORDER_SERVICES" select l.LOV_VAL1).ToList().FirstOrDefault();

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.ServerCertificateValidationCallback = (s, certificate, chain, sslPolicyErrors) => true;

                //R24.10 Call Access Token FBSS
                using (var service = new FBSSAccessToken.CustomOrderService(accessToken))
                {
                    if (!string.IsNullOrEmpty(endpointFbssOrderServices)) service.Url = endpointFbssOrderServices;
                    result.RESULT = service.QueryOrder(reqOrder, query.ORDER_TYPE, out RESULT_DESC, out respOrder);
                }
                result.RESULT_DESC = RESULT_DESC;
                result.Order_Details_List = respOrder.Select(x => new Order_Details()
                {
                    //ACTIVITY_DETAILS = x.ACTIVITY_DETAILS.Cast<Activity_Details>().ToList(),                        
                    ACTIVITY_DETAILS = x.ACTIVITY_DETAILS.Select(y => new Activity_Details()
                    {
                        ACTIVITY = y.ACTIVITY,
                        APPOINTMENT_DATE = y.APPOINTMENT_DATE,
                        APPOINTMENT_TIMESLOT = y.APPOINTMENT_TIMESLOT,
                        COMPLETED_DATE = y.COMPLETED_DATE,
                        CREATED_DATE = y.CREATED_DATE,
                        FOA_REJECT_REASON = y.FOA_REJECT_REASON,
                        HANDLE_JOB = y.HANDLE_JOB,
                        HANDLE_ORG = y.HANDLE_ORG,
                        HANDLE_STAFF = y.HANDLE_STAFF,
                        REMARKS = y.REMARKS,
                        SUBJECT = y.SUBJECT,
                        TIMEOUT_DATE = y.TIMEOUT_DATE,
                        WARNING_DATE = y.WARNING_DATE,
                        WORK_ORDER_NO = y.WORK_ORDER_NO,
                        WORK_ORDER_STATE = y.WORK_ORDER_STATE,
                        CURRENT_WORK_ORDER_FLAG = y.CURRENT_WORK_ORDER_FLAG
                    }).ToList(),
                    APPOINTMENT_DATE = x.APPOINTMENT_DATE,
                    APPOINTMENT_TIMESLOT = x.APPOINTMENT_TIMESLOT,
                    CUSTOMER_NAME = x.CUSTOMER_NAME,
                    FIBRENET_ID = x.FIBRENET_ID,
                    ORDER_TYPE = x.ORDER_TYPE,
                    SERVICE_PACKAGE_NAME = x.SERVICE_PACKAGE_NAME,
                    TRANACTION_NOTE = x.TRANACTION_NOTE,
                    TRANSACTION_DATE = x.TRANSACTION_DATE,
                    TRANSACTION_NUMBER = x.TRANSACTION_NUMBER,
                    TRANSACTION_STATE = x.TRANSACTION_STATE,
                    EVENT = x.EVENT
                }).ToList();

                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, "Success", RESULT_DESC, "");
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

                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, webEx.Message, log, "Failed", webEx.Message, "");
                return new QueryOrderModel();
            }
            catch (Exception ex)
            {
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex.Message, log, "Failed", ex.Message, "");
                return new QueryOrderModel();
            }
        }
    }

    public class ReleaseTimeSlotQueryHandler : IQueryHandler<ReleaseTimeSlotQuery, ReleaseTimeSlotModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<FBB_CFG_LOV> _lov;
        private readonly IWBBUnitOfWork _uow;
        private readonly IQueryHandler<GetTokenFbbQuery, GetTokenFbbModel> _queryProcessor;
        private readonly MemoryCache cache = MemoryCache.Default;

        public ReleaseTimeSlotQueryHandler(ILogger logger,
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

        public ReleaseTimeSlotModel Handle(ReleaseTimeSlotQuery query)
        {
            int countException = 0;
            InterfaceLogCommand log = null;

            log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.ORDER_ID, "ReleaseTimeSlotQuery", "ReleaseTimeSlotQueryHandler", null, "FBB", "");

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
                var logToken = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, accessToken, query.ORDER_ID, "ReleaseTimeSlotQueryToken", "ReleaseTimeSlotQueryHandlerToken", null, "FBB", "");
                
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
                ReleaseTimeSlotModel result = new ReleaseTimeSlotModel();

                //R21.2 Endpoint FBSSOrderServices.OrderService
                var endpointFbssOrderServices = (from l in _lov.Get() where l.LOV_NAME == "FBSS_ORDER_SERVICES" select l.LOV_VAL1).ToList().FirstOrDefault();

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.ServerCertificateValidationCallback = (s, certificate, chain, sslPolicyErrors) => true;

                //R24.10 Call Access Token FBSS
                using (var service = new FBSSAccessToken.CustomOrderService(accessToken))
                {
                    if (!string.IsNullOrEmpty(endpointFbssOrderServices)) service.Url = endpointFbssOrderServices;

                    result.RESULT = service.ReleaseTimeSlot(query.RESERVED_ID, out RESULT_DESC);
                }
                result.RESULT_DESC = RESULT_DESC;

                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, "Success", RESULT_DESC, "");
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

                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, webEx.Message, log, "Failed", webEx.Message, "");
                return new ReleaseTimeSlotModel();
            }
            catch (Exception ex)
            {
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex.Message, log, "Failed", ex.Message, "");
                return new ReleaseTimeSlotModel();
            }
        }
    }

    public class ResReleaseQueryHandler : IQueryHandler<ResReleaseQuery, ResReleaseModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<FBB_CFG_LOV> _lov;
        private readonly IWBBUnitOfWork _uow;
        private readonly IQueryHandler<GetTokenFbbQuery, GetTokenFbbModel> _queryProcessor;
        private readonly MemoryCache cache = MemoryCache.Default;

        public ResReleaseQueryHandler(ILogger logger,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IEntityRepository<FBB_CFG_LOV> lov,
            IWBBUnitOfWork uow ,
            IQueryHandler<GetTokenFbbQuery, GetTokenFbbModel> queryProcessor)
        {
            _logger = logger;
            _intfLog = intfLog;
            _lov = lov;
            _uow = uow;
            _queryProcessor = queryProcessor;
        }

        public ResReleaseModel Handle(ResReleaseQuery query)
        {
            int countException = 0;
            InterfaceLogCommand log = null;

            log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.ORDER_ID, "ResReleaseQuery", "ResReleaseQueryHandler", null, "FBB", "");

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
                var logToken = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, accessToken, query.ORDER_ID, "ResReleaseQueryToken", "ResReleaseQueryHandlerToken", null, "FBB", "");
                
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
                ResReleaseModel result = new ResReleaseModel();

                //R21.2 Endpoint FBSSOrderServices.OrderService
                var endpointFbssOrderServices = (from l in _lov.Get() where l.LOV_NAME == "FBSS_ORDER_SERVICES" select l.LOV_VAL1).ToList().FirstOrDefault();

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.ServerCertificateValidationCallback = (s, certificate, chain, sslPolicyErrors) => true;

                //R24.10 Call Access Token FBSS
                using (var service = new FBSSAccessToken.CustomOrderService(accessToken))
                {
                    if (!string.IsNullOrEmpty(endpointFbssOrderServices)) service.Url = endpointFbssOrderServices;

                    result.RESULT = service.ReleaseTimeSlot(query.RES_RESERVATION_ID, out RESULT_DESC);
                }
                result.RESULT_DESC = RESULT_DESC;

                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, "Success", RESULT_DESC, "");
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
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, webEx.Message, log, "Failed", webEx.Message, "");
                return new ResReleaseModel();
            }
            catch (Exception ex)
            {
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex.Message, log, "Failed", ex.Message, "");
                return new ResReleaseModel();
            }
        }
    }
}
