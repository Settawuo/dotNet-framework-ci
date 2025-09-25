using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Queries.WebServices;
using WBBEntity.Extensions;
using WBBEntity.PanelModels;
using WBBEntity.PanelModels.WebServiceModels;
using WBBWeb.Extension;
using WBBWeb.Models;
namespace WBBWeb.Controllers
{
    public class DormitoryMasterController : WBBController
    {
        private readonly IQueryProcessor _queryProcessor;
        public DormitoryMasterController(IQueryProcessor queryProcessor, ILogger logger)
        {
            _queryProcessor = queryProcessor;
            base.Logger = logger;
        }

        public ActionResult Index()
        {
            var langFlg = Session[WebConstants.SessionKeys.CurrentUICulture].ToSafeString() == "1" ? "TH" : "EN";
            ViewBag.labelFBBDORM008 = GetScreenConfig();
            ViewBag.Language = Session[WebConstants.SessionKeys.CurrentUICulture].ToSafeString() == "1" ? "TH" : "EN";
            ViewBag.labelFBBORV00 = GetScreenConfig("FBBORV00");
            // ViewBag.LabelFBBTR003 = GetCustRegisterScreenConfig();
            return View();
        }
        public List<LovScreenValueModel> GetScreenConfig()
        {
            try
            {
                var langFlg = Session[WebConstants.SessionKeys.CurrentUICulture].ToSafeString() == "1" ? "TH" : "EN";
                List<LovValueModel> config = null;
                ViewBag.FbbConstant = GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);
                config = base.LovData.Where(l =>
                    (!string.IsNullOrEmpty(l.Type) && l.Type == "FBBDORM_SCREEN")
                        && (!string.IsNullOrEmpty(l.LovValue5) && l.LovValue5.Equals("FBBDORM008"))).ToList();
                var screenValue = new List<LovScreenValueModel>();
                //if (SiteSession.CurrentUICulture.IsThaiCulture())

                if (langFlg == "TH")
                {
                    screenValue = config.Select(l => new LovScreenValueModel
                    {
                        Name = l.Name,
                        PageCode = l.LovValue5,
                        DisplayValue = l.LovValue1,
                        LovValue3 = l.LovValue3,
                        GroupByPDF = l.LovValue4,
                        OrderByPDF = l.OrderBy,
                        Type = l.Type,
                        DefaultValue = l.DefaultValue
                    }).ToList();
                }
                else
                {
                    screenValue = config.Select(l => new LovScreenValueModel
                    {
                        Name = l.Name,
                        PageCode = l.LovValue5,
                        DisplayValue = l.LovValue2,
                        LovValue3 = l.LovValue3,
                        GroupByPDF = l.LovValue4,
                        OrderByPDF = l.OrderBy,
                        Type = l.Type,
                        DefaultValue = l.DefaultValue
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
        public List<LovScreenValueModel> GetScreenConfig(string pageCode)
        {
            try
            {
                List<LovValueModel> config = null;
                if (pageCode == null)
                {
                    config = base.LovData.Where(l => l.LovValue5 == null && l.Type == "SCREEN" || l.Type == "VAS_CODE_CONFIG").ToList();
                }
                else
                {
                    config = base.LovData.Where(l =>
                        (!string.IsNullOrEmpty(l.Type) && l.Type == "SCREEN" || l.Type == "VAS_CODE_CONFIG")
                            && (!string.IsNullOrEmpty(l.LovValue5) && l.LovValue5.Equals(pageCode))).ToList();
                }
                //config = config.Where(a => a.Name == "L_DETAIL_DISCOUNT_SINGLE_BILL_1").ToList();
                var screenValue = new List<LovScreenValueModel>();
                if (SiteSession.CurrentUICulture.IsThaiCulture())
                {
                    screenValue = config.Select(l => new LovScreenValueModel
                    {
                        Name = l.Name,
                        PageCode = l.LovValue5,
                        DisplayValue = l.LovValue1,
                        LovValue3 = l.LovValue3,
                        GroupByPDF = l.LovValue4,
                        OrderByPDF = l.OrderBy,
                        Type = l.Type,
                        DefaultValue = l.DefaultValue
                    }).ToList();
                }
                else
                {
                    screenValue = config.Select(l => new LovScreenValueModel
                    {
                        Name = l.Name,
                        PageCode = l.LovValue5,
                        DisplayValue = l.LovValue2,
                        LovValue3 = l.LovValue3,
                        GroupByPDF = l.LovValue4,
                        OrderByPDF = l.OrderBy,
                        Type = l.Type,
                        DefaultValue = l.DefaultValue
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
        [HttpPost]
        public ActionResult ProcessLineDormitoryMaster(string LcCode = "", string ASCCode = "", string outType = "", string outSubType = "", string outPartname = "")
        {

            // var model = new PreregisterModel();
            // model.Pre_PartnerName = outPartname;

            ViewBag.Vas = "";
            ViewBag.preregister = "";
            if (Session["EndProcessFlag"].ToSafeBoolean())
            {
                Session["PopupStatus"] = "Success";
                Session["EndProcessFlag"] = null;
            }
            else
                Session["PopupStatus"] = null;

            if (Session["PreRegisterModel"] != null)
            {
                ViewBag.LoginEnd = true;
            }
            else
            {
                ViewBag.LoginEnd = false;
            }
            ViewBag.labelFBBDORM008 = GetScreenConfig();
            //   ViewBag.LoginName = model.Pre_PartnerName;
            ViewBag.Language = Session[WebConstants.SessionKeys.CurrentUICulture].ToSafeString() == "1" ? "TH" : "EN";
            ViewBag.labelFBBORV00 = GetScreenConfig("FBBORV00");
            //     ViewBag.LabelFBBTR003 = GetCustRegisterScreenConfig();
            Logger.Info("Line Login => Acess through DormitoryMaster");
            ViewBag.LoginEnd = true;
            //Session["PreRegisterModel"] = model;
            //return View("Index", model);
            return View();

        }
        public JsonResult CheckSeibel(string LocationCode = "", string ASCCode = "")
        {
            string FullUrl = "";
            if (Session["FullUrl"] != null)
                FullUrl = Session["FullUrl"].ToSafeString();

            var model = new PreregisterModel();
            // model.Pre_PartnerNames =


            if (LocationCode != "")
            {
                var query = new GetSeibelInfoQuery()
                {
                    LocationCode = LocationCode,
                    Transaction_Id = LocationCode,
                    FullURL = FullUrl
                };
                var result = _queryProcessor.Execute(query);
                return Json(result, JsonRequestBehavior.AllowGet);

            }
            else if (LocationCode == "" && ASCCode != "")
            {
                var query = new GetSeibelInfoQuery()
                {
                    ASCCode = ASCCode,
                    Inparam1 = "IVR",
                    Transaction_Id = ASCCode,
                    FullURL = FullUrl
                };
                var result = _queryProcessor.Execute(query);

                if (result.outLocationCode.ToSafeString() != "")
                {
                    var query2 = new GetSeibelInfoQuery()
                    {
                        LocationCode = result.outLocationCode.ToSafeString(),
                        Inparam1 = "",
                        Transaction_Id = result.outLocationCode.ToSafeString(),
                        FullURL = FullUrl
                    };
                    var result2 = _queryProcessor.Execute(query2);
                    return Json(result2, JsonRequestBehavior.AllowGet);
                }

            }

            var errormodel = new SeibelResultModel();
            errormodel.outStatus = "Error";
            return Json(errormodel, JsonRequestBehavior.AllowGet);


        }
    }
}
