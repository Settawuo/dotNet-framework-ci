using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WBBContract;
using WBBEntity.Extensions;
using WBBEntity.PanelModels;
using WBBWeb.Extension;
using WBBWeb.Models;

namespace WBBWeb.Controllers
{
    [CustomActionFilter]
    [IENoCache]
    public class ProcessSelectController : WBBController
    {

        private readonly IQueryProcessor _queryProcessor;


        public ProcessSelectController(IQueryProcessor queryProcessor)
        {

            _queryProcessor = queryProcessor;

        }

        [AuthorizeUserAttribute]
        public ActionResult Index(bool SaveSuccess = false, string LSAVE = "", string LCLOSE = "",
            string LPOPUPSAVE = "", string LanguagePage = "", string SWiFi = "")//bool SaveSuccess, bool L_SAVE, string LCLOSE = "", string LPOPUPSAVE = "", string LanguagePage="", string SWiFi=""
        {
            return ProcessSelectIndexHandler(SaveSuccess, LSAVE, LPOPUPSAVE, LanguagePage, SWiFi);
        }

        private ActionResult ProcessSelectIndexHandler(bool SaveSuccess, string LSAVE, string LPOPUPSAVE, string LanguagePage, string SWiFi)
        {
            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "AccountService");

            ViewBag.User = base.CurrentUser;

            if (base.CurrentUser.SSOFields != null)
            {
                ViewBag.LC = base.CurrentUser.SSOFields.LocationCode;
            }

            var data = base.LovData
                .Where(l => !string.IsNullOrEmpty(l.Name) && l.Name.Equals("B_CLOSE"))
                .FirstOrDefault();

            if (data != null)
            {
                if (SiteSession.CurrentUICulture.IsThaiCulture())
                {
                    if (data.LovValue1 != null)
                    {
                        ViewBag.LCLOSE = data.LovValue1.ToString();
                        ViewBag.LanguagePage = "1";
                    }
                }
                else
                {
                    if (data.LovValue2 != null)
                    {
                        ViewBag.LCLOSE = data.LovValue2.ToString();
                        ViewBag.LanguagePage = "2";
                    }
                }
            }
            else
            {
                ViewBag.LCLOSE = "";
            }

            if (Session["EndProcessFlag"].ToSafeBoolean())
            {
                Session["PopupStatus"] = "Success";
                Session["EndProcessFlag"] = null;
            }
            else
                Session["PopupStatus"] = null;

            ViewBag.LabelFBBTR001 = GetCoverageScreenConfig();
            ViewBag.LSAVE = LSAVE;
            ViewBag.SWiFi = SWiFi;
            ViewBag.LanguagePage = LanguagePage;
            //ViewBag.LCLOSE = LCLOSE;
            ViewBag.LPOPUPSAVE = LPOPUPSAVE;
            ViewBag.SaveSuccess = SaveSuccess;
            ViewBag.Fbb_Service = GetDisplay_Select_Type_Service();
            ViewBag.LabelFBBTR002 = GetDisplayPackageScreenConfig();
            ViewBag.FbbConstant = GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);
            ViewBag.Vas22 = "Logout";
            ViewBag.Version = GetVersion();
            return View();
        }

        private string GetVersion()
        {
            string version = "";

            var query = new WBBContract.Queries.Commons.Master.GetVersionQuery
            {

            };

            var versionModel = _queryProcessor.Execute(query);

            version = versionModel.InternalServiceVersion;

            return version;
        }

        public List<LovScreenValueModel> GetDisplay_Select_Type_Service()
        {
            var screenData = GetScreenConfig(WebConstants.LovConfigName.SelectType_Service);
            return screenData;
        }

        private List<FbbConstantModel> GetFbbConstantModel(string fbbConstType)
        {
            var data = base.LovData
               .Where(l => l.Type.Equals(fbbConstType))
               .Select(l => new FbbConstantModel
               {
                   Field = l.Name,
                   Validation = l.LovValue1,
                   SubValidation = l.LovValue2
               }).ToList();

            return data;
        }

        public List<LovScreenValueModel> GetScreenConfig(string pageCode)
        {
            try
            {
                List<LovValueModel> config = null;
                if (pageCode == null)
                {
                    config = base.LovData.Where(l => l.LovValue5 == null && l.Type.Equals("SCREEN")).ToList();
                }
                else
                {
                    config = base.LovData.Where(l =>
                        (!string.IsNullOrEmpty(l.Type) && l.Type.Equals(WebConstants.LovConfigName.Screen))
                            && (!string.IsNullOrEmpty(l.LovValue5) && l.LovValue5.Equals(pageCode))).ToList();
                }

                var screenValue = new List<LovScreenValueModel>();
                if (SiteSession.CurrentUICulture.IsThaiCulture())
                {
                    screenValue = config.Select(l => new LovScreenValueModel
                    {
                        Name = l.Name,
                        PageCode = l.LovValue5,
                        DisplayValue = l.LovValue1,
                        GroupByPDF = l.LovValue4,
                        OrderByPDF = l.OrderBy,
                    }).ToList();
                }
                else
                {
                    screenValue = config.Select(l => new LovScreenValueModel
                    {
                        Name = l.Name,
                        PageCode = l.LovValue5,
                        DisplayValue = l.LovValue2,
                        GroupByPDF = l.LovValue4,
                        OrderByPDF = l.OrderBy,
                    }).ToList();
                }

                return screenValue;
            }
            catch (Exception ex)
            {
                Logger.Info(ex.GetErrorMessage());
                return new List<LovScreenValueModel>();
            }
        }
        public List<LovScreenValueModel> GetDisplayPackageScreenConfig()
        {
            var screenData = GetScreenConfig(WebConstants.LovConfigName.DisplayPackagePageCode);
            return screenData;
        }
        public List<LovScreenValueModel> GetCoverageScreenConfig()
        {
            var screenData = GetScreenConfig(WebConstants.LovConfigName.CoveragePageCode);
            return screenData;
        }


        //public JsonResult GetIpAdress()
        //{
        //    var query = new GetCardModelQuery();

        //    var result = _queryProcessor.Execute(query);
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}

    }
}
