using FBBConfig.Extensions;
using FBBConfig.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Xml;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.Commons.Master;
using WBBContract.Queries.Commons.Masters;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBContract.Queries.WebServices;
using WBBEntity.Extensions;
using WBBEntity.PanelModels;
using WBBEntity.PanelModels.ExWebServiceModels;
using WBBEntity.PanelModels.FBBWebConfigModels;
using WBBEntity.PanelModels.WebServiceModels;

namespace FBBConfig.Controllers
{
    public class ResendOrderController : FBBConfigController
    {

        private readonly IQueryProcessor _queryProcessor;

        private OrderErrorLogModel result = new OrderErrorLogModel();
        private readonly ICommandHandler<CustRegisterCommand> _custRegCommand;

        public string OWNER_PRODUCT { get; set; }
        public string PACKAGE_CLASS { get; set; }
        public string PACKAGE_CODE { get; set; }

        public ResendOrderController(ILogger logger
            , IQueryProcessor queryProcessor
            , ICommandHandler<CustRegisterCommand> custRegCommand)
        {
            base._Logger = logger;
            _queryProcessor = queryProcessor;
            _custRegCommand = custRegCommand;
        }

        //public RegisterSummaryController(IQueryProcessor queryProcessor,
        //ICommandHandler<CustRegisterCommand> custRegCommand,
        //ICommandHandler<CoverageResultCommand> covResultCommand,
        //ILogger logger)
        //{
        //    _queryProcessor = queryProcessor;
        //    _custRegCommand = custRegCommand;
        //    _covResultCommand = covResultCommand;
        //    base.Logger = logger;
        //}

        public ActionResult ResendOrder()
        {
            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "Account");

            ViewBag.User = base.CurrentUser;
            SiteSession.CurrentUICulture = 1;
            var headerQuery = new GetLovQuery
            {
                LovType = "FBBDORM_CONSTANT",
                LovName = "H_GRID_REQREPORT",
            };
            var headerData = _queryProcessor.Execute(headerQuery);
            ViewBag.configscreen = headerData;

            // ViewBag.LabelFBBTR003 = GetScreenConfig();
            ViewBag.LabelFBBTR001 = GetCoverageScreenConfig();
            ViewBag.LabelFBBTR002 = GetDisplayPackageScreenConfig();
            ViewBag.LabelFBBTR003 = GetCustRegisterScreenConfig();
            ViewBag.LabelFBBTR000 = GetGeneralScreenConfig();
            ViewBag.LabelFBBTR010 = GetVas_Select_Package_ScreenConfig();

            ViewBag.FbbConstant = GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);
            ViewBag.FbbException = GetFbbConstantModel(WebConstants.LovConfigName.FbbExceptionIdCard);
            return View();
        }

        public List<LovScreenValueModel> GetVas_Select_Package_ScreenConfig()
        {
            var screenData = GetScreenConfig(WebConstants.LovConfigName.Vas_Package);
            return screenData;
        }
        //protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        //{
        //    base.Initialize(requestContext);

        //    const string culture = "th-TH";
        //    CultureInfo ci = CultureInfo.GetCultureInfo(culture);

        //    Thread.CurrentThread.CurrentCulture = ci;
        //    Thread.CurrentThread.CurrentUICulture = ci;
        //}

        public List<LovScreenValueModel> GetGeneralScreenConfig()
        {
            var screenData = GetScreenConfig(null);
            return screenData;
        }

        public List<LovScreenValueModel> GetCustRegisterScreenConfig()
        {
            var screenData = GetScreenConfig(WebConstants.LovConfigName.CustomerRegisterPageCode);
            return screenData;
        }
        public List<LovScreenValueModel> GetDisplayPackageScreenConfig()
        {
            var screenData = GetScreenConfig(WebConstants.LovConfigName.DisplayPackagePageCode);
            return screenData;
        }
        public List<LovScreenValueModel> GetCoverageScreenConfig()
        {
            var screenData = GetScreenConfig(WebConstants.LovConfigName.CoveragePageCode);
            return screenData;
        }
        public ActionResult SearchDataSourceOrderErrorLog([DataSourceRequest] DataSourceRequest request, string dataS = "")
        {

            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "Account");

            if (dataS != null && dataS != "")
            {
                var SearchPara = new JavaScriptSerializer().Deserialize<GetOrderErrorLogQuery>(dataS);
                result = GetDataSearchModel(SearchPara);

                return Json(new
                {
                    Data = result.P_RES_DATA,
                    Total = result.P_PAGE_COUNT
                });

            }
            else
            {
                return null;
            }
        }
        private OrderErrorLogModel GetDataSearchModel(GetOrderErrorLogQuery SearchPara)
        {

            try
            {
                var query = new GetOrderErrorLogQuery()
                {
                    P_DATE_FROM = SearchPara.P_DATE_FROM,
                    P_DATE_TO = SearchPara.P_DATE_TO,
                    P_ID_CARD_NO = SearchPara.P_ID_CARD_NO,
                    P_REQUEST_STATUS = SearchPara.P_REQUEST_STATUS,
                    P_PAGE_INDEX = SearchPara.P_PAGE_INDEX,
                    P_PAGE_SIZE = SearchPara.P_PAGE_SIZE

                };

                result = _queryProcessor.Execute(query);
                Session["OrderErrorModel"] = result;

                return result;
            }
            catch (Exception ex)
            {
                _Logger.Info(ex.GetBaseException());
                return result;
            }
        }

        private string RenderRazorViewToString(string viewName, object model)
        {
            ViewData.Model = model;

            using (var stringWriter = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, stringWriter);
                viewResult.View.Render(viewContext, stringWriter);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);

                return stringWriter.GetStringBuilder().ToString();
            }
        }

        [HttpPost]
        public JsonResult GetCustomerRegisViewHtml(string IDCardNo = "")
        {
            string viewHtml = string.Empty;

            QuickWinPanelModel quickWinPanelModel = new QuickWinPanelModel();
            //quickWinPanelModel.CustomerRegisterPanelModel.L_FIRST_NAME = "testtt";
            var viewModel = quickWinPanelModel;
            //EditResendOrder(IDCardNo);

            viewHtml = RenderRazorViewToString("EditOrder", viewModel);

            var hashtable = new Hashtable();
            hashtable["viewHtml"] = viewHtml;

            return Json(hashtable);
        }

        //test

        [HttpPost]
        public ActionResult QuerySomthing(string IDCardNo = "")
        {
            QuickWinPanelModel quickWinPanelModel = new QuickWinPanelModel();
            quickWinPanelModel.CustomerRegisterPanelModel.L_FIRST_NAME = "testtt";
            var viewModel = quickWinPanelModel;

            var hashtable = new Hashtable();
            hashtable["viewHtml"] = viewModel;

            return Json(quickWinPanelModel, JsonRequestBehavior.AllowGet);

        }

        private string IIf(bool Expression, string TruePart, string FalsePart)
        {
            string ReturnValue = Expression == true ? TruePart : FalsePart;

            return ReturnValue;
        }
        private List<FbbConstantModel> GetFbbConstantModel(string fbbConstType)
        {
            var data = base.LovData
               .Where(l => l.Type.Equals(fbbConstType))
               .Select(l => new FbbConstantModel
               {
                   Field = l.Name,
                   Validation = l.LovValue1,
                   SubValidation = l.LovValue2
               }).ToList();

            return data;
        }

        [HttpPost]
        public ActionResult Check_Session()
        {
            string _Result = "1";
            if (null == base.CurrentUser)
            {
                _Result = "0";
            }
            return Json(new { Result = _Result });
        }

        [HttpPost]
        public ActionResult EditResendOrder(string IDCardNo = "")
        {

            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "Account");
            ViewBag.User = base.CurrentUser;

            ///////////// For Serenade_Flag !!
            //StringReader streamSer = null;
            //XmlTextReader readerSer = null;
            //DataSet xmlDSSer = new DataSet();  
            //var query = new GetPackagebyServiceQuery()
            //{
            //    P_IN_TRANSACTION_ID = "0687236473"
            //};
            //var _FB_Interfce_log_byServiceModel = _queryProcessor.Execute(query);

            //streamSer = new StringReader(_FB_Interfce_log_byServiceModel.P_IN_XML_PARAM[0].IN_XML_PARAM);
            //readerSer = new XmlTextReader(streamSer);
            //xmlDSSer.ReadXml(readerSer);
            //DataTable _GetListPackageByServiceQuery = xmlDSSer.Tables["GetListPackageByServiceQuery"];
            //_GetListPackageByServiceQuery_Serenade_Flag = _GetListPackageByServiceQuery.Rows[0]["P_Serenade_Flag"].ToSafeString();
            //ViewBag.GetListPackageByServiceQuery_Serenade_Flag = "test";

            QuickWinPanelModel quickWinPanelModel = new QuickWinPanelModel();
            GetSaveOrderRespQuery getsaveorder = new GetSaveOrderRespQuery();
            result = Session["OrderErrorModel"] as OrderErrorLogModel;

            List<string> xml = new List<string>();
            if (result.P_RES_DATA != null)
            {
                foreach (string xmlPara in result.P_RES_DATA.Where(r => r.IN_ID_CARD_NO == IDCardNo).Select(r => r.IN_XML_PARAM))
                {
                    if (!string.IsNullOrEmpty(xmlPara))
                    {
                        xml.Add(xmlPara);
                    }

                }
            }

            StringReader stream = null;
            XmlTextReader reader = null;
            DataSet xmlDS = new DataSet();
            try
            {

                stream = new StringReader(xml[0]);
                reader = new XmlTextReader(stream);
                xmlDS.ReadXml(reader);

                DataTable addressDt = new DataTable();
                DataTable coveragePanelDt = new DataTable();
                DataTable quickWinPanelModelDt = new DataTable();
                DataTable customerRegisterPanelModelDt = new DataTable();
                DataTable summaryPanelModelDt = new DataTable();
                DataTable addressPanelModelSetupDt = new DataTable();
                DataTable addressPanelModelSendDocDt = new DataTable();
                DataTable coverageAreaResultModelDt = new DataTable();
                DataTable packageModelDt = new DataTable();
                DataTable timeslotDt = new DataTable();
                DataTable uploadImageDt = new DataTable();
                DataTable GetSaveOrderDt = new DataTable();
                DataTable addressPanelModelVatDt = new DataTable();
                DataTable DisplayPackagePanelModelDt = new DataTable();

                GetSaveOrderDt = xmlDS.Tables["GetSaveOrderRespQuery"];
                quickWinPanelModelDt = xmlDS.Tables["QuickWinPanelModel"];
                customerRegisterPanelModelDt = xmlDS.Tables["CustomerRegisterPanelModel"];
                addressDt = xmlDS.Tables["Address"];
                coveragePanelDt = xmlDS.Tables["CoveragePanelModel"];
                summaryPanelModelDt = xmlDS.Tables["SummaryPanelModel"];
                addressPanelModelSetupDt = xmlDS.Tables["AddressPanelModelSetup"];
                addressPanelModelSendDocDt = xmlDS.Tables["AddressPanelModelSendDoc"];
                coverageAreaResultModelDt = xmlDS.Tables["CoverageAreaResultModel"];
                packageModelDt = xmlDS.Tables["PackageModel"];
                timeslotDt = xmlDS.Tables["FBSSTimeSlot"];
                uploadImageDt = xmlDS.Tables["UploadImage"];
                addressPanelModelVatDt = xmlDS.Tables["AddressPanelModelVat"];
                DisplayPackagePanelModelDt = xmlDS.Tables["DisplayPackagePanelModel"];

                if (GetSaveOrderDt != null)
                {
                    getsaveorder.CurrentCulture = 1;
                    if (GetSaveOrderDt.Rows.Count > 0)
                    {
                        if (GetSaveOrderDt.Columns.Contains("CurrentCulture"))
                        {
                            if (GetSaveOrderDt.Rows[0]["CurrentCulture"].ToSafeString() == "") TempData["CurrentCulture"] = 0;
                            else TempData["CurrentCulture"] = Convert.ToInt32(GetSaveOrderDt.Rows[0]["CurrentCulture"].ToSafeString());
                        }
                    }
                }

                if (addressPanelModelVatDt != null)
                {
                    if (addressPanelModelVatDt.Rows.Count > 0)
                    {
                        quickWinPanelModel.CustomerRegisterPanelModel.AddressPanelModelVat.AddressId = "";
                        quickWinPanelModel.CustomerRegisterPanelModel.AddressPanelModelVat.GIFT_VOUCHER = "";
                        quickWinPanelModel.CustomerRegisterPanelModel.AddressPanelModelVat.INSTALL_STAFF_ID = "";
                        quickWinPanelModel.CustomerRegisterPanelModel.AddressPanelModelVat.INSTALL_STAFF_NAME = "";
                        quickWinPanelModel.CustomerRegisterPanelModel.AddressPanelModelVat.L_BUILD_NAME = "";
                        quickWinPanelModel.CustomerRegisterPanelModel.AddressPanelModelVat.L_BUILD_NO_Hied = "";
                        quickWinPanelModel.CustomerRegisterPanelModel.AddressPanelModelVat.L_FLOOR = "";
                        quickWinPanelModel.CustomerRegisterPanelModel.AddressPanelModelVat.L_HOME_NUMBER_1 = "";
                        quickWinPanelModel.CustomerRegisterPanelModel.AddressPanelModelVat.L_HOME_NUMBER_2 = "";
                        quickWinPanelModel.CustomerRegisterPanelModel.AddressPanelModelVat.L_MOO = "";
                        quickWinPanelModel.CustomerRegisterPanelModel.AddressPanelModelVat.L_MOOBAN = "";
                        quickWinPanelModel.CustomerRegisterPanelModel.AddressPanelModelVat.L_ROAD = "";
                        quickWinPanelModel.CustomerRegisterPanelModel.AddressPanelModelVat.L_ROOM = "";
                        quickWinPanelModel.CustomerRegisterPanelModel.AddressPanelModelVat.L_SOI = "";
                        quickWinPanelModel.CustomerRegisterPanelModel.AddressPanelModelVat.SUB_CONTRACT_NAME = "";
                        quickWinPanelModel.CustomerRegisterPanelModel.AddressPanelModelVat.SUB_LOCATION_ID = "";
                        quickWinPanelModel.CustomerRegisterPanelModel.AddressPanelModelVat.L_PROVINCE = "";
                        quickWinPanelModel.CustomerRegisterPanelModel.AddressPanelModelVat.L_TUMBOL = "";
                        quickWinPanelModel.CustomerRegisterPanelModel.AddressPanelModelVat.L_AMPHUR = "";
                        quickWinPanelModel.CustomerRegisterPanelModel.AddressPanelModelVat.L_ZIPCODE = "";
                        quickWinPanelModel.CustomerRegisterPanelModel.AddressPanelModelVat.ZIPCODE_ID = "";

                        if (addressPanelModelVatDt.Columns.Contains("AddressId"))
                        {
                            quickWinPanelModel.CustomerRegisterPanelModel.AddressPanelModelVat.AddressId = IIf(addressPanelModelVatDt.Rows[0]["AddressId"].ToString() != null, addressPanelModelVatDt.Rows[0]["AddressId"].ToString(), "");
                        }

                        if (addressPanelModelVatDt.Columns.Contains("GIFT_VOUCHER"))
                        {
                            quickWinPanelModel.CustomerRegisterPanelModel.AddressPanelModelVat.GIFT_VOUCHER = IIf(addressPanelModelVatDt.Rows[0]["GIFT_VOUCHER"].ToString() != null, addressPanelModelVatDt.Rows[0]["GIFT_VOUCHER"].ToString(), "");
                        }

                        if (addressPanelModelVatDt.Columns.Contains("INSTALL_STAFF_ID"))
                        {
                            quickWinPanelModel.CustomerRegisterPanelModel.AddressPanelModelVat.INSTALL_STAFF_ID = IIf(addressPanelModelVatDt.Rows[0]["INSTALL_STAFF_ID"].ToString() != null, addressPanelModelVatDt.Rows[0]["INSTALL_STAFF_ID"].ToString(), "");
                        }

                        if (addressPanelModelVatDt.Columns.Contains("INSTALL_STAFF_NAME"))
                        {
                            quickWinPanelModel.CustomerRegisterPanelModel.AddressPanelModelVat.INSTALL_STAFF_NAME = IIf(addressPanelModelVatDt.Rows[0]["INSTALL_STAFF_NAME"].ToString() != null, addressPanelModelVatDt.Rows[0]["INSTALL_STAFF_NAME"].ToString(), "");
                        }

                        if (addressPanelModelVatDt.Columns.Contains("L_BUILD_NAME"))
                        {
                            quickWinPanelModel.CustomerRegisterPanelModel.AddressPanelModelVat.L_BUILD_NAME = IIf(addressPanelModelVatDt.Rows[0]["L_BUILD_NAME"].ToString() != null, addressPanelModelVatDt.Rows[0]["L_BUILD_NAME"].ToString(), "");
                        }

                        if (addressPanelModelVatDt.Columns.Contains("L_BUILD_NO_Hied"))
                        {
                            quickWinPanelModel.CustomerRegisterPanelModel.AddressPanelModelVat.L_BUILD_NO_Hied = IIf(addressPanelModelVatDt.Rows[0]["L_BUILD_NO_Hied"].ToString() != null, addressPanelModelVatDt.Rows[0]["L_BUILD_NO_Hied"].ToString(), "");
                        }

                        if (addressPanelModelVatDt.Columns.Contains("L_FLOOR"))
                        {
                            quickWinPanelModel.CustomerRegisterPanelModel.AddressPanelModelVat.L_FLOOR = IIf(addressPanelModelVatDt.Rows[0]["L_FLOOR"].ToString() != null, addressPanelModelVatDt.Rows[0]["L_FLOOR"].ToString(), "");
                        }

                        if (addressPanelModelVatDt.Columns.Contains("L_HOME_NUMBER_1"))
                        {
                            quickWinPanelModel.CustomerRegisterPanelModel.AddressPanelModelVat.L_HOME_NUMBER_1 = IIf(addressPanelModelVatDt.Rows[0]["L_HOME_NUMBER_1"].ToString() != null, addressPanelModelVatDt.Rows[0]["L_HOME_NUMBER_1"].ToString(), "");
                        }

                        if (addressPanelModelVatDt.Columns.Contains("L_HOME_NUMBER_2"))
                        {
                            quickWinPanelModel.CustomerRegisterPanelModel.AddressPanelModelVat.L_HOME_NUMBER_2 = IIf(addressPanelModelVatDt.Rows[0]["L_HOME_NUMBER_2"].ToString() != null, addressPanelModelVatDt.Rows[0]["L_HOME_NUMBER_2"].ToString(), "");
                        }

                        if (addressPanelModelVatDt.Columns.Contains("L_MOO"))
                        {
                            quickWinPanelModel.CustomerRegisterPanelModel.AddressPanelModelVat.L_MOO = IIf(addressPanelModelVatDt.Rows[0]["L_MOO"].ToString() != null, addressPanelModelVatDt.Rows[0]["L_MOO"].ToString(), "");
                        }

                        if (addressPanelModelVatDt.Columns.Contains("L_MOOBAN"))
                        {
                            quickWinPanelModel.CustomerRegisterPanelModel.AddressPanelModelVat.L_MOOBAN = IIf(addressPanelModelVatDt.Rows[0]["L_MOOBAN"].ToString() != null, addressPanelModelVatDt.Rows[0]["L_MOOBAN"].ToString(), "");
                        }

                        if (addressPanelModelVatDt.Columns.Contains("L_ROAD"))
                        {
                            quickWinPanelModel.CustomerRegisterPanelModel.AddressPanelModelVat.L_ROAD = IIf(addressPanelModelVatDt.Rows[0]["L_ROAD"].ToString() != null, addressPanelModelVatDt.Rows[0]["L_ROAD"].ToString(), "");
                        }

                        if (addressPanelModelVatDt.Columns.Contains("L_ROOM"))
                        {
                            quickWinPanelModel.CustomerRegisterPanelModel.AddressPanelModelVat.L_ROOM = IIf(addressPanelModelVatDt.Rows[0]["L_ROOM"].ToString() != null, addressPanelModelVatDt.Rows[0]["L_ROOM"].ToString(), "");
                        }

                        if (addressPanelModelVatDt.Columns.Contains("L_SOI"))
                        {
                            quickWinPanelModel.CustomerRegisterPanelModel.AddressPanelModelVat.L_SOI = IIf(addressPanelModelVatDt.Rows[0]["L_SOI"].ToString() != null, addressPanelModelVatDt.Rows[0]["L_SOI"].ToString(), "");
                        }

                        if (addressPanelModelVatDt.Columns.Contains("SUB_CONTRACT_NAME"))
                        {
                            quickWinPanelModel.CustomerRegisterPanelModel.AddressPanelModelVat.SUB_CONTRACT_NAME = IIf(addressPanelModelVatDt.Rows[0]["SUB_CONTRACT_NAME"].ToString() != null, addressPanelModelVatDt.Rows[0]["SUB_CONTRACT_NAME"].ToString(), "");
                        }

                        if (addressPanelModelVatDt.Columns.Contains("SUB_LOCATION_ID"))
                        {
                            quickWinPanelModel.CustomerRegisterPanelModel.AddressPanelModelVat.SUB_LOCATION_ID = IIf(addressPanelModelVatDt.Rows[0]["SUB_LOCATION_ID"].ToString() != null, addressPanelModelVatDt.Rows[0]["SUB_LOCATION_ID"].ToString(), "");
                        }

                        if (addressPanelModelVatDt.Columns.Contains("L_PROVINCE"))
                        {
                            quickWinPanelModel.CustomerRegisterPanelModel.AddressPanelModelVat.L_PROVINCE = IIf(addressPanelModelVatDt.Rows[0]["L_PROVINCE"].ToString() != null, addressPanelModelVatDt.Rows[0]["L_PROVINCE"].ToString(), "");
                        }

                        if (addressPanelModelVatDt.Columns.Contains("L_TUMBOL"))
                        {
                            quickWinPanelModel.CustomerRegisterPanelModel.AddressPanelModelVat.L_TUMBOL = IIf(addressPanelModelVatDt.Rows[0]["L_TUMBOL"].ToString() != null, addressPanelModelVatDt.Rows[0]["L_TUMBOL"].ToString(), "");
                        }

                        if (addressPanelModelVatDt.Columns.Contains("L_AMPHUR"))
                        {
                            quickWinPanelModel.CustomerRegisterPanelModel.AddressPanelModelVat.L_AMPHUR = IIf(addressPanelModelVatDt.Rows[0]["L_AMPHUR"].ToString() != null, addressPanelModelVatDt.Rows[0]["L_AMPHUR"].ToString(), "");
                        }

                        if (addressPanelModelVatDt.Columns.Contains("L_ZIPCODE"))
                        {
                            quickWinPanelModel.CustomerRegisterPanelModel.AddressPanelModelVat.L_ZIPCODE = IIf(addressPanelModelVatDt.Rows[0]["L_ZIPCODE"].ToString() != null, addressPanelModelVatDt.Rows[0]["L_ZIPCODE"].ToString(), "");
                        }

                        if (addressPanelModelVatDt.Columns.Contains("ZIPCODE_ID"))
                        {
                            quickWinPanelModel.CustomerRegisterPanelModel.AddressPanelModelVat.ZIPCODE_ID = IIf(addressPanelModelVatDt.Rows[0]["ZIPCODE_ID"].ToString() != null, addressPanelModelVatDt.Rows[0]["ZIPCODE_ID"].ToString(), "");
                        }
                    }
                }


                if (uploadImageDt != null)
                {
                    if (uploadImageDt.Rows.Count > 0)
                    {
                        UploadImage filename;
                        quickWinPanelModel.CustomerRegisterPanelModel.ListImageFile = new List<UploadImage>();
                        foreach (DataRow dr in uploadImageDt.Rows)
                        {
                            if (uploadImageDt.Columns.Contains("FileName"))
                            {
                                if (dr["FileName"].ToString() != "")
                                {
                                    filename = new UploadImage();
                                    //string[] arrFilename = dr["FileName"].ToString().Split(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);

                                    //filename.FileName = arrFilename[arrFilename.Length - 1];
                                    filename.FileName = dr["FileName"].ToString();
                                    quickWinPanelModel.CustomerRegisterPanelModel.ListImageFile.Add(filename);
                                }
                            }
                        }
                    }
                }

                if (quickWinPanelModelDt != null)
                {
                    if (quickWinPanelModelDt.Rows.Count > 0)
                    {
                        quickWinPanelModel.PlugAndPlayFlow = "";
                        quickWinPanelModel.TopUp = "";
                        if (quickWinPanelModelDt.Columns.Contains("PlugAndPlayFlow"))
                        {
                            quickWinPanelModel.PlugAndPlayFlow = IIf(quickWinPanelModelDt.Rows[0]["PlugAndPlayFlow"].ToString() != null, quickWinPanelModelDt.Rows[0]["PlugAndPlayFlow"].ToString(), "");
                        }
                        if (quickWinPanelModelDt.Columns.Contains("TopUp"))
                        {
                            quickWinPanelModel.TopUp = IIf(quickWinPanelModelDt.Rows[0]["TopUp"].ToString() != null, quickWinPanelModelDt.Rows[0]["TopUp"].ToString(), "");
                        }
                    }
                }

                //summary
                if (summaryPanelModelDt != null)
                {
                    if (summaryPanelModelDt.Rows.Count > 0)
                    {
                        quickWinPanelModel.SummaryPanelModel.VAS_FLAG = "";
                        if (summaryPanelModelDt.Columns.Contains("VAS_FLAG"))
                        {
                            quickWinPanelModel.SummaryPanelModel.VAS_FLAG = IIf(summaryPanelModelDt.Rows[0]["VAS_FLAG"].ToString() != null, summaryPanelModelDt.Rows[0]["VAS_FLAG"].ToString(), "");
                        }
                    }
                }

                if (packageModelDt != null)
                {
                    if (packageModelDt.Rows.Count > 0)
                    {
                        quickWinPanelModel.SummaryPanelModel.PackageModel.OWNER_PRODUCT = "";
                        quickWinPanelModel.SummaryPanelModel.PackageModel.PACKAGE_CLASS = "";
                        quickWinPanelModel.SummaryPanelModel.PackageModel.PACKAGE_TYPE = "";
                        quickWinPanelModel.SummaryPanelModel.PackageModel.PACKAGE_CODE = "";
                        quickWinPanelModel.SummaryPanelModel.PackageModel.PRODUCT_SUBTYPE = "";
                        quickWinPanelModel.SummaryPanelModel.PackageModel.SelectVas_Flag = "";
                        quickWinPanelModel.SummaryPanelModel.PackageModel.SelectPlayBox_Flag = "";
                        quickWinPanelModel.SummaryPanelModel.PackageModel.SelectPlayPBL_Flag = "";
                        quickWinPanelModel.SummaryPanelModel.PackageModel.DISCOUNT_VALUE = 0;
                        quickWinPanelModel.SummaryPanelModel.PackageModel.DISCOUNT_DAY = 0;

                        List<PackageModel> ObjOwner = new List<PackageModel>();
                        PackageModel pakagemo;
                        foreach (DataRow dr in packageModelDt.Rows)
                        {
                            pakagemo = new PackageModel();
                            if (packageModelDt.Columns.Contains("SelectVas_Flag"))
                            {
                                pakagemo.SelectVas_Flag = dr["SelectVas_Flag"].ToString();
                            }
                            if (packageModelDt.Columns.Contains("SelectPlayBox_Flag"))
                            {
                                pakagemo.SelectPlayBox_Flag = dr["SelectPlayBox_Flag"].ToString();
                            }
                            if (packageModelDt.Columns.Contains("SelectPlayPBL_Flag"))
                            {
                                pakagemo.SelectPlayPBL_Flag = dr["SelectPlayPBL_Flag"].ToString();
                            }
                            if (packageModelDt.Columns.Contains("DISCOUNT_VALUE"))
                            {
                                if (dr["DISCOUNT_VALUE"] != null)
                                {
                                    if (dr["DISCOUNT_VALUE"].ToSafeString() == "") pakagemo.DISCOUNT_VALUE = 0;
                                    else pakagemo.DISCOUNT_VALUE = Convert.ToInt32(dr["DISCOUNT_VALUE"].ToString());
                                }
                            }
                            if (packageModelDt.Columns.Contains("DISCOUNT_DAY"))
                            {
                                if (dr["DISCOUNT_DAY"] != null)
                                {
                                    if (dr["DISCOUNT_DAY"].ToSafeString() == "") pakagemo.DISCOUNT_DAY = 0;
                                    else pakagemo.DISCOUNT_DAY = Convert.ToInt32(dr["DISCOUNT_DAY"].ToString());
                                }
                            }
                            if (packageModelDt.Columns.Contains("OWNER_PRODUCT"))
                            {
                                pakagemo.OWNER_PRODUCT = dr["OWNER_PRODUCT"].ToString();
                            }
                            if (packageModelDt.Columns.Contains("OWNER_PRODUCT"))
                            {
                                pakagemo.OWNER_PRODUCT = dr["OWNER_PRODUCT"].ToString();
                            }

                            if (packageModelDt.Columns.Contains("PACKAGE_CLASS"))
                            {
                                pakagemo.PACKAGE_CLASS = dr["PACKAGE_CLASS"].ToString();
                            }

                            if (packageModelDt.Columns.Contains("PACKAGE_TYPE"))
                            {
                                pakagemo.PACKAGE_TYPE = dr["PACKAGE_TYPE"].ToString();
                            }

                            if (packageModelDt.Columns.Contains("PACKAGE_CODE"))
                            {
                                pakagemo.PACKAGE_CODE = dr["PACKAGE_CODE"].ToString();
                            }

                            if (packageModelDt.Columns.Contains("PRODUCT_SUBTYPE"))
                            {
                                pakagemo.PRODUCT_SUBTYPE = dr["PRODUCT_SUBTYPE"].ToString();
                            }
                            if (packageModelDt.Columns.Contains("FAX_FLAG"))
                            {
                                pakagemo.FAX_FLAG = dr["FAX_FLAG"].ToString();
                            }
                            if (packageModelDt.Columns.Contains("IDD_FLAG"))
                            {
                                pakagemo.IDD_FLAG = dr["IDD_FLAG"].ToString();
                            }
                            if (packageModelDt.Columns.Contains("INITIATION_CHARGE"))
                            {
                                if (dr["INITIATION_CHARGE"] != null)
                                {
                                    if (dr["INITIATION_CHARGE"].ToSafeString() == "") pakagemo.INITIATION_CHARGE = 0;
                                    else pakagemo.INITIATION_CHARGE = Convert.ToInt32(dr["INITIATION_CHARGE"].ToString());
                                }
                            }
                            ObjOwner.Add(pakagemo);
                        }
                        quickWinPanelModel.ObjOwnerPackage = ObjOwner;

                    }
                }

                if (addressDt.Rows.Count > 0)
                {
                    quickWinPanelModel.CoveragePanelModel.Address.AddressId = "";
                    if (addressDt.Columns.Contains("AddressId"))
                    {
                        quickWinPanelModel.CoveragePanelModel.Address.AddressId = IIf(addressDt.Rows[0]["AddressId"].ToString() != null, addressDt.Rows[0]["AddressId"].ToString(), "");
                    }
                }

                if (coveragePanelDt.Rows.Count > 0)
                {
                    quickWinPanelModel.CoveragePanelModel.L_HAVE_FIXED_LINE = "";
                    quickWinPanelModel.CoveragePanelModel.ChargeType = "";
                    quickWinPanelModel.CoveragePanelModel.Serenade_Flag = "";
                    quickWinPanelModel.CoveragePanelModel.PRODUCT_SUBTYPE = "";
                    quickWinPanelModel.CoveragePanelModel.AccessMode = "";

                    // New 2017/04/12
                    quickWinPanelModel.CoveragePanelModel.BuildingType = "";
                    quickWinPanelModel.CoveragePanelModel.CVRID = "";
                    quickWinPanelModel.CoveragePanelModel.CVR_NODE = "";
                    quickWinPanelModel.CoveragePanelModel.CVR_TOWER = "";
                    quickWinPanelModel.CoveragePanelModel.PRODUCT_SUBTYPE = "";
                    quickWinPanelModel.CoveragePanelModel.P_MOBILE = "";
                    quickWinPanelModel.CoveragePanelModel.SffServiceYear = "";
                    quickWinPanelModel.CoveragePanelModel.P_FTTX_VENDOR = "";
                    quickWinPanelModel.CoveragePanelModel.CoverageResult = "";
                    quickWinPanelModel.CoveragePanelModel.AccessMode = "";
                    quickWinPanelModel.CoveragePanelModel.ServiceCode = "";
                    quickWinPanelModel.CoveragePanelModel.Serenade_Flag = "";
                    quickWinPanelModel.CoveragePanelModel.RESULT_ID = "";
                    quickWinPanelModel.CoveragePanelModel.L_RESULT = "";
                    quickWinPanelModel.CoveragePanelModel.L_FIRST_LAST = "";

                    if (coveragePanelDt.Columns.Contains("L_HAVE_FIXED_LINE"))
                    {
                        quickWinPanelModel.CoveragePanelModel.L_HAVE_FIXED_LINE = IIf(coveragePanelDt.Rows[0]["L_HAVE_FIXED_LINE"].ToString() != null, coveragePanelDt.Rows[0]["L_HAVE_FIXED_LINE"].ToString(), "");
                    }
                    if (coveragePanelDt.Columns.Contains("ChargeType"))
                    {
                        quickWinPanelModel.CoveragePanelModel.ChargeType = IIf(coveragePanelDt.Rows[0]["ChargeType"].ToString() != null, coveragePanelDt.Rows[0]["ChargeType"].ToString(), "");
                    }
                    if (coveragePanelDt.Columns.Contains("Serenade_Flag"))
                    {
                        quickWinPanelModel.CoveragePanelModel.Serenade_Flag = IIf(coveragePanelDt.Rows[0]["Serenade_Flag"].ToString() != null, coveragePanelDt.Rows[0]["Serenade_Flag"].ToString(), "");
                    }
                    if (coveragePanelDt.Columns.Contains("PRODUCT_SUBTYPE"))
                    {
                        quickWinPanelModel.CoveragePanelModel.PRODUCT_SUBTYPE = IIf(coveragePanelDt.Rows[0]["PRODUCT_SUBTYPE"].ToString() != null, coveragePanelDt.Rows[0]["PRODUCT_SUBTYPE"].ToString(), "");
                    }
                    if (coveragePanelDt.Columns.Contains("AccessMode"))
                    {
                        quickWinPanelModel.CoveragePanelModel.AccessMode = IIf(coveragePanelDt.Rows[0]["AccessMode"].ToString() != null, coveragePanelDt.Rows[0]["AccessMode"].ToString(), "");
                    }
                    // New 2017/04/12
                    if (coveragePanelDt.Columns.Contains("BuildingType"))
                    {
                        quickWinPanelModel.CoveragePanelModel.BuildingType = IIf(coveragePanelDt.Rows[0]["BuildingType"].ToString() != null, coveragePanelDt.Rows[0]["BuildingType"].ToString(), "");
                    }
                    if (coveragePanelDt.Columns.Contains("CVRID"))
                    {
                        quickWinPanelModel.CoveragePanelModel.CVRID = IIf(coveragePanelDt.Rows[0]["CVRID"].ToString() != null, coveragePanelDt.Rows[0]["CVRID"].ToString(), "");
                    }
                    if (coveragePanelDt.Columns.Contains("CVR_NODE"))
                    {
                        quickWinPanelModel.CoveragePanelModel.CVR_NODE = IIf(coveragePanelDt.Rows[0]["CVR_NODE"].ToString() != null, coveragePanelDt.Rows[0]["CVR_NODE"].ToString(), "");
                    }
                    if (coveragePanelDt.Columns.Contains("CVR_TOWER"))
                    {
                        quickWinPanelModel.CoveragePanelModel.CVR_TOWER = IIf(coveragePanelDt.Rows[0]["CVR_TOWER"].ToString() != null, coveragePanelDt.Rows[0]["CVR_TOWER"].ToString(), "");
                    }
                    if (coveragePanelDt.Columns.Contains("PRODUCT_SUBTYPE"))
                    {
                        quickWinPanelModel.CoveragePanelModel.PRODUCT_SUBTYPE = IIf(coveragePanelDt.Rows[0]["PRODUCT_SUBTYPE"].ToString() != null, coveragePanelDt.Rows[0]["PRODUCT_SUBTYPE"].ToString(), "");
                    }
                    if (coveragePanelDt.Columns.Contains("P_MOBILE"))
                    {
                        quickWinPanelModel.CoveragePanelModel.P_MOBILE = IIf(coveragePanelDt.Rows[0]["P_MOBILE"].ToString() != null, coveragePanelDt.Rows[0]["P_MOBILE"].ToString(), "");
                    }
                    if (coveragePanelDt.Columns.Contains("SffServiceYear"))
                    {
                        quickWinPanelModel.CoveragePanelModel.SffServiceYear = IIf(coveragePanelDt.Rows[0]["SffServiceYear"].ToString() != null, coveragePanelDt.Rows[0]["SffServiceYear"].ToString(), "");
                    }
                    if (coveragePanelDt.Columns.Contains("P_FTTX_VENDOR"))
                    {
                        quickWinPanelModel.CoveragePanelModel.P_FTTX_VENDOR = IIf(coveragePanelDt.Rows[0]["P_FTTX_VENDOR"].ToString() != null, coveragePanelDt.Rows[0]["P_FTTX_VENDOR"].ToString(), "");
                    }
                    if (coveragePanelDt.Columns.Contains("CoverageResult"))
                    {
                        quickWinPanelModel.CoveragePanelModel.CoverageResult = IIf(coveragePanelDt.Rows[0]["CoverageResult"].ToString() != null, coveragePanelDt.Rows[0]["CoverageResult"].ToString(), "");
                    }
                    if (coveragePanelDt.Columns.Contains("AccessMode"))
                    {
                        quickWinPanelModel.CoveragePanelModel.AccessMode = IIf(coveragePanelDt.Rows[0]["AccessMode"].ToString() != null, coveragePanelDt.Rows[0]["AccessMode"].ToString(), "");
                    }
                    if (coveragePanelDt.Columns.Contains("ServiceCode"))
                    {
                        quickWinPanelModel.CoveragePanelModel.ServiceCode = IIf(coveragePanelDt.Rows[0]["ServiceCode"].ToString() != null, coveragePanelDt.Rows[0]["ServiceCode"].ToString(), "");
                    }
                    if (coveragePanelDt.Columns.Contains("Serenade_Flag"))
                    {
                        quickWinPanelModel.CoveragePanelModel.Serenade_Flag = IIf(coveragePanelDt.Rows[0]["Serenade_Flag"].ToString() != null, coveragePanelDt.Rows[0]["Serenade_Flag"].ToString(), "");
                    }
                    if (coveragePanelDt.Columns.Contains("RESULT_ID"))
                    {
                        quickWinPanelModel.CoveragePanelModel.RESULT_ID = IIf(coveragePanelDt.Rows[0]["RESULT_ID"].ToString() != null, coveragePanelDt.Rows[0]["RESULT_ID"].ToString(), "");
                    }
                    if (coveragePanelDt.Columns.Contains("L_RESULT"))
                    {
                        quickWinPanelModel.CoveragePanelModel.L_RESULT = IIf(coveragePanelDt.Rows[0]["L_RESULT"].ToString() != null, coveragePanelDt.Rows[0]["L_RESULT"].ToString(), "");
                    }
                    if (coveragePanelDt.Columns.Contains("L_FIRST_LAST"))
                    {
                        quickWinPanelModel.CoveragePanelModel.L_FIRST_LAST = IIf(coveragePanelDt.Rows[0]["L_FIRST_LAST"].ToString() != null, coveragePanelDt.Rows[0]["L_FIRST_LAST"].ToString(), "");
                    }
                }

                //customer profile
                if (customerRegisterPanelModelDt.Rows.Count > 0)
                {
                    quickWinPanelModel.CustomerRegisterPanelModel.L_TITLE = "";
                    quickWinPanelModel.CustomerRegisterPanelModel.L_TITLE_CODE = "";
                    quickWinPanelModel.CustomerRegisterPanelModel.L_FIRST_NAME = "";
                    quickWinPanelModel.CustomerRegisterPanelModel.L_LAST_NAME = "";
                    quickWinPanelModel.CustomerRegisterPanelModel.L_CARD_TYPE = "";
                    quickWinPanelModel.CustomerRegisterPanelModel.L_CARD_NO = "";
                    quickWinPanelModel.CustomerRegisterPanelModel.L_GENDER = "";
                    quickWinPanelModel.CustomerRegisterPanelModel.L_BIRTHDAY = "";
                    quickWinPanelModel.CustomerRegisterPanelModel.L_MOBILE = "";
                    quickWinPanelModel.CustomerRegisterPanelModel.L_SPECIFIC_TIME = "";
                    quickWinPanelModel.CustomerRegisterPanelModel.L_EMAIL = "";
                    quickWinPanelModel.CustomerRegisterPanelModel.L_NATIONALITY = "";
                    quickWinPanelModel.CustomerRegisterPanelModel.L_TYPE_ADDR = "";
                    quickWinPanelModel.CustomerRegisterPanelModel.L_INSTALL_DATE = "";
                    quickWinPanelModel.CustomerRegisterPanelModel.L_CONTACT_PHONE = "";
                    quickWinPanelModel.CustomerRegisterPanelModel.L_CONTACT_PERSON = "";
                    quickWinPanelModel.CustomerRegisterPanelModel.L_OR = "";
                    quickWinPanelModel.CustomerRegisterPanelModel.L_HOME_PHONE = "";
                    quickWinPanelModel.CustomerRegisterPanelModel.L_REMARK = "";
                    quickWinPanelModel.CustomerRegisterPanelModel.CateType = "";
                    quickWinPanelModel.CustomerRegisterPanelModel.SubCateType = "";

                    quickWinPanelModel.CustomerRegisterPanelModel.outType = "";
                    quickWinPanelModel.CustomerRegisterPanelModel.L_ASC_CODE = "";
                    //quickWinPanelModel.CustomerRegisterPanelModel.L_LOC_CODE = "";
                    quickWinPanelModel.CustomerRegisterPanelModel.outSubType = "";
                    quickWinPanelModel.CustomerRegisterPanelModel.AccountCategory = "";
                    quickWinPanelModel.CustomerRegisterPanelModel.L_GOVERNMENT_NAME = "";

                    quickWinPanelModel.CustomerRegisterPanelModel.DocType = "";
                    quickWinPanelModel.CustomerRegisterPanelModel.L_SALE_REP = "";
                    quickWinPanelModel.CustomerRegisterPanelModel.L_ASC_CODE = "";
                    quickWinPanelModel.CustomerRegisterPanelModel.L_STAFF_ID = "";
                    quickWinPanelModel.CustomerRegisterPanelModel.L_LOC_CODE = "";
                    quickWinPanelModel.CustomerRegisterPanelModel.L_FOR_CS_TEAM = "";
                    quickWinPanelModel.CustomerRegisterPanelModel.CPE_Serial = "";
                    quickWinPanelModel.CustomerRegisterPanelModel.L_EVENT_CODE = "";
                    quickWinPanelModel.CustomerRegisterPanelModel.L_VOUCHER_PIN = "";

                    quickWinPanelModel.CustomerRegisterPanelModel.RentalFlag = "";


                    if (customerRegisterPanelModelDt.Columns.Contains("L_TITLE_CODE"))
                    {
                        quickWinPanelModel.CustomerRegisterPanelModel.L_TITLE_CODE = IIf(customerRegisterPanelModelDt.Rows[0]["L_TITLE_CODE"].ToString() != null, customerRegisterPanelModelDt.Rows[0]["L_TITLE_CODE"].ToString(), "");
                    }

                    if (customerRegisterPanelModelDt.Columns.Contains("L_TITLE"))
                    {
                        quickWinPanelModel.CustomerRegisterPanelModel.L_TITLE = IIf(customerRegisterPanelModelDt.Rows[0]["L_TITLE"].ToString() != null, customerRegisterPanelModelDt.Rows[0]["L_TITLE"].ToString(), "");
                    }

                    if (customerRegisterPanelModelDt.Columns.Contains("L_GOVERNMENT_NAME"))
                    {
                        quickWinPanelModel.CustomerRegisterPanelModel.L_GOVERNMENT_NAME = IIf(customerRegisterPanelModelDt.Rows[0]["L_GOVERNMENT_NAME"].ToString() != null, customerRegisterPanelModelDt.Rows[0]["L_GOVERNMENT_NAME"].ToString(), "");
                    }


                    if (customerRegisterPanelModelDt.Columns.Contains("L_FIRST_NAME"))
                    {
                        quickWinPanelModel.CustomerRegisterPanelModel.L_FIRST_NAME = customerRegisterPanelModelDt.Rows[0]["L_FIRST_NAME"].ToString();
                    }

                    if (customerRegisterPanelModelDt.Columns.Contains("L_LAST_NAME"))
                    {
                        quickWinPanelModel.CustomerRegisterPanelModel.L_LAST_NAME = customerRegisterPanelModelDt.Rows[0]["L_LAST_NAME"].ToString();
                    }

                    if (customerRegisterPanelModelDt.Columns.Contains("L_CARD_TYPE"))
                    {
                        quickWinPanelModel.CustomerRegisterPanelModel.L_CARD_TYPE = IIf(customerRegisterPanelModelDt.Rows[0]["L_CARD_TYPE"].ToString() != null, customerRegisterPanelModelDt.Rows[0]["L_CARD_TYPE"].ToString(), "");
                        quickWinPanelModel.CustomerRegisterPanelModel.L_CARD_TYPE = get_fbb_cfg_lov(WebConstants.LovConfigName.CardType, quickWinPanelModel.CustomerRegisterPanelModel.L_CARD_TYPE);
                    }

                    if (customerRegisterPanelModelDt.Columns.Contains("L_CARD_NO"))
                    {
                        quickWinPanelModel.CustomerRegisterPanelModel.L_CARD_NO = IIf(customerRegisterPanelModelDt.Rows[0]["L_CARD_NO"].ToString() != null, customerRegisterPanelModelDt.Rows[0]["L_CARD_NO"].ToString(), "");
                    }

                    if (customerRegisterPanelModelDt.Columns.Contains("L_GENDER"))
                    {
                        quickWinPanelModel.CustomerRegisterPanelModel.L_GENDER = IIf(customerRegisterPanelModelDt.Rows[0]["L_GENDER"].ToString() != null, customerRegisterPanelModelDt.Rows[0]["L_GENDER"].ToString(), "");
                        quickWinPanelModel.CustomerRegisterPanelModel.L_GENDER = get_fbb_cfg_lov_gender("GENDER", quickWinPanelModel.CustomerRegisterPanelModel.L_GENDER);
                    }

                    if (customerRegisterPanelModelDt.Columns.Contains("L_BIRTHDAY"))
                    {
                        quickWinPanelModel.CustomerRegisterPanelModel.L_BIRTHDAY = IIf(customerRegisterPanelModelDt.Rows[0]["L_BIRTHDAY"].ToString() != null, customerRegisterPanelModelDt.Rows[0]["L_BIRTHDAY"].ToString(), "");
                    }

                    if (customerRegisterPanelModelDt.Columns.Contains("L_MOBILE"))
                    {
                        quickWinPanelModel.CustomerRegisterPanelModel.L_MOBILE = IIf(customerRegisterPanelModelDt.Rows[0]["L_MOBILE"].ToString() != null, customerRegisterPanelModelDt.Rows[0]["L_MOBILE"].ToString(), "");
                    }

                    if (customerRegisterPanelModelDt.Columns.Contains("L_SPECIFIC_TIME"))
                    {
                        quickWinPanelModel.CustomerRegisterPanelModel.L_SPECIFIC_TIME = IIf(customerRegisterPanelModelDt.Rows[0]["L_SPECIFIC_TIME"].ToString() != null, customerRegisterPanelModelDt.Rows[0]["L_SPECIFIC_TIME"].ToString().Trim(), "");
                    }

                    if (customerRegisterPanelModelDt.Columns.Contains("L_EMAIL"))
                    {
                        quickWinPanelModel.CustomerRegisterPanelModel.L_EMAIL = IIf(customerRegisterPanelModelDt.Rows[0]["L_EMAIL"].ToString() != null, customerRegisterPanelModelDt.Rows[0]["L_EMAIL"].ToString(), "");
                    }

                    if (customerRegisterPanelModelDt.Columns.Contains("L_NATIONALITY"))
                    {
                        quickWinPanelModel.CustomerRegisterPanelModel.L_NATIONALITY = IIf(customerRegisterPanelModelDt.Rows[0]["L_NATIONALITY"].ToString() != null, customerRegisterPanelModelDt.Rows[0]["L_NATIONALITY"].ToString(), "");
                    }
                    if (customerRegisterPanelModelDt.Columns.Contains("L_TYPE_ADDR"))
                    {
                        quickWinPanelModel.CustomerRegisterPanelModel.L_TYPE_ADDR = IIf(customerRegisterPanelModelDt.Rows[0]["L_TYPE_ADDR"].ToString() != null, customerRegisterPanelModelDt.Rows[0]["L_TYPE_ADDR"].ToString(), "");
                    }

                    if (customerRegisterPanelModelDt.Columns.Contains("L_INSTALL_DATE"))
                    {
                        quickWinPanelModel.CustomerRegisterPanelModel.L_INSTALL_DATE = IIf(customerRegisterPanelModelDt.Rows[0]["L_INSTALL_DATE"].ToString() != null, customerRegisterPanelModelDt.Rows[0]["L_INSTALL_DATE"].ToString(), "");
                    }

                    if (customerRegisterPanelModelDt.Columns.Contains("L_CONTACT_PHONE"))
                    {
                        quickWinPanelModel.CustomerRegisterPanelModel.L_CONTACT_PHONE = IIf(customerRegisterPanelModelDt.Rows[0]["L_CONTACT_PHONE"].ToString() != null, customerRegisterPanelModelDt.Rows[0]["L_CONTACT_PHONE"].ToString(), "");
                    }

                    if (customerRegisterPanelModelDt.Columns.Contains("L_CONTACT_PERSON"))
                    {
                        quickWinPanelModel.CustomerRegisterPanelModel.L_CONTACT_PERSON = IIf(customerRegisterPanelModelDt.Rows[0]["L_CONTACT_PERSON"].ToString() != null, customerRegisterPanelModelDt.Rows[0]["L_CONTACT_PERSON"].ToString(), "");
                    }

                    if (customerRegisterPanelModelDt.Columns.Contains("L_OR"))
                    {
                        quickWinPanelModel.CustomerRegisterPanelModel.L_OR = IIf(customerRegisterPanelModelDt.Rows[0]["L_OR"].ToString() != null, customerRegisterPanelModelDt.Rows[0]["L_OR"].ToString(), "");
                    }

                    if (customerRegisterPanelModelDt.Columns.Contains("L_HOME_PHONE"))
                    {
                        quickWinPanelModel.CustomerRegisterPanelModel.L_HOME_PHONE = IIf(customerRegisterPanelModelDt.Rows[0]["L_HOME_PHONE"].ToString() != null, customerRegisterPanelModelDt.Rows[0]["L_HOME_PHONE"].ToString(), "");
                    }


                    if (customerRegisterPanelModelDt.Columns.Contains("L_REMARK"))
                    {
                        quickWinPanelModel.CustomerRegisterPanelModel.L_REMARK = IIf(customerRegisterPanelModelDt.Rows[0]["L_REMARK"].ToString() != null, customerRegisterPanelModelDt.Rows[0]["L_REMARK"].ToString(), "");
                    }

                    if (customerRegisterPanelModelDt.Columns.Contains("CateType"))
                    {
                        quickWinPanelModel.CustomerRegisterPanelModel.CateType = IIf(customerRegisterPanelModelDt.Rows[0]["CateType"].ToString() != null, customerRegisterPanelModelDt.Rows[0]["CateType"].ToString(), "");
                    }

                    if (customerRegisterPanelModelDt.Columns.Contains("SubCateType"))
                    {
                        quickWinPanelModel.CustomerRegisterPanelModel.SubCateType = IIf(customerRegisterPanelModelDt.Rows[0]["SubCateType"].ToString() != null, customerRegisterPanelModelDt.Rows[0]["SubCateType"].ToString(), "");
                    }
                    if (customerRegisterPanelModelDt.Columns.Contains("DocType"))
                    {
                        quickWinPanelModel.CustomerRegisterPanelModel.DocType = IIf(customerRegisterPanelModelDt.Rows[0]["DocType"].ToString() != null, customerRegisterPanelModelDt.Rows[0]["DocType"].ToString(), "");
                    }
                    if (customerRegisterPanelModelDt.Columns.Contains("L_SALE_REP"))
                    {
                        quickWinPanelModel.CustomerRegisterPanelModel.L_SALE_REP = IIf(customerRegisterPanelModelDt.Rows[0]["L_SALE_REP"].ToString() != null, customerRegisterPanelModelDt.Rows[0]["L_SALE_REP"].ToString(), "");
                    }
                    if (customerRegisterPanelModelDt.Columns.Contains("L_ASC_CODE"))
                    {
                        quickWinPanelModel.CustomerRegisterPanelModel.L_ASC_CODE = IIf(customerRegisterPanelModelDt.Rows[0]["L_ASC_CODE"].ToString() != null, customerRegisterPanelModelDt.Rows[0]["L_ASC_CODE"].ToString(), "");
                    }
                    if (customerRegisterPanelModelDt.Columns.Contains("L_STAFF_ID"))
                    {
                        quickWinPanelModel.CustomerRegisterPanelModel.L_STAFF_ID = IIf(customerRegisterPanelModelDt.Rows[0]["L_STAFF_ID"].ToString() != null, customerRegisterPanelModelDt.Rows[0]["L_STAFF_ID"].ToString(), "");
                    }
                    if (customerRegisterPanelModelDt.Columns.Contains("L_LOC_CODE"))
                    {
                        quickWinPanelModel.CustomerRegisterPanelModel.L_LOC_CODE = IIf(customerRegisterPanelModelDt.Rows[0]["L_LOC_CODE"].ToString() != null, customerRegisterPanelModelDt.Rows[0]["L_LOC_CODE"].ToString(), "");
                    }
                    if (customerRegisterPanelModelDt.Columns.Contains("L_FOR_CS_TEAM"))
                    {
                        quickWinPanelModel.CustomerRegisterPanelModel.L_FOR_CS_TEAM = IIf(customerRegisterPanelModelDt.Rows[0]["L_FOR_CS_TEAM"].ToString() != null, customerRegisterPanelModelDt.Rows[0]["L_FOR_CS_TEAM"].ToString(), "");
                    }
                    if (customerRegisterPanelModelDt.Columns.Contains("CPE_Serial"))
                    {
                        quickWinPanelModel.CustomerRegisterPanelModel.CPE_Serial = IIf(customerRegisterPanelModelDt.Rows[0]["CPE_Serial"].ToString() != null, customerRegisterPanelModelDt.Rows[0]["CPE_Serial"].ToString(), "");
                    }
                    if (customerRegisterPanelModelDt.Columns.Contains("PlayBox_Serial"))
                    {
                        quickWinPanelModel.CustomerRegisterPanelModel.PlayBox_Serial = IIf(customerRegisterPanelModelDt.Rows[0]["PlayBox_Serial"].ToString() != null, customerRegisterPanelModelDt.Rows[0]["PlayBox_Serial"].ToString(), "");
                    }
                    if (customerRegisterPanelModelDt.Columns.Contains("L_VOUCHER_PIN"))
                    {
                        quickWinPanelModel.CustomerRegisterPanelModel.L_VOUCHER_PIN = IIf(customerRegisterPanelModelDt.Rows[0]["L_VOUCHER_PIN"].ToString() != null, customerRegisterPanelModelDt.Rows[0]["L_VOUCHER_PIN"].ToString(), "");
                    }
                    if (customerRegisterPanelModelDt.Columns.Contains("L_EVENT_CODE"))
                    {
                        quickWinPanelModel.CustomerRegisterPanelModel.L_EVENT_CODE = IIf(customerRegisterPanelModelDt.Rows[0]["L_EVENT_CODE"].ToString() != null, customerRegisterPanelModelDt.Rows[0]["L_EVENT_CODE"].ToString(), "");
                    }
                    if (customerRegisterPanelModelDt.Columns.Contains("RentalFlag"))
                    {
                        quickWinPanelModel.CustomerRegisterPanelModel.RentalFlag = IIf(customerRegisterPanelModelDt.Rows[0]["RentalFlag"].ToString() != null, customerRegisterPanelModelDt.Rows[0]["RentalFlag"].ToString(), "");
                    }

                    if (customerRegisterPanelModelDt.Columns.Contains("outType"))
                    {
                        quickWinPanelModel.CustomerRegisterPanelModel.outType = IIf(customerRegisterPanelModelDt.Rows[0]["outType"].ToString() != null, customerRegisterPanelModelDt.Rows[0]["outType"].ToString(), "");
                    }
                    if (customerRegisterPanelModelDt.Columns.Contains("outSubType"))
                    {
                        quickWinPanelModel.CustomerRegisterPanelModel.outSubType = IIf(customerRegisterPanelModelDt.Rows[0]["outSubType"].ToString() != null, customerRegisterPanelModelDt.Rows[0]["outSubType"].ToString(), "");
                    }
                    if (customerRegisterPanelModelDt.Columns.Contains("AccountCategory"))
                    {
                        quickWinPanelModel.CustomerRegisterPanelModel.AccountCategory = IIf(customerRegisterPanelModelDt.Rows[0]["AccountCategory"].ToString() != null, customerRegisterPanelModelDt.Rows[0]["AccountCategory"].ToString(), "");
                    }
                    if (customerRegisterPanelModelDt.Columns.Contains("JOB_ORDER_TYPE"))
                    {
                        quickWinPanelModel.CustomerRegisterPanelModel.JOB_ORDER_TYPE = IIf(customerRegisterPanelModelDt.Rows[0]["JOB_ORDER_TYPE"].ToString() != null, customerRegisterPanelModelDt.Rows[0]["JOB_ORDER_TYPE"].ToString(), "");
                    }
                    if (customerRegisterPanelModelDt.Columns.Contains("ASSIGN_RULE"))
                    {
                        quickWinPanelModel.CustomerRegisterPanelModel.ASSIGN_RULE = IIf(customerRegisterPanelModelDt.Rows[0]["ASSIGN_RULE"].ToString() != null, customerRegisterPanelModelDt.Rows[0]["ASSIGN_RULE"].ToString(), "");
                    }
                    if (customerRegisterPanelModelDt.Columns.Contains("EStatmentFlag"))
                    {
                        quickWinPanelModel.CustomerRegisterPanelModel.EStatmentFlag = IIf(customerRegisterPanelModelDt.Rows[0]["EStatmentFlag"].ToString() != null, customerRegisterPanelModelDt.Rows[0]["EStatmentFlag"].ToString(), "");
                    }
                    if (customerRegisterPanelModelDt.Columns.Contains("ReceiveEmailFlag"))
                    {
                        quickWinPanelModel.CustomerRegisterPanelModel.ReceiveEmailFlag = IIf(customerRegisterPanelModelDt.Rows[0]["ReceiveEmailFlag"].ToString() != null, customerRegisterPanelModelDt.Rows[0]["ReceiveEmailFlag"].ToString(), "");
                    }

                }

                // Address customer profile

                if (addressPanelModelSetupDt.Rows.Count > 0)
                {
                    quickWinPanelModel.CustomerRegisterPanelModel.AddressPanelModelSetup.L_HOME_NUMBER_2 = "";
                    quickWinPanelModel.CustomerRegisterPanelModel.AddressPanelModelSetup.L_MOO = "";
                    quickWinPanelModel.CustomerRegisterPanelModel.AddressPanelModelSetup.L_MOOBAN = "";
                    quickWinPanelModel.CustomerRegisterPanelModel.AddressPanelModelSetup.L_BUILD_NAME = "";
                    quickWinPanelModel.CustomerRegisterPanelModel.AddressPanelModelSetup.L_FLOOR = "";
                    quickWinPanelModel.CustomerRegisterPanelModel.AddressPanelModelSetup.L_ROOM = "";
                    quickWinPanelModel.CustomerRegisterPanelModel.AddressPanelModelSetup.L_SOI = "";
                    quickWinPanelModel.CustomerRegisterPanelModel.AddressPanelModelSetup.L_ROAD = "";
                    quickWinPanelModel.CustomerRegisterPanelModel.AddressPanelModelSetup.L_TUMBOL = "";
                    quickWinPanelModel.CustomerRegisterPanelModel.AddressPanelModelSetup.L_AMPHUR = "";
                    quickWinPanelModel.CustomerRegisterPanelModel.AddressPanelModelSetup.L_PROVINCE = "";
                    quickWinPanelModel.CustomerRegisterPanelModel.AddressPanelModelSetup.L_ZIPCODE = "";

                    quickWinPanelModel.CustomerRegisterPanelModel.AddressPanelModelSetup.L_HOME_NUMBER_2 = "";

                    if (addressPanelModelSetupDt.Columns.Contains("L_HOME_NUMBER_2"))
                    {
                        quickWinPanelModel.CustomerRegisterPanelModel.AddressPanelModelSetup.L_HOME_NUMBER_2 = IIf(addressPanelModelSetupDt.Rows[0]["L_HOME_NUMBER_2"].ToString() != null, addressPanelModelSetupDt.Rows[0]["L_HOME_NUMBER_2"].ToString(), "");
                    }
                    if (addressPanelModelSetupDt.Columns.Contains("L_MOO"))
                    {
                        quickWinPanelModel.CustomerRegisterPanelModel.AddressPanelModelSetup.L_MOO = IIf(addressPanelModelSetupDt.Rows[0]["L_MOO"].ToString() != null, addressPanelModelSetupDt.Rows[0]["L_MOO"].ToString(), "");
                    }
                    if (addressPanelModelSetupDt.Columns.Contains("L_MOOBAN"))
                    {
                        quickWinPanelModel.CustomerRegisterPanelModel.AddressPanelModelSetup.L_MOOBAN = IIf(addressPanelModelSetupDt.Rows[0]["L_MOOBAN"].ToString() != null, addressPanelModelSetupDt.Rows[0]["L_MOOBAN"].ToString(), "");
                    }
                    if (addressPanelModelSetupDt.Columns.Contains("L_BUILD_NAME"))
                    {
                        quickWinPanelModel.CustomerRegisterPanelModel.AddressPanelModelSetup.L_BUILD_NAME = IIf(addressPanelModelSetupDt.Rows[0]["L_BUILD_NAME"].ToString() != null, addressPanelModelSetupDt.Rows[0]["L_BUILD_NAME"].ToString(), "");
                    }
                    if (addressPanelModelSetupDt.Columns.Contains("L_FLOOR"))
                    {
                        quickWinPanelModel.CustomerRegisterPanelModel.AddressPanelModelSetup.L_FLOOR = IIf(addressPanelModelSetupDt.Rows[0]["L_FLOOR"].ToString() != null, addressPanelModelSetupDt.Rows[0]["L_FLOOR"].ToString(), "");
                    }
                    if (addressPanelModelSetupDt.Columns.Contains("L_ROOM"))
                    {
                        quickWinPanelModel.CustomerRegisterPanelModel.AddressPanelModelSetup.L_ROOM = IIf(addressPanelModelSetupDt.Rows[0]["L_ROOM"].ToString() != null, addressPanelModelSetupDt.Rows[0]["L_ROOM"].ToString(), "");
                    }
                    if (addressPanelModelSetupDt.Columns.Contains("L_ROAD"))
                    {
                        quickWinPanelModel.CustomerRegisterPanelModel.AddressPanelModelSetup.L_ROAD = IIf(addressPanelModelSetupDt.Rows[0]["L_ROAD"].ToString() != null, addressPanelModelSetupDt.Rows[0]["L_ROAD"].ToString(), "");
                    }
                    if (addressPanelModelSetupDt.Columns.Contains("L_TUMBOL"))
                    {
                        quickWinPanelModel.CustomerRegisterPanelModel.AddressPanelModelSetup.L_TUMBOL = IIf(addressPanelModelSetupDt.Rows[0]["L_TUMBOL"].ToString() != null, addressPanelModelSetupDt.Rows[0]["L_TUMBOL"].ToString(), "");
                    }
                    if (addressPanelModelSetupDt.Columns.Contains("L_AMPHUR"))
                    {
                        quickWinPanelModel.CustomerRegisterPanelModel.AddressPanelModelSetup.L_AMPHUR = IIf(addressPanelModelSetupDt.Rows[0]["L_AMPHUR"].ToString() != null, addressPanelModelSetupDt.Rows[0]["L_AMPHUR"].ToString(), "");
                    }
                    if (addressPanelModelSetupDt.Columns.Contains("L_PROVINCE"))
                    {
                        quickWinPanelModel.CustomerRegisterPanelModel.AddressPanelModelSetup.L_PROVINCE = IIf(addressPanelModelSetupDt.Rows[0]["L_PROVINCE"].ToString() != null, addressPanelModelSetupDt.Rows[0]["L_PROVINCE"].ToString(), "");
                    }
                    if (addressPanelModelSetupDt.Columns.Contains("L_ZIPCODE"))
                    {
                        quickWinPanelModel.CustomerRegisterPanelModel.AddressPanelModelSetup.L_ZIPCODE = IIf(addressPanelModelSetupDt.Rows[0]["L_ZIPCODE"].ToString() != null, addressPanelModelSetupDt.Rows[0]["L_ZIPCODE"].ToString(), "");
                    }
                    if (addressPanelModelSetupDt.Columns.Contains("L_SOI"))
                    {
                        quickWinPanelModel.CustomerRegisterPanelModel.AddressPanelModelSetup.L_SOI = IIf(addressPanelModelSetupDt.Rows[0]["L_SOI"].ToString() != null, addressPanelModelSetupDt.Rows[0]["L_SOI"].ToString(), "");
                    }
                }

                // Address customer Send Doc

                if (addressPanelModelSendDocDt.Rows.Count > 0)
                {
                    quickWinPanelModel.CustomerRegisterPanelModel.AddressPanelModelSendDoc.L_HOME_NUMBER_2 = "";
                    quickWinPanelModel.CustomerRegisterPanelModel.AddressPanelModelSendDoc.L_MOO = "";
                    quickWinPanelModel.CustomerRegisterPanelModel.AddressPanelModelSendDoc.L_MOOBAN = "";
                    quickWinPanelModel.CustomerRegisterPanelModel.AddressPanelModelSendDoc.L_BUILD_NAME = "";
                    quickWinPanelModel.CustomerRegisterPanelModel.AddressPanelModelSendDoc.L_FLOOR = "";
                    quickWinPanelModel.CustomerRegisterPanelModel.AddressPanelModelSendDoc.L_ROOM = "";
                    quickWinPanelModel.CustomerRegisterPanelModel.AddressPanelModelSendDoc.L_SOI = "";
                    quickWinPanelModel.CustomerRegisterPanelModel.AddressPanelModelSendDoc.L_ROAD = "";
                    quickWinPanelModel.CustomerRegisterPanelModel.AddressPanelModelSendDoc.L_TUMBOL = "";
                    quickWinPanelModel.CustomerRegisterPanelModel.AddressPanelModelSendDoc.L_AMPHUR = "";
                    quickWinPanelModel.CustomerRegisterPanelModel.AddressPanelModelSendDoc.L_PROVINCE = "";
                    quickWinPanelModel.CustomerRegisterPanelModel.AddressPanelModelSendDoc.L_ZIPCODE = "";

                    if (addressPanelModelSendDocDt.Columns.Contains("L_HOME_NUMBER_2"))
                    {
                        quickWinPanelModel.CustomerRegisterPanelModel.AddressPanelModelSendDoc.L_HOME_NUMBER_2 = IIf(addressPanelModelSendDocDt.Rows[0]["L_HOME_NUMBER_2"].ToString() != null, addressPanelModelSendDocDt.Rows[0]["L_HOME_NUMBER_2"].ToString(), "");
                    }
                    if (addressPanelModelSendDocDt.Columns.Contains("L_MOO"))
                    {
                        quickWinPanelModel.CustomerRegisterPanelModel.AddressPanelModelSendDoc.L_MOO = IIf(addressPanelModelSendDocDt.Rows[0]["L_MOO"].ToString() != null, addressPanelModelSendDocDt.Rows[0]["L_MOO"].ToString(), "");
                    }
                    if (addressPanelModelSendDocDt.Columns.Contains("L_MOOBAN"))
                    {
                        quickWinPanelModel.CustomerRegisterPanelModel.AddressPanelModelSendDoc.L_MOOBAN = IIf(addressPanelModelSendDocDt.Rows[0]["L_MOOBAN"].ToString() != null, addressPanelModelSendDocDt.Rows[0]["L_MOOBAN"].ToString(), "");
                    }
                    if (addressPanelModelSendDocDt.Columns.Contains("L_BUILD_NAME"))
                    {
                        quickWinPanelModel.CustomerRegisterPanelModel.AddressPanelModelSendDoc.L_BUILD_NAME = IIf(addressPanelModelSendDocDt.Rows[0]["L_BUILD_NAME"].ToString() != null, addressPanelModelSendDocDt.Rows[0]["L_BUILD_NAME"].ToString(), "");
                    }
                    if (addressPanelModelSendDocDt.Columns.Contains("L_FLOOR"))
                    {
                        quickWinPanelModel.CustomerRegisterPanelModel.AddressPanelModelSendDoc.L_FLOOR = IIf(addressPanelModelSendDocDt.Rows[0]["L_FLOOR"].ToString() != null, addressPanelModelSendDocDt.Rows[0]["L_FLOOR"].ToString(), "");
                    }
                    if (addressPanelModelSendDocDt.Columns.Contains("L_ROOM"))
                    {
                        quickWinPanelModel.CustomerRegisterPanelModel.AddressPanelModelSendDoc.L_ROOM = IIf(addressPanelModelSendDocDt.Rows[0]["L_ROOM"].ToString() != null, addressPanelModelSendDocDt.Rows[0]["L_ROOM"].ToString(), "");
                    }
                    if (addressPanelModelSendDocDt.Columns.Contains("L_SOI"))
                    {
                        quickWinPanelModel.CustomerRegisterPanelModel.AddressPanelModelSendDoc.L_SOI = IIf(addressPanelModelSendDocDt.Rows[0]["L_SOI"].ToString() != null, addressPanelModelSendDocDt.Rows[0]["L_SOI"].ToString(), "");
                    }
                    if (addressPanelModelSendDocDt.Columns.Contains("L_ROAD"))
                    {
                        quickWinPanelModel.CustomerRegisterPanelModel.AddressPanelModelSendDoc.L_ROAD = IIf(addressPanelModelSendDocDt.Rows[0]["L_ROAD"].ToString() != null, addressPanelModelSendDocDt.Rows[0]["L_ROAD"].ToString(), "");
                    }
                    if (addressPanelModelSendDocDt.Columns.Contains("L_TUMBOL"))
                    {
                        quickWinPanelModel.CustomerRegisterPanelModel.AddressPanelModelSendDoc.L_TUMBOL = IIf(addressPanelModelSendDocDt.Rows[0]["L_TUMBOL"].ToString() != null, addressPanelModelSendDocDt.Rows[0]["L_TUMBOL"].ToString(), "");
                    }
                    if (addressPanelModelSendDocDt.Columns.Contains("L_AMPHUR"))
                    {
                        quickWinPanelModel.CustomerRegisterPanelModel.AddressPanelModelSendDoc.L_AMPHUR = IIf(addressPanelModelSendDocDt.Rows[0]["L_AMPHUR"].ToString() != null, addressPanelModelSendDocDt.Rows[0]["L_AMPHUR"].ToString(), "");
                    }
                    if (addressPanelModelSendDocDt.Columns.Contains("L_PROVINCE"))
                    {
                        quickWinPanelModel.CustomerRegisterPanelModel.AddressPanelModelSendDoc.L_PROVINCE = IIf(addressPanelModelSendDocDt.Rows[0]["L_PROVINCE"].ToString() != null, addressPanelModelSendDocDt.Rows[0]["L_PROVINCE"].ToString(), "");
                    }
                    if (addressPanelModelSendDocDt.Columns.Contains("L_ZIPCODE"))
                    {
                        quickWinPanelModel.CustomerRegisterPanelModel.AddressPanelModelSendDoc.L_ZIPCODE = IIf(addressPanelModelSendDocDt.Rows[0]["L_ZIPCODE"].ToString() != null, addressPanelModelSendDocDt.Rows[0]["L_ZIPCODE"].ToString(), "");
                    }
                }

                if (DisplayPackagePanelModelDt.Rows.Count > 0)
                {
                    quickWinPanelModel.DisplayPackagePanelModel.WIFIAccessPoint = "";
                    if (DisplayPackagePanelModelDt.Columns.Contains("WIFIAccessPoint"))
                    {
                        quickWinPanelModel.DisplayPackagePanelModel.WIFIAccessPoint = IIf(DisplayPackagePanelModelDt.Rows[0]["WIFIAccessPoint"].ToString() != null, DisplayPackagePanelModelDt.Rows[0]["WIFIAccessPoint"].ToString(), "");
                    }
                }

                if (coverageAreaResultModelDt.Rows.Count > 0)
                {
                    quickWinPanelModel.CoverageAreaResultModel.ADDRRESS_TYPE = "";
                    quickWinPanelModel.CoverageAreaResultModel.BUILDING_NO = "";
                    quickWinPanelModel.CoverageAreaResultModel.IS_PARTNER = "";
                    quickWinPanelModel.CoverageAreaResultModel.PARTNER_NAME = "";
                    quickWinPanelModel.CoverageAreaResultModel.ACCESS_MODE_LIST = "";
                    quickWinPanelModel.CoverageAreaResultModel.TRANSACTION_ID = "";

                    if (coverageAreaResultModelDt.Columns.Contains("ADDRRESS_TYPE"))
                    {
                        quickWinPanelModel.CoverageAreaResultModel.ADDRRESS_TYPE = IIf(coverageAreaResultModelDt.Rows[0]["ADDRRESS_TYPE"].ToString() != null, coverageAreaResultModelDt.Rows[0]["ADDRRESS_TYPE"].ToString(), "");
                    }
                    if (coverageAreaResultModelDt.Columns.Contains("BUILDING_NO"))
                    {
                        quickWinPanelModel.CoverageAreaResultModel.BUILDING_NO = IIf(coverageAreaResultModelDt.Rows[0]["BUILDING_NO"].ToString() != null, coverageAreaResultModelDt.Rows[0]["BUILDING_NO"].ToString(), "");
                    }
                    if (coverageAreaResultModelDt.Columns.Contains("IS_PARTNER"))
                    {
                        quickWinPanelModel.CoverageAreaResultModel.IS_PARTNER = IIf(coverageAreaResultModelDt.Rows[0]["IS_PARTNER"].ToString() != null, coverageAreaResultModelDt.Rows[0]["IS_PARTNER"].ToString(), "");
                    }
                    if (coverageAreaResultModelDt.Columns.Contains("PARTNER_NAME"))
                    {
                        quickWinPanelModel.CoverageAreaResultModel.PARTNER_NAME = IIf(coverageAreaResultModelDt.Rows[0]["PARTNER_NAME"].ToString() != null, coverageAreaResultModelDt.Rows[0]["PARTNER_NAME"].ToString(), "");
                    }
                    if (coverageAreaResultModelDt.Columns.Contains("ACCESS_MODE_LIST"))
                    {
                        quickWinPanelModel.CoverageAreaResultModel.ACCESS_MODE_LIST = IIf(coverageAreaResultModelDt.Rows[0]["ACCESS_MODE_LIST"].ToString() != null, coverageAreaResultModelDt.Rows[0]["ACCESS_MODE_LIST"].ToString(), "");
                    }
                    if (coverageAreaResultModelDt.Columns.Contains("TRANSACTION_ID"))
                    {
                        quickWinPanelModel.CoverageAreaResultModel.TRANSACTION_ID = IIf(coverageAreaResultModelDt.Rows[0]["TRANSACTION_ID"].ToString() != null, coverageAreaResultModelDt.Rows[0]["TRANSACTION_ID"].ToString(), "");
                    }
                }

                if (timeslotDt.Rows.Count > 0)
                {
                    quickWinPanelModel.CustomerRegisterPanelModel.FBSSTimeSlot.AppointmentDate = null;
                    quickWinPanelModel.CustomerRegisterPanelModel.FBSSTimeSlot.TimeSlot = null;
                    quickWinPanelModel.CustomerRegisterPanelModel.FBSSTimeSlot.InstallationCapacity = null;

                    if (timeslotDt.Columns.Contains("AppointmentDate"))
                    {
                        System.Globalization.CultureInfo cultureinfo = new System.Globalization.CultureInfo("en-US");

                        if (timeslotDt.Rows[0]["AppointmentDate"].ToString() != "")
                        {
                            DateTime? timeslot = DateTime.Parse(IIf(timeslotDt.Rows[0]["AppointmentDate"].ToString() != null, timeslotDt.Rows[0]["AppointmentDate"].ToString(), ""), cultureinfo);
                            // DateTime? timeslot = new DateTime();
                            // timeslot = Convert.ToDateTime(IIf(timeslotDt.Rows[0]["AppointmentDate"].ToString() != null, timeslotDt.Rows[0]["AppointmentDate"].ToString(), ""));

                            quickWinPanelModel.CustomerRegisterPanelModel.FBSSTimeSlot.AppointmentDate = timeslot;
                        }
                    }
                    if (timeslotDt.Columns.Contains("TimeSlot"))
                    {
                        quickWinPanelModel.CustomerRegisterPanelModel.FBSSTimeSlot.TimeSlot = IIf(timeslotDt.Rows[0]["TimeSlot"].ToString() != null, timeslotDt.Rows[0]["TimeSlot"].ToString(), "");
                    }
                    if (timeslotDt.Columns.Contains("InstallationCapacity"))
                    {
                        quickWinPanelModel.CustomerRegisterPanelModel.FBSSTimeSlot.InstallationCapacity = IIf(timeslotDt.Rows[0]["InstallationCapacity"].ToString() != null, timeslotDt.Rows[0]["InstallationCapacity"].ToString(), "");
                    }
                }
            }
            catch
            {
                return null;
            }
            finally
            {
                if (reader != null) reader.Close();
            }

            return Json(quickWinPanelModel, JsonRequestBehavior.AllowGet);

        }

        public string get_fbb_cfg_lov(string _lov_type, string input_where)
        {
            try
            {
                var _result = base.LovData.Where(p => p.Type == _lov_type && (p.LovValue1 == input_where || p.LovValue2 == input_where));
                string _result_string = _result.FirstOrDefault().Name.ToSafeString();
                return _result_string;
            }
            catch (Exception ex)
            {
                return input_where;
            }
        }

        public string get_fbb_cfg_lov_gender(string _lov_type, string input_where)
        {
            try
            {
                var _result = base.LovData.Where(p => p.Type == _lov_type && (p.LovValue1 == input_where || p.LovValue2 == input_where));
                string _result_string = _result.FirstOrDefault().LovValue1.ToSafeString();
                return _result_string;
            }
            catch (Exception ex)
            {
                return input_where;
            }
        }

        public JsonResult GetCustomerSubCat(string idCardName)
        {
            var a = base.LovData.Where(p => p.Name == idCardName && p.Type == WebConstants.LovConfigName.CardType);
            LovValueModel model = new LovValueModel();
            if (a.Any())
            {
                model.LovValue4 = a.FirstOrDefault().LovValue4;
                model.LovValue5 = a.FirstOrDefault().LovValue5;
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        //[ActionName("IndexWithModel")]
        public ActionResult Index()
        {
            QuickWinPanelModel model = new QuickWinPanelModel();
            return View(model);
            // return RedirectToAction("IndexWithModel", new { model = "" });
        }


        public JsonResult SelectStatus()
        {

            var query = new GetStatusLogQuery
            {
                status = ""
            };

            var data = _queryProcessor.Execute(query);


            data.Insert(0, new StatusLogDropdown { TEXT = "เลือกทั้งหมด", VALUE = "" });

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [OutputCache(Location = OutputCacheLocation.Client, Duration = 10)]
        public JsonResult GetCustomerTitle(string customerCardType, string L_TITLE_CODE)
        {
            //var langFlg = (SiteSession.CurrentUICulture.IsThaiCulture() ?
            //    WebConstants.LovConfigName.TitleCodeTh : WebConstants.LovConfigName.TitleCodeEn);
            var _CurrentCulture = 0;
            if (L_TITLE_CODE == "004" || L_TITLE_CODE == "005" || L_TITLE_CODE == "129")
                _CurrentCulture = 2;
            else _CurrentCulture = 1;

            var query = new GetCustomerTitleQuery
            {
                CurrentCulture = _CurrentCulture, // 0 = all , 1 = thai , 2 = eng
                CustomerType = customerCardType,
            };

            var dropDown = _queryProcessor.Execute(query)
                .Select(t => new DropdownModel
                {
                    Text = t.Title,
                    Value = t.TitleCode,
                    DefaultValue = t.DefaultValue,
                }).ToList();
            return Json(dropDown, JsonRequestBehavior.AllowGet);
        }
        [OutputCache(Location = OutputCacheLocation.None)]
        public JsonResult GetCustomerCardType(string customerType)
        {
            if (customerType == "All")
            {
                var dropDown = base.LovData
                    .Where(l => l.Type.Equals("ID_CARD_TYPE"))
                    .Select(l => new DropdownModel
                    {
                        Text = l.LovValue1,
                        Value = l.Name,
                        DefaultValue = l.DefaultValue,
                    }).ToList();
                return Json(dropDown, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var dropDown = base.LovData
                    .Where(l => l.Type.Equals("ID_CARD_TYPE") && l.LovValue3.Equals(customerType))
                    .Select(l => new DropdownModel
                    {
                        Text = l.LovValue1,
                        Value = l.Name,
                        DefaultValue = l.DefaultValue,
                    }).ToList();
                return Json(dropDown, JsonRequestBehavior.AllowGet);
            }

        }

        [OutputCache(Location = OutputCacheLocation.Client, Duration = 10)]
        public JsonResult GetCustomerGender()
        {
            //var langFlg = (SiteSession.CurrentUICulture.IsThaiCulture() ?
            //   WebConstants.LovConfigName.TitleCodeTh : WebConstants.LovConfigName.TitleCodeEn);

            var dropDown = base.LovData
                .Where(l => l.Type.Equals("GENDER"))
                .Select(l => new DropdownModel
                {
                    Text = l.LovValue1,
                    Value = l.Name,
                    DefaultValue = l.DefaultValue,
                }).ToList();

            return Json(dropDown, JsonRequestBehavior.AllowGet);
        }

        [OutputCache(Location = OutputCacheLocation.Client, Duration = 10)]
        public JsonResult GetContactTime()
        {
            //var langFlg = (SiteSession.CurrentUICulture.IsThaiCulture() ?
            //                WebConstants.LovConfigName.TitleCodeTh : WebConstants.LovConfigName.TitleCodeEn);

            var dropDown = base.LovData
                .Where(l => l.Type.Equals("CONTACT_TIME"))
                .OrderBy(l => l.OrderBy)
                .Select(l => new DropdownModel
                {
                    Text = l.LovValue1,
                    Value = l.Name.Trim(),
                    DefaultValue = l.DefaultValue,
                })
                .ToList();

            return Json(dropDown, JsonRequestBehavior.AllowGet);
        }

        [OutputCache(Location = OutputCacheLocation.Client, Duration = 10)]
        public JsonResult GetAddress(string province, string amphur, string tumbon)
        {
            Dictionary<string, List<DropdownModel>> dict = new Dictionary<string, List<DropdownModel>>();
            try
            {
                var provType = new List<DropdownModel>();
                provType = base.ZipCodeData(SiteSession.CurrentUICulture)
                    .GroupBy(z => z.Province)
                    .Select(z =>
                    {
                        var item = z.First();
                        return new DropdownModel { Text = item.Province, Value = item.Province };
                    })
                    .OrderBy(o => o.Text)
                    .ToList();

                var amphType = new List<DropdownModel>();
                amphType = base.ZipCodeData(SiteSession.CurrentUICulture)
                    .Where(z => (string.IsNullOrEmpty(z.Province) || z.Province.Equals(province)))
                    .GroupBy(z => z.Amphur)
                    .Select(z =>
                    {
                        var item = z.First();
                        return new DropdownModel { Text = item.Amphur, Value = item.Amphur };
                    })
                    .OrderBy(o => o.Text)
                    .ToList();

                var tumbType = new List<DropdownModel>();
                tumbType = base.ZipCodeData(SiteSession.CurrentUICulture)
                    .Where(z => (string.IsNullOrEmpty(z.Province) || z.Province.Equals(province))
                                    && (string.IsNullOrEmpty(z.Amphur) || z.Amphur.Equals(amphur)))
                    .GroupBy(z => z.Tumbon)
                    .Select(z =>
                    {
                        var item = z.First();
                        return new DropdownModel { Text = item.Tumbon, Value = item.Tumbon };
                    })
                    .OrderBy(o => o.Text)
                    .ToList();

                var amphurToFilter = "";
                int index1 = amphur.IndexOf('(');
                if (index1 > 0)
                {
                    int index2 = amphur.IndexOf(')');
                    amphurToFilter = amphur.Remove(index1, index2 - index1 + 1);
                }
                else
                {
                    amphurToFilter = amphur;
                }

                var zipCodeList = new List<DropdownModel>();
                zipCodeList = base.ZipCodeData(SiteSession.CurrentUICulture)
                   .Where(z => (!string.IsNullOrEmpty(z.Province) && z.Province.Equals(province))
                       && (!string.IsNullOrEmpty(z.Amphur) && z.Amphur.Contains(amphurToFilter))
                       && (!string.IsNullOrEmpty(z.Tumbon) && z.Tumbon.Equals(tumbon)))
                   .Select(z => new DropdownModel { Text = z.ZipCode, Value = z.ZipCodeId, })
                   .ToList();

                dict.Add("province", provType);
                dict.Add("amphur", amphType);
                dict.Add("tumbon", tumbType);
                dict.Add("zipcode", zipCodeList);
            }
            catch (Exception)
            { }

            return Json(dict, JsonRequestBehavior.AllowGet);
        }

        public List<LovScreenValueModel> GetScreenConfig(string pageCode)
        {
            try
            {
                List<LovValueModel> config = null;
                if (pageCode == null)
                {
                    config = base.LovData.Where(l => l.LovValue5 == null && l.Type == "SCREEN" || l.Type == "VAS_CODE_CONFIG").ToList();
                }
                else if (pageCode == "ALLPAGE")
                {
                    config = base.LovData.Where(l => l.Type == "SCREEN").ToList();
                }
                else
                {
                    config = base.LovData.Where(l =>
                        (!string.IsNullOrEmpty(l.Type) && l.Type == "SCREEN" || l.Type == "VAS_CODE_CONFIG")
                            && (!string.IsNullOrEmpty(l.LovValue5) && l.LovValue5.Equals(pageCode))).ToList();
                }

                var screenValue = new List<LovScreenValueModel>();
                SiteSession.CurrentUICulture = 1;

                if (SiteSession.CurrentUICulture == 1)
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
                        DefaultValue = l.DefaultValue,
                        Blob = l.Image_blob != null ? Convert.ToBase64String(l.Image_blob, 0, l.Image_blob.Length) : "",
                        DisplayValueJing = l.Text
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
                        DefaultValue = l.DefaultValue,
                        Blob = l.Image_blob != null ? Convert.ToBase64String(l.Image_blob, 0, l.Image_blob.Length) : "",
                        DisplayValueJing = l.Text
                    }).ToList();
                }

                return screenValue;
            }
            catch (Exception ex)
            {
                //  Logger.Info(ex.GetErrorMessage());
                return new List<LovScreenValueModel>();
            }
        }

        [OutputCache(Location = OutputCacheLocation.Client, Duration = 10)]
        public JsonResult GetP_Region(string rowid)
        {
            SiteSession.CurrentUICulture = 1;
            var query = new SelectSubRegionQuery()
            {
                rowid = rowid,
                currentculture = SiteSession.CurrentUICulture
            };

            var result = _queryProcessor.Execute(query);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private void SetLovValueToViewBag(QuickWinPanelModel model)
        {
            if (SiteSession.CurrentUICulture.IsThaiCulture())
            { ViewBag.LanguagePage = "1"; model.SummaryPanelModel.PDFPackageModel.PDF_L_UNIT = "1"; }
            else
            { ViewBag.LanguagePage = "2"; model.SummaryPanelModel.PDFPackageModel.PDF_L_UNIT = "2"; }

            var lovData = base.LovData.Where(l => !string.IsNullOrEmpty(l.Name) && l.Name.Equals("B_CLOSE"));
            if (SiteSession.CurrentUICulture.IsThaiCulture())
            { ViewBag.LCLOSE = lovData.FirstOrDefault().LovValue1; }
            else
            { ViewBag.LCLOSE = lovData.FirstOrDefault().LovValue2; }

            lovData = base.LovData.Where(l => !string.IsNullOrEmpty(l.Name) && l.Name.Equals("B_SAVE"));
            if (SiteSession.CurrentUICulture.IsThaiCulture())
            { ViewBag.LSAVE = lovData.FirstOrDefault().LovValue1; }
            else
            { ViewBag.LSAVE = lovData.FirstOrDefault().LovValue2; }

            if (model.PlugAndPlayFlow == "Y")
            {
                lovData = base.LovData.Where(l => !string.IsNullOrEmpty(l.Name) && l.Name.Equals("L_POPUP_PLUG_AND_PLAY"));
            }
            else
            {
                lovData = base.LovData.Where(l => !string.IsNullOrEmpty(l.Name) && l.Name.Equals("L_POPUP_SAVE"));
            }

            if (SiteSession.CurrentUICulture.IsThaiCulture())
            { ViewBag.LPOPUPSAVE = HttpUtility.HtmlEncode(lovData.FirstOrDefault().LovValue1); }
            else
            { ViewBag.LPOPUPSAVE = HttpUtility.HtmlEncode(lovData.FirstOrDefault().LovValue2); }

            //lovData = base.LovData.Where(l => !string.IsNullOrEmpty(l.Name) && l.Name.Equals("L_USE_ADDR_CARD"));
            //if (SiteSession.CurrentUICulture.IsThaiCulture())
            //{ ViewBag.L_USE_ADDR_CARD = lovData.FirstOrDefault().LovValue1; }
            //else
            //{ ViewBag.L_USE_ADDR_CARD = lovData.FirstOrDefault().LovValue2; }

            //lovData = base.LovData.Where(l => !string.IsNullOrEmpty(l.Name) && l.Name.Equals("B_YES"));
            //if (SiteSession.CurrentUICulture.IsThaiCulture())
            //{ ViewBag.B_YES = lovData.FirstOrDefault().LovValue1; }
            //else
            //{ ViewBag.B_YES = lovData.FirstOrDefault().LovValue2; }

            //lovData = base.LovData.Where(l => !string.IsNullOrEmpty(l.Name) && l.Name.Equals("B_NO"));
            //if (SiteSession.CurrentUICulture.IsThaiCulture())
            //{ ViewBag.B_NO = lovData.FirstOrDefault().LovValue1; }
            //else
            //{ ViewBag.B_NO = lovData.FirstOrDefault().LovValue2; }
        }

        private ConfirmChangePromotionModelLine4 ConfirmChangePromotionModelLine4(string mobileNo, string promotionCd)
        {
            var flag1 = base.LovData.Where(l => l.Type == "FBB_CONSTANT" && l.Name == "CONFIRM_PROMOTION" && l.Text == "evOMServiceConfirmChangePromotion").SingleOrDefault();
            var flag2 = base.LovData.Where(l => l.Type == "FBB_CONSTANT" && l.Name == "CONFIRM_PROMOTION" && l.Text == "evOMCreateOrderChangePromotion").SingleOrDefault();
            var query = new GetConfirmChangePromotionQuery()
            {
                mobileNo = mobileNo,
                promotionCode = promotionCd,
                FlagCallService_evOMServiceConfirmChangePromotion = flag1.LovValue1,
                FlagCallService_evOMCreateOrderChangePromotion = flag2.LovValue1
            };

            var result = _queryProcessor.Execute(query);
            return result;
        }

        private CheckChangePromotionModelLine4 GetCheckChangePromotionLine4(string mobileNo, string promotionType, string promotionCd, string orderChannel)
        {
            var query = new GetCheckChangePromotionQuery()
            {
                mobileNo = mobileNo,
                promotionType = promotionType,
                promotionCd = promotionCd,
                orderChannel = orderChannel
            };

            var result = _queryProcessor.Execute(query);
            return result;
        }

        public JsonResult OnSave(QuickWinPanelModel model)
        {
            //  QuickWinPanelModel model = new QuickWinPanelModel();
            var customerRowID = "";
            var saveOrderResp = new SaveOrderResp();


            if (null == base.CurrentUser)
                return Json("Sesson Time Out", JsonRequestBehavior.AllowGet);
            var LoginUser = base.CurrentUser;

            try
            {
                #region Get IP Address
                string ClientIP = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (string.IsNullOrEmpty(ClientIP))
                {
                    ClientIP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                }

                model.ClientIP = ClientIP;
                #endregion

                //if (model.Register_device == "MOBILE APP")
                //    model = SaveFileImage(files, model);

                //string pdfSignatureBase64 = model.SignaturePDF;
                //model.SignaturePDF = "";

                //string pdfSignatureBase64_2 = model.SignaturePDF2;
                //model.SignaturePDF2 = "";


                #region Line4
                //model.CustomerRegisterPanelModel.L_BIRTHDAY =
                if (model.SummaryPanelModel.VAS_FLAG.ToSafeString() == "4")
                {
                    ViewBag.SaveSuccess = true;
                    SetLovValueToViewBag(model);
                    Session["EndProcessFlag"] = false;
                    foreach (var a in model.SummaryPanelModel.PackageModelList)
                    {
                        var CheckChangePromotionModelLine4 = new CheckChangePromotionModelLine4();
                        CheckChangePromotionModelLine4 = GetCheckChangePromotionLine4(model.CoveragePanelModel.P_MOBILE.ToSafeString(),
                                                                                        "CHECK", a.UPLOAD_SPEED.ToSafeString(), "WEB");

                        if (CheckChangePromotionModelLine4.returnCode != "001" && CheckChangePromotionModelLine4.returnCode != "003" && CheckChangePromotionModelLine4.existFlag == "Y")
                        {
                            var customerRowID1 = RegisterCustomer(model,
                                CheckChangePromotionModelLine4.returnCode.ToSafeString(),
                                "ERROR:" + a.PACKAGE_NAME,
                                "", ClientIP);

                            ViewBag.SWiFi = "fail";
                        }
                        else if (CheckChangePromotionModelLine4.returnCode != "001" && CheckChangePromotionModelLine4.returnCode != "003")
                        {
                            var customerRowID1 = RegisterCustomer(model,
                                CheckChangePromotionModelLine4.returnCode.ToSafeString(),
                                "ERROR:" + a.PACKAGE_NAME,
                                "", ClientIP);

                            ViewBag.SWiFi = "fail";
                        }
                    }
                    var sumcode = "";
                    for (var i = 0; i < model.SummaryPanelModel.PackageModelList.Count; i++)
                    {
                        sumcode = sumcode + model.SummaryPanelModel.PackageModelList[i].UPLOAD_SPEED.ToSafeString();
                        if (i < model.SummaryPanelModel.PackageModelList.Count - 1)
                        {
                            sumcode = sumcode + ",";
                            continue;
                        }
                        else
                        {
                            var ConfirmChangePromotionModel = new ConfirmChangePromotionModelLine4();
                            ConfirmChangePromotionModel = ConfirmChangePromotionModelLine4(model.CoveragePanelModel.P_MOBILE.ToSafeString()
                                                                                          , sumcode);
                            if (ConfirmChangePromotionModel.SuccessFlag != "Y")
                            {
                                var customerRowID3 = RegisterCustomer(model,
                                ConfirmChangePromotionModel.SuccessFlag.ToSafeString(),
                                ConfirmChangePromotionModel.ReturnMessage,
                                ConfirmChangePromotionModel.ReturnCode,
                                ClientIP);
                                var namepac = "";

                                if (ConfirmChangePromotionModel.ReturnMessage.Contains("EB0216")
                                || ConfirmChangePromotionModel.ReturnMessage.Contains("Playbox Old Launcher"))
                                {
                                    ViewBag.NamePackageFail = "old";
                                    ViewBag.SWiFi = "fail";
                                }
                                else
                                {
                                    //if (i == model.SummaryPanelModel.PackageModelList.Count - 1) { namepac = model.SummaryPanelModel.PackageModelList[i].PACKAGE_NAME; }
                                    ViewBag.NamePackageFail = namepac;
                                    ViewBag.SWiFi = "fail";
                                }
                            }
                            else
                            {
                                if (i == model.SummaryPanelModel.PackageModelList.Count - 1)
                                {
                                    var customerRowID2 = RegisterCustomer(model,
                                    ConfirmChangePromotionModel.SuccessFlag.ToSafeString(),
                                    ConfirmChangePromotionModel.ReturnMessage,
                                    ConfirmChangePromotionModel.ReturnCode,
                                    ClientIP);

                                    ViewBag.SWiFi = "success";
                                }
                            }
                        }
                    }
                }
                #endregion Line4


                #region sortPackage
                if (model.ForCoverageResult != true)
                {
                    if (model.SummaryPanelModel.PackageModelList != null && model.SummaryPanelModel.PackageModelList.Count != 0)
                    {
                        if (model.SummaryPanelModel.PackageModelList[0].PACKAGE_CLASS.ToSafeString().Contains("TRIPLE"))
                        {
                            var tempcount = 0;
                            var temp = new List<PackageModel>();
                            do
                            {
                                if (model.SummaryPanelModel.PackageModelList[tempcount].PRODUCT_SUBTYPE.ToSafeString() == "WireBB" ||
                                    model.SummaryPanelModel.PackageModelList[tempcount].PRODUCT_SUBTYPE.ToSafeString() == "FTTx")
                                {
                                    if (model.SummaryPanelModel.PackageModelList[tempcount].PACKAGE_CLASS == "TRIPLE")
                                    {
                                        temp.Add(model.SummaryPanelModel.PackageModelList[tempcount]);
                                        model.SummaryPanelModel.PackageModelList.RemoveAt(tempcount);
                                        tempcount = 0;
                                    }
                                    else if (model.SummaryPanelModel.PackageModelList[tempcount].PACKAGE_CLASS == "Ontop")
                                    {
                                        if (temp.Count < 1)
                                        {
                                            tempcount++;
                                            continue;
                                        }
                                        else
                                        {
                                            temp.Add(model.SummaryPanelModel.PackageModelList[tempcount]);
                                            model.SummaryPanelModel.PackageModelList.RemoveAt(tempcount);
                                            tempcount = 0;
                                        }
                                    }
                                }
                                else if (model.SummaryPanelModel.PackageModelList[tempcount].PRODUCT_SUBTYPE.ToSafeString() == "VOIP")
                                {
                                    if (temp.Count < 2)
                                    {
                                        tempcount++;
                                        continue;
                                    }
                                    else
                                    {
                                        temp.Add(model.SummaryPanelModel.PackageModelList[tempcount]);
                                        model.SummaryPanelModel.PackageModelList.RemoveAt(tempcount);
                                        tempcount = 0;
                                    }
                                }
                                else if (model.SummaryPanelModel.PackageModelList[tempcount].PRODUCT_SUBTYPE.ToSafeString() == "PBOX")/// pbox
                                {
                                    if (temp.Count < 3)
                                    {
                                        tempcount++;
                                        continue;
                                    }
                                    else
                                    {
                                        temp.Add(model.SummaryPanelModel.PackageModelList[tempcount]);
                                        model.SummaryPanelModel.PackageModelList.RemoveAt(tempcount);
                                        tempcount = 0;
                                    }
                                }
                                else
                                {
                                    model.SummaryPanelModel.PackageModelList.RemoveAt(tempcount);
                                    tempcount++;
                                    tempcount = 0;
                                    continue;
                                }
                            } while (model.SummaryPanelModel.PackageModelList.Count != 0);
                            model.SummaryPanelModel.PackageModelList = temp;
                        }
                    }
                }

                #endregion sortPackage

                #region customerRegister
                List<UploadImage> img = new List<UploadImage>();
                img = model.CustomerRegisterPanelModel.ListImageFile;

                #endregion

                model.CustomerRegisterPanelModel.RegisterChannelSaveOrder = "";
                model.CustomerRegisterPanelModel.AutoCreateProspectFlag = "";
                model.CustomerRegisterPanelModel.OrderVerify = "";

                var result_data = GetSaveOrderResp(model);

                #region Save register
                //register customer
                saveOrderResp.RETURN_CODE = result_data.RETURN_CODE;
                saveOrderResp.RETURN_MESSAGE = result_data.RETURN_MESSAGE;
                saveOrderResp.RETURN_IA_NO = result_data.RETURN_IA_NO;

                if (result_data.RETURN_CODE != -1) // -1 = Error
                {
                    customerRowID = RegisterCustomer(model,
                        saveOrderResp.RETURN_CODE.ToString(),
                        saveOrderResp.RETURN_MESSAGE,
                        saveOrderResp.RETURN_IA_NO,
                        ClientIP);
                    model.CustomerRegisterPanelModel.OrderNo = saveOrderResp.RETURN_IA_NO;
                }

                #endregion Save register


            }
            catch (Exception ex)
            {
                Json(new { Status = "Error" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { Status = "Success" }, JsonRequestBehavior.AllowGet);
        }

        private SaveOrderResp GetSaveOrderResp(QuickWinPanelModel model)
        {
            Session["FullUrl"] = this.Url.Action("OnSave", "ResendOrder", null, this.Request.Url.Scheme);
            string FullUrl = "";
            if (Session["FullUrl"] != null)
            {
                FullUrl = Session["FullUrl"].ToSafeString();
            }
            DateTime birthDate;
            var date = DateTime.TryParseExact(model.CustomerRegisterPanelModel.L_BIRTHDAY.ToSafeString(), "dd/MM/yyyy",
                                                CultureInfo.InvariantCulture, DateTimeStyles.None, out birthDate);

            if (Convert.ToInt32(TempData["CurrentCulture"]) == 1) // 1 = thai, 2 = english ,
            {
                if (birthDate > DateTime.MinValue.AddYears(543))
                    model.CustomerRegisterPanelModel.L_BIRTHDAY = birthDate.AddYears(543).ToDateDisplayText();
            }
            var query = new GetSaveOrderRespQuery
            {
                //CurrentCulture = SiteSession.CurrentUICulture,
                CurrentCulture = Convert.ToInt32(TempData["CurrentCulture"]),
                QuickWinPanelModel = model,
                FullUrl = FullUrl
            };
            var data = _queryProcessor.Execute(query);
            return data;
        }

        private string RegisterCustomer(QuickWinPanelModel model, string interfaceCode,
         string interfaceDesc, string interfaceOrder, string ClientIP)
        {
            var coverageResultId = "";
            if (null != model.CoveragePanelModel)
                // test Fix
                //model.CoveragePanelModel.RESULT_ID = "1";
                coverageResultId = model.CoveragePanelModel.RESULT_ID;

            var command = new CustRegisterCommand
            {
                QuickWinPanelModel = model,
                CurrentCulture = SiteSession.CurrentUICulture,
                InterfaceCode = interfaceCode,
                InterfaceDesc = interfaceDesc,
                InterfaceOrder = interfaceOrder,
                CoverageResultId = decimal.Parse(coverageResultId),
                ClientIP = ClientIP
            };

            _custRegCommand.Handle(command);

            return command.CustomerId;
        }


    }
}

