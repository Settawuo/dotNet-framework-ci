using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using WBBBusinessLayer;
using WBBEntity.Extensions;
using WBBEntity.PanelModels.Account;
using WBBWeb.CompositionRoot;
using WBBWeb.Solid.CompositionRoot;

namespace WBBWeb.Extension
{
    public class AuthorizeUserAttribute : AuthorizeAttribute
    {
        private ILogger _Logger = Bootstrapper.GetInstance<DebugLogger>();

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (null == httpContext)
                throw new ArgumentNullException("httpContext");

            // check authenticated user
            if (null != httpContext.User && !httpContext.User.Identity.IsAuthenticated)
            {
                return false;
            }

            return true;
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            _Logger.Info("Begin OnAuthorization");
            if (null == filterContext)
                throw new ArgumentNullException("filterContext");

            var currentUser = (UserModel)filterContext.HttpContext.Session[WebConstants.FBBConfigSessionKeys.User];
            if (currentUser == null)
            {
                _Logger.Info("null currentUser");
                return;
            }

            bool skipAuthorization = (filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), inherit: true)
                || filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), inherit: true)
                || currentUser.ForceLogOut);

            if (skipAuthorization)
            {
                return;
            }

            if (!this.AuthorizeCore(filterContext.HttpContext))
            {
                this.HandleUnauthorizedRequest(filterContext);
                return;
            }

            // syn current user sso token
            _Logger.Info(filterContext.HttpContext.Request.Headers["X-Requested-With"]);

            if ((currentUser.AuthenticateType == AuthenticateType.SSO
                    || currentUser.AuthenticateType == AuthenticateType.SSOPartner)
                && filterContext.HttpContext.Request.Headers["X-Requested-With"] != "XMLHttpRequest")
            {

                //using (var ssoService = new EmployeeServices.EmployeeServiceWebServiceV2Service())
                if (currentUser.AuthenticateType == AuthenticateType.SSO)
                {
                    _Logger.Info("Normal SSOSynchronization");
                    SSOSynchronization(filterContext, currentUser);
                }
                else if (currentUser.AuthenticateType == AuthenticateType.SSOPartner)
                {
                    _Logger.Info("Partner SSOSynchronization");
                    PartnerSSOSynchronization(filterContext, currentUser);
                }
                else
                {
                    _Logger.Info("SSOType Not Found");

                    ((UserModel)filterContext.HttpContext.Session[WebConstants.FBBConfigSessionKeys.User])
                            .ForceLogOut = true;

                    filterContext.Result = new RedirectToRouteResult(
                        new RouteValueDictionary(
                            new
                            {
                                controller = "Account",
                                action = "Logout",
                            })
                        );
                    return;
                }
            }
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new HttpUnauthorizedResult();
        }

        private void SSOSynchronization(AuthorizationContext filterContext, UserModel currentUser)
        {
            var ssoError = false;

            _Logger.Info("EmployeeServiceWebServiceV2Service");

            using (var ssoService = new EmployeeServices.EmployeeServiceWebServiceV2Service())
            {
                _Logger.Info(string.Format("Syncing user session to sso, Token:{0}", currentUser.SSOFields.Token));

                try
                {
                    var syncUserSessionResponse = ssoService.syncUserSession(currentUser.SSOFields.Token);

                    if (syncUserSessionResponse.Message.ErrorCode == Constants.SSOReturnStatus.Success)
                    {
                        _Logger.Info("SSO syncUserSession SUCCESS " +
                            string.Format("{0}:{1}",
                                syncUserSessionResponse.Message.ErrorCode,
                                syncUserSessionResponse.Message.ErrorMesg));
                    }
                    else
                    {
                        //sync session ไม่สำเร็จ, force logout
                        _Logger.Info("SSO syncUserSession FAIL " +
                             string.Format("{0}:{1}",
                                 syncUserSessionResponse.Message.ErrorCode,
                                 syncUserSessionResponse.Message.ErrorMesg));

                        _Logger.Info("Sync SSO session ไม่สำเร็จ, force logout");
                        ssoError = true;
                    }
                }
                catch (TimeoutException tex)
                {
                    _Logger.Info("SSO syncUserSession TIMEOUT " + tex.Message);
                    ssoError = true;
                }
                catch (Exception ex)
                {
                    _Logger.Info("SSO syncUserSession ERROR " + ex.Message);
                    ssoError = true;
                }

                if (ssoError)
                {
                    ((UserModel)filterContext.HttpContext.Session[WebConstants.FBBConfigSessionKeys.User])
                            .ForceLogOut = true;

                    filterContext.Result = new RedirectToRouteResult(
                        new RouteValueDictionary(
                            new
                            {
                                controller = "Account",
                                action = "Logout",
                            })
                        );
                    return;
                }
            }
        }

        private void PartnerSSOSynchronization(AuthorizationContext filterContext, UserModel currentUser)
        {
            var ssoError = false;

            _Logger.Info("PartnerServiceWebServiceV2Service");

            using (var ssoService = new PartnerServices.PartnerServiceWebServiceV2Service())
            {
                _Logger.Info(string.Format("Syncing user session to sso, Token:{0}", currentUser.SSOFields.Token));

                try
                {
                    var syncUserSessionResponse = ssoService.syncUserSession(currentUser.SSOFields.Token);

                    if (syncUserSessionResponse.Message.ErrorCode == Constants.SSOReturnStatus.Success)
                    {
                        _Logger.Info("SSO syncUserSession SUCCESS " +
                            string.Format("{0}:{1}",
                                syncUserSessionResponse.Message.ErrorCode,
                                syncUserSessionResponse.Message.ErrorMesg));
                    }
                    else
                    {
                        //sync session ไม่สำเร็จ, force logout
                        _Logger.Info("SSO syncUserSession FAIL " +
                             string.Format("{0}:{1}",
                                 syncUserSessionResponse.Message.ErrorCode,
                                 syncUserSessionResponse.Message.ErrorMesg));

                        _Logger.Info("Sync SSO session ไม่สำเร็จ, force logout");
                        ssoError = true;
                    }
                }
                catch (TimeoutException tex)
                {
                    _Logger.Info("SSO syncUserSession TIMEOUT " + tex.Message);
                    ssoError = true;
                }
                catch (Exception ex)
                {
                    _Logger.Info("SSO syncUserSession ERROR " + ex.Message);
                    ssoError = true;
                }

                if (ssoError)
                {
                    ((UserModel)filterContext.HttpContext.Session[WebConstants.FBBConfigSessionKeys.User])
                            .ForceLogOut = true;

                    filterContext.Result = new RedirectToRouteResult(
                        new RouteValueDictionary(
                            new
                            {
                                controller = "Account",
                                action = "Logout",
                            })
                        );
                    return;
                }
            }
        }
    }
}