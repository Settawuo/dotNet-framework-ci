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
    public class CreateCaseGenericFlowQueryHandler : IQueryHandler<CreateCaseGenericFlowQuery, CreateCaseGenericFlowModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_CFG_LOV> _cfgLov;
        private readonly IEntityRepository<object> _objService;

        public CreateCaseGenericFlowQueryHandler(ILogger logger,
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

        public CreateCaseGenericFlowModel Handle(CreateCaseGenericFlowQuery query)
        {
            //R22.08 Order Deadlock Change Service not allow
            InterfaceLogCommand log = null;
            CreateCaseGenericFlowModel resultGetIM = new CreateCaseGenericFlowModel();

            try
            {
                var loveConfigList = _cfgLov.Get(lov => lov.LOV_NAME.Equals("URL_CALL_IM_MESH") && lov.ACTIVEFLAG == "Y").FirstOrDefault();

                CreateCaseGenericFlowConfigUrlModel config = new CreateCaseGenericFlowConfigUrlModel();
                config.Url = loveConfigList != null ? loveConfigList.LOV_VAL1 : "";
                config.UseSecurityProtocol = loveConfigList != null ? loveConfigList.LOV_VAL3 : "";

                CreateCaseGenericFlowConfigBody iMQueryCampaignContactConfigBody = new CreateCaseGenericFlowConfigBody()
                {
                    MobileNo = query.MobileNo.ToSafeString(),
                    InteractionType = query.InteractionType.ToSafeString(),
                    OwnerName = query.OwnerName.ToSafeString(),
                    Status = query.Status.ToSafeString(),
                    TopicName = query.TopicName.ToSafeString(),
                    SubTopic = query.SubTopic.ToSafeString(),
                    AssignedType = query.AssignedType.ToSafeString(),
                    AssignedTo = query.AssignedTo.ToSafeString(),
                    Comments = query.Comments.ToSafeString(),
                    CapturingList = query.CapturingList
                };

                string BodyStr = JsonConvert.SerializeObject(iMQueryCampaignContactConfigBody);
                config.BodyStr = BodyStr;

                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, config, query.MobileNo, "CreateCaseGenericFlow", "CreateCaseGenericFlowQueryHandler", "", "FBB", "");

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
                    var result = JsonConvert.DeserializeObject<CreateCaseGenericFlowModel>(responseData.Content) ?? new CreateCaseGenericFlowModel();
                    if (result != null)
                    {
                        if (result.ErrorCode.Equals("000") && result.ErrorMessage.Equals("Success"))
                        {
                            resultGetIM.CaseID = result.CaseID.ToSafeString();
                            resultGetIM.ErrorCode = result.ErrorCode == "000" ? "0" : result.ErrorCode.ToSafeString();
                            resultGetIM.ErrorMessage = result.ErrorMessage.ToSafeString();
                            InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, "Success", "", "");
                        }
                        else
                        {
                            resultGetIM.ErrorCode = "-1";
                            resultGetIM.ErrorMessage = result.ErrorMessage.ToSafeString();
                            InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, resultGetIM, log, "Failed", "", "");
                        }
                    }
                    else
                    {
                        resultGetIM.ErrorCode = "-1";
                        resultGetIM.ErrorMessage = "result null";
                        InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, resultGetIM, log, "Failed", "", "");
                    }
                }
                else
                {
                    resultGetIM.ErrorCode = responseData.StatusCode.ToSafeString();
                    resultGetIM.ErrorMessage = responseData.ErrorMessage.ToSafeString();
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, resultGetIM, log, "Failed", "", "");
                }
            }
            catch (Exception ex)
            {
                resultGetIM.ErrorCode = "-1";
                resultGetIM.ErrorMessage = ex.Message;
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, resultGetIM, log, "Failed", ex.Message, "");
            }
            return resultGetIM;
        }
    }
}
