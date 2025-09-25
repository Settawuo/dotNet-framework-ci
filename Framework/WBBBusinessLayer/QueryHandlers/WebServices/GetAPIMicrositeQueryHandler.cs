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
    public class GetAPIMicrositeQueryHandler : IQueryHandler<GetAPIMicrositeQuery, GetAPIMicrositeModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_CFG_LOV> _cfgLov;

        public GetAPIMicrositeQueryHandler(ILogger logger, IEntityRepository<FBB_INTERFACE_LOG> intfLog, IWBBUnitOfWork uow, IEntityRepository<FBB_CFG_LOV> cfgLov)
        {
            _logger = logger;
            _intfLog = intfLog;
            _uow = uow;
            _cfgLov = cfgLov;
        }

        public GetAPIMicrositeModel Handle(GetAPIMicrositeQuery query)
        {
            var StatusCode = "";
            var StatusMessage = "";
            var TransactionID = "";

            var result = new GetAPIMicrositeModel();
            try
            {
                var apiCongif = (from l in _cfgLov.Get()
                                 where l.LOV_TYPE == "FBB_CONFIG_MICROSITE"
                                 && l.LOV_NAME == "API_URL"
                                 && l.ACTIVEFLAG == "Y"
                                 select new { l.LOV_VAL1, l.LOV_VAL2 });
                string FullURL = apiCongif.FirstOrDefault().LOV_VAL1.ToSafeString();
                string useSecurityProtocol = apiCongif.FirstOrDefault().LOV_VAL2.ToSafeString();

                var randomX = new Random();

                query.Transaction_Id = DateTime.Now.ToString("yyyyMMddHHmmssfff") + randomX.Next(9999).ToString();
                query.App_Source = "WBB";
                query.App_Destination = "FBBSaleportal,IM";
                query.Content_Type = "application/json; charset=utf-8";

                var client = new RestClient(FullURL);
                var request = new RestRequest();
                request.Method = Method.POST;
                request.AddHeader("TRANSACTION-ID", query.Transaction_Id);
                request.AddHeader("App-Source", query.App_Source);
                request.AddHeader("App-Destination", query.App_Destination);
                request.AddHeader("ContentType", query.Content_Type);
                request.AddParameter("application/json", query.BodyJson, ParameterType.RequestBody);

                var log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.Transaction_Id, "APIMicrosite", "GetAPIMicrositeQueryHandler", null, "FBB", "");
                try
                {

                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

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
                        result = JsonConvert.DeserializeObject<GetAPIMicrositeModel>(response.Content) ?? new GetAPIMicrositeModel();
                        if (result != null)
                        {
                            StatusCode = result.RESULT_CODE.ToSafeString();
                            StatusMessage = result.RESULT_DESC.ToSafeString();
                            result.TRANSACTIONID = query.Transaction_Id.ToSafeString();
                        }
                        else
                        {
                            StatusCode = "Failed";
                            StatusMessage = (!string.IsNullOrEmpty(response.ErrorMessage)
                                ? response.ErrorMessage
                                : response.Content).ToSafeString();
                            result.TRANSACTIONID = query.Transaction_Id.ToSafeString();
                        }
                    }
                    else
                    {
                        StatusCode = "Failed";
                        StatusMessage = (!string.IsNullOrEmpty(response.ErrorMessage)
                                ? response.ErrorMessage
                                : response.Content).ToSafeString();
                        result.TRANSACTIONID = query.Transaction_Id.ToSafeString();
                    }

                    if (string.IsNullOrEmpty(result?.RESULT_CODE))
                    {
                        result.RESULT_CODE = StatusCode;
                        result.RESULT_DESC = StatusMessage;
                        result.TRANSACTIONID = TransactionID.ToSafeString();
                    }
                }
                catch (Exception ex)
                {
                    StatusCode = "Failed";
                    StatusMessage = ex.GetBaseException().ToString();
                    TransactionID = "";
                    _logger.Info("GetAPIMicrositeQueryHandler Exception " + ex.GetErrorMessage());
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
