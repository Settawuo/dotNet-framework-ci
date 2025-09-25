using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
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
using WBBEntity.PanelModels;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.GetListPackageV2
{
    public class GetListPackageSellRouterV2QueryHangler : IQueryHandler<GetListPackageSellRouterV2Query, GetListPackageSellRouterV2Model>
    {
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IAirNetEntityRepository<ListPackageSellRouterV2Model> _objService;
        private readonly IEntityRepository<FBB_CFG_LOV> _lov;

        public GetListPackageSellRouterV2QueryHangler(IWBBUnitOfWork uow,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IAirNetEntityRepository<ListPackageSellRouterV2Model> objService,
            IEntityRepository<FBB_CFG_LOV> lov)
        {
            _uow = uow;
            _intfLog = intfLog;
            _objService = objService;
            _lov = lov;
        }

        public GetListPackageSellRouterV2Model Handle(GetListPackageSellRouterV2Query query)
        {
            InterfaceLogCommand log = null;
            List<FBB_CFG_LOV> loveList = null;
            GetListPackageSellRouterV2Model getListPackageSellRouterV2Model = new GetListPackageSellRouterV2Model();
            log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.TransactionID, "GetListPackageSellRouterQueryHandler", "GetListPackageSellRouterQuery", "", "FBB|" + query.FullUrl, "");

            // check status of rest or soap (A = "SOAP", O = "Rest API")
            string OnlineQueryUse = "A";
            loveList = _lov
              .Get(lov => lov.ACTIVEFLAG.Equals("Y") && lov.LOV_NAME.Equals("CALL_LIST_PACKSELLROUTER")).ToList();
            if (loveList != null && loveList.Count() > 0)
            {
                OnlineQueryUse = loveList.FirstOrDefault().LOV_VAL1.ToSafeString();
            }

            if (OnlineQueryUse == "A")
            {
                try
                {
                    var o_return_code = new OracleParameter
                    {
                        OracleDbType = OracleDbType.Decimal,
                        ParameterName = "o_return_code",
                        Direction = ParameterDirection.Output
                    };

                    var ioResults = new OracleParameter
                    {
                        ParameterName = "ioResults",
                        OracleDbType = OracleDbType.RefCursor,
                        Direction = ParameterDirection.Output
                    };

                    var executeResult = _objService.ExecuteReadStoredProc("AIR_ADMIN.PKG_AIROR910.LIST_PACKAGE_SELL_ROUTER",
                       new
                       {
                           p_sale_channel = query.P_SALE_CHANNEL,

                           // Out
                           o_return_code = o_return_code,
                           ioResults = ioResults

                       }).ToList();

                    getListPackageSellRouterV2Model.o_return_code = o_return_code.Value.ToSafeString() != "null" ? decimal.Parse(o_return_code.Value.ToSafeString()) : 0;
                    getListPackageSellRouterV2Model.ListPackageSellRouter = executeResult;
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, executeResult, log, "Success", "", "");

                }
                catch (Exception ex)
                {
                    getListPackageSellRouterV2Model.o_return_code = -1;
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex, log, "Failed", "inner" + ex.InnerException + " Message:" + ex.Message, "");
                }
            }
            else
            {
                try
                {
                    OnlineQueryConfigModel config = new OnlineQueryConfigModel();
                    List<FBB_CFG_LOV> loveConfigList = null;
                    loveConfigList = _lov
                    .Get(lov => lov.ACTIVEFLAG.Equals("Y") && lov.LOV_TYPE.Equals("FBB_CONFIG")).ToList();

                    if (loveConfigList != null && loveConfigList.Count() > 0)
                    {
                        config.Url = loveConfigList.FirstOrDefault(t => t.LOV_NAME == "URL_CALL_PACKSELLROUTER") != null ? loveConfigList.FirstOrDefault(t => t.LOV_NAME == "URL_CALL_PACKSELLROUTER").LOV_VAL1 : "";
                        config.UseSecurityProtocol = loveConfigList.FirstOrDefault(t => t.LOV_NAME == "URL_CALL_PACKSELLROUTER") != null ? loveConfigList.FirstOrDefault(t => t.LOV_NAME == "URL_CALL_PACKSELLROUTER").LOV_VAL2 : "";
                        config.ContentType = loveConfigList.FirstOrDefault(t => t.LOV_NAME == "ContentType") != null ? loveConfigList.FirstOrDefault(t => t.LOV_NAME == "ContentType").LOV_VAL1 : "";
                        config.Channel = loveConfigList.FirstOrDefault(t => t.LOV_NAME == "x-online-query-channel") != null ? loveConfigList.FirstOrDefault(t => t.LOV_NAME == "x-online-query-channel").LOV_VAL1 : "";

                        if (config.Url != "")
                        {
                            var model = new List<ListPackageSellRouterV2Model>();
                            System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
                            stopWatch.Start();
                            string XOnlineQueryTransactionD = "";
                            string TimeStamp = DateTime.Now.ToString("yyyyMMddHHmmssFFF");
                            int _min = 0000;
                            int _max = 9999;
                            Random _rdm = new Random();
                            var Nonce = _rdm.Next(_min, _max).ToString();
                            XOnlineQueryTransactionD = TimeStamp + Nonce;
                            try
                            {
                                // Body send data to Rest API
                                var onlineQueryConfigBody = new GetListPackageSellRouterV2OnlineQuery()
                                {
                                    SALE_CHANNEL = query.P_SALE_CHANNEL
                                };


                                var result = new GetListPackageSellRouterV2OnlineModel();

                                string BodyStr = JsonConvert.SerializeObject(onlineQueryConfigBody);

                                config.BodyStr = BodyStr;


                                var client = new RestClient(config.Url);
                                var request = new RestRequest();
                                request.Method = Method.POST;
                                request.AddHeader("Content-Type", config.ContentType);
                                request.AddHeader("x-online-query-transaction-id", XOnlineQueryTransactionD);
                                request.AddHeader("x-online-query-channel", config.Channel);
                                request.AddParameter("application/json", BodyStr, ParameterType.RequestBody);

                                // execute the request

                                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                                if (config.UseSecurityProtocol == "Y")
                                {
                                    ServicePointManager.Expect100Continue = true;
                                    ServicePointManager.ServerCertificateValidationCallback = (s, certificate, chain, sslPolicyErrors) => true;
                                }

                                var responseData = client.Execute(request);

                                var content = responseData.Content; // raw content as string
                                if (HttpStatusCode.OK.Equals(responseData.StatusCode))
                                {
                                    result = JsonConvert.DeserializeObject<GetListPackageSellRouterV2OnlineModel>(responseData.Content) ?? new GetListPackageSellRouterV2OnlineModel();
                                    if (result != null)
                                    {
                                        if (result.LIST_PACKAGE_SELL_ROUTER != null && result.LIST_PACKAGE_SELL_ROUTER.Count > 0)
                                        {
                                            model = result.LIST_PACKAGE_SELL_ROUTER.Select(s => new ListPackageSellRouterV2Model
                                            {

                                                MAPPING_CODE = s.MAPPING_CODE.ToSafeString(),
                                                PACKAGE_SERVICE_CODE = s.PACKAGE_SERVICE_CODE.ToSafeString(),
                                                SFF_PROMOTION_CODE = s.SFF_PROMOTION_CODE.ToSafeString(),
                                                PACKAGE_NAME = s.PACKAGE_NAME.ToSafeString(),
                                                PACKAGE_GROUP = s.PACKAGE_GROUP.ToSafeString(),
                                                PACKAGE_GROUP_DESC_THA = s.PACKAGE_GROUP_DESC_THA.ToSafeString(),
                                                PACKAGE_GROUP_DESC_ENG = s.PACKAGE_GROUP_DESC_ENG.ToSafeString(),
                                                PACKAGE_REMARK_THA = s.PACKAGE_REMARK_THA.ToSafeString(),
                                                PACKAGE_REMARK_ENG = s.PACKAGE_REMARK_ENG.ToSafeString(),
                                                PACKAGE_GROUP_SEQ = s.PACKAGE_GROUP_SEQ.ToSafeDecimal(),
                                                PRICE_CHARGE = s.PRICE_CHARGE.ToSafeDecimal(),
                                                PRICE_CHARGE_VAT = s.PRICE_CHARGE_VAT.ToSafeDecimal(),
                                                PRE_PRICE_CHARGE = s.PRE_PRICE_CHARGE.ToSafeDecimal(),
                                                SFF_WORD_IN_STATEMENT_THA = s.SFF_WORD_IN_STATEMENT_THA.ToSafeString(),
                                                SFF_WORD_IN_STATEMENT_ENG = s.SFF_WORD_IN_STATEMENT_ENG.ToSafeString(),
                                                PACKAGE_DISPLAY_THA = s.PACKAGE_DISPLAY_THA.ToSafeString(),
                                                PACKAGE_DISPLAY_ENG = s.PACKAGE_DISPLAY_ENG.ToSafeString(),
                                                DOWNLOAD_SPEED = s.DOWNLOAD_SPEED.ToSafeString(),
                                                UPLOAD_SPEED = s.UPLOAD_SPEED.ToSafeString(),
                                                PACKAGE_TYPE = s.PACKAGE_TYPE.ToSafeString(),
                                                PACKAGE_TYPE_DESC = s.PACKAGE_TYPE_DESC.ToSafeString(),
                                                PRODUCT_SUBTYPE = s.PRODUCT_SUBTYPE.ToSafeString(),
                                                OWNER_PRODUCT = s.OWNER_PRODUCT.ToSafeString(),
                                                ACCESS_MODE = s.ACCESS_MODE.ToSafeString(),
                                                SERVICE_CODE = s.SERVICE_CODE.ToSafeString(),
                                                PRODUCT_SUBTYPE3 = s.PRODUCT_SUBTYPE3.ToSafeString(),
                                                DISCOUNT_TYPE = s.DISCOUNT_TYPE.ToSafeString(),
                                                DISCOUNT_VALUE = s.DISCOUNT_VALUE.ToSafeString(),
                                                DISCOUNT_DAY = s.DISCOUNT_DAY.ToSafeString(),
                                                AUTO_MAPPING_PROMOTION_CODE = s.AUTO_MAPPING_PROMOTION_CODE.ToSafeString(),
                                                DISPLAY_FLAG = s.DISPLAY_FLAG.ToSafeString(),
                                                DISPLAY_SEQ = s.DISPLAY_SEQ.ToSafeDecimal(),
                                                SUB_SEQ = s.SUB_SEQ.ToSafeDecimal()

                                            }).ToList();

                                            getListPackageSellRouterV2Model.ListPackageSellRouter = model;
                                            getListPackageSellRouterV2Model.o_return_code = 0;

                                            InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, getListPackageSellRouterV2Model.ListPackageSellRouter, log, "Success", "", "");


                                            log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, config, query.TransactionID,
                                                    "GetListPackageSellRouterV2Online", "GetListPackageSellRouterV2Online", "", "FBB|" + query.FullUrl, "");
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
                            catch (Exception ex)
                            {

                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    getListPackageSellRouterV2Model.o_return_code = -1;
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex, log, "Failed", "inner" + ex.InnerException + " Message:" + ex.Message, "");
                }

            }

            return getListPackageSellRouterV2Model;
        }
    }
}
