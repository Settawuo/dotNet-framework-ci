using AIRNETEntity.Extensions;
using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using WBBBusinessLayer.SBNV2WebService;
using WBBContract;
using WBBContract.Commands;
using WBBContract.WebService;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Models;
using WBBEntity.PanelModels;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class GetListPackageDisplayQueryHandler : IQueryHandler<GetListPackageDisplayQuery, List<ListPackageDisplayModel>>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IAirNetEntityRepository<ListPackageDisplayModel> _objService;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<FBB_CFG_LOV> _lov;

        public GetListPackageDisplayQueryHandler(ILogger logger,
            IWBBUnitOfWork uow,
            IAirNetEntityRepository<ListPackageDisplayModel> objService,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IEntityRepository<FBB_CFG_LOV> lov)
        {
            _logger = logger;
            _uow = uow;
            _objService = objService;
            _intfLog = intfLog;
            _lov = lov;
        }

        public List<ListPackageDisplayModel> Handle(GetListPackageDisplayQuery query)
        {
            return null;

        }
    }

    public class GetListPackageChangeQueryHandler : IQueryHandler<GetListPackageChangeQuery, List<ListPackageChangeModel>>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IAirNetEntityRepository<ListPackageChangeModel> _objService;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<FBB_CFG_LOV> _lov;

        public GetListPackageChangeQueryHandler(ILogger logger,
            IWBBUnitOfWork uow,
            IAirNetEntityRepository<ListPackageChangeModel> objService,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IEntityRepository<FBB_CFG_LOV> lov)
        {
            _logger = logger;
            _uow = uow;
            _objService = objService;
            _intfLog = intfLog;
            _lov = lov;
        }

        public List<ListPackageChangeModel> Handle(GetListPackageChangeQuery query)
        {
            InterfaceLogCommand log = null;
            List<FBB_CFG_LOV> loveList = null;
            DateTime Curr_DateTime = DateTime.Now;
            log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.non_mobile_no + query.client_ip, "GetListPackageChangeQuery", "AIR_ADMIN.PKG_AIROR905.LIST_PACKAGE_BY_CHANGE", null, "FBB|" + query.FullUrl, "FBBOR016");
            List<ListPackageChangeModel> executeResult = null;

            // check status of rest or soap (A = "SOAP", O = "Rest API")
            string OnlineQueryUse = "A";
            loveList = _lov
              .Get(lov => lov.ACTIVEFLAG.Equals("Y") && lov.LOV_NAME.Equals("CALL_LIST_PACKBYCHANGE")).ToList();
            if (loveList != null && loveList.Count() > 0)
            {
                OnlineQueryUse = loveList.FirstOrDefault().LOV_VAL1.ToSafeString();
            }

            if (OnlineQueryUse == "A")
            {

                try
                {
                    //var o_return_code = new OracleParameter();
                    //o_return_code.ParameterName = "o_return_code";
                    //o_return_code.OracleDbType = OracleDbType.Decimal;
                    //o_return_code.Direction = ParameterDirection.Output;

                    //var ioResults = new OracleParameter();
                    //ioResults.ParameterName = "ioResults";
                    //ioResults.OracleDbType = OracleDbType.RefCursor;
                    //ioResults.Direction = ParameterDirection.Output;

                    //List<ListPackageChangeModel> executeResult = _objService.ExecuteReadStoredProc("AIR_ADMIN.PKG_AIROR905.LIST_PACKAGE_BY_CHANGE",
                    //            new
                    //            {
                    //                p_owner_product = query.owner_product,
                    //                p_package_for = query.package_for,
                    //                p_serenade_flag = query.serenade_flag,
                    //                p_ref_row_id = query.ref_row_id,
                    //                /// return //////
                    //                o_return_code = o_return_code,

                    //                ioResults = ioResults

                    //            }).ToList();

                    //var Return_Code = o_return_code.Value != null ? Convert.ToInt32(o_return_code.Value.ToSafeString()) : -1;

                    //R20.6 ChangePromotionCheckRight
                    var listCondflag = new List<airProjectCondFlagRecord>();
                    if (query.ProjectCondFlagArray.Any())
                    {
                        //TODO: set send
                        foreach (var item in query.ProjectCondFlagArray)
                        {
                            listCondflag.Add(new airProjectCondFlagRecord()
                            {
                                projectCondFlag = item.projectCondFlag,
                                projectCondValue = item.projectCondValue.ToSafeString()
                            });
                        }
                    }


                    SBNV2WebService.sbnAirWorkflowResponse objResp = new SBNV2WebService.sbnAirWorkflowResponse();

                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                    ServicePointManager.ServerCertificateValidationCallback =
                        (s, certificate, chain, sslPolicyErrors) => true;

                    using (var service = new SBNV2WebService.AIRInterfaceWorkflowServiceService())
                    {
                        service.Timeout = 600000;

                        string tmpUrl = (from r in _lov.Get()
                                         where r.LOV_NAME == "SaveOrderNewURL" && r.ACTIVEFLAG == "Y"
                                         select r.LOV_VAL1).FirstOrDefault().ToSafeString();
                        if (tmpUrl != "")
                        {
                            service.Url = tmpUrl;
                        }

                        objResp = service.getListPackageByChange(query.sale_channel, query.owner_product, query.package_for, query.customer_type, query.partner_type, query.partner_subtype,
                                                                query.location_code, query.asc_code, query.employee_id, query.region, query.province, query.district, query.sub_district,
                                                                query.address_id, query.serenade_flag, query.penalty_flag, query.package_main, query.product_subtype, listCondflag.ToArray());
                    }
                    executeResult = objResp.listPackageByChangeResult.Select(r => new ListPackageChangeModel
                    {
                        mapping_code = r.MAPPING_CODE,
                        auto_mapping_code = r.AUTO_MAPPING_PROMOTION_CODE, //auto_mapping_promotion_code
                        discount_day = r.DISCOUNT_DAY,
                        discount_type = r.DISCOUNT_TYPE,
                        discount_value = r.DISCOUNT_VALUE,
                        download_speed = r.DOWNLOAD_SPEED,
                        //initiation_charge = r.INITIATION_CHARGE,
                        owner_product = r.OWNER_PRODUCT,
                        //package_code = r.PACKAGE_CODE,
                        package_group = r.PACKAGE_GROUP,
                        package_type = r.PACKAGE_TYPE, // ของใหม่
                        package_type_desc = r.PACKAGE_TYPE_DESC,
                        //pre_initiation_charge = r.PRE_INITIATION_CHARGE,
                        pre_recurring_charge = r.PRE_PRICE_CHARGE.ToSafeDecimal(),
                        product_subtype = r.PRODUCT_SUBTYPE,
                        recurring_charge = r.PRICE_CHARGE.ToSafeDecimal(),
                        //seq = r.SEQ,
                        serenade_flag = r.SERENADE_FLAG,
                        sff_promotion_bill_eng = r.PACKAGE_DISPLAY_THA,
                        sff_promotion_bill_tha = r.PACKAGE_DISPLAY_ENG,
                        sff_promotion_code = r.SFF_PROMOTION_CODE,
                        //technology = r.TECHNOLOGY,
                        upload_speed = r.UPLOAD_SPEED,
                        display_seq = r.DISPLAY_SEQ,
                        display_flag = r.DISPLAY_FLAG,
                        sff_word_in_statement_tha = r.SFF_WORD_IN_STATEMENT_THA,
                        sff_word_in_statement_eng = r.SFF_WORD_IN_STATEMENT_ENG,
                        package_group_seq = r.PACKAGE_GROUP_SEQ,
                        sub_seq = r.SUB_SEQ,

                        //mapping_code
                        //package_type_desc
                        //sff_word_in_statement_tha
                        //sff_word_in_statement_eng
                        //package_group_seq
                        //sub_seq

                        //R20.6 ChangePromotionCheckRight
                        package_display_eng = r.PACKAGE_DISPLAY_ENG,
                        package_display_tha = r.PACKAGE_DISPLAY_THA,

                    }).ToList();

                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, executeResult, log, "Success", "", "FBBOR016");

                }
                catch (Exception ex)
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, new List<ListPackageChangeModel>(), log, "Failed", ex.Message, "FBBOR016");
                    return null;
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
                        config.Url = loveConfigList.FirstOrDefault(t => t.LOV_NAME == "URL_CALL_PACKBYCHANGE") != null ? loveConfigList.FirstOrDefault(t => t.LOV_NAME == "URL_CALL_PACKBYCHANGE").LOV_VAL1 : "";
                        config.UseSecurityProtocol = loveConfigList.FirstOrDefault(t => t.LOV_NAME == "URL_CALL_PACKBYCHANGE") != null ? loveConfigList.FirstOrDefault(t => t.LOV_NAME == "URL_CALL_PACKBYCHANGE").LOV_VAL2 : "";
                        config.ContentType = loveConfigList.FirstOrDefault(t => t.LOV_NAME == "ContentType") != null ? loveConfigList.FirstOrDefault(t => t.LOV_NAME == "ContentType").LOV_VAL1 : "";
                        config.Channel = loveConfigList.FirstOrDefault(t => t.LOV_NAME == "x-online-query-channel") != null ? loveConfigList.FirstOrDefault(t => t.LOV_NAME == "x-online-query-channel").LOV_VAL1 : "";

                        if (config.Url != "")
                        {
                            var model = new List<ListPackageChangeModel>();
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
                                var listCondflag = new List<AirProjectCondFlagArray>();
                                if (query.ProjectCondFlagArray.Any())
                                {
                                    //TODO: set send
                                    foreach (var item in query.ProjectCondFlagArray)
                                    {
                                        listCondflag.Add(new AirProjectCondFlagArray()
                                        {
                                            Flag = item.projectCondFlag,
                                            Value = item.projectCondValue.ToSafeString()
                                        });
                                    }
                                }

                                // Body send data to Rest API
                                var onlineQueryConfigBody = new GetListPackageChangeOnlineQuery()
                                {
                                    SALE_CHANNEL = query.sale_channel,
                                    OWNER_PRODUCT = query.owner_product,
                                    PACKAGE_FOR = query.package_for,
                                    CUSTOMER_TYPE = query.customer_type,
                                    PARTNER_TYPE = query.partner_type,
                                    PARTNER_SUBTYPE = query.partner_subtype,
                                    DISTRIBUTION_CHANNEL = query.distribution_channel,
                                    CHANNEL_SALES_GROUP = query.channel_sales_group,
                                    SHOP_SEGMENT = query.shop_segment,
                                    LOCATION_CODE = query.location_code,
                                    ASC_CODE = query.asc_code,
                                    EMPLOYEE_ID = query.employee_id,
                                    REGION = query.region,
                                    PROVINCE = query.province,
                                    DISTRICT = query.district,
                                    SUB_DISTRICT = query.sub_district,
                                    ADDRESS_ID = query.address_id,
                                    SERENADE_FLAG = query.serenade_flag,
                                    PENALTY_FLAG = query.penalty_flag,
                                    PACKAGE_MAIN = query.package_main,
                                    PRODUCT_SUBTYPE = query.product_subtype,
                                    AIR_PROJECT_COND_FLAG_ARRAY = listCondflag,
                                    LOCATION_PROVINCE = query.location_Province.ToSafeString()
                                };



                                var result = new ListPackageChangeOnlineModel();

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
                                    result = JsonConvert.DeserializeObject<ListPackageChangeOnlineModel>(responseData.Content) ?? new ListPackageChangeOnlineModel();
                                    if (result != null)
                                    {
                                        if (result.LIST_PACKAGE_BY_CHANGE != null && result.LIST_PACKAGE_BY_CHANGE.Count > 0)
                                        {
                                            foreach (var item in result.LIST_PACKAGE_BY_CHANGE)
                                            {
                                                item.discount_day = item.discount_day ?? 0;
                                                item.discount_value = item.discount_value ?? 0;
                                                item.recurring_charge = item.price_charge.ToSafeDecimal();
                                                item.sff_promotion_bill_tha = item.package_display_tha;
                                                item.sff_promotion_bill_eng = item.package_display_eng;
                                            }
                                            executeResult = result.LIST_PACKAGE_BY_CHANGE;

                                            result.RESULT_CODE = "0";
                                            result.RESULT_CODE = responseData.StatusCode.ToString();

                                            InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, executeResult, log, "Success", "", "FBBOR016");

                                            log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, config, query.non_mobile_no + query.client_ip, "GetListPackageChangeOnlineQuery", "GetListPackageChangeOnlineQuery", null, "FBB|" + query.FullUrl, "FBBOR016");
                                        }

                                        InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, content, log, "Success", "", "FBBOR016");
                                    }
                                    else
                                    {
                                        result.RESULT_CODE = "1";
                                        result.RESULT_DESC = "result null";
                                        InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, content, log, "Failed", "", "FBBOR016");
                                    }
                                }
                                else
                                {
                                    result.RESULT_CODE = "1";
                                    result.RESULT_DESC = responseData.StatusCode.ToString();
                                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, content, log, "Failed", "", "FBBOR016");
                                }

                            }
                            catch (Exception ex)
                            {
                                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, new List<ListPackageChangeModel>(), log, "Failed", ex.Message, "FBBOR016");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex, log, "Failed", "inner" + ex.InnerException + " Message:" + ex.Message, "FBBOR016");
                }

            }

            return executeResult;
        }
    }

    public class GetListChangePackageQueryHandler : IQueryHandler<GetListChangePackageQuery, List<ListChangePackageModel>>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IAirNetEntityRepository<ListChangePackageModel> _objService;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<FBB_CFG_LOV> _lov;
        private readonly IEntityRepository<FBB_ORD_CHANGE_PACKAGE> _OrdChangePackageTable;

        public GetListChangePackageQueryHandler(ILogger logger,
            IWBBUnitOfWork uow,
            IAirNetEntityRepository<ListChangePackageModel> objService,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IEntityRepository<FBB_ORD_CHANGE_PACKAGE> OrdChangePackageTable,
            IEntityRepository<FBB_CFG_LOV> lov)
        {
            _logger = logger;
            _uow = uow;
            _objService = objService;
            _intfLog = intfLog;
            _OrdChangePackageTable = OrdChangePackageTable;
            _lov = lov;
        }

        public List<ListChangePackageModel> Handle(GetListChangePackageQuery query)
        {
            InterfaceLogCommand log = null;
            DateTime Curr_DateTime = DateTime.Now;
            log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.non_mobile_no + query.client_ip, "GetListChangePackageQuery", "AIR_ADMIN.PKG_AIROR905.LIST_CHANGE_PACKAGE", null, "FBB|" + query.FullUrl, "FBBOR016");
            List<ListChangePackageModel> result = null;

            try
            {
                //var ioResults = new OracleParameter();
                //ioResults.ParameterName = "ioResults";
                //ioResults.OracleDbType = OracleDbType.RefCursor;
                //ioResults.Direction = ParameterDirection.Output;

                //var airChageOldPackageObjectModel = new AirChagePackageObjectModel();
                //airChageOldPackageObjectModel.AIR_CHANGE_PACKAGE_ARRAY = query.AirChangePromotionCodeOldArray.Select(a => new Air_Change_Package_ArrayMapping
                //{
                //    SFF_PROMOTION_CODE = a.SFF_PROMOTION_CODE,
                //    startDt = a.startDt,
                //    endDt = a.endDt,
                //    PRODUCT_SEQ = a.PRODUCT_SEQ
                //}).ToArray();

                //var airChageNewPackageObjectModel = new AirChagePackageObjectModel();
                //airChageNewPackageObjectModel.AIR_CHANGE_PACKAGE_ARRAY = query.AirChangePromotionCodeNewArray.Select(a => new Air_Change_Package_ArrayMapping
                //{
                //    SFF_PROMOTION_CODE = a.SFF_PROMOTION_CODE,
                //    startDt = null,
                //    endDt = null,
                //    PRODUCT_SEQ = a.PRODUCT_SEQ
                //}).ToArray();

                //var changeOldPackage = OracleCustomTypeUtilities.CreateUDTArrayParameter("p_AIR_CHANGE_OLD_PACKAGE_ARRAY", "AIR_CHANGE_PACKAGE_ARRAY", airChageOldPackageObjectModel);
                //var changeNewPackage = OracleCustomTypeUtilities.CreateUDTArrayParameter("p_AIR_CHANGE_NEW_PACKAGE_ARRAY", "AIR_CHANGE_PACKAGE_ARRAY", airChageNewPackageObjectModel);

                //var outp = new List<object>();
                //var paramOut = outp.ToArray();

                //result = _objService.ExecuteReadStoredProc("AIR_ADMIN.PKG_AIROR905.LIST_CHANGE_PACKAGE",
                //  new
                //  {
                //      p_non_mobile_no = query.non_mobile_no,
                //      p_relate_mobile = query.relate_mobile,
                //      p_network_type = query.network_type,
                //      p_current_project_name = query.current_project_name,


                //      /// input array
                //      p_air_change_old_package_array = changeOldPackage,
                //      p_air_change_new_package_array = changeNewPackage,

                //      /// Output Curser
                //      ioResults = ioResults

                //  }).ToList();

                airChangePackageRecord[] changeOldPackage = query.AirChangePromotionCodeOldArray.Select(a => new airChangePackageRecord
                {
                    sffPromotionCode = a.SFF_PROMOTION_CODE,
                    startdt = a.startDt,
                    enddt = a.endDt,
                    productSeq = a.PRODUCT_SEQ
                }).ToArray();
                airChangePackageRecord[] changeNewPackage = query.AirChangePromotionCodeNewArray.Select(a => new airChangePackageRecord
                {
                    sffPromotionCode = a.SFF_PROMOTION_CODE,
                    startdt = a.startDt,
                    enddt = a.endDt,
                    productSeq = a.PRODUCT_SEQ
                }).ToArray();

                //airChangePackageRecord[] changePlayboxPackage = null;
                //if (query.AirChangePlayBoxPromotionCodeNewArray != null)
                //{
                //    changePlayboxPackage = query.AirChangePlayBoxPromotionCodeNewArray.Select(a => new airChangePackageRecord
                //    {
                //        sffPromotionCode = a.SFF_PROMOTION_CODE,
                //        startdt = a.startDt,
                //        enddt = a.endDt,
                //        productSeq = a.PRODUCT_SEQ
                //    }).ToArray();
                //}
                //else
                //{
                //    List<airChangePackageRecord> changePlayboxPackageList = new List<airChangePackageRecord>();
                //    airChangePackageRecord airChangePackageRecord = new airChangePackageRecord() { sffPromotionCode = "P11111111", startdt = null, enddt = null, productSeq = "" };
                //     changePlayboxPackageList.Add(airChangePackageRecord);
                //     changePlayboxPackage = changePlayboxPackageList.ToArray();                  
                //}
                SBNV2WebService.sbnAirWorkflowResponse objResp = new SBNV2WebService.sbnAirWorkflowResponse();

                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                ServicePointManager.ServerCertificateValidationCallback =
                    (s, certificate, chain, sslPolicyErrors) => true;

                using (var service = new SBNV2WebService.AIRInterfaceWorkflowServiceService())
                {
                    service.Timeout = 600000;

                    string tmpUrl = (from r in _lov.Get()
                                     where r.LOV_NAME == "SaveOrderNewURL" && r.ACTIVEFLAG == "Y"
                                     select r.LOV_VAL1).FirstOrDefault().ToSafeString();
                    if (tmpUrl != "")
                    {
                        service.Url = tmpUrl;
                    }

                    objResp = service.getListChangePackage(query.non_mobile_no
                                                            , query.relate_mobile
                                                            , query.network_type
                                                            , query.existing_mobile_flag
                                                            , query.current_project_name
                                                            , query.current_project_name_opt
                                                            , query.current_mobile_chk_right
                                                            , query.current_mobile_chk_right_opt
                                                            , query.current_mobile_get_benefit
                                                            , query.current_mobile_get_benefit_opt
                                                            , query.new_mobile_chk_right
                                                            , query.new_mobile_get_benefit
                                                            //, query.relate_mobile != query.oldRelateMobile ? "Y" : "N"
                                                            //, query.oldRelateMobile
                                                            , changeOldPackage, changeNewPackage);
                }
                result = objResp.listChangePackageResult.Select(r => new ListChangePackageModel
                {
                    action_status = r.ACTION_STATUS,
                    error_code = r.ERROR_CODE,
                    error_msg = r.ERROR_MSG,
                    non_mobile_no = r.NON_MOBILE_NO,
                    order_no = r.ORDER_NO,
                    package_state = r.PACKAGE_STATE,
                    product_seq = r.PRODUCT_SEQ,
                    project_name = r.PROJECT_NAME,
                    relate_mobile = r.RELATE_MOBILE,
                    sff_promotion_code = r.SFF_PROMOTION_CODE,
                    old_relate_mobile = r.OLD_RELATE_MOBILE,
                    bundling_mobile_action = r.BUNDLING_MOBILE_ACTION,

                    // R20.6 Add by Aware : Atipon
                    send_sff_flag = r.SEND_SFF_FLAG,
                    new_project_name = r.NEW_PROJECT_NAME,
                    new_project_name_opt = r.NEW_PROJECT_NAME_OPT,
                    new_mobile_check_right = r.NEW_MOBILE_CHECK_RIGHT,
                    new_mobile_check_right_opt = r.NEW_MOBILE_CHECK_RIGHT_OPT,
                    new_mobile_get_benefit = r.NEW_MOBILE_GET_BENEFIT,
                    new_mobile_get_benefit_opt = r.NEW_MOBILE_GET_BENEFIT_OPT,

                    change_benefit_type_from = r.CHANGE_BENEFIT_TYPE_FROM,
                    change_benefit_type_to = r.CHANGE_BENEFIT_TYPE_TO,
                    change_mobile_benefit_from = r.CHANGE_MOBILE_BENEFIT_FROM,
                    change_mobile_benefit_to = r.CHANGE_MOBILE_BENEFIT_TO
                }).ToList();

                var resultList = result.FirstOrDefault();
                if (resultList != null)
                {
                    var wordSummary = MapWordingSummary(resultList, query.Language);

                    result.ForEach(p => p.wordingChangePromotionSummary = wordSummary);
                }

                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, "Success", "", "FBBOR016");


                //R20.6 move to CreateOrderChangePromotionCommandHandler
                //
                //DateTime CurrDt = DateTime.Now;
                //foreach (var tmp in result)
                //{
                //    FBB_ORD_CHANGE_PACKAGE newrow = new FBB_ORD_CHANGE_PACKAGE()
                //    {
                //        ORDER_NO = tmp.order_no.ToSafeString(),
                //        NON_MOBILE_NO = tmp.non_mobile_no.ToSafeString(),
                //        RELATE_MOBILE = tmp.relate_mobile.ToSafeString(),
                //        SFF_PROMOTION_CODE = tmp.sff_promotion_code.ToSafeString(),
                //        ACTION_STATUS = tmp.action_status.ToSafeString(),
                //        PACKAGE_STATE = tmp.package_state.ToSafeString(),
                //        PROJECT_NAME = tmp.project_name.ToSafeString(),
                //        CREATED_BY = "FBBOR016",
                //        CREATED_DATE = CurrDt,
                //        UPDATED_BY = "FBBOR016",
                //        UPDATED_DATE = CurrDt,
                //        PRODUCT_SEQ = tmp.product_seq.ToSafeString(),
                //        BUNDLING_ACTION = query.acTion,
                //        OLD_RELATE_MOBILE = query.oldRelateMobile,
                //        MOBILE_CONTACT = query.mobileNumberContact
                //    };
                //    _OrdChangePackageTable.Create(newrow);                    
                //}
                //_uow.Persist();

                return result;
            }
            catch (Exception ex)
            {
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, "Failed", ex.Message, "FBBOR016");

                return result;
            }
        }

        private string MapWordingSummary(ListChangePackageModel listChangePackage, string Language)
        {
            var resultWording = string.Empty;
            try
            {
                if (listChangePackage == null) return resultWording; //ไม่มีอะไรเปลี่ยนแปลง ไม่ต้องแสดง

                var inputparam = (from z in _lov.Get()
                                  where z.ACTIVEFLAG == "Y" && z.LOV_TYPE == "CONFIG_MSG_CP_RIGHT"
                                  select z).ToList();

                var messageCase1_1 = "";
                var messageCase1_2 = "";
                var messageCase1_3 = "";
                var messageCase1_4 = "";
                var messageCase2_1 = "";
                var messageCase2_2 = "";
                var messageCase3_1 = "";
                var messageCase3_2 = "";

                if (Language == "1")
                {
                    messageCase1_1 = inputparam.FirstOrDefault(p => p.LOV_NAME == "MESSAGE_CASE_1_1") != null ? inputparam.FirstOrDefault(p => p.LOV_NAME == "MESSAGE_CASE_1_1").LOV_VAL1 : "";
                    messageCase1_2 = inputparam.FirstOrDefault(p => p.LOV_NAME == "MESSAGE_CASE_1_2") != null ? inputparam.FirstOrDefault(p => p.LOV_NAME == "MESSAGE_CASE_1_2").LOV_VAL1 : "";
                    messageCase1_3 = inputparam.FirstOrDefault(p => p.LOV_NAME == "MESSAGE_CASE_1_3") != null ? inputparam.FirstOrDefault(p => p.LOV_NAME == "MESSAGE_CASE_1_3").LOV_VAL1 : "";
                    messageCase1_4 = inputparam.FirstOrDefault(p => p.LOV_NAME == "MESSAGE_CASE_1_4") != null ? inputparam.FirstOrDefault(p => p.LOV_NAME == "MESSAGE_CASE_1_4").LOV_VAL1 : "";
                    messageCase2_1 = inputparam.FirstOrDefault(p => p.LOV_NAME == "MESSAGE_CASE_2_1") != null ? inputparam.FirstOrDefault(p => p.LOV_NAME == "MESSAGE_CASE_2_1").LOV_VAL1 : "";
                    messageCase2_2 = inputparam.FirstOrDefault(p => p.LOV_NAME == "MESSAGE_CASE_2_2") != null ? inputparam.FirstOrDefault(p => p.LOV_NAME == "MESSAGE_CASE_2_2").LOV_VAL1 : "";
                    messageCase3_1 = inputparam.FirstOrDefault(p => p.LOV_NAME == "MESSAGE_CASE_3_1") != null ? inputparam.FirstOrDefault(p => p.LOV_NAME == "MESSAGE_CASE_3_1").LOV_VAL1 : "";
                    messageCase3_2 = inputparam.FirstOrDefault(p => p.LOV_NAME == "MESSAGE_CASE_3_2") != null ? inputparam.FirstOrDefault(p => p.LOV_NAME == "MESSAGE_CASE_3_2").LOV_VAL1 : "";
                }
                else
                {
                    messageCase1_1 = inputparam.FirstOrDefault(p => p.LOV_NAME == "MESSAGE_CASE_1_1") != null ? inputparam.FirstOrDefault(p => p.LOV_NAME == "MESSAGE_CASE_1_1").LOV_VAL2 : "";
                    messageCase1_2 = inputparam.FirstOrDefault(p => p.LOV_NAME == "MESSAGE_CASE_1_2") != null ? inputparam.FirstOrDefault(p => p.LOV_NAME == "MESSAGE_CASE_1_2").LOV_VAL2 : "";
                    messageCase1_3 = inputparam.FirstOrDefault(p => p.LOV_NAME == "MESSAGE_CASE_1_3") != null ? inputparam.FirstOrDefault(p => p.LOV_NAME == "MESSAGE_CASE_1_3").LOV_VAL2 : "";
                    messageCase1_4 = inputparam.FirstOrDefault(p => p.LOV_NAME == "MESSAGE_CASE_1_4") != null ? inputparam.FirstOrDefault(p => p.LOV_NAME == "MESSAGE_CASE_1_4").LOV_VAL2 : "";
                    messageCase2_1 = inputparam.FirstOrDefault(p => p.LOV_NAME == "MESSAGE_CASE_2_1") != null ? inputparam.FirstOrDefault(p => p.LOV_NAME == "MESSAGE_CASE_2_1").LOV_VAL2 : "";
                    messageCase2_2 = inputparam.FirstOrDefault(p => p.LOV_NAME == "MESSAGE_CASE_2_2") != null ? inputparam.FirstOrDefault(p => p.LOV_NAME == "MESSAGE_CASE_2_2").LOV_VAL2 : "";
                    messageCase3_1 = inputparam.FirstOrDefault(p => p.LOV_NAME == "MESSAGE_CASE_3_1") != null ? inputparam.FirstOrDefault(p => p.LOV_NAME == "MESSAGE_CASE_3_1").LOV_VAL2 : "";
                    messageCase3_2 = inputparam.FirstOrDefault(p => p.LOV_NAME == "MESSAGE_CASE_3_2") != null ? inputparam.FirstOrDefault(p => p.LOV_NAME == "MESSAGE_CASE_3_2").LOV_VAL2 : "";
                }

                var newMobileMain = inputparam.FirstOrDefault(p => p.LOV_NAME == "NEW_MOBILE_MAIN") != null ? inputparam.FirstOrDefault(p => p.LOV_NAME == "NEW_MOBILE_MAIN").LOV_VAL1 : "";
                var existingMobileOntop = inputparam.FirstOrDefault(p => p.LOV_NAME == "EXISTING_MOBILE_ONTOP") != null ? inputparam.FirstOrDefault(p => p.LOV_NAME == "EXISTING_MOBILE_ONTOP").LOV_VAL1 : "";
                var existingMobile = inputparam.FirstOrDefault(p => p.LOV_NAME == "EXISTING_MOBILE") != null ? inputparam.FirstOrDefault(p => p.LOV_NAME == "EXISTING_MOBILE").LOV_VAL1 : "";

                var v_change_mobile_benefit_from = listChangePackage.change_mobile_benefit_from.ToSafeTrim();
                var v_change_mobile_benefit_to = listChangePackage.change_mobile_benefit_to.ToSafeTrim();
                var v_change_benefit_type_from = listChangePackage.change_benefit_type_from.ToSafeTrim();
                var v_change_benefit_type_to = listChangePackage.change_benefit_type_to.ToSafeTrim();

                var v1_change_benefit_type_from = string.Empty;
                var v1_change_benefit_type_to = string.Empty;

                //V_CHANGE_BENEFIT_TYPE_FROM
                if (v_change_benefit_type_from.ToCompareText(existingMobile))
                {
                    v1_change_benefit_type_from = existingMobile;
                }
                else if (v_change_benefit_type_from.ToCompareText(existingMobileOntop))
                {
                    v1_change_benefit_type_from = existingMobile;
                }
                else
                {
                    v1_change_benefit_type_from = v_change_benefit_type_from;
                }

                //V_CHANGE_BENEFIT_TYPE_TO
                if (v_change_benefit_type_to.ToCompareText(existingMobile))
                {
                    v1_change_benefit_type_to = existingMobile;
                }
                else if (v_change_benefit_type_to.ToCompareText(existingMobileOntop))
                {
                    v1_change_benefit_type_to = existingMobile;
                }
                else
                {
                    v1_change_benefit_type_to = v_change_benefit_type_to;
                }

                //Set Message
                if (v_change_mobile_benefit_from.ToCompareText(v_change_mobile_benefit_to))
                {
                    if (v1_change_benefit_type_from.ToCompareText(v1_change_benefit_type_to))
                    {
                        // ไม่มีอะไรเปลี่ยนแปลง ไม่ต้องแสดง
                        return string.Empty;
                    }
                    else
                    {
                        // มีการเปลี่ยนแปลง type เบอร์เดิม
                        if (string.IsNullOrEmpty(v1_change_benefit_type_from))
                        {
                            if (v1_change_benefit_type_to.ToCompareText(newMobileMain))
                            {
                                // Case 1 mobile รับสิทธิ์ ไม่เปลี่ยน แต่เปลี่ยนแปลงการรับสิทธิ์
                                // Case 1.3 จาก project ใดๆ หรือ non project -> main

                                resultWording = messageCase1_3;

                            }
                            else if (v1_change_benefit_type_to.ToCompareText(existingMobile))
                            {
                                // Case 1 mobile รับสิทธิ์ ไม่เปลี่ยน แต่เปลี่ยนแปลงการรับสิทธิ์
                                // Case 1.4 จาก project ใดๆ หรือ non project -> ontop
                                resultWording = messageCase1_4;
                            }
                            else
                            {
                                // ยังไม่ได้รับสิทธิ์ใดๆ ไม่ต้องแจ้ง
                                return string.Empty;
                            }

                        }
                        else // v1_change_benefit_type_from not is null
                        {
                            if (v1_change_benefit_type_from.ToCompareText(newMobileMain))
                            {
                                if (v1_change_benefit_type_to.ToCompareText(existingMobile))
                                {
                                    // Case 1 mobile รับสิทธิ์ ไม่เปลี่ยน แต่เปลี่ยนแปลงการรับสิทธิ์
                                    //Case 1.2 จาก main -> ontop
                                    resultWording = messageCase1_2;
                                }
                            }
                            else if (v1_change_benefit_type_from.ToCompareText(existingMobile))
                            {
                                if (v1_change_benefit_type_to.ToCompareText(newMobileMain))
                                {
                                    // Case 1 mobile รับสิทธิ์ ไม่เปลี่ยน แต่เปลี่ยนแปลงการรับสิทธิ์
                                    // Case 1.1 จาก ontop -> main
                                    resultWording = messageCase1_1;
                                }
                            }
                        }
                    }
                }
                else //nvl(v_change_mobile_benefit_from,'-') <> nvl(v_change_mobile_benefit_to,'-') // มีการเปลี่ยนแปลงเบอร์
                {
                    if (v1_change_benefit_type_from.ToCompareText(v1_change_benefit_type_to))
                    {
                        // ไม่มีการเปลี่ยนแปลง type
                        //Case 2 mobile รับสิทธิ์เปลี่ยน แต่ไม่เปลี่ยนการรับสิทธิ์
                        if (v1_change_benefit_type_to.ToCompareText(newMobileMain))
                        {
                            //Case 2.1 mobile รับสิทธิ์ 081 -> 082 รับสิทธิ์ main
                            resultWording = messageCase2_1;

                        }
                        else if (v1_change_benefit_type_to.ToCompareText(existingMobile))
                        {
                            //Case 2.2 mobile รับสิทธิ์ 081 -> 082 รับสิทธิ์ ontop  
                            resultWording = messageCase2_2;
                        }

                    }
                    else //v_change_benefit_type_from <> v_change_benefit_type_to
                    {
                        // Case 3 mobile รับสิทธิ์เปลี่ยน และ เปลี่ยนแปลงการรับสิทธิ์
                        if (v1_change_benefit_type_from.ToCompareText(newMobileMain))
                        {
                            if (v1_change_benefit_type_to.ToCompareText(existingMobile))
                            {
                                //Case 3.1 081 รับสิทธิ์ main -> 082 รับสิทธิ์ ontop
                                resultWording = messageCase3_1;
                            }
                        }
                        else if (v1_change_benefit_type_from.ToCompareText(existingMobile))
                        {
                            if (v1_change_benefit_type_to.ToCompareText(newMobileMain))
                            {
                                //Case 3.2 081 รับสิทธิ์ ontop -> 082 รับสิทธิ์ main
                                resultWording = messageCase3_2;
                            }
                        }
                        else if (string.IsNullOrEmpty(v1_change_benefit_type_from))
                        {
                            if (v1_change_benefit_type_to.ToCompareText(newMobileMain))
                            {
                                // Case 1.3 จาก project ใดๆ หรือ non project -> main
                                resultWording = messageCase1_3;
                            }
                            else if (v1_change_benefit_type_to.ToCompareText(existingMobile))
                            {
                                // Case 1.4 จาก project ใดๆ หรือ non project -> ontop
                                resultWording = messageCase1_4;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                return "-";
            }

            return resultWording;
        }
    }

    #region Mapping air_change_package_array Type Oracle

    public class AirChagePackageObjectModel : Oracle.ManagedDataAccess.Types.INullable, IOracleCustomType
    {
        [OracleArrayMapping()]
        public Air_Change_Package_ArrayMapping[] AIR_CHANGE_PACKAGE_ARRAY { get; set; }

        private bool objectIsNull;

        public bool IsNull
        {
            get { return objectIsNull; }
        }

        public static AirChagePackageObjectModel Null
        {
            get
            {
                AirChagePackageObjectModel obj = new AirChagePackageObjectModel();
                obj.objectIsNull = true;
                return obj;
            }
        }

        public void FromCustomObject(OracleConnection con, object udt)
        {
            OracleUdt.SetValue(con, udt, 0, AIR_CHANGE_PACKAGE_ARRAY);
        }

        public void ToCustomObject(OracleConnection con, object udt)
        {
            AIR_CHANGE_PACKAGE_ARRAY = (Air_Change_Package_ArrayMapping[])OracleUdt.GetValue(con, udt, 0);
        }
    }

    [OracleCustomTypeMappingAttribute("AIR_CHANGE_PACKAGE_RECORD")]
    public class AirChagePackageOracleTypeMappingFactory : IOracleCustomTypeFactory
    {
        public IOracleCustomType CreateObject()
        {
            return new Air_Change_Package_ArrayMapping();
        }
    }

    [OracleCustomTypeMapping("AIR_CHANGE_PACKAGE_ARRAY")]
    public class AirChagePackageObjectModelFactory : IOracleCustomTypeFactory, IOracleArrayTypeFactory
    {
        #region IOracleCustomTypeFactory Members

        public IOracleCustomType CreateObject()
        {
            return new AirChagePackageObjectModel();
        }

        #endregion IOracleCustomTypeFactory Members

        #region IOracleArrayTypeFactory Members

        public Array CreateArray(int numElems)
        {
            return new Air_Change_Package_ArrayMapping[numElems];
        }

        public Array CreateStatusArray(int numElems)
        {
            return null;
        }

        #endregion IOracleArrayTypeFactory Members
    }

    public class Air_Change_Package_ArrayMapping : Oracle.ManagedDataAccess.Types.INullable, IOracleCustomType
    {
        private bool objectIsNull;

        #region Attribute Mapping

        [OracleObjectMappingAttribute("SFF_PROMOTION_CODE")]
        public string SFF_PROMOTION_CODE { get; set; }
        [OracleObjectMappingAttribute("STARTDT")]
        public string startDt { get; set; }
        [OracleObjectMappingAttribute("ENDDT")]
        public string endDt { get; set; }
        [OracleObjectMappingAttribute("PRODUCT_SEQ")]
        public string PRODUCT_SEQ { get; set; }

        #endregion Attribute Mapping

        public static Air_Change_Package_ArrayMapping Null
        {
            get
            {
                Air_Change_Package_ArrayMapping obj = new Air_Change_Package_ArrayMapping();
                obj.objectIsNull = true;
                return obj;
            }
        }

        public bool IsNull
        {
            get { return objectIsNull; }
        }

        public void FromCustomObject(OracleConnection con, object udt)
        {
            OracleUdt.SetValue(con, udt, "SFF_PROMOTION_CODE", SFF_PROMOTION_CODE);
            OracleUdt.SetValue(con, udt, "STARTDT", startDt);
            OracleUdt.SetValue(con, udt, "ENDDT", endDt);
            OracleUdt.SetValue(con, udt, "PRODUCT_SEQ", PRODUCT_SEQ);
        }

        public void ToCustomObject(OracleConnection con, object udt)
        {
            throw new NotImplementedException();
        }
    }

    #endregion Mapping  fbb_event_sub_array Type Oracle

}
