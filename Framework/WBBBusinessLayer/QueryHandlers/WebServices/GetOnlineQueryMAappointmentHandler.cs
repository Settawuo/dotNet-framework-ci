using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using WBBContract;
using WBBContract.Queries.WebServices;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class GetOnlineQueryMAappointmentHandler : IQueryHandler<GetOnlineQueryMAappointmentQuery, GetOnlineQueryMAappointmentModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_CFG_LOV> _cfgLov;

        public GetOnlineQueryMAappointmentHandler(ILogger logger, IEntityRepository<FBB_INTERFACE_LOG> intfLog, IWBBUnitOfWork uow, IEntityRepository<FBB_CFG_LOV> cfgLov)
        {
            _logger = logger;
            _intfLog = intfLog;
            _uow = uow;
            _cfgLov = cfgLov;
        }

        public GetOnlineQueryMAappointmentModel Handle(GetOnlineQueryMAappointmentQuery query)
        {
            var StatusCode = "";
            var StatusMessage = "";

            var result = new GetOnlineQueryMAappointmentModel();
            try
            {
                var apiCongif = (from l in _cfgLov.Get()
                                 where l.LOV_NAME == "APPOINTMENT_BY_ONLINE_QUERY_URL"
                                 && l.ACTIVEFLAG == "Y"
                                 select new { l.LOV_VAL1, l.LOV_VAL2 });
                query.Request_Url = apiCongif.FirstOrDefault().LOV_VAL1.ToSafeString();
                string useSecurityProtocol = apiCongif.FirstOrDefault().LOV_VAL2.ToSafeString();

                var apiHeader = from l in _cfgLov.Get()
                                where l.LOV_NAME == "APPOINTMENT_BY_ONLINE_QUERY_HEADER"
                                && l.ACTIVEFLAG == "Y"
                                select l;

                var client = new RestClient(query.Request_Url);
                var request = new RestRequest();
                request.Method = Method.POST;

                if (apiHeader.Any())
                {
                    var hName = string.Empty;
                    var hValue = string.Empty;

                    foreach (var h in apiHeader)
                    {
                        if (!string.IsNullOrEmpty(h.DISPLAY_VAL))
                        {
                            hName = h.DISPLAY_VAL;
                            hValue = h.LOV_VAL1;
                            if (h.DISPLAY_VAL == "x-online-query-transaction-id")
                            {
                                var dateformat = DateTime.Now.ToString(h.LOV_VAL2);

                                var r = new Random();
                                var rMin = 0;
                                var rMax = h.LOV_VAL3.ToSafeInteger();
                                var uuid = r.Next(rMin, rMax).ToString("D4");

                                hValue = h.LOV_VAL1;
                                hValue = hValue.Replace("{dateformat}", dateformat);
                                hValue = hValue.Replace("{uuid}", uuid);
                            }
                            request.AddHeader(hName, hValue);
                        }
                    }
                }

                var plist = new OnlineQueryMAParameterList
                {
                    ParameterType = "",
                    Parameter = new[]
                    {

                        new OnlineQueryMAParameters {Name = "ACCESS_NO", Value = query.Body.AccessNo.ToSafeString()},
                        new OnlineQueryMAParameters {Name = "MA_DATE", Value = query.Body.MADate.ToSafeString()},
                        new OnlineQueryMAParameters {Name = "ACCESS_MODE", Value = query.Body.AccessMode.ToSafeString()},
                        new OnlineQueryMAParameters {Name = "DAYS", Value = query.Body.Days.ToSafeString()},
                        new OnlineQueryMAParameters {Name = "SYMPTOM_CODE", Value = query.Body.SymptomCode.ToSafeString()},
                        new OnlineQueryMAParameters {Name = "SERVICE_LEVEL", Value = query.Body.ServiceLevel.ToSafeString()},
                        new OnlineQueryMAParameters {Name = "TIME_ADD", Value = query.Body.TimeAdd.ToSafeString()},
                        new OnlineQueryMAParameters {Name = "ACTION_TIME_SLOT", Value = query.Body.ActionTimeSlot.ToSafeString()},
                        new OnlineQueryMAParameters {Name = "NUM_TIME_SLOT", Value = query.Body.NumTimeSlot.ToSafeString()}
                    },
                };

                var objRequest = new OnlineQueryMARequest
                {
                    Event = "evQueryMAappointment",
                    ParameterList = plist
                };
                var objBody = new OnlineQueryMAappointmentBody
                {
                    onlinequeryrequest = objRequest,
                };

                string BodyStr = JsonConvert.SerializeObject(objBody);
                query.BodyJson = BodyStr;
                request.AddParameter("application/json", query.BodyJson, ParameterType.RequestBody);

                var log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, !string.IsNullOrEmpty(query.Internet_No) ? query.Internet_No : query.Transaction_Id, "GetOnlineQueryMAappointment", "GetOnlineQueryMAappointmentHandler", null, "FBB", "");
                try
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                    // execute the request
                    //useSecurityProtocol = "Y";
                    if (useSecurityProtocol == "Y")
                    {
                        ServicePointManager.Expect100Continue = true;
                        ServicePointManager.ServerCertificateValidationCallback =
                            (s, certificate, chain, sslPolicyErrors) => true;
                    }

                    var response = client.Execute(request);

                    var content = response.Content; //raw content as string

                    if (HttpStatusCode.OK.Equals(response.StatusCode))
                    {
                        result = JsonConvert.DeserializeObject<GetOnlineQueryMAappointmentModel>(response.Content) ?? new GetOnlineQueryMAappointmentModel();
                        if (result != null)
                        {
                            StatusCode = result.RESULT_CODE == "20000" ? "Success" : "Failed";
                            StatusMessage = result.RESULT_CODE == "20000" ? "" : result.RESULT_DESC;
                        }
                        else
                        {
                            StatusCode = "Failed";
                            StatusMessage = (!string.IsNullOrEmpty(response.ErrorMessage)
                                ? response.ErrorMessage
                                : response.Content).ToSafeString();
                        }
                    }
                    else
                    {
                        StatusCode = "Failed";
                        StatusMessage = (!string.IsNullOrEmpty(response.ErrorMessage)
                                ? response.ErrorMessage
                                : response.Content).ToSafeString();
                    }

                    if (string.IsNullOrEmpty(result?.RESULT_CODE))
                    {
                        result.RESULT_CODE = StatusCode;
                        result.RESULT_DESC = StatusMessage;
                    }
                }
                catch (Exception ex)
                {
                    StatusCode = "Failed";
                    StatusMessage = ex.GetBaseException().ToString();
                    _logger.Info("GetOnlineQueryMAappointmentHandler Exception " + ex.GetErrorMessage());
                }
                finally
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, StatusCode, StatusMessage, "");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public class OnlineQueryMAappointmentBody
        {
            public OnlineQueryMARequest onlinequeryrequest { get; set; }
        }

        public class OnlineQueryMARequest
        {
            public string Event { get; set; }
            public OnlineQueryMAParameterList ParameterList { get; set; }
        }

        public class OnlineQueryMAParameterList
        {
            public string ParameterType { get; set; }
            public OnlineQueryMAParameters[] Parameter { get; set; }
        }

        public class OnlineQueryMAParameters
        {
            public string Name { get; set; }
            public string Value { get; set; }
            public List<OnlineQueryMAParameter>[] Values { get; set; }
        }

        public class OnlineQueryMAParameter
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }
    }
}
