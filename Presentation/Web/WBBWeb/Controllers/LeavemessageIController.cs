using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Commands.FBBWebConfigCommands;
using WBBContract.Queries.Commons.Master;
using WBBEntity.Extensions;
using WBBEntity.PanelModels;
using WBBWeb.Extension;
using WBBWeb.Models;

namespace WBBWeb.Controllers
{
    [CustomActionFilter]
    [CustomHandleError]
    [IENoCache]
    public class LeavemessageIController : WBBController
    {
        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<SaveLeavemessageIICommand> _saveLeavemessageCommand;
        //
        // GET: /Leavemessage/

        public LeavemessageIController(IQueryProcessor queryProcessor
              , ICommandHandler<SaveLeavemessageIICommand> saveLeavemessageCommand
              , ILogger logger)
        {
            _queryProcessor = queryProcessor;
            _saveLeavemessageCommand = saveLeavemessageCommand;
            base.Logger = logger;
        }

        public ActionResult Index(string SaveStatus = "", string RefNo = "")
        {
            if (SaveStatus != "")
            {
                ViewBag.SaveStatus = SaveStatus;
                ViewBag.RefNo = RefNo;
                //Session["LeavemessageSaveStatus"] = "Y";
            }
            var LovScreenFBBOR026 = GetScreenConfig("FBBOR026");

            ViewBag.LabelFBBOR026 = LovScreenFBBOR026;
            ViewBag.FbbConstant = GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);

            if (LovScreenFBBOR026 != null && LovScreenFBBOR026.Count > 0)
            {
                ViewBag.TitleText = LovScreenFBBOR026.Any(c => c.Name == "L_REGISTER_AIS_FIBRE_I") ? LovScreenFBBOR026.FirstOrDefault(c => c.Name == "L_REGISTER_AIS_FIBRE_I").DisplayValue : "";
            }
            ViewBag.UrlRef = "/LeavemessageI";

            var expired = LovScreenFBBOR026.FirstOrDefault(item => item.Name == "LEAVEMESSAGEI_EXPIRED" && item.DisplayValueJing.ToDate().GetValueOrDefault().Date > DateTime.Now.Date);
            if (expired == null)
            {
                var messageExpired =
                    LovScreenFBBOR026.FirstOrDefault(item => item.Name == "L_LEAVEMESSAGEI_EXPIRED_MESSAGE") ??
                    new LovScreenValueModel();
                ViewBag.MessageExpiredI = messageExpired.DisplayValue;

                // return RedirectToAction("Expired", "LeavemessageI");

                return View("Expired");
            }

            return View();
        }

        public ActionResult Expired()
        {
           
            return View();
        }

        public ActionResult SaveRegister(LeavemessageIPanelModel Model)
        {
            string SaveStatus = "";
            string RefNo = "";

            string Language = "";
            if (SiteSession.CurrentUICulture.IsThaiCulture())
            {
                Language = "T";
            }
            else
            {
                Language = "E";
            }

            var Command = new SaveLeavemessageIICommand
            {
                p_language = Language,
                p_service_speed = Model.SELECT_SERVICE,
                p_cust_name = Model.NAME,
                p_cust_surname = Model.SURNAME,
                p_contact_mobile_no = Model.CONTACT_MOBILE,
                p_is_ais_mobile = Model.MOBILE_IS_AIS,
                p_contact_email = Model.EMAIL,
                p_address_type = Model.SELECT_ADDRESS_TYPE,
                p_building_name = Model.BUILDING,
                p_house_no = Model.HOUSE_NO,
                p_soi = Model.SOI,
                p_road = Model.ROAD,
                p_sub_district = Model.SUB_DISTRICT,
                p_district = Model.DISTRICT,
                p_province = Model.PROVINCE,
                p_postal_code = Model.POSTAL_CODE,
                p_contact_time = Model.CONTACT_TIME,
                p_rental_flag = Model.SELECT_TYPE_BUILD

            };
            _saveLeavemessageCommand.Handle(Command);
            if (Command.return_code == 0)
            {
                SaveStatus = "Y";
                RefNo = Command.return_message;

            }
            else
            {
                SaveStatus = "N";
            }

            return RedirectToAction("Index", new { SaveStatus = SaveStatus, RefNo = RefNo });
        }

        public JsonResult GetContactTime()
        {
            var CONTACT = new List<DropdownModel>();
            try
            {
                CONTACT = GetDropDownConfig("FBBOR026", "FBB_CONSTANT", "L_CONTACT_TIME_I");
            }
            catch (Exception) { }

            return Json(CONTACT, JsonRequestBehavior.AllowGet);
        }

        [OutputCache(Location = OutputCacheLocation.Client, Duration = 10)]
        public JsonResult GetProvince()
        {
            var provType = new List<DropdownModel>();
            try
            {
                provType = base.ZipCodeData(SiteSession.CurrentUICulture)
                    .GroupBy(z => z.Province)
                    .Select(z =>
                    {
                        var item = z.First();
                        return new DropdownModel { Text = item.Province, Value = item.Province };
                    })
                    .OrderBy(o => o.Text)
                    .ToList();
            }
            catch (Exception) { }

            return Json(provType, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAmphur(string provinceFilter)
        {
            var amphType = new List<DropdownModel>();
            try
            {
                /// เพิ่ม เอา ปณ ออก 01/07/2015
                amphType = base.ZipCodeData(SiteSession.CurrentUICulture)
                    .Where(z => (string.IsNullOrEmpty(z.Province) || z.Province.Equals(provinceFilter)) && (!z.Amphur.Contains("ปณ") && !z.Amphur.Contains("PO")))
                    .GroupBy(z => z.Amphur)
                    .Select(z =>
                    {
                        var item = z.First();

                        return new DropdownModel { Text = item.Amphur, Value = item.Amphur };
                    })
                    .OrderBy(o => o.Text)
                    .ToList();
            }
            catch (Exception) { }

            return Json(amphType, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTumbon(string provinceFilter, string amphurFilter)
        {
            var tumbType = new List<DropdownModel>();
            try
            {
                tumbType = base.ZipCodeData(SiteSession.CurrentUICulture)
                    .Where(z => (string.IsNullOrEmpty(z.Province) || z.Province.Equals(provinceFilter))
                                    && (string.IsNullOrEmpty(z.Amphur) || z.Amphur.Equals(amphurFilter)))
                    .GroupBy(z => z.Tumbon)
                    .Select(z =>
                    {
                        var item = z.First();
                        return new DropdownModel { Text = item.Tumbon, Value = item.Tumbon };
                    })
                    .OrderBy(o => o.Text)
                    .ToList();
            }
            catch (Exception) { }

            //tumbType.Insert(0, new DropdownModel { Text = "กรุณาเลือก", Value = "" });

            return Json(tumbType, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCoverageZipCode(string provinceFilter, string amphurFilter, string tumbonFilter, string sso)
        {
            var zipCodeList = new List<DropdownModel>();
            try
            {
                var amphurToFilter = "";
                int index1 = amphurFilter.IndexOf('(');
                if (index1 > 0)
                {
                    int index2 = amphurFilter.IndexOf(')');
                    amphurToFilter = amphurFilter.Remove(index1, index2 - index1 + 1);
                }
                else
                {
                    amphurToFilter = amphurFilter;
                }

                // find zip code.
                zipCodeList = base.ZipCodeData(SiteSession.CurrentUICulture)
                    .Where(z => (!string.IsNullOrEmpty(z.Province) && z.Province.Equals(provinceFilter))
                        && (!string.IsNullOrEmpty(z.Amphur) && z.Amphur.Contains(amphurToFilter))
                        && (!string.IsNullOrEmpty(z.Tumbon) && z.Tumbon.Equals(tumbonFilter)))
                    .Select(z => new DropdownModel { Text = z.ZipCode, Value = z.ZipCodeId, })
                    .OrderBy(o => o.Text)
                    .ToList();
            }
            catch (Exception) { }

            return Json(new { zipCodeList }, JsonRequestBehavior.AllowGet);
        }

        private List<DropdownModel> GetDropDownConfig(string pageCode ,string type ,string name)
        {
            return base.LovData
                        .Where(l => l.LovValue5 == pageCode & l.Type == type & l.Name == name)
                        .Select(l => new DropdownModel
                        {
                            Text = SiteSession.CurrentUICulture.IsThaiCulture() ? l.LovValue1 : l.LovValue2,
                            Value = l.Text,
                        })
                        .ToList();
        }

        private List<LovScreenValueModel> GetScreenConfig(string pageCode)
        {
            try
            {
                List<LovValueModel> config = null;
                if (pageCode == null)
                {
                    config = base.LovData.Where(l => l.LovValue5 == null && l.Type == "SCREEN" || l.Type == "VAS_CODE_CONFIG").ToList();
                }
                else if (pageCode == "ALLPAGE")
                {
                    config = base.LovData.Where(l => l.Type == "SCREEN").ToList();
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
                        DefaultValue = l.DefaultValue,
                        Blob = l.Image_blob != null ? Convert.ToBase64String(l.Image_blob, 0, l.Image_blob.Length) : "",
                        DisplayValueJing = l.Text
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
                        DefaultValue = l.DefaultValue,
                        Blob = l.Image_blob != null ? Convert.ToBase64String(l.Image_blob, 0, l.Image_blob.Length) : "",
                        DisplayValueJing = l.Text
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

        private List<CoverageValueModel> GetCoverageValueModel(string zipCodeId, string province, string amphur, string sso)
        {
            try
            {
                var query = new GetCoverageAreaQuery
                {
                    CurrentCulture = SiteSession.CurrentUICulture,
                    ZipCodeId = zipCodeId,
                    Province = province,
                    Amphur = amphur,
                    SSO = sso
                };

                var result = _queryProcessor.Execute(query);
                return result;
            }
            catch (Exception) { }

            return new List<CoverageValueModel>();
        }

    }
}
