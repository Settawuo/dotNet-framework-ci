using Newtonsoft.Json;
using RestSharp;
using System;
using System.Data;
using System.Linq;
using System.Net;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.WebServices;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class GetListQueryConfigContractQueryHandler : IQueryHandler<GetListQueryConfigContractQuery, GetListQueryConfigContractModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_CFG_LOV> _cfgLov;

        public GetListQueryConfigContractQueryHandler(ILogger logger, IEntityRepository<FBB_INTERFACE_LOG> intfLog, IWBBUnitOfWork uow, IEntityRepository<FBB_CFG_LOV> cfgLov)
        {
            _logger = logger;
            _intfLog = intfLog;
            _uow = uow;
            _cfgLov = cfgLov;
        }

        public GetListQueryConfigContractModel Handle(GetListQueryConfigContractQuery query)
        {
            InterfaceLogCommand log = null;

            var StatusCode = "";
            var StatusDescription = "";
            var result = new GetListQueryConfigContractModel();

            try
            {
                var apiCongif = (from l in _cfgLov.Get()
                                 where l.LOV_NAME == "CONTRACT_DEVICE_BY_TDM_URL"
                                 && l.LOV_TYPE == "CONFIG"
                                 && l.ACTIVEFLAG == "Y"
                                 select new { l.LOV_VAL1, l.LOV_VAL2 });
                query.Request_Url = apiCongif.FirstOrDefault().LOV_VAL1.ToSafeString();
                string useSecurityProtocol = apiCongif.FirstOrDefault().LOV_VAL2.ToSafeString();

                var client = new RestClient(query.Request_Url);
                var request = new RestRequest();
                request.Method = Method.POST;

                var objBody = new ListQueryConfigContractBody
                {
                    contractName = query.contract_name.ToSafeString(),
                    contractId = query.contract_id.ToSafeString()
                };

                string BodyStr = JsonConvert.SerializeObject(objBody);
                query.BodyJson = BodyStr;
                request.AddParameter("application/json", query.BodyJson, ParameterType.RequestBody);

                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.TransactionId, "listQueryConfigContract", "GetListQueryConfigContractQueryHandler", null, "FBB", "");
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
                        result = JsonConvert.DeserializeObject<GetListQueryConfigContractModel>(response.Content) ?? new GetListQueryConfigContractModel();
                        if (result != null)
                        {
                            if (result.resultCode == "20000" || result.resultCode == "50000")
                            {
                                StatusCode = "Success";
                                StatusDescription = result.resultDescription;
                            }
                        }
                        else
                        {
                            StatusCode = "Failed";
                            StatusDescription = (!string.IsNullOrEmpty(response.ErrorMessage)
                                ? response.ErrorMessage
                                : response.Content).ToSafeString();
                        }
                    }
                    else
                    {
                        StatusCode = "Failed";
                        StatusDescription = (!string.IsNullOrEmpty(response.ErrorMessage)
                                ? response.ErrorMessage
                                : response.Content).ToSafeString();
                    }

                    if (string.IsNullOrEmpty(result?.resultCode))
                    {
                        result.resultCode = StatusCode;
                        result.resultDescription = StatusDescription;
                    }
                }
                catch (Exception ex)
                {
                    result.resultCode = "Failed";
                    result.resultDescription = ex.GetBaseException().ToString();
                    result.developerMessage = ex.GetBaseException().ToString();
                    _logger.Info("GetListQueryConfigContractQueryHandler Exception " + ex.GetErrorMessage());
                }
                finally
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, StatusCode, StatusDescription, "");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public class ListQueryConfigContractBody
        {
            public string contractName { get; set; }
            public string contractId { get; set; }

        }
    }
}
