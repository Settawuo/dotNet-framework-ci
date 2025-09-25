using System;
using System.Web.Mvc;
using System.Web.Routing;
using WBBBusinessLayer;
using WBBEntity.PanelModels;
using WBBWeb.CompositionRoot;
using WBBWeb.Controllers;
using WBBWeb.Solid.CompositionRoot;

namespace WBBWeb.Extension
{

    public class AuthorizePaymentUserAttribute : AuthorizeAttribute
    {
        private readonly ILogger _logger = Bootstrapper.GetInstance<DebugLogger>();

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            _logger.Info("Begin OnAuthorization");
            if (null == filterContext)
            {
                throw new ArgumentNullException("filterContext");
            }

            if (filterContext.ActionDescriptor.IsDefined
                (typeof(AllowAnonymousAttribute), true) ||
                filterContext.ActionDescriptor.ControllerDescriptor.IsDefined
                    (typeof(AllowAnonymousAttribute), true))
            {
                return;
            }

            var controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            var controllerAction = filterContext.ActionDescriptor.ActionName;
            string webReferrer = string.Empty;

            var urlReferrer = filterContext.HttpContext.Request.UrlReferrer;
            if (urlReferrer != null)
            {
                webReferrer = urlReferrer.ToString();
            }

            if (controllerAction.ToUpper().Equals(WebConstants.PaymentAndReport.PAYMENT_AND_REPORT_PAGE_LOGIN.ToUpper()))
            {
                filterContext.HttpContext.Session[WebConstants.FBBConfigSessionKeys.UserPaymentPromblemReport] = null;
            }
            else if (
                controllerAction.ToUpper()
                    .Equals(WebConstants.PaymentAndReport.PAYMENT_AND_REPORT_PAGE_LOGIN_CONCURRENT.ToUpper()) ||
                controllerAction.ToUpper()
                    .Equals(WebConstants.PaymentAndReport.PAYMENT_AND_REPORT_PAGE_SESSION_TIMEOUT.ToUpper()))
            {
                var sessionManagement = new SessionManagement();

                sessionManagement.SetNewSessionId();
                filterContext.HttpContext.Session[WebConstants.FBBConfigSessionKeys.UserPaymentPromblemReport] = null;
            }
            else
            {
                var paymentAndReportLogin =
                    (LoginPaymentReportProblemModel)
                        filterContext.HttpContext.Session[
                            WebConstants.FBBConfigSessionKeys.UserPaymentPromblemReport];
                var sessionId = SessionManagement.GetSessionId();

                if (paymentAndReportLogin != null && paymentAndReportLogin.CustIdCardNum != null && paymentAndReportLogin.CustIdCardType != null && paymentAndReportLogin.CustInternetNum != null)
                {
                    var masterController = Bootstrapper.GetInstance<MasterDataController>();
                    var sessionLoginStatusConcurrent =
                        masterController.GetSessionLoginStatus(paymentAndReportLogin.CustInternetNum, sessionId);

                    if (sessionLoginStatusConcurrent == 3)
                    {
                        filterContext.Result = new RedirectToRouteResult(
                            new RouteValueDictionary(new
                            {
                                controller = controllerName,
                                action = WebConstants.PaymentAndReport.PAYMENT_AND_REPORT_PAGE_LOGIN_CONCURRENT,
                            }));
                    }
                }
                else
                {
                    string actionRedirect = WebConstants.PaymentAndReport.PAYMENT_AND_REPORT_PAGE_LOGIN;
                    if (webReferrer.ToLower().Contains("/payment") || webReferrer.ToLower().Contains("/reportproblem"))
                    {
                        if (webReferrer.ToLower().Contains("/loginconcurrent") || webReferrer.ToLower().Contains("/sessiontimeout"))
                        {
                            actionRedirect = WebConstants.PaymentAndReport.PAYMENT_AND_REPORT_PAGE_LOGIN;
                        }
                        else
                        {
                            var masterController = Bootstrapper.GetInstance<MasterDataController>();
                            var sessionLoginStatusTimeOut = masterController.GetSessionLoginStatus(string.Empty, sessionId);

                            actionRedirect = (sessionLoginStatusTimeOut == 1)
                                ? WebConstants.PaymentAndReport.PAYMENT_AND_REPORT_PAGE_SESSION_TIMEOUT
                                : WebConstants.PaymentAndReport.PAYMENT_AND_REPORT_PAGE_LOGIN;
                        }
                    }
                    filterContext.Result = new RedirectToRouteResult(
                        new RouteValueDictionary(new
                        {
                            controller = controllerName,
                            action = actionRedirect,
                        }));
                }
            }
        }
    }

}