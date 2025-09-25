using System;
using System.Collections.Generic;
using System.Net;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using WBBBusinessLayer;
using WBBWeb.CompositionRoot;
using WBBWeb.Solid.CompositionRoot;

namespace WBBWeb.Extension
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class AjaxValidateAntiForgeryTokenAttribute : FilterAttribute, IAuthorizationFilter
    {
        private ILogger _Logger = Bootstrapper.GetInstance<DebugLogger>();
        private List<int> redirectionStatusCode = new List<int>()
        {(int)HttpStatusCode.Ambiguous, (int)HttpStatusCode.MovedPermanently, (int)HttpStatusCode.Moved, (int)HttpStatusCode.Found, (int)HttpStatusCode.Redirect,
            (int)HttpStatusCode.SeeOther, (int)HttpStatusCode.RedirectMethod, (int)HttpStatusCode.NotModified, (int)HttpStatusCode.UseProxy, (int)HttpStatusCode.Unused,
             (int)HttpStatusCode.TemporaryRedirect, (int)HttpStatusCode.RedirectKeepVerb
        };

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            string sessionId = string.Empty;
            string mobileNo = string.Empty;
            try
            {
                //_Logger.Info("AjaxValidateAntiForgeryTokenAttribute");

                string myAIS = filterContext.HttpContext.Request.Headers["myAIS"];
                string myAISform = filterContext.HttpContext.Request.Form["myAIS"];

                sessionId = filterContext.HttpContext.Session != null ? filterContext.HttpContext.Session.SessionID : string.Empty;
                mobileNo = filterContext.HttpContext.Request.Form["CoveragePanelModel.P_MOBILE"];
                if (string.IsNullOrEmpty(mobileNo))
                {
                    mobileNo = filterContext.HttpContext.Request.Form["no"] + " (no)";
                }
                // for log
                //var controllerName = filterContext.RouteData.Values["controller"].ToString();
                //var actionName = filterContext.RouteData.Values["action"].ToString();
                //if (controllerName == "Process" && actionName == "Index")
                //{
                //var rawUrl = filterContext.HttpContext.Request.RawUrl;                  
                ////SummaryPanelModel.PackageModelList[0].SFF_PROMOTION_CODE
                //var SFF_PROMOTION_CODE1 = filterContext.HttpContext.Request.Form["SummaryPanelModel.PackageModelList[0].SFF_PROMOTION_CODE"];
                //if (SFF_PROMOTION_CODE1 == null) SFF_PROMOTION_CODE1 = "";
                ////SummaryPanelModel.PackageModelList[1].SFF_PROMOTION_CODE
                //var SFF_PROMOTION_CODE2 = filterContext.HttpContext.Request.Form["SummaryPanelModel.PackageModelList[1].SFF_PROMOTION_CODE"];
                //if (SFF_PROMOTION_CODE2 == null) SFF_PROMOTION_CODE2 = "";
                ////SummaryPanelModel.PackageModelList[2].SFF_PROMOTION_CODE
                //var SFF_PROMOTION_CODE3 = filterContext.HttpContext.Request.Form["SummaryPanelModel.PackageModelList[2].SFF_PROMOTION_CODE"];
                //if (SFF_PROMOTION_CODE3 == null) SFF_PROMOTION_CODE3 = "";
                ////CustomerRegisterPanelModel.L_FIRST_NAME
                //var FIRST_NAME = filterContext.HttpContext.Request.Form["CustomerRegisterPanelModel.L_FIRST_NAME"];
                //if (FIRST_NAME == null) FIRST_NAME = "";
                ////CustomerRegisterPanelModel.L_FIRST_NAME
                //var LAST_NAME = filterContext.HttpContext.Request.Form["CustomerRegisterPanelModel.L_LAST_NAME"];
                //if (LAST_NAME == null) LAST_NAME = "";

                //var resultRequest = filterContext.HttpContext.Request.Form;
                //if(resultRequest != null)
                //{
                //    _Logger.Info("sessionId = " + sessionId + " mobileNo =  " + mobileNo + " resultRequest =  " + resultRequest);
                //}

                //_Logger.Info("sessionId = " + sessionId + " mobileNo =  " + mobileNo + " rawUrl =  " + rawUrl);
                //_Logger.Info("sessionId = " + sessionId + " mobileNo =  " + mobileNo + " SFF_PROMOTION_CODE1 =  " + SFF_PROMOTION_CODE1);
                //_Logger.Info("sessionId = " + sessionId + " mobileNo =  " + mobileNo + " SFF_PROMOTION_CODE2 =  " + SFF_PROMOTION_CODE2);
                //_Logger.Info("sessionId = " + sessionId + " mobileNo =  " + mobileNo + " SFF_PROMOTION_CODE3 =  " + SFF_PROMOTION_CODE3);
                //_Logger.Info("sessionId = " + sessionId + " mobileNo =  " + mobileNo + " FIRST_NAME =  " + FIRST_NAME);
                //_Logger.Info("sessionId = " + sessionId + " mobileNo =  " + mobileNo + " LAST_NAME =  " + LAST_NAME);
                //_Logger.Info("sessionId = " + sessionId + " mobileNo =  " + mobileNo + " END rawUrl =  " + rawUrl);

                // ตรวจสอบว่ามีการเกิดข้อผิดพลาดในการทำงานของ Action หรือไม่
                //if (filterContext.RouteData.Values["exception"] != null)
                //{

                // ตรวจสอบว่าข้อผิดพลาดเป็น HttpException หรือไม่
                //if (filterContext.RouteData.Values["exception"].GetType() == typeof(HttpException))
                //{
                //var ex = (HttpException)filterContext.RouteData.Values["exception"];
                //if (ex.ErrorCode < 500)
                //_Logger.InfoFormat("{0} {1} {2} {3} {4} {5} {6}", sessionId, mobileNo, rawUrl, ex.GetHttpCode(), controllerName, actionName, ex);
                //else
                //_Logger.ErrorFormat("{0} {1} {2} {3} {4} {5} {6}", sessionId, mobileNo, rawUrl, ex.GetHttpCode(), controllerName, actionName, ex);
                //} 
                //else
                //{
                //var ex = (Exception)filterContext.RouteData.Values["exception"];
                //_Logger.ErrorFormat(sessionId, null, rawUrl, (int)HttpStatusCode.InternalServerError, controllerName, actionName, ex);
                //}
                //}
                //else if (filterContext.Controller.ViewData.ModelState[""] != null && filterContext.Controller.ViewData.ModelState[""].Errors.Any(e => !string.IsNullOrEmpty(e.ErrorMessage)))
                //{
                // ตรวจสอบว่ามีข้อผิดพลาดใน ModelState ของ Controller หรือไม่
                // หากมีข้อผิดพลาดให้สร้างข้อความ message โดยรวมข้อผิดพลาดทั้งหมด
                //string message = string.Empty;
                //foreach (var obj in filterContext.Controller.ViewData.ModelState[""].Errors.Where(e => !string.IsNullOrEmpty(e.ErrorMessage)))
                //{
                //if (!string.IsNullOrEmpty(obj.ErrorMessage))
                //{
                //if (!string.IsNullOrEmpty(message))
                //message += ", ";
                //message += obj.ErrorMessage;
                //}
                //}
                // บันทึกข้อความข้อผิดพลาดลงใน log และกำหนดรหัสสถานะการตอบกลับเป็น Precondition Failed
                //_Logger.ErrorFormat("{0} {1} {2} {3} {4} {5} {6}", sessionId, mobileNo, message, rawUrl, (int)HttpStatusCode.PreconditionFailed, controllerName, actionName);
                //}
                //else
                //{

                // กรณีอื่น ๆ (ไม่มีข้อผิดพลาดในการทำงานและไม่ได้เปลี่ยนเส้นทางการเชื่อมโยง)
                // ตรวจสอบว่าเป็นการตอบกลับ HTTP สำเร็จ (รหัสสถานะ 200) หรือเป็นการเปลี่ยนเส้นทางการเชื่อมโยง (รหัสสถานะที่กำหนดไว้ในรายการ redirectionStatusCode)
                //if (filterContext.HttpContext.Response.StatusCode == 200 || redirectionStatusCode.Any(s => s == filterContext.HttpContext.Response.StatusCode))
                //{
                //    // กรณีเป็นการตอบกลับ HTTP สำเร็จหรือการเปลี่ยนเส้นทางการเชื่อมโยง
                //    string message = string.Empty;
                //    if (redirectionStatusCode.Any(s => s == filterContext.HttpContext.Response.StatusCode))
                //    {
                //        message += $"redirection:{filterContext.HttpContext.Response.RedirectLocation}";
                //        _Logger.Info(sessionId + " " + mobileNo + " " + message + " " + rawUrl + " " + controllerName + " " + actionName);
                //    }
                //    else
                //        _Logger.Info(sessionId + " " + mobileNo + " " + message + " " + rawUrl + " " + controllerName + " " + actionName);
                //}
                //else
                //    _Logger.Error(sessionId + " " + mobileNo + " " + " " + " " + rawUrl + " " + controllerName + " " + actionName);
                //}

                //}
                // end for log
                if (filterContext.HttpContext.Request.IsAjaxRequest() || filterContext.HttpContext.Request.Form["VerificationToken"] != null) // if it is ajax request.
                {
                    this.ValidateRequestHeader(filterContext.HttpContext.Request); // run the validation.
                }
                else if (!string.IsNullOrEmpty(myAIS) || !string.IsNullOrEmpty(myAISform))
                {

                }
                else
                {
                    AntiForgery.Validate();
                }
            }
            catch (HttpAntiForgeryException e)
            {
                _Logger.Info("sessionId = " + sessionId + ", mobileNo = " + mobileNo + string.Format("HttpAntiForgeryException : {0}", e.Message));

                throw new HttpAntiForgeryException(string.Format("Anti forgery token not found : {0}", e.Message));
            }
        }

        private static string oldToken;

        private void ValidateRequestHeader(HttpRequestBase request)
        {
            string cookieToken = string.Empty;
            string formToken = string.Empty;
            string tokenValue = string.Empty;
            string tokenValueForm = string.Empty;

            try
            {
                if (null == oldToken)
                    oldToken = "";

                tokenValue = request.Headers["VerificationToken"]; // read the header key and validate the tokens.
                if (tokenValue == null) tokenValue = "";
                tokenValueForm = request.Form["VerificationToken"];
                if (tokenValueForm == null) tokenValueForm = "";

                HttpCookie cookie = HttpContext.Current.Request.Cookies["passVerify"];

                string myAIS = request.Headers["myAIS"];
                if (myAIS == null) myAIS = "";
                string myAISform = request.Form["myAIS"];
                if (myAISform == null) myAISform = "";
                string isLog = request.Headers["isLog"];
                if (isLog == null) isLog = "";

                if (string.IsNullOrEmpty(tokenValue))
                {
                    if (string.IsNullOrEmpty(tokenValueForm))
                        tokenValue = "";
                    else
                        tokenValue = tokenValueForm;
                }

                if (oldToken != tokenValue || (!string.IsNullOrEmpty(isLog) && Boolean.Parse(isLog)))
                {
                    oldToken = tokenValue;
                    if (!string.IsNullOrEmpty(tokenValue))
                    {
                        string[] tokens = tokenValue.Split(',');
                        if (tokens.Length == 2)
                        {
                            cookieToken = tokens[0].Trim();
                            formToken = tokens[1].Trim();
                        }
                    }
                    if (cookie != null && Boolean.Parse(cookie.Value.ToString()))
                    {
                        //cookie.Value = "False";
                        //HttpContext.Response.SetCookie(cookie);
                    }
                    else if (string.IsNullOrEmpty(myAIS) && string.IsNullOrEmpty(myAISform))
                        AntiForgery.Validate(cookieToken, formToken); // this validates the request token.
                }
                else
                {
                    AntiForgery.Validate();
                }
            }
            catch (Exception ex)
            {
                _Logger.Info(string.Format("Anti forgery token Exception: {0}", ex.Message));
                _Logger.Info(string.Format("CookieToken : {0}", cookieToken));
                _Logger.Info(string.Format("formToken : {0}", formToken));
                _Logger.Info(string.Format("tokenValue : {0}", tokenValue));
                _Logger.Info(string.Format("tokenValueForm : {0}", tokenValueForm));
                _Logger.Info(string.Format("oldToken : {0}", oldToken));

                throw new HttpAntiForgeryException(string.Format("Anti forgery token: {0}", ex.Message));
            }
        }
    }
}
