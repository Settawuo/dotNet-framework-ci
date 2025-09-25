using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WBBContract;
using WBBEntity.PanelModels;
using WBBWeb.Controllers;
using WBBWeb.Solid.CompositionRoot;

namespace WBBWeb.Extension
{
    public class JsonNetResult : ActionResult
    {
        public Encoding ContentEncoding { get; set; }

        public string ContentType { get; set; }

        public object Data { get; set; }

        public JsonSerializerSettings SerializerSettings { get; set; }

        public Newtonsoft.Json.Formatting Formatting { get; set; }

        public JsonNetResult()
        {
            SerializerSettings = new JsonSerializerSettings();
        }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            HttpResponseBase response = context.HttpContext.Response;

            response.ContentType = !string.IsNullOrEmpty(ContentType) ? ContentType : "application/json";

            if (ContentEncoding != null)
                response.ContentEncoding = ContentEncoding;

            if (Data != null)
            {
                var writer = new JsonTextWriter(response.Output) { Formatting = Formatting };
                var serializer = JsonSerializer.Create(SerializerSettings);
                serializer.Serialize(writer, Data);
                writer.Flush();
            }
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

    public static class WebExtension
    {
        //public static bool IsThaiCulture(this int currentCulture)
        //{
        //    if (currentCulture == 1)
        //        return true;

        //    return false;
        //}

        //public static bool IsEngCulture(this int currentCulture)
        //{
        //    if (currentCulture == 2)
        //        return true;

        //    return false;
        //}
        private static IQueryProcessor _queryProcessor;

        public static List<LovValueModel> Get_TitleName()
        {
            var masterController = Bootstrapper.GetInstance<MasterDataController>();
            var _LovData = (List<LovValueModel>)masterController.GetLovList("SCREEN", "L_TITLE_BAR").ToList();

            return _LovData;
        }
    }



}

namespace System.Web.Optimization
{
    using System.Text.RegularExpressions;

    /// <summary>
    /// Fixes for the standard System.Web.Optimization.CssRewriteUrlTransform. 
    /// Now plays nice with:
    ///  * Data URIs, including svgs (https://aspnetoptimization.codeplex.com/workitem/88)
    ///  * URLs to other resources that are already absolute 
    ///  * Virtual directories (http://aspnetoptimization.codeplex.com/workitem/83)
    /// </summary>
    public class CssRewriteUrlTransformFixed : IItemTransform
    {
        private static string RebaseUrlToAbsolute(string baseUrl, string url, string prefix, string suffix)
        {
            if (string.IsNullOrWhiteSpace(url) || string.IsNullOrWhiteSpace(baseUrl) || url.StartsWith("/", StringComparison.OrdinalIgnoreCase)
                 || url.StartsWith("http://") || url.StartsWith("https://"))
            {
                return url;
            }

            if (url.StartsWith("data:"))
            {
                // Keep the prefix and suffix quotation chars as is in case they are needed (e.g. non-base64 encoded svg)
                return prefix + url + suffix;
            }

            if (!baseUrl.EndsWith("/", StringComparison.OrdinalIgnoreCase))
            {
                baseUrl += "/";
            }

            return VirtualPathUtility.ToAbsolute(baseUrl + url);
        }
        private static string ConvertUrlsToAbsolute(string baseUrl, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return content;
            }

            var regex = new Regex("url\\((?<prefix>['\"]?)(?<url>[^)]+?)(?<suffix>['\"]?)\\)");

            return regex.Replace(content, (Match match) => "url(" + CssRewriteUrlTransformFixed.RebaseUrlToAbsolute(baseUrl, match.Groups["url"].Value, match.Groups["prefix"].Value, match.Groups["suffix"].Value) + ")");
        }
        public string Process(string includedVirtualPath, string input)
        {
            if (includedVirtualPath == null)
            {
                throw new ArgumentNullException("includedVirtualPath");
            }
            if (includedVirtualPath.Length < 1 || includedVirtualPath[0] != '~')
            {
                throw new ArgumentException("includedVirtualPath must be valid ( i.e. have a length and start with ~ )");
            }

            var directory = VirtualPathUtility.GetDirectory(includedVirtualPath);

            return CssRewriteUrlTransformFixed.ConvertUrlsToAbsolute(directory, input);
        }
    }
}