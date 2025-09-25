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
    public class GetOnlineQueryAppointmentHandler : IQueryHandler<GetOnlineQueryAppointmentQuery, GetOnlineQueryAppointmentModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_CFG_LOV> _cfgLov;

        public GetOnlineQueryAppointmentHandler(ILogger logger, IEntityRepository<FBB_INTERFACE_LOG> intfLog, IWBBUnitOfWork uow, IEntityRepository<FBB_CFG_LOV> cfgLov)
        {
            _logger = logger;
            _intfLog = intfLog;
            _uow = uow;
            _cfgLov = cfgLov;
        }

        public GetOnlineQueryAppointmentModel Handle(GetOnlineQueryAppointmentQuery query)
        {
            var StatusCode = "";
            var StatusMessage = "";

            var result = new GetOnlineQueryAppointmentModel();
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

                List<OnlineQueryParameter>[] assignCondition = null;
                var assignConditionStr = string.Empty;
                if (query.Body.ASSIGN_CONDITION_LIST.Any())
                {
                    var arrayAssignStr = new List<OnlineQueryParameter>[query.Body.ASSIGN_CONDITION_LIST.Count];
                    for (int i = 0; i < query.Body.ASSIGN_CONDITION_LIST.Count; i++)
                    {
                        var listAssign = new List<OnlineQueryParameter>();
                        listAssign.Add(new OnlineQueryParameter { Name = "ATTR_NAME", Value = query.Body.ASSIGN_CONDITION_LIST[i].ATTR_NAME });
                        listAssign.Add(new OnlineQueryParameter { Name = "VALUE", Value = query.Body.ASSIGN_CONDITION_LIST[i].VALUE });
                        arrayAssignStr[i] = listAssign;
                    }
                    assignCondition = arrayAssignStr;
                    //assignConditionStr = JsonConvert.SerializeObject(arrayAssignStr);
                }

                var plist = new OnlineQueryParameterList
                {
                    ParameterType = "",
                    Parameter = new[]
                    {
                        new OnlineQueryParameters {Name = "INSTALLATION_DATE", Value = query.Body.InstallationDate.ToSafeString()},
                        new OnlineQueryParameters {Name = "PROD_SPEC_CODE", Value = query.Body.ProductSpecCode.ToSafeString()},
                        new OnlineQueryParameters {Name = "ACCESS_MODE", Value = query.Body.AccessMode.ToSafeString()},
                        new OnlineQueryParameters {Name = "ADDRESS_ID", Value = query.Body.AddressId.ToSafeString()},
                        new OnlineQueryParameters {Name = "DAYS", Value = query.Body.Days.ToSafeString()},
                        new OnlineQueryParameters {Name = "SUBDISTRICT", Value = query.Body.SubDistrict.ToSafeString()},
                        new OnlineQueryParameters {Name = "POSTCODE", Value = query.Body.Postal_Code.ToSafeString()},
                        new OnlineQueryParameters {Name = "SUB_ACCESS_MODE", Value = query.Body.SubAccessMode.ToSafeString()},
                        new OnlineQueryParameters {Name = "TASK_TYPE", Value = string.IsNullOrEmpty(query.Body.TaskType) ? "INSTALL" : query.Body.TaskType.ToSafeString()},
                        //new SFFServices.Parameter {Name = "EXT_ATTR", Value = query.Internet_No.ToSafeString()}, //FBSS ส่งค่า NULL
                        new OnlineQueryParameters {Name = "ASSIGN_CONDITION_LIST", Values = assignCondition},
                        //R21.3
                        new OnlineQueryParameters {Name = "TIME_ADD", Value = query.Body.TimeAdd.ToSafeString()},
                        new OnlineQueryParameters {Name = "ACTION_TIME_SLOT", Value = query.Body.ActionTimeSlot.ToSafeString()},
                        new OnlineQueryParameters {Name = "NUM_TIME_SLOT", Value = query.Body.NumTimeSlot.ToSafeString()}
                    },
                };

                var objRequest = new OnlineQueryRequest
                {
                    Event = "evQueryAppointment",
                    ParameterList = plist
                };
                var objBody = new OnlineQueryAppointmentBody
                {
                    onlinequeryrequest = objRequest,
                };

                string BodyStr = JsonConvert.SerializeObject(objBody);
                query.BodyJson = BodyStr;
                request.AddParameter("application/json", query.BodyJson, ParameterType.RequestBody);

                var log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, !string.IsNullOrEmpty(query.Internet_No) ? query.Internet_No : query.Transaction_Id, "GetOnlineQueryAppointment", "GetOnlineQueryAppointmentHandler", null, "FBB", "");
                try
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    //useSecurityProtocol = "Y";
                    if (useSecurityProtocol == "Y")
                    {
                        // execute the request
                        ServicePointManager.Expect100Continue = true;
                        ServicePointManager.ServerCertificateValidationCallback =
                            (s, certificate, chain, sslPolicyErrors) => true;
                    }

                    var response = client.Execute(request);

                    var content = response.Content; //raw content as string

                    if (HttpStatusCode.OK.Equals(response.StatusCode))
                    {
                        result = JsonConvert.DeserializeObject<GetOnlineQueryAppointmentModel>(response.Content) ?? new GetOnlineQueryAppointmentModel();
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
                    _logger.Info("GetOnlineQueryAppointmentHandler Exception " + ex.GetErrorMessage());
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

        public class OnlineQueryAppointmentBody
        {
            public OnlineQueryRequest onlinequeryrequest { get; set; }
        }

        public class OnlineQueryRequest
        {
            public string Event { get; set; }
            public OnlineQueryParameterList ParameterList { get; set; }
        }

        public class OnlineQueryParameterList
        {
            public string ParameterType { get; set; }
            public OnlineQueryParameters[] Parameter { get; set; }
        }

        public class OnlineQueryParameters
        {
            public string Name { get; set; }
            public string Value { get; set; }
            public List<OnlineQueryParameter>[] Values { get; set; }
        }

        public class OnlineQueryParameter
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }
    }
}
