using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Commands.FBSS;
using WBBContract.Queries.Commons.Master;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBContract.Queries.WebServices;
using WBBEntity.Extensions;
using WBBEntity.PanelModels;
using WBBEntity.PanelModels.WebServiceModels;
using WBBWeb.Extension;
using WBBWeb.Extension.MvcAttribute;

namespace WBBWeb.Controllers
{
    [IENoCache(Order = 1)]
    [SessionExpireCheckAttribute(Order = 2)]

    [CustomHandleError]

    public class PreRegisterController : WBBController
    {
        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<GenPasswordCommand> _savepre;
        private readonly ICommandHandler<CreateOderPreRegisterCommand> _createorder;
        private readonly ICommandHandler<UpdateRegisterCommand> _UpdateRegister;
        public PreRegisterController(IQueryProcessor queryProcessor, ICommandHandler<GenPasswordCommand> savepre
            , ICommandHandler<CreateOderPreRegisterCommand> createorder, ILogger logger, ICommandHandler<UpdateRegisterCommand> UpdateRegister)
        {
            _queryProcessor = queryProcessor;
            _savepre = savepre;
            _createorder = createorder;
            _UpdateRegister = UpdateRegister;
            base.Logger = logger;
        }
        //
        // GET: /PreRegister/

        public ActionResult Index()
        {
            var langFlg = Session[WebConstants.SessionKeys.CurrentUICulture].ToSafeString() == "1" ? "TH" : "EN";
            PreregisterModel model = new PreregisterModel();

            ViewBag.Vas = "";
            ViewBag.preregister = "";
            if (Session["EndProcessFlag"].ToSafeBoolean())
            {
                Session["PopupStatus"] = "Success";
                Session["EndProcessFlag"] = null;
            }
            else
            {
                Session["PopupStatus"] = null;
            }
            Session["PreRegisterModel"] = model;


            ViewBag.labelFBBDORM008 = GetScreenConfig();
            ViewBag.Language = Session[WebConstants.SessionKeys.CurrentUICulture].ToSafeString() == "1" ? "TH" : "EN";
            ViewBag.labelFBBORV00 = GetScreenConfig("FBBORV00");
            ViewBag.LabelFBBTR003 = GetCustRegisterScreenConfig();

            ViewBag.LabelFBBProvince = GetScreenConfigText("FBBDORM_SCREEN", "FBBDORM002");
            ViewBag.LabelFBBRegion = GetScreenConfigText("FBBDORM_ADMIN_SCREEN", "ADMIN_FBBDORM001");
            return View(model);
        }

        [HttpPost]
        public ActionResult ProcessLinePreregister(string LcCode = "", string ASCCode = "", string outType = "", string outSubType = "", string outPartname = "")
        {
            var langFlg = Session[WebConstants.SessionKeys.CurrentUICulture].ToSafeString() == "1" ? "TH" : "EN";
            var model = new PreregisterModel();
            model.Pre_PartnerName = outPartname;

            ViewBag.Vas = "";
            ViewBag.preregister = "";

            if (Session["EndProcessFlag"].ToSafeBoolean())
            {
                Session["PopupStatus"] = "Success";
                Session["EndProcessFlag"] = null;
            }
            else
            {
                Session["PopupStatus"] = null;
            }
            if (Session["PreRegisterModel"] != null)
            {
                ViewBag.LoginEnd = true;
            }
            else
            {
                ViewBag.LoginEnd = false;
            }
            ViewBag.labelFBBDORM008 = GetScreenConfig();
            ViewBag.LoginName = model.Pre_PartnerName;
            ViewBag.Language = langFlg;
            ViewBag.labelFBBORV00 = GetScreenConfig("FBBORV00");
            ViewBag.LabelFBBTR003 = GetCustRegisterScreenConfig();
            Logger.Info("Line Login => Acess through Preregister");
            ViewBag.LoginEnd = true;
            Session["PreRegisterModel"] = model;

            ViewBag.LabelFBBProvince = GetScreenConfigText("FBBDORM_SCREEN", "FBBDORM002");
            ViewBag.LabelFBBRegion = GetScreenConfigText("FBBDORM_ADMIN_SCREEN", "ADMIN_FBBDORM001");

            return View("Index", model);

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

        public List<DomitoryModel> GetDormitaryALL(string netnumber = "")
        {

            var langFlg = Session[WebConstants.SessionKeys.CurrentUICulture].ToSafeString() == "1" ? "TH" : "EN";
            //(SiteSession.CurrentUICulture.IsThaiCulture() ? "TH" : "EN");

            var result = new GetDormitoryQuery
            {
                language = langFlg,
                netnumber = netnumber

            };

            var q_result = _queryProcessor.Execute(result);

            return q_result;

        }
        public JsonResult GetDormitaryDDL(string netnumber = "", string region = "", string province = "")
        {

            var langFlg = Session[WebConstants.SessionKeys.CurrentUICulture].ToSafeString() == "1" ? "TH" : "EN";

            List<DomitoryModel> data = GetDormitaryALL();

            if (!string.IsNullOrEmpty(netnumber))
            {
                data = data.Where(a => a.Pre_PREPAID_NON_MOBILE == netnumber).ToList();
            }

            if (!string.IsNullOrEmpty(region) && !string.IsNullOrEmpty(province))
            {
                data = data.Where(a => (a.Pre_RegionCode == region) &&
                                       (a.Pre_Province == province)).ToList();
            }
            else if (string.IsNullOrEmpty(region) && !string.IsNullOrEmpty(province))
            {
                data = data.Where(a => a.Pre_Province == province).ToList();
            }
            else if (!string.IsNullOrEmpty(region) && string.IsNullOrEmpty(province))
            {
                data = data.Where(a => a.Pre_RegionCode == region).ToList();
            }

            if (langFlg == "TH")
            {
                var newList = data.GroupBy(x => new { x.Pre_dormitory_id, x.Pre_dormitory_name_th, x.Pre_dormitory_no_th, x.Pre_RegionCode, x.Pre_Province })
                    .Select(y => new DropdownModel()
                    {
                        Value = y.Key.Pre_dormitory_id + ':' + y.Key.Pre_dormitory_name_th + ':' + y.Key.Pre_dormitory_no_th,
                        Text = y.Key.Pre_dormitory_name_th + ':' + y.Key.Pre_dormitory_no_th
                    }
                ).OrderBy(a => a.Text);

                return Json(newList, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var newList = data.GroupBy(x => new { x.Pre_dormitory_id, x.Pre_dormitory_name_en, x.Pre_dormitory_no_en, x.Pre_RegionCode, x.Pre_Province })
                    .Select(y => new DropdownModel()
                    {
                        Value = y.Key.Pre_dormitory_id + ':' + y.Key.Pre_dormitory_name_en + ':' + y.Key.Pre_dormitory_no_en,
                        Text = y.Key.Pre_dormitory_name_en + ':' + y.Key.Pre_dormitory_no_en
                    }
              ).OrderBy(a => a.Text);

                return Json(newList, JsonRequestBehavior.AllowGet);

            }
        }

        public JsonResult GetFloors(string DormID = "", string DormName = "", string DormNO = "", string Netnumber = "")
        {
            var langFlg = Session[WebConstants.SessionKeys.CurrentUICulture].ToSafeString() == "1" ? "TH" : "EN";

            List<DomitoryModel> data;
            data = GetDormitaryALL();
            if (langFlg == "TH")
            {
                if (Netnumber != "")
                {
                    data = data.Where(x => x.Pre_PREPAID_NON_MOBILE.Equals(Netnumber)).ToList();
                }
                else
                {

                    if (DormID != "" && DormName != "" && DormNO != "")
                    {
                        data = data.Where(x => x.Pre_dormitory_id.Equals(DormID) && (x.Pre_dormitory_name_th).Equals(DormName)
                             && (x.Pre_dormitory_no_th).Equals(DormNO)).ToList();
                    }
                    else
                    {
                    }
                }
            }
            else
            {
                if (Netnumber != "")
                {
                    data = data.Where(x => x.Pre_PREPAID_NON_MOBILE.Equals(Netnumber)).ToList();
                }
                else
                {

                    if (DormID != "" && DormName != "" && DormNO != "")
                    {
                        data = data.Where(x => x.Pre_dormitory_id.Equals(DormID) && (x.Pre_dormitory_name_en).Equals(DormName)
                             && (x.Pre_dormitory_no_en).Equals(DormNO)).ToList();
                    }
                    else
                    {
                    }
                }

            }

            List<DropdownModel> newList;
            newList = data.GroupBy(x => new { x.Pre_floor_no })
              .Select(y => new DropdownModel()
              {
                  Value = y.Key.Pre_floor_no,
                  Text = y.Key.Pre_floor_no
              }
               ).ToList();
            List<string> newStrList = new List<string>();
            foreach (var item in newList)
            {
                string ddlstr = item.Text;
                newStrList.Add(ddlstr);
            }

            newStrList.Sort((a, b) => new StringNum(a).CompareTo(new StringNum(b)));
            var newListFloor = new List<DropdownModel>();
            foreach (var item in newStrList)
            {
                DropdownModel newlist = new DropdownModel();
                newlist.Text = item;
                newlist.Value = item;
                newListFloor.Insert(newListFloor.Count(), newlist);
            }

            return Json(newListFloor, JsonRequestBehavior.AllowGet);

        }


        public JsonResult GetDormitaryRoomDDL(string DormID = "", string DormName = "", string DormNO = "", string DromFloor = "", string Netnumber = "")
        {

            var langFlg = Session[WebConstants.SessionKeys.CurrentUICulture].ToSafeString() == "1" ? "TH" : "EN";

            List<DomitoryModel> data;
            data = GetDormitaryALL();
            if (langFlg == "TH")
            {
                if (Netnumber != "")
                {
                    data = data.Where(x => x.Pre_PREPAID_NON_MOBILE.Equals(Netnumber)).ToList();
                }
                else
                {

                    if (DormID != "" && DormName != "" && DormNO != "" && DromFloor != "")
                    {
                        data = GetDormitaryALL().Where(x => x.Pre_dormitory_id.Equals(DormID) && (x.Pre_dormitory_name_th).Equals(DormName)
                     && (x.Pre_dormitory_no_th).Equals(DormNO) && (x.Pre_floor_no).Equals(DromFloor)).ToList();
                    }
                    else
                    {
                    }
                }
            }
            else
            {
                if (Netnumber != "")
                {
                    data = data.Where(x => x.Pre_PREPAID_NON_MOBILE.Equals(Netnumber)).ToList();
                }
                else
                {

                    if (DormID != "" && DormName != "" && DormNO != "" && DromFloor != "")
                    {
                        data = GetDormitaryALL().Where(x => x.Pre_dormitory_id.Equals(DormID) && (x.Pre_dormitory_name_en).Equals(DormName)
                     && (x.Pre_dormitory_no_en).Equals(DormNO) && (x.Pre_floor_no).Equals(DromFloor)).ToList();
                    }
                    else
                    {
                    }
                }


            }
            var newList = data.GroupBy(x => new { x.Pre_room_no })
                          .Select(y => new DropdownModel()
                          {
                              Value = y.Key.Pre_room_no,
                              Text = y.Key.Pre_room_no
                          }
                          ).ToList();
            List<string> newStrList = new List<string>();
            foreach (var item in newList)
            {
                string ddlstr = item.Text;
                newStrList.Add(ddlstr);
            }
            var newListRoom = new List<DropdownModel>();

            foreach (var item in newStrList)
            {
                DropdownModel newlist = new DropdownModel();
                newlist.Text = item;
                newlist.Value = item;
                newListRoom.Insert(newListRoom.Count(), newlist);
            }
            return Json(newListRoom, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetNonPreMobile(string DormID = "", string DormName = "", string DormNO = "", string DromFloor = "", string Dormroom = "")
        {
            var langFlg = Session[WebConstants.SessionKeys.CurrentUICulture].ToSafeString() == "1" ? "TH" : "EN";

            string NonPreMo = "";
            List<DomitoryModel> data;
            if (langFlg == "TH")
            {
                data = GetDormitaryALL().Where(x => x.Pre_dormitory_id.Equals(DormID) && (x.Pre_dormitory_name_th).Equals(DormName)
                       && (x.Pre_dormitory_no_th).Equals(DormNO) && (x.Pre_floor_no).Equals(DromFloor) && (x.Pre_room_no).Equals(Dormroom)).ToList();

                if (DormID != "" && DormName != "" && DormNO != "" && DromFloor != "" && Dormroom != "")
                {

                    NonPreMo = data.ToList().FirstOrDefault().Pre_PREPAID_NON_MOBILE;
                }
            }
            else
            {

                data = GetDormitaryALL().Where(x => x.Pre_dormitory_id.Equals(DormID) && (x.Pre_dormitory_name_en).Equals(DormName)
                          && (x.Pre_dormitory_no_en).Equals(DormNO) && (x.Pre_floor_no).Equals(DromFloor) && (x.Pre_room_no).Equals(Dormroom)).ToList();

                if (DormID != "" && DormName != "" && DormNO != "" && DromFloor != "" && Dormroom != "")
                {

                    NonPreMo = data.ToList().FirstOrDefault().Pre_PREPAID_NON_MOBILE;
                }

            }


            return Json(NonPreMo, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSubcontract(string Netnumber = "")
        {
            var langFlg = Session[WebConstants.SessionKeys.CurrentUICulture].ToSafeString() == "1" ? "TH" : "EN";

            List<DomitoryModel> data;
            data = GetDormitaryALL();
            if (langFlg == "TH")
            {
                if (Netnumber != "")
                {
                    data = data.Where(a => a.Pre_PREPAID_NON_MOBILE == Netnumber).ToList();
                }
                else
                {
                }
                var newList = data.GroupBy(x => new { x.Pre_SubcontractID, x.Pre_SubcontractTH })
                    .Select(y => new DropdownModel()
                    {
                        Value = y.Key.Pre_SubcontractTH,
                        Text = y.Key.Pre_SubcontractTH
                    }
                );
                return Json(newList, JsonRequestBehavior.AllowGet);
            }
            else
            {
                if (Netnumber != "")
                {
                    data = data.Where(a => a.Pre_PREPAID_NON_MOBILE == Netnumber).ToList();
                }
                else
                {
                }
                var newList = data.GroupBy(x => new { x.Pre_SubcontractID, x.Pre_SubcontractEN })
                    .Select(y => new DropdownModel()
                    {
                        Value = y.Key.Pre_SubcontractEN,
                        Text = y.Key.Pre_SubcontractEN
                    }
                );
                return Json(newList, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult GetSubcontractID(string Netnumber = "")
        {
            var langFlg = Session[WebConstants.SessionKeys.CurrentUICulture].ToSafeString() == "1" ? "TH" : "EN";
            var SubcontractId = "";
            List<DomitoryModel> data;
            data = GetDormitaryALL();
            if (langFlg == "TH")
            {
                data = data.Where(a => a.Pre_PREPAID_NON_MOBILE == Netnumber).ToList();

                if (Netnumber != "")
                {
                    SubcontractId = data.ToList().FirstOrDefault().Pre_SubcontractId;

                }
            }
            else
            {
                data = data.Where(a => a.Pre_PREPAID_NON_MOBILE == Netnumber).ToList();

                if (Netnumber != "")
                {
                    SubcontractId = data.ToList().FirstOrDefault().Pre_SubcontractId;

                }
            }
            return Json(SubcontractId, JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetContactTime()
        {
            //var langFlg = (SiteSession.CurrentUICulture.IsThaiCulture() ?
            //                WebConstants.LovConfigName.TitleCodeTh : WebConstants.LovConfigName.TitleCodeEn);
            var langFlg = Session[WebConstants.SessionKeys.CurrentUICulture].ToSafeString() == "1" ? "TH" : "EN";
            if (langFlg == "TH")
            {
                var dropDown = base.LovData
                    .Where(l => l.Type.Equals(WebConstants.LovConfigName.TimeSlot))
                    .Select(l => new DropdownModel
                    {
                        Text = l.LovValue1,
                        Value = l.Name,
                        DefaultValue = l.DefaultValue,
                    })
                    .ToList();
                dropDown = dropDown.OrderBy(l => l.Value).ToList();
                return Json(dropDown, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var dropDown = base.LovData
                       .Where(l => l.Type.Equals(WebConstants.LovConfigName.TimeSlot))
                       .Select(l => new DropdownModel
                       {
                           Text = l.LovValue2,
                           Value = l.Name,
                           DefaultValue = l.DefaultValue,
                       })
                       .ToList();
                dropDown = dropDown.OrderBy(l => l.Value).ToList();
                return Json(dropDown, JsonRequestBehavior.AllowGet);
            }

        }
        public JsonResult GetContactTimeToday(string Datetime)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            //string Datetimes = Datetime.ToString();
            //string Dates = Datetimes.Substring(0, 10);
            string[] tempDate = Datetime.Split('-');
            string resultDate = tempDate[0] + "/" + tempDate[1] + "/" + tempDate[2];
            var langFlg = Session[WebConstants.SessionKeys.CurrentUICulture].ToSafeString() == "1" ? "TH" : "EN";
            DateTime Today = DateTime.Now;
            DateTime DateTimeSelect;
            Today = Today.AddHours(1);
            // DateTime DateTimeSelect = Datetime;

            if (langFlg == "TH")
            {
                //DateTimeSelect = Datetime.;

                DateTimeSelect = DateTime.ParseExact(resultDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                DateTimeSelect = DateTimeSelect.AddYears(-543);
            }
            else
            {
                // DateTimeSelect = Datetime;
                DateTimeSelect = DateTime.ParseExact(resultDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            }
            if (Today.Date != DateTimeSelect)
            {
                return GetContactTime(); // allGetContactTime
            }
            else
            {

                if (langFlg == "TH")
                {

                    List<DropdownModel> dropDownList = new List<DropdownModel>();
                    foreach (var item in base.LovData.Where(l => l.Type.Equals(WebConstants.LovConfigName.TimeSlot)))
                    {
                        var tmpDateTimeSelect = DateTimeSelect.AddHours(int.Parse(item.LovValue3));
                        if (Today < tmpDateTimeSelect)
                        {
                            DropdownModel dropDown2 = new DropdownModel();
                            dropDown2.Text = item.LovValue1;
                            dropDown2.Value = item.Name;
                            dropDownList.Add(dropDown2);
                        }
                        //     dropDownList.OrderBy(a => a.Value);
                    }
                    if (dropDownList.Count == 0)
                    {
                        DropdownModel dropDown2 = new DropdownModel();
                        dropDown2.Text = "";
                        dropDown2.Value = "0";
                        dropDownList.Add(dropDown2);
                    }
                    //  dropDownList.OrderBy(a => a.Value);
                    return Json(dropDownList, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    List<DropdownModel> dropDownList = new List<DropdownModel>();
                    foreach (var item in base.LovData.Where(l => l.Type.Equals(WebConstants.LovConfigName.TimeSlot)))
                    {
                        var tmpDateTimeSelect = DateTimeSelect.AddHours(int.Parse(item.LovValue3));
                        if (Today < tmpDateTimeSelect)
                        {
                            DropdownModel dropDown2 = new DropdownModel();
                            dropDown2.Text = item.LovValue2;
                            dropDown2.Value = item.Name;
                            dropDownList.Add(dropDown2);
                        }
                        //     dropDownList.OrderBy(a => a.Value);
                    }
                    if (dropDownList.Count == 0)
                    {
                        DropdownModel dropDown2 = new DropdownModel();
                        dropDown2.Text = "0";
                        dropDown2.Value = "0";
                        dropDownList.Add(dropDown2);
                    }
                    // dropDownList.OrderBy(a =>a.Value);
                    return Json(dropDownList, JsonRequestBehavior.AllowGet);
                }
            }
        }
        public JsonResult CheckValidate(string mobile = "")
        {
            if (mobile != "")
            {
                var result = new ValidateNonMobilePinQuery
                {
                    NonMobilePin = mobile
                };

                var aa = _queryProcessor.Execute(result);

                return Json(aa, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }

        }
        public JsonResult GetDormitaryAddress(string netnumber = "", string address = "")
        {
            var langFlg = Session[WebConstants.SessionKeys.CurrentUICulture].ToSafeString() == "1" ? "TH" : "EN";
            //  var Addressname = "";
            List<DropdownModel> ListAddress;
            List<DomitoryModel> data = GetDormitaryALL();
            if (langFlg == "TH")
            {
                data = data.Where(a => a.Pre_PREPAID_NON_MOBILE == netnumber).ToList();


                ListAddress = data.GroupBy(x => new { x.Pre_dormitory_id, x.Pre_dormitory_no_th, x.Pre_dormitory_name_th, x.Pre_room_no, x.Pre_PIN_CODE, x.Pre_floor_no, x.Pre_MOO_TH, x.Pre_STREET_NAME_TH, x.Pre_SOI_TH, x.Pre_Pre_STATE })
                        .Select(y => new DropdownModel()
                        {
                            Text = y.Key.Pre_dormitory_name_th + ' ' + y.Key.Pre_dormitory_no_th + ' ' + "ห้องที่" + y.Key.Pre_room_no + "ชั้นที่" + y.Key.Pre_floor_no + '{' + y.Key.Pre_PIN_CODE + '}' + ' ' + "หมู่" + y.Key.Pre_MOO_TH + ' ' + "ถนน" + y.Key.Pre_STREET_NAME_TH + ' ' + "ซอย" + y.Key.Pre_SOI_TH + ' ' + y.Key.Pre_Pre_STATE
                        }
                  ).ToList();
            }
            else
            {

                data = data.Where(a => a.Pre_PREPAID_NON_MOBILE == netnumber).ToList();


                ListAddress = data.GroupBy(x => new { x.Pre_dormitory_id, x.Pre_dormitory_no_en, x.Pre_dormitory_name_en, x.Pre_room_no, x.Pre_PIN_CODE, x.Pre_floor_no, x.Pre_MOO_EN, x.Pre_STREET_NAME_EN, x.Pre_SOI_EN, x.Pre_Pre_STATE })
                        .Select(y => new DropdownModel()
                        {
                            Text = y.Key.Pre_dormitory_no_en + ' ' + y.Key.Pre_dormitory_name_en + ' ' + "Room No." + y.Key.Pre_room_no + "Floor." + y.Key.Pre_floor_no + '{' + y.Key.Pre_PIN_CODE + '}' + ' ' + "MOO." + y.Key.Pre_MOO_EN + ' ' + "Street." + y.Key.Pre_STREET_NAME_EN + ' ' + "Soi" + y.Key.Pre_SOI_EN + ' ' + y.Key.Pre_Pre_STATE
                        }
                  ).ToList();

            }
            return Json(ListAddress, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetDormitaryAddressID(string netnumber = "")
        {
            var langFlg = Session[WebConstants.SessionKeys.CurrentUICulture].ToSafeString() == "1" ? "TH" : "EN";
            var AddressId = "";
            List<DomitoryModel> data = GetDormitaryALL();
            if (langFlg == "TH")
            {
                data = data.Where(a => a.Pre_PREPAID_NON_MOBILE == netnumber).ToList();

                if (netnumber != "")
                {
                    AddressId = data.ToList().FirstOrDefault().Pre_AddressID;

                }
            }
            else
            {
                data = data.Where(a => a.Pre_PREPAID_NON_MOBILE == netnumber).ToList();

                if (netnumber != "")
                {
                    AddressId = data.ToList().FirstOrDefault().Pre_AddressID;

                }
            }
            return Json(AddressId, JsonRequestBehavior.AllowGet);

        }
        [LoginActionFilterAttribute]
        public ActionResult InsertPreregister(PreregisterModel model)
        {

            var langFlg = Session[WebConstants.SessionKeys.CurrentUICulture].ToSafeString() == "1" ? "TH" : "EN";
            var SessionModel = Session["PreRegisterModel"] as PreregisterModel;
            model.Pre_service_status = "FBB";
            model.Pre_updated_by = SessionModel.Pre_PartnerName;
            model.Pre_IA_NO = model.Pre_prepaid_non_mobile;

            if (langFlg == "TH")
            {
                string tempDate = model.Pre_AppointmentDateTH;
                string[] tempDateTH = tempDate.Split('-');
                string resultDate = tempDateTH[0] + "-" + tempDateTH[1] + "-" + (int.Parse(tempDateTH[2]) - 543);
                model.Pre_AppointmentDateTime = resultDate;
            }
            else
            {
                model.Pre_AppointmentDateTime = model.Pre_AppointmentDateEN;
            }
            var command = new GenPasswordCommand
            {
                //ProductType = model.ProductType,
                //CustID = model.Cus_ID,
                //UserName = model.Cust_name
            };

            _savepre.Handle(command);

            model.Pre_PasswordDecrypt = command.PasswordDec;
            model.Pre_GenPassword = command.Genpassword;


            var command2 = new CreateOderPreRegisterCommand
            {

                TimeSlot = model.Pre_ContactTime.Replace("–", "-"),
                AppointmentDate = model.Pre_AppointmentDateTime,
                AddressId = model.Pre_AddressId,
                PreNonmobile = model.Pre_prepaid_non_mobile,
                Address = model.Pre_Addressname,
                Cus_ID = model.Pre_Cus_ID,
                Cus_Name = model.Pre_Cust_name,
                Cus_Surname = model.Pre_Cus_surname,
                Subcontract = model.Pre_Subcontract,
                Upsubcontractid = model.SubcontractID,
                SubcontractID = model.SubcontractID,
                Customername = model.Pre_Cust_name + "  " + model.Pre_Cus_surname,
                PasswordDec = model.Pre_PasswordDecrypt,
                Phone = model.Pre_TEL

            };

            _createorder.Handle(command2);

            model.Up_FBBDORM_Order_No = command2.Up_FBBDORM_Order_No;
            if (command2.Pre_Total_Results == "0")
            {
                //model.Pre_AddressId = model.Pre_AddressId;
                var command3 = new UpdateRegisterCommand
                {
                    p_customer_name = model.Pre_Cust_name,
                    p_customer_lastname = model.Pre_Cus_surname,
                    p_card_type = model.Pre_Card_Type,
                    p_card_no = model.Pre_Cus_ID,
                    p_mobile_no = model.Pre_TEL,
                    p_fbbdorm_order_no = command2.Up_FBBDORM_Order_No,
                    p_prepaid_non_mobile = model.Pre_prepaid_non_mobile,
                    p_time_slot = model.Pre_ContactTime,
                    p_address_id = model.Pre_AddressId,
                    p_service_code = command.Genpassword,
                    p_event_code = "New",
                    p_time_slot_id = model.Pre_AppointmentDateTime,
                    p_updated_by = SessionModel.Pre_PartnerName,
                    p_in_no = model.Pre_prepaid_non_mobile

                };
                _UpdateRegister.Handle(command3);


                ViewBag.User = base.CurrentUser;
                ViewBag.LanguagePage = langFlg;
                ViewBag.SaveSuccess = true;
                ViewBag.LoginName = SessionModel.Pre_PartnerName;
                ViewBag.labelFBBDORM008 = GetScreenConfig();
                ViewBag.Language = Session[WebConstants.SessionKeys.CurrentUICulture].ToSafeString() == "1" ? "TH" : "EN";
                ViewBag.FbbConstant = GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);
                return View("Index", model);

            }
            else
            {
                ViewBag.User = base.CurrentUser;
                ViewBag.LanguagePage = langFlg;
                ViewBag.SaveFail = true;
                ViewBag.LoginName = SessionModel.Pre_PartnerName;
                ViewBag.labelFBBDORM008 = GetScreenConfig();
                ViewBag.Language = Session[WebConstants.SessionKeys.CurrentUICulture].ToSafeString() == "1" ? "TH" : "EN";
                ViewBag.FbbConstant = GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);
                return View("Index", model);
            }



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

        public List<LovScreenValueModel> GetGeneralScreenConfig()
        {
            var screenData = GetScreenConfig(null);
            return screenData;
        }
        public List<LovScreenValueModel> GetCoverageScreenConfig()
        {
            var screenData = GetScreenConfig(WebConstants.LovConfigName.CoveragePageCode);
            return screenData;
        }
        public List<LovScreenValueModel> GetDisplayPackageScreenConfig()
        {
            var screenData = GetScreenConfig(WebConstants.LovConfigName.DisplayPackagePageCode);
            return screenData;
        }
        public List<LovScreenValueModel> GetCustRegisterScreenConfig()
        {
            var screenData = GetScreenConfig(WebConstants.LovConfigName.CustomerRegisterPageCode);
            return screenData;
        }
        public List<LovScreenValueModel> GetSummaryScreenConfig()
        {
            var screenData = GetScreenConfig(WebConstants.LovConfigName.SummaryPageCode);
            return screenData;
        }
        public List<LovScreenValueModel> GetVas_Select_Package_ScreenConfig()
        {
            var screenData = GetScreenConfig(WebConstants.LovConfigName.Vas_Package);
            return screenData;
        }
        public List<LovScreenValueModel> GetVasPopUpScreenConfig()
        {
            var screenData = GetScreenConfig(WebConstants.LovConfigName.DisplayPopupVasConfrim);
            return screenData;
        }
        public List<LovScreenValueModel> GetDisplay_Select_Type_Service()
        {
            var screenData = GetScreenConfig(WebConstants.LovConfigName.SelectType_Service);
            return screenData;
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
                var langFlg = Session[WebConstants.SessionKeys.CurrentUICulture].ToSafeString() == "1" ? "TH" : "EN";
                var screenValue = new List<LovScreenValueModel>();
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public JsonResult GetRegion()
        {
            var query = new SelectLovQuery
            {
                LOV_TYPE = "REGION_CODE"
            };

            var data = _queryProcessor.Execute(query);

            List<DropdownModel> result;

            if (Session[WebConstants.SessionKeys.CurrentUICulture].ToSafeString() == "1") //TH
            {

                result = data.Select(y => new DropdownModel()
                {
                    Value = y.LOV_NAME,
                    Text = y.LOV_VAL2
                }
                ).ToList();
            }
            else
            {
                result = data.Select(y => new DropdownModel()
                {
                    Value = y.LOV_NAME,
                    Text = y.LOV_VAL1
                }
                ).ToList();
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProvince(string regionfilter)
        {
            List<DropdownModel> result;

            if (regionfilter != "")
            {
                result = base.ZipCodeData(Convert.ToInt32(Session[WebConstants.SessionKeys.CurrentUICulture].ToSafeString()))
                        .Where(z => z.RegionCode == regionfilter)
                        .GroupBy(z => z.Province)
                        .Select(z =>
                        {
                            var item = z.First();
                            return new DropdownModel { Text = item.Province, Value = item.Province };
                        })
                        .ToList();
            }
            else
            {
                result = base.ZipCodeData(Convert.ToInt32(Session[WebConstants.SessionKeys.CurrentUICulture].ToSafeString()))
                        .GroupBy(z => z.Province)
                        .Select(z =>
                        {
                            var item = z.First();
                            return new DropdownModel { Text = item.Province, Value = item.Province };
                        })
                        .ToList();
            }

            return Json(result, JsonRequestBehavior.AllowGet);

        }

        public List<LovScreenValueModel> GetScreenConfigText(string type, string lov5)
        {
            try
            {
                var langFlg = Session[WebConstants.SessionKeys.CurrentUICulture].ToSafeString() == "1" ? "TH" : "EN";
                List<LovValueModel> config = null;
                config = base.LovData.Where(l =>
                    (!string.IsNullOrEmpty(l.Type) && l.Type == type) &&
                    (!string.IsNullOrEmpty(l.LovValue5) && l.LovValue5.Equals(lov5))).ToList();
                var screenValue = new List<LovScreenValueModel>();

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

        public JsonResult GetRegionAndProvoinceForDorm(string dormId = "")
        {

            var langFlg = Session[WebConstants.SessionKeys.CurrentUICulture].ToSafeString() == "1" ? "TH" : "EN";
            string regionVal = "";
            string regionTxt = "";
            string provinceVal = "";
            string provinceTxt = "";

            List<DomitoryModel> data = GetDormitaryALL();

            if (!string.IsNullOrEmpty(dormId))
            {
                data = data.Where(a => a.Pre_dormitory_id == dormId).ToList();
            }

            if (langFlg == "TH")
            {


                regionVal = data.First().Pre_RegionCode;
                provinceVal = data.First().Pre_Province;

                return Json(new { regionVal, regionTxt, provinceVal, provinceTxt }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                regionVal = data.First().Pre_RegionCode;
                provinceVal = data.First().Pre_Province;

                return Json(new { regionVal, provinceVal }, JsonRequestBehavior.AllowGet);

            }
        }

    }
}
