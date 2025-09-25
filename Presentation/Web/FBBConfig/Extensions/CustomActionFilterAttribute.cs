using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Routing;
using WBBBusinessLayer;
using WBBEntity.Extensions;

namespace FBBConfig.Extensions
{
    public class CustomActionFilterAttribute : ActionFilterAttribute
    {
        #region Logging
        /// <summary>
        /// Access to the log4Net logging object
        /// </summary>

        //private const string StopwatchKey = "ProfileLoggingStopWatch";
        //protected readonly ILogger _Logger;
        //protected DateTime dateTime;
        private Stopwatch _stopwatch = new Stopwatch();

        private const string LOG_ACTION_PARAMETER = "LOG_ACTION_PARAMETER";
        private const string LOG_START_TIME = "LOG_START_TIME";

        [ImportAttribute]
        public ILogger _Logger { get; set; }
        public string InfoMessage { get; set; }
        public string LogType { get; set; }

        #endregion

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var controllerName = (string)filterContext.RouteData.Values["controller"];
            var actionName = (string)filterContext.RouteData.Values["action"];

            //StringBuilder criteria;
            //if (filterContext == null)
            //    return;

            var session = filterContext.HttpContext.Session;
            //var sessionId = session["SessionId"];
            if (session["Dummy"] == null)
            {
                //Session expired
                _Logger.Info("Dummy Session Expire on " + controllerName + " " + actionName);
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(
                        new
                        {
                            controller = "Account",
                            action = "Logout",
                        })
                    );
            }

            if (null == session[WebConstants.FBBConfigSessionKeys.User]
                && controllerName != "Account")
            {
                _Logger.Info("User Session Expire on " + controllerName + " " + actionName);

                filterContext.Result = new RedirectToRouteResult(
                     new RouteValueDictionary(
                         new
                         {
                             controller = "Account",
                             action = "Logout",
                         })
                     );
            }

        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            //WebConstants.LogTypeKeys.ViewData.ToString();
            //if (filterContext == null)
            //{
            //    return;
            //}

            //var wfController = filterContext.Controller as iWorkflowsController;
            //if (wfController != null)
            //{
            //    if (_Logger.IsInfoEnabled)
            //    {
            //        SettupLog4netGlobalContext(filterContext);
            //        string msgInfo = InfoMessage;
            //        string msgDetail = filterContext.HttpContext.Items[LOG_ACTION_PARAMETER].ToString();

            //        string msgSummary = InfoMessage + " (" + msgDetail + ")";

            //        //var message = new StringBuilder();
            //        //message.Append(msgSummary);
            //        _Logger.Info(msgSummary);
            //    }
            //}
        }

        protected void SettupLog4netGlobalContext(ControllerContext filterContext)
        {
            //var wfController = filterContext.Controller as iWorkflowsController;
            int elaspedTime;

            //if (wfController != null)
            //{
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
            //log4net.GlobalContext.Properties["URL"] = "Dew";
            log4net.GlobalContext.Properties["LOG_SOURCE"] = "Web";
            log4net.GlobalContext.Properties["LOG_TYPE"] = LogType;

            if (!String.IsNullOrEmpty(filterContext.HttpContext.Items[LOG_START_TIME].ToString()))
            {
                //DateTime dateTime = Convert.ToDateTime(filterContext.HttpContext.Items[LOG_START_TIME]);
                var dateTime = filterContext.HttpContext.Items[LOG_START_TIME].ToSafeString().ToDateTime();
                CultureInfo en = new CultureInfo("en-GB");

                var dateTimeString = dateTime.GetValueOrDefault().ToString("dd/MM/yyyy HH:mm:ss", en);
                log4net.GlobalContext.Properties["DATETIME_START"] = dateTimeString;

                var dateTimeString2 = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", en);

                log4net.GlobalContext.Properties["DATETIME_END"] = dateTimeString2;

                //log4net.GlobalContext.Properties["ELASPED_TIME"] = Convert.ToDecimal(timeEnd.Subtract(dateTime).Seconds);
                _stopwatch.Stop();
                elaspedTime = _stopwatch.Elapsed.Seconds;
                log4net.GlobalContext.Properties["ELASPED_TIME"] = Convert.ToDecimal(elaspedTime);
            }
            //}
        }
    }
}