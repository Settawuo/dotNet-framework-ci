using Newtonsoft.Json;
using RestSharp;
using System;
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
    public class UpdateListBuildingVillageSDHandler : IQueryHandler<UpdateListBuildingVillageSDQuery, UpdateListBuildingVillageSDModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_CFG_LOV> _cfgLov;
        private readonly IEntityRepository<object> _objService;

        public UpdateListBuildingVillageSDHandler(ILogger logger,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IWBBUnitOfWork uow,
            IEntityRepository<FBB_CFG_LOV> cfgLov,
            IEntityRepository<object> objService)
        {
            _logger = logger;
            _intfLog = intfLog;
            _uow = uow;
            _cfgLov = cfgLov;
            _objService = objService;
        }

        public UpdateListBuildingVillageSDModel Handle(UpdateListBuildingVillageSDQuery query)
        {
            //R23.02_22022023
            InterfaceLogCommand log = null;
            UpdateListBuildingVillageSDModel resultGetSD = new UpdateListBuildingVillageSDModel();

            try
            {
                string XOnlineQueryTransaction = "";
                string TimeStamp = DateTime.Now.ToString("yyyyMMddHHmmssFFF");
                int _min = 00000;
                int _max = 99999;
                Random _rdm = new Random();
                var Nonce = _rdm.Next(_min, _max).ToString("D5");
                XOnlineQueryTransaction = TimeStamp + Nonce;

                var lovConfigList = _cfgLov.Get(lov => lov.LOV_NAME.Equals("WEB_SERVICE_UPDATELISTBUILDINGVILLAGE_SD") && lov.ACTIVEFLAG == "Y").FirstOrDefault();
                var lovContentType = _cfgLov.Get(lov => lov.LOV_NAME.Equals("Content-Type") && lov.ACTIVEFLAG == "Y").FirstOrDefault();

                BuildingVillageSDConfigUrlModel config = new BuildingVillageSDConfigUrlModel();
                config.Url = lovConfigList != null ? lovConfigList.LOV_VAL1 : "";
                config.UseSecurityProtocol = lovConfigList != null ? lovConfigList.LOV_VAL3 : "Y";
                config.Channel = lovConfigList != null ? lovConfigList.LOV_VAL4 : "FBB_WEB";
                config.Authorization = lovConfigList != null ? lovConfigList.LOV_VAL5 : "";
                config.ContentType = lovContentType != null ? lovContentType.LOV_VAL1 : "application/json; charset=utf-8";

                BuildingVillageSDConfigBody sDUpdateBuildingVillageConfigBody = new BuildingVillageSDConfigBody()
                {
                    channel = config.Channel.ToSafeString(),
                    buildingList = query.buildingListSD
                };

                string BodyStr = JsonConvert.SerializeObject(sDUpdateBuildingVillageConfigBody);
                config.BodyStr = BodyStr;

                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, config, query.UpdateBy, "UpdateListBuildingVillageSD", "UpdateListBuildingVillageSDHandler", XOnlineQueryTransaction, "FBB", "");

                var client = new RestClient(config.Url);
                var request = new RestRequest();
                request.Method = Method.POST;
                request.AddHeader("Content-Type", config.ContentType);
                request.AddHeader("x-api-request-id", XOnlineQueryTransaction);
                request.AddHeader("x-authorization", config.Authorization);
                request.AddParameter("application/json", BodyStr, ParameterType.RequestBody);

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                if (config.UseSecurityProtocol == "Y")
                {
                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.ServerCertificateValidationCallback =
                        (s, certificate, chain, sslPolicyErrors) => true;
                }

                var responseData = client.Execute(request);
                var content = responseData.Content; // raw content as string

                if (HttpStatusCode.OK.Equals(responseData.StatusCode))
                {
                    var result = JsonConvert.DeserializeObject<UpdateListBuildingVillageSDModel>(responseData.Content) ?? new UpdateListBuildingVillageSDModel();
                    if (result != null)
                    {
                        if (result.resultCode.Equals("20000"))
                        {
                            resultGetSD.resultCode = result.resultCode == "20000" ? "0" : result.resultCode.ToSafeString();
                            resultGetSD.resultDescription = result.resultDescription.ToSafeString();
                            resultGetSD.developerMessage = result.developerMessage.ToSafeString();
                            resultGetSD.data = result.data;
                            InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, "Success", "", "");
                        }
                        else
                        {
                            resultGetSD.resultCode = result.resultCode.ToSafeString();
                            resultGetSD.resultDescription = result.resultDescription.ToSafeString();
                            resultGetSD.developerMessage = result.developerMessage.ToSafeString();
                            resultGetSD.data = result.data;
                            InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, resultGetSD, log, "Failed", "", "");
                        }
                    }
                    else
                    {
                        resultGetSD.resultCode = "-1";
                        resultGetSD.resultDescription = "result null";
                        InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, resultGetSD, log, "Failed", "", "");
                    }
                }
                else
                {
                    resultGetSD.resultCode = responseData.StatusCode.ToSafeString() == "0" ? "-1" : responseData.StatusCode.ToSafeString();
                    resultGetSD.resultDescription = responseData.ErrorMessage.ToSafeString();
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, resultGetSD, log, "Failed", "", "");
                }
            }
            catch (Exception ex)
            {
                resultGetSD.resultCode = "-1";
                resultGetSD.resultDescription = ex.Message;
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, resultGetSD, log, "Failed", ex.Message, "");
            }

            return resultGetSD;
        }
    }
}
