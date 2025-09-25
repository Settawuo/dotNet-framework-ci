using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Linq;
using System.Net;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.WebServices;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Models;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class CheckCoverage3bbQueryHandler : IQueryHandler<CheckCoverage3bbQuery, CheckCoverage3bbQueryModel>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<string> _objService;
        private readonly IEntityRepository<FBB_CFG_LOV> _cfgLov;

        public CheckCoverage3bbQueryHandler(ILogger logger,
            IWBBUnitOfWork uow,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IEntityRepository<string> objService,
            IEntityRepository<FBB_CFG_LOV> cfgLov)
        {
            _logger = logger;
            _uow = uow;
            _intfLog = intfLog;
            _objService = objService;
            _cfgLov = cfgLov;
        }

        public CheckCoverage3bbQueryModel Handle(CheckCoverage3bbQuery query)
        {
            InterfaceLogCommand log = null;
            CheckCoverage3bbQueryModel result = new CheckCoverage3bbQueryModel();

            try
            {
                Guid myuuid = Guid.NewGuid();
                string myuuidAsString = myuuid.ToString();

                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.TRANSACTION_ID,
                    "CheckCoverage3bbQuery", "CheckCoverage3bbQueryHandler", "", "FBB", "");

                //"Get apiCongif"
                string tmpUrl = @"https://3bb-coverage-stag.triplet.co.th/network/coverage/service/v1/coverage/check?";
                string tmpUser = "ais-coverage";
                string tmpPass = "b5A3bU4siNXKokMemBMTGiadx2l9x44h";

                var apiCongifUrl = (from l in _cfgLov.Get()
                                    where l.LOV_NAME == "CheckCoverage3bbUrl"
                                    && l.ACTIVEFLAG == "Y"
                                    select new { l.LOV_VAL1 });
                var apiCongifUser = (from l in _cfgLov.Get()
                                     where l.LOV_NAME == "CheckCoverage3bbUser"
                                     && l.ACTIVEFLAG == "Y"
                                     select new { l.LOV_VAL1, l.LOV_VAL2 });

                if (apiCongifUrl != null && apiCongifUser != null && apiCongifUrl.Count() > 0 && apiCongifUser.Count() > 0)
                {
                    tmpUrl = apiCongifUrl.FirstOrDefault().LOV_VAL1;
                    tmpUser = apiCongifUser.FirstOrDefault().LOV_VAL1;
                    tmpPass = apiCongifUser.FirstOrDefault().LOV_VAL2;
                }

                tmpUrl = tmpUrl + "latitude=" + query.latitude + "&longitude=" + query.longitude + "&transactionId=" + myuuidAsString;

                var client = new RestClient(tmpUrl);
                client.Authenticator = new HttpBasicAuthenticator(tmpUser, tmpPass);
                var request = new RestRequest();
                request.Method = Method.GET;
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
                        result = JsonConvert.DeserializeObject<CheckCoverage3bbQueryModel>(response.Content) ?? new CheckCoverage3bbQueryModel();
                        InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, content, log,
                        result.returnCode == "00000" ? "Success" : "Failed", "", "");
                    }
                    catch
                    {
                    }
                }
                else
                {
                    result.returnMessage = "response is null.";
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, "", log, "Failed", result.returnMessage, "");
                }
            }
            catch (Exception ex)
            {
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, "", log, "Failed", ex.GetBaseException().ToString(), "");
            }

            return result;
        }

    }
}
