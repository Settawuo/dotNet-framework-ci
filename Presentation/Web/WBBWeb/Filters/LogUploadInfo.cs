using System.Web.Mvc;


namespace WBBWeb.Filters
{
    public class LogUploadInfo : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //Logging operation code here

        }
    }
}