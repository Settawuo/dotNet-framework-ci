using log4net;
using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Commands.WebServices;
using WBBContract.Queries.ExWebServices;
using WBBContract.Queries.WebServices;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels;
using WBBEntity.PanelModels.ExWebServiceModels;
using WBBEntity.PanelModels.WebServiceModels;
using WBBWeb.Controllers.ConfigLovs;
using WBBWeb.Extension;
using WBBWeb.Models;

namespace WBBWeb.Controllers
{
    [CustomHandleError]
    [IENoCache]
    [AuthorizePaymentUserAttribute]
    [CustomActionFilter]
    public class PaymentController : WBBController
    {
        private readonly ICommandHandler<SessionLoginCommand> _sessionLoginCommand;
        private readonly ICommandHandler<PaymentLogCommand> _paymentLogCommandHandler;
        private readonly IQueryProcessor _queryProcessor;
        private ConfigLovHelpers _configLovHelpers;
        private readonly ILog _logPayment = LogManager.GetLogger("RollingPayment");
        private readonly ICommandHandler<InterfaceLogCommand> _intfLogCommand;
        private readonly ICommandHandler<SaveDeductionLogCommand> _saveDeductionLogCommand;

        public PaymentController(IQueryProcessor queryProcessor,
             ILogger logger, ICommandHandler<SessionLoginCommand> sessionLoginCommand, ICommandHandler<PaymentLogCommand> paymentLogCommandHandler,
             ICommandHandler<InterfaceLogCommand> intfLogCommand,
             ICommandHandler<SaveDeductionLogCommand> saveDeductionLogCommand)
        {
            _queryProcessor = queryProcessor;
            _sessionLoginCommand = sessionLoginCommand;
            _paymentLogCommandHandler = paymentLogCommandHandler;
            Logger = logger;
            _intfLogCommand = intfLogCommand;
            _saveDeductionLogCommand = saveDeductionLogCommand;
        }

        public ActionResult Index()
        {
            _configLovHelpers = new ConfigLovHelpers();

            ViewBag.LogoutController = ControllerContext.RouteData.Values["controller"].ToString();
            ViewBag.LabelFBBOR_MENU = _configLovHelpers.GetLovByTypePageCode(WebConstants.LovConfigName.LovTypeMenu, WebConstants.LovConfigName.PaymentPageCode);
            ViewBag.LabelFBBOR019 = _configLovHelpers.GetLovScreenByPageCode(WebConstants.LovConfigName.PaymentPageCode);



            //log 
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            var paymentLog = new FBB_PAYMENT_LOG
            {
                EvenName = WebConstants.PaymentAndReport.LOG_EVENT_NAME_CHECKBALANCE,
                TransactionId = CurrentSessionPaymentProblemReport.TransactionId,
                RequestTime = DateTime.Now.ToString(WebConstants.PaymentAndReport.LOG_DATETIME_FORMAT),
                ServiceName = WebConstants.PaymentAndReport.LOG_SERVICE_NAME_A_LSTPMMOBDTL,
                InternetNumber = CurrentSessionPaymentProblemReport.CustInternetNum,
                Status = string.Empty
            };
            var LOV_OTSDB = base.LovData.Where(i => i.Type == "CONFIG_OUTSANDINGBAL" && i.Name == "OUTSANDINGBAL_FLAG" && i.ActiveFlag == "Y").FirstOrDefault().LovValue1;
            string LOVFLAG = !string.IsNullOrEmpty(LOV_OTSDB) ? LOV_OTSDB : "";
            PmModleDetailResponse result = new PmModleDetailResponse();
            if (LOVFLAG == "Y") //chita571 19/09/24
            {
                GetOutstandingBalanceQuery balanceQuery = new GetOutstandingBalanceQuery()
                {
                    InternetNo = CurrentSessionPaymentProblemReport.CustInternetNum
                };
                result = _queryProcessor.Execute(balanceQuery);
            }
            else
            {
                var pmMobileDetialQuery = new GetListPmMobileDetialQuery
                {
                    InternetNo = CurrentSessionPaymentProblemReport.CustInternetNum
                };
                result = _queryProcessor.Execute(pmMobileDetialQuery);
            }


            //log login response
            paymentLog.ResponseTime = DateTime.Now.ToString(WebConstants.PaymentAndReport.LOG_DATETIME_FORMAT);

            if (result.StatusCode == "0")
            {
                result.DueDate = DataConvertHelpers.To_FormatDD_MM_YYYY(result.DueDate);

                // TODO: add to sesstion
                CurrentSessionPayment = new PaymentLogCommand
                {
                    CustInternetNum = CurrentSessionPaymentProblemReport.CustInternetNum,
                    CustIdCardType = CurrentSessionPaymentProblemReport.CustIdCardType,
                    CustIdCardNum = CurrentSessionPaymentProblemReport.CustIdCardNum,
                    DueDate = result.DueDate,
                    RequestParamAmount = DataConvertHelpers.ToStrCurrencyFormat(result.TotalBalance)
                };

                result.TransactionId = CurrentSessionPaymentProblemReport.TransactionId;
                result.InternetNo = CurrentSessionPaymentProblemReport.CustInternetNum;

                paymentLog.Status = WebConstants.PaymentAndReport.LOG_STATUS_SUCCESS;
                paymentLog.Duedate = CurrentSessionPayment.DueDate;
                paymentLog.Amount = CurrentSessionPayment.RequestParamAmount;

                //Write Log
                WriteLogPayment(paymentLog);

                return View(result);
            }
            paymentLog.Status = WebConstants.PaymentAndReport.LOG_STATUS_FAILED;
            paymentLog.Description = result.StatusMessage;

            //Write Log
            WriteLogPayment(paymentLog);

            return RedirectToAction("Failed");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogPayment(string mode, string paymentMethod, string merchantId, string serviceId, string ref1, string ref2, string amount, string requestUrl)
        {
            var logUrlRequestmPay = string.Format("{0}?mode={1}&paymentMethod={2}&serviceId={3}&ref1={4}&ref2={5}&amount={6}",
                                                                   requestUrl, mode, paymentMethod, serviceId, ref1, ref2, amount);
            var logUrlRequestmCredit = string.Format("{0}?mode={1}&merchantId={2}&serviceId={3}&ref1={4}&ref2={5}&amount={6}",
                                                                  requestUrl, mode, merchantId, serviceId, ref1, ref2, amount);
            //log db
            var requestCommand = new PaymentLogCommand
            {
                PaymentId = null,
                SessionId = CurrentSessionPaymentProblemReport.SessionId,
                Browser =
                    string.Format("{0} v.{1}", HttpContext.Request.Browser.Browser, HttpContext.Request.Browser.Version),
                CustInternetNum = CurrentSessionPaymentProblemReport.CustInternetNum,
                CustIdCardType = CurrentSessionPaymentProblemReport.CustIdCardType,
                CustIdCardNum = CurrentSessionPaymentProblemReport.CustIdCardNum,
                DueDate = CurrentSessionPayment.DueDate,
                RequestParamMode = mode,
                RequestParamPaymentMethod = paymentMethod,
                RequestParamMerchantId = merchantId,
                RequestParamServiceId = serviceId,
                RequestParamRef1 = ref1,
                RequestParamRef2 = ref2,
                RequestParamAmount = amount,
                RequestUrl = mode.ToUpper() == "START" ? logUrlRequestmPay : logUrlRequestmCredit,
                RequestDatetime = DateTime.Now,
                Status = string.Empty
            };
            _paymentLogCommandHandler.Handle(requestCommand);

            CurrentSessionPayment.PaymentId = requestCommand.PaymentId;

            //Write Log file
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            var paymentLog = new FBB_PAYMENT_LOG
            {
                //EVENT_NAME|TSID|INTERNET_NUM|REQ_TIME|REQUEST|URL_REQUST
                EvenName = mode.ToUpper() == "START" ? WebConstants.PaymentAndReport.LOG_EVENT_NAME_PAYMENTMPAY
                                                    : WebConstants.PaymentAndReport.LOG_EVENT_NAME_PAYMENTCREDIT,
                TransactionId = CurrentSessionPaymentProblemReport.TransactionId,
                InternetNumber = CurrentSessionPaymentProblemReport.CustInternetNum,
                RequestTime = DateTime.Now.ToString(WebConstants.PaymentAndReport.LOG_DATETIME_FORMAT),
                Request = WebConstants.PaymentAndReport.LOG_TYPE_REQUEST,
                UrlRequest = mode.ToUpper() == "START" ? logUrlRequestmPay : logUrlRequestmCredit,
                Status = WebConstants.PaymentAndReport.LOG_TYPE_REQUEST
            };
            WriteLogPayment(paymentLog);

            return Json("0");
        }

        public ActionResult WaitResponse(string mobileNo, string amount, string receiptNum, string ref1, string txId, string urlForward, string sessRef1
            , string status, string responseCode, string responseMsg, string balance, string validity, string ref2)
        {

            var logResponseParametermPay = string.Format("moblieNo={0}&amount={1}&receiptNum={2}" +
                                                             "&Ref1={3}&txId={4}&sessRef1={5}&urlForward={6}",
                                                                   mobileNo, amount, receiptNum
                                                                   , ref1, txId, sessRef1, urlForward);
            var logResponseParameterCredit = string.Format("status={0}&responseCode={1}&responseMsg={2}&balance={3}&validity={4}&amount={5}" +
                                                             "&mobileNo={6}&Ref1={7}&Ref2={8}&txId={9}&receiptNum={10}&urlForward={11}",
                                                                  status, responseCode, responseMsg, balance, validity, amount
                                                                  , mobileNo, ref1, ref2, txId, receiptNum, urlForward);


            //response Credit
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

            var paymentCommand = new PaymentLogCommand
            {
                PaymentId = CurrentSessionPayment.PaymentId,
                ResponseUrl = HttpContext.Request.UrlReferrer != null ? DataConvertHelpers.ToStr(HttpContext.Request.UrlReferrer.ToString()) : string.Empty,
                Status = DataConvertHelpers.ToStr(status),
                ResponseCode = DataConvertHelpers.ToStr(responseCode),
                ResponseMsg = DataConvertHelpers.ToStr(responseMsg),
                ResponseBalance = DataConvertHelpers.ToStr(balance),
                ResponseValidity = DataConvertHelpers.ToStr(validity),
                ResponseMoblieNo = DataConvertHelpers.ToStr(mobileNo),
                ResponseAmount = !string.IsNullOrEmpty(DataConvertHelpers.ToStr(amount)) ? DataConvertHelpers.ToStr(amount) : CurrentSessionPayment.RequestParamAmount,
                ResponseReceiptNum = DataConvertHelpers.ToStr(receiptNum),
                ResponseRef1 = DataConvertHelpers.ToStr(ref1),
                ResponseTxid = DataConvertHelpers.ToStr(txId),
                ResponseUrlForward = DataConvertHelpers.ToStr(urlForward),
                ResponseSessRef1 = DataConvertHelpers.ToStr(sessRef1),
                ResponseRef2 = DataConvertHelpers.ToStr(ref2),
                ResponseDatetime = DateTime.Now,
                CustInternetNum = CurrentSessionPayment.CustInternetNum
            };

            //EVENT_NAME|TSID|INTERNET_NUM|RESP_TIME|RESPONSE|STATUS|URL_REF
            //log 
            var paymentLog = new FBB_PAYMENT_LOG
            {
                TransactionId = CurrentSessionPaymentProblemReport.TransactionId,
                ResponseTime = DateTime.Now.ToString(WebConstants.PaymentAndReport.LOG_DATETIME_FORMAT),
                Response = WebConstants.PaymentAndReport.LOG_TYPE_RESPONSE,
                Status = WebConstants.PaymentAndReport.LOG_STATUS_FAILED,
                UrlReference = paymentCommand.ResponseUrl,
            };

            if (string.IsNullOrEmpty(paymentCommand.Status)
                && string.IsNullOrEmpty(paymentCommand.ResponseCode)
                && string.IsNullOrEmpty(paymentCommand.ResponseMsg)
                && !string.IsNullOrEmpty(paymentCommand.ResponseSessRef1))

            {
                // TODO: Payment mPay
                if (!string.IsNullOrEmpty(paymentCommand.ResponseMoblieNo)
                    && !string.IsNullOrEmpty(paymentCommand.ResponseAmount)
                    && !string.IsNullOrEmpty(paymentCommand.ResponseReceiptNum)
                    && !string.IsNullOrEmpty(paymentCommand.ResponseRef1)
                    && !string.IsNullOrEmpty(paymentCommand.ResponseTxid)
                    && !string.IsNullOrEmpty(paymentCommand.ResponseSessRef1)
                    && paymentCommand.CustInternetNum == paymentCommand.ResponseRef1)
                {
                    paymentCommand.Status = WebConstants.PaymentAndReport.MPAY_STATUS_SUCCESS;
                    paymentLog.Status = WebConstants.PaymentAndReport.LOG_STATUS_SUCCESS;
                }
                paymentLog.EvenName = WebConstants.PaymentAndReport.LOG_EVENT_NAME_PAYMENTMPAY;
                paymentLog.InternetNumber = !string.IsNullOrEmpty(paymentCommand.ResponseRef1)
                                                ? paymentCommand.ResponseRef1
                                                : paymentCommand.ResponseSessRef1;

                paymentLog.ResponseParameter = logResponseParametermPay;
            }
            else
            {
                // TODO: Payment Credit
                switch (paymentCommand.Status.ToLower())
                {
                    case WebConstants.PaymentAndReport.MPAY_STATUS_SUCCESS:
                        paymentCommand.Status = WebConstants.PaymentAndReport.MPAY_STATUS_SUCCESS;
                        paymentLog.Status = WebConstants.PaymentAndReport.LOG_STATUS_SUCCESS;
                        break;
                    case WebConstants.PaymentAndReport.MPAY_STATUS_FAILED:
                        paymentCommand.Status = WebConstants.PaymentAndReport.MPAY_STATUS_FAILED;
                        break;
                }
                paymentLog.EvenName = WebConstants.PaymentAndReport.LOG_EVENT_NAME_PAYMENTCREDIT;
                paymentLog.InternetNumber = !string.IsNullOrEmpty(paymentCommand.ResponseRef1)
                                                ? paymentCommand.ResponseRef1
                                                : paymentCommand.CustInternetNum;

                paymentLog.ResponseParameter = logResponseParameterCredit;

            }

            CurrentSessionPayment.ResponseUrl = paymentCommand.ResponseUrl;
            CurrentSessionPayment.Status = paymentCommand.Status;
            CurrentSessionPayment.ResponseCode = paymentCommand.ResponseCode;
            CurrentSessionPayment.ResponseMsg = paymentCommand.ResponseMsg;
            CurrentSessionPayment.ResponseBalance = paymentCommand.ResponseBalance;
            CurrentSessionPayment.ResponseValidity = paymentCommand.ResponseValidity;
            CurrentSessionPayment.ResponseMoblieNo = paymentCommand.ResponseMoblieNo;
            CurrentSessionPayment.ResponseAmount = paymentCommand.ResponseAmount;
            CurrentSessionPayment.ResponseReceiptNum = paymentCommand.ResponseReceiptNum;
            CurrentSessionPayment.ResponseRef1 = paymentCommand.ResponseRef1;
            CurrentSessionPayment.ResponseTxid = paymentCommand.ResponseTxid;
            CurrentSessionPayment.ResponseUrlForward = paymentCommand.ResponseUrlForward;
            CurrentSessionPayment.ResponseSessRef1 = paymentCommand.ResponseSessRef1;
            CurrentSessionPayment.ResponseRef2 = paymentCommand.ResponseRef2;
            CurrentSessionPayment.ResponseDatetime = paymentCommand.ResponseDatetime;
            CurrentSessionPayment.Status = paymentCommand.Status;

            // TODO: write log DB
            PaymentLogging(paymentCommand);

            //Write Log file
            WriteLogPayment(paymentLog);


            if (paymentCommand.Status == WebConstants.PaymentAndReport.MPAY_STATUS_SUCCESS)
            {
                return RedirectToAction("Success");
            }

            //else if (paymentCommand.Status == WebConstants.PaymentAndReport.MPAY_STATUS_FAILED)
            //{
            //    return RedirectToAction("Failed");
            //}

            return RedirectToAction("Index");
        }

        public ActionResult Success()
        {
            _configLovHelpers = new ConfigLovHelpers();

            ViewBag.LogoutController = ControllerContext.RouteData.Values["controller"].ToString();
            ViewBag.LabelFBBOR_MENU = _configLovHelpers.GetLovByTypePageCode(WebConstants.LovConfigName.LovTypeMenu, WebConstants.LovConfigName.PaymentPageCode);
            ViewBag.LabelFBBOR019 = _configLovHelpers.GetLovScreenByPageCode(WebConstants.LovConfigName.PaymentPageCode);

            var paymentLog = CurrentSessionPayment;

            return View(paymentLog);
        }

        public ActionResult Failed()
        {
            _configLovHelpers = new ConfigLovHelpers();

            ViewBag.LogoutController = ControllerContext.RouteData.Values["controller"].ToString();
            ViewBag.LabelFBBOR_MENU = _configLovHelpers.GetLovByTypePageCode(WebConstants.LovConfigName.LovTypeMenu, WebConstants.LovConfigName.PaymentPageCode);
            ViewBag.LabelFBBOR019 = _configLovHelpers.GetLovScreenByPageCode(WebConstants.LovConfigName.PaymentPageCode);

            return View();
        }


        private void PaymentLogging(PaymentLogCommand paymentCommand)
        {
            if (paymentCommand.PaymentId == null) return;

            var requestCommand = new PaymentLogCommand
            {
                PaymentId = paymentCommand.PaymentId,
                SessionId = paymentCommand.SessionId,
                CustInternetNum = paymentCommand.CustInternetNum,
                Status = paymentCommand.Status,
                ResponseCode = paymentCommand.ResponseCode,
                ResponseMsg = paymentCommand.ResponseMsg,
                ResponseBalance = paymentCommand.ResponseBalance,
                ResponseValidity = paymentCommand.ResponseValidity,
                ResponseUrlForward = paymentCommand.ResponseUrlForward,
                ResponseMoblieNo = paymentCommand.ResponseMoblieNo,
                ResponseAmount = paymentCommand.ResponseAmount,
                ResponseReceiptNum = paymentCommand.ResponseReceiptNum,
                ResponseRef1 = paymentCommand.ResponseRef1,
                ResponseRef2 = paymentCommand.ResponseRef2,
                ResponseTxid = paymentCommand.ResponseTxid,
                ResponseSessRef1 = paymentCommand.ResponseSessRef1,
                ResponseDatetime = paymentCommand.ResponseDatetime,
                ResponseUrl = paymentCommand.ResponseUrl
            };
            _paymentLogCommandHandler.Handle(requestCommand);
        }

        [HttpGet]
        public ActionResult Login()
        {
            _configLovHelpers = new ConfigLovHelpers();

            ViewBag.LabelFBBTR001 = _configLovHelpers.GetLovScreenByPageCode(WebConstants.LovConfigName.CoveragePageCode);
            ViewBag.LabelFBBTR003 = _configLovHelpers.GetLovScreenByPageCode(WebConstants.LovConfigName.CustomerRegisterPageCode);
            ViewBag.LabelFBBOR015 = _configLovHelpers.GetLovScreenByPageCode(WebConstants.LovConfigName.CheckPrePostPaid);
            ViewBag.LabelFBBOR019 = _configLovHelpers.GetLovScreenByPageCode(WebConstants.LovConfigName.PaymentPageCode);
            ViewBag.FbbConstant = _configLovHelpers.GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);
            return View();
        }

        [HttpPost]
        [AjaxValidateAntiForgeryToken]
        public ActionResult Login(string custInternetNum, string custIdCardNum, string custIdCardType, string languagePage)
        {
            string FullUrl = "";
            if (Session["FullUrl"] != null)
                FullUrl = Session["FullUrl"].ToSafeString();

            if (!string.IsNullOrEmpty(languagePage))
            {
                Session[WebConstants.SessionKeys.CurrentUICulture] = Convert.ToInt32(languagePage);
                SiteSession.CurrentUICulture = Convert.ToInt32(languagePage);
            }

            _configLovHelpers = new ConfigLovHelpers();
            ViewBag.LabelFBBTR001 = _configLovHelpers.GetLovScreenByPageCode(WebConstants.LovConfigName.CoveragePageCode);
            ViewBag.LabelFBBTR003 =
                _configLovHelpers.GetLovScreenByPageCode(WebConstants.LovConfigName.CustomerRegisterPageCode);
            ViewBag.LabelFBBOR015 = _configLovHelpers.GetLovScreenByPageCode(WebConstants.LovConfigName.CheckPrePostPaid);
            ViewBag.LabelFBBOR019 = _configLovHelpers.GetLovScreenByPageCode(WebConstants.LovConfigName.PaymentPageCode);
            ViewBag.FbbConstant = _configLovHelpers.GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);

            if (string.IsNullOrEmpty(custInternetNum) || string.IsNullOrEmpty(custIdCardNum) || string.IsNullOrEmpty(custIdCardType)) return View();

            var errorLogin = string.Empty;

            //string loginStatus = WebConstants.PaymentAndReport.LOGIN_PAYMENT_AND_REPORT_RESULT_FAIL;
            var sessionId = SessionManagement.GetSessionId();

            var user = (CurrentUser != null) ? CurrentUser.UserName : string.Empty;
            var query = new evESeServiceQueryMassCommonAccountInfoQuery
            {
                inOption = WebConstants.PaymentAndReport.LOGIN_PAYMENT_AND_REPORT_IN_OPTION,
                inMobileNo = custInternetNum,
                inCardNo = custIdCardNum,
                inCardType = custIdCardType,
                Page = WebConstants.PaymentAndReport.PAYMENT_PAGE_LOGIN,
                Username = user,
                FullUrl = FullUrl
            };

            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

            var transactionId = custInternetNum + DateTime.Now.ToString(WebConstants.PaymentAndReport.TRANSACTION_ID_DATETIME_FORMAT);

            //log login 
            var paymentLog = new FBB_PAYMENT_LOG
            {
                EvenName = WebConstants.PaymentAndReport.LOG_EVENT_NAME_LOGIN,
                TransactionId = transactionId,
                RequestTime = DateTime.Now.ToString(WebConstants.PaymentAndReport.LOG_DATETIME_FORMAT),
                ServiceName = WebConstants.PaymentAndReport.LOG_SERVICE_NAME_ESERVICE_QUERY_MASS_COMMON_ACCOUNT_INFO,
                InternetNumber = custInternetNum,
                IdCardNumber = custIdCardNum,
                CardType = custIdCardType,
                Status = string.Empty
            };

            var resultQueryMassCommonAccountInfo = _queryProcessor.Execute(query);

            //log login response
            paymentLog.ResponseTime = DateTime.Now.ToString(WebConstants.PaymentAndReport.LOG_DATETIME_FORMAT);

            if ((resultQueryMassCommonAccountInfo != null) &&
                (string.IsNullOrEmpty(resultQueryMassCommonAccountInfo.outErrorMessage)))
            {

                paymentLog.Status = WebConstants.PaymentAndReport.LOG_STATUS_SUCCESS;
                var requestCommand = new SessionLoginCommand
                {
                    CustInternetNum = custInternetNum,
                    SessionId = sessionId
                };
                _sessionLoginCommand.Handle(requestCommand);

                //Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
                var sessionLogin = new LoginPaymentReportProblemModel
                {
                    CustInternetNum = custInternetNum,
                    CustIdCardType = custIdCardType,
                    CustIdCardNum = custIdCardNum,
                    SessionId = sessionId,
                    TransactionId = transactionId
                };
                CurrentSessionPaymentProblemReport = sessionLogin;
                ViewBag.ErrorLogin = errorLogin;

                //Write Log
                WriteLogPayment(paymentLog);

                return Json(new { result = "Redirect", url = Url.Action("Index", "Payment") });

                //loginStatus = WebConstants.PaymentAndReport.LOGIN_PAYMENT_AND_REPORT_RESULT_SUCCESS;
            }


            errorLogin = "Login Fail";
            ViewBag.ErrorLogin = errorLogin;
            paymentLog.Status = WebConstants.PaymentAndReport.LOG_STATUS_FAILED;
            paymentLog.Description = resultQueryMassCommonAccountInfo != null
                ? resultQueryMassCommonAccountInfo.outErrorMessage
                : null;

            WriteLogPayment(paymentLog);

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
            return RedirectToAction("Login", "Payment");
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

        //private string CreateToken(string message, string secret)
        //{
        //    secret = secret ?? "";
        //    var encoding = new System.Text.ASCIIEncoding();
        //    byte[] keyByte = encoding.GetBytes(secret);
        //    byte[] messageBytes = encoding.GetBytes(message);
        //    using (var hmacsha256 = new HMACSHA256(keyByte))
        //    {
        //        byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
        //        return Convert.ToBase64String(hashmessage);
        //    }
        //}

        //private void SetSessionPayment(HttpContextBase context)
        //{
        //    CurrentSessionPayment.ResponseUrlForward = context.Request.Url != null ? DataConvertHelpers.ToStr(context.Request.Url.AbsoluteUri) : string.Empty;
        //    CurrentSessionPayment.ResponseMoblieNo = DataConvertHelpers.ToStr(context.Request.QueryString["mobileNo"]);
        //    CurrentSessionPayment.ResponseAmount = DataConvertHelpers.ToStr(context.Request.QueryString["amount"]);
        //    CurrentSessionPayment.ResponseReceiptNum = DataConvertHelpers.ToStr(context.Request.QueryString["receiptNum"]);
        //    CurrentSessionPayment.ResponseRef1 = DataConvertHelpers.ToStr(context.Request.QueryString["ref1"]);
        //    CurrentSessionPayment.ResponseTxid = DataConvertHelpers.ToStr(context.Request.QueryString["txId"]);
        //    CurrentSessionPayment.ResponseSessRef1 = DataConvertHelpers.ToStr(context.Request.QueryString["sessRef1"]);
        //    CurrentSessionPayment.ResponseDatetime = DateTime.Now;

        //    //validate response status
        //    if (!string.IsNullOrEmpty(CurrentSessionPayment.ResponseMoblieNo)
        //        && !string.IsNullOrEmpty(CurrentSessionPayment.ResponseAmount)
        //        && !string.IsNullOrEmpty(CurrentSessionPayment.ResponseReceiptNum)
        //        && !string.IsNullOrEmpty(CurrentSessionPayment.ResponseRef1)
        //        && !string.IsNullOrEmpty(CurrentSessionPayment.ResponseTxid)
        //        && !string.IsNullOrEmpty(CurrentSessionPayment.ResponseSessRef1))
        //    {
        //        CurrentSessionPayment.ResponseStatus = WebConstants.PaymentAndReport.MPAY_STATUS_SUCCESS;
        //    }
        //    else
        //    {
        //        CurrentSessionPayment.ResponseStatus = WebConstants.PaymentAndReport.MPAY_STATUS_FAILED;
        //    }
        //}

        private void WriteLogPayment(FBB_PAYMENT_LOG fbbPaymentLog)
        {
            //return;
            var logMessage = string.Empty;

            switch (fbbPaymentLog.EvenName)
            {
                case WebConstants.PaymentAndReport.LOG_EVENT_NAME_LOGIN:
                    // EVENT_NAME|TSID|REQ_TIME|RESP_TIME|SERVICE_NAME|INTERNET_NUM|STATUS|DUEDATE|AMOUNT|DESC
                    logMessage = string.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}",
                        ToStringLog(fbbPaymentLog.EvenName),
                        ToStringLog(fbbPaymentLog.TransactionId),
                        ToStringLog(fbbPaymentLog.RequestTime),
                        ToStringLog(fbbPaymentLog.ResponseTime),
                        ToStringLog(fbbPaymentLog.ServiceName),
                        ToStringLog(fbbPaymentLog.InternetNumber),
                        ToStringLog(fbbPaymentLog.IdCardNumber),
                        ToStringLog(fbbPaymentLog.CardType),
                        ToStringLog(fbbPaymentLog.Status),
                        ToStringLog(fbbPaymentLog.Description));

                    break;
                case WebConstants.PaymentAndReport.LOG_EVENT_NAME_CHECKBALANCE:
                    // EVENT_NAME|TSID|REQ_TIME|RESP_TIME|SERVICE_NAME|INTERNET_NUM|STATUS|DUEDATE|AMOUNT|DESC
                    logMessage = string.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}",
                        ToStringLog(fbbPaymentLog.EvenName),
                        ToStringLog(fbbPaymentLog.TransactionId),
                        ToStringLog(fbbPaymentLog.RequestTime),
                        ToStringLog(fbbPaymentLog.ResponseTime),
                        ToStringLog(fbbPaymentLog.ServiceName),
                        ToStringLog(fbbPaymentLog.InternetNumber),
                        ToStringLog(fbbPaymentLog.Status),
                        ToStringLog(fbbPaymentLog.Duedate),
                        ToStringLog(fbbPaymentLog.Amount),
                        ToStringLog(fbbPaymentLog.Description));

                    break;

                case WebConstants.PaymentAndReport.LOG_EVENT_NAME_PAYMENTMPAY:
                    if (fbbPaymentLog.Status.Equals(WebConstants.PaymentAndReport.LOG_TYPE_REQUEST))
                    {
                        //EVENT_NAME|TSID|INTERNET_NUM|REQ_TIME|REQUEST|URL_REQUST
                        logMessage = string.Format("{0}|{1}|{2}|{3}|{4}|{5}",
                            ToStringLog(fbbPaymentLog.EvenName),
                            ToStringLog(fbbPaymentLog.TransactionId),
                            ToStringLog(fbbPaymentLog.InternetNumber),
                            ToStringLog(fbbPaymentLog.RequestTime),
                            ToStringLog(fbbPaymentLog.Request),
                            ToStringLog(fbbPaymentLog.UrlRequest));
                    }
                    else
                    {
                        //EVENT_NAME|TSID|INTERNET_NUM|RESP_TIME|RESPONSE|STATUS|URL_REF|RESP_PARM
                        logMessage = string.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}",
                            ToStringLog(fbbPaymentLog.EvenName),
                            ToStringLog(fbbPaymentLog.TransactionId),
                            ToStringLog(fbbPaymentLog.InternetNumber),
                            ToStringLog(fbbPaymentLog.ResponseTime),
                            ToStringLog(fbbPaymentLog.Response),
                            ToStringLog(fbbPaymentLog.Status),
                            ToStringLog(fbbPaymentLog.UrlReference),
                            ToStringLog(fbbPaymentLog.ResponseParameter));
                    }
                    break;
                case WebConstants.PaymentAndReport.LOG_EVENT_NAME_PAYMENTCREDIT:
                    if (fbbPaymentLog.Status.Equals(WebConstants.PaymentAndReport.LOG_TYPE_REQUEST))
                    {
                        //EVENT_NAME|TSID|INTERNET_NUM|REQ_TIME|REQUEST|URL_REQUST
                        logMessage = string.Format("{0}|{1}|{2}|{3}|{4}|{5}",
                            ToStringLog(fbbPaymentLog.EvenName),
                            ToStringLog(fbbPaymentLog.TransactionId),
                            ToStringLog(fbbPaymentLog.InternetNumber),
                            ToStringLog(fbbPaymentLog.RequestTime),
                            ToStringLog(fbbPaymentLog.Request),
                            ToStringLog(fbbPaymentLog.UrlRequest));
                    }
                    else
                    {
                        //EVENT_NAME|TSID|INTERNET_NUM|RESP_TIME|RESPONSE|STATUS|URL_REF|RESP_PARM
                        logMessage = string.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}",
                            ToStringLog(fbbPaymentLog.EvenName),
                            ToStringLog(fbbPaymentLog.TransactionId),
                            ToStringLog(fbbPaymentLog.InternetNumber),
                            ToStringLog(fbbPaymentLog.ResponseTime),
                            ToStringLog(fbbPaymentLog.Response),
                            ToStringLog(fbbPaymentLog.Status),
                            ToStringLog(fbbPaymentLog.UrlReference),
                            ToStringLog(fbbPaymentLog.ResponseParameter));
                    }
                    break;
            }

            if (!string.IsNullOrEmpty(logMessage)) _logPayment.Info(logMessage);
        }

        private string ToStringLog(string value)
        {
            return !string.IsNullOrEmpty(value) ? value : WebConstants.PaymentAndReport.LOG_NULL_TEXT;
        }

        [HttpPost]
        public ActionResult PaymentToDeduction(string productName = "", string serviceName = "", string amount = "", string internetNo = "", string billingNo = "")
        {
            ActionResult action;
            try
            {
                var fullUrlStr = this.Url.Action("PaymentToDeduction", "Payment", null, this.Request.Url.Scheme);
                string endPointUrl = "";
                var query = new GetPaymentToDeductionQuery
                {
                    UpdateBy = "Deduction",
                    FullUrl = fullUrlStr,
                    ProductName = productName,
                    ServiceName = serviceName,
                    InternetNo = internetNo,
                    Amount = amount,
                    BillingNo = billingNo,
                };
                var result = _queryProcessor.Execute(query);
                if (result != null)
                {
                    endPointUrl = result.form_url.ToSafeString();
                }

                if (endPointUrl != "")
                {
                    action = Redirect(endPointUrl);
                }
                else
                {
                    var fullUrlFail = this.Url.Action("Failed", "Payment", null, this.Request.Url.Scheme);
                    action = Redirect(fullUrlFail);
                }
            }
            catch (Exception ex)
            {
                var fullUrlFail = this.Url.Action("Failed", "Payment", null, this.Request.Url.Scheme);
                action = Redirect(fullUrlFail);
                base.Logger.Info("PaymentToDeduction Exception : " + ex.GetErrorMessage());
            }
            return action;
        }

        [HttpGet]
        public ActionResult PaymentToDeductionSuccessResult(string transactionId = "")
        {
            ActionResult action = null;
            var logStatus = "ERROR";
            var logOutput = "";
            InterfaceLogCommand log = null;
            try
            {
                log = StartInterface(transactionId, "PaymentToDeductionSuccessResult", transactionId, "", "PAYMENT");
            }
            catch (Exception ex)
            {
                base.Logger.Info("PaymentToDeductionSuccessResult StartInterface Exception : " + ex.GetErrorMessage());
            }
            try
            {
                _configLovHelpers = new ConfigLovHelpers();
                ViewBag.LogoutController = ControllerContext.RouteData.Values["controller"].ToString();
                ViewBag.LabelFBBOR_MENU = _configLovHelpers.GetLovByTypePageCode(WebConstants.LovConfigName.LovTypeMenu, WebConstants.LovConfigName.PaymentPageCode);
                ViewBag.LabelFBBOR019 = _configLovHelpers.GetLovScreenByPageCode(WebConstants.LovConfigName.PaymentPageCode);

                var paymentLog = GetPaymentLogByRegisPendingDeduction(transactionId);
                action = View("Success", paymentLog);

                logStatus = "Success";
                logOutput = "";
            }
            catch (Exception ex)
            {
                base.Logger.Info("PaymentToDeductionSuccessResult Exception : " + ex.GetErrorMessage());
                logStatus = "ERROR";
                logOutput = ex.GetErrorMessage();
                var fullUrlFail = this.Url.Action("Failed", "Payment", null, this.Request.Url.Scheme);
                action = Redirect(fullUrlFail);
            }
            finally
            {
                base.Logger.Info("PaymentToDeductionSuccessResult End");
                EndInterface(logOutput, log, transactionId, logStatus, "");
            }

            return action;
        }

        [HttpGet]
        public ActionResult PaymentToDeductionFailResult(string Data = "")
        {
            ActionResult action = null;
            InterfaceLogCommand log = null;
            var orderTransactionId = "";
            var logStatus = "Failed";
            var logOutput = "";
            try
            {
                orderTransactionId = DecodeOrderTransactionId(Data);
                log = StartInterface(Data, "PaymentToDeductionFailResult", orderTransactionId, "", "PAYMENT");
            }
            catch (Exception ex)
            {
                base.Logger.Info("PaymentToDeductionFailResult StartInterface Exception : " + ex.GetErrorMessage());
            }
            try
            {
                UpdateCancelPendingOrderDeduction(orderTransactionId, "PAYMENT");
                _configLovHelpers = new ConfigLovHelpers();
                ViewBag.LogoutController = ControllerContext.RouteData.Values["controller"].ToString();
                ViewBag.LabelFBBOR_MENU = _configLovHelpers.GetLovByTypePageCode(WebConstants.LovConfigName.LovTypeMenu, WebConstants.LovConfigName.PaymentPageCode);
                ViewBag.LabelFBBOR019 = _configLovHelpers.GetLovScreenByPageCode(WebConstants.LovConfigName.PaymentPageCode);

                action = View("Failed");

                logStatus = "Success";
                logOutput = "";
            }
            catch (Exception ex)
            {
                base.Logger.Info("PaymentToDeductionFailResult Exception : " + ex.GetErrorMessage());
                logStatus = "Failed";
                logOutput = ex.GetErrorMessage();
            }
            finally
            {
                EndInterface(logOutput, log, Data, logStatus, "");
            }
            return action;
        }

        private PaymentLogCommand GetPaymentLogByRegisPendingDeduction(string TransactionId)
        {
            var result = new PaymentLogCommand();
            var pendindDeduction = GetRegisterPendingDeduction(TransactionId);
            if (pendindDeduction != null && pendindDeduction.Data != null)
            {
                result = new PaymentLogCommand
                {
                    ResponseAmount = pendindDeduction.Data.PAID_AMT.ToSafeString(),
                    RequestParamAmount = pendindDeduction.Data.PAID_AMT.ToSafeString(),
                    CustInternetNum = pendindDeduction.Data.NON_MOBILE_NO.ToSafeString(),
                    ResponseDatetime = DateTime.Now
                };
            }
            return result;
        }

        private GetRegisterPendingDeductionModel GetRegisterPendingDeduction(string TransactionId)
        {
            var getConfigReqPaymentQuery = new GetRegisterPendingDeductionQuery()
            {
                Url = this.Url.Action("GetRegisterPendingDeduction", "Payment", null, this.Request.Url.Scheme),
                transaction_id = TransactionId,
            };
            return _queryProcessor.Execute(getConfigReqPaymentQuery);
        }

        private void UpdateCancelPendingOrderDeduction(string orderTransactionId, string pageRequest)
        {
            InterfaceLogCommand log = null;
            var logStatus = "Failed";
            var logMessage = "";
            var dataDecrypt = "";
            try
            {
                log = StartInterface(orderTransactionId, "UpdateCancelPendingOrderDeduction", orderTransactionId, "", pageRequest);
            }
            catch (Exception ex)
            {
                base.Logger.Info("UpdateCancelPendingOrderDeduction StartInterface Exception : " + ex.GetErrorMessage());
            }
            try
            {
                var commandCancelDeduc = new SaveDeductionLogCommand()
                {
                    p_action = "Cancel",
                    p_service_name = "WebHook Notify",
                    p_user_name = "WebHook Notify Cancel",
                    p_order_transaction_id = orderTransactionId,
                };
                _saveDeductionLogCommand.Handle(commandCancelDeduc);

                logStatus = "Success";
                logMessage = "";
            }
            catch (Exception ex)
            {
                base.Logger.Info("UpdateCancelPendingOrderDeduction Exception : " + ex.GetErrorMessage());
                logStatus = "Failed";
                logMessage = ex.GetErrorMessage();
            }
            finally
            {
                EndInterface(dataDecrypt, log, "", logStatus, logMessage);
            }
        }

        private string DecodeOrderTransactionId(string Data = "")
        {
            var result = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(Data))
                {
                    var dataDecrypt = EncryptionUtility.Base64Decode(Data);
                    if (!string.IsNullOrEmpty(dataDecrypt))
                    {
                        var arrData = dataDecrypt.Split('=');
                        if (arrData.Length == 2)
                        {
                            var orderTransactionId = arrData[1];
                            if (!string.IsNullOrEmpty(orderTransactionId))
                            {
                                result = orderTransactionId;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = string.Empty;
                base.Logger.Info("DecodeOrderTransactionId Exception : " + ex.GetErrorMessage());
            }
            return result;
        }

        private InterfaceLogCommand StartInterface<T>(T query, string methodName, string transactionId, string idCardNo, string INTERFACE_NODE)
        {
            string FullUrl = "";
            string SERVICE_NAME = INTERFACE_NODE;
            if (Session["FullUrl"] != null)
            {
                FullUrl = Session["FullUrl"].ToSafeString();
                INTERFACE_NODE = INTERFACE_NODE + "|" + FullUrl;
            }

            var dbIntfCmd = new InterfaceLogCommand
            {
                ActionType = WBBContract.Commands.ActionType.Insert,
                IN_TRANSACTION_ID = transactionId,
                METHOD_NAME = methodName,
                SERVICE_NAME = SERVICE_NAME,
                IN_ID_CARD_NO = idCardNo,
                IN_XML_PARAM = query.DumpToXml(),
                INTERFACE_NODE = INTERFACE_NODE,
                CREATED_BY = "FBBWEB",
            };

            _intfLogCommand.Handle(dbIntfCmd);

            return dbIntfCmd;
        }

        private void EndInterface<T>(T output, InterfaceLogCommand dbIntfCmd,
            string transactionId, string result, string reason)
        {
            if (null == dbIntfCmd)
                return;

            dbIntfCmd.ActionType = WBBContract.Commands.ActionType.Update;
            dbIntfCmd.REQUEST_STATUS = (result == "Success") ? "Success" : "Error";
            dbIntfCmd.OUT_RESULT = result;
            dbIntfCmd.OUT_ERROR_RESULT = (result == "Success") ? (reason.Length > 100 ? reason.Substring(0, 100) : result) : result;
            dbIntfCmd.OUT_XML_PARAM = (result == "Success") ? output.DumpToXml() : reason;

            _intfLogCommand.Handle(dbIntfCmd);
        }
    }
}
