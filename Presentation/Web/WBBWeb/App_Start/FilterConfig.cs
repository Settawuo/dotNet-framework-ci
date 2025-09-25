using System.Web.Mvc;
using WBBWeb.Extension;

namespace WBBWeb
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new ETagAttribute());
            filters.Add(new CompressAttribute());
        }
    }
}