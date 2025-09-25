using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.UI;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Queries.Commons.Master;
using WBBContract.Queries.ExWebServices;
using WBBContract.Queries.FBBHVR;
using WBBContract.Queries.FBBShareplex;
using WBBContract.Queries.WebServices;
using WBBContract.Queries.WebServices.FBSS;
using WBBEntity.Extensions;
using WBBEntity.PanelModels;
using WBBEntity.PanelModels.ExWebServiceModels;
using WBBEntity.PanelModels.ShareplexModels;
using WBBEntity.PanelModels.WebServiceModels;
using WBBWeb.Extension;
using WBBWeb.Models;

namespace WBBWeb.Controllers
{
    public class SelectPackageController : WBBController
    {
        private IQueryProcessor _queryProcessor;
        public SelectPackageController(ILogger logger,
            IQueryProcessor queryProcessor)
        {
            base.Logger = logger;
            _queryProcessor = queryProcessor;
        }

        [OutputCache(Location = OutputCacheLocation.Server, Duration = 3600)]
        public ActionResult GetListPackageServiceV2(string AisAirNumber = "", string accessMode = "", string isPartner = "", string partnerName = "",
            string subNetworkType = "", string accountCategory = "", string sffServiceYear = "", string p_projectname = "",
            string P_PRODUCT_SUBTYPE = "", string P_PACKAGE_FOR = "", string P_LOCATION_CODE = "", string P_ASC_CODE = "", string P_REGION = "", string P_PROVINCE = "",
            string P_DISTRICT = "", string P_SUB_DISTRICT = "", string P_PARTNER_TYPE = "", string P_PARTNER_SUBTYPE = "", string P_SERENADE_FLAG = "",
            string P_CUSTOMER_TYPE = "", string P_ADDRESS_ID = "", string P_CUSTOMER_SUBTYPE = "", string P_STAFF_ID = "", string P_SALE_CHANNEL = "CUSTOMER",
            string P_NON_RES_FLAG = "", string P_DistChn = "", string P_ChnSales = "", string P_OperatorClass = "", string P_LocationProvince = "",
            string P_FMPA_FLAG = "", string P_CVM_FLAG = "", string P_FMC_SPECIAL_FLAG = "", string P_EXISTING_MOBILE_FLAG = "", string P_USE_INPUT_MOBILE_ONLINE_QUERY = "",
            string P_MOU_FLAG = "", string Is3bbCoverage = "", string addressType = "")
        {
            string FullUrl = "";
            if (Session["FullUrl"] != null)
                FullUrl = Session["FullUrl"].ToSafeString();

            #region Get IP Address Interface Log (Update 17.2)

            // Get IP Address
            string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress)) ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];


            var httpCookie = Request.Cookies["ASP.NET_SessionId"];
            var cookieSessionID = httpCookie != null ? httpCookie.Value : Session.SessionID;

            string transactionId = AisAirNumber + ipAddress;

            #endregion

            string userAgent = "";
            if (Request.Headers["User-Agent"] != null)
            {
                var headers = Request.Headers.GetValues("User-Agent");

                StringBuilder sb = new StringBuilder();

                foreach (var header in headers)
                {
                    sb.Append(header);

                    // Re-add spaces stripped when user agent string was split up.
                    sb.Append(" ");
                }

                userAgent = sb.ToString().Trim();
            }
            bool IsThaiCulture = SiteSession.CurrentUICulture.IsThaiCulture();

            var jsonResult = new JsonNetResult();
            List<FBSSAccessModeInfo> accessModeModel = new List<FBSSAccessModeInfo>();
            if (!string.IsNullOrEmpty(accessMode))
                accessModeModel = Newtonsoft.Json.JsonConvert.DeserializeObject<List<FBSSAccessModeInfo>>(accessMode);

            var owenerProduct = "";

            owenerProduct = _queryProcessor.Execute(new GetMappingSbnOwnerProd
            {
                IsPartner = isPartner,
                PartnerName = partnerName,
                FBSSAccessModeInfo = accessModeModel
            });

            if (Is3bbCoverage == "Y" && accessModeModel != null && accessModeModel.Count() > 0 && addressType != "B")
            {
                foreach (var item in accessModeModel)
                {
                    if (!string.IsNullOrEmpty(item.AccessMode))
                    {
                        P_PRODUCT_SUBTYPE = item.AccessMode;
                    }
                }
            }

            /// Update PRODUCT_SUBTYPE R.2103
            if (P_PRODUCT_SUBTYPE == "" && !string.IsNullOrEmpty(P_ADDRESS_ID))
            {
                var checkTech = new GetCheckTechnologyQuery
                {
                    P_OWNER_PRODUCT = owenerProduct,
                    P_ADDRESS_ID = P_ADDRESS_ID
                };

                var modelCheckTech = _queryProcessor.Execute(checkTech);

                if (modelCheckTech != null && modelCheckTech.PRODUCT_SUBTYPE.ToSafeString() != "")
                {
                    P_PRODUCT_SUBTYPE = modelCheckTech.PRODUCT_SUBTYPE;
                }
            }
            else if (P_PRODUCT_SUBTYPE == "" && string.IsNullOrEmpty(P_ADDRESS_ID))
            {
                P_PRODUCT_SUBTYPE = "FTTH";
            }


            //owenerProduct = "AWN"; //For test
            //P_ADDRESS_ID = "20614863"; //For test

            string priceExclVatData = "";
            string serviceYearData = "";

            #region Set FMPA Flag

            string p_fmpa_flag = null;
            if (P_USE_INPUT_MOBILE_ONLINE_QUERY != "Y")
            {
                if ((subNetworkType == "POSTPAID") && (accountCategory == "Residential"))
                {
                    p_fmpa_flag = "N";

                    var fmpaDateFr = 0;
                    var fmpaDateTo = 0;
                    var fmpaPrice = 0;
                    var fmpaCfg = LovData.Where(
                        l => (!string.IsNullOrEmpty(l.Type) && l.Type == "FMPA_CONSTANT")).ToList();

                    if (fmpaCfg.Any())
                    {
                        var fmpaDateCfg = fmpaCfg.FirstOrDefault(n => n.Name == "SERVICE_BETWEEN_DATE");
                        if (fmpaDateCfg != null)
                        {
                            fmpaDateFr = fmpaDateCfg.LovValue1.ToSafeInteger();
                            fmpaDateTo = fmpaDateCfg.LovValue2.ToSafeInteger();
                        }

                        var fmpaPriceCfg = fmpaCfg.FirstOrDefault(n => n.Name == "PRICE_EXCEL_VAT");
                        if (fmpaPriceCfg != null)
                        {
                            fmpaPrice = fmpaPriceCfg.LovValue1.ToSafeInteger();
                        }
                    }

                    var psInfoRequest = new evESQueryPersonalInformationQuery
                    {
                        mobileNo = AisAirNumber.ToSafeString(),
                        option = "2",
                        FullUrl = FullUrl
                    };
                    var psInfoResult = _queryProcessor.Execute(psInfoRequest);

                    if (psInfoResult.Any())
                    {
                        decimal priceExclVat = 0;
                        decimal serviceYear = sffServiceYear.ToSafeInteger();
                        serviceYearData = sffServiceYear.ToSafeString();

                        //19.2 Set FMPA Flag By Package Main
                        var psInfoRow = psInfoResult.FirstOrDefault(item =>
                            item.productClass == "Main" &&
                            item.startDt.ToDateNotTime() <= DateTime.Now.Date &&
                            item.endDt.ToDateNotTime() >= DateTime.Now.Date);
                        if (psInfoRow != null)
                        {
                            priceExclVat = psInfoRow.priceExclVat;
                            priceExclVatData = priceExclVat.ToSafeString();
                        }
                        if ((priceExclVat >= fmpaPrice) &&
                            (serviceYear >= fmpaDateFr && serviceYear <= fmpaDateTo) &&
                            (string.IsNullOrEmpty(p_projectname)))
                        {
                            p_fmpa_flag = "Y";
                        }
                    }
                }
            }
            else
            {
                p_fmpa_flag = P_FMPA_FLAG;
            }
            #endregion Set FMPA Flag

            #region Set FMC Special Flag

            string p_fmc_special_flag = null;
            if (P_USE_INPUT_MOBILE_ONLINE_QUERY != "Y")
            {
                if ((subNetworkType == "POSTPAID") && (accountCategory == "Residential"))
                {
                    p_fmc_special_flag = "N";

                    var fmcSpecialDateFr = 0;
                    var fmcSpecialDateTo = 0;
                    var fmcSpecialPrice = 0;
                    var fmcSpecialCfg = LovData.Where(
                        l => (!string.IsNullOrEmpty(l.Type) && l.Type == "FMC_SPECIAL_CONSTANT")).ToList();

                    if (fmcSpecialCfg.Any())
                    {
                        var fmcSpecialDateCfg = fmcSpecialCfg.FirstOrDefault(n => n.Name == "SERVICE_BETWEEN_DATE");
                        if (fmcSpecialDateCfg != null)
                        {
                            fmcSpecialDateFr = fmcSpecialDateCfg.LovValue1.ToSafeInteger();
                            fmcSpecialDateTo = fmcSpecialDateCfg.LovValue2.ToSafeInteger();
                        }

                        var fmcSpecialPriceCfg = fmcSpecialCfg.FirstOrDefault(n => n.Name == "PRICE_EXCLUDE_VAT");
                        if (fmcSpecialPriceCfg != null)
                        {
                            fmcSpecialPrice = fmcSpecialPriceCfg.LovValue1.ToSafeInteger();
                        }
                    }

                    var psInfoRequest = new evESQueryPersonalInformationQuery
                    {
                        mobileNo = AisAirNumber.ToSafeString(),
                        option = "2",
                        FullUrl = FullUrl
                    };
                    var psInfoResult = _queryProcessor.Execute(psInfoRequest);

                    if (psInfoResult.Any())
                    {
                        decimal priceExclVat = 0;
                        decimal serviceYear = sffServiceYear.ToSafeInteger();
                        serviceYearData = sffServiceYear.ToSafeString();

                        //19.2 Set FMPA Flag By Package Main
                        var psInfoRow = psInfoResult.FirstOrDefault(item =>
                            item.productClass == "Main" &&
                            item.startDt.ToDateNotTime() <= DateTime.Now.Date &&
                            item.endDt.ToDateNotTime() >= DateTime.Now.Date);
                        if (psInfoRow != null)
                        {
                            priceExclVat = psInfoRow.priceExclVat;
                            priceExclVatData = priceExclVat.ToSafeString();
                        }
                        if ((priceExclVat >= fmcSpecialPrice) &&
                            (serviceYear >= fmcSpecialDateFr && serviceYear <= fmcSpecialDateTo) &&
                            (string.IsNullOrEmpty(p_projectname)))
                        {
                            p_fmc_special_flag = "Y";
                        }
                    }
                }
            }
            else
            {
                p_fmc_special_flag = P_FMC_SPECIAL_FLAG;
            }
            #endregion Set FMC Special Flag

            #region Set Existing_Mobile - P_EXISTING_MOBILE_FLAG
            var Existing_Mobile = "";
            if (P_USE_INPUT_MOBILE_ONLINE_QUERY != "Y")
            {
                if (subNetworkType == "POSTPAID" && (string.IsNullOrEmpty(p_projectname)))
                {
                    if (priceExclVatData != null && priceExclVatData != "")
                    {
                        decimal fmcFr = 0;
                        decimal fmcTo = 0;
                        decimal fmcPrice = 0;
                        var dataCheckPrice = base.LovData.FirstOrDefault(l => !string.IsNullOrEmpty(l.Name) && l.Name.Equals("FMC_CHECK_PRICE") && !string.IsNullOrEmpty(l.Type) && l.Type.Equals("FBB_CONSTANT"));
                        if (dataCheckPrice != null)
                        {
                            fmcFr = dataCheckPrice.LovValue1.ToSafeDecimal();
                            fmcTo = dataCheckPrice.LovValue2.ToSafeDecimal();
                            fmcPrice = priceExclVatData.ToSafeDecimal();

                            if (fmcFr <= fmcPrice && fmcPrice < fmcTo)
                            {
                                Existing_Mobile = "N";
                            }
                            else
                            {
                                Existing_Mobile = "Y";
                            }
                        }
                    }
                }
            }
            else
            {
                Existing_Mobile = P_EXISTING_MOBILE_FLAG;
            }
            #endregion Set Existing_Mobile

            #region Set CVM_ Flag
            // Set CVM_flag
            string CVM_flag = "N";
            if (P_USE_INPUT_MOBILE_ONLINE_QUERY != "Y")
            {
                if ((subNetworkType == "PREPAID" || subNetworkType == "POSTPAID") && P_LOCATION_CODE != "")
                {
                    CheckPrivilegeQueryModel resultData = CheckPrivilege(AisAirNumber);

                    if (resultData.HttpStatus.ToSafeString() == "200" && resultData.Status.ToSafeString() == "20000")
                    {
                        CVM_flag = "Y";
                    }

                    if (CVM_flag != "Y" && subNetworkType == "POSTPAID" && resultData.Status.ToSafeString() != "E:04305")
                    {
                        if (sffServiceYear != "")
                        {
                            float CVMServiceYear = 0;
                            float CVMArpu = 0;
                            float CVMServiceYearConfig = 0;
                            float CVMArpuConfig = 0;

                            string CVMServiceYearStr = base.LovData.FirstOrDefault(l => !string.IsNullOrEmpty(l.Name) && l.Name.Equals("CVM_SERVICE_YEAR")).LovValue1.ToSafeString();
                            string CVMArpuStr = base.LovData.FirstOrDefault(l => !string.IsNullOrEmpty(l.Name) && l.Name.Equals("CVM_ARPU")).LovValue1.ToSafeString();
                            if (CVMServiceYearStr != "" && CVMArpuStr != "")
                            {
                                var queryGetMobileDataUsage = new GetMobileDataUsageQuery
                                {
                                    MOBILE_NO = AisAirNumber,
                                    networkType = subNetworkType
                                };
                                MobileDataUsageModel DataUsageModel = null;
                                try { DataUsageModel = _queryProcessor.Execute(queryGetMobileDataUsage); } catch { }
                                if (DataUsageModel != null && DataUsageModel.INVOICE_AMOUNT_AVG != null && DataUsageModel.INVOICE_AMOUNT_AVG != "")
                                {
                                    if (float.TryParse(sffServiceYear, out CVMServiceYear) && float.TryParse(DataUsageModel.INVOICE_AMOUNT_AVG, out CVMArpu) && float.TryParse(CVMServiceYearStr, out CVMServiceYearConfig) && float.TryParse(CVMArpuStr, out CVMArpuConfig))
                                    {
                                        if (CVMServiceYear > CVMServiceYearConfig && CVMArpu >= CVMArpuConfig)
                                        {
                                            CVM_flag = "Y";
                                        }
                                    }
                                }
                            }
                        }

                    }

                    // Check CMV2
                    if (CVM_flag != "Y")
                    {
                        CheckPrivilegeQueryModel checkPrivilegeDataCVM2 = CheckPrivilege(AisAirNumber, "checkPrivilege_2");
                        if (checkPrivilegeDataCVM2.HttpStatus.ToSafeString() == "200" && checkPrivilegeDataCVM2.Status.ToSafeString() == "20000")
                        {
                            CVM_flag = "C";
                        }
                    }
                }
            }
            else
            {
                CVM_flag = P_CVM_FLAG;
            }
            #endregion Set CVM_ Flag

            GetListPackageByServiceV2Query getListPackageV2 = new GetListPackageByServiceV2Query
            {
                SessionId = cookieSessionID,
                TransactionID = transactionId,
                FullUrl = FullUrl,
                P_OWNER_PRODUCT = owenerProduct,
                P_PRODUCT_SUBTYPE = P_PRODUCT_SUBTYPE,
                P_PACKAGE_FOR = P_PACKAGE_FOR,
                P_Location_Code = P_LOCATION_CODE,
                P_ASC_CODE = P_ASC_CODE,
                P_Region = P_REGION,
                P_Province = P_PROVINCE,
                P_District = P_DISTRICT,
                P_Sub_District = P_SUB_DISTRICT,
                P_Partner_Type = P_PARTNER_TYPE,
                P_Partner_SubType = P_PARTNER_SUBTYPE,
                P_Serenade_Flag = P_SERENADE_FLAG,
                P_Customer_Type = P_CUSTOMER_TYPE,
                P_Address_Id = P_ADDRESS_ID,
                P_Customer_subtype = P_CUSTOMER_SUBTYPE,
                P_FMPA_Flag = p_fmpa_flag,
                P_EMPLOYEE_ID = P_STAFF_ID,
                P_CVM_FLAG = CVM_flag,
                P_SALE_CHANNEL = P_SALE_CHANNEL,
                P_SFF_PROMOTION_CODE = "",
                P_MOBILE_PRICE = priceExclVatData,
                P_EXISTING_MOBILE = Existing_Mobile,
                P_FMC_SPECIAL_FLAG = p_fmc_special_flag,
                P_NON_RES_FLAG = P_NON_RES_FLAG,
                P_DistChn = P_DistChn,
                P_ChnSales = P_ChnSales,
                P_OperatorClass = P_OperatorClass,
                P_LocationProvince = P_LocationProvince,
                P_MOU_FLAG = P_MOU_FLAG //R21.10 MOU
            };

            List<PackageGroupV2Model> packageGroupList = new List<PackageGroupV2Model>();
            List<PackageV2Model> packageV2List = new List<PackageV2Model>();
            List<PackageV2Model> packageMainList = new List<PackageV2Model>();
            List<PackageV2Model> packageInstallList = new List<PackageV2Model>();
            List<PackageV2Model> packageInstallList2 = new List<PackageV2Model>();
            List<PackageV2Model> packageAutoList = new List<PackageV2Model>();
            List<PackageV2Model> packageDiscountList = new List<PackageV2Model>();
            List<PackageV2Model> packagEntryFeeList = new List<PackageV2Model>();
            List<PackageV2Model> packageSuperMESHWifiList = new List<PackageV2Model>();
            List<PackageV2Model> packageAutoSuperMESHWifiList = new List<PackageV2Model>();//R21.11 ATV
            List<PackageV2Model> packagePlayboxList = new List<PackageV2Model>();
            List<PackageV2Model> packagePlayboxMonthlyFeeList = new List<PackageV2Model>();
            List<PackageV2Model> packageAutoContentPlayboxList = new List<PackageV2Model>();//R21.11 ATV
            List<PackageV2Model> packageContentPlayboxList = new List<PackageV2Model>();
            List<PackageV2Model> packageFixlineList = new List<PackageV2Model>();
            List<PackageV2Model> packageFixlineInstallList = new List<PackageV2Model>();
            List<PackageV2Model> packagWiFiLogList = new List<PackageV2Model>();
            List<PackageV2Model> packageSpeedBoostList = new List<PackageV2Model>();
            List<PackageV2Model> packageAutoSuperMESHWifiListMeshArpu = new List<PackageV2Model>();//mesh Arpu
            List<PackageV2Model> packageIpCameraList = new List<PackageV2Model>(); //R23.06 IP Camera (Ontop IP Camera)
            List<PackageV2Model> packageAutoIpCameraList = new List<PackageV2Model>(); //R23.08 IP Camera (Order Fee)
            PackageDataV2ToViewModel packageDataV2ToViewModel = new PackageDataV2ToViewModel();
            string[] PRODUCT_SUBTYPE_ARRY = new string[4];
            PRODUCT_SUBTYPE_ARRY[0] = "WireBB";
            PRODUCT_SUBTYPE_ARRY[1] = "FTTx";
            PRODUCT_SUBTYPE_ARRY[2] = "FTTR";
            PRODUCT_SUBTYPE_ARRY[3] = "WTTx";

            PackageDataV2Model packageDataV2 = _queryProcessor.Execute(getListPackageV2);
            if (packageDataV2 != null && packageDataV2.PackageGroupList != null && packageDataV2.PackageGroupList.Count > 0)
            {
                packageGroupList = packageDataV2.PackageGroupList;
            }

            if (packageDataV2 != null && packageDataV2.PackageList != null && packageDataV2.PackageList.Count > 0)
            {
                packageV2List = packageDataV2.PackageList;
                packageMainList = packageV2List.Where(t => t.PACKAGE_TYPE == "1").ToList();//R21.11 ATV

                if (owenerProduct != "WTTx")
                {
                    packageInstallList = packageV2List.Where(t => t.PACKAGE_TYPE == "2" && PRODUCT_SUBTYPE_ARRY.Contains(t.PRODUCT_SUBTYPE) && !String.IsNullOrEmpty(t.AUTO_MAPPING_PROMOTION_CODE)).OrderBy(t => t.SFF_PROMOTION_CODE).ToList();
                    packageInstallList2 = packageV2List.Where(t => t.PACKAGE_TYPE == "2" && PRODUCT_SUBTYPE_ARRY.Contains(t.PRODUCT_SUBTYPE) && String.IsNullOrEmpty(t.AUTO_MAPPING_PROMOTION_CODE)).ToList();
                }
                else
                {
                    packageInstallList = packageV2List.Where(t => t.PACKAGE_TYPE == "2" && PRODUCT_SUBTYPE_ARRY.Contains(t.PRODUCT_SUBTYPE) && String.IsNullOrEmpty(t.AUTO_MAPPING_PROMOTION_CODE)).ToList();
                    packageInstallList2 = packageV2List.Where(t => t.PACKAGE_TYPE == "2" && PRODUCT_SUBTYPE_ARRY.Contains(t.PRODUCT_SUBTYPE) && !String.IsNullOrEmpty(t.AUTO_MAPPING_PROMOTION_CODE)).ToList();
                }
                packageAutoList = packageV2List.Where(t => !String.IsNullOrEmpty(t.AUTO_MAPPING_PROMOTION_CODE) && t.PACKAGE_SERVICE_CODE == "00001").ToList();
                packageDiscountList = packageV2List.Where(t => t.PACKAGE_TYPE == "4" && String.IsNullOrEmpty(t.AUTO_MAPPING_PROMOTION_CODE)).ToList();
                packagEntryFeeList = packageV2List.Where(t => t.PACKAGE_TYPE == "6" && t.PACKAGE_SERVICE_CODE == "00001" && String.IsNullOrEmpty(t.AUTO_MAPPING_PROMOTION_CODE)).ToList();
                packageSuperMESHWifiList = packageV2List.Where(t => t.PACKAGE_TYPE == "13" && !String.IsNullOrEmpty(t.AUTO_MAPPING_PROMOTION_CODE) && t.PRODUCT_SUBTYPE == "MESH").ToList();
                packageAutoSuperMESHWifiList = packageV2List.Where(t => t.PACKAGE_TYPE == "6" && t.PACKAGE_SERVICE_CODE == "00006" && !String.IsNullOrEmpty(t.AUTO_MAPPING_PROMOTION_CODE) && t.PRODUCT_SUBTYPE == "MESH").ToList();//R21.11 ATV
                packageFixlineList = packageV2List.Where(t => t.PACKAGE_TYPE == "1" && t.PRODUCT_SUBTYPE == "VOIP" && String.IsNullOrEmpty(t.AUTO_MAPPING_PROMOTION_CODE)).ToList();
                packageFixlineInstallList = packageV2List.Where(t => t.PACKAGE_TYPE == "2" && t.PRODUCT_SUBTYPE == "VOIP" && String.IsNullOrEmpty(t.AUTO_MAPPING_PROMOTION_CODE)).ToList();
                packagePlayboxList = packageV2List.Where(t => t.PACKAGE_TYPE == "7" && t.PRODUCT_SUBTYPE == "PBOX" && String.IsNullOrEmpty(t.AUTO_MAPPING_PROMOTION_CODE)).ToList();
                packagePlayboxMonthlyFeeList = packageV2List.Where(t => t.PACKAGE_TYPE == "11" && t.PRODUCT_SUBTYPE == "PBOX" && String.IsNullOrEmpty(t.AUTO_MAPPING_PROMOTION_CODE)).ToList();
                packageAutoContentPlayboxList = packageV2List.Where(t => t.PACKAGE_TYPE == "8" && t.PRODUCT_SUBTYPE == "PBOX" && t.PACKAGE_SERVICE_CODE == "00002" && !(String.IsNullOrEmpty(t.AUTO_MAPPING_PROMOTION_CODE))).ToList();//R21.11 ATV
                packageContentPlayboxList = packageV2List.Where(t => t.PACKAGE_TYPE == "8" && t.PRODUCT_SUBTYPE == "PBOX" && t.PACKAGE_SERVICE_CODE == "00001" && String.IsNullOrEmpty(t.AUTO_MAPPING_PROMOTION_CODE)).ToList();
                packagWiFiLogList = packageV2List.Where(t => t.PACKAGE_TYPE == "12" && String.IsNullOrEmpty(t.AUTO_MAPPING_PROMOTION_CODE)).ToList();
                packageSpeedBoostList = packageV2List.Where(t => t.PACKAGE_TYPE == "9" && String.IsNullOrEmpty(t.AUTO_MAPPING_PROMOTION_CODE)).ToList();
                packageAutoSuperMESHWifiListMeshArpu = packageV2List.Where(t => t.PACKAGE_TYPE == "6" && t.PACKAGE_SERVICE_CODE == "00006" && String.IsNullOrEmpty(t.AUTO_MAPPING_PROMOTION_CODE)).ToList();//mesh arpu
                packageIpCameraList = packageV2List.Where(t => t.PACKAGE_TYPE == "14" && t.PACKAGE_SERVICE_CODE == "00007" && String.IsNullOrEmpty(t.AUTO_MAPPING_PROMOTION_CODE) && t.PRODUCT_SUBTYPE == "IP_CAMERA").ToList(); //R23.08 IP Camera (Ontop IP Camera)
                packageAutoIpCameraList = packageV2List.Where(t => t.PACKAGE_TYPE == "6" && t.PACKAGE_SERVICE_CODE == "00007" && !String.IsNullOrEmpty(t.AUTO_MAPPING_PROMOTION_CODE) && t.PRODUCT_SUBTYPE == "IP_CAMERA").ToList(); //R23.08 IP Camera (Order Fee)

                packageDataV2ToViewModel.packageMainList = packageMainList;//R21.11 ATV
                packageDataV2ToViewModel.packageInstallList = packageInstallList;
                packageDataV2ToViewModel.packageInstallList2 = packageInstallList2;
                packageDataV2ToViewModel.packageAutoList = packageAutoList;
                packageDataV2ToViewModel.packageDiscountList = packageDiscountList;
                packageDataV2ToViewModel.packagEntryFeeList = packagEntryFeeList;
                packageDataV2ToViewModel.packageSuperMESHWifiList = packageSuperMESHWifiList;
                packageDataV2ToViewModel.packageAutoSuperMESHWifiList = packageAutoSuperMESHWifiList;//R21.11 ATV
                packageDataV2ToViewModel.packagePlayboxList = packagePlayboxList;
                packageDataV2ToViewModel.packagePlayboxMonthlyFeeList = packagePlayboxMonthlyFeeList;
                packageDataV2ToViewModel.packageAutoContentPlayboxList = packageAutoContentPlayboxList;//R21.11 ATV
                packageDataV2ToViewModel.packageContentPlayboxList = packageContentPlayboxList;
                packageDataV2ToViewModel.packageFixlineList = packageFixlineList;
                packageDataV2ToViewModel.packageFixlineInstallList = packageFixlineInstallList;
                packageDataV2ToViewModel.packagWiFiLogList = packagWiFiLogList;
                packageDataV2ToViewModel.packageSpeedBoostList = packageSpeedBoostList;
                packageDataV2ToViewModel.packageIpCameraList = packageIpCameraList; //23.06 IP Camera (Ontop IP Camera)
                packageDataV2ToViewModel.packageAutoIpCameraList = packageAutoIpCameraList; //23.06 IP Camera (Order Fee)

                packageDataV2ToViewModel.owenerProduct = owenerProduct;
                packageDataV2ToViewModel.fmpa_flag = p_fmpa_flag;
                packageDataV2ToViewModel.cvm_flag = CVM_flag;
                packageDataV2ToViewModel.fmc_special_flag = p_fmc_special_flag;
                packageDataV2ToViewModel.mou_flag = P_MOU_FLAG;//R21.10 MOU
                packageDataV2ToViewModel.packageAutoSuperMESHWifiListMeshArpu = packageAutoSuperMESHWifiListMeshArpu;//mesh arpu
            }

            foreach (var PakageGroupItem in packageGroupList)
            {
                if (PakageGroupItem.PackageItems != null && PakageGroupItem.PackageItems.Count > 0)
                {
                    foreach (var tmpMainPakage in PakageGroupItem.PackageItems)
                    {
                        // Set Value Install in main Pakage
                        tmpMainPakage.INSTALL_SHOW = "N";
                        List<PackageV2Model> tmpPackageInstall = packageInstallList.Where(t => t.MAPPING_CODE == tmpMainPakage.MAPPING_CODE).ToList();
                        if (tmpPackageInstall != null && tmpPackageInstall.Count > 0)
                        {
                            tmpMainPakage.INSTALL_SHOW = "Y";
                        }
                        // Set Value EntryFee in main Pakage
                        tmpMainPakage.ENTRYFEE_SHOW = "N";
                        List<PackageV2Model> tmpPackagEntryFeeList = packagEntryFeeList.Where(t => t.MAPPING_CODE == tmpMainPakage.MAPPING_CODE).ToList();
                        if (tmpPackagEntryFeeList != null && tmpPackagEntryFeeList.Count > 0)
                        {
                            if (tmpPackagEntryFeeList.Count > 1)
                            {
                                tmpMainPakage.ENTRYFEE_SHOW = "Y";
                            }
                        }
                    }
                }
            }
            packageDataV2ToViewModel.packageGroupList = packageGroupList;
            jsonResult.Data = packageDataV2ToViewModel;
            return jsonResult;
        }

        [OutputCache(Location = OutputCacheLocation.Server, Duration = 3600)]
        public ActionResult GetListPackageService(string P_OWNER_PRODUCT = "", string P_PRODUCT_SUBTYPE = "", string P_NETWORK_TYPE = "", string P_SERVICE_DAY = "",
            string line = "", string P_PACKAGE_FOR = "", string P_PACKAGE_CODE = "", string isPartner = "", string partnerName = "", string accessMode = "",
            string interfaceId = "", string no = "", string topUp = "", string P_SFF_PROMOCODE = "", string p_location_code = "", string p_asc_code = "", string p_region = "",
            string p_province = "", string p_district = "", string p_sub_district = "", string p_address_type = "", string p_building_name = "", string p_building_no = "",
            string p_partner_type = "", string p_partner_subtype = "", string p_Serenade_Flag = "", string p_Customer_Type = "", string p_Address_Id = "", string p_InstallationType = "",
            string p_Plug_And_Play_Flag = "", string AisAirNumber = "", string p_rental_flag = "", string p_customer_subtype = "", string p_router_flag = "",
            string subNetworkType = "", string accountCategory = "", string sffServiceYear = "", string p_projectname = "", string p_mobile_segment = "", string p_staff_id = "")
        {
            string FullUrl = "";
            if (Session["FullUrl"] != null) FullUrl = Session["FullUrl"].ToSafeString();

            #region Get IP Address Interface Log (Update 17.2)

            // Get IP Address
            string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress)) ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            var httpCookie = Request.Cookies["ASP.NET_SessionId"];
            var cookieSessionID = httpCookie != null ? httpCookie.Value : Session.SessionID;

            string transactionId = AisAirNumber + ipAddress;

            #endregion

            string userAgent = "";
            if (Request.Headers["User-Agent"] != null)
            {
                var headers = Request.Headers.GetValues("User-Agent");

                StringBuilder sb = new StringBuilder();

                foreach (var header in headers)
                {
                    sb.Append(header);

                    // Re-add spaces stripped when user agent string was split up.
                    sb.Append(" ");
                }

                userAgent = sb.ToString().Trim();
            }

            var jsonResult = new JsonNetResult();
            // ส่ง access mode list ที่ได้มาจาก GetFBSSFeasibilityCheckHandler

            var accessModeModel = Newtonsoft.Json.JsonConvert.DeserializeObject<List<FBSSAccessModeInfo>>(accessMode);
            var owenerProduct = "";
            var productSubtype = P_PRODUCT_SUBTYPE;

            if (line == "2" || topUp == "1" || topUp == "4")
            {
                if (P_OWNER_PRODUCT == "")
                {
                    var q = new GetOwnerProductByNoQuery
                    {
                        No = no
                    };
                    var getCusData = _queryProcessor.Execute(q);
                    owenerProduct = getCusData.Value;
                }
                else
                    owenerProduct = P_OWNER_PRODUCT;
            }
            else
            {
                base.Logger.Info("Begin get Owner Product");

                base.Logger.Info("Prameter for list owner \r\n IsPartner:" + isPartner + "\r\n PartnerName:" + partnerName + "\r\n FBSSAccessModeInfo:" + accessModeModel);
                owenerProduct = _queryProcessor.Execute(new GetMappingSbnOwnerProd
                {
                    IsPartner = isPartner,
                    PartnerName = partnerName,
                    FBSSAccessModeInfo = accessModeModel
                });

                base.Logger.Info("Result Owner :" + owenerProduct);
                base.Logger.Info("End get Owner Product");
            }

            List<PackageModel> list;

            if (line == "2" || topUp == "1" || topUp == "4")
            {

                var query = new GetPackageListBySFFPromoQuery
                {
                    P_SFF_PROMOCODE = P_SFF_PROMOCODE,
                    P_OWNER_PRODUCT = owenerProduct,
                    P_PRODUCT_SUBTYPE = productSubtype,
                    VAS_SERVICE = "Y",
                    TransactionID = transactionId
                };
                list = _queryProcessor.Execute(query);

            }
            else
            {
                string priceExclVatData = "";
                string serviceYearData = "";

                if (line == "1")
                {
                    p_location_code = "SSO";
                }//ถ้าเป็น SSO จะส่ง location code is "SSO"

                //19.1 Set FMPA Flag
                #region Set FMPA Flag

                string p_fmpa_flag = null;
                if ((subNetworkType == "POSTPAID") && (accountCategory == "Residential"))
                {
                    p_fmpa_flag = "N";

                    var fmpaDateFr = 0;
                    var fmpaDateTo = 0;
                    var fmpaPrice = 0;
                    var fmpaCfg = LovData.Where(
                        l => (!string.IsNullOrEmpty(l.Type) && l.Type == "FMPA_CONSTANT")).ToList();

                    if (fmpaCfg.Any())
                    {
                        var fmpaDateCfg = fmpaCfg.FirstOrDefault(n => n.Name == "SERVICE_BETWEEN_DATE");
                        if (fmpaDateCfg != null)
                        {
                            fmpaDateFr = fmpaDateCfg.LovValue1.ToSafeInteger();
                            fmpaDateTo = fmpaDateCfg.LovValue2.ToSafeInteger();
                        }

                        var fmpaPriceCfg = fmpaCfg.FirstOrDefault(n => n.Name == "PRICE_EXCEL_VAT");
                        if (fmpaPriceCfg != null)
                        {
                            fmpaPrice = fmpaPriceCfg.LovValue1.ToSafeInteger();
                        }
                    }

                    var psInfoRequest = new evESQueryPersonalInformationQuery
                    {
                        mobileNo = AisAirNumber.ToSafeString(),
                        option = "2",
                        FullUrl = FullUrl
                    };
                    var psInfoResult = _queryProcessor.Execute(psInfoRequest);

                    if (psInfoResult.Any())
                    {
                        decimal priceExclVat = 0;
                        decimal serviceYear = sffServiceYear.ToSafeInteger();
                        serviceYearData = sffServiceYear.ToSafeString();

                        //19.2 Set FMPA Flag By Package Main
                        var psInfoRow = psInfoResult.FirstOrDefault(item =>
                            item.productClass == "Main" &&
                            item.startDt.ToDateNotTime() <= DateTime.Now.Date &&
                            item.endDt.ToDateNotTime() >= DateTime.Now.Date);
                        if (psInfoRow != null)
                        {
                            priceExclVat = psInfoRow.priceExclVat;
                            priceExclVatData = priceExclVat.ToSafeString();
                        }
                        if ((priceExclVat >= fmpaPrice) &&
                            (serviceYear >= fmpaDateFr && serviceYear <= fmpaDateTo) &&
                            (string.IsNullOrEmpty(p_projectname)))
                        {
                            p_fmpa_flag = "Y";
                        }
                    }
                }

                #endregion Set FMPA Flag

                // Set FMC 
                //19.3
                var Existing_Mobile = "";
                if (subNetworkType == "POSTPAID" && (string.IsNullOrEmpty(p_projectname)))
                {
                    if (priceExclVatData != null && priceExclVatData != "")
                    {
                        decimal fmcFr = 0;
                        decimal fmcTo = 0;
                        decimal fmcPrice = 0;
                        var dataCheckPrice = base.LovData.FirstOrDefault(l => !string.IsNullOrEmpty(l.Name) && l.Name.Equals("FMC_CHECK_PRICE") && !string.IsNullOrEmpty(l.Type) && l.Type.Equals("FBB_CONSTANT"));
                        if (dataCheckPrice != null)
                        {
                            fmcFr = dataCheckPrice.LovValue1.ToSafeDecimal();
                            fmcTo = dataCheckPrice.LovValue2.ToSafeDecimal();
                            fmcPrice = priceExclVatData.ToSafeDecimal();

                            if (fmcFr <= fmcPrice && fmcPrice < fmcTo)
                            {
                                Existing_Mobile = "N";
                            }
                            else
                            {
                                Existing_Mobile = "Y";
                            }
                        }
                    }
                }
                #region Set CVM_ Flag
                // Set CVM_flag
                string CVM_flag = "N";

                CheckPrivilegeQueryModel resultData = CheckPrivilege(AisAirNumber);

                if (resultData.HttpStatus.ToSafeString() == "200" && resultData.Status.ToSafeString() == "20000")
                {
                    CVM_flag = "Y";
                }

                if (CVM_flag != "Y" && subNetworkType == "POSTPAID" && resultData.Status.ToSafeString() != "E:04354")
                {
                    if (sffServiceYear != "")
                    {
                        float CVMServiceYear = 0;
                        float CVMArpu = 0;
                        float CVMServiceYearConfig = 0;
                        float CVMArpuConfig = 0;

                        string CVMServiceYearStr = base.LovData.FirstOrDefault(l => !string.IsNullOrEmpty(l.Name) && l.Name.Equals("CVM_SERVICE_YEAR")).LovValue1.ToSafeString();
                        string CVMArpuStr = base.LovData.FirstOrDefault(l => !string.IsNullOrEmpty(l.Name) && l.Name.Equals("CVM_ARPU")).LovValue1.ToSafeString();
                        if (CVMServiceYearStr != "" && CVMArpuStr != "")
                        {
                            var queryGetMobileDataUsage = new GetMobileDataUsageQuery
                            {
                                MOBILE_NO = AisAirNumber,
                                networkType = subNetworkType
                            };
                            MobileDataUsageModel DataUsageModel = null;
                            try { DataUsageModel = _queryProcessor.Execute(queryGetMobileDataUsage); } catch { }
                            if (DataUsageModel != null && DataUsageModel.INVOICE_AMOUNT_AVG != null && DataUsageModel.INVOICE_AMOUNT_AVG != "")
                            {
                                if (float.TryParse(sffServiceYear, out CVMServiceYear) && float.TryParse(DataUsageModel.INVOICE_AMOUNT_AVG, out CVMArpu) && float.TryParse(CVMServiceYearStr, out CVMServiceYearConfig) && float.TryParse(CVMArpuStr, out CVMArpuConfig))
                                {
                                    if (CVMServiceYear > CVMServiceYearConfig && CVMArpu >= CVMArpuConfig)
                                    {
                                        CVM_flag = "Y";
                                    }
                                }
                            }
                        }
                    }

                }

                #endregion Set CVM_ Flag

                var query = new GetListPackageByServiceQuery
                {
                    P_OWNER_PRODUCT = owenerProduct, //P_OWNER_PRODUCT แทนที่ด้วย access mode mapping,
                    P_PRODUCT_SUBTYPE = productSubtype,
                    P_NETWORK_TYPE = P_NETWORK_TYPE,
                    P_SERVICE_DAY = P_SERVICE_DAY,
                    P_PACKAGE_FOR = P_PACKAGE_FOR,
                    P_PACKAGE_CODE = P_PACKAGE_CODE,
                    TransactionID = transactionId,
                    P_Location_Code = p_location_code,
                    P_Asc_Code = p_asc_code,
                    P_Region = p_region,
                    P_Province = p_province,
                    P_District = p_district,
                    P_Sub_District = p_sub_district,
                    P_Address_Type = p_address_type,
                    P_Building_Name = p_building_name,
                    P_Building_No = p_building_no,
                    P_Partner_Type = p_partner_type,
                    P_Partner_SubType = p_partner_subtype,
                    P_Serenade_Flag = p_Serenade_Flag,
                    P_Customer_Type = p_Customer_Type,
                    P_Address_Id = p_Address_Id,
                    P_Plug_And_Play_Flag = p_Plug_And_Play_Flag,
                    FullUrl = FullUrl,
                    P_Rental_Flag = p_rental_flag,
                    P_Customer_subtype = p_customer_subtype,
                    P_Router_Flag = p_router_flag,
                    P_FMPA_Flag = p_fmpa_flag,
                    P_Mobile_Price = priceExclVatData,
                    P_Service_Year = serviceYearData,
                    P_Existing_Mobile = Existing_Mobile,
                    P_Mobile_No = AisAirNumber,
                    P_EMPLOYEE_ID = p_staff_id,
                    P_CVM_FLAG = CVM_flag,
                    SessionId = cookieSessionID
                };
                list = _queryProcessor.Execute(query);
            }

            string ShowInstall = "";
            string TecnologyType = "";
            var listAuto = new List<PackageModel>();

            base.Logger.Info("GetListPackageService KeepAlive Log " + userAgent + " : After GetListPackageByServiceQuery Success");

            if (list != null && list.Count > 0)
            {
                listAuto = list.Where(p => p.PACKAGE_TYPE != "Main" && p.AUTO_MAPPING == "A").ToList();
                list = list.Where(p => p.AUTO_MAPPING != "A" || p.PACKAGE_TYPE == "Main").ToList();
                var tecnology = list.Select(p => new { p.PRODUCT_SUBTYPE, p.PACKAGE_TYPE })
                    .Where(
                        p =>
                            p.PACKAGE_TYPE == "Main" && p.PRODUCT_SUBTYPE != "VOIP" &&
                            p.PRODUCT_SUBTYPE != "PBOX")
                    .FirstOrDefault();

                if (tecnology != null)
                {
                    TecnologyType = tecnology.PRODUCT_SUBTYPE;
                }
            }

            base.Logger.Info("GetListPackageService KeepAlive Log " + userAgent + " : Checked list null");

            var LovConfig = GetLovConfigOptionInstallData();
            var LovConfigShowOptionInstall = GetLovConfigShowOptionInstall();
            string[] LovConfigShowOptionInstalldata;
            if (LovConfigShowOptionInstall != null && LovConfigShowOptionInstall.Count > 0)
            {
                base.Logger.Info("GetListPackageService KeepAlive Log " + userAgent + " : checked LovConfigShowOptionInstall");
                LovConfigShowOptionInstalldata = new string[LovConfigShowOptionInstall.Count()];
                int LovConfigShowOptionInstallDataCount = 0;
                base.Logger.Info("GetListPackageService KeepAlive Log " + userAgent + " : Let's Loop this LovConfigShowOptionInstall");
                foreach (var item in LovConfigShowOptionInstall)
                {
                    LovConfigShowOptionInstalldata[LovConfigShowOptionInstallDataCount] = item.LovValue1;
                    LovConfigShowOptionInstallDataCount++;
                }
                base.Logger.Info("GetListPackageService KeepAlive Log " + userAgent + " : finished Loop add data to LovConfigShowOptionInstalldata");
            }
            else
            {
                base.Logger.Info("GetListPackageService KeepAlive Log " + userAgent + " : don't have LovConfigShowOptionInstall");
                LovConfigShowOptionInstalldata = new string[1];
            }

            base.Logger.Info("GetListPackageService KeepAlive Log " + userAgent + " : Got LOV");

            if (LovConfig != null && LovConfig.Count > 0)
            {
                base.Logger.Info("GetListPackageService KeepAlive Log " + userAgent + " : checked LOV");
                if (p_InstallationType == "0")
                {
                    base.Logger.Info("GetListPackageService KeepAlive Log " + userAgent + " : p_InstallationType == 0");
                    var LovConfigStr = LovConfig.Where(p => p.Text == TecnologyType && p.DefaultValue == "1").ToList();
                    if (LovConfigStr != null && LovConfigStr.Count > 0)
                    {
                        base.Logger.Info("GetListPackageService KeepAlive Log " + userAgent + " : Have LovConfigStr");
                        var MappingCodeInGroup = list.Where(p => LovConfigShowOptionInstalldata.Contains(p.PACKAGE_GROUP)).Select(p => p.MAPPING_CODE).ToList();
                        var LovConfigStrTmp = LovConfigStr.FirstOrDefault();
                        if (LovConfigStrTmp != null)
                        {
                            base.Logger.Info("GetListPackageService KeepAlive Log " + userAgent + " : Have LovConfigStrTmp");
                            string[] codesarray = new string[4];
                            codesarray[0] = LovConfigStrTmp.LovValue1.ToSafeString() != "" ? LovConfigStrTmp.LovValue1.ToSafeString() : "Pxxx1";
                            codesarray[1] = LovConfigStrTmp.LovValue2.ToSafeString() != "" ? LovConfigStrTmp.LovValue2.ToSafeString() : "Pxxx2";
                            codesarray[2] = LovConfigStrTmp.LovValue3.ToSafeString() != "" ? LovConfigStrTmp.LovValue3.ToSafeString() : "Pxxx3";
                            codesarray[3] = LovConfigStrTmp.LovValue4.ToSafeString() != "" ? LovConfigStrTmp.LovValue4.ToSafeString() : "Pxxx4";

                            if (LovConfigStrTmp.LovValue1 != "" && LovConfigStrTmp.LovValue2 != "")
                            {
                                base.Logger.Info("GetListPackageService KeepAlive Log " + userAgent + " : Have LovConfigStrTmp LovValue1 and LovValue2");
                                list = list.Where(p => !(codesarray.Contains(p.SFF_PROMOTION_CODE) && !MappingCodeInGroup.Contains(p.MAPPING_CODE))).ToList();
                                ShowInstall = "True";
                            }
                        }
                    }
                }
                else
                {
                    base.Logger.Info("GetListPackageService KeepAlive Log " + userAgent + " : p_InstallationType != 0");
                    var LovConfigStr = LovConfig.Where(p => p.Text == TecnologyType && p.DefaultValue == "0");
                    if (LovConfigStr != null)
                    {
                        base.Logger.Info("GetListPackageService KeepAlive Log " + userAgent + " : Have LovConfigStr");
                        var MappingCodeInGroup = list.Where(p => LovConfigShowOptionInstalldata.Contains(p.PACKAGE_GROUP)).Select(p => p.MAPPING_CODE).ToList();
                        var LovConfigStrTmp = LovConfigStr.FirstOrDefault();
                        if (LovConfigStrTmp != null)
                        {
                            base.Logger.Info("GetListPackageService KeepAlive Log " + userAgent + " : Have LovConfigStrTmp");
                            string codes = LovConfigStrTmp.LovValue1 + "," + LovConfigStrTmp.LovValue2;
                            string[] codesarray = new string[4];
                            codesarray[0] = LovConfigStrTmp.LovValue1.ToSafeString() != "" ? LovConfigStrTmp.LovValue1.ToSafeString() : "Pxxx1";
                            codesarray[1] = LovConfigStrTmp.LovValue2.ToSafeString() != "" ? LovConfigStrTmp.LovValue2.ToSafeString() : "Pxxx2";
                            codesarray[2] = LovConfigStrTmp.LovValue3.ToSafeString() != "" ? LovConfigStrTmp.LovValue3.ToSafeString() : "Pxxx3";
                            codesarray[3] = LovConfigStrTmp.LovValue4.ToSafeString() != "" ? LovConfigStrTmp.LovValue4.ToSafeString() : "Pxxx4";

                            if (LovConfigStrTmp.LovValue1 != "" && LovConfigStrTmp.LovValue2 != "")
                            {
                                base.Logger.Info("GetListPackageService KeepAlive Log " + userAgent + " : Have LovConfigStrTmp LovValue1 and LovValue2");
                                list = list.Where(p => !(codesarray.Contains(p.SFF_PROMOTION_CODE) && !MappingCodeInGroup.Contains(p.MAPPING_CODE))).ToList();
                                ShowInstall = "True";
                            }
                        }
                    }
                }
            }

            base.Logger.Info("GetListPackageService KeepAlive Log " + userAgent + " : Finish LOV");
            if (!list.Any())
            {
                base.Logger.Info("Package Model Is Null  " + userAgent);
                jsonResult.Formatting = Newtonsoft.Json.Formatting.Indented;
                jsonResult.Data = new List<PackageModel>();

                return jsonResult;
            }

            base.Logger.Info("GetListPackageService KeepAlive Log " + userAgent + " : Checked list again with !list.Any");

            if (line == "" || line == "3" || line == "6")
            {
                base.Logger.Info("GetListPackageService KeepAlive Log " + userAgent + " : Useless condition because comment code (line = empytstring | = 3 | = 6)");
                // list = list.Where(p => p.PACKAGE_TYPE != "Bundle" && !p.PACKAGE_GROUP.Contains("Bundle")).ToList();
            }
            else
            {
                base.Logger.Info("GetListPackageService KeepAlive Log " + userAgent + " : check line (not emptystring nor 3 nor 6");
                int componentBundle = 0;

                if (base.CurrentUser != null && base.CurrentUser.ComponentModel != null && base.CurrentUser.Groups != null)
                    componentBundle = base.CurrentUser.ComponentModel.Where(w => w.ComponentName == "FBBOR002_REGISTER_BUNDLE"
                    && base.CurrentUser.Groups.Contains(w.GroupID)).Count();

                if (componentBundle == 0)
                    list = list.Where(p => p.PACKAGE_TYPE != "Bundle" && !p.PACKAGE_GROUP.Contains("Bundle")).ToList();
            }
            base.Logger.Info("GetListPackageService KeepAlive Log " + userAgent + " : After Line check for bundle package");

            var listGroup = new List<SGroup>();

            var top = list.Where(p => p.PACKAGE_GROUP != null && p.PACKAGE_GROUP != "" && !p.PACKAGE_GROUP.Contains("Incredible")
                && p.PRODUCT_SUBTYPE != "VOIP" && p.PRODUCT_SUBTYPE != "Equipment" && p.PACKAGE_TYPE != "Ontop" && p.PRODUCT_SUBTYPE != "PBOX"
                && p.PACKAGE_TYPE != "Ontop Special")
                .GroupBy(g => new { g.PACKAGE_GROUP, g.PRODUCT_SUBTYPE, g.PACKAGE_TYPE }).Select(s => new { s.Key.PACKAGE_GROUP, s.Key.PRODUCT_SUBTYPE, s.Key.PACKAGE_TYPE });

            var bottom = list.Where(p => p.PACKAGE_GROUP != null && p.PACKAGE_GROUP != "" && p.PACKAGE_GROUP.Contains("Incredible")
                && p.PRODUCT_SUBTYPE != "VOIP" && p.PRODUCT_SUBTYPE != "Equipment" && p.PACKAGE_TYPE != "Ontop" && p.PRODUCT_SUBTYPE != "PBOX"
                && p.PACKAGE_TYPE != "Ontop Special")
                .GroupBy(g => new { g.PACKAGE_GROUP, g.PRODUCT_SUBTYPE, g.PACKAGE_TYPE }).Select(s => new { s.Key.PACKAGE_GROUP, s.Key.PRODUCT_SUBTYPE, s.Key.PACKAGE_TYPE });

            base.Logger.Info("GetListPackageService KeepAlive Log " + userAgent + " : find something top? bottom?");

            if (top.Any())
                foreach (var a in top)
                    listGroup.Add(new SGroup { PACKAGE_GROUP = a.PACKAGE_GROUP, PRODUCT_SUBTYPE = a.PRODUCT_SUBTYPE, PACKAGE_TYPE = a.PACKAGE_TYPE });

            if (bottom.Any())
                foreach (var a in bottom)
                    listGroup.Add(new SGroup { PACKAGE_GROUP = a.PACKAGE_GROUP, PRODUCT_SUBTYPE = a.PRODUCT_SUBTYPE, PACKAGE_TYPE = a.PACKAGE_TYPE });

            base.Logger.Info("GetListPackageService KeepAlive Log " + userAgent + " : checked top and bottom and Add to listGroup");

            list[0].S_GROUP = listGroup;
            list[0].TEMP_IA = ShowInstall;

            base.Logger.Info("GetListPackageService KeepAlive Log " + userAgent + " : init value to list[0].S_Group and . TEMP_IA");

            if (listAuto.Any() && listAuto.Count > 0)
            {
                base.Logger.Info("GetListPackageService KeepAlive Log " + userAgent + " : checked listAuto to call PackageAuto");
                List<PackageAutoModel> packageAutoDatas = new List<PackageAutoModel>();
                foreach (var item in listAuto)
                {
                    PackageAutoModel packageAutoData = new PackageAutoModel
                    {
                        ACCESS_MODE = item.ACCESS_MODE,
                        DISCOUNT_INITIATION_CHARGE = item.DISCOUNT_INITIATION_CHARGE,
                        DOWNLOAD_SPEED = item.DOWNLOAD_SPEED,
                        INITIATION_CHARGE = item.INITIATION_CHARGE,
                        MAPPING_CODE = item.MAPPING_CODE,
                        MAPPING_PRODUCT = item.MAPPING_PRODUCT,
                        OWNER_PRODUCT = item.OWNER_PRODUCT,
                        PACKAGE_CODE = item.PACKAGE_CODE,
                        PACKAGE_GROUP = item.PACKAGE_GROUP,
                        PACKAGE_NAME = item.PACKAGE_NAME,
                        PACKAGE_TYPE = item.PACKAGE_TYPE,
                        PRE_INITIATION_CHARGE = item.PRE_INITIATION_CHARGE,
                        PRE_RECURRING_CHARGE = item.PRE_RECURRING_CHARGE,
                        PRODUCT_SUBTYPE = item.PRODUCT_SUBTYPE,
                        PRODUCT_SUBTYPE3 = item.PRODUCT_SUBTYPE3,
                        RECURRING_CHARGE = item.RECURRING_CHARGE,
                        SERVICE_CODE = item.SERVICE_CODE,
                        SFF_PROMOTION_BILL_ENG = item.SFF_PROMOTION_BILL_ENG,
                        SFF_PROMOTION_BILL_THA = item.SFF_PROMOTION_BILL_THA,
                        SFF_PROMOTION_CODE = item.SFF_PROMOTION_CODE,
                        TECHNOLOGY = item.TECHNOLOGY,
                        UPLOAD_SPEED = item.UPLOAD_SPEED,
                        AUTO_MAPPING = item.AUTO_MAPPING,
                        DISPLAY_FLAG = item.DISPLAY_FLAG,
                        DISPLAY_SEQ = item.DISPLAY_SEQ,
                        DISCOUNT_TYPE = item.DISCOUNT_TYPE,
                        DISCOUNT_VALUE = item.DISCOUNT_VALUE,
                        DISCOUNT_DAY = item.DISCOUNT_DAY

                    };
                    packageAutoDatas.Add(packageAutoData);

                }
                base.Logger.Info("GetListPackageService KeepAlive Log " + userAgent + " : Finished Loop call PackageAuto");
                list[0].PACKAGE_AUTO = packageAutoDatas;
                base.Logger.Info("GetListPackageService KeepAlive Log " + userAgent + " : init value in list[0].PACKAGE_AUTO with data from PackageAuto");
            }
            base.Logger.Info("GetListPackageService KeepAlive Log " + userAgent + " : Finished All func");
            //}

            //TimeSpan ts = stopWatch.Elapsed;
            //string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
            ////Console.Out.WriteLine("GetListPackageService elasped time is " + elapsedTime);

            jsonResult.Formatting = Newtonsoft.Json.Formatting.Indented;
            base.Logger.Info("GetListPackageService KeepAlive Log " + userAgent + " : Formatting jsonResult");

            // Test Data Sell Router


            /////////////////////////

            jsonResult.Data = topUp == "4" ? ValidatePackageModel(list) : list;
            base.Logger.Info("GetListPackageService KeepAlive Log " + userAgent + " : End KeepAlive Log");

            return jsonResult;
        }

        [OutputCache(Location = OutputCacheLocation.Server, Duration = 3600)]
        public ActionResult GetListPackageSellRounter(string logID = "", string OptionInstallRouter = "")
        {
            string FullUrl = "";
            if (Session["FullUrl"] != null)
                FullUrl = Session["FullUrl"].ToSafeString();

            #region Get IP Address Interface Log (Update 17.2)

            // Get IP Address
            string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress)) ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            string transactionId = logID + ipAddress;

            #endregion

            string TecnologyType = "";
            var jsonResult = new JsonNetResult();
            List<ListPackageSellRouterModel> list = new List<ListPackageSellRouterModel>();
            string MappingProject = "";
            string MappingValue = "";

            if (OptionInstallRouter == "5")
            {
                MappingProject = "NO_ROUTER";
                MappingValue = "SALE_ROUTER";
            }
            if (OptionInstallRouter == "3")
            {
                MappingProject = "NO_ROUTER";
                MappingValue = "BYOD";
            }

            var query = new GetListPackageSellRouterQuery()
            {
                P_MAPPING_PROJECT = MappingProject,
                P_MAPPING_VALUE = MappingValue,
                TransactionID = transactionId + "|",
                FullUrl = FullUrl
            };
            var queryData = _queryProcessor.Execute(query);
            if (queryData.o_return_code == 0)
            {
                list = queryData.ListPackageSellRouter;
            }

            if (list != null && list.Count > 0)
            {
                list = list.Where(p => p.AUTO_MAPPING != "A" || p.PACKAGE_TYPE == "Main").ToList();
                TecnologyType = list.Select(p => new { p.PRODUCT_SUBTYPE, p.PACKAGE_TYPE }).Where(p => p.PACKAGE_TYPE == "Main").FirstOrDefault().PRODUCT_SUBTYPE;
            }

            if (!list.Any())
            {
                jsonResult.Formatting = Newtonsoft.Json.Formatting.Indented;
                jsonResult.Data = new List<PackageModel>();

                return jsonResult;
            }

            var listGroup = new List<SGroup>();

            var top = list.Where(p => p.PACKAGE_GROUP != null && p.PACKAGE_GROUP != "" && !p.PACKAGE_GROUP.Contains("Incredible")
                && p.PRODUCT_SUBTYPE != "VOIP" && p.PRODUCT_SUBTYPE != "Equipment" && p.PACKAGE_TYPE != "Ontop" && p.PRODUCT_SUBTYPE != "PBOX"
                && p.PACKAGE_TYPE != "Ontop Special")
                .GroupBy(g => new { g.PACKAGE_GROUP, g.PRODUCT_SUBTYPE, g.PACKAGE_TYPE }).Select(s => new { s.Key.PACKAGE_GROUP, s.Key.PRODUCT_SUBTYPE, s.Key.PACKAGE_TYPE });

            var bottom = list.Where(p => p.PACKAGE_GROUP != null && p.PACKAGE_GROUP != "" && p.PACKAGE_GROUP.Contains("Incredible")
                && p.PRODUCT_SUBTYPE != "VOIP" && p.PRODUCT_SUBTYPE != "Equipment" && p.PACKAGE_TYPE != "Ontop" && p.PRODUCT_SUBTYPE != "PBOX"
                && p.PACKAGE_TYPE != "Ontop Special")
                .GroupBy(g => new { g.PACKAGE_GROUP, g.PRODUCT_SUBTYPE, g.PACKAGE_TYPE }).Select(s => new { s.Key.PACKAGE_GROUP, s.Key.PRODUCT_SUBTYPE, s.Key.PACKAGE_TYPE });

            if (top.Any())
                foreach (var a in top)
                    listGroup.Add(new SGroup { PACKAGE_GROUP = a.PACKAGE_GROUP, PRODUCT_SUBTYPE = a.PRODUCT_SUBTYPE, PACKAGE_TYPE = a.PACKAGE_TYPE });

            if (bottom.Any())
                foreach (var a in bottom)
                    listGroup.Add(new SGroup { PACKAGE_GROUP = a.PACKAGE_GROUP, PRODUCT_SUBTYPE = a.PRODUCT_SUBTYPE, PACKAGE_TYPE = a.PACKAGE_TYPE });

            list[0].S_GROUP = listGroup;
            jsonResult.Formatting = Newtonsoft.Json.Formatting.Indented;
            jsonResult.Data = list;

            return jsonResult;
        }

        [OutputCache(Location = OutputCacheLocation.Server, Duration = 3600)]
        public ActionResult GetListPackageSellRounterV2(string logID = "", string OptionInstallRouter = "")
        {
            string FullUrl = "";
            if (Session["FullUrl"] != null)
                FullUrl = Session["FullUrl"].ToSafeString();

            #region Get IP Address Interface Log (Update 17.2)

            // Get IP Address
            string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress)) ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            string transactionId = logID + ipAddress;

            #endregion

            string TecnologyType = "";
            var jsonResult = new JsonNetResult();
            List<ListPackageSellRouterV2Model> list = new List<ListPackageSellRouterV2Model>();
            string SaleChannel = "";

            if (OptionInstallRouter == "5")
            {
                SaleChannel = "SALE_ROUTER";
            }
            if (OptionInstallRouter == "3")
            {
                SaleChannel = "BYOD";
            }

            var query = new GetListPackageSellRouterV2Query()
            {
                P_SALE_CHANNEL = SaleChannel,
                TransactionID = transactionId + "|",
                FullUrl = FullUrl
            };
            var queryData = _queryProcessor.Execute(query);
            if (queryData.o_return_code == 0)
            {
                list = queryData.ListPackageSellRouter;
            }

            if (list != null && list.Count > 0)
            {
                string[] PRODUCT_SUBTYPE_ARRY = new string[3];
                PRODUCT_SUBTYPE_ARRY[0] = "WireBB";
                PRODUCT_SUBTYPE_ARRY[1] = "FTTx";
                PRODUCT_SUBTYPE_ARRY[2] = "FTTR";

                list = list.Where(p => p.PACKAGE_TYPE == "1" && PRODUCT_SUBTYPE_ARRY.Contains(p.PRODUCT_SUBTYPE) && string.IsNullOrEmpty(p.AUTO_MAPPING_PROMOTION_CODE)).ToList();
                if (list != null && list.Count > 0)
                    TecnologyType = list.FirstOrDefault().PRODUCT_SUBTYPE.ToSafeString();

                foreach (var item in list)
                {
                    if (item.PACKAGE_GROUP == null)
                    {
                        item.PACKAGE_GROUP = "";
                        item.PACKAGE_GROUP_DESC_ENG = "";
                        item.PACKAGE_GROUP_DESC_THA = "";
                        item.PACKAGE_REMARK_ENG = "";
                        item.PACKAGE_REMARK_THA = "";
                    }
                }
            }

            if (!list.Any())
            {
                jsonResult.Formatting = Newtonsoft.Json.Formatting.Indented;
                jsonResult.Data = new List<PackageModel>();

                return jsonResult;
            }

            var listGroup = new List<SGroupV2>();

            var topNoGroup = list.Where(p => string.IsNullOrEmpty(p.PACKAGE_GROUP)).ToList();
            if (topNoGroup != null && topNoGroup.Count > 0)
            {
                var tmpTopNoGroup = topNoGroup.GroupBy(g => new { g.PRODUCT_SUBTYPE, g.PACKAGE_TYPE })
                .Select(s => new { s.Key.PRODUCT_SUBTYPE, s.Key.PACKAGE_TYPE }).ToList();

                if (tmpTopNoGroup.Any())
                    foreach (var a in tmpTopNoGroup)
                        listGroup.Add(new SGroupV2
                        {
                            PACKAGE_GROUP = "",
                            PACKAGE_GROUP_DESC_ENG = "",
                            PACKAGE_GROUP_DESC_THA = "",
                            PACKAGE_REMARK_ENG = "",
                            PACKAGE_REMARK_THA = "",
                            PRODUCT_SUBTYPE = a.PRODUCT_SUBTYPE.ToSafeString(),
                            PACKAGE_TYPE = a.PACKAGE_TYPE.ToSafeString()
                        });
            }

            var topHaveGroup = list.Where(p => !string.IsNullOrEmpty(p.PACKAGE_GROUP))
                .GroupBy(g => new { g.PACKAGE_GROUP, g.PACKAGE_GROUP_DESC_ENG, g.PACKAGE_GROUP_DESC_THA, g.PACKAGE_REMARK_ENG, g.PACKAGE_REMARK_THA, g.PRODUCT_SUBTYPE, g.PACKAGE_TYPE })
                .Select(s => new
                {
                    s.Key.PACKAGE_GROUP,
                    s.Key.PACKAGE_GROUP_DESC_ENG,
                    s.Key.PACKAGE_GROUP_DESC_THA,
                    s.Key.PACKAGE_REMARK_ENG,
                    s.Key.PACKAGE_REMARK_THA,
                    s.Key.PRODUCT_SUBTYPE,
                    s.Key.PACKAGE_TYPE
                }).ToList();
            if (topHaveGroup != null && topHaveGroup.Count > 0)
            {
                if (topHaveGroup.Any())
                    foreach (var a in topHaveGroup)
                        listGroup.Add(new SGroupV2
                        {
                            PACKAGE_GROUP = a.PACKAGE_GROUP.ToSafeString(),
                            PACKAGE_GROUP_DESC_ENG = a.PACKAGE_GROUP_DESC_ENG.ToSafeString(),
                            PACKAGE_GROUP_DESC_THA = a.PACKAGE_GROUP_DESC_THA.ToSafeString(),
                            PACKAGE_REMARK_ENG = a.PACKAGE_REMARK_ENG.ToSafeString(),
                            PACKAGE_REMARK_THA = a.PACKAGE_REMARK_THA.ToSafeString(),
                            PRODUCT_SUBTYPE = a.PRODUCT_SUBTYPE.ToSafeString(),
                            PACKAGE_TYPE = a.PACKAGE_TYPE.ToSafeString()
                        });
            }

            list[0].S_GROUP = listGroup;
            jsonResult.Formatting = Newtonsoft.Json.Formatting.Indented;
            jsonResult.Data = list;

            return jsonResult;
        }

        //R18.1 FTTB Sell Router
        public JsonResult GetValidateShowSellRouter(string topUp = "", string line = "", string aisAirNumber = "",
            string partnerName = "", string accessMode = "")
        {
            var showSellRouterFlag = "";

            Logger.Info("GetValidateShowSellRouter: AisAirNumber=" + aisAirNumber + ", Start function");

            if (topUp == "5")
            {
                var accessModeModel = Newtonsoft.Json.JsonConvert.DeserializeObject<List<FBSSAccessModeInfo>>(accessMode);
                if (accessModeModel.Any())
                {
                    var accessModeRow = accessModeModel.FirstOrDefault();
                    if (accessModeRow != null)
                    {
                        var config = LovData.Where(l =>
                            l.Type == "SCREEN" && l.Name == "MAPPING_OWNER_PRODUCT" &&
                            l.LovValue1 == accessModeRow.AccessMode &&
                            l.LovValue2 == partnerName).ToList();

                        if (config.Any())
                        {
                            var firstOrDefault = config.FirstOrDefault();
                            if (firstOrDefault != null)
                            {
                                showSellRouterFlag = firstOrDefault.Text;
                            }
                        }
                    }
                }

            }

            Logger.Info("GetValidateShowSellRouter: AisAirNumber=" + aisAirNumber + ", resultFlag=" + showSellRouterFlag.ToSafeString() + ", End function");

            return Json(showSellRouterFlag, JsonRequestBehavior.AllowGet);
        }

        //Process/Topup , Process/TopupPlaybox// 26Dec2016 by PatthamawadeeH.
        [OutputCache(Location = OutputCacheLocation.Server, Duration = 3600)]
        public ActionResult GetListPackagebySFFPromo(string P_OWNER_PRODUCT = "", string P_PRODUCT_SUBTYPE = "", string P_NETWORK_TYPE = "", string P_SERVICE_DAY = "",
            string line = "", string P_PACKAGE_FOR = "", string P_PACKAGE_CODE = "", string isPartner = "", string partnerName = "", string accessMode = "",
            string interfaceId = "", string no = "", string topUp = "", string P_SFF_PROMOCODE = "", string p_location_code = "", string p_asc_code = "", string p_region = "",
            string p_province = "", string p_district = "", string p_sub_district = "", string p_address_type = "", string p_building_name = "", string p_building_no = "",
            string p_partner_type = "", string p_partner_subtype = "", string p_Serenade_Flag = "", string p_Customer_Type = "", string p_Address_Id = "", string p_InstallationType = "",
            string p_Plug_And_Play_Flag = "", string AisAirNumber = "", string isMesh = "N")
        {
            string FullUrl = "";
            if (Session["FullUrl"] != null)
                FullUrl = Session["FullUrl"].ToSafeString();

            #region Get IP Address Interface Log (Update 17.2)

            // Get IP Address
            string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress)) ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            string transactionId = AisAirNumber + ipAddress;

            #endregion

            var httpCookie = Request.Cookies["ASP.NET_SessionId"];
            var cookieSessionID = httpCookie != null ? httpCookie.Value : Session.SessionID;

            var jsonResult = new JsonNetResult();
            // ส่ง access mode list ที่ได้มาจาก GetFBSSFeasibilityCheckHandler

            var accessModeModel = Newtonsoft.Json.JsonConvert.DeserializeObject<List<FBSSAccessModeInfo>>(accessMode);
            var owenerProduct = "";
            var productSubtype = P_PRODUCT_SUBTYPE;

            if (line == "2" || topUp == "1" || topUp == "4")
            {
                if (P_OWNER_PRODUCT == "")
                {
                    var q = new GetOwnerProductByNoQuery
                    {
                        No = no
                    };
                    var getCusData = _queryProcessor.Execute(q);
                    owenerProduct = getCusData.Value;
                }
                else
                    owenerProduct = P_OWNER_PRODUCT;
            }

            else
            {
                base.Logger.Info("Begin get Owner Product");

                base.Logger.Info("Prameter for list owner \r\n IsPartner:" + isPartner + "\r\n PartnerName:" + partnerName + "\r\n FBSSAccessModeInfo:" + accessModeModel);
                owenerProduct = _queryProcessor.Execute(new GetMappingSbnOwnerProd
                {
                    IsPartner = isPartner,
                    PartnerName = partnerName,
                    FBSSAccessModeInfo = accessModeModel

                });

                base.Logger.Info("Result Owner :" + owenerProduct);
                base.Logger.Info("End get Owner Product");
            }

            List<PackageModel> list;

            if (line == "2" || topUp == "1" || topUp == "4")
            {
                string VAS_SERVICE = "Y";

                if (isMesh == "Y")
                {
                    VAS_SERVICE = "";
                }

                var query = new GetPackageListBySFFPromoQuery
                {
                    P_SFF_PROMOCODE = P_SFF_PROMOCODE,
                    P_OWNER_PRODUCT = owenerProduct,
                    P_PRODUCT_SUBTYPE = productSubtype,
                    VAS_SERVICE = VAS_SERVICE,
                    TransactionID = transactionId
                };
                list = _queryProcessor.Execute(query);

            }
            else
            {
                if (line == "1")
                {
                    p_location_code = "SSO";
                }//ถ้าเป็น SSO จะส่ง location code is "SSO"


                var query = new GetListPackageByServiceQuery
                {
                    P_OWNER_PRODUCT = owenerProduct, //P_OWNER_PRODUCT แทนที่ด้วย access mode mapping,
                    P_PRODUCT_SUBTYPE = productSubtype,
                    P_NETWORK_TYPE = P_NETWORK_TYPE,
                    P_SERVICE_DAY = P_SERVICE_DAY,
                    P_PACKAGE_FOR = P_PACKAGE_FOR,
                    P_PACKAGE_CODE = P_PACKAGE_CODE,
                    TransactionID = transactionId,
                    P_Location_Code = p_location_code,
                    P_Asc_Code = p_asc_code,
                    P_Region = p_region,
                    P_Province = p_province,
                    P_District = p_district,
                    P_Sub_District = p_sub_district,
                    P_Address_Type = p_address_type,
                    P_Building_Name = p_building_name,
                    P_Building_No = p_building_no,
                    P_Partner_Type = p_partner_type,
                    P_Partner_SubType = p_partner_subtype,
                    P_Serenade_Flag = p_Serenade_Flag,
                    P_Customer_Type = p_Customer_Type,
                    P_Address_Id = p_Address_Id,
                    FullUrl = FullUrl,
                    SessionId = cookieSessionID,

                };
                list = _queryProcessor.Execute(query);
            }

            string ShowInstall = "";
            string TecnologyType = "";
            if (list != null && list.Count > 0)
            {
                TecnologyType = list.Select(p => new { p.PRODUCT_SUBTYPE, p.PACKAGE_TYPE }).Where(p => p.PACKAGE_TYPE == "Main").FirstOrDefault().PRODUCT_SUBTYPE;
            }
            var LovConfig = GetLovConfigOptionInstallData();


            if (LovConfig != null && LovConfig.Count > 0)
            {
                if (p_InstallationType == "0")
                {
                    var LovConfigStr = LovConfig.Where(p => p.Text == TecnologyType && p.DefaultValue == "1").ToList();
                    if (LovConfigStr != null && LovConfigStr.Count > 0)
                    {
                        var LovConfigStrTmp = LovConfigStr.FirstOrDefault();
                        if (LovConfigStrTmp != null)
                        {
                            string[] codesarray = new string[2];
                            codesarray[0] = LovConfigStrTmp.LovValue1;
                            codesarray[1] = LovConfigStrTmp.LovValue2;
                            if (LovConfigStrTmp.LovValue1 != "" && LovConfigStrTmp.LovValue2 != "")
                            {
                                list = list.Where(p => !codesarray.Contains(p.SFF_PROMOTION_CODE)).ToList();
                                ShowInstall = "True";
                            }
                        }
                    }
                }
                else
                {
                    var LovConfigStr = LovConfig.Where(p => p.Text == TecnologyType && p.DefaultValue == "0");
                    if (LovConfigStr != null)
                    {
                        var LovConfigStrTmp = LovConfigStr.FirstOrDefault();
                        if (LovConfigStrTmp != null)
                        {
                            string codes = LovConfigStrTmp.LovValue1 + "," + LovConfigStrTmp.LovValue2;
                            string[] codesarray = new string[2];
                            codesarray[0] = LovConfigStrTmp.LovValue1;
                            codesarray[1] = LovConfigStrTmp.LovValue2;
                            if (LovConfigStrTmp.LovValue1 != "" && LovConfigStrTmp.LovValue2 != "")
                            {
                                list = list.Where(p => !codesarray.Contains(p.SFF_PROMOTION_CODE)).ToList();
                                ShowInstall = "True";
                            }
                        }
                    }
                }
            }

            if (!list.Any())
            {
                base.Logger.Info("Package Model Is Null");
                jsonResult.Formatting = Newtonsoft.Json.Formatting.Indented;
                jsonResult.Data = new List<PackageModel>();

                return jsonResult;
            }

            int componentBundle = 0;
            /*
               if (base.CurrentUser.ComponentModel != null && base.CurrentUser.Groups != null)
                componentBundle = base.CurrentUser.ComponentModel.Where(w => w.ComponentName == "FBBOR002_REGISTER_BUNDLE"
                && base.CurrentUser.Groups.Contains(w.GroupID)).Count();
            */
            if (componentBundle == 0)
                list = list.Where(p => p.PACKAGE_TYPE != "Bundle" && !p.PACKAGE_GROUP.Contains("Bundle")).ToList();

            var listGroup = new List<SGroup>();

            var top = list.Where(p => p.PACKAGE_GROUP != null && p.PACKAGE_GROUP != "" && !p.PACKAGE_GROUP.Contains("Incredible")
                && p.PRODUCT_SUBTYPE != "VOIP" && p.PRODUCT_SUBTYPE != "Equipment" && p.PACKAGE_TYPE != "Ontop" && p.PRODUCT_SUBTYPE != "PBOX"
                && p.PACKAGE_TYPE != "Ontop Special")
                .GroupBy(g => new { g.PACKAGE_GROUP, g.PRODUCT_SUBTYPE, g.PACKAGE_TYPE }).Select(s => new { s.Key.PACKAGE_GROUP, s.Key.PRODUCT_SUBTYPE, s.Key.PACKAGE_TYPE });

            var bottom = list.Where(p => p.PACKAGE_GROUP != null && p.PACKAGE_GROUP != "" && p.PACKAGE_GROUP.Contains("Incredible")
                && p.PRODUCT_SUBTYPE != "VOIP" && p.PRODUCT_SUBTYPE != "Equipment" && p.PACKAGE_TYPE != "Ontop" && p.PRODUCT_SUBTYPE != "PBOX"
                && p.PACKAGE_TYPE != "Ontop Special")
                .GroupBy(g => new { g.PACKAGE_GROUP, g.PRODUCT_SUBTYPE, g.PACKAGE_TYPE }).Select(s => new { s.Key.PACKAGE_GROUP, s.Key.PRODUCT_SUBTYPE, s.Key.PACKAGE_TYPE });


            if (top.Any())
                foreach (var a in top)
                    listGroup.Add(new SGroup { PACKAGE_GROUP = a.PACKAGE_GROUP, PRODUCT_SUBTYPE = a.PRODUCT_SUBTYPE, PACKAGE_TYPE = a.PACKAGE_TYPE });

            if (bottom.Any())
                foreach (var a in bottom)
                    listGroup.Add(new SGroup { PACKAGE_GROUP = a.PACKAGE_GROUP, PRODUCT_SUBTYPE = a.PRODUCT_SUBTYPE, PACKAGE_TYPE = a.PACKAGE_TYPE });

            list[0].S_GROUP = listGroup;
            list[0].TEMP_IA = ShowInstall;

            jsonResult.Formatting = Newtonsoft.Json.Formatting.Indented;
            jsonResult.Data = list;

            return jsonResult;
        }

        //R20.1 Process/Topup , Process/TopupPlaybox
        [OutputCache(Location = OutputCacheLocation.Server, Duration = 3600)]
        public ActionResult GetListPackagebySFFPromoV2(string P_OWNER_PRODUCT = "", string P_PRODUCT_SUBTYPE = "", string accessMode = "", string no = "", string P_SFF_PROMOCODE = "", string P_EXISTING_REQ = "", string P_SALE_CHANNEL = "")
        {
            string FullUrl = "";
            if (accessMode == "CallIn")
            {
                accessMode = "";
                FullUrl = "CallInProcess";
            }
            else
            {
                if (Session["FullUrl"] != null)
                    FullUrl = Session["FullUrl"].ToSafeString();
                else
                    FullUrl = System.Web.HttpContext.Current.Request.Url.ToSafeString().Split('?')[0];
            }

            #region Get IP Address Interface Log (Update 17.2)

            // Get IP Address
            string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress)) ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            string transactionId = no + ipAddress;

            #endregion

            var jsonResult = new JsonNetResult();

            var accessModeModel = Newtonsoft.Json.JsonConvert.DeserializeObject<List<FBSSAccessModeInfo>>(accessMode);
            var owenerProduct = "";
            var productSubtype = P_PRODUCT_SUBTYPE;

            if (P_OWNER_PRODUCT == "")
            {
                var q = new GetOwnerProductByNoQuery
                {
                    No = no
                };
                var getCusData = _queryProcessor.Execute(q);
                owenerProduct = getCusData.Value;
            }
            else
                owenerProduct = P_OWNER_PRODUCT;


            List<PackageModel> list;
            var query = new GetPackageListBySFFPromoV2Query
            {
                P_SFF_PROMOCODE = P_SFF_PROMOCODE,
                P_OWNER_PRODUCT = owenerProduct,
                P_PRODUCT_SUBTYPE = productSubtype,
                P_EXISTING_REQ = P_EXISTING_REQ,
                P_INTERNET_NO = no,
                P_SALE_CHANNEL = P_SALE_CHANNEL,//R21.5
                TransactionID = transactionId,
                FullUrl = FullUrl
            };
            list = _queryProcessor.Execute(query);

            if (P_EXISTING_REQ == "MESH")
            {
                // SFF_PRODUCT_PACKAGE Penalty Package
                List<PackageModel> listPenalty = list.Where(t => t.SFF_PRODUCT_PACKAGE.IndexOf("Penalty Package") > -1).ToList();
                if (listPenalty == null || listPenalty.Count == 0)
                {
                    GetOnlineQueryPackPenaltyModel result = new GetOnlineQueryPackPenaltyModel();
                    GetOnlineQueryPackPenaltyQuery reqOnline = new GetOnlineQueryPackPenaltyQuery
                    {
                        FullUrl = "",
                        Transaction_Id = transactionId,
                        Internet_No = no,
                        Body = new GetOnlineQueryPackPenaltyBody
                        {
                            SFF_PROMOTION_CODE = P_SFF_PROMOCODE,
                            PRODUCT_SUBTYPE = productSubtype,
                            OWNER_PRODUCT = owenerProduct,
                            SALE_CHANNEL = P_SALE_CHANNEL
                        }
                    };

                    result = _queryProcessor.Execute(reqOnline);

                    if (result.RESULT_CODE == "20000" && result.LIST_PACKAGE_PENALTY != null && result.LIST_PACKAGE_PENALTY.Count > 0)
                    {
                        List<PackPenaltyResult> tmpPackPenaltyResult = result.LIST_PACKAGE_PENALTY.Where(t => !String.IsNullOrEmpty(t.SFF_PRODUCT_PACKAGE) && t.SFF_PRODUCT_PACKAGE.IndexOf("Penalty Package") > -1).ToList();
                        if (tmpPackPenaltyResult != null && tmpPackPenaltyResult.Count > 0)
                        {
                            PackageModel packPenaltyResult = tmpPackPenaltyResult.Select(t => new PackageModel
                            {
                                PACKAGE_CODE = t.SFF_PROMOTION_CODE.ToSafeString(),
                                SFF_PROMOTION_BILL_THA = t.SFF_PRODUCT_NAME.ToSafeString(),
                                SFF_PROMOTION_BILL_ENG = t.SFF_PRODUCT_NAME.ToSafeString(),
                                TECHNOLOGY = t.SFF_PRODUCT_NAME.ToSafeString(),
                                PRODUCT_SUBTYPE = productSubtype.ToSafeString(),
                                OWNER_PRODUCT = owenerProduct.ToSafeString(),
                                SFF_PROMOTION_CODE = t.SFF_PROMOTION_CODE.ToSafeString(),
                                PACKAGE_TYPE_DESC = t.SFF_PRODUCT_PACKAGE.ToSafeString(),
                                SFF_PRODUCT_NAME = t.SFF_PRODUCT_NAME.ToSafeString()
                            }).FirstOrDefault();
                            list.Add(packPenaltyResult);
                        }
                    }
                }
            }

            string TecnologyType = P_PRODUCT_SUBTYPE;// "" เอา PRODUCT_SUBTYPE จากอันแรกมาใส่แจ้

            if (!list.Any())
            {
                base.Logger.Info("Package Model Is Null");
                jsonResult.Formatting = Newtonsoft.Json.Formatting.Indented;
                jsonResult.Data = new List<PackageModel>();

                return jsonResult;
            }
            jsonResult.Formatting = Newtonsoft.Json.Formatting.Indented;
            jsonResult.Data = list;

            return jsonResult;
        }

        //Multiple Playbox
        [OutputCache(Location = OutputCacheLocation.Server, Duration = 3600)]
        public ActionResult GetPlayboxExtensionInfo(string AisAirNumber = "", string UsedNumberOfPlaybox = "0", string NumberOfPlaybox = "0")
        {
            #region Get IP Address Interface Log (Update 17.2)

            // Get IP Address
            string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress))
                ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            string transactionId = AisAirNumber + ipAddress;

            #endregion

            var jsonResult = new JsonNetResult();
            var query = new EvOmPlayboxExtensionInfoQuery
            {
                FbbId = AisAirNumber,
                TransactionId = transactionId
            };
            var list = _queryProcessor.Execute(query);

            jsonResult.Formatting = Newtonsoft.Json.Formatting.Indented;

            if (list != null)
            {
                int tryInt;
                int usedNumberOfPlaybox = int.TryParse(UsedNumberOfPlaybox, out tryInt) ? int.Parse(UsedNumberOfPlaybox) : 0;
                int numberOfPlaybox = int.TryParse(NumberOfPlaybox, out tryInt) ? int.Parse(NumberOfPlaybox) : 0;
                int numberOfDdl = numberOfPlaybox - usedNumberOfPlaybox;

                var tempAvailPlayboxList = new List<AvailPlaybox>();
                var listPlaybox = list.AvailPlayboxList;
                if (listPlaybox != null)
                {
                    foreach (var listRow in list.AvailPlayboxList.OrderBy(o => o.PlayBoxExt).ToList())
                    {
                        if ((listRow.PlayBoxExt >= usedNumberOfPlaybox) && (tempAvailPlayboxList.Count < numberOfDdl))
                        {
                            tempAvailPlayboxList.Add(listRow);
                        }

                    }
                }
                list.AvailPlayboxList = tempAvailPlayboxList;
            }
            jsonResult.Data = list ?? new EvOmPlayboxExtensionInfoModel();

            return jsonResult;
        }

        //Multiple Playbox
        [OutputCache(Location = OutputCacheLocation.Server, Duration = 3600)]
        public ActionResult GetCheckPackageDownloadSpeed(string ProductCode, string AisAirNumber = "")
        {
            #region Get IP Address Interface Log (Update 17.2)

            // Get IP Address
            string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress))
                ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            string transactionId = AisAirNumber + ipAddress;

            #endregion

            var jsonResult = new JsonNetResult();
            var query = new CheckPackageDownloadSpeedQuery
            {
                ProductCode = ProductCode,
                TransactionId = transactionId
            };
            var list = _queryProcessor.Execute(query);

            jsonResult.Formatting = Newtonsoft.Json.Formatting.Indented;
            jsonResult.Data = list ?? new CheckPackageDownloadSpeedModel();

            return jsonResult;
        }

        [OutputCache(Location = OutputCacheLocation.Server, Duration = 3600)]
        public ActionResult GetCheckPackageDownloadSpeedTopUpPlaybox(string productCode, string aisAirNumber = "")
        {
            //R17.9 Speed boost
            #region Get IP Address Interface Log (Update 17.2)

            // Get IP Address
            string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress))
                ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            string transactionId = aisAirNumber + ipAddress;

            #endregion

            var jsonResult = new JsonNetResult();
            var query = new CheckPackageSpeedTopUpPlayboxQuery
            {
                ProductCode = productCode,
                TransactionId = transactionId
            };
            var list = _queryProcessor.Execute(query);

            jsonResult.Formatting = Newtonsoft.Json.Formatting.Indented;
            jsonResult.Data = list ?? new CheckPackageDownloadSpeedModel();

            return jsonResult;
        }

        //R21.11 ATV 
        [OutputCache(Location = OutputCacheLocation.Server, Duration = 3600)]
        public ActionResult GetListDeviceContractOnlineQuery(string AisAirNumber = "", string P_CHANNEL = "", string P_EVENT = "",
            string PROMOTION_CODE_ARRAY = "", string P_PRODUCT_SUBTYPE = "", string P_OWNER_PRODUCT = "", string P_SALE_CHANNEL = "",
            string accessMode = "", string isPartner = "", string partnerName = "", string addressId = "")
        {
            string FullUrl = "";
            if (Session["FullUrl"] != null) FullUrl = Session["FullUrl"].ToSafeString();

            #region Get IP Address Interface Log
            // Get IP Address
            string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress)) ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            var httpCookie = Request.Cookies["ASP.NET_SessionId"];
            var cookieSessionID = httpCookie != null ? httpCookie.Value : Session.SessionID;

            string transactionId = AisAirNumber + ipAddress;

            #endregion

            //R22.07
            if (P_EVENT == "New")
            {
                var accessModeModel = Newtonsoft.Json.JsonConvert.DeserializeObject<List<FBSSAccessModeInfo>>(accessMode);
                var owenerProduct = "";

                owenerProduct = _queryProcessor.Execute(new GetMappingSbnOwnerProd
                {
                    IsPartner = isPartner,
                    PartnerName = partnerName,
                    FBSSAccessModeInfo = accessModeModel
                });

                if (P_PRODUCT_SUBTYPE == "")
                {
                    var checkTech = new GetCheckTechnologyQuery
                    {
                        P_OWNER_PRODUCT = owenerProduct,
                        P_ADDRESS_ID = addressId
                    };

                    var modelCheckTech = _queryProcessor.Execute(checkTech);

                    if (modelCheckTech != null && modelCheckTech.PRODUCT_SUBTYPE.ToSafeString() != "")
                    {
                        P_PRODUCT_SUBTYPE = modelCheckTech.PRODUCT_SUBTYPE;
                    }
                }
            }
            //--------------------

            var jsonResult = new JsonNetResult();
            ContractDeviceDataModel contractDeviceDataModel = new ContractDeviceDataModel();
            List<ContractDeviceModel> result = new List<ContractDeviceModel>();
            List<SFF_PROMOTION_CODE_DEVICE_CONTRACT> list_sff_promotion_code = new List<SFF_PROMOTION_CODE_DEVICE_CONTRACT>();
            var js_old = new JavaScriptSerializer();
            var deserializedAirOldItems = (object[])js_old.DeserializeObject(PROMOTION_CODE_ARRAY);
            if (deserializedAirOldItems != null)
            {
                foreach (Dictionary<string, object> oldItem in deserializedAirOldItems)
                {
                    SerializeSffPromotionCodeDeviceContractJSonModel stmp = new SerializeSffPromotionCodeDeviceContractJSonModel(oldItem);
                    SFF_PROMOTION_CODE_DEVICE_CONTRACT air_old = new SFF_PROMOTION_CODE_DEVICE_CONTRACT()
                    {
                        SFF_PROMOTION_CODE = stmp.SFF_PROMOTION_CODE
                    };
                    list_sff_promotion_code.Add(air_old);
                }

                var query = new GetListDeviceContractQuery()
                {
                    TransactionID = transactionId,
                    FullUrl = FullUrl,
                    P_CHANNEL = P_CHANNEL,
                    P_EVENT = P_EVENT,
                    P_PRODUCT_SUBTYPE = P_PRODUCT_SUBTYPE, //R22.07
                    P_OWNER_PRODUCT = P_OWNER_PRODUCT, //R22.07
                    P_SALE_CHANNEL = P_SALE_CHANNEL, //R22.07
                    LIST_SFF_PROMOTION_CODE = list_sff_promotion_code
                };

                result = _queryProcessor.Execute(query);
            }

            contractDeviceDataModel.ContractDeviceList = result;
            jsonResult.Data = contractDeviceDataModel;

            return jsonResult;
        }

        //R22.03 TopupReplace 
        [OutputCache(Location = OutputCacheLocation.Server, Duration = 3600)]
        public ActionResult GetCheckPromotionTopupReplace(string AisAirNumber = "", string Lang = "", string SffPromotionCodeMain = "")
        {
            string FullUrl = Session["FullUrl"] != null ? Session["FullUrl"].ToSafeString() : "";

            #region Get IP Address Interface Log

            // Get IP Address
            string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress))
                ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            string transactionId = AisAirNumber + ipAddress;

            #endregion

            var query = new GetCheckPromotionTopupReplaceQuery
            {
                P_FLAG_LANG = Lang,
                TransactionId = transactionId,
                FullUrl = FullUrl
            };

            query.SffPromotionCodeList = new List<ContractDeviceArrayModel>();
            query.SffPromotionCodeList.Add(new ContractDeviceArrayModel { PROMOTION_CODE = SffPromotionCodeMain });

            var resultdata = _queryProcessor.Execute(query);

            var jsonResult = new JsonNetResult();
            jsonResult.Formatting = Newtonsoft.Json.Formatting.Indented;
            jsonResult.Data = resultdata ?? new CheckPromotionTopupReplaceModel();

            return jsonResult;
        }

        //R22.03 TopupReplace
        [OutputCache(Location = OutputCacheLocation.Server, Duration = 3600)]
        public ActionResult GetDetailOrderFeeTopupReplace(string AisAirNumber = "", string Lang = "")
        {
            string FullUrl = Session["FullUrl"] != null ? Session["FullUrl"].ToSafeString() : "";

            #region Get IP Address Interface Log

            // Get IP Address
            string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress)) ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            string transactionId = AisAirNumber + ipAddress;

            #endregion

            var query = new GetDetailOrderFeeTopupReplaceQuery
            {
                P_FLAG_LANG = Lang,
                TransactionId = transactionId,
                FullUrl = FullUrl
            };

            DetailOrderFeeTopupReplaceModel data = _queryProcessor.Execute(query);
            List<PriceATV> dataList = new List<PriceATV>();

            if (data != null && data.RETURN_PRICE_CURROR != null && data.RETURN_PRICE_CURROR.Count() > 0)
            {
                dataList = data.RETURN_PRICE_CURROR.Select(c => new PriceATV
                {
                    DISPLAY_ORDER_FEE = c.DISPLAY_ORDER_FEE.ToSafeString(),
                    PRICE_ORDER_FEE = c.PRICE_ORDER_FEE.ToSafeString()
                }).ToList();
            }

            var jsonResult = new JsonNetResult();
            jsonResult.Data = dataList;
            return jsonResult;
        }

        public class SerializeSffPromotionCodeDeviceContractJSonModel
        {
            public SerializeSffPromotionCodeDeviceContractJSonModel(Dictionary<string, object> newFeature)
            {
                if (newFeature.ContainsKey("SFF_PROMOTION_CODE"))
                {
                    SFF_PROMOTION_CODE = (string)newFeature["SFF_PROMOTION_CODE"];
                }
            }
            public string SFF_PROMOTION_CODE { get; set; }
        }

        [HttpPost]
        public JsonResult GetListServiceAndPromotionByPackType(string mobileNo = "", string idCard = "", string idCardType = "")
        {

            string FullUrl = "";
            if (Session["FullUrl"] != null)
                FullUrl = Session["FullUrl"].ToSafeString();

            #region Get IP Address Interface Log (Update 17.2)

            // Get IP Address
            string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress)) ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            string transactionId = mobileNo + ipAddress;

            #endregion
            evOMQueryListServiceAndPromotionByPackageTypeModel model = new evOMQueryListServiceAndPromotionByPackageTypeModel();
            model.ErrorMessage = "No Profile";

            if (idCard != "" && idCardType != "")
            {
                bool haveProfile = false;

                try
                {
                    var queryMassCommonAccountInfo = new evESeServiceQueryMassCommonAccountInfoQuery
                    {
                        inOption = "2",
                        inMobileNo = mobileNo,
                        inCardNo = idCard,
                        inCardType = idCardType,
                        Page = "Process",
                        Username = "",
                        FullUrl = FullUrl
                    };
                    var massCommon = _queryProcessor.Execute(queryMassCommonAccountInfo);
                    if (massCommon.errorMessage == "")
                    {
                        haveProfile = true;
                    }

                }
                catch (Exception ex)
                {
                    model.ErrorMessage = ex.Message;
                }

                if (haveProfile)
                {
                    var query = new evOMQueryListServiceAndPromotionByPackageTypeQuery
                    {
                        mobileNo = mobileNo,//"8850001230",
                        idCard = idCard,
                        FullUrl = FullUrl
                    };

                    model = _queryProcessor.Execute(query);
                }

            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckMeshPackage(string mobileNo, string promotionCode, string sffProductName, string sffServiceYear, string topUp, string Channel, string lang, string RegisteredDate, string ContractFlagFbb, string CountContractFbb, string FBBLimitContract)
        {

            var query = new GetMeshParameterPackageQuery
            {
                FibrenetID = mobileNo,//"8800023695",
                PromotionCode = promotionCode
            };

            var MeshParameter = _queryProcessor.Execute(query);

            var packageSFF = GetListPackagebySFFPromo(MeshParameter.RES_COMPLETE_CUR[0].PACKAGE_OWNER, MeshParameter.RES_COMPLETE_CUR[0].PACKAGE_SUBTYPE, sffProductName, sffServiceYear, "", "", promotionCode, "", "", "", "", "", topUp, MeshParameter.RES_COMPLETE_CUR[0].SFF_PROMOCODE, "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", mobileNo, "Y");
            var meshCompare = GetMeshCompareDevice(mobileNo, Channel, lang, "", ContractFlagFbb, CountContractFbb, FBBLimitContract);

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            dynamic modelPackageSFF = serializer.Deserialize<object>(packageSFF.ToSafeString());
            MeshCompareDeviceModel modelMeshCompare = (MeshCompareDeviceModel)serializer.DeserializeObject(meshCompare.ToSafeString());
            Boolean ret = false;
            foreach (var p in modelPackageSFF)
            {
                foreach (var m in modelMeshCompare.RES_COMPLETE_CUR)
                {
                    string base64String = Convert.ToBase64String(m.PIC_MESH, 0, m.PIC_MESH.Length);
                    m.PIC_MESH_BASE64 = "data:image/png;base64," + base64String;
                    m.PIC_MESH = null;
                    if (m.MESH_PROMOTION_CODE == p.UPLOAD_SPEED && !ret)
                    {
                        ret = true;
                    }
                }
            }
            GC.Collect();

            return Json(ret, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMeshParameterPackage(string mobileNo, string promotionCode)
        {

            string FullUrl = "";
            if (Session["FullUrl"] != null)
                FullUrl = Session["FullUrl"].ToSafeString();

            #region Get IP Address Interface Log (Update 17.2)

            // Get IP Address
            string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress)) ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            string transactionId = mobileNo + ipAddress;

            #endregion

            var query = new GetMeshParameterPackageQuery
            {
                FibrenetID = mobileNo,//"8800023695",
                PromotionCode = promotionCode
            };

            var model = _queryProcessor.Execute(query);


            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMeshCompareDevice(string mobileNo, string Channel, string lang, string RegisterDate, string ContractFlagFbb, string CountContractFbb, string FBBLimitContract)
        {
            string FullUrl = Session["FullUrl"] != null ? Session["FullUrl"].ToSafeString() : "";

            #region Get IP Address Interface Log

            // Get IP Address
            string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress)) ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            string transactionId = mobileNo + ipAddress;

            #endregion

            var query = new GetFBSSQueryCPEPenaltyQuery
            {
                OPTION = "5",
                FIBRENET_ID = mobileNo,
                SERIAL_NO = "",
                STATUS = "4",
                MAC_ADDRESS = "",
                TransactionId = transactionId,
                FullUrl = FullUrl
            };

            List<FBSSQueryCPEPenaltyModel> dataCPEPenalty = _queryProcessor.Execute(query);

            string flagPenalty = "0";
            if (dataCPEPenalty.Count() > 0)
                flagPenalty = "1";

            var query2 = new GetMeshCompareDeviceQuery
            {
                FibrenetID = mobileNo,
                Channel = Channel,
                flagPenalty = flagPenalty,
                lang = lang,
                RegisterDate = RegisterDate,
                CompareArray = dataCPEPenalty,
                ContractFlagFbb = ContractFlagFbb,
                CountContractFbb = CountContractFbb,
                FBBLimitContract = FBBLimitContract
            };

            MeshCompareDeviceModel model = _queryProcessor.Execute(query2);
            if (model != null && model.RES_COMPLETE_CUR != null && model.RES_COMPLETE_CUR.Count() > 0)
            {
                List<CompareDevice> compareDeviceList = model.RES_COMPLETE_CUR.Select(c => new CompareDevice
                {
                    FIBRENETID = c.FIBRENETID,
                    MESH_BRAND_NAME = c.MESH_BRAND_NAME,
                    MESH_PROMOTION_CODE = c.MESH_PROMOTION_CODE,
                    CHANNEL_MESH = c.CHANNEL_MESH,
                    NUMBER_OF_PURCHSES = c.NUMBER_OF_PURCHSES,
                    FLAG_POPUP_MESH = c.FLAG_POPUP_MESH,
                    DETAIL_MESH = c.DETAIL_MESH,
                    PIC_MESH = c.PIC_MESH,
                    PIC_MESH_BASE64 = c.PIC_MESH_BASE64,
                    OPTION_MESH = c.OPTION_MESH,
                    OPTION_MESH_DETAIL = c.OPTION_MESH_DETAIL,
                    POPUP_ALERT = c.POPUP_ALERT,
                    PRICE_VALUE = c.PRICE_VALUE,
                    WORDING_DISPLAY = c.WORDING_DISPLAY,
                    ORDER_BY = c.ORDER_BY,
                    ODR = int.Parse(c.ORDER_BY),
                    CONTRACT_ID = c.CONTRACT_ID,
                    CONTRACT_NAME = c.CONTRACT_NAME,
                    DURATION = c.DURATION,
                    PROMOTION_DEVICE = c.PROMOTION_DEVICE, //R22.05 Device Contract Mesh
                    FLAG_OPTION = c.FLAG_OPTION, //R22.11 Mesh with arpu
                    FLAG_MESH = c.FLAG_MESH //R22.11 Mesh with arpu
                }).ToList();
                model.RES_COMPLETE_CUR = compareDeviceList.OrderBy(t => t.ODR).ToList();
                foreach (var m in model.RES_COMPLETE_CUR)
                {
                    string base64String = Convert.ToBase64String(m.PIC_MESH, 0, m.PIC_MESH.Length);
                    m.PIC_MESH_BASE64 = "data:image/png;base64," + base64String;
                    m.PIC_MESH = null;
                    if (m.WORDING_DISPLAY == null || m.WORDING_DISPLAY == "null")
                        m.WORDING_DISPLAY = "";
                }
            }
            GC.Collect();
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMeshCompareDeviceOrder(string mobileNo, string Channel, string MeshBrandName,
            string MeshPromotionCode, string BuyMesh, string PenaltyInstall, string ContractID, string ContractName, string Duration,
            string ContractRuleId, string PenaltyType, string PenaltyId, string CountFlag)
        {

            string FullUrl = "";
            if (Session["FullUrl"] != null)
                FullUrl = Session["FullUrl"].ToSafeString();

            #region Get IP Address Interface Log (Update 17.2)

            // Get IP Address
            string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress)) ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            string transactionId = mobileNo + ipAddress;

            #endregion

            var query2 = new GetMeshCompareDeviceOrderQuery
            {
                FibrenetID = mobileNo,
                Channel = Channel,
                MeshBrandName = MeshBrandName,
                BuyMesh = BuyMesh,
                PenaltyInstall = PenaltyInstall,
                ContractID = ContractID,
                ContractName = ContractName,
                Duration = Duration,
                ContractRuleId = ContractRuleId,
                PenaltyType = PenaltyType,
                PenaltyId = PenaltyId,
                CountFlag = CountFlag
            };

            MeshCompareDeviceOrderModel model = _queryProcessor.Execute(query2);

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public string CheckUsePoin(string SFFPromotionCode)
        {
            string result = "0";
            CheckUsePoinBySFFPromotionCodeQuery query = new CheckUsePoinBySFFPromotionCodeQuery()
            {
                SFF_PROMOTION_CODE = SFFPromotionCode.ToSafeString()
            };
            var Poin = _queryProcessor.Execute(query);
            if (Poin != null)
            {
                result = Poin.PRE_PRICE_CHARGE.ToSafeString();
            }
            return result;
        }

        [OutputCache(Location = OutputCacheLocation.Client, Duration = 30)]
        public JsonResult GetDescription(string Packgrop)
        {
            //List<LovValueModel> config = base.LovData.Where(l => l.Name == "DESCRIPTION_BY_PACKAGE_GROUP" && Packgrop.Trim().Contains(l.Text)).ToList();
            List<LovValueModel> config = base.LovData.Where(l => l.Name == "DESCRIPTION_BY_PACKAGE_GROUP" && l.Text == Packgrop.Trim()).ToList();
            List<LovScreenValueModel> screenValue;
            if (base.GetCurrentCulture().IsThaiCulture())
            {
                screenValue = config.Select(l => new LovScreenValueModel
                {
                    Name = l.Name,
                    PageCode = l.LovValue5,
                    DisplayValue = l.LovValue1,
                    LovValue3 = l.LovValue3,
                    GroupByPDF = l.LovValue4,
                    OrderByPDF = l.OrderBy,
                    Type = l.Type,
                    DefaultValue = l.DefaultValue
                }).ToList();
            }
            else
            {
                screenValue = config.Select(l => new LovScreenValueModel
                {
                    Name = l.Name,
                    PageCode = l.LovValue5,
                    DisplayValue = l.LovValue2,
                    LovValue3 = l.LovValue3,
                    GroupByPDF = l.LovValue4,
                    OrderByPDF = l.OrderBy,
                    Type = l.Type,
                    DefaultValue = l.DefaultValue
                }).ToList();
            }
            return Json(screenValue, JsonRequestBehavior.AllowGet);

        }

        [OutputCache(Location = OutputCacheLocation.Client, Duration = 30)]
        public JsonResult GetRemark(string Packgroup, string BuildingName)
        {
            //List<LovValueModel> config = base.LovData.Where(l => l.Name == "REMARK_BY_PACKAGE_GROUP" && Packgroup.Trim().Contains(l.Text) && l.LovValue3 == BuildingName).ToList();
            List<LovValueModel> config = base.LovData.Where(l => l.Name == "REMARK_BY_PACKAGE_GROUP" && l.Text == Packgroup.Trim() && l.LovValue3 == BuildingName).ToList();
            if (config.Count < 1)
            {
                config = base.LovData.Where(l => l.Name == "REMARK_BY_PACKAGE_GROUP" && l.Text == Packgroup.Trim() && string.IsNullOrEmpty(l.LovValue3)).ToList();
            }
            List<LovScreenValueModel> screenValue;
            if (base.GetCurrentCulture().IsThaiCulture())
            {
                screenValue = config.Select(l => new LovScreenValueModel
                {
                    Name = l.Name,
                    PageCode = l.LovValue5,
                    DisplayValue = l.LovValue1,
                    LovValue3 = l.LovValue3,
                    GroupByPDF = l.LovValue4,
                    OrderByPDF = l.OrderBy,
                    Type = l.Type,
                    DefaultValue = l.DefaultValue
                }).ToList();
            }
            else
            {
                screenValue = config.Select(l => new LovScreenValueModel
                {
                    Name = l.Name,
                    PageCode = l.LovValue5,
                    DisplayValue = l.LovValue2,
                    LovValue3 = l.LovValue3,
                    GroupByPDF = l.LovValue4,
                    OrderByPDF = l.OrderBy,
                    Type = l.Type,
                    DefaultValue = l.DefaultValue
                }).ToList();
            }
            return Json(screenValue, JsonRequestBehavior.AllowGet);
        }

        [OutputCache(Location = OutputCacheLocation.Client, Duration = 30)]
        public JsonResult GetAllowPopup(string Packgrop, string line)
        {
            List<LovValueModel> config = base.LovData.Where(l => l.Name == "SHOW_PAGE_SELECT_VAS_" + line && Packgrop.Trim().Contains(l.Text)).ToList();
            List<LovScreenValueModel> screenValue;

            screenValue = config.Select(l => new LovScreenValueModel
            {
                Name = l.Name,
                PageCode = l.LovValue5,
                DisplayValue = l.LovValue1,
                LovValue3 = l.LovValue3,
                GroupByPDF = l.LovValue4,
                OrderByPDF = l.OrderBy,
                Type = l.Type,
                DefaultValue = l.DefaultValue
            }).ToList();

            return Json(screenValue, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult GetListTopup(string mobile = "", string topupExistGroup = "", string dataName = "")
        {
            //17.9 Speed boost
            List<LovValueModel> listTopupType = base.LovData.Where(l => l.Text == "TOPUP_GROUP").OrderBy(l => l.OrderBy).ToList();
            List<DatAutoOntopPlayboxModel> screenValue = new List<DatAutoOntopPlayboxModel>();
            List<string> GroupList = new List<string>();

            foreach (var topup in listTopupType)
            {
                List<LovValueModel> config = base.LovData.Where(l => l.Type == topup.Name).OrderBy(l => l.OrderBy).ToList();
                //byte[] bytes = (byte[])config[0].Image_blob;
                //string base64String = Convert.ToBase64String(config[0].Image_blob, 0, config[0].Image_blob.Length);


                var groupedData = config
                  .GroupBy(u => u.Name)
                  .Select(grp => grp.ToList())
                  .ToList();


                foreach (var a in groupedData)  // name group
                {
                    var tempmodel = new DatAutoOntopPlayboxModel();

                    tempmodel.Group = "";
                    tempmodel.TopupGroup = topup.Name;
                    foreach (var b in a)
                    {
                        tempmodel.DataName = b.Name;

                        if (b.Text == "ICON_PATH")
                            tempmodel.IconPath = b.Image_blob != null ? Convert.ToBase64String(b.Image_blob, 0, b.Image_blob.Length) : "";
                        //else if (b.Text == "LOGO_PATH")
                        //    tempmodel.LogoPath = b.Image_blob != null ? Convert.ToBase64String(b.Image_blob, 0, b.Image_blob.Length) : "";
                        else if (b.Text == "ICON_DES")
                            tempmodel.DataIconDes = base.GetCurrentCulture().IsThaiCulture() ? b.LovValue1.ToSafeString() : b.LovValue2.ToSafeString();
                        else if (b.Text == "DETAIL_HEADER1")
                            tempmodel.DetailHead1 = base.GetCurrentCulture().IsThaiCulture() ? b.LovValue1.ToSafeString() : b.LovValue2.ToSafeString();
                        else if (b.Text == "DETAIL_HEADER2")
                            tempmodel.DetailHead2 = base.GetCurrentCulture().IsThaiCulture() ? b.LovValue1.ToSafeString() : b.LovValue2.ToSafeString();
                        //else if (b.Text == "DETAIL_DES")
                        //{
                            //tempmodel.DetailDes = base.GetCurrentCulture().IsThaiCulture() ? b.LovValue1.ToSafeString() : b.LovValue2.ToSafeString()

                        else if (b.Text == "DETAIL_DES")
                        {
                            var ContentPlayboxDetail = a
                             .Where(x => x.Text == "DETAIL_DES")
                             .OrderBy(x => x.OrderBy)
                             .ToList();

                            var sb = new StringBuilder();
                            foreach (var item in ContentPlayboxDetail)
                            {
                                sb.Append(base.GetCurrentCulture().IsThaiCulture() ? item.LovValue1.ToSafeString() : item.LovValue2.ToSafeString());
                            }
                            tempmodel.DetailDes = sb.ToString();
                            
                            List<Package> pk = new List<Package>();

                            int index = 0;
                            if (!string.IsNullOrWhiteSpace(b.LovValue3))
                            {
                                if (b.LovValue3.IndexOf('|') > -1)
                                {
                                    string[] tra = b.LovValue3.Split('|');
                                    foreach (var item1 in tra)
                                    {
                                        Package p = new Package();
                                        p.Trial = item1;

                                        //
                                        if (!string.IsNullOrWhiteSpace(b.LovValue4))
                                        {
                                            if (b.LovValue4.IndexOf('|') > -1)
                                            {
                                                string[] top = b.LovValue4.Split('|');
                                                p.TopUp = top[index];
                                            }
                                        }
                                        pk.Add(p);
                                        index++;
                                    }
                                }
                                else
                                {
                                    Package p = new Package();
                                    p.Trial = b.LovValue3;
                                    p.TopUp = b.LovValue4;
                                    pk.Add(p);
                                }
                            }
                            else
                            {
                                // lov3 empty
                                if (!string.IsNullOrWhiteSpace(b.LovValue4))
                                {
                                    if (b.LovValue4.IndexOf('|') > -1)
                                    {
                                        string[] top = b.LovValue4.Split('|');
                                        foreach (var item1 in top)
                                        {
                                            Package p = new Package();
                                            p.TopUp = item1;
                                            pk.Add(p);
                                        }
                                    }
                                    else
                                    {
                                        Package p = new Package();
                                        p.TopUp = b.LovValue4;
                                        pk.Add(p);
                                    }
                                }
                            }
                            tempmodel.Package = pk;
                        }

                        else if (b.Text == "SELECT_GROUP")
                        {
                            tempmodel.Group = b.LovValue1;
                            tempmodel.TopupGroupName = base.GetCurrentCulture().IsThaiCulture() ? b.LovValue3.ToSafeString() : b.LovValue4.ToSafeString();
                        }
                        else if (b.Text == "GROUP_NAME")
                        {
                            int tmpOrderby = 0;
                            tempmodel.GroupName = base.GetCurrentCulture().IsThaiCulture() ? b.LovValue1.ToSafeString() : b.LovValue2.ToSafeString();
                            int.TryParse(b.DefaultValue, out tmpOrderby);
                            tempmodel.GroupOrderBy = tmpOrderby;
                        }

                    }
                    tempmodel.GroupName = tempmodel.GroupName.ToSafeString();
                    screenValue.Add(tempmodel);
                }
            }

            //R17.11 Officer for Existing Customer
            if (screenValue.Any() && !string.IsNullOrEmpty(topupExistGroup))
            {
                //Check Topup Type: TOPUP_FIBRE, TOPUP_PLAYBOX
                screenValue = screenValue.Where(w => w.TopupGroup == topupExistGroup).ToList();
            }

            if (screenValue.Any())
            {
                screenValue = screenValue.OrderBy(t => t.GroupOrderBy).ToList();
                GroupList = screenValue.Select(t => t.GroupName).Distinct().ToList();
            }

            if (dataName != "")
            {
                List<DatAutoOntopPlayboxModel> tmpScreenValues = new List<DatAutoOntopPlayboxModel>();

                string[] tmpDataName = dataName.Split(',');
                foreach (var item in tmpDataName)
                {
                    DatAutoOntopPlayboxModel tmpScreenValue = screenValue.FirstOrDefault(t => t.DataName == item);
                    if (tmpScreenValue != null)
                    {
                        tmpScreenValues.Add(tmpScreenValue);
                    }
                }
                screenValue = tmpScreenValues;
            }

            return Json(new { Data = screenValue, DataGroup = GroupList.ToArray() }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetListLogoTopup(string packageCode = "")
        {
            //get logopath lov 13/06/2025 chats885
            var listTopupType = base.LovData.Where(l => l.Text == "LOGO_PATH" && l.Name == packageCode).FirstOrDefault();
            var logoPath =  listTopupType.Image_blob != null ? Convert.ToBase64String(listTopupType.Image_blob, 0, listTopupType.Image_blob.Length) : "";

            return Json(new { Data = logoPath }, JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetPrivilegeByMobile(string mobile)
        {
            //check promotion
            var query = new evESeServiceQueryPrivilegeByMobileNoQuery()
            {
                mobileNo = mobile// "8800014295"

            };
            List<PrivilegePromotionModel> Privilege = new List<PrivilegePromotionModel>();
            var result = _queryProcessor.Execute(query);

            if (result.assetPromotionItemList.Count > 0 && result.assetPromotionItemList != null)
            {
                Privilege = result.assetPromotionItemList;
            }
            //end
            return Json(Privilege, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetpackageGroup(string Package)
        {
            List<LovValueModel> config = base.LovData.Where(l => l.Name == "VOUCHER_PACKAGE_GROUP" && l.Type == "FBB_CONSTANT" && l.LovValue1 == Package).ToList();

            var Name = config.Select(i => i.LovValue1).FirstOrDefault();

            return Json(Name, JsonRequestBehavior.AllowGet);
        }

        //FTTB Sell Router
        public JsonResult GetListModelRouter()
        {
            var screenValue = new List<DatListRouterModel>();

            var config = LovData.Where(l => l.Type == "ROUTER_FIBRE").OrderBy(l => l.OrderBy).ToList();

            var groupedData = config
                .GroupBy(u => u.Name)
                .Select(grp => grp.ToList())
                .ToList();

            foreach (var a in groupedData) // name group
            {
                var tempmodel = new DatListRouterModel { Group = "", TopupGroup = "ROUTER_FIBRE" };

                foreach (var b in a)
                {
                    tempmodel.DataName = b.Name;

                    if (b.Text == "ICON_PATH")
                        tempmodel.IconPath = b.Image_blob != null
                            ? Convert.ToBase64String(b.Image_blob, 0, b.Image_blob.Length)
                            : "";
                    else if (b.Text == "ICON_DES")
                        tempmodel.DataIconDes = base.GetCurrentCulture().IsThaiCulture()
                            ? b.LovValue1.ToSafeString()
                            : b.LovValue2.ToSafeString();
                    else if (b.Text == "PRICE")
                    {
                        tempmodel.TopupPrice = !string.IsNullOrEmpty(b.LovValue1)
                            ? b.LovValue1.ToSafeDecimal().ToSafeString()
                            : "0";
                        tempmodel.TopupPriceText = !string.IsNullOrEmpty(b.LovValue1)
                            ? b.LovValue1.ToSafeString()
                            : "0";
                    }
                    else if (b.Text == "SELECT_GROUP")
                    {
                        tempmodel.Group = b.LovValue1;
                        tempmodel.TopupGroupName = base.GetCurrentCulture().IsThaiCulture()
                            ? b.LovValue3.ToSafeString()
                            : b.LovValue4.ToSafeString();
                    }
                }
                screenValue.Add(tempmodel);
            }

            return Json(screenValue, JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetPromotionCode()
        {
            List<LovValueModel> config = base.LovData.Where(l => l.Name == "PROMOTION_CODE_DISCOUNT_POST").ToList();

            var promotionCode = config.Select(i => i.LovValue1).ToList();

            return Json(promotionCode, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetACC_CATEGORY(string ACC_CATEGORY = "")
        {
            List<LovValueModel> config = base.LovData.Where(l => l.Name == "MAPPING_ACC_CATEGORY" && l.Type == "FBB_CONSTANT" && l.LovValue1 == ACC_CATEGORY).ToList();

            var acc_category = config.Select(i => i.LovValue2).FirstOrDefault();

            return Json(acc_category, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidateGiftVoucherPIN(string PINCode)
        {
            bool chk;
            if (GenChkSum(PINCode.Substring(0, PINCode.Length - 1)) == PINCode.Substring(PINCode.Length - 1))
            {
                chk = true;
            }
            else
            {
                chk = false;
            }
            return Json(chk, JsonRequestBehavior.AllowGet);
        }

        public string GenChkSum(string input)
        {
            for (int i = 0; i <= 9; i++)
            {
                input = input.Replace(i.ToString(), Convert.ToChar(i).ToString());
            }
            byte[] asciiBytes = Encoding.ASCII.GetBytes(input);
            string asctmp = "";
            foreach (byte tmp in asciiBytes)
                asctmp += tmp.ToString();

            char[] _revInput = asctmp.ToCharArray();
            Array.Reverse(_revInput);
            int sum = 0;
            for (int i = 0; i < _revInput.Length; i++)
            {
                //if(i % 2 == 0)
                //{
                //    int inttmp = int.Parse(_revInput[i].ToString()) * 2;
                //    int devidetmp = inttmp / 10;
                //    int modtmp = inttmp % 10;
                //    sum += (devidetmp + modtmp);
                //}
                if (IsPrime(i))
                {
                    int inttmp = int.Parse(_revInput[i].ToString()) * 2;
                    int devidetmp = inttmp / 10;
                    int modtmp = inttmp % 10;
                    sum += (devidetmp + modtmp);
                }
                else
                {
                    sum += int.Parse(_revInput[i].ToString());
                }
            }
            return ((10 - (sum % 10)) % 10).ToString();
        }

        public bool IsPrime(int candidate)
        {
            if ((candidate & 1) == 0)
            {
                if (candidate == 2)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            for (int i = 3; (i * i) <= candidate; i += 2)
            {
                if ((candidate % i) == 0)
                {
                    return false;
                }
            }
            return candidate != 1;
        }

        public JsonResult CheckDateGiftVoucherPIN(string VoucherPIN)
        {
            var query = new GetDateVoucherPINQuery()
            {
                VoucherPIN = VoucherPIN
            };
            string statusPIN = _queryProcessor.Execute(query);

            return Json(statusPIN, JsonRequestBehavior.AllowGet);
        }

        public List<LovValueModel> GetLovConfigOptionInstall(string pageCode)
        {
            try
            {
                List<LovValueModel> config = null;
                if (pageCode == null)
                {
                    config = base.LovData.Where(l => l.LovValue5 == null && l.Type == "OPTION_INSTALL").ToList();
                }
                else if (pageCode == "ALLPAGE")
                {
                    config = base.LovData.Where(l => l.Type == "OPTION_INSTALL").ToList();
                }
                else
                {
                    config = base.LovData.Where(l =>
                        (!string.IsNullOrEmpty(l.Type) && l.Type == "OPTION_INSTALL")
                            && (!string.IsNullOrEmpty(l.LovValue5) && l.LovValue5.Equals(pageCode))).ToList();
                }

                return config;
            }
            catch (Exception ex)
            {
                Logger.Info(ex.GetErrorMessage());
                return new List<LovValueModel>();
            }
        }

        public List<LovValueModel> GetLovConfigOptionInstallData()
        {
            var screenData = GetLovConfigOptionInstall(WebConstants.LovConfigName.DisplayPackagePageCode);
            return screenData;
        }

        public List<LovValueModel> GetLovConfigShowOptionInstall()
        {
            try
            {
                List<LovValueModel> config = null;

                config = base.LovData.Where(l =>
                    (!string.IsNullOrEmpty(l.Type) && l.Type == "PACKAGE_GROUP")).ToList();

                return config;
            }
            catch (Exception ex)
            {
                Logger.Info(ex.GetErrorMessage());
                return new List<LovValueModel>();
            }
        }

        public List<LovValueModel> GetLovConfigShowOptionInstallData()
        {
            var screenData = GetLovConfigShowOptionInstall();
            return screenData;
        }

        public JsonNetResult GetLovConfigShowOptionInstallJsonData()
        {
            var jsonResult = new JsonNetResult();
            var screenData = GetLovConfigShowOptionInstall();
            jsonResult.Formatting = Newtonsoft.Json.Formatting.Indented;
            jsonResult.Data = screenData;

            return jsonResult;
        }

        [HttpPost]
        public JsonResult GetLovConfigDetailFreeInstall(string productCode)
        {
            string result = "";

            List<LovValueModel> config = null;

            config = base.LovData.Where(l =>
                (!string.IsNullOrEmpty(l.Name) && l.Name == "SELECT_FREE_INSTALL_VALUE" && !string.IsNullOrEmpty(l.LovValue3) && l.LovValue3 == productCode)).ToList();
            if (config != null && config.Count > 0)
            {
                if (SiteSession.CurrentUICulture.IsThaiCulture())
                {
                    result = config.FirstOrDefault().LovValue1.ToSafeString();
                }
                else
                {
                    result = config.FirstOrDefault().LovValue1.ToSafeString();
                }
            }
            else
            {
                config = base.LovData.Where(l =>
                (!string.IsNullOrEmpty(l.Name) && l.Name == "SELECT_FREE_INSTALL_VALUE" && string.IsNullOrEmpty(l.LovValue3))).ToList();
                if (SiteSession.CurrentUICulture.IsThaiCulture())
                {
                    result = config.FirstOrDefault().LovValue1.ToSafeString();
                }
                else
                {
                    result = config.FirstOrDefault().LovValue1.ToSafeString();
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private List<PackageModel> ValidatePackageModel(IEnumerable<PackageModel> listModel)
        {
            //R17.9 Speed boost
            var packageModel = new List<PackageModel>();
            var config = base.LovData.Where(l => l.LovValue1 == "FTTH" && l.Name == "MAPPING_OWNER_PRODUCT").ToList();

            foreach (var item in listModel)
            {
                var lovValue = config.FirstOrDefault(val => val.LovValue3 == item.OWNER_PRODUCT);
                if (lovValue == null)
                {
                    //OWNER_PRODUCT != FTTH 
                    //if (item.PRODUCT_SUBTYPE3 != "TOPUP1DAYSPEEDBOOST" && item.PRODUCT_SUBTYPE3 != "TOPUP3DAYSPEEDBOOST" && item.PRODUCT_SUBTYPE3 != "TOPUP7DAYSPEEDBOOST")
                    //{
                    //    packageModel.Add(item);
                    //}
                    var rgx = new Regex(@"TOPUP\w+SPEEDBOOST\b", RegexOptions.IgnoreCase);
                    var matches = rgx.Matches(item.PRODUCT_SUBTYPE3);
                    if (matches.Count <= 0)
                    {
                        packageModel.Add(item);
                    }

                }
                else
                {
                    //WNER_PRODUCT = FTTH
                    packageModel.Add(item);
                }
            }

            return packageModel;
        }


        public JsonResult CheckPopupFMC(string PromotionCode = "", string ExistingMobileFlag = "")
        {
            var query = new CheckPopupFMCQuery()
            {
                p_sff_promotion_code = PromotionCode,
                p_existing_mobile_flag = ExistingMobileFlag
            };
            string ResultValue = _queryProcessor.Execute(query);

            return Json(ResultValue, JsonRequestBehavior.AllowGet);
        }

        public CheckPrivilegeQueryModel CheckPrivilege(string MobileNo = "", string ShortCodeLovName = "")
        {
            CheckPrivilegeQuery query = new CheckPrivilegeQuery
            {
                MobileNo = MobileNo,
                ShortCodeLovName = ShortCodeLovName,
                FullURL = Session["FullUrl"].ToSafeString()
            };
            CheckPrivilegeQueryModel resultData = _queryProcessor.Execute(query);
            return resultData;
        }

        [HttpPost]
        public JsonResult CheckPrivilegePoint(string InternetNo = "", string MobileNo = "", string PaymentOrderID = "")
        {
            var controller = DependencyResolver.Current.GetService<ProcessController>();

            string InternetNoPiont = "0";
            string MobileNoPoint = "0";

            string url = "";

            List<LovValueModel> lovConfigData = new List<LovValueModel>();
            lovConfigData = LovData.Where(t => t.Name == "getPointSet" && t.LovValue5 == "FBBOR041" && t.Type == "FBB_CONSTANT").ToList();
            if (lovConfigData.Count > 0)
            {
                url = lovConfigData.Where(t => t.Text == "url").Select(t => t.LovValue1).Any()
                    ? lovConfigData.Where(t => t.Text == "url").Select(t => t.LovValue1).FirstOrDefault()
                    : "";
            }

            MobileNo = MobileNo.Replace("-", "");

            SavePaymentLogModel savePaymentLogNew = new SavePaymentLogModel()
            {
                ACTION = "New",
                PROCESS_NAME = "getPointSet",
                PAYMENT_ORDER_ID = PaymentOrderID,
                ENDPOINT = url,
                REQ_COMMAND = "getPointSet",
                REQ_MOBILE_NO = MobileNo + "|" + InternetNo
            };
            controller.SavePaymentLog(savePaymentLogNew);
            SavePaymentLogModel savePaymentLogUpdate;
            CheckPrivilegePointQueryModel CheckPrivilegeData = new CheckPrivilegePointQueryModel();
            CheckPrivilegePointQuery query = new CheckPrivilegePointQuery
            {
                FullURL = "",
                InternetNo = InternetNo,
                MobileNo = MobileNo,
                PaymentOrderID = PaymentOrderID
            };
            CheckPrivilegeData = _queryProcessor.Execute(query);
            if (CheckPrivilegeData != null)
            {
                if (CheckPrivilegeData.PrivilegePointList != null && CheckPrivilegeData.PrivilegePointList.Count() > 0)
                {
                    var InternetNoData = CheckPrivilegeData.PrivilegePointList.FirstOrDefault(t => t.msisdn == InternetNo);
                    if (InternetNoData != null)
                    {
                        decimal InternetNoPiontSum = InternetNoData.points + InternetNoData.pointsBonus;
                        InternetNoPiont = InternetNoPiontSum.ToString();
                    }
                    var MobileNoData = CheckPrivilegeData.PrivilegePointList.FirstOrDefault(t => t.msisdn == MobileNo);
                    if (MobileNoData != null)
                    {
                        decimal MobileNoPiontSum = MobileNoData.points + MobileNoData.pointsBonus;
                        MobileNoPoint = MobileNoPiontSum.ToString();
                    }

                    var jsonSerialiser = new JavaScriptSerializer();
                    string json = jsonSerialiser.Serialize(CheckPrivilegeData.PrivilegePointList);

                    savePaymentLogUpdate = new SavePaymentLogModel()
                    {
                        ACTION = "Modify",
                        PROCESS_NAME = "getPointSet",
                        PAYMENT_ORDER_ID = PaymentOrderID,
                        ENDPOINT = url,
                        RESP_STATUS = CheckPrivilegeData.HttpStatus.ToSafeString(),
                        RESP_RESP_CODE = CheckPrivilegeData.Status.ToSafeString(),
                        RESP_RESP_DESC = CheckPrivilegeData.Description.ToSafeString(),
                        RESP_REF1 = json
                    };
                }
                else
                {
                    savePaymentLogUpdate = new SavePaymentLogModel()
                    {
                        ACTION = "Modify",
                        PROCESS_NAME = "getPointSet",
                        PAYMENT_ORDER_ID = PaymentOrderID,
                        ENDPOINT = url,
                        RESP_STATUS = CheckPrivilegeData.HttpStatus.ToSafeString(),
                        RESP_RESP_CODE = CheckPrivilegeData.Status.ToSafeString(),
                        RESP_RESP_DESC = CheckPrivilegeData.Description.ToSafeString(),
                        RESP_REF1 = "No Data"
                    };
                }
            }
            else
            {

                savePaymentLogUpdate = new SavePaymentLogModel()
                {
                    ACTION = "Modify",
                    PROCESS_NAME = "getPointSet",
                    PAYMENT_ORDER_ID = PaymentOrderID,
                    ENDPOINT = url,
                    RESP_STATUS = "",
                    RESP_RESP_CODE = "",
                    RESP_RESP_DESC = "CheckPrivilegeData is null",
                    RESP_REF1 = "No Data"
                };
            }
            controller.SavePaymentLog(savePaymentLogUpdate);

            return Json(new { InternetNoPiont = InternetNoPiont, MobileNoPoint = MobileNoPoint }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckPremiumArea(string SubDistrict = "", string District = "", string Province = "", string PostalCode = "", string Language = "")
        {
            var fmpaCfg = LovData.Where(
                        l => (!string.IsNullOrEmpty(l.Type) && l.Name == "HVR_USE_FLAG")).ToList();

            var flagHVR = fmpaCfg.Where(l => l.LovValue1 == "Y").Select(o => o.LovValue1).FirstOrDefault() ?? "N";

            PremiumAreaModel result = new PremiumAreaModel();

            if (flagHVR == "Y")
            {
                var query = new GetPremiumAreaHVRQuery()
                {
                    SubDistrict = SubDistrict.ToSafeString(),
                    District = District.ToSafeString(),
                    Province = Province.ToSafeString(),
                    PostalCode = PostalCode.ToSafeString(),
                    Language = Language.ToSafeString()
                };

                result = _queryProcessor.Execute(query);
            }
            else 
            {
                //19.10 Premium Install Check call package
                var query = new GetPremiumAreaQuery()
                {
                    SubDistrict = SubDistrict.ToSafeString(),
                    District = District.ToSafeString(),
                    Province = Province.ToSafeString(),
                    PostalCode = PostalCode.ToSafeString(),
                    Language = Language.ToSafeString()
                };

                result = _queryProcessor.Execute(query);
            }
            
            var ReturnPremiumConfig = new List<PremiumConfigModel>();

            var p = result.ReturnPremiumConfig.Count;

            if (result.ReturnPremiumConfig.Count > 0)
            {
                ReturnPremiumConfig = result.ReturnPremiumConfig.Select(m => new PremiumConfigModel()
                {
                    Region = m.Region,
                    ProvinceTh = m.ProvinceTh,
                    DistrictTh = m.DistrictTh,
                    SubdistrictTh = m.SubdistrictTh,
                    ProvinceEn = m.ProvinceEn,
                    DistrictEn = m.DistrictEn,
                    SubdistrictEn = m.SubdistrictEn,
                    Postcode = m.Postcode
                }).ToList();
            }

            return Json(new
            {
                outReturnCode = result.ReturnCode,
                outReturnMessage = result.ReturnMessage,
                outReturnPremiumConfig = ReturnPremiumConfig
            }, JsonRequestBehavior.AllowGet);
        }

        //R22.03 TopupReplace
        public JsonResult CheckHavePlayboxNonATV(string AisAirNumber = "", string Lang = "", string AddressID = "")
        {
            string FullUrl = Session["FullUrl"] != null ? Session["FullUrl"].ToSafeString() : "";

            #region Get IP Address Interface Log

            // Get IP Address
            string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress)) ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            string transactionId = AisAirNumber + ipAddress;

            #endregion

            var query = new GetFBSSQueryCPEPenaltyQuery
            {
                OPTION = "1",
                FIBRENET_ID = AisAirNumber,
                SERIAL_NO = "",
                STATUS = "4",
                MAC_ADDRESS = "",
                TransactionId = transactionId,
                FullUrl = FullUrl
            };

            List<FBSSQueryCPEPenaltyModel> dataCPEPenalty = _queryProcessor.Execute(query);

            CheckATVTopupReplaceModel dataCheckATV = null;

            if (dataCPEPenalty != null)
            {
                var query2 = new GetCheckATVTopupReplaceQuery
                {
                    P_FIBRENET_ID = AisAirNumber,
                    P_FLAG_LANG = Lang,
                    P_ADDRESS_ID = AddressID,
                    TransactionId = transactionId,
                    FullUrl = FullUrl
                };
                query2.Fbbor050PlayboxList = new List<Fbbor050PlayboxArrayModel>();

                foreach (var i in dataCPEPenalty)
                {
                    Fbbor050PlayboxArrayModel fbbor050PlayboxArrayModel = new Fbbor050PlayboxArrayModel
                    {
                        CPE_TYPE = i.CPE_TYPE,
                        CPE_MODEL_NAME = i.CPE_MODEL_NAME,
                        STATUS_DESC = i.STATUS_DESC,
                        CPE_BRAND_NAME = i.CPE_BRAND_NAME,
                        CPE_MODEL_ID = i.CPE_MODEL_ID,
                        CPE_GROUP_TYPE = i.CPE_GROUP_TYPE,
                        SN_PATTERN = i.SN_PATTERN,
                        SERIAL_NO = i.SERIAL_NO,
                        STATUS = i.STATUS
                    };
                    query2.Fbbor050PlayboxList.Add(fbbor050PlayboxArrayModel);
                }

                dataCheckATV = _queryProcessor.Execute(query2);

                if (dataCheckATV != null && dataCheckATV.RETURN_SERIAL_CURROR != null && dataCheckATV.RETURN_SERIAL_CURROR.Count() > 0)
                {
                    List<SerialATV> serialATVList = dataCheckATV.RETURN_SERIAL_CURROR.Select(c => new SerialATV
                    {
                        FIBRENETID = c.FIBRENETID.ToSafeString(),
                        SERIAL_NO = c.SERIAL_NO.ToSafeString()
                    }).ToList();

                    dataCheckATV.RETURN_SERIAL_CURROR = serialATVList.ToList();
                }
            }

            return Json(dataCheckATV, JsonRequestBehavior.AllowGet);
        }

    }
}
