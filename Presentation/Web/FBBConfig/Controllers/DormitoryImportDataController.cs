using Kendo.Mvc.Extensions;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using WBBEntity.PanelModels.FBBWebConfigModels;
namespace FBBConfig.Controllers
{
    public class DormitoryImportData : FBBConfigController
    {
        //
        // GET: /DormitoryImportData/
        public ActionResult Index()
        {
            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "Account");
            ViewBag.User = base.CurrentUser;
            SetViewBagLov();
            return View();
        }
        private void SetViewBagLov()
        {
            var LovDataScreen = base.LovData.Where(p => p.Type == "FBBDORM_ADMIN_SCREEN" && p.LovValue5 == "ADMIN_FBBDORM001").ToList();
            ViewBag.configscreen = LovDataScreen;
            // ViewBag.DormConstant = GetFbbConstantModel(WebConstants.LovConfigName.DormConstants);
        }
        public ActionResult clearSession()
        {
            var addedit = Session["tempupload"] as List<ImportDormDataModels>;
            if (addedit != null)
                Session.Remove("tempupload");

            var filename = Session["filename"];
            if (filename != null)
                Session.Remove("filename");
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        public ActionResult total()
        {

            string ttr = string.Empty;

            var tempupload = Session["tempupload"] as List<ImportDormDataModels>;


            ttr = tempupload.Count.ToString();



            return Json(new { ttr = ttr }, JsonRequestBehavior.AllowGet);
        }
    }
}
