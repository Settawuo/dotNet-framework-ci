using AIRNETEntity.StoredProc;
using DotNetOpenAuth.Messaging;
using log4net.Appender;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Mvc;
using System.Web.UI;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.Commons.Master;
using WBBContract.Queries.Commons.Masters;
using WBBContract.Queries.WebServices;
using WBBContract.Queries.WebServices.Tracking;
using WBBEntity.Extensions;
using WBBEntity.PanelModels;
using WBBEntity.PanelModels.WebServiceModels;
using WBBWeb.Extension;

namespace WBBWeb.Controllers
{
    [IENoCache]
    public class TrackingController : WBBController
    {
        //
        // GET: /Tracking/
        private readonly ICommandHandler<InterfaceLogCommand> _IntfLogCommand;
        private readonly ICommandHandler<ProfileCustomerCommand> _ProfileCustomerCommand;
        private readonly ICommandHandler<WBBContract.Commands.TrackingLogCommand> _TrackingLogCommand;
        private readonly ICommandHandler<InsertAppointmentTrackingCommand> _InsertAppointmentTrackingCommand;
        private readonly IQueryProcessor _queryProcessor;

        public TrackingController(ILogger logger,
            IQueryProcessor queryProcessor,
            ICommandHandler<InterfaceLogCommand> intfLogCommand,
            ICommandHandler<ProfileCustomerCommand> profileCustomerCommand,
            ICommandHandler<WBBContract.Commands.TrackingLogCommand> trackingLogCommand,
            ICommandHandler<InsertAppointmentTrackingCommand> insertAppointmentTrackingCommand)
        {
            //TestProfileCustomerService(); // For test FBBProfileCustomerService 

            base.Logger = logger;
            _queryProcessor = queryProcessor;
            _IntfLogCommand = intfLogCommand;
            _ProfileCustomerCommand = profileCustomerCommand;
            _TrackingLogCommand = trackingLogCommand;
            _InsertAppointmentTrackingCommand = insertAppointmentTrackingCommand;
        }


        public ActionResult Index()
        {
            Session.Remove("selfservice_idcard");
            Session.Remove("selfservice_mobileno");
            Session.Remove("selfservice_card_type");
            Session.Remove("selfservice_order_no");
            Session.Remove("tracking_order");

            ViewBag.LabelWBB = GetScreenConfig();
            ViewBag.LabelWBBFBBWEB030 = GetScreenConfig("FBBWEB030");
            ViewBag.FbbConstant = GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);
            ViewBag.Language = Session[WebConstants.SessionKeys.CurrentUICulture].ToSafeString() == "1" ? "TH" : "EN";

            return View("TrackingIndex");
        }

        public ActionResult IndexBack(string backkey = "")
        {
            InterfaceLogCommand log = null;
            var idCard = string.Empty;

            _ = Index();
            if (!string.IsNullOrEmpty(backkey))
            {
                var toDecode = new GetEncryptDecryptQuery
                {
                    ToDecryptText = backkey,
                };
                var result = _queryProcessor.Execute(toDecode);
                if (!string.IsNullOrEmpty(result?.DecryptResult))
                {
                    ViewBag.IdCard = result?.DecryptResult.Replace("idCard=", string.Empty);
                }
            }

            log = StartInterface(backkey, "IndexBack", idCard, idCard, "Tracking");
            EndInterface("", log, idCard, "Success", "");

            return View("TrackingIndex");
        }

        [HttpPost]
        public ActionResult GotoTrack(GotoTrackModel track)
        {
            var url = new UriBuilder(track.wfmfurl);
            url.AppendQueryArgument("type", track.p_type);
            url.AppendQueryArgument("language", track.p_language);
            url.AppendQueryArgument("option", track.p_option);
            url.AppendQueryArgument("channel", track.p_channel);
            url.AppendQueryArgument("refId", track.p_refId);

            var idCard = Session["tracking_idcard"].ToString();
            Session.Remove("tracking_idcard");
            if (!string.IsNullOrEmpty(track.p_urlback))
            {
                url.AppendQueryArgument("urlback", track.p_urlback);
            }

            InterfaceLogCommand log = StartInterface(track, "GotoTrack", idCard, idCard, "Tracking");
            EndInterface("", log, idCard, "Success", "");

            return new RedirectResult(url.Uri.AbsoluteUri);
        }

        [HttpPost]
        [AjaxValidateAntiForgeryToken]
        public JsonResult WriteToLog(GotoTrackModel track)
        {
            try
            {
                var idCard = Session["tracking_idcard"].ToString();
                Session.Remove("tracking_idcard");

                InterfaceLogCommand log = StartInterface(track, "GotoTrack", idCard, idCard, "Tracking");
                EndInterface("", log, idCard, "Success", "");
                return Json("Success");
            }
            catch (Exception)
            {
                return Json("Error");
            }
        }

        // P_Id_Card = "3209800123027", 
        // Get Order
        // เมื่อมีการแก้ไข Logic กรุณาแก้ไขที่ FBBConfig.Controllers => SaleTrackingController Method GetOrders ด้วย
        [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
        public JsonResult GetCustormes(string idcard, string firstname, string lastname)
        {
            try
            {
                var query = new GetTrackingQuery
                {
                    P_Id_Card = idcard.ToSafeString(),
                    P_First_Name = firstname.ToSafeString(),
                    P_Last_Name = lastname.ToSafeString(),
                    P_Location_Code = "",
                    P_Asc_Code = "",
                    P_Date_From = "",
                    P_Date_To = "",
                    P_Cust_Name = "",
                    P_User = "CUSTOMER"
                };
                Session.Remove("TRACKINGDATA");
                var lov = _queryProcessor.Execute(query);
                var data = lov.Select(x => new { x.first_name, x.last_name }).Distinct().ToList();
                Session["TRACKINGDATA"] = lov;

                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Logger.Info("InnerException: " + ex.InnerException.ToString() + "/ Message: " + ex.Message + "/ StackTrace: " + ex.StackTrace.ToString());
                return Json("InnerException: " + ex.InnerException.ToString() + "/ Message: " + ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        // GetOrders
        // เมื่อมีการแก้ไข Logic กรุณาแก้ไขที่ FBBConfig.Controllers => SaleTrackingController Method GetOrders ด้วย
        [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
        public JsonResult GetOrders(string idcard, string firstname, string lastname, string location_code, string asc_code, string date_from, string date_to, string cust_name, string user)
        {
            // Get IP Address
            string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress)) ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            // Get FullUrl
            Session["FullUrl"] = this.Url.Action("TrackingIndex", "Tracking", null, this.Request.Url.Scheme);
            string FullUrl = Session["FullUrl"] != null ? Session["FullUrl"].ToSafeString() : "";

            var lov = new List<AIRNETEntity.StoredProc.TrackingModel>();
            decimal count_appointment = 0;
            if (Session["TRACKINGDATA"] != null)
            {
                var lov2 = (List<AIRNETEntity.StoredProc.TrackingModel>)(Session["TRACKINGDATA"]);

                lov = lov2;

                if (!string.IsNullOrEmpty(idcard))
                {
                    lov = (from track in lov
                           where track.id_card_no == idcard || track.tax_id == idcard
                           select track).ToList();
                }
                if (!string.IsNullOrEmpty(firstname))
                {
                    lov = (from track in lov
                           where track.first_name == null || track.first_name.ToUpper() == firstname.ToUpper()
                           select track).ToList();
                }
                if (!string.IsNullOrEmpty(lastname))
                {
                    lov = (from track in lov
                           where track.last_name == null || track.last_name.ToUpper() == lastname.ToUpper()
                           select track).ToList();
                }
            }
            else
            {
                var query = new GetTrackingQuery
                {
                    P_Id_Card = idcard,
                    P_First_Name = firstname,
                    P_Last_Name = lastname,
                    P_Location_Code = location_code,
                    P_Asc_Code = asc_code,
                    P_Date_From = date_from,
                    P_Date_To = date_to,
                    P_Cust_Name = cust_name,
                    P_User = user

                };
                lov = _queryProcessor.Execute(query);
            }

            // เมื่อมีการแก้ไข Logic กรุณาแก้ไขที่ FBBConfig.Controllers => SaleTrackingController Method GetOrders ด้วย
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
                additional_package = x.additional_package
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

                        var Appointment_State_Info = result.Order_Details_List[0].ACTIVITY_DETAILS.Where(x => x.ACTIVITY == "Appointment");
                        if (Appointment_State_Info.Any())
                        {
                            tmp.current_state = "Appointment";
                            int count = 1;

                            //foreach (var info_tmp in Appointment_State_Info)
                            //{
                            //    PropertyInfo propertyInfo = tmp.GetType().GetProperty("appointment_date_" + count + "_str");
                            //    propertyInfo.SetValue(tmp, Convert.ChangeType(info_tmp.APPOINTMENT_DATE, propertyInfo.PropertyType), null);

                            //    propertyInfo = tmp.GetType().GetProperty("appointment_timeslot_" + count + "_str");
                            //    propertyInfo.SetValue(tmp, Convert.ChangeType(info_tmp.APPOINTMENT_TIMESLOT, propertyInfo.PropertyType), null);
                            //    count++;
                            //}


                            // Update 17.8 
                            foreach (var info_tmp in Appointment_State_Info)
                            {
                                // Save Appointment
                                var command = new InsertAppointmentTrackingCommand
                                {
                                    order_no = tmp.order_no,
                                    id_card_no = tmp.id_card_no,
                                    non_mobile_no = tmp.fibrenet_id,
                                    create_date_zte = info_tmp.CREATED_DATE,
                                    appointment_date = info_tmp.APPOINTMENT_DATE,
                                    appointment_timeslot = info_tmp.APPOINTMENT_TIMESLOT,

                                    client_ip = ipAddress,
                                    FullUrl = FullUrl
                                };

                                _InsertAppointmentTrackingCommand.Handle(command);
                            }

                            // Get Appointment
                            var query = new GetAppointmentTrackingQuery
                            {
                                order_no = tmp.order_no,
                                id_card_no = tmp.id_card_no,
                                non_mobile_no = tmp.fibrenet_id
                            };

                            List<AppointmentDisplayTrackingList> DisplayTrackingList = _queryProcessor.Execute(query);

                            if (DisplayTrackingList.Any())
                            {
                                foreach (var info_appointment in DisplayTrackingList)
                                {
                                    PropertyInfo propertyInfo_date = tmp.GetType().GetProperty("appointment_date_" + count + "_str");
                                    if (propertyInfo_date != null)
                                        propertyInfo_date.SetValue(tmp, Convert.ChangeType(info_appointment.appointment_date.ToSafeString(), propertyInfo_date.PropertyType), null);

                                    PropertyInfo propertyInfo_timeslot = tmp.GetType().GetProperty("appointment_timeslot_" + count + "_str");
                                    if (propertyInfo_timeslot != null)
                                        propertyInfo_timeslot.SetValue(tmp, Convert.ChangeType(info_appointment.appointment_timeslot.ToSafeString(), propertyInfo_timeslot.PropertyType), null);

                                    count++;
                                }
                            }
                        }

                        //R24.09 Edit condition add CURRENT_WORK_ORDER_FLAG check Normal (Because FBSS has only these few parameters available)
                        var Install_State_Info = result.Order_Details_List[0].ACTIVITY_DETAILS.Where(x => x.ACTIVITY == "Install" && x.CURRENT_WORK_ORDER_FLAG == "Normal");
                        if (Install_State_Info.Any())
                        {
                            tmp.current_state = "Installation";
                            var complete_state = Install_State_Info.Where(x => string.IsNullOrEmpty(x.FOA_REJECT_REASON));
                            if (complete_state.Any())
                            {
                                DateTime CompleteInstallDate;
                                tmp.complete_install_date = DateTime.TryParseExact(complete_state.OrderByDescending(x => x.COMPLETED_DATE).FirstOrDefault().COMPLETED_DATE, "dd/MM/yyyy HH:mm:ss", new CultureInfo("en-US"), DateTimeStyles.None, out CompleteInstallDate) ? CompleteInstallDate : tmp.complete_install_date;
                                tmp.complete_install_date_str = complete_state.OrderByDescending(x => x.COMPLETED_DATE).FirstOrDefault().COMPLETED_DATE;
                            }
                            else
                            {
                                var newest_state = Install_State_Info.OrderByDescending(x => x.COMPLETED_DATE).FirstOrDefault();
                                tmp.complete_install_date_str = newest_state.COMPLETED_DATE;
                                tmp.cancel_install_reason_en = newest_state.FOA_REJECT_REASON;
                            }
                            if (string.IsNullOrEmpty(idcard)
                                && !string.IsNullOrEmpty(tmp.id_card_no))
                            {
                                idcard = tmp.id_card_no;
                            }
                            if (string.IsNullOrEmpty(idcard))
                            {
                                Logger.Warn($"Can identify for idCard for order no '{tmp.order_no}'");
                            }
                            Session["tracking_idcard"] = idcard;

                            var req = new GetLovQuery { LovType = "CONFIG_TRACKING", LovName = "URL_WMFURL" };
                            var wmfurl = _queryProcessor.Execute(req).FirstOrDefault() ?? new LovValueModel();
                            if (wmfurl?.ActiveFlag == "Y")
                            {
                                var reqGotoTrack = new TechnicianTrackingQuery
                                {
                                    prefixUrl = Request.Url.GetLeftPart(UriPartial.Authority),
                                    idcard = idcard,
                                    fibrenetId = tmp.fibrenet_id,
                                    language = Session[WBBWeb.Extension.WebConstants.SessionKeys.CurrentUICulture].ToString() == "1" ? "TH" : "EN"
                                };
                                var resGotoTrack = _queryProcessor.Execute(reqGotoTrack);
                                tmp.track = resGotoTrack?.AsTotractModel();
                            }
                            tmp.track_enable = wmfurl?.ActiveFlag == "Y";
                        }

                        var Complete_State_Info = result.Order_Details_List[0].ACTIVITY_DETAILS.Where(x => x.ACTIVITY == "SFF");
                        if (Complete_State_Info.Any())
                        {
                            tmp.current_state = "On Service";
                            tmp.onservice_date = Complete_State_Info.FirstOrDefault().COMPLETED_DATE;

                            var req = new GetLovQuery { LovType = "CONFIG_TRACKING", LovName = "URL_CSATURL" };
                            var csat = _queryProcessor.Execute(req).FirstOrDefault() ?? new LovValueModel();
                            tmp.customerSatisfactionSurveyUrl = csat?.ActiveFlag == "Y" ? csat?.LovValue1 : string.Empty;
                        }
                    }
                    if (string.IsNullOrEmpty(tmp.additional_package)) tmp.additional_package = "-";
                    if (string.IsNullOrEmpty(tmp.ontop_package)) tmp.ontop_package = "-";
                }


                var count_ap = new ChangeCountInstallDateQuery
                {
                    p_id_card = idcard,
                    p_order_no = tmp.order_no
                };
                var c = _queryProcessor.Execute(count_ap);

                tmp.count_appointment = c.RET_RESULT;

                var ch = new CheckOrderCancelQuery()
                {
                    P_ORDER_ID = tmp.order_no,
                    P_ID_CARD = idcard
                };
                var result_ordercancel = _queryProcessor.Execute(ch);
                tmp.order_cancel = result_ordercancel.RET_CODE == -1 ? false : true;
            }


            #endregion

            Session.Remove("TRACKINGDATA");

            return Json(new { data = dataout }, JsonRequestBehavior.AllowGet);

        }

        public QueryOrderModel FBSSQueryOrder(List<FIBRENetID> FIBRENetID_List, string ORDER_TYPE)
        {
            var query = new QueryOrderQuery();
            query.ORDER_TYPE = ORDER_TYPE;
            query.FIBRENetID_List = FIBRENetID_List;

            var result = _queryProcessor.Execute(query);
            return result;
        }

        private List<AIRNETEntity.PanelModel.LovScreenValueModel> GetScreenConfig(string pageCode = "")
        {
            try
            {
                List<LovValueModel> config = null;

                config = base.LovData.Where(l => l.Type.Equals("SCREEN")).ToList();
                if (pageCode != "")
                {
                    config = config.Where(x => x.LovValue5 == pageCode).ToList();
                }
                var screenValue = new List<AIRNETEntity.PanelModel.LovScreenValueModel>();
                //if (SiteSession.CurrentUICulture.IsThaiCulture())

                if (Session[WebConstants.SessionKeys.CurrentUICulture].ToSafeString() == "1")
                {
                    screenValue = config.Select(l => new AIRNETEntity.PanelModel.LovScreenValueModel
                    {
                        Name = l.Name,
                        PageCode = l.LovValue5,
                        DisplayValue = l.LovValue1,
                        GroupByPDF = l.LovValue4,
                        OrderByPDF = l.OrderBy,
                    }).ToList();
                }
                else
                {
                    screenValue = config.Select(l => new AIRNETEntity.PanelModel.LovScreenValueModel
                    {
                        Name = l.Name,
                        PageCode = l.LovValue5,
                        DisplayValue = l.LovValue2,
                        GroupByPDF = l.LovValue4,
                        OrderByPDF = l.OrderBy,
                    }).ToList();
                }

                return screenValue;
            }
            catch (Exception ex)
            {
                Logger.Info(ex.GetErrorMessage());
                return new List<AIRNETEntity.PanelModel.LovScreenValueModel>();
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

        public void SaveTrackingLog(string IDCard, string FirstName, string LastName, string resultCode, string returnOrder)
        {
            try
            {
                WBBContract.Commands.TrackingLogCommand command = new WBBContract.Commands.TrackingLogCommand();
                command.IDCard = IDCard;
                command.FirstName = FirstName;
                command.LastName = LastName;
                command.CreatedBy = "system";
                if (!string.IsNullOrEmpty(returnOrder)) { resultCode = "1"; } else { resultCode = "0"; }
                command.ResultCode = resultCode;
                command.ReturnOrder = returnOrder;
                _TrackingLogCommand.Handle(command);
            }
            catch (Exception ex)
            {
                Logger.Info(ex.GetErrorMessage());
            }
        }

        //public string ToDateDisplayText(this DateTime? dt)
        //{
        //    return ToDisplayText(dt, Constants.DisplayFormats.DateFormat);
        //}

        public void TestProfileCustomerService()
        {
            //FBBProfileCustomer.FBBProfileCustomer pc = new FBBProfileCustomer.FBBProfileCustomer();
            //FBBProfileCustomer.ProfileCustomerCommand cm = new FBBProfileCustomer.ProfileCustomerCommand();

            //cm.Cust_Non_Mobile = "8900012841";
            //cm.Ca_Id = "31100000023781";
            //cm.Sa_Id = "31100000023781";
            //cm.Ba_Id = "31100000023781";
            //cm.Ia_Id = "5021660005";
            //cm.Cust_Name = "aaa";
            //cm.Cust_Surname = "aaa";
            //cm.Cust_Id_Card_Type = "ID_CARD";
            //cm.Cust_Id_Card_Num = "31100000023781";
            //cm.Cust_Category = "R";
            //cm.Cust_Sub_Category = "T";
            //cm.Cust_Gender = "Male";
            //cm.Cust_Birthday = "03/06/1989";
            //cm.Cust_Nationality = "THAI";
            //cm.Cust_Title = "Miss";
            //cm.Online_Number = "Direct";
            //cm.Condo_Type = "";
            //cm.Condo_Direction = "";
            //cm.Condo_Limit = "";
            //cm.Condo_Area = "";
            //cm.Home_Type = "";
            //cm.Home_Area = "";
            //cm.Document_Type = "";
            //cm.Cvr_Id = "";
            //cm.Port_Id = "";
            //cm.Order_No = "";
            //cm.Remark = "";
            //cm.Port_Active_Date = "15/09/2014";
            //cm.Installation_Date = "15/09/2014";

            //List<FBBProfileCustomer.REC_CUST_CONTACT> listCustContact = new List<FBBProfileCustomer.REC_CUST_CONTACT>();
            //FBBProfileCustomer.REC_CUST_CONTACT custContact = new FBBProfileCustomer.REC_CUST_CONTACT();
            //custContact.Address_In_Seq = "";
            //custContact.Contact_First_Name = "aaa";
            //custContact.Contact_Last_Name = "aaa";
            //custContact.Contact_Home_Phone = "021234567";
            //custContact.Contact_Mobile_Phone1 = "";
            //custContact.Contact_Mobile_Phone2 = "";
            //custContact.Contact_Email = "siriratr@ais.co,th";
            //listCustContact.Add(custContact);

            //cm.Rec_Cust_Contact = listCustContact.ToArray();

            //List<FBBProfileCustomer.REC_CUST_ADDRESS> listCustAddress = new List<FBBProfileCustomer.REC_CUST_ADDRESS>();
            //FBBProfileCustomer.REC_CUST_ADDRESS custAddress = new FBBProfileCustomer.REC_CUST_ADDRESS();
            //custAddress.Addr_Type = "LG";
            //custAddress.House_No = "1";
            //custAddress.Soi = "";
            //custAddress.Moo = "";
            //custAddress.Mooban = "";
            //custAddress.Building_Name = "";
            //custAddress.Floor = "";
            //custAddress.Room = "";
            //custAddress.Street_Name = "";
            //custAddress.Zipcode_Id = "6A3C56EE181661C0E0440000BEA816B7";
            //custAddress.Address_Vat = "";
            //custAddress.Address_In_Seq = "";
            //listCustAddress.Add(custAddress);

            //cm.Rec_Cust_Address = listCustAddress.ToArray();

            //List<FBBProfileCustomer.REC_CUST_ASSET> listCustAsset = new List<FBBProfileCustomer.REC_CUST_ASSET>();
            //FBBProfileCustomer.REC_CUST_ASSET custAsset = new FBBProfileCustomer.REC_CUST_ASSET();
            //custAsset.Asset_Code = "P1234";
            //custAsset.Package_Code = "P1234";
            //custAsset.Equipment_Type = "";
            //custAsset.Asset_Status = "";
            //custAsset.Asset_Name = "";
            //custAsset.Asset_Charge = "";
            //custAsset.Asset_Discount = "100";
            //custAsset.Asset_Start_Dt = "01/06/2014";
            //custAsset.Asset_End_Dt = "01/06/2014";
            //custAsset.Serial_Number = "1234";
            //custAsset.Asset_Model = "";
            //listCustAsset.Add(custAsset);

            //cm.Rec_Cust_Asset = listCustAsset.ToArray();

            //List<FBBProfileCustomer.REC_CUST_PACKAGE> listCustPackage = new List<FBBProfileCustomer.REC_CUST_PACKAGE>();
            //FBBProfileCustomer.REC_CUST_PACKAGE custPackage = new FBBProfileCustomer.REC_CUST_PACKAGE();
            //custPackage.Package_Code = "P1234";
            //custPackage.Package_Class = "4";
            //custPackage.Package_Type = "";
            //custPackage.Package_Group = "";
            //custPackage.Package_Subtype = "";
            //custPackage.Package_Owner = "";
            //custPackage.Technology = "";
            //custPackage.package_Status = "";
            //custPackage.package_Name = "";
            //custPackage.Recurring_Charge = "100";
            //custPackage.Recurring_Discount = "100";
            //custPackage.Recurring_Discount_Exp = "01/06/2014";
            //custPackage.Recurring_Start_Dt = "01/06/2014";
            //custPackage.Recurring_End_Dt = "01/06/2014";
            //custPackage.Initiation_Charge = "100";
            //custPackage.Initiation_Discount = "100";
            //custPackage.Package_Bill_Tha = "1234";
            //custPackage.Download_Speed = "7GB";
            //custPackage.Upload_Speed = "7GB";
            //listCustPackage.Add(custPackage);

            //cm.Rec_Cust_Package = listCustPackage.ToArray();

            //int ret_code;
            //bool ret_code_bool;
            //string ret_message;

            //pc.SaveProfileCustomer(cm, out ret_code, out ret_code_bool, out ret_message);
        }

        [HttpPost]
        public JsonResult GetAppLog(GetLogQuery query)
        {
            var result = _queryProcessor.Execute(query);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetWebLog([System.Web.Http.FromBody] GetLogQuery query)
        {
            var repo = base.Logger.Logger.Repository;
            var logAppender = repo.GetAppenders().OfType<FileAppender>().Where(t => t.Name == "RollingLogFileAppender").FirstOrDefault();

            string filename = logAppender != null ? logAppender.File : string.Empty;

            if (!string.IsNullOrEmpty(query.Date))
            {
                filename += query.Date;
            }

            if (!string.IsNullOrEmpty(query.LogAppendedNo))
            {
                filename += "." + query.LogAppendedNo;
            }

            var result = new StringBuilder();
            FileStream fs = null;
            try
            {
                fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using (var sr = new StreamReader(fs, Encoding.UTF8))
                {
                    var lot = "";
                    while ((lot = sr.ReadLine()) != null)
                    {
                        result.AppendLine(lot);
                    }
                }
            }
            finally
            {
                if (null != fs)
                {
                    fs.Dispose();
                }
            }

            return Json(result.ToString(), JsonRequestBehavior.AllowGet);
        }

        [AjaxValidateAntiForgeryToken]
        public ActionResult CheckPdf(string orderNo)
        {
            try
            {
                Session["FILEPDF"] = null;

                var query = new GetPathPdfQuery
                {
                    OderNo = orderNo
                };

                var result = _queryProcessor.Execute(query);

                var path = result.PathPdf;
                Session["FILEPDF"] = path;

                if (string.IsNullOrEmpty(path)) return Json(new { status = "notFound" }, JsonRequestBehavior.AllowGet);

                return Json(new { status = "found" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {

            }
            return new EmptyResult();
        }

        public ActionResult Download()
        {
            try
            {
                var impersonateVar = base.LovData.SingleOrDefault(l => l.Type == "FBB_CONSTANT" && l.Name == "Impersonate");
                if (impersonateVar == null) return new EmptyResult();
                string user = impersonateVar.LovValue1;
                string pass = impersonateVar.LovValue2;
                string ip = impersonateVar.LovValue3;

                using (var impersonator = new Impersonator(user, ip, pass, false))
                {
                    string pathPdf = Session["FILEPDF"].ToSafeString();
                    string filename = !string.IsNullOrEmpty(pathPdf) ? (pathPdf.Split('\\').Last()) : String.Empty;
                    byte[] bytes = System.IO.File.ReadAllBytes(pathPdf);
                    return File(bytes, "application/pdf", filename);
                }
            }
            catch (Exception)
            {
                return new EmptyResult();
            }

        }

        private InterfaceLogCommand StartInterface<T>(T query, string methodName, string transactionId, string idCardNo, string INTERFACE_NODE)
        {
            string FullUrl = Request.Url.ToString();
            string SERVICE_NAME = INTERFACE_NODE;

            var dbIntfCmd = new InterfaceLogCommand
            {
                ActionType = WBBContract.Commands.ActionType.Insert,
                IN_TRANSACTION_ID = transactionId,
                METHOD_NAME = methodName,
                SERVICE_NAME = SERVICE_NAME,
                IN_ID_CARD_NO = idCardNo,
                IN_XML_PARAM = query.DumpToXml(),
                INTERFACE_NODE = $"{INTERFACE_NODE}|{FullUrl}",
                CREATED_BY = "FBBWEB",
            };

            _IntfLogCommand.Handle(dbIntfCmd);

            return dbIntfCmd;
        }

        private void EndInterface<T>(T output, InterfaceLogCommand dbIntfCmd, string transactionId, string result, string reason)
        {
            if (null == dbIntfCmd)
                return;

            dbIntfCmd.ActionType = WBBContract.Commands.ActionType.Update;
            dbIntfCmd.REQUEST_STATUS = (result == "Success") ? "Success" : "Error";
            dbIntfCmd.OUT_RESULT = result;
            dbIntfCmd.OUT_ERROR_RESULT = (result == "Success") ? (reason.Length > 100 ? reason.Substring(0, 100) : result) : result;
            dbIntfCmd.OUT_XML_PARAM = (result == "Success") ? output.DumpToXml() : reason;

            _IntfLogCommand.Handle(dbIntfCmd);
        }
    }
}
