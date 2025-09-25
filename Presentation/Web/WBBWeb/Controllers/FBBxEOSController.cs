using System;
using System.Linq;
using System.Web.Mvc;
using WBBContract;
using WBBContract.Queries.ExWebServices;
using WBBContract.Queries.WebServices;
using WBBEntity.Extensions;
using WBBEntity.PanelModels;
using WBBEntity.PanelModels.ExWebServiceModels;
using WBBEntity.PanelModels.WebServiceModels;
using WBBWeb.Extension;
using WBBWeb.Models;

namespace WBBWeb.Controllers
{
    [CustomActionFilter]
    [CustomHandleError]
    [IENoCache]
    public class FBBxEOSController : WBBController
    {
        private readonly IQueryProcessor _queryProcessor;
        public FBBxEOSController(IQueryProcessor queryProcessor)
        {
            _queryProcessor = queryProcessor;
        }
        //
        // GET: /FBBxEOS/
        [HttpPost]
        public ActionResult Index(string Language = "", string PromotionCode = "")
        {
            if (String.IsNullOrEmpty(Language))
                Language = "T";

            if (Language == "T")
                ChangeCurrentCulture(1);
            else
                ChangeCurrentCulture(2);

            var controller = DependencyResolver.Current.GetService<ProcessController>();
            ViewBag.LabelFBBTR001 = controller.GetCoverageScreenConfig();
            ViewBag.LabelFBBOR015 = controller.GetProfilePrePostPaid();
            ViewBag.FbbConstant = controller.GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);
            ViewBag.PromotionCode = PromotionCode;
            ViewBag.Language = Language;
            return View();
        }

        [HttpPost]
        public ActionResult LoadCheckCoverage(string PromotionCode, string L_CARD_NOs, string Language)
        {
            QuickWinPanelModel model = new QuickWinPanelModel();

            string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress))
            {
                ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }

            var controller = DependencyResolver.Current.GetService<ProcessController>();
            ViewBag.LabelFBBTR000 = controller.GetGeneralScreenConfig();
            ViewBag.LabelFBBTR001 = controller.GetCoverageScreenConfig();
            ViewBag.LabelFBBTR002 = controller.GetDisplayPackageScreenConfig();
            ViewBag.LabelFBBTR003 = controller.GetCustRegisterScreenConfig();
            ViewBag.LabelFBBTR004 = controller.GetSummaryScreenConfig();
            ViewBag.LabelFBBTR010 = controller.GetVas_Select_Package_ScreenConfig();
            ViewBag.LabelFBBORV11 = controller.GetVasPopUpScreenConfig();
            ViewBag.LabelFBBOR015 = controller.GetProfilePrePostPaid();
            ViewBag.LabelFBBORV25 = controller.GetSelectRouter();
            ViewBag.FbbConstant = controller.GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);
            ViewBag.FbbException = controller.GetFbbConstantModel(WebConstants.LovConfigName.FbbExceptionIdCard);
            ViewBag.Fbb_Service = controller.GetDisplay_Select_Type_Service();
            ViewBag.Version = controller.GetVersion();
            ViewBag.Vas = "";
            ViewBag.User = base.CurrentUser;
            ViewBag.PlugandplayMessage = controller.GetPlugandplayMessage();
            ViewBag.ContentPlaybox = controller.GetLovConfigBytype("NEW_CONTENT_PLAYBOX");
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
                    }
                }
                else
                {
                    if (data.LovValue2 != null)
                    {
                        ViewBag.LCLOSE = data.LovValue2.ToString();
                    }
                }
            }
            else
            {
                ViewBag.LCLOSE = "";
            }
            ViewBag.FromFBBxEOS = "Y";
            ViewBag.PromotionCodeFBBxEOS = PromotionCode;
            ViewBag.CARD_NOsFBBxEOS = L_CARD_NOs;
            ViewBag.LanguageEOS = Language;
            model.ClientIP = ipAddress;
            return View("~/Views/Process/Index.cshtml", model);
        }

        [HttpPost]
        public JsonResult QueryDBDProfile(string TAX_ID)
        {
            QueryDBDProfileQuery query = new QueryDBDProfileQuery()
            {
                //TAX_ID = "01055541313"
                TAX_ID = TAX_ID.ToSafeString()
            };

            QueryDBDProfileModel result = _queryProcessor.Execute(query);
            if (result == null)
            {
                result = new QueryDBDProfileModel()
                {
                    ResultCode = "0001",
                    ResultMessage = "No Data Found"
                };
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult evAMQueryCustomerProfileByTaxId(string TAX_ID)
        {
            evAMQueryCustomerProfileByTaxIdQuery query = new evAMQueryCustomerProfileByTaxIdQuery()
            {
                //TAX_ID = "5217584974537",
                TAX_ID = TAX_ID.ToSafeString(),
            };

            evAMQueryCustomerProfileByTaxIdModel result = _queryProcessor.Execute(query);
            if (result == null)
            {
                result = new evAMQueryCustomerProfileByTaxIdModel()
                {
                    errorCode = "0001",
                    errorMessage = "No Data Found"
                };
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

    }
}
