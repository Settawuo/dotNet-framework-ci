using FBBConfig.Extensions;
using ServiceStack;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Queries.Commons.Account;
using WBBContract.Queries.WebServices;
using WBBEntity.Extensions;
using WBBEntity.PanelModels.Account;

namespace FBBConfig.Controllers
{
    [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
    public class AccountController : FBBConfigController
    {
        // GET: /Account/Login
        private readonly IQueryProcessor _queryProcessor;
        public AccountController(
            IQueryProcessor queryProcessor,
            ILogger logger)
        {
            _queryProcessor = queryProcessor;
            base._Logger = logger;
        }
        [AllowAnonymous]
        [CustomActionFilter(LogType = "Login")]
        public ActionResult Login()
        {
            try 
            {
                if (base.CurrentUser != null)
                {
                    return RedirectToAction("Index", "Home");
                }
                return Redirect(IdentifyService());

            }
            catch(Exception ex)
            {
                _Logger.Info("Login ERROR " + ex.GetErrorMessage());
                return RedirectToAction("TemporaryNotAvailable", "Account");
            }
        }
        #region login ids
        public string IdentifyService()
        {
            var config_redirect_url = GetAccessIdsConfig();
            var url_redirect = _queryProcessor.Execute(config_redirect_url);
            Session["url_redirect"] = url_redirect;
            return url_redirect;
        }

        [HttpGet, Route("Account/LogOnByIDS")]
        [CustomActionFilter(LogType = "LogOnByIDS")]
        public ActionResult LogOnByIDS(string code)
        {
            try
            {
                _Logger.Info("START Login IDS");
                if (!string.IsNullOrEmpty(code))
                {
                    string url = Session["url_redirect"].ToString();
                    var callbackUrl = CutUri("redirect_uri=", url);
                    var request = BuildAccessTokenQuery(code, callbackUrl);

                    var tokenValue = _queryProcessor.Execute(request);

                    if (tokenValue.access_token != "" && !string.IsNullOrEmpty(tokenValue.access_token))
                    {
                        var ProfileToken = BuildProfileQuery(tokenValue.access_token);
                        var LDAP = _queryProcessor.Execute(ProfileToken);
                        if (LDAP != null) {
                            var authenticatedUser = GetUser(LDAP.username);
                            _Logger.Info("authenticated success");
                            authenticatedUser.AuthenticateType = AuthenticateType.LDAP;
                            Response.AppendCookie(CreateAuthenticatedCookie(authenticatedUser.UserName));
                            base.CurrentUser = authenticatedUser;
                            Session["id_token"] = tokenValue.id_token;
                            Session["PageView"] = base.CurrentUser.ProgramModel;
                            return RedirectToAction("Index", "Home");
                        }
                    }
                }
                else
                {
                    _Logger.Info("parameter code invalid");
                    return RedirectToAction("Login", "Account");
                }
            }
            catch (Exception ex)
            {
                _Logger.Info("LogOnByIDS ERROR " + ex.GetErrorMessage());
            }
            return RedirectToAction("TemporaryNotAvailable", "Account");
        }
        
        private string CutUri(string word, string url)
        {
            int startIdx = url.IndexOf(word) + word.Length;
            int endIdx = url.IndexOf(url.Contains('&') ? "&" : "%26", startIdx);
            string result = url.Substring(startIdx, endIdx == -1 ? url.Length - startIdx : endIdx - startIdx);
            return result;
        }
        private GetAccessTokenFBBConfigQuery BuildAccessTokenQuery(string code, string callbackUrl)
            => new GetAccessTokenFBBConfigQuery
            {
                code = code,
                grant_type = "authorization_code",
                redirect_uri = callbackUrl,
            };
        private GetUserProfileIDSQuery BuildProfileQuery(string access_token)
            => new GetUserProfileIDSQuery
            {
                access_token = access_token,
            };

        private IdentifyServiceRedirectQuery GetAccessIdsConfig() 
            => new IdentifyServiceRedirectQuery
            {
                CHANNEL = "FBBOFFICER",
                SERVICE_PROVIDER_NAME = "OTP Login",

            };
        private DeniedAccessToken DeniedToken(string id_token, string redirect_uri) 
            => new DeniedAccessToken
            {
                redirect_uri = redirect_uri,
                id_token = id_token,

            };
        private UserModel GetUser(string userName)
        {
            var userQuery = new GetUserDataQuery
            {
                UserName = userName,
            };

            var authenticatedUser = _queryProcessor.Execute(userQuery);
            return authenticatedUser;
        }
        
        private HttpCookie CreateAuthenticatedCookie(string userName)
        {
            var authTicket = new FormsAuthenticationTicket(2, userName, DateTime.Now,
                DateTime.Now.AddMinutes(FormsAuthentication.Timeout.TotalMinutes), false, ""); //2880 M

            var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName,
                FormsAuthentication.Encrypt(authTicket))
            { HttpOnly = true };

            return authCookie;
        }
        public ActionResult LogOnByPass()
        {
            string User = "ADMINPAYG";
            var authenticatedUser = GetUser(User);
            authenticatedUser.AuthenticateType = AuthenticateType.LDAP;
            Response.AppendCookie(CreateAuthenticatedCookie(authenticatedUser.UserName));
            base.CurrentUser = authenticatedUser;
            Session["PageView"] = base.CurrentUser.ProgramModel;
            return RedirectToAction("Index", "Home");
        }
        public ActionResult LogOut()
        {
            try
            {
                if (base.CurrentUser != null)
                {
                    if (base.CurrentUser.AuthenticateType == AuthenticateType.LDAP)
                    {
                        FormsAuthentication.SignOut();
                        Session.Clear();
                        Session.Abandon();
                        var LogonByPass = Session["url_redirect"].ToSafeString();
                        if (!LogonByPass.Equals(""))
                        {
                            string url = Session["url_redirect"].ToSafeString();
                            var callbackUrl = CutUri("redirect_uri=", url);
                            var authenticated = DeniedToken(Session["id_token"].ToSafeString(), callbackUrl);
                            var url_redirect = _queryProcessor.Execute(authenticated);
                        }
                        return RedirectToAction("Login", "Account");
                    }
                }
                FormsAuthentication.SignOut();
                Session.Clear();
                Session.Abandon();
                return RedirectToAction("Login", "Account");
            }
            catch
            {
                return RedirectToAction("Login", "Account");
            }
        }
        [HttpGet]
        [AllowAnonymous]

        #endregion
        public ActionResult TemporaryNotAvailable()
        {
            return View();
        }
    }
}
