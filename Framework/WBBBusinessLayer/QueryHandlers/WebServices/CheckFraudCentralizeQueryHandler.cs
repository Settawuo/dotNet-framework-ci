using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
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
    public class CheckFraudCentralizeQueryHandler : IQueryHandler<CheckFraudCentralizeQuery, CheckFraudCentralizeQueryModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_CFG_LOV> _cfgLov;

        public CheckFraudCentralizeQueryHandler(ILogger logger, IEntityRepository<FBB_INTERFACE_LOG> intfLog, IWBBUnitOfWork uow, IEntityRepository<FBB_CFG_LOV> cfgLov)
        {
            _logger = logger;
            _intfLog = intfLog;
            _uow = uow;
            _cfgLov = cfgLov;
        }

        //R23.05 CheckFraud
        public CheckFraudCentralizeQueryModel Handle(CheckFraudCentralizeQuery query)
        {
            //for test
            List<string> configList = new List<string> { "2023#FEB@0987654321", "fbbworkflow", "https://dev-airnetregws.ais.co.th:8543/airnet-service/restext/fraudCentralize/verify" };
            var errorMsg = "";
            try
            {
                configList = GetFruadWorkFlowConfig();
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
            }
            //for test

            //for test //var configList = GetFruadWorkFlowConfig();
            var url = configList.Where(w => w.Contains("fraudCentralize")).FirstOrDefault();
            var client = new RestClient(url);
            var request = RequestBuilder(query, configList, url);
            var log = CreateFraudCentralizeLog(query);
            var response = client.Execute(request);
            var result = CheckStatus(response, log);
            
            //for test 
            if (errorMsg != "")
            {
                result.returnCode = errorMsg;
            }
            //for test

            return result;
        }


        private List<string> GetFruadWorkFlowConfig()
            => _cfgLov.Get(l => (l.LOV_NAME == "URL_FRAUD_NOGO" || l.LOV_NAME.Contains("FRAUD_AUTH")) && l.ACTIVEFLAG == "Y").Select(s => s.LOV_VAL1).ToList();

        private RestRequest RequestBuilder(CheckFraudCentralizeQuery query, List<string> config, string url)
        {
            var request = new RestRequest();
            request.Method = Method.POST;
            string auth = GetBasicAuth(config, url);
            string bodyStr = JsonConvert.SerializeObject(query);

            request.AddHeader("Authorization", $"Basic {auth}");
            request.AddParameter("application/json", bodyStr, ParameterType.RequestBody);

            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback = (s, certificate, chain, sslPolicyErrors) => true;

            return request;
        }

        private string GetBasicAuth(List<string> config, string url)
        {
            var clientId = config.Where(w => w.Contains("fbbworkflow")).FirstOrDefault();
            var clientSecret = config.Where(w => !w.Contains("fbbworkflow") || !w.Contains(url)).FirstOrDefault();
            var authenticationString = $"{clientId}:{clientSecret}";
            var base64EncodedAuthenticationString = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(authenticationString));
            return base64EncodedAuthenticationString;
        }

        private InterfaceLogCommand CreateFraudCentralizeLog(CheckFraudCentralizeQuery query)
        => InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.Id_card_no, "CheckFraudCentralize", "CheckFraudCentralizeQueryHandler", null, "FBB", "");

        private CheckFraudCentralizeQueryModel CheckStatus(IRestResponse response, InterfaceLogCommand log)
        {
            var StatusCode = "Failed";
            var StatusMessage = "";
            var result = new CheckFraudCentralizeQueryModel();

            try
            {
                result = JsonConvert.DeserializeObject<CheckFraudCentralizeQueryModel>(response.Content);

                if (!HttpStatusCode.OK.Equals(response.StatusCode) || result == null)
                {
                    StatusMessage = (string.IsNullOrEmpty(response.ErrorMessage)
                            ? response.Content
                            : response.ErrorMessage).ToSafeString();
                }

                if (result.returnCode == "20000" || result.returnCode == "00000")
                {
                    StatusCode = "Success";
                    StatusMessage = result.returnMessage;
                }

                if (string.IsNullOrEmpty(result?.returnCode))
                {
                    result.returnCode = StatusCode;
                    result.returnMessage = StatusMessage;
                }
            }
            catch (Exception ex)
            {
                StatusMessage = ex.GetBaseException().ToString();
                _logger.Info("CheckFraudCentralizeQueryHandler Exception " + ex.GetErrorMessage());
            }
            finally
            {
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, StatusCode, StatusMessage, "");
            }

            return result;
        }
    }
}
