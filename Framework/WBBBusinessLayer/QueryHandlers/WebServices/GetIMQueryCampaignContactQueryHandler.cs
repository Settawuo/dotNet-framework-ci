using Newtonsoft.Json;
using RestSharp;
using System;
using System.Globalization;
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
    public class GetIMQueryCampaignContactQueryHandler : IQueryHandler<GetIMQueryCampaignContactQuery, GetIMQueryCampaignContactModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_CFG_LOV> _cfgLov;
        private readonly IEntityRepository<object> _objService;

        public GetIMQueryCampaignContactQueryHandler(ILogger logger,
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

        public GetIMQueryCampaignContactModel Handle(GetIMQueryCampaignContactQuery query)
        {
            //R22.06.14062022

            InterfaceLogCommand log = null;
            InterfaceLogCommand log2 = null;
            GetIMQueryCampaignContactModel resultGetIM = new GetIMQueryCampaignContactModel();

            try
            {
                var loveConfigList = _cfgLov.Get(lov => lov.LOV_NAME.Equals("URL_IM_QueryCampaignConact") && lov.ACTIVEFLAG == "Y").FirstOrDefault();

                IMQueryCampaignContactConfigModel config = new IMQueryCampaignContactConfigModel();
                config.Url = loveConfigList != null ? loveConfigList.LOV_VAL1 : "";
                config.UseSecurityProtocol = loveConfigList != null ? loveConfigList.LOV_VAL3 : "";

                IMQueryCampaignContactConfigDetail[] parameter = new IMQueryCampaignContactConfigDetail[1];
                parameter[0] = new IMQueryCampaignContactConfigDetail()
                {
                    Name = "InParameter1",
                    Value = query.p_inparameter1
                };

                IMQueryCampaignContactConfigParameterList parameterList = new IMQueryCampaignContactConfigParameterList()
                {
                    Parameter = parameter
                };

                IMQueryCampaignContactConfigBody iMQueryCampaignContactConfigBody = new IMQueryCampaignContactConfigBody()
                {
                    ServiceOption = query.p_queryoption.ToSafeString(),
                    MobileNumber = query.p_mobilenumber.ToSafeString(),
                    ChildCampaignCode = query.p_childcampaigncode.ToSafeString(),
                    ParameterList = parameterList
                };

                string BodyStr = JsonConvert.SerializeObject(iMQueryCampaignContactConfigBody);
                config.BodyStr = BodyStr;

                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, config, query.p_mobilenumber, "QueryCampaignContact", "GetIMQueryCampaignContactQueryHandler(CallService)", "", "FBB", "");
                log2 = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, config, query.p_mobilenumber, "QueryCampaignContact", "GetIMQueryCampaignContactQueryHandler", "", "FBB", "");

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
                    var result = JsonConvert.DeserializeObject<IMQueryCampaignContactResponse>(responseData.Content) ?? new IMQueryCampaignContactResponse();
                    if (result != null)
                    {
                        if (string.IsNullOrEmpty(result.ErrorMessage) && result.MobileNumber.Equals(query.p_mobilenumber))
                        {
                            resultGetIM.MOBILE_NUMBER = result.MobileNumber;

                            var LastUpdatedDate = new DateTime();
                            foreach (var r in result.CampaignContactList)
                            {
                                var LastUpdatedDateTmp = new DateTime();
                                var date = DateTime.TryParseExact(r.LastUpdatedDate.ToSafeString(), "dd-MM-yyyy HH:mm:ss",
                                    CultureInfo.InvariantCulture, DateTimeStyles.None, out LastUpdatedDateTmp);

                                if (result.CampaignContactList.First() == r)
                                {
                                    LastUpdatedDate = LastUpdatedDateTmp; //first

                                    resultGetIM.LAST_UPDATED_DATE = r.LastUpdatedDate.ToSafeString();
                                    resultGetIM.VALUE_1 = r.Value1.ToSafeString();
                                    resultGetIM.CONTACT_LIST_INFO = r.ContactListInfo.ToSafeString();
                                }
                                else if (LastUpdatedDateTmp > LastUpdatedDate)
                                {
                                    LastUpdatedDate = LastUpdatedDateTmp; //more than

                                    resultGetIM.LAST_UPDATED_DATE = r.LastUpdatedDate.ToSafeString();
                                    resultGetIM.VALUE_1 = r.Value1.ToSafeString();
                                    resultGetIM.CONTACT_LIST_INFO = r.ContactListInfo.ToSafeString();
                                }
                            }

                            resultGetIM.RESULT_CODE = result.ErrorCode == "001" ? "0" : result.ErrorCode.ToSafeString();
                            resultGetIM.RESULT_DESC = result.ErrorMessage.ToSafeString();
                            InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, "Success", "", "");
                            InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, resultGetIM, log2, "Success", "", "");
                        }
                        else
                        {
                            resultGetIM.RESULT_CODE = "-1";
                            resultGetIM.RESULT_DESC = result.ErrorMessage.ToSafeString();
                            InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, resultGetIM, log, "Failed", "", "");
                            InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, resultGetIM, log2, "Failed", "", "");
                        }
                    }
                    else
                    {
                        resultGetIM.RESULT_CODE = "-1";
                        resultGetIM.RESULT_DESC = "result null";
                        InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, resultGetIM, log, "Failed", "", "");
                        InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, resultGetIM, log2, "Failed", "", "");
                    }
                }
                else
                {
                    resultGetIM.RESULT_CODE = responseData.StatusCode.ToSafeString();
                    resultGetIM.RESULT_DESC = responseData.ErrorMessage.ToSafeString();
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, resultGetIM, log, "Failed", "", "");
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, resultGetIM, log2, "Failed", "", "");
                }
            }
            catch (Exception ex)
            {
                resultGetIM.RESULT_CODE = "-1";
                resultGetIM.RESULT_DESC = ex.Message;
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, resultGetIM, log, "Failed", ex.Message, "");
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, resultGetIM, log2, "Failed", ex.Message, "");
            }
            return resultGetIM;
        }
    }
}
