using log4net;
using System;
using System.Diagnostics;
using System.Web.Mvc;
using System.Web.Routing;
using WBBWeb.CompositionRoot;
using WBBWeb.Solid.CompositionRoot;

namespace WBBWeb.Extension.MvcAttribute
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class SessionExpireCheckAttribute : ActionFilterAttribute
    {
        #region Loggings

        private Stopwatch _stopwatch = new Stopwatch();

        private ILog _Logger = Bootstrapper.GetInstance<DebugLogger>();

        public string InfoMessage { get; set; }

        public string LogType { get; set; }

        #endregion Logging

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var ctx = filterContext.HttpContext;
            // check if session is supported
            if (ctx.Session != null)
            {
                // check if a new session id was generated
                if (ctx.Session.IsNewSession)
                {
                    // If it says it is a new session, but an existing cookie exists, then it must
                    // have timed out
                    var cookie = filterContext.HttpContext.Request.Headers["Cookie"];
                    if ((cookie != null) && (cookie.IndexOf("ASP.NET_SessionId") >= 0))
                    {
                        _Logger.Info("Session Is Expired : " + ctx.Session.SessionID);
                        filterContext.Result = new RedirectToRouteResult(
                                                    new RouteValueDictionary(
                                                        new { controller = "FirstPage", action = "Index", }));

                        // need review
                        //var listOfSessionItemKey = new List<string>();
                        //foreach (var appKey in HttpContext.Current.Application.Keys)
                        //{
                        //    listOfSessionItemKey.Add(appKey.ToString());
                        //}

                        //foreach (var sessAppKey in listOfSessionItemKey)
                        //{
                        //    HttpContext.Current.Application.Remove(sessAppKey);
                        //}

                        return;
                    }
                }
            }

            base.OnActionExecuting(filterContext);

            //var controllerName = (string)filterContext.RouteData.Values["controller"];
            //var actionName = (string)filterContext.RouteData.Values["action"];

            //if (filterContext == null)
            //    return;

            //var session = filterContext.HttpContext.Session;

            //if (session["Dummy"] == null)
            //{
            //    //Session expired
            //    _Logger.Info("Dummy Session Expire on " + controllerName + " " + actionName);
            //    filterContext.Result = new RedirectToRouteResult(
            //        new RouteValueDictionary(
            //            new
            //            {
            //                controller = "First",
            //                action = "Index",
            //            })
            //        );
            //}
        }
    }
}