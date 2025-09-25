namespace System.Web.Mvc.Html
{
    public static class HtmlHelperExtension
    {
        public static void RenderPartialWithPrefix(this HtmlHelper helper, string partialViewName, object model, ViewDataDictionary viewDataDict, string prefix)
        {
            var viewData = new ViewDataDictionary
            {
                TemplateInfo = new System.Web.Mvc.TemplateInfo { HtmlFieldPrefix = prefix }
            };
            foreach (var item in viewDataDict)
            {
                viewData.Add(item);
            }

            helper.RenderPartial(partialViewName, model, viewData);
        }
    }

    public static class OptimizationExtensions
    {
        public static MvcHtmlString CssMinify(
            this HtmlHelper helper,
            Func<object, object> markup)
        {
            string notMinifiedCss =
                (markup.DynamicInvoke(helper.ViewContext) ?? "").ToString();
#if (!DEBUG)
            
            var minifier = new Microsoft.Ajax.Utilities.Minifier();
            var minifiedJs = minifier.MinifyStyleSheet(notMinifiedCss);
            return new MvcHtmlString(minifiedJs);
#else
            return new MvcHtmlString(notMinifiedCss);
#endif
        }

        public static MvcHtmlString JsMinify(
            this HtmlHelper helper,
            Func<object, object> markup)
        {
            string notMinifiedJs =
                (markup.DynamicInvoke(helper.ViewContext) ?? "").ToString();
#if (!DEBUG)
            
            var minifier = new Microsoft.Ajax.Utilities.Minifier();
            var minifiedJs = minifier.MinifyJavaScript(notMinifiedJs);
            return new MvcHtmlString(minifiedJs);
#else
            return new MvcHtmlString(notMinifiedJs);
#endif
        }
    }
}