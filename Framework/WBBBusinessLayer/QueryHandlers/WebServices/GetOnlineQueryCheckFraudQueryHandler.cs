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
    public class GetOnlineQueryCheckFraudQueryHandler : IQueryHandler<GetOnlineQueryCheckFraudQuery, GetOnlineQueryCheckFraudQueryModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_CFG_LOV> _cfgLov;

        public GetOnlineQueryCheckFraudQueryHandler(ILogger logger, IEntityRepository<FBB_INTERFACE_LOG> intfLog, IWBBUnitOfWork uow, IEntityRepository<FBB_CFG_LOV> cfgLov)
        {
            _logger = logger;
            _intfLog = intfLog;
            _uow = uow;
            _cfgLov = cfgLov;
        }

        //R23.05 CheckFraud
        public GetOnlineQueryCheckFraudQueryModel Handle(GetOnlineQueryCheckFraudQuery query)
        {
            query.OFFERING = GetOffering(query.OFFERINGJSON);
            query.PROMOTIONLIST = JsonConvert.DeserializeObject<List<Promotionlist>>(query.PROMOTIONLISTJSON);

            string url = GetConfig("URL_GET_CHECK_FRAUD");
            var client = new RestClient(url);
            var request = RequestBuilder(query);
            var log = CreateCheckFraudLog(query);
            var response = client.Execute(request);
            var result = CheckStatus(response, log);
            return result;
        }

        private string GetConfig(string target)
        => _cfgLov.Get(l => l.LOV_NAME == target && l.ACTIVEFLAG == "Y").Select(s => s.LOV_VAL1).FirstOrDefault();

        private List<string> GetOffering(string offeringJson)
        {
            var target = JsonConvert.DeserializeObject<List<string>>(offeringJson);
            var result = _cfgLov.Get(l => l.LOV_NAME == "LIST_SPECIAL_OFFER" && l.ACTIVEFLAG == "Y" && target.Contains(l.DISPLAY_VAL)).Select(s => s.LOV_VAL1);
            return result.ToList();
        }

        private RestRequest RequestBuilder(GetOnlineQueryCheckFraudQuery query)
        {
            var request = new RestRequest();
            request.Method = Method.POST;
            request.AddHeader("Authorization", "Bearer " + query.ONLINEAUTH_TOKEN);
            request.AddParameter("application/json", query, ParameterType.RequestBody);

            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback =
                (s, certificate, chain, sslPolicyErrors) => true;

            return request;
        }

        private InterfaceLogCommand CreateCheckFraudLog(GetOnlineQueryCheckFraudQuery query)
        => InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.CONTACT_MOBILE_NO, "GetOnlineQueryCheckFraud", "GetOnlineQueryCheckFraudQueryHandler", null, "FBB", "");

        private GetOnlineQueryCheckFraudQueryModel CheckStatus(IRestResponse response, InterfaceLogCommand log)
        {
            var StatusCode = "Failed";
            var StatusMessage = "";
            var result = new GetOnlineQueryCheckFraudQueryModel();
            try
            {
                result = MockCheckFraud(1);// replace resultJson


                var resultJson = JsonConvert.DeserializeObject<GetOnlineQueryCheckFraudQueryModel>(response.Content);

                if (!HttpStatusCode.OK.Equals(response.StatusCode) || result == null)
                {
                    StatusMessage = (string.IsNullOrEmpty(response.ErrorMessage)
                            ? response.Content
                            : response.ErrorMessage).ToSafeString();
                }

                if (result.RESULT_CODE == "20000")
                {
                    StatusCode = "Success";
                    StatusMessage = result.RESULT_DESC;
                }

                if (string.IsNullOrEmpty(result?.RESULT_CODE))
                {
                    result.RESULT_CODE = StatusCode;
                    result.RESULT_DESC = StatusMessage;
                }
            }
            catch (Exception ex)
            {
                StatusMessage = ex.GetBaseException().ToString();
                _logger.Info("GetOnlineQueryMobileInfoHandler Exception " + ex.GetErrorMessage());
            }
            finally
            {
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, StatusCode, StatusMessage, "");
            }
            return result;
        }

        private GetOnlineQueryCheckFraudQueryModel MockCheckFraud(int number)
        {
            var result = new GetOnlineQueryCheckFraudQueryModel
            {
                RESULT_CODE = "20000",
                RESULT_DESC = "Success",
                TRANSACTION_ID = "trannsaction",
                CHECK_FRAUD_INFO = new checkFraudInfo()
            };
            switch (number)
            {
                case 1:
                    result.CHECK_FRAUD_INFO.NOTIFY_POPUP = "Y";
                    result.CHECK_FRAUD_INFO.NOTIFY_MESSAGE = "แนะนำ Renew AIS Fibre เดิม ให้ลูกค้าติดต่อ AIS Shop , Call Center 1175 กรณีลูกค้าไม่สะดวกแนบ empowerment เพิ่มเติม";
                    result.CHECK_FRAUD_INFO.VERIFY_REASON = "ID card + Address Dup +Status Order เดิม =Disconnect Customer Request,Terminate+ Terminated <90 วัน ";
                    result.CHECK_FRAUD_INFO.FLAG_GO_NOGO = "Go";
                    result.CHECK_FRAUD_INFO.AUTO_CREATE_PROSPECT = "S";
                    result.CHECK_FRAUD_INFO.FRAUD_SCORE = "218";
                    result.CHECK_FRAUD_INFO.FRAUD_REASONS
                        = new List<FRAUDREASONS>
                        {
                        new FRAUDREASONS()
                        {
                            REASON="ID Card Duplicated",
                            SCORE= 100},
                        new FRAUDREASONS
                        {
                            REASON="Contact Duplicated",
                            SCORE= 100
                        },
                        new FRAUDREASONS
                        {
                              REASON="Contact No FMC <= 2",
                            SCORE= 18
                        }};
                    result.CHECK_FRAUD_INFO.CEN_FRAUD_FLAG = "Y";
                    break;

                case 2:
                    result.CHECK_FRAUD_INFO.NOTIFY_POPUP = "Y";
                    result.CHECK_FRAUD_INFO.NOTIFY_MESSAGE = "Internet no. มียอดค้าง   \r\n\r\n-แนะนำชำระยอดค้างก่อนทำรายการสมัครใหม่";
                    result.CHECK_FRAUD_INFO.VERIFY_REASON = "ID card +Contract no. Dup +Status Order เดิม  = Suspend, Disconnect Customer Request,Terminate +AR balance (CA) (Over Due date) ";
                    result.CHECK_FRAUD_INFO.FLAG_GO_NOGO = "No Go";
                    result.CHECK_FRAUD_INFO.FRAUD_SCORE = "218";
                    result.CHECK_FRAUD_INFO.FRAUD_REASONS = new List<FRAUDREASONS>
                        {
                        new FRAUDREASONS()
                        {
                            REASON="ID Card Duplicated",
                            SCORE= 100},
                        new FRAUDREASONS
                        {
                            REASON="Contact Duplicated",
                            SCORE= 100
                        },
                        new FRAUDREASONS
                        {
                              REASON="Contact No FMC <= 2",
                            SCORE= 18
                        }};
                    result.CHECK_FRAUD_INFO.CEN_FRAUD_FLAG = "Y";
                    break;

                case 3:
                    result.CHECK_FRAUD_INFO.VERIFY_REASON = "โปร Serenade";
                    result.CHECK_FRAUD_INFO.AUTO_CREATE_PROSPECT = "S";
                    result.CHECK_FRAUD_INFO.FRAUD_SCORE = "0";
                    result.CHECK_FRAUD_INFO.CEN_FRAUD_FLAG = "Y";
                    break;

                case 4:
                    result.CHECK_FRAUD_INFO.VERIFY_REASON = "ลูกค้า Non-Residential";
                    result.CHECK_FRAUD_INFO.AUTO_CREATE_PROSPECT = "N";
                    result.CHECK_FRAUD_INFO.FRAUD_SCORE = "0";
                    result.CHECK_FRAUD_INFO.CEN_FRAUD_FLAG = "Y";
                    break;

                case 5:
                    result.RESULT_CODE = "40003";
                    result.RESULT_DESC = "Request parameter(s) should not be null or empty. (Parameter 'CONTACT_MOBILE_NO')";
                    result.CHECK_FRAUD_INFO.CEN_FRAUD_FLAG = "N";
                    break;

                default:
                    result.RESULT_CODE = "40001";
                    result.RESULT_DESC = "Data Not Found.";
                    result.CHECK_FRAUD_INFO.AUTO_CREATE_PROSPECT = "N";
                    result.CHECK_FRAUD_INFO.CEN_FRAUD_FLAG = "N";
                    break;
            }
            return result;
        }
    }
}
