using Newtonsoft.Json;
using RestSharp;
using System;
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
    public class EncryptIntraAISServiceHandler : IQueryHandler<EncryptIntraAISServiceQuery, EncryptIntraAISServiceModel>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<string> _objService;

        public EncryptIntraAISServiceHandler(ILogger logger,
            IWBBUnitOfWork uow,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IEntityRepository<string> objService)
        {
            _logger = logger;
            _uow = uow;
            _intfLog = intfLog;
            _objService = objService;
        }

        public EncryptIntraAISServiceModel Handle(EncryptIntraAISServiceQuery query)
        {
            InterfaceLogCommand log = null;
            EncryptIntraAISServiceModel result = new EncryptIntraAISServiceModel();

            try
            {
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, !string.IsNullOrEmpty(query.p_non_mobile_no) ? query.p_non_mobile_no : query.p_transaction_id,
                    "EncryptIntraAISService", "EncryptIntraAISServiceHandler", "", "FBB", "");
                var client = new RestClient(query.Url);
                //client.Authenticator = new HttpBasicAuthenticator(query.username, query.password);
                var request = new RestRequest();
                request.Method = Method.POST;
                request.AddJsonBody(query.body);
                // execute the request

                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                ServicePointManager.ServerCertificateValidationCallback = (s, certificate, chain, sslPolicyErrors) => true;

                var response = client.Execute(request);

                var content = response.Content; // raw content as string

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    try
                    {
                        result = JsonConvert.DeserializeObject<EncryptIntraAISServiceModel>(response.Content) ?? new EncryptIntraAISServiceModel();
                        InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, content, log,
                        result.resultcode == "1" ? "Success" : "Failed", "", "");
                    }
                    catch
                    {
                        var StatusMessage1 = (!string.IsNullOrEmpty(response.ErrorMessage)
                        ? response.ErrorMessage
                        : response.Content).ToSafeString();
                        InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, response.Content, log,
                        "Failed", StatusMessage1, "");
                    }
                }
                else
                {
                    var StatusMessage2 = (!string.IsNullOrEmpty(response.ErrorMessage)
                        ? response.ErrorMessage
                        : response.Content).ToSafeString();

                    result.errmsg = StatusMessage2;
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, "", log, "Failed", StatusMessage2, "");
                }

            }
            catch (Exception ex)
            {
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, "", log, "Failed", ex.GetBaseException().ToString(), "");
                _logger.Info(ex.GetErrorMessage());
            }

            return result;
        }

    }

}
