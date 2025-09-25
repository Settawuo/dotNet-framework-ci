using Notify;
using System;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using WBBBusinessLayer;
using WBBWeb.CompositionRoot;
using WBBWeb.Controllers;
using WBBWeb.Extension;
using WBBWeb.Solid.CompositionRoot;

namespace WBBWeb
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public ILogger _logger { get; set; }

        protected void Application_BeginRequest()
        {
            //Response.Cache.SetCacheability(HttpCacheability.NoCache);
            //Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            //Response.Cache.SetNoStore();
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception exception = Server.GetLastError();
            Response.Clear();

            LineNotify.SendMessage(WebConstants.NotifyKey.LineNotifyFBB, this.GetType().Name, exception);

            HttpException httpException = exception as HttpException;

            if (httpException != null)
            {
                string action;

                switch (httpException.GetHttpCode())
                {
                    case 404:
                        action = "HttpError404";
                        break;
                    case 500:
                        action = "HttpError500";
                        break;
                    default:
                        action = "General";
                        break;
                }

                Server.ClearError();

                Response.Redirect(String.Format("~/Error/{0}/?message={1}", action, exception.Message));
            }
        }

        protected void Application_Start()
        {
            AuthConfig.RegisterAuth();
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            WebApiConfig.Register(GlobalConfiguration.Configuration);

            // Remove X-AspNetMvc-Version
            MvcHandler.DisableMvcResponseHeader = true;

            // Remove all view engine
            ViewEngines.Engines.Clear();

            // Add Razor view Engine
            var ve = new CSharpRazorViewEngine();
            ve.ViewLocationCache = new TwoLevelViewCache(ve.ViewLocationCache);
            ViewEngines.Engines.Add(ve);

            BundleConfig.RegisterBundles(BundleTable.Bundles);

            Bootstrapper.Bootstrap();

            _logger = Bootstrapper.GetInstance<DebugLogger>();

            LineNotify.SendMessage(WebConstants.NotifyKey.LineNotifyFBB, this.GetType().Name, "Application Start");
            _logger.Info("Application Start");
            var WBBController = Bootstrapper.GetInstance<WBBController>();
            WBBController.LoadLOV();
        }

        protected void Application_Stop()
        {
            Session.Abandon();
        }

        protected void Session_End(object sender, EventArgs e)
        {
            //Clean-up Code
            //var _logger = DependencyResolver.Current.GetService<DebugLogger>();
            //Bootstrapper.GetInstance<DebugLogger>().Info("Session_end");
            _logger = Bootstrapper.GetInstance<DebugLogger>();
            _logger.Info("Session_End");
            //FormsAuthentication.SignOut();
            //Response.RedirectToRoute("Default");
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            // ASP.NET will create a new session for every request unless you store something in it.
            // So the fact that IE doesn't request a new session
            Session["Dummy"] = 1;
            Session["SessionId"] = Session.SessionID;

            // 1 is thai
            Session[WebConstants.SessionKeys.CurrentUICulture] = 1;

            #region Set IP Address to Log4Net

            string ip = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (string.IsNullOrEmpty(ip))
            {
                ip = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }

            if (string.IsNullOrEmpty(ip))
            {
                ip = HttpContext.Current.Request.UserHostAddress;
            }
            log4net.GlobalContext.Properties["IP_ADDRESS"] = ip;

            #endregion Set IP Address to Log4Net
        }
    }
}