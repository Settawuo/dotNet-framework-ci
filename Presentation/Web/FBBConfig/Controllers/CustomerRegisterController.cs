using FBBConfig.Extensions;
using FBBConfig.Models;
using FBBConfig.Solid.CompositionRoot;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.Commons.Master;
using WBBContract.Queries.Commons.Masters;
using WBBContract.Queries.ExWebServices;
using WBBContract.Queries.WebServices;
using WBBContract.Queries.WebServices.FBSS;
using WBBEntity.Extensions;
using WBBEntity.PanelModels;
using WBBWeb.Extension;


namespace FBBConfig.Controllers
{
    [CustomHandleError]
    public class CustomerRegisterController : FBBConfigController
    {
        //
        // GET: /Process/   

        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<ReserveTimeSlotCommand> _commandReserveTimeslot;

        public CustomerRegisterController(IQueryProcessor queryProcessor, ILogger logger, ICommandHandler<ReserveTimeSlotCommand> commandReserveTimeslot)
        {
            _queryProcessor = queryProcessor;
            _commandReserveTimeslot = commandReserveTimeslot;
            base._Logger = logger;
        }

        //protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        //{
        //    base.Initialize(requestContext);

        //    const string culture = "th-TH";
        //    CultureInfo ci = CultureInfo.GetCultureInfo(culture);

        //    Thread.CurrentThread.CurrentCulture = ci;
        //    Thread.CurrentThread.CurrentUICulture = ci;
        //}


        public JsonResult CustomerRegisTestLog(string txtLog)
        {
            _Logger.Info(txtLog);
            return Json("", JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTimeSlotConfigSelectDate(string technology, string subtype)
        {
            #region sql
            //select lov_val1,lov_val2 
            //from fbb_cfg_lov 
            //where lov_name='L_TIME_SLOT'
            //and display_val=@Technology
            //and activeflag='Y'
            #endregion

            if (!string.IsNullOrEmpty(subtype))
            {
                var ConfigTSbySubtype = from t in base.LovData
                                        where t.Type == "TIME_SLOT_BY_SUBTYPE"
                                        && t.Name == subtype
                                        && t.Text.ToUpper() == technology.ToUpper()
                                        select t;
                if (ConfigTSbySubtype != null)
                {
                    if (ConfigTSbySubtype.Any())
                    {
                        return Json(ConfigTSbySubtype.FirstOrDefault(), JsonRequestBehavior.AllowGet);
                    }
                }
            }

            var DefaultConfigTS = from t in base.LovData
                                  where t.Name == "L_TIME_SLOT"
                                  && t.Text.ToUpper() == technology.ToUpper()
                                  select t;

            if (DefaultConfigTS != null)
            {
                if (DefaultConfigTS.Any())
                {
                    return Json(DefaultConfigTS.FirstOrDefault(), JsonRequestBehavior.AllowGet);
                }
            }

            return Json("", JsonRequestBehavior.AllowGet);
        }

        //public void CustomerRegisterOnLoad()
        //{
        //    var MasterDataController = Bootstrapper.GetInstance<MasterDataController>();
        //    //ViewBag.GetCustomerCardType = MasterDataController.GetCustomerCardType();
        //}

        public JsonResult GetCustomerSubCat(string idCardName)
        {
            var a = base.LovData.Where(p => p.Name == idCardName && p.Type == WebConstants.LovConfigName.CardType);
            LovValueModel model = new LovValueModel();
            if (a.Any())
            {
                model.LovValue4 = a.FirstOrDefault().LovValue4;
                model.LovValue5 = a.FirstOrDefault().LovValue5;
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetAppointment(string installation_Date, string access_Mode = "", string address_Id = "",
            string serviceCode = "", string district = "", string subDistrict = "", string province = "", string postalCode = "", int lineSelect = 1,
            long days = 0, string productSpecCode = "",
            bool isThai = true, string timeSlotId = "", bool smallSize = false, string timeSlot = "", string SubAccessMode = ""
            )
        {

            var strBuilder = new StringBuilder();
            var strRemarkBuilder = new StringBuilder();
            var strTime = new StringBuilder();
            var installationDate = new DateTime();
            var strFBSSTimeSlot = "";

            try
            {
                if (isThai)
                {
                    // DateTimeFormatInfo thDtfi = new CultureInfo("th-TH", false).DateTimeFormat;
                    installationDate = Convert.ToDateTime(installation_Date, new CultureInfo("th-TH"));//
                }
                else
                {
                    DateTimeFormatInfo usDtfi = new CultureInfo("en-GB", false).DateTimeFormat;
                    installationDate = Convert.ToDateTime(installation_Date, usDtfi);
                }

                /// check access_Mode for set FBSSTimeSlot

                // transform to null safe string commpare.
                var LovConstant = (from t in base.LovData
                                   where t.Type == "FBB_CONSTANT"
                                   && t.Name == "DEFAULT_TIMESLOT"
                                   && t.Text == access_Mode
                                   select t.LovValue1).ToList();

                if (LovConstant != null && LovConstant.Count > 0)
                {
                    strFBSSTimeSlot = LovConstant.FirstOrDefault().ToString();
                }

                /// End

                var query = new GetFBSSAppointment()
                {
                    AccessMode = access_Mode,
                    AddressId = address_Id,
                    Days = days,
                    ExtendingAttributes = "",
                    InstallationDate = installationDate.ToString("yyyy-MM-dd"),
                    ProductSpecCode = productSpecCode,
                    District = district,
                    Language = isThai ? "T" : "E",
                    Postal_Code = postalCode,
                    Province = province,
                    Service_Code = serviceCode,
                    SubDistrict = subDistrict,
                    SubAccessMode = SubAccessMode,
                    LineSelect = (LineType)lineSelect
                };

                #region Data test
                //Random rnd = new Random();
                //List<FBSSTimeSlot> data = new List<FBSSTimeSlot>();

                //for (int i = 1; i <= days; i++)
                //{
                //    FBSSTimeSlot timeSlot = new FBSSTimeSlot();
                //    timeSlot.AppointmentDate = installationDate;
                //    timeSlot.InstallationCapacity = rnd.Next(0, 11).ToString() + "/10";
                //    timeSlot.TimeSlot = "08:00 - 10:00";
                //    data.Add(timeSlot);

                //    timeSlot = new FBSSTimeSlot();
                //    timeSlot.AppointmentDate = installationDate;
                //    timeSlot.InstallationCapacity = rnd.Next(0, 11).ToString() + "/10";
                //    timeSlot.TimeSlot = "10:00 - 12:00";
                //    data.Add(timeSlot);

                //    timeSlot = new FBSSTimeSlot();
                //    timeSlot.AppointmentDate = installationDate;
                //    timeSlot.InstallationCapacity = rnd.Next(0, 11).ToString() + "/10";
                //    timeSlot.TimeSlot = "13:00 - 15:00";
                //    data.Add(timeSlot);

                //    timeSlot = new FBSSTimeSlot();
                //    timeSlot.AppointmentDate = installationDate;
                //    timeSlot.InstallationCapacity = rnd.Next(0, 11).ToString() + "/10";
                //    timeSlot.TimeSlot = "15:00 - 18:00";
                //    data.Add(timeSlot);

                //    timeSlot = new FBSSTimeSlot();
                //    timeSlot.AppointmentDate = installationDate;
                //    timeSlot.InstallationCapacity = rnd.Next(0, 11).ToString() + "/10";
                //    timeSlot.TimeSlot = "18:00 - 20:00";
                //    data.Add(timeSlot);

                //    //timeSlot = new FBSSTimeSlot();
                //    //timeSlot.AppointmentDate = installationDate;
                //    //timeSlot.InstallationCapacity = rnd.Next(0, 11).ToString() + "/10";
                //    //timeSlot.TimeSlot = "20:00-22:00";
                //    //listTimeSlot.Add(timeSlot);

                //    installationDate = installationDate.AddDays(1);
                //}
                #endregion

                var data = _queryProcessor.Execute(query);

                if (data != null)
                {
                    if (data.Count > 0)
                    {
                        #region Time column
                        var listDay = base.LovData.Where(p => p.LovValue5 == WebConstants.LovConfigName.CustomerRegisterPageCode &&
                            p.Name.Contains("L_INSTALL_WEEK_OF_DAY")).OrderBy(p => p.OrderBy).ToList();

                        var listMonth = base.LovData.Where(p => p.LovValue5 == WebConstants.LovConfigName.CustomerRegisterPageCode &&
                            p.Name.Contains("L_INSTALL_MONTH_OF_YEAR")).OrderBy(p => p.OrderBy).ToList();

                        var lblTime = isThai ? base.LovData.Where(p => p.Name == "L_INSTALL_TIME").FirstOrDefault().LovValue1 : base.LovData.Where(p => p.Name == "L_INSTALL_TIME").FirstOrDefault().LovValue2;

                        var oldDate = data[0].AppointmentDate;
                        int remarkTop = 0;


                        //L_INSTALL_TIME
                        strBuilder.Append("<div style=\"float:left;\">");
                        strBuilder.Append("<div class=\"select-header-show\">");
                        strBuilder.Append("</div>");
                        strBuilder.Append("<div class=\"time-header\">");
                        strBuilder.Append("<label>" + lblTime + "</label> ");
                        strBuilder.Append("</div>");

                        for (int i = 0; i < data.Count; i++)
                        {
                            if (oldDate == data[i].AppointmentDate && data[i].TimeSlot != "08:00-10:00")
                            {
                                strBuilder.Append("<div class=\"time-item\">");
                                strBuilder.Append("<label id=lbTimeSlot" + i + ">" + data[i].TimeSlot + "</label>");
                                strBuilder.Append("</div>");
                                remarkTop++;
                            }
                        }

                        strBuilder.Append("<div class=\"time-footer\">");
                        strBuilder.Append("</div>");
                        strBuilder.Append("</div>");

                        #endregion

                        #region Day column
                        int LIMIT_TIME_FOR_FAST_JOB = 0;
                        var tmp_limittime = base.LovData.Where(p => p.Name == "LIMIT_TIME_FOR_FAST_JOB").OrderBy(p => p.OrderBy).ToList();
                        if (tmp_limittime != null)
                            if (tmp_limittime.Any())
                                LIMIT_TIME_FOR_FAST_JOB = int.Parse(tmp_limittime.FirstOrDefault().LovValue1);
                        DateTime Curr_dt = DateTime.Now;

                        for (int i = 0; i < data.Count; i++)
                        {
                            if (oldDate != data[i].AppointmentDate || i == 0)
                            {
                                oldDate = data[i].AppointmentDate;

                                if (i != 0)
                                {
                                    strBuilder.Append("<div class=\"day-footer-show\">");
                                    strBuilder.Append("<div class=\"day-footer\">");
                                    strBuilder.Append("</div>");
                                    strBuilder.Append("</div>");
                                    strBuilder.Append("</div>");
                                }

                                var dayNum = data[i].AppointmentDate.Value.Day.ToString();
                                var dayName = isThai ? listDay[(int)data[i].AppointmentDate.Value.DayOfWeek].LovValue1 : listDay[(int)data[i].AppointmentDate.Value.DayOfWeek].LovValue2;
                                var monthName = isThai ? listMonth[data[i].AppointmentDate.Value.Month - 1].LovValue1 : listMonth[data[i].AppointmentDate.Value.Month - 1].LovValue2;

                                strBuilder.Append("<div style=\"float:left;\">");
                                strBuilder.Append("<div class=\"select-header-show\">");
                                strBuilder.Append("<div class=\"select-header\">");
                                strBuilder.Append("<span style=\"color:white;\">" + dayName + "</span>");
                                strBuilder.Append("</div>");
                                strBuilder.Append("</div>");
                                strBuilder.Append("<div class=\"day-header\">");
                                strBuilder.Append("<div class=\"header-label\">");
                                strBuilder.Append("<span>" + dayName + "</span>");
                                strBuilder.Append("<p style=\"margin: 0 0 5px;\"></p>");
                                strBuilder.Append("<span>" + dayNum + " " + monthName + "</span>");
                                strBuilder.Append("</div>");
                                strBuilder.Append("<div class=\"header-label-hidden\">");
                                strBuilder.Append("<span>" + dayNum + "</span>");
                                strBuilder.Append("<p style=\"margin: 0 0 1px;\"></p>");
                                strBuilder.Append("<span>" + monthName + "</span>");
                                strBuilder.Append("</div>");
                                strBuilder.Append("</div>");
                            }

                            if (oldDate == data[i].AppointmentDate && data[i].TimeSlot != "08:00-10:00")
                            {
                                var slot = data[i].InstallationCapacity.Split('/');

                                strBuilder.Append("<div id=\"divSelected\" class=\"day-item\" onclick=\"onDayItemClick(this,'" + timeSlotId + "');\">");
                                strBuilder.Append("<div class=\"day-item-circle\">");

                                if (Convert.ToInt32(slot[0]) <= 0)
                                    strBuilder.Append("<i class=\"fa fa-circle color-red\"></i>");
                                else
                                {
                                    DateTime TimeslotPeriod = new DateTime(data[i].AppointmentDate.Value.Year,
                                        data[i].AppointmentDate.Value.Month,
                                        data[i].AppointmentDate.Value.Day,
                                        int.Parse(data[i].TimeSlot.Substring(0, 2)),
                                        int.Parse(data[i].TimeSlot.Substring(3, 2)),
                                        0);
                                    if (TimeslotPeriod > Curr_dt.AddMinutes(LIMIT_TIME_FOR_FAST_JOB))
                                    {
                                        if (installationDate == data[i].AppointmentDate && data[i].TimeSlot == timeSlot)
                                        {
                                            strBuilder.Append("<i class=\"fa fa-circle color-green\"></i>");
                                        }
                                        else
                                        {
                                            strBuilder.Append("<i class=\"fa fa-circle color-gray\"></i>");
                                        }

                                    }
                                    else
                                    {

                                        if (installationDate == data[i].AppointmentDate && data[i].TimeSlot == timeSlot)
                                        {
                                            strBuilder.Append("<i class=\"fa fa-circle color-green\"></i>");
                                        }
                                        else
                                        {
                                            strBuilder.Append("<i class=\"fa fa-circle color-red\"></i>");
                                        }
                                    }
                                }

                                strBuilder.Append("<div style=\"display:none;\">");
                                strBuilder.Append("<input type=\"text\" name=\"TimeSlotId\" value=\"" + data[i].TimeSlotId + "\"/>");
                                strBuilder.Append("<input type=\"text\" name=\"FBSSAppointmentDate\" value=\"" + data[i].AppointmentDate.Value.ToString("yyyy/MM/dd") + "\"/>");
                                strBuilder.Append("<input type=\"text\" name=\"FBSSTimeSlot\" value=\"" + data[i].TimeSlot + "\"/>");
                                strBuilder.Append("<input type=\"text\" name=\"FBSSInstallationCapacity\" value=\"" + data[i].InstallationCapacity + "\"/>");
                                strBuilder.Append("<input type=\"text\" name=\"FBSSInstallDate\" value=\"" + data[i].AppointmentDate.Value.ToString("dd/MM/yyyy") + "\"/>");
                                strBuilder.Append("</div>");

                                strBuilder.Append("</div>");
                                strBuilder.Append("</div>");
                            }

                        }

                        strBuilder.Append("<div class=\"day-footer-show\">");
                        strBuilder.Append("<div class=\"day-footer\">");
                        strBuilder.Append("</div>");
                        strBuilder.Append("</div>");
                        strBuilder.Append("</div>");
                        strBuilder.Append("<div style=\"clear: both;\"></div>");
                        #endregion

                        QuickWinPanelModel Model = new QuickWinPanelModel();
                        List<FbbConstantModel> SHOW_TIME_SLOT_LIST = new List<FbbConstantModel>();
                        var SHOW_TIME_SLOT = SHOW_TIME_SLOT_LIST.Any(f => f.Validation == Model.FlowFlag) ? SHOW_TIME_SLOT_LIST.First(f => f.Validation == Model.FlowFlag).SubValidation : "";
                        strTime.Append(SHOW_TIME_SLOT);


                        #region Remark
                        string insSelected = isThai ? base.LovData.Where(p => p.Name == "L_INSTALL_SELECTED").Select(p => p.LovValue1).FirstOrDefault() : base.LovData.Where(p => p.Name == "L_INSTALL_SELECTED").Select(p => p.LovValue2).FirstOrDefault();
                        string insAvailable = isThai ? base.LovData.Where(p => p.Name == "L_INSTALL_AVAILABLE").Select(p => p.LovValue1).FirstOrDefault() : base.LovData.Where(p => p.Name == "L_INSTALL_AVAILABLE").Select(p => p.LovValue2).FirstOrDefault();
                        string insNotAvailable = isThai ? base.LovData.Where(p => p.Name == "L_INSTALL_NOT_ AVAILABLE").Select(p => p.LovValue1).FirstOrDefault() : base.LovData.Where(p => p.Name == "L_INSTALL_NOT_ AVAILABLE").Select(p => p.LovValue2).FirstOrDefault();

                        //if (smallSize)
                        //    strBuilder.Append("<div style=\"text-align:center;margin-left:80px;\">");
                        //else
                        //    strBuilder.Append("<div style=\"text-align:center;margin-left:220px;\">");

                        strRemarkBuilder.Append("<fieldset class=\"timeslot-remark center-block\">");
                        strRemarkBuilder.Append("<div style=\"margin-top:3px;\">");
                        strRemarkBuilder.Append("<i class=\"fa fa-circle color-green-remark\"></i>");
                        strRemarkBuilder.Append("&nbsp&nbsp<label>" + insSelected + "</label>");
                        strRemarkBuilder.Append("&nbsp&nbsp<i class=\"fa fa-circle color-gray-remark\"></i>");
                        strRemarkBuilder.Append("&nbsp&nbsp<label>" + insAvailable + "</label>");
                        strRemarkBuilder.Append("&nbsp&nbsp<i class=\"fa fa-circle color-red-remark\"></i>");
                        strRemarkBuilder.Append("&nbsp&nbsp<label>" + insNotAvailable + "</label>");
                        strRemarkBuilder.Append("</div>");
                        strRemarkBuilder.Append("</fieldset>");
                        //strBuilder.Append("</div>");

                        #endregion
                    }
                    else
                    {
                        strBuilder.Append("<div style=\"display:none;\"> List data = 0");
                        strBuilder.Append(", installation_Date = " + installation_Date);
                        strBuilder.Append(", access_Mode = " + access_Mode);
                        strBuilder.Append(", address_Id = " + address_Id);
                        strBuilder.Append(", days = " + days);
                        strBuilder.Append(", productSpecCode =" + productSpecCode);
                        strBuilder.Append(", ExtendingAttributes = ");
                        strBuilder.Append("<input type=\"text\" name=\"TimeSlotId\" value=\"\"/>");
                        strBuilder.Append("<input type=\"text\" name=\"FBSSAppointmentDate\" value=\"" + installationDate.ToString("yyyy/MM/dd") + "\"/>");
                        strBuilder.Append("<input type=\"text\" name=\"FBSSTimeSlot\" value=\"" + strFBSSTimeSlot + "\"/>");
                        strBuilder.Append("<input type=\"text\" name=\"FBSSInstallationCapacity\" value=\"0/1\"/>");
                        strBuilder.Append("<input type=\"text\" name=\"FBSSInstallDate\" value=\"" + installationDate.ToString("dd/MM/yyyy") + "\"/>");
                        strBuilder.Append("</div>");
                    }
                }
                else
                {
                    strBuilder.Append("<div style=\"display:none;\"> Data null");
                    strBuilder.Append(", installation_Date = " + installation_Date);
                    strBuilder.Append(", access_Mode = " + access_Mode);
                    strBuilder.Append(", address_Id = " + address_Id);
                    strBuilder.Append(", days = " + days);
                    strBuilder.Append(", productSpecCode =" + productSpecCode);
                    strBuilder.Append(", ExtendingAttributes = ");
                    strBuilder.Append("<input type=\"text\" name=\"TimeSlotId\" value=\"\"/>");
                    strBuilder.Append("<input type=\"text\" name=\"FBSSAppointmentDate\" value=\"" + installationDate.ToString("yyyy/MM/dd") + "\"/>");
                    strBuilder.Append("<input type=\"text\" name=\"FBSSTimeSlot\" value=\"" + strFBSSTimeSlot + "\"/>");
                    strBuilder.Append("<input type=\"text\" name=\"FBSSInstallationCapacity\" value=\"0/1\"/>");
                    strBuilder.Append("<input type=\"text\" name=\"FBSSInstallDate\" value=\"" + installationDate.ToString("dd/MM/yyyy") + "\"/>");
                    strBuilder.Append("</div>");
                }

                return Json(
                    new { timeSlotData = strBuilder.ToSafeString(), timeSlotRemark = strRemarkBuilder.ToSafeString(), timeDataflag = strTime.ToSafeString() },
                    JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                base._Logger.Info(ex.RenderExceptionMessage());
                strBuilder.Append("<div style=\"display:none;\"> Data error " + ex.ToString());
                strBuilder.Append(", installation_Date = " + installation_Date);
                strBuilder.Append(", access_Mode = " + access_Mode);
                strBuilder.Append(", address_Id = " + address_Id);
                strBuilder.Append(", days = " + days);
                strBuilder.Append(", productSpecCode =" + productSpecCode);
                strBuilder.Append(", ExtendingAttributes = ");
                strBuilder.Append("<input type=\"text\" name=\"TimeSlotId\" value=\"\"/>");
                strBuilder.Append("<input type=\"text\" name=\"FBSSAppointmentDate\" value=\"" + installationDate.ToString("yyyy/MM/dd") + "\"/>");
                strBuilder.Append("<input type=\"text\" name=\"FBSSTimeSlot\" value=\"" + strFBSSTimeSlot + "\"/>");
                strBuilder.Append("<input type=\"text\" name=\"FBSSInstallationCapacity\" value=\"0/1\"/>");
                strBuilder.Append("<input type=\"text\" name=\"FBSSInstallDate\" value=\"" + installationDate.ToString("dd/MM/yyyy") + "\"/>");
                strBuilder.Append("</div>");
            }

            return Json(
                new { timeSlotData = strBuilder.ToSafeString(), timeSlotRemark = strRemarkBuilder.ToSafeString(), },
                JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCheckEventCode(string event_Code)
        {
            var query = new GetCheckEventCodeQuery()
            {
                Event_Code = event_Code
            };

            EventCodeModel data = _queryProcessor.Execute(query);



            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetEventDetail(string EventCode, string DatePeriodTH = "", string DatePeriodEN = "", string IDCardNo = "", string Technology = "")
        {
            //EventCodeModel EventDetail = new EventCodeModel();
            var query = new GetEventDetailQuery()
            {
                DatePeriodTH = DatePeriodTH,
                DatePeriodEN = DatePeriodEN,
                EventCode = EventCode,
                IDCardNo = IDCardNo,
                Language = SiteSession.CurrentUICulture.IsEngCulture(),
                Technology = Technology
            };

            var EventDetail = _queryProcessor.Execute(query);

            return Json(EventDetail, JsonRequestBehavior.AllowGet);
        }

        public String Get_CPEERROR()
        {
            string result = "";
            //var langFlg = Session[WebConstants.FBBConfigSessionKeys.CurrentUICulture].ToSafeString() == "1" ? "TH" : "EN";
            var langFlg = "TH";
            var masterController = Bootstrapper.GetInstance<MasterDataController>();
            LovValueModel msgLov = masterController.GetLovList("SCREEN", "L_ERROR_CPE").ToList().FirstOrDefault();
            if (langFlg == "TH")
            {
                result = msgLov.LovValue1;
            }
            else
            {
                result = msgLov.LovValue2;
            }
            return result;
        }

        public JsonResult Get_ScanBarcode()
        {
            List<LovValueModel> config = base.LovData.Where(l => l.Name == "C_BARCODE" && l.Type == "SCREEN").ToList();

            var c_Barcode = config.Select(i => i.LovValue1).FirstOrDefault();

            return Json(c_Barcode, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ReserveTimeSlot(Guid transactionGuid, decimal timeSlotId = 0)
        {
            var command = new ReserveTimeSlotCommand
            {
                IdReserve = transactionGuid,
                ReserveDTM = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                TimeSlotId = timeSlotId
            };

            _commandReserveTimeslot.Handle(command);

            return Json(command.Return_Code, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// // 16/06/2015 by john
        /// </summary>

        public JsonResult CheckBlackList(string idcardNo = "")
        {

            var query = new evOMServiceIVRCheckBlackListQuery
            {
                inCardNo = idcardNo
            };
            var a = _queryProcessor.Execute(query);


            return Json(a, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckInputMobileList(string lMobile = "")
        {
            bool data = true;

            try
            {
                var query = new GetLovQuery
                {
                    LovType = "FBB_CONSTANT",
                    LovName = "INPUT_MOBILE"
                };

                var result = _queryProcessor.Execute(query);
                var chkResult = result.Where(t => t.LovValue1 == lMobile);
                if (chkResult.Count() > 0)
                {
                    data = true;
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    data = false;
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                _Logger.Info(ex.GetErrorMessage());
                return null;
            }

        }

        [OutputCache(Location = OutputCacheLocation.None, NoStore = false)]
        public JsonResult GetOrderDup(string p_id_card = "", string LanguagePage = "")
        {
            // ส่ง access mode list ที่ได้มาจาก GetFBSSFeasibilityCheckHandler
            //var accessModeModel = new JavaScriptSerializer().Deserialize<List<OrderDupModel>>(OrderDup);
            var query = new GetOrderDupQuery
            {
                p_id_card = p_id_card,
                p_eng_flag = base.GetCurrentCulture().IsEngCulture().ToYesNoFlgString(),
            };

            var orders = _queryProcessor.Execute(query);

            return Json(orders, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetConfirmDeleteMSG()
        {
            var masterController = Bootstrapper.GetInstance<MasterDataController>();
            LovValueModel msgLov = masterController.GetLovList("SCREEN", "L_CONFIRM_CANCEL_DUP").ToList().FirstOrDefault();
            return Json(msgLov, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetConfirmCancelMSG()
        {
            var masterController = Bootstrapper.GetInstance<MasterDataController>();
            LovValueModel msgLov = masterController.GetLovList("SCREEN", "L_BUTTON_CANCEL_ORD").ToList().FirstOrDefault();
            return Json(msgLov, JsonRequestBehavior.AllowGet);
        }


        //public void RemoveOrderDup(String[] DeleteOrderNo[])
        //{
        //    // ส่ง access mode list ที่ได้มาจาก GetFBSSFeasibilityCheckHandler
        //    //var accessModeModel = new JavaScriptSerializer().Deserialize<List<OrderDupModel>>(OrderDup);
        //    //var query = new GetOrderDupQuery
        //    //{
        //    //    p_id_card = p_id_card
        //    //};

        //    //var a = _queryProcessor.Execute(query);

        //   // return Json(a, JsonRequestBehavior.AllowGet);
        //}        


        [HttpPost]
        public ActionResult UploadTest()
        {
            if (Request.Files.Count > 0)
            {
                try
                {
                    List<string> Arr_files = new List<string>();
                    HttpFileCollectionBase files = Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        //string path = AppDomain.CurrentDomain.BaseDirectory + "Uploads/";  
                        //string filename = Path.GetFileName(Request.Files[i].FileName);  

                        HttpPostedFileBase file = files[i];
                        string fname;

                        if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                        {
                            string[] testfiles = file.FileName.Split(new char[] { '\\' });
                            fname = testfiles[testfiles.Length - 1];
                        }
                        else
                        {
                            fname = file.FileName;
                        }

                        if (file.ContentLength > 0)
                        {
                            Arr_files.Add(files.AllKeys[i]);
                        }

                        //fname = Path.Combine(Server.MapPath("~/Uploads/"), fname);
                        //file.SaveAs(fname);
                    }
                    return Json(Arr_files);
                }
                catch (Exception ex)
                {
                    return Json(ex.Message);
                }
            }
            else
            {
                return Json("");
            }
        }

        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public JsonResult ReservedTimeslot(string APPOINTMENT_DATE, string TIME_SLOT, string ACCESS_MODE, string PROD_SPEC_CODE, string ADDRESS_ID, string LOCATION_CODE, string SUBDISTRICT, string POSTAL_CODE, string ASSIGN_RULE, string AISAIRNUMBER, string SUB_ACCESS_MODE = "")
        {
            string FullUrl = "";
            if (Session["FullUrl"] != null)
                FullUrl = Session["FullUrl"].ToSafeString();
            try
            {
                #region Get IP Address Interface Log (Update 17.2)

                string transactionId = "";

                // Get IP Address
                string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (string.IsNullOrEmpty(ipAddress)) ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                transactionId = AISAIRNUMBER + ipAddress;

                #endregion

                ZTEReservedTimeslotQuery query = new ZTEReservedTimeslotQuery()
                {
                    ACCESS_MODE = ACCESS_MODE,
                    ADDRESS_ID = ADDRESS_ID,
                    APPOINTMENT_DATE = APPOINTMENT_DATE.Replace("/", "-"),
                    LOCATION_CODE = LOCATION_CODE,
                    PROD_SPEC_CODE = PROD_SPEC_CODE,
                    TIME_SLOT = TIME_SLOT,
                    SUBDISTRICT = SUBDISTRICT,
                    POSTAL_CODE = POSTAL_CODE,
                    ASSIGN_RULE = ASSIGN_RULE,
                    TRANSACTION_ID = transactionId,
                    SUB_ACCESS_MODE = SUB_ACCESS_MODE,
                    FullUrl = FullUrl
                };
                ZTEReservedTimeslotRespModel zte_result = _queryProcessor.Execute(query);

                return Json(zte_result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new ZTEReservedTimeslotRespModel() { RESULT = "-1", RESULT_DESC = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult IsFastJob(string AppointmentDate, string AccessMode)
        {
            string showPopup = "FALSE";
            int LimitDay = 0;
            var a = base.LovData.Where(p => p.Name == "IS_FAST_JOB" && p.Text == AccessMode);
            if (a != null)
                if (a.Any())
                {
                    LimitDay = int.Parse(a.FirstOrDefault().LovValue1);
                }
            DateTime appointment_dt;
            if (DateTime.TryParseExact(AppointmentDate, "yyyy/MM/dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out appointment_dt))
            {
                DateTime curr_dt = DateTime.Now.Date;
                if (curr_dt.AddDays(LimitDay) > appointment_dt)
                    showPopup = "TRUE";
            }

            return Json(
                    new { Popup = showPopup },
                    JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCheckCPE(string CPECode = "", string playbox = "", string idcardNo = "", string AisAirNumber = "")
        {
            #region Get IP Address Interface Log (Update 17.2)

            // Get IP Address
            string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress)) ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            string transactionId = AisAirNumber + ipAddress;

            #endregion

            // 17.6 Interface Log Add Url
            string FullUrl = "";
            if (Session["FullUrl"] != null)
                FullUrl = Session["FullUrl"].ToSafeString();

            var query = new GetFBSSCheckCPEQuery()
            {
                CPE = CPECode.Trim(),
                playbox = playbox.Trim(),
                IN_ID_CARD_NO = idcardNo.Trim(),
                Transaction_Id = transactionId,
                FullUrl = FullUrl
            };

            var CPEListDetail = _queryProcessor.Execute(query);

            List<CPEINFO> CPE_Info = new List<CPEINFO>();

            int rowNo = 0;
            foreach (var item in CPEListDetail)
            {
                rowNo += 1;

                var s_display_val = CheckCPE_TYPE(item.CPE_TYPE);
                string cpe_err = "";
                string cpe_type = "";

                cpe_type = s_display_val;
                if (s_display_val == "CPE")
                {
                    cpe_err = "";
                    if (rowNo == 2)
                    {
                        cpe_type = "PLAYBOX";
                        cpe_err = Get_CPEERROR();
                    }

                }
                else if (s_display_val == "PLAYBOX")
                {
                    cpe_err = "";
                    if (CPEListDetail.Count == 2)
                    {
                        if (rowNo == 1)
                        {
                            if (cpe_type == "PLAYBOX")
                            {
                                cpe_type = "CPE";
                                cpe_err = Get_CPEERROR();
                            }
                        }
                    }


                }
                else
                {
                    cpe_err = Get_CPEERROR();
                    if (string.IsNullOrEmpty(s_display_val))
                    {
                        if (rowNo == 1)
                        {
                            cpe_type = "CPE";
                        }
                        else
                        {
                            cpe_type = "PLAYBOX";
                        }
                    }

                }

                CPEINFO CPEitem = new CPEINFO();

                if (item.SN == CPECode.Trim())
                {
                    //check cpe
                    if (cpe_err == "")
                    {
                        CPEitem.SN = item.SN;
                        CPEitem.CPE_MAC_ADDR = item.CPE_MAC_ADDR;
                        CPEitem.cpe_type = ("CPE");

                        CPEitem.Check_Result_Code = item.Check_Result_Code;
                        CPEitem.Check_Result_Desc = item.Check_Result_Desc;
                        CPEitem.STATUS_DESC = item.STATUS_DESC;
                        CPEitem.Status_ID = item.Status_ID;
                        CPEitem.CPE_TYPE_ERR = "";

                        //20.4
                        CPEitem.SN_PATTERN = item.SN_PATTERN;
                        CPEitem.CPE_MODEL_NAME = item.CPE_MODEL_NAME;
                        CPEitem.CPE_COMPANY_CODE = item.CPE_COMPANY_CODE;
                        CPEitem.CPE_PLANT = item.CPE_PLANT;
                        CPEitem.CPE_STORAGE_LOCATION = item.CPE_STORAGE_LOCATION;
                        CPEitem.CPE_MATERIAL_CODE = item.CPE_MATERIAL_CODE;
                        CPEitem.REGISTER_DATE = item.REGISTER_DATE;
                        CPEitem.FIBRENET_ID = item.FIBRENET_ID;
                        CPEitem.SHIP_TO = item.SHIP_TO;
                        CPEitem.WARRANTY_START_DATE = item.WARRANTY_START_DATE;
                        CPEitem.WARRANTY_END_DATE = item.WARRANTY_END_DATE;
                        CPEitem.MAC_ADDRESS = item.MAC_ADDRESS;
                    }
                    else
                    {
                        CPEitem.SN = item.SN;
                        CPEitem.CPE_MAC_ADDR = item.CPE_MAC_ADDR;
                        CPEitem.cpe_type = cpe_type;

                        CPEitem.Check_Result_Code = item.Check_Result_Code;
                        CPEitem.Check_Result_Desc = item.Check_Result_Desc;
                        CPEitem.STATUS_DESC = item.STATUS_DESC;
                        CPEitem.Status_ID = item.Status_ID;
                        CPEitem.CPE_TYPE_ERR = cpe_err;

                        //20.4
                        CPEitem.SN_PATTERN = item.SN_PATTERN;
                        CPEitem.CPE_MODEL_NAME = item.CPE_MODEL_NAME;
                        CPEitem.CPE_COMPANY_CODE = item.CPE_COMPANY_CODE;
                        CPEitem.CPE_PLANT = item.CPE_PLANT;
                        CPEitem.CPE_STORAGE_LOCATION = item.CPE_STORAGE_LOCATION;
                        CPEitem.CPE_MATERIAL_CODE = item.CPE_MATERIAL_CODE;
                        CPEitem.REGISTER_DATE = item.REGISTER_DATE;
                        CPEitem.FIBRENET_ID = item.FIBRENET_ID;
                        CPEitem.SHIP_TO = item.SHIP_TO;
                        CPEitem.WARRANTY_START_DATE = item.WARRANTY_START_DATE;
                        CPEitem.WARRANTY_END_DATE = item.WARRANTY_END_DATE;
                        CPEitem.MAC_ADDRESS = item.MAC_ADDRESS;
                    }
                }
                else
                {
                    //check playbox

                    if (cpe_err == "")
                    {
                        CPEitem.SN = item.SN;
                        CPEitem.CPE_MAC_ADDR = item.CPE_MAC_ADDR;
                        CPEitem.cpe_type = ("PLAYBOX");

                        CPEitem.Check_Result_Code = item.Check_Result_Code;
                        CPEitem.Check_Result_Desc = item.Check_Result_Desc;
                        CPEitem.STATUS_DESC = item.STATUS_DESC;
                        CPEitem.Status_ID = item.Status_ID;
                        CPEitem.CPE_TYPE_ERR = "";

                        //20.4
                        CPEitem.SN_PATTERN = item.SN_PATTERN;
                        CPEitem.CPE_MODEL_NAME = item.CPE_MODEL_NAME;
                        CPEitem.CPE_COMPANY_CODE = item.CPE_COMPANY_CODE;
                        CPEitem.CPE_PLANT = item.CPE_PLANT;
                        CPEitem.CPE_STORAGE_LOCATION = item.CPE_STORAGE_LOCATION;
                        CPEitem.CPE_MATERIAL_CODE = item.CPE_MATERIAL_CODE;
                        CPEitem.REGISTER_DATE = item.REGISTER_DATE;
                        CPEitem.FIBRENET_ID = item.FIBRENET_ID;
                        CPEitem.SHIP_TO = item.SHIP_TO;
                        CPEitem.WARRANTY_START_DATE = item.WARRANTY_START_DATE;
                        CPEitem.WARRANTY_END_DATE = item.WARRANTY_END_DATE;
                        CPEitem.MAC_ADDRESS = item.MAC_ADDRESS;
                    }
                    else
                    {
                        CPEitem.SN = item.SN;
                        CPEitem.CPE_MAC_ADDR = item.CPE_MAC_ADDR;
                        CPEitem.cpe_type = cpe_type;//("PLAYBOX");

                        CPEitem.Check_Result_Code = item.Check_Result_Code;
                        CPEitem.Check_Result_Desc = item.Check_Result_Desc;
                        CPEitem.STATUS_DESC = item.STATUS_DESC;
                        CPEitem.Status_ID = item.Status_ID;
                        CPEitem.CPE_TYPE_ERR = cpe_err;

                        //20.4
                        CPEitem.SN_PATTERN = item.SN_PATTERN;
                        CPEitem.CPE_MODEL_NAME = item.CPE_MODEL_NAME;
                        CPEitem.CPE_COMPANY_CODE = item.CPE_COMPANY_CODE;
                        CPEitem.CPE_PLANT = item.CPE_PLANT;
                        CPEitem.CPE_STORAGE_LOCATION = item.CPE_STORAGE_LOCATION;
                        CPEitem.CPE_MATERIAL_CODE = item.CPE_MATERIAL_CODE;
                        CPEitem.REGISTER_DATE = item.REGISTER_DATE;
                        CPEitem.FIBRENET_ID = item.FIBRENET_ID;
                        CPEitem.SHIP_TO = item.SHIP_TO;
                        CPEitem.WARRANTY_START_DATE = item.WARRANTY_START_DATE;
                        CPEitem.WARRANTY_END_DATE = item.WARRANTY_END_DATE;
                        CPEitem.MAC_ADDRESS = item.MAC_ADDRESS;
                    }
                }
                CPE_Info.Add(CPEitem);
            }



            return Json(CPE_Info, JsonRequestBehavior.AllowGet);
        }

        public string CheckCPE_TYPE(string cpe_type)
        {
            List<LovValueModel> lov = base.LovData.Where(l => l.LovValue1 == cpe_type && l.Type == "CHECK_CPE" && l.Name == "CPE_TYPE").ToList();
            var s_display_val = lov.Select(i => i.Text).FirstOrDefault();

            return s_display_val;
        }


    }
}
