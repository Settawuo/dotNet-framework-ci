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
    [CustomActionFilter]
    [CustomHandleError]
    public class LoginPreregisterController : WBBController
    {
        private readonly IQueryProcessor _queryProcessor;

        public LoginPreregisterController(IQueryProcessor queryProcessor, ILogger logger)
        {
            _queryProcessor = queryProcessor;
            base.Logger = logger;
        }

        public ActionResult Index()
        {
            ViewBag.labelFBBORV00 = GetScreenConfig("FBBORV00");
            ViewBag.LabelFBBTR003 = GetCustRegisterScreenConfig();
            ViewBag.FbbConstant = GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);
            int dd = SiteSession.CurrentUICulture;
            return View();

        }
        public List<LovScreenValueModel> GetScreenConfig(string page)
        {
            try
            {
                List<LovValueModel> config = null;

                config = base.LovData.Where(l =>
                    (!string.IsNullOrEmpty(l.Type) && l.Type == "SCREEN")
                        && (!string.IsNullOrEmpty(l.LovValue5) && l.LovValue5.Equals(page))).ToList();

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

        public List<LovScreenValueModel> GetCustRegisterScreenConfig()
        {
            var screenData = GetScreenConfig(WebConstants.LovConfigName.CustomerRegisterPageCode);
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

        public JsonResult CheckSeibel(string LocationCode = "", string ASCCode = "")
        {
            string FullUrl = "";
            if (Session["FullUrl"] != null)
                FullUrl = Session["FullUrl"].ToSafeString();

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
