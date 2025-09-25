using System;
using System.Linq;
using System.Web.Mvc;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.ExWebServices;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBEntity.Extensions;
using WBBEntity.PanelModels;
using WBBEntity.PanelModels.FBBWebConfigModels;
using WBBWeb.Controllers.ConfigLovs;
using WBBWeb.Extension;
using WBBWeb.Models;

namespace WBBWeb.Controllers
{
    [CustomHandleError]
    [IENoCache]
    [AuthorizePaymentUserAttribute]
    [CustomActionFilter]
    public class ReportProblemController : WBBController
    {
        private readonly ICommandHandler<SessionLoginCommand> _sessionLoginCommand;
        private readonly IQueryProcessor _queryProcessor;
        private ConfigLovHelpers _configLovHelpers;
        private readonly ICommandHandler<ReportProblemCommand> _reportProblemCommand;

        string IdCard = "";
        string CardType = "";

        public ReportProblemController(IQueryProcessor queryProcessor, ILogger logger
            , ICommandHandler<SessionLoginCommand> sessionLoginCommand, ICommandHandler<ReportProblemCommand> reportProblemCommand)
        {
            _sessionLoginCommand = sessionLoginCommand;
            _queryProcessor = queryProcessor;
            _reportProblemCommand = reportProblemCommand;

            Logger = logger;
        }

        public ActionResult Index()
        {
            _configLovHelpers = new ConfigLovHelpers();

            ViewBag.LogoutController = ControllerContext.RouteData.Values["controller"].ToString();
            ViewBag.LabelFBBOR_MENU = _configLovHelpers.GetLovByTypePageCode(WebConstants.LovConfigName.LovTypeMenu, WebConstants.LovConfigName.PaymentPageCode);
            ViewBag.LabelFBBOR020 = _configLovHelpers.GetLovScreenByPageCode(WebConstants.LovConfigName.ReportProblemsPageCode);
            ViewBag.dropDownListProblemType = _configLovHelpers.GetLovDropdownListByType(WebConstants.LovConfigName.ProblemType).Select(x =>
                                  new SelectListItem
                                  {
                                      Text = x.Text,
                                      Value = x.Value
                                  }).ToList();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SendReport(ReportProblemCommand reportProblem)
        {
            _configLovHelpers = new ConfigLovHelpers();

            reportProblem.CustInternetNum = CurrentSessionPaymentProblemReport.CustInternetNum;
            reportProblem.CustIdCardType = CurrentSessionPaymentProblemReport.CustIdCardType;
            reportProblem.CustIdCardNum = CurrentSessionPaymentProblemReport.CustIdCardNum;

            reportProblem.ContactEmail = !string.IsNullOrEmpty(reportProblem.ContactEmail) ? reportProblem.ContactEmail.Trim() : string.Empty;
            reportProblem.ContactInfo = !string.IsNullOrEmpty(reportProblem.ContactInfo) ? reportProblem.ContactInfo.Trim() : string.Empty;
            reportProblem.ContactNumber = !string.IsNullOrEmpty(reportProblem.ContactNumber) ? reportProblem.ContactNumber.Trim() : string.Empty;
            reportProblem.ProblemDetails = !string.IsNullOrEmpty(reportProblem.ProblemDetails) ? reportProblem.ProblemDetails.Trim() : string.Empty;

            _reportProblemCommand.Handle(reportProblem);

            if (reportProblem.ReturnCode == "0")
            {
                return RedirectToAction("Success");
            }
            return RedirectToAction("Failed");
        }

        [HttpGet]
        public ActionResult Success()
        {
            _configLovHelpers = new ConfigLovHelpers();

            ViewBag.LogoutController = ControllerContext.RouteData.Values["controller"].ToString();
            ViewBag.LabelFBBOR_MENU = _configLovHelpers.GetLovByTypePageCode(WebConstants.LovConfigName.LovTypeMenu, WebConstants.LovConfigName.PaymentPageCode);
            ViewBag.LabelFBBOR020 = _configLovHelpers.GetLovScreenByPageCode(WebConstants.LovConfigName.ReportProblemsPageCode);

            return View();
        }

        [HttpGet]
        public ActionResult Failed()
        {
            _configLovHelpers = new ConfigLovHelpers();

            ViewBag.LogoutController = ControllerContext.RouteData.Values["controller"].ToString();
            ViewBag.LabelFBBOR_MENU = _configLovHelpers.GetLovByTypePageCode(WebConstants.LovConfigName.LovTypeMenu, WebConstants.LovConfigName.PaymentPageCode);
            ViewBag.LabelFBBOR020 = _configLovHelpers.GetLovScreenByPageCode(WebConstants.LovConfigName.ReportProblemsPageCode);
            return View();
        }

        [HttpGet]
        public ActionResult Login(string Data = "")
        {
            string custInternetNum = "";
            string custIdCardNum = "";
            string custIdCardType = "";
            string languagePage = "";
            bool CheckInput = true;
            string timeStamp = "";

            if (Data != "")
            {
                string DataDec = Decrypt(Data);
                string[] DataTemps = DataDec.Split('&');
                string GetIdCardStatus = "";

                if (DataTemps.Count() > 1)
                {
                    foreach (var item in DataTemps)
                    {
                        string[] DataTemp = item.Split('=');
                        if (DataTemp != null && DataTemp.Count() == 2)
                        {
                            if (DataTemp[0].ToSafeString() == "mobileNo")
                            {
                                custInternetNum = DataTemp[1].ToSafeString();
                            }
                            if (DataTemp[0].ToSafeString() == "lang")
                            {
                                languagePage = DataTemp[1].ToSafeString();
                                if (languagePage == "TH")
                                {
                                    ViewBag.LanguagePage = "1";
                                    languagePage = "1";
                                }
                                else
                                {
                                    ViewBag.LanguagePage = "2";
                                    languagePage = "2";
                                }
                            }
                            if (DataTemp[0].ToSafeString() == "timeStamp")
                            {
                                timeStamp = DataTemp[1].ToSafeString();
                            }
                        }
                        else
                        {
                            // value in put ไม่ถูกต้อง
                            CheckInput = false;
                            break;

                        }
                    }
                }
                else
                {
                    // value in put ไม่ถูกต้อง
                    CheckInput = false;

                }

                if (CheckInput)
                {
                    GetIdCardStatus = GetInfoByNonMobileNo(custInternetNum);
                    if (GetIdCardStatus == "")
                    {
                        custIdCardNum = IdCard;
                        custIdCardType = CardType;
                    }
                    else
                    {
                        // Login fail
                        custInternetNum = "";
                        custIdCardNum = "";
                        custIdCardType = "";
                    }
                }
                else
                {
                    // Login fail
                    custInternetNum = "";
                    custIdCardNum = "";
                    custIdCardType = "";
                }

            }

            string FullUrl = "";
            if (Session["FullUrl"] != null)
                FullUrl = Session["FullUrl"].ToSafeString();

            if (!string.IsNullOrEmpty(languagePage))
            {
                Session[WebConstants.SessionKeys.CurrentUICulture] = Convert.ToInt32(languagePage);
                SiteSession.CurrentUICulture = Convert.ToInt32(languagePage);
            }

            string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress))
            {
                ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
            string TransactionID = custInternetNum + ipAddress;

            InterfaceLogCommand log = null;
            //log = StartInterface("DataEncrypt: " + Data + "\r\n custInternetNum: " + custInternetNum + "\r\n Language: " + languagePage + "\r\n timeStamp: " + timeStamp, "/ReportProblem/Login", TransactionID, "", "ReportProblemLoginGET");

            // EndInterface("", log, TransactionID, "Success", "");

            _configLovHelpers = new ConfigLovHelpers();

            ViewBag.LabelFBBTR001 = _configLovHelpers.GetLovScreenByPageCode(WebConstants.LovConfigName.CoveragePageCode);
            ViewBag.LabelFBBTR003 = _configLovHelpers.GetLovScreenByPageCode(WebConstants.LovConfigName.CustomerRegisterPageCode);
            ViewBag.LabelFBBOR015 = _configLovHelpers.GetLovScreenByPageCode(WebConstants.LovConfigName.CheckPrePostPaid);
            ViewBag.LabelFBBOR019 = _configLovHelpers.GetLovScreenByPageCode(WebConstants.LovConfigName.PaymentPageCode);
            ViewBag.FbbConstant = _configLovHelpers.GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);

            if (string.IsNullOrEmpty(Data) || string.IsNullOrEmpty(custInternetNum)) return View();

            var errorLogin = string.Empty;

            string loginStatus = WebConstants.PaymentAndReport.LOGIN_PAYMENT_AND_REPORT_RESULT_FAIL;
            var sessionId = SessionManagement.GetSessionId();

            var user = (CurrentUser != null) ? CurrentUser.UserName : string.Empty;
            var query = new evESeServiceQueryMassCommonAccountInfoQuery
            {
                inOption = WebConstants.PaymentAndReport.LOGIN_PAYMENT_AND_REPORT_IN_OPTION,
                inMobileNo = custInternetNum,
                inCardNo = custIdCardNum,
                inCardType = custIdCardType,
                Page = WebConstants.PaymentAndReport.REPORT_PAGE_LOGIN,
                Username = user,
                FullUrl = FullUrl
            };

            var resultQueryMassCommonAccountInfo = _queryProcessor.Execute(query);

            if ((resultQueryMassCommonAccountInfo != null) &&
                (string.IsNullOrEmpty(resultQueryMassCommonAccountInfo.outErrorMessage)))
            {
                loginStatus = WebConstants.PaymentAndReport.LOGIN_PAYMENT_AND_REPORT_RESULT_SUCCESS;
            }

            if (loginStatus.Equals(WebConstants.PaymentAndReport.LOGIN_PAYMENT_AND_REPORT_RESULT_SUCCESS))
            {
                var requestCommand = new SessionLoginCommand
                {
                    CustInternetNum = custInternetNum,
                    SessionId = sessionId
                };
                _sessionLoginCommand.Handle(requestCommand);

                var sessionLogin = new LoginPaymentReportProblemModel
                {
                    CustInternetNum = custInternetNum,
                    CustIdCardType = custIdCardType,
                    CustIdCardNum = custIdCardNum,
                    SessionId = sessionId
                };
                CurrentSessionPaymentProblemReport = sessionLogin;

                ViewBag.ErrorLogin = errorLogin;
                //return Json(new { result = "Redirect", url = Url.Action("Index", "ReportProblem") });
                return RedirectToAction("Index", "ReportProblem");
            }

            errorLogin = "Login Fail";
            ViewBag.ErrorLogin = errorLogin;

            return View();
        }

        [HttpPost]
        //[AjaxValidateAntiForgeryToken]
        public ActionResult Login(string custInternetNum, string custIdCardNum, string custIdCardType, string languagePage, string Data = "")
        {
            bool CheckInput = true;
            string timeStamp = "";

            if (Data != "")
            {
                string DataDec = Decrypt(Data);
                string[] DataTemps = DataDec.Split('&');
                string GetIdCardStatus = "";

                if (DataTemps.Count() > 1)
                {
                    foreach (var item in DataTemps)
                    {
                        string[] DataTemp = item.Split('=');
                        if (DataTemp != null && DataTemp.Count() == 2)
                        {
                            if (DataTemp[0].ToSafeString() == "mobileNo")
                            {
                                custInternetNum = DataTemp[1].ToSafeString();
                            }
                            if (DataTemp[0].ToSafeString() == "lang")
                            {
                                languagePage = DataTemp[1].ToSafeString();
                                if (languagePage == "TH")
                                {
                                    ViewBag.LanguagePage = "1";
                                    languagePage = "1";
                                }
                                else
                                {
                                    ViewBag.LanguagePage = "2";
                                    languagePage = "2";
                                }
                            }
                            if (DataTemp[0].ToSafeString() == "timeStamp")
                            {
                                timeStamp = DataTemp[1].ToSafeString();
                            }
                        }
                        else
                        {
                            // value in put ไม่ถูกต้อง
                            CheckInput = false;
                            break;

                        }
                    }
                }
                else
                {
                    // value in put ไม่ถูกต้อง
                    CheckInput = false;

                }

                if (CheckInput)
                {
                    GetIdCardStatus = GetInfoByNonMobileNo(custInternetNum);
                    if (GetIdCardStatus == "")
                    {
                        custIdCardNum = IdCard;
                        custIdCardType = CardType;
                    }
                    else
                    {
                        // Login fail
                        custInternetNum = "";
                        custIdCardNum = "";
                        custIdCardType = "";
                    }
                }

            }

            string FullUrl = "";
            if (Session["FullUrl"] != null)
                FullUrl = Session["FullUrl"].ToSafeString();

            if (!string.IsNullOrEmpty(languagePage))
            {
                Session[WebConstants.SessionKeys.CurrentUICulture] = Convert.ToInt32(languagePage);
                SiteSession.CurrentUICulture = Convert.ToInt32(languagePage);
            }

            string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress))
            {
                ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
            string TransactionID = custInternetNum + ipAddress;

            InterfaceLogCommand log = null;
            //log = StartInterface("DataEncrypt: " + Data + "\r\n custInternetNum: " + custInternetNum + "\r\n Language: " + languagePage + "\r\n timeStamp: " + timeStamp, "/ReportProblem/Login", TransactionID, "", "ReportProblemLoginPOST");

            //EndInterface("", log, TransactionID, "Success", "");

            _configLovHelpers = new ConfigLovHelpers();

            ViewBag.LabelFBBTR001 = _configLovHelpers.GetLovScreenByPageCode(WebConstants.LovConfigName.CoveragePageCode);
            ViewBag.LabelFBBTR003 = _configLovHelpers.GetLovScreenByPageCode(WebConstants.LovConfigName.CustomerRegisterPageCode);
            ViewBag.LabelFBBOR015 = _configLovHelpers.GetLovScreenByPageCode(WebConstants.LovConfigName.CheckPrePostPaid);
            ViewBag.LabelFBBOR019 = _configLovHelpers.GetLovScreenByPageCode(WebConstants.LovConfigName.PaymentPageCode);
            ViewBag.FbbConstant = _configLovHelpers.GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);


            if (string.IsNullOrEmpty(custInternetNum)) return View();

            var errorLogin = string.Empty;

            string loginStatus = WebConstants.PaymentAndReport.LOGIN_PAYMENT_AND_REPORT_RESULT_FAIL;
            var sessionId = SessionManagement.GetSessionId();

            var user = (CurrentUser != null) ? CurrentUser.UserName : string.Empty;
            var query = new evESeServiceQueryMassCommonAccountInfoQuery
            {
                inOption = WebConstants.PaymentAndReport.LOGIN_PAYMENT_AND_REPORT_IN_OPTION,
                inMobileNo = custInternetNum,
                inCardNo = custIdCardNum,
                inCardType = custIdCardType,
                Page = WebConstants.PaymentAndReport.REPORT_PAGE_LOGIN,
                Username = user,
                FullUrl = FullUrl
            };

            var resultQueryMassCommonAccountInfo = _queryProcessor.Execute(query);

            if ((resultQueryMassCommonAccountInfo != null) &&
                (string.IsNullOrEmpty(resultQueryMassCommonAccountInfo.outErrorMessage)))
            {
                loginStatus = WebConstants.PaymentAndReport.LOGIN_PAYMENT_AND_REPORT_RESULT_SUCCESS;
            }

            if (loginStatus.Equals(WebConstants.PaymentAndReport.LOGIN_PAYMENT_AND_REPORT_RESULT_SUCCESS))
            {
                var requestCommand = new SessionLoginCommand
                {
                    CustInternetNum = custInternetNum,
                    SessionId = sessionId
                };
                _sessionLoginCommand.Handle(requestCommand);

                var sessionLogin = new LoginPaymentReportProblemModel
                {
                    CustInternetNum = custInternetNum,
                    CustIdCardType = custIdCardType,
                    CustIdCardNum = custIdCardNum,
                    SessionId = sessionId
                };
                CurrentSessionPaymentProblemReport = sessionLogin;

                ViewBag.ErrorLogin = errorLogin;
                if (Data != "")
                    return RedirectToAction("Index", "ReportProblem");
                return Json(new { result = "Redirect", url = Url.Action("Index", "ReportProblem") });
            }

            errorLogin = "Login Fail";
            ViewBag.ErrorLogin = errorLogin;

            if (Data != "")
                return View();
            return Json(
                new
                {
                    data = new
                    {
                        result = errorLogin,
                        url = string.Empty
                    }
                }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Logout()
        {
            try
            {
                string languagePage = (Session[WebConstants.SessionKeys.CurrentUICulture] ?? 0).ToString();
                var sessionManagement = new SessionManagement();
                sessionManagement.ClearAllSession();
                sessionManagement.SetNewSessionId();
                if (!string.IsNullOrEmpty(languagePage))
                {
                    Session[WebConstants.SessionKeys.CurrentUICulture] = Convert.ToInt32(languagePage);
                    SiteSession.CurrentUICulture = Convert.ToInt32(languagePage);
                }
            }
            catch (Exception ex)
            {
                CurrentSessionPaymentProblemReport = null;
                Logger.Info(ex.GetErrorMessage());
            }
            return RedirectToAction("Login", "ReportProblem");
        }

        [HttpGet]
        public ActionResult LoginConcurrent()
        {
            _configLovHelpers = new ConfigLovHelpers();

            ViewBag.LogoutController = ControllerContext.RouteData.Values["controller"].ToString();
            ViewBag.LabelFBBOR_MENU = _configLovHelpers.GetLovByTypePageCode(WebConstants.LovConfigName.LovTypeMenu, WebConstants.LovConfigName.PaymentPageCode);
            ViewBag.LabelFBBOR019 = _configLovHelpers.GetLovScreenByPageCode(WebConstants.LovConfigName.PaymentPageCode);
            ViewBag.LanguagePage = Session[WebConstants.SessionKeys.CurrentUICulture].ToString();

            return View();
        }

        [HttpGet]
        public ActionResult SessionTimeout()
        {
            _configLovHelpers = new ConfigLovHelpers();

            ViewBag.LogoutController = ControllerContext.RouteData.Values["controller"].ToString();
            ViewBag.LabelFBBOR_MENU = _configLovHelpers.GetLovByTypePageCode(WebConstants.LovConfigName.LovTypeMenu, WebConstants.LovConfigName.PaymentPageCode);
            ViewBag.LabelFBBOR019 = _configLovHelpers.GetLovScreenByPageCode(WebConstants.LovConfigName.PaymentPageCode);
            ViewBag.LanguagePage = Session[WebConstants.SessionKeys.CurrentUICulture].ToString();

            return View();
        }

        [HttpGet]
        public ActionResult RedirectPage()
        {
            var url = "https://www.ais.co.th/fibre/";
            try
            {
                _configLovHelpers = new ConfigLovHelpers();
                var lovlist = _configLovHelpers.GetLovByTypePageCode(WebConstants.LovConfigName.LovTypeMenu, WebConstants.LovConfigName.PaymentPageCode);
                var lovScreenValueModel = lovlist.FirstOrDefault(item => item.Name == "L_MENU_FIRST_PAGE_URL") ?? new LovScreenValueModel();
                url = !string.IsNullOrEmpty(lovScreenValueModel.DisplayValue) ? lovScreenValueModel.DisplayValue : url;

                var sessionManagement = new SessionManagement();
                sessionManagement.ClearAllSession();
                sessionManagement.SetNewSessionId();
            }
            catch (Exception ex)
            {
                CurrentSessionPaymentProblemReport = null;
                Logger.Info(ex.GetErrorMessage());
            }
            return Redirect(url);
        }

        private string GetInfoByNonMobileNo(string NonMobileNo = "")
        {
            string FullUrl = "";
            if (Session["FullUrl"] != null)
                FullUrl = Session["FullUrl"].ToSafeString();

            #region Get IP Address Interface Log (Update 17.2)

            // Get IP Address
            string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress)) ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            #endregion

            string user = "";
            string InOption = "2";
            if (base.CurrentUser != null) user = base.CurrentUser.UserName;

            var query = new evESeServiceQueryMassCommonAccountInfoQuery
            {
                inOption = InOption,
                inMobileNo = NonMobileNo,
                Page = "Check SearchProfilePrePostpaid",
                Username = user,
                ClientIP = ipAddress,
                FullUrl = FullUrl
            };
            var a = _queryProcessor.Execute(query);
            if (a != null)
            {
                var CustomerData = GetCustomerInfo(a.outAccountNumber);
                if (CustomerData != null && !string.IsNullOrEmpty(CustomerData.idCardNum) && !string.IsNullOrEmpty(CustomerData.idCardType))
                {
                    IdCard = CustomerData.idCardNum;
                    CardType = CustomerData.idCardType;

                    return "";
                }
                else
                {
                    IdCard = "";
                    CardType = "";
                    return "NodataCustomer";
                }
            }
            else
            {
                return "NodataCustomer";
            }
        }

        private evAMQueryCustomerInfoModel GetCustomerInfo(string accntNo = "")
        {
            var query = new evAMQueryCustomerInfoQuery
            {
                accntNo = accntNo
            };
            evAMQueryCustomerInfoModel a = _queryProcessor.Execute(query);

            return a;
        }

    }
}