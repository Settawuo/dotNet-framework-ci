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
    public class UpdateListBuildingVillageIMHandler : IQueryHandler<UpdateListBuildingVillageIMQuery, UpdateListBuildingVillageIMModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_CFG_LOV> _cfgLov;
        private readonly IEntityRepository<object> _objService;

        public UpdateListBuildingVillageIMHandler(ILogger logger,
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

        public UpdateListBuildingVillageIMModel Handle(UpdateListBuildingVillageIMQuery query)
        {
            //R23.02_22022023
            InterfaceLogCommand log = null;
            UpdateListBuildingVillageIMModel resultGetIM = new UpdateListBuildingVillageIMModel();

            try
            {
                var loveConfigList = _cfgLov.Get(lov => lov.LOV_NAME.Equals("WEB_SERVICE_UPDATELISTBUILDINGVILLAGE_IM") && lov.ACTIVEFLAG == "Y").FirstOrDefault();

                BuildingVillageIMConfigUrlModel config = new BuildingVillageIMConfigUrlModel();
                config.Url = loveConfigList != null ? loveConfigList.LOV_VAL1 : "";
                config.UseSecurityProtocol = loveConfigList != null ? loveConfigList.LOV_VAL3 : "Y";

                BuildingVillageIMConfigBody iMUpdateBuildingVillageConfigBody = new BuildingVillageIMConfigBody()
                {
                    buildingList = query.buildingListIM,
                    UpdateBy = query.UpdateBy.ToSafeString()
                };

                string BodyStr = JsonConvert.SerializeObject(iMUpdateBuildingVillageConfigBody);
                config.BodyStr = BodyStr;

                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, config, query.UpdateBy, "UpdateListBuildingVillageIM", "UpdateListBuildingVillageIMHandler", "", "FBB", "");

                var client = new RestClient(config.Url);
                var request = new RestRequest();
                request.Method = Method.POST;
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
                    var result = JsonConvert.DeserializeObject<UpdateListBuildingVillageIMModel>(responseData.Content) ?? new UpdateListBuildingVillageIMModel();
                    if (result != null)
                    {
                        if (result.ResultCode.Equals("00"))
                        {
                            resultGetIM.ResultCode = result.ResultCode == "00" ? "0" : result.ResultCode.ToSafeString();
                            resultGetIM.ResultDesc = result.ResultDesc == "Successfuly" ? "Success" : result.ResultDesc.ToSafeString();
                            InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, "Success", "", "");
                        }
                        else
                        {
                            resultGetIM.ResultCode = result.ResultCode.ToSafeString();
                            resultGetIM.ResultDesc = result.ResultDesc.ToSafeString();
                            InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, resultGetIM, log, "Failed", "", "");
                        }
                    }
                    else
                    {
                        resultGetIM.ResultCode = "-1";
                        resultGetIM.ResultDesc = "result null";
                        InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, resultGetIM, log, "Failed", "", "");
                    }
                }
                else
                {
                    resultGetIM.ResultCode = responseData.StatusCode.ToSafeString() == "0" ? "-1" : responseData.StatusCode.ToSafeString();
                    resultGetIM.ResultDesc = responseData.ErrorMessage.ToSafeString();
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, resultGetIM, log, "Failed", "", "");
                }
            }
            catch (Exception ex)
            {
                resultGetIM.ResultCode = "-1";
                resultGetIM.ResultDesc = ex.Message;
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, resultGetIM, log, "Failed", ex.Message, "");
            }

            return resultGetIM;
        }
    }
}
