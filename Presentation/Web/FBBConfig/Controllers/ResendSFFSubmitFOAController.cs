using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries.SAPOnline;
using WBBEntity.Extensions;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace FBBConfig.Controllers
{
    public class ResendSFFSubmitFOAController : FBBConfigController
    {
        private readonly IQueryProcessor _queryProcessor;

        public ResendSFFSubmitFOAController(
            ILogger logger,
            IQueryProcessor queryProcessor)
        {
            base._Logger = logger;
            _queryProcessor = queryProcessor;
        }

        public ActionResult Configuration(string page)
        {
            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "Account");

            ViewBag.User = base.CurrentUser;
            SetViewBagLov("FBBPAYG_SCREEN", page);

            return null;
        }

        private void SetViewBagLov(string screenType, string LovValue5)
        {
            var LovDataScreen = base.LovData.Where(p => p.Type == screenType && p.LovValue5 == LovValue5).ToList();
            ViewBag.configscreen = LovDataScreen;
        }

        public ActionResult Index()
        {
            this.Configuration("REPORTSLADTL");
            return View();
        }


        public ActionResult ResendSFFSubmitFOAReportAsync([DataSourceRequest] DataSourceRequest request, string dataS = "")
        {
            ResendSFFSubmitFOAQuery searchModel = new ResendSFFSubmitFOAQuery();
            var result = this.GetSubmitFOA(searchModel);
            return Json(result.ToDataSourceResult(request));
            //return null;
        }

        public List<ListPendingSFFSubmitFOA> GetSubmitFOA(ResendSFFSubmitFOAQuery model)
        {
            try
            {
                return _queryProcessor.Execute(model);
            }
            catch (Exception ex)
            {
                _Logger.Info(ex.GetErrorMessage());
                return new List<ListPendingSFFSubmitFOA>();
            }
        }


        public JsonResult ResendToSFFSubmitFOA([DataSourceRequest] DataSourceRequest request, string dataS = "")
        {
            ResendToSFFSubmitFOAQuery searchModel = new ResendToSFFSubmitFOAQuery();
            ResultResendToSFFSubmitFOA result = new ResultResendToSFFSubmitFOA();
            try
            {
                result = _queryProcessor.Execute(searchModel);
                return Json(result);
            }
            catch (Exception ex)
            {
                _Logger.Info(ex.GetErrorMessage());
                result.RETURN_CODE = -1;
                result.RETURN_MSG = ex.GetErrorMessage();
                return Json(result);
            }
        }
    }
}
