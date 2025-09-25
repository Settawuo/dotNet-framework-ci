using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using WBBEntity.Extensions;
using WBBEntity.PanelModels;
using WBBWeb.Extension;
using WBBWeb.Models;

namespace WBBWeb.Controllers
{
    [CustomActionFilter]
    [CustomHandleError]
    [IENoCache]
    public class PrePrintController : WBBController
    {
        //
        // GET: /PrePrint/

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetPrePrintForm()
        {
            try
            {
                List<LovValueModel> config = null;
                config = base.LovData.Where(l => l.Type == "PRE_PRINT_FORM").ToList();

                var screenValue = new List<LovScreenValueModel>();
                if (SiteSession.CurrentUICulture.IsThaiCulture())
                {
                    screenValue = config.Select(l => new LovScreenValueModel
                    {
                        Name = l.Name,
                        DisplayValue = l.LovValue1,
                    }).ToList();
                }
                else
                {
                    screenValue = config.Select(l => new LovScreenValueModel
                    {
                        Name = l.Name,
                        DisplayValue = l.LovValue2,
                    }).ToList();
                }

                return Json(screenValue, JsonRequestBehavior.AllowGet);
                //return screenValue;
            }
            catch (Exception ex)
            {
                Logger.Info(ex.GetErrorMessage());
                return Json("", JsonRequestBehavior.AllowGet);
            }
        }

        public List<LovScreenValueModel> GetPrePrintFormLovScreen()
        {
            List<LovScreenValueModel> screenValue = new List<LovScreenValueModel>();
            try
            {
                List<LovValueModel> config = null;
                config = base.LovData.Where(l => l.Type == "PRE_PRINT_FORM").ToList();
                if (SiteSession.CurrentUICulture.IsThaiCulture())
                {
                    screenValue = config.Select(l => new LovScreenValueModel
                    {
                        Name = l.Name,
                        DisplayValue = l.LovValue1,
                    }).ToList();
                }
                else
                {
                    screenValue = config.Select(l => new LovScreenValueModel
                    {
                        Name = l.Name,
                        DisplayValue = l.LovValue2,
                    }).ToList();
                }

            }
            catch (Exception ex)
            {
                Logger.Info(ex.GetErrorMessage());
            }
            return screenValue;
        }

        public List<LovScreenValueModel> GetlovCusregister()
        {
            List<LovScreenValueModel> screenValue = new List<LovScreenValueModel>();
            try
            {
                List<LovValueModel> config = null;
                config = base.LovData.Where(l => l.Type == "SCREEN" && l.Name == "IPCamera_MonthlyFee_Point" && l.ActiveFlag == "Y").ToList();
                if (SiteSession.CurrentUICulture.IsThaiCulture())
                {
                    screenValue = config.Select(l => new LovScreenValueModel
                    {
                        Name = l.Name,
                        DisplayValue = l.LovValue1,
                    }).ToList();
                }
                else
                {
                    screenValue = config.Select(l => new LovScreenValueModel
                    {
                        Name = l.Name,
                        DisplayValue = l.LovValue2,
                    }).ToList();
                }

            }
            catch (Exception ex)
            {
                Logger.Info(ex.GetErrorMessage());
            }
            return screenValue;
        }

        public JsonResult getSubtype()
        {
            var AISsubtype = LovData.Where(
            item => item.Name == "AIS_SHOP_SUB_TYPE" && item.LovValue5 == "FBBOR004").ToList();

            string[] myList;
            if (AISsubtype != null && AISsubtype.Count > 0)
            {
                myList = new string[AISsubtype.Count()];
                int myListCount = 0;
                foreach (var item in AISsubtype)
                {
                    string AISsubtypeLOV1 = item.LovValue1 != null ? item.LovValue1 : "";
                    myList[myListCount] = AISsubtypeLOV1;
                    myListCount++;
                }
            }
            else
            {
                myList = new string[1];
                myList[0] = "";
            }

            return Json(myList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getToday(string language)
        {
            Dictionary<string, string> time = new Dictionary<string, string>();

            if (language == "2")
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            else
                Thread.CurrentThread.CurrentCulture = new CultureInfo("th-TH");
            string today = DateTime.Today.ToString("dd MMMM yyyy");
            time.Add("today", today);

            return Json(time, JsonRequestBehavior.AllowGet);
        }

    }
}
