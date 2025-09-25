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
    public class ReservePort3bbQueryHandler : IQueryHandler<ReservePort3bbQuery, ReservePort3bbQueryModel>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<string> _objService;
        private readonly IEntityRepository<FBB_CFG_LOV> _cfgLov;

        public ReservePort3bbQueryHandler(ILogger logger,
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

        public ReservePort3bbQueryModel Handle(ReservePort3bbQuery query)
        {
            InterfaceLogCommand log = null;
            ReservePort3bbQueryModel result = new ReservePort3bbQueryModel();

            try
            {
                Guid myuuid = Guid.NewGuid();
                string myuuidAsString = myuuid.ToString();
                string mobileNo = query.transactionId;
                query.transactionId = "FBB_WBB_"+myuuidAsString;

                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, mobileNo,
                    "ReservePort3bbQuery", "ReservePort3bbQueryHandler", "", "FBB", "");

                //"Get apiCongif"
                string tmpUrl = @"https://3bb-coverage-stag.triplet.co.th/network/coverage/service/v1/port/reserve";
                string tmpUser = "ais-coverage";
                string tmpPass = "b5A3bU4siNXKokMemBMTGiadx2l9x44h";

                var apiCongifUrl = (from l in _cfgLov.Get()
                                    where l.LOV_NAME == "ReservePort3bbUrl"
                                    && l.ACTIVEFLAG == "Y"
                                    select new { l.LOV_VAL1 });
                var apiCongifUser = (from l in _cfgLov.Get()
                                     where l.LOV_NAME == "ReservePort3bbUser"
                                     && l.ACTIVEFLAG == "Y"
                                     select new { l.LOV_VAL1, l.LOV_VAL2 });

                if (apiCongifUrl != null && apiCongifUser != null && apiCongifUrl.Count() > 0 && apiCongifUser.Count() > 0)
                {
                    tmpUrl = apiCongifUrl.FirstOrDefault().LOV_VAL1;
                    tmpUser = apiCongifUser.FirstOrDefault().LOV_VAL1;
                    tmpPass = apiCongifUser.FirstOrDefault().LOV_VAL2;
                }

                string BodyStr = JsonConvert.SerializeObject(query);

                var client = new RestClient(tmpUrl);
                client.Authenticator = new HttpBasicAuthenticator(tmpUser, tmpPass);
                var request = new RestRequest();
                request.Method = Method.POST;
                request.AddParameter("application/json", BodyStr, ParameterType.RequestBody);
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
                        result = JsonConvert.DeserializeObject<ReservePort3bbQueryModel>(response.Content) ?? new ReservePort3bbQueryModel();
                        InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, content, log,
                        result.returnCode == "00000" ? "Success" : "Failed", "", "");
                    }
                    catch
                    {
                    }
                }
                else
                {
                    result.returnMessage = "StatusCode is : " + response.StatusCode;
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, content, log, "Failed", result.returnMessage, "");
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
