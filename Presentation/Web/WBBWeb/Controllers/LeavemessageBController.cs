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
    public class LeavemessageBController : WBBController
    {
        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<SaveLeavemessageIICommand> _saveLeavemessageIICommand;
        //
        // GET: /Leavemessage/

        public LeavemessageBController(IQueryProcessor queryProcessor
              , ICommandHandler<SaveLeavemessageIICommand> saveLeavemessageIICommand
              , ILogger logger)
        {
            _queryProcessor = queryProcessor;
            _saveLeavemessageIICommand = saveLeavemessageIICommand;
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
            var LovScreenFBBOR038 = GetScreenConfig("FBBOR038");
            ViewBag.LabelFBBOR038 = LovScreenFBBOR038;

            var LabelFBBOR028 = GetScreenConfig("FBBOR028");
            ViewBag.LabelFBBOR028 = LabelFBBOR028;

            ViewBag.FbbConstant = GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);

            if (LovScreenFBBOR038 != null && LovScreenFBBOR038.Count > 0)
            {
                ViewBag.TitleText = LovScreenFBBOR038.Any(c => c.Name == "L_REGISTER_AIS_FIBRE_B") ? LovScreenFBBOR038.FirstOrDefault(c => c.Name == "L_REGISTER_AIS_FIBRE_B").DisplayValue : "";
            }
            ViewBag.UrlRef = "/LeavemessageB";

            var expired = LovScreenFBBOR038.FirstOrDefault(item => item.Name == "LEAVEMESSAGEB_EXPIRED" && item.DisplayValueJing.ToDate().GetValueOrDefault().Date > DateTime.Now.Date);
            if (expired == null)
            {
                var messageExpired =
                    LovScreenFBBOR038.FirstOrDefault(item => item.Name == "L_LEAVEMESSAGEB_EXPIRED_MESSAGE") ??
                    new LovScreenValueModel();
                ViewBag.MessageExpiredB = messageExpired.DisplayValue;

                // return RedirectToAction("Expired", "LeavemessageI");

                return View("Expired");
            }

            return View();
        }

        public ActionResult Expired()
        {
            return View();
        }

        public ActionResult SaveRegister(LeavemessageIIPanelModel Model)
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
                p_is_ais_mobile = Model.MOBILE_IS_AIS + "|SpecialOffer2",
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
                p_rental_flag = Model.SELECT_TYPE_BUILD,
                
                p_location_code = Model.LOCATION_CODE,
                p_asc_code = Model.ASC_CODE,
                p_emp_id = Model.EMP_ID,
                p_sales_rep = Model.SALES_REP

            };
            _saveLeavemessageIICommand.Handle(Command);
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
                CONTACT = GetDropDownConfig("FBBOR038", "FBB_CONSTANT", "L_CONTACT_TIME_B");
            }
            catch (Exception) { }

            return Json(CONTACT, JsonRequestBehavior.AllowGet);
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

        private List<DropdownModel> GetDropDownConfig(string pageCode, string type, string name)
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

    }
}
