using System;
using System.Collections.Specialized;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Queries.Commons.Account;
using WBBEntity.Extensions;
using WBBEntity.PanelModels.Account;
namespace WBBWeb.Controllers
{
    [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
    public class AccountServiceController : WBBController
    {
        //
        // GET: /AccountService/
        private readonly IQueryProcessor _QueryProcessor;

        public AccountServiceController(
            IQueryProcessor queryProcessor,
            ILogger logger)
        {
            _QueryProcessor = queryProcessor;
            base.Logger = logger;
        }


        //public ActionResult LogOnByPass()
        //{
        //    var authenticatedUser = GetUser("chanawun", "");
        //    authenticatedUser.AuthenticateType = AuthenticateType.LDAP;
        //    Response.AppendCookie(CreateAuthenticatedCookie(authenticatedUser.UserName));
        //    base.CurrentUser = authenticatedUser;

        //    return RedirectToAction("Index", "ProcessSelect");
        //}


        //[HttpPost]
        //[AllowAnonymous]
        //[CustomActionFilter(LogType = "LogOnBySSO")]
        //public ActionResult LogOnBySSO()
        //{
        //    return SSOLogOnHandler("Index", "ProcessSelect", AuthenticateType.SSO);
        //}


        //[HttpPost]
        //[AllowAnonymous]
        //[CustomActionFilter(LogType = "LogOnBySSOPartner")]
        //public ActionResult LogOnBySSOPartner()
        //{
        //    return SSOLogOnHandler("Index", "ProcessSelect", AuthenticateType.SSOPartner);
        //}

        private ActionResult SSOLogOnHandler(string actionTo, string controller, AuthenticateType authenType)
        {
            try
            {
                var ssoData = HttpContext.Request.Form;
                var ssoFields = LoadSSOFieldsFromPostData(ssoData);
                Logger.Info(ssoFields.Token);
                Logger.Info(ssoFields.UserName);

                Logger.Info("Host: " + ssoFields.EmployeeServiceWebRootUrl + ", Location Code: " + ssoFields.LocationCode +
                    ", Group Location: " + ssoFields.GroupLocation + ", Department Code: " + ssoFields.DepartmentCode);

                //get profile
                Logger.Info("Get User Model. " + authenType.ToString());
                var authenticatedUser = GetUser(ssoFields.UserName, authenType.ToString());
                if (null != authenticatedUser)
                {
                    authenticatedUser.AuthenticateType = authenType;
                    authenticatedUser.SSOFields = ssoFields;

                    base.CurrentUser = authenticatedUser;
                    Logger.Info("Authenticate: " + base.CurrentUser.AuthenticateType);
                    Response.AppendCookie(CreateAuthenticatedCookie(authenticatedUser.UserName));

                    return RedirectToAction(actionTo, controller);
                }
                else
                {
                    Logger.Info("cannot log on by using sso.");
                    base.CurrentUser = new UserModel();
                    base.CurrentUser.AuthenticateType = authenType;

                    FormsAuthentication.SignOut();
                    Session.Clear();
                    Session.Abandon();
                }
            }
            catch (Exception ex)
            {
                Logger.Info(ex.GetErrorMessage());
                Logger.Info(ex.StackTrace);
            }

            return RedirectToAction("Logout", "AccountService");
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
                    else if (base.CurrentUser.AuthenticateType == AuthenticateType.SSOPartner)
                    {
                        var currentUser = base.CurrentUser;

                        using (var ssoService = new PartnerServices.PartnerServiceWebServiceV2Service())
                        {
                            Logger.Info(string.Format("Decreasing SSOPartner, Token:{0}", currentUser.SSOFields.Token));

                            try
                            {
                                var syncUserSessionResponse = ssoService.decreaseCounter(currentUser.SSOFields.Token);
                                if (syncUserSessionResponse.Message.ErrorCode == Constants.SSOReturnStatus.Success)
                                {
                                    Logger.Info("SSOPartner Decreasing SUCCESS " +
                                        string.Format("{0}:{1}",
                                            syncUserSessionResponse.Message.ErrorCode,
                                            syncUserSessionResponse.Message.ErrorMesg));
                                }
                                else
                                {
                                    Logger.Info("SSOPartner Decreasing FAIL " +
                                         string.Format("{0}:{1}",
                                             syncUserSessionResponse.Message.ErrorCode,
                                             syncUserSessionResponse.Message.ErrorMesg));
                                }
                            }
                            catch (TimeoutException tex)
                            {
                                Logger.Info("SSOPartner syncUserSession TIMEOUT " + tex.GetErrorMessage());
                            }
                            catch (Exception ex)
                            {
                                Logger.Info("SSOPartner syncUserSession ERROR " + ex.GetErrorMessage());
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

            return RedirectToAction("LogoutSso", "AccountService",
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

        private SSOFields LoadSSOFieldsFromPostData(NameValueCollection form)
        {
            var ssoFields = new SSOFields();

            ssoFields.Token = form["token"];

            var tokenValues = ssoFields.Token.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            ssoFields.SessionID = (tokenValues.Length > 0) ? tokenValues[0] : null;
            ssoFields.UserName = (tokenValues.Length > 1) ? tokenValues[1] : null;
            ssoFields.GroupID = (tokenValues.Length > 2) ? tokenValues[2] : null;
            ssoFields.SubModuleIDInToken = (tokenValues.Length > 3) ? tokenValues[3] : null;
            ssoFields.ClientIP = (tokenValues.Length > 4) ? (tokenValues[4] == "null" ? null : tokenValues[4]) : null;

            ssoFields.RoleID = form["rid"];
            ssoFields.SubModuleID = form["sid"];
            ssoFields.RoleName = form["rn"];
            ssoFields.SubModuleName = form["sn"];
            ssoFields.FirstName = form["fn"];
            ssoFields.LastName = form["ln"];
            ssoFields.ThemeName = form["theme"];
            ssoFields.TemplateName = form["template"];
            ssoFields.EmployeeServiceWebRootUrl = form["host"];
            ssoFields.LocationCode = form["lc"];
            ssoFields.GroupLocation = form["gl"];
            ssoFields.DepartmentCode = form["dc"];
            ssoFields.SectionCode = form["sc"];
            ssoFields.PositionByJob = form["pt"];

            return ssoFields;
        }

        public UserModel GetUser(string userName, string authenType)
        {
            var userQuery = new GetUserDataQuery
            {
                UserName = userName,
                AuthenType = authenType
            };

            var authenticatedUser = _QueryProcessor.Execute(userQuery);
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
    }
}
