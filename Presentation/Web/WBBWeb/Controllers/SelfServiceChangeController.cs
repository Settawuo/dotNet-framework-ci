using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Queries.Commons;
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
    [IENoCache]
    public class SelfServiceChangeController : WBBController
    {
        //
        // GET: /SelfServiceChange/
        private readonly IQueryProcessor _queryProcessor;
        //
        // GET: /Leavemessage/

        public SelfServiceChangeController(IQueryProcessor queryProcessor
              , ILogger logger)
        {
            _queryProcessor = queryProcessor;
            base.Logger = logger;
        }
        [HttpPost]
        public JsonResult SetSession(List<SetSessionSelf> data)
        {
            if (data != null)
            {
                foreach (var item in data)
                {
                    Session[item.session_name] = Base64Encode(item.session_data.ToSafeString());
                }
            }

            return Json(true, JsonRequestBehavior.AllowGet);

        }
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
        public ActionResult Index(string isSaveCommplete = "")
        {
            string order_id = Base64Decode(Session["tracking_order"].ToSafeString());
            if (isSaveCommplete != "")
                ViewBag.SaveSuccess = isSaveCommplete;

            #region viewbag close
            var data = base.LovData
                .Where(l => !string.IsNullOrEmpty(l.Name) && l.Name.Equals("B_CLOSE"))
                .FirstOrDefault();

            if (data != null)
            {
                if (SiteSession.CurrentUICulture.IsThaiCulture())
                {
                    if (data.LovValue1 != null)
                    {
                        ViewBag.LCLOSE = data.LovValue1.ToString();
                    }
                }
                else
                {
                    if (data.LovValue2 != null)
                    {
                        ViewBag.LCLOSE = data.LovValue2.ToString();
                    }
                }
            }
            else
            {
                ViewBag.LCLOSE = "";
            }
            #endregion
            ViewBag.OrderId = order_id;
            ViewBag.User = base.CurrentUser;
            ViewBag.LabelFBBTR000 = GetGeneralScreenConfig();
            ViewBag.LabelWBB = GetScreenConfig("FBBWEB030");
            ViewBag.LabelFBBTR001 = GetCoverageScreenConfig();
            ViewBag.LabelFBBTR003 = GetCustRegisterScreenConfig();
            ViewBag.LabelFBBOR015 = GetProfilePrePostPaid();
            ViewBag.LanguagePage = Session[WBBWeb.Extension.WebConstants.SessionKeys.CurrentUICulture].ToString();
            ViewBag.FbbConstant = GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);
            ViewBag.FbbException = GetFbbConstantModel(WebConstants.LovConfigName.FbbExceptionIdCard);


            return View();
        }
        public ActionResult ManageSelfServiceChange()
        {
            string idcard = Base64Decode(Session["selfservice_idcard"].ToSafeString());
            string mobileno = Base64Decode(Session["selfservice_mobileno"].ToSafeString());
            string card_type = Base64Decode(Session["selfservice_card_type"].ToSafeString());
            string order_no = Base64Decode(Session["selfservice_order_no"].ToSafeString());
            string order_type = Base64Decode(Session["tracking_ordertype"].ToSafeString());
            //check string empty
            if (idcard == "" || mobileno == "")
            {
                return RedirectToAction("Index");
            }
            //
            ///getService
            ///
            var query = new SelfServiceChangeQuery()
            {
                P_ID_CARD = idcard,
                P_MOBILE_NO = mobileno,
                P_ID_CARD_TYPE = card_type,
                P_ORDER_ID = order_no
            };
            var result = _queryProcessor.Execute(query);
            List<ListTimeSlotModel> ser = new List<ListTimeSlotModel>();

            if (result.RET_DATA != null && result.RET_DATA.Count > 0)
            {
                ser = result.RET_DATA;

                //get create prospect
                if (ser.Any(x => x.INTERFACE_STATUS == "Create Prospect" && x.INTERFACE_RESULT == "Success"))
                {
                    List<FIBRENetID> FIBRENetID_List = new List<FIBRENetID>();
                    FIBRENetID mod = new FIBRENetID();
                    mod.FIBRENET_ID = ser.FirstOrDefault().FIBRENET_ID;
                    mod.START_DATE = ser.FirstOrDefault().TEST_DATE;
                    mod.END_DATE = ser.FirstOrDefault().DATE_NOW;
                    FIBRENetID_List.Add(mod);

                    var creat = FBSSQueryOrder(FIBRENetID_List, order_type);
                    if (creat.Order_Details_List != null && creat.Order_Details_List.Count > 0)
                    {

                        //if (creat.Order_Details_List.Any(x => x.ACTIVITY_DETAILS.Any(a => a.ACTIVITY == "Appointment")))
                        //{
                        //var Appointment = creat.Order_Details_List.Select(x => x.ACTIVITY_DETAILS.Where(a => a.ACTIVITY == "Appointment").OrderByDescending(a => a.CREATED_DATE));
                        ser.ToList().ForEach(c =>
                        {
                            c.INSTALL_DATE = creat.Order_Details_List.FirstOrDefault().APPOINTMENT_DATE;
                            c.TIME_SLOT = creat.Order_Details_List.FirstOrDefault().APPOINTMENT_TIMESLOT;
                        }
                              );
                        //}

                    }

                }

            }
            ViewBag.SelfData = ser;
            ///
            ////config
            ViewBag.LabelWBB = GetScreenConfig("FBBWEB030");
            ViewBag.LabelFBBTR003 = GetCustRegisterScreenConfig();
            #region viewbag close
            var data = base.LovData
                .Where(l => !string.IsNullOrEmpty(l.Name) && l.Name.Equals("B_CLOSE"))
                .FirstOrDefault();

            if (data != null)
            {
                if (SiteSession.CurrentUICulture.IsThaiCulture())
                {
                    if (data.LovValue1 != null)
                    {
                        ViewBag.LCLOSE = data.LovValue1.ToString();
                    }
                }
                else
                {
                    if (data.LovValue2 != null)
                    {
                        ViewBag.LCLOSE = data.LovValue2.ToString();
                    }
                }
            }
            else
            {
                ViewBag.LCLOSE = "";
            }
            #endregion
            ////
            //ViewBag.CountAppointment = CountAppointment(idcard);

            string SplitterFlagFirstTime = "2";
            var listTimeSlotMessage = LovData.FirstOrDefault(
                               item => item.Name == (SplitterFlagFirstTime == "2" ? "L_PORT_SPLITTER2_FULL" : "L_PORT_SPLITTER_FULL")
                                   && item.LovValue5 == "FBBOR004");
            ViewBag.TimeSlotMessage = (listTimeSlotMessage != null) ? (SiteSession.CurrentUICulture.IsThaiCulture()) ? listTimeSlotMessage.LovValue1 : listTimeSlotMessage.LovValue2 : "";

            return View();
        }
        public JsonResult CheckMobile(string idcard = "", string mobileno = "", string card_type = "", string order_no = "")
        {
            var query = new SelfServiceChangeQuery()
            {
                P_ID_CARD = idcard,
                P_MOBILE_NO = mobileno,
                P_ID_CARD_TYPE = card_type,
                P_ORDER_ID = order_no
            };
            var result = _queryProcessor.Execute(query);
            List<ListTimeSlotModel> ser = new List<ListTimeSlotModel>();
            if (result.RET_DATA != null && result.RET_DATA.Count > 0)
            {
                ser = result.RET_DATA;
                Session["CONTRACTMOBILENO"] = mobileno;
            }
            return Json(ser, JsonRequestBehavior.AllowGet);

        }

        public JsonResult Submit(ListTimeSlotModel model)
        {
            bool save = false;
            string transactionId = "";
            string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress)) ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            transactionId = model.Mobile_No + ipAddress;
            string FullUrl = this.Url.Action("ManageSelfServiceChange", "SelfServiceChange", null, this.Request.Url.Scheme);
            //Create Prospect
            if (model.INTERFACE_RESULT == "Success" && model.INTERFACE_STATUS == "Create Prospect")
            {
                //SelfServiceAppoint
                #region SelfServiceAppoint
                var selfservice = new ZTESelfServiceAppointQuery()
                {
                    FIBRENET_ID = model.FIBRENET_ID,
                    APPOINTMENT_DATE = model.FBSSAPPOINTMENTDATE,
                    TIME_SLOT = model.FBSSTIMESLOT,
                    LOCATION_CODE = model.LOCATION_CODE,
                    ASSIGN_RULE = model.ASSIGN_RULE,
                    CHANNEL = "Web",
                    INSTALL_STAFF_CODE = "",
                    FullUrl = FullUrl,
                    ID_CARD_NO = model.ID_CARD_NO
                };
                var result_selfservice = _queryProcessor.Execute(selfservice);
                #endregion
                string L_CHANGE_INSTALL_STATUS = "";
                string L_CHANGE_INSTALL_ACTIVITYCATE = "";
                string L_CHANGE_INSTALL_TYPE = "";
                string L_CHANGE_INSTALL_ACTIVITY_SUB_CAT = "";

                if (result_selfservice.RESULT_CODE == "0")
                {
                    if (model.LIST_EVENT_RULE == "UpdateProfile")
                    {
                        save = true;


                    }

                    else
                    {

                        #region SiebelGenActivity
                        var configActivity = SiebelGenActivity();
                        if (configActivity != null && configActivity.Count > 0)
                        {
                            L_CHANGE_INSTALL_STATUS = configActivity.Any(c => c.Name == "L_CHANGE_INSTALL_STATUS") ? configActivity.FirstOrDefault(c => c.Name == "L_CHANGE_INSTALL_STATUS").DisplayValueJing : "";
                            L_CHANGE_INSTALL_ACTIVITYCATE = configActivity.Any(c => c.Name == "L_CHANGE_INSTALL_ACTIVITYCATE") ? configActivity.FirstOrDefault(c => c.Name == "L_CHANGE_INSTALL_ACTIVITYCATE").DisplayValueJing : "";
                            L_CHANGE_INSTALL_TYPE = configActivity.Any(c => c.Name == "L_CHANGE_INSTALL_TYPE") ? configActivity.FirstOrDefault(c => c.Name == "L_CHANGE_INSTALL_TYPE").DisplayValueJing : "";
                            L_CHANGE_INSTALL_ACTIVITY_SUB_CAT = configActivity.Any(c => c.Name == "L_CHANGE_INSTALL_ACTIVITY_SUB_CAT") ? configActivity.FirstOrDefault(c => c.Name == "L_CHANGE_INSTALL_ACTIVITY_SUB_CAT").DisplayValueJing : "";
                        }
                        var genActivity = new SiebelGenActivityQuery()
                        {
                            SRNUMBER = "",
                            ORDERID = "",
                            SERIALNUMBER = "",//model.FIBRENET_ID,
                            DONE = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"),//"07/21/2017 15:55:59",
                            STATUS = L_CHANGE_INSTALL_STATUS,//"Done",
                            PLANNED = "",
                            CAMPAIGNID = "",
                            PRIMARYOWNERID = "",
                            COMMENT = "",
                            NOSOONERTHANDATE = "",
                            ACTIVITYCATEGORY = L_CHANGE_INSTALL_ACTIVITYCATE,//"AIS FIBRE - นัดติดตั้ง",
                            REASON = "",//Unbar GPRS Success
                            TYPE = L_CHANGE_INSTALL_TYPE,//"Web",
                            STARTED = "",
                            DOCUMENTNO = "",
                            MOREINFO = "",
                            ASSETID = "",
                            ACCOUNTID = "NUM" + model.SFF_CA_NUMBER,
                            SUBSTATUS = "",
                            CONTACTID = "",
                            ACTIVITYSUBCATEGORY = L_CHANGE_INSTALL_ACTIVITY_SUB_CAT,//"เลื่อนนัดติดตั้งใช้งาน",
                            PRIORITY = "",
                            PRIMARYPRODUCTID = "",
                            OWNERNAME = "",
                            FullURL = FullUrl,
                            ID_CARD_NO = model.ID_CARD_NO
                        };
                        //var result_genActivity = _queryProcessor.Execute(genActivity);
                        //if (result_genActivity.ERRORMESSAGE == "" && result_genActivity.ERROR_SPCMESSAGE == "")
                        if (true)
                        {

                            var result_change = ChangeInstallDate(model.ORDER_NO, model.ID_CARD_NO, model.Mobile_No, model.FIBRENET_ID, model.FBSSAPPOINTMENTDATE, model.FBSSTIMESLOT);
                            if (result_change.RET_CODE == 0)
                            {
                                save = true;
                            }
                        }
                        #endregion

                    }


                }
            }
            else
            {
                //ReleaseTimeSlot
                #region ReleaseTimeSlot

                var release = new ZTEReleaseTimeslotQuery()
                {
                    RESERVED_ID = model.RESERVED_ID,
                    FullUrl = FullUrl,
                    ID_CARD_NO = model.ID_CARD_NO
                };
                var result_release = _queryProcessor.Execute(release);
                //end
                #endregion
                //ReservedTimeslot
                #region ReservedTimeslot
                //if (result_release.RESULT == "0")
                //{
                List<ASSIGN_CONDITION_ATTR> assignConditionList = new List<ASSIGN_CONDITION_ATTR>
                {
                    new ASSIGN_CONDITION_ATTR(){ ATTR_NAME = "SERVICE_LEVEL", VALUE = model.LIST_SERVICE_LEVEL },
                    new ASSIGN_CONDITION_ATTR(){ ATTR_NAME = "AREA_REGION", VALUE = model.LIST_AREA_REGION},
                    new ASSIGN_CONDITION_ATTR(){ ATTR_NAME = "EVENT_RULE", VALUE = model.LIST_EVENT_RULE }
                };
                var reserved = new ZTEReservedTimeslotQuery()
                {
                    APPOINTMENT_DATE = model.FBSSAPPOINTMENTDATE,
                    TIME_SLOT = model.FBSSTIMESLOT,
                    ACCESS_MODE = model.ACCESS_MODE,
                    PROD_SPEC_CODE = model.PRODUCTSPECCODE,
                    ADDRESS_ID = model.ADDRESS_ID,
                    LOCATION_CODE = model.LOCATION_CODE,
                    SUBDISTRICT = model.TUMBON,
                    POSTAL_CODE = model.ZIPCODE,
                    ASSIGN_RULE = model.ASSIGN_RULE,
                    TRANSACTION_ID = transactionId,
                    FullUrl = FullUrl,
                    ID_CARD_NO = model.ID_CARD_NO,
                    ASSIGN_CONDITION_LIST = assignConditionList
                };
                var result_resered = _queryProcessor.Execute(reserved);
                //end
                #endregion
                //UpdateOrdTimeSlot
                #region UpdateOrdTimeSlot
                if (result_resered.RESULT == "0")
                {
                    var update = new UpdateOrdTimeSlotQuery()
                    {
                        P_ORDER_NO = model.ORDER_NO,
                        P_INSTALL_DATE = model.FBSSAPPOINTMENTDATE,
                        P_TIME_SLOT = model.FBSSTIMESLOT,
                        P_RESERVED_ID = result_resered.RESERVED_ID,
                        P_USER = "FBB",
                        ID_CARD_NO = model.ID_CARD_NO
                    };
                    var result_update = _queryProcessor.Execute(update);

                    if (result_update.RET_CODE == 0)
                    {
                        var result_change = ChangeInstallDate(model.ORDER_NO, model.ID_CARD_NO, model.Mobile_No, model.FIBRENET_ID, model.FBSSAPPOINTMENTDATE, model.FBSSTIMESLOT);
                        if (result_change.RET_CODE == 0)
                        {
                            save = true;
                        }
                    }

                }

                #endregion
                //}

            }
            return Json(save, JsonRequestBehavior.AllowGet);
        }
        public ChangeInstallDateModel ChangeInstallDate(string order_no, string id_card, string mobile_no, string non_mobile_no
            , string installdate, string timeslot)
        {
            var query = new ChangeInstallDateQuery()
            {
                P_ORDER_NO = order_no,
                P_ID_CARD = id_card,
                P_MOBILE_NO = mobile_no,
                P_NON_MOBILE_NO = non_mobile_no,
                P_INSTALL_DATE = installdate,
                P_TIME_SLOT = timeslot
            };
            var result = _queryProcessor.Execute(query);
            return result;
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
        public List<LovScreenValueModel> GetCoverageScreenConfig()
        {
            var screenData = GetScreenConfig(WebConstants.LovConfigName.CoveragePageCode);
            return screenData;
        }
        public List<LovScreenValueModel> GetCustRegisterScreenConfig()
        {
            var screenData = GetScreenConfig(WebConstants.LovConfigName.CustomerRegisterPageCode);
            return screenData;
        }
        public List<LovScreenValueModel> GetGeneralScreenConfig()
        {
            var screenData = GetScreenConfig(null);
            return screenData;
        }
        public List<LovScreenValueModel> GetProfilePrePostPaid()
        {
            var screenData = GetScreenConfig(WebConstants.LovConfigName.CheckPrePostPaid);
            return screenData;
        }
        public List<LovScreenValueModel> SiebelGenActivity()
        {
            var data = base.LovData
             .Where(l => l.Type.Equals("FBB_CONSTANT") && l.LovValue5 == "FBBWEB030")
             .Select(l => new LovScreenValueModel
             {
                 Name = l.Name,
                 DisplayValueJing = l.Text

             }).ToList();

            return data;
        }

        public int CountAppointment(string idcard)
        {
            int count = 0;
            List<AIRNETEntity.StoredProc.TrackingModel> lov;
            var query = new GetTrackingQuery
            {
                P_Id_Card = idcard,
                P_First_Name = "",
                P_Last_Name = "",
                P_Location_Code = "",
                P_Asc_Code = "",
                P_Date_From = "",
                P_Date_To = "",
                P_Cust_Name = "",
                P_User = "CUSTOMER"

            };
            lov = _queryProcessor.Execute(query);
            #region Logic GetOrders

            var data = lov.OrderByDescending(l => l.register_date).ToList().Select(x => new AIRNETEntity.StoredProc.TrackingModel()
            {
                ret_code = x.ret_code,
                ret_msg = x.ret_msg.ToSafeString(),
                order_no = x.order_no.ToSafeString(),
                first_name = x.first_name.ToSafeString(),
                last_name = x.last_name.ToSafeString(),
                work_flow_id = x.work_flow_id,
                flow_seq = x.flow_seq,
                current_state = x.current_state.ToSafeString(),
                register_date = x.register_date,
                register_date_str = x.register_date.ToDateDisplayText(),
                package_name_tha = x.package_name_tha.ToSafeString(),
                house_no = x.house_no.ToSafeString(),
                coverage_flag = x.coverage_flag.ToSafeString(),
                appointment_date_1_str = x.appointment_date_1.ToDateTimeDisplayText(),
                appointment_date_2_str = x.appointment_date_2.ToDateTimeDisplayText(),
                appointment_date_3_str = x.appointment_date_3.ToDateTimeDisplayText(),
                complete_install_date_str = x.complete_install_date.ToDateDisplayText(),
                appointment_date_1 = x.appointment_date_1,
                appointment_date_2 = x.appointment_date_2,
                appointment_date_3 = x.appointment_date_3,
                complete_install_date = x.complete_install_date,
                cancel_install_reason_th = x.cancel_install_reason_th.ToSafeString(),
                cancel_install_reason_en = x.cancel_install_reason_en.ToSafeString(),
                id_card_no = x.id_card_no.ToSafeString(),
                tax_id = x.tax_id.ToSafeString(),
                technology = x.technology.ToSafeString(),
                install_address = x.install_address.ToSafeString(),
                expect_install_date = x.expect_install_date.ToSafeString(),
                ontop_package = x.ontop_package.ToSafeString(),
                onservice_date = x.onservice_date.ToSafeString(),
                fibrenet_id = x.fibrenet_id.ToSafeString(),
                order_type = x.order_type.ToSafeString(),
                start_date = x.start_date.ToSafeString(),
                end_date = x.end_date.ToSafeString(),
                //appointment_timeslot_1_str = x.appointment_timeslot_1_str.ToSafeString(),
                //appointment_timeslot_2_str = x.appointment_timeslot_2_str.ToSafeString(),
                //appointment_timeslot_3_str = x.appointment_timeslot_3_str.ToSafeString()
            });

            List<AIRNETEntity.StoredProc.TrackingModel> dataout = data.ToList();
            foreach (var tmp in dataout)
            {
                if (!string.IsNullOrEmpty(tmp.fibrenet_id)
                    && !string.IsNullOrEmpty(tmp.start_date)
                    && !string.IsNullOrEmpty(tmp.end_date)
                    && !string.IsNullOrEmpty(tmp.order_type))
                {
                    List<FIBRENetID> FIBRENetID_List = new List<FIBRENetID>();
                    FIBRENetID FibreNet = new FIBRENetID()
                    {
                        FIBRENET_ID = tmp.fibrenet_id,
                        START_DATE = tmp.start_date,
                        END_DATE = tmp.end_date
                    };
                    FIBRENetID_List.Add(FibreNet);
                    var result = FBSSQueryOrder(FIBRENetID_List, tmp.order_type);

                    tmp.current_state = "No_Order";
                    if (result.Order_Details_List.Any())
                    {
                        tmp.current_state = "Wait_for_Appointment";

                        tmp.transaction_state = result.Order_Details_List.FirstOrDefault().TRANSACTION_STATE;

                        count = result.Order_Details_List[0].ACTIVITY_DETAILS.Where(x => x.ACTIVITY == "Appointment").Count();

                    }
                }
            }

            #endregion
            return count;
        }
        public QueryOrderModel FBSSQueryOrder(List<FIBRENetID> FIBRENetID_List, string ORDER_TYPE)
        {
            var query = new QueryOrderQuery();
            query.ORDER_TYPE = ORDER_TYPE;
            query.FIBRENetID_List = FIBRENetID_List;

            var result = _queryProcessor.Execute(query);
            return result;
        }

        public JsonResult CheckPremiumInstall(string order_no = "")
        {
            try
            {
                var query = new GetCheckPremiumInstallQuery()
                {
                    p_order_no = order_no
                };

                var result = _queryProcessor.Execute(query);

                if (result != null)
                {
                    if (result.ret_code == 0 && result.ret_msg == "PREMIUM")
                        return Json(result.ret_msg, JsonRequestBehavior.AllowGet);
                }

                return Json("", JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
        }
    }
}
