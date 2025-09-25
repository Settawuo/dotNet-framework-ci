using System.Web.Mvc;
using WBBBusinessLayer;
using WBBContract;
using WBBWeb.Extension;

namespace WBBWeb.Controllers
{
    [CustomActionFilter]
    [CustomHandleError]
    [IENoCache]
    public class TestController : WBBController
    {
        //
        // GET: /Testja/

        private readonly IQueryProcessor _queryProcessor;

        public TestController(IQueryProcessor queryProcessor,
            ILogger logger)
        {
            _queryProcessor = queryProcessor;
            base.Logger = logger;
        }

        public ActionResult test()
        {
            return View();
        }

        public ActionResult searchApi()
        {
            return View();
        }

        public ActionResult mapTracking()
        {
            return View();
        }

        //R22.08
        public ActionResult createEncrypBypass()
        {
            return View();
        }

        //R22.08
        public JsonResult EncryptBypass(string inParam)
        {
            var tmpEncrypt = Encrypt(inParam);
            return Json(new { result = tmpEncrypt }, JsonRequestBehavior.AllowGet);
        }

    }
}
