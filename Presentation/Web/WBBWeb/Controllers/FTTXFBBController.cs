using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBEntity.PanelModels;

namespace WBBWeb.Controllers
{



    public class FTTXFBBController : WBBController
    {
        private readonly IQueryProcessor _queryProcessor;

        public FTTXFBBController(IQueryProcessor queryProcessor, ILogger logger)
        {
            _queryProcessor = queryProcessor;
            base.Logger = logger;
        }

        public ActionResult Index(string region, string province, string aumphur, string tumbon, string submitt = "0")
        {
            var searchresult = new List<Fttx_Fbb_PanelModel>();
            if (submitt != "0")
            {
                searchresult = searchair_FTTX(region, province, aumphur, tumbon);
            }

            ViewBag.searchairresult = searchresult;
            ViewBag.configscreen = GetScreenConfig_FTTX(null);

            if (province == "not" || tumbon == "not")
            {
                ViewBag.notshowregion = "yes";
            }

            if (region != "not" && province != "not" && tumbon != "not")
            {
                ViewBag.region = region;
                ViewBag.province = province;
                ViewBag.aumphur = aumphur;
                ViewBag.tumbon = tumbon;
            }

            return View();
        }




        public List<LovScreenValueModel> GetScreenConfig_FTTX(string pageCode)
        {
            try
            {
                List<LovValueModel> config = null;
                if (pageCode == null)
                {
                    config = base.LovData.Where(l => l.Type.Equals("SCREEN")).ToList();
                }

                var screenValue = new List<LovScreenValueModel>();

                screenValue = config.Select(l => new LovScreenValueModel
                {
                    Name = l.Name,
                    DisplayValue = l.LovValue1,

                }).ToList();


                return screenValue;
            }
            catch (Exception)
            {
                return new List<LovScreenValueModel>();
            }
        }

        public JsonResult GetRegion_FTTX()
        {
            var query = new SelectLovQuery
            {
                LOV_TYPE = "REGION_CODE"
            };

            var result = _queryProcessor.Execute(query);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Getprovince_FTTX(string regionfilter)
        {
            var query = new SelectProvinceFttxQuery
            {
                REGION_CODE = regionfilter
            };

            var result = _queryProcessor.Execute(query);
            return Json(result, JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetAumphur_FTTX(string regionfilter, string provincefilter)
        {
            var query = new SelectAmphurFttxQuery
            {
                REGION_CODE = regionfilter,
                PROVINCE = provincefilter
            };

            var result = _queryProcessor.Execute(query);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTumbon_FTTX(string regionfilter, string provincefilter, string aumphurfilter)
        {
            var query = new SelectTumbonFTTTXQuery
            {
                REGION_CODE = regionfilter,
                PROVINCE = provincefilter,
                AUMPHUR = aumphurfilter
            };

            var result = _queryProcessor.Execute(query);
            return Json(result, JsonRequestBehavior.AllowGet);

        }

        public List<Fttx_Fbb_PanelModel> searchair_FTTX(string region, string province, string aumphur, string tumbon)
        {
            var query = new SelectFTTXFBBQuery
            {
                region = region,
                province = province,
                aumphur = aumphur,
                tumbon = tumbon
            };

            var result = _queryProcessor.Execute(query);
            return result;
        }
    }
}
