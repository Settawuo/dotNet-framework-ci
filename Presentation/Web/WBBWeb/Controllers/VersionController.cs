using System;
using System.Diagnostics;
using System.Web.Mvc;
using WBBContract;
using WBBEntity.Extensions;
using WBBWeb.QueryService;

namespace WBBWeb.Controllers
{
    public class VersionController : Controller
    {
        private readonly IQueryProcessor _queryProcessor;

        public VersionController(IQueryProcessor queryProcessor)
        {
            _queryProcessor = queryProcessor;
        }

        public ActionResult Index()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);

            string version = fvi.FileVersion + ".17/07/2023-v001";
            ViewBag.WebVersion = version;

            try
            {
                using (var service = new QueryServiceClient())
                {
                    var result = service.Version();
                    ViewBag.AppVersion = result.ToSafeString();
                }
            }
            catch (Exception ex)
            {
                ViewBag.AppVersion = ex.ToSafeString();

            }

            return View();
        }
    }
}
