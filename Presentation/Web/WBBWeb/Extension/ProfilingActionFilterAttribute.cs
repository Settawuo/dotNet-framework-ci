using System.Diagnostics;
using System.Web.Mvc;
using WBBBusinessLayer;
using WBBWeb.CompositionRoot;
using WBBWeb.Solid.CompositionRoot;

namespace WBBWeb.Extension
{
    public class ProfilingActionFilterAttribute : CustomActionFilterAttribute
    {
        protected Stopwatch timer;
        private ILogger _Logger = Bootstrapper.GetInstance<DebugLogger>();

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            timer = Stopwatch.StartNew();
            base.OnActionExecuting(filterContext);
        }

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            base.SettupLog4netGlobalContext(filterContext);
            timer.Stop();
            _Logger.Info(string.Format("Total elapsed time: {0} seconds.", timer.Elapsed.TotalSeconds));
        }
    }
}