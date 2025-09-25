using System.Web.Mvc;
using System.Web.Routing;

namespace WBBWeb
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("favicon.ico");
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
               "Track",                                              // Route name
               "Tracking/Back/{backkey}",                           // URL with parameters
               new { controller = "Tracking", action = "IndexBack", backkey = UrlParameter.Optional }  // Parameter defaults
           );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Process", action = "Index", id = UrlParameter.Optional }
                //defaults: new { controller = "AisRabbit", action = "Index", id = UrlParameter.Optional }
                //defaults: new { controller = "ProfilePrePostPaid", action = "Index", id = UrlParameter.Optional }
                //defaults: new { controller = "Leavemessage", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
               name: "PreRegister",
               url: "{controller}/{action}/{id}",
               defaults: new { controller = "PreRegister", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}