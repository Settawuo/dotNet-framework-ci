using iTextSharp.text;
using iTextSharp.text.html;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Xml.Serialization;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.Commons.Master;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBContract.Queries.WebServices;
using WBBEntity.Extensions;
using WBBEntity.PanelModels;
using WBBEntity.PanelModels.ExWebServiceModels;
using WBBEntity.PanelModels.FBBWebConfigModels;
using WBBEntity.PanelModels.WebServiceModels;
using WBBWeb.Extension;
using WBBWeb.Hubs;
using WBBWeb.Models;
using WBBWeb.Solid.CompositionRoot;

namespace WBBWeb.Controllers
{
    [CustomActionFilter]
    [CustomHandleError]
    [IENoCache]
    public class ScpeController : WBBController
    {
        //
        // GET: /Scpe/

        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<NotificationCommand> _noticeCommand;
        private readonly ICommandHandler<SendSmsCommand> _sendSmsCommand;
        private readonly ICommandHandler<CustRegisterCommand> _custRegCommand;
        private readonly ICommandHandler<MailLogCommand> _mailLogCommand;
        private readonly ICommandHandler<InterfaceLogCommand> _intfLogCommand;
        private readonly ICommandHandler<UpdateFileNameCommand> _updateFileNameCommand;
        private readonly ICommandHandler<SavePaymentLogCommand> _savePaymentLogCommand;
        private readonly ICommandHandler<CustRegisterJobCommand> _custRegJobCommand;

        public ScpeController(IQueryProcessor queryProcessor,
            ICommandHandler<NotificationCommand> noticeCommand,
            ICommandHandler<SendSmsCommand> SendSmsCommand,
            ICommandHandler<CustRegisterCommand> custRegCommand,
            ICommandHandler<MailLogCommand> mailLogCommand,
            ICommandHandler<InterfaceLogCommand> intfLogCommand,
            ICommandHandler<UpdateFileNameCommand> UpdateFileNameCommand,
            ICommandHandler<SavePaymentLogCommand> savePaymentLogCommand,
            ICommandHandler<CustRegisterJobCommand> custRegJobCommand,
            ILogger logger)
        {
            _queryProcessor = queryProcessor;
            _noticeCommand = noticeCommand;
            _sendSmsCommand = SendSmsCommand;
            _custRegCommand = custRegCommand;
            _mailLogCommand = mailLogCommand;
            _intfLogCommand = intfLogCommand;
            _updateFileNameCommand = UpdateFileNameCommand;
            _savePaymentLogCommand = savePaymentLogCommand;
            _custRegJobCommand = custRegJobCommand;
            base.Logger = logger;
        }

        // Inactive SCPE 10/07/2024 #Comment Index
        #region Inactive SCPE 10/07/2024 #Comment Index
        //public ActionResult Index(string statusPay = "")
        //{
        //    Session["FullUrl"] = this.Url.Action("Index", "SCPE", null, this.Request.Url.Scheme);
        //    string FullUrl = "";
        //    if (Session["FullUrl"] != null)
        //        FullUrl = Session["FullUrl"].ToSafeString();

        //    if (Session[WebConstants.SessionKeys.CurrentUICulture] == null)
        //    {
        //        SiteSession.CurrentUICulture = 1;
        //        Session[WebConstants.SessionKeys.CurrentUICulture] = 1;
        //    }

        //    if (TempData["SaveStatus"] != null)
        //    {
        //        ViewBag.SaveStatus = TempData["SaveStatus"];
        //    }
        //    TempData["SaveStatus"] = null;

        //    if (TempData["CoverageResultSaveStatus"] != null)
        //    {
        //        ViewBag.CoverageResultSaveStatus = TempData["CoverageResultSaveStatus"];
        //    }
        //    TempData["CoverageResultSaveStatus"] = null;

        //    var controller = DependencyResolver.Current.GetService<ProcessController>();
        //    ViewBag.FbbConstant = controller.GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);
        //    ViewBag.FbbException = controller.GetFbbConstantModel(WebConstants.LovConfigName.FbbExceptionIdCard);
        //    ViewBag.LabelFBBOR028 = controller.GetScreenConfig("FBBOR028");
        //    ViewBag.LabelFBBOR003 = controller.GetScreenConfig("FBBOR003");
        //    ViewBag.labelFBBTR004 = controller.GetScreenConfig("FBBOR004");
        //    ViewBag.TimeSlotConfigSelectDate = base.LovData.Any(f => f.Name == "L_TIME_SLOT_SCPE") ? base.LovData.First(f => f.Name == "L_TIME_SLOT_SCPE") : null;

        //    if (statusPay == "")
        //    {
        //        var stringViewBag = "";
        //        //R20.8
        //        if (TempData["statusPay"] != null)
        //        {
        //            ViewBag.BackFromPaymentMsg = TempData["statusPay"];

        //            stringViewBag = TempData["statusPay"].ToSafeString();

        //            TempData["statusPay"] = null;
        //        }
        //        else if (System.Web.HttpContext.Current.Session["statusPayScpe"] != null)
        //        {
        //            ViewBag.BackFromPaymentMsg = System.Web.HttpContext.Current.Session["statusPayScpe"].ToSafeString();

        //            stringViewBag = System.Web.HttpContext.Current.Session["statusPayScpe"].ToSafeString();

        //            System.Web.HttpContext.Current.Session["statusPayScpe"] = null;
        //        }

        //        if (!string.IsNullOrEmpty(stringViewBag))
        //        {
        //            Logger.Info("ViewBag.BackFromPaymentMsg V002 : " + stringViewBag);
        //        }
        //        else
        //        {
        //            Logger.Info("ViewBag.BackFromPaymentMsg V002 is Null");
        //        }

        //        return View();
        //    }
        //    else if (statusPay == "ErrorSPDP")
        //    {
        //        ViewBag.BackFromPaymentMsgErrorSPDP = statusPay;
        //        return View();
        //    }
        //    else
        //    {
        //        ViewBag.BackFromPaymentMsg = statusPay;
        //        return View();
        //    }
        //}
        #endregion Inactive SCPE 10/07/2024 #Comment Index

        public ActionResult Test()
        {

            return View();
        }

        [HttpPost]
        public ActionResult Index(QuickWinPanelModel model, HttpPostedFileBase[] files)
        {

            #region Get IP Address (Update 17.2)

            string ClientIP = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ClientIP))
            {
                ClientIP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }

            model.ClientIP = ClientIP;

            #endregion

            string pdfSignatureBase64 = model.SignaturePDF;
            model.SignaturePDF = "";

            string pdfSignatureBase64_2 = model.SignaturePDF2;
            model.SignaturePDF2 = "";

            var customerRowID = "";

            bool checkPayment = true;
            TempData["SaveStatus"] = "D";

            if (
                    model.CustomerRegisterPanelModel.Plug_and_play_flag == "3"
                    && model.CustomerRegisterPanelModel.L_EVENT_CODE == "EVENT_BYOD"
                )
            {
                var SHOW_PAYMENT_PAGE = base.LovData.Any(f => f.Name == "SHOW_PAYMENT_PAGE") ? base.LovData.First(f => f.Name == "SHOW_PAYMENT_PAGE").LovValue1 : "";
                if (SHOW_PAYMENT_PAGE == "Y" && (model.PayMentOrderID == null || model.PayMentOrderID == ""))
                {
                    checkPayment = false;
                }
            }

            if (checkPayment ||
                 (model.CustomerRegisterPanelModel.CPE_Info != null
                 && model.CustomerRegisterPanelModel.CPE_Info.Count > 0
                 && model.CustomerRegisterPanelModel.CPE_Info[0].SN != null
                 && model.CustomerRegisterPanelModel.CPE_Info[0].SN != "")
                )
            {

                try
                {

                    #region Log Model

                    var log = "";
                    try
                    {
                        log = "model => CoveragePanelModel \r\n";
                        foreach (PropertyInfo propertyInfo in model.CoveragePanelModel.GetType().GetProperties())
                        {
                            if (propertyInfo.Name.ToString() != "ModelState")
                            {
                                log += propertyInfo.Name.ToString();
                                if (propertyInfo.Name.ToString() == "Address")
                                {
                                    foreach (PropertyInfo propertyAddress in model.CoveragePanelModel.Address.GetType().GetProperties())
                                    {
                                        if (propertyAddress.Name.ToString() != "ModelState")
                                        {
                                            log += propertyAddress.Name.ToString();
                                            log += " : " + GetModelValue(model.CoveragePanelModel.Address, propertyAddress.Name.ToString());
                                            log += " \r\n ";
                                        }
                                    }
                                }
                                else
                                {
                                    log += " : " + GetModelValue(model.CoveragePanelModel, propertyInfo.Name.ToString());
                                    log += " \r\n ";
                                }
                            }
                        }

                        log += "model => DisplayPackagePanelModel \r\n";
                        foreach (PropertyInfo propertyInfo in model.DisplayPackagePanelModel.GetType().GetProperties())
                        {
                            if (propertyInfo.Name.ToString() != "ModelState")
                            {
                                log += propertyInfo.Name.ToString();
                                if (propertyInfo.Name.Equals(propertyInfo.Name.ToString()))
                                {
                                    log += " : " + GetModelValue(model.DisplayPackagePanelModel, propertyInfo.Name.ToString());
                                    log += " \r\n ";
                                }
                            }
                        }

                        log += "model => CustomerRegisterPanelModel \r\n";
                        foreach (PropertyInfo propertyInfo in model.CustomerRegisterPanelModel.GetType().GetProperties())
                        {
                            if (propertyInfo.Name.ToString() != "ModelState")
                            {
                                log += propertyInfo.Name.ToString();
                                if (propertyInfo.Name.ToString() == "AddressPanelModelSendDoc")
                                {
                                    foreach (PropertyInfo propertyAddress in model.CustomerRegisterPanelModel.AddressPanelModelSendDoc.GetType().GetProperties())
                                    {
                                        if (propertyAddress.Name.ToString() != "ModelState")
                                        {
                                            log += propertyAddress.Name.ToString();
                                            log += " : " + GetModelValue(model.CustomerRegisterPanelModel.AddressPanelModelSendDoc, propertyAddress.Name.ToString());
                                            log += " \r\n ";
                                        }
                                    }
                                }
                                if (propertyInfo.Name.ToString() == "AddressPanelModelSetup")
                                {
                                    foreach (PropertyInfo propertyAddress in model.CustomerRegisterPanelModel.AddressPanelModelSetup.GetType().GetProperties())
                                    {
                                        if (propertyAddress.Name.ToString() != "ModelState")
                                        {
                                            log += propertyAddress.Name.ToString();
                                            log += " : " + GetModelValue(model.CustomerRegisterPanelModel.AddressPanelModelSetup, propertyAddress.Name.ToString());
                                            log += " \r\n ";
                                        }
                                    }
                                }
                                if (propertyInfo.Name.ToString() == "AddressPanelModelVat")
                                {
                                    foreach (PropertyInfo propertyAddress in model.CustomerRegisterPanelModel.AddressPanelModelVat.GetType().GetProperties())
                                    {
                                        if (propertyAddress.Name.ToString() != "ModelState")
                                        {
                                            log += propertyAddress.Name.ToString();
                                            log += " : " + GetModelValue(model.CustomerRegisterPanelModel.AddressPanelModelVat, propertyAddress.Name.ToString());
                                            log += " \r\n ";
                                        }
                                    }
                                }
                                else
                                {
                                    log += " : " + GetModelValue(model.CoveragePanelModel, propertyInfo.Name.ToString());
                                    log += " \r\n ";
                                }
                            }
                        }

                        log += "model => SummaryPanelModel \r\n";
                        foreach (PropertyInfo propertyInfo in model.SummaryPanelModel.GetType().GetProperties())
                        {
                            if (propertyInfo.Name.ToString() != "ModelState")
                            {
                                log += propertyInfo.Name.ToString();
                                if (propertyInfo.Name.ToString() == "PackageModel")
                                {
                                    foreach (PropertyInfo propertyAddress in model.SummaryPanelModel.PackageModel.GetType().GetProperties())
                                    {
                                        if (propertyAddress.Name.ToString() != "ModelState")
                                        {
                                            if (propertyAddress.Name.ToString() != "RECURRING_CHARGE")
                                            {
                                                log += propertyAddress.Name.ToString();
                                                log += " : " + GetModelValue(model.SummaryPanelModel.PackageModel, propertyAddress.Name.ToString());
                                                log += " \r\n ";
                                            }
                                        }
                                    }
                                }
                                else if ((propertyInfo.Name.ToString() != "L_SEND_EMAIL") && (propertyInfo.Name.ToString() != "L_SAVE"))
                                {
                                    log += " : " + GetModelValue(model.SummaryPanelModel, propertyInfo.Name.ToString());
                                    log += " \r\n ";
                                }
                            }
                        }

                        log += "model SiteCode : " + model.SiteCode + "\r\n";
                        log += "model FlowFlag : " + model.FlowFlag + "\r\n";
                        log += "model TransactionID : " + model.TransactionID + "\r\n";
                    }
                    catch (Exception ex)
                    {
                        Logger.Info(ex.GetErrorMessage());
                    }

                    Logger.Info(log);

                    #endregion Log Model

                    #region Check zipcoderowid

                    if (model.CustomerRegisterPanelModel.AddressPanelModelSetup.ZIPCODE_ID.ToSafeString() == "")
                    {
                        Logger.Info("CustomerRegisterPanelModel.AddressPanelModelSetup.ZIPCODE_ID : is null\r\n");
                        Logger.Info("CustomerRegisterPanelModel.AddressPanelModelSetup.ZIPCODE_ID :" + model.CustomerRegisterPanelModel.AddressPanelModelSetup.ZIPCODE_ID.ToSafeString() + "\r\n");
                        Logger.Info("model.CoveragePanelModel.Address.ZIPCODE_ID :" + model.CoveragePanelModel.Address.ZIPCODE_ID.ToSafeString() + "\r\n");
                        model.CustomerRegisterPanelModel.AddressPanelModelSetup.ZIPCODE_ID = model.CoveragePanelModel.Address.ZIPCODE_ID.ToSafeString();
                        Logger.Info("AddressPanelModelSetup.ZIPCODE_ID after recieve from CoveragePanelModel  :" + model.CustomerRegisterPanelModel.AddressPanelModelSetup.ZIPCODE_ID);
                    }

                    #endregion Check zipcoderowid

                    List<string> SplitterFlags = new List<string>();
                    SplitterFlags.Add("2");
                    SplitterFlags.Add("3");
                    SplitterFlags.Add("5");
                    if (SplitterFlags.IndexOf(model.SplitterFlag) != -1)
                    {
                        model.CustomerRegisterPanelModel.FBSSTimeSlot.AppointmentDate = null;
                        model.CustomerRegisterPanelModel.FBSSTimeSlot.InstallationCapacity = "";
                        model.CustomerRegisterPanelModel.FBSSTimeSlot.TimeSlot = "";
                        model.CustomerRegisterPanelModel.FBSSTimeSlot.TimeSlotId = "";
                        model.CustomerRegisterPanelModel.L_INSTALL_DATE = "";
                        model.CustomerRegisterPanelModel.FBSSTimeSlot.FirstInstallDate = "";
                        model.CustomerRegisterPanelModel.FBSSTimeSlot.FirstTimeSlot = "";
                    }

                    if (SiteSession.CurrentUICulture.IsThaiCulture())
                    {
                        if (model.CustomerRegisterPanelModel.L_INSTALL_DATE.ToSafeString() != "")
                        {
                            var value = model.CustomerRegisterPanelModel.L_INSTALL_DATE.ToSafeString().Split('/');
                            if (value.Count() != 0)
                            {
                                value[2] = (Convert.ToInt32(value[2]) * 1 + 543).ToSafeString();
                                model.CustomerRegisterPanelModel.L_INSTALL_DATE = value[0] + "/" + value[1] + "/" + value[2];
                            }
                        }
                    }
                    if (model.CustomerRegisterPanelModel.AddressPanelModelSendDoc.L_BUILD_NAME != "")
                        model.CustomerRegisterPanelModel.AddressPanelModelSendDoc.L_BUILD_NAME = model.CustomerRegisterPanelModel.AddressPanelModelSendDoc.L_BUILD_NAME + " " + model.CustomerRegisterPanelModel.AddressPanelModelSendDoc.L_BUILD_NO_Hied;
                    if (model.CustomerRegisterPanelModel.AddressPanelModelSendDocIDCard.L_BUILD_NAME != "")
                        model.CustomerRegisterPanelModel.AddressPanelModelSendDocIDCard.L_BUILD_NAME = model.CustomerRegisterPanelModel.AddressPanelModelSendDocIDCard.L_BUILD_NAME + " " + model.CustomerRegisterPanelModel.AddressPanelModelSendDocIDCard.L_BUILD_NO_Hied;
                    if (model.CustomerRegisterPanelModel.AddressPanelModelSetup.L_BUILD_NAME != "")
                        model.CustomerRegisterPanelModel.AddressPanelModelSetup.L_BUILD_NAME = model.CustomerRegisterPanelModel.AddressPanelModelSetup.L_BUILD_NAME + " " + model.CustomerRegisterPanelModel.AddressPanelModelSetup.L_BUILD_NO_Hied;
                    if (model.CustomerRegisterPanelModel.AddressPanelModelVat.L_BUILD_NAME != "")
                        model.CustomerRegisterPanelModel.AddressPanelModelVat.L_BUILD_NAME = model.CustomerRegisterPanelModel.AddressPanelModelVat.L_BUILD_NAME + " " + model.CustomerRegisterPanelModel.AddressPanelModelVat.L_BUILD_NO_Hied;




                    var saveOrderResp = new SaveOrderResp();

                    model.CustomerRegisterPanelModel.RegisterChannelSaveOrder = "FBBWF";
                    model.CustomerRegisterPanelModel.AutoCreateProspectFlag = "Y";
                    model.CustomerRegisterPanelModel.OrderVerify = "";

                    saveOrderResp = GetSaveOrderResp(model);
                    if (saveOrderResp.RETURN_CODE != 0)
                    {
                        TempData["SaveStatus"] = "N";
                        Logger.Info(saveOrderResp.RETURN_MESSAGE);
                    }
                    else
                    {
                        TempData["SaveStatus"] = "Y";
                    }

                    #region set value

                    if (string.IsNullOrEmpty(model.CoveragePanelModel.Address.L_MOOBAN))
                    { model.CoveragePanelModel.Address.L_MOOBAN = "-"; }
                    if (string.IsNullOrEmpty(model.CoveragePanelModel.L_FLOOR_VILLAGE))
                    { model.CoveragePanelModel.L_FLOOR_VILLAGE = "1"; }

                    #endregion set value

                    #region Save register

                    // register customer
                    customerRowID = RegisterCustomer(model,
                        saveOrderResp.RETURN_CODE.ToSafeString(),
                        saveOrderResp.RETURN_MESSAGE,
                        saveOrderResp.RETURN_ORDER_NO,
                        ClientIP);

                    model.CustomerRegisterPanelModel.OrderNo = saveOrderResp.RETURN_ORDER_NO;


                    #endregion Save register

                    #region PDF

                    var running_no = InsertMailLog(customerRowID);
                    string uploadFileWebPath = Configurations.UploadFilePath;
                    string uploadFileAppPath = Configurations.UploadFileTempPath;

                    model.CustomerRegisterPanelModel.L_INSTALL_DATE = model.CustomerRegisterPanelModel.L_INSTALL_DATE + "  " + model.CustomerRegisterPanelModel.FBSSTimeSlot.TimeSlot;

                    System.IFormatProvider format = new System.Globalization.CultureInfo("en-US");
                    string filename = "Request" + DateTime.Now.ToString("ddMMyy", format) + "_" + running_no.ToSafeString();
                    string directoryPath = "";
                    string directoryPathApp = "";

                    var AISsubtype = LovData.FirstOrDefault(
                    item => item.Name == "AIS_SHOP_SUB_TYPE" && item.LovValue5 == "FBBOR004").LovValue1;

                    Logger.Info("PDFLAST");
                    directoryPath = GeneratePDF_HTML(model, @uploadFileWebPath, @uploadFileAppPath, filename);


                    Session["FILENAME"] = filename;

                    var langPDFAPP = "";
                    if (SiteSession.CurrentUICulture.IsThaiCulture())
                        langPDFAPP = "T";
                    else
                        langPDFAPP = "E";
                    @directoryPathApp = GeneratePDFApp(model.CustomerRegisterPanelModel.L_CARD_NO, model.CustomerRegisterPanelModel.OrderNo, langPDFAPP, model.CoveragePanelModel.L_CONTACT_PHONE);

                    #endregion PDF

                    #region SendEmail

                    if (model.SummaryPanelModel.L_SEND_EMAIL)
                    {
                        if (!string.IsNullOrEmpty(model.CustomerRegisterPanelModel.L_EMAIL))
                        {
                            if (model.CustomerRegisterPanelModel.ReceiveEmailFlag == "Y")
                            {
                                SendEmail(customerRowID, running_no, model.CustomerRegisterPanelModel.L_EMAIL, "", @directoryPathApp);
                            }
                        }
                    }

                    #endregion SendEmail

                    #region SendSMS

                    if (checkPayment)
                    {
                        string mainCode = "";
                        if (model.SummaryPanelModel.PackageModelList.Count > 0)
                        {
                            for (int i = 0; i <= model.SummaryPanelModel.PackageModelList.Count - 1; i++)
                            {
                                if (!string.IsNullOrEmpty(model.SummaryPanelModel.PackageModelList[i].SFF_PROMOTION_CODE) && model.SummaryPanelModel.PackageModelList[i].PACKAGE_CLASS == "Main")
                                {
                                    mainCode += "|" + model.SummaryPanelModel.PackageModelList[i].SFF_PROMOTION_CODE;
                                }
                            }
                        }

                        SendSMS(model.CustomerRegisterPanelModel.L_MOBILE, mainCode);
                    }

                    #endregion SendSMS

                }
                catch (Exception ex)
                {
                    Logger.Info(ex.GetErrorMessage());
                }

            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult SaveCoverageResult(QuickWinPanelModel model)
        {
            string CheckXDSL = model.CheckXDSL.ToSafeString();
            model.CustomerRegisterPanelModel.CateType = "R";
            model.CustomerRegisterPanelModel.SubCateType = "T";
            model.SummaryPanelModel.PackageModel.PACKAGE_CODE = "Default";
            model.SummaryPanelModel.PackageModel.PACKAGE_CODE_ONTOP = "";
            model.CustomerRegisterPanelModel.L_CARD_TYPE = "บัตรประชาชน";
            model.CustomerRegisterPanelModel.L_GENDER = "Female";
            model.CustomerRegisterPanelModel.L_BIRTHDAY = "01/01/2542";
            model.CustomerRegisterPanelModel.L_SPECIFIC_TIME = "08.00 - 19.00 น.";
            model.CustomerRegisterPanelModel.L_NATIONALITY = "THAI";
            model.DisplayPackagePanelModel.WIFIAccessPoint = "N";
            model.CoveragePanelModel.L_RESULT = "N";

            model.CustomerRegisterPanelModel.AddressPanelModelSendDoc = model.CoveragePanelModel.Address;
            model.CustomerRegisterPanelModel.AddressPanelModelSetup = model.CoveragePanelModel.Address;
            model.CustomerRegisterPanelModel.AddressPanelModelSetup.L_HOME_NUMBER_2 = model.CoveragePanelModel.Address.L_HOME_NUMBER_1;

            model.CoveragePanelModel.L_FLOOR_VILLAGE = "";

            var saveOrderResp = GetSaveOrderResp(model);
            TempData["CoverageResultSaveStatus"] = "Y";
            if (saveOrderResp.RETURN_CODE != 0)
            {
                TempData["CoverageResultSaveStatus"] = "N";
                Logger.Info(saveOrderResp.RETURN_MESSAGE);
            }

            #region Update Coverage Result

            var a = Bootstrapper.GetInstance<CheckCoverageController>();
            a.FBSSCoverageResultCommand(

                actionType: "Update",
                resultId: Decimal.Parse(model.CoveragePanelModel.RESULT_ID.ToSafeString()),
                preName: model.CoveragePanelModel.L_FIRST_LAST.ToSafeString(),
                fName: model.CoveragePanelModel.L_FIRST_NAME.ToSafeString(),
                lName: model.CoveragePanelModel.L_LAST_NAME.ToSafeString(),
                contactNo: model.CoveragePanelModel.L_CONTACT_PHONE.ToSafeString(),
                recode: saveOrderResp.RETURN_CODE,
                remessage: saveOrderResp.RETURN_MESSAGE.ToSafeString(),
                reorder: saveOrderResp.RETURN_ORDER_NO.ToSafeString(),
                email: model.CoveragePanelModel.L_CONTACT_EMAIL.ToSafeString(),
                lineid: model.CoveragePanelModel.L_CONTACT_LINE_ID.ToSafeString(),
                addrId: CheckXDSL
            );

            #endregion Update Coverage Result

            return RedirectToAction("Index");
        }

        private string GetModelValue(object model, string propertyName)
        {
            foreach (PropertyInfo propertyInfo in model.GetType().GetProperties())
            {
                if (propertyInfo.Name.Equals(propertyName))
                {
                    var value_ = propertyInfo.GetValue(model, null);
                    return value_.ToSafeString();
                }
            }

            return string.Empty;
        }

        private void SendEmail(string custRowId, decimal runningNo, string mailTo, string filePath, string filePathApp)
        {
            try
            {
                Logger.Info("SCPE SendEmail :" + custRowId);

                var ImpersonateVar = base.LovData.Where(l => l.Type == "FBB_CONSTANT" && l.Name == "Impersonate_App").SingleOrDefault();
                var imagepathimer = @ImpersonateVar.LovValue4;
                string user = ImpersonateVar.LovValue1;
                string pass = ImpersonateVar.LovValue2;
                string ip = ImpersonateVar.LovValue3;

                string filePathAppNASTemp = "";
                if (filePathApp != "")
                {
                    filePathAppNASTemp = filePathApp.Substring(2);
                }

                string filePathAppNAS = "\\\\" + ip + filePathAppNASTemp.Replace(filePathAppNASTemp.Split('\\')[0], "");


                Logger.Info("SCPE SendEmail PathNAS :" + filePathAppNAS);

                var command = new NotificationCommand
                {
                    CustomerId = custRowId,
                    CurrentCulture = SiteSession.CurrentUICulture,
                    RunningNo = runningNo,
                    EmailModel = new EmailModel
                    {
                        MailTo = mailTo,
                        FilePath = filePath,
                        FilePath2 = filePathAppNAS,
                    },
                    ImpersonateUser = user,
                    ImpersonatePass = pass,
                    ImpersonateIP = ip
                };

                SendEmailHandler(command);
            }
            catch (Exception ex)
            {
                Logger.Info("SCPE SendEmail Error :" + ex.GetBaseException());

                throw;
            }

        }

        public void SendEmailHandler(NotificationCommand command)
        {

            _noticeCommand.Handle(command);
        }

        public string GetPostalCode(string outPostalCode, string outTumbol, string outAmphur, string outProvince)
        {
            var query = new GetoutPostalCodeQuery
            {
                outPostalCode = outPostalCode,
                outTumbol = outTumbol,
                outAmphur = outAmphur,
                outProvince = outProvince
            };

            var data = _queryProcessor.Execute(query);
            return data.ToSafeString();
        }

        private SaveOrderResp GetSaveOrderResp(QuickWinPanelModel model)
        {
            string FullUrl = "";
            if (Session["FullUrl"] != null)
                FullUrl = Session["FullUrl"].ToSafeString();
            model.CoveragePanelModel.P_MOBILE = "|";
            var query = new GetSaveOrderRespQuery
            {
                CurrentCulture = SiteSession.CurrentUICulture,
                QuickWinPanelModel = model,
                FullUrl = FullUrl
            };
            var data = _queryProcessor.Execute(query);
            return data;
        }

        private string RegisterCustomer(QuickWinPanelModel model, string interfaceCode,
            string interfaceDesc, string interfaceOrder, string ClientIP)
        {
            var coverageResultId = "";
            if (null != model.CoveragePanelModel)
                coverageResultId = model.CoveragePanelModel.RESULT_ID;

            var command = new CustRegisterCommand
            {
                QuickWinPanelModel = model,
                CurrentCulture = model.languageCulture,
                InterfaceCode = interfaceCode,
                InterfaceDesc = interfaceDesc,
                InterfaceOrder = interfaceOrder,
                CoverageResultId = coverageResultId.ToSafeDecimal(),
                ClientIP = ClientIP
            };

            _custRegCommand.Handle(command);

            return command.CustomerId;
        }

        private decimal InsertMailLog(string customerId)
        {
            var command = new MailLogCommand
            {
                CustomerId = customerId,
            };

            _mailLogCommand.Handle(command);

            return command.RunningNo;
        }

        public string GeneratePDF_HTML(QuickWinPanelModel model, string directoryPath,
           string directoryTempPath, string fileName)
        {
            InterfaceLogCommand log = null;
            string orderNo = model.CustomerRegisterPanelModel.OrderNo;
            string CardNo = model.CustomerRegisterPanelModel.L_CARD_NO;
            string Language = "";
            if (SiteSession.CurrentUICulture.IsThaiCulture())
                Language = "T";
            else
                Language = "E";

            try
            {

                log = StartInterface("", "Generate PDF", model.ClientIP, CardNo, "WEB");

                var html = "<?xml version=\"1.0\" encoding=\"utf-8\"?>";
                string[] htmlPage = new string[2];

                Byte[] bytes;

                PDFDataQuery query = new PDFDataQuery();
                query.orderNo = orderNo;
                query.Language = Language;
                query.isEApp = false;
                query.isShop = "N";

                query.pageNo = 1;
                var htmlFromPackage = QueryGeneratePDF(query);
                htmlPage[0] = html + htmlFromPackage;

                query.pageNo = 2;
                htmlFromPackage = QueryGeneratePDF(query);
                htmlPage[1] = html + htmlFromPackage;

                bytes = htmlToPDF(htmlPage);

                bytes = PdfSecurity.SetPasswordPdf(bytes, CardNo);

                Session["PFDBYTE"] = bytes;

                log = StartInterface("directoryPath:" + directoryPath + "\r\n" + "fileName:" + fileName, "Generate PDF", model.ClientIP, model.CustomerRegisterPanelModel.L_CARD_NO, "WEB");

                //Write file to NAS
                var pathfile = directoryPath + "\\" + fileName + ".pdf";
                PdfSecurity.WriteFile(pathfile, bytes);

                EndInterface("", log, model.ClientIP, "Success", "");

                return directoryTempPath + "\\" + fileName + ".pdf";
            }
            catch (Exception ex)
            {
                EndInterface("", log, model.ClientIP, "ERROR", "ErrorMessage: " + ex.GetErrorMessage() + "\r\n" + "RenderExceptionMessage: " + ex.RenderExceptionMessage());
                return "";
            }
        }

        public string GeneratePDFApp(string CardNo, string orderNo, string Language, string contactNo)
        {
            InterfaceLogCommand log = null;

            try
            {

                log = StartInterface("", "Generate PDF APP", orderNo, CardNo, "WEB");

                var html = "<?xml version=\"1.0\" encoding=\"utf-8\"?>";

                Session["OrderNoAPP"] = orderNo;
                Session["LanguageAPP"] = Language;

                Byte[] bytes;

                PDFDataQuery query = new PDFDataQuery();
                query.orderNo = orderNo;
                query.Language = Language;
                query.isEApp = true;
                var htmlFromPackage = QueryGeneratePDF(query);

                html = html + htmlFromPackage;

                html = html.Replace("{Sign}", "");

                Logger.Info("HTML :" + html);

                bytes = htmlToPDF(html);

                bytes = PdfSecurity.SetPasswordPdf(bytes, CardNo);

                var queryName = new GetFormatFileNameEAPPQuery
                {
                    ID_CardNo = CardNo,
                };

                var result = _queryProcessor.Execute(queryName);

                string fileName = result.file_name;

                var ImpersonateVar = base.LovData.Where(l => l.Type == "FBB_CONSTANT" && l.Name == "Impersonate").SingleOrDefault();
                var imagepathimer = @ImpersonateVar.LovValue4;
                string user = ImpersonateVar.LovValue1;
                string pass = ImpersonateVar.LovValue2;
                string ip = ImpersonateVar.LovValue3;
                string yearweek = (DateTime.Now.Year.ToString());
                string monthyear = (DateTime.Now.Month.ToString("00"));

                var imagepathimerTemp = Path.Combine(imagepathimer, (yearweek + monthyear));


                imagepathimer = imagepathimerTemp;

                Logger.Info("CardNo: " + CardNo + "|| Start Impersonate:");

                using (var impersonator = new Impersonator(user, ip, pass, false))
                {
                    System.IO.Directory.CreateDirectory(imagepathimer);
                    var pathfileImpesontae = imagepathimer + "\\" + fileName + ".pdf";

                    try
                    {
                        Logger.Info("SCPE WriteFile : " + pathfileImpesontae);

                        PdfSecurity.WriteFile(pathfileImpesontae, bytes);

                    }
                    catch (Exception ex)
                    {
                        Logger.Info("SCPE WriteFile Error : " + ex.GetBaseException());

                        throw;
                    }

                    UpdateFileName(orderNo, pathfileImpesontae, contactNo);
                    Logger.Info("orderNo: " + orderNo + "|| pathfileImpesontae: " + pathfileImpesontae + "contactNo: " + contactNo);
                }

                EndInterface("", log, orderNo, "Success", "");

                return imagepathimer + "\\" + fileName + ".pdf"; ;
            }
            catch (Exception ex)
            {
                Logger.Info("ErrorMessage GeneratePDFApp : " + ex.GetBaseException());

                EndInterface("", log, orderNo, "ERROR", "ErrorMessage: " + ex.GetErrorMessage() + "\r\n" + "RenderExceptionMessage: " + ex.RenderExceptionMessage());
                return "";
            }
        }

        public string QueryGeneratePDF(PDFDataQuery query)
        {
            InterfaceLogCommand log = null;
            var pdfData = new PDFData();

            try
            {
                log = StartInterface<PDFDataQuery>(query, "QueryGeneratePDF",
                   query.orderNo, "", "");

                pdfData = _queryProcessor.Execute(query);

                EndInterface<PDFData>(pdfData, log, query.orderNo,
                        "Success", "");

                return pdfData.str_pdf_html;

            }
            catch (System.Exception ex)
            {

                EndInterface<PDFDataQuery>(query, log, query.orderNo,
                   ex.Message, ex.GetErrorMessage());

                return ex.Message.ToString();
            }
        }

        public void UpdateFileName(string orderNo, string filename, string AisAirNumber)
        {
            string FullUrl = "";
            if (Session["FullUrl"] != null)
                FullUrl = Session["FullUrl"].ToSafeString();

            #region Get IP Address Interface Log (Update 17.2)

            // Get IP Address
            string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress)) ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            string transactionId = ipAddress + "|";

            #endregion

            var commamd = new UpdateFileNameCommand
            {
                OrderNo = orderNo,
                FileName = filename,
                Transaction_Id = transactionId,
                FullUrl = FullUrl
            };
            _updateFileNameCommand.Handle(commamd);

        }

        public JsonResult CheckNewRegisProspect(string idCardNo, string firstName, string lastName, string locationCd, string ascCd, string TRANSACTION_ID = "")
        {
            string ClientIP = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ClientIP))
            {
                ClientIP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }

            if (TRANSACTION_ID == "|")
            {
                TRANSACTION_ID = ClientIP + "|";
            }

            var query = new CheckNewRegisProspectQuery()
            {
                idCardNo = idCardNo,
                locationCd = locationCd,
                ascCd = ascCd,
                TRANSACTION_ID = TRANSACTION_ID
            };

            CheckNewRegisProspectQueryModel result = _queryProcessor.Execute(query);

            if (result.firstName != null && result.lastName != null)
            {
                if (result.firstName != firstName && result.lastName != lastName)
                {
                    result.firstName = "N";
                    result.lastName = "N";
                }
                else
                {
                    result.firstName = "Y";
                    result.lastName = "Y";
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        struct MyObj
        {
            public string sN { get; set; }
            public string sellFlag { get; set; }
            public string errorMessage { get; set; }
        }

        struct retObj
        {
            public bool showpopup { get; set; }
            public bool isError { get; set; }
        }

        [HttpPost]
        public JsonResult queryStockTDM(string sN = "")
        {
            bool showpopup = false;
            bool isError = false;

            string ClientIP = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ClientIP))
            {
                ClientIP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }

            InterfaceLogCommand log = null;
            log = StartInterface("sN:" + sN + "\r\n", "queryStockTDM", ClientIP, "", "WEB");

            string URL = base.LovData.Where(l => l.Type == "SCREEN" && l.Name == "URL_queryStockTDM").FirstOrDefault().LovValue1;
            string serverCert = base.LovData.Where(l => l.Type == "SCREEN" && l.Name == "TDM_SERVER_CERT").FirstOrDefault().LovValue1;
            //string URL = "https://10.104.240.135:8543/TDMWSRestful/resteasy/stockReport/queryStockTDM";
            try
            {
                if (serverCert == "Y")
                    ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
                //ServicePointManager.ServerCertificateValidationCallback = TrustCertificate;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
                request.ContentType = "application/json";
                request.Method = "POST";

                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    string json = new JavaScriptSerializer().Serialize(new
                    {
                        sN = sN
                    });

                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();
                }


                var httpResponse = (HttpWebResponse)request.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var resultJson = streamReader.ReadToEnd();
                    var result = new JavaScriptSerializer().Deserialize<MyObj>(resultJson);

                    if (result.sellFlag.ToSafeString() == "Available")
                    {
                        showpopup = true;
                    }
                    else if (result.sellFlag.ToSafeString() == "")
                    {
                        showpopup = true;
                        isError = true;
                    }

                    EndInterface(resultJson, log, ClientIP, "Success", "");

                }

            }
            catch (Exception ex)
            {
                showpopup = true;
                isError = true;
                EndInterface("", log, ClientIP, "ERROR", "ErrorMessage: " + ex.GetErrorMessage() + "\r\n" + "RenderExceptionMessage: " + ex.RenderExceptionMessage());

            }

            retObj retData = new retObj();
            retData.showpopup = showpopup;
            retData.isError = isError;

            return Json(retData, JsonRequestBehavior.AllowGet);
        }

        //private static bool TrustCertificate(object sender, X509Certificate x509Certificate, X509Chain x509Chain, SslPolicyErrors sslPolicyErrors)
        //{
        //    // all Certificates are accepted
        //    return true;
        //}

        private InterfaceLogCommand StartInterface<T>(T query, string methodName, string transactionId, string idCardNo, string INTERFACE_NODE)
        {
            string FullUrl = "";
            string SERVICE_NAME = INTERFACE_NODE;
            if (Session["FullUrl"] != null)
            {
                FullUrl = Session["FullUrl"].ToSafeString();
                INTERFACE_NODE = INTERFACE_NODE + "|" + FullUrl;
            }

            var dbIntfCmd = new InterfaceLogCommand
            {
                ActionType = WBBContract.Commands.ActionType.Insert,
                IN_TRANSACTION_ID = transactionId + "|",
                METHOD_NAME = methodName,
                SERVICE_NAME = SERVICE_NAME,
                IN_ID_CARD_NO = idCardNo,
                IN_XML_PARAM = query.DumpToXml(),
                INTERFACE_NODE = INTERFACE_NODE,
                CREATED_BY = "FBBWEB",
            };

            _intfLogCommand.Handle(dbIntfCmd);

            return dbIntfCmd;
        }

        private void EndInterface<T>(T output, InterfaceLogCommand dbIntfCmd,
            string transactionId, string result, string reason)
        {
            if (null == dbIntfCmd)
                return;

            dbIntfCmd.ActionType = WBBContract.Commands.ActionType.Update;
            dbIntfCmd.REQUEST_STATUS = (result == "Success") ? "Success" : "Error";
            dbIntfCmd.OUT_RESULT = result;
            dbIntfCmd.OUT_ERROR_RESULT = (result == "Success") ? (reason.Length > 100 ? reason.Substring(0, 100) : result) : result;
            dbIntfCmd.OUT_XML_PARAM = (result == "Success") ? output.DumpToXml() : reason;

            _intfLogCommand.Handle(dbIntfCmd);
        }

        private Byte[] htmlToPDF(string html)
        {
            using (var ms = new System.IO.MemoryStream())
            {
                using (var doc = new Document())
                {

                    doc.SetMargins(doc.LeftMargin / 4, doc.RightMargin / 4, doc.TopMargin, doc.BottomMargin);

                    using (var writer = PdfWriter.GetInstance(doc, ms))
                    {

                        doc.Open();
                        //doc.NewPage();

                        using (var htmlWorker = new iTextSharp.text.html.simpleparser.HTMLWorker(doc))
                        {

                            using (var sr = new StringReader(html))
                            {

                                Logger.Info("Get Font");
                                //Path to our font
                                //string arialuniTff = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "ARIALUNI.TTF");
                                string fonttttts = Server.MapPath("~/Content/fonts/tahoma.ttf");
                                //Register the font with iTextSharp
                                iTextSharp.text.FontFactory.Register(fonttttts);
                                iTextSharp.text.FontFactory.GetFont(fonttttts, 6f, BaseColor.BLACK);

                                Logger.Info("StyleSheet");

                                //Create a new stylesheet
                                iTextSharp.text.html.simpleparser.StyleSheet ST = new iTextSharp.text.html.simpleparser.StyleSheet();
                                //Set the default body font to our registered font's internal name
                                ST.LoadTagStyle(HtmlTags.BODY, HtmlTags.FACE, "Tahoma");
                                //Set the default encoding to support Unicode characters
                                ST.LoadTagStyle(HtmlTags.BODY, HtmlTags.ENCODING, BaseFont.IDENTITY_H);
                                ST.LoadTagStyle(HtmlTags.BODY, HtmlTags.FONTSIZE, "4");

                                Logger.Info("Image Providers");

                                Dictionary<string, object> providers = new Dictionary<string, object>();

                                providers.Add(HTMLWorker.IMG_PROVIDER, new ImageThing(doc));

                                Logger.Info("HTML Parser to List");
                                ////Parse our HTML using the stylesheet created above
                                List<IElement> list = HTMLWorker.ParseToList(sr, ST, providers);
                                Logger.Info("Before create doc");
                                //htmlWorker.Parse(sr);
                                try
                                {
                                    foreach (var element in list)
                                    {
                                        Logger.Info("foreach A");
                                        doc.Add(element);
                                        Logger.Info("foreach B");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Logger.Info("Error ja: " + ex.Message + " ;;; " + ex.InnerException);
                                }

                                Logger.Info("Finished HTML Parse");
                            }
                        }

                        doc.Close();
                    }
                }
                return ms.ToArray();
            }

        }

        private Byte[] htmlToPDF(string[] html)
        {
            using (var ms = new System.IO.MemoryStream())
            {
                using (var doc = new Document())
                {

                    doc.SetMargins(doc.LeftMargin / 2, doc.RightMargin / 2, doc.TopMargin, doc.BottomMargin);

                    using (var writer = PdfWriter.GetInstance(doc, ms))
                    {

                        doc.Open();

                        using (var htmlWorker = new iTextSharp.text.html.simpleparser.HTMLWorker(doc))
                        {
                            for (int i = 0; i < html.Count(); i++)
                            {
                                using (var sr = new StringReader(html[i]))
                                {
                                    string fonttttts = Server.MapPath("~/Content/fonts/tahoma.ttf");
                                    iTextSharp.text.FontFactory.Register(fonttttts);
                                    iTextSharp.text.FontFactory.GetFont(fonttttts, 6f, BaseColor.BLACK);

                                    //Create a new stylesheet
                                    iTextSharp.text.html.simpleparser.StyleSheet ST = new iTextSharp.text.html.simpleparser.StyleSheet();
                                    ST.LoadTagStyle(HtmlTags.BODY, HtmlTags.FACE, "Tahoma");
                                    ST.LoadTagStyle(HtmlTags.BODY, HtmlTags.ENCODING, BaseFont.IDENTITY_H);
                                    ST.LoadTagStyle(HtmlTags.BODY, HtmlTags.FONTSIZE, "4");

                                    Logger.Info("Image Providers Page1");

                                    Dictionary<string, object> providers = new Dictionary<string, object>();

                                    providers.Add(HTMLWorker.IMG_PROVIDER, new ImageThing(doc));

                                    Logger.Info("HTML Parser to List Page1");
                                    List<IElement> list = HTMLWorker.ParseToList(sr, ST, providers);
                                    Logger.Info("Before create doc");
                                    try
                                    {
                                        foreach (var element in list)
                                        {
                                            Logger.Info("foreach A");
                                            doc.Add(element);
                                            Logger.Info("foreach B");
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.Info("Error ja: " + ex.Message + " ;;; " + ex.InnerException);
                                    }

                                    Logger.Info("Finished HTML Parse Page1");
                                }
                                if (i != html.Count() - 1)
                                    doc.NewPage();
                            }

                        }

                        doc.Close();
                    }
                }
                return ms.ToArray();
            }

        }

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

            //provType.Insert(0, new DropdownModel { Text = "กรุณาเลือก", Value = "" });
            return Json(provType, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCustomerTitle(string customerCardType)
        {
            var langFlg = (SiteSession.CurrentUICulture.IsThaiCulture() ?
                WebConstants.LovConfigName.TitleCodeTh : WebConstants.LovConfigName.TitleCodeEn);

            var query = new GetCustomerTitleQuery
            {
                CurrentCulture = SiteSession.CurrentUICulture,
                CustomerType = customerCardType,
            };

            var dropDown = _queryProcessor.Execute(query)
                .Select(t => new DropdownModel
                {
                    Text = t.Title,
                    Value = t.TitleCode,
                    DefaultValue = t.DefaultValue,
                }).ToList();

            return Json(dropDown, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetConditionLov(string lovType = "", string lovName = "")
        {
            if (string.IsNullOrEmpty(lovName))
            {
                var data = base.LovData
                .Where(l => l.Type.Equals(lovType))
                .OrderBy(o => o.OrderBy);

                return Json(data, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var data = base.LovData
                .Where(l => l.Type.Equals(lovType) && l.Name.Equals(lovName))
                .OrderBy(o => o.OrderBy);

                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetTermCon(string LovType)
        {
            var data = base.LovData
                .Where(l => l.Type.Equals(LovType));

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [Serializable()]
        public class response
        {
            public string saleId { get; set; }
            public string endPointUrl { get; set; }
            public string status { get; set; }
            public string respCode { get; set; }
            public string respDesc { get; set; }
            public string orderId { get; set; }
        }

        [HttpPost]
        public async Task<ActionResult> PaymentToMPayGateway(QuickWinPanelModel model)
        {
            #region Get IP Address Interface Log : Edit 2017-01-30

            string transactionId = "";

            // Get IP Address
            string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress)) ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            transactionId = ipAddress + "|";

            #endregion

            string tmpPurchaseAmt = "";
            decimal purchaseAmtDe = 0;
            if (decimal.TryParse(model.PayMentRecurringChargeVAT, out purchaseAmtDe))
            {
                tmpPurchaseAmt = purchaseAmtDe.ToString("F");
                tmpPurchaseAmt = tmpPurchaseAmt.Replace(".", "");
                tmpPurchaseAmt = tmpPurchaseAmt.Replace(",", "");
            }

            var LovConfigData = base.LovData.Where(t => t.Name == "RequestOrderTepsApi" && t.LovValue5 == "FBBOR028").ToList();

            string url = "";
            string projectCode = "";
            string command = "";
            string sid = "";
            string redirectUrl = "";
            string merchantId = "";
            string currency = "";
            string smsFlag = "";
            string orderExpire = "";
            string integrityStr = "";
            string SecretKey = "";
            string orderId = "";
            string reqRef5 = "64";

            orderId = model.PayMentOrderID;
            Session[orderId] = model;


            if (LovConfigData != null && LovConfigData.Count > 0)
            {
                url = LovConfigData.Where(t => t.Text == "url").Select(t => t.LovValue1).Count() > 0 ? LovConfigData.Where(t => t.Text == "url").Select(t => t.LovValue1).FirstOrDefault() : "";
                projectCode = LovConfigData.Where(t => t.Text == "projectCode").Select(t => t.LovValue1).Count() > 0 ? LovConfigData.Where(t => t.Text == "projectCode").Select(t => t.LovValue1).FirstOrDefault() : "";
                command = LovConfigData.Where(t => t.Text == "command").Select(t => t.LovValue1).Count() > 0 ? LovConfigData.Where(t => t.Text == "command").Select(t => t.LovValue1).FirstOrDefault() : "";
                sid = LovConfigData.Where(t => t.Text == "sid").Select(t => t.LovValue1).Count() > 0 ? LovConfigData.Where(t => t.Text == "sid").Select(t => t.LovValue1).FirstOrDefault() : "";
                redirectUrl = LovConfigData.Where(t => t.Text == "redirectUrl").Select(t => t.LovValue1).Count() > 0 ? LovConfigData.Where(t => t.Text == "redirectUrl").Select(t => t.LovValue1).FirstOrDefault() : "";
                merchantId = LovConfigData.Where(t => t.Text == "merchantId").Select(t => t.LovValue1).Count() > 0 ? LovConfigData.Where(t => t.Text == "merchantId").Select(t => t.LovValue1).FirstOrDefault() : "";
                currency = LovConfigData.Where(t => t.Text == "currency").Select(t => t.LovValue1).Count() > 0 ? LovConfigData.Where(t => t.Text == "currency").Select(t => t.LovValue1).FirstOrDefault() : "";
                smsFlag = LovConfigData.Where(t => t.Text == "smsFlag").Select(t => t.LovValue1).Count() > 0 ? LovConfigData.Where(t => t.Text == "smsFlag").Select(t => t.LovValue1).FirstOrDefault() : "";
                orderExpire = LovConfigData.Where(t => t.Text == "orderExpire").Select(t => t.LovValue1).Count() > 0 ? LovConfigData.Where(t => t.Text == "orderExpire").Select(t => t.LovValue1).FirstOrDefault() : "";
                SecretKey = LovConfigData.Where(t => t.Text == "SecretKEY").Select(t => t.LovValue1).Count() > 0 ? LovConfigData.Where(t => t.Text == "SecretKEY").Select(t => t.LovValue1).FirstOrDefault() : "";

                //cal integrityStr

                integrityStr = HashToHex(sid + merchantId + orderId + tmpPurchaseAmt + SecretKey);
            }
            var result = new response();
            try
            {
                SavePaymentLogModel savePaymentLogModel = new SavePaymentLogModel()
                {
                    ACTION = "New",
                    PROCESS_NAME = "RequestPayment",
                    PAYMENT_ORDER_ID = orderId,
                    ENDPOINT = url,
                    REQ_PROJECT_CODE = projectCode,
                    REQ_COMMAND = command,
                    REQ_SID = sid,
                    REQ_REDIRECT_URL = redirectUrl,
                    REQ_MERCHANT_ID = merchantId,
                    REQ_ORDER_ID = orderId,
                    REQ_PURCHASE_AMT = tmpPurchaseAmt,
                    REQ_CURRENCY = currency,
                    REQ_PAYMENT_METHOD = model.PayMentMethod,
                    REQ_SMS_FLAG = smsFlag,
                    REQ_ORDER_EXPIRE = orderExpire,
                    REQ_INTEGRITY_STR = integrityStr,
                    REQ_REF5 = reqRef5
                };
                SavePaymentLog(savePaymentLogModel);
                var contents = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("projectCode", projectCode),
                        new KeyValuePair<string, string>("command", command),
                        new KeyValuePair<string, string>("sid", sid),
                        new KeyValuePair<string, string>("redirectUrl", redirectUrl),
                        //new KeyValuePair<string, string>("redirectUrl", "http://localhost:50960/scpe/PaymentToMPayGatewayResult"), // ForDev
                        new KeyValuePair<string, string>("merchantId", merchantId),
                        new KeyValuePair<string, string>("orderId", orderId),
                        new KeyValuePair<string, string>("purchaseAmt", tmpPurchaseAmt),
                        new KeyValuePair<string, string>("currency", currency),
                        new KeyValuePair<string, string>("paymentMethod", model.PayMentMethod),
                        new KeyValuePair<string, string>("smsFlag", smsFlag),
                        new KeyValuePair<string, string>("orderExpire", orderExpire),
                        new KeyValuePair<string, string>("integrityStr", integrityStr),
                        new KeyValuePair<string, string>("ref5", reqRef5)
                    });

                using (var client = new HttpClient())
                {

                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await client.PostAsync(url, contents);

                    response.EnsureSuccessStatusCode();

                    using (HttpContent content = response.Content)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();

                        XmlSerializer serializer = new XmlSerializer(typeof(response), new XmlRootAttribute("response"));
                        StringReader stringReader = new StringReader(responseBody);
                        result = (response)serializer.Deserialize(stringReader);
                        if (result.endPointUrl != null && result.endPointUrl != "")
                        {
                            result.endPointUrl = HttpUtility.UrlDecode(result.endPointUrl);
                        }
                        else
                        {
                            result.endPointUrl = "";
                        }

                    }

                }

            }
            catch (Exception ex)
            {
                result.status = "F";
                result.endPointUrl = "";
            }

            SavePaymentLogModel savePaymentLogModelUpdate = new SavePaymentLogModel()
            {
                ACTION = "Modify",
                PROCESS_NAME = "RequestPayment",
                PAYMENT_ORDER_ID = orderId,
                ENDPOINT = url,

                RESP_SALE_ID = result.saleId,
                RESP_ENDPOINT_URL = result.endPointUrl,
                RESP_STATUS = result.status,
                RESP_RESP_CODE = result.respCode,
                RESP_RESP_DESC = result.respDesc
            };
            SavePaymentLog(savePaymentLogModelUpdate);

            if (result.endPointUrl != "")
            {
                return Redirect(result.endPointUrl);
            }
            else
            {
                Session[orderId] = null;
                var controller = DependencyResolver.Current.GetService<ProcessController>();
                var LovData = controller.GetScreenConfig("FBBOR028");
                string ErrorMsg = LovData.Where(t => t.Name == "L_POPUP_PAYMENT_ERROR").Select(t => t.DisplayValue).Count() > 0 ? LovData.Where(t => t.Name == "L_POPUP_PAYMENT_ERROR").Select(t => t.DisplayValue).FirstOrDefault() : "";
                return RedirectToAction("Index", new { statusPay = ErrorMsg });
            }
        }

        [HttpPost]
        public JsonResult GetPaymentToMerchantQrCode(string payMentOrderId, string payMentRecurringChargeVat)
        {
            GetCreateMerchantQrCodeModel result;
            try
            {
                #region Get IP Address Interface Log : Edit 2017-01-30

                var transactionId = "";

                // Get IP Address
                var ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (string.IsNullOrEmpty(ipAddress))
                    ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                transactionId = ipAddress + "|";

                #endregion

                //มาทำต่อหลัง Interface เรียบร้อย
                var tmpPurchaseAmt = "";
                decimal purchaseAmtDe = 0;
                if (decimal.TryParse(payMentRecurringChargeVat, out purchaseAmtDe))
                {
                    tmpPurchaseAmt = purchaseAmtDe.ToString("F");
                    tmpPurchaseAmt = tmpPurchaseAmt.Replace(",", "");
                }

                var channel = "";
                var serviceId = "";
                var terminalId = "";
                var locationName = "";
                var amount = tmpPurchaseAmt;
                var qrType = "";
                var ref1 = "";
                var ref2 = "";
                var ref3 = "";
                var ref4 = "";
                var ref5 = "";
                var appId = "";
                var appSecret = "";
                var urlFull = "";
                var url = "";
                var method = "";

                var orderId = payMentOrderId;

                var lovConfigDataQr = LovData.Where(t => t.Name == "RequestOrderQrcodeApi" && t.LovValue5 == "FBBOR028").ToList();
                if (lovConfigDataQr.Count > 0)
                {
                    channel = lovConfigDataQr.Where(t => t.Text == "channel").Select(t => t.LovValue1).Any()
                        ? lovConfigDataQr.Where(t => t.Text == "channel").Select(t => t.LovValue1).FirstOrDefault()
                        : "";

                    serviceId = lovConfigDataQr.Where(t => t.Text == "serviceId").Select(t => t.LovValue1).Any()
                        ? lovConfigDataQr.Where(t => t.Text == "serviceId").Select(t => t.LovValue1).FirstOrDefault()
                        : "";

                    terminalId = lovConfigDataQr.Where(t => t.Text == "terminalId").Select(t => t.LovValue1).Any()
                        ? lovConfigDataQr.Where(t => t.Text == "terminalId").Select(t => t.LovValue1).FirstOrDefault()
                        : "";

                    locationName = lovConfigDataQr.Where(t => t.Text == "locationName").Select(t => t.LovValue1).Any()
                        ? lovConfigDataQr.Where(t => t.Text == "locationName").Select(t => t.LovValue1).FirstOrDefault()
                        : "";

                    qrType = lovConfigDataQr.Where(t => t.Text == "qrType").Select(t => t.LovValue1).Any()
                        ? lovConfigDataQr.Where(t => t.Text == "qrType").Select(t => t.LovValue1).FirstOrDefault()
                        : "";

                    appId = lovConfigDataQr.Where(t => t.Text == "appId").Select(t => t.LovValue1).Any()
                        ? lovConfigDataQr.Where(t => t.Text == "appId").Select(t => t.LovValue1).FirstOrDefault()
                        : "";

                    appSecret = lovConfigDataQr.Where(t => t.Text == "appSecret").Select(t => t.LovValue1).Any()
                        ? lovConfigDataQr.Where(t => t.Text == "appSecret").Select(t => t.LovValue1).FirstOrDefault()
                        : "";

                    ref1 = lovConfigDataQr.Where(t => t.Text == "ref1").Select(t => t.LovValue1).Any()
                        ? lovConfigDataQr.Where(t => t.Text == "ref1").Select(t => t.LovValue1).FirstOrDefault()
                        : "";

                    ref2 = lovConfigDataQr.Where(t => t.Text == "ref2").Select(t => t.LovValue1).Any()
                        ? lovConfigDataQr.Where(t => t.Text == "ref2").Select(t => t.LovValue1).FirstOrDefault()
                        : "";

                    ref3 = lovConfigDataQr.Where(t => t.Text == "ref3").Select(t => t.LovValue1).Any()
                        ? lovConfigDataQr.Where(t => t.Text == "ref3").Select(t => t.LovValue1).FirstOrDefault()
                        : "";

                    ref4 = lovConfigDataQr.Where(t => t.Text == "ref4").Select(t => t.LovValue1).Any()
                        ? lovConfigDataQr.Where(t => t.Text == "ref4").Select(t => t.LovValue1).FirstOrDefault()
                        : "";

                    ref5 = lovConfigDataQr.Where(t => t.Text == "ref5").Select(t => t.LovValue1).Any()
                        ? lovConfigDataQr.Where(t => t.Text == "ref5").Select(t => t.LovValue1).FirstOrDefault()
                        : "";

                    url = lovConfigDataQr.Where(t => t.Text == "url_endpoint").Select(t => t.LovValue1).Any()
                        ? lovConfigDataQr.Where(t => t.Text == "url_endpoint")
                            .Select(t => t.LovValue1)
                            .FirstOrDefault()
                        : "";

                    method = lovConfigDataQr.Where(t => t.Text == "resource").Select(t => t.LovValue1).Any()
                        ? lovConfigDataQr.Where(t => t.Text == "resource").Select(t => t.LovValue1).FirstOrDefault()
                        : "";

                    urlFull = string.Format("{0}{1}", url, method);
                }

                var savePaymentLogModel = new SavePaymentLogModel
                {
                    ACTION = "New",
                    PROCESS_NAME = "qr-code-create", //RequestQrCode
                    PAYMENT_ORDER_ID = orderId,

                    ENDPOINT = url,
                    REQ_COMMAND = method,
                    REQ_APP_ID = appId,
                    REQ_APP_SECRET = appSecret,
                    REQ_CHANNEL = channel,
                    REQ_QR_TYPE = qrType,
                    REQ_TERMINAL_ID = terminalId,
                    REQ_SERVICE_ID = serviceId,
                    REQ_LOCATION_NAME = locationName,
                    REQ_TRAN_ID = transactionId,
                    REQ_REF1 = ref1,
                    REQ_REF2 = ref2,
                    REQ_REF3 = ref3,
                    REQ_REF4 = ref4,
                    REQ_REF5 = ref5
                };
                SavePaymentLog(savePaymentLogModel);

                var query = new GetCreateMerchantQrCodeQuery
                {
                    AppId = appId,
                    AppSecret = appSecret,
                    Url = url,
                    Method = method,
                    Body = new MerchantQrCodeBody
                    {
                        orderId = orderId,
                        channel = channel,
                        serviceId = serviceId,
                        terminalId = terminalId,
                        locationName = locationName,
                        amount = amount,
                        qrType = qrType,
                        ref1 = ref1,
                        ref2 = ref2,
                        ref3 = ref3,
                        ref4 = ref4,
                        ref5 = ref5
                    }
                };
                result = _queryProcessor.Execute(query);

                var savePaymentLogModelUpdate = new SavePaymentLogModel
                {
                    ACTION = "Modify",
                    PROCESS_NAME = "qr-code-create",
                    PAYMENT_ORDER_ID = string.IsNullOrEmpty(result.OrderId) ? orderId : result.OrderId,
                    ENDPOINT = url,

                    RESP_RESP_CODE = result.RespCode,
                    RESP_RESP_DESC = result.RespDesc,
                    RESP_QR_CODE_STR = result.QrCodeStr,
                    RESP_QR_FORMAT = result.QrFormat,
                    RESP_QR_CODE_VALIDITY = result.QrCodeValidity,
                    RESP_REFERENCE = result.Reference
                };
                SavePaymentLog(savePaymentLogModelUpdate);
            }
            catch (Exception)
            {
                result = new GetCreateMerchantQrCodeModel
                {
                    RespCode = "-1",
                    RespDesc = ""
                };
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult PaymentToMPayGatewayResult(string status, string respCode, string respDesc, string tranId, int saleId = 0, string orderId = "", string currency = "", int exchangeRate = 0)
        {
            string fullUrl = this.Url.Action("PaymentToMPayGatewayResult", "SCPE", null, this.Request.Url.Scheme);

            SavePaymentLogModel savePaymentLogModel = new SavePaymentLogModel()
            {
                ACTION = "New",
                PROCESS_NAME = "PostPaymentResult",
                PAYMENT_ORDER_ID = orderId,
                ENDPOINT = fullUrl,

                POST_STATUS = status,
                POST_RESP_CODE = respCode,
                POST_RESP_DESC = respDesc,
                POST_TRAN_ID = tranId,
                POST_SALE_ID = saleId.ToString(),
                POST_ORDER_ID = orderId,
                POST_CURRENCY = currency,
                POST_EXCHANGE_RATE = exchangeRate.ToString()
            };
            SavePaymentLog(savePaymentLogModel);

            QuickWinPanelModel model = new QuickWinPanelModel();
            if (Session[orderId] != null)
                model = (QuickWinPanelModel)Session[orderId];

            Session[orderId] = null;
            model.PayMentTranID = tranId.ToSafeString();
            if (status == "S" && respCode == "0000")
            {
                ActionResult action = Index(model, null);
                return action;
            }
            else
            {
                var controller = DependencyResolver.Current.GetService<ProcessController>();
                var LovData = controller.GetScreenConfig("FBBOR028");
                string ErrorMsg = LovData.Where(t => t.Name == "L_POPUP_PAYMENT_ERROR").Select(t => t.DisplayValue).Count() > 0 ? LovData.Where(t => t.Name == "L_POPUP_PAYMENT_ERROR").Select(t => t.DisplayValue).FirstOrDefault() : "";
                return RedirectToAction("Index", new { statusPay = ErrorMsg });
            }
        }

        [HttpPost]
        public ActionResult PayResult(string orderId, string tranDtm, string tranId, string serviceId, string terminalId,
            string locationName, string amount, string status, string sof, string qrType, string ref1, string ref2,
            string ref3, string ref4, string ref5)
        {
            bool result;
            try
            {
                Logger.Info(string.Format(
                    "START QR-CODE-PayResult : orderId = {0},tranDtm = {1},tranId = {2},serviceId = {3},terminalId = {4},locationName = {5},amount = {6},status = {7},sof = {8},qrType = {9},ref1 = {10},ref2 = {11},ref3 = {12},ref4 = {13},ref5 = {14}",
                    orderId, tranDtm, tranId, serviceId, terminalId, locationName, amount, status, sof, qrType, ref1,
                    ref2, ref3, ref4, ref5));

                var fullUrl = this.Url.Action("PayResult", "SCPE", null, Request.Url.Scheme);

                var savePaymentLogModel = new SavePaymentLogModel
                {
                    ACTION = "New",
                    PROCESS_NAME = "qr-code-notify",
                    PAYMENT_ORDER_ID = orderId,
                    ENDPOINT = fullUrl,

                    POST_ORDER_ID = orderId,
                    POST_TRAN_DTM = tranDtm,
                    POST_TRAN_ID = tranId,
                    POST_SERVICE_ID = serviceId,
                    POST_TERMINAL_ID = terminalId,
                    POST_LOCATION_NAME = locationName,
                    POST_AMOUNT = amount,
                    POST_STATUS = status,
                    POST_SOF = sof,
                    POST_QR_TYPE = qrType,
                    POST_REF1 = ref1,
                    POST_REF2 = ref2,
                    POST_REF3 = ref3,
                    POST_REF4 = ref4,
                    POST_REF5 = ref5,
                };
                SavePaymentLog(savePaymentLogModel);

                var hubContext = GlobalHost.ConnectionManager.GetHubContext<BaseHub>();
                hubContext.Clients.All.addNewMessageToPage(orderId, tranId);

                //if (status.ToSafeString().ToUpper() == "SUCCESS")
                //{
                //    var errorMsg = "";
                //    var msgSentTh = "";
                //    var msgSentEn = "";
                //    var msgSentData = LovData.FirstOrDefault(
                //        l => !string.IsNullOrEmpty(l.LovValue5)
                //             && !string.IsNullOrEmpty(l.Name)
                //             && l.LovValue5.Equals("FBBOR028")
                //             && l.Name.Equals("SMS_MESSAGE"));
                //    if (msgSentData != null)
                //    {
                //        msgSentTh = msgSentData.LovValue1;
                //        msgSentEn = msgSentData.LovValue2;
                //    }

                //    var user = "";
                //    var pass = "";
                //    var ip = "";
                //    var impersonate = "";
                //    var impersonateVar = LovData.FirstOrDefault(
                //        o => !string.IsNullOrEmpty(o.Type) && !string.IsNullOrEmpty(o.Name) &&
                //             o.Type.Equals("FBB_CONSTANT") && o.Name.Equals("Impersonate"));
                //    if (impersonateVar != null)
                //    {
                //        user = impersonateVar.LovValue1;
                //        pass = impersonateVar.LovValue2;
                //        ip = impersonateVar.LovValue3;
                //        impersonate = impersonateVar.LovValue4;
                //    }

                //    //Get List Create
                //    Logger.Info("Qr PayResult order_id :" + orderId);
                //    var dataOrderDetailCreate = QrcodeGetListOrderDetailCreate(orderId);
                //    if (dataOrderDetailCreate != null)
                //    {
                //        var engFlag = dataOrderDetailCreate.ODRDetailCustomerList[0].eng_flag;
                //        var isThai = (engFlag != "Y");

                //        //Fix paymentmethod if not return CheckPayment
                //        var resultQrcode = LovData.FirstOrDefault(l =>  l.LovValue5 == "FBBOR028" && l.Name == "RequestOrderQrcodeApi" && l.Text == "ref5");
                //        var paymentmethodQrCode = resultQrcode != null ? resultQrcode.LovValue1 : "147";

                //        dataOrderDetailCreate.ODRDetailCustomerList[0].paymentmethod = paymentmethodQrCode; 
                //        dataOrderDetailCreate.ODRDetailCustomerList[0].transactionid = tranId;
                //        Logger.Info("Qr PayResult order_id :" + orderId + " IsOK");

                //        //SaveOrder
                //        Logger.Info("GetSaveOrderResp Qr PayResult order_id :" + orderId);
                //        var saveOrderResp = QrcodeGetSaveOrderResp(dataOrderDetailCreate);
                //        bool checkSaveJob;
                //        if (saveOrderResp != null &&
                //            saveOrderResp.RETURN_IA_NO != null &&
                //            saveOrderResp.RETURN_IA_NO != "")
                //        {
                //            dataOrderDetailCreate.return_ia_no = saveOrderResp.RETURN_IA_NO;
                //            Logger.Info("Qr PayResult order_id :" + orderId + " IsOK");
                //            checkSaveJob = true;
                //        }
                //        else
                //        {
                //            dataOrderDetailCreate.return_ia_no = "";
                //            Logger.Info("Qr PayResult order_id :" + orderId + " IsOK");
                //            checkSaveJob = false;
                //            errorMsg = "error SaveOrder";
                //        }

                //        //register customer
                //        if (checkSaveJob)
                //        {
                //            Logger.Info("Qr PayResult RegisterCustomer order_id :" + orderId);
                //            var customerRowId = QrcodeRegisterCustomer(dataOrderDetailCreate);
                //            if (string.IsNullOrEmpty(customerRowId))
                //            {
                //                Logger.Info("Qr PayResult RegisterCustomer order_id :" + orderId + " IsOK");
                //                errorMsg = "error RegisterCustomer";
                //            }
                //            else
                //            {
                //                Logger.Info("Qr PayResult RegisterCustomer order_id :" + orderId + " IsOK");

                //                // SendEmail
                //                var emailAddress = "";
                //                if (!string.IsNullOrEmpty(dataOrderDetailCreate.ODRDetailCustomerList[0].email_address))
                //                {
                //                    Logger.Info("Qr PayResult Have SendEmail order_id :" + orderId);
                //                    emailAddress = dataOrderDetailCreate.ODRDetailCustomerList[0].email_address;
                //                    Logger.Info("Qr PayResult GenPDFAndSendEmail order_id :" + orderId);

                //                    var langPDFAPP = "";
                //                    if (isThai)
                //                        langPDFAPP = "T";
                //                    else
                //                        langPDFAPP = "E";

                //                    string directoryPathApp = "";
                //                    @directoryPathApp = GeneratePDFApp(dataOrderDetailCreate.ODRDetailCustomerList[0].id_card_no, dataOrderDetailCreate.return_ia_no, langPDFAPP, dataOrderDetailCreate.ODRDetailCustomerList[0].mobile_no);

                //                    var running_no = InsertMailLog(customerRowId);
                //                    SendEmail(customerRowId, running_no, emailAddress, "", @directoryPathApp);

                //                    //QrcodeGenPdfAndSendEmail(customerRowId,
                //                    //    dataOrderDetailCreate.ODRDetailCustomerList[0].id_card_no,
                //                    //    saveOrderResp.RETURN_IA_NO,
                //                    //    dataOrderDetailCreate.ODRDetailCustomerList[0].mobile_no, emailAddress, isThai, user,
                //                    //    pass, ip, impersonate);

                //                }


                //                // SendSMS
                //                var mainCode = "";
                //                if (dataOrderDetailCreate.ODRDetailPackageList.Count > 0)
                //                {
                //                    for (int i = 0; i <= dataOrderDetailCreate.ODRDetailPackageList.Count - 1; i++)
                //                    {
                //                        if (
                //                            !string.IsNullOrEmpty(
                //                                dataOrderDetailCreate.ODRDetailPackageList[i].sff_promotion_code) &&
                //                            dataOrderDetailCreate.ODRDetailPackageList[i].package_type == "Main")
                //                        {
                //                            mainCode += "|" +
                //                                        dataOrderDetailCreate.ODRDetailPackageList[i].sff_promotion_code;
                //                        }
                //                    }
                //                }
                //                var msgSent = isThai ? msgSentTh : msgSentEn;

                //                Logger.Info("Qr PayResult SendSMS order_id :" + orderId);
                //                QrcodeSendSms(dataOrderDetailCreate.ODRDetailCustomerList[0].mobile_no, mainCode, isThai,
                //                    msgSent);
                //            }
                //        }
                //    }
                //}

                //result = true;
            }
            catch (Exception ex)
            {
                Logger.Info(string.Format(
                    "QR-CODE-PayResult Error: orderId = {0},tranDtm = {1},tranId = {2},serviceId = {3},terminalId = {4},locationName = {5},amount = {6},status = {7},sof = {8},qrType = {9},ref1 = {10},ref2 = {11},ref3 = {12},ref4 = {13},ref5 = {14}, Error= {15}",
                    orderId, tranDtm, tranId, serviceId, terminalId, locationName, amount, status, sof, qrType, ref1,
                    ref2, ref3, ref4, ref5, ex.GetBaseException()));
            }
            finally
            {
                Logger.Info(string.Format(
                    "END QR-CODE-PayResult : orderId = {0},tranDtm = {1},tranId = {2},serviceId = {3},terminalId = {4},locationName = {5},amount = {6},status = {7},sof = {8},qrType = {9},ref1 = {10},ref2 = {11},ref3 = {12},ref4 = {13},ref5 = {14}",
                    orderId, tranDtm, tranId, serviceId, terminalId, locationName, amount, status, sof, qrType, ref1,
                    ref2, ref3, ref4, ref5));
            }

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        //[HttpPost]
        //public ActionResult PaymentToSuperDuper(QuickWinPanelModel model)
        //{
        //    var controller = DependencyResolver.Current.GetService<ProcessController>();
        //    GetPaymentToSuperDuperModel PaymentToSuperDuperData = controller.GetPaymentToSuperDuper(model.CoveragePanelModel.P_MOBILE.ToSafeString(), model.SuperDuperProductName.ToSafeString(), model.SuperDuperServiceName.ToSafeString(), model.PayMentOrderID.ToSafeString());

        //}

        [HttpPost]
        public string SavePreRegisterCustomer(QuickWinPanelModel model)
        {
            #region Get IP Address Interface Log : Edit 2017-01-30

            string ClientIP = "";

            // Get IP Address
            string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress)) ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            ClientIP = ipAddress;

            #endregion

            string orderId = "";

            //Gen orderId
            orderId = GetPaymentOrderID();
            model.PayMentOrderID = orderId;

            if (SiteSession.CurrentUICulture.IsThaiCulture())
            {
                if (model.CustomerRegisterPanelModel.L_INSTALL_DATE.ToSafeString() != "")
                {
                    var value = model.CustomerRegisterPanelModel.L_INSTALL_DATE.ToSafeString().Split('/');
                    if (value.Count() != 0)
                    {
                        value[2] = (Convert.ToInt32(value[2]) * 1 + 543).ToSafeString();
                        model.CustomerRegisterPanelModel.L_INSTALL_DATE = value[0] + "/" + value[1] + "/" + value[2];
                    }
                }
            }

            if (model.CustomerRegisterPanelModel.AddressPanelModelSendDoc.L_BUILD_NAME != "")
                model.CustomerRegisterPanelModel.AddressPanelModelSendDoc.L_BUILD_NAME = model.CustomerRegisterPanelModel.AddressPanelModelSendDoc.L_BUILD_NAME + " " + model.CustomerRegisterPanelModel.AddressPanelModelSendDoc.L_BUILD_NO_Hied;
            if (model.CustomerRegisterPanelModel.AddressPanelModelSendDocIDCard.L_BUILD_NAME != "")
                model.CustomerRegisterPanelModel.AddressPanelModelSendDocIDCard.L_BUILD_NAME = model.CustomerRegisterPanelModel.AddressPanelModelSendDocIDCard.L_BUILD_NAME + " " + model.CustomerRegisterPanelModel.AddressPanelModelSendDocIDCard.L_BUILD_NO_Hied;
            if (model.CustomerRegisterPanelModel.AddressPanelModelSetup.L_BUILD_NAME != "")
                model.CustomerRegisterPanelModel.AddressPanelModelSetup.L_BUILD_NAME = model.CustomerRegisterPanelModel.AddressPanelModelSetup.L_BUILD_NAME + " " + model.CustomerRegisterPanelModel.AddressPanelModelSetup.L_BUILD_NO_Hied;
            if (model.CustomerRegisterPanelModel.AddressPanelModelVat.L_BUILD_NAME != "")
                model.CustomerRegisterPanelModel.AddressPanelModelVat.L_BUILD_NAME = model.CustomerRegisterPanelModel.AddressPanelModelVat.L_BUILD_NAME + " " + model.CustomerRegisterPanelModel.AddressPanelModelVat.L_BUILD_NO_Hied;


            //save RegisterCustomer

            string customerRowID = RegisterCustomer(model, "", "", "", ClientIP);

            return orderId;
        }

        private string GetPaymentOrderID()
        {
            InterfaceLogCommand log = null;
            string data = "";
            try
            {
                log = StartInterface("", "GetPaymentOrderID", "", "", "WEB");
                GetPaymentOrderIDQuery query = new GetPaymentOrderIDQuery();
                data = _queryProcessor.Execute(query);
                EndInterface(data, log, "", "Success", "");
            }
            catch (Exception ex)
            {
                EndInterface(data, log, "", "ERROR", "ErrorMessage: " + ex.GetErrorMessage() + "\r\n" + "RenderExceptionMessage: " + ex.RenderExceptionMessage());
            }
            return data;
        }

        private void SavePaymentLog(SavePaymentLogModel model)
        {
            string FullUrl = "";
            if (Session["FullUrl"] != null)
                FullUrl = Session["FullUrl"].ToSafeString();

            #region Get IP Address Interface Log (Update 17.2)

            // Get IP Address
            string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress)) ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            string transactionId = ipAddress + "|";

            #endregion

            var commamd = new SavePaymentLogCommand
            {
                p_action = model.ACTION,
                p_payment_order_id = model.PAYMENT_ORDER_ID,
                p_process_name = model.PROCESS_NAME,
                p_endpoint = model.ENDPOINT,

                p_req_project_code = model.REQ_PROJECT_CODE,
                p_req_command = model.REQ_COMMAND,
                p_req_sid = model.REQ_SID,
                p_req_redirect_url = model.REQ_REDIRECT_URL,
                p_req_merchant_id = model.REQ_MERCHANT_ID,
                p_req_order_id = model.REQ_ORDER_ID,
                p_req_currency = model.REQ_CURRENCY,
                p_req_purchase_amt = model.REQ_PURCHASE_AMT,
                p_req_payment_method = model.REQ_PAYMENT_METHOD,
                p_req_product_desc = model.REQ_PRODUCT_DESC,
                p_req_ref1 = model.REQ_REF1,
                p_req_ref2 = model.REQ_REF2,
                p_req_ref3 = model.REQ_REF3,
                p_req_ref4 = model.REQ_REF4,
                p_req_ref5 = model.REQ_REF5,
                p_req_integrity_str = model.REQ_INTEGRITY_STR,
                p_req_sms_flag = model.REQ_SMS_FLAG,
                p_req_sms_mobile = model.REQ_SMS_MOBILE,
                p_req_mobile_no = model.REQ_MOBILE_NO,
                p_req_token_key = model.REQ_TOKEN_KEY,
                p_req_order_expire = model.REQ_ORDER_EXPIRE,

                p_resp_status = model.RESP_STATUS,
                p_resp_resp_code = model.RESP_RESP_CODE,
                p_resp_resp_desc = model.RESP_RESP_DESC,
                p_resp_sale_id = model.RESP_SALE_ID,
                p_resp_endpoint_url = model.RESP_ENDPOINT_URL,
                p_resp_detail1 = model.RESP_DETAIL1,
                p_resp_detail2 = model.RESP_DETAIL2,
                p_resp_detail3 = model.RESP_DETAIL3,

                p_post_status = model.POST_STATUS,
                p_post_resp_code = model.POST_RESP_CODE,
                p_post_resp_desc = model.POST_RESP_DESC,
                p_post_tran_id = model.POST_TRAN_ID,
                p_post_sale_id = model.POST_SALE_ID,
                p_post_order_id = model.POST_ORDER_ID,
                p_post_currency = model.POST_CURRENCY,
                p_post_exchange_rate = model.POST_EXCHANGE_RATE,
                p_post_purchase_amt = model.POST_PURCHASE_AMT,
                p_post_amount = model.POST_AMOUNT,
                p_post_inc_customer_fee = model.POST_INC_CUSTOMER_FEE,
                p_post_exc_customer_fee = model.POST_EXC_CUSTOMER_FEE,
                p_post_payment_status = model.POST_PAYMENT_STATUS,
                p_post_payment_code = model.POST_PAYMENT_CODE,
                p_post_order_expire_date = model.POST_ORDER_EXPIRE_DATE,

                //18.10 : QR Code
                p_req_app_id = model.REQ_APP_ID,
                p_req_app_secret = model.REQ_APP_SECRET,
                p_req_channel = model.REQ_CHANNEL,
                p_req_qr_type = model.REQ_QR_TYPE,
                p_req_terminal_id = model.REQ_TERMINAL_ID,
                p_req_service_id = model.REQ_SERVICE_ID,
                p_req_location_name = model.REQ_LOCATION_NAME,
                p_req_tran_id = model.REQ_TRAN_ID,

                p_resp_qr_format = model.RESP_QR_FORMAT,
                p_resp_qr_code_str = model.RESP_QR_CODE_STR,
                p_resp_qr_code_validity = model.RESP_QR_CODE_VALIDITY,
                p_resp_reference = model.RESP_REFERENCE,
                p_resp_tran_dtm = model.RESP_TRAN_DTM,
                p_resp_tran_id = model.RESP_TRAN_ID,
                p_resp_service_id = model.RESP_SERVICE_ID,
                p_resp_terminal_id = model.RESP_TERMINAL_ID,
                p_resp_location_name = model.RESP_LOCATION_NAME,
                p_resp_amount = model.RESP_AMOUNT,
                p_resp_sof = model.RESP_SOF,
                p_resp_qr_type = model.RESP_QR_TYPE,
                p_resp_refund_dt = model.RESP_REFUND_DT,
                p_resp_dispute_id = model.RESP_DISPUTE_ID,
                p_resp_dispute_status = model.RESP_DISPUT_STATUS,
                p_resp_dispute_reason_id = model.RESP_DISPUT_REASON_ID,
                p_resp_ref1 = model.RESP_REF1,
                p_resp_ref2 = model.RESP_REF2,
                p_resp_ref3 = model.RESP_REF3,
                p_resp_ref4 = model.RESP_REF4,
                p_resp_ref5 = model.RESP_REF5,

                p_post_tran_dtm = model.POST_TRAN_DTM,
                p_post_service_id = model.POST_SERVICE_ID,
                p_post_terminal_id = model.POST_TERMINAL_ID,
                p_post_location_name = model.POST_LOCATION_NAME,
                p_post_sof = model.POST_SOF,
                p_post_qr_type = model.POST_QR_TYPE,
                p_post_ref1 = model.POST_REF1,
                p_post_ref2 = model.POST_REF2,
                p_post_ref3 = model.POST_REF3,
                p_post_ref4 = model.POST_REF4,
                p_post_ref5 = model.POST_REF5,

                Transaction_Id = transactionId,
                FullUrl = FullUrl
            };
            _savePaymentLogCommand.Handle(commamd);
        }

        private Object thisLock = new Object();
        public void SendSMS(string mobileNo, string mainCode)
        {
            string FullUrl = "";
            if (Session["FullUrl"] != null)
                FullUrl = Session["FullUrl"].ToSafeString();

            #region Get IP Address Interface Log : Edit 2017-01-30

            string transactionId = "";

            // Get IP Address
            string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress)) ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            transactionId = ipAddress + "|";

            #endregion

            string[] strMainCode = null;
            if (!string.IsNullOrEmpty(mainCode))
            {
                strMainCode = mainCode.Split('|');
            }
            lock (thisLock)
            {
                string msgtxt1 = "";

                var data = base.LovData
                    .Where(l => !string.IsNullOrEmpty(l.LovValue5) && !string.IsNullOrEmpty(l.Name) && l.LovValue5.Equals("FBBOR028") && l.Name.Equals("SMS_MESSAGE"))
                    .FirstOrDefault();

                if (data != null)
                {
                    if (SiteSession.CurrentUICulture.IsThaiCulture())
                    {
                        msgtxt1 = data.LovValue1;
                    }
                    else
                    {
                        msgtxt1 = data.LovValue2;
                    }
                }

                if (mobileNo.Substring(0, 2) != "66")
                {
                    if (mobileNo.Substring(0, 1) == "0")
                    {
                        mobileNo = "66" + mobileNo.Substring(1);
                    }
                }

                List<string> promotionCode = new List<string>();
                if (strMainCode != null)
                {
                    foreach (string mCode in strMainCode)
                    {
                        if (!string.IsNullOrEmpty(mCode))
                        {
                            promotionCode.Add(mCode);
                        }

                    }
                }

                var air_old_list = new List<AIR_CHANGE_OLD_PACKAGE_ARRAY>();
                foreach (var code in promotionCode)
                {
                    if (!string.IsNullOrEmpty(code))
                    {
                        var air = new AIR_CHANGE_OLD_PACKAGE_ARRAY()
                        {
                            enddt = "",
                            productSeq = "",
                            sffPromotionCode = code,
                            startdt = ""
                        };
                        air_old_list.Add(air);
                    }
                }

                var query = new GetListShortNamePackageQuery()
                {
                    id_card_no = "",
                    airChangePromotionCode_List = air_old_list,
                    transaction_id = transactionId,
                    FullUrl = FullUrl
                };

                var packageMainName = "";
                var result = _queryProcessor.Execute(query);
                var command = new SendSmsCommand();
                command.FullUrl = FullUrl;
                command.Source_Addr = "AISFIBRE";
                command.Destination_Addr = mobileNo;
                // Update 17.2
                command.Transaction_Id = transactionId;
                if (result != null)
                {
                    if (result.Count > 0)
                    {
                        foreach (var name in result)
                        {
                            if (name.package_class == "MAIN")  //Main
                            {
                                if (SiteSession.CurrentUICulture.IsThaiCulture())
                                {
                                    packageMainName = name.package_Short_Name_TH;
                                }
                                else
                                {
                                    packageMainName = name.package_Short_Name_EN;
                                }

                                msgtxt1 = msgtxt1.Replace("{Package}", packageMainName);

                                command.Message_Text = msgtxt1;
                                if (!string.IsNullOrEmpty(command.Message_Text))
                                {
                                    _sendSmsCommand.Handle(command);
                                    //Thread.Sleep(15000);
                                }
                                command.Message_Text = "";
                                msgtxt1 = "";

                            }
                        }

                    }
                }
            }

        }

        public static byte[] BytesToHex(byte[] bInput)
        {
            byte[] bOutput;
            int nInputIndex = 0;
            int nOutputIndex = 0;
            byte nThisByte;

            bOutput = new byte[bInput.Length * 2];
            while (nInputIndex < bInput.Length)
            {
                nThisByte = (byte)((bInput[nInputIndex] & 0xf0) >> 4);
                if (nThisByte >= 10)
                    nThisByte = (byte)((nThisByte - 10) + (byte)'A');
                else
                    nThisByte += (byte)'0';
                bOutput[nOutputIndex++] = nThisByte;
                nThisByte = (byte)(bInput[nInputIndex++] & 0x0f);
                if (nThisByte >= 10)
                    nThisByte = (byte)((nThisByte - 10) + (byte)'A');
                else
                    nThisByte += (byte)'0';
                bOutput[nOutputIndex++] = nThisByte;
            }
            return bOutput;
        }

        private string HashToHex(string plainText)
        {
            string hashed = null;
            char[] cEncryptedChars;
            try
            {
                byte[] inputBytes = UTF8Encoding.ASCII.GetBytes(plainText);
                SHA256Managed hash = new SHA256Managed();
                byte[] hashBytes = hash.ComputeHash(inputBytes);
                cEncryptedChars = ASCIIEncoding.ASCII.GetChars(BytesToHex(hashBytes));
                hashed = new String(cEncryptedChars).ToLower();
            }
            catch (Exception e)
            {
                throw new Exception("Hash failsed: " + e.Message);
            }
            return hashed;
        }

        private const string initVector = "pemgail9uzpgzl88";
        // This constant is used to determine the keysize of the encryption algorithm
        private const int keysize = 256;
        //Encrypt
        public string EncryptString(string plainText, string passPhrase)
        {
            byte[] initVectorBytes = Encoding.UTF8.GetBytes(initVector);
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, null);
            byte[] keyBytes = password.GetBytes(keysize / 8);
            RijndaelManaged symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;
            ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
            cryptoStream.FlushFinalBlock();
            byte[] cipherTextBytes = memoryStream.ToArray();
            memoryStream.Close();
            cryptoStream.Close();
            return Convert.ToBase64String(cipherTextBytes);
        }
        //Decrypt
        public string DecryptString(string cipherText, string passPhrase)
        {
            byte[] initVectorBytes = Encoding.UTF8.GetBytes(initVector);
            byte[] cipherTextBytes = Convert.FromBase64String(cipherText);
            PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, null);
            byte[] keyBytes = password.GetBytes(keysize / 8);
            RijndaelManaged symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;
            ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes);
            MemoryStream memoryStream = new MemoryStream(cipherTextBytes);
            CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            byte[] plainTextBytes = new byte[cipherTextBytes.Length];
            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
            memoryStream.Close();
            cryptoStream.Close();
            return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
        }

        struct ObjReservedDslam
        {
            public string ReservationIdResult { get; set; }
        }

        #region R18.10 QR Code

        [HttpPost]
        public ActionResult GetOrderPayment(string orderId)
        {
            var paymentStatus = string.Empty;
            try
            {
                var query = new OrderPaymentQuery
                {
                    p_order_id = orderId
                };

                var data = _queryProcessor.Execute(query);
                paymentStatus = data.payment_status;

            }
            catch (Exception ex)
            {

            }
            return Json(paymentStatus, JsonRequestBehavior.AllowGet);
        }


        private GetListORDDetailCreateModel QrcodeGetListOrderDetailCreate(string orderId)
        {
            var query = new GetListORDDetailCreateQuery
            {
                p_order_id = orderId
            };
            var data = _queryProcessor.Execute(query);

            return data;
        }

        private SaveOrderResp QrcodeGetSaveOrderResp(GetListORDDetailCreateModel model)
        {
            var query = new GetSaveOrderRespJobQuery();
            if (model.ODRDetailCustomerList != null && model.ODRDetailCustomerList.Count > 0)
            {
                foreach (var item in model.ODRDetailCustomerList)
                {
                    query.CUSTOMER_TYPE = item.customer_type.ToSafeString();
                    query.CUSTOMER_SUBTYPE = item.customer_subtype.ToSafeString();
                    query.TITLE_CODE = item.title_code.ToSafeString();
                    query.FIRST_NAME = item.first_name.ToSafeString();
                    query.LAST_NAME = item.last_name.ToSafeString();
                    query.CONTACT_TITLE_CODE = item.contact_title_code.ToSafeString();
                    query.CONTACT_FIRST_NAME = item.contact_first_name.ToSafeString();
                    query.CONTACT_LAST_NAME = item.contact_last_name.ToSafeString();
                    query.ID_CARD_TYPE_DESC = item.id_card_type_desc.ToSafeString();
                    query.ID_CARD_NO = item.id_card_no.ToSafeString();
                    query.TAX_ID = item.tax_id.ToSafeString();
                    query.GENDER = item.gender.ToSafeString();
                    query.BIRTH_DATE = item.birth_date.ToSafeString();
                    query.MOBILE_NO = item.mobile_no.ToSafeString();
                    query.MOBILE_NO_2 = item.mobile_no_2.ToSafeString();
                    query.HOME_PHONE_NO = item.home_phone_no.ToSafeString();
                    query.EMAIL_ADDRESS = item.email_address.ToSafeString();
                    query.CONTACT_TIME = item.contact_time.ToSafeString();
                    query.NATIONALITY_DESC = item.nationality_desc.ToSafeString();
                    query.CUSTOMER_REMARK = item.customer_remark.ToSafeString();
                    query.HOUSE_NO = item.house_no.ToSafeString();
                    query.MOO_NO = item.moo_no.ToSafeString();
                    query.BUILDING = item.building.ToSafeString();
                    query.FLOOR = item.floor.ToSafeString();
                    query.ROOM = item.room.ToSafeString();
                    query.MOOBAN = item.mooban.ToSafeString();
                    query.SOI = item.soi.ToSafeString();
                    query.ROAD = item.road.ToSafeString();
                    query.ZIPCODE_ROWID = item.zipcode_rowid.ToSafeString();
                    query.LATITUDE = item.latitude.ToSafeString();
                    query.LONGTITUDE = item.longtitude.ToSafeString();
                    query.ASC_CODE = item.asc_code.ToSafeString();
                    query.EMPLOYEE_ID = item.employee_id.ToSafeString();
                    query.LOCATION_CODE = item.location_code.ToSafeString();
                    query.SALE_REPRESENT = item.sale_represent.ToSafeString();
                    query.CS_NOTE = item.cs_note.ToSafeString();
                    query.WIFI_ACCESS_POINT = item.wifi_access_point.ToSafeString();
                    query.INSTALL_STATUS = item.install_status.ToSafeString();
                    query.COVERAGE = item.coverage.ToSafeString();
                    query.EXISTING_AIRNET_NO = item.existing_airnet_no.ToSafeString();
                    query.GSM_MOBILE_NO = item.gsm_mobile_no.ToSafeString();
                    query.CONTACT_NAME_1 = item.contact_name_1.ToSafeString();
                    query.CONTACT_NAME_2 = item.contact_name_2.ToSafeString();
                    query.CONTACT_MOBILE_NO_1 = item.contact_mobile_no_1.ToSafeString();
                    query.CONTACT_MOBILE_NO_2 = item.contact_mobile_no_2.ToSafeString();
                    query.CONDO_FLOOR = item.condo_floor.ToSafeString();
                    query.CONDO_ROOF_TOP = item.condo_roof_top.ToSafeString();
                    query.CONDO_BALCONY = item.condo_balcony.ToSafeString();
                    query.BALCONY_NORTH = item.balcony_north.ToSafeString();
                    query.BALCONY_SOUTH = item.balcony_south.ToSafeString();
                    query.BALCONY_EAST = item.balcony_east.ToSafeString();
                    query.BALCONY_WAST = item.balcony_wast.ToSafeString();
                    query.HIGH_BUILDING = item.high_building.ToSafeString();
                    query.HIGH_TREE = item.high_tree.ToSafeString();
                    query.BILLBOARD = item.billboard.ToSafeString();
                    query.EXPRESSWAY = item.expressway.ToSafeString();
                    query.ADDRESS_TYPE_WIRE = item.address_type_wire.ToSafeString();
                    query.ADDRESS_TYPE = item.address_type.ToSafeString();
                    query.FLOOR_NO = item.floor_no.ToSafeString();
                    query.HOUSE_NO_BL = item.house_no_bl.ToSafeString();
                    query.MOO_NO_BL = item.moo_no_bl.ToSafeString();
                    query.BUILDING_BL = item.building_bl.ToSafeString();
                    query.FLOOR_BL = item.floor_bl.ToSafeString();
                    query.ROOM_BL = item.room_bl.ToSafeString();
                    query.MOOBAN_BL = item.mooban_bl.ToSafeString();
                    query.SOI_BL = item.soi_bl.ToSafeString();
                    query.ROAD_BL = item.road_bl.ToSafeString();
                    query.ZIPCODE_ROWID_BL = item.zipcode_rowid_bl.ToSafeString();
                    query.HOUSE_NO_VT = item.house_no_vt.ToSafeString();
                    query.MOO_NO_VT = item.moo_no_vt.ToSafeString();
                    query.BUILDING_VT = item.building_vt.ToSafeString();
                    query.FLOOR_VT = item.floor_vt.ToSafeString();
                    query.ROOM_VT = item.room_vt.ToSafeString();
                    query.MOOBAN_VT = item.mooban_vt.ToSafeString();
                    query.SOI_VT = item.soi_vt.ToSafeString();
                    query.ROAD_VT = item.road_vt.ToSafeString();
                    query.ZIPCODE_ROWID_VT = item.zipcode_rowid_vt.ToSafeString();
                    query.CVR_ID = item.cvr_id.ToSafeString();
                    query.CVR_NODE = item.cvr_node.ToSafeString();
                    query.CVR_TOWER = item.cvr_tower.ToSafeString();
                    query.RELATE_MOBILE = item.relate_mobile.ToSafeString();
                    query.RELATE_NON_MOBILE = item.relate_non_mobile.ToSafeString();
                    query.SFF_CA_NO = item.sff_ca_no.ToSafeString();
                    query.SFF_SA_NO = item.sff_sa_no.ToSafeString();
                    query.SFF_BA_NO = item.sff_ba_no.ToSafeString();
                    query.NETWORK_TYPE = item.network_type.ToSafeString();
                    query.SERVICE_DAY = item.service_day.ToSafeString();
                    query.EXPECT_INSTALL_DATE = item.expect_install_date.ToSafeString();
                    query.SERVICE_DAYSpecified = item.service_dayspecified; // xxx
                    query.FTTX_VENDOR = item.fttx_vendor.ToSafeString();
                    query.INSTALL_NOTE = item.install_note.ToSafeString();
                    query.PHONE_FLAG = item.phone_flag.ToSafeString();
                    query.TIME_SLOT = item.time_slot.ToSafeString();
                    query.INSTALLATION_CAPACITY = item.installation_capacity.ToSafeString();
                    query.ADDRESS_ID = item.address_id.ToSafeString();
                    query.ACCESS_MODE = item.access_mode.ToSafeString();
                    query.ENG_FLAG = item.eng_flag.ToSafeString();
                    query.EVENT_CODE = item.event_code.ToSafeString();
                    query.INSTALLADDRESS1 = item.installaddress1.ToSafeString();
                    query.INSTALLADDRESS2 = item.installaddress2.ToSafeString();
                    query.INSTALLADDRESS3 = item.installaddress3.ToSafeString();
                    query.INSTALLADDRESS4 = item.installaddress4.ToSafeString();
                    query.INSTALLADDRESS5 = item.installaddress5.ToSafeString();
                    query.PBOX_COUNT = item.pbox_count.ToSafeString();
                    query.CONVERGENCE_FLAG = item.convergence_flag.ToSafeString();
                    query.TIME_SLOT_ID = item.time_slot_id.ToSafeString();
                    query.GIFT_VOUCHER = item.gift_voucher.ToSafeString();
                    query.SUB_LOCATION_ID = item.sub_location_id.ToSafeString();
                    query.SUB_CONTRACT_NAME = item.sub_contract_name.ToSafeString();
                    query.INSTALL_STAFF_ID = item.install_staff_id.ToSafeString();
                    query.INSTALL_STAFF_NAME = item.install_staff_name.ToSafeString();
                    query.FLOW_FLAG = item.flow_flag.ToSafeString();
                    query.SITE_CODE = item.site_code.ToSafeString();
                    query.LINE_ID = item.line_id.ToSafeString();
                    query.RELATE_PROJECT_NAME = item.relate_project_name.ToSafeString();
                    query.PLUG_AND_PLAY_FLAG = item.plug_and_play_flag.ToSafeString();
                    query.RESERVED_ID = item.reserved_id.ToSafeString();
                    query.JOB_ORDER_TYPE = item.job_order_type.ToSafeString();
                    query.ASSIGN_RULE = item.assign_rule.ToSafeString();
                    query.OLD_ISP = item.old_isp.ToSafeString();
                    query.SPLITTER_FLAG = item.splitter_flag.ToSafeString();
                    query.RESERVED_PORT_ID = item.reserved_port_id.ToSafeString();
                    query.SPECIAL_REMARK = item.special_remark.ToSafeString();
                    query.ORDER_NO = item.order_no.ToSafeString();
                    query.SOURCE_SYSTEM = item.source_system.ToSafeString();
                    query.BILL_MEDIA = item.bill_media.ToSafeString();
                    query.PRE_ORDER_NO = item.pre_order_no.ToSafeString();
                    query.VOUCHER_DESC = item.voucher_desc.ToSafeString();
                    query.CAMPAIGN_PROJECT_NAME = item.campaign_project_name.ToSafeString();
                    query.PRE_ORDER_CHANEL = item.pre_order_chanel.ToSafeString();
                    query.RENTAL_FLAG = item.rental_flag.ToSafeString();
                    query.DEV_PROJECT_CODE = item.dev_project_code.ToSafeString();
                    query.DEV_BILL_TO = item.dev_bill_to.ToSafeString();
                    query.DEV_PO_NO = item.dev_po_no.ToSafeString();
                    query.PARTNER_TYPE = item.partner_type.ToSafeString();
                    query.PARTNER_SUBTYPE = item.partner_subtype.ToSafeString();
                    query.MOBILE_BY_ASC = item.mobile_by_asc.ToSafeString();
                    query.LOCATION_NAME = item.location_name.ToSafeString();
                    query.PAYMENTMETHOD = item.paymentmethod.ToSafeString();
                    query.TRANSACTIONID_IN = item.transactionid_in.ToSafeString();
                    query.TRANSACTIONID = item.transactionid.ToSafeString();
                    query.SUB_ACCESS_MODE = item.sub_access_mode.ToSafeString();
                    query.REQUEST_SUB_FLAG = item.request_sub_flag.ToSafeString();
                    query.PREMIUM_FLAG = item.premium_flag.ToSafeString();
                    query.RELATE_MOBILE_SEGMENT = item.relate_mobile_segment.ToSafeString();
                    query.REF_UR_NO = item.ref_ur_no.ToSafeString();
                    query.LOCATION_EMAIL_BY_REGION = item.location_email_by_region.ToSafeString();
                }
            }
            if (model.ODRDetailPackageList != null && model.ODRDetailPackageList.Count() > 0)
            {
                var registPackageList = new List<REGIST_PACKAGE>();
                foreach (var item in model.ODRDetailPackageList)
                {
                    var registPackage = new REGIST_PACKAGE
                    {
                        faxFlag = item.fax_flag.ToSafeString(),
                        homeIp = item.home_ip.ToSafeString(),
                        homePort = item.home_port.ToSafeString(),
                        iddFlag = item.idd_flag.ToSafeString(),
                        mobileForward = item.mobile_forward.ToSafeString(),
                        packageCode = item.package_code.ToSafeString(),
                        packagePrice = item.package_price,
                        packagePriceSpecified = true,
                        packageType = item.package_type.ToSafeString(),
                        pboxExt = item.pbox_ext.ToSafeString(),
                        productSubtype = item.product_subtype.ToSafeString(),
                        tempIa = item.temp_ia.ToSafeString()
                    };
                    registPackageList.Add(registPackage);
                }
                query.REGIST_PACKAGE_LIST = registPackageList;
            }

            if (model.ODRDetailFileList != null && model.ODRDetailFileList.Count() > 0)
            {
                var registFileList = new List<REGIST_FILE>();
                foreach (var item in model.ODRDetailFileList)
                {
                    var registFile = new REGIST_FILE
                    {
                        fileName = item.file_name.ToSafeString()
                    };
                    registFileList.Add(registFile);
                }
                query.REGIST_FILE_LIST = registFileList;
            }

            if (model.ODRDetailSplitterList != null && model.ODRDetailSplitterList.Count() > 0)
            {
                var registSplitterList = new List<REGIST_SPLITTER>();
                foreach (var item in model.ODRDetailSplitterList)
                {
                    var registSplitter = new REGIST_SPLITTER
                    {
                        distance = item.distance,
                        distanceSpecified = true,
                        distanceType = item.distance_type.ToSafeString(),
                        resourceType = item.resource_type.ToSafeString(),
                        splitterName = item.splitter_name.ToSafeString()
                    };
                    registSplitterList.Add(registSplitter);
                }
                query.REGIST_SPLITTER_LIST = registSplitterList;
            }

            if (model.ODRDetailCPEList != null && model.ODRDetailCPEList.Count() > 0)
            {
                var registCpeList = new List<REGIST_CPE_SERIAL>();
                foreach (var item in model.ODRDetailCPEList)
                {
                    var registCpe = new REGIST_CPE_SERIAL
                    {
                        cpeType = item.cpe_type.ToSafeString(),
                        macAddress = item.mac_address.ToSafeString(),
                        serialNo = item.serial_no.ToSafeString()
                    };
                    if (registCpe.serialNo != null && registCpe.serialNo != "")
                    {
                        registCpeList.Add(registCpe);
                    }
                }

                query.REGIST_CPE_SERIAL_LIST = registCpeList;
            }

            var data = _queryProcessor.Execute(query);
            return data;
        }

        private string QrcodeRegisterCustomer(GetListORDDetailCreateModel model)
        {
            var command = new CustRegisterJobCommand
            {
                RETURN_IA_NO = model.return_ia_no,
                RETURN_ORDER_NO = model.return_ia_no,
                TRANSACTIONID_IN = model.ODRDetailCustomerList[0].transactionid_in,
                TRANSACTIONID = model.ODRDetailCustomerList[0].transactionid,
                PAYMENTMETHOD = model.ODRDetailCustomerList[0].paymentmethod,
                PLUG_AND_PLAY_FLAG = model.ODRDetailCustomerList[0].plug_and_play_flag,
                ClientIP = "",
                FullUrl = ""
            };
            _custRegJobCommand.Handle(command);

            return command.CUSTOMERID;
        }

        private void QrcodeGenPdfAndSendEmail(string custRowId, string L_CARD_NO, string OrderNo, string L_CONTACT_PHONE, string mailTo, bool IsThaiCulture, string user, string pass, string ip, string imagepathimer)
        {
            int CurrentUICulture = 2;
            string langPDFAPP = "E";
            if (IsThaiCulture)
            {
                CurrentUICulture = 1;
                langPDFAPP = "T";
            }

            #region PDF

            var running_no = InsertMailLog(custRowId);

            System.IFormatProvider format = new System.Globalization.CultureInfo("en-US");
            string filename = "Request" + DateTime.Now.ToString("ddMMyy", format) + "_" + running_no.ToSafeString();
            string directoryPathApp = "";

            @directoryPathApp = QrcodeGeneratePdfApp(L_CARD_NO, OrderNo, langPDFAPP, L_CONTACT_PHONE, user, pass, ip, imagepathimer);

            #endregion PDF

            if (mailTo != "")
            {
                string filePathAppNASTemp = "";
                if (directoryPathApp != "")
                {
                    filePathAppNASTemp = directoryPathApp.Substring(2);
                }

                string filePathAppNAS = "";

                if (filePathAppNASTemp != "")
                {
                    filePathAppNAS = "\\\\" + ip + filePathAppNASTemp.Replace(filePathAppNASTemp.Split('\\')[0], "");
                }

                var command = new NotificationCommand
                {
                    CustomerId = custRowId,
                    CurrentCulture = CurrentUICulture,
                    RunningNo = running_no,
                    EmailModel = new EmailModel
                    {
                        MailTo = mailTo,
                        FilePath = "",
                        FilePath2 = filePathAppNAS,
                    },
                    ImpersonateUser = user,
                    ImpersonatePass = pass,
                    ImpersonateIP = ip
                };

                _noticeCommand.Handle(command);
            }
        }

        private string QrcodeGeneratePdfApp(string CardNo, string orderNo, string Language, string contactNo, string user, string pass, string ip, string imagepathimer)
        {
            try
            {

                var html = "<?xml version=\"1.0\" encoding=\"utf-8\"?>";

                Byte[] bytes = null;

                PDFDataQuery query = new PDFDataQuery();
                query.orderNo = orderNo;
                query.Language = Language;
                query.isEApp = true;
                var htmlFromPackage = QueryGeneratePDF(query);

                html = html + htmlFromPackage;

                html = html.Replace("{Sign}", "");

                try
                {
                    bytes = htmlToPDF(html);
                }
                catch (Exception ex1)
                {
                    throw new Exception("htmlToPDF error. Error : " + ex1.Message);
                }

                bytes = PdfSecurity.SetPasswordPdf(bytes, CardNo);

                var queryName = new GetFormatFileNameEAPPQuery
                {
                    ID_CardNo = CardNo,
                };

                var result = _queryProcessor.Execute(queryName);

                string fileName = result.file_name;
                string yearweek = (DateTime.Now.Year.ToString());
                string monthyear = (DateTime.Now.Month.ToString("00"));

                string imagepathimerTemp = Path.Combine(imagepathimer, yearweek + monthyear);

                imagepathimer = imagepathimerTemp;

                Logger.Info("GeneratePDFApp : imagepathimer = " + imagepathimer);

                string pathfileImpesontae = "";

                using (var impersonator = new Impersonator(user, ip, pass, false))
                {
                    System.IO.Directory.CreateDirectory(@imagepathimer);

                    pathfileImpesontae = Path.Combine(imagepathimer, fileName + ".pdf");
                    Logger.Info("GeneratePDFApp : pathfileImpesontae = " + pathfileImpesontae);

                    if (bytes != null)
                    {
                        try
                        {
                            PdfSecurity.WriteFile(pathfileImpesontae, bytes);
                        }
                        catch (Exception ex2)
                        {
                            throw new Exception("PdfSecurity.WriteFile is Error : " + ex2.Message);
                        }

                        UpdateFileName(orderNo, pathfileImpesontae, contactNo);

                    }
                    else
                    {
                        Logger.Info("GeneratePDFApp : bytes file is null.");
                    }
                }

                return pathfileImpesontae;
            }
            catch (Exception ex)
            {
                Logger.Info("GeneratePDFApp : " + ex.GetBaseException());
                return "";
            }
        }

        public void QrcodeSendSms(string mobileNo, string mainCode, bool IsThaiCulture, string msgtxt)
        {

            string[] strMainCode = null;
            if (!string.IsNullOrEmpty(mainCode))
            {
                strMainCode = mainCode.Split('|');
            }
            lock (thisLock)
            {
                if (mobileNo.Substring(0, 2) != "66")
                {
                    if (mobileNo.Substring(0, 1) == "0")
                    {
                        mobileNo = "66" + mobileNo.Substring(1);
                    }
                }

                List<string> promotionCode = new List<string>();
                if (strMainCode != null)
                {
                    foreach (string mCode in strMainCode)
                    {
                        if (!string.IsNullOrEmpty(mCode))
                        {
                            promotionCode.Add(mCode);
                        }

                    }
                }

                var air_old_list = new List<AIR_CHANGE_OLD_PACKAGE_ARRAY>();
                foreach (var code in promotionCode)
                {
                    if (!string.IsNullOrEmpty(code))
                    {
                        var air = new AIR_CHANGE_OLD_PACKAGE_ARRAY()
                        {
                            enddt = "",
                            productSeq = "",
                            sffPromotionCode = code,
                            startdt = ""
                        };
                        air_old_list.Add(air);
                    }
                }

                List<ListShortNameModel> result = new List<ListShortNameModel>();

                if (air_old_list != null && air_old_list.Count > 0)
                {

                    var query = new GetListShortNamePackageQuery()
                    {
                        id_card_no = "",
                        airChangePromotionCode_List = air_old_list,
                        transaction_id = "",
                        FullUrl = ""
                    };
                    result = _queryProcessor.Execute(query);
                }

                var command = new SendSmsCommand();
                command.FullUrl = "";
                command.Source_Addr = "AISFIBRE";
                command.Destination_Addr = mobileNo;
                // Update 17.2
                command.Transaction_Id = "";
                var packageMainName = "";

                if (result != null && result.Count > 0)
                {
                    foreach (var name in result)
                    {
                        if (name.package_class == "MAIN")  //Main
                        {
                            if (IsThaiCulture)
                            {
                                packageMainName = name.package_Short_Name_TH;
                            }
                            else
                            {
                                packageMainName = name.package_Short_Name_EN;
                            }

                            msgtxt = msgtxt.Replace("{Package}", packageMainName);

                            command.Message_Text = msgtxt;
                            if (!string.IsNullOrEmpty(command.Message_Text))
                            {
                                _sendSmsCommand.Handle(command);
                                //Thread.Sleep(15000);
                            }
                            command.Message_Text = "";
                            msgtxt = "";

                        }
                    }

                }
                else
                {
                    command.Message_Text = msgtxt;
                    if (!string.IsNullOrEmpty(command.Message_Text))
                    {
                        _sendSmsCommand.Handle(command);
                        //Thread.Sleep(15000);
                    }
                    command.Message_Text = "";
                    msgtxt = "";
                }

            }

        }

        #endregion R18.10 QR Code

        public ActionResult ErrorUrlEmptyPaymentScpe(string ErrorMsg = "", string FullUrl = "")
        {
            if (!string.IsNullOrEmpty(FullUrl))
            {
                System.Web.HttpContext.Current.Session["FullUrl"] = FullUrl;
            }

            //if (!string.IsNullOrEmpty(ErrorMsg))
            //{
            //    TempData["statusPay"] = ErrorMsg;
            //    System.Web.HttpContext.Current.Session["statusPayScpe"] = ErrorMsg;
            //}

            return RedirectToAction("Index", "Scpe", new { statusPay = "ErrorSPDP" });
        }
    }
}
