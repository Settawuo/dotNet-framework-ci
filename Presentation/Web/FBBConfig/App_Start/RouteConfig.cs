using System.Web.Mvc;
using System.Web.Routing;

namespace FBBConfig
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("quartz/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Account", action = "Login", id = UrlParameter.Optional }
                //defaults: new { controller = "Coverage_Search", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                "QuartzManager",                                              // Route name
                "quartz/*",                           // URL with parameters
                new { controller = "Home", action = "QuartzManager" }  // Parameter defaults
            );
        }
    }
}