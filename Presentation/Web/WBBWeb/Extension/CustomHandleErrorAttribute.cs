using System;
using System.Web;
using System.Web.Mvc;
using WBBBusinessLayer;
using WBBWeb.CompositionRoot;
using WBBWeb.Solid.CompositionRoot;

namespace WBBWeb.Extension
{
    public class CustomHandleErrorAttribute : HandleErrorAttribute
    {
        private ILogger _Logger = Bootstrapper.GetInstance<DebugLogger>();

        public override void OnException(ExceptionContext filterContext)
        {
            SettupLog4netGlobalContext(filterContext);

            _Logger.Error(filterContext.Exception.Message);
            _Logger.Error(filterContext.Exception.StackTrace);

            if (filterContext.ExceptionHandled || !filterContext.HttpContext.IsCustomErrorEnabled)
            {
                return;
            }

            if (new HttpException(null, filterContext.Exception).GetHttpCode() != 500)
            {
                return;
            }

            if (!ExceptionType.IsInstanceOfType(filterContext.Exception))
            {
                return;
            }

            // if the request is AJAX return JSON else view.
            if (filterContext.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                filterContext.Result = new JsonResult
                {
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    Data = new
                    {
                        error = true,
                        message = filterContext.Exception.Message
                    }
                };
            }
            else
            {
                var controllerName = (string)filterContext.RouteData.Values["controller"];
                var actionName = (string)filterContext.RouteData.Values["action"];
                var model = new HandleErrorInfo(filterContext.Exception, controllerName, actionName);

                filterContext.Result = new ViewResult
                {
                    ViewName = View,
                    MasterName = Master,
                    ViewData = new ViewDataDictionary<HandleErrorInfo>(model),
                    TempData = filterContext.Controller.TempData
                };
            }

            filterContext.ExceptionHandled = true;
            filterContext.HttpContext.Response.Clear();
            filterContext.HttpContext.Response.StatusCode = 500;
            filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
        }

        protected void SettupLog4netGlobalContext(ExceptionContext filterContext)
        {
            ControllerContext ctx = new ControllerContext(filterContext.RequestContext, filterContext.Controller);

            // paramter of properties must be case sensitive.
            log4net.ThreadContext.Properties["GUID"] = Guid.NewGuid().ToString();
            log4net.GlobalContext.Properties["STATUS"] = "";
            log4net.GlobalContext.Properties["STATUS_REASON"] = "";
            string ip = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ip))
            {
                ip = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
            log4net.GlobalContext.Properties["IP_ADDRESS"] = ip;

            log4net.GlobalContext.Properties["URL"] = System.Web.HttpContext.Current.Request.Url;
            log4net.GlobalContext.Properties["LOG_SOURCE"] = "Web";
            log4net.GlobalContext.Properties["LOG_TYPE"] = "Exception";
        }
    }
}