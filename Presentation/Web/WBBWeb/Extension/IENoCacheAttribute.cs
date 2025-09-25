using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Web;
using System.Web.Mvc;

namespace WBBWeb.Extension
{
    /// <summary>
    /// force browser not do page caching
    /// </summary>
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

    public class CompressAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var request = filterContext.HttpContext.Request;

            string acceptEncoding = request.Headers["Accept-Encoding"];

            if (string.IsNullOrEmpty(acceptEncoding)) return;

            acceptEncoding = acceptEncoding.ToUpperInvariant();

            var response = filterContext.HttpContext.Response;

            if (acceptEncoding.Contains("GZIP"))
            {
                response.AppendHeader("Content-encoding", "gzip");
                response.Filter = new GZipStream(response.Filter, CompressionMode.Compress);
            }
            else if (acceptEncoding.Contains("DEFLATE"))
            {
                response.AppendHeader("Content-encoding", "deflate");
                response.Filter = new DeflateStream(response.Filter, CompressionMode.Compress);
            }
        }
    }

    public class CSharpRazorViewEngine : RazorViewEngine
    {
        public CSharpRazorViewEngine()
        {
            AreaViewLocationFormats = new[]
             {
             "~/Areas/{2}/Views/{1}/{0}.cshtml",
             "~/Areas/{2}/Views/Shared/{0}.cshtml"
             };
            AreaMasterLocationFormats = new[]
             {
             "~/Areas/{2}/Views/{1}/{0}.cshtml",
             "~/Areas/{2}/Views/Shared/{0}.cshtml"
             };
            AreaPartialViewLocationFormats = new[]
             {
             "~/Areas/{2}/Views/{1}/{0}.cshtml",
             "~/Areas/{2}/Views/Shared/{0}.cshtml"
             };
            ViewLocationFormats = new[]
             {
             "~/Views/{1}/{0}.cshtml",
             "~/Views/Shared/{0}.cshtml"
             };
            MasterLocationFormats = new[]
             {
             "~/Views/{1}/{0}.cshtml",
             "~/Views/Shared/{0}.cshtml"
             };
            PartialViewLocationFormats = new[]
             {
             "~/Views/{1}/{0}.cshtml",
             "~/Views/Shared/{0}.cshtml"
             };
        }
    }

    public class TwoLevelViewCache : IViewLocationCache
    {
        private readonly static object s_key = new object();
        private readonly IViewLocationCache _cache;

        public TwoLevelViewCache(IViewLocationCache cache)
        {
            _cache = cache;
        }

        private static IDictionary<string, string> GetRequestCache(HttpContextBase httpContext)
        {
            var d = httpContext.Items[s_key] as IDictionary<string, string>;
            if (d == null)
            {
                d = new Dictionary<string, string>();
                httpContext.Items[s_key] = d;
            }
            return d;
        }

        public string GetViewLocation(HttpContextBase httpContext, string key)
        {
            var d = GetRequestCache(httpContext);
            string location;
            if (!d.TryGetValue(key, out location))
            {
                location = _cache.GetViewLocation(httpContext, key);
                d[key] = location;
            }
            return location;
        }

        public void InsertViewLocation(HttpContextBase httpContext, string key, string virtualPath)
        {
            _cache.InsertViewLocation(httpContext, key, virtualPath);
        }
    }

    public class ETagAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                filterContext.HttpContext.Response.Filter = new ETagFilter(filterContext.HttpContext.Response);

                HttpCachePolicyBase cache = filterContext.HttpContext.Response.Cache;
                TimeSpan cacheDuration = TimeSpan.FromSeconds(60);

                cache.SetCacheability(HttpCacheability.Public);
                cache.SetExpires(DateTime.Now.Add(cacheDuration));
                cache.SetMaxAge(cacheDuration);
                cache.AppendCacheExtension("must-revalidate, proxy-revalidate");
            }
            catch (System.Exception)
            {
                // Do Nothing
            };
        }
    }

    public class ETagFilter : MemoryStream
    {
        private HttpResponseBase o = null;
        private Stream filter = null;

        public ETagFilter(HttpResponseBase response)
        {
            o = response;
            filter = response.Filter;
        }

        private string GetToken(Stream stream)
        {
            byte[] checksum;
            checksum = System.Security.Cryptography.MD5.Create().ComputeHash(stream);
            return Convert.ToBase64String(checksum, 0, checksum.Length);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            byte[] data = new byte[count];
            Buffer.BlockCopy(buffer, offset, data, 0, count);
            filter.Write(data, 0, count);
            o.AddHeader("ETag", GetToken(new MemoryStream(data)));
        }
    }
}