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
    public class ReleaseReservePort3bbQueryHandler : IQueryHandler<ReleaseReservePort3bbQuery, ReleaseReservePort3bbQueryModel>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<string> _objService;
        private readonly IEntityRepository<FBB_CFG_LOV> _cfgLov;

        public ReleaseReservePort3bbQueryHandler(ILogger logger,
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

        public ReleaseReservePort3bbQueryModel Handle(ReleaseReservePort3bbQuery query)
        {
            InterfaceLogCommand log = null;
            ReleaseReservePort3bbQueryModel result = new ReleaseReservePort3bbQueryModel();

            try
            {
                Guid myuuid = Guid.NewGuid();
                string myuuidAsString = myuuid.ToString();
                query.transactionId = "FBB_WBB_" + myuuidAsString;
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, myuuidAsString,
                    "ReleaseReservePort3bbQuery", "ReleaseReservePort3bbQueryHandler", "", "FBB", "");

                //"Get apiCongif"
                string tmpUrl = "";
                string tmpUser = "";
                string tmpPass = "";
                var apiAuthConfig = (from l in _cfgLov.Get()
                                    where l.LOV_NAME == "ReleaseReservePort3bbAuth"
                                    && l.ACTIVEFLAG == "Y"
                                    select new { l.LOV_VAL1, l.LOV_VAL2 , l.LOV_VAL3});
                if (apiAuthConfig != null)
                {
                    tmpUrl = apiAuthConfig.FirstOrDefault().LOV_VAL1;
                    tmpUser = apiAuthConfig.FirstOrDefault().LOV_VAL2;
                    tmpPass = apiAuthConfig.FirstOrDefault().LOV_VAL3;
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
                        result = JsonConvert.DeserializeObject<ReleaseReservePort3bbQueryModel>(response.Content) ?? new ReleaseReservePort3bbQueryModel();
                        InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, content, log, result.returnCode == "00000" ? "Success" : "Failed", "", "");
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
