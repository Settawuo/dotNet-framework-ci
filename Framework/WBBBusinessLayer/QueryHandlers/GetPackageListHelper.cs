using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net;
using WBBBusinessLayer.CommandHandlers;
using WBBContract.Commands;
using WBBContract.Queries.Commons.Master;
using WBBContract.Queries.ExWebServices;
using WBBContract.Queries.ExWebServices.FbbCpGw;
using WBBContract.Queries.WebServices;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.ExWebServiceModels;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.ExWebServices.FbbCpGw
{
    public static class GetPackageListHelper
    {
        public static InterfaceLogCommand StartInterfaceAirWfLog<T>(IWBBUnitOfWork uow,
           IEntityRepository<FBB_INTERFACE_LOG> interfaceLog,
           T query, string transactionId, string serviceName,
           string methodName)
        {
            var dbIntfCmd = new InterfaceLogCommand
            {
                ActionType = WBBContract.Commands.ActionType.Insert,
                IN_TRANSACTION_ID = transactionId,
                METHOD_NAME = methodName,
                SERVICE_NAME = serviceName,
                IN_XML_PARAM = query.DumpToXml(),
                INTERFACE_NODE = "FBB",
                CREATED_BY = "FbbSbnInterfaceLog",

            };

            var log = InterfaceLogHelper.Log(interfaceLog, dbIntfCmd);
            uow.Persist();

            dbIntfCmd.OutInterfaceLogId = log.INTERFACE_ID;
            return dbIntfCmd;
        }

        public static void EndInterfaceAirWfLog<T>(IWBBUnitOfWork uow,
            IEntityRepository<FBB_INTERFACE_LOG> interfaceLog,
            T output, InterfaceLogCommand dbIntfCmd,
            string result, string reason)
        {
            dbIntfCmd.ActionType = WBBContract.Commands.ActionType.Update;
            dbIntfCmd.REQUEST_STATUS = (result == "Success") ? "Success" : "Error";
            dbIntfCmd.OUT_RESULT = result;
            dbIntfCmd.OUT_ERROR_RESULT = (result == "Success") ? (reason.Length > 100 ? reason.Substring(0, 100) : result) : result;
            dbIntfCmd.OUT_XML_PARAM = output.DumpToXml();

            InterfaceLogHelper.Log(interfaceLog, dbIntfCmd);
            uow.Persist();
        }

        /// <summary>
        /// ListPackageByService AirNet Workflow Service
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public static List<PackageModel> GetPackageList(ILogger logger, IGetPackageListQuery query, IEntityRepository<FBB_CFG_LOV> _lov)
        {
            var model = new List<PackageModel>();

            var SBNStatus = _lov.Get(l => l.LOV_TYPE == ("FBB_CONSTANT") && l.LOV_NAME == ("SBN_SERVICE")).FirstOrDefault();


            System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
            stopWatch.Start();
            if (SBNStatus.LOV_VAL1 == "NEW")
            {
                #region newSBNService

                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                ServicePointManager.ServerCertificateValidationCallback =
                    (s, certificate, chain, sslPolicyErrors) => true;

                using (var service = new SBNNewWebService.SBNWebServiceService())
                {
                    //TODO: for log Change pack fieldwork R19.5
                    //var transactionId = query.TransactionID.ToSafeString();
                    //if (transactionId.IndexOf(":", System.StringComparison.Ordinal) >= 0)
                    //{
                    //    var result = transactionId.Substring(transactionId.LastIndexOf(':'));
                    //    transactionId = transactionId.Substring(0, transactionId.Length - result.Length);
                    //}
                    //var userReferencetransaction = string.Format("{0}-{1}-{2}", query.P_Address_Id.ToSafeString(), transactionId, query.SessionId);
                    var userReferencetransaction = query.SessionId;
                    service.Credentials = new NetworkCredential(userReferencetransaction, "");
                    //
                    service.Timeout = 600000;

                    var data = service.listPackageByService(query.P_OWNER_PRODUCT, query.P_PRODUCT_SUBTYPE,
                        query.P_NETWORK_TYPE, query.P_SERVICE_DAY,
                        query.P_PACKAGE_FOR, query.P_PACKAGE_CODE, query.P_Location_Code, query.P_Asc_Code, query.P_Partner_Type, query.P_Partner_SubType,
                        query.P_Region, query.P_Province, query.P_District, query.P_Sub_District, query.P_Address_Type,
                        query.P_Building_Name, query.P_Building_No, query.P_Serenade_Flag, query.P_Customer_Type, query.P_Address_Id,
                        query.P_Plug_And_Play_Flag, query.P_Rental_Flag, query.P_Customer_subtype, query.P_Router_Flag, query.P_FMPA_Flag, query.P_Mobile_Price, query.P_Service_Year, query.P_Mobile_No, query.P_EMPLOYEE_ID, query.P_CVM_FLAG, "");

                    if (data.RETURN_CODE != 0
                        || null == data.packageByServiceArray)
                        logger.Info(data.RETURN_MESSAGE);
                    else
                    {
                        var packages = data.packageByServiceArray.Select(s => new PackageModel
                        {
                            MAPPING_CODE = s.MAPPING_CODE.ToSafeString(),
                            MAPPING_PRODUCT = s.MAPPING_PRODUCT.ToSafeString(),
                            PACKAGE_CODE = s.PACKAGE_CODE.ToSafeString(),
                            PACKAGE_NAME = s.PACKAGE_NAME.ToSafeString(),
                            PACKAGE_GROUP = s.PACKAGE_GROUP.ToSafeString(),
                            RECURRING_CHARGE = s.RECURRING_CHARGE.ToSafeDecimal(),
                            PRE_RECURRING_CHARGE = s.PRE_RECURRING_CHARGE.ToSafeDecimal(),
                            SFF_PROMOTION_BILL_THA = s.SFF_PROMOTION_BILL_THA.ToSafeString(),
                            SFF_PROMOTION_BILL_ENG = s.SFF_PROMOTION_BILL_ENG.ToSafeString(),
                            TECHNOLOGY = s.TECHNOLOGY.ToSafeString(),
                            DOWNLOAD_SPEED = s.DOWNLOAD_SPEED.ToSafeString(),
                            UPLOAD_SPEED = s.UPLOAD_SPEED.ToSafeString(),
                            INITIATION_CHARGE = s.INITIATION_CHARGE.ToSafeDecimal(),
                            PRE_INITIATION_CHARGE = s.PRE_INITIATION_CHARGE.ToSafeDecimal(),
                            PACKAGE_TYPE = s.PACKAGE_TYPE.ToSafeString(),
                            PRODUCT_SUBTYPE = s.PRODUCT_SUBTYPE.ToSafeString(),
                            OWNER_PRODUCT = s.OWNER_PRODUCT.ToSafeString(),

                            ACCESS_MODE = s.ACCESS_MODE.ToSafeString(),
                            SERVICE_CODE = s.SERVICE_CODE.ToSafeString(),

                            PRODUCT_SUBTYPE3 = s.PRODUCT_SUBTYPE3.ToSafeString(),

                            DISCOUNT_TYPE = s.DISCOUNT_TYPE.ToSafeString(),
                            DISCOUNT_VALUE = s.DISCOUNT_VALUE,
                            DISCOUNT_DAY = s.DISCOUNT_DAY,
                            SFF_PROMOTION_CODE = s.SFF_PROMOTION_CODE.ToSafeString(),

                            AUTO_MAPPING = s.AUTO_MAPPING.ToSafeString(),
                            DISPLAY_FLAG = s.DISPLAY_FLAG.ToSafeString(),
                            DISPLAY_SEQ = s.DISPLAY_SEQ.ToSafeString(),

                            MOBILE_PRICE = query.P_Mobile_Price.ToSafeString(),
                            EXISTING_MOBILE = query.P_Existing_Mobile.ToSafeString()

                        });

                        model = packages.ToList();
                    }
                }
                #endregion
            }
            else
            {
                #region oldSBNService

                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                ServicePointManager.ServerCertificateValidationCallback =
                    (s, certificate, chain, sslPolicyErrors) => true;

                using (var service = new SBNWebService.SBNWebServiceService())
                {
                    service.Timeout = 600000;
                    var data = service.listPackageByService(query.P_OWNER_PRODUCT, query.P_PRODUCT_SUBTYPE,
                        query.P_NETWORK_TYPE, query.P_SERVICE_DAY,
                        query.P_PACKAGE_FOR, query.P_PACKAGE_CODE, query.P_Location_Code, query.P_Asc_Code, query.P_Partner_Type, query.P_Partner_SubType,
                        query.P_Region, query.P_Province, query.P_District, query.P_Sub_District, query.P_Address_Type,
                        query.P_Building_Name, query.P_Building_No);

                    if (data.RETURN_CODE.GetValueOrDefault() != 0
                        || null == data.PackageByServiceArray)
                        logger.Info(data.RETURN_MESSAGE);
                    else
                    {
                        var packages = data.PackageByServiceArray.Select(s => new PackageModel
                        {
                            MAPPING_CODE = s.MAPPING_CODE.ToSafeString(),
                            MAPPING_PRODUCT = s.MAPPING_PRODUCT.ToSafeString(),
                            PACKAGE_CODE = s.PACKAGE_CODE.ToSafeString(),
                            PACKAGE_NAME = s.PACKAGE_NAME.ToSafeString(),
                            PACKAGE_GROUP = s.PACKAGE_GROUP.ToSafeString(),
                            RECURRING_CHARGE = s.RECURRING_CHARGE.ToSafeInteger(),
                            PRE_RECURRING_CHARGE = s.PRE_RECURRING_CHARGE.ToSafeInteger(),
                            SFF_PROMOTION_BILL_THA = s.SFF_PROMOTION_BILL_THA.ToSafeString(),
                            SFF_PROMOTION_BILL_ENG = s.SFF_PROMOTION_BILL_ENG.ToSafeString(),
                            TECHNOLOGY = s.TECHNOLOGY.ToSafeString(),
                            DOWNLOAD_SPEED = s.DOWNLOAD_SPEED.ToSafeString(),
                            UPLOAD_SPEED = s.UPLOAD_SPEED.ToSafeString(),
                            INITIATION_CHARGE = s.INITIATION_CHARGE.ToSafeInteger(),
                            PRE_INITIATION_CHARGE = s.PRE_INITIATION_CHARGE.ToSafeInteger(),
                            PACKAGE_TYPE = s.PACKAGE_TYPE.ToSafeString(),
                            PRODUCT_SUBTYPE = s.PRODUCT_SUBTYPE.ToSafeString(),
                            OWNER_PRODUCT = s.OWNER_PRODUCT.ToSafeString(),

                            ACCESS_MODE = s.ACCESS_MODE.ToSafeString(),
                            SERVICE_CODE = s.SERVICE_CODE.ToSafeString(),

                            PRODUCT_SUBTYPE3 = s.PRODUCT_SUBTYPE3.ToSafeString(),

                            DISCOUNT_TYPE = s.DISCOUNT_TYPE.ToSafeString(),
                            DISCOUNT_VALUE = s.DISCOUNT_VALUE,
                            DISCOUNT_DAY = s.DISCOUNT_DAY,
                            SFF_PROMOTION_CODE = s.SFF_PROMOTION_CODE.ToSafeString()
                        });

                        model = packages.ToList();
                    }
                }
                #endregion
            }
            TimeSpan ts = stopWatch.Elapsed;
            string SBNServiceListPackageElapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
            logger.Info("SBNWebServiceService.listPackageByService elapsed time is " + SBNServiceListPackageElapsedTime);

            return model;

        }

        /// <summary>
        /// ListPackageService for change package
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="query"></param>
        /// <param name="_lov"></param>
        /// <returns></returns>
        public static List<PackageModel> GetPackageByChange(ILogger logger, IGetPackageListQuery query, IEntityRepository<FBB_CFG_LOV> _lov)
        {
            var model = new List<PackageModel>();
            try
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                ServicePointManager.ServerCertificateValidationCallback =
                    (s, certificate, chain, sslPolicyErrors) => true;

                using (var service = new SBNNewWebService.SBNWebServiceService())
                {
                    //var data = service.listPackageByChange(query.P_OWNER_PRODUCT, query.P_PACKAGE_FOR, query.P_Serenade_Flag, query.P_Ref_Row_Id);

                    //if (data.RETURN_CODE != 0
                    //         || null == data.packageByServiceArray)
                    //    logger.Info(data.RETURN_MESSAGE);
                    //else
                    //{
                    //    var packages = data.packageByServiceArray.Select(s => new PackageModel
                    //    {
                    //        MAPPING_CODE = s.MAPPING_CODE.ToSafeString(),
                    //        MAPPING_PRODUCT = s.MAPPING_PRODUCT.ToSafeString(),
                    //        PACKAGE_CODE = s.PACKAGE_CODE.ToSafeString(),
                    //        PACKAGE_NAME = s.PACKAGE_NAME.ToSafeString(),
                    //        PACKAGE_GROUP = s.PACKAGE_GROUP.ToSafeString(),
                    //        RECURRING_CHARGE = s.RECURRING_CHARGE.ToSafeInteger(),
                    //        PRE_RECURRING_CHARGE = s.PRE_RECURRING_CHARGE.ToSafeInteger(),
                    //        SFF_PROMOTION_BILL_THA = s.SFF_PROMOTION_BILL_THA.ToSafeString(),
                    //        SFF_PROMOTION_BILL_ENG = s.SFF_PROMOTION_BILL_ENG.ToSafeString(),
                    //        TECHNOLOGY = s.TECHNOLOGY.ToSafeString(),
                    //        DOWNLOAD_SPEED = s.DOWNLOAD_SPEED.ToSafeString(),
                    //        UPLOAD_SPEED = s.UPLOAD_SPEED.ToSafeString(),
                    //        INITIATION_CHARGE = s.INITIATION_CHARGE.ToSafeInteger(),
                    //        PRE_INITIATION_CHARGE = s.PRE_INITIATION_CHARGE.ToSafeInteger(),
                    //        PACKAGE_TYPE = s.PACKAGE_TYPE.ToSafeString(),
                    //        PRODUCT_SUBTYPE = s.PRODUCT_SUBTYPE.ToSafeString(),
                    //        OWNER_PRODUCT = s.OWNER_PRODUCT.ToSafeString(),

                    //        ACCESS_MODE = s.ACCESS_MODE.ToSafeString(),
                    //        SERVICE_CODE = s.SERVICE_CODE.ToSafeString(),

                    //        PRODUCT_SUBTYPE3 = s.PRODUCT_SUBTYPE3.ToSafeString(),

                    //        DISCOUNT_TYPE = s.DISCOUNT_TYPE.ToSafeString(),
                    //        DISCOUNT_VALUE = s.DISCOUNT_VALUE,
                    //        DISCOUNT_DAY = s.DISCOUNT_DAY,
                    //        SFF_PROMOTION_CODE = s.SFF_PROMOTION_CODE.ToSafeString()
                    //    });

                    //    model = packages.ToList();
                    //}
                }
                return model;
            }
            catch (Exception ex)
            {
                return model;
            }
        }

        /// <summary>
        /// ListPackageByService AirNet Workflow Service
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public static List<PackageModel> GetPackageListbySFFPromo(ILogger logger, GetPackageListBySFFPromoQuery query, IEntityRepository<FBB_CFG_LOV> lov)
        {
            var model = new List<PackageModel>();
            try
            {
                var sbnStatus = lov.Get(l => l.LOV_TYPE == ("FBB_CONSTANT") && l.LOV_NAME == ("SBN_SERVICE")).FirstOrDefault();

                if (sbnStatus.LOV_VAL1 == "NEW")
                {
                    #region newSBNService

                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                    ServicePointManager.ServerCertificateValidationCallback =
                        (s, certificate, chain, sslPolicyErrors) => true;

                    using (var service = new SBNNewWebService.SBNWebServiceService())
                    {
                        service.Timeout = 600000;
                        //  var data = service.listPackageBySFFPromo("P15042960", "VAS", "WireBB");
                        var data = service.listPackageBySFFPromo(query.P_SFF_PROMOCODE, query.P_PRODUCT_SUBTYPE, query.P_OWNER_PRODUCT, query.VAS_SERVICE);

                        if (data.RETURN_CODE != 0
                            || null == data.packageByServiceArray)
                            logger.Info(data.RETURN_MESSAGE);
                        else
                        {
                            var packages = data.packageByServiceArray.Select(s => new PackageModel
                            {
                                MAPPING_CODE = s.MAPPING_CODE.ToSafeString(),
                                MAPPING_PRODUCT = s.MAPPING_PRODUCT.ToSafeString(),
                                PACKAGE_CODE = s.PACKAGE_CODE.ToSafeString(),
                                PACKAGE_NAME = s.PACKAGE_NAME.ToSafeString(),
                                PACKAGE_GROUP = s.PACKAGE_GROUP.ToSafeString(),
                                RECURRING_CHARGE = s.RECURRING_CHARGE.ToSafeInteger(),
                                PRE_RECURRING_CHARGE = s.PRE_RECURRING_CHARGE.ToSafeInteger(),
                                SFF_PROMOTION_BILL_THA = s.SFF_PROMOTION_BILL_THA.ToSafeString(),
                                SFF_PROMOTION_BILL_ENG = s.SFF_PROMOTION_BILL_ENG.ToSafeString(),
                                TECHNOLOGY = s.TECHNOLOGY.ToSafeString(),
                                DOWNLOAD_SPEED = s.DOWNLOAD_SPEED.ToSafeString(),
                                UPLOAD_SPEED = s.UPLOAD_SPEED.ToSafeString(),
                                INITIATION_CHARGE = s.INITIATION_CHARGE.ToSafeInteger(),
                                PRE_INITIATION_CHARGE = s.PRE_INITIATION_CHARGE.ToSafeInteger(),
                                PACKAGE_TYPE = s.PACKAGE_TYPE.ToSafeString(),
                                PRODUCT_SUBTYPE = s.PRODUCT_SUBTYPE.ToSafeString(),
                                OWNER_PRODUCT = s.OWNER_PRODUCT.ToSafeString(),

                                ACCESS_MODE = s.ACCESS_MODE.ToSafeString(),
                                SERVICE_CODE = s.SERVICE_CODE.ToSafeString(),

                                PRODUCT_SUBTYPE3 = s.PRODUCT_SUBTYPE3.ToSafeString()
                            });

                            model = packages.ToList();
                        }
                    }
                    #endregion

                }
                else
                {
                    #region oldSBNService

                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                    ServicePointManager.ServerCertificateValidationCallback =
                        (s, certificate, chain, sslPolicyErrors) => true;

                    using (var service = new SBNWebService.SBNWebServiceService())
                    {
                        service.Timeout = 600000;
                        //  var data = service.listPackageBySFFPromo("P15042960", "VAS", "WireBB");
                        var data = service.listPackageBySFFPromo(query.P_SFF_PROMOCODE, query.P_PRODUCT_SUBTYPE, query.P_OWNER_PRODUCT);

                        if (data.RETURN_CODE.GetValueOrDefault() != 0
                            || null == data.PackageByServiceArray)
                            logger.Info(data.RETURN_MESSAGE);
                        else
                        {
                            var packages = data.PackageByServiceArray.Select(s => new PackageModel
                            {
                                MAPPING_CODE = s.MAPPING_CODE.ToSafeString(),
                                MAPPING_PRODUCT = s.MAPPING_PRODUCT.ToSafeString(),
                                PACKAGE_CODE = s.PACKAGE_CODE.ToSafeString(),
                                PACKAGE_NAME = s.PACKAGE_NAME.ToSafeString(),
                                PACKAGE_GROUP = s.PACKAGE_GROUP.ToSafeString(),
                                RECURRING_CHARGE = s.RECURRING_CHARGE.ToSafeInteger(),
                                PRE_RECURRING_CHARGE = s.PRE_RECURRING_CHARGE.ToSafeInteger(),
                                SFF_PROMOTION_BILL_THA = s.SFF_PROMOTION_BILL_THA.ToSafeString(),
                                SFF_PROMOTION_BILL_ENG = s.SFF_PROMOTION_BILL_ENG.ToSafeString(),
                                TECHNOLOGY = s.TECHNOLOGY.ToSafeString(),
                                DOWNLOAD_SPEED = s.DOWNLOAD_SPEED.ToSafeString(),
                                UPLOAD_SPEED = s.UPLOAD_SPEED.ToSafeString(),
                                INITIATION_CHARGE = s.INITIATION_CHARGE.ToSafeInteger(),
                                PRE_INITIATION_CHARGE = s.PRE_INITIATION_CHARGE.ToSafeInteger(),
                                PACKAGE_TYPE = s.PACKAGE_TYPE.ToSafeString(),
                                PRODUCT_SUBTYPE = s.PRODUCT_SUBTYPE.ToSafeString(),
                                OWNER_PRODUCT = s.OWNER_PRODUCT.ToSafeString(),

                                ACCESS_MODE = s.ACCESS_MODE.ToSafeString(),
                                SERVICE_CODE = s.SERVICE_CODE.ToSafeString(),

                                PRODUCT_SUBTYPE3 = s.PRODUCT_SUBTYPE3.ToSafeString()
                            });

                            model = packages.ToList();
                        }
                    }
                    #endregion

                }
            }
            catch (Exception ex)
            {

            }

            return model;
        }

        /// <summary>
        /// ListPackageByService AirNet Workflow Service
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public static List<PackageModel> GetPackageListbySFFPromoV2(ILogger logger, GetPackageListBySFFPromoV2Query query, IEntityRepository<FBB_CFG_LOV> lov)
        {
            var model = new List<PackageModel>();
            try
            {
                #region newSBNService

                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                ServicePointManager.ServerCertificateValidationCallback =
                    (s, certificate, chain, sslPolicyErrors) => true;

                using (var service = new SBNV2WebService.AIRInterfaceWorkflowServiceService())
                {
                    service.Timeout = 600000;

                    string tmpUrl = (from r in lov.Get()
                                     where r.LOV_NAME == "SaveOrderNewURL" && r.ACTIVEFLAG == "Y"
                                     select r.LOV_VAL1).FirstOrDefault().ToSafeString();
                    if (tmpUrl != "")
                    {
                        service.Url = tmpUrl;
                    }

                    var data = service.getListPackageBySFFPromo(query.P_SFF_PROMOCODE, query.P_PRODUCT_SUBTYPE, query.P_OWNER_PRODUCT, query.P_EXISTING_REQ);

                    if (data.RETURN_CODE != 0
                        || null == data.listPackageBySffPromoResult)
                        logger.Info(data.RETURN_MESSAGE);
                    else
                    {
                        var packages = data.listPackageBySffPromoResult.Select(s => new PackageModel
                        {
                            MAPPING_CODE = s.MAPPING_CODE.ToSafeString(),
                            OWNER_PRODUCT = s.OWNER_PRODUCT.ToSafeString(),
                            PACKAGE_DISPLAY_ENG = s.PACKAGE_DISPLAY_ENG.ToSafeString(),
                            PACKAGE_DISPLAY_THA = s.PACKAGE_DISPLAY_THA.ToSafeString(),
                            PACKAGE_SERVICE_CODE = s.PACKAGE_SERVICE_CODE.ToSafeString(),
                            PACKAGE_SERVICE_NAME = s.PACKAGE_SERVICE_NAME.ToSafeString(),
                            PACKAGE_TYPE = s.PACKAGE_TYPE.ToSafeString(),
                            PACKAGE_TYPE_DESC = s.PACKAGE_TYPE_DESC.ToSafeString(),
                            PRE_PRICE_CHARGE = s.PRE_PRICE_CHARGE.ToSafeDecimal(),
                            PRICE_CHARGE = s.PRICE_CHARGE.ToSafeDecimal(),
                            PRODUCT_SUBTYPE = s.PRODUCT_SUBTYPE.ToSafeString(),
                            PRODUCT_SUBTYPE3 = s.PRODUCT_SUBTYPE3.ToSafeString(),
                            SFF_PRODUCT_NAME = s.SFF_PRODUCT_NAME.ToSafeString(),
                            SFF_PRODUCT_PACKAGE = s.SFF_PRODUCT_PACKAGE.ToSafeString(),
                            SFF_PROMOTION_CODE = s.SFF_PROMOTION_CODE.ToSafeString(),
                            SFF_WORD_IN_STATEMENT_ENG = s.SFF_WORD_IN_STATEMENT_ENG.ToSafeString(),
                            SFF_WORD_IN_STATEMENT_THA = s.SFF_WORD_IN_STATEMENT_THA.ToSafeString(),
                            SERVICE_CODE = s.SERVICE_CODE.ToSafeString(),
                            DOWNLOAD_SPEED = s.DOWNLOAD_SPEED.ToSafeString(),
                            UPLOAD_SPEED = s.UPLOAD_SPEED.ToSafeString()
                        });

                        model = packages.ToList();
                    }
                }
                #endregion

            }
            catch (Exception ex)
            {

            }

            return model;
        }
        public static List<PackageModel> GetPackageListbySFFPromoOnline(IWBBUnitOfWork _uow, IEntityRepository<FBB_INTERFACE_LOG> _intfLog, ILogger logger, GetPackageListBySFFPromoV2Query query, IEntityRepository<FBB_CFG_LOV> lov, OnlineQueryConfigModel config)
        {
            InterfaceLogCommand log = null;
            var model = new List<PackageModel>();
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

                var onlineQueryConfigBody = new GetPackageListBySFFPromoOnlineQuery()
                {
                    SFF_PROMOCODE = query.P_SFF_PROMOCODE,
                    PRODUCT_SUBTYPE = query.P_PRODUCT_SUBTYPE,
                    OWNER_PRODUCT = query.P_OWNER_PRODUCT,
                    EXISTING_REQ = query.P_EXISTING_REQ,
                    INTERNET_NO = query.P_INTERNET_NO,
                    SALE_CHANNEL = query.P_SALE_CHANNEL
                };

                var result = new GetPackageListbySFFPromoOnlineRestult();

                string BodyStr = JsonConvert.SerializeObject(onlineQueryConfigBody);

                config.BodyStr = BodyStr;

                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, config, query.TransactionID,
                        "GetPackageListbySFFPromoOnline", "GetPackageListbySFFPromoOnline", "", "FBB|" + query.FullUrl, "");

                var client = new RestClient(config.Url);
                var request = new RestRequest();
                request.Method = Method.POST;
                request.AddHeader("Content-Type", config.ContentType);
                request.AddHeader("x-online-query-transaction-id", XOnlineQueryTransactionD);
                request.AddHeader("x-online-query-channel", config.Channel);
                request.AddParameter("application/json", BodyStr, ParameterType.RequestBody);

                // execute the request

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                //config.UseSecurityProtocol = "Y";
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
                    result = JsonConvert.DeserializeObject<GetPackageListbySFFPromoOnlineRestult>(responseData.Content) ?? new GetPackageListbySFFPromoOnlineRestult();
                    if (result != null)
                    {
                        if (result.LIST_PACKAGE_BY_SFFPROMO != null && result.LIST_PACKAGE_BY_SFFPROMO.Count > 0)
                        {
                            model = result.LIST_PACKAGE_BY_SFFPROMO.Select(s => new PackageModel
                            {

                                MAPPING_CODE = s.mapping_code.ToSafeString(),
                                OWNER_PRODUCT = s.owner_product.ToSafeString(),
                                PACKAGE_DISPLAY_ENG = s.package_display_eng.ToSafeString(),
                                PACKAGE_DISPLAY_THA = s.package_display_tha.ToSafeString(),
                                PACKAGE_SERVICE_CODE = s.package_service_code.ToSafeString(),
                                PACKAGE_SERVICE_NAME = s.package_service_name.ToSafeString(),
                                PACKAGE_TYPE = s.package_type.ToSafeString(),
                                PACKAGE_TYPE_DESC = s.package_type_desc.ToSafeString(),
                                PRE_PRICE_CHARGE = s.pre_price_charge.ToSafeDecimal(),
                                PRICE_CHARGE = s.price_charge.ToSafeDecimal(),
                                PRODUCT_SUBTYPE = s.product_subtype.ToSafeString(),
                                PRODUCT_SUBTYPE3 = s.product_subtype3.ToSafeString(),
                                SFF_PRODUCT_NAME = s.sff_product_name.ToSafeString(),
                                SFF_PRODUCT_PACKAGE = s.sff_product_package.ToSafeString(),
                                SFF_PROMOTION_CODE = s.sff_promotion_code.ToSafeString(),
                                SFF_WORD_IN_STATEMENT_ENG = s.sff_word_in_statement_eng.ToSafeString(),
                                SFF_WORD_IN_STATEMENT_THA = s.sff_word_in_statement_tha.ToSafeString(),
                                SERVICE_CODE = s.service_code.ToSafeString(),
                                DOWNLOAD_SPEED = s.download_speed.ToSafeString(),
                                UPLOAD_SPEED = s.upload_speed.ToSafeString(),
                                AUTO_MAPPING_PROMOTION_CODE = s.auto_mapping_promotion_code.ToSafeString(),
                                PACKAGE_FOR_SALE_FLAG = s.package_for_sale_flag.ToSafeString()

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
            catch (Exception ex)
            {

            }

            return model;
        }

        /// <summary>
        /// ListPackage AirNet Workflow Service
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public static List<PackageModel> GetPackageListAll(ILogger logger, IGetPackageListQuery query, IEntityRepository<FBB_CFG_LOV> lov)
        {

            string tmpUrl = (from r in lov.Get()
                             where r.LOV_NAME == "SaveOrderNewURL" && r.ACTIVEFLAG == "Y"
                             select r.LOV_VAL1).FirstOrDefault().ToSafeString();

            var model = new List<PackageModel>();

            var sbnStatus = lov.Get(l => l.LOV_TYPE == ("FBB_CONSTANT") && l.LOV_NAME == ("SBN_SERVICE")).FirstOrDefault();

            if (sbnStatus.LOV_VAL1 == "NEW")
            {
                #region newSBNService

                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                ServicePointManager.ServerCertificateValidationCallback =
                    (s, certificate, chain, sslPolicyErrors) => true;

                using (var service = new SBNNewWebService.SBNWebServiceService())
                {
                    service.Timeout = 600000;
                    var data = service.listPackage();

                    if (data.RETURN_CODE != 0
                        || null == data.packageArray)
                        logger.Info(data.RETURN_MESSAGE);
                    else
                    {
                        var packages = data.packageArray.Select(s => new PackageModel
                        {
                            PACKAGE_CODE = s.PACKAGE_CODE.ToSafeString(),
                            PACKAGE_NAME = s.PACKAGE_NAME.ToSafeString(),
                        });

                        model = packages.OrderBy(o => o.PACKAGE_GROUP).ToList();
                    }
                }
                #endregion
            }
            else
            {
                #region oldSBNService
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                ServicePointManager.ServerCertificateValidationCallback =
                    (s, certificate, chain, sslPolicyErrors) => true;

                using (var service = new SBNWebService.SBNWebServiceService())
                {
                    service.Timeout = 600000;
                    var data = service.listPackage();

                    if (data.RETURN_CODE.GetValueOrDefault() != 0
                        || null == data.PackageArray)
                        logger.Info(data.RETURN_MESSAGE);
                    else
                    {
                        var packages = data.PackageArray.Select(s => new PackageModel
                        {
                            PACKAGE_CODE = s.PACKAGE_CODE.ToSafeString(),
                            PACKAGE_NAME = s.PACKAGE_NAME.ToSafeString(),
                        });

                        model = packages.OrderBy(o => o.PACKAGE_GROUP).ToList();
                    }
                }
                #endregion
            }

            return model;
        }

        /// <summary>
        /// ListPackage V2 AirNet Workflow Service
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public static List<PackageV2Model> GetPackageListV2(ILogger logger, GetListPackageByServiceV2Query query, string tmpUrl)
        {
            var model = new List<PackageV2Model>();


            System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
            stopWatch.Start();

            #region newSBNServiceV2

            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            ServicePointManager.ServerCertificateValidationCallback =
                (s, certificate, chain, sslPolicyErrors) => true;

            using (var service = new SBNV2WebService.AIRInterfaceWorkflowServiceService())
            {
                var userReferencetransaction = query.SessionId;
                service.Credentials = new NetworkCredential(userReferencetransaction, "");

                service.Timeout = 600000;

                if (tmpUrl != "")
                {
                    service.Url = tmpUrl;
                }

                var data = service.getListPackageByService(query.P_SALE_CHANNEL, query.P_OWNER_PRODUCT, query.P_PACKAGE_FOR, query.P_SFF_PROMOTION_CODE, query.P_Customer_Type,
                    query.P_Customer_subtype, query.P_Partner_Type, query.P_Partner_SubType, query.P_Location_Code, query.P_ASC_CODE, query.P_EMPLOYEE_ID,
                    query.P_Region, query.P_Province, query.P_District, query.P_Sub_District, query.P_Address_Id, query.P_Serenade_Flag, query.P_FMPA_Flag, query.P_CVM_FLAG, query.P_FMC_SPECIAL_FLAG, query.P_NON_RES_FLAG);

                if (data.RETURN_CODE != 0
                    || null == data.packageByServiceArray)
                    logger.Info(data.RETURN_MESSAGE);
                else
                {
                    var packages = data.packageByServiceArray.Select(s => new PackageV2Model
                    {
                        ACCESS_MODE = s.ACCESS_MODE.ToSafeString(),
                        AUTO_MAPPING_PROMOTION_CODE = s.AUTO_MAPPING_PROMOTION_CODE.ToSafeString(),
                        DISCOUNT_DAY = s.DISCOUNT_DAY.ToSafeString(),
                        DISCOUNT_TYPE = s.DISCOUNT_TYPE.ToSafeString(),
                        DISCOUNT_VALUE = s.DISCOUNT_VALUE.ToSafeString(),
                        DISPLAY_FLAG = s.DISPLAY_FLAG.ToSafeString(),
                        DISPLAY_SEQ = s.DISPLAY_SEQ.ToSafeString(),
                        DOWNLOAD_SPEED = s.DOWNLOAD_SPEED.ToSafeString(),
                        MAPPING_CODE = s.MAPPING_CODE.ToSafeString(),
                        OWNER_PRODUCT = s.OWNER_PRODUCT.ToSafeString(),
                        PACKAGE_DISPLAY_ENG = s.PACKAGE_DISPLAY_ENG.ToSafeString(),
                        PACKAGE_DISPLAY_THA = s.PACKAGE_DISPLAY_THA.ToSafeString(),
                        PACKAGE_FOR_SALE_FLAG = s.PACKAGE_FOR_SALE_FLAG.ToSafeString(),
                        PACKAGE_GROUP = s.PACKAGE_GROUP.ToSafeString(),
                        PACKAGE_GROUP_DESC_ENG = s.PACKAGE_GROUP_DESC_ENG.ToSafeString(),
                        PACKAGE_GROUP_DESC_THA = s.PACKAGE_GROUP_DESC_THA.ToSafeString(),
                        PACKAGE_GROUP_SEQ = s.PACKAGE_GROUP_SEQ.ToSafeString(),
                        PACKAGE_REMARK_ENG = s.PACKAGE_REMARK_ENG.ToSafeString(),
                        PACKAGE_REMARK_THA = s.PACKAGE_REMARK_THA.ToSafeString(),
                        PACKAGE_SERVICE_CODE = s.PACKAGE_SERVICE_CODE.ToSafeString(),
                        PACKAGE_TYPE = s.PACKAGE_TYPE.ToSafeString(),
                        PACKAGE_TYPE_DESC = s.PACKAGE_TYPE_DESC.ToSafeString(),
                        PRE_PRICE_CHARGE = s.PRE_PRICE_CHARGE.ToSafeString(),
                        PRICE_CHARGE = s.PRICE_CHARGE.ToSafeString(),
                        PRODUCT_SUBTYPE = s.PRODUCT_SUBTYPE.ToSafeString(),
                        PRODUCT_SUBTYPE3 = s.PRODUCT_SUBTYPE3.ToSafeString(),
                        SERVICE_CODE = s.SERVICE_CODE.ToSafeString(),
                        SFF_PRODUCT_CLASS = s.SFF_PRODUCT_CLASS.ToSafeString(),
                        SFF_PRODUCT_NAME = s.SFF_PRODUCT_NAME.ToSafeString(),
                        SFF_PROMOTION_CODE = s.SFF_PROMOTION_CODE.ToSafeString(),
                        SFF_WORD_IN_STATEMENT_ENG = s.SFF_WORD_IN_STATEMENT_ENG.ToSafeString(),
                        SFF_WORD_IN_STATEMENT_THA = s.SFF_WORD_IN_STATEMENT_THA.ToSafeString(),
                        SUB_SEQ = s.SUB_SEQ.ToSafeString(),
                        UPLOAD_SPEED = s.UPLOAD_SPEED.ToSafeString(),
                        CUSTOMER_TYPE = s.CUSTOMER_TYPE.ToSafeString(),
                        MOBILE_PRICE = query.P_MOBILE_PRICE.ToSafeString(),
                        EXISTING_MOBILE = query.P_EXISTING_MOBILE.ToSafeString()
                    });



                    model = packages.ToList();
                }
            }
            #endregion

            TimeSpan ts = stopWatch.Elapsed;
            string SBNServiceListPackageElapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
            logger.Info("SBNWebServiceService.listPackageByService elapsed time is " + SBNServiceListPackageElapsedTime);

            return model;

        }

        public static List<PackageV2Model> GetPackageListOnlineQuery(IWBBUnitOfWork _uow, IEntityRepository<FBB_INTERFACE_LOG> _intfLog, ILogger logger, GetListPackageByServiceV2Query query, OnlineQueryConfigModel config)
        {
            InterfaceLogCommand log = null;
            List<PackageV2Model> model = new List<PackageV2Model>();
            System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
            stopWatch.Start();
            string XOnlineQueryTransactionD = "";
            string TimeStamp = DateTime.Now.ToString("yyyyMMddHHmmssFFF");
            int _min = 0000;
            int _max = 9999;
            Random _rdm = new Random();
            var Nonce = _rdm.Next(_min, _max).ToString();
            XOnlineQueryTransactionD = TimeStamp + Nonce;

            ProductSubtypeArray[] PRODUCT_SUBTYPE_ARRAY = new ProductSubtypeArray[1];
            PRODUCT_SUBTYPE_ARRAY[0] = new ProductSubtypeArray()
            {
                OWNER_PRODUCT = query.P_OWNER_PRODUCT.ToSafeString(),
                PRODUCT_SUBTYPE = query.P_PRODUCT_SUBTYPE.ToSafeString()
            };

            string[] SFF_PROMOTION_ARRAY = new string[1];
            SFF_PROMOTION_ARRAY[0] = query.P_SFF_PROMOTION_CODE.ToSafeString();

            string[] ADDRESS_ID_ARRAY = new string[1];
            ADDRESS_ID_ARRAY[0] = query.P_Address_Id.ToSafeString();

            Project_Cond_Flag_Array[] PROJECT_COND_FLAG_ARRAY = new Project_Cond_Flag_Array[6];
            PROJECT_COND_FLAG_ARRAY[0] = new Project_Cond_Flag_Array()
            {
                Flag = "SERENADE_FLAG",
                Value = query.P_Serenade_Flag.ToSafeString()
            };
            PROJECT_COND_FLAG_ARRAY[1] = new Project_Cond_Flag_Array()
            {
                Flag = "FMPA_FLAG",
                Value = query.P_FMPA_Flag.ToSafeString()
            };
            PROJECT_COND_FLAG_ARRAY[2] = new Project_Cond_Flag_Array()
            {
                Flag = "CVM_FLAG",
                Value = query.P_CVM_FLAG.ToSafeString()
            };
            PROJECT_COND_FLAG_ARRAY[3] = new Project_Cond_Flag_Array()
            {
                Flag = "FMC_SPECIAL_FLAG",
                Value = query.P_FMC_SPECIAL_FLAG.ToSafeString()
            };
            PROJECT_COND_FLAG_ARRAY[4] = new Project_Cond_Flag_Array()
            {
                Flag = "NON_RES_FLAG",
                Value = query.P_NON_RES_FLAG.ToSafeString()
            };
            PROJECT_COND_FLAG_ARRAY[5] = new Project_Cond_Flag_Array()
            {
                Flag = "MOU_FLAG",
                Value = query.P_MOU_FLAG.ToSafeString()
            };//R21.10 MOU

            OnlineQueryConfigBody onlineQueryConfigBody = new OnlineQueryConfigBody()
            {
                SALE_CHANNEL = query.P_SALE_CHANNEL.ToSafeString(),
                PRODUCT_SUBTYPE_ARRAY = PRODUCT_SUBTYPE_ARRAY,
                PACKAGE_FOR = query.P_PACKAGE_FOR.ToSafeString(),
                SFF_PROMOTION_ARRAY = SFF_PROMOTION_ARRAY,
                CUSTOMER_TYPE = query.P_Customer_Type.ToSafeString(),
                CUSTOMER_SUBTYPE = query.P_Customer_subtype.ToSafeString(),
                PARTNER_TYPE = query.P_Partner_Type.ToSafeString(),
                PARTNER_SUBTYPE = query.P_Partner_SubType.ToSafeString(),
                DISTRIBUTION_CHANNEL = query.P_DistChn.ToSafeString(),
                CHANNEL_SALES_GROUP = query.P_ChnSales.ToSafeString(),
                SHOP_SEGMENT = query.P_OperatorClass.ToSafeString(),
                LOCATION_CODE = query.P_Location_Code.ToSafeString(),
                ASC_CODE = query.P_ASC_CODE.ToSafeString(),
                EMPLOYEE_ID = query.P_EMPLOYEE_ID.ToSafeString(),
                REGION = query.P_Region.ToSafeString(),
                PROVINCE = query.P_Province.ToSafeString(),
                DISTRICT = query.P_District.ToSafeString(),
                SUB_DISTRICT = query.P_Sub_District.ToSafeString(),
                ADDRESS_ID_ARRAY = ADDRESS_ID_ARRAY,
                PROJECT_COND_FLAG_ARRAY = PROJECT_COND_FLAG_ARRAY,
                LOCATION_PROVINCE = query.P_LocationProvince.ToSafeString()
            };


            OnlineQueryConfigResult result = new OnlineQueryConfigResult();

            string BodyStr = JsonConvert.SerializeObject(onlineQueryConfigBody);

            config.BodyStr = BodyStr;

            log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, config, query.TransactionID,
                    "GetPackageListOnlineQuery", "GetPackageListOnlineQuery", "", "FBB|" + query.FullUrl, "");
            try
            {
                var client = new RestClient(config.Url);
                var request = new RestRequest();
                request.Method = Method.POST;
                request.AddHeader("Content-Type", config.ContentType);
                request.AddHeader("x-online-query-transaction-id", XOnlineQueryTransactionD);
                request.AddHeader("x-online-query-channel", config.Channel);
                request.AddParameter("application/json", BodyStr, ParameterType.RequestBody);

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                //config.UseSecurityProtocol = "Y";
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
                    result = JsonConvert.DeserializeObject<OnlineQueryConfigResult>(responseData.Content) ?? new OnlineQueryConfigResult();
                    if (result != null)
                    {
                        if (result.LIST_PACKAGE_BY_SERVICE != null && result.LIST_PACKAGE_BY_SERVICE.Count > 0)
                        {
                            model = result.LIST_PACKAGE_BY_SERVICE.Select(t => new PackageV2Model
                            {
                                ACCESS_MODE = t.access_mode.ToSafeString(),
                                AUTO_MAPPING_PROMOTION_CODE = t.auto_mapping_promotion_code.ToSafeString(),
                                DISCOUNT_DAY = t.discount_day.ToSafeString(),
                                DISCOUNT_TYPE = t.discount_type.ToSafeString(),
                                DISCOUNT_VALUE = t.discount_value.ToSafeString(),
                                DISPLAY_FLAG = t.display_flag.ToSafeString(),
                                DISPLAY_SEQ = t.display_seq.ToSafeString(),
                                DOWNLOAD_SPEED = t.download_speed.ToSafeString(),
                                MAPPING_CODE = t.mapping_code.ToSafeString(),
                                OWNER_PRODUCT = t.owner_product.ToSafeString(),
                                PACKAGE_DISPLAY_ENG = t.package_display_eng.ToSafeString(),
                                PACKAGE_DISPLAY_THA = t.package_display_tha.ToSafeString(),
                                PACKAGE_FOR_SALE_FLAG = t.package_for_sale_flag.ToSafeString(),
                                PACKAGE_GROUP = t.package_group.ToSafeString(),
                                PACKAGE_GROUP_DESC_ENG = t.package_group_desc_eng.ToSafeString(),
                                PACKAGE_GROUP_DESC_THA = t.package_group_desc_tha.ToSafeString(),
                                PACKAGE_GROUP_SEQ = t.package_group_seq.ToSafeString(),
                                PACKAGE_REMARK_ENG = t.package_remark_eng.ToSafeString(),
                                PACKAGE_REMARK_THA = t.package_remark_tha.ToSafeString(),
                                PACKAGE_SERVICE_CODE = t.package_service_code.ToSafeString(),
                                PACKAGE_TYPE = t.package_type.ToSafeString(),
                                PACKAGE_TYPE_DESC = t.package_type_desc.ToSafeString(),
                                PRE_PRICE_CHARGE = t.pre_price_charge.ToSafeString(),
                                PRICE_CHARGE = t.price_charge.ToSafeString(),
                                PRODUCT_SUBTYPE = t.product_subtype.ToSafeString(),
                                PRODUCT_SUBTYPE3 = t.product_subtype3.ToSafeString(),
                                SERVICE_CODE = t.service_code.ToSafeString(),
                                SFF_PRODUCT_CLASS = t.sff_product_class.ToSafeString(),
                                SFF_PRODUCT_NAME = t.sff_product_name.ToSafeString(),
                                SFF_PROMOTION_CODE = t.sff_promotion_code.ToSafeString(),
                                SFF_WORD_IN_STATEMENT_ENG = t.sff_word_in_statement_eng.ToSafeString(),
                                SFF_WORD_IN_STATEMENT_THA = t.sff_word_in_statement_tha.ToSafeString(),
                                SUB_SEQ = t.sub_seq.ToSafeString(),
                                UPLOAD_SPEED = t.upload_speed.ToSafeString(),
                                MOBILE_PRICE = query.P_MOBILE_PRICE.ToSafeString(),
                                EXISTING_MOBILE = query.P_EXISTING_MOBILE.ToSafeString(),
                                CUSTOMER_TYPE = t.customer_type.ToSafeString(),
                                INSTALL_SHOW = "",
                                ENTRYFEE_SHOW = "",
                                PACKAGE_DURATION = t.package_duration.ToSafeString(),
                                PRICE_CHARGE_VAT = t.price_charge_vat.ToSafeString()
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
            catch (Exception ex)
            {
                result.RESULT_CODE = "1";
                result.RESULT_DESC = ex.Message;
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, "Failed", ex.Message, "");
            }

            TimeSpan ts = stopWatch.Elapsed;
            string SBNServiceListPackageElapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
            logger.Info("GetPackageListOnlineQuery elapsed time is " + SBNServiceListPackageElapsedTime);

            return model;
        }

        /// <summary>
        /// List of Special offer according to internet technology and queried offers
        /// </summary>
        /// <param name="lov"></param>
        /// <param name="acqOffers">offers ที่ลูกค้ามีสิทธิ์ได้รับ</param>
        /// <param name="technology"></param>
        /// <param name="lang"></param>
        /// <returns></returns>
        public static List<SpecialOfferModel> SpecialOfferList(IEntityRepository<FBB_CFG_LOV> lov,
        List<string> acqOffers, string packageGroup, string lang)
        {
            var isThai = lang.ToCultureCode().IsThaiCulture();

            var lovSpecialOffer = (from t in lov.Get()
                                   where t.LOV_TYPE == "SPECIAL_OFFER"
                                        && t.DISPLAY_VAL == packageGroup
                                        && t.ACTIVEFLAG == "Y"
                                        && (!acqOffers.Any() || acqOffers.Contains(t.LOV_NAME))
                                   select new SpecialOfferModel
                                   {
                                       Name = t.LOV_NAME,
                                       Description = isThai ? t.LOV_VAL1 : t.LOV_VAL2,
                                   }).ToList();

            //var specialOffer = string.Join(",", lovSpecialOffer);

            return lovSpecialOffer;
        }

        public static List<string> ValidateSpecialOffer(IEntityRepository<FBB_CFG_LOV> lov,
          IEntityRepository<FBB_SFF_CHKPROFILE_LOG> sffChkProfLog,
          IEntityRepository<FBB_VSMP_LOG> vsmpLog,
          GetCustomerSpeOfferQuery query)
        {
            var checker = new List<string>();

            var offerConfigList = (from t in lov.Get()
                                   where t.LOV_TYPE == "SPECIAL_OFFER"
                                    && t.DISPLAY_VAL == query.PackageGroup
                                    && t.ACTIVEFLAG == "Y"
                                   //&& t.DISPLAY_VAL == query.Technology
                                   select new
                                   {
                                       OfferName = t.LOV_NAME,
                                       Technology = t.DISPLAY_VAL,
                                   });

            if (!offerConfigList.Any())
                return checker;

            foreach (var offerConfig in offerConfigList)
            {
                if (offerConfig.OfferName == "WiFi Router")
                {
                    checker.Add(offerConfig.OfferName);
                }
                else if (offerConfig.OfferName == "DOUBLE_SPEED")
                {
                    if ((!string.IsNullOrEmpty(query.ReferenceID)
                                    && query.IsAWNProduct
                                    //&& (from s in sffChkProfLog.Get()
                                    //    where (s.SFF_CHKPROFILE_ID == query.SffChkProfLogID
                                    //            || s.TRANSACTION_ID == query.ReferenceID)
                                    //    select s).Any(s => s.OUTPRODUCTNAME == "3G")
                                    && (!query.IsNonMobile || (from v in vsmpLog.Get()
                                                               where v.ORDER_REF == query.ReferenceID
                                                               select v).Any(v => v.CHM == "0"))
                                        ))
                    {
                        checker.Add(offerConfig.OfferName);
                    }
                }
            }

            return checker;
        }

        public static string AirnetWfAccessModeMapper(ILogger logger,
            IEntityRepository<FBB_CFG_LOV> lov,
            GetMappingSbnOwnerProd query)
        {
            var sbnwfOwnerProd = "";
            var config = (from t in lov.Get()
                          where t.LOV_TYPE == "SCREEN"
                              && t.LOV_NAME == "MAPPING_OWNER_PRODUCT"
                          select t)
                          .AsEnumerable()
                          .Where(t => t.ACTIVEFLAG.IsActive());

            if (!config.Any())
            {
                logger.Info("MAPPING_OWNER_PRODUCT Not Found.");
                return sbnwfOwnerProd;
            }

            var ownerProductList = new HashSet<string>();
            //ownerProductList.Add("SWiFi");
            //ownerProductList.Add("SIMAT");

            foreach (var acm in query.FBSSAccessModeInfo)
            {
                var mapping = config.Where(t => t.LOV_VAL1 == acm.AccessMode.ToSafeString());

                if (!mapping.Any())
                    continue;

                //if (query.IsPartner.ToYesNoFlgBoolean())
                //{
                //    var airOwnerProduct = mapping
                //        .Where(t => t.LOV_VAL2 == query.PartnerName)
                //        .Select(t => t.LOV_VAL3)
                //        .FirstOrDefault();

                //    if (!string.IsNullOrEmpty(airOwnerProduct))
                //    {
                //        ownerProductList.Add(airOwnerProduct);
                //    }
                //}
                //else
                //{
                //if (!acm.AccessMode.IsFTTH())
                //{
                var nonPartnerProduct = mapping
                    .Where(t => t.LOV_VAL2 == query.PartnerName)
                    .Select(t => t.LOV_VAL3)
                    .FirstOrDefault();

                logger.Info("nonPartnerProduct is " + nonPartnerProduct);

                if (!string.IsNullOrEmpty(nonPartnerProduct))
                {
                    ownerProductList.Add(nonPartnerProduct);
                }
                else
                {
                    ownerProductList.Add(mapping.Where(w => w.DEFAULT_VALUE == "Y").Select(s => s.LOV_VAL3).FirstOrDefault());
                }

                //    }
                //}

                //ownerProductList.Add(mapping.Select(t => t.LOV_VAL3).FirstOrDefault());
                //ownerProductList.Add(acm.AccessMode.ToSafeString());
            }

            sbnwfOwnerProd = string.Join("|", ownerProductList);
            logger.Info("OWER_PRODUCT is " + sbnwfOwnerProd);

            return sbnwfOwnerProd;
        }

        public static string ReverseAirnetWfAccessModeMapper(ILogger logger,
            IEntityRepository<FBB_CFG_LOV> lov,
            string airOwnerStrn,
            bool isPartner,
            string partnerName)
        {
            var config = (from t in lov.Get()
                          where t.LOV_TYPE == "SCREEN"
                              && t.LOV_NAME == "MAPPING_OWNER_PRODUCT"
                          select t)
                          .AsEnumerable()
                          .Where(t => t.ACTIVEFLAG.IsActive());

            if (!config.Any())
            {
                logger.Info("MAPPING_OWNER_PRODUCT Not Found.");
                return "";
            }

            var accessMode = "";

            var mapping = config.Where(t => t.LOV_VAL3 == airOwnerStrn.ToSafeString());

            var airOwnerProduct = mapping
                .Where(t => t.LOV_VAL2 == partnerName)
                .Select(t => t.LOV_VAL1)
                .FirstOrDefault();

            if (!string.IsNullOrEmpty(airOwnerProduct))
            {
                accessMode = airOwnerProduct;
                logger.Info("ACCESS_MODE is " + accessMode);
                return accessMode;
            }

            accessMode = mapping.Select(t => t.LOV_VAL1).FirstOrDefault();
            logger.Info("ACCESS_MODE is " + accessMode);
            return accessMode;
        }

        /// <summary>
        /// ListPackageByService AirNet Workflow Service
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public static List<PackageModel> GetPackageOntopListbySFFPromo(ILogger logger, GetPackageOntopListBySFFPromoQuery query, IEntityRepository<FBB_CFG_LOV> lov)
        {
            var model = new List<PackageModel>();
            try
            {
                var sbnStatus = lov.Get(l => l.LOV_TYPE == ("FBB_CONSTANT") && l.LOV_NAME == ("SBN_SERVICE")).FirstOrDefault();

                #region newSBNService
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                ServicePointManager.ServerCertificateValidationCallback =
                    (s, certificate, chain, sslPolicyErrors) => true;
                using (var service = new SBNNewWebService.SBNWebServiceService())
                {
                    service.Timeout = 600000;
                    var data = service.ListPackageOntopByChange(query.P_OWNER_PRODUCT, query.P_PRODUCT_SUBTYPE, "PUBLIC", query.P_SFF_PROMOCODE);

                    if (data.RETURN_CODE != 0
                        || null == data.listPackageOntopByChange)
                        logger.Info(data.RETURN_MESSAGE);
                    else
                    {
                        var packages = data.listPackageOntopByChange.Select(s => new PackageModel
                        {
                            SFF_PROMOTION_CODE = s.sff_promotion_code.ToSafeString(),
                            MAPPING_CODE = s.mapping_code.ToSafeString(),
                            PACKAGE_CODE = s.package_code.ToSafeString(),
                            PACKAGE_GROUP = s.package_group.ToSafeString(),
                            SFF_PROMOTION_BILL_THA = s.sff_promotion_bill_tha.ToSafeString(),
                            SFF_PROMOTION_BILL_ENG = s.sff_promotion_bill_eng.ToSafeString(),
                            TECHNOLOGY = s.technology.ToSafeString(),
                            PACKAGE_TYPE = s.package_type.ToSafeString(),
                            PRODUCT_SUBTYPE = s.product_subtype.ToSafeString(),
                            OWNER_PRODUCT = s.owner_product.ToSafeString(),
                        });

                        model = packages.ToList();
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {

            }

            return model;
        }

        public static string GetProductSubtype(IEntityRepository<object> _obj, string OWNER_PRODUCT, string Address_Id)
        {
            string ProductSubtype = "";

            var P_OWNER_PRODUCT = new OracleParameter();
            P_OWNER_PRODUCT.ParameterName = "p_owner_product";
            P_OWNER_PRODUCT.Size = 2000;
            P_OWNER_PRODUCT.OracleDbType = OracleDbType.Varchar2;
            P_OWNER_PRODUCT.Direction = ParameterDirection.Input;
            P_OWNER_PRODUCT.Value = OWNER_PRODUCT.ToSafeString();

            var P_ADDRESS_ID = new OracleParameter();
            P_ADDRESS_ID.ParameterName = "p_address_id";
            P_ADDRESS_ID.Size = 2000;
            P_ADDRESS_ID.OracleDbType = OracleDbType.Varchar2;
            P_ADDRESS_ID.Direction = ParameterDirection.Input;
            P_ADDRESS_ID.Value = Address_Id.ToSafeString();

            var O_RETURN_CODE = new OracleParameter();
            O_RETURN_CODE.ParameterName = "o_return_code";
            O_RETURN_CODE.Size = 2000;
            O_RETURN_CODE.OracleDbType = OracleDbType.Varchar2;
            O_RETURN_CODE.Direction = ParameterDirection.Output;

            var IORESULTS = new OracleParameter();
            IORESULTS.ParameterName = "ioResults";
            IORESULTS.OracleDbType = OracleDbType.RefCursor;
            IORESULTS.Direction = ParameterDirection.Output;

            var resultExecute = _obj.ExecuteStoredProcMultipleCursor("PKG_FBBOR044.GET_PRODUCT_SUBTYPE",
                  new object[]
                  {
                                    //Parameter Input
                                    P_OWNER_PRODUCT,
                                    P_ADDRESS_ID,
                                    //Parameter Output
                                    O_RETURN_CODE,
                                    IORESULTS
                  });
            if (resultExecute != null)
            {
                DataTable dtRespones = (DataTable)resultExecute[1];
                List<ProductSubtypeCursor> Respones = dtRespones.DataTableToList<ProductSubtypeCursor>();
                if (Respones != null && Respones.Count > 0)
                {
                    ProductSubtype = Respones.FirstOrDefault().PRODUCT_SUBTYPE.ToSafeString();
                }
            }

            return ProductSubtype;
        }
    }

    public static class SffServiceConseHelper
    {
        public static InterfaceLogCommand StartInterfaceSffLog<T>(IWBBUnitOfWork uow,
            IEntityRepository<FBB_INTERFACE_LOG> interfaceLog,
            T query, string transactionId,
            string methodName, string serviceName, string idCardNo)
        {
            var dbIntfCmd = new InterfaceLogCommand
            {
                ActionType = WBBContract.Commands.ActionType.Insert,
                IN_TRANSACTION_ID = transactionId,
                METHOD_NAME = methodName,
                SERVICE_NAME = serviceName,
                IN_ID_CARD_NO = idCardNo,
                IN_XML_PARAM = query.DumpToXml(),
                INTERFACE_NODE = "FBB",
                CREATED_BY = "FbbCpGwInterface",
            };

            var log = InterfaceLogHelper.Log(interfaceLog, dbIntfCmd);
            uow.Persist();

            dbIntfCmd.OutInterfaceLogId = log.INTERFACE_ID;
            return dbIntfCmd;
        }

        public static void EndInterfaceSffLog<T>(IWBBUnitOfWork uow,
            IEntityRepository<FBB_INTERFACE_LOG> interfaceLog,
            T output, InterfaceLogCommand dbIntfCmd,
            string result, string reason)
        {
            dbIntfCmd.ActionType = WBBContract.Commands.ActionType.Update;
            dbIntfCmd.REQUEST_STATUS = (result == "Success") ? "Success" : "Error";
            dbIntfCmd.OUT_RESULT = result;
            dbIntfCmd.OUT_ERROR_RESULT = (result == "Success") ? (reason.Length > 100 ? reason.Substring(0, 100) : result) : result;
            dbIntfCmd.OUT_XML_PARAM = output.DumpToXml();

            InterfaceLogHelper.Log(interfaceLog, dbIntfCmd);
            uow.Persist();
        }

        public static void GetMassCommonAccountInfo(ILogger logger,
            IWBBUnitOfWork uow,
            IEntityRepository<FBB_CFG_LOV> lovService,
            IEntityRepository<FBB_SFF_CHKPROFILE_LOG> sffLog,
            IGetMassCommonAccount query,
            SFFServices.SffRequest request,
            evESeServiceQueryMassCommonAccountInfoModel model,
             IEntityRepository<FBB_CUST_PACKAGE> custPackage,
            IEntityRepository<FBB_CUST_PROFILE> custProfile,
            IEntityRepository<FBB_REGISTER> register,
            IEntityRepository<FBB_PACKAGE_TRAN> packageTran)
        {
            using (var service = new SFFServices.SFFServiceService())
            {
                var data = service.ExecuteService(request);

                if (data != null)
                {
                    logger.Info(data.ErrorMessage);
                    model.outErrorMessage = data.ErrorMessage;
                    var errSp = data.ErrorMessage.Trim().Split(':');
                    model.errorMessage = errSp[0];
                    foreach (var a in data.ParameterList.Parameter)
                    {
                        if (a.Name == "outPrimaryContactFirstName") model.outPrimaryContactFirstName = a.Value;
                        else if (a.Name == "outContactLastName") model.outContactLastName = a.Value;
                        else if (a.Name == "outAmphur") model.outAmphur = a.Value;
                        else if (a.Name == "outBuildingName") model.outBuildingName = a.Value;
                        else if (a.Name == "outFloor") model.outFloor = a.Value;
                        else if (a.Name == "outHouseNumber") model.outHouseNumber = a.Value;
                        else if (a.Name == "outMoo") model.outMoo = a.Value;
                        else if (a.Name == "outMooban") model.outMooban = a.Value;
                        else if (a.Name == "outProvince") model.outProvince = a.Value;
                        else if (a.Name == "outRoom") model.outRoom = a.Value;
                        else if (a.Name == "outSoi") model.outSoi = a.Value;
                        else if (a.Name == "outStreetName") model.outStreetName = a.Value;
                        else if (a.Name == "outBillLanguage") model.outBillLanguage = a.Value;
                        else if (a.Name == "outTumbol") model.outTumbol = a.Value;
                        else if (a.Name == "outBirthDate") model.outBirthDate = a.Value;
                        else if (a.Name == "outEmail") model.outEmail = a.Value;
                        else if (a.Name == "outparameter2") model.outparameter2 = a.Value;
                        else if (a.Name == "outBillingAccountNumber") model.outBillingAccountNumber = a.Value;
                        else if (a.Name == "outAccountNumber") model.outAccountNumber = a.Value;
                        else if (a.Name == "outServiceAccountNumber") model.outServiceAccountNumber = a.Value;
                        else if (a.Name == "outAccountName") model.outAccountName = a.Value;
                        else if (a.Name == "outProductName") model.outProductName = a.Value;
                        else if (a.Name == "outRegisteredDate") model.outRegisteredDate = a.Value;
                        else if (a.Name == "outServiceYear") model.outServiceYear = a.Value;
                        else if (a.Name == "outAccountSubCategory") model.outAccountSubCategory = a.Value;
                        else if (a.Name == "outPostalCode") model.outPostalCode = a.Value;
                        else if (a.Name == "outTitle") model.outTitle = a.Value;
                        else if (a.Name == "outBillingSystem") model.outBillingSystem = a.Value;
                        else if (a.Name == "outFullAddress") model.outFullAddress = a.Value;
                        //15.7 add output parameter                      
                        else if (a.Name == "vatAddress1") model.vatAddress1 = a.Value;
                        else if (a.Name == "vatAddress2") model.vatAddress2 = a.Value;
                        else if (a.Name == "vatAddress3") model.vatAddress3 = a.Value;
                        else if (a.Name == "vatAddress4") model.vatAddress4 = a.Value;
                        else if (a.Name == "vatAddress5") model.vatAddress5 = a.Value;
                        else if (a.Name == "vatPostalCd") model.vatPostalCd = a.Value;
                        //16.3 add output parameter                      
                        else if (a.Name == "outMobileSegment") model.outMobileSegment = a.Value;
                        else if (a.Name == "outAccountCategory") model.outAccountCategory = a.Value;
                        //16.6
                        else if (a.Name == "outServiceMobileNo") model.outServiceMobileNo = a.Value;
                        //20.3 Service Level
                        else if (a.Name == "outServiceLevel") model.outServiceLevel = a.Value;
                        else if (a.Name == "outPaGroup") model.outPaGroup = a.Value;
                    }
                    if (data.ParameterList.ParameterList1 != null)
                    {
                        foreach (var c in data.ParameterList.ParameterList1)
                        {
                            //R20.6 Edit by Aware : Atipon
                            //if (c != null && c.Parameter[0].Name == "projectName")
                            //{
                            //    model.projectName = c.Parameter[0].Value;
                            //}
                            if (c != null)
                            {
                                var projectName = c.Parameter.Where(p => p.Name == "projectName").FirstOrDefault();

                                if (projectName != null)
                                {
                                    model.projectName = projectName.Value;
                                }

                                var projectOption = c.Parameter.Where(p => p.Name == "projectOption").FirstOrDefault();

                                if (projectOption != null)
                                {
                                    model.projectOption = projectOption.Value;
                                }
                            }
                            //

                            //หาเบอร์โทรศัพท์ กรณีกรอกเบอร์ 88 เข้ามา
                            if (c != null)
                            {

                                if (c.ParameterList1 != null)
                                {
                                    foreach (var d in c.ParameterList1)
                                    {
                                        if (d != null)
                                        {
                                            SFFServices.Parameter tmp = new SFFServices.Parameter();
                                            foreach (var e in d.Parameter)
                                            {
                                                if (e.Name == "instanceName")
                                                {
                                                    tmp.Name = e.Value;
                                                }
                                                else if (e.Name == "mobileNo")
                                                {
                                                    tmp.Value = e.Value;
                                                }

                                                if (tmp.Name == "3G" && !string.IsNullOrEmpty(tmp.Value))
                                                {
                                                    model.outMobileNumber = tmp.Value;
                                                    break;
                                                }
                                            }

                                            //R20.6 ChangePromotionCheckRight
                                            var isRowFBB = d.Parameter.FirstOrDefault(f => f.Name == "instanceName" && f.Value == "FBB");
                                            if (isRowFBB != null)
                                            {
                                                var rowType = d.Parameter.FirstOrDefault(f => f.Name == "rowType" && !string.IsNullOrEmpty(f.Value));
                                                if (rowType != null)
                                                {
                                                    model.outInstanceNameFBBrowType = rowType.Value;
                                                }

                                                var nonMobileNumber = d.Parameter.FirstOrDefault(f => f.Name == "mobileNo" && !string.IsNullOrEmpty(f.Value));
                                                if (nonMobileNumber != null)
                                                {
                                                    model.outInstanceNameFBBnonMobileNumber = nonMobileNumber.Value;
                                                }
                                            }

                                            //R20.6 Add by Aware : Atipon
                                            var rowTypeCheckRight = d.Parameter.Where(p => p.Name == "rowType" && p.Value == "checkRight").FirstOrDefault();

                                            if (rowTypeCheckRight != null)
                                            {
                                                var curMobileCheckRight = d.Parameter.Where(p => p.Name == "mobileNo").FirstOrDefault();

                                                if (curMobileCheckRight != null)
                                                {
                                                    model.curMobileCheckRight = curMobileCheckRight.Value;
                                                }

                                                var curMobileCheckRightOption = d.Parameter.Where(p => p.Name == "projectOption").FirstOrDefault();

                                                if (curMobileCheckRightOption != null)
                                                {
                                                    model.curMobileCheckRightOption = curMobileCheckRightOption.Value;
                                                }
                                            }

                                            var rowTypeGetBenefit = d.Parameter.Where(p => p.Name == "rowType" && p.Value == "getBenefit").FirstOrDefault();

                                            if (rowTypeGetBenefit != null)
                                            {
                                                var curMobileGetBenefit = d.Parameter.Where(p => p.Name == "mobileNo").FirstOrDefault();

                                                if (curMobileGetBenefit != null)
                                                {
                                                    model.curMobileGetBenefit = curMobileGetBenefit.Value;
                                                }

                                                var curMobileGetBenefitOption = d.Parameter.Where(p => p.Name == "projectOption").FirstOrDefault();

                                                if (curMobileGetBenefitOption != null)
                                                {
                                                    model.curMobileGetBenefitOption = curMobileGetBenefitOption.Value;
                                                }
                                            }
                                            //
                                        }
                                    }
                                }
                            }
                        }
                    }
                    model.vatAddressFull = model.vatAddress1 + " " + model.vatAddress2 + " " + model.vatAddress3 + " " + model.vatAddress4 + " " + model.vatAddress5 + model.vatPostalCd;

                    if (model.projectName == "Triple Play" || model.projectName != null)
                    {
                        model.vataddTripleplay = "N";//NEW
                    }
                    else
                    {
                        model.vataddTripleplay = "Y";//AIS
                    }

                    logger.Info(data.ErrorMessage);

                    logger.Info("Product: " + model.outProductName);

                    var tempProduct = model.outProductName;

                    if (model.outBillingSystem == "BOS")
                        model.IsAWNProduct = false;
                    else
                    {
                        model.IsAWNProduct = true;
                        var chk3G = (from r in lovService.Get()
                                     where r.LOV_NAME == model.outProductName && r.ACTIVEFLAG == "Y" && r.LOV_TYPE == "AWN_PRODUCT"
                                     select r);

                        model.IsAWNProduct = chk3G.Any();

                        if (model.IsAWNProduct)
                            model.outProductName = "3G";
                        else
                            model.outProductName = "2G";
                    }

                    model.cardType = (from r in lovService.Get()
                                      where r.LOV_TYPE == "ID_CARD_TYPE" && r.LOV_NAME == query.inCardType
                                      select r.LOV_VAL3).FirstOrDefault();

                    //insert log
                    var log = new FBB_SFF_CHKPROFILE_LOG
                    {
                        INAPPLICATION = query.Page,
                        INMOBILENO = query.inMobileNo,
                        INIDCARDNO = query.inCardNo,
                        INIDCARDTYPE = query.inCardType,
                        OUTBUILDINGNAME = model.outBuildingName,
                        OUTFLOOR = model.outFloor,
                        OUTHOUSENUMBER = model.outHouseNumber,
                        OUTMOO = model.outMoo,
                        OUTSOI = model.outSoi,
                        OUTSTREETNAME = model.outStreetName,
                        OUTEMAIL = model.outEmail,
                        OUTPROVINCE = model.outProvince,
                        OUTAMPHUR = model.outAmphur,
                        OUTTUMBOL = model.outTumbol,
                        OUTACCOUNTNUMBER = model.outAccountNumber,
                        OUTSERVICEACCOUNTNUMBER = model.outServiceAccountNumber,
                        OUTBILLINGACCOUNTNUMBER = model.outBillingAccountNumber,
                        OUTBIRTHDATE = model.outBirthDate,
                        OUTACCOUNTNAME = model.outAccountName,
                        OUTPRIMARYCONTACTFIRSTNAME = model.outPrimaryContactFirstName,
                        OUTCONTACTLASTNAME = model.outContactLastName,
                        ERRORMESSAGE = model.errorMessage,
                        OUTPRODUCTNAME = tempProduct + "/" + model.outProductName,
                        OUTSERVICEYEAR = model.outServiceYear,
                        CREATED_BY = query.Username,
                        CREATED_DATE = DateTime.Now,
                        OUTMOOBAN = model.outMooban,
                        OUTACCOUNTSUBCATEGORY = model.outAccountSubCategory,
                        OUTPARAMETER2 = model.outparameter2,
                        OUTPOSTALCODE = model.outPostalCode,
                        OUTTITLE = model.outTitle,
                        TRANSACTION_ID = query.ReferenceID,
                        OUTFULLADDRESS = model.outFullAddress,

                        PROJECTNAME = model.projectName,
                        VATADDRESS1 = model.vatAddress1,
                        VATADDRESS2 = model.vatAddress2,
                        VATADDRESS3 = model.vatAddress3,
                        VATADDRESS4 = model.vatAddress4,
                        VATADDRESS5 = model.vatAddress5,
                        VATPOSTALCD = model.vatPostalCd,

                        OUTMOBILESEGMENT = model.outMobileSegment,

                    };

                    sffLog.Create(log);
                    uow.Persist();

                    model.SffProfileLogID = log.SFF_CHKPROFILE_ID;

                    logger.Info("SFF_CHKPROFILE_ID LASTEST: " + model.SffProfileLogID);
                    logger.Info("outRegisteredDate: " + model.outRegisteredDate);

                    //for Order Dup --boy.
                    //select 'Y' 
                    //into v_add_playbox
                    //from fbb_cust_package
                    //where cust_non_mobile=@หมายเลขผู้ใช้ Internet
                    //and package_subtype='PBOX'
                    //and rownum=1;
                    //var cpb = (from r in custPackage.Get()
                    //           where r.CUST_NON_MOBILE == query.inMobileNo
                    //           && r.PACKAGE_SUBTYPE == "PBOX"
                    //           select r);

                    //if (cpb.Any())
                    //{
                    //    model.checkPlayBox = "DUPLICATE";
                    //}
                    //else
                    //{
                    //    #region old query by yeen
                    //    //select 'Y'
                    //    //from fbb_cust_profile p,fbb_register r,fbb_package_tran pk
                    //    //where p.cust_non_mobile='8850002996'
                    //    //and p.order_no=r.return_order
                    //    //and r.row_id=pk.cust_row_id
                    //    //and pk.product_subtype='PBOX'
                    //    //and rownum=1

                    //    //var cpbInOrder = from p in custProfile.Get()
                    //    //                 join r in register.Get() on p.ORDER_NO equals r.RETURN_ORDER
                    //    //                 join pk in packageTran.Get() on r.ROW_ID equals pk.CUST_ROW_ID
                    //    //                 where p.CUST_NON_MOBILE == query.inMobileNo
                    //    //                 && pk.PRODUCT_SUBTYPE == "PBOX"
                    //    //                 select p;
                    //    #endregion

                    //    //select 'Y'
                    //    //from fbb_register r,fbb_package_tran pk
                    //    //where r.ais_non_mobile='8850003064'
                    //    //and r.row_id=pk.cust_row_id
                    //    //and pk.product_subtype='PBOX'
                    //    //and rownum=1

                    //    var cpbInOrder = from r in register.Get()
                    //                     join pk in packageTran.Get() on r.ROW_ID equals pk.CUST_ROW_ID
                    //                     where r.AIS_NON_MOBILE == query.inMobileNo
                    //                     && pk.PRODUCT_SUBTYPE == "PBOX"
                    //                     select r;

                    //    if (cpbInOrder.Any())
                    //        model.checkPlayBox = "IN_PROGRESS";
                    //}


                    //var regisDate = Convert.ToDateTime(model.outRegisteredDate).Date; //14/10/2014 16:20:51
                    //model.outDayOfServiceYear = (DateTime.Now.Date - regisDate).Days.ToString();
                    if (model.outRegisteredDate != null)
                    {
                        var tempRegisDate = model.outRegisteredDate.Split(' ');
                        if (tempRegisDate.Any())
                        {
                            logger.Info("tempRegisDate: " + tempRegisDate[0]);
                            var regisDate = DateTime.ParseExact(tempRegisDate[0], "dd/MM/yyyy", CultureInfo.InvariantCulture);

                            logger.Info("convertRegisteredDate: " + regisDate);
                            model.outDayOfServiceYear = (DateTime.Now.Date - regisDate).Days.ToString();
                            model.outRegisteredDate = tempRegisDate[0].ToSafeString();
                            logger.Info("outDayOfServiceYear: " + model.outDayOfServiceYear);
                        }
                    }
                }
            }
        }

        public static void GetMassCommonAccountInfoForAddBundling(ILogger logger,
            IWBBUnitOfWork uow,
            IEntityRepository<FBB_CFG_LOV> lovService,
            IEntityRepository<FBB_SFF_CHKPROFILE_LOG> sffLog,
            IGetMassCommonAccount query,
            SFFServices.SffRequest request,
            evESeServiceQueryMassCommonAccountInfoModel model,
             IEntityRepository<FBB_CUST_PACKAGE> custPackage,
            IEntityRepository<FBB_CUST_PROFILE> custProfile,
            IEntityRepository<FBB_REGISTER> register,
            IEntityRepository<FBB_PACKAGE_TRAN> packageTran)
        {
            using (var service = new SFFServices.SFFServiceService())
            {
                var data = service.ExecuteService(request);

                if (data != null)
                {
                    logger.Info(data.ErrorMessage);
                    model.outErrorMessage = data.ErrorMessage;
                    var errSp = data.ErrorMessage.Trim().Split(':');
                    model.errorMessage = errSp[0];
                    foreach (var a in data.ParameterList.Parameter)
                    {
                        if (a.Name == "outPrimaryContactFirstName") model.outPrimaryContactFirstName = a.Value;
                        else if (a.Name == "outContactLastName") model.outContactLastName = a.Value;
                        else if (a.Name == "outAmphur") model.outAmphur = a.Value;
                        else if (a.Name == "outBuildingName") model.outBuildingName = a.Value;
                        else if (a.Name == "outFloor") model.outFloor = a.Value;
                        else if (a.Name == "outHouseNumber") model.outHouseNumber = a.Value;
                        else if (a.Name == "outMoo") model.outMoo = a.Value;
                        else if (a.Name == "outMooban") model.outMooban = a.Value;
                        else if (a.Name == "outProvince") model.outProvince = a.Value;
                        else if (a.Name == "outRoom") model.outRoom = a.Value;
                        else if (a.Name == "outSoi") model.outSoi = a.Value;
                        else if (a.Name == "outStreetName") model.outStreetName = a.Value;
                        else if (a.Name == "outBillLanguage") model.outBillLanguage = a.Value;
                        else if (a.Name == "outTumbol") model.outTumbol = a.Value;
                        else if (a.Name == "outBirthDate") model.outBirthDate = a.Value;
                        else if (a.Name == "outEmail") model.outEmail = a.Value;
                        else if (a.Name == "outparameter2") model.outparameter2 = a.Value;
                        else if (a.Name == "outBillingAccountNumber") model.outBillingAccountNumber = a.Value;
                        else if (a.Name == "outAccountNumber") model.outAccountNumber = a.Value;
                        else if (a.Name == "outServiceAccountNumber") model.outServiceAccountNumber = a.Value;
                        else if (a.Name == "outAccountName") model.outAccountName = a.Value;
                        else if (a.Name == "outProductName") model.outProductName = a.Value;
                        else if (a.Name == "outRegisteredDate") model.outRegisteredDate = a.Value;
                        else if (a.Name == "outServiceYear") model.outServiceYear = a.Value;
                        else if (a.Name == "outAccountSubCategory") model.outAccountSubCategory = a.Value;
                        else if (a.Name == "outPostalCode") model.outPostalCode = a.Value;
                        else if (a.Name == "outTitle") model.outTitle = a.Value;
                        else if (a.Name == "outBillingSystem") model.outBillingSystem = a.Value;
                        else if (a.Name == "outFullAddress") model.outFullAddress = a.Value;
                        //15.7 add output parameter                      
                        else if (a.Name == "vatAddress1") model.vatAddress1 = a.Value;
                        else if (a.Name == "vatAddress2") model.vatAddress2 = a.Value;
                        else if (a.Name == "vatAddress3") model.vatAddress3 = a.Value;
                        else if (a.Name == "vatAddress4") model.vatAddress4 = a.Value;
                        else if (a.Name == "vatAddress5") model.vatAddress5 = a.Value;
                        else if (a.Name == "vatPostalCd") model.vatPostalCd = a.Value;
                        //16.3 add output parameter                      
                        else if (a.Name == "outMobileSegment") model.outMobileSegment = a.Value;
                        else if (a.Name == "outAccountCategory") model.outAccountCategory = a.Value;
                        //16.6
                        else if (a.Name == "outServiceMobileNo") model.outServiceMobileNo = a.Value;
                        //20.3 Service Level
                        else if (a.Name == "outServiceLevel") model.outServiceLevel = a.Value;
                        else if (a.Name == "outPaGroup") model.outPaGroup = a.Value;
                    }
                    if (data.ParameterList.ParameterList1 != null)
                    {
                        foreach (var c in data.ParameterList.ParameterList1)
                        {
                            if (c != null && c.Parameter[0].Name == "projectName")
                            {
                                model.projectName = c.Parameter[0].Value;
                            }

                            //หาเบอร์โทรศัพท์ หรือ เบอร์ 88
                            if (c != null)
                            {

                                if (c.ParameterList1 != null)
                                {
                                    foreach (var d in c.ParameterList1)
                                    {
                                        if (d != null)
                                        {
                                            SFFServices.Parameter tmp = new SFFServices.Parameter();
                                            foreach (var e in d.Parameter)
                                            {
                                                if (e.Name == "instanceName")
                                                {
                                                    tmp.Name = e.Value;
                                                }
                                                else if (e.Name == "mobileNo")
                                                {
                                                    tmp.Value = e.Value;
                                                }

                                                if ((tmp.Name == "3G" || tmp.Name == "FBB") && !string.IsNullOrEmpty(tmp.Value))
                                                {
                                                    model.outMobileNumber = tmp.Value;
                                                    break;
                                                }
                                            }

                                        }
                                    }
                                }
                            }
                        }
                    }
                    model.vatAddressFull = model.vatAddress1 + " " + model.vatAddress2 + " " + model.vatAddress3 + " " + model.vatAddress4 + " " + model.vatAddress5 + model.vatPostalCd;

                    if (model.projectName == "Triple Play" || model.projectName != null)
                    {
                        model.vataddTripleplay = "N";//NEW
                    }
                    else
                    {
                        model.vataddTripleplay = "Y";//AIS
                    }

                    logger.Info(data.ErrorMessage);

                    logger.Info("Product: " + model.outProductName);

                    var tempProduct = model.outProductName;

                    if (model.outBillingSystem == "BOS")
                        model.IsAWNProduct = false;
                    else
                    {
                        model.IsAWNProduct = true;
                        var chk3G = (from r in lovService.Get()
                                     where r.LOV_NAME == model.outProductName && r.ACTIVEFLAG == "Y" && r.LOV_TYPE == "AWN_PRODUCT"
                                     select r);

                        model.IsAWNProduct = chk3G.Any();

                        if (model.IsAWNProduct)
                            model.outProductName = "3G";
                        else
                            model.outProductName = "2G";
                    }

                    model.cardType = (from r in lovService.Get()
                                      where r.LOV_TYPE == "ID_CARD_TYPE" && r.LOV_NAME == query.inCardType
                                      select r.LOV_VAL3).FirstOrDefault();

                    //insert log
                    var log = new FBB_SFF_CHKPROFILE_LOG
                    {
                        INAPPLICATION = query.Page,
                        INMOBILENO = query.inMobileNo,
                        INIDCARDNO = query.inCardNo,
                        INIDCARDTYPE = query.inCardType,
                        OUTBUILDINGNAME = model.outBuildingName,
                        OUTFLOOR = model.outFloor,
                        OUTHOUSENUMBER = model.outHouseNumber,
                        OUTMOO = model.outMoo,
                        OUTSOI = model.outSoi,
                        OUTSTREETNAME = model.outStreetName,
                        OUTEMAIL = model.outEmail,
                        OUTPROVINCE = model.outProvince,
                        OUTAMPHUR = model.outAmphur,
                        OUTTUMBOL = model.outTumbol,
                        OUTACCOUNTNUMBER = model.outAccountNumber,
                        OUTSERVICEACCOUNTNUMBER = model.outServiceAccountNumber,
                        OUTBILLINGACCOUNTNUMBER = model.outBillingAccountNumber,
                        OUTBIRTHDATE = model.outBirthDate,
                        OUTACCOUNTNAME = model.outAccountName,
                        OUTPRIMARYCONTACTFIRSTNAME = model.outPrimaryContactFirstName,
                        OUTCONTACTLASTNAME = model.outContactLastName,
                        ERRORMESSAGE = model.errorMessage,
                        OUTPRODUCTNAME = tempProduct + "/" + model.outProductName,
                        OUTSERVICEYEAR = model.outServiceYear,
                        CREATED_BY = query.Username,
                        CREATED_DATE = DateTime.Now,
                        OUTMOOBAN = model.outMooban,
                        OUTACCOUNTSUBCATEGORY = model.outAccountSubCategory,
                        OUTPARAMETER2 = model.outparameter2,
                        OUTPOSTALCODE = model.outPostalCode,
                        OUTTITLE = model.outTitle,
                        TRANSACTION_ID = query.ReferenceID,
                        OUTFULLADDRESS = model.outFullAddress,

                        PROJECTNAME = model.projectName,
                        VATADDRESS1 = model.vatAddress1,
                        VATADDRESS2 = model.vatAddress2,
                        VATADDRESS3 = model.vatAddress3,
                        VATADDRESS4 = model.vatAddress4,
                        VATADDRESS5 = model.vatAddress5,
                        VATPOSTALCD = model.vatPostalCd,

                        OUTMOBILESEGMENT = model.outMobileSegment,

                    };

                    sffLog.Create(log);
                    uow.Persist();

                    model.SffProfileLogID = log.SFF_CHKPROFILE_ID;

                    logger.Info("SFF_CHKPROFILE_ID LASTEST: " + model.SffProfileLogID);
                    logger.Info("outRegisteredDate: " + model.outRegisteredDate);

                    if (model.outRegisteredDate != null)
                    {
                        var tempRegisDate = model.outRegisteredDate.Split(' ');
                        if (tempRegisDate.Any())
                        {
                            logger.Info("tempRegisDate: " + tempRegisDate[0]);
                            var regisDate = DateTime.ParseExact(tempRegisDate[0], "dd/MM/yyyy", CultureInfo.InvariantCulture);

                            logger.Info("convertRegisteredDate: " + regisDate);
                            model.outDayOfServiceYear = (DateTime.Now.Date - regisDate).Days.ToString();
                            model.outRegisteredDate = tempRegisDate[0].ToSafeString();
                            logger.Info("outDayOfServiceYear: " + model.outDayOfServiceYear);
                        }
                    }
                }
            }
        }

        public static void CheckChangePromotion(
            SFFServices.SffRequest request,
            evOMServiceCheckChangePromotionModel model)
        {
            using (var service = new SFFServices.SFFServiceService())
            {
                var data = service.ExecuteService(request);
                if (data != null)
                {
                    foreach (var a in data.ParameterList.Parameter)
                    {
                        if (a.Name == "returnCode") model.ReturnCode = a.Value;
                    }
                }
            }
        }

        /// <summary>
        /// Do asyn at this method
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="lov"></param>
        /// <param name="sffLog"></param>
        /// <param name="query"></param>
        /// <param name="model"></param>
        public static void ConfirmChangePromotion(
            SFFServices.SffRequest request,
            evOMServiceConfirmChangePromotionModel model)
        {
            using (var service = new SFFServices.SFFServiceService())
            {
                var data = service.ExecuteService(request);

                if (data != null)
                {
                    foreach (var a in data.ParameterList.Parameter)
                    {
                        if (a.Name == "successFlag") model.SuccessFlag = a.Value;
                    }
                }
                else
                {
                    model.SuccessFlag = "N";
                }

                // ย้ายไปเรียก PROC_REGISTER_AISWIFI ก่อนที่จะทำ evOMServiceConfirmChangePromotion
                // เนื่องจากต้อง return order_no

            }
        }

        public static void OMCheckDeviceContract(SFFServices.SffRequest request, evOMCheckDeviceContractModel model)
        {
            using (var service = new SFFServices.SFFServiceService())
            {
                SFFServices.SffResponse data = service.ExecuteService(request);

                if (data != null)
                {
                    string returnCode = "";
                    foreach (var a in data.ParameterList.Parameter)
                    {
                        if (a.Name == "contractFlagFbb") model.contractFlagFbb = a.Value.ToSafeString();
                        else if (a.Name == "countContractFbb") model.countContractFbb = a.Value.ToSafeString();
                        else if (a.Name == "fbbLimitContract") model.fbbLimitContract = a.Value.ToSafeString();
                        else if (a.Name == "contractProfileCountFbb") model.contractProfileCountFbb = a.Value.ToSafeString();
                        else if (a.Name == "returnCode") returnCode = a.Value.ToSafeString();
                    }
                    if (returnCode != "")
                    {
                        model.errorMessage = "returnCode : " + returnCode;
                    }
                }
                else
                {
                    model.errorMessage = "No SffResponse data.";
                }
            }
        }
    }


    public static class VsmpServiceConseHelper
    {
        public static InterfaceLogCommand StartInterfaceVsmpLog<T>(IWBBUnitOfWork uow,
            IEntityRepository<FBB_INTERFACE_LOG> interfaceLog,
            T query, string transactionId, string methodName, string serviceName)
        {
            var dbIntfCmd = new InterfaceLogCommand
            {
                ActionType = WBBContract.Commands.ActionType.Insert,
                IN_TRANSACTION_ID = transactionId,
                METHOD_NAME = methodName,
                SERVICE_NAME = serviceName,
                IN_XML_PARAM = query.DumpToXml(),
                INTERFACE_NODE = "FBB",
                CREATED_BY = "FbbVsmpInterfaceLog",
            };

            var log = InterfaceLogHelper.Log(interfaceLog, dbIntfCmd);
            uow.Persist();

            dbIntfCmd.OutInterfaceLogId = log.INTERFACE_ID;
            return dbIntfCmd;
        }

        public static void EndInterfaceVsmpLog<T>(IWBBUnitOfWork uow,
            IEntityRepository<FBB_INTERFACE_LOG> interfaceLog,
            T output, InterfaceLogCommand dbIntfCmd,
            string result, string reason)
        {
            dbIntfCmd.ActionType = WBBContract.Commands.ActionType.Update;
            dbIntfCmd.REQUEST_STATUS = (result == "Success") ? "Success" : "Error";
            dbIntfCmd.OUT_RESULT = result;
            dbIntfCmd.OUT_ERROR_RESULT = (result == "Success") ? (reason.Length > 100 ? reason.Substring(0, 100) : result) : result;
            dbIntfCmd.OUT_XML_PARAM = output.DumpToXml();

            InterfaceLogHelper.Log(interfaceLog, dbIntfCmd);
            uow.Persist();
        }

    }

    #region SACF
    public static class SacfServiceConseHelper
    {
        public static InterfaceLogCommand StartInterfaceSffLog<T>(IWBBUnitOfWork uow,
            IEntityRepository<FBB_INTERFACE_LOG> interfaceLog,
            T query, string transactionId,
            string methodName, string serviceName, string idCardNo)
        {
            var dbIntfCmd = new InterfaceLogCommand
            {
                ActionType = WBBContract.Commands.ActionType.Insert,
                IN_TRANSACTION_ID = transactionId,
                METHOD_NAME = methodName,
                SERVICE_NAME = serviceName,
                IN_ID_CARD_NO = idCardNo,
                IN_XML_PARAM = query.DumpToXml(),
                INTERFACE_NODE = "FBB",
                CREATED_BY = "FbbSacfInterface",
            };

            var log = InterfaceLogHelper.Log(interfaceLog, dbIntfCmd);
            uow.Persist();

            dbIntfCmd.OutInterfaceLogId = log.INTERFACE_ID;
            return dbIntfCmd;
        }

        public static void EndInterfaceSffLog<T>(IWBBUnitOfWork uow,
            IEntityRepository<FBB_INTERFACE_LOG> interfaceLog,
            T output, InterfaceLogCommand dbIntfCmd,
            string result, string reason)
        {
            dbIntfCmd.ActionType = WBBContract.Commands.ActionType.Update;
            dbIntfCmd.REQUEST_STATUS = (result == "Success") ? "Success" : "Error";
            dbIntfCmd.OUT_RESULT = result;
            dbIntfCmd.OUT_ERROR_RESULT = (result == "Success") ? (reason.Length > 100 ? reason.Substring(0, 100) : result) : result;
            dbIntfCmd.OUT_XML_PARAM = output.DumpToXml();

            InterfaceLogHelper.Log(interfaceLog, dbIntfCmd);
            uow.Persist();
        }

        public static bool GetUuidAndMobileByFbbId(
            IEntityRepository<FBB_CFG_LOV> lovService,
            IGetMassCommonAccount query,
            evESeServiceQueryMassCommonAccountInfoModel model)
        {
            bool result = false;
            var fbbConfigByIPCamera = lovService.Get().FirstOrDefault(lov => lov.LOV_TYPE == "FBB_CONFIG" && lov.LOV_NAME == "3BB_IPCAMERA");

            if (fbbConfigByIPCamera?.ACTIVEFLAG == "Y") // Flag check use 3bb
            {
                result = true;

                // req
                var reqInterfaceExternalAPI = new
                {
                    Url = fbbConfigByIPCamera.LOV_VAL1, // URL
                    X_App = fbbConfigByIPCamera.LOV_VAL2, // X-APP
                    X_Tid = fbbConfigByIPCamera.LOV_VAL3, // X-Tid
                };

                try
                {
                    // client
                    var restClient = new RestClient(reqInterfaceExternalAPI.Url);

                    var restRequest = new RestRequest(Method.POST);

                    // Set Content-Type
                    restRequest.AddHeader("Content-Type", "application/json");

                    // Form URL-Encoded
                    restRequest.AddHeader("X-app", reqInterfaceExternalAPI.X_App);
                    restRequest.AddHeader("X-Tid", reqInterfaceExternalAPI.X_Tid);

                    // Set Body
                    restRequest.AddJsonBody(new { fbbId = query.inMobileNo });

                    // execute the request
                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                    ServicePointManager.ServerCertificateValidationCallback = (s, certificate, chain, sslPolicyErrors) => true;

                    var restResponse = restClient.Execute(restRequest);

                    if (restResponse.StatusCode == HttpStatusCode.OK)
                    {
                        dynamic data = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(restResponse.Content);

                        if (data.returnCode == "1")
                        {
                            model.outIPCamera3bbErrorMessage = string.Empty;
                            model.outAccountStatus = data.accountInfo?.accountStatus;
                            model.outIPCamera3bbAcountNumber = data.accountInfo?.accountNum;
                            model.outServiceMobileNo = data.accountInfo?.mobileNo;
                            model.outMobileNumber = data.accountInfo?.mobileNo;
                            model.outIPCamera3BBAccountUuid = data.accountInfo?.uuid;
                        }
                        else
                        {
                            model.outIPCamera3bbErrorMessage = data.returnMessage;
                        }
                    }
                    else
                    {
                        model.outIPCamera3bbErrorMessage = $"{restResponse.StatusCode.ToString()}: {restResponse.ErrorMessage}";
                    }
                }
                catch (Exception ex)
                {
                    model.outIPCamera3bbErrorMessage = $"Exception: {ex.Message}";
                }
            }

            return result;
        }


        public static void GetMassCommonAccountInfoForAddBundling(ILogger logger,
            IWBBUnitOfWork uow,
            IEntityRepository<FBB_CFG_LOV> lovService,
            IEntityRepository<FBB_SFF_CHKPROFILE_LOG> sffLog,
            IGetMassCommonAccount query,
            SFFServices.SffRequest request,
            evESeServiceQueryMassCommonAccountInfoModel model,
             IEntityRepository<FBB_CUST_PACKAGE> custPackage,
            IEntityRepository<FBB_CUST_PROFILE> custProfile,
            IEntityRepository<FBB_REGISTER> register,
            IEntityRepository<FBB_PACKAGE_TRAN> packageTran)
        {
            using (var service = new SFFServices.SFFServiceService())
            {
                var data = service.ExecuteService(request);

                if (data != null)
                {
                    logger.Info(data.ErrorMessage);
                    model.outErrorMessage = data.ErrorMessage;
                    var errSp = data.ErrorMessage.Trim().Split(':');
                    model.errorMessage = errSp[0];
                    foreach (var a in data.ParameterList.Parameter)
                    {
                        if (a.Name == "outPrimaryContactFirstName") model.outPrimaryContactFirstName = a.Value;
                        else if (a.Name == "outContactLastName") model.outContactLastName = a.Value;
                        else if (a.Name == "outAmphur") model.outAmphur = a.Value;
                        else if (a.Name == "outBuildingName") model.outBuildingName = a.Value;
                        else if (a.Name == "outFloor") model.outFloor = a.Value;
                        else if (a.Name == "outHouseNumber") model.outHouseNumber = a.Value;
                        else if (a.Name == "outMoo") model.outMoo = a.Value;
                        else if (a.Name == "outMooban") model.outMooban = a.Value;
                        else if (a.Name == "outProvince") model.outProvince = a.Value;
                        else if (a.Name == "outRoom") model.outRoom = a.Value;
                        else if (a.Name == "outSoi") model.outSoi = a.Value;
                        else if (a.Name == "outStreetName") model.outStreetName = a.Value;
                        else if (a.Name == "outBillLanguage") model.outBillLanguage = a.Value;
                        else if (a.Name == "outTumbol") model.outTumbol = a.Value;
                        else if (a.Name == "outBirthDate") model.outBirthDate = a.Value;
                        else if (a.Name == "outEmail") model.outEmail = a.Value;
                        else if (a.Name == "outparameter2") model.outparameter2 = a.Value;
                        else if (a.Name == "outBillingAccountNumber") model.outBillingAccountNumber = a.Value;
                        else if (a.Name == "outAccountNumber") model.outAccountNumber = a.Value;
                        else if (a.Name == "outServiceAccountNumber") model.outServiceAccountNumber = a.Value;
                        else if (a.Name == "outAccountName") model.outAccountName = a.Value;
                        else if (a.Name == "outProductName") model.outProductName = a.Value;
                        else if (a.Name == "outRegisteredDate") model.outRegisteredDate = a.Value;
                        else if (a.Name == "outServiceYear") model.outServiceYear = a.Value;
                        else if (a.Name == "outAccountSubCategory") model.outAccountSubCategory = a.Value;
                        else if (a.Name == "outPostalCode") model.outPostalCode = a.Value;
                        else if (a.Name == "outTitle") model.outTitle = a.Value;
                        else if (a.Name == "outBillingSystem") model.outBillingSystem = a.Value;
                        else if (a.Name == "outFullAddress") model.outFullAddress = a.Value;
                        //15.7 add output parameter                      
                        else if (a.Name == "vatAddress1") model.vatAddress1 = a.Value;
                        else if (a.Name == "vatAddress2") model.vatAddress2 = a.Value;
                        else if (a.Name == "vatAddress3") model.vatAddress3 = a.Value;
                        else if (a.Name == "vatAddress4") model.vatAddress4 = a.Value;
                        else if (a.Name == "vatAddress5") model.vatAddress5 = a.Value;
                        else if (a.Name == "vatPostalCd") model.vatPostalCd = a.Value;
                        //16.3 add output parameter                      
                        else if (a.Name == "outMobileSegment") model.outMobileSegment = a.Value;
                        else if (a.Name == "outAccountCategory") model.outAccountCategory = a.Value;
                        //16.6
                        else if (a.Name == "outServiceMobileNo") model.outServiceMobileNo = a.Value;
                        //20.3 Service Level
                        else if (a.Name == "outServiceLevel") model.outServiceLevel = a.Value;
                        else if (a.Name == "outPaGroup") model.outPaGroup = a.Value;
                    }
                    if (data.ParameterList.ParameterList1 != null)
                    {
                        foreach (var c in data.ParameterList.ParameterList1)
                        {
                            if (c != null && c.Parameter[0].Name == "projectName")
                            {
                                model.projectName = c.Parameter[0].Value;
                            }

                            //หาเบอร์โทรศัพท์ หรือ เบอร์ 88
                            if (c != null)
                            {

                                if (c.ParameterList1 != null)
                                {
                                    foreach (var d in c.ParameterList1)
                                    {
                                        if (d != null)
                                        {
                                            SFFServices.Parameter tmp = new SFFServices.Parameter();
                                            foreach (var e in d.Parameter)
                                            {
                                                if (e.Name == "instanceName")
                                                {
                                                    tmp.Name = e.Value;
                                                }
                                                else if (e.Name == "mobileNo")
                                                {
                                                    tmp.Value = e.Value;
                                                }

                                                if ((tmp.Name == "3G" || tmp.Name == "FBB") && !string.IsNullOrEmpty(tmp.Value))
                                                {
                                                    model.outMobileNumber = tmp.Value;
                                                    break;
                                                }
                                            }

                                        }
                                    }
                                }
                            }
                        }
                    }
                    model.vatAddressFull = model.vatAddress1 + " " + model.vatAddress2 + " " + model.vatAddress3 + " " + model.vatAddress4 + " " + model.vatAddress5 + model.vatPostalCd;

                    if (model.projectName == "Triple Play" || model.projectName != null)
                    {
                        model.vataddTripleplay = "N";//NEW
                    }
                    else
                    {
                        model.vataddTripleplay = "Y";//AIS
                    }

                    logger.Info(data.ErrorMessage);

                    logger.Info("Product: " + model.outProductName);

                    var tempProduct = model.outProductName;

                    if (model.outBillingSystem == "BOS")
                        model.IsAWNProduct = false;
                    else
                    {
                        model.IsAWNProduct = true;
                        var chk3G = (from r in lovService.Get()
                                     where r.LOV_NAME == model.outProductName && r.ACTIVEFLAG == "Y" && r.LOV_TYPE == "AWN_PRODUCT"
                                     select r);

                        model.IsAWNProduct = chk3G.Any();

                        if (model.IsAWNProduct)
                            model.outProductName = "3G";
                        else
                            model.outProductName = "2G";
                    }

                    model.cardType = (from r in lovService.Get()
                                      where r.LOV_TYPE == "ID_CARD_TYPE" && r.LOV_NAME == query.inCardType
                                      select r.LOV_VAL3).FirstOrDefault();

                    //insert log
                    var log = new FBB_SFF_CHKPROFILE_LOG
                    {
                        INAPPLICATION = query.Page,
                        INMOBILENO = query.inMobileNo,
                        INIDCARDNO = query.inCardNo,
                        INIDCARDTYPE = query.inCardType,
                        OUTBUILDINGNAME = model.outBuildingName,
                        OUTFLOOR = model.outFloor,
                        OUTHOUSENUMBER = model.outHouseNumber,
                        OUTMOO = model.outMoo,
                        OUTSOI = model.outSoi,
                        OUTSTREETNAME = model.outStreetName,
                        OUTEMAIL = model.outEmail,
                        OUTPROVINCE = model.outProvince,
                        OUTAMPHUR = model.outAmphur,
                        OUTTUMBOL = model.outTumbol,
                        OUTACCOUNTNUMBER = model.outAccountNumber,
                        OUTSERVICEACCOUNTNUMBER = model.outServiceAccountNumber,
                        OUTBILLINGACCOUNTNUMBER = model.outBillingAccountNumber,
                        OUTBIRTHDATE = model.outBirthDate,
                        OUTACCOUNTNAME = model.outAccountName,
                        OUTPRIMARYCONTACTFIRSTNAME = model.outPrimaryContactFirstName,
                        OUTCONTACTLASTNAME = model.outContactLastName,
                        ERRORMESSAGE = model.errorMessage,
                        OUTPRODUCTNAME = tempProduct + "/" + model.outProductName,
                        OUTSERVICEYEAR = model.outServiceYear,
                        CREATED_BY = query.Username,
                        CREATED_DATE = DateTime.Now,
                        OUTMOOBAN = model.outMooban,
                        OUTACCOUNTSUBCATEGORY = model.outAccountSubCategory,
                        OUTPARAMETER2 = model.outparameter2,
                        OUTPOSTALCODE = model.outPostalCode,
                        OUTTITLE = model.outTitle,
                        TRANSACTION_ID = query.ReferenceID,
                        OUTFULLADDRESS = model.outFullAddress,

                        PROJECTNAME = model.projectName,
                        VATADDRESS1 = model.vatAddress1,
                        VATADDRESS2 = model.vatAddress2,
                        VATADDRESS3 = model.vatAddress3,
                        VATADDRESS4 = model.vatAddress4,
                        VATADDRESS5 = model.vatAddress5,
                        VATPOSTALCD = model.vatPostalCd,

                        OUTMOBILESEGMENT = model.outMobileSegment,

                    };

                    sffLog.Create(log);
                    uow.Persist();

                    model.SffProfileLogID = log.SFF_CHKPROFILE_ID;

                    logger.Info("SFF_CHKPROFILE_ID LASTEST: " + model.SffProfileLogID);
                    logger.Info("outRegisteredDate: " + model.outRegisteredDate);

                    if (model.outRegisteredDate != null)
                    {
                        var tempRegisDate = model.outRegisteredDate.Split(' ');
                        if (tempRegisDate.Any())
                        {
                            logger.Info("tempRegisDate: " + tempRegisDate[0]);
                            var regisDate = DateTime.ParseExact(tempRegisDate[0], "dd/MM/yyyy", CultureInfo.InvariantCulture);

                            logger.Info("convertRegisteredDate: " + regisDate);
                            model.outDayOfServiceYear = (DateTime.Now.Date - regisDate).Days.ToString();
                            model.outRegisteredDate = tempRegisDate[0].ToSafeString();
                            logger.Info("outDayOfServiceYear: " + model.outDayOfServiceYear);
                        }
                    }
                }
            }
        }

        public static void CheckChangePromotion(
            SFFServices.SffRequest request,
            evOMServiceCheckChangePromotionModel model)
        {
            using (var service = new SFFServices.SFFServiceService())
            {
                var data = service.ExecuteService(request);
                if (data != null)
                {
                    foreach (var a in data.ParameterList.Parameter)
                    {
                        if (a.Name == "returnCode") model.ReturnCode = a.Value;
                    }
                }
            }
        }

        /// <summary>
        /// Do asyn at this method
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="lov"></param>
        /// <param name="sffLog"></param>
        /// <param name="query"></param>
        /// <param name="model"></param>
        public static void ConfirmChangePromotion(
            SFFServices.SffRequest request,
            evOMServiceConfirmChangePromotionModel model)
        {
            using (var service = new SFFServices.SFFServiceService())
            {
                var data = service.ExecuteService(request);

                if (data != null)
                {
                    foreach (var a in data.ParameterList.Parameter)
                    {
                        if (a.Name == "successFlag") model.SuccessFlag = a.Value;
                    }
                }
                else
                {
                    model.SuccessFlag = "N";
                }

                // ย้ายไปเรียก PROC_REGISTER_AISWIFI ก่อนที่จะทำ evOMServiceConfirmChangePromotion
                // เนื่องจากต้อง return order_no

            }
        }

        public static void OMCheckDeviceContract(SFFServices.SffRequest request, evOMCheckDeviceContractModel model)
        {
            using (var service = new SFFServices.SFFServiceService())
            {
                SFFServices.SffResponse data = service.ExecuteService(request);

                if (data != null)
                {
                    string returnCode = "";
                    foreach (var a in data.ParameterList.Parameter)
                    {
                        if (a.Name == "contractFlagFbb") model.contractFlagFbb = a.Value.ToSafeString();
                        else if (a.Name == "countContractFbb") model.countContractFbb = a.Value.ToSafeString();
                        else if (a.Name == "fbbLimitContract") model.fbbLimitContract = a.Value.ToSafeString();
                        else if (a.Name == "contractProfileCountFbb") model.contractProfileCountFbb = a.Value.ToSafeString();
                        else if (a.Name == "returnCode") returnCode = a.Value.ToSafeString();
                    }
                    if (returnCode != "")
                    {
                        model.errorMessage = "returnCode : " + returnCode;
                    }
                }
                else
                {
                    model.errorMessage = "No SffResponse data.";
                }
            }
        }
    }
    #endregion

}
