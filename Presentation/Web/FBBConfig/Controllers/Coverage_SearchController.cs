using FBBConfig.Extensions;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries.CoverageManagePage;
using WBBEntity.Extensions;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace FBBConfig.Controllers
{
    public class Coverage_SearchController : FBBConfigController
    {
        private readonly IQueryProcessor _queryProcessor;
        //
        // GET: /Coverage_Search/
        public Coverage_SearchController(ILogger logger,
            IQueryProcessor queryProcessor)
        {
            base._Logger = logger;
            _queryProcessor = queryProcessor;
        }

        [AuthorizeUserAttribute]
        public ActionResult Index()
        {
            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "Account");

            ViewBag.User = base.CurrentUser;

            return View();
        }

        public JsonResult Read([DataSourceRequest] DataSourceRequest request, string amphur, string ipranCode, string locationCode, string nodeName, string nodeStatus, string port, string province, string regionCode)
        {
            var query = new GetCoverageAreaQuery
            {
                Amphur = amphur,
                IpRanCode = ipranCode,
                LocationCode = locationCode,
                NodeName = nodeName,
                NodeStaus = nodeStatus,
                Port = port,
                Province = province,
                RegionCode = regionCode
            };

            var coverageSite = _queryProcessor.Execute(query);
            if (coverageSite.CoverageAreaPanel != null)
            {
                var data = coverageSite.CoverageAreaPanel.ToList().DistinctBy(x => x.ContactId);
                coverageSite.TotalSite = data.Count().ToString();
                coverageSite.CoverageAreaPanel = new List<CoverageAreaPanel>();
                coverageSite.CoverageAreaPanel = (List<CoverageAreaPanel>)data.ToList();
                Session["CoverageSite"] = coverageSite;
                return Json(coverageSite.CoverageAreaPanel.ToDataSourceResult(request));
            }
            else
            {
                return Json("");
            }

        }

        public JsonResult GetSummary()
        {
            var coverageSite = (CoverageSitePanelModel)Session["CoverageSite"];
            if (coverageSite == null)
            {
                coverageSite = new CoverageSitePanelModel();
            }
            return Json(coverageSite, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExportExcel()
        {
            var data = (CoverageSitePanelModel)Session["CoverageSite"];

            var dataExcel = (from r in data.CoverageAreaPanel
                             select new CoverageExcelModel
                             {
                                 REGION = r.RegionCode,
                                 IPRAN_CODE = r.IpRanSiteCode,
                                 LOCATIONCODE = r.CondoCode,
                                 NODESTATUS = r.Status,
                                 ONTARGET_DATE_IN = r.OnTargetDateIn.ToSafeString(),
                                 ONTARGET_DATE_EX = r.OnTargetDateEx.ToSafeString(),
                                 NODETYPE = r.NodeType,
                                 NODENAME_TH = r.NodeNameTH,
                                 NODENAME_EN = r.NodeNameEN,
                                 CONTACT_NUMBER = r.ContactNumber,
                                 MOO = r.MooTH.ToString(),
                                 SOI_TH = r.SoiTH,
                                 ROAD_TH = r.RoadTH,
                                 ZIPCODE = r.ZipCodeTH,
                                 TUMBON = r.TumbonTH,
                                 AMPHUR = r.AmphurTH,
                                 PROVINCE = r.ProvinceTH
                             }).ToList();

            string filename = "Coverage_" + String.Format("{0:yyyyMMdd_HHmmss}", DateTime.Now);

            var bytes = GenerateEntitytoExcel<CoverageExcelModel>(dataExcel, filename);
            return File(bytes, "application/excel", filename + ".xlsx");
        }

    }
}
