using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Runtime.Caching;
using System.Text.RegularExpressions;
using System.Web.Caching;
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
using WBBEntity.PanelModels.StoredProc;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class GetFBSSAppointmentHandler : IQueryHandler<GetFBSSAppointment, List<FBSSTimeSlot>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_CFG_LOV> _cfgLov;
        private readonly IEntityRepository<TimeSlotModel> _timeSlot;
        private readonly IQueryHandler<GetOnlineQueryAppointmentQuery, GetOnlineQueryAppointmentModel> _getOnlineQuery;
        private readonly IQueryHandler<GetOnlineQueryMAappointmentQuery, GetOnlineQueryMAappointmentModel> _getOnlineQueryMA;
        private readonly IQueryHandler<GetTokenFbbQuery, GetTokenFbbModel> _queryProcessor;
        private readonly MemoryCache cache = MemoryCache.Default;

        public GetFBSSAppointmentHandler(ILogger logger,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IWBBUnitOfWork uow,
            IEntityRepository<FBB_CFG_LOV> cfgLov,
            IEntityRepository<TimeSlotModel> timeSlot,
            IQueryHandler<GetOnlineQueryAppointmentQuery, GetOnlineQueryAppointmentModel> getOnlineQuery,
            IQueryHandler<GetOnlineQueryMAappointmentQuery, GetOnlineQueryMAappointmentModel> getOnlineQueryMA,
            IQueryHandler<GetTokenFbbQuery, GetTokenFbbModel> queryProcessor)
        {
            _logger = logger;
            _intfLog = intfLog;
            _uow = uow;
            _cfgLov = cfgLov;
            _timeSlot = timeSlot;
            _getOnlineQuery = getOnlineQuery;
            _getOnlineQueryMA = getOnlineQueryMA;
            _queryProcessor = queryProcessor;
        }

        public List<FBSSTimeSlot> Handle(GetFBSSAppointment query)
        {
            bool getAppointFromZTE = false;

            List<FBSSTimeSlot> listTimeSlot = new List<FBSSTimeSlot>();

            #region sql
            //เช็คว่าจะให้ใช้ Time Slot ของ ZTE หรือว่าของหน้า Web
            //select lov_val1
            //into v_time_slot_zte
            //from fbb_cfg_lov
            //where lov_type = ‘TIME_SLOT_ZTE’
            //and activeflag = 'Y'

            //o	ถ้า v_time_slot_zte = ‘Y’ แล้ว
            #endregion

            InterfaceLogCommand log = null;
            //log = FBSSExtensions.StartInterfaceFBSSLog(_uow, _intfLog, query,
            //        "", "appointment(getAppointmentFrom)", "GetFBSSAppointmentHandler");
            log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.Transaction_Id, "appointment(getAppointmentFrom)", "GetFBSSAppointmentHandler", null, "FBB|" + query.FullUrl, "");
            if (query.LineSelect == LineType.Line1)
            {
                var lovTech = from l in _cfgLov.Get()
                              where l.LOV_NAME == "L_TIME_SLOT"
                              && l.DISPLAY_VAL == query.AccessMode
                              && l.ACTIVEFLAG == "Y"
                              select l.LOV_VAL4;

                if (lovTech.Any())
                {
                    if (lovTech.FirstOrDefault() == "Y") getAppointFromZTE = true;
                }
            }
            else
            {
                var lovResult = from l in _cfgLov.Get()
                                where l.LOV_NAME == "TIME_SLOT_ZTE_VAS"
                                && l.ACTIVEFLAG == "Y"
                                select l.LOV_VAL1;

                if (lovResult.Any())
                {
                    if (lovResult.FirstOrDefault() == "Y") getAppointFromZTE = true;
                }
            }

            //20.10 Appointment From Online Query
            var getAppointFromOnlineQuery = false;
            var onlineQueryFlag = from l in _cfgLov.Get()
                                  where l.LOV_NAME == "APPOINTMENT_BY_ONLINE_QUERY_FLAG"
                                  && l.ACTIVEFLAG == "Y"
                                  select l.LOV_VAL1;

            if (onlineQueryFlag.Any())
            {
                if (onlineQueryFlag.FirstOrDefault() == "Y") getAppointFromOnlineQuery = true;
            }

            //FBSSExtensions.EndInterfaceFBSSLog(_uow, _intfLog, getAppointFromZTE, log,
            //                "Success", "");
            InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, getAppointFromZTE, log, "Success", "", "");

            //InterfaceLogCommand log = null;
            if (getAppointFromZTE)
            {
                #region Get Appointment from ZTE

                //log = FBSSExtensions.StartInterfaceFBSSLog(_uow, _intfLog, query,
                //    "", "appointment", "GetFBSSAppointmentHandler");
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.Transaction_Id, "appointment", "GetFBSSAppointmentHandler", null, "FBB|" + query.FullUrl, "");

                var resultCode = "";
                var resultDesc = "";

                try
                {
                    List<FBSSTimeSlot> result = new List<FBSSTimeSlot>();
                    if (query.TaskType == "MA")
                    {
                        string postCode = "";
                        TTComplainSheet.slotList[] TimeSlotList = null;

                        string displayValue = "";
                        string SymptomCode = "";
                        if (query.PlayBoxCountOld == "1" && query.PlayBoxCountNew == "1" && query.AccessMode == "VDSL")
                        {
                            displayValue = "VDSL_EXT_1";
                        }
                        else if (query.PlayBoxCountOld == "1" && query.PlayBoxCountNew == "2" && query.AccessMode == "VDSL")
                        {
                            displayValue = "VDSL_EXT_2";
                        }
                        else if (query.PlayBoxCountOld == "2" && query.PlayBoxCountNew == "1" && query.AccessMode == "VDSL")
                        {
                            displayValue = "VDSL_EXT_1_2";
                        }
                        else if (query.PlayBoxCountOld == "1" && query.PlayBoxCountNew == "1" && query.AccessMode == "FTTH")
                        {
                            displayValue = "FTTH_EXT_1";
                        }
                        else if (query.PlayBoxCountOld == "1" && query.PlayBoxCountNew == "2" && query.AccessMode == "FTTH")
                        {
                            displayValue = "FTTH_EXT_2";
                        }
                        else if (query.PlayBoxCountOld == "2" && query.PlayBoxCountNew == "1" && query.AccessMode == "FTTH")
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

                        //R20.11
                        if (getAppointFromOnlineQuery)
                        {
                            var reqOnline = new GetOnlineQueryMAappointmentQuery
                            {
                                FullUrl = query.FullUrl,
                                Transaction_Id = query.Transaction_Id,
                                Internet_No = query.AisAirNumber,
                                Body = new OnlineQueryMABody
                                {
                                    AccessNo = query.AisAirNumber,
                                    MADate = query.InstallationDate,
                                    AccessMode = query.AccessMode.ToString(),
                                    Days = query.Days.ToString(),
                                    SymptomCode = SymptomCode,
                                    ServiceLevel = ServiceLevel,
                                    TimeAdd = query.TimeAdd,
                                    ActionTimeSlot = query.ActionTimeSlot,
                                    NumTimeSlot = query.NumTimeSlot
                                }
                            };
                            var resOnline = _getOnlineQueryMA.Handle(reqOnline);
                            if (resOnline?.RESULT_CODE == "20000" && resOnline?.DAY_OFF_RESULT?.TIME_SLOT_LIST.Count > 0)
                            {
                                result = resOnline?.DAY_OFF_RESULT?.TIME_SLOT_LIST.Select(t => new FBSSTimeSlot
                                {
                                    AppointmentDate = t.MA_DATE.ToFBSSDate(),
                                    InstallationCapacity = t.CAPACITY,
                                    TimeSlot = t.MA_TIME_SLOT,
                                    DayOffFlag = t.DAY_OFF_FLAG
                                }).ToList();
                            }
                        }
                        else
                        {
                            int countException = 0;

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
                                var logToken = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, accessToken, query.Transaction_Id, "appointmentToken", "GetFBSSAppointmentHandlerToken", "MAappointment", "FBB|" + query.FullUrl, "");

                                if (!string.IsNullOrEmpty(accessToken))
                                {
                                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, "", logToken, "Success", "", "");
                                }
                                else
                                {
                                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, "", logToken, "Failed", "Access Token is Null", "");
                                }
                                #endregion

                                //R21.2 Endpoint TTComplainSheet.TTComplainSheet
                                var endpointFbssTTComplainsheet = (from l in _cfgLov.Get() where l.LOV_NAME == "FBSS_TT_COMPLAINSHEET" select l.LOV_VAL1).ToList().FirstOrDefault();

                                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                                ServicePointManager.Expect100Continue = true;
                                ServicePointManager.ServerCertificateValidationCallback = (s, certificate, chain, sslPolicyErrors) => true;

                                //R24.10 Call Access Token FBSS
                                using (var service = new FBSSAccessToken.CustomTTComplainSheet(accessToken))
                                {
                                    if (!string.IsNullOrEmpty(endpointFbssTTComplainsheet)) service.Url = endpointFbssTTComplainsheet;

                                    service.MAappointment(
                                        query.AisAirNumber,
                                        query.InstallationDate,
                                        query.AccessMode,
                                        query.Days.ToSafeString(),
                                        SymptomCode,
                                        ServiceLevel,
                                        out postCode,
                                        out TimeSlotList,
                                        out resultCode,
                                        out resultDesc);
                                    if (TimeSlotList != null && TimeSlotList.Count() > 0)
                                    {
                                        result = TimeSlotList.Select(t => new FBSSTimeSlot
                                        {
                                            AppointmentDate = t.MA_DATE.ToFBSSDate(),
                                            InstallationCapacity = t.CAPACITY,
                                            TimeSlot = t.MA_TIME_SLOT,
                                        }).ToList();
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

                                throw;
                            }
                            
                        }
                    }
                    else
                    {
                        if (getAppointFromOnlineQuery)
                        {
                            var reqOnline = new GetOnlineQueryAppointmentQuery
                            {
                                FullUrl = query.FullUrl,
                                Transaction_Id = query.Transaction_Id,
                                Internet_No = query.AisAirNumber,
                                Body = new OnlineQueryBody
                                {
                                    InstallationDate = query.InstallationDate,
                                    ProductSpecCode = query.ProductSpecCode,
                                    AccessMode = query.AccessMode.ToString(),
                                    AddressId = query.AddressId,
                                    Days = query.Days.ToString(),
                                    SubDistrict = query.SubDistrict,
                                    Postal_Code = query.Postal_Code,
                                    SubAccessMode = query.SubAccessMode,
                                    TaskType = query.TaskType,
                                    ASSIGN_CONDITION_LIST = query.ASSIGN_CONDITION_LIST,
                                    TimeAdd = query.TimeAdd,
                                    ActionTimeSlot = query.ActionTimeSlot,
                                    NumTimeSlot = query.NumTimeSlot
                                }
                            };
                            var resOnline = _getOnlineQuery.Handle(reqOnline);
                            if (resOnline?.RESULT_CODE == "20000" && resOnline?.DAY_OFF_RESULT?.TIME_SLOT_LIST.Count > 0)
                            {
                                result = resOnline?.DAY_OFF_RESULT?.TIME_SLOT_LIST.Select(t => new FBSSTimeSlot
                                {
                                    AppointmentDate = t.APPOINTMENT_DATE.ToFBSSDate(),
                                    InstallationCapacity = t.INSTALLATION_CAPACITY,
                                    TimeSlot = t.TIME_SLOT,
                                    DayOffFlag = t.DAY_OFF_FLAG,
                                    ActiveFlag = t.ACTIVE_FLAG
                                }).ToList();
                            }
                        }
                        else
                        {
                            int countException = 0;

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
                                var logToken = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, accessToken, query.Transaction_Id, "appointmentToken", "GetFBSSAppointmentHandlerToken", "appointment", "FBB|" + query.FullUrl, "");

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

                                    result = service.appointment(
                                        query.InstallationDate,
                                        query.ProductSpecCode,
                                        query.AccessMode.ToString(),
                                        query.AddressId,
                                        query.Days,
                                        true,
                                        null, //EXT_ATTR
                                        query.SubDistrict, // SUBDISTRICT
                                        query.Postal_Code, // POSTCODE
                                        query.SubAccessMode,
                                        query.TaskType,
                                        condiAttrArr, //เพิ่ม 0520
                                        out resultCode,
                                        out resultDesc)
                                        .Select(t => new FBSSTimeSlot
                                        {
                                            AppointmentDate = t.APPOINTMENT_DATE.ToFBSSDate(),
                                            InstallationCapacity = t.INSTALLATION_CAPACITY,
                                            TimeSlot = t.TIME_SLOT,
                                        }).ToList();
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

                                throw;
                            }
                        }
                    }
                    //FBSSExtensions.EndInterfaceFBSSLog(_uow, _intfLog, result, log,
                    //    "Success", resultCode + " : " + resultDesc);
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, "Success", resultCode + " : " + resultDesc, "");

                    listTimeSlot = result;

                }
                catch (Exception ex)
                {
                    if (null != log)
                    {
                        //FBSSExtensions.EndInterfaceFBSSLog(_uow, _intfLog, ex.RenderExceptionMessage(), log,
                        //    "Failed", ex.GetErrorMessage());
                        InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex.RenderExceptionMessage(), log, "Failed", ex.GetErrorMessage(), "");
                    }

                    throw ex;
                }
                #endregion 
            }
            else
            {
                #region Get Appointment from WBB

                try
                {
                    //log = FBSSExtensions.StartInterfaceFBSSLog(_uow, _intfLog, query,
                    //"", "appointment", "GetFBSSAppointmentHandler(PKG_FBB_TIME_SLOT.LIST_TIME_SLOT)");
                    log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, "", "appointment", "GetFBSSAppointmentHandler(PKG_FBB_TIME_SLOT.LIST_TIME_SLOT)", null, "FBB", "");

                    var cursor = new OracleParameter();
                    cursor.OracleDbType = OracleDbType.RefCursor;
                    cursor.Direction = ParameterDirection.Output;

                    listTimeSlot = _timeSlot.ExecuteReadStoredProc("WBB.PKG_FBB_TIME_SLOT.LIST_TIME_SLOT",
                    new
                    {
                        p_service_code = query.Service_Code,
                        p_lang = query.Language,
                        p_subdistrict = query.SubDistrict,
                        p_district = query.District,
                        p_province = query.Province,
                        p_postal_code = query.Postal_Code,
                        p_installation_date = query.InstallationDate,
                        p_days = query.Days,
                        result_data = cursor
                    }).Select(t => new FBSSTimeSlot
                    {
                        AppointmentDate = t.Appointment_Date,
                        InstallationCapacity = t.Installation_Capacity,
                        TimeSlot = t.Time_Slot,
                        TimeSlotId = t.Time_Slot_Id.ToSafeString()
                    }).ToList();

                    //FBSSExtensions.EndInterfaceFBSSLog(_uow, _intfLog, listTimeSlot, log,
                    //        "Success", "");
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, listTimeSlot, log, "Success", "", "");

                }
                catch (Exception ex)
                {
                    _logger.Error(ex.GetErrorMessage());
                    _logger.Error("Error from GetFBSSAppointment-WBB : " + ex.RenderExceptionMessage());

                    if (null != log)
                    {
                        //FBSSExtensions.EndInterfaceFBSSLog(_uow, _intfLog, ex.RenderExceptionMessage(), log,
                        //    "Failed", ex.GetErrorMessage());
                        InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex.RenderExceptionMessage(), log, "Failed", ex.GetErrorMessage(), "");
                    }
                }
                #endregion
            }

            return listTimeSlot;
        }
    }

    public class GetAppointmentChageProQueryHandler : IQueryHandler<GetAppointmentChageProQuery, List<FBSSTimeSlotChangePro>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_CFG_LOV> _cfgLov;
        private readonly IEntityRepository<TimeSlotModel> _timeSlot;
        private readonly IQueryHandler<GetTokenFbbQuery, GetTokenFbbModel> _queryProcessor;
        private readonly MemoryCache cache = MemoryCache.Default;

        public GetAppointmentChageProQueryHandler(ILogger logger,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IWBBUnitOfWork uow,
            IEntityRepository<FBB_CFG_LOV> cfgLov,
            IEntityRepository<TimeSlotModel> timeSlot, 
            IQueryHandler<GetTokenFbbQuery, GetTokenFbbModel> queryProcessor)
        {
            _logger = logger;
            _intfLog = intfLog;
            _uow = uow;
            _cfgLov = cfgLov;
            _timeSlot = timeSlot;
            _queryProcessor = queryProcessor;
        }

        public List<FBSSTimeSlotChangePro> Handle(GetAppointmentChageProQuery query)
        {
            int countException = 0;
            List<FBSSTimeSlotChangePro> listTimeSlot = new List<FBSSTimeSlotChangePro>();
            InterfaceLogCommand log = null;
            log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.TRANSACTION_ID, "GetAppointmentChagePro", "GetAppointmentChageProQueryHandler", null, "FBB|" + query.FULL_URL, "");

            #region Get Appointment from ZTE

            var resultCode = "";
            var resultDesc = "";
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
                var logToken = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, accessToken, query.TRANSACTION_ID, "GetAppointmentChageProToken", "GetAppointmentChageProQueryHandlerToken", "appointment", "FBB|" + query.FULL_URL, "");

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
                var endpointFbssOrderServices = (from l in _cfgLov.Get() where l.LOV_NAME == "FBSS_ORDER_SERVICES" select l.LOV_VAL1).ToList().FirstOrDefault();

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.ServerCertificateValidationCallback = (s, certificate, chain, sslPolicyErrors) => true;

                using (var service = new FBSSAccessToken.CustomOrderService(accessToken))
                {
                    if (!string.IsNullOrEmpty(endpointFbssOrderServices)) service.Url = endpointFbssOrderServices;
                    var result = service.appointment(
                        query.INSTALLATION_DATE,
                        query.PROD_SPEC_CODE,
                        query.ACCESS_MODE.ToString(),
                        query.ADDRESS_ID,
                        int.Parse(query.DAYS),
                        true,
                        null,
                        query.SUBDISTRICT,
                        query.POSTCODE,
                        query.SUB_ACCESS_MODE,
                        query.TaskType,
                        null, //เพิ่ม 0520
                        out resultCode,
                        out resultDesc)
                        .Select(t => new FBSSTimeSlotChangePro
                        {
                            AppointmentDate = t.APPOINTMENT_DATE.ToSafeString(),
                            InstallationCapacity = t.INSTALLATION_CAPACITY.ToSafeString(),
                            TimeSlot = t.TIME_SLOT.ToSafeString(),
                        }).ToList();

                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, "Success", resultCode + " : " + resultDesc, "");

                    listTimeSlot = result;
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
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, webEx.RenderExceptionMessage(), log, "Failed", webEx.GetErrorMessage(), "");
                }

                throw webEx;
            }
            catch (Exception ex)
            {
                if (null != log)
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex.RenderExceptionMessage(), log, "Failed", ex.GetErrorMessage(), "");
                }

                throw ex;
            }
            #endregion 

            return listTimeSlot;
        }
    }

    public class GetDataForAppointmentQueryHandler : IQueryHandler<GetDataForAppointmentQuery, List<DataForAppointment>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<object> _dataForAppointment;

        public GetDataForAppointmentQueryHandler(ILogger logger,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IWBBUnitOfWork uow,
            IEntityRepository<object> dataForAppointment)
        {
            _logger = logger;
            _intfLog = intfLog;
            _uow = uow;
            _dataForAppointment = dataForAppointment;
        }

        public List<DataForAppointment> Handle(GetDataForAppointmentQuery query)
        {
            List<DataForAppointment> listDataForAppointment = new List<DataForAppointment>();

            InterfaceLogCommand log = null;
            log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.TRANSACTION_ID, "GetDataForAppointment", "GetDataForAppointmentQueryHandler", null, "FBB|" + query.FULL_URL, "");

            try
            {
                OracleParameter NON_MOBILE = new OracleParameter();
                NON_MOBILE.ParameterName = "NON_MOBILE";
                NON_MOBILE.OracleDbType = OracleDbType.Varchar2;
                NON_MOBILE.Size = 2000;
                NON_MOBILE.Direction = ParameterDirection.Input;
                NON_MOBILE.Value = query.NON_MOBILE;

                OracleParameter ID_CARD = new OracleParameter();
                ID_CARD.ParameterName = "ID_CARD";
                ID_CARD.OracleDbType = OracleDbType.Varchar2;
                ID_CARD.Size = 2000;
                ID_CARD.Direction = ParameterDirection.Input;
                ID_CARD.Value = query.ID_CARD;

                OracleParameter BILL_CYCLE = new OracleParameter();
                BILL_CYCLE.ParameterName = "BILL_CYCLE";
                BILL_CYCLE.OracleDbType = OracleDbType.Varchar2;
                BILL_CYCLE.Size = 2000;
                BILL_CYCLE.Direction = ParameterDirection.Input;
                BILL_CYCLE.Value = query.BILL_CYCLE;

                OracleParameter LANQUAGE_SCREEN = new OracleParameter();
                LANQUAGE_SCREEN.ParameterName = "LANQUAGE_SCREEN";
                LANQUAGE_SCREEN.OracleDbType = OracleDbType.Varchar2;
                LANQUAGE_SCREEN.Size = 2000;
                LANQUAGE_SCREEN.Direction = ParameterDirection.Input;
                LANQUAGE_SCREEN.Value = query.LANQUAGE_SCREEN;

                OracleParameter P_ADDRESS_ID = new OracleParameter();
                P_ADDRESS_ID.ParameterName = "P_ADDRESS_ID";
                P_ADDRESS_ID.OracleDbType = OracleDbType.Varchar2;
                P_ADDRESS_ID.Size = 2000;
                P_ADDRESS_ID.Direction = ParameterDirection.Input;
                P_ADDRESS_ID.Value = query.P_ADDRESS_ID;

                OracleParameter INSTALL_ADDRESS_1 = new OracleParameter();
                INSTALL_ADDRESS_1.ParameterName = "INSTALL_ADDRESS_1";
                INSTALL_ADDRESS_1.OracleDbType = OracleDbType.Varchar2;
                INSTALL_ADDRESS_1.Size = 2000;
                INSTALL_ADDRESS_1.Direction = ParameterDirection.Input;
                INSTALL_ADDRESS_1.Value = query.INSTALL_ADDRESS_1;

                OracleParameter INSTALL_ADDRESS_2 = new OracleParameter();
                INSTALL_ADDRESS_2.ParameterName = "INSTALL_ADDRESS_2";
                INSTALL_ADDRESS_2.OracleDbType = OracleDbType.Varchar2;
                INSTALL_ADDRESS_2.Size = 2000;
                INSTALL_ADDRESS_2.Direction = ParameterDirection.Input;
                INSTALL_ADDRESS_2.Value = query.INSTALL_ADDRESS_2;

                OracleParameter INSTALL_ADDRESS_3 = new OracleParameter();
                INSTALL_ADDRESS_3.ParameterName = "INSTALL_ADDRESS_3";
                INSTALL_ADDRESS_3.OracleDbType = OracleDbType.Varchar2;
                INSTALL_ADDRESS_3.Size = 2000;
                INSTALL_ADDRESS_3.Direction = ParameterDirection.Input;
                INSTALL_ADDRESS_3.Value = query.INSTALL_ADDRESS_3;

                OracleParameter INSTALL_ADDRESS_4 = new OracleParameter();
                INSTALL_ADDRESS_4.ParameterName = "INSTALL_ADDRESS_4";
                INSTALL_ADDRESS_4.OracleDbType = OracleDbType.Varchar2;
                INSTALL_ADDRESS_4.Size = 2000;
                INSTALL_ADDRESS_4.Direction = ParameterDirection.Input;
                INSTALL_ADDRESS_4.Value = query.INSTALL_ADDRESS_4;

                OracleParameter INSTALL_ADDRESS_5 = new OracleParameter();
                INSTALL_ADDRESS_5.ParameterName = "LANQUAGE_SCREEN";
                INSTALL_ADDRESS_5.OracleDbType = OracleDbType.Varchar2;
                INSTALL_ADDRESS_5.Size = 2000;
                INSTALL_ADDRESS_5.Direction = ParameterDirection.Input;
                INSTALL_ADDRESS_5.Value = query.INSTALL_ADDRESS_5;

                var RETURN_CODE = new OracleParameter();
                RETURN_CODE.ParameterName = "RETURN_CODE";
                RETURN_CODE.OracleDbType = OracleDbType.Decimal;
                RETURN_CODE.Direction = ParameterDirection.Output;

                var RETURN_MESSAGE = new OracleParameter();
                RETURN_MESSAGE.ParameterName = "RETURN_MESSAGE";
                RETURN_MESSAGE.Size = 2000;
                RETURN_MESSAGE.OracleDbType = OracleDbType.Varchar2;
                RETURN_MESSAGE.Direction = ParameterDirection.Output;

                var RETURN_APPOINTMENT = new OracleParameter();
                RETURN_APPOINTMENT.ParameterName = "RETURN_APPOINTMENT";
                RETURN_APPOINTMENT.OracleDbType = OracleDbType.RefCursor;
                RETURN_APPOINTMENT.Direction = ParameterDirection.Output;

                //listDataForAppointment 

                var result = _dataForAppointment.ExecuteStoredProcMultipleCursor("WBB.PKG_CHANGEPRO_APPOINTMENT.APPOINTMENT_TABLE",
                new object[]
                {
                    NON_MOBILE,
                    ID_CARD,
                    BILL_CYCLE,
                    LANQUAGE_SCREEN,
                    P_ADDRESS_ID,
                    INSTALL_ADDRESS_1,
                    INSTALL_ADDRESS_2,
                    INSTALL_ADDRESS_3,
                    INSTALL_ADDRESS_4,
                    INSTALL_ADDRESS_5,
                    RETURN_CODE,
                    RETURN_MESSAGE,
                    RETURN_APPOINTMENT

                });

                string Return_Code = result[0] != null ? result[0].ToSafeString() : "-1";
                string Return_Message = result[1] != null ? result[1].ToSafeString() : "error";
                if (Return_Code != "-1")
                {
                    DataTable data1 = (DataTable)result[2];
                    listDataForAppointment = data1.DataTableToList<DataForAppointment>();
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, listDataForAppointment, log, "Success", "", "");
                }
                else
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, Return_Message, log, "Failed", Return_Message, "");
                }

            }
            catch (Exception ex)
            {
                _logger.Error(ex.GetErrorMessage());
                _logger.Error("Error from GetDataForAppointmentQueryHandler : " + ex.RenderExceptionMessage());

                if (null != log)
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex.RenderExceptionMessage(), log, "Failed", ex.GetErrorMessage(), "");
                }
            }

            return listDataForAppointment;
        }
    }

    public class GetDataForReserveTimeslotQueryHandler : IQueryHandler<GetDataForReserveTimeslotQuery, List<DataForReserveTimeslot>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<DataForReserveTimeslot> _dataForReserveTimeslot;

        public GetDataForReserveTimeslotQueryHandler(ILogger logger,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IWBBUnitOfWork uow,
            IEntityRepository<DataForReserveTimeslot> dataForReserveTimeslot)
        {
            _logger = logger;
            _intfLog = intfLog;
            _uow = uow;
            _dataForReserveTimeslot = dataForReserveTimeslot;
        }

        public List<DataForReserveTimeslot> Handle(GetDataForReserveTimeslotQuery query)
        {
            List<DataForReserveTimeslot> listDataForReserveTimeslot = new List<DataForReserveTimeslot>();

            InterfaceLogCommand log = null;
            log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.TRANSACTION_ID, "GetDataForReserveTimeslot", "GetDataForReserveTimeslotQueryHandler", null, "FBB|" + query.FULL_URL, "");

            try
            {
                var ret_code = new OracleParameter();
                ret_code.ParameterName = "RETURN_CODE";
                ret_code.OracleDbType = OracleDbType.Decimal;
                ret_code.Direction = ParameterDirection.Output;

                var p_return_message = new OracleParameter();
                p_return_message.ParameterName = "RETURN_MESSAGE";
                p_return_message.Size = 2000;
                p_return_message.OracleDbType = OracleDbType.Varchar2;
                p_return_message.Direction = ParameterDirection.Output;

                var cursor = new OracleParameter();
                cursor.OracleDbType = OracleDbType.RefCursor;
                cursor.Direction = ParameterDirection.Output;

                listDataForReserveTimeslot = _dataForReserveTimeslot.ExecuteReadStoredProc("WBB.PKG_CHANGEPRO_APPOINTMENT.RESERVE_TIMESLOT",
                new
                {
                    NON_MOBILE = query.NON_MOBILE,
                    ID_CARD = query.ID_CARD,
                    APPOINTMENT_DATE = query.APPOINTMENT_DATE,
                    TIME_SLOT = query.TIME_SLOT,
                    LANQUAGE_SCREEN = query.LANQUAGE_SCREEN,
                    P_ADDRESS_ID = query.P_ADDRESS_ID,
                    INSTALL_ADDRESS_1 = query.INSTALL_ADDRESS_1,
                    INSTALL_ADDRESS_2 = query.INSTALL_ADDRESS_2,
                    INSTALL_ADDRESS_3 = query.INSTALL_ADDRESS_3,
                    INSTALL_ADDRESS_4 = query.INSTALL_ADDRESS_4,
                    INSTALL_ADDRESS_5 = query.INSTALL_ADDRESS_5,
                    RETURN_CODE = ret_code,
                    RETURN_MESSAGE = p_return_message,
                    RETURN_TIMESLOT = cursor

                }).ToList();

                string Return_Code = ret_code.Value != null ? ret_code.Value.ToSafeString() : "-1";
                string Return_Message = p_return_message.Value != null ? p_return_message.Value.ToSafeString() : "error";
                if (Return_Code != "-1")
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, listDataForReserveTimeslot, log, "Success", "", "");
                }
                else
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, Return_Message, log, "Failed", Return_Message, "");
                }

            }
            catch (Exception ex)
            {
                _logger.Error(ex.GetErrorMessage());
                _logger.Error("Error from GetDataForReserveTimeslotQueryHandler : " + ex.RenderExceptionMessage());

                if (null != log)
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex.RenderExceptionMessage(), log, "Failed", ex.GetErrorMessage(), "");
                }
            }

            return listDataForReserveTimeslot;
        }
    }

    public class CheckNoToPBQueryHandler : IQueryHandler<CheckNoToPBQuery, List<CheckNoToPB>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;
        private readonly IAirNetEntityRepository<CheckNoToPB> _checkNoToPB;

        public CheckNoToPBQueryHandler(ILogger logger,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IWBBUnitOfWork uow,
            IAirNetEntityRepository<CheckNoToPB> checkNoToPB)
        {
            _logger = logger;
            _intfLog = intfLog;
            _uow = uow;
            _checkNoToPB = checkNoToPB;
        }

        public List<CheckNoToPB> Handle(CheckNoToPBQuery query)
        {
            List<CheckNoToPB> listCheckNoToPB = new List<CheckNoToPB>();

            InterfaceLogCommand log = null;
            log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.TRANSACTION_ID, "CheckNoToPB", "CheckNoToPBQueryHandler", null, "FBB|" + query.FULL_URL, "");

            try
            {
                var ret_code = new OracleParameter();
                ret_code.ParameterName = "o_return_code";
                ret_code.OracleDbType = OracleDbType.Decimal;
                ret_code.Direction = ParameterDirection.Output;

                var cursor = new OracleParameter();
                cursor.OracleDbType = OracleDbType.RefCursor;
                cursor.Direction = ParameterDirection.Output;

                listCheckNoToPB = _checkNoToPB.ExecuteReadStoredProc("AIR_ADMIN.PKG_CHECK_NO_TO_PB.CHECK_NO_TO_PB",
                new
                {
                    p_sff_promocode_current = query.p_sff_promocode_current,
                    p_sff_promocode_futurn = query.p_sff_promocode_futurn,
                    P_status_option = query.P_status_option,
                    RETURN_CODE = ret_code,
                    ioResults_CHECK = cursor
                }).ToList();

                string Return_Code = ret_code.Value != null ? ret_code.Value.ToSafeString() : "-1";

                if (Return_Code != "-1")
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, listCheckNoToPB, log, "Success", "", "");
                }
                else
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, "ExecuteReadStoredProc Failed", log, "Failed", "ExecuteReadStoredProc Failed", "");
                }

            }
            catch (Exception ex)
            {
                _logger.Error(ex.GetErrorMessage());
                _logger.Error("Error from CheckNoToPBQueryHandler : " + ex.RenderExceptionMessage());

                if (null != log)
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex.RenderExceptionMessage(), log, "Failed", ex.GetErrorMessage(), "");
                }
            }

            return listCheckNoToPB;
        }
    }

    public class CheckUnlockOrderQueryHandler : IQueryHandler<CheckUnlockOrderQuery, string>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;
        private readonly IAirNetEntityRepository<string> _checkUnlockOrder;

        public CheckUnlockOrderQueryHandler(ILogger logger,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IWBBUnitOfWork uow,
            IAirNetEntityRepository<string> checkUnlockOrder)
        {
            _logger = logger;
            _intfLog = intfLog;
            _uow = uow;
            _checkUnlockOrder = checkUnlockOrder;
        }

        public string Handle(CheckUnlockOrderQuery query)
        {

            InterfaceLogCommand log = null;
            string result = "";
            log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.TRANSACTION_ID, "CheckUnlockOrder", "CheckUnlockOrderQueryHandler", null, "FBB|" + query.FULL_URL, "");

            try
            {
                var o_return_code = new OracleParameter();
                o_return_code.ParameterName = "o_return_code";
                o_return_code.OracleDbType = OracleDbType.Decimal;
                o_return_code.Direction = ParameterDirection.Output;

                var o_return_message = new OracleParameter();
                o_return_message.ParameterName = "o_return_message";
                o_return_message.Size = 2000;
                o_return_message.OracleDbType = OracleDbType.Varchar2;
                o_return_message.Direction = ParameterDirection.Output;

                var dd = _checkUnlockOrder.ExecuteStoredProc("AIR_ADMIN.PKG_CHECK_NO_TO_PB.CHECK_UNLOCK_ORDER",
                new
                {
                    p_sff_promocode_current = query.p_sff_promocode_current,
                    p_sff_promocode_futurn = query.p_sff_promocode_futurn,
                    o_return_code = o_return_code,
                    o_return_message = o_return_message
                });

                string Return_Code = o_return_code.Value != null ? o_return_code.Value.ToSafeString() : "-1";

                if (Return_Code != "-1")
                {
                    result = o_return_message.Value.ToSafeString();
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, "Success", "", "");
                }
                else
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, "ExecuteReadStoredProc Failed", log, "Failed", "ExecuteReadStoredProc Failed", "");
                }

            }
            catch (Exception ex)
            {
                _logger.Error(ex.GetErrorMessage());
                _logger.Error("Error from CheckNoToPBQueryHandler : " + ex.RenderExceptionMessage());

                if (null != log)
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex.RenderExceptionMessage(), log, "Failed", ex.GetErrorMessage(), "");
                }
            }

            return result;
        }
    }

    public class GetInputGetListPackageBySFFPROMOQueryHandler : IQueryHandler<GetInputGetListPackageBySFFPROMOQuery, List<GetInputGetListPackageBySFFPROMO>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;
        private readonly IAirNetEntityRepository<GetInputGetListPackageBySFFPROMO> _getInputGetListPackageBySFFPROMO;

        public GetInputGetListPackageBySFFPROMOQueryHandler(ILogger logger,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IWBBUnitOfWork uow,
            IAirNetEntityRepository<GetInputGetListPackageBySFFPROMO> getInputGetListPackageBySFFPROMO)
        {
            _logger = logger;
            _intfLog = intfLog;
            _uow = uow;
            _getInputGetListPackageBySFFPROMO = getInputGetListPackageBySFFPROMO;
        }

        public List<GetInputGetListPackageBySFFPROMO> Handle(GetInputGetListPackageBySFFPROMOQuery query)
        {
            List<GetInputGetListPackageBySFFPROMO> listGetInputGetListPackageBySFFPROMO = new List<GetInputGetListPackageBySFFPROMO>();

            InterfaceLogCommand log = null;
            log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.TRANSACTION_ID, "GetInputGetListPackageBySFFPROMO", "GetInputGetListPackageBySFFPROMOQueryHandler", null, "FBB|" + query.FULL_URL, "");

            try
            {
                var ret_code = new OracleParameter();
                ret_code.ParameterName = "o_return_code";
                ret_code.OracleDbType = OracleDbType.Decimal;
                ret_code.Direction = ParameterDirection.Output;

                var cursor = new OracleParameter();
                cursor.OracleDbType = OracleDbType.RefCursor;
                cursor.Direction = ParameterDirection.Output;

                listGetInputGetListPackageBySFFPROMO = _getInputGetListPackageBySFFPROMO.ExecuteReadStoredProc("AIR_ADMIN.PKG_CHECK_NO_TO_PB.INPUT_CALL_905",
                new
                {
                    p_sff_promocode_current = query.p_sff_promocode_current,
                    p_sff_promocode_futurn = query.p_sff_promocode_futurn,
                    P_NON_MOBILE = query.P_NON_MOBILE,
                    RETURN_CODE = ret_code,
                    ioResults = cursor
                }).ToList();

                string Return_Code = ret_code.Value != null ? ret_code.Value.ToSafeString() : "-1";

                if (Return_Code != "-1")
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, listGetInputGetListPackageBySFFPROMO, log, "Success", "", "");
                }
                else
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, "ExecuteReadStoredProc Failed", log, "Failed", "ExecuteReadStoredProc Failed", "");
                }

            }
            catch (Exception ex)
            {
                _logger.Error(ex.GetErrorMessage());
                _logger.Error("Error from GetInputGetListPackageBySFFPROMOQueryHandler : " + ex.RenderExceptionMessage());

                if (null != log)
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex.RenderExceptionMessage(), log, "Failed", ex.GetErrorMessage(), "");
                }
            }

            return listGetInputGetListPackageBySFFPROMO;
        }
    }

    public class GetPackageListBySFFPromoForChangProQueryHandler : IQueryHandler<GetPackageListBySFFPromoForChangProQuery, List<PackageModel>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_CFG_LOV> _FBB_CFG_LOV;

        public GetPackageListBySFFPromoForChangProQueryHandler(ILogger logger,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IWBBUnitOfWork uow, IEntityRepository<FBB_CFG_LOV> fbb_CFG_LOV)
        {
            _logger = logger;
            _intfLog = intfLog;
            _uow = uow;
            _FBB_CFG_LOV = fbb_CFG_LOV;
        }

        public List<PackageModel> Handle(GetPackageListBySFFPromoForChangProQuery query)
        {

            InterfaceLogCommand log = null;
            log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.TRANSACTION_ID, "GetPackageListBySFFPromoForChangPro", "GetPackageListBySFFPromoForChangProQueryHandler", null, "FBB|" + query.FULL_URL, "");
            List<PackageModel> packages = new List<PackageModel>();
            try
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                ServicePointManager.ServerCertificateValidationCallback =
                    (s, certificate, chain, sslPolicyErrors) => true;

                using (var service = new SBNV2WebService.AIRInterfaceWorkflowServiceService())
                {
                    service.Timeout = 600000;

                    string tmpUrl = (from r in _FBB_CFG_LOV.Get()
                                     where r.LOV_NAME == "SaveOrderNewURL" && r.ACTIVEFLAG == "Y"
                                     select r.LOV_VAL1).FirstOrDefault().ToSafeString();
                    if (tmpUrl != "")
                    {
                        service.Url = tmpUrl;
                    }

                    var data = service.getListPackageBySFFPromo(query.P_SFF_PROMOCODE, query.P_PRODUCT_SUBTYPE, query.P_OWNER_PRODUCT, query.EXISTING_REQ);

                    if (data.RETURN_CODE == 0 && null != data.listPackageBySffPromoResult)
                    {
                        packages = data.listPackageBySffPromoResult.Select(s => new PackageModel
                        {
                            MAPPING_CODE = s.MAPPING_CODE.ToSafeString(),
                            PACKAGE_SERVICE_CODE = s.PACKAGE_SERVICE_CODE.ToSafeString(),
                            SFF_PROMOTION_CODE = s.SFF_PROMOTION_CODE.ToSafeString(),
                            SFF_PRODUCT_NAME = s.SFF_PRODUCT_NAME.ToSafeString(),
                            PRICE_CHARGE = s.PRICE_CHARGE.ToSafeDecimal(),
                            PRE_PRICE_CHARGE = s.PRE_PRICE_CHARGE.ToSafeDecimal(),
                            SFF_WORD_IN_STATEMENT_THA = s.SFF_WORD_IN_STATEMENT_THA.ToSafeString(),
                            SFF_WORD_IN_STATEMENT_ENG = s.SFF_WORD_IN_STATEMENT_ENG.ToSafeString(),
                            PACKAGE_DISPLAY_THA = s.PACKAGE_DISPLAY_THA.ToSafeString(),
                            PACKAGE_DISPLAY_ENG = s.PACKAGE_DISPLAY_ENG.ToSafeString(),
                            PACKAGE_TYPE_DESC = s.PACKAGE_TYPE_DESC.ToSafeString(),
                            PACKAGE_SERVICE_NAME = s.PACKAGE_SERVICE_NAME.ToSafeString(),
                            DOWNLOAD_SPEED = s.DOWNLOAD_SPEED.ToSafeString(),
                            UPLOAD_SPEED = s.UPLOAD_SPEED.ToSafeString(),
                            PACKAGE_TYPE = s.PACKAGE_TYPE.ToSafeString(),
                            PRODUCT_SUBTYPE = s.PRODUCT_SUBTYPE.ToSafeString(),
                            OWNER_PRODUCT = s.OWNER_PRODUCT.ToSafeString(),
                            SERVICE_CODE = s.SERVICE_CODE.ToSafeString(),
                            PRODUCT_SUBTYPE3 = s.PRODUCT_SUBTYPE3.ToSafeString()
                        }).ToList();
                        InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, packages, log, "Success", "", "");
                    }
                    else if (data.RETURN_MESSAGE == null)
                    {
                        InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, packages, log, "Success", "", "");

                    }
                    else
                    {
                        InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, "ExecuteReadStoredProc Failed:" + data.RETURN_MESSAGE, log, "Failed", "ExecuteReadStoredProc Failed: " + data.RETURN_MESSAGE, "");
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.Error(ex.GetErrorMessage());
                _logger.Error("Error from GetPackageListBySFFPromoForChangProQueryHandler : " + ex.RenderExceptionMessage());

                if (null != log)
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex.RenderExceptionMessage(), log, "Failed", ex.GetErrorMessage(), "");
                }
            }

            return packages;
        }

    }

    public class GetDataDATAILChangeproQueryHandler : IQueryHandler<GetDataDATAILChangeproQuery, DataDATAILChangepro>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<object> _dataForAppointment;

        public GetDataDATAILChangeproQueryHandler(ILogger logger,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IWBBUnitOfWork uow,
            IEntityRepository<object> dataForAppointment)
        {
            _logger = logger;
            _intfLog = intfLog;
            _uow = uow;
            _dataForAppointment = dataForAppointment;
        }

        public DataDATAILChangepro Handle(GetDataDATAILChangeproQuery query)
        {
            DataDATAILChangepro dataDATAILChangepro = dataDATAILChangepro = new DataDATAILChangepro();

            InterfaceLogCommand log = null;
            log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.TRANSACTION_ID, "GetDataDATAILChangepro", "GetDataDATAILChangeproQueryHandler", null, "FBB|" + query.FULL_URL, "");

            try
            {
                var P_BILL_CYCLE = new OracleParameter();
                P_BILL_CYCLE.ParameterName = "P_BILL_CYCLE";
                P_BILL_CYCLE.OracleDbType = OracleDbType.Varchar2;
                P_BILL_CYCLE.Size = 2000;
                P_BILL_CYCLE.Direction = ParameterDirection.Input;
                P_BILL_CYCLE.Value = query.P_BILL_CYCLE;

                var P_APPOINTMENT_DATE = new OracleParameter();
                P_APPOINTMENT_DATE.ParameterName = "P_APPOINTMENT_DATE";
                P_APPOINTMENT_DATE.OracleDbType = OracleDbType.Varchar2;
                P_APPOINTMENT_DATE.Size = 2000;
                P_APPOINTMENT_DATE.Direction = ParameterDirection.Input;
                P_APPOINTMENT_DATE.Value = query.P_APPOINTMENT_DATE;

                var P_TIME_SLOT = new OracleParameter();
                P_TIME_SLOT.ParameterName = "P_TIME_SLOT";
                P_TIME_SLOT.OracleDbType = OracleDbType.Varchar2;
                P_TIME_SLOT.Size = 2000;
                P_TIME_SLOT.Direction = ParameterDirection.Input;
                P_TIME_SLOT.Value = query.P_TIME_SLOT;

                var O_START_DATE = new OracleParameter();
                O_START_DATE.ParameterName = "O_START_DATE";
                O_START_DATE.Size = 2000;
                O_START_DATE.OracleDbType = OracleDbType.Varchar2;
                O_START_DATE.Direction = ParameterDirection.Output;

                var O_INSTALL_DATE = new OracleParameter();
                O_INSTALL_DATE.ParameterName = "O_INSTALL_DATE";
                O_INSTALL_DATE.Size = 2000;
                O_INSTALL_DATE.OracleDbType = OracleDbType.Varchar2;
                O_INSTALL_DATE.Direction = ParameterDirection.Output;

                var RETURN_CODE = new OracleParameter();
                RETURN_CODE.ParameterName = "RETURN_CODE";
                RETURN_CODE.OracleDbType = OracleDbType.Decimal;
                RETURN_CODE.Direction = ParameterDirection.Output;

                var RETURN_MESSAGE = new OracleParameter();
                RETURN_MESSAGE.ParameterName = "RETURN_MESSAGE";
                RETURN_MESSAGE.Size = 2000;
                RETURN_MESSAGE.OracleDbType = OracleDbType.Varchar2;
                RETURN_MESSAGE.Direction = ParameterDirection.Output;

                //listDataForAppointment 

                var result = _dataForAppointment.ExecuteStoredProc("WBB.PKG_CHANGEPRO_APPOINTMENT.DATAIL_CHANGEPRO",
                new
                {
                    P_BILL_CYCLE,
                    P_APPOINTMENT_DATE,
                    P_TIME_SLOT,
                    O_START_DATE,
                    O_INSTALL_DATE,
                    RETURN_CODE,
                    RETURN_MESSAGE

                });

                string Return_Code = RETURN_CODE != null ? RETURN_CODE.Value.ToSafeString() : "-1";
                string Return_Message = RETURN_MESSAGE != null ? RETURN_MESSAGE.Value.ToSafeString() : "error";
                if (Return_Code != "-1")
                {
                    dataDATAILChangepro.START_DATE = O_START_DATE != null ? O_START_DATE.Value.ToSafeString() : "";
                    dataDATAILChangepro.INSTALL_DATE = O_INSTALL_DATE != null ? O_INSTALL_DATE.Value.ToSafeString() : "";
                    dataDATAILChangepro.RETURN_CODE = Return_Code;
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, dataDATAILChangepro, log, "Success", "", "");
                }
                else
                {
                    dataDATAILChangepro.START_DATE = "";
                    dataDATAILChangepro.INSTALL_DATE = "";
                    dataDATAILChangepro.RETURN_CODE = Return_Code;
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, Return_Message, log, "Failed", Return_Message, "");
                }

            }
            catch (Exception ex)
            {
                _logger.Error(ex.GetErrorMessage());
                _logger.Error("Error from GetDataDATAILChangeproQueryHandler : " + ex.RenderExceptionMessage());

                if (null != log)
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex.RenderExceptionMessage(), log, "Failed", ex.GetErrorMessage(), "");
                }
                dataDATAILChangepro.START_DATE = "";
                dataDATAILChangepro.INSTALL_DATE = "";
                dataDATAILChangepro.RETURN_CODE = "-1";
            }

            return dataDATAILChangepro;
        }
    }

}
