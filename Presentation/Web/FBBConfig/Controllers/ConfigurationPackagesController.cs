using FBBConfig.Extensions;
using FBBConfig.Solid.CompositionRoot;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Commands.FBBWebConfigCommands;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBContract.Queries.FBBWebConfigQueries.AirWirelessCoverage;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace FBBConfig.Controllers
{
    [IENoCache(Order = 1)]
    public class ConfigurationPackagesController : FBBConfigController
    {
        //
        // GET: /ConfigurationPackage/
        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<SaveConfigPackagePage1Command> _SaveConfigurationPackagePage1Command;
        private readonly ICommandHandler<SaveVendorPartnerCommand> _SaveVendorPartnerCommand;
        private readonly ICommandHandler<SavePackageGroupDescCommand> _SavePackageGroupDescCommand;
        private readonly ICommandHandler<SavePackageMappingCommand> _SavePackageMappingCommand;
        private readonly ICommandHandler<SavePackageUserGroupCommand> _SavePackageUserGroupCommand;
        private readonly ICommandHandler<SavePackageLocationCommand> _SavePackageLocationCommand;

        public ConfigurationPackagesController(ILogger logger,
             IQueryProcessor queryProcessor,
            ICommandHandler<SaveConfigPackagePage1Command> saveConfigurationPackagePage1Command,
            ICommandHandler<SaveVendorPartnerCommand> saveVendorPartnerCommand,
            ICommandHandler<SavePackageGroupDescCommand> savePackageGroupDescCommand,
            ICommandHandler<SavePackageMappingCommand> savePackageMappingCommand,
            ICommandHandler<SavePackageUserGroupCommand> savePackageUserGroupCommand,
            ICommandHandler<SavePackageLocationCommand> savePackageLocationCommand)
        {
            base._Logger = logger;
            _queryProcessor = queryProcessor;
            _SaveConfigurationPackagePage1Command = saveConfigurationPackagePage1Command;
            _SaveVendorPartnerCommand = saveVendorPartnerCommand;
            _SavePackageGroupDescCommand = savePackageGroupDescCommand;
            _SavePackageMappingCommand = savePackageMappingCommand;
            _SavePackageUserGroupCommand = savePackageUserGroupCommand;
            _SavePackageLocationCommand = savePackageLocationCommand;


        }

        [AuthorizeUserAttribute]
        public ActionResult Index()
        {
            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "Account");
            ViewBag.User = base.CurrentUser;
            SetViewBagLov();

            ConfigurationPackagesModel Model = new ConfigurationPackagesModel();

            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-us");
            DateTime Today = DateTime.Now;
            Model.SALE_START_DATE = Today.ToString("dd/MM/yyyy");
            Model.SALE_END_DATE = Today.AddYears(5).ToString("dd/MM/yyyy");

            // Add ListPackageType

            Model.ListPackageType = new List<PackageType>();
            Model.ListPackageType = SetProductTypeTable();

            // Add ListVendorOrPartner
            Model.ListVendorOrPartner = new List<VendorOrPartner>();
            Model.ListVendorOrPartner = SetVendorOrPartner();

            // Add ListCatalogAndAuthorize
            Model.ListCatalogAndAuthorizeTable = new List<CatalogAndAuthorizeTable>();

            // Add ListRegionTable
            Model.ListRegionTable = new List<RegionTable>();
            Model.RegionUse = new List<RegionTable>();

            // Add ListTechnologyTable
            Model.ListTechnologyTable = new List<TechnologyTable>();

            Model.StatusPage = (int)StatusPages.SearchPage;
            Model.VendorOrPartnerShow = "False";
            Model.SaveStatus = "N";

            return View(Model);
        }

        [AuthorizeUserAttribute]
        public ActionResult GetConfigurationPackageToView(string ProductCode, string PackageCode)
        {
            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "Account");
            ViewBag.User = base.CurrentUser;
            SetViewBagLov();

            if (ProductCode == "undefined")
                ProductCode = "";

            ConfigurationPackagesModel Model = GetConfigurationPackage((int)StatusPages.SavePage1, ProductCode, PackageCode);
            Model.StatusPage = (int)StatusPages.SavePage1;
            Model.SaveStatus = "E";

            var ListPackageTypeModelView = SetProductTypeTable();
            foreach (var PackageTypeModelView in ListPackageTypeModelView)
            {
                Session["PackageGroupTmp" + PackageTypeModelView.PACKAGE_TYPE] = null;
            }
            Session["ProductSubtype3Tmp"] = null;

            return View("Index", Model);
        }

        [AuthorizeUserAttribute]
        public ActionResult GetConfigurationPackageToViewSelectPage(int PageNo, string ProductCode, string PackageCode)
        {
            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "Account");
            ViewBag.User = base.CurrentUser;
            SetViewBagLov();

            ConfigurationPackagesModel Model = GetConfigurationPackage(PageNo, ProductCode, PackageCode);
            Model.StatusPage = PageNo;
            Model.SaveStatus = "E";

            return View("Index", Model);
        }

        public ConfigurationPackagesModel GetConfigurationPackage(int PageNO, string ProductCode = "", string PackageCode = "")
        {

            List<ConfigurationPackageDetail> result1 = GetAWConfigurationPackageDetailEdit(ProductCode, PackageCode);
            List<ProductTypePackage> result2 = GetAWConfigurationPackageProductTypeEdit(PackageCode);
            List<VendorPartner> result3 = GetAWConfigurationPackageVendorEdit(PackageCode);

            ///// setModel For View

            ConfigurationPackagesModel Model = new ConfigurationPackagesModel();
            Model.PACKAGE_CODE_CATALOG = PackageCode;
            Model.PACKAGE_CODE_MAPPING = PackageCode;
            Model.PACKAGE_CODE_LOCATION = PackageCode;
            Model.PACKAGE_CODE_REPORT = PackageCode;

            // Add Detal

            if (result1.Count > 0)
            {
                var tmpConfigurationPackageDetail = result1.FirstOrDefault();

                Model.SFFProductCode = tmpConfigurationPackageDetail.sff_promotion_code;
                Model.SFFProductNameThai = tmpConfigurationPackageDetail.sff_promotion_bill_tha;
                Model.SFFProductNameEng = tmpConfigurationPackageDetail.sff_promotion_bill_eng;
                Model.PACKAGE_TYPE = tmpConfigurationPackageDetail.package_type;
                Model.PACKAGE_CLASS = tmpConfigurationPackageDetail.package_class;
                Model.VAS_SERVICE = tmpConfigurationPackageDetail.vas_service;
                Model.PACKAGE_FOR = tmpConfigurationPackageDetail.product_subtype2;
                Model.PACKAGE_CODE = tmpConfigurationPackageDetail.package_code;
                Model.PACKAGE_DISPLAY_WEB = tmpConfigurationPackageDetail.technology;
                Model.PACKAGE_NAME_THAI = tmpConfigurationPackageDetail.package_name_tha;
                Model.PACKAGE_NAME_ENG = tmpConfigurationPackageDetail.package_name_eng;
                Model.DOWNLOAD_SPEED = tmpConfigurationPackageDetail.download_speed;
                Model.UPLOAD_SPEED = tmpConfigurationPackageDetail.upload_speed;
                Model.SALE_START_DATE = tmpConfigurationPackageDetail.sale_start_date;
                Model.SALE_END_DATE = tmpConfigurationPackageDetail.sale_end_date;
                Model.PRE_INITIATION_CHARGE = tmpConfigurationPackageDetail.pre_initiation_charge;
                Model.INITIATION_CHARGE = tmpConfigurationPackageDetail.initiation_charge;
                Model.PRE_RECURRING_CHARGE = tmpConfigurationPackageDetail.pre_recurring_charge;
                Model.RECURRING_CHARGE = tmpConfigurationPackageDetail.recurring_charge;
                Model.DISCOUNT_TYPE = tmpConfigurationPackageDetail.discount_type;
                Model.DISCOUNT_VALUE = tmpConfigurationPackageDetail.discount_value;
                Model.DISCOUNT_DAY = tmpConfigurationPackageDetail.discount_day;
                Model.PRODUCT_TYPE = tmpConfigurationPackageDetail.package_type;

                Model.SFFProductCodeMapping = tmpConfigurationPackageDetail.sff_promotion_code;
                Model.SFFProductCodeCatalog = tmpConfigurationPackageDetail.sff_promotion_code;
                Model.SFFProductCodeLocation = tmpConfigurationPackageDetail.sff_promotion_code;
                Model.SFFProductCodeReport = tmpConfigurationPackageDetail.sff_promotion_code;

                // End

                // Add Data For Edit To ListPackageType

                Model.ListPackageType = new List<PackageType>();
                if (PageNO == 1)
                {
                    var ListPackageTypeModelView = SetProductTypeTable();
                    foreach (var PackageTypeModelView in ListPackageTypeModelView)
                    {
                        var TempData = result2.Where(t => t.product_subtype == PackageTypeModelView.PACKAGE_TYPE).ToList();
                        if (TempData.Count() > 0)
                        {
                            string tmpPackageSubType3 = "";
                            if (PackageTypeModelView.PACKAGE_TYPE_DISPLAY == "PBOX")
                            {
                                tmpPackageSubType3 = TempData.FirstOrDefault().product_subtype3;
                                PackageTypeModelView.PRODUCT_SUBTYPE3 = tmpPackageSubType3;
                            }

                            string tmpPackageGroup = TempData.FirstOrDefault().package_group;

                            PackageTypeModelView.PACKAGE_GROUP = tmpPackageGroup;
                            List<PackageGroupDesc> PackageGroupDescList = GetPackageGroupDescriptionByPackageGroup(tmpPackageGroup);
                            PackageGroupDesc packageGroupDesc = new PackageGroupDesc();
                            if (PackageGroupDescList.Count > 0)
                            {
                                packageGroupDesc = PackageGroupDescList.FirstOrDefault();
                            }
                            PackageTypeModelView.PACKAGE_GROUP_DESCRIPTION_THAI = packageGroupDesc.PackageGroupDescriptionThai;
                            PackageTypeModelView.PACKAGE_GROUP_DESCRIPTION_THAI_OLD = packageGroupDesc.PackageGroupDescriptionThai;
                            PackageTypeModelView.PACKAGE_GROUP_DESCRIPTION_ENG = packageGroupDesc.PackageGroupDescriptionEng;
                            PackageTypeModelView.PACKAGE_GROUP_DESCRIPTION_ENG_OLD = packageGroupDesc.PackageGroupDescriptionEng;
                            PackageTypeModelView.PACKAGE_SELECT = true;
                            PackageTypeModelView.PACKAGE_SELECTOld = true;
                        }
                        else
                        {
                            PackageTypeModelView.PACKAGE_GROUP_DESCRIPTION_THAI = "";
                            PackageTypeModelView.PACKAGE_GROUP_DESCRIPTION_THAI_OLD = "";
                            PackageTypeModelView.PACKAGE_GROUP_DESCRIPTION_ENG = "";
                            PackageTypeModelView.PACKAGE_GROUP_DESCRIPTION_ENG_OLD = "";
                            PackageTypeModelView.PACKAGE_SELECT = false;
                            PackageTypeModelView.PACKAGE_SELECTOld = false;
                        }

                    }
                    Model.ListPackageType = ListPackageTypeModelView;
                }
                else
                {
                    var ListPackageTypeModelView = SetProductTypeTable();
                    foreach (var PackageTypeModelView in ListPackageTypeModelView)
                    {
                        Session["PackageGroupTmp" + PackageTypeModelView.PACKAGE_TYPE] = null;
                    }
                    Session["ProductSubtype3Tmp"] = null;
                }
                // End

                bool CheckHaveVendor = false;
                string[] codepackagetype = { "FTTx", "PBOX", "VOIP" };
                var tmpCheckHaveVendor = result2.Where(t => codepackagetype.Contains(t.product_subtype)).ToList();
                if (tmpCheckHaveVendor.Count > 0)
                {
                    CheckHaveVendor = true;
                }

                if (CheckHaveVendor)
                {
                    Model.VendorOrPartnerShow = "True";
                }
                else
                {
                    Model.VendorOrPartnerShow = "False";
                }

                // Add Data For Edit To  ListVendorOrPartner
                Model.ListVendorOrPartner = new List<VendorOrPartner>();
                if (PageNO == 1)
                {
                    var ListVendorOrPartnerModelView = SetVendorOrPartner();
                    foreach (var VendorOrPartnerModelView in ListVendorOrPartnerModelView)
                    {
                        var TempData = result3.Where(t => t.owner_product == VendorOrPartnerModelView.VendorOrPartnerName).ToList();
                        if (TempData.Count() > 0)
                        {
                            VendorOrPartnerModelView.VendorOrPartnerSelect = true;
                            VendorOrPartnerModelView.VendorOrPartnerSelectOld = true;
                        }
                        else
                        {
                            VendorOrPartnerModelView.VendorOrPartnerSelect = false;
                            VendorOrPartnerModelView.VendorOrPartnerSelectOld = false;
                        }

                    }
                    Model.ListVendorOrPartner = ListVendorOrPartnerModelView;
                }
                // End

                Session["PackageTypeSearchTmp"] = null;
                if (PageNO == 2)
                {
                    if (Model.PACKAGE_TYPE == "2" || Model.PACKAGE_TYPE == "4")
                    {
                        Model.PackageTypeSearchShow = "N";
                    }
                    else if (Model.PACKAGE_TYPE == "1" || Model.PACKAGE_TYPE == "3" || Model.PACKAGE_TYPE == "5")
                    {
                        Model.PackageTypeSearchShow = "Y";
                        Session["PackageTypeSearchTmp"] = GetPackageTypeUse(PackageCode);
                    }
                    else
                    {
                        Model.PackageTypeSearchShow = "N";
                    }
                }

                // Add ListCatalogAndAuthorize
                Model.ListCatalogAndAuthorizeTable = new List<CatalogAndAuthorizeTable>();
                if (PageNO == 3)
                {
                    Model.ListCatalogAndAuthorizeTable = SetCatalogAndAuthorize(Model.PACKAGE_CODE);
                }
                // End

                Model.ListRegionTable = new List<RegionTable>();
                Model.RegionUse = new List<RegionTable>();
                Model.ListTechnologyTable = new List<TechnologyTable>();
                Session["ProvinceTmp"] = null;
                Session["BuildingTmp"] = null;
                if (PageNO == 4)
                {
                    Model.ListRegionTable = SetZipCode();
                    List<RegionTable> RegionUse = GetRegionByPackageCode(PackageCode);
                    Model.RegionUse = RegionUse.Distinct().ToList();
                    Session["ProvinceTmp"] = GetProvinceUseByPackageCode(PackageCode);
                    Session["BuildingTmp"] = SetBuildingTableUse(PackageCode);
                    Model.ListTechnologyTable = GetTechnology();
                }

                if (PageNO == 5)
                {

                }
            }

            Model.SaveStatus = "E";
            return Model;

        }

        [ValidateInput(false)]
        public ActionResult SaveConfigurationPackagePage1(ConfigurationPackagesModel Model)
        {
            string SaveResult = "";
            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "Account");

            var User = base.CurrentUser;
            if (Model != null)
            {
                var command = new SaveConfigPackagePage1Command();
                command.user = User.UserName;
                command.service_option = Model.SaveStatus;
                command.package_code = Model.PACKAGE_CODE;
                command.package_type = Model.PACKAGE_TYPE;
                if (Model.SaveStatus == "N")
                {
                    if (Model.PACKAGE_TYPE == "1" || Model.PACKAGE_TYPE == "3" || Model.PACKAGE_TYPE == "5")
                    {
                        command.package_class = "4";
                    }
                    else if (Model.PACKAGE_TYPE == "2" || Model.PACKAGE_TYPE == "4")
                    {
                        command.package_class = "2";
                    }
                    else
                    {
                        command.package_class = "";
                    }
                }
                else
                {
                    command.package_class = Model.PACKAGE_CLASS;
                }
                command.sale_start_date = Model.SALE_START_DATE;
                command.sale_end_date = Model.SALE_END_DATE;
                command.pre_initiation_charge = Model.PRE_INITIATION_CHARGE;
                command.initiation_charge = Model.INITIATION_CHARGE;
                command.pre_recurring_charge = Model.PRE_RECURRING_CHARGE;
                command.recurring_charge = Model.RECURRING_CHARGE;
                command.package_name_tha = Model.PACKAGE_NAME_THAI;
                command.package_name_eng = Model.PACKAGE_NAME_ENG;
                command.sff_promotion_code = Model.SFFProductCode;
                command.sff_promotion_bill_tha = Model.SFFProductNameThai;
                command.sff_promotion_bill_eng = Model.SFFProductNameEng;
                command.download_speed = Model.DOWNLOAD_SPEED;
                command.upload_speed = Model.UPLOAD_SPEED;
                command.discount_type = Model.DISCOUNT_TYPE;
                command.discount_value = Model.DISCOUNT_VALUE;
                command.discount_day = Model.DISCOUNT_DAY;
                command.vas_service = Model.VAS_SERVICE;
                command.product_subtype2 = Model.PACKAGE_FOR;
                command.technology = Model.PACKAGE_DISPLAY_WEB;

                List<AirPackageDetail> ListAirPackageDetail = new List<AirPackageDetail>();
                foreach (var item in Model.ListPackageType.Where(t => t.PACKAGE_SELECT == true || t.PACKAGE_SELECTOld == true))
                {
                    AirPackageDetail airPackageDetail = new AirPackageDetail();
                    if (item.PACKAGE_SELECTOld == false && item.PACKAGE_SELECT == true)
                    {
                        airPackageDetail.service_option = "N";
                    }
                    else if (item.PACKAGE_SELECTOld == true && item.PACKAGE_SELECT == true)
                    {
                        airPackageDetail.service_option = "E";
                    }
                    else if (item.PACKAGE_SELECTOld == true && item.PACKAGE_SELECT == false)
                    {
                        airPackageDetail.service_option = "D";
                    }
                    airPackageDetail.package_code = Model.PACKAGE_CODE;
                    airPackageDetail.product_type = "Internet";
                    airPackageDetail.product_subtype = item.PACKAGE_TYPE;
                    airPackageDetail.product_subtype3 = item.PRODUCT_SUBTYPE3;
                    airPackageDetail.package_group = item.PACKAGE_GROUP;
                    airPackageDetail.network_type = item.NETWORK_TYPE;
                    airPackageDetail.service_day_stary = item.SERVICE_DAY_STARY;
                    airPackageDetail.service_day_end = item.SERVICE_DAY_END;

                    ListAirPackageDetail.Add(airPackageDetail);
                }
                command.airPackageDetail = ListAirPackageDetail;

                List<AirPackageVendor> ListAirPackageVendor = new List<AirPackageVendor>();
                foreach (var item in Model.ListVendorOrPartner.Where(t => t.VendorOrPartnerSelect == true || t.VendorOrPartnerSelectOld == true))
                {
                    AirPackageVendor airPackageVendor = new AirPackageVendor();
                    if (item.VendorOrPartnerSelectOld == false && item.VendorOrPartnerSelect == true)
                    {
                        airPackageVendor.service_option = "N";
                    }
                    else if (item.VendorOrPartnerSelectOld == true && item.VendorOrPartnerSelect == true)
                    {
                        if (Model.VendorOrPartnerShow == "True")
                        {
                            airPackageVendor.service_option = "E";
                        }
                        else
                        {
                            airPackageVendor.service_option = "D";
                        }
                    }
                    else if (item.VendorOrPartnerSelectOld == true && item.VendorOrPartnerSelect == false)
                    {
                        airPackageVendor.service_option = "D";
                    }
                    airPackageVendor.owner_product = item.VendorOrPartnerValue;

                    ListAirPackageVendor.Add(airPackageVendor);
                }
                command.airPackageVendor = ListAirPackageVendor;

                List<PackageType> ListPackageTypeNewDesc = Model.ListPackageType.Where(t => t.PACKAGE_SELECT == true && (t.PACKAGE_GROUP_DESCRIPTION_ENG != t.PACKAGE_GROUP_DESCRIPTION_ENG_OLD || t.PACKAGE_GROUP_DESCRIPTION_THAI != t.PACKAGE_GROUP_DESCRIPTION_THAI_OLD)).ToList();

                try
                {
                    _SaveConfigurationPackagePage1Command.Handle(command);
                    if (Model.SaveStatus == "N")
                    {
                        Model.PACKAGE_CODE = command.package_code;
                    }
                    if (ListPackageTypeNewDesc.Count > 0)
                    {
                        foreach (var item in ListPackageTypeNewDesc)
                        {
                            SavePackageGroupDescCommand packageGroupDescCommand = new SavePackageGroupDescCommand();
                            packageGroupDescCommand.package_group = item.PACKAGE_GROUP;
                            packageGroupDescCommand.package_group_desc_eng = item.PACKAGE_GROUP_DESCRIPTION_ENG;
                            packageGroupDescCommand.package_group_desc_thai = item.PACKAGE_GROUP_DESCRIPTION_THAI;
                            packageGroupDescCommand.user = User.UserName;
                            _SavePackageGroupDescCommand.Handle(packageGroupDescCommand);
                        }
                    }

                    SaveResult = "SaveSuccess";
                }
                catch (Exception ex)
                {
                    SaveResult = "SaveFail";
                }

                Model.SaveResult = SaveResult;
            }

            /// Set to page2

            ViewBag.User = base.CurrentUser;
            if (Model.SaveToPage2 == "N")
            {
                Model.StatusPage = 1;
                return RedirectToAction("GetDataToReport", Model);
                //return RedirectToAction("Index", new { SaveStatus = SaveStatus });
            }
            else
            {
                Model.StatusPage = 2;
                Model.SFFProductCodeMapping = Model.SFFProductCode;
                Model.PACKAGE_CODE_MAPPING = Model.PACKAGE_CODE;
                return RedirectToAction("GetDataToPage2", Model);
            }

        }

        public ActionResult GetDataToPage2(ConfigurationPackagesModel Model)
        {
            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "Account");
            ViewBag.User = base.CurrentUser;
            SetViewBagLov();

            ConfigurationPackagesModel configurationPackagesModel = GetConfigurationPackage((int)StatusPages.SavePage2, Model.SFFProductCode, Model.PACKAGE_CODE);
            configurationPackagesModel.StatusPage = (int)StatusPages.SavePage2;

            if (configurationPackagesModel.PACKAGE_TYPE == "2" || configurationPackagesModel.PACKAGE_TYPE == "4")
            {
                configurationPackagesModel.PackageTypeSearchShow = "N";
            }
            else if (configurationPackagesModel.PACKAGE_TYPE == "1" || configurationPackagesModel.PACKAGE_TYPE == "3" || configurationPackagesModel.PACKAGE_TYPE == "5")
            {
                configurationPackagesModel.PackageTypeSearchShow = "Y";
            }
            else
            {
                configurationPackagesModel.PackageTypeSearchShow = "N";
            }

            if (Model.SaveResult != "")
            {
                if (Model.SaveResult == "SaveSuccess")
                {
                    ViewBag.Success = Model.SaveResult;
                }
                else
                {
                    ViewBag.Error = Model.SaveResult;
                }
            }

            return View("Index", configurationPackagesModel);
        }

        public ActionResult SaveConfigurationPackagePage2(ConfigurationPackagesModel Model)
        {
            string SaveResult = "";
            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "Account");

            var User = base.CurrentUser;

            List<PackageTypeUse> packageTypeUseOldList = new List<PackageTypeUse>();
            packageTypeUseOldList = GetPackageTypeUse(Model.PACKAGE_CODE_MAPPING);
            List<PackageTypeUse> packageTypeUseList = new List<PackageTypeUse>();
            if (null != Session["PackageTypeSearchTmp"])
                packageTypeUseList = (List<PackageTypeUse>)Session["PackageTypeSearchTmp"];

            if (packageTypeUseList.Count > 0 || packageTypeUseOldList.Count > 0)
            {
                var command = new SavePackageMappingCommand();
                if (packageTypeUseOldList.Count > 0)
                {
                    command.service_option = "E";
                }
                else
                {
                    command.service_option = "N";
                }
                command.package_code = Model.PACKAGE_CODE_MAPPING;
                command.airPackageMappingArray = new List<AirPackageMapping>();
                if (packageTypeUseOldList.Count > 0 && packageTypeUseList.Count > 0)
                {
                    var packageTypeUseOld = packageTypeUseOldList.FirstOrDefault();
                    string MAPPING_CODE = packageTypeUseOld.mapping_code;
                    string MAPPING_PRODUCT = packageTypeUseOld.mapping_product;
                    List<AirPackageMapping> listPackageMappingTmp = new List<AirPackageMapping>();
                    foreach (var item in packageTypeUseOldList)
                    {

                        AirPackageMapping airPackageMapping = new AirPackageMapping();
                        var checkHave = packageTypeUseList.Where(t => t.sff_promotion_code == item.sff_promotion_code && t.sff_promotion_bill_tha == item.sff_promotion_bill_tha && t.sff_promotion_bill_eng == item.sff_promotion_bill_eng && t.expire_dtm == item.expire_dtm).ToList();
                        if (checkHave.Count == 0)
                        {
                            airPackageMapping.SERVICE_OPTION = "D";
                            airPackageMapping.MAPPING_CODE = item.mapping_code;
                            airPackageMapping.MAPPING_PRODUCT = item.mapping_product;
                            airPackageMapping.PACKAGE_CODE = item.package_code;
                            airPackageMapping.UPD_BY = User.UserName;
                            airPackageMapping.EFFECTIVE_DTM = item.effective_dtm;
                            airPackageMapping.EXPIRE_DTM = item.expire_dtm;

                            listPackageMappingTmp.Add(airPackageMapping);
                        }
                        else
                        {
                            packageTypeUseList = packageTypeUseList.Where(t => !(t.sff_promotion_code == item.sff_promotion_code && t.sff_promotion_bill_tha == item.sff_promotion_bill_tha && t.sff_promotion_bill_eng == item.sff_promotion_bill_eng && t.expire_dtm == item.expire_dtm)).ToList();
                        }
                    }

                    foreach (var item in packageTypeUseList)
                    {
                        AirPackageMapping airPackageMapping = new AirPackageMapping();
                        airPackageMapping.SERVICE_OPTION = "N";
                        airPackageMapping.MAPPING_CODE = MAPPING_CODE;
                        airPackageMapping.MAPPING_PRODUCT = MAPPING_PRODUCT;
                        airPackageMapping.PACKAGE_CODE = item.package_code;
                        airPackageMapping.UPD_BY = User.UserName;
                        airPackageMapping.EFFECTIVE_DTM = item.effective_dtm;
                        airPackageMapping.EXPIRE_DTM = item.expire_dtm;

                        listPackageMappingTmp.Add(airPackageMapping);
                    }

                    command.airPackageMappingArray = listPackageMappingTmp;
                }
                else if (packageTypeUseOldList.Count > 0 && packageTypeUseList.Count == 0)
                {
                    List<AirPackageMapping> listPackageMappingTmp = new List<AirPackageMapping>();
                    foreach (var item in packageTypeUseOldList)
                    {
                        AirPackageMapping airPackageMapping = new AirPackageMapping();
                        airPackageMapping.SERVICE_OPTION = "D";
                        airPackageMapping.MAPPING_CODE = item.mapping_code;
                        airPackageMapping.MAPPING_PRODUCT = item.mapping_product;
                        airPackageMapping.PACKAGE_CODE = item.package_code;
                        airPackageMapping.UPD_BY = User.UserName;
                        airPackageMapping.EFFECTIVE_DTM = item.effective_dtm != null ? item.effective_dtm : "";
                        airPackageMapping.EXPIRE_DTM = item.expire_dtm != null ? item.expire_dtm : "";

                        command.airPackageMappingArray.Add(airPackageMapping);
                    }

                }
                else
                {
                    foreach (var item in packageTypeUseList)
                    {
                        AirPackageMapping airPackageMapping = new AirPackageMapping();
                        airPackageMapping.SERVICE_OPTION = "N";
                        airPackageMapping.MAPPING_CODE = "";
                        airPackageMapping.MAPPING_PRODUCT = "";
                        airPackageMapping.PACKAGE_CODE = item.package_code;
                        airPackageMapping.UPD_BY = User.UserName;
                        airPackageMapping.EFFECTIVE_DTM = item.effective_dtm != null ? item.effective_dtm : "";
                        airPackageMapping.EXPIRE_DTM = item.expire_dtm != null ? item.expire_dtm : "";

                        command.airPackageMappingArray.Add(airPackageMapping);
                    }
                }

                // Save Data
                try
                {
                    _SavePackageMappingCommand.Handle(command);
                    SaveResult = "SaveSuccess";
                }
                catch (Exception ex)
                {
                    SaveResult = "SaveFail";
                }

                Model.SaveResult = SaveResult;
            }

            /// Set to page2

            ViewBag.User = base.CurrentUser;
            if (Model.SaveToPage3 == "N")
            {
                Session["PackageTypeSearchTmp"] = null;
                Model.StatusPage = 2;
                return RedirectToAction("GetDataToReport", Model);
                //return RedirectToAction("Index", new { SaveStatus = SaveStatus });
            }
            else
            {
                Session["PackageTypeSearchTmp"] = null;
                Model.StatusPage = 3;
                return RedirectToAction("GetDataToPage3", Model);
            }

        }

        public ActionResult GetDataToPage3(ConfigurationPackagesModel Model)
        {
            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "Account");
            ViewBag.User = base.CurrentUser;
            SetViewBagLov();

            ConfigurationPackagesModel configurationPackagesModel = GetConfigurationPackage((int)StatusPages.SavePage3, Model.SFFProductCodeMapping, Model.PACKAGE_CODE_MAPPING);
            configurationPackagesModel.StatusPage = (int)StatusPages.SavePage3;

            if (Model.SaveResult != "")
            {
                if (Model.SaveResult == "SaveSuccess")
                {
                    ViewBag.Success = Model.SaveResult;
                }
                else
                {
                    ViewBag.Error = Model.SaveResult;
                }
            }

            return View("Index", configurationPackagesModel);
        }

        public ActionResult SaveConfigurationPackagePage3(ConfigurationPackagesModel Model)
        {
            string SaveResult = "";
            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "Account");

            var User = base.CurrentUser;
            bool CheckSave = false;
            foreach (var Catalog in Model.ListCatalogAndAuthorizeTable)
            {
                if (Catalog.CatalogAndAuthorizeTableSelect != Catalog.CatalogAndAuthorizeTableSelectOld)
                {
                    CheckSave = CheckSave || true;
                }
            }
            if (CheckSave)
            {
                // Save Data
                try
                {
                    SavePackageUserGroupCommand command = new SavePackageUserGroupCommand();
                    command.airPackageUserArray = new List<AirPackageUserArray>();
                    command.service_option = "";
                    command.package_code = Model.PACKAGE_CODE_CATALOG;
                    foreach (var item in Model.ListCatalogAndAuthorizeTable)
                    {
                        AirPackageUserArray airPackageUserArray = new AirPackageUserArray();
                        if (item.CatalogAndAuthorizeTableSelect == true && item.CatalogAndAuthorizeTableSelectOld == false)
                        {
                            airPackageUserArray.SERVICE_OPTION = "N";
                            airPackageUserArray.USER_GROUP = item.CatalogAndAuthorizeTableValue;
                            airPackageUserArray.EFFECTIVE_DTM = "";
                            airPackageUserArray.EXPIRE_DTM = "";
                            airPackageUserArray.UPD_BY = User.UserName;
                            command.airPackageUserArray.Add(airPackageUserArray);
                        }
                        else if (item.CatalogAndAuthorizeTableSelect == false && item.CatalogAndAuthorizeTableSelectOld == true)
                        {
                            airPackageUserArray.SERVICE_OPTION = "D";
                            airPackageUserArray.USER_GROUP = item.CatalogAndAuthorizeTableValue;
                            airPackageUserArray.EFFECTIVE_DTM = item.CatalogAndAuthorizeTableEffective;
                            airPackageUserArray.EXPIRE_DTM = item.CatalogAndAuthorizeTableExpire;
                            airPackageUserArray.UPD_BY = User.UserName;
                            command.airPackageUserArray.Add(airPackageUserArray);
                        }
                    }
                    if (command.airPackageUserArray.Count > 0)
                    {
                        _SavePackageUserGroupCommand.Handle(command);
                    }
                    SaveResult = "SaveSuccess";
                }
                catch (Exception ex)
                {
                    SaveResult = "SaveFail";
                }

                Model.SaveResult = SaveResult;
            }


            /// Set to page4

            ViewBag.User = base.CurrentUser;
            if (Model.SaveToPage4 == "N")
            {
                Model.StatusPage = 3;
                return RedirectToAction("GetDataToReport", Model);
                //return RedirectToAction("Index", new { SaveStatus = SaveStatus });
            }
            else
            {
                Model.StatusPage = 4;
                return RedirectToAction("GetDataToPage4", Model);
            }

        }

        public ActionResult GetDataToPage4(ConfigurationPackagesModel Model)
        {
            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "Account");
            ViewBag.User = base.CurrentUser;
            SetViewBagLov();

            ConfigurationPackagesModel configurationPackagesModel = GetConfigurationPackage((int)StatusPages.SavePage4, Model.SFFProductCodeCatalog, Model.PACKAGE_CODE_CATALOG);
            configurationPackagesModel.StatusPage = (int)StatusPages.SavePage4;

            if (Model.SaveResult != "")
            {
                if (Model.SaveResult == "SaveSuccess")
                {
                    ViewBag.Success = Model.SaveResult;
                }
                else
                {
                    ViewBag.Error = Model.SaveResult;
                }
            }

            return View("Index", configurationPackagesModel);
        }

        public ActionResult SaveConfigurationPackagePage4(ConfigurationPackagesModel Model)
        {
            string SaveResult = "";
            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "Account");

            var User = base.CurrentUser;
            List<RegionTable> regionListOldList = new List<RegionTable>();
            regionListOldList = GetRegionByPackageCode(Model.PACKAGE_CODE_LOCATION);
            List<ProvinceTable> provinceListOldList = new List<ProvinceTable>();
            provinceListOldList = GetProvinceUseByPackageCode(Model.PACKAGE_CODE_LOCATION);
            List<BuildingTable> buildingTableOldList = new List<BuildingTable>();
            buildingTableOldList = SetBuildingTableUse(Model.PACKAGE_CODE_LOCATION);

            List<ProvinceTable> provinceList = new List<ProvinceTable>();
            List<RegionTable> regionList = new List<RegionTable>();
            if (null != Session["ProvinceTmp"])
            {
                provinceList = (List<ProvinceTable>)Session["ProvinceTmp"];
                foreach (var item in provinceList.Select(t => t.SubRegion).Distinct().ToList())
                {
                    RegionTable regionTableTmp = new RegionTable();
                    regionTableTmp.RegionTableName = item;
                    regionList.Add(regionTableTmp);
                }
            }

            List<BuildingTable> buildingList = new List<BuildingTable>();
            if (null != Session["BuildingTmp"])
                buildingList = (List<BuildingTable>)Session["BuildingTmp"];

            if (provinceList.Count > 0 || provinceListOldList.Count > 0 || buildingList.Count > 0 || buildingTableOldList.Count > 0)
            {
                var command = new SavePackageLocationCommand();
                if (provinceListOldList.Count > 0 || buildingTableOldList.Count > 0)
                {
                    command.service_option = "E";
                }
                else
                {
                    command.service_option = "N";
                }
                command.package_code = Model.PACKAGE_CODE_LOCATION;
                command.user = User.UserName;
                command.airPackageRegionArray = new List<AirPackageRegion>();
                command.airPackageProvinceArray = new List<AirPackageProvince>();
                command.airPackageBuildingArray = new List<AirPackageBuilding>();

                if (regionListOldList.Count > 0 && regionList.Count > 0)
                {

                    List<AirPackageRegion> listPackageRegionTmp = new List<AirPackageRegion>();
                    foreach (var item in regionListOldList)
                    {
                        string strEffectiveDtm = item.EffectiveDtm != null ? item.EffectiveDtm.Value.Date.ToString("dd/MM/yyyy") : "";
                        string strExpireDtm = item.ExpireDtm != null ? item.ExpireDtm.Value.Date.ToString("dd/MM/yyyy") : "";

                        AirPackageRegion airPackageRegion = new AirPackageRegion();
                        var checkHave = regionList.Where(t => t.RegionTableName == item.RegionTableName).ToList();
                        if (checkHave.Count == 0)
                        {
                            airPackageRegion.SERVICE_OPTION = "D";
                            airPackageRegion.REGION = item.RegionTableName;
                            airPackageRegion.EFFECTIVE_DTM = strEffectiveDtm;
                            airPackageRegion.EXPIRE_DTM = strExpireDtm;

                            listPackageRegionTmp.Add(airPackageRegion);
                        }
                        else
                        {
                            regionList = regionList.Where(t => t.RegionTableName != item.RegionTableName).ToList();
                        }
                    }

                    foreach (var item in regionList)
                    {
                        AirPackageRegion airPackageRegion = new AirPackageRegion();
                        airPackageRegion.SERVICE_OPTION = "N";
                        airPackageRegion.REGION = item.RegionTableName;
                        airPackageRegion.EFFECTIVE_DTM = "";
                        airPackageRegion.EXPIRE_DTM = "";

                        listPackageRegionTmp.Add(airPackageRegion);
                    }

                    command.airPackageRegionArray = listPackageRegionTmp;
                }
                else if (regionListOldList.Count > 0 && regionList.Count == 0)
                {
                    foreach (var item in regionListOldList)
                    {
                        string strEffectiveDtm = item.EffectiveDtm != null ? item.EffectiveDtm.Value.Date.ToString("dd/MM/yyyy") : "";
                        string strExpireDtm = item.ExpireDtm != null ? item.ExpireDtm.Value.Date.ToString("dd/MM/yyyy") : "";

                        AirPackageRegion airPackageRegion = new AirPackageRegion();
                        airPackageRegion.SERVICE_OPTION = "D";
                        airPackageRegion.REGION = item.RegionTableName;
                        airPackageRegion.EFFECTIVE_DTM = strEffectiveDtm;
                        airPackageRegion.EXPIRE_DTM = strExpireDtm;

                        command.airPackageRegionArray.Add(airPackageRegion);
                    }
                }
                else if (regionListOldList.Count == 0 && regionList.Count > 0)
                {
                    foreach (var item in regionList)
                    {
                        AirPackageRegion airPackageRegion = new AirPackageRegion();
                        airPackageRegion.SERVICE_OPTION = "N";
                        airPackageRegion.REGION = item.RegionTableName;
                        airPackageRegion.EFFECTIVE_DTM = "";
                        airPackageRegion.EXPIRE_DTM = "";

                        command.airPackageRegionArray.Add(airPackageRegion);
                    }
                }

                if (provinceListOldList.Count > 0 && provinceList.Count > 0)
                {

                    List<AirPackageProvince> listPackageProvinceTmp = new List<AirPackageProvince>();
                    foreach (var item in provinceListOldList)
                    {
                        string strEffectiveDtm = item.EffectiveDtm != null ? item.EffectiveDtm.Value.Date.ToString("dd/MM/yyyy") : "";
                        string strExpireDtm = item.ExpireDtm != null ? item.ExpireDtm.Value.Date.ToString("dd/MM/yyyy") : "";

                        AirPackageProvince airPackageProvince = new AirPackageProvince();
                        var checkHave = provinceList.Where(t => t.SubRegion == item.SubRegion && t.ProvinceName == item.ProvinceName).ToList();
                        if (checkHave.Count == 0)
                        {
                            airPackageProvince.SERVICE_OPTION = "D";
                            airPackageProvince.PROVINCE = item.ProvinceName;
                            airPackageProvince.REGION = item.SubRegion;
                            airPackageProvince.EFFECTIVE_DTM = strEffectiveDtm;
                            airPackageProvince.EXPIRE_DTM = strExpireDtm;

                            listPackageProvinceTmp.Add(airPackageProvince);
                        }
                        else
                        {
                            provinceList = provinceList.Where(t => !(t.SubRegion == item.SubRegion && t.ProvinceName == item.ProvinceName)).ToList();
                        }
                    }

                    foreach (var item in provinceList)
                    {
                        AirPackageProvince airPackageProvince = new AirPackageProvince();
                        airPackageProvince.SERVICE_OPTION = "N";
                        airPackageProvince.PROVINCE = item.ProvinceName;
                        airPackageProvince.REGION = item.SubRegion;
                        airPackageProvince.EFFECTIVE_DTM = "";
                        airPackageProvince.EXPIRE_DTM = "";

                        listPackageProvinceTmp.Add(airPackageProvince);
                    }

                    command.airPackageProvinceArray = listPackageProvinceTmp;
                }
                else if (provinceListOldList.Count > 0 && provinceList.Count == 0)
                {
                    foreach (var item in provinceListOldList)
                    {
                        string strEffectiveDtm = item.EffectiveDtm != null ? item.EffectiveDtm.Value.Date.ToString("dd/MM/yyyy") : "";
                        string strExpireDtm = item.ExpireDtm != null ? item.ExpireDtm.Value.Date.ToString("dd/MM/yyyy") : "";

                        AirPackageProvince airPackageProvince = new AirPackageProvince();
                        airPackageProvince.SERVICE_OPTION = "D";
                        airPackageProvince.PROVINCE = item.ProvinceName;
                        airPackageProvince.REGION = item.SubRegion;
                        airPackageProvince.EFFECTIVE_DTM = strEffectiveDtm;
                        airPackageProvince.EXPIRE_DTM = strExpireDtm;

                        command.airPackageProvinceArray.Add(airPackageProvince);
                    }
                }
                else if (provinceListOldList.Count == 0 && provinceList.Count > 0)
                {
                    foreach (var item in provinceList)
                    {
                        AirPackageProvince airPackageProvince = new AirPackageProvince();
                        airPackageProvince.SERVICE_OPTION = "N";
                        airPackageProvince.PROVINCE = item.ProvinceName;
                        airPackageProvince.REGION = item.SubRegion;
                        airPackageProvince.EFFECTIVE_DTM = "";
                        airPackageProvince.EXPIRE_DTM = "";

                        command.airPackageProvinceArray.Add(airPackageProvince);
                    }
                }

                if (buildingTableOldList.Count > 0 && buildingList.Count > 0)
                {

                    List<AirPackageBuilding> listPackageBuildingTmp = new List<AirPackageBuilding>();
                    foreach (var item in buildingTableOldList)
                    {
                        string strEffectiveDtm = item.EffectiveDtm != null ? item.EffectiveDtm.Value.Date.ToString("dd/MM/yyyy") : "";
                        string strExpireDtm = item.ExpireDtm != null ? item.ExpireDtm.Value.Date.ToString("dd/MM/yyyy") : "";

                        AirPackageBuilding airPackageBuilding = new AirPackageBuilding();
                        var checkHave = buildingList.Where(t => t.BuildingName == item.BuildingName && t.BuildingNo == item.BuildingNo).ToList();
                        if (checkHave.Count == 0)
                        {
                            airPackageBuilding.SERVICE_OPTION = "D";
                            airPackageBuilding.BUILDING_NAME = item.BuildingName;
                            airPackageBuilding.BUILDING_NO = item.BuildingNo;
                            airPackageBuilding.ADDRESS_ID = item.AddressID;
                            airPackageBuilding.ADDRESS_TYPE = item.AddressType;
                            airPackageBuilding.BUILDING_NAME_E = item.BuildingNameEng;
                            airPackageBuilding.BUILDING_NO_E = item.BuildingNoEng;
                            airPackageBuilding.EFFECTIVE_DTM = strEffectiveDtm;
                            airPackageBuilding.EXPIRE_DTM = strExpireDtm;

                            listPackageBuildingTmp.Add(airPackageBuilding);
                        }
                        else
                        {
                            buildingList = buildingList.Where(t => !(t.BuildingName == item.BuildingName && t.BuildingNo == item.BuildingNo)).ToList();
                        }
                    }

                    foreach (var item in buildingList)
                    {
                        AirPackageBuilding airPackageBuilding = new AirPackageBuilding();
                        airPackageBuilding.SERVICE_OPTION = "N";
                        airPackageBuilding.BUILDING_NAME = item.BuildingName;
                        airPackageBuilding.BUILDING_NO = item.BuildingNo;
                        airPackageBuilding.ADDRESS_ID = item.AddressID;
                        airPackageBuilding.ADDRESS_TYPE = item.AddressType;
                        airPackageBuilding.BUILDING_NAME_E = item.BuildingNameEng;
                        airPackageBuilding.BUILDING_NO_E = item.BuildingNoEng;
                        airPackageBuilding.EFFECTIVE_DTM = "";
                        airPackageBuilding.EXPIRE_DTM = "";

                        listPackageBuildingTmp.Add(airPackageBuilding);
                    }

                    command.airPackageBuildingArray = listPackageBuildingTmp;
                }
                else if (buildingTableOldList.Count > 0 && buildingList.Count == 0)
                {
                    foreach (var item in buildingTableOldList)
                    {
                        string strEffectiveDtm = item.EffectiveDtm != null ? item.EffectiveDtm.Value.Date.ToString("dd/MM/yyyy") : "";
                        string strExpireDtm = item.ExpireDtm != null ? item.ExpireDtm.Value.Date.ToString("dd/MM/yyyy") : "";

                        AirPackageBuilding airPackageBuilding = new AirPackageBuilding();
                        airPackageBuilding.SERVICE_OPTION = "D";
                        airPackageBuilding.BUILDING_NAME = item.BuildingName;
                        airPackageBuilding.BUILDING_NO = item.BuildingNo;
                        airPackageBuilding.ADDRESS_ID = item.AddressID;
                        airPackageBuilding.ADDRESS_TYPE = item.AddressType;
                        airPackageBuilding.BUILDING_NAME_E = item.BuildingNameEng;
                        airPackageBuilding.BUILDING_NO_E = item.BuildingNoEng;
                        airPackageBuilding.EFFECTIVE_DTM = strEffectiveDtm;
                        airPackageBuilding.EXPIRE_DTM = strExpireDtm;

                        command.airPackageBuildingArray.Add(airPackageBuilding);
                    }
                }
                else if (buildingTableOldList.Count == 0 && buildingList.Count > 0)
                {
                    foreach (var item in buildingList)
                    {
                        AirPackageBuilding airPackageBuilding = new AirPackageBuilding();
                        airPackageBuilding.SERVICE_OPTION = "N";
                        airPackageBuilding.BUILDING_NAME = item.BuildingName;
                        airPackageBuilding.BUILDING_NO = item.BuildingNo;
                        airPackageBuilding.ADDRESS_ID = item.AddressID;
                        airPackageBuilding.ADDRESS_TYPE = item.AddressType;
                        airPackageBuilding.BUILDING_NAME_E = item.BuildingNameEng;
                        airPackageBuilding.BUILDING_NO_E = item.BuildingNoEng;
                        airPackageBuilding.EFFECTIVE_DTM = "";
                        airPackageBuilding.EXPIRE_DTM = "";

                        command.airPackageBuildingArray.Add(airPackageBuilding);
                    }
                }

                if (command.airPackageRegionArray.Count > 0 || command.airPackageProvinceArray.Count > 0 || command.airPackageBuildingArray.Count > 0)
                {
                    // Save Data
                    try
                    {
                        _SavePackageLocationCommand.Handle(command);
                        SaveResult = "SaveSuccess";
                    }
                    catch (Exception ex)
                    {
                        SaveResult = "SaveFail";
                    }

                    Model.SaveResult = SaveResult;

                }

            }

            ViewBag.User = base.CurrentUser;

            Model.StatusPage = 4;
            return RedirectToAction("GetDataToReport", Model);
        }

        public ActionResult GetDataToReport(ConfigurationPackagesModel Model)
        {
            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "Account");
            ViewBag.User = base.CurrentUser;
            SetViewBagLov();

            string ProductCode = "";
            string PackageCode = "";

            if (Model.StatusPage == 1)
            {
                ProductCode = Model.SFFProductCode;
                PackageCode = Model.PACKAGE_CODE;
            }
            else if (Model.StatusPage == 2)
            {
                ProductCode = Model.SFFProductCodeMapping;
                PackageCode = Model.PACKAGE_CODE_MAPPING;
            }
            else if (Model.StatusPage == 3)
            {
                ProductCode = Model.SFFProductCodeCatalog;
                PackageCode = Model.PACKAGE_CODE_CATALOG;
            }
            else if (Model.StatusPage == 4)
            {
                ProductCode = Model.SFFProductCodeLocation;
                PackageCode = Model.PACKAGE_CODE_LOCATION;
            }

            ConfigurationPackagesModel configurationPackagesModel = GetConfigurationPackage((int)StatusPages.ReportPage, ProductCode, PackageCode);
            configurationPackagesModel.StatusPage = (int)StatusPages.ReportPage;

            if (Model.SaveResult != "")
            {
                if (Model.SaveResult == "SaveSuccess")
                {
                    ViewBag.Success = Model.SaveResult;
                }
                else
                {
                    ViewBag.Error = Model.SaveResult;
                }
            }

            return View("Index", configurationPackagesModel);
        }

        public ActionResult SaveConfigurationPackageFinish()
        {
            return RedirectToAction("Index");
        }

        public ActionResult SearchDataSourceRequest([DataSourceRequest] DataSourceRequest request, string dataS = "")
        {
            if (dataS != null && dataS != "")
            {
                var searchMapPromotionModel = new JavaScriptSerializer().Deserialize<ConfigurationPackagesModel>(dataS);
                var result = GetDataSearchModel(searchMapPromotionModel);

                return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);

            }
            else
            {
                return null;
            }
        }

        public ActionResult SearchPackageType([DataSourceRequest] DataSourceRequest request, string dataS = "")
        {

            if (dataS != null && dataS != "")
            {
                var searchMapPromotionModel = new JavaScriptSerializer().Deserialize<ConfigurationPackagesModel>(dataS);

                if (null == Session["PackageTypeSearchTmp"])
                    Session["PackageTypeSearchTmp"] = GetPackageTypeUse(searchMapPromotionModel.PACKAGE_CODE);

                List<PackageTypeUse> packageTypeUse = (List<PackageTypeUse>)Session["PackageTypeSearchTmp"];

                var result = GetPackageTypeDataSearchModel(searchMapPromotionModel.PACKAGE_CODE, searchMapPromotionModel.PackageTypeSearch);
                foreach (var item in packageTypeUse)
                {
                    if (item.expire_dtm == null || item.expire_dtm.ToString() == "")
                    {
                        result = result.Where(t => !(t.package_code == item.package_code && t.sff_promotion_bill_tha == item.sff_promotion_bill_tha && t.sff_promotion_bill_eng == item.sff_promotion_bill_eng)).ToList();
                    }
                }

                return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return null;
            }
        }

        public ActionResult SearchProvince([DataSourceRequest] DataSourceRequest request, string dataS = "")
        {

            if (dataS != null && dataS != "")
            {
                List<ProvinceTable> provinceUse;
                var searchProvince = new JavaScriptSerializer().Deserialize<RegionTables>(dataS);
                var searchProvinceS = searchProvince.RegionTableName;

                var result = SetZipCodeProvince(searchProvinceS);
                if (null != Session["ProvinceTmp"])
                {
                    if (result.Count > 0)
                    {
                        provinceUse = (List<ProvinceTable>)Session["ProvinceTmp"];
                        foreach (var item in provinceUse)
                        {
                            result = result.Where(t => !(t.ProvinceName == item.ProvinceName && t.SubRegion == item.SubRegion)).ToList();
                        }
                    }
                }

                return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return null;
            }
        }

        public ActionResult SearchBuilding([DataSourceRequest] DataSourceRequest request, string dataS = "")
        {

            if (dataS != null && dataS != "")
            {
                List<BuildingTable> buildingUse;
                var searchBuilding = new JavaScriptSerializer().Deserialize<BuildingSearch>(dataS);
                var Building = searchBuilding.Building;
                var Technology = searchBuilding.Technology;

                var result = SetBuildingTable(Building, Technology);
                if (null != Session["BuildingTmp"])
                {
                    if (result.Count > 0)
                    {
                        buildingUse = (List<BuildingTable>)Session["BuildingTmp"];
                        foreach (var item in buildingUse)
                        {
                            result = result.Where(t => !(t.BuildingNo == item.BuildingNo && t.BuildingName == item.BuildingName)).ToList();
                        }
                    }
                }

                return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return null;
            }
        }

        public ActionResult PackageForMappingUse([DataSourceRequest] DataSourceRequest request, string dataS = "")
        {
            var searchMapPromotionModel = new JavaScriptSerializer().Deserialize<ConfigurationPackagesModel>(dataS);

            List<PackageTypeUse> packageTypeUse;
            if (null != Session["PackageTypeSearchTmp"])
            {
                packageTypeUse = (List<PackageTypeUse>)Session["PackageTypeSearchTmp"];
            }
            else
            {
                packageTypeUse = new List<PackageTypeUse>();
            }

            return Json(packageTypeUse.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ProvinceUse([DataSourceRequest] DataSourceRequest request, string dataS = "")
        {
            var searchMapPromotionModel = new JavaScriptSerializer().Deserialize<ConfigurationPackagesModel>(dataS);

            List<ProvinceTable> provinceUse;
            if (null != Session["ProvinceTmp"])
            {
                provinceUse = (List<ProvinceTable>)Session["ProvinceTmp"];
            }
            else
            {
                provinceUse = new List<ProvinceTable>();
            }

            return Json(provinceUse.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public ActionResult BuildingUse([DataSourceRequest] DataSourceRequest request, string dataS = "")
        {
            var searchMapPromotionModel = new JavaScriptSerializer().Deserialize<BuildingSearch>(dataS);

            List<BuildingTable> buildingUse;
            if (null != Session["BuildingTmp"])
            {
                buildingUse = (List<BuildingTable>)Session["BuildingTmp"];
            }
            else
            {
                buildingUse = new List<BuildingTable>();
            }

            return Json(buildingUse.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public ActionResult SearchSummaryPackageMaster([DataSourceRequest] DataSourceRequest request, string dataS = "")
        {
            if (dataS != null && dataS != "")
            {
                var searchMapPromotionModel = new JavaScriptSerializer().Deserialize<SummaryPackageSearch>(dataS);
                var result = GetSummaryPackageMasterByPackageCode(searchMapPromotionModel.PACKAGE_CODE);
                if (result != null)
                {
                    return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public ActionResult SearchSummaryPackageMapping([DataSourceRequest] DataSourceRequest request, string dataS = "")
        {
            if (dataS != null && dataS != "")
            {
                var searchMapPromotionModel = new JavaScriptSerializer().Deserialize<SummaryPackageSearch>(dataS);
                var result = GetSummaryPackageMappingByPackageCode(searchMapPromotionModel.PACKAGE_CODE);
                if (result != null)
                {
                    return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public ActionResult SearchSummaryPackageUser([DataSourceRequest] DataSourceRequest request, string dataS = "")
        {
            if (dataS != null && dataS != "")
            {
                var searchMapPromotionModel = new JavaScriptSerializer().Deserialize<SummaryPackageSearch>(dataS);
                var result = GetSummaryPackageUserByPackageCode(searchMapPromotionModel.PACKAGE_CODE);
                if (result != null)
                {
                    return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public ActionResult SearchSummaryPackageLoc([DataSourceRequest] DataSourceRequest request, string dataS = "")
        {
            if (dataS != null && dataS != "")
            {
                var searchMapPromotionModel = new JavaScriptSerializer().Deserialize<SummaryPackageSearch>(dataS);
                var result = GetSummaryPackageLocByPackageCode(searchMapPromotionModel.PACKAGE_CODE);
                if (result != null)
                {
                    return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public string AddPakageMappingToTmp(string DataS)
        {
            List<PackageTypeUse> packageTypeUseList = new List<PackageTypeUse>();
            if (null != Session["PackageTypeSearchTmp"])
                packageTypeUseList = (List<PackageTypeUse>)Session["PackageTypeSearchTmp"];

            if (DataS != null && DataS != "")
            {
                DataTable dt = (DataTable)JsonConvert.DeserializeObject(DataS, (typeof(DataTable)));
                DataColumnCollection columns = dt.Columns;
                foreach (DataRow dr in dt.Rows)
                {
                    PackageTypeUse packageTypeUse = new PackageTypeUse();
                    packageTypeUse.package_code = dr["package_code"] != null ? dr["package_code"].ToString() : "";
                    packageTypeUse.package_type = dr["package_type"] != null ? dr["package_type"].ToString() : "";
                    packageTypeUse.package_name_tha = dr["package_name_tha"] != null ? dr["package_name_tha"].ToString() : "";
                    if (columns.Contains("sff_promotion_code"))
                    {
                        packageTypeUse.sff_promotion_code = dr["sff_promotion_code"] != null ? dr["sff_promotion_code"].ToString() : "";
                    }
                    else
                    {
                        packageTypeUse.sff_promotion_code = "";
                    }
                    if (columns.Contains("effective_date"))
                    {
                        packageTypeUse.effective_dtm = dr["effective_date"] != null ? dr["effective_date"].ToString() : "";
                    }
                    else
                    {
                        packageTypeUse.effective_dtm = "";
                    }
                    if (columns.Contains("expire_date"))
                    {
                        packageTypeUse.expire_dtm = dr["expire_date"] != null ? dr["expire_date"].ToString() : "";
                    }
                    else
                    {
                        packageTypeUse.expire_dtm = "";
                    }
                    if (columns.Contains("mapping_code"))
                    {
                        packageTypeUse.mapping_code = dr["mapping_code"] != null ? dr["mapping_code"].ToString() : "";
                    }
                    else
                    {
                        packageTypeUse.mapping_code = "";
                    }
                    if (columns.Contains("mapping_product"))
                    {
                        packageTypeUse.mapping_product = dr["mapping_product"] != null ? dr["mapping_product"].ToString() : "";
                    }
                    else
                    {
                        packageTypeUse.mapping_product = "";
                    }
                    if (columns.Contains("sale_start_date"))
                    {
                        packageTypeUse.sale_start_date = dr["sale_start_date"] != null ? dr["sale_start_date"].ToString() : "";
                    }
                    else
                    {
                        packageTypeUse.sale_start_date = "";
                    }
                    if (columns.Contains("sale_end_date"))
                    {
                        packageTypeUse.sale_end_date = dr["sale_end_date"] != null ? dr["sale_end_date"].ToString() : "";
                    }
                    else
                    {
                        packageTypeUse.sale_end_date = "";
                    }
                    packageTypeUse.sff_promotion_bill_tha = dr["sff_promotion_bill_tha"] != null ? dr["sff_promotion_bill_tha"].ToString() : "";
                    packageTypeUse.sff_promotion_bill_eng = dr["sff_promotion_bill_eng"] != null ? dr["sff_promotion_bill_eng"].ToString() : "";

                    packageTypeUseList.Insert(0, packageTypeUse);
                }
            }

            Session["PackageTypeSearchTmp"] = packageTypeUseList;

            return "OK";
        }

        public string DeletePakageMappingToTmp(string DataS)
        {
            List<PackageTypeUse> packageTypeUseList = new List<PackageTypeUse>();
            if (null != Session["PackageTypeSearchTmp"])
                packageTypeUseList = (List<PackageTypeUse>)Session["PackageTypeSearchTmp"];

            if (DataS != null && DataS != "")
            {
                DataTable dt = (DataTable)JsonConvert.DeserializeObject(DataS, (typeof(DataTable)));
                DataColumnCollection columns = dt.Columns;
                foreach (DataRow dr in dt.Rows)
                {
                    string sff_promotion_code = "";
                    string sff_promotion_bill_tha = "";
                    string sff_promotion_bill_eng = "";
                    string expire_dtm = "";

                    if (columns.Contains("sff_promotion_code"))
                    {
                        sff_promotion_code = dr["sff_promotion_code"] != null ? dr["sff_promotion_code"].ToString() : "";
                    }
                    sff_promotion_bill_tha = dr["sff_promotion_bill_tha"] != null ? dr["sff_promotion_bill_tha"].ToString() : "";
                    sff_promotion_bill_eng = dr["sff_promotion_bill_eng"] != null ? dr["sff_promotion_bill_eng"].ToString() : "";
                    if (columns.Contains("expire_dtm"))
                    {
                        expire_dtm = dr["expire_dtm"] != null ? dr["expire_dtm"].ToString() : "";
                    }

                    packageTypeUseList = packageTypeUseList.Where(t => !(t.sff_promotion_code == sff_promotion_code && t.sff_promotion_bill_tha == sff_promotion_bill_tha && t.sff_promotion_bill_eng == sff_promotion_bill_eng && (t.expire_dtm != null ? t.expire_dtm : "") == expire_dtm)).ToList();
                }
            }

            Session["PackageTypeSearchTmp"] = packageTypeUseList;

            return "OK";
        }

        public string AddProvinceToTmp(string DataS)
        {
            List<ProvinceTable> provinceTableList = new List<ProvinceTable>();
            if (null != Session["ProvinceTmp"])
                provinceTableList = (List<ProvinceTable>)Session["ProvinceTmp"];

            if (DataS != null && DataS != "")
            {
                DataTable dt = (DataTable)JsonConvert.DeserializeObject(DataS, (typeof(DataTable)));
                DataColumnCollection columns = dt.Columns;
                foreach (DataRow dr in dt.Rows)
                {
                    ProvinceTable provinceTable = new ProvinceTable();
                    provinceTable.ProvinceName = dr["ProvinceName"] != null ? dr["ProvinceName"].ToString() : "";
                    provinceTable.SubRegion = dr["SubRegion"] != null ? dr["SubRegion"].ToString() : "";


                    provinceTableList.Insert(0, provinceTable);
                }
            }

            Session["ProvinceTmp"] = provinceTableList;

            return "OK";
        }

        public string DeleteProvinceToTmp(string DataS)
        {
            List<ProvinceTable> ProvinceUseList = new List<ProvinceTable>();
            if (null != Session["ProvinceTmp"])
                ProvinceUseList = (List<ProvinceTable>)Session["ProvinceTmp"];

            if (DataS != null && DataS != "")
            {
                DataTable dt = (DataTable)JsonConvert.DeserializeObject(DataS, (typeof(DataTable)));
                DataColumnCollection columns = dt.Columns;
                foreach (DataRow dr in dt.Rows)
                {
                    string ProvinceName = "";
                    string SubRegion = "";

                    ProvinceTable provinceTable = new ProvinceTable();
                    provinceTable.ProvinceName = dr["ProvinceName"] != null ? dr["ProvinceName"].ToString() : "";
                    provinceTable.SubRegion = dr["SubRegion"] != null ? dr["SubRegion"].ToString() : "";

                    ProvinceName = dr["ProvinceName"] != null ? dr["ProvinceName"].ToString() : "";
                    SubRegion = dr["SubRegion"] != null ? dr["SubRegion"].ToString() : "";

                    ProvinceUseList = ProvinceUseList.Where(t => !(t.ProvinceName == ProvinceName && t.SubRegion == SubRegion)).ToList();
                }
            }

            Session["ProvinceTmp"] = ProvinceUseList;

            return "OK";
        }

        public string AddBuildingToTmp(string DataS)
        {
            List<BuildingTable> buildingTableList = new List<BuildingTable>();
            if (null != Session["BuildingTmp"])
                buildingTableList = (List<BuildingTable>)Session["BuildingTmp"];

            if (DataS != null && DataS != "")
            {
                DataTable dt = (DataTable)JsonConvert.DeserializeObject(DataS, (typeof(DataTable)));
                DataColumnCollection columns = dt.Columns;
                foreach (DataRow dr in dt.Rows)
                {
                    BuildingTable buildingTable = new BuildingTable();
                    if (columns.Contains("BuildingName"))
                    {
                        buildingTable.BuildingName = dr["BuildingName"] != null ? dr["BuildingName"].ToString() : "";
                    }
                    else
                    {
                        buildingTable.BuildingName = "";
                    }
                    if (columns.Contains("BuildingNo"))
                    {
                        buildingTable.BuildingNo = dr["BuildingNo"] != null ? dr["BuildingNo"].ToString() : "";
                    }
                    else
                    {
                        buildingTable.BuildingNo = "";
                    }
                    if (columns.Contains("AddressID"))
                    {
                        buildingTable.AddressID = dr["AddressID"] != null ? dr["AddressID"].ToString() : "";
                    }
                    else
                    {
                        buildingTable.AddressID = "";
                    }
                    if (columns.Contains("AddressType"))
                    {
                        buildingTable.AddressType = dr["AddressType"] != null ? dr["AddressType"].ToString() : "";
                    }
                    else
                    {
                        buildingTable.AddressType = "";
                    }
                    if (columns.Contains("BuildingNameEng"))
                    {
                        buildingTable.BuildingNameEng = dr["BuildingNameEng"] != null ? dr["BuildingNameEng"].ToString() : "";
                    }
                    else
                    {
                        buildingTable.BuildingNameEng = "";
                    }
                    if (columns.Contains("BuildingNoEng"))
                    {
                        buildingTable.BuildingNoEng = dr["BuildingNoEng"] != null ? dr["BuildingNoEng"].ToString() : "";
                    }
                    else
                    {
                        buildingTable.BuildingNoEng = "";
                    }


                    buildingTableList.Insert(0, buildingTable);
                }
            }

            Session["BuildingNo"] = buildingTableList;

            return "OK";
        }

        public string DeleteBuildingToTmp(string DataS)
        {
            List<BuildingTable> BuildingTableList = new List<BuildingTable>();
            if (null != Session["BuildingTmp"])
                BuildingTableList = (List<BuildingTable>)Session["BuildingTmp"];

            if (DataS != null && DataS != "")
            {
                DataTable dt = (DataTable)JsonConvert.DeserializeObject(DataS, (typeof(DataTable)));
                DataColumnCollection columns = dt.Columns;
                foreach (DataRow dr in dt.Rows)
                {
                    string BuildingName = "";
                    string BuildingNo = "";

                    BuildingTable buildingTable = new BuildingTable();
                    buildingTable.BuildingName = dr["BuildingName"] != null ? dr["BuildingName"].ToString() : "";
                    buildingTable.BuildingNo = dr["BuildingNo"] != null ? dr["BuildingNo"].ToString() : "";

                    BuildingName = dr["BuildingName"] != null ? dr["BuildingName"].ToString() : "";
                    BuildingNo = dr["BuildingNo"] != null ? dr["BuildingNo"].ToString() : "";

                    BuildingTableList = BuildingTableList.Where(t => !(t.BuildingName == BuildingName && t.BuildingNo == BuildingNo)).ToList();
                }
            }

            Session["BuildingTmp"] = BuildingTableList;

            return "OK";
        }

        private List<NewPackageMaster> GetDataSearchModel(ConfigurationPackagesModel searchawcModel)
        {

            var query = new GetAWConfigurationPackagesQuery()
            {
                PromotionCode = searchawcModel.SFFProductCodeSearch ?? "",
                PromotionNameThai = searchawcModel.SFFProductNameThaiSearch ?? "",
                PromotionNameEng = searchawcModel.SFFProductNameEngSearch ?? ""

            };
            List<NewPackageMaster> result = _queryProcessor.Execute(query);
            return result;
        }

        private List<PackageGroupDesc> GetPackageGroupDescriptionByPackageGroup(string PackageGroupName)
        {

            var query = new GetPackageGroupDescriptionByPackageGroupQuery()
            {
                PackageGroupName = PackageGroupName ?? "",

            };
            List<PackageGroupDesc> result = _queryProcessor.Execute(query);
            return result;
        }

        private List<ConfigurationPackageDetail> GetAWConfigurationPackageDetailEdit(string ProductCode = "", string PackageCode = "")
        {
            var query1 = new GetAWConfigurationPackageDetailEditQuery()
            {
                PromotionCode = ProductCode ?? "",
                PackageCode = PackageCode ?? ""

            };
            return _queryProcessor.Execute(query1);
        }

        private List<ProductTypePackage> GetAWConfigurationPackageProductTypeEdit(string PackageCode = "")
        {
            var query2 = new GetAWConfigurationPackageProductTypeEditQuery
            {
                PackageCode = PackageCode ?? ""
            };
            return _queryProcessor.Execute(query2);
        }

        private List<VendorPartner> GetAWConfigurationPackageVendorEdit(string PackageCode = "")
        {
            var query3 = new GetAWConfigurationPackageVendorEditQuery()
            {
                PackageCode = PackageCode ?? ""

            };
            return _queryProcessor.Execute(query3);
        }

        private List<PackageTypeSearch> GetPackageTypeDataSearchModel(string PackageCode = "", string ProductType = "")
        {

            var query = new GetAWPackageForMappingQuery()
            {
                PackageCode = PackageCode ?? "",
                ProductType = ProductType ?? ""

            };
            List<PackageTypeSearch> result = _queryProcessor.Execute(query);
            return result;
        }

        private List<PackageTypeUse> GetPackageTypeUse(string PackageCode = "")
        {

            var query = new GetAWPackageForMappingUseQuery()
            {
                PackageCode = PackageCode ?? ""

            };
            List<PackageTypeUse> result = _queryProcessor.Execute(query);
            return result;
        }

        private List<ChannelGroup> GetChannelGroup(string PackageCode = "")
        {

            var query = new GetAWChannelGroupQuery();
            List<ChannelGroup> result;
            result = _queryProcessor.Execute(query);
            ChannelGroup SSO = new ChannelGroup();
            SSO.CatalogAndAuthorizeName = "SSO";
            result.Add(SSO);
            ChannelGroup STAFF = new ChannelGroup();
            STAFF.CatalogAndAuthorizeName = "STAFF";
            result.Add(STAFF);
            return result;
        }

        private List<ZipCode> GetZipCode()
        {

            var query = new GetAWZipCodeQuery();
            List<ZipCode> result = _queryProcessor.Execute(query);
            return result;
        }

        private List<ZipCodeProvince> GetZipCodeProvince(string[] SubRegion)
        {
            var query = new GetAWZipCodeProvinceQuery();
            query.RegionNames = SubRegion;
            List<ZipCodeProvince> result = _queryProcessor.Execute(query);
            return result;
        }

        private List<PackageUserGroup> GetPackageUserGroup(string PackageCode = "")
        {

            var query = new GetAWPackageUserGroupQuery()
            {
                PackageCode = PackageCode ?? ""

            };
            List<PackageUserGroup> result = _queryProcessor.Execute(query);
            return result;
        }

        private List<RegionTable> GetRegionByPackageCode(string PackageCode = "")
        {
            var query = new GetAWRegionQuery()
            {
                PackageCode = PackageCode ?? ""

            };
            List<RegionTable> result = new List<RegionTable>();
            try
            {
                result = _queryProcessor.Execute(query);
            }
            catch (Exception ex)
            {
                throw new Exception("GetRegionByPackageCode : " + ex.Message);
            }
            return result;
        }

        private List<ProvinceTable> GetProvinceUseByPackageCode(string PackageCode = "")
        {
            var query = new GetAWProvinceUseByPackageCodeQuery()
            {
                PackageCode = PackageCode ?? ""

            };
            List<ProvinceTable> result = new List<ProvinceTable>();
            try
            {
                result = _queryProcessor.Execute(query);
            }
            catch (Exception ex)
            {
                throw new Exception("GetProvinceUseByPackageCode : " + ex.Message);
            }
            return result;
        }

        private List<TechnologyTable> GetTechnology()
        {
            var LovTechnology = (List<LovModel>)ViewBag.technology;

            List<TechnologyTable> result = new List<TechnologyTable>();
            foreach (var item in LovTechnology)
            {
                TechnologyTable technologyTable = new TechnologyTable();
                technologyTable.TechnologyName = item.LOV_NAME;
                result.Add(technologyTable);
            }

            return result.OrderBy(t => t.TechnologyName).ToList();
        }

        private List<BuildingDetail> GetBuildingByBuildingNameAndTechnology(string BuildingName = "", string Technology = "")
        {
            var query = new GetAWBuildingByBuildingNameAndTechnologyQuery()
            {
                Building = BuildingName,
                Technology = Technology,

            };
            List<BuildingDetail> result = new List<BuildingDetail>();
            try
            {
                result = _queryProcessor.Execute(query);
            }
            catch (Exception ex)
            {
                throw new Exception("GetBuildingByBuildingNameAndTechnology : " + ex.Message);
            }
            return result;
        }

        private List<BuildingDetail> GetBuildingByPackageCode(string PackageCode = "")
        {
            var query = new GetAWBuildingByPackageCodeQuery()
            {
                PackageCode = PackageCode

            };
            List<BuildingDetail> result = new List<BuildingDetail>();
            try
            {
                result = _queryProcessor.Execute(query);
            }
            catch (Exception ex)
            {
                throw new Exception("GetAWBuildingByPackageCodeQuery : " + ex.Message);
            }
            return result;
        }

        private List<SummaryPackageMaster> GetSummaryPackageMasterByPackageCode(string PackageCode = "")
        {
            var query = new GetAWSummaryPackageMasterQuery()
            {
                PackageCode = PackageCode

            };
            List<SummaryPackageMaster> result = new List<SummaryPackageMaster>();
            try
            {
                result = _queryProcessor.Execute(query);
            }
            catch (Exception ex)
            {
                throw new Exception("GetSummaryPackageMasterByPackageCode : " + ex.Message);
            }
            return result;
        }

        private List<SummaryPackageMapping> GetSummaryPackageMappingByPackageCode(string PackageCode = "")
        {
            var query = new GetAWSummaryPackageMappingQuery()
            {
                PackageCode = PackageCode

            };
            List<SummaryPackageMapping> result = new List<SummaryPackageMapping>();
            try
            {
                result = _queryProcessor.Execute(query);
            }
            catch (Exception ex)
            {
                throw new Exception("GetSummaryPackageMappingByPackageCode : " + ex.Message);
            }
            return result;
        }

        private List<SummaryPackageUser> GetSummaryPackageUserByPackageCode(string PackageCode = "")
        {
            var query = new GetAWSummaryPackageUserQuery()
            {
                PackageCode = PackageCode

            };
            List<SummaryPackageUser> result = new List<SummaryPackageUser>();
            try
            {
                result = _queryProcessor.Execute(query);
            }
            catch (Exception ex)
            {
                throw new Exception("GetSummaryPackageUserByPackageCode : " + ex.Message);
            }
            return result;
        }

        private List<SummaryPackageLoc> GetSummaryPackageLocByPackageCode(string PackageCode = "")
        {
            var query = new GetAWSummaryPackageLocQuery()
            {
                PackageCode = PackageCode

            };
            List<SummaryPackageLoc> result = new List<SummaryPackageLoc>();
            try
            {
                result = _queryProcessor.Execute(query);
            }
            catch (Exception ex)
            {
                throw new Exception("GetSummaryPackageLocByPackageCode : " + ex.Message);
            }
            return result;
        }

        public JsonResult SelectPackageGroupTmp(string ProductType)
        {
            List<LovModel> ListLovModel;
            if (null == Session["PackageGroupTmp" + ProductType])
                Session["PackageGroupTmp" + ProductType] = SelectPackageGroup(ProductType);

            ListLovModel = (List<LovModel>)Session["PackageGroupTmp" + ProductType];
            return Json(ListLovModel, JsonRequestBehavior.AllowGet);
        }

        public List<LovModel> SelectPackageGroup(string ProductType)
        {
            var query = new GetAWPackageGroupQuery
            {
                ProductType = ProductType
            };
            var data = _queryProcessor.Execute(query);

            List<LovModel> ListLovModel = new List<LovModel>();
            foreach (var item in data)
            {
                LovModel lovModel = new LovModel();
                lovModel.LOV_NAME = item.PACKAGE_GROUP;
                lovModel.LOV_VAL1 = item.PACKAGE_GROUP;
                ListLovModel.Add(lovModel);
            }
            ListLovModel = ListLovModel.OrderBy(p => p.LOV_NAME).ToList();
            ListLovModel.Insert(0, new LovModel { DISPLAY_VAL = "", LOV_NAME = "", LOV_VAL1 = "" });
            return ListLovModel;
        }

        public JsonResult SelectProductSubtype3Tmp()
        {
            List<LovModel> ListLovModel;
            if (null == Session["ProductSubtype3Tmp"])
                Session["ProductSubtype3Tmp"] = SelectProductSubtype3();

            ListLovModel = (List<LovModel>)Session["ProductSubtype3Tmp"];
            return Json(ListLovModel, JsonRequestBehavior.AllowGet);
        }

        public List<LovModel> SelectProductSubtype3()
        {
            var query = new GetAWProductSubtype3Query
            {

            };
            var data = _queryProcessor.Execute(query);

            List<LovModel> ListLovModel = new List<LovModel>();
            foreach (var item in data)
            {
                LovModel lovModel = new LovModel();
                lovModel.LOV_NAME = item.PRODUCT_SUBTYPE3;
                lovModel.LOV_VAL1 = item.PRODUCT_SUBTYPE3;
                ListLovModel.Add(lovModel);
            }
            ListLovModel = ListLovModel.OrderBy(p => p.LOV_NAME).ToList();
            ListLovModel.Insert(0, new LovModel { DISPLAY_VAL = "", LOV_NAME = "", LOV_VAL1 = "" });
            return ListLovModel;
        }

        public ActionResult SavePackageGroup(string ProductType, string PackageGroup, string PackageCode, string Mode)
        {

            List<LovModel> ListLovModel;
            if (null == Session["PackageGroupTmp" + ProductType])
                Session["PackageGroupTmp" + ProductType] = SelectPackageGroup(ProductType);

            ListLovModel = (List<LovModel>)Session["PackageGroupTmp" + ProductType];

            LovModel lovModel = new LovModel();
            lovModel.LOV_NAME = PackageGroup;
            lovModel.LOV_VAL1 = PackageGroup;
            //ListLovModel.Add(lovModel);
            ListLovModel.Insert(0, lovModel);

            Session["PackageGroupTmp" + ProductType] = ListLovModel;

            SetViewBagLov();

            List<PackageType> model;
            if (Mode == "E")
            {
                model = SetProductTypeTable(ProductType, PackageCode, "PackageGroup");
            }
            else
            {
                model = SetProductTypeTable();
            }

            return PartialView("_ProductTypeTable", model);
        }

        public ActionResult SaveProductSubtype3(string ProductType, string ProductSubtype3, string PackageCode, string Mode)
        {
            List<LovModel> ListLovModel;
            if (null == Session["ProductSubtype3Tmp"])
                Session["ProductSubtype3Tmp"] = SelectProductSubtype3();

            ListLovModel = (List<LovModel>)Session["ProductSubtype3Tmp"];

            LovModel lovModel = new LovModel();
            lovModel.LOV_NAME = ProductSubtype3;
            lovModel.LOV_VAL1 = ProductSubtype3;
            //ListLovModel.Add(lovModel);
            ListLovModel.Insert(0, lovModel);
            Session["ProductSubtype3Tmp"] = ListLovModel;

            SetViewBagLov();

            List<PackageType> model;
            if (Mode == "E")
            {
                model = SetProductTypeTable(ProductType, PackageCode, "ProductSubtype3");
            }
            else
            {
                model = SetProductTypeTable();
            }

            return PartialView("_ProductTypeTable", model);
        }

        public ActionResult SaveVendorPartner(string VendorPartner, string PackageCode, string Mode)
        {
            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "Account");

            var User = base.CurrentUser;

            if (VendorPartner != "")
            {
                var command = new SaveVendorPartnerCommand
                {
                    user = User.UserName,
                    vendor_partner = VendorPartner
                };
                _SaveVendorPartnerCommand.Handle(command);
            }


            SetViewBagLov();

            List<VendorOrPartner> model;
            if (Mode == "E")
            {
                model = SetVendorOrPartner(PackageCode);
            }
            else
            {
                model = SetVendorOrPartner();
            }

            return PartialView("_VendorOrPartnerTable", model);
        }

        public ActionResult SelectPackageGroupDesc(string dataItemSelect = "")
        {
            if (dataItemSelect == "")
            {
                return Json(new
                {
                    PackageGroupDescThai = "",
                    PackageGroupDescEng = ""
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                List<PackageGroupDesc> PackageGroupDescList = GetPackageGroupDescriptionByPackageGroup(dataItemSelect);
                PackageGroupDesc packageGroupDesc = new PackageGroupDesc();
                if (PackageGroupDescList.Count > 0)
                {
                    packageGroupDesc = PackageGroupDescList.FirstOrDefault();
                }
                else
                {
                    packageGroupDesc.PackageGroupName = dataItemSelect;
                    packageGroupDesc.PackageGroupDescriptionThai = "";
                    packageGroupDesc.PackageGroupDescriptionEng = "";
                }

                return Json(new
                {
                    PackageGroupDescThai = packageGroupDesc.PackageGroupDescriptionThai,
                    PackageGroupDescEng = packageGroupDesc.PackageGroupDescriptionEng,
                }, JsonRequestBehavior.AllowGet);
            }

        }

        private void SetViewBagLov()
        {
            var LovDataScreen = base.LovData.Where(p => p.Type == "SCREEN" && p.LovValue5 == "FBBOR012_1").ToList();
            ViewBag.configscreen = LovDataScreen;

            var masterController = Bootstrapper.GetInstance<MasterDataController>();
            var LovDataTechnology = masterController.GetLovByTypeAndLovVal5("TECHNOLOGY", "FBBOR012_1");
            ViewBag.technology = LovDataTechnology;
        }

        private List<PackageType> SetProductTypeTable()
        {
            var LovProductTypeData = base.LovData.Where(p => p.Type == "PRODUCT_TYPE" && p.LovValue5 == "FBBOR012_1").ToList();
            ViewBag.configDDLProductType = LovProductTypeData;

            List<PackageType> ListPackageType = new List<PackageType>();
            foreach (var item in LovProductTypeData)
            {
                PackageType PackageTypeModel = new PackageType();
                PackageTypeModel.PACKAGE_SELECT = false;
                PackageTypeModel.PACKAGE_SELECTOld = false;
                PackageTypeModel.PACKAGE_TYPE = item.LovValue1;
                PackageTypeModel.PACKAGE_TYPE_DISPLAY = item.Name;

                ListPackageType.Add(PackageTypeModel);
            }

            return ListPackageType;

        }

        private List<PackageType> SetProductTypeTable(string ProductType, string PackageCode, string SaveType)
        {
            var result2 = GetAWConfigurationPackageProductTypeEdit(PackageCode);

            List<PackageType> ListPackageType;
            var ListPackageTypeModelView = SetProductTypeTable();
            foreach (var PackageTypeModelView in ListPackageTypeModelView)
            {
                var TempDatas = result2.Where(t => t.product_subtype == PackageTypeModelView.PACKAGE_TYPE).ToList();
                if (TempDatas.Count() > 0)
                {
                    var TempData = TempDatas.FirstOrDefault();
                    if (TempData.product_subtype != ProductType)
                    {
                        string tmpPackageSubType3 = "";
                        if (PackageTypeModelView.PACKAGE_TYPE_DISPLAY == "PBOX")
                        {
                            if (SaveType == "PackageGroup")
                            {
                                tmpPackageSubType3 = TempData.product_subtype3;
                                PackageTypeModelView.PRODUCT_SUBTYPE3 = tmpPackageSubType3;
                            }
                        }

                        string tmpPackageGroup = TempData.package_group;

                        PackageTypeModelView.PACKAGE_GROUP = tmpPackageGroup;
                        List<PackageGroupDesc> PackageGroupDescList = GetPackageGroupDescriptionByPackageGroup(tmpPackageGroup);
                        PackageGroupDesc packageGroupDesc = new PackageGroupDesc();
                        if (PackageGroupDescList.Count > 0)
                        {
                            packageGroupDesc = PackageGroupDescList.FirstOrDefault();
                        }
                        PackageTypeModelView.PACKAGE_GROUP_DESCRIPTION_THAI = packageGroupDesc.PackageGroupDescriptionThai;
                        PackageTypeModelView.PACKAGE_GROUP_DESCRIPTION_THAI_OLD = packageGroupDesc.PackageGroupDescriptionThai;
                        PackageTypeModelView.PACKAGE_GROUP_DESCRIPTION_ENG = packageGroupDesc.PackageGroupDescriptionEng;
                        PackageTypeModelView.PACKAGE_GROUP_DESCRIPTION_ENG_OLD = packageGroupDesc.PackageGroupDescriptionEng;
                        PackageTypeModelView.PACKAGE_SELECT = true;
                        PackageTypeModelView.PACKAGE_SELECTOld = true;
                    }
                    else
                    {
                        PackageTypeModelView.PACKAGE_GROUP_DESCRIPTION_THAI = "";
                        PackageTypeModelView.PACKAGE_GROUP_DESCRIPTION_THAI_OLD = "";
                        PackageTypeModelView.PACKAGE_GROUP_DESCRIPTION_ENG = "";
                        PackageTypeModelView.PACKAGE_GROUP_DESCRIPTION_ENG_OLD = "";
                        PackageTypeModelView.PACKAGE_SELECT = true;
                        PackageTypeModelView.PACKAGE_SELECTOld = true;
                    }
                }
                else
                {
                    PackageTypeModelView.PACKAGE_GROUP_DESCRIPTION_THAI = "";
                    PackageTypeModelView.PACKAGE_GROUP_DESCRIPTION_THAI_OLD = "";
                    PackageTypeModelView.PACKAGE_GROUP_DESCRIPTION_ENG = "";
                    PackageTypeModelView.PACKAGE_GROUP_DESCRIPTION_ENG_OLD = "";
                    PackageTypeModelView.PACKAGE_SELECT = false;
                    PackageTypeModelView.PACKAGE_SELECTOld = false;
                }

            }
            ListPackageType = ListPackageTypeModelView;

            return ListPackageType;
        }

        private List<VendorOrPartner> SetVendorOrPartner()
        {
            var masterController = Bootstrapper.GetInstance<MasterDataController>();
            //var LovVendorPartnerData = base.LovData.Where(p => p.Type == "VENDOR_PARTNER" && p.LovValue5 == "FBBOR012_1").ToList();
            var LovVendorPartnerData = masterController.GetLovList("VENDOR_PARTNER", "").Where(p => p.LovValue5 == "FBBOR012_1").ToList();

            List<VendorOrPartner> ListVendorOrPartner = new List<VendorOrPartner>();

            foreach (var Item in LovVendorPartnerData)
            {
                VendorOrPartner VendorOrPartnerModel = new VendorOrPartner();
                VendorOrPartnerModel.VendorOrPartnerSelect = false;
                VendorOrPartnerModel.VendorOrPartnerSelectOld = false;
                VendorOrPartnerModel.VendorOrPartnerName = Item.Name;
                VendorOrPartnerModel.VendorOrPartnerValue = Item.LovValue1;

                ListVendorOrPartner.Add(VendorOrPartnerModel);
            }

            return ListVendorOrPartner;
        }

        private List<VendorOrPartner> SetVendorOrPartner(string PackageCode)
        {
            var result3 = GetAWConfigurationPackageVendorEdit(PackageCode);

            List<VendorOrPartner> ListVendorOrPartner;
            var ListVendorOrPartnerModelView = SetVendorOrPartner();
            foreach (var VendorOrPartnerModelView in ListVendorOrPartnerModelView)
            {
                var TempData = result3.Where(t => t.owner_product == VendorOrPartnerModelView.VendorOrPartnerName).ToList();
                if (TempData.Count() > 0)
                {
                    VendorOrPartnerModelView.VendorOrPartnerSelect = true;
                    VendorOrPartnerModelView.VendorOrPartnerSelectOld = true;
                }
                else
                {
                    VendorOrPartnerModelView.VendorOrPartnerSelect = false;
                    VendorOrPartnerModelView.VendorOrPartnerSelectOld = false;
                }

            }
            ListVendorOrPartner = ListVendorOrPartnerModelView;
            return ListVendorOrPartner;

        }

        private List<CatalogAndAuthorizeTable> SetCatalogAndAuthorize(string PackageCode)
        {
            var ChannelGroups = GetChannelGroup();
            var PackageUserGroups = GetPackageUserGroup(PackageCode);
            List<CatalogAndAuthorizeTable> results = new List<CatalogAndAuthorizeTable>();
            foreach (var item in ChannelGroups)
            {
                var PackageUserGroup = PackageUserGroups.Where(t => t.CatalogAndAuthorizeName == item.CatalogAndAuthorizeName).ToList();
                CatalogAndAuthorizeTable result = new CatalogAndAuthorizeTable();
                if (PackageUserGroup.Count > 0)
                {
                    result.CatalogAndAuthorizeTableSelect = true;
                    result.CatalogAndAuthorizeTableSelectOld = true;
                }
                else
                {
                    result.CatalogAndAuthorizeTableSelect = false;
                    result.CatalogAndAuthorizeTableSelectOld = false;
                }
                result.CatalogAndAuthorizeTableName = item.CatalogAndAuthorizeName;
                result.CatalogAndAuthorizeTableValue = item.CatalogAndAuthorizeName;
                results.Add(result);
            }
            return results;
        }

        private List<RegionTable> SetZipCode()
        {
            var SetZipCode = GetZipCode();
            List<RegionTable> results = new List<RegionTable>();
            foreach (var item in SetZipCode)
            {
                RegionTable result = new RegionTable();

                //result.RegionTableSelect = false;
                result.RegionTableName = item.SUB_REGION;
                results.Add(result);
            }
            return results;
        }

        private List<ProvinceTable> SetZipCodeProvince(string[] SubRegion)
        {
            var SetZipCodeProvince = GetZipCodeProvince(SubRegion);
            List<ProvinceTable> results = new List<ProvinceTable>();
            foreach (var item in SetZipCodeProvince)
            {
                ProvinceTable result = new ProvinceTable();
                result.ProvinceSelect = item.ProvinceSelect;
                result.ProvinceName = item.ProvinceName;
                result.SubRegion = item.SUB_REGION;
                results.Add(result);
            }
            return results;
        }

        private List<BuildingTable> SetBuildingTable(string Building, string[] Technology)
        {
            var masterController = Bootstrapper.GetInstance<MasterDataController>();
            var LovDataTechnology = masterController.GetLovByTypeAndLovVal5("TECHNOLOGY", "FBBOR012_1");
            string strTechnology = "";
            bool isFirst = true;
            foreach (var item in Technology)
            {
                var TechnologyValue1 = LovDataTechnology.Select(t => new { t.LOV_VAL1, t.LOV_NAME }).Where(p => p.LOV_NAME == item).ToList();
                string strTechnologyValue1 = TechnologyValue1.Select(t => t.LOV_VAL1).FirstOrDefault();
                if (isFirst)
                {
                    strTechnology = strTechnologyValue1;
                    isFirst = false;
                }
                else
                {
                    strTechnology = strTechnology + "," + strTechnologyValue1;
                }
            }
            var SetBuilding = GetBuildingByBuildingNameAndTechnology(Building, strTechnology);
            List<BuildingTable> results = new List<BuildingTable>();
            foreach (var item in SetBuilding)
            {
                BuildingTable result = new BuildingTable();
                result.BuildingSelect = false;
                result.BuildingName = item.building_name;
                result.BuildingNo = item.building_no;
                result.AddressID = item.address_id;
                result.AddressType = item.address_type;
                result.BuildingNameEng = item.building_name_eng;
                result.BuildingNoEng = item.building_no_eng;
                results.Add(result);
            }

            return results;
        }

        private List<BuildingTable> SetBuildingTableUse(string PackageCode)
        {
            var SetBuilding = GetBuildingByPackageCode(PackageCode);
            List<BuildingTable> results = new List<BuildingTable>();
            foreach (var item in SetBuilding)
            {
                BuildingTable result = new BuildingTable();
                result.BuildingSelect = false;
                result.BuildingName = item.building_name;
                result.BuildingNo = item.building_no;
                result.AddressID = item.address_id;
                result.AddressType = item.address_type;
                result.BuildingNameEng = item.building_name_eng;
                result.BuildingNoEng = item.building_no_eng;
                result.EffectiveDtm = item.EffectiveDtm;
                result.ExpireDtm = item.ExpireDtm;
                results.Add(result);
            }

            return results;
        }

        enum StatusPages
        {
            SearchPage,
            SavePage1,
            SavePage2,
            SavePage3,
            SavePage4,
            ReportPage
        }

    }
}
