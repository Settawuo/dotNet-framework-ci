using Kendo.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Commands.WebServices;
using WBBContract.Queries.Commons.Account;
using WBBContract.Queries.Commons.Masters;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBContract.Queries.WebServices;
using WBBEntity.Extensions;
using WBBEntity.PanelModels;
using WBBEntity.PanelModels.Account;
using WBBEntity.PanelModels.WebServiceModels;
using WBBWeb.Extension;
using WBBWeb.Solid.CompositionRoot;

namespace WBBWeb.Controllers
{
    [CustomHandleError]
    [IENoCache]
    public class FbbsaleportalController : WBBController
    {
        #region Properties

        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<StatLogCommand> _StatLogCommand;
        private readonly ICommandHandler<UpdatePreregisterStatusCommand> _UpdatePreregisterStatusCommand;
        private readonly ICommandHandler<UpdatePreregisterCommand> _UpdatePreregisterCommand;
        private readonly ICommandHandler<ConfigurationAutoMailCommand> _saveCommand;
        private readonly ICommandHandler<UpdateFBBSaleportalPreRegisterByOrdermcCommand> _updateFBBSaleportalPreRegisterByOrdermcCommand;

        private string rptName = "Report Name : {0}";
        private string rptCriteria = "Date From : {0}  To : {1}";
        private string rptDate = "Run Report Date/Time : {0}";
        private List<string> rptCriterias = new List<string>();

        #endregion

        #region Constructor

        public FbbsaleportalController(IQueryProcessor queryProcessor,
            ICommandHandler<StatLogCommand> StatLogCommand,
            ICommandHandler<UpdatePreregisterStatusCommand> UpdatePreregisterStatusCommand,
            ICommandHandler<UpdatePreregisterCommand> UpdatePreregisterCommand,
            ILogger logger,
            ICommandHandler<ConfigurationAutoMailCommand> saveCommand,
            ICommandHandler<UpdateFBBSaleportalPreRegisterByOrdermcCommand> updateFBBSaleportalPreRegisterByOrdermcCommand)
        {
            _queryProcessor = queryProcessor;
            _StatLogCommand = StatLogCommand;
            _UpdatePreregisterStatusCommand = UpdatePreregisterStatusCommand;
            _UpdatePreregisterCommand = UpdatePreregisterCommand;
            base.Logger = logger;
            _saveCommand = saveCommand;
            _updateFBBSaleportalPreRegisterByOrdermcCommand = updateFBBSaleportalPreRegisterByOrdermcCommand;
        }

        #endregion

        #region ActionResult

        public ActionResult Index()
        {
            if (null == base.CurrentUser)
                return RedirectToAction("Login", "Fbbsaleportal");

            ViewBag.User = base.CurrentUser;

            return View();
        }

        public ActionResult LeaveMessageReport()
        {
            if (null == base.CurrentUser)
                return RedirectToAction("Login", "Fbbsaleportal");

            ViewBag.User = base.CurrentUser;
            ViewBag.configscreen = GetScreenConfig("SALE_PORTAL_RPT_1");
            ViewBag.FbbConstant = GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant, "SALE_PORTAL_RPT_1");

            return View();
        }

        public ActionResult LeaveMessageData(string SaveStatus = "")
        {
            if (null == base.CurrentUser)
                return RedirectToAction("Login", "Fbbsaleportal");


            ViewBag.User = base.CurrentUser;
            ViewBag.configscreen = GetScreenConfig("SALE_PORTAL_MNG_1");
            ViewBag.FbbConstant = GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);
            ViewBag.LabelFBBOR039 = GetScreenConfig("FBBOR039");

            if (SaveStatus != "")
            {
                ViewBag.SaveStatus = SaveStatus;
            }

            return View();
        }

        public ActionResult LeaveMessageDataEdit(string RefNo = "")
        {
            if (null == base.CurrentUser)
                return RedirectToAction("Login", "Fbbsaleportal");

            if (RefNo == "")
                return RedirectToAction("LeaveMessageData", "Fbbsaleportal");

            ViewBag.User = base.CurrentUser;
            ViewBag.configscreen = GetScreenConfig("SALE_PORTAL_MNG_1");
            ViewBag.FbbConstant = GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);

            var lovRegister = base.LovData.FirstOrDefault(l => l.Type.Equals("FBB_CONSTANT") && l.Name.Equals("B_ACTIVE_REGISTER"));
            ViewBag.bRegister = lovRegister != null ? lovRegister.Text : string.Empty;

            LeaveMessageDataModel Model = new LeaveMessageDataModel() { IM_ORDER = false };

            var SalePortalLeaveMessageList = GetSalePortalLeaveMessageListByRefferenceNo(RefNo);

            if (SalePortalLeaveMessageList != null && SalePortalLeaveMessageList.Count > 0)
            {
                var SalePortalLeaveMessage = SalePortalLeaveMessageList.FirstOrDefault();
                Model.REF_NO = SalePortalLeaveMessage.REF_NO.ToSafeString();
                Model.LANGUAGE = SalePortalLeaveMessage.LANGUAGE.ToSafeString();
                Model.VILLAGE_NAME = SalePortalLeaveMessage.VILLAGE_NAME.ToSafeString();
                Model.BUILDING_NAME = SalePortalLeaveMessage.BUILDING_NAME.ToSafeString();
                Model.SERVICE_SPEED = SalePortalLeaveMessage.SERVICE_SPEED.ToSafeString();
                Model.CUST_NAME = SalePortalLeaveMessage.CUST_NAME.ToSafeString();
                Model.CUST_SURNAME = SalePortalLeaveMessage.CUST_SURNAME.ToSafeString();
                Model.CONTACT_MOBILE_NO = SalePortalLeaveMessage.CONTACT_MOBILE_NO.ToSafeString();
                Model.CONTACT_EMAIL = SalePortalLeaveMessage.CONTACT_EMAIL.ToSafeString();
                Model.HOUSE_NO = SalePortalLeaveMessage.HOUSE_NO.ToSafeString();
                Model.SOI = SalePortalLeaveMessage.SOI.ToSafeString();
                Model.ROAD = SalePortalLeaveMessage.ROAD.ToSafeString();
                Model.LOCATOIN_CODE = SalePortalLeaveMessage.LOCATION_CODE.ToSafeString();
                Model.ASC_CODE = SalePortalLeaveMessage.ASC_CODE.ToSafeString();
                Model.CHANNEL = SalePortalLeaveMessage.CHANNEL.ToSafeString();
                //if (SalePortalLeaveMessage.BUILDING_NAME != null && SalePortalLeaveMessage.BUILDING_NAME != "")
                //{
                //    Model.BUILDING_NAME = SalePortalLeaveMessage.BUILDING_NAME.ToSafeString();
                //}
                //else
                //{
                //    Model.BUILDING_NAME = SalePortalLeaveMessage.VILLAGE_NAME.ToSafeString();
                //}
                Model.SUB_DISTRICT = SalePortalLeaveMessage.SUB_DISTRICT.ToSafeString();
                Model.DISTRICT = SalePortalLeaveMessage.DISTRICT.ToSafeString();
                Model.PROVINCE = SalePortalLeaveMessage.PROVINCE.ToSafeString();
                Model.POSTAL_CODE = SalePortalLeaveMessage.POSTAL_CODE.ToSafeString();
                Model.CONTACT_TIME = SalePortalLeaveMessage.CONTACT_TIME.ToSafeString();
                Model.LINE_ID = SalePortalLeaveMessage.LINE_ID.ToSafeString();
                Model.VOUCHER_DESC = SalePortalLeaveMessage.VOUCHER_DESC.ToSafeString();
                Model.CAMPAIGN_PROJECT_NAME = SalePortalLeaveMessage.CAMPAIGN_PROJECT_NAME.ToSafeString();
                Model.RFF_INTERNET_NO = SalePortalLeaveMessage.REFERRAL_INTERNET_NO.ToSafeString();
                Model.LOCATION = SalePortalLeaveMessage.LOCATION_CHECK_COVERAGE.ToSafeString();
                Model.FULL_ADDRESS = SalePortalLeaveMessage.FULL_ADDRESS.ToSafeString();
                Model.ADDRESS_TYPE = SalePortalLeaveMessage.ADDRESS_TYPE.ToSafeString();
                if (SalePortalLeaveMessage.STATUS == "Register")
                {
                    UpdatePreregisterStatus(Model.REF_NO, "Lock");

                    Model.MODE = "Edit";

                    //Model.CONTACT_RESULT = "Y";
                    //Model.COVERAGE_RESULT = "Y";
                    //Model.REGISTER_RESULT = "Y";

                    Model.CONTACT_RESULT = string.IsNullOrEmpty(SalePortalLeaveMessage.IS_CONTACT_CUST) ? "Y" : SalePortalLeaveMessage.IS_CONTACT_CUST.ToSafeString();
                    Model.COVERAGE_RESULT = string.IsNullOrEmpty(SalePortalLeaveMessage.IS_IN_COV) ? "Y" : SalePortalLeaveMessage.IS_IN_COV.ToSafeString();
                    Model.REGISTER_RESULT = string.IsNullOrEmpty(SalePortalLeaveMessage.CLOSING_SALE) ? "Y" : SalePortalLeaveMessage.CLOSING_SALE.ToSafeString();

                    // Dropdownlist สาเหตุ การตรวจสอบพื้นที่ให้บริการ
                    Model.CONTACT = SalePortalLeaveMessage.REMARK_FOR_CONTACT_CUST.ToSafeString();
                    // Dropdownlist สาเหตุ ผลการดำเนินการ
                    Model.COVERAGE = SalePortalLeaveMessage.REMARK_FOR_NO_COV.ToSafeString();
                    // Dropdownlist สาเหตุ ลูกค้าสมัครบริการเอไอเอสไฟเบอร์หรือไม่
                    Model.REGISTER = SalePortalLeaveMessage.REMARK_FOR_NO_REG.ToSafeString();
                }
                else
                {
                    Model.MODE = "";
                    Model.CONTACT_RESULT = SalePortalLeaveMessage.IS_CONTACT_CUST.ToSafeString();
                    Model.COVERAGE_RESULT = SalePortalLeaveMessage.IS_IN_COV.ToSafeString();
                    Model.REGISTER_RESULT = SalePortalLeaveMessage.CLOSING_SALE.ToSafeString();

                    // Dropdownlist สาเหตุ การตรวจสอบพื้นที่ให้บริการ
                    Model.CONTACT = SalePortalLeaveMessage.REMARK_FOR_CONTACT_CUST.ToSafeString();
                    // Dropdownlist สาเหตุ ผลการดำเนินการ
                    Model.COVERAGE = SalePortalLeaveMessage.REMARK_FOR_NO_COV.ToSafeString();
                    // Dropdownlist สาเหตุ ลูกค้าสมัครบริการเอไอเอสไฟเบอร์หรือไม่
                    Model.REGISTER = SalePortalLeaveMessage.REMARK_FOR_NO_REG.ToSafeString();
                }

                if (SalePortalLeaveMessage.STATUS == "Complete")
                {
                    Model.COMPLEATED = true;
                }
                else
                {
                    Model.COMPLEATED = false;
                }

                //20.4
                Model.RELATE_MOBILE_NO = SalePortalLeaveMessage.RELATE_MOBILE_NO.ToSafeString();
                Model.FBB_PERCENT_DISCOUNT = SalePortalLeaveMessage.FBB_PERCENT_DISCOUNT.ToSafeString();

                //20.9
                Model.SALES_REP = SalePortalLeaveMessage.SALES_REP.ToSafeString();

                //R20.12
                //Model.ADDRESS_MC = SalePortalLeaveMessage.ADDRESS_MC.ToSafeString();
                Model.ORDER_MC_NO = SalePortalLeaveMessage.ORDER_MC_NO.ToSafeString();

                //R21.6 Out of Coverage
                Model.LATITUDE = SalePortalLeaveMessage.LATITUDE.ToSafeString();
                Model.LONGITUDE = SalePortalLeaveMessage.LONGITUDE.ToSafeString();

                //UR_PARTNER
                Model.EMP_ID = SalePortalLeaveMessage.EMP_ID.ToSafeString();
            }

            //17.10
            ViewBag.StaffLogin = base.CurrentUser.UserName;

            return View(Model);
        }

        public ActionResult UpdatePreregister(LeaveMessageDataModel Model)
        {
            if (null == base.CurrentUser)
                return RedirectToAction("Login", "Fbbsaleportal");

            string SaveStatus = "";

            var User = base.CurrentUser.UserName;
            // Checkbox Compleate
            string CompleatedFlag = "";
            if (Model.COMPLEATED)
                CompleatedFlag = "Y";
            else
                CompleatedFlag = "N";

            //19.6
            //Disable CONTACT_RESULT and COVERAGE_RESULT.
            // Dropdownlist สาเหตุ การตรวจสอบพื้นที่ให้บริการ
            string RemarkForNoCov = "";
            //if (Model.COVERAGE_RESULT.ToSafeString() == "Y")
            //    RemarkForNoCov = Model.COVERAGE_SUCCESS.ToSafeString();
            //else
            //    RemarkForNoCov = Model.COVERAGE_FAIL.ToSafeString();
            // Dropdownlist สาเหตุ ผลการดำเนินการ
            string RemarkForContactCust = "";
            //if (Model.CONTACT_RESULT.ToSafeString() == "Y")
            //    RemarkForContactCust = Model.CONTACT_SUCCESS.ToSafeString();
            //else
            //    RemarkForContactCust = Model.CONTACT_FAIL.ToSafeString();

            // Dropdownlist สาเหตุ ลูกค้าสมัครบริการเอไอเอสไฟเบอร์หรือไม่ 
            string RemarkForNoReg = "";
            if (Model.REGISTER_RESULT.ToSafeString() == "Y")
                RemarkForNoReg = Model.REGISTER_SUCCESS.ToSafeString();
            else
                RemarkForNoReg = Model.REGISTER_FAIL.ToSafeString();

            //R20.12
            //string VillageName = Model.ADDRESS_TYPE.ToSafeString() == "B" ? Model.BUILDING_NAME.ToSafeString() : Model.VILLAGE_NAME.ToSafeString();
            var command = new UpdatePreregisterCommand()
            {
                p_refference_no = Model.REF_NO.ToSafeString(),
                p_is_contact_cust = Model.CONTACT_RESULT.ToSafeString(),
                p_remark_for_contact_cust = RemarkForContactCust, //Model.CONTACT.ToSafeString(),
                p_is_in_cov = Model.COVERAGE_RESULT.ToSafeString(),
                p_remark_for_no_cov = RemarkForNoCov, //Model.COVERAGE.ToSafeString(),
                p_closing_sale = Model.REGISTER_RESULT.ToSafeString(),
                p_remark_for_no_reg = RemarkForNoReg, //Model.REGISTER.ToSafeString(),
                p_compleated_flag = CompleatedFlag,
                p_user_name = User

                //R20.12
                //p_house_no = Model.HOUSE_NO.ToSafeString(),
                //p_village_name = VillageName,
                //p_soi = Model.SOI.ToSafeString(),
                //p_road = Model.ROAD.ToSafeString()
            };

            _UpdatePreregisterCommand.Handle(command);

            string result = command.return_message;
            if (result == "Success")
            {
                UpdateFBBSaleportalPreRegisterByOrdermc(Model.REF_NO, Model.ORDER_MC_NO);
                SaveStatus = "Save Success";
            }
            else
            {
                SaveStatus = "Save Fail";
            }

            return RedirectToAction("LeaveMessageData", new { SaveStatus = SaveStatus });
        }

        public ActionResult ProcessLineOfficer_IM(string RefNo = "", string LcCode = "")
        {
            TempData["ProcessLineOffier"] = null;

            if (!String.IsNullOrWhiteSpace(RefNo) && !String.IsNullOrWhiteSpace(LcCode))
            {
                RefNo = RefNo.Trim().ToUpper();

                var SalePortalLeaveMessageList = GetSalePortalLeaveMessageListByRefferenceNo_IM(RefNo);
                if (SalePortalLeaveMessageList != null && SalePortalLeaveMessageList.Count > 0)
                {
                    OfficerController officerCtor = Bootstrapper.GetInstance<OfficerController>();
                    LeaveMessageDataModel leaveMessageModel = new LeaveMessageDataModel() { IM_ORDER = true };
                    SalePortalLeaveMessageList leaveMessageData = SalePortalLeaveMessageList.FirstOrDefault();
                    SeibelResultModel seibelData = officerCtor.GetSeibelData(LcCode).Data as SeibelResultModel;

                    if (seibelData.outStatus != "Error" && leaveMessageData != null)
                    {
                        leaveMessageModel.SERVICE_CASE_ID = leaveMessageData.SERVICE_CASE_ID.ToSafeString();
                        leaveMessageModel.LC_CODE = LcCode;
                        leaveMessageModel.OUT_TYPE = seibelData.outType;
                        leaveMessageModel.OUT_SUBTYPE = seibelData.outSubType;

                        leaveMessageModel.REF_NO = leaveMessageData.REF_NO.ToSafeString();
                        leaveMessageModel.LANGUAGE = leaveMessageData.LANGUAGE.ToSafeString();
                        leaveMessageModel.VILLAGE_NAME = leaveMessageData.VILLAGE_NAME.ToSafeString();
                        leaveMessageModel.BUILDING_NAME = leaveMessageData.BUILDING_NAME.ToSafeString();
                        //ใช้สำหรับการแยกระหว่างหมู่บ้านกับคอนโดในหน้า _CheckCoverage
                        //if (SalePortalLeaveMessage.BUILDING_NAME != null && SalePortalLeaveMessage.BUILDING_NAME != "")
                        //{
                        //    Model.BUILDING_NAME = SalePortalLeaveMessage.BUILDING_NAME.ToSafeString();
                        //}
                        //else
                        //{
                        //    Model.BUILDING_NAME = SalePortalLeaveMessage.VILLAGE_NAME.ToSafeString();
                        //}
                        leaveMessageModel.SERVICE_SPEED = leaveMessageData.SERVICE_SPEED.ToSafeString();
                        leaveMessageModel.CUST_NAME = leaveMessageData.CUST_NAME.ToSafeString();
                        leaveMessageModel.CUST_SURNAME = leaveMessageData.CUST_SURNAME.ToSafeString();
                        leaveMessageModel.CONTACT_MOBILE_NO = leaveMessageData.CONTACT_MOBILE_NO.ToSafeString();
                        leaveMessageModel.CONTACT_EMAIL = leaveMessageData.CONTACT_EMAIL.ToSafeString();
                        leaveMessageModel.HOUSE_NO = leaveMessageData.HOUSE_NO.ToSafeString();
                        leaveMessageModel.SOI = leaveMessageData.SOI.ToSafeString();
                        leaveMessageModel.ROAD = leaveMessageData.ROAD.ToSafeString();
                        leaveMessageModel.LOCATOIN_CODE = leaveMessageData.LOCATION_CODE.ToSafeString();
                        leaveMessageModel.ASC_CODE = leaveMessageData.ASC_CODE.ToSafeString();
                        leaveMessageModel.CHANNEL = leaveMessageData.CHANNEL.ToSafeString();
                        leaveMessageModel.SUB_DISTRICT = leaveMessageData.SUB_DISTRICT.ToSafeString();
                        leaveMessageModel.DISTRICT = leaveMessageData.DISTRICT.ToSafeString();
                        leaveMessageModel.PROVINCE = leaveMessageData.PROVINCE.ToSafeString();
                        leaveMessageModel.POSTAL_CODE = leaveMessageData.POSTAL_CODE.ToSafeString();
                        leaveMessageModel.CONTACT_TIME = leaveMessageData.CONTACT_TIME.ToSafeString();
                        leaveMessageModel.LINE_ID = leaveMessageData.LINE_ID.ToSafeString();
                        leaveMessageModel.VOUCHER_DESC = leaveMessageData.VOUCHER_DESC.ToSafeString();
                        leaveMessageModel.CAMPAIGN_PROJECT_NAME = leaveMessageData.CAMPAIGN_PROJECT_NAME.ToSafeString();
                        leaveMessageModel.RFF_INTERNET_NO = leaveMessageData.REFERRAL_INTERNET_NO.ToSafeString();
                        leaveMessageModel.LOCATION = leaveMessageData.LOCATION_CHECK_COVERAGE.ToSafeString();
                        leaveMessageModel.FULL_ADDRESS = leaveMessageData.FULL_ADDRESS.ToSafeString();
                        leaveMessageModel.ADDRESS_TYPE = leaveMessageData.ADDRESS_TYPE.ToSafeString();
                        leaveMessageModel.FLOOR = leaveMessageData.FLOOR.ToSafeString();
                        leaveMessageModel.MOO = leaveMessageData.MOO.ToSafeString();
                        leaveMessageModel.BUILDING_NO = leaveMessageData.BUILDING_NO.ToSafeString();
                        if (leaveMessageData.STATUS == "Register")
                        {
                            UpdatePreregisterStatus(leaveMessageModel.REF_NO, "Lock");

                            leaveMessageModel.MODE = "Edit";

                            //Model.CONTACT_RESULT = "Y";
                            //Model.COVERAGE_RESULT = "Y";
                            //Model.REGISTER_RESULT = "Y";

                            leaveMessageModel.CONTACT_RESULT = string.IsNullOrEmpty(leaveMessageData.IS_CONTACT_CUST) ? "Y" : leaveMessageData.IS_CONTACT_CUST.ToSafeString();
                            leaveMessageModel.COVERAGE_RESULT = string.IsNullOrEmpty(leaveMessageData.IS_IN_COV) ? "Y" : leaveMessageData.IS_IN_COV.ToSafeString();
                            leaveMessageModel.REGISTER_RESULT = string.IsNullOrEmpty(leaveMessageData.CLOSING_SALE) ? "Y" : leaveMessageData.CLOSING_SALE.ToSafeString();
                        }
                        else
                        {
                            leaveMessageModel.MODE = "";
                            leaveMessageModel.CONTACT_RESULT = leaveMessageData.IS_CONTACT_CUST.ToSafeString();
                            leaveMessageModel.COVERAGE_RESULT = leaveMessageData.IS_IN_COV.ToSafeString();
                            leaveMessageModel.REGISTER_RESULT = leaveMessageData.CLOSING_SALE.ToSafeString();
                        }
                        // Dropdownlist สาเหตุ การตรวจสอบพื้นที่ให้บริการ
                        leaveMessageModel.CONTACT = leaveMessageData.REMARK_FOR_CONTACT_CUST.ToSafeString();
                        // Dropdownlist สาเหตุ ผลการดำเนินการ
                        leaveMessageModel.COVERAGE = leaveMessageData.REMARK_FOR_NO_COV.ToSafeString();
                        // Dropdownlist สาเหตุ ลูกค้าสมัครบริการเอไอเอสไฟเบอร์หรือไม่
                        leaveMessageModel.REGISTER = leaveMessageData.REMARK_FOR_NO_REG.ToSafeString();

                        leaveMessageModel.COMPLEATED = leaveMessageData.STATUS == "Complete" ? true : false;

                        TempData["ProcessLineOffier"] = leaveMessageModel;
                    }
                }
            }

            return RedirectToAction("Index", "Process");
        }

        [AllowAnonymous]
        [CustomActionFilter(LogType = "Login")]
        public ActionResult Login(string returnUrl)
        {
            try
            {
                var query = new SelectLovQuery
                {
                    LOV_TYPE = "CONFIG_TEMP_LOGIN_PAGE"
                };
                var data = _queryProcessor.Execute(query);

                //Logger.Info(data.DumpToString("TemporaryLogin"));

                string flg = "N";
                if (data.Any())
                    flg = data.FirstOrDefault().LOV_VAL1;

                if (flg == "Y")
                {
                    ViewBag.ReturnUrl = returnUrl;
                    var titleQuery = new GetLovQuery
                    {
                        LovType = "SCREEN",
                        LovName = "L_PAGE_SALE_PORTAL_TITLE",
                    };
                    var titleLogin = _queryProcessor.Execute(titleQuery);
                    ViewBag.configscreen = titleLogin;
                    return View();
                }
                else
                {
                    ViewBag.ReturnUrl = returnUrl;
                    return RedirectToAction("TemporaryNotAvailable", "Fbbsaleportal");
                }

            }
            catch (Exception ex)
            {
                Logger.Info(ex.GetErrorMessage());
                Logger.Info(ex.StackTrace);

                return RedirectToAction("TemporaryNotAvailable", "Fbbsaleportal");
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [CustomActionFilter(LogType = "Login")]
        public ActionResult Login(LoginPanelModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var userName = model.UserName.Replace("\"", "").Replace("'", "");
                var passWord = model.Password.Replace("'", "").Replace("'", "");
                var grouIdQuery = new GetLovQuery
                {
                    LovType = "FBB_CONSTANT",
                    LovName = "USER_TELESALE_GROUP_ID"
                };
                var groupId = _queryProcessor.Execute(grouIdQuery);

                var msgUserQuery = new GetLovQuery
                {
                    LovType = "FBB_CONSTANT",
                    LovName = "MSG_USER_FAILD"
                };
                var msgUser = _queryProcessor.Execute(msgUserQuery);

                var cfgLDAPQuery = new GetLovQuery
                {
                    LovType = "FBB_CONSTANT",
                    LovName = "URLAuthenLDAP"
                };
                var cfgLDAP = _queryProcessor.Execute(cfgLDAPQuery);
                bool useLDAP = false;
                if (cfgLDAP != null && cfgLDAP.Count > 0)
                {
                    useLDAP = true;
                }

                var msgPassQuery = new GetLovQuery
                {
                    LovType = "FBB_CONSTANT",
                    LovName = "MSG_USER_PASS_FAILD"
                };
                var msgPass = _queryProcessor.Execute(msgPassQuery);

                if (useLDAP)
                {
                    UserModel authenticatedUser = null;
                    LovValueModel tmpGroup = null;
                    foreach (var item in groupId)
                    {
                        if (authenticatedUser == null || authenticatedUser.Groups == null)
                        {
                            tmpGroup = item;
                            authenticatedUser = GetUser(userName, item.LovValue1);
                        }
                    }

                    if (null != authenticatedUser && (null != authenticatedUser.UserName
                        && (null != authenticatedUser.Groups)
                        && (authenticatedUser.Groups[0] == Convert.ToDecimal(tmpGroup.LovValue1))))
                    {
                        var authenResultMessage = "";
                        if (AuthenLDAP(userName, passWord, out authenResultMessage))
                        {
                            //var authenticatedUser = GetUser(userName);
                            authenticatedUser.AuthenticateType = AuthenticateType.LDAP;
                            Response.AppendCookie(CreateAuthenticatedCookie(authenticatedUser.UserName));
                            base.CurrentUser = authenticatedUser;

                            return RedirectToAction("Index", "Fbbsaleportal");
                        }
                        else
                        {
                            ModelState.AddModelError("", msgPass[0].LovValue1);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", userName + " " + msgUser[0].LovValue1);
                    }
                }
                else
                {
                    // bypass authen
                    UserModel authenticatedUser = null;
                    foreach (var item in groupId)
                    {
                        if (authenticatedUser == null || authenticatedUser.Groups == null)
                        {
                            authenticatedUser = GetUser(userName, item.LovValue1);
                        }
                    }
                    //Session["userName"] = userName;
                    if (null != authenticatedUser && (null != authenticatedUser.UserName && (null != authenticatedUser.Groups)))
                    {
                        authenticatedUser.AuthenticateType = AuthenticateType.LDAP;
                        Response.AppendCookie(CreateAuthenticatedCookie(authenticatedUser.UserName));
                        base.CurrentUser = authenticatedUser;
                        return RedirectToAction("Index", "Fbbsaleportal");
                    }
                    else
                    {
                        ModelState.AddModelError("", userName + " " + msgUser[0].LovValue1);
                    }
                }
            }
            var titleQuery = new GetLovQuery
            {
                LovType = "SCREEN",
                LovName = "L_PAGE_SALE_PORTAL_TITLE",
            };
            var titleLogin = _queryProcessor.Execute(titleQuery);
            ViewBag.configscreen = titleLogin;
            return View(model);
        }

        public ActionResult Logout()
        {
            var normalLogout = true;
            var sessionTimeOut = false;
            var isSSO = false;
            try
            {
                if (null != base.CurrentUser)
                {
                    if (base.CurrentUser.AuthenticateType == AuthenticateType.SSO)
                    {
                        var currentUser = base.CurrentUser;

                        using (var ssoService = new EmployeeServices.EmployeeServiceWebServiceV2Service())
                        {
                            Logger.Info(string.Format("Decreasing SSO, Token:{0}", currentUser.SSOFields.Token));

                            try
                            {
                                var syncUserSessionResponse = ssoService.decreaseCounter(currentUser.SSOFields.Token);
                                if (syncUserSessionResponse.Message.ErrorCode == Constants.SSOReturnStatus.Success)
                                {
                                    Logger.Info("SSO Decreasing SUCCESS " +
                                        string.Format("{0}:{1}",
                                            syncUserSessionResponse.Message.ErrorCode,
                                            syncUserSessionResponse.Message.ErrorMesg));
                                }
                                else
                                {
                                    Logger.Info("SSO Decreasing FAIL " +
                                         string.Format("{0}:{1}",
                                             syncUserSessionResponse.Message.ErrorCode,
                                             syncUserSessionResponse.Message.ErrorMesg));
                                }
                            }
                            catch (TimeoutException tex)
                            {
                                Logger.Info("SSO syncUserSession TIMEOUT " + tex.GetErrorMessage());
                            }
                            catch (Exception ex)
                            {
                                Logger.Info("SSO syncUserSession ERROR " + ex.GetErrorMessage());
                            }

                        }

                        normalLogout = true;
                        isSSO = true;
                    }
                    else
                    {
                        normalLogout = true;
                        isSSO = false;
                    }

                    FormsAuthentication.SignOut();
                    Session.Clear();
                    Session.Abandon();
                }
                else
                {
                    Logger.Info("Session Time Out");

                    sessionTimeOut = true;
                    normalLogout = false;
                    FormsAuthentication.SignOut();
                    Session.Clear();
                    Session.Abandon();
                }
            }
            catch (Exception ex)
            {
                Logger.Info(ex.GetErrorMessage());
                Logger.Info(ex.StackTrace);

                FormsAuthentication.SignOut();
                Session.Clear();
                Session.Abandon();
            }

            return RedirectToAction("LogoutSso", "Fbbsaleportal",
                new { logout = normalLogout, sessionIsTimeOut = sessionTimeOut, sso = isSSO, });
        }

        [HttpGet]
        public ActionResult LogoutSso(bool logout, bool sessionIsTimeOut, bool sso)
        {
            ViewBag.NormalLogOut = logout;
            ViewBag.SessionTimeOut = sessionIsTimeOut;
            ViewBag.IsSSO = sso;
            return View();
        }

        [HttpPost]
        public string UpdateFBBSaleportalPreRegisterByOrdermc(string RefferenceNo = "", string OrderMC = "")
        {
            string ReturnCode = "";
            if (RefferenceNo != "" && OrderMC != "")
            {
                UpdateFBBSaleportalPreRegisterByOrdermcCommand command = new UpdateFBBSaleportalPreRegisterByOrdermcCommand()
                {
                    RefferenceNo = RefferenceNo,
                    OrderMC = OrderMC
                };
                _updateFBBSaleportalPreRegisterByOrdermcCommand.Handle(command);
                ReturnCode = command.ReturnCode;
            }
            else
            {
                ReturnCode = "Param Is null.";
            }
            // command.ReturnCode == "20000" is Success
            return ReturnCode;

        }

        #endregion

        #region JsonResult

        public JsonResult GetContact()
        {
            var CONTACT = new List<DropdownModel>();
            try
            {
                CONTACT = GetDropDownConfig("SALE_PORTAL_MNG_1", "FBB_CONSTANT", "LIST_RESULT_CONTACT");
                DropdownModel dropdownModel = new DropdownModel();
                dropdownModel.Text = "กรุณาเลือก";
                dropdownModel.Value = "";
                CONTACT.Insert(0, dropdownModel);
            }
            catch (Exception) { }

            return Json(CONTACT, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCoverage()
        {
            var CONTACT = new List<DropdownModel>();
            try
            {
                CONTACT = GetDropDownConfig("SALE_PORTAL_MNG_1", "FBB_CONSTANT", "LIST_RESULT_COVERAGE");
                DropdownModel dropdownModel = new DropdownModel();
                dropdownModel.Text = "กรุณาเลือก";
                dropdownModel.Value = "";
                CONTACT.Insert(0, dropdownModel);
            }
            catch (Exception) { }

            return Json(CONTACT, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetRegister()
        {
            var CONTACT = new List<DropdownModel>();
            try
            {
                CONTACT = GetDropDownConfig("SALE_PORTAL_MNG_1", "FBB_CONSTANT", "LIST_RESULT_REGISTER");
                DropdownModel dropdownModel = new DropdownModel();
                dropdownModel.Text = "กรุณาเลือก";
                dropdownModel.Value = "";
                CONTACT.Insert(0, dropdownModel);
            }
            catch (Exception) { }

            return Json(CONTACT, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetContactSuccess()
        {   //19.1
            var CONTACT = new List<DropdownModel>();
            try
            {
                CONTACT = GetDropDownConfig("SALE_PORTAL_MNG_1", "FBB_CONSTANT", "LIST_RESULT_CONTACT_SUCCESS");
                DropdownModel dropdownModel = new DropdownModel();
                dropdownModel.Text = "กรุณาเลือก";
                dropdownModel.Value = "";
                CONTACT.Insert(0, dropdownModel);
            }
            catch (Exception) { }

            return Json(CONTACT, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetContactFail()
        {   //19.1
            var CONTACT = new List<DropdownModel>();
            try
            {
                CONTACT = GetDropDownConfig("SALE_PORTAL_MNG_1", "FBB_CONSTANT", "LIST_RESULT_CONTACT_FAIL");
                DropdownModel dropdownModel = new DropdownModel();
                dropdownModel.Text = "กรุณาเลือก";
                dropdownModel.Value = "";
                CONTACT.Insert(0, dropdownModel);
            }
            catch (Exception) { }

            return Json(CONTACT, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCoverageSuccess()
        {   //19.1
            var COVERAGE = new List<DropdownModel>();
            try
            {
                COVERAGE = GetDropDownConfig("SALE_PORTAL_MNG_1", "FBB_CONSTANT", "LIST_RESULT_COVERAGE_SUCCESS");
                DropdownModel dropdownModel = new DropdownModel();
                dropdownModel.Text = "กรุณาเลือก";
                dropdownModel.Value = "";
                COVERAGE.Insert(0, dropdownModel);
            }
            catch (Exception) { }

            return Json(COVERAGE, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCoverageFail()
        {   //19.1
            var COVERAGE = new List<DropdownModel>();
            try
            {
                COVERAGE = GetDropDownConfig("SALE_PORTAL_MNG_1", "FBB_CONSTANT", "LIST_RESULT_COVERAGE_FAIL");
                DropdownModel dropdownModel = new DropdownModel();
                dropdownModel.Text = "กรุณาเลือก";
                dropdownModel.Value = "";
                COVERAGE.Insert(0, dropdownModel);
            }
            catch (Exception) { }

            return Json(COVERAGE, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetRegisterSuccess()
        {   //19.1
            var REGISTER = new List<DropdownModel>();
            try
            {
                REGISTER = GetDropDownConfig("SALE_PORTAL_MNG_1", "FBB_CONSTANT", "LIST_RESULT_REGISTER_SUCCESS");
                DropdownModel dropdownModel = new DropdownModel();
                dropdownModel.Text = "กรุณาเลือก";
                dropdownModel.Value = "";
                REGISTER.Insert(0, dropdownModel);
            }
            catch (Exception) { }

            return Json(REGISTER, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetRegisterFail()
        {   //19.1
            var REGISTER = new List<DropdownModel>();
            try
            {
                REGISTER = GetDropDownConfig("SALE_PORTAL_MNG_1", "FBB_CONSTANT", "LIST_RESULT_REGISTER_FAIL");
                DropdownModel dropdownModel = new DropdownModel();
                dropdownModel.Text = "กรุณาเลือก";
                dropdownModel.Value = "";
                REGISTER.Insert(0, dropdownModel);
            }
            catch (Exception) { }

            return Json(REGISTER, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetStaffId(string refNo, string staffLogin)
        {   //17.10 
            var pinCode = "";
            try
            {
                string fullUrl = this.Url.Action("GetStaffId", "Fbbsaleportal", null, this.Request.Url.Scheme);

                var query = new WsLdapQueryIdQuery
                {
                    Username = staffLogin,
                    FullUrl = fullUrl,
                    TransactionId = refNo
                };

                var response = _queryProcessor.Execute(query);
                pinCode = response.PinCode;

            }
            catch (Exception) { }

            return Json(pinCode, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Public Method

        public bool UpdatePreregisterStatus(string refNo, string status)
        {
            if (null != base.CurrentUser)
            {
                var User = base.CurrentUser.UserName;

                try
                {
                    var command = new UpdatePreregisterStatusCommand()
                    {
                        p_refference_no = refNo.ToSafeString(),
                        p_status = status.ToSafeString(),
                        p_user_name = User

                    };

                    _UpdatePreregisterStatusCommand.Handle(command);

                    return true;
                }
                catch (Exception ex)
                {
                    Logger.Info("Error when call GetSalePortalLeaveMessageList");
                    Logger.Info(ex.GetErrorMessage());
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public List<LovScreenValueModel> GetScreenConfig(string page)
        {
            try
            {
                List<LovValueModel> config = null;

                config = base.LovData.Where(l =>
                    (!string.IsNullOrEmpty(l.Type) && l.Type == "SCREEN")
                        && (!string.IsNullOrEmpty(l.LovValue5) && l.LovValue5.Equals(page))).ToList();

                var screenValue = new List<LovScreenValueModel>();

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
                return screenValue;
            }
            catch (Exception ex)
            {
                Logger.Info(ex.GetErrorMessage());
                return new List<LovScreenValueModel>();
            }
        }

        #endregion

        #region Private Method

        private List<DropdownModel> GetDropDownConfig(string pageCode, string type, string name)
        {
            return base.LovData
                        .Where(l => l.LovValue5 == pageCode & l.Type == type & l.Name == name)
                        .Select(l => new DropdownModel
                        {
                            Text = l.LovValue1,
                            Value = l.Text,
                        })
                        .ToList();
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

        private List<FbbConstantModel> GetFbbConstantModel(string fbbConstType, string page)
        {
            var data = base.LovData
               .Where(l => (l.Type.Equals(fbbConstType)) && (!string.IsNullOrEmpty(l.LovValue5) && l.LovValue5.Equals(page)))
               .Select(l => new FbbConstantModel
               {
                   Field = l.Name,
                   Validation = l.LovValue1,
                   SubValidation = l.LovValue2
               }).ToList();

            return data;
        }

        #endregion

        #region อื่นๆ

        public UserModel GetUser(string userName, string groupId)
        {
            var userQuery = new GetUserDataQuery
            {
                UserName = userName,
                GroupId = groupId
            };

            var authenticatedUser = _queryProcessor.Execute(userQuery);
            return authenticatedUser;
        }

        private HttpCookie CreateAuthenticatedCookie(string userName)
        {
            var authTicket = new FormsAuthenticationTicket(2, userName, DateTime.Now,
                DateTime.Now.AddMinutes(FormsAuthentication.Timeout.TotalMinutes), false, "");

            var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName,
                FormsAuthentication.Encrypt(authTicket))
            { HttpOnly = true };

            return authCookie;
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult TemporaryNotAvailable()
        {
            return View();
        }

        public bool AuthenLDAP(string userName, string password, out string authenMessage)
        {
            var authLDAPQuery = new GetAuthenLDAPQuery
            {
                UserName = userName,
                Password = password,
                ProjectCode = Configurations.ProjectCodeLdapFBB,
            };

            var authenLDAPResult = _queryProcessor.Execute(authLDAPQuery);
            authenMessage = "";
            return authenLDAPResult;
        }

        #endregion

        #region ExportExcel

        public ActionResult ReadSearchSalePortalLeaveMessage([DataSourceRequest] DataSourceRequest request, string dataS = "")
        {
            Logger.Info("ReadSearchSalePortalLeaveMessage start");
            if (dataS != null && dataS != "")
            {
                try
                {
                    Logger.Info("ReadSearchSalePortalLeaveMessage try");
                    var searchRpt02Model = new JavaScriptSerializer().Deserialize<SalePortalLeaveMessageModel>(dataS);
                    searchRpt02Model.RefferenceNo = "";
                    searchRpt02Model.CustomerName = "";
                    searchRpt02Model.ContactMobile = "";
                    var result = GetSalePortalLeaveMessageList(searchRpt02Model);

                    return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    Logger.Info("Error when call ReadSearchSalePortalLeaveMessage");
                    Logger.Info(ex.GetErrorMessage());
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public ActionResult ReadSearchSalePortalLeaveMessageData([DataSourceRequest] DataSourceRequest request, string dataS = "")
        {
            Logger.Info("ReadSearchSalePortalLeaveMessage start");
            if (dataS != null && dataS != "")
            {
                try
                {
                    Logger.Info("ReadSearchSalePortalLeaveMessage try");
                    var searchModel = new JavaScriptSerializer().Deserialize<SalePortalLeaveMessageModel>(dataS);
                    var result = GetSalePortalLeaveMessageList(searchModel);

                    foreach (var item in result)
                    {
                        item.CUST_NAME = item.CUST_NAME + " " + item.CUST_SURNAME;
                        if (item.FULL_ADDRESS == null || item.FULL_ADDRESS == "")
                        {
                            item.HOUSE_NO = item.HOUSE_NO + " " + item.BUILDING_NAME + item.VILLAGE_NAME + " " + item.SOI + " " + item.ROAD + " "
                            + item.SUB_DISTRICT + " " + item.DISTRICT + " " + item.PROVINCE + " " + item.POSTAL_CODE;
                        }
                        else
                        {
                            item.HOUSE_NO = item.FULL_ADDRESS;
                        }
                    }

                    return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    Logger.Info("Error when call ReadSearchSalePortalLeaveMessage");
                    Logger.Info(ex.GetErrorMessage());
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public ActionResult GetChannel()
        {
            List<DropdownModel> data = new List<DropdownModel>();
            List<SalePortalChannelModel> channelList = GetSalePortalChannelList();
            foreach (var item in channelList)
                data.Add(new DropdownModel() { Value = item.Name, Text = item.Name });

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCreateFrom()
        {
            var data = new List<DropdownModel>();

            string[] lovNames = new string[] { "ORDER_TIME_0830", "ORDER_TIME_1700", "ORDER_TIME_1930" };
            var lovConfig = GetScreenConfig("FBBOR039").Where(o => lovNames.Contains(o.Name)).OrderBy(o => o.OrderByPDF);
            foreach (var item in lovConfig)
                data.Add(new DropdownModel() { Value = item.DisplayValue, Text = item.DisplayValue });

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCreateTo()
        {
            var data = new List<DropdownModel>();

            string[] lovNames = new string[] { "ORDER_TIME_0830", "ORDER_TIME_1700", "ORDER_TIME_1930" };
            var lovConfig = GetScreenConfig("FBBOR039").Where(o => lovNames.Contains(o.Name)).OrderBy(o => o.OrderByPDF);
            foreach (var item in lovConfig)
                data.Add(new DropdownModel() { Value = item.DisplayValue, Text = item.DisplayValue });

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        private List<SalePortalChannelModel> GetSalePortalChannelList()
        {
            Logger.Info("GetSalePortalChannelList Start");
            try
            {
                Logger.Info("GetSalePortalChannelList Try");

                var query = new GetSalePortalChannelQuery();
                List<SalePortalChannelModel> returnList = _queryProcessor.Execute(query);

                return returnList;
            }
            catch (Exception ex)
            {
                Logger.Info("Error when call GetSalePortalChannelList");
                Logger.Info(ex.GetErrorMessage());

                return new List<SalePortalChannelModel>();
            }
        }

        private List<SalePortalLeaveMessageList> GetSalePortalLeaveMessageList(SalePortalLeaveMessageModel searchRptModel)
        {
            Logger.Info("GetSalePortalLeaveMessageList Start");
            try
            {
                Logger.Info("GetSalePortalLeaveMessageList Try");
                var query = new SalePortalLeaveMessageQuery()
                {
                    p_reg_date_from = searchRptModel.DateFrom.ToSafeString(),
                    p_reg_date_to = searchRptModel.DateTo.ToSafeString(),
                    p_refference_no = searchRptModel.RefferenceNo.ToSafeString(),
                    p_customer_name = searchRptModel.CustomerName.ToSafeString(),
                    p_contact_mobile = searchRptModel.ContactMobile.ToSafeString(),
                    p_channel = searchRptModel.Channel,
                    p_reg_time_from = searchRptModel.TimeFrom,
                    p_reg_time_to = searchRptModel.TimeTo
                };

                var returnList = _queryProcessor.Execute(query);

                return returnList;
            }
            catch (Exception ex)
            {
                Logger.Info("Error when call GetSalePortalLeaveMessageList");
                Logger.Info(ex.GetErrorMessage());
                return new List<SalePortalLeaveMessageList>();
            }
        }

        private List<SalePortalLeaveMessageList> GetSalePortalLeaveMessageListByRefferenceNo(string RefferenceNO)
        {
            Logger.Info("GetSalePortalLeaveMessageList Start");
            try
            {
                Logger.Info("GetSalePortalLeaveMessageList Try");
                var query = new SalePortalLeaveMessageByRefferenceNoQuery()
                {
                    p_refference_no = RefferenceNO.ToSafeString()
                };

                var returnList = _queryProcessor.Execute(query);

                return returnList;
            }
            catch (Exception ex)
            {
                Logger.Info("Error when call GetSalePortalLeaveMessageList");
                Logger.Info(ex.GetErrorMessage());
                return new List<SalePortalLeaveMessageList>();
            }

        }

        private List<SalePortalLeaveMessageList> GetSalePortalLeaveMessageListByRefferenceNo_IM(string RefferenceNO)
        {
            Logger.Info("GetSalePortalLeaveMessageList_IM Start");
            try
            {
                Logger.Info("GetSalePortalLeaveMessageList_IM Try");
                var query = new SalePortalLeaveMessageByRefferenceNoQuery_IM()
                {
                    p_refference_no = RefferenceNO.ToSafeString()
                };

                var returnList = _queryProcessor.Execute(query);

                return returnList;
            }
            catch (Exception ex)
            {
                Logger.Info("Error when call GetSalePortalLeaveMessageList_IM");
                Logger.Info(ex.GetErrorMessage());
                return new List<SalePortalLeaveMessageList>();
            }

        }

        string datefromfromCri, datetofromCri;

        public ActionResult ExportSalePortalLeaveMessageData(string dataS)
        {
            List<SalePortalLeaveMessageList> listall;
            var searchModel = new JavaScriptSerializer().Deserialize<SalePortalLeaveMessageModel>(dataS);
            searchModel.RefferenceNo = "";
            searchModel.CustomerName = "";
            searchModel.ContactMobile = "";

            listall = GetSalePortalLeaveMessageList(searchModel);

            rptCriteria = string.Format(rptCriteria, searchModel.DateFrom, searchModel.DateTo);
            rptName = string.Format(rptName, "Sale Portal Leave Message Report");
            rptDate = string.Format(rptDate, DateTime.Now.ToDisplayText());

            datefromfromCri = searchModel.DateFrom.ToSafeString();
            datetofromCri = searchModel.DateTo.ToSafeString();

            string filename = GetSalePortalLeaveMessageExcelName("SalePortalLeaveMessageReport");
            var bytes = GenerateSalePortalLeaveMessageEntitytoExcel<SalePortalLeaveMessageList>(listall, filename, "SALE_PORTAL_RPT_1");

            return File(bytes, "application/excel", filename + ".xls");

        }

        // // Get Excel File Name// //
        private string GetSalePortalLeaveMessageExcelName(string fileName)
        {
            string result = string.Empty;

            //DateTime currDateTime = DateTime.Now;

            string datefrom = datefromfromCri.Replace("/", "");
            string dateto = datetofromCri.Replace("/", "");

            result = string.Format("{0}_{1}_{2}", fileName, datefrom, dateto);

            return result;
        }

        // // Generate Entity ///
        public byte[] GenerateSalePortalLeaveMessageEntitytoExcel<T>(List<T> data, string fileName, string LovValue5)
        {
            Logger.Info("GenerateSalePortalLeaveMessageEntitytoExcel start");
            System.ComponentModel.PropertyDescriptorCollection props =
            System.ComponentModel.TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();

            var lovDataScreen = base.LovData.Where(p => p.Type == "SCREEN" && p.LovValue5 == LovValue5 && p.Name.Contains("C_")).OrderBy(p => p.OrderBy).ToList();

            if (lovDataScreen.Count != 0)
            {
                lovDataScreen = lovDataScreen.OrderBy(t => t.OrderBy).ToList();
                foreach (var item in lovDataScreen)
                {
                    table.Columns.Add(item.LovValue1.ToSafeString().Trim(), System.Type.GetType("System.String"));
                }

                object[] values = new object[lovDataScreen.Count];
                foreach (T item in data)
                {

                    int CountColumn = 0;
                    foreach (var itemColumn in lovDataScreen)
                    {
                        values[CountColumn] = props.Find(itemColumn.LovValue2.ToSafeString().Trim(), true).GetValue(item);
                        CountColumn++;
                    }
                    table.Rows.Add(values);
                }
            }
            else
            {
                for (int i = 0; i < props.Count; i++)
                {
                    System.ComponentModel.PropertyDescriptor prop = props[i];
                    table.Columns.Add(prop.Name, System.Type.GetType("System.String"));
                }

                object[] values = new object[props.Count];
                foreach (T item in data)
                {
                    for (int i = 0; i < values.Length; i++)
                    {
                        values[i] = props[i].GetValue(item);
                    }
                    table.Rows.Add(values);
                }
            }


            string tempPath = System.IO.Path.GetTempPath();

            var data_ = GenerateSalePortalLeaveMessageRptExcel(table, "WorkSheet", tempPath, fileName, LovValue5);
            return data_;
        }

        // // Generate Excel ///
        private byte[] GenerateSalePortalLeaveMessageRptExcel(DataTable dataToExcel, string excelSheetName, string directoryPath, string fileName, string LovValue5)
        {
            Logger.Info("GenerateSalePortalLeaveMessageRptExcel start");
            if (System.IO.File.Exists(directoryPath + "\\" + fileName + ".xls"))
            { System.IO.File.Delete(directoryPath + "\\" + fileName + ".xls"); }

            //string currentDirectorypath = Environment.CurrentDirectory;
            string finalFileNameWithPath = string.Empty;

            finalFileNameWithPath = string.Format("{0}\\{1}.xls", directoryPath, fileName);

            if (System.IO.File.Exists(finalFileNameWithPath))
            { System.IO.File.Delete(finalFileNameWithPath); }

            //Delete existing file with same file name.

            var newFile = new FileInfo(finalFileNameWithPath);
            ExcelRange range = null;
            ExcelRange rangeReportDetail = null;
            ExcelRange rangeHeader = null;

            int iRow;
            int iHeaderRow;
            string strRow;
            string strMergeRow = string.Empty;
            string strColumn1 = string.Empty;
            string strColumn2 = string.Empty;
            int iCol = 8;

            //Step 1 : Create object of ExcelPackage class and pass file path to constructor.
            using (var package = new ExcelPackage(newFile))
            {
                //Step 2 : Add a new worksheet to ExcelPackage object and give a suitable name
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(excelSheetName);

                worksheet.Cells["A2:G2"].Merge = true;
                worksheet.Cells["A2,G2"].LoadFromText(rptName);
                worksheet.Cells["A3:I3"].Merge = true;
                worksheet.Cells["A3,I3"].LoadFromText(rptCriteria);
                worksheet.Cells["A4:I4"].Merge = true;
                worksheet.Cells["A4,I4"].LoadFromText(rptDate);
                rangeReportDetail = worksheet.SelectedRange[2, 1, 4, 4];
                rangeReportDetail.Style.Fill.PatternType = ExcelFillStyle.None;
                rangeReportDetail.Style.Font.Bold = true;
                rangeReportDetail.Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#17324E"));

                iRow = 7;
                iHeaderRow = iRow + 1;
                strRow = iRow.ToSafeString();

                switch (LovValue5)
                {

                    case "SALE_PORTAL_RPT_1":
                        range = worksheet.SelectedRange[1, 8, dataToExcel.Rows.Count + iHeaderRow, 8];
                        range.Style.Numberformat.Format = "dd/MM/yyyy";
                        break;
                    default:
                        iCol = 14;
                        break;
                }

                rangeHeader = worksheet.SelectedRange[iRow, 1, iRow, iCol];
                rangeHeader.Style.Fill.PatternType = ExcelFillStyle.Solid;
                rangeHeader.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#c9c9c9"));
                rangeHeader.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                worksheet.View.FreezePanes(iHeaderRow, 1);
                strColumn1 = string.Format("A{0}", strRow);

                //Step 3 : Start loading datatable form A1 cell of worksheet.
                worksheet.Cells[strColumn1].LoadFromDataTable(dataToExcel, true, TableStyles.None);

                //Step 4 : (Optional) Set the file properties like title, author and subject
                package.Workbook.Properties.Title = @"FBB Config";
                package.Workbook.Properties.Author = "FBB";
                package.Workbook.Properties.Subject = @"" + excelSheetName;

                //Step 5 : Save all changes to ExcelPackage object which will create Excel 2007 file.
                package.Save();

                byte[] data = System.IO.File.ReadAllBytes(finalFileNameWithPath);
                return data;
            }

        }

        #endregion

        #region Configuration automail report
        //Configuration automail report
        public ActionResult ConfigurationAutoMailReport(string reportName, string scheduler)
        {
            if (null == base.CurrentUser)
                return RedirectToAction("Login", "Fbbsaleportal");

            ViewBag.User = CurrentUser;
            ViewBag.ReportName = reportName;
            ViewBag.Scheduler = scheduler;
            return View();
        }

        public ActionResult ConfigurationInformation(string reportId, string reportName, string scheduler)
        {
            if (null == base.CurrentUser)
                return RedirectToAction("Login", "Fbbsaleportal");

            var configModel = new ConfigurationReportModel();

            ViewBag.User = CurrentUser;
            ViewBag.ReportName = reportName;
            ViewBag.Scheduler = scheduler;
            if (!string.IsNullOrEmpty(reportId))
            {
                var query = new GetConfigurationReportByIdQuery
                {
                    ReportId = reportId
                };
                configModel = _queryProcessor.Execute(query);
            }


            return View(configModel);
        }

        public ActionResult SaveConfigurationInformation(ConfigurationReportModel model)
        {
            string saveStatus;

            if (null == base.CurrentUser)
            {
                saveStatus = "T";
                return Json(new { saveStatus }, JsonRequestBehavior.AllowGet);
            }
            string errMsg = "";


            var command = new ConfigurationAutoMailCommand
            {
                report_id = model.REPORT_ID,
                report_name = model.REPORT_NAME,
                scheduler = model.SCHEDULER,
                day_of_week = model.DAY_OF_WEEK,
                month_of_year = model.MONTH_OF_YEAR,
                day_of_month = model.DAY_OF_MONTH,
                email_to = model.EMAIL_TO,
                email_from = model.EMAIL_FROM,
                email_cc = model.EMAIL_CC,
                email_subject = model.EMAIL_SUBJECT,
                email_content = model.EMAIL_CONTENT,
                email_to_admin = model.EMAIL_TO_ADMIN,
                active_flag = model.ACTIVE_FLAG,
                report_type = model.REPORT_TYPE,
                created_by = CurrentUser.UserName
            };
            foreach (var item in model.ConfigurationQueryList)
            {
                if (item.QUERY_ID == 0 && item.QUERY_TYPE == "D")
                    continue;

                var config = new ConfigurationQueryArrayModel
                {
                    query_id = item.QUERY_ID,
                    sheet_name = item.SHEET_NAME,
                    owner_db = item.OWNER_DB,
                    query_1 = item.QUERY_1,
                    query_2 = item.QUERY_2,
                    query_3 = item.QUERY_3,
                    query_4 = item.QUERY_4,
                    query_5 = item.QUERY_5,
                    query_type = item.QUERY_TYPE
                };
                command.ConfigurationQueryList.Add(config);

            }

            _saveCommand.Handle(command);
            if (command.return_code == 0)
            {
                saveStatus = "Y";

            }
            else if (command.return_code == 1)
            {
                saveStatus = "D";
            }
            else
            {
                saveStatus = "E";
                errMsg = command.return_msg;
            }

            return Json(new { saveStatus, errMsg }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteReport(string reportId)
        {
            string saveStatus = "N";

            if (null == base.CurrentUser)
            {
                saveStatus = "T";
                return Json(new { saveStatus }, JsonRequestBehavior.AllowGet);
            }

            string errMsg = "";

            var command = new ConfigurationAutoMailCommand
            {
                report_id = Convert.ToDecimal(reportId),
                report_type = "D",
                ConfigurationQueryList = new List<ConfigurationQueryArrayModel>()
            };

            _saveCommand.Handle(command);
            if (command.return_code == 0)
            {
                saveStatus = "Y";
                errMsg = command.return_msg;
            }

            return Json(new { saveStatus, errMsg }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetScheduler(string type)
        {
            var data = new List<DropdownModel>
            {
                new DropdownModel {Value = "Daily", Text = "Daily"},
                new DropdownModel {Value = "Weekly",Text = "Weekly"},
                new DropdownModel {Value = "Monthly",Text = "Monthly"}
            };
            if (type == "SEARCH")
            {
                data.Insert(0, new DropdownModel { Value = "All", Text = "All" });
            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDayOfWeek(string type)
        {
            var data = new List<DropdownModel>
            {
                new DropdownModel {Value = "Monday", Text = "Monday"},
                new DropdownModel {Value = "Tuesday", Text = "Tuesday"},
                new DropdownModel {Value = "Wednesday", Text = "Wednesday"},
                new DropdownModel {Value = "Thursday", Text = "Thursday"},
                new DropdownModel {Value = "Friday", Text = "Friday"},
                new DropdownModel {Value = "Saturday", Text = "Saturday"},
                new DropdownModel {Value = "Sunday", Text = "Sunday"}
            };
            if (type == "SEARCH")
            {
                data.Insert(0, new DropdownModel { Value = "All", Text = "All" });
            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMonthOfYear(string type)
        {
            var data = new List<DropdownModel>
            {
                new DropdownModel {Value = "All", Text = "All"},
                new DropdownModel {Value = "January", Text = "January"},
                new DropdownModel {Value = "February", Text = "February"},
                new DropdownModel {Value = "March", Text = "March"},
                new DropdownModel {Value = "April", Text = "April"},
                new DropdownModel {Value = "May", Text = "May"},
                new DropdownModel {Value = "June", Text = "June"},
                new DropdownModel {Value = "July", Text = "July"},
                new DropdownModel {Value = "August", Text = "August"},
                new DropdownModel {Value = "September", Text = "September"},
                new DropdownModel {Value = "October", Text = "October"},
                new DropdownModel {Value = "November", Text = "November"},
                new DropdownModel {Value = "December", Text = "December"}
            };

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetEverydayOfMonth(string type)
        {
            var data = new List<DropdownModel>();

            for (int i = 1; i <= 31; i++)
            {
                data.Add(new DropdownModel { Value = i.ToString(), Text = i.ToString() });
            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ReportRead([DataSourceRequest] DataSourceRequest request, string dataS = "")
        {
            if (null == base.CurrentUser)
                return RedirectToAction("Login", "Fbbsaleportal");

            if (!string.IsNullOrEmpty(dataS))
            {
                var searchrptModel = new JavaScriptSerializer().Deserialize<GetConfigurationReportQuery>(dataS);
                var requestsort = request.Sorts.FirstOrDefault() ?? new SortDescriptor();
                searchrptModel.SortColumn = requestsort.Member;
                searchrptModel.SortBy = !string.IsNullOrEmpty(requestsort.Member) ? (requestsort.SortDirection == ListSortDirection.Ascending ? "asc" : "desc") : string.Empty;
                searchrptModel.PageNo = request.Page;
                searchrptModel.RecordsPerPage = request.PageSize;
                var reportdatas = GetDataReport(searchrptModel);
                request.Sorts = null;

                var result = reportdatas.ToDataSourceResult(request);
                result.Data = reportdatas;
                var reportTotal = reportdatas.FirstOrDefault();

                result.Total = reportTotal != null ? Convert.ToInt32(reportTotal.ALL_RECORDS) : 0;

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        public List<ConfigurationReportModel> GetDataReport(GetConfigurationReportQuery searchrptModel)
        {
            List<ConfigurationReportModel> listResult = _queryProcessor.Execute(searchrptModel);
            return listResult;
        }

        public ActionResult CheckQueryReport(string query, string owner)
        {
            string status;

            if (null == base.CurrentUser)
            {
                status = "T";
                return Json(new { status }, JsonRequestBehavior.AllowGet);
            }

            string errMsg = "";

            var checkQuery = new CheckQueryReportQuery
            {
                Query = query,
                Owner = owner
            };

            var check = _queryProcessor.Execute(checkQuery);
            status = check.Status;
            errMsg = check.Message;

            return Json(new { status, errMsg }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Resend(string reportId)
        {
            string status = "N";

            if (null == base.CurrentUser)
            {
                status = "T";
                return Json(new { status }, JsonRequestBehavior.AllowGet);
            }
            var configLov = base.LovData.Where(l => l.Type == "FBB_CONSTANT_SALEPORTAL" && l.Name == "Impersonate_saleportal" && l.ActiveFlag == "Y").FirstOrDefault();
            string errMsg = "";
            try
            {
                var query = new ReportAutoMailQuery()
                {
                    ReportId = reportId,
                    CreateBy = CurrentUser.UserName,
                    PathTempFile = configLov.LovValue4,
                    DomainTempFile = configLov.LovValue3,
                    PassTempFile = configLov.LovValue2,
                    UserTempFile = configLov.LovValue1
                };

                var check = _queryProcessor.Execute(query);
                if (check.ReturnCode == "0")
                    status = "Y";

                status = "Y";
            }
            catch (Exception)
            {
                errMsg = "Error";
            }

            return Json(new { status, errMsg }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CheckReportName(string reportId, string reportName)
        {
            string status = "Y";

            if (null == base.CurrentUser)
            {
                status = "T";
                return Json(new { status }, JsonRequestBehavior.AllowGet);
            }


            var searchrptModel = new GetConfigurationReportQuery
            {
                ReportName = reportName,
                PageNo = 1,
                RecordsPerPage = 20
            };

            var request = new DataSourceRequest();

            var reportdatas = GetDataReport(searchrptModel);
            var result = reportdatas.ToDataSourceResult(request);
            result.Data = reportdatas;
            var reportTotal = reportdatas.FirstOrDefault();

            result.Total = reportTotal != null ? Convert.ToInt32(reportTotal.ALL_RECORDS) : 0;

            if (result.Total > 0)
            {
                if (reportId != null && reportId != "0")
                {
                    foreach (var item in reportdatas)
                    {
                        if (item.REPORT_ID.ToString() != reportId)
                        {
                            status = "N";
                        }
                    }
                }
                else
                {
                    status = "N";
                }
            }

            return Json(new { status }, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}
