using ServiceStack.Text;
using System;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace FBBConfig.Extensions
{
    public static class Extensions
    {
        public static string GetTransectionId(this MethodBase MethodName)
        {
            string result = MethodName.Name;
            result += DateTime.Now.ToString("ddMMyyyyhhmmss");

            return result;
        }
    }

    public class ServiceStackJsonResult : JsonResult
    {
        public Newtonsoft.Json.Formatting Formatting { get; set; }
        public override void ExecuteResult(ControllerContext context)
        {
            // todo : implement this code later.
            //if (this.JsonRequestBehavior == JsonRequestBehavior.DenyGet &&
            //    string.Equals(context.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
            //    throw new InvalidOperationException("GET request not allowed");

            var response = context.HttpContext.Response;

            response.ContentType = !string.IsNullOrEmpty(this.ContentType) ?
                this.ContentType : "application/json";

            if (this.ContentEncoding != null)
                response.ContentEncoding = this.ContentEncoding;

            if (this.Data == null)
                return;

            response.Write(JsonSerializer.SerializeToString(this.Data));
        }
    }

    public static class WebSecurity
    {
        public static string Encode(this string encode)
        {
            byte[] encoded = System.Text.Encoding.UTF8.GetBytes(encode);
            return Convert.ToBase64String(encoded);
        }

        public static string Decode(this string decode)
        {
            byte[] decoded = Convert.FromBase64String(decode);
            return System.Text.Encoding.UTF8.GetString(decoded);
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class IENoCacheAttribute : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            //exceptional for old IEs, dont force no cache due to it's bug about no-cache header
            if (IsFileResultAndOldIE(filterContext) == false)
            {
                filterContext.HttpContext.Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
                filterContext.HttpContext.Response.Cache.SetValidUntilExpires(false);
                filterContext.HttpContext.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
                filterContext.HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                filterContext.HttpContext.Response.Cache.SetNoStore();
            }

            base.OnResultExecuting(filterContext);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filterContext"></param>
        /// <returns><c>true</c> for FileResults and if the browser is < IE9</returns>
        private bool IsFileResultAndOldIE(ResultExecutingContext filterContext)
        {
            return ((filterContext.Result is FileResult
                        || filterContext.Result is FileStreamResult
                        || filterContext.Result is EmptyResult) &&
                   (filterContext.HttpContext.Request.IsSecureConnection &&
                   string.Equals(filterContext.HttpContext.Request.Browser.Browser, "IE", StringComparison.OrdinalIgnoreCase) &&
                   filterContext.HttpContext.Request.Browser.MajorVersion < 9));
        }
    }



}