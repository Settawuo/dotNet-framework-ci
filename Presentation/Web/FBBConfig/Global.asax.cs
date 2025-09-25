using FBBConfig.Extensions;
using FBBConfig.Solid.CompositionRoot;
using WBBBusinessLayer;

namespace FBBConfig
{
    using System;
    using System.Web.Http;
    using System.Web.Mvc;
    using System.Web.Optimization;
    using System.Web.Routing;

    public class MvcApplication : System.Web.HttpApplication
    {
        public ILogger _logger { get; set; }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            AuthConfig.RegisterAuth();

            ModelBinders.Binders.Add(typeof(RouteValues), new RouteBinder());

            WebApiConfig.Register(GlobalConfiguration.Configuration);

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);

            RouteConfig.RegisterRoutes(RouteTable.Routes);

            BundleTable.EnableOptimizations = true;

            BundleConfig.RegisterBundles(BundleTable.Bundles);

            Bootstrapper.Bootstrap();

            _logger = Bootstrapper.GetInstance<DebugLogger>();
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            // ASP.NET will create a new session for every request unless you store something in it.
            // So the fact that IE doesn't request a new session
            Session["Dummy"] = 1;
            Session["SessionId"] = Session.SessionID;
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

    }
}