using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data;
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
    public class GetListDeviceContractQueryHandler : IQueryHandler<GetListDeviceContractQuery, List<ContractDeviceModel>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_CFG_LOV> _lov;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<object> _obj;

        public GetListDeviceContractQueryHandler(ILogger logger,
            IEntityRepository<FBB_CFG_LOV> lov,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IWBBUnitOfWork uow,
            IEntityRepository<object> obj)
        {
            _logger = logger;
            _lov = lov;
            _intfLog = intfLog;
            _uow = uow;
            _obj = obj;
        }

        public List<ContractDeviceModel> Handle(GetListDeviceContractQuery query)
        {
            InterfaceLogCommand log = null;
            List<ContractDeviceModel> model = new List<ContractDeviceModel>();
            ContractDeviceOnlineQueryConfigResult result = new ContractDeviceOnlineQueryConfigResult();
            System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
            stopWatch.Start();

            try
            {
                OnlineQueryConfigModel config = new OnlineQueryConfigModel();
                List<FBB_CFG_LOV> loveConfigList = null;
                loveConfigList = _lov.Get(lov => lov.ACTIVEFLAG.Equals("Y") && lov.LOV_TYPE.Equals("FBB_CONFIG")).ToList();

                if (loveConfigList != null && loveConfigList.Count() > 0)
                {
                    config.Url = loveConfigList.FirstOrDefault(t => t.LOV_NAME == "URL_CALL_DCONTRACT") != null ? loveConfigList.FirstOrDefault(t => t.LOV_NAME == "URL_CALL_DCONTRACT").LOV_VAL1 : "";
                    config.UseSecurityProtocol = loveConfigList.FirstOrDefault(t => t.LOV_NAME == "URL_CALL_DCONTRACT") != null ? loveConfigList.FirstOrDefault(t => t.LOV_NAME == "URL_CALL_DCONTRACT").LOV_VAL2 : "";
                    config.ContentType = loveConfigList.FirstOrDefault(t => t.LOV_NAME == "URL_CALL_DCONTRACT") != null ? loveConfigList.FirstOrDefault(t => t.LOV_NAME == "URL_CALL_DCONTRACT").LOV_VAL3 : "";
                    config.Channel = loveConfigList.FirstOrDefault(t => t.LOV_NAME == "URL_CALL_DCONTRACT") != null ? loveConfigList.FirstOrDefault(t => t.LOV_NAME == "URL_CALL_DCONTRACT").LOV_VAL4 : "";

                    if (config.Url != "")
                    {
                        string XOnlineQueryTransactionD = "";
                        string TimeStamp = DateTime.Now.ToString("yyyyMMddHHmmssFFF");
                        int _min = 0000;
                        int _max = 9999;
                        Random _rdm = new Random();
                        var Nonce = _rdm.Next(_min, _max).ToString();
                        XOnlineQueryTransactionD = TimeStamp + Nonce;

                        List<PROMOTION_LIST> promotion_list = new List<PROMOTION_LIST>();
                        if (query.LIST_SFF_PROMOTION_CODE != null && query.LIST_SFF_PROMOTION_CODE.Count > 0)
                        {
                            promotion_list = query.LIST_SFF_PROMOTION_CODE.Select(p => new PROMOTION_LIST
                            {
                                PROMOTION_CODE = p.SFF_PROMOTION_CODE.ToSafeString()
                            }).ToList();
                        }

                        ContractDeviceOnlineQueryConfigBody onlineQueryConfigBody = new ContractDeviceOnlineQueryConfigBody()
                        {
                            CHANNEL = query.P_CHANNEL.ToSafeString(),
                            EVENT = query.P_EVENT.ToSafeString(),
                            PRODUCT_SUBTYPE = query.P_PRODUCT_SUBTYPE.ToSafeString(), //R22.07
                            OWNER_PRODUCT = query.P_OWNER_PRODUCT.ToSafeString(), //R22.07
                            SALE_CHANNEL = query.P_SALE_CHANNEL.ToSafeString(), //R22.07
                            PROMOTION_LIST = promotion_list.ToArray()
                        };

                        string BodyStr = JsonConvert.SerializeObject(onlineQueryConfigBody);

                        config.BodyStr = BodyStr;

                        log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, config, query.TransactionID, "GetListDeviceContractOnlineQuery", "GetListDeviceContractOnlineQuery", "", "FBB|" + query.FullUrl, "");

                        var client = new RestClient(config.Url);
                        var request = new RestRequest();
                        request.Method = Method.POST;
                        request.AddHeader("Content-Type", config.ContentType);
                        request.AddHeader("x-online-query-transaction-id", XOnlineQueryTransactionD);
                        request.AddHeader("x-online-query-channel", config.Channel);
                        request.AddParameter("application/json", BodyStr, ParameterType.RequestBody);

                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                        // execute the request
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
                            result = JsonConvert.DeserializeObject<ContractDeviceOnlineQueryConfigResult>(responseData.Content) ?? new ContractDeviceOnlineQueryConfigResult();
                            if (result != null)
                            {
                                if (result.ListDeviceContract != null && result.ListDeviceContract.Count > 0)
                                {
                                    model = result.ListDeviceContract.Select(t => new ContractDeviceModel
                                    {
                                        SFF_PROMOTION_CODE = t.SFF_PROMOTION_CODE.ToSafeString(),
                                        PRODUCT_SUBTYPE = t.PRODUCT_SUBTYPE.ToSafeString(),
                                        CONTRACT_ID = t.CONTRACT_ID.ToSafeString(),
                                        CONTRACT_NAME = t.CONTRACT_NAME.ToSafeString(),
                                        DURATION_BY_MONTH = t.DURATION_BY_MONTH.ToSafeString(),
                                        DURATION = t.DURATION.ToSafeString(),
                                        CONTRACT_DISPLAY_TH_1 = t.CONTRACT_DISPLAY_TH_1.ToSafeString(),
                                        CONTRACT_DISPLAY_TH_2 = t.CONTRACT_DISPLAY_TH_2.ToSafeString(),
                                        CONTRACT_DISPLAY_EN_1 = t.CONTRACT_DISPLAY_EN_1.ToSafeString(),
                                        CONTRACT_DISPLAY_EN_2 = t.CONTRACT_DISPLAY_EN_2.ToSafeString(),
                                        CONTRACT_FLAG = t.CONTRACT_FLAG.ToSafeString(),
                                        PENALTY_CHARGE = t.PENALTY_CHARGE.ToSafeString(),
                                        CONTRACT_TYPE = t.CONTRACT_TYPE.ToSafeString(),
                                        CONTRACT_RULE_ID = t.CONTRACT_RULE_ID.ToSafeString(),
                                        PENALTY_TYPE = t.PENALTY_TYPE.ToSafeString(),
                                        PENALTY_ID = t.PENALTY_ID.ToSafeString(),
                                        LIMIT_CONTRACT = t.LIMIT_CONTRACT.ToSafeString(),
                                        COUNT_FLAG = t.COUNT_FLAG.ToSafeString(),
                                        DEFAULT_FLAG = t.DEFAULT_FLAG.ToSafeString(),
                                        CALL_TDM_FLAG = t.CALL_TDM_FLAG.ToSafeString()
                                    }).ToList();

                                }
                                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, content, log, "Success", "", "");
                            }
                            else
                            {
                                result.RESULT_CODE = "1";
                                result.RESULT_DESC = "result null";
                                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, content, log, "Failed", "", "");
                            }
                        }
                        else
                        {
                            result.RESULT_CODE = "1";
                            result.RESULT_DESC = responseData.StatusCode.ToString();
                            InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, content, log, "Failed", "", "");
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                result.RESULT_CODE = "1";
                result.RESULT_DESC = ex.Message;
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, "Failed", ex.Message, "");
            }

            TimeSpan ts = stopWatch.Elapsed;
            string SBNServiceListPackageElapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
            _logger.Info("GetListDeviceContractQuery elapsed time is " + SBNServiceListPackageElapsedTime);

            return model;
        }
    }
}
