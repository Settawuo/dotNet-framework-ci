using Newtonsoft.Json;
using RestSharp;
using System;
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
    public class GetOnlineQueryMobileInfoHandler : IQueryHandler<GetOnlineQueryMobileInfoQuery, GetOnlineQueryMobileInfoModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_CFG_LOV> _cfgLov;

        public GetOnlineQueryMobileInfoHandler(ILogger logger, IEntityRepository<FBB_INTERFACE_LOG> intfLog, IWBBUnitOfWork uow, IEntityRepository<FBB_CFG_LOV> cfgLov)
        {
            _logger = logger;
            _intfLog = intfLog;
            _uow = uow;
            _cfgLov = cfgLov;
        }

        public GetOnlineQueryMobileInfoModel Handle(GetOnlineQueryMobileInfoQuery query)
        {
            var StatusCode = "";
            var StatusMessage = "";

            var result = new GetOnlineQueryMobileInfoModel();
            try
            {
                var apiCongif = (from l in _cfgLov.Get()
                                 where l.LOV_NAME == "MOBILE_INFO_BY_ONLINE_QUERY_URL"
                                 && l.ACTIVEFLAG == "Y"
                                 select new { l.LOV_VAL1, l.LOV_VAL2 });
                query.Request_Url = apiCongif.FirstOrDefault().LOV_VAL1.ToSafeString();
                string useSecurityProtocol = apiCongif.FirstOrDefault().LOV_VAL2.ToSafeString();

                var apiHeader = from l in _cfgLov.Get()
                                where l.LOV_NAME == "MOBILE_INFO_BY_ONLINE_QUERY_HEADER"
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

                string BodyStr = JsonConvert.SerializeObject(query.Body);
                query.BodyJson = BodyStr;
                request.AddParameter("application/json", query.BodyJson, ParameterType.RequestBody);

                var log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, !string.IsNullOrEmpty(query.Internet_No) ? query.Internet_No : query.Transaction_Id, "GetOnlineQueryMobileInfo", "GetOnlineQueryMobileInfoHandler", null, "FBB", "");
                try
                {
                    // execute the request

                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    // useSecurityProtocol = "Y";
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
                        result = JsonConvert.DeserializeObject<GetOnlineQueryMobileInfoModel>(response.Content) ?? new GetOnlineQueryMobileInfoModel();
                        if (result != null)
                        {
                            if (result.RESULT_CODE == "20000" || result.RESULT_CODE == "40001")
                            {
                                StatusCode = "Success";
                            }
                            else
                            {
                                StatusCode = "Failed";
                            }

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
                    _logger.Info("GetOnlineQueryMobileInfoHandler Exception " + ex.GetErrorMessage());
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
    }

    public class GetOnlineQueryPackPenaltyHandler : IQueryHandler<GetOnlineQueryPackPenaltyQuery, GetOnlineQueryPackPenaltyModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_CFG_LOV> _cfgLov;

        public GetOnlineQueryPackPenaltyHandler(ILogger logger, IEntityRepository<FBB_INTERFACE_LOG> intfLog, IWBBUnitOfWork uow, IEntityRepository<FBB_CFG_LOV> cfgLov)
        {
            _logger = logger;
            _intfLog = intfLog;
            _uow = uow;
            _cfgLov = cfgLov;
        }

        public GetOnlineQueryPackPenaltyModel Handle(GetOnlineQueryPackPenaltyQuery query)
        {
            var StatusCode = "";
            var StatusMessage = "";

            GetOnlineQueryPackPenaltyModel result = new GetOnlineQueryPackPenaltyModel();
            try
            {
                var apiCongif = (from l in _cfgLov.Get()
                                 where l.LOV_NAME == "PACK_PENALTY_BY_ONLINE_QUERY_URL"
                                 && l.ACTIVEFLAG == "Y"
                                 select new { l.LOV_VAL1, l.LOV_VAL2 });
                query.Request_Url = apiCongif.FirstOrDefault().LOV_VAL1.ToSafeString();
                string useSecurityProtocol = apiCongif.FirstOrDefault().LOV_VAL2.ToSafeString();

                var apiHeader = from l in _cfgLov.Get()
                                where l.LOV_NAME == "PACK_PENALTY_BY_ONLINE_QUERY_HEADER"
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

                string BodyStr = JsonConvert.SerializeObject(query.Body);
                query.BodyJson = BodyStr;
                request.AddParameter("application/json", query.BodyJson, ParameterType.RequestBody);

                var log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, !string.IsNullOrEmpty(query.Internet_No) ? query.Internet_No : query.Transaction_Id,
                    "GetOnlineQueryPackPenalty", "GetOnlineQueryPackPenaltyHandler", null, "FBB", "");
                try
                {
                    // execute the request
                    if (useSecurityProtocol == "Y")
                    {
                        ServicePointManager.Expect100Continue = true;
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                        ServicePointManager.ServerCertificateValidationCallback =
                            (s, certificate, chain, sslPolicyErrors) => true;
                    }

                    var response = client.Execute(request);

                    var content = response.Content; //raw content as string

                    if (HttpStatusCode.OK.Equals(response.StatusCode))
                    {
                        result = JsonConvert.DeserializeObject<GetOnlineQueryPackPenaltyModel>(response.Content) ?? new GetOnlineQueryPackPenaltyModel();
                        if (result != null)
                        {
                            if (result.RESULT_CODE == "20000" || result.RESULT_CODE == "40001")
                            {
                                StatusCode = "Success";
                            }
                            else
                            {
                                StatusCode = "Failed";
                            }

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
                    _logger.Info("GetOnlineQueryPackPenaltyHandler Exception " + ex.GetErrorMessage());
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
    }
}
