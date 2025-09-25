using Kendo.Mvc.UI;
using System.Web.Mvc;
using System.Web.Routing;
using WBBBusinessLayer;
using WBBWeb.CompositionRoot;
using WBBWeb.Solid.CompositionRoot;

namespace WBBWeb.Extension
{
    public class SessionExpireFilterAttribute : ActionFilterAttribute
    {
        private readonly ILogger _logger = Bootstrapper.GetInstance<DebugLogger>();

        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            if (null == filterContext.HttpContext.Session[WebConstants.FBBConfigSessionKeys.User])
            {
                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {

                    var jsonResult = filterContext.Result as JsonResult;
                    if (jsonResult == null) return;
                    jsonResult.Data = new DataSourceResult { Errors = "Timeout" };

                }
                else
                {
                    //Redirects user to login screen if session has timed out and request is non AJAX
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                    {
                        controller = "Fbbsaleportal",
                        action = "Login",
                        returnUrl = filterContext.HttpContext.Request.Url
                    }));
                }
            }

            base.OnResultExecuting(filterContext);
        }

    }
}