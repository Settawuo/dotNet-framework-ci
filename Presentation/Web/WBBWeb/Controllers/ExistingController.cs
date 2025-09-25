using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WBBWeb.Controllers.ConfigLovs;
using WBBWeb.Extension;

namespace WBBWeb.Controllers
{
    [CustomHandleError]
    [IENoCache]
    [CustomActionFilter]
    public class ExistingController : WBBController
    {
        private ConfigLovHelpers _configLovHelpers;

        //
        // GET: /Existing/
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Login()
        {
            _configLovHelpers = new ConfigLovHelpers();

            ViewBag.LabelFBBTR001 = _configLovHelpers.GetLovScreenByPageCode(WebConstants.LovConfigName.CoveragePageCode);
            ViewBag.LabelFBBTR003 = _configLovHelpers.GetLovScreenByPageCode(WebConstants.LovConfigName.CustomerRegisterPageCode);
            ViewBag.LabelFBBOR015 = _configLovHelpers.GetLovScreenByPageCode(WebConstants.LovConfigName.CheckPrePostPaid);
            ViewBag.LabelFBBOR019 = _configLovHelpers.GetLovScreenByPageCode(WebConstants.LovConfigName.PaymentPageCode);
            ViewBag.FbbConstant = _configLovHelpers.GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string custInternetNum, string custIdCardNum, string custIdCardType, string languagePage)
        {
            return Json(new { result = "Redirect", url = Url.Action("Index", "Existing") });
        }
    }
}
