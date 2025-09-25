using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.X509;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Commands.WebServices;
using WBBContract.Queries.Commons.Account;
using WBBContract.Queries.Commons.Masters;
using WBBContract.Queries.ExWebServices;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBContract.Queries.FBSS;
using WBBContract.Queries.WebServices;
using WBBContract.Queries.WebServices.WTTX;
using WBBEntity.Extensions;
using WBBEntity.PanelModels;
using WBBEntity.PanelModels.Account;
using WBBEntity.PanelModels.ExWebServiceModels;
using WBBEntity.PanelModels.FBBWebConfigModels;
using WBBEntity.PanelModels.WebServiceModels;
using WBBWeb.Extension;
using WBBWeb.Models;
using WBBWeb.Solid.CompositionRoot;
using WBBSECURE = WBBBusinessLayer.Extension.Security;

using RestSharp;
using System.Security.Policy;
using WBBBusinessLayer.SBNV2WebService;
using Org.BouncyCastle.Asn1.Utilities;
using WBBContract.Models;

namespace WBBWeb.Controllers
{
    // rebuild
    public enum ResizeOptions
    {
        // Use fixed width & height without keeping the proportions
        ExactWidthAndHeight,

        // Use maximum width (as defined) and keeping the proportions
        MaxWidth,

        // Use maximum height (as defined) and keeping the proportions
        MaxHeight,

        // Use maximum width or height (the biggest) and keeping the proportions
        MaxWidthAndHeight
    }


    //public class MemoryPostedFile : HttpPostedFileBase
    //{
    //    private readonly byte[] fileBytes;

    //    public MemoryPostedFile(byte[] fileBytes, string fileName = null)
    //    {
    //        this.fileBytes = fileBytes;
    //        //this.FileName = fileName;
    //        //this.InputStream = new MemoryStream(fileBytes);
    //    }

    //    public override int ContentLength {
    //        get { return fileBytes.Length; }
    //    }

    //    public override string FileName { get; }

    //    public override Stream InputStream { get {
    //        return new MemoryStream(this.fileBytes);
    //    } }
    //}

    [CustomActionFilter]
    [CustomHandleError]
    [IENoCache]
    public partial class ProcessController : WBBController
    {
        private readonly ICommandHandler<InsertMasterTDMContractDeviceCommand> _insertMasterTDMContractDeviceCommand;
        private readonly ICommandHandler<InsertMasterTDMContractDeviceMeshCommand> _insertMasterTDMContractDeviceMeshCommand;
        private readonly ICommandHandler<ChangeContactMobileCommand> _changeContactMobileCommand;
        private readonly ICommandHandler<UpdateFileNameCommand> _updateFileNameCommand;
        private readonly ICommandHandler<CustRegisterCommand> _custRegCommand;
        private readonly ICommandHandler<InterfaceLogCommand> _intfLogCommand;
        private readonly ICommandHandler<MailLogCommand> _mailLogCommand;
        private readonly ICommandHandler<NotificationCommand> _noticeCommand;
        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<SendSmsCommand> _sendSmsCommand;
        private readonly ICommandHandler<StatLogCommand> _StatLogCommand;
        private readonly ICommandHandler<UpdatePreregisterStatusPackageCommand> _updatePregisterCommand;
        private readonly ICommandHandler<SessionLoginCommand> _sessionLoginCommand;
        private readonly ICommandHandler<SaveChangeStatusBuildingCommand> _saveChangeStatusBuildingCommand;
        private readonly ICommandHandler<GetFBBOrderNoCommand> _getFBBOrderNoCommand;
        private readonly IQueryHandler<GetFBBFBSSCoverageAreaResultQuery, GetLeaveMsgReferenceNoCommand> _coverageResult;
        private readonly ICommandHandler<GetLeaveMsgReferenceNoCommand> _getLeaveMsgRefCommand;
        private readonly ICommandHandler<SavePaymentLogCommand> _savePaymentLogCommand;
        private readonly ICommandHandler<CreateOrderMeshPromotionCommand> _createOrderMeshPromotionCommand;
        private readonly ICommandHandler<CustRegisterJobCommand> _custRegJobCommand;
        private readonly ICommandHandler<SavePaymentSPDPLogCommand> _savePaymentSPDPLogCommand;
        private readonly ICommandHandler<InsertSaveOrderNew911Command> _insertSaveOrderNew911Command;
        private readonly ICommandHandler<SendSmsMGMCommand> _sendSmsMGMCommand;
        private readonly IQueryHandler<UpdateListBuildingVillageIMQuery, UpdateListBuildingVillageIMModel> _updateListBuildingVillageIMQuery;
        private readonly IQueryHandler<UpdateListBuildingVillageSDQuery, UpdateListBuildingVillageSDModel> _updateListBuildingVillageSDQuery;
        private readonly ICommandHandler<InsertCloudIPCameraCommand> _insertRegIpCameraCommand;
        private readonly ICommandHandler<SaveRegisterFraudCommand> _saveRegisterFraudCommand;
        private readonly ICommandHandler<SaveRegisterFraudNoGoCommand> _saveRegisterFraudNoGoCommand;

        //R23.05.2023 Created: THOTST49
        private readonly ICommandHandler<InsertConsentLogNewRegisterCommand> _insertConsentLogNewRegisterCommand;

        public ProcessController(IQueryProcessor queryProcessor,
            ICommandHandler<InterfaceLogCommand> intfLogCommand,
            ICommandHandler<CustRegisterCommand> custRegCommand,
            ICommandHandler<NotificationCommand> noticeCommand,
            ICommandHandler<MailLogCommand> mailLogCommand,
            ICommandHandler<StatLogCommand> StatLogCommand,
            ICommandHandler<SendSmsCommand> SendSmsCommand,
            ICommandHandler<InsertMasterTDMContractDeviceCommand> InsertMasterTDMContractDeviceCommand,
            ICommandHandler<InsertMasterTDMContractDeviceMeshCommand> InsertMasterTDMContractDeviceMeshCommand,
            ICommandHandler<ChangeContactMobileCommand> ChangeContactMobileCommand,
            ICommandHandler<UpdateFileNameCommand> UpdateFileNameCommand,
            ICommandHandler<UpdatePreregisterStatusPackageCommand> updatePregisterCommand,
            ICommandHandler<SessionLoginCommand> sessionLoginCommand,
            ICommandHandler<SaveChangeStatusBuildingCommand> saveChangeStatusBuildingCommand,
            ICommandHandler<GetFBBOrderNoCommand> getFBBOrderNoCommand,
            IQueryHandler<GetFBBFBSSCoverageAreaResultQuery, GetLeaveMsgReferenceNoCommand> coverageResult,
            ICommandHandler<GetLeaveMsgReferenceNoCommand> getLeaveMsgRefCommand,
            ICommandHandler<SavePaymentLogCommand> savePaymentLogCommand,
            ICommandHandler<CreateOrderMeshPromotionCommand> createOrderMeshPromotionCommand,
            ICommandHandler<CustRegisterJobCommand> custRegJobCommand,
            ILogger logger,
            ICommandHandler<SavePaymentSPDPLogCommand> savePaymentSPDPLogCommand,
            ICommandHandler<InsertSaveOrderNew911Command> insertSaveOrderNew911Command,
            ICommandHandler<SendSmsMGMCommand> sendSmsMGMCommand,
            IQueryHandler<UpdateListBuildingVillageIMQuery, UpdateListBuildingVillageIMModel> updateListBuildingVillageIMQuery,
            IQueryHandler<UpdateListBuildingVillageSDQuery, UpdateListBuildingVillageSDModel> updateListBuildingVillageSDQuery,
            ICommandHandler<InsertCloudIPCameraCommand> insertRegIpCameraCommand

            //R23.05.2023 Created: THOTST49
            , ICommandHandler<InsertConsentLogNewRegisterCommand> insertConsentLogNewRegisterCommand,
            //R24.01 check fraud
            ICommandHandler<SaveRegisterFraudCommand> saveRegisterFraudCommand,
            ICommandHandler<SaveRegisterFraudNoGoCommand> saveRegisterFraudNoGoCommand
            )
        {
            _queryProcessor = queryProcessor;
            _intfLogCommand = intfLogCommand;
            _custRegCommand = custRegCommand;
            _noticeCommand = noticeCommand;
            _mailLogCommand = mailLogCommand;
            _StatLogCommand = StatLogCommand;
            _sendSmsCommand = SendSmsCommand;
            _insertMasterTDMContractDeviceCommand = InsertMasterTDMContractDeviceCommand;
            _insertMasterTDMContractDeviceMeshCommand = InsertMasterTDMContractDeviceMeshCommand;
            _changeContactMobileCommand = ChangeContactMobileCommand;
            _updateFileNameCommand = UpdateFileNameCommand;
            _updatePregisterCommand = updatePregisterCommand;
            _sessionLoginCommand = sessionLoginCommand;
            _saveChangeStatusBuildingCommand = saveChangeStatusBuildingCommand;
            _getFBBOrderNoCommand = getFBBOrderNoCommand;
            _coverageResult = coverageResult;
            _getLeaveMsgRefCommand = getLeaveMsgRefCommand;
            _savePaymentLogCommand = savePaymentLogCommand;
            _createOrderMeshPromotionCommand = createOrderMeshPromotionCommand;
            _custRegJobCommand = custRegJobCommand;
            base.Logger = logger;
            _savePaymentSPDPLogCommand = savePaymentSPDPLogCommand;
            _insertSaveOrderNew911Command = insertSaveOrderNew911Command;
            _sendSmsMGMCommand = sendSmsMGMCommand;
            _updateListBuildingVillageIMQuery = updateListBuildingVillageIMQuery;
            _updateListBuildingVillageSDQuery = updateListBuildingVillageSDQuery;
            _insertRegIpCameraCommand = insertRegIpCameraCommand;

            //R23.05.2023 Created: THOTST49
            _insertConsentLogNewRegisterCommand = insertConsentLogNewRegisterCommand;

            //R24.01 check fraud
            _saveRegisterFraudCommand = saveRegisterFraudCommand;
            _saveRegisterFraudNoGoCommand = saveRegisterFraudNoGoCommand;
        }

        [HttpPost]
        public ActionResult AssignValueCardReaderToModel()
        {
            //fix test
            //Session["CARD_READER_NATIONALID"] = "1409900046859";
            //Session["CARD_READER_TITLE_TH"] = "นาย";
            //Session["CARD_READER_FIRSTNAME_TH"] = "แจ๊กกี้";
            //Session["CARD_READER_LASTNAME_TH"] = "ทดสอบ";
            //Session["CARD_READER_TITLE_EN"] = "MR";
            //Session["CARD_READER_FIRSTNAME_EN"] = "F_TEST";
            //Session["CARD_READER_LASTNAME_EN"] = "L_TEST";
            //Session["CARD_READER_BIRTHDATE"] = "28-06-2527";
            //Session["CARD_READER_SEX"] = "M";
            //Session["CARD_READER_ADDRESS"] = "123";
            //Session["CARD_READER_MOO"] = "หมู่5";
            //Session["CARD_READER_ALLEY"] = "";
            //Session["CARD_READER_SOI"] = "จรัญสนิทวงศ์ 65";
            //Session["CARD_READER_STREET"] = "จรัญสนิทวงศ์";
            //Session["CARD_READER_SUBDISTRICT"] = "บางพลัด";
            //Session["CARD_READER_DISTRICT"] = "บางพลัด";
            //Session["CARD_READER_PROVINCE"] = "กรุงเทพ";
            //Session["CARD_READER_PICTURE"] = "";
            //Session["CARD_READER_ISSUEPLACE"] = "";
            //Session["CARD_READER_ISSUEDATE"] = "28-06-2527";
            //Session["CARD_READER_EXPIREDATE"] = "28-06-2527";

            var model = new FBB_IDCARDREADER();
            if (null != Session["CARD_READER_NATIONALID"])
            //if (1==1)
            {
                model.NATIONALID = Session["CARD_READER_NATIONALID"].ToString();
                model.TITLE_TH = Session["CARD_READER_TITLE_TH"].ToString();
                model.FIRSTNAME_TH = Session["CARD_READER_FIRSTNAME_TH"].ToString();
                model.LASTNAME_TH = Session["CARD_READER_LASTNAME_TH"].ToString();
                model.TITLE_EN = Session["CARD_READER_TITLE_EN"].ToString();
                model.FIRSTNAME_EN = Session["CARD_READER_FIRSTNAME_EN"].ToString();
                model.LASTNAME_EN = Session["CARD_READER_LASTNAME_EN"].ToString();
                model.BIRTHDATE = Session["CARD_READER_BIRTHDATE"].ToString();
                model.SEX = Session["CARD_READER_SEX"].ToString();
                model.ADDRESS = Session["CARD_READER_ADDRESS"].ToString();
                model.MOO = Session["CARD_READER_MOO"].ToString();
                model.ALLEY = Session["CARD_READER_ALLEY"].ToString();
                model.SOI = Session["CARD_READER_SOI"].ToString();
                model.STREET = Session["CARD_READER_STREET"].ToString();
                model.SUBDISTRICT = Session["CARD_READER_SUBDISTRICT"].ToString();
                model.DISTRICT = Session["CARD_READER_DISTRICT"].ToString();
                model.PROVINCE = Session["CARD_READER_PROVINCE"].ToString();
                model.PICTURE = Session["CARD_READER_PICTURE"].ToString();
                model.ISSUEPLACE = Session["CARD_READER_ISSUEPLACE"].ToString();
                model.ISSUEDATE = Session["CARD_READER_ISSUEDATE"].ToString();
                model.EXPIREDATE = Session["CARD_READER_EXPIREDATE"].ToString();

                Logger.Info("function AssignValueCardReaderToModel ===> NATIONALID  :" + Session["CARD_READER_NATIONALID"].ToString()
                            + "\r\n" + "ISSUEPLACE:  " + model.ISSUEPLACE
                            + "\r\n" + "ISSUEDATE:  " + model.ISSUEDATE
                            + "\r\n" + "EXPIREDATE:  " + model.EXPIREDATE);
            }

            return Json(new
            {
                data = new
                {
                    NATIONALID = model.NATIONALID,
                    TITLE_TH = model.TITLE_TH,
                    FIRSTNAME_TH = model.FIRSTNAME_TH,
                    LASTNAME_TH = model.LASTNAME_TH,
                    TITLE_EN = model.TITLE_EN,
                    FIRSTNAME_EN = model.FIRSTNAME_EN,
                    LASTNAME_EN = model.LASTNAME_EN,
                    BIRTHDATE = model.BIRTHDATE,
                    SEX = model.SEX,
                    ADDRESS = model.ADDRESS,
                    MOO = model.MOO,
                    ALLEY = model.ALLEY,
                    SOI = model.SOI,
                    STREET = model.STREET,
                    SUBDISTRICT = model.SUBDISTRICT,
                    DISTRICT = model.DISTRICT,
                    PROVINCE = model.PROVINCE,
                    PICTURE = model.PICTURE,
                    ISSUEPLACE = model.ISSUEPLACE,
                    ISSUEDATE = model.ISSUEDATE,
                    EXPIREDATE = model.EXPIREDATE
                },
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Base64Photo(string base64photo)
        {
            Logger.Info("Start Add Session :");
            if (null == Session["photoSignature"])
            {
                Session["photoSignature"] = base64photo;
            }
            Logger.Info("End Add Session :");

            return Json(new { result = Session["photoSignature"], }, JsonRequestBehavior.AllowGet);
        }

        public void CancelOrder(string templist, string idcard)
        {
            var cancellist = new JavaScriptSerializer().Deserialize<List<string>>(templist);
            var query = new GetCancelOrderQuery
            {
                ID_Card_No = idcard,
                ListOrder = cancellist
            };

            var result = _queryProcessor.Execute(query);
        }

        public JsonResult ChangeContactMobile(string orderNo, string mobileNo)
        {
            var commamd = new ChangeContactMobileCommand
            {
                OrderNo = orderNo,
                NewMobileContact = mobileNo
            };
            _changeContactMobileCommand.Handle(commamd);

            return Json(
                   new
                   {
                       data = new
                       {
                           wbb_ret_code = commamd.WBB_returnCode,
                           wbb_ret_msg = commamd.WBB_returnMsg,
                           air_ret_code = commamd.AIRadmin_returnCode,
                           air_ret_msg = commamd.AIRadmin_returnMsg
                       }
                   }, JsonRequestBehavior.AllowGet);
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

            string transactionId = AisAirNumber + ipAddress;

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

        public List<LovScreenValueModel> GetChangePromotionPageCode()
        {
            var screenData = GetScreenConfig(WebConstants.LovConfigName.ChangePromotionPageCode);
            return screenData;
        }

        public ActionResult ChangePackagePromotion(string Data = "")
        {
            ViewBag.hasLoadingBlock = true;
            if (Data != "" && Data != "Officer")
            {
                bool CheckInput = true;
                string DataDec = Decrypt(Data);
                string[] DataTemps = DataDec.Split('&');
                string NonMobileNo = "";
                string lang = "";
                string GetIdCardStatus = "";
                string timeStamp = "";
                string cardType = "";
                string cardNo = "";

                string ClientIP = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (string.IsNullOrEmpty(ClientIP))
                {
                    ClientIP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                }

                if (DataTemps.Count() > 1)
                {
                    foreach (var item in DataTemps)
                    {
                        string[] DataTemp = item.Split('=');
                        if (DataTemp != null && DataTemp.Count() == 2)
                        {
                            //if (DataTemp[0].ToSafeString() == "channel")
                            //{
                            //    channel = DataTemp[1].ToSafeString();
                            //}
                            if (DataTemp[0].ToSafeString() == "mobileNo")
                            {
                                NonMobileNo = DataTemp[1].ToSafeString();
                                //NonMobileNo = "8800010059";
                                ViewBag.NonMobileNo = NonMobileNo;
                            }
                            if (DataTemp[0].ToSafeString() == "lang")
                            {
                                lang = DataTemp[1].ToSafeString();
                                if (lang == "TH")
                                {
                                    ViewBag.LanguagePage = "1";
                                }
                                else
                                {
                                    ViewBag.LanguagePage = "2";
                                }
                            }
                            if (DataTemp[0].ToSafeString() == "timeStamp")
                            {
                                timeStamp = DataTemp[1].ToSafeString();
                            }
                            if (DataTemp[0].ToSafeString() == "cardType")
                            {
                                cardType = DataTemp[1].ToSafeString();
                            }
                            if (DataTemp[0].ToSafeString() == "cardNo")
                            {
                                cardNo = DataTemp[1].ToSafeString();
                            }
                        }
                        else
                        {
                            // value in put ไม่ถูกต้อง
                            CheckInput = false;
                            break;

                        }
                    }
                }
                else
                {
                    // value in put ไม่ถูกต้อง
                    CheckInput = false;

                }

                string TransactionID = NonMobileNo + ClientIP;

                InterfaceLogCommand log = null;
                log = StartInterface("DataEncrypt: " + Data + "\r\n NonMobileNo: " + NonMobileNo + "\r\n Language: " + lang + "\r\n timeStamp: " + timeStamp, "/process/ChangePackagePromotion", TransactionID, "", "ChangePackagePromotionGET");

                EndInterface("", log, TransactionID, "Success", "");

                if (cardType == "" || cardNo == "")
                {
                    CheckInput = false;
                }
                else
                {
                    List<FbbConstantModel> cardtypeDatas = GetFbbConstantModel("ID_CARD_TYPE");
                    if (cardtypeDatas != null && cardtypeDatas.Count > 0)
                    {
                        List<FbbConstantModel> cardtypeData = cardtypeDatas.Where(t => t.Field == cardType).ToList();
                        if (cardtypeData == null || cardtypeData.Count == 0)
                        {
                            CheckInput = false;
                        }
                    }
                }

                if (CheckInput)
                {
                    GetIdCardStatus = GetInfoByNonMobileNo(NonMobileNo);
                    if (GetIdCardStatus == "")
                    {
                        ViewBag.IdCard = IdCard;
                        ViewBag.CardType = CardType;
                        ViewBag.PageShow = "Mobile";
                    }
                    else
                    {
                        // Login fail
                        IdCard = "";
                        CardType = "";
                        ViewBag.PageShow = "LoginFail";
                    }
                }
                else
                {
                    // value in put ไม่ถูกต้อง
                    ViewBag.LanguagePage = "1";
                    ViewBag.PageShow = "LoginFail";
                }
            }

            Session["FullUrl"] = this.Url.Action("ChangePackagePromotion", "Process", null, this.Request.Url.Scheme);

            ViewBag.LabelFBBTR000 = GetGeneralScreenConfig();
            ViewBag.LabelFBBTR001 = GetCoverageScreenConfig();
            ViewBag.LabelFBBTR003 = GetCustRegisterScreenConfig();
            ViewBag.LabelFBBOR015 = GetProfilePrePostPaid();
            ViewBag.LabelFBBOR016 = GetChangePromotionPageCode();
            ViewBag.LabelFBBTR016 = GetChangePromotionScreenConfig();
            ViewBag.FbbConstant = GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);
            ViewBag.LabelLovScreen = GetScreenConfig("ALLPAGE");

            ViewBag.Function = "ChangePromotion";
            ViewBag.SetAction = @"/Process/ChangePackagePromotion";

            if (Data == "Officer")
            {
                ViewBag.isOfficer = true;
            }

            return View("SearchProfiles/New_Main");
        }

        [HttpPost]
        //[AjaxValidateAntiForgeryToken]
        public ActionResult ChangePackagePromotion(QuickWinPanelModel model, string Data = "")
        {
            if (Data != "" && Data != "Officer" && Session["PageLoadChangePackagePromotion"] == null)
            {
                PageLoadOntopModel tmpModel = new PageLoadOntopModel();
                tmpModel.Data = Data;
                ViewBag.PageGo = "ChangePackagePromotion";
                Session["PageLoadChangePackagePromotion"] = "HaveLoad";
                return View("PageLoadOntop", tmpModel);
            }
            else
            {
                Session["PageLoadChangePackagePromotion"] = null;
            }

            ViewBag.hasLoadingBlock = true;

            if (Data != "" && Data != "Officer")
            {
                bool CheckInput = true;
                string DataDec = Decrypt(Data);
                //DataDec = string.IksNullOrEmpty(DataDec) ? "mobileNo=8850125473&lang=TH&timeStamp=202404041030123&cardType=ID_CARD&cardNo=3226178612217" : DataDec;//R24.04 Add loading block screen on /topup, /topupinternet and /topupmesh by max kunlp885
                string[] DataTemps = DataDec.Split('&');
                string NonMobileNo = "";
                string lang = "";
                string GetIdCardStatus = "";
                string originHeader = "";
                string timeStamp = "";
                string cardType = "";
                string cardNo = "";

                try
                {
                    if (Request.UrlReferrer != null)
                    {
                        originHeader = Request.UrlReferrer.OriginalString;
                        // Do stuff with the values... probably .FirstOrDefault()
                    }
                }
                catch
                {
                    originHeader = "";
                }

                string ClientIP = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (string.IsNullOrEmpty(ClientIP))
                {
                    ClientIP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                }

                if (DataTemps.Count() > 1)
                {
                    foreach (var item in DataTemps)
                    {
                        string[] DataTemp = item.Split('=');
                        if (DataTemp != null && DataTemp.Count() == 2)
                        {
                            //if (DataTemp[0].ToSafeString() == "channel")
                            //{
                            //    channel = DataTemp[1].ToSafeString();
                            //}
                            if (DataTemp[0].ToSafeString() == "mobileNo")
                            {
                                NonMobileNo = DataTemp[1].ToSafeString();
                                //NonMobileNo = "8800010059";
                                ViewBag.NonMobileNo = NonMobileNo;
                                string checkNonMobileNo = NonMobileNo.Substring(0, 3);
                                var getLovForcheckMobileandFlag = GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);
                                var getLovCheckMobileNo = getLovForcheckMobileandFlag.FirstOrDefault(x => x.Field == "Existing_Check_Prefix_3BB").Validation;
                                var getLovFlagCheck3BB = getLovForcheckMobileandFlag.FirstOrDefault(x => x.Field == "Existing_Check_3BB_Flag").Validation;
                                if (getLovFlagCheck3BB == "Y")
                                {
                                    if (checkNonMobileNo == getLovCheckMobileNo)
                                    {
                                        ViewBag.LabelFBBTR000 = GetGeneralScreenConfig();
                                        ViewBag.LabelFBBTR001 = GetCoverageScreenConfig();
                                        ViewBag.LabelFBBTR003 = GetCustRegisterScreenConfig();
                                        ViewBag.LabelFBBOR015 = GetProfilePrePostPaid();
                                        ViewBag.LabelFBBOR016 = GetChangePromotionPageCode();
                                        ViewBag.LabelFBBTR016 = GetChangePromotionScreenConfig();
                                        ViewBag.FbbConstant = GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);
                                        ViewBag.LabelLovScreen = GetScreenConfig("ALLPAGE");
                                        ViewBag.PageShow = "LoginFail";
                                        ViewBag.Flag3BB = "3BB";
                                        ViewBag.Function = "ChangePromotion";
                                        ViewBag.SetAction = @"/Process/ChangePackagePromotion";
                                        Session["FullUrl"] = this.Url.Action("ChangePackagePromotion", "Process", null, this.Request.Url.Scheme);
                                        if (Data == "Officer")
                                        {
                                            ViewBag.isOfficer = true;
                                        }

                                        return View("SearchProfiles/New_Main");
                                    }
                                }
                            }
                            if (DataTemp[0].ToSafeString() == "lang")
                            {
                                lang = DataTemp[1].ToSafeString();
                                if (lang == "TH")
                                {
                                    ViewBag.LanguagePage = "1";
                                }
                                else
                                {
                                    ViewBag.LanguagePage = "2";
                                }
                            }

                            if (DataTemp[0].ToSafeString() == "timeStamp")
                            {
                                timeStamp = DataTemp[1].ToSafeString();
                            }
                            if (DataTemp[0].ToSafeString() == "cardType")
                            {
                                cardType = DataTemp[1].ToSafeString();
                            }
                            if (DataTemp[0].ToSafeString() == "cardNo")
                            {
                                cardNo = DataTemp[1].ToSafeString();
                            }
                        }
                        else
                        {
                            // value in put ไม่ถูกต้อง
                            CheckInput = false;
                            break;

                        }
                    }
                }
                else
                {
                    // value in put ไม่ถูกต้อง
                    CheckInput = false;

                }

                string TransactionID = NonMobileNo + ClientIP;

                InterfaceLogCommand log = null;
                log = StartInterface("DataEncrypt: " + Data + "\r\n NonMobileNo: " + NonMobileNo + "\r\n Language: " + lang + "\r\n timeStamp: " + timeStamp + "\r\n originHeader: " + originHeader, "/process/ChangePackagePromotion", TransactionID, "", "ChangePackagePromotionPost");

                EndInterface("", log, TransactionID, "Success", "");

                if (cardType == "" || cardNo == "")
                {
                    CheckInput = false;
                }
                else
                {
                    List<FbbConstantModel> cardtypeDatas = GetFbbConstantModel("ID_CARD_TYPE");
                    if (cardtypeDatas != null && cardtypeDatas.Count > 0)
                    {
                        List<FbbConstantModel> cardtypeData = cardtypeDatas.Where(t => t.Field == cardType).ToList();
                        if (cardtypeData == null || cardtypeData.Count == 0)
                        {
                            CheckInput = false;
                        }
                    }
                }

                if (CheckInput)
                {
                    GetIdCardStatus = GetInfoByNonMobileNo(NonMobileNo);
                    if (GetIdCardStatus == "")
                    {
                        ViewBag.IdCard = IdCard;
                        ViewBag.CardType = CardType;
                        ViewBag.PageShow = "Mobile";
                    }
                    else
                    {
                        // Login fail
                        IdCard = "";
                        CardType = "";
                        ViewBag.PageShow = "LoginFail";
                    }
                }
                else
                {
                    // value in put ไม่ถูกต้อง
                    ViewBag.LanguagePage = "1";
                    ViewBag.PageShow = "LoginFail";
                }
                Session["FullUrl"] = this.Url.Action("ChangePackagePromotion", "Process", null, this.Request.Url.Scheme);

                ViewBag.LabelFBBTR000 = GetGeneralScreenConfig();
                ViewBag.LabelFBBTR001 = GetCoverageScreenConfig();
                ViewBag.LabelFBBTR003 = GetCustRegisterScreenConfig();
                ViewBag.LabelFBBOR015 = GetProfilePrePostPaid();
                ViewBag.LabelFBBOR016 = GetChangePromotionPageCode();
                ViewBag.LabelFBBTR016 = GetChangePromotionScreenConfig();
                ViewBag.FbbConstant = GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);
                ViewBag.LabelLovScreen = GetScreenConfig("ALLPAGE");

                ViewBag.Function = "ChangePromotion";
                ViewBag.SetAction = @"/Process/ChangePackagePromotion";

                //20.7 change promotion - change device
                var url = this.Url.Action("ChangePackagePromotion", "Process", null, this.Request.Url.Scheme);

                if (model != null)
                {
                    model.PendingOrderFbss_Change_Promotion_Flag = string.IsNullOrEmpty(model.PendingOrderFbss_Flag)
                        ? GetCheckPendingOrderFbssFlag(model.CoveragePanelModel.P_MOBILE, url)
                        : model.PendingOrderFbss_Flag;
                }

                return View("SearchProfiles/New_Main");
            }
            else
            {
                List<LovValueModel> config = base.LovData.Where(l => l.Name == "MAPPING_ACC_CATEGORY" && l.Type == "FBB_CONSTANT" && l.LovValue1 == model.outAccountCategory).ToList();
                if (config.Any())
                {
                    model.CATEGORY = config.Select(i => i.LovValue2).FirstOrDefault();
                }
                else
                {
                    model.CATEGORY = "";
                }

                ViewBag.LabelFBBTR016 = GetChangePromotionScreenConfig();
                ViewBag.LabelFBBOR016 = GetChangePromotionPageCode();
                ViewBag.FbbConstant = GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);
                ViewBag.LabelLovScreen = GetScreenConfig("ALLPAGE");
                ViewBag.OverruleDropdown = GetDropDownConfig("OVERRULE");

                Session["FullUrl"] = this.Url.Action("ChangePackagePromotion", "Process", null, this.Request.Url.Scheme);

                if (Data == "Officer")
                {
                    ViewBag.isOfficer = true;
                }

                //20.7 change promotion - change device
                var url = this.Url.Action("ChangePackagePromotion", "Process", null, this.Request.Url.Scheme);

                if (model != null)
                {
                    model.PendingOrderFbss_Change_Promotion_Flag = string.IsNullOrEmpty(model.PendingOrderFbss_Flag)
                        ? GetCheckPendingOrderFbssFlag(model.CoveragePanelModel.P_MOBILE, url)
                        : model.PendingOrderFbss_Flag;
                }

                return View("ChangePromotion/Main", model);
            }
        }

        public JsonResult Check_DubTriple(string mobileNo)
        {
            try
            {
                var query = new GetTriplePlayDupQuery()
                {
                    MobileNo = mobileNo
                };

                var result = _queryProcessor.Execute(query);

                if (result == "True")
                {
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error when call Check_DubTriple in ProcessController : " + ex.InnerException);
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult Check_pre_postpaid(string msisdn, string orderRef, string cond = "")
        {
            bool returnValue = false;
            try
            {
                var query = new GetAisMobileServiceQuery()
                {
                    Msisdn = msisdn.ToSafeString(),
                    Opt1 = "",
                    Opt2 = "",
                    OrderDesc = "query sub",
                    OrderRef = orderRef,
                    User = "TriplePlay",
                    UserName = "FBB"
                };

                var result = _queryProcessor.Execute(query);

                if (cond == "dbspeed")
                {
                    return Json(orderRef, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    if (result.IsSuccess != null && result.Chm != null)
                    {
                        if (result.IsSuccess.ToUpper() == "TRUE" && result.Chm == "0")
                            returnValue = true;
                        else
                            returnValue = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error when call Check_pre_postpaid in ProcessController : " + ex.InnerException);
            }

            return Json(returnValue, JsonRequestBehavior.AllowGet);
        }

        public JsonResult WatchlistConfirmOneTimePW(string pwd, string transactionID)
        {
            var result = new GssoSsoResponseModel();

            if (null == Session["CONTRACTMOBILENO"])
            {
                return null;
            }

            try
            {
                var query = new ConfirmOneTimePWQuery()
                {
                    msisdn = Session["CONTRACTMOBILENO"] as string,
                    pwd = pwd,
                    transactionID = transactionID
                };
                result = new GssoSsoResponseModel();
                result = _queryProcessor.Execute(query);
            }
            catch (Exception ex)
            {
                return null;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ConfirmOneTimePW(string msisdn, string pwd, string transactionID)
        {
            //send OTP
            GssoSsoResponseModel result = new GssoSsoResponseModel();
            try
            {
                var query = new ConfirmOneTimePWQuery()
                {
                    msisdn = msisdn,
                    pwd = pwd,
                    transactionID = transactionID
                };
                result = new GssoSsoResponseModel();
                result = _queryProcessor.Execute(query);
            }
            catch (Exception ex)
            {
                return null;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CreateSignature()
        {
            return View();
        }

        public JsonResult getWatermark()
        {
            var watermarklov = base.LovData.Where(l => !string.IsNullOrEmpty(l.Name) && l.Name.Equals("L_WATERMARK"));
            var watermarktxt = string.Empty;
            if (SiteSession.CurrentUICulture.IsThaiCulture())
            { watermarktxt = watermarklov.FirstOrDefault().LovValue1; }
            else
            { watermarktxt = watermarklov.FirstOrDefault().LovValue2; }

            string txtdata = string.Empty;
            txtdata = Newtonsoft.Json.JsonConvert.SerializeObject(watermarktxt);
            return Json(txtdata, JsonRequestBehavior.AllowGet);
        }

        public JsonResult deleteSignature_flag(string status)
        {
            Session["deleteSignature_flag"] = "N";
            if (status == "Y")
            {
                Session["deleteSignature_flag"] = status;
            }

            return Json(new { result = Session["deleteSignature_flag"], }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Download()
        {

            string filename = "";
            if (Session["FILENAME"] != null)
            {
                filename = (string)Session["FILENAME"];
            }
            string fullPath = Configurations.UploadFileTempPath + "\\" + filename + ".pdf";

            byte[] bytes = null;
            if (Session["PFDBYTE"] != null)
            {
                bytes = (byte[])Session["PFDBYTE"];
                //bytes = new PdfSecurity().PdfSettingPassword(bytes, "Password#111"); //AWARE NEW PDF PASSWORD
            }

            return File(bytes, "application/pdf", filename + ".pdf");
        }

        public JsonResult GetPDF()
        {
            string filename = "";
            if (Session["FILENAME"] != null)
            {
                filename = (string)Session["FILENAME"];
            }

            string fullPath = Configurations.UploadFileTempPath + "\\" + filename + ".pdf";

            byte[] bytes = null;
            if (Session["PFDBYTE"] != null)
            {
                bytes = (byte[])Session["PFDBYTE"];
            }
            if (bytes != null && bytes.Length > 0)
            {

                string base64String = Convert.ToBase64String(bytes, 0, bytes.Length);

                var jsonResult = Json("data:application/pdf;base64," + base64String, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = Int32.MaxValue;
                return jsonResult;
            }
            else
            {
                Logger.Info("Session['PFDBYTE'] is NULL");
                return null;
            }

        }

        public ActionResult Engineers(bool SaveSuccess = false, string LSAVE = "", string LCLOSE = "",
          string LPOPUPSAVE = "", string LanguagePage = "", string SWiFi = "")
        {
            if (null == base.CurrentUser)
                //if ((null == base.CurrentUser) || (Session["userName"] == null))
                return RedirectToAction("Login", "Process");

            InterfaceLogCommand log = null;
            //log = StartInterface(EmpName + " " + EmpLastName + ", " + EmpID, "/process/Engineer", TransactionID, EmpID, "Engineer");

            //EndInterface("", log, TransactionID, "Success", "");

            ViewBag.TopUp = "7";
            Session["EnginerrLine"] = "7";
            Session["OfficerModel"] = null;
            Session["StaffModel"] = null;
            Session["TripleLine"] = null;
            Session["ProcessLine1"] = null;
            if (Session["EndProcessFlag"].ToSafeBoolean())
            {
                Session["PopupStatus"] = "Success";
                Session["EndProcessFlag"] = null;
            }
            else
                Session["PopupStatus"] = null;

            if (Session["isSaleManager"].ToSafeString() == "true")
            {
                ViewBag.isSaleManager = "true";
            }
            else
            {
                ViewBag.isSaleManager = "false";
            }

            string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress))
            {
                ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
            SaveStatlog("Engineer", "Engineer", ipAddress, "FBB REGISTER", "", "");

            var model = new QuickWinPanelModel();
            ViewBag.LabelFBBTR000 = GetGeneralScreenConfig();
            ViewBag.LabelFBBTR001 = GetCoverageScreenConfig();
            ViewBag.LabelFBBTR002 = GetDisplayPackageScreenConfig();
            ViewBag.LabelFBBTR003 = GetCustRegisterScreenConfig();
            ViewBag.LabelFBBTR004 = GetSummaryScreenConfig();
            ViewBag.LabelFBBTR010 = GetVas_Select_Package_ScreenConfig();
            ViewBag.LabelFBBORV11 = GetVasPopUpScreenConfig();
            ViewBag.FbbConstant = GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);
            ViewBag.FbbException = GetFbbConstantModel(WebConstants.LovConfigName.FbbExceptionIdCard);
            ViewBag.Fbb_Service = GetDisplay_Select_Type_Service();
            ViewBag.Version = GetVersion();
            ViewBag.Vas = "";
            ViewBag.User = base.CurrentUser;

            ViewBag.LSAVE = LSAVE;
            ViewBag.SWiFi = SWiFi;
            ViewBag.LanguagePage = LanguagePage;
            ViewBag.LPOPUPSAVE = LPOPUPSAVE;
            ViewBag.LCLOSE = LCLOSE;
            ViewBag.SaveSuccess = SaveSuccess;
            model.TopUp = "7";
            model.SummaryPanelModel.TOPUP = "7";

            ViewBag.ContentPlaybox = GetLovConfigBytype("NEW_CONTENT_PLAYBOX");

            return View("Index", model);
        }

        [HttpPost]
        public JsonResult GetContractMobileNo(string mobileNo, string cardNo = "", string cardType = "")
        {
            string FullUrl = "";
            if (Session["FullUrl"] != null)
                FullUrl = Session["FullUrl"].ToSafeString();

            #region Get IP Address Interface Log (Update 17.2)

            // Get IP Address
            string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress)) ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            #endregion

            string user = "Customer";
            if (cardNo == "officer")  /// case officer
            {
                user = cardType;
                cardNo = "";
            }
            if (base.CurrentUser != null) user = base.CurrentUser.UserName;

            var contractMobileNo = "";
            Session["CONTRACTMOBILENO"] = null;

            if ((!String.IsNullOrEmpty(cardNo) && !String.IsNullOrEmpty(cardType)) || user != "Customer")
            {
                bool haveProfile = false;
                if (user == "Customer")
                {
                    try
                    {
                        var query = new evESeServiceQueryMassCommonAccountInfoQuery
                        {
                            inOption = "2",
                            inMobileNo = mobileNo,
                            inCardNo = cardNo,
                            inCardType = cardType,
                            Page = "Change Package Promotion",
                            Username = "USER",
                            ClientIP = ipAddress,
                            FullUrl = FullUrl
                        };
                        var massCommon = _queryProcessor.Execute(query);
                        if (massCommon.errorMessage == "")
                        {
                            haveProfile = true;
                        }

                    }
                    catch (Exception ex)
                    {
                        base.Logger.Info(ex.GetErrorMessage());
                        return Json(new { data = contractMobileNo }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    haveProfile = true;
                }
                if (haveProfile)
                {
                    try
                    {
                        var query = new evESeServiceQueryMassCommonAccountInfoQuery
                        {
                            inOption = "4",
                            inMobileNo = mobileNo,
                            Page = "Change Package Promotion",
                            Username = "USER",
                            ClientIP = ipAddress,
                            FullUrl = FullUrl
                        };
                        var massCommon = _queryProcessor.Execute(query);

                        contractMobileNo = massCommon.outServiceMobileNo;

                        Session["CONTRACTMOBILENO"] = contractMobileNo;

                        return Json(new { data = contractMobileNo.Remove(0, 6).Insert(0, "XXXXXX"), contractMobileNo = contractMobileNo }, JsonRequestBehavior.AllowGet);
                    }
                    catch (Exception ex)
                    {
                        base.Logger.Info(ex.GetErrorMessage());
                    }

                    return Json(new { data = contractMobileNo }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { data = contractMobileNo }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { data = contractMobileNo }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult evESeServiceQueryMassCommonAccountInfo(string mobileNo = "", string SubNetworkType = "", string idCardNo = "", string idCardType = "")
        {
            string FullUrl = "";
            if (Session["FullUrl"] != null)
                FullUrl = Session["FullUrl"].ToSafeString();

            string user = "Customer";
            if (idCardNo == "officer" || SubNetworkType == "PREPAID")  /// case officer
            {
                user = idCardType;
                idCardNo = "";
            }

            if (base.CurrentUser != null) user = base.CurrentUser.UserName;

            mobileNo = DecryptStringAES(mobileNo, "fbbwebABCDEFGHIJ");

            if ((idCardNo != "" && idCardType != "") || user != "Customer")
            {
                bool haveProfile = false;
                if (user == "Customer")
                {
                    try
                    {
                        var query = new evESeServiceQueryMassCommonAccountInfoQuery
                        {
                            inOption = "2",
                            inMobileNo = mobileNo,
                            inCardNo = idCardNo,
                            inCardType = idCardType,
                            Page = "Process",
                            Username = user,
                            FullUrl = FullUrl
                        };
                        var massCommon = _queryProcessor.Execute(query);
                        if (massCommon.errorMessage == "")
                        {
                            haveProfile = true;
                        }

                    }
                    catch (Exception ex)
                    {
                        return Json(
                            new
                            {
                                data = new
                                {
                                    GUIDKEY = WBBSECURE.WBBEncrypt.textEncrpyt("XXX"),
                                    errorMessage = "No Profile"
                                }
                            }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    haveProfile = true;
                }
                if (haveProfile)
                {

                    #region Get IP Address Interface Log (Update 17.2)

                    // Get IP Address
                    string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                    if (string.IsNullOrEmpty(ipAddress)) ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                    #endregion

                    string InOption = "";
                    if (SubNetworkType == "PREPAID")
                    {
                        InOption = "4";
                    }
                    else
                    {
                        InOption = "2";
                    }

                    var query = new evESeServiceQueryMassCommonAccountInfoQuery
                    {
                        //inOption = "1",
                        inOption = InOption,
                        inMobileNo = mobileNo,
                        inCardNo = idCardNo,
                        inCardType = idCardType,
                        Page = "Check SearchProfilePrePostpaid",
                        Username = user,
                        ClientIP = ipAddress,
                        FullUrl = FullUrl
                    };
                    var a = _queryProcessor.Execute(query);
                    if (a.outMobileSegment != null)
                    {
                        a.outMobileSegment = a.outMobileSegment.ToUpper();
                    }

                    a.GUIDKEY = Guid.NewGuid().ToSafeString();
                    Session["PROCESS_SFFDATA"] = a;

                    return Json(
                        new
                        {
                            data = new
                            {
                                GUIDKEY = WBBSECURE.WBBEncrypt.textEncrpyt(a.GUIDKEY),
                                errorMessage = a.errorMessage
                            }
                        }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(
                        new
                        {
                            data = new
                            {
                                GUIDKEY = WBBSECURE.WBBEncrypt.textEncrpyt("XXX"),
                                errorMessage = "No Profile"
                            }
                        }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(
                new
                {
                    data = new
                    {
                        errorMessage = "EB0001"
                    }
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [AuthorizeUserAttribute]
        public ActionResult FixedlineDetail()
        {
            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "AccountService");

            ViewBag.User = base.CurrentUser;

            ViewBag.LabelFBBTR000 = GetGeneralScreenConfig();
            ViewBag.LabelFBBTR003 = GetCustRegisterScreenConfig();
            ViewBag.LabelFBBTR010 = GetVas_Select_Package_ScreenConfig();
            ViewBag.LabelFBBTR002 = GetDisplayPackageScreenConfig();
            ViewBag.FbbConstant = GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);
            return View("_TotalVas");
        }

        public ActionResult get_deleteSignature_flag()
        {
            if (null == Session["deleteSignature_flag"])
            {
                Session["deleteSignature_flag"] = "N";
            }
            return Json(new { result = Session["deleteSignature_flag"], }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult get_signature_flag()
        {
            if (null == Session["signature_flag"])
            {
                Session["signature_flag"] = "N";
            }
            if (null == Session["pic64signature"])
            {
                Session["pic64signature"] = "";
            }

            return Json(new { result = Session["signature_flag"], signature64result = Session["pic64signature"] }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult get_usedData_flag()
        {
            if (null == Session["usedData_flag"])
            {
                Session["usedData_flag"] = "N";
            }
            return Json(new { result = Session["usedData_flag"], }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetCheckProcess(string key)
        {
            var decryptedGuidKey = WBBSECURE.WBBDecrypt.textDecrpyt(key);
            var sffData =
                Session["PROCESS_SFFDATA"] as WBBEntity.PanelModels.ExWebServiceModels.evESeServiceQueryMassCommonAccountInfoModel;

            if (sffData.GUIDKEY == decryptedGuidKey)
            {
                return Json(
                    new
                    {
                        data = new
                        {
                            projectName = sffData.projectName,
                            vataddTripleplay = sffData.vataddTripleplay,
                            outMobileSegment = sffData.outMobileSegment,
                            outAccountCategory = sffData.outAccountCategory,
                            outAccountNumber = sffData.outAccountNumber,
                            outserviceMobileNo = sffData.outServiceMobileNo,
                            outDayOfServiceYear = sffData.outDayOfServiceYear,
                            outServiceLevel = sffData.outServiceLevel,
                            outPaGroup = sffData.outPaGroup
                        }
                    }, JsonRequestBehavior.AllowGet);
            }
            return Json(
                   new
                   {
                       data = new
                       {
                           errorMessage = "ERROR"
                       }
                   }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetOtherInformation(string mobileno, string caid, string baid, string selectvalue)
        {
            string FullUrl = Session["FullUrl"] != null ? Session["FullUrl"].ToSafeString() : "";
            string Page = string.IsNullOrEmpty(FullUrl) ? "" : FullUrl.Split('/')[FullUrl.Split('/').Count() - 1].ToSafeString();
            // Get IP Address
            string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress)) ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            string username = ViewBag.User;
            // Update R18.11 GetContact Mobile Call Service SFF evESeServiceQueryMassCommonAccountInfo
            string servicemobileno = "";
            if (selectvalue == "CONTACT")
            {
                var resultMassCommon = new evESeServiceQueryMassCommonAccountInfoQuery
                {
                    inOption = "4",
                    inMobileNo = mobileno.ToSafeString(),
                    Page = Page.ToSafeString(),
                    Username = username.ToSafeString(),
                    ClientIP = ipAddress.ToSafeString(),
                    FullUrl = FullUrl.ToSafeString()
                };
                var massCommon = _queryProcessor.Execute(resultMassCommon);
                servicemobileno = massCommon.outServiceMobileNo;
            }
            else
            {
                servicemobileno = "";
            }

            var query = new GetOtherInfo
            {
                mobileno = mobileno,
                caid = caid,
                baid = baid,
                selectvalue = selectvalue,
                servicemobileno = servicemobileno.ToSafeString()
            };

            var data = _queryProcessor.Execute(query);
            return Json(data, JsonRequestBehavior.AllowGet);
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

        [HttpPost]
        public JsonResult GetPostalCodeJson(string outPostalCode, string outTumbol, string outAmphur, string outProvince)
        {
            var query = new GetoutPostalCodeQuery
            {
                outPostalCode = outPostalCode,
                outTumbol = outTumbol,
                outAmphur = outAmphur,
                outProvince = outProvince
            };

            var data = _queryProcessor.Execute(query);

            return Json(new { result = data.ToSafeString() }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetProcessAisAirnets(string key)
        {
            var decryptedGuidKey = WBBSECURE.WBBDecrypt.textDecrpyt(key);

            var sffData =
                Session["PROCESS_SFFDATA"] as WBBEntity.PanelModels.ExWebServiceModels.evESeServiceQueryMassCommonAccountInfoModel;
            if (sffData.GUIDKEY == decryptedGuidKey)
            {
                return Json(
                    new
                    {
                        data = new
                        {
                            outMobileNumber = sffData.outMobileNumber,
                            projectName = sffData.projectName,
                            checkPlayBox = sffData.checkPlayBox,
                            outPrimaryContactFirstName = sffData.outPrimaryContactFirstName,
                            outContactLastName = sffData.outContactLastName,
                            outAmphur = sffData.outAmphur,
                            outBuildingName = sffData.outBuildingName,
                            outFloor = sffData.outFloor,
                            outHouseNumber = sffData.outHouseNumber,
                            outMoo = sffData.outMoo,
                            outMooban = sffData.outMooban,
                            outProvince = sffData.outProvince,
                            outRoom = sffData.outRoom,
                            outSoi = sffData.outSoi,
                            outStreetName = sffData.outStreetName,
                            outBillLanguage = sffData.outBillLanguage,
                            outTumbol = sffData.outTumbol,
                            outBirthDate = sffData.outBirthDate,
                            outEmail = sffData.outEmail,
                            outparameter2 = sffData.outparameter2,
                            outAccountCategory = sffData.outAccountCategory,
                            outAccountName = sffData.outAccountName,
                            outAccountNumber = sffData.outAccountNumber,
                            outServiceAccountNumber = sffData.outServiceAccountNumber,
                            outBillingAccountNumber = sffData.outBillingAccountNumber,
                            outProductName = sffData.outProductName,
                            outDayOfServiceYear = sffData.outDayOfServiceYear,
                            cardType = sffData.cardType,
                            outAccountSubCategory = sffData.outAccountSubCategory,
                            outPostalCode = sffData.outPostalCode,
                            outTitle = sffData.outTitle,
                            SffProfileLogID = sffData.SffProfileLogID,
                            OwnerProduct = sffData.OwnerProduct,
                            PackageCode = sffData.PackageCode,
                            outFullAddress = sffData.outFullAddress,
                            vatAddress1 = sffData.vatAddress1,
                            vatAddress2 = sffData.vatAddress2,
                            vatAddress3 = sffData.vatAddress3,
                            vatAddress4 = sffData.vatAddress4,
                            vatAddress5 = sffData.vatAddress5,
                            vatPostalCd = sffData.vatPostalCd,
                            vatAddressFull = sffData.vatAddressFull,
                        }
                    }, JsonRequestBehavior.AllowGet);
            }
            return Json(
                   new
                   {
                       data = new
                       {
                           errorMessage = "ERROR"
                       }
                   }, JsonRequestBehavior.AllowGet);
        }

        public List<LovScreenValueModel> GetProfilePrePostPaid()
        {
            var screenData = GetScreenConfig(WebConstants.LovConfigName.CheckPrePostPaid);
            return screenData;
        }

        private HttpPostedFileBase[] filesPostedRegisterTempStep;

        [HttpGet]
        [ActionName("Index")]
        public ActionResult Index()
        {
            var bc = Request.Browser;
            if (TempData["ProcessLineOffier"] != null) //Set data from Fbbsaleportal/ProcessLineOfficer_IM for bypass order of IM.
            {
                LeaveMessageDataModel data = TempData["ProcessLineOffier"] as LeaveMessageDataModel;
                TempData["ProcessLineOffier"] = null;

                return ProcessLineOfficer(LcCode: data.LC_CODE, outType: data.OUT_TYPE, outSubType: data.OUT_SUBTYPE, leaveMessageDataModel: data);
            }

            if (Session["OfficerModel"] == null)
            {
                Session["FullUrl"] = this.Url.Action("Index", "Process", null, this.Request.Url.Scheme);
            }

            return SearchProfileIndex(model: null);
        }

        [HttpGet]
        [ActionName("IndexWithModel")]
        public ActionResult Index(string model)
        {
            ViewBag.LabelFBBTR001 = TempData["LabelFBBTR001"];
            ViewBag.LabelFBBTR002 = TempData["LabelFBBTR002"];
            ViewBag.LabelFBBTR003 = TempData["LabelFBBTR003"];
            ViewBag.LabelFBBTR004 = TempData["LabelFBBTR004"];
            ViewBag.LabelFBBOR015 = TempData["LabelFBBOR015"];
            ViewBag.FbbConstant = TempData["FbbConstant"];
            ViewBag.officer = TempData["officer"];
            ViewBag.TopUp = TempData["TopUp"];
            ViewBag.SaveSuccess = TempData["SaveSuccess"];
            ViewBag.SWifi = TempData["SWifi"];
            ViewBag.LanguagePage = TempData["LanguagePage"];
            ViewBag.LCLOSE = TempData["LCLOSE"];
            ViewBag.LSAVE = TempData["LSAVE"];
            ViewBag.LPOPUPSAVE = TempData["LPOPUPSAVE"];
            ViewBag.officeEnd = TempData["officeEnd"];
            ViewBag.ESRIflag = TempData["ESRIflag"];
            ViewBag.Vas = TempData["Vas"];
            ViewBag.LabelFBBORV25 = TempData["LabelFBBORV25"];
            //Chist699 release port 3bb
            ViewBag.popupMessageErrorView = (string)Session["popupMessageErrorText"];
            var quickwinModel = TempData["QuickWinPanelModel"] as QuickWinPanelModel;
            return SearchProfileIndex(model: quickwinModel);
        }

        [HttpPost]
        [AjaxValidateAntiForgeryTokenAttribute]
        public ActionResult Index(QuickWinPanelModel model, HttpPostedFileBase[] files)
        {
            try
            {
                //log model
                try
                {
                    Logger.Info(string.Format("MOBILE = {0} : Index Data {1}", model.CustomerRegisterPanelModel.L_MOBILE, model.DumpToXml()));
                }
                catch (Exception ex1)
                {
                    Logger.Info(string.Format("MOBILE = {0} : Exception {1}", model.CustomerRegisterPanelModel.L_MOBILE, ex1.Message + ex1.StackTrace));
                }

                //check dup OrderSubmit
                if (model.OrderSubLine.ToSafeString() == "RegisterSubmit")
                {
                    if (null == Session[model.OrderSubID])
                    {
                        Session[model.OrderSubID] = true;
                    }
                    else
                    {
                        var lovRegisDup = base.LovData.Where(l => !string.IsNullOrEmpty(l.Name) && l.Name.Equals("L_DUPLICATE_MESSAGE"));
                        var lovRegisDupTxt = string.Empty;
                        if (SiteSession.CurrentUICulture.IsThaiCulture())
                        { lovRegisDupTxt = lovRegisDup.FirstOrDefault().LovValue1; }
                        else
                        { lovRegisDupTxt = lovRegisDup.FirstOrDefault().LovValue2; }

                        TempData["isWTTx"] = true;
                        TempData["WTTxErrorMSG"] = lovRegisDupTxt;
                        return RedirectToAction("IndexWithModel", new { model = "" });
                    }
                }

                System.Web.HttpCookie cookie = HttpContext.Request.Cookies["passVerify"];
                if (cookie != null && Boolean.Parse(cookie.Value.ToString()))
                {
                    cookie.Value = "False";
                    HttpContext.Response.SetCookie(cookie);
                }

                var httpCookie = Request.Cookies["ASP.NET_SessionId"];
                var cookieSessionID = httpCookie != null ? httpCookie.Value : Session.SessionID;

                #region Get IP Address (Update 17.2)

                string ClientIP = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (string.IsNullOrEmpty(ClientIP))
                {
                    ClientIP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                }

                string transactionId = model.CoveragePanelModel.P_MOBILE + ClientIP;
                model.TransactionID = transactionId;
                model.ClientIP = ClientIP;

                #endregion

                InterfaceLogCommand interfacelog = null;
                interfacelog = StartInterface("", "ProcessSubmit", transactionId, model.IDCardNo, "ProcessSubmit");

                EndInterface("", interfacelog, transactionId, "Success", "");

                List<FbbConstantModel> fbbConstance = GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);

                #region Line4

                if (model.SummaryPanelModel.VAS_FLAG.ToSafeString() == "4")
                {
                    ViewBag.SaveSuccess = true;
                    SetLovValueToViewBag(model);
                    Session["EndProcessFlag"] = false;
                    foreach (var a in model.SummaryPanelModel.PackageModelList)
                    {
                        var CheckChangePromotionModelLine4 = new CheckChangePromotionModelLine4();
                        CheckChangePromotionModelLine4 = GetCheckChangePromotionLine4(model.CoveragePanelModel.P_MOBILE.ToSafeString(),
                                                                                        "CHECK", a.SFF_PROMOTION_CODE.ToSafeString(), "WEB");

                        if (CheckChangePromotionModelLine4.returnCode != "001" && CheckChangePromotionModelLine4.returnCode != "003" && CheckChangePromotionModelLine4.existFlag == "Y")
                        {

                            model.SessionId = cookieSessionID;
                            var customerRowID1 = RegisterCustomer(model,
                                CheckChangePromotionModelLine4.returnCode.ToSafeString(),
                                "ERROR:" + a.PACKAGE_NAME,
                                "", ClientIP);

                            ViewBag.SWiFi = "fail";

                            return RedirectToAction(GetRedirectTopupPage("TopupPlaybox", model.ExistingFlag), new
                            {
                                SaveSuccess = ViewBag.SaveSuccess,
                                LSAVE = ViewBag.LSAVE,
                                LCLOSE = ViewBag.LCLOSE,
                                LPOPUPSAVE = ViewBag.LPOPUPSAVE,
                                LanguagePage = ViewBag.LanguagePage,
                                SWiFi = ViewBag.SWiFi,
                                DUP = "Y",
                                ExistingPopupFlag = GetExistingTopupFlag(model.ExistingFlag)
                            });
                        }
                        else if (CheckChangePromotionModelLine4.returnCode != "001" && CheckChangePromotionModelLine4.returnCode != "003")
                        {
                            model.SessionId = cookieSessionID;
                            var customerRowID1 = RegisterCustomer(model,
                                CheckChangePromotionModelLine4.returnCode.ToSafeString(),
                                "ERROR:" + a.PACKAGE_NAME,
                                "", ClientIP);

                            ViewBag.SWiFi = "fail";
                            return RedirectToAction(GetRedirectTopupPage("TopupPlaybox", model.ExistingFlag), new
                            {
                                SaveSuccess = ViewBag.SaveSuccess,
                                LSAVE = ViewBag.LSAVE,
                                LCLOSE = ViewBag.LCLOSE,
                                LPOPUPSAVE = ViewBag.LPOPUPSAVE,
                                LanguagePage = ViewBag.LanguagePage,
                                SWiFi = ViewBag.SWiFi,
                                DUP = "",
                                ExistingPopupFlag = GetExistingTopupFlag(model.ExistingFlag)
                            });
                        }
                    }
                    var sumcode = "";
                    for (var i = 0; i < model.SummaryPanelModel.PackageModelList.Count; i++)
                    {
                        sumcode = sumcode + model.SummaryPanelModel.PackageModelList[i].SFF_PROMOTION_CODE.ToSafeString();
                        if (i < model.SummaryPanelModel.PackageModelList.Count - 1)
                        {
                            sumcode = sumcode + ",";
                            continue;
                        }
                        else
                        {
                            string promotionCdContentTMP = model.PruductCD_Content != null ? model.PruductCD_Content : "";
                            var ConfirmChangePromotionModel = new ConfirmChangePromotionModelLine4();
                            ConfirmChangePromotionModel = ConfirmChangePromotionModelLine4(model.CoveragePanelModel.P_MOBILE.ToSafeString()
                                                                                          , sumcode, promotionCdContentTMP
                                                                                          , model.CustomerRegisterPanelModel.L_LOC_CODE
                                                                                          , model.CustomerRegisterPanelModel.L_ASC_CODE
                                                                                          , model.CustomerRegisterPanelModel.L_STAFF_ID
                                                                                          , model.CustomerRegisterPanelModel.EMP_NAME);
                            if (ConfirmChangePromotionModel.SuccessFlag != "Y")
                            {
                                model.SessionId = cookieSessionID;
                                var customerRowID3 = RegisterCustomer(model,
                                ConfirmChangePromotionModel.SuccessFlag.ToSafeString(),
                                ConfirmChangePromotionModel.ReturnMessage,
                                ConfirmChangePromotionModel.ReturnCode,
                                ClientIP);
                                var namepac = "";

                                if (ConfirmChangePromotionModel.ReturnMessage.Contains("EB0216")
                                || ConfirmChangePromotionModel.ReturnMessage.Contains("Playbox Old Launcher"))
                                {
                                    ViewBag.NamePackageFail = "old";
                                    ViewBag.SWiFi = "fail";
                                }
                                else
                                {
                                    //if (i == model.SummaryPanelModel.PackageModelList.Count - 1) { namepac = model.SummaryPanelModel.PackageModelList[i].PACKAGE_NAME; }
                                    ViewBag.NamePackageFail = namepac;
                                    ViewBag.SWiFi = "fail";
                                }

                                return RedirectToAction(GetRedirectTopupPage("TopupPlaybox", model.ExistingFlag), new
                                {
                                    PACKAGE_NAME = ViewBag.NamePackageFail,
                                    SaveSuccess = ViewBag.SaveSuccess,
                                    LSAVE = ViewBag.LSAVE,
                                    LCLOSE = ViewBag.LCLOSE,
                                    LPOPUPSAVE = ViewBag.LPOPUPSAVE,
                                    LanguagePage = ViewBag.LanguagePage,
                                    SWiFi = ViewBag.SWiFi,
                                    DUP = "",
                                    ExistingPopupFlag = GetExistingTopupFlag(model.ExistingFlag)
                                });
                            }
                            else
                            {
                                if (i == model.SummaryPanelModel.PackageModelList.Count - 1)
                                {
                                    model.SessionId = cookieSessionID;
                                    var customerRowID2 = RegisterCustomer(model,
                                    ConfirmChangePromotionModel.SuccessFlag.ToSafeString(),
                                    ConfirmChangePromotionModel.ReturnMessage,
                                    ConfirmChangePromotionModel.ReturnCode,
                                    ClientIP);

                                    ViewBag.SWiFi = "success";
                                    return RedirectToAction(GetRedirectTopupPage("TopupPlaybox", model.ExistingFlag), new
                                    {
                                        SaveSuccess = ViewBag.SaveSuccess,
                                        LSAVE = ViewBag.LSAVE,
                                        LCLOSE = ViewBag.LCLOSE,
                                        LPOPUPSAVE = ViewBag.LPOPUPSAVE,
                                        LanguagePage = ViewBag.LanguagePage,
                                        SWiFi = ViewBag.SWiFi,
                                        DUP = "",
                                        ExistingPopupFlag = GetExistingTopupFlag(model.ExistingFlag)
                                    });
                                }
                            }
                        }
                    }
                }

                #endregion Line4
                if (!string.IsNullOrEmpty(model.CustomerRegisterPanelModel.DEVELOPER))
                {
                    string[] arrDev = model.CustomerRegisterPanelModel.DEVELOPER.Split(':');
                    model.CustomerRegisterPanelModel.p_dev_project_code = arrDev[0].Trim();
                    model.CustomerRegisterPanelModel.p_dev_bill_to = arrDev[1].Trim();
                }

                string pdfSignatureBase64 = model.SignaturePDF;
                model.SignaturePDF = "";

                string pdfSignatureBase64_2 = model.SignaturePDF2;
                model.SignaturePDF2 = "";

                Logger.Info("PDF SIGNATURE1");
                Logger.Info(pdfSignatureBase64);

                Session["SIGNATURE1"] = pdfSignatureBase64;
                Session["SIGNATURE2"] = pdfSignatureBase64_2;

                var customerRowID = "";
                int referenceNoStatus;
                string orderNo = "";

                try
                {
                    ViewBag.L_SAVE = false;
                    ViewBag.FileName = "";
                    ViewBag.FullName = "";
                    ViewBag.SWiFi = model.CoveragePanelModel.PRODUCT_SUBTYPE;

                    if (SiteSession.CurrentUICulture.IsThaiCulture())
                    { ViewBag.LanguagePage = "1"; model.SummaryPanelModel.PDFPackageModel.PDF_L_UNIT = "1"; }
                    else
                    { ViewBag.LanguagePage = "2"; model.SummaryPanelModel.PDFPackageModel.PDF_L_UNIT = "2"; }

                    var data = base.LovData.Where(l => !string.IsNullOrEmpty(l.Name) && l.Name.Equals("B_CLOSE"));
                    if (SiteSession.CurrentUICulture.IsThaiCulture())
                    { ViewBag.LCLOSE = data.FirstOrDefault().LovValue1; }
                    else
                    { ViewBag.LCLOSE = data.FirstOrDefault().LovValue2; }

                    data = base.LovData.Where(l => !string.IsNullOrEmpty(l.Name) && l.Name.Equals("B_SAVE"));
                    if (SiteSession.CurrentUICulture.IsThaiCulture())
                    { ViewBag.LSAVE = data.FirstOrDefault().LovValue1; }
                    else
                    { ViewBag.LSAVE = data.FirstOrDefault().LovValue2; }

                    data = base.LovData.Where(l => !string.IsNullOrEmpty(l.Name) && l.Name.Equals("L_POPUP_SAVE"));

                    if (SiteSession.CurrentUICulture.IsThaiCulture())
                    { ViewBag.LPOPUPSAVE = HttpUtility.HtmlEncode(data.FirstOrDefault().LovValue1); }
                    else
                    { ViewBag.LPOPUPSAVE = HttpUtility.HtmlEncode(data.FirstOrDefault().LovValue2); }

                    if (model.ForCoverageResult == true)
                    {
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
                    }
                    else
                    {
                        model.CoveragePanelModel.L_RESULT = "Y";
                    }

                    #region Log Model

                    Logger.Info("Coverage Result: " + model.ForCoverageResult);

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

                    var tempzipcode = "";

                    if (model.SummaryPanelModel.VAS_FLAG == "2" || model.SummaryPanelModel.TOPUP == "1")
                    {
                        tempzipcode = model.CustomerRegisterPanelModel.AddressPanelModelVat.L_ZIPCODE;
                        model.CustomerRegisterPanelModel.AddressPanelModelVat.L_ZIPCODE = GetPostalCode(model.CustomerRegisterPanelModel.AddressPanelModelVat.L_ZIPCODE, model.CustomerRegisterPanelModel.AddressPanelModelVat.L_TUMBOL, model.CustomerRegisterPanelModel.AddressPanelModelVat.L_AMPHUR, model.CustomerRegisterPanelModel.AddressPanelModelVat.L_PROVINCE);

                        model.CustomerRegisterPanelModel.AddressPanelModelSendDoc.L_ZIPCODE = GetPostalCode(model.CustomerRegisterPanelModel.AddressPanelModelSendDoc.L_ZIPCODE, model.CustomerRegisterPanelModel.AddressPanelModelSendDoc.L_TUMBOL, model.CustomerRegisterPanelModel.AddressPanelModelSendDoc.L_AMPHUR, model.CustomerRegisterPanelModel.AddressPanelModelSendDoc.L_PROVINCE);

                        model.CustomerRegisterPanelModel.AddressPanelModelSetup.L_ZIPCODE = GetPostalCode(model.CustomerRegisterPanelModel.AddressPanelModelSetup.L_ZIPCODE, model.CustomerRegisterPanelModel.AddressPanelModelSetup.L_TUMBOL, model.CustomerRegisterPanelModel.AddressPanelModelSetup.L_AMPHUR, model.CustomerRegisterPanelModel.AddressPanelModelSetup.L_PROVINCE);
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

                    if (model.CoveragePanelModel.AccessMode == "VDSL")
                    {
                        List<string> SplitterFlag = new List<string>();
                        SplitterFlag.Add("3");
                        SplitterFlag.Add("5");

                        if (SplitterFlag.IndexOf(model.SplitterFlag) != -1 || model.CustomerRegisterPanelModel.FBSSTimeSlot.AppointmentDate == null)
                        {
                            model.CustomerRegisterPanelModel.L_INSTALL_DATE = "";
                            model.CustomerRegisterPanelModel.FBSSTimeSlot.TimeSlot = "";
                            model.CustomerRegisterPanelModel.FBSSTimeSlot.TimeSlotId = "";
                            model.CustomerRegisterPanelModel.FBSSTimeSlot.InstallationCapacity = "";
                            model.CustomerRegisterPanelModel.FBSSTimeSlot.AppointmentDate = null;
                            model.CustomerRegisterPanelModel.FBSSTimeSlot.FirstInstallDate = "";
                            model.CustomerRegisterPanelModel.FBSSTimeSlot.FirstTimeSlot = "";
                        }
                    }

                    //18.6
                    if (model.CustomerRegisterPanelModel.SelectFlagCheckPlugAndPlayFlow == "Y")
                    {
                        model.CustomerRegisterPanelModel.Plug_and_play_flag = "S";
                        model.CustomerRegisterPanelModel.L_EVENT_CODE = "EVENT_PPS";
                    }

                    //20.3 Service Level
                    string serviceLevel_H = fbbConstance.Any(c => c.Field == "SERVICE_LEVEL_H") ? fbbConstance.FirstOrDefault(c => c.Field == "SERVICE_LEVEL_H").Validation : "";
                    if (model.CustomerRegisterPanelModel.ServiceLevel == null)
                        model.CustomerRegisterPanelModel.ServiceLevel = model.CustomerRegisterPanelModel.ServiceLevel.ToSafeString();
                    if (model.CustomerRegisterPanelModel.ServiceLevel_Flag == null)
                        model.CustomerRegisterPanelModel.ServiceLevel_Flag = model.CustomerRegisterPanelModel.ServiceLevel_Flag.ToSafeString();
                    if (model.CustomerRegisterPanelModel.ServiceLevel == serviceLevel_H &&
                        (String.IsNullOrEmpty(model.CustomerRegisterPanelModel.FBSSTimeSlot.TimeSlot) || !model.CustomerRegisterPanelModel.FBSSTimeSlot.AppointmentDate.HasValue))
                    {
                        Logger.Info(model.CoveragePanelModel.P_MOBILE + " : Timeslot or Appointment date is empty.");
                        //EXPECT_INSTALL_DATE
                        if (!model.CustomerRegisterPanelModel.FBSSTimeSlot.AppointmentDate.HasValue)
                        {
                            string strInstallationDate = model.CustomerRegisterPanelModel.FBSSTimeSlot.AppointmentDate_Initial;
                            string strCurrentDate = model.CustomerRegisterPanelModel.FBSSTimeSlot.CurrentDate_Initial;

                            DateTime appointmentDate = new DateTime();
                            if (SiteSession.CurrentUICulture.IsThaiCulture())
                            {
                                DateTimeFormatInfo thDtfi = new CultureInfo("th-TH", false).DateTimeFormat;
                                appointmentDate = Convert.ToDateTime(strInstallationDate, thDtfi);
                            }
                            else
                            {
                                DateTimeFormatInfo usDtfi = new CultureInfo("en-GB", false).DateTimeFormat;
                                appointmentDate = Convert.ToDateTime(strInstallationDate, usDtfi);
                            }

                            if (strInstallationDate == strCurrentDate)
                            {
                                var addHr = model.CustomerRegisterPanelModel.FBSSTimeSlot.TimeSlotRegisterHR.ToSafeInteger() >= 0 ?
                                    model.CustomerRegisterPanelModel.FBSSTimeSlot.TimeSlotRegisterHR.ToSafeInteger() :
                                    24;

                                appointmentDate = appointmentDate.AddHours(addHr);
                            }

                            model.CustomerRegisterPanelModel.FBSSTimeSlot.AppointmentDate = appointmentDate;
                            Logger.Info("Appointdate value is " + appointmentDate.ToDateDisplayText());
                        }
                        //TIMESLOT
                        if (String.IsNullOrEmpty(model.CustomerRegisterPanelModel.FBSSTimeSlot.TimeSlot))
                        {
                            // transform to null safe string commpare.
                            var LovConstant = (from t in base.LovData
                                               where t.Type == "FBB_CONSTANT"
                                               && t.Name == "DEFAULT_TIMESLOT"
                                               && t.Text == model.CoveragePanelModel.AccessMode
                                               select t.LovValue1).ToList();

                            if (LovConstant != null && LovConstant.Count > 0)
                                model.CustomerRegisterPanelModel.FBSSTimeSlot.TimeSlot = LovConstant.FirstOrDefault().ToString();

                            Logger.Info("Timeslot value is " + model.CustomerRegisterPanelModel.FBSSTimeSlot.TimeSlot);
                        }
                        Logger.Info("End set value timeslot and Appointment date.");
                    }
                    //20.5 Non-Res Register Residential
                    if (model.CustomerRegisterPanelModel.Non_Res_Flag == null)
                        model.CustomerRegisterPanelModel.Non_Res_Flag = model.CustomerRegisterPanelModel.Non_Res_Flag.ToSafeString();

                    if (model.CustomerRegisterPanelModel.mesh_count == null)
                        model.CustomerRegisterPanelModel.mesh_count = model.CustomerRegisterPanelModel.mesh_count.ToSafeString();

                    var saveOrderResp = new SaveOrderResp();
                    SaveOrderResp saveOrderVoIpResp = null;
                    if (model.CoveragePanelModel.CoverageResult.ToSafeString().ToLower().Contains("plan"))
                    {
                        saveOrderResp.RETURN_CODE = -1;
                        saveOrderResp.RETURN_MESSAGE = "Check coverage is plan";
                        saveOrderResp.RETURN_IA_NO = "No IA_NO";
                        saveOrderResp.RETURN_ORDER_NO = "No ORDER_NO";
                    }
                    else if (model.SummaryPanelModel.VAS_FLAG.ToSafeString() == "7" && model.SummaryPanelModel.TOPUP.ToSafeString() == "1")
                    {//R22.03 TopupReplace
                        saveOrderResp.RETURN_CODE = -1;
                        saveOrderResp.RETURN_MESSAGE = "Call Service IM is Failed";
                        saveOrderResp.RETURN_IA_NO = "No IA_NO";
                        saveOrderResp.RETURN_ORDER_NO = "No ORDER_NO";

                        GetIMCaseFBBRestServiceModel saveIMTopupReplace = GetSaveIMTopupReplace(model.SummaryPanelModel);

                        if (saveIMTopupReplace.ResultCode == "01")
                        {
                            saveOrderResp.RETURN_CODE = 0;
                            saveOrderResp.RETURN_MESSAGE = saveIMTopupReplace.ResultDesc;
                            saveOrderResp.RETURN_IA_NO = "";
                            saveOrderResp.RETURN_ORDER_NO = saveIMTopupReplace.ResultOrderNo;
                        }
                        else
                        {
                            Logger.Info(saveIMTopupReplace.ResultDesc);
                        }
                    }
                    else
                    {
                        model.SessionId = cookieSessionID;
                        saveOrderResp = GetSaveOrderResp(model);
                        if (saveOrderResp.RETURN_CODE != 0)
                        {
                            Logger.Info(saveOrderResp.RETURN_MESSAGE);
                        }
                        else
                        {
                            // case register Playbox and VOIP
                            if (model.SummaryPanelModel.VAS_FLAG.ToSafeString() == "2" || model.SummaryPanelModel.TOPUP.ToSafeString() == "1")
                            {
                                if (model.SummaryPanelModel.PackageModel.SelectPlayBox_Flag == "1" && model.SummaryPanelModel.PackageModel.SelectVas_Flag == "1")
                                {
                                    model.SummaryPanelModel.VOIP_FLAG = "1";
                                    model.SessionId = cookieSessionID;
                                    saveOrderVoIpResp = GetSaveOrderResp(model);
                                    if (saveOrderVoIpResp.RETURN_CODE != 0)
                                    {
                                        Logger.Info(saveOrderVoIpResp.RETURN_MESSAGE);
                                    }
                                }
                            }

                            // INSERT FBB_CONSENT_LOG
                            // R23.05.2023 Created: THOTST49
                            InsertConsentLog(
                                model.MobileNoRegister,
                                model.CoveragePanelModel.L_CONTACT_PHONE,
                                model.IsCheckConsentTerm,
                                saveOrderResp.RETURN_ORDER_NO,
                                model.ClientIP
                                );
                        }
                    }

                    //chist699 releasePort3bb
                    if (saveOrderResp != null)
                    {
                        if (saveOrderResp.RETURN_CODE == -1)
                        {
                            ReleaseReservePort3bbQuery releaseReservePort3bbQuery = new ReleaseReservePort3bbQuery()
                            {
                                referenceId = model.CoverageAreaResultModel.SPLITTER_3BB_RESERVED_LIST.FirstOrDefault().referenceId
                            };
                            ReleaseReservePort3bbQueryModel reservePort3bbQueryModel = _queryProcessor.Execute(releaseReservePort3bbQuery);
                            var popupMessageError = (from t in base.LovData
                                                     where t.Type == "SCREEN"
                                                     && t.Name == "L_POPUP_CONTACT_US"
                                                     select t.LovValue1).FirstOrDefault();

                            Session["popupMessageErrorText"] = popupMessageError;
                        }
                    }
                    else if (saveOrderVoIpResp != null)
                    {
                        if (saveOrderVoIpResp.RETURN_CODE == -1)
                        {
                            ReleaseReservePort3bbQuery releaseReservePort3bbQuery = new ReleaseReservePort3bbQuery()
                            {
                                referenceId = model.CoverageAreaResultModel.SPLITTER_3BB_RESERVED_LIST.FirstOrDefault().referenceId
                            };
                            ReleaseReservePort3bbQueryModel reservePort3bbQueryModel = _queryProcessor.Execute(releaseReservePort3bbQuery);
                            var popupMessageError = (from t in base.LovData
                                                     where t.Type == "SCREEN"
                                                     && t.Name == "L_POPUP_CONTACT_US"
                                                     select t.LovValue1).FirstOrDefault();

                            Session["popupMessageErrorText"] = popupMessageError;
                        }

                    }


                    if (model.ForCoverageResult == true)
                    {
                        #region Update Coverage Result

                        string statusUpdate = "";
                        var resultId = Decimal.Parse(model.CoveragePanelModel.RESULT_ID.ToSafeString());
                        try
                        {
                            var a = Bootstrapper.GetInstance<CheckCoverageController>();
                            var resultCoverageResult = a.FBSSCoverageResultCommand(
                                actionType: "Update",
                                resultId: resultId,
                                preName: model.CoveragePanelModel.L_FIRST_LAST.ToSafeString(),
                                fName: model.CoveragePanelModel.L_FIRST_NAME.ToSafeString(),
                                lName: model.CoveragePanelModel.L_LAST_NAME.ToSafeString(),
                                contactNo: model.CoveragePanelModel.L_CONTACT_PHONE.ToSafeString(),
                                recode: saveOrderResp.RETURN_CODE,
                                remessage: saveOrderResp.RETURN_MESSAGE.ToSafeString(),
                                reorder: saveOrderResp.RETURN_ORDER_NO.ToSafeString(),
                                email: model.CoveragePanelModel.L_CONTACT_EMAIL.ToSafeString(),
                                lineid: model.CoveragePanelModel.L_CONTACT_LINE_ID.ToSafeString(),
                                lcCode: model.CustomerRegisterPanelModel.L_LOC_CODE.ToSafeString(),
                                ascCode: model.CustomerRegisterPanelModel.L_ASC_CODE.ToSafeString(),
                                employeeID: model.CustomerRegisterPanelModel.L_STAFF_ID.ToSafeString(),
                                saleFirstname: model.OfficerInfoPanelModel.THFirstName.ToSafeString(),
                                saleLastname: model.OfficerInfoPanelModel.THLastName.ToSafeString(),
                                locationName: model.CustomerRegisterPanelModel.PartnerName.ToSafeString(),
                                subRegion: model.OfficerInfoPanelModel.outLocationSubRegion.ToSafeString(),
                                region: model.OfficerInfoPanelModel.outLocationRegion.ToSafeString(),
                                ascName: model.OfficerInfoPanelModel.outASCPartnerName.ToSafeString(),
                                channelName: model.CustomerRegisterPanelModel.PartnerName.ToSafeString(),
                                saleChannel: model.OfficerInfoPanelModel.outChnSales.ToSafeString(),
                                //R21.2
                                addressTypeDTL: model.CoveragePanelModel.ADDRESS_TYPE_DTL.ToSafeString(),
                                remark: model.CoveragePanelModel.REMARK.ToSafeString(),
                                technology: model.CoveragePanelModel.TECHNOLOGY.ToSafeString(),
                                projectName: model.CoveragePanelModel.PROJECTNAME.ToSafeString()
                            );
                            statusUpdate = "0";
                        }
                        catch (Exception ex)
                        {
                            statusUpdate = "-2";
                        }

                        //19.3 : Send to IM Service.
                        if (model.CoveragePanelModel.CoverageMemberGetMember.IS_CHANNEL_IM.ToLower() == "true" && resultId.ToSafeString() != "0")
                        {
                            var s = Bootstrapper.GetInstance<CheckCoverageController>();
                            var resultData = s.FBSSCoverageResultQuery(
                                resultId: resultId,
                                transactionId: "",
                                assetNumber: model.CoveragePanelModel.CoverageMemberGetMember.ASSET_NUMBER,
                                caseId: model.CoveragePanelModel.CoverageMemberGetMember.CASE_ID,
                                referenceNoStatus: "0"
                            );

                            var Command = new GetLeaveMsgReferenceNoCommand();
                            Command = resultData;
                            Command.referenceNoStatus = statusUpdate;
                            _getLeaveMsgRefCommand.Handle(Command);
                        }

                        #endregion Update Coverage Result

                        //17.7 Member get Member //Update status preregister
                        if (model.MemberGetMemberFlag == "Y")
                        {
                            var preRegisterStatus = new UpdatePreregisterStatusPackageCommand
                            {
                                p_refference_no = model.CoveragePanelModel.CoverageMemberGetMember.RefferenceNo,
                                p_status = "Cancel (No Coverage)"
                            };
                            _updatePregisterCommand.Handle(preRegisterStatus);

                            //R22.06 Member get Member Send SMS Case Out of Coverage
                            var LovCampaignProjectNameMGM = GetLovCampaignProjectNameMGM("CAMPAIGN_PROJECT_NAME");
                            if (model.CoveragePanelModel.CoverageResult == "NO" &&
                                model.CoveragePanelModel.CoverageMemberGetMember.CampaignProjectName == LovCampaignProjectNameMGM)
                            {
                                var languageMGM = SiteSession.CurrentUICulture.IsThaiCulture() ? "T" : "E";
                                var fullUrl = this.Url.Action("", "fbbsaleportal", null, this.Request.Url.Scheme);
                                var sendSmsMGM = new SendSmsMGMCommand
                                {
                                    p_refference_no = model.CoveragePanelModel.CoverageMemberGetMember.RefferenceNo,
                                    p_coverage_result = model.CoveragePanelModel.CoverageResult,
                                    p_mgm_flag = model.MemberGetMemberFlag,
                                    p_language = languageMGM,
                                    ClientIP = ClientIP,
                                    FullUrl = fullUrl
                                };
                                _sendSmsMGMCommand.Handle(sendSmsMGM);
                            }
                        }
                    }
                    else
                    {
                        #region set value

                        if (string.IsNullOrEmpty(model.CoveragePanelModel.Address.L_MOOBAN))
                        { model.CoveragePanelModel.Address.L_MOOBAN = "-"; }
                        if (string.IsNullOrEmpty(model.CoveragePanelModel.L_FLOOR_VILLAGE))
                        { model.CoveragePanelModel.L_FLOOR_VILLAGE = "1"; }

                        #endregion set value

                        #region Save register

                        // register customer
                        model.SessionId = cookieSessionID;
                        customerRowID = RegisterCustomer(model,
                            saveOrderResp.RETURN_CODE.ToSafeString(),
                            saveOrderResp.RETURN_MESSAGE,
                            saveOrderResp.RETURN_ORDER_NO,
                            ClientIP);
                        if (model.CustomerRegisterPanelModel.CHECK_FRAUD_INFO.FLAG_GO_NOGO == "GO")
                        {
                            SaveRegisterFraud(model, customerRowID);
                        }
                        model.CustomerRegisterPanelModel.OrderNo = saveOrderResp.RETURN_ORDER_NO;
                        //Register VOIP
                        if (model.SummaryPanelModel.VAS_FLAG.ToSafeString() == "2" ||
                            model.SummaryPanelModel.TOPUP.ToSafeString() == "1")
                        {
                            if (model.SummaryPanelModel.PackageModel.SelectVas_Flag == "1")
                            {
                                if (saveOrderVoIpResp != null && saveOrderVoIpResp.RETURN_CODE == 0)
                                {
                                    model.SessionId = cookieSessionID;
                                    RegisterCustomer(model,
                                     saveOrderVoIpResp.RETURN_CODE.ToSafeString(),
                                     saveOrderVoIpResp.RETURN_MESSAGE,
                                     saveOrderVoIpResp.RETURN_IA_NO,
                                     ClientIP);
                                    model.CustomerRegisterPanelModel.OrderNo = saveOrderResp.RETURN_IA_NO;
                                }
                            }
                        }
                        #endregion Save register

                        #region Insert Log IP Camera 
                        //R23.06 IP Camera
                        try
                        {
                            var packageModelList = new List<PackageModel>();
                            packageModelList = model.SummaryPanelModel.PackageModelList;
                            packageModelList = packageModelList.Where(p => p.PRODUCT_SUBTYPE == "IP_CAMERA").ToList();
                            if (packageModelList != null && packageModelList.Any())
                            {
                                InsertRegisterCloudIPCamera(model, customerRowID, saveOrderResp.RETURN_ORDER_NO);
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Info("Error InsertRegisterCloudIPCamera : " + ex.GetBaseException());
                        }
                        #endregion

                        if (model.SummaryPanelModel.VAS_FLAG == "2" || model.SummaryPanelModel.TOPUP == "1")
                        {
                            model.CustomerRegisterPanelModel.AddressPanelModelVat.L_ZIPCODE = tempzipcode;
                            model.CustomerRegisterPanelModel.AddressPanelModelSendDoc.L_ZIPCODE = tempzipcode;
                            model.CustomerRegisterPanelModel.AddressPanelModelSetup.L_ZIPCODE = tempzipcode;
                        }

                        #region PDF

                        var newregisterFlag = true;
                        var running_no = InsertMailLog(customerRowID);
                        string uploadFileWebPath = Configurations.UploadFilePath;
                        string uploadFileAppPath = Configurations.UploadFileTempPath;

                        model.CustomerRegisterPanelModel.L_INSTALL_DATE = model.CustomerRegisterPanelModel.L_INSTALL_DATE + "  " + model.CustomerRegisterPanelModel.FBSSTimeSlot.TimeSlot;

                        System.IFormatProvider format = new System.Globalization.CultureInfo("en-US");
                        string filename = "Request" + DateTime.Now.ToString("ddMMyy", format) + "_" + running_no.ToSafeString();
                        string directoryPath = "";
                        string directoryPathApp = "";

                        //R22.06 Modify E-App
                        //Logic New
                        var AISsubtype = LovData.Where(item => item.Name == "AIS_SHOP_SUB_TYPE" && item.LovValue5 == "FBBOR004").Select(s => s.LovValue1).ToList();
                        bool isShop = model.TopUp != "5" || AISsubtype.Contains(model.CustomerRegisterPanelModel.outSubType);
                        //Logic Old
                        //var AISsubtype = LovData.FirstOrDefault(item => item.Name == "AIS_SHOP_SUB_TYPE" && item.LovValue5 == "FBBOR004").LovValue1;
                        //bool isShop = model.TopUp != "5" || model.CustomerRegisterPanelModel.outSubType == AISsubtype;

                        string isShopString = isShop ? "Y" : "N";

                        Logger.Info(model.DumpToXml());

                        if ((model.SummaryPanelModel.VAS_FLAG == "1"
                            || model.SummaryPanelModel.VAS_FLAG == "3"
                            || model.SummaryPanelModel.VAS_FLAG == "6"
                            || (model.SummaryPanelModel.VAS_FLAG == "0" && model.SummaryPanelModel.TOPUP != "1"))
                                && model.SummaryPanelModel.PackageModel.SelectVas_Flag == "1"
                                && model.SummaryPanelModel.PackageModel.SelectPlayBox_Flag == "0") /// create new vas and coverage
                        {
                            Logger.Info("PDFFIRST");
                            directoryPath = GeneratePDF_HTML(model, @uploadFileWebPath, @uploadFileAppPath, filename, isShopString);

                        }
                        else if ((model.SummaryPanelModel.VAS_FLAG == "1"
                                || model.SummaryPanelModel.VAS_FLAG == "3"
                                || model.SummaryPanelModel.VAS_FLAG == "6"
                                || (model.SummaryPanelModel.VAS_FLAG == "0" && model.SummaryPanelModel.TOPUP != "1"))
                                    && model.SummaryPanelModel.PackageModel.SelectVas_Flag == "1"
                                    && model.SummaryPanelModel.PackageModel.SelectPlayBox_Flag == "1") /// playbox + fixline
                        {
                            Logger.Info("PDF2");
                            directoryPath = GeneratePDF_HTML(model, @uploadFileWebPath, @uploadFileAppPath, filename, isShopString);
                        }
                        else if ((model.SummaryPanelModel.VAS_FLAG == "1"
                                || model.SummaryPanelModel.VAS_FLAG == "3"
                                || model.SummaryPanelModel.VAS_FLAG == "6"
                                || (model.SummaryPanelModel.VAS_FLAG == "0" && model.SummaryPanelModel.TOPUP != "1"))
                                    && model.SummaryPanelModel.PackageModel.SelectVas_Flag == "0"
                                    && model.SummaryPanelModel.PackageModel.SelectPlayBox_Flag == "1") /// playbox only
                        {
                            Logger.Info("PDF3");
                            directoryPath = GeneratePDF_HTML(model, @uploadFileWebPath, @uploadFileAppPath, filename, isShopString);
                        }
                        else if ((model.SummaryPanelModel.VAS_FLAG == "2"
                                || model.SummaryPanelModel.TOPUP == "1")
                                    && (model.SummaryPanelModel.PackageModel.SelectVas_Flag == "1"
                                        && model.SummaryPanelModel.PackageModel.SelectPlayBox_Flag == "0"))  // create new vas only Line2
                        {
                            Logger.Info("PDF4");
                            newregisterFlag = false;
                            directoryPath = GeneratePDF_HTML(model, @uploadFileWebPath, @uploadFileAppPath, filename, "TOPUP");
                        }
                        else if ((model.SummaryPanelModel.VAS_FLAG == "2"
                                || model.SummaryPanelModel.TOPUP == "1")
                                    && (model.SummaryPanelModel.PackageModel.SelectVas_Flag == "0"
                                        && model.SummaryPanelModel.PackageModel.SelectPlayBox_Flag == "1")) // Playbox only Line2
                        {
                            Logger.Info("PDF5");
                            newregisterFlag = false;
                            directoryPath = GeneratePDF_HTML(model, @uploadFileWebPath, @uploadFileAppPath, filename, "TOPUP");

                        }
                        else if ((model.SummaryPanelModel.VAS_FLAG == "2"
                                || model.SummaryPanelModel.TOPUP == "1")
                                    && (model.SummaryPanelModel.PackageModel.SelectVas_Flag == "1"
                                        && model.SummaryPanelModel.PackageModel.SelectPlayBox_Flag == "1")) /// playbox + fixline Line2
                        {
                            Logger.Info("PDF6");
                            newregisterFlag = false;

                            directoryPath = GeneratePDF_HTML(model, @uploadFileWebPath, @uploadFileAppPath, filename, "TOPUP");
                        }
                        else if (model.SummaryPanelModel.VAS_FLAG == "8"
                               && model.SummaryPanelModel.TOPUP == "1") /// ipcamera Line2
                        {
                            Logger.Info("PDF7");
                            newregisterFlag = false;

                            directoryPath = GeneratePDF_HTML(model, @uploadFileWebPath, @uploadFileAppPath, filename, "IP_CAMERA");
                        }
                        else
                        {
                            Logger.Info("PDFLAST");
                            directoryPath = GeneratePDF_HTML(model, @uploadFileWebPath, @uploadFileAppPath, filename, isShopString);
                        }

                        Session["FILENAME"] = filename;

                        if (isShop)
                        {
                            var langPDFAPP = "";
                            if (SiteSession.CurrentUICulture.IsThaiCulture())
                                langPDFAPP = "T";
                            else
                                langPDFAPP = "E";
                            @directoryPathApp = GeneratePDFApp(model.CustomerRegisterPanelModel.L_CARD_NO, model.CustomerRegisterPanelModel.OrderNo, langPDFAPP, model.CoveragePanelModel.L_CONTACT_PHONE, model);
                        }

                        #endregion PDF

                        #region SendEmail

                        try
                        {
                            Logger.Info(string.Format("Step 1 ,SendEmail ,running_no = {0},L_SEND_EMAIL = {1}", running_no, model.SummaryPanelModel.L_SEND_EMAIL));

                            if (model.SummaryPanelModel.L_SEND_EMAIL)
                            {
                                Logger.Info(string.Format("Step 2 ,SendEmail ,running_no = {0},L_EMAIL = {1}", running_no, model.CustomerRegisterPanelModel.L_EMAIL));

                                if (!string.IsNullOrEmpty(model.CustomerRegisterPanelModel.L_EMAIL))
                                {
                                    Logger.Info(string.Format("Step 3 ,SendEmail ,running_no = {0},newregisterFlag = {1}", running_no, newregisterFlag));

                                    if (!newregisterFlag)
                                    {
                                        SendEmail(customerRowID, running_no, model.CustomerRegisterPanelModel.L_EMAIL, @directoryPath, @directoryPathApp);
                                    }
                                    else
                                    {
                                        Logger.Info(string.Format("Step 4 ,SendEmail ,running_no = {0},ReceiveEmailFlag = {1}", running_no, model.CustomerRegisterPanelModel.ReceiveEmailFlag));
                                        if (model.CustomerRegisterPanelModel.ReceiveEmailFlag == "Y")
                                        {
                                            SendEmail(customerRowID, running_no, model.CustomerRegisterPanelModel.L_EMAIL, @directoryPath, @directoryPathApp);
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Info("Error SendEmail , Error message = " + ex.Message + " , Error Base" + ex.GetBaseException());
                        }

                        if (model.SummaryPanelModel.L_SAVE)
                        {
                            Session["FILENAME"] = filename;
                        }

                        #endregion SendEmail
                    }
                    orderNo = saveOrderResp.RETURN_ORDER_NO;
                    referenceNoStatus = saveOrderResp.RETURN_CODE;
                }
                catch (Exception ex)
                {
                    referenceNoStatus = -2;
                    Logger.Info("Error Submit , " + ex.GetBaseException());
                }

                if (cookie != null)
                {
                    HttpContext.Response.Cookies.Remove("passVerify");
                    cookie.Expires = DateTime.Now.AddDays(-10);
                    cookie.Value = null;
                    HttpContext.Response.SetCookie(cookie);
                }

                //Call web service to transfer order.
                if (model.CoveragePanelModel.CoverageMemberGetMember.IM_ORDER)
                {
                    var Command = new GetFBBOrderNoCommand()
                    {
                        orderNoStatus = referenceNoStatus.ToSafeString(),
                        orderNo = orderNo,
                        caseID = model.CoveragePanelModel.CoverageMemberGetMember.SERVICE_CASE_ID.ToSafeString(),
                        addressAmphur = model.CustomerRegisterPanelModel.AddressPanelModelSetup.L_AMPHUR.ToSafeString(),
                        addressBuilding = model.CoveragePanelModel.CVR_TOWER.ToSafeString(),
                        addressFloor = model.CustomerRegisterPanelModel.AddressPanelModelSetup.L_FLOOR.ToSafeString(),
                        addressMoo = model.CustomerRegisterPanelModel.AddressPanelModelSetup.L_MOO.ToSafeString(),
                        addressMooban = model.CoveragePanelModel.CVR_NODE != null ? model.CoveragePanelModel.CVR_NODE.ToSafeString() : model.CustomerRegisterPanelModel.AddressPanelModelSetup.L_BUILD_NAME.ToSafeString(),
                        addressNo = model.CustomerRegisterPanelModel.AddressPanelModelSetup.L_HOME_NUMBER_2.ToSafeString(),
                        addressPostCode = model.CustomerRegisterPanelModel.AddressPanelModelSetup.L_ZIPCODE.ToSafeString(),
                        addressProvince = model.CustomerRegisterPanelModel.AddressPanelModelSetup.L_PROVINCE.ToSafeString(),
                        addressRoad = model.CustomerRegisterPanelModel.AddressPanelModelSetup.L_ROAD.ToSafeString(),
                        addressSoi = model.CustomerRegisterPanelModel.AddressPanelModelSetup.L_SOI.ToSafeString(),
                        addressTumbol = model.CustomerRegisterPanelModel.AddressPanelModelSetup.L_TUMBOL.ToSafeString()
                    };
                    _getFBBOrderNoCommand.Handle(Command);

                }
                ViewBag.LabelFBBTR000 = GetGeneralScreenConfig();
                ViewBag.LabelFBBTR001 = GetCoverageScreenConfig();
                ViewBag.LabelFBBTR002 = GetDisplayPackageScreenConfig();
                ViewBag.LabelFBBTR003 = GetCustRegisterScreenConfig();
                ViewBag.LabelFBBTR004 = GetSummaryScreenConfig();
                ViewBag.LabelFBBTR010 = GetVas_Select_Package_ScreenConfig();
                ViewBag.LabelFBBORV11 = GetVasPopUpScreenConfig();
                ViewBag.LabelFBBTR023 = GetTopUpFixedlineScreenConfig();
                ViewBag.LabelFBBTR024 = GetTopUpInternet_ScreenConfig();
                ViewBag.LabelFBBOR050 = GetTopUpReplace_ScreenConfig();//R22.03 TopUpReplace
                ViewBag.FbbConstant = fbbConstance;
                ViewBag.FbbException = GetFbbConstantModel(WebConstants.LovConfigName.FbbExceptionIdCard);
                ViewBag.Fbb_Service = GetDisplay_Select_Type_Service();
                ViewBag.SaveSuccess = true;//update 16.3
                SetLovValueToViewBag(model);
                ViewBag.Version = GetVersion();
                ViewBag.ContentPlaybox = GetLovConfigBytype("NEW_CONTENT_PLAYBOX");
                ViewBag.LabelFBBORV25 = GetSelectRouter();
                ViewBag.MeshConfig = GetMeshConfig_ScreenConfig();
                //R23.06 IP Camera
                ViewBag.IpCameraConfig = GetScreenConfig("FBBTR003");

                if (model.SummaryPanelModel.VAS_FLAG == "1" || model.SummaryPanelModel.VAS_FLAG == "2")
                {
                    ViewBag.Vas = model.SummaryPanelModel.VAS_FLAG;
                    if (model.ForCoverageResult == true)
                    {
                        Session["ProcessLine1"] = null;
                        Session["EndProcessFlag"] = true;
                        return RedirectToAction("Index", "ProcessSelect");
                    }
                    else
                    {
                        Session["ProcessLine1"] = null;
                        Session["EndProcessFlag"] = false;
                        return RedirectToAction("Index", "ProcessSelect", new
                        {
                            SaveSuccess = ViewBag.SaveSuccess,
                            LSAVE = ViewBag.LSAVE,
                            LCLOSE = ViewBag.LCLOSE,
                            LPOPUPSAVE = ViewBag.LPOPUPSAVE,
                            LanguagePage = ViewBag.LanguagePage,
                            SWiFi = ViewBag.SWiFi
                        });
                    }
                }
                else if (model.SummaryPanelModel.TOPUP == "1" && model.SummaryPanelModel.VAS_FLAG == "8") //IPCAMERA
                {
                    ViewBag.Vas = model.SummaryPanelModel.VAS_FLAG;
                    if (model.ForCoverageResult == true)
                    {
                        Session["EndProcessFlag"] = true;
                        return RedirectToAction(GetRedirectTopupPage("TopupIPCamera", model.ExistingFlag));
                    }
                    else
                    {
                        Session["EndProcessFlag"] = false;
                        return RedirectToAction(GetRedirectTopupPage("TopupIPCamera", model.ExistingFlag), new
                        {
                            SaveSuccess = ViewBag.SaveSuccess,
                            LSAVE = ViewBag.LSAVE,
                            LCLOSE = ViewBag.LCLOSE,
                            LPOPUPSAVE = ViewBag.LPOPUPSAVE,
                            LanguagePage = ViewBag.LanguagePage,
                            SWiFi = ViewBag.SWiFi,
                            ExistingPopupFlag = GetExistingTopupFlag(model.ExistingFlag)
                        });
                    }
                }
                else if (model.SummaryPanelModel.VAS_FLAG == "0" && model.SummaryPanelModel.TOPUP == "1")  // custiomer acess line 2
                {
                    ViewBag.Vas = model.SummaryPanelModel.VAS_FLAG;
                    ViewBag.TopUp = model.SummaryPanelModel.TOPUP;
                    if (model.ForCoverageResult == true)
                    {
                        Session["EndProcessFlag"] = true;
                        return RedirectToAction(GetRedirectTopupPage("Topup", model.ExistingFlag));
                    }
                    else
                    {
                        Session["EndProcessFlag"] = false;
                        return RedirectToAction(GetRedirectTopupPage("Topup", model.ExistingFlag), new
                        {
                            SaveSuccess = ViewBag.SaveSuccess,
                            LSAVE = ViewBag.LSAVE,
                            LCLOSE = ViewBag.LCLOSE,
                            LPOPUPSAVE = ViewBag.LPOPUPSAVE,
                            LanguagePage = ViewBag.LanguagePage,
                            SWiFi = ViewBag.SWiFi,
                            ExistingPopupFlag = GetExistingTopupFlag(model.ExistingFlag)
                        });
                    }
                }
                else if (model.SummaryPanelModel.VAS_FLAG == "3")  // custiomer acess TriplePlay
                {
                    ViewBag.Vas = model.SummaryPanelModel.VAS_FLAG;
                    if (model.ForCoverageResult == true)
                    {
                        Session["TripleLine"] = null;
                        Session["EndProcessFlag"] = true;
                        return RedirectToAction("TriplePlay");
                    }
                    else
                    {
                        Session["TripleLine"] = null;
                        Session["EndProcessFlag"] = false;
                        return RedirectToAction("TriplePlay", new
                        {
                            SaveSuccess = ViewBag.SaveSuccess,
                            LSAVE = ViewBag.LSAVE,
                            LCLOSE = ViewBag.LCLOSE,
                            LPOPUPSAVE = ViewBag.LPOPUPSAVE,
                            LanguagePage = ViewBag.LanguagePage,
                            SWiFi = ViewBag.SWiFi
                        });
                    }
                }
                else if (model.SummaryPanelModel.VAS_FLAG == "7" && model.SummaryPanelModel.TOPUP == "1")  // R22.03 TopUpReplace line 2
                {
                    if (model.ForCoverageResult == true)
                    {
                        Session["EndProcessFlag"] = true;
                        return RedirectToAction(GetRedirectTopupPage("TopupReplace", model.ExistingFlag));
                    }
                    else
                    {
                        Session["EndProcessFlag"] = false;
                        ViewBag.TopUp = model.SummaryPanelModel.TOPUP;
                        ViewBag.Vas = model.SummaryPanelModel.VAS_FLAG;
                        ViewBag.User = base.CurrentUser;
                        ViewBag.SaveTopupReplace = referenceNoStatus == 0 ? true : false;
                        string ExistingPopupFlag = GetExistingTopupFlag(model.ExistingFlag);
                        ViewBag.ExistingFlag = ExistingPopupFlag == "Y" ? "PLAYBOXREPLACE:MENU" : "PLAYBOXREPLACE";
                        ViewBag.ExistingPopupFlag = ExistingPopupFlag;

                        return View("New_SearchProfile2");
                    }
                }
                else if (model.SummaryPanelModel.TOPUP == "1")  // Topup Line
                {
                    ViewBag.Vas = model.SummaryPanelModel.VAS_FLAG;
                    if (model.ForCoverageResult == true)
                    {
                        Session["EndProcessFlag"] = true;
                        return RedirectToAction(GetRedirectTopupPage("Topup", model.ExistingFlag));
                    }
                    else
                    {
                        Session["EndProcessFlag"] = false;
                        return RedirectToAction(GetRedirectTopupPage("Topup", model.ExistingFlag), new
                        {
                            SaveSuccess = ViewBag.SaveSuccess,
                            LSAVE = ViewBag.LSAVE,
                            LCLOSE = ViewBag.LCLOSE,
                            LPOPUPSAVE = ViewBag.LPOPUPSAVE,
                            LanguagePage = ViewBag.LanguagePage,
                            SWiFi = ViewBag.SWiFi,
                            ExistingPopupFlag = GetExistingTopupFlag(model.ExistingFlag)
                        });
                    }
                }
                else if (model.SummaryPanelModel.VAS_FLAG == "4")  // TopupPlaybox Line
                {
                    ViewBag.Vas = model.SummaryPanelModel.VAS_FLAG;
                    if (model.ForCoverageResult == true)
                    {
                        Session["EndProcessFlag"] = true;
                        return RedirectToAction(GetRedirectTopupPage("TopupPlaybox", model.ExistingFlag));
                    }
                    else
                    {
                        Session["EndProcessFlag"] = false;
                        return RedirectToAction(GetRedirectTopupPage("TopupPlaybox", model.ExistingFlag), new
                        {
                            PACKAGE_NAME = "",
                            SaveSuccess = ViewBag.SaveSuccess,
                            LSAVE = ViewBag.LSAVE,
                            LCLOSE = ViewBag.LCLOSE,
                            LPOPUPSAVE = ViewBag.LPOPUPSAVE,
                            LanguagePage = ViewBag.LanguagePage,
                            SWiFi = ViewBag.SWiFi,
                            DUP = "",
                            ExistingPopupFlag = GetExistingTopupFlag(model.ExistingFlag)
                        });
                    }
                }
                else if (model.SummaryPanelModel.VAS_FLAG == "6")  // STAFF line
                {
                    ViewBag.Vas = model.SummaryPanelModel.VAS_FLAG;
                    if (model.ForCoverageResult == true)
                    {
                        Session["EndProcessFlag"] = true;
                        return RedirectToAction("staff", new
                        {
                            TransactionID = model.TransactionID,
                            EmpID = model.CustomerRegisterPanelModel.L_STAFF_ID,
                            EmpName = model.CustomerRegisterPanelModel.L_FIRST_NAME,
                            EmpLastName = model.CustomerRegisterPanelModel.L_LAST_NAME
                        });
                    }
                    else
                    {
                        Session["EndProcessFlag"] = false;
                        return RedirectToAction("staff", new
                        {
                            TransactionID = model.TransactionID,
                            EmpID = model.CustomerRegisterPanelModel.L_STAFF_ID,
                            EmpName = model.CustomerRegisterPanelModel.L_FIRST_NAME,
                            EmpLastName = model.CustomerRegisterPanelModel.L_LAST_NAME,
                            SaveSuccess = ViewBag.SaveSuccess,
                            LSAVE = ViewBag.LSAVE,
                            LCLOSE = ViewBag.LCLOSE,
                            LPOPUPSAVE = ViewBag.LPOPUPSAVE,
                            LanguagePage = ViewBag.LanguagePage,
                            SWiFi = ViewBag.SWiFi
                        });
                    }
                }
                else if (model.TopUp == "7")  // Engineer Line (Temp)
                {
                    ViewBag.TopUp = model.SummaryPanelModel.TOPUP;
                    if (model.ForCoverageResult == true)
                    {
                        Session["EnginerrLine"] = null;
                        Session["EndProcessFlag"] = true;
                        return RedirectToAction("Engineer");
                    }
                    else
                    {
                        Session["EnginerrLine"] = null;
                        Session["EndProcessFlag"] = false;
                        return RedirectToAction("Engineer", new
                        {
                            SaveSuccess = ViewBag.SaveSuccess,
                            LSAVE = ViewBag.LSAVE,
                            LCLOSE = ViewBag.LCLOSE,
                            LPOPUPSAVE = ViewBag.LPOPUPSAVE,
                            LanguagePage = ViewBag.LanguagePage,
                            SWiFi = ViewBag.SWiFi
                        });
                    }
                }

                else
                {
                    if (model.ForCoverageResult == true)
                    {
                        Session["EndProcessFlag"] = true;
                        if (Session["CheckCoverage"] != null)
                        {
                            Session["CheckCoverage"] = null;
                            return RedirectToAction("CheckCoverage");
                            //return Content("<html><script>>window.top.location.href = 'CheckCoverage'; </script></html>");
                        }
                        Session["NonCoverage"] = true;
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        Session["EndProcessFlag"] = false;
                        if (model.TopUp == "5")
                        {
                            ViewBag.officeEnd = "1";
                        }
                    }
                }

                //return SearchProfileIndex(model: model);

                TempData["Version"] = ViewBag.Version;
                TempData["LabelFBBTR001"] = ViewBag.LabelFBBTR001;
                TempData["LabelFBBTR002"] = ViewBag.LabelFBBTR002;
                TempData["LabelFBBTR003"] = ViewBag.LabelFBBTR003;
                TempData["LabelFBBTR004"] = ViewBag.LabelFBBTR004;
                TempData["LabelFBBOR015"] = ViewBag.LabelFBBOR015;
                TempData["LabelFBBORV25"] = ViewBag.LabelFBBORV25;
                TempData["FbbConstant"] = ViewBag.FbbConstant;
                TempData["officer"] = ViewBag.officer;
                TempData["TopUp"] = ViewBag.TopUp;
                TempData["SaveSuccess"] = ViewBag.SaveSuccess;
                TempData["SWifi"] = ViewBag.SWifi;
                TempData["LanguagePage"] = ViewBag.LanguagePage;
                TempData["LCLOSE"] = ViewBag.LCLOSE;
                TempData["LSAVE"] = ViewBag.LSAVE;
                TempData["LPOPUPSAVE"] = ViewBag.LPOPUPSAVE;
                TempData["officeEnd"] = ViewBag.officeEnd;
                TempData["ESRIflag"] = ViewBag.ESRIflag;
                TempData["Vas"] = ViewBag.Vas;
                TempData["QuickWinPanelModel"] = model;

                //20.2
                ViewBag.isWTTx = model.CoveragePanelModel.WTTX_COVERAGE_RESULT == "YES" ? true : false;
                TempData["isWTTx"] = ViewBag.isWTTx;

            }
            catch (Exception mEx)
            {
                Logger.Info("Error Index , " + mEx.GetBaseException());
            }
            return RedirectToAction("IndexWithModel", new { model = "" });
        }

        public ActionResult SearchProfileIndex(QuickWinPanelModel model)
        {
            if (Session["EndProcessFlag"].ToSafeBoolean())
            {
                Session["PopupStatus"] = "Success";
                Session["EndProcessFlag"] = null;
            }
            else
                Session["PopupStatus"] = null;

            if (Session["NonCoverage"].ToSafeBoolean())
            {
                Session["NonCoverage"] = null;
                ViewBag.nonCoverage = true;
            }
            else
            {
                ViewBag.nonCoverage = false;
            }

            string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress))
            {
                ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
            SaveStatlog("CUSTOMER", "CUSTOMER", ipAddress, "FBB REGISTER", "", "");

            ViewBag.LabelFBBTR000 = GetGeneralScreenConfig();
            ViewBag.LabelFBBTR001 = GetCoverageScreenConfig();
            ViewBag.LabelFBBTR002 = GetDisplayPackageScreenConfig();
            ViewBag.LabelFBBTR003 = GetCustRegisterScreenConfig();
            ViewBag.LabelFBBTR004 = GetSummaryScreenConfig();
            ViewBag.LabelFBBTR010 = GetVas_Select_Package_ScreenConfig();
            ViewBag.LabelFBBORV11 = GetVasPopUpScreenConfig();
            ViewBag.LabelFBBOR015 = GetProfilePrePostPaid();
            ViewBag.LabelFBBORV25 = GetSelectRouter();
            ViewBag.LabelFBBOR050 = GetTopUpReplace_ScreenConfig();
            ViewBag.MeshConfig = GetMeshConfig_ScreenConfig();
            // ViewBag.Deverloper = GetScreenConfig("DEVELOPER");
            ViewBag.FbbConstant = GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);
            ViewBag.FbbException = GetFbbConstantModel(WebConstants.LovConfigName.FbbExceptionIdCard);
            ViewBag.Fbb_Service = GetDisplay_Select_Type_Service();
            ViewBag.Version = GetVersion();
            ViewBag.Vas = "";
            ViewBag.User = base.CurrentUser;
            ViewBag.ContentPlaybox = GetLovConfigBytype("NEW_CONTENT_PLAYBOX");
            //mesh arpu
            ViewBag.MeshArpuFBBTR001 = GetScreenConfig("1");
            ViewBag.MeshArpuFBBTR002 = GetScreenConfig("2");
            ViewBag.MeshArpuFBBTR003 = GetScreenConfig("3");
            ViewBag.MeshArpuFBBTR004 = GetScreenConfig("99");
            ViewBag.MeshArpuFlag = GetScreenConfig("98");
            ViewBag.MeshArpuPromotionCode = GetScreenConfig("305");

            //R23.06 IP Camera
            ViewBag.IpCameraConfig = GetScreenConfig("FBBTR003");

            var newmodel = new QuickWinPanelModel();
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

            if (Session["ESRI"] != null)
            {
                base.Logger.Info("in esri");

                ViewBag.ESRIflag = Session["ESRI"] as string;
                newmodel = Session["ESRI_COVERAGEMODEL"] as QuickWinPanelModel;
                Session["ESRI"] = null;
                newmodel.ClientIP = ipAddress;
                return View(newmodel);
            }

            if (Session["StaffModel"] != null)
            {
                ViewBag.Vas = "6";
                newmodel = Session["StaffModel"] as QuickWinPanelModel;
                Session["FullUrl"] = this.Url.Action("staff", "Process", null, this.Request.Url.Scheme);
            }

            if (Session["OfficerModel"] != null)
            {
                ViewBag.officer = "1";
                newmodel = Session["OfficerModel"] as QuickWinPanelModel;
                Session["FullUrl"] = this.Url.Action("Index", "Officer", null, this.Request.Url.Scheme);
                if (model != null)
                    newmodel.PlugAndPlayFlow = model.PlugAndPlayFlow;
                if (Session["isSaleManager"] != null)
                    ViewBag.isSaleManager = Session["isSaleManager"].ToString();

                ViewBag.LcCode = newmodel.CustomerRegisterPanelModel.L_LOC_CODE;
                ViewBag.ASCCode = newmodel.CustomerRegisterPanelModel.L_ASC_CODE;
                ViewBag.EmployeeID = newmodel.CustomerRegisterPanelModel.L_STAFF_ID;
                ViewBag.outType = newmodel.CustomerRegisterPanelModel.outType;
                ViewBag.outSubType = newmodel.CustomerRegisterPanelModel.outSubType;
                ViewBag.outMobileNo = newmodel.CustomerRegisterPanelModel.outMobileNo;
                ViewBag.PartnerName = newmodel.CustomerRegisterPanelModel.PartnerName;
                ViewBag.outLocationEmailByRegion = newmodel.CustomerRegisterPanelModel.LOCATION_EMAIL_BY_REGION;
                ViewBag.outEmpName = newmodel.CustomerRegisterPanelModel.EMP_NAME;
                //R21.4 
                ViewBag.outTitle = newmodel.OfficerInfoPanelModel.outTitle;
                ViewBag.outCompanyName = newmodel.OfficerInfoPanelModel.outCompanyName;
                ViewBag.outDistChn = newmodel.OfficerInfoPanelModel.outDistChn;
                ViewBag.outChnSales = newmodel.OfficerInfoPanelModel.outChnSales;
                ViewBag.outShopType = newmodel.OfficerInfoPanelModel.outShopType;
                ViewBag.outOperatorClass = newmodel.OfficerInfoPanelModel.outOperatorClass;
                ViewBag.outASCTitleThai = newmodel.OfficerInfoPanelModel.outASCTitleThai;
                ViewBag.outASCPartnerName = newmodel.OfficerInfoPanelModel.outASCPartnerName;
                ViewBag.outMemberCategory = newmodel.OfficerInfoPanelModel.outMemberCategory;
                ViewBag.outPosition = newmodel.OfficerInfoPanelModel.outPosition;
                ViewBag.outLocationRegion = newmodel.OfficerInfoPanelModel.outLocationRegion;
                ViewBag.outLocationSubRegion = newmodel.OfficerInfoPanelModel.outLocationSubRegion;
                ViewBag.THFirstName = newmodel.OfficerInfoPanelModel.THFirstName;
                ViewBag.THLastName = newmodel.OfficerInfoPanelModel.THLastName;
                //R21.5 Pool Villa
                ViewBag.outLocationProvince = newmodel.OfficerInfoPanelModel.outLocationProvince;


            }
            //เพิ่ม
            else
            {
                if (checkSessionOfficerModel(model))
                {
                    ViewBag.officer = "1";
                    Session["FullUrl"] = this.Url.Action("Index", "Officer", null, this.Request.Url.Scheme);
                    if (model != null)
                        model.PlugAndPlayFlow = model.PlugAndPlayFlow;
                    if (Session["isSaleManager"] != null)
                        ViewBag.isSaleManager = Session["isSaleManager"].ToString();

                    ViewBag.LcCode = model.CustomerRegisterPanelModel.L_LOC_CODE;
                    ViewBag.ASCCode = model.CustomerRegisterPanelModel.L_ASC_CODE;
                    ViewBag.EmployeeID = model.CustomerRegisterPanelModel.L_STAFF_ID;
                    ViewBag.outType = model.CustomerRegisterPanelModel.outType;
                    ViewBag.outSubType = model.CustomerRegisterPanelModel.outSubType;
                    ViewBag.outMobileNo = model.CustomerRegisterPanelModel.outMobileNo;
                    ViewBag.PartnerName = model.CustomerRegisterPanelModel.PartnerName;
                    ViewBag.outLocationEmailByRegion = model.CustomerRegisterPanelModel.LOCATION_EMAIL_BY_REGION;
                    ViewBag.outEmpName = model.CustomerRegisterPanelModel.EMP_NAME;
                    //R21.4
                    ViewBag.outTitle = model.OfficerInfoPanelModel.outTitle;
                    ViewBag.outCompanyName = model.OfficerInfoPanelModel.outCompanyName;
                    ViewBag.outDistChn = model.OfficerInfoPanelModel.outDistChn;
                    ViewBag.outChnSales = model.OfficerInfoPanelModel.outChnSales;
                    ViewBag.outShopType = model.OfficerInfoPanelModel.outShopType;
                    ViewBag.outOperatorClass = model.OfficerInfoPanelModel.outOperatorClass;
                    ViewBag.outASCTitleThai = model.OfficerInfoPanelModel.outASCTitleThai;
                    ViewBag.outASCPartnerName = model.OfficerInfoPanelModel.outASCPartnerName;
                    ViewBag.outMemberCategory = model.OfficerInfoPanelModel.outMemberCategory;
                    ViewBag.outPosition = model.OfficerInfoPanelModel.outPosition;
                    ViewBag.outLocationRegion = model.OfficerInfoPanelModel.outLocationRegion;
                    ViewBag.outLocationSubRegion = model.OfficerInfoPanelModel.outLocationSubRegion;
                    ViewBag.THFirstName = model.OfficerInfoPanelModel.THFirstName;
                    ViewBag.THLastName = model.OfficerInfoPanelModel.THLastName;
                    //R21.5 Pool Villa
                    ViewBag.outLocationProvince = model.OfficerInfoPanelModel.outLocationProvince;

                    Session["OfficerModel"] = model;
                }
            }

            if (Session["TripleLine"] != null)
            {
                ViewBag.Vas = "3";
            }

            if (Session["ProcessLine1"] != null)
            {
                ViewBag.Vas = "1";
                newmodel.SummaryPanelModel.VAS_FLAG = "1";
            }

            if (Session["EnginerrLine"] != null)
            {
                ViewBag.Vas = "";
                newmodel.TopUp = "7";
            }

            if (null != model)
            {
                newmodel = model;
            }

            //20.6 AI Chat Bot - Bypass
            if (TempData["tempAIChatBotBypass"] != null)
            {
                var aiModel = TempData["tempAIChatBotBypass"] as QuickWinPanelModel;
                if (aiModel != null)
                {
                    aiModel.languageCulture = SiteSession.CurrentUICulture.IsThaiCulture() ? 1 : 2;
                    newmodel = aiModel;
                    Session["FullUrl"] = this.Url.Action("Index", "Process", null, this.Request.Url.Scheme);
                    Session["AIChatBotBypass"] = aiModel;
                }
            }
            else
            {
                Session["AIChatBotBypass"] = null;
            }

            if (TempData["isWTTx"] != null)
            {
                ViewBag.SaveSuccess = true;
                ViewBag.isWTTx = TempData["isWTTx"];
                if (ViewBag.isWTTx)
                {
                    if (TempData["WTTxMSG"] != null)
                    {
                        ViewBag.WTTxMSG = TempData["WTTxMSG"];
                    }
                    else if (TempData["WTTxErrorMSG"] != null)
                    {
                        ViewBag.WTTxErrorMSG = TempData["WTTxErrorMSG"];
                    }
                }
            }

            newmodel.ClientIP = ipAddress;
            return View("New_SearchProfilePrePostPaid", newmodel);
        }


        public ActionResult CheckCoverage(QuickWinPanelModel model)
        {
            QuickWinPanelModel moumodel = model;//R21.10
            Session["ESRI_BYPASSCOVERAGEMODEL"] = null;
            Session["CheckCoverage"] = true;
            if (Session["EndProcessFlag"].ToSafeBoolean())
            {
                Session["PopupStatus"] = "Success";
                Session["EndProcessFlag"] = null;
            }
            else
                Session["PopupStatus"] = null;

            string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress))
            {
                ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
            SaveStatlog("CUSTOMER", "CUSTOMER", ipAddress, "FBB REGISTER", "", "");

            ViewBag.LabelFBBTR000 = GetGeneralScreenConfig();
            ViewBag.LabelFBBTR001 = GetCoverageScreenConfig();
            ViewBag.LabelFBBTR002 = GetDisplayPackageScreenConfig();
            ViewBag.LabelFBBTR003 = GetCustRegisterScreenConfig();
            ViewBag.LabelFBBTR004 = GetSummaryScreenConfig();
            ViewBag.LabelFBBTR010 = GetVas_Select_Package_ScreenConfig();
            ViewBag.LabelFBBORV11 = GetVasPopUpScreenConfig();
            ViewBag.LabelFBBOR015 = GetProfilePrePostPaid();
            ViewBag.LabelFBBORV25 = GetSelectRouter();
            ViewBag.MeshConfig = GetMeshConfig_ScreenConfig();
            // ViewBag.Deverloper = GetScreenConfig("DEVELOPER");
            ViewBag.FbbConstant = GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);
            ViewBag.FbbException = GetFbbConstantModel(WebConstants.LovConfigName.FbbExceptionIdCard);
            ViewBag.Fbb_Service = GetDisplay_Select_Type_Service();
            ViewBag.Version = GetVersion();
            ViewBag.Vas = "";
            ViewBag.User = base.CurrentUser;
            ViewBag.ContentPlaybox = GetLovConfigBytype("NEW_CONTENT_PLAYBOX");
            //R23.06 IP Camera
            ViewBag.IpCameraConfig = GetScreenConfig("FBBTR003");

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
            if (Session["ESRI"] != null)
            {
                base.Logger.Info("in esri");

                ViewBag.ESRIflag = Session["ESRI"] as string;
                model = Session["ESRI_COVERAGEMODEL"] as QuickWinPanelModel;
                model.ClientIP = ipAddress;
                Session["ESRI"] = null;

            }

            ViewBag.Vas = "1";
            ViewBag.chkCoverage = true;
            ViewBag.IsCheckCoverage = "Y";
            model.SummaryPanelModel.VAS_FLAG = "1";
            model.ClientIP = ipAddress;
            return View("Index", model);
        }

        [HttpGet]
        public ActionResult LoadCheckCoverage(string statusData = "")
        {
            Session["CheckCoverage"] = null;
            if (Session["EndProcessFlag"].ToSafeBoolean())
            {
                Session["PopupStatus"] = "Success";
                Session["EndProcessFlag"] = null;
            }
            else
                Session["PopupStatus"] = null;

            string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress))
            {
                ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
            SaveStatlog("CUSTOMER", "CUSTOMER", ipAddress, "FBB REGISTER", "", "");

            ViewBag.LabelFBBTR000 = GetGeneralScreenConfig();
            ViewBag.LabelFBBTR001 = GetCoverageScreenConfig();
            ViewBag.LabelFBBTR002 = GetDisplayPackageScreenConfig();
            ViewBag.LabelFBBTR003 = GetCustRegisterScreenConfig();
            ViewBag.LabelFBBTR004 = GetSummaryScreenConfig();
            ViewBag.LabelFBBTR010 = GetVas_Select_Package_ScreenConfig();
            ViewBag.LabelFBBORV11 = GetVasPopUpScreenConfig();
            ViewBag.LabelFBBOR015 = GetProfilePrePostPaid();
            ViewBag.LabelFBBORV25 = GetSelectRouter();
            ViewBag.MeshConfig = GetMeshConfig_ScreenConfig();
            //ViewBag.Deverloper = GetScreenConfig("DEVELOPER");
            // ViewBag.DropDownDeveloper = Getdropdowndeveloper();
            ViewBag.FbbConstant = GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);
            ViewBag.FbbException = GetFbbConstantModel(WebConstants.LovConfigName.FbbExceptionIdCard);
            ViewBag.Fbb_Service = GetDisplay_Select_Type_Service();
            ViewBag.Version = GetVersion();
            ViewBag.Vas = "";
            ViewBag.User = base.CurrentUser;
            //update17.2
            ViewBag.PlugandplayMessage = GetPlugandplayMessage();
            ViewBag.ContentPlaybox = GetLovConfigBytype("NEW_CONTENT_PLAYBOX");

            //mesh arpu
            ViewBag.MeshArpuFBBTR001 = GetScreenConfig("1");
            ViewBag.MeshArpuFBBTR002 = GetScreenConfig("2");
            ViewBag.MeshArpuFBBTR003 = GetScreenConfig("3");
            ViewBag.MeshArpuFBBTR004 = GetScreenConfig("99");
            ViewBag.MeshArpuFlag = GetScreenConfig("98");
            ViewBag.MeshArpuPromotionCode = GetScreenConfig("305");
            //R23.06 IP Camera
            ViewBag.IpCameraConfig = GetScreenConfig("FBBTR003");

            //var newmodel = new QuickWinPanelModel();
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

            var model = new QuickWinPanelModel();
            var moumodel = new QuickWinPanelModel();//R21.10 MOU

            if (Session["LoadCheckCoverageModel"] != null)
            {
                model = Session["LoadCheckCoverageModel"] as QuickWinPanelModel;
                moumodel = model;
                if (Session["ESRI_BYPASSCOVERAGEMODEL"] != null)
                {
                    QuickWinPanelModel BypassModel = Session["ESRI_BYPASSCOVERAGEMODEL"] as QuickWinPanelModel;
                    /// bind value search mobile to BypassModel

                    BypassModel.CoveragePanelModel.P_MOBILE = model.CoveragePanelModel.P_MOBILE;
                    BypassModel.CoveragePanelModel.Serenade_Flag = model.CoveragePanelModel.Serenade_Flag;
                    BypassModel.CoveragePanelModel.L_CONTACT_PHONE = model.CoveragePanelModel.L_CONTACT_PHONE;
                    BypassModel.CustomerRegisterPanelModel.L_MOBILE = model.CustomerRegisterPanelModel.L_MOBILE;
                    BypassModel.CoveragePanelModel.Mobile_Segment = model.CoveragePanelModel.Mobile_Segment;
                    BypassModel.CoveragePanelModel.NetworkType = model.CoveragePanelModel.NetworkType;
                    BypassModel.CoveragePanelModel.ChargeType = model.CoveragePanelModel.ChargeType;
                    BypassModel.CustomerRegisterPanelModel.L_CARD_NO = model.CustomerRegisterPanelModel.L_CARD_NO;
                    BypassModel.CustomerRegisterPanelModel.Project_name = model.CustomerRegisterPanelModel.Project_name;
                    BypassModel.CustomerRegisterPanelModel.AccountCategory = model.CustomerRegisterPanelModel.AccountCategory;
                    BypassModel.Register_device = model.Register_device;
                    BypassModel.Browser_type = model.Browser_type;
                    BypassModel.CustomerRegisterPanelModel.OrderNo = model.CustomerRegisterPanelModel.OrderNo;
                    BypassModel.CustomerRegisterPanelModel.outSubType = model.CustomerRegisterPanelModel.outSubType;
                    BypassModel.CustomerRegisterPanelModel.outMobileNo = model.CustomerRegisterPanelModel.outMobileNo;
                    BypassModel.CustomerRegisterPanelModel.PartnerName = model.CustomerRegisterPanelModel.PartnerName;
                    BypassModel.CustomerRegisterPanelModel.SUB_ACCESS_MODE = model.CustomerRegisterPanelModel.SUB_ACCESS_MODE;
                    BypassModel.CustomerRegisterPanelModel.REQUEST_SUB_FLAG = model.CustomerRegisterPanelModel.REQUEST_SUB_FLAG;
                    BypassModel.CustomerRegisterPanelModel.PREMIUM_FLAG = model.CustomerRegisterPanelModel.PREMIUM_FLAG;
                    BypassModel.CustomerRegisterPanelModel.RELATE_MOBILE_SEGMENT = model.CustomerRegisterPanelModel.RELATE_MOBILE_SEGMENT;
                    BypassModel.CustomerRegisterPanelModel.REF_UR_NO = model.CustomerRegisterPanelModel.REF_UR_NO;
                    BypassModel.CustomerRegisterPanelModel.LOCATION_EMAIL_BY_REGION = model.CustomerRegisterPanelModel.LOCATION_EMAIL_BY_REGION;
                    BypassModel.CustomerRegisterPanelModel.EMP_NAME = model.CustomerRegisterPanelModel.EMP_NAME;
                    BypassModel.SummaryPanelModel.VAS_FLAG = "";
                    model = BypassModel;
                    ViewBag.BypassESRIflag = "true";
                    if (statusData == "")
                    {
                        Session["ESRI_BYPASSCOVERAGEMODEL"] = null;
                    }
                }
            }

            if (Session["ESRI"] != null)
            {
                base.Logger.Info("in esri");

                ViewBag.ESRIflag = Session["ESRI"] as string;
                model = Session["ESRI_COVERAGEMODEL"] as QuickWinPanelModel;
                Session["ESRI"] = null;
            }

            if (Session["StaffModel"] != null)
            {
                ViewBag.Vas = "6";
                model = Session["StaffModel"] as QuickWinPanelModel;
            }

            if (Session["OfficerModel"] != null)
            {
                ViewBag.officer = "1";
                var officerModel = Session["OfficerModel"] as QuickWinPanelModel;
                //20.2
                officerModel.CoveragePanelModel.WTTX_MOBILE_NO = model.CoveragePanelModel.P_MOBILE;

                model.SummaryPanelModel.TOPUP = "5";
                model.CustomerRegisterPanelModel.L_ASC_CODE = officerModel.CustomerRegisterPanelModel.L_ASC_CODE;
                model.CustomerRegisterPanelModel.L_LOC_CODE = officerModel.CustomerRegisterPanelModel.L_LOC_CODE;
                model.CustomerRegisterPanelModel.L_STAFF_ID = officerModel.CustomerRegisterPanelModel.L_STAFF_ID;

                model.CustomerRegisterPanelModel.outType = officerModel.CustomerRegisterPanelModel.outType;
                model.CustomerRegisterPanelModel.outSubType = officerModel.CustomerRegisterPanelModel.outSubType;
                model.CustomerRegisterPanelModel.outMobileNo = officerModel.CustomerRegisterPanelModel.outMobileNo;
                model.CustomerRegisterPanelModel.PartnerName = officerModel.CustomerRegisterPanelModel.PartnerName;
                model.PlugAndPlayFlow = officerModel.PlugAndPlayFlow;
                //R20.9
                model.OfficerInfoPanelModel.THFirstName = officerModel.OfficerInfoPanelModel.THFirstName;
                model.OfficerInfoPanelModel.THLastName = officerModel.OfficerInfoPanelModel.THLastName;
                model.OfficerInfoPanelModel.outLocationRegion = officerModel.OfficerInfoPanelModel.outLocationRegion;
                model.OfficerInfoPanelModel.outLocationSubRegion = officerModel.OfficerInfoPanelModel.outLocationSubRegion;
                model.OfficerInfoPanelModel.outASCPartnerName = officerModel.OfficerInfoPanelModel.outASCPartnerName;
                //R20.12
                model.OfficerInfoPanelModel.outChnSales = officerModel.OfficerInfoPanelModel.outChnSales;
                //R21.5 Pool Villa
                model.OfficerInfoPanelModel.outLocationProvince = officerModel.OfficerInfoPanelModel.outLocationProvince;

                if (Session["isSaleManager"] != null)
                    ViewBag.isSaleManager = Session["isSaleManager"].ToString();

                model.TopUp = "5";
            }

            if (Session["TripleLine"] != null)
            {
                ViewBag.Vas = "3";
            }

            if (Session["ProcessLine1"] != null)
            {
                ViewBag.Vas = "1";
                model.SummaryPanelModel.VAS_FLAG = "1";
            }

            if (Session["EnginerrLine"] != null)
            {
                ViewBag.Vas = "";
                model.TopUp = "7";
            }

            if (model.CoveragePanelModel.CoverageMemberGetMember.IM_ORDER && !String.IsNullOrEmpty(model.CoveragePanelModel.CoverageMemberGetMember.RefferenceNo))
            {
                //model.CoveragePanelModel.CoverageMemberGetMember.VillageName = "หมู่บ้าน ทีทีซีที ซีทีอี";

                ViewBag.IMData = Newtonsoft.Json.JsonConvert.SerializeObject(model.CoveragePanelModel.CoverageMemberGetMember);
            }

            ////Get location Type from Lov #04-10-16
            //var LocTypeQuery = new GetLovQuery
            //{
            //    LovType = "FBB_CONSTANT",
            //    LovName = "LOCATION_CODE_TYPE",
            //};
            //var LocTypeData = _queryProcessor.Execute(LocTypeQuery);
            //if (LocTypeData.Count > 0)
            //{
            //    if (model.CustomerRegisterPanelModel.outType == LocTypeData[0].Text.ToSafeString())
            //    {
            //        model.CustomerRegisterPanelModel.L_STAFF_ID = model.CustomerRegisterPanelModel.L_STAFF_ID;
            //        model.CustomerRegisterPanelModel.L_ASC_CODE = "";
            //    }
            //}

            //20.6 AI Chat Bot - Bypass
            if (Session["AIChatBotBypass"] != null)
            {
                var aiModel = Session["AIChatBotBypass"] as QuickWinPanelModel;
                if (aiModel != null)
                {
                    if (aiModel.CustomerRegisterPanelModel != null && !string.IsNullOrEmpty(aiModel.CustomerRegisterPanelModel.RegisterChannelSaveOrder) &&
                        model != null && model.CustomerRegisterPanelModel != null)
                    {
                        model.CustomerRegisterPanelModel.RegisterChannelSaveOrder = aiModel.CustomerRegisterPanelModel.RegisterChannelSaveOrder;
                        model.AIChatBotBypass_Channel = aiModel.AIChatBotBypass_Channel;
                        model.AIChatBotBypass_Flag = aiModel.AIChatBotBypass_Flag;
                    }
                }
            }

            //21.3
            if (model.StaffPrivilegeBypass_Flag.ToSafeString() == "Y")
            {
                model.CustomerRegisterPanelModel.StaffPrivilegeBypass_TransactionID = model.StaffPrivilegeBypass_TransactionID.ToSafeString();
                Session["StaffPrivilegeBypass"] = model;
            }
            else
            {
                Session["StaffPrivilegeBypass"] = null;
            }
            model.ClientIP = ipAddress;

            //R24.01 Max test ViewBag.AllowCheckFraud
            if (model.CoveragePanelModel.Address.L_HOME_NUMBER_1 == "allowCheckFraud")
            {
                ViewBag.AllowCheckFraud = "allowCheckFraud";
                model.CoveragePanelModel.Address.L_HOME_NUMBER_1 = "";
            }
            return View("Index", model);
        }

        [HttpPost]
        public ActionResult LoadCheckCoverage(QuickWinPanelModel model)
        {
            bool checkSessionOfficertmp = checkSessionOfficer();
            if (checkSessionOfficertmp)
            {
                return RedirectToAction("Index", "Officer");
            }

            Session["LoadCheckCoverageModel"] = model;
            return LoadCheckCoverage("1");
        }

        [HttpPost]
        public ActionResult ProcessLine2(QuickWinPanelModel model)
        {
            //R24.05 Add loading block screen by max kunlp885
            /*if (checkLoadingBlock("ProcessLine2", ""))
            {
                Session["LoadingBlockModel"] = model;
                return View("ExistingFibreExtensions/_Loading");
            }
            model = Session["LoadingBlockModel"] as QuickWinPanelModel;
            //end R24.05 Add loading block screen by max kunlp885
            */

            string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress))
                ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            ViewBag.v_icon_pb = model.v_icon_pb;
            ViewBag.v_check_pb = model.v_check_pb;
            ViewBag.v_add_playbox = model.v_add_playbox;
            ViewBag.Channel = "WEB";
            ViewBag.TopUp = "1";
            model.SummaryPanelModel.TOPUP = "1";
            if (Session["EndProcessFlag"].ToSafeBoolean())
            {
                Session["PopupStatus"] = "Success";
                Session["EndProcessFlag"] = null;
            }
            else
                Session["PopupStatus"] = null;


            if (model.ExistingFlag == "MESH" || model.ExistingFlag == "MESH:MENU")
            {
                model.TopupMesh = "Y";
            }
            else if (model.ExistingFlag == "PLAYBOXREPLACE" || model.ExistingFlag == "PLAYBOXREPLACE:MENU")
            {//R22.03 TopupReplace
                model.TopupReplace = "Y";
                ViewBag.count_atv = model.count_atv;
                ViewBag.sympton_code_pbreplace = model.sympton_code_pbreplace;
            }


            ViewBag.LabelFBBTR000 = GetGeneralScreenConfig();
            ViewBag.LabelFBBTR001 = GetCoverageScreenConfig();
            ViewBag.LabelFBBTR002 = GetDisplayPackageScreenConfig();
            ViewBag.LabelFBBTR003 = GetCustRegisterScreenConfig();
            ViewBag.LabelFBBTR004 = GetSummaryScreenConfig();
            ViewBag.LabelFBBTR010 = GetVas_Select_Package_ScreenConfig();
            ViewBag.LabelFBBORV11 = GetVasPopUpScreenConfig();
            ViewBag.LabelFBBTR023 = GetTopUpFixedlineScreenConfig();
            ViewBag.FbbConstant = GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);
            ViewBag.FbbException = GetFbbConstantModel(WebConstants.LovConfigName.FbbExceptionIdCard);
            ViewBag.Fbb_Service = GetDisplay_Select_Type_Service();
            ViewBag.SFFProductName = model.outProductName;
            ViewBag.SFFServiceYear = model.outDayOfServiceYear;
            ViewBag.Version = GetVersion();
            ViewBag.ExistingFlag = model.ExistingFlag;
            ViewBag.ContentPlaybox = GetLovConfigBytype("NEW_CONTENT_PLAYBOX");
            ViewBag.LabelFBBORV41 = GetTopUpMesh_ScreenConfig();
            ViewBag.MeshConfig = GetMeshConfig_ScreenConfig();
            ViewBag.IsSetServiceLevel = "";
            ViewBag.LabelFBBOR050 = GetTopUpReplace_ScreenConfig();//R22.03 TopupReplace
            ViewBag.IpCamera = GetIpCameraScreenConfig();   //R23.06 IP Camera
            ViewBag.IpCameraConfig = GetScreenConfig("FBBTR003");
            ViewBag.LabelAddressIPCam = GetScreenConfig("FBBOR028");  //23.08 IPCAMERA
            ViewBag.IPCameraDetailInfo = GetListByLOVName("IPCamera_Detail_Info");//23.09 IPCAMERA
            ViewBag.IPCameraServiceCondition = GetListByLOVName("Existing_Service_Condition");//23.09 IPCAMERA
            ViewBag.IPCameraLegalCondition = GetListByLOVName("Existing_Legal_Condition");//23.09 IPCAMERA
            ViewBag.IPCameraConfirmCondition = GetListByLOVName("Existing_Confirm_Condition");//23.09 IPCAMERA
            ViewBag.LabelFBBOR041 = GetScreenConfig("FBBOR041");
            if (!string.IsNullOrEmpty(model.outProductName) && !string.IsNullOrEmpty(model.outDayOfServiceYear))
            {
                if (model.TopUp == "1")
                {
                    ViewBag.Vas = "";
                    model.SummaryPanelModel.VAS_FLAG = "";
                }
                else
                {
                    ViewBag.Vas = "2";
                    model.SummaryPanelModel.VAS_FLAG = "2";
                }
                if (model.ExistingFlag == "IP_CAMERA" || model.ExistingFlag == "IP_CAMERA:MENU")
                {
                    ViewBag.Vas = "8";
                    model.SummaryPanelModel.VAS_FLAG = "8";
                    if (ViewBag.CheckChannel == "Godzila")
                    {
                        ViewBag.Channel = "Godzila";
                    }
                    //IPCAMERA 23.03 KONG
                }
                var q = new GetOwnerProductByNoQuery
                {
                    No = model.no,
                    BA_ID = model.CoveragePanelModel.BA_ID
                };
                var aa = _queryProcessor.Execute(q);
                ViewBag.OwnerProduct = aa.Value;
                ViewBag.PackageCode = aa.Value2;
                //ViewBag.AddressID = aa.Value3;
                ViewBag.AddressID = model.CoveragePanelModel.Address.AddressId;
                ViewBag.SffProfileLogID = model.SffProfileLogID;
                ViewBag.ContactMobileNo = aa.Value4;
            }
            else
            {
                ViewBag.Vas = "1";
                model.SummaryPanelModel.VAS_FLAG = "1";
            }

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
                        ViewBag.LanguagePage = "1";
                    }
                }
                else
                {
                    if (data.LovValue2 != null)
                    {
                        ViewBag.LCLOSE = data.LovValue2.ToString();
                        ViewBag.LanguagePage = "2";
                    }
                }
            }
            else
            {
                ViewBag.LCLOSE = "";
            }

            model.ClientIP = ipAddress;
            return View("Index", model);
        }

        public ActionResult ProcessLine3(QuickWinPanelModel model)
        {
            //R24.05 Add loading block screen by max kunlp885
            /*if (checkLoadingBlock("ProcessLine3", ""))
            {
                Session["LoadingBlockModel"] = model;
                return View("ExistingFibreExtensions/_Loading");
            }
            model = Session["LoadingBlockModel"] as QuickWinPanelModel;
            //end R24.05 Add loading block screen by max kunlp885
            */

            string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress))
                ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            ViewBag.v_icon_pb = model.v_icon_pb;
            ViewBag.v_check_pb = model.v_check_pb;
            ViewBag.v_add_playbox = model.v_add_playbox;

            //if (model.TopUp == "1")
            //{
            ViewBag.TopUp = "1";
            model.SummaryPanelModel.TOPUP = "1";
            if (Session["EndProcessFlag"].ToSafeBoolean())
            {
                Session["PopupStatus"] = "Success";
                Session["EndProcessFlag"] = null;
            }
            else
                Session["PopupStatus"] = null;
            ViewBag.LabelFBBTR000 = GetGeneralScreenConfig();
            ViewBag.LabelFBBTR001 = GetCoverageScreenConfig();
            ViewBag.LabelFBBTR002 = GetDisplayPackageScreenConfig();
            ViewBag.LabelFBBTR003 = GetCustRegisterScreenConfig();
            ViewBag.LabelFBBTR004 = GetSummaryScreenConfig();
            ViewBag.LabelFBBTR010 = GetVas_Select_Package_ScreenConfig();
            ViewBag.LabelFBBORV11 = GetVasPopUpScreenConfig();
            ViewBag.FbbConstant = GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);
            ViewBag.FbbException = GetFbbConstantModel(WebConstants.LovConfigName.FbbExceptionIdCard);
            ViewBag.Fbb_Service = GetDisplay_Select_Type_Service();
            ViewBag.SFFProductName = model.outProductName;
            ViewBag.SFFServiceYear = model.outDayOfServiceYear;
            ViewBag.Version = GetVersion();
            ViewBag.ContentPlaybox = GetLovConfigBytype("NEW_CONTENT_PLAYBOX");
            ViewBag.MeshConfig = GetMeshConfig_ScreenConfig();
            //20.3 Service Level
            ViewBag.IsSetServiceLevel = false;
            //R23.06 IP Camera
            ViewBag.IpCameraConfig = GetScreenConfig("FBBTR003");

            if (!string.IsNullOrEmpty(model.outProductName) && !string.IsNullOrEmpty(model.outDayOfServiceYear))
            {
                if (model.TopUp == "1")
                {
                    ViewBag.Vas = "";
                    model.SummaryPanelModel.VAS_FLAG = "";
                }
                else
                {
                    ViewBag.Vas = "2";
                    model.SummaryPanelModel.VAS_FLAG = "2";
                }

                var q = new GetOwnerProductByNoQuery
                {
                    No = model.no,
                    BA_ID = model.CoveragePanelModel.BA_ID
                };
                var aa = _queryProcessor.Execute(q);
                ViewBag.OwnerProduct = aa.Value;
                ViewBag.PackageCode = aa.Value2;
                ViewBag.AddressID = aa.Value3;
                ViewBag.SffProfileLogID = model.SffProfileLogID;
                ViewBag.ContactMobileNo = aa.Value4;
            }
            else
            {
                ViewBag.Vas = "1";
                model.SummaryPanelModel.VAS_FLAG = "1";
            }

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
                        ViewBag.LanguagePage = "1";
                    }
                }
                else
                {
                    if (data.LovValue2 != null)
                    {
                        ViewBag.LCLOSE = data.LovValue2.ToString();
                        ViewBag.LanguagePage = "2";
                    }
                }
            }
            else
            {
                ViewBag.LCLOSE = "";
            }
            model.ClientIP = ipAddress;
            return View("Index", model);
        }

        [HttpPost]
        public ActionResult ProcessLine4(QuickWinPanelModel model)
        {
            //R24.05 Add loading block screen by max kunlp885
            /*if (checkLoadingBlock("ProcessLine4", ""))
            {
                Session["LoadingBlockModel"] = model;
                return View("ExistingFibreExtensions/_Loading");
            }
            model = Session["LoadingBlockModel"] as QuickWinPanelModel;
            //end R24.05 Add loading block screen by max kunlp885
            */

            string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress))
                ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            ViewBag.Vas = "4";
            model.SummaryPanelModel.VAS_FLAG = "4";
            if (Session["EndProcessFlag"].ToSafeBoolean())
            {
                Session["PopupStatus"] = "Success";
                Session["EndProcessFlag"] = null;
            }
            else
                Session["PopupStatus"] = null;

            Logger.Info("Line 4 => Acess through Topup Playbox(BPL) Link");

            ViewBag.LabelFBBTR000 = GetGeneralScreenConfig();
            ViewBag.LabelFBBTR001 = GetCoverageScreenConfig();
            ViewBag.LabelFBBTR002 = GetDisplayPackageScreenConfig();
            ViewBag.LabelFBBTR003 = GetCustRegisterScreenConfig();
            ViewBag.LabelFBBTR004 = GetSummaryScreenConfig();
            ViewBag.LabelFBBTR010 = GetVas_Select_Package_ScreenConfig();
            ViewBag.LabelFBBORV11 = GetVasPopUpScreenConfig();
            ViewBag.LabelFBBORV12 = GetTopUpPlaybox_ScreenConfig();
            ViewBag.LabelFBBORV24 = GetTopUpInternet_ScreenConfig();
            ViewBag.FbbConstant = GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);
            ViewBag.FbbException = GetFbbConstantModel(WebConstants.LovConfigName.FbbExceptionIdCard);
            ViewBag.Fbb_Service = GetDisplay_Select_Type_Service();
            ViewBag.SFFProductName = model.outProductName;
            ViewBag.SFFServiceYear = model.outDayOfServiceYear;
            ViewBag.Version = GetVersion();
            ViewBag.MobileAis = model.CoveragePanelModel.P_MOBILE;
            model.ClientIP = ipAddress;
            ViewBag.ExistingFlag = model.ExistingFlag;
            ViewBag.ContentPlaybox = GetLovConfigBytype("NEW_CONTENT_PLAYBOX");
            //20.3 Service Level
            ViewBag.IsSetServiceLevel = false;
            ViewBag.MeshConfig = GetMeshConfig_ScreenConfig();
            //R23.06 IP Camera
            ViewBag.IpCameraConfig = GetScreenConfig("FBBTR003");

            //R17.9 Speed boost
            if (model.PackagePromotionList.Any())
            {
                PackageTopupInternetNotUseModel NotUseData = new PackageTopupInternetNotUseModel();
                List<CurrentPromotionData> ListCurrentPromotion = new List<CurrentPromotionData>();
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
                foreach (var row in model.PackagePromotionList)
                {
                    row.IsShowPacage = "Y";
                    if (!string.IsNullOrEmpty(row.ProductCd) && !string.IsNullOrEmpty(row.EndDate))
                    {
                        if (row.EndDate.ToDateTime() > DateTime.Now)
                        {
                            row.IsShowPacage = "N";
                        }
                    }
                    CurrentPromotionData currentPromotionData = new CurrentPromotionData()
                    {
                        product_cd = row.ProductCd,
                        product_class = row.ProductClass,
                        start_date = row.StartDate,
                        end_date = row.EndDate,
                        product_status = row.ProductStatus
                    };
                    ListCurrentPromotion.Add(currentPromotionData);
                }
                PackageTopupInternetNotUseQuery queryNotUse = new PackageTopupInternetNotUseQuery
                {
                    NonMobileNo = model.CoveragePanelModel.P_MOBILE,
                    ListCurrentPromotion = ListCurrentPromotion
                };

                NotUseData = _queryProcessor.Execute(queryNotUse);

                if (NotUseData != null && NotUseData.ListPackageTopupInternetNotUse != null && NotUseData.ListPackageTopupInternetNotUse.Count > 0)
                {
                    model.PackageTopupInternetNotUseList = NotUseData.ListPackageTopupInternetNotUse.Where(a => !string.IsNullOrEmpty(a.sff_promotion_code)).ToList();
                }
            }

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
                        ViewBag.LanguagePage = "1";
                    }
                }
                else
                {
                    if (data.LovValue2 != null)
                    {
                        ViewBag.LCLOSE = data.LovValue2.ToString();
                        ViewBag.LanguagePage = "2";
                    }
                }
            }
            else
            {
                ViewBag.LCLOSE = "";
            }

            return View("Index", model);
        }

        [HttpPost]
        public ActionResult ProcessLineOfficer(string LcCode = "", string ASCCode = "", string EmployeeID = "", string outType = ""
            , string outSubType = "", string outMobileNo = "", string PartnerName = ""
            , string outLocationEmailByRegion = "", LeaveMessageDataModel leaveMessageDataModel = null, string outEmpName = ""
            , string outTitle = "", string outCompanyName = "", string outDistChn = "", string outChnSales = "", string outShopType = ""
            , string outOperatorClass = "", string outASCTitleThai = "", string outASCPartnerName = "", string outMemberCategory = "", string outPosition = ""
            , string outLocationRegion = "", string outLocationSubRegion = "", string THFirstName = "", string THLastName = "", string outLocationProvince = ""
            )
        {

            ViewBag.LcCode = LcCode;
            ViewBag.ASCCode = ASCCode;
            ViewBag.EmployeeID = EmployeeID;
            ViewBag.outType = outType;
            ViewBag.outSubType = outSubType;
            ViewBag.outMobileNo = outMobileNo;
            ViewBag.PartnerName = PartnerName;
            ViewBag.outLocationEmailByRegion = outLocationEmailByRegion;
            ViewBag.outEmpName = outEmpName;

            Session["FullUrl"] = this.Url.Action("Index", "Officer", null, this.Request.Url.Scheme);
            string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress))
            {
                ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }


            var PlugAndPlayFlow = "";

            Logger.Info("Access OFFICER Line => LcCode:" + LcCode + ", ASCCode:" + ASCCode + ", EmployeeID:" + EmployeeID + ", outType:" + outType + ", outSubType:" + outSubType + ", outMobileNo:" + outMobileNo + ", PartnerName:" + PartnerName);

            //var hello = base.LovData.Where(l => !string.IsNullOrEmpty(l.Type) && l.Type.Equals("FBB_CONSTANT")).ToList();
            //var hello3 = hello.Where(l => !string.IsNullOrEmpty(l.LovValue1) && l.LovValue1.Equals(outType)).ToList();
            //var hello4 = hello3.Where(l => !string.IsNullOrEmpty(l.LovValue2) && l.LovValue2.Equals(outSubType)).ToList();
            //var hello2 = hello4.Where(l => !string.IsNullOrEmpty(l.Name) && l.Name.Equals("PLUG_AND_PLAY_LOCATION")).ToList();

            var isPlugAndPlay = base.LovData
                   .Where(l => (l.Type == "FBB_CONSTANT" && l.Name == "PLUG_AND_PLAY_LOCATION"
                                && l.LovValue1 == outType && l.LovValue2 == outSubType))
                   .Select(l => new FbbConstantModel
                   {
                       Field = l.Name,
                       Validation = l.LovValue1,
                       SubValidation = l.LovValue2
                   })
                   .Union(base.LovData
                   .Where(l => (l.Type == "FBB_CONSTANT" && l.Name == "PLUG_AND_PLAY_LOCATION"
                                && l.LovValue1 == LcCode))
                   .Select(l => new FbbConstantModel
                   {
                       Field = l.Name,
                       Validation = l.LovValue1,
                   })).ToList();

            if (isPlugAndPlay.Any())
                PlugAndPlayFlow = "N";

            var grouIdSaleQuery = new GetLovQuery
            {
                LovType = "FBB_CONSTANT",
                LovName = "USER_SALE_MNG_GROUP_ID"
            };
            var groupId = _queryProcessor.Execute(grouIdSaleQuery);
            Session["isSaleManager"] = null;
            var authenticatedUser = GetUserPinCode(EmployeeID, groupId[0].LovValue1);
            if (null == authenticatedUser.UserName || (authenticatedUser.Groups == null))
                Session["isSaleManager"] = "false";
            else
                Session["isSaleManager"] = "true";

            var model = new QuickWinPanelModel();
            model.TopUp = "5";
            model.SummaryPanelModel.TOPUP = "5";
            model.CustomerRegisterPanelModel.L_ASC_CODE = ASCCode;
            model.CustomerRegisterPanelModel.L_LOC_CODE = LcCode;
            model.CustomerRegisterPanelModel.L_STAFF_ID = EmployeeID;
            model.CustomerRegisterPanelModel.outType = outType;
            model.CustomerRegisterPanelModel.outSubType = outSubType;
            model.CustomerRegisterPanelModel.outMobileNo = outMobileNo;
            model.CustomerRegisterPanelModel.PartnerName = PartnerName;
            model.CustomerRegisterPanelModel.LOCATION_EMAIL_BY_REGION = outLocationEmailByRegion;
            model.CustomerRegisterPanelModel.EMP_NAME = outEmpName;
            model.PlugAndPlayFlow = PlugAndPlayFlow;

            //20.3 Channel Report
            model.OfficerInfoPanelModel.outTitle = outTitle;
            model.OfficerInfoPanelModel.outCompanyName = outCompanyName;
            model.OfficerInfoPanelModel.outDistChn = outDistChn;
            model.OfficerInfoPanelModel.outChnSales = outChnSales;
            model.OfficerInfoPanelModel.outShopType = outShopType;
            model.OfficerInfoPanelModel.outOperatorClass = outOperatorClass;
            model.OfficerInfoPanelModel.outASCTitleThai = outASCTitleThai;
            model.OfficerInfoPanelModel.outASCPartnerName = outASCPartnerName;
            model.OfficerInfoPanelModel.outMemberCategory = outMemberCategory;
            model.OfficerInfoPanelModel.outPosition = outPosition;
            model.OfficerInfoPanelModel.outLocationRegion = outLocationRegion;
            model.OfficerInfoPanelModel.outLocationSubRegion = outLocationSubRegion;
            model.OfficerInfoPanelModel.THFirstName = THFirstName;
            model.OfficerInfoPanelModel.THLastName = THLastName;
            model.OfficerInfoPanelModel.outLocationProvince = outLocationProvince;

            ViewBag.outTitle = outTitle;
            ViewBag.outCompanyName = outCompanyName;
            ViewBag.outDistChn = outDistChn;
            ViewBag.outChnSales = outChnSales;
            ViewBag.outShopType = outShopType;
            ViewBag.outOperatorClass = outOperatorClass;
            ViewBag.outASCTitleThai = outASCTitleThai;
            ViewBag.outASCPartnerName = outASCPartnerName;
            ViewBag.outMemberCategory = outMemberCategory;
            ViewBag.outPosition = outPosition;
            ViewBag.outLocationRegion = outLocationRegion;
            ViewBag.outLocationSubRegion = outLocationSubRegion;
            ViewBag.THFirstName = THFirstName;
            ViewBag.THLastName = THLastName;
            //R21.5 Pool Villa
            ViewBag.outLocationProvince = outLocationProvince;

            //17.7  Member get member
            if (leaveMessageDataModel != null && !string.IsNullOrEmpty(leaveMessageDataModel.REF_NO))
            {
                model.MemberGetMemberFlag = "Y";
                model.CoveragePanelModel.CoverageMemberGetMember.IM_ORDER = leaveMessageDataModel.IM_ORDER;
                model.CoveragePanelModel.CoverageMemberGetMember.RefferenceNo = leaveMessageDataModel.REF_NO;
                model.CoveragePanelModel.CoverageMemberGetMember.SERVICE_CASE_ID = leaveMessageDataModel.SERVICE_CASE_ID;
                model.CoveragePanelModel.CoverageMemberGetMember.BuildingName = leaveMessageDataModel.BUILDING_NAME;
                model.CoveragePanelModel.CoverageMemberGetMember.VillageName = leaveMessageDataModel.VILLAGE_NAME;
                model.CoveragePanelModel.CoverageMemberGetMember.Province = leaveMessageDataModel.PROVINCE;
                model.CoveragePanelModel.CoverageMemberGetMember.Distrinct = leaveMessageDataModel.DISTRICT;
                model.CoveragePanelModel.CoverageMemberGetMember.SubDistrict = leaveMessageDataModel.SUB_DISTRICT;
                model.CoveragePanelModel.CoverageMemberGetMember.PostCode = leaveMessageDataModel.POSTAL_CODE;
                model.CoveragePanelModel.CoverageMemberGetMember.HouseNo = leaveMessageDataModel.HOUSE_NO;
                model.CoveragePanelModel.CoverageMemberGetMember.Soi = leaveMessageDataModel.SOI;
                model.CoveragePanelModel.CoverageMemberGetMember.Road = leaveMessageDataModel.ROAD;
                model.RegisterChannel = leaveMessageDataModel.CHANNEL;
                model.CoveragePanelModel.CoverageMemberGetMember.Language = leaveMessageDataModel.LANGUAGE;
                model.CoveragePanelModel.CoverageMemberGetMember.CustomerName = leaveMessageDataModel.CUST_NAME;
                model.CoveragePanelModel.CoverageMemberGetMember.CustomerSurname = leaveMessageDataModel.CUST_SURNAME;
                model.CoveragePanelModel.CoverageMemberGetMember.CustomerEmail = leaveMessageDataModel.CONTACT_EMAIL;
                model.CoveragePanelModel.CoverageMemberGetMember.CustomerMobileNo = leaveMessageDataModel.CONTACT_MOBILE_NO;
                model.CoveragePanelModel.CoverageMemberGetMember.CustomerLineId = leaveMessageDataModel.LINE_ID;
                model.CoveragePanelModel.CoverageMemberGetMember.VoucherDesc = leaveMessageDataModel.VOUCHER_DESC;
                model.CoveragePanelModel.CoverageMemberGetMember.CampaignProjectName = leaveMessageDataModel.CAMPAIGN_PROJECT_NAME;
                model.CoveragePanelModel.CoverageMemberGetMember.VoucherPin = leaveMessageDataModel.RFF_INTERNET_NO;
                model.CoveragePanelModel.CoverageMemberGetMember.AddressType = leaveMessageDataModel.ADDRESS_TYPE;
                model.CoveragePanelModel.CoverageMemberGetMember.FULL_ADDRESS = leaveMessageDataModel.FULL_ADDRESS;
                model.CoveragePanelModel.LOCATION = leaveMessageDataModel.LOCATION.ToSafeString();
                model.CoveragePanelModel.CoverageMemberGetMember.FLOOR = leaveMessageDataModel.FLOOR;
                model.CoveragePanelModel.CoverageMemberGetMember.MOO = leaveMessageDataModel.MOO;
                model.CoveragePanelModel.CoverageMemberGetMember.BUILDING_NO = leaveMessageDataModel.BUILDING_NO;
                model.CoveragePanelModel.CoverageMemberGetMember.RELATE_MOBILE_NO = leaveMessageDataModel.RELATE_MOBILE_NO.ToSafeString();
                model.CoveragePanelModel.CoverageMemberGetMember.FBB_PERCENT_DISCOUNT = leaveMessageDataModel.FBB_PERCENT_DISCOUNT.ToSafeString();
                //20.9
                if (leaveMessageDataModel.SALES_REP.ToSafeString() != "")
                {
                    model.CustomerRegisterPanelModel.L_SALE_REP = leaveMessageDataModel.SALES_REP.ToSafeString();
                }
                var culture = leaveMessageDataModel.LANGUAGE == "T" ? 1 : 2;
                SiteSession.CurrentUICulture = culture;
                Session[WebConstants.SessionKeys.CurrentUICulture] = culture;
            }

            Session["OfficerModel"] = model;
            Session["StaffModel"] = null;
            Session["EnginerrLine"] = null;
            Session["TripleLine"] = null;
            Session["ProcessLine1"] = null;

            ViewBag.Vas = "";
            ViewBag.officer = "";
            if (Session["EndProcessFlag"].ToSafeBoolean())
            {
                Session["PopupStatus"] = "Success";
                Session["EndProcessFlag"] = null;
            }
            else
                Session["PopupStatus"] = null;

            /*
            //R24.05 Add loading block screen by max kunlp885
            if (checkLoadingBlock("ProcessLineOfficer", ""))
            {
                return View("ExistingFibreExtensions/_Loading");
            }
            model = Session["OfficerModel"] as QuickWinPanelModel;
            //end R24.05 Add loading block screen by max kunlp885
            */


            ViewBag.LabelFBBTR000 = GetGeneralScreenConfig();
            ViewBag.LabelFBBTR001 = GetCoverageScreenConfig();
            ViewBag.LabelFBBTR002 = GetDisplayPackageScreenConfig();
            ViewBag.LabelFBBTR003 = GetCustRegisterScreenConfig();
            ViewBag.LabelFBBTR004 = GetSummaryScreenConfig();
            ViewBag.LabelFBBTR010 = GetVas_Select_Package_ScreenConfig();
            ViewBag.LabelFBBORV11 = GetVasPopUpScreenConfig();
            ViewBag.FbbConstant = GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);
            ViewBag.FbbException = GetFbbConstantModel(WebConstants.LovConfigName.FbbExceptionIdCard);
            ViewBag.Fbb_Service = GetDisplay_Select_Type_Service();
            ViewBag.SFFProductName = model.outProductName;
            ViewBag.SFFServiceYear = model.outDayOfServiceYear;
            ViewBag.Version = GetVersion();
            //update16.3
            ViewBag.LabelFBBOR015 = GetProfilePrePostPaid();

            ViewBag.MeshConfig = GetMeshConfig_ScreenConfig();
            //R23.06 IP Camera
            ViewBag.IpCameraConfig = GetScreenConfig("FBBTR003");

            //ViewBag.Deverloper = GetScreenConfig("DEVELOPER");
            Logger.Info("Line Login => Acess through Officer");


            model.ClientIP = ipAddress;
            return View("New_SearchProfilePrePostPaid", model);
        }

        [HttpPost]
        public ActionResult ProcessRegister(string TransactionID = "",
            string language = "", string siteCode = "", string buildingName = "", string houseNo = "", string moo = "",
            string soi = "", string road = "", string subDistrict = "", string district = "", string province = "",
            string postCode = "", string latitude = "", string longitude = "", string phoneFlag = "", string coverage = "", string addressFlag = "", string TriplePlayFlag = "N",
            string SplitterList = null, string FlowFlag = "", string AddressID = "", string status = "", string subStatus = ""
            )
        {
            bool is3BBSit = false;
            string MobileNo = "";
            if (Session["LoadCheckCoverageModel"] != null)
            {
                QuickWinPanelModel modeltmp = Session["LoadCheckCoverageModel"] as QuickWinPanelModel;
                MobileNo = modeltmp.CustomerRegisterPanelModel.L_MOBILE.ToSafeString();
            }
            Stopwatch timer = Stopwatch.StartNew();

            //Decode Special character
            TransactionID = WebUtility.UrlDecode(TransactionID);
            language = WebUtility.UrlDecode(language);
            siteCode = WebUtility.UrlDecode(siteCode);
            buildingName = WebUtility.UrlDecode(buildingName);
            houseNo = WebUtility.UrlDecode(houseNo);
            moo = WebUtility.UrlDecode(moo);
            soi = WebUtility.UrlDecode(soi);
            road = WebUtility.UrlDecode(road);
            subDistrict = WebUtility.UrlDecode(subDistrict);
            district = WebUtility.UrlDecode(district);
            province = WebUtility.UrlDecode(province);
            postCode = WebUtility.UrlDecode(postCode);
            latitude = WebUtility.UrlDecode(latitude);
            longitude = WebUtility.UrlDecode(longitude);
            phoneFlag = WebUtility.UrlDecode(phoneFlag);
            coverage = WebUtility.UrlDecode(coverage);
            addressFlag = WebUtility.UrlDecode(addressFlag);
            TriplePlayFlag = WebUtility.UrlDecode(TriplePlayFlag);
            FlowFlag = WebUtility.UrlDecode(FlowFlag);
            SplitterList = WebUtility.UrlDecode(SplitterList);
            AddressID = WebUtility.UrlDecode(AddressID);
            status = WebUtility.UrlDecode(status);
            subStatus = WebUtility.UrlDecode(subStatus);

            SplitterInfoList SplitterListResult = new SplitterInfoList();
            //SplitterInfo3bbList SplitterList3bbResult = new SplitterInfo3bbList();
            List<SplitterInfo3bb> List3bbResult = new List<SplitterInfo3bb>();

            if (siteCode == "3BB")
            {
                is3BBSit = true;
            }
            else
            {
                if (!string.IsNullOrEmpty(SplitterList))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(SplitterInfoList));
                    using (TextReader reader = new StringReader(SplitterList))
                    {
                        SplitterListResult = (SplitterInfoList)serializer.Deserialize(reader);
                    }
                }
            }

            string loginfo = "";
            loginfo += "Recieve Parameter from ESRI: \r\n";
            loginfo += "TransactionID:" + TransactionID + "\r\n";
            loginfo += "language:" + language + "\r\n";
            loginfo += "siteCode:" + siteCode + "\r\n";
            loginfo += "buildingName:" + buildingName + "\r\n";
            loginfo += "houseNo:" + houseNo + "\r\n";
            loginfo += "moo:" + moo + "\r\n";
            loginfo += "soi:" + soi + "\r\n";
            loginfo += "road:" + road + "\r\n";
            loginfo += "subDistrict:" + subDistrict + "\r\n";
            loginfo += "district:" + district + "\r\n";
            loginfo += "province:" + province + "\r\n";
            loginfo += "postCode:" + postCode + "\r\n";
            loginfo += "latitude:" + latitude + "\r\n";
            loginfo += "longitude:" + longitude + "\r\n";
            loginfo += "phoneFlag:" + phoneFlag + "\r\n";
            loginfo += "coverage:" + coverage + "\r\n";
            loginfo += "addressFlag:" + addressFlag + "\r\n";
            loginfo += "TriplePlayFlag:" + TriplePlayFlag + "\r\n";
            loginfo += "SplitterList:" + SplitterList;
            loginfo += "FlowFlag:" + FlowFlag + "\r\n";
            loginfo += "AddressID:" + AddressID + "\r\n";
            base.Logger.Info(loginfo);

            InterfaceLogCommand log = null;
            log = StartInterface(loginfo, "/process/ProcessRegister", TransactionID, "", "ESRI");

            try
            {
                var feastCheckFlag = "";
                if (string.IsNullOrEmpty(TransactionID) || coverage == "N")
                {
                    if (!string.IsNullOrEmpty(TransactionID))
                    {
                        var up = Bootstrapper.GetInstance<CheckCoverageController>();
                        var chkCoverage = false;
                        if (Session["CheckCoverage"] != null)
                            chkCoverage = true;
                        else
                            chkCoverage = false;

                        string statusUpdate = "";

                        try
                        {
                            if (Session["OfficerModel"] != null)  /// case officer
                            {
                                var model = Session["OfficerModel"] as QuickWinPanelModel;

                                up.FBSSCoverageResultCommand(
                                    actionType: "Update",
                                    interfaceId: TransactionID,
                                    addrType: "ESRI",
                                    buildName: buildingName,
                                    addrNo: houseNo,
                                    moo: moo.ToSafeDecimal(),
                                    soi: soi,
                                    road: road,
                                    chkCoverage: chkCoverage,
                                    coverage: coverage,
                                    lcCode: model.CustomerRegisterPanelModel.L_LOC_CODE,
                                    ascCode: model.CustomerRegisterPanelModel.L_ASC_CODE,
                                    employeeID: model.CustomerRegisterPanelModel.L_STAFF_ID,
                                    saleFirstname: model.OfficerInfoPanelModel.THFirstName,
                                    saleLastname: model.OfficerInfoPanelModel.THLastName,
                                    locationName: model.CustomerRegisterPanelModel.PartnerName,
                                    subRegion: model.OfficerInfoPanelModel.outLocationSubRegion,
                                    region: model.OfficerInfoPanelModel.outLocationRegion,
                                    ascName: model.OfficerInfoPanelModel.outASCPartnerName,
                                    channelName: model.CustomerRegisterPanelModel.PartnerName,
                                    saleChannel: model.OfficerInfoPanelModel.outChnSales,
                                    //R21.2
                                    addressTypeDTL: "",
                                    remark: "",
                                    technology: "",
                                    projectName: ""
                                );
                            }
                            else
                            {
                                up.FBSSCoverageResultCommand(
                                    actionType: "Update",
                                    interfaceId: TransactionID,
                                    addrType: "ESRI",
                                    buildName: buildingName,
                                    addrNo: houseNo,
                                    moo: moo.ToSafeDecimal(),
                                    soi: soi,
                                    road: road,
                                    chkCoverage: chkCoverage,
                                    coverage: coverage
                                );
                            }

                            statusUpdate = "0";
                        }
                        catch
                        {
                            statusUpdate = "-1";
                        }

                        //19.3 : Send to IM Service.
                        if (TriplePlayFlag.IndexOf("$") != -1)
                        {
                            string[] spData = TriplePlayFlag.Split('$');
                            var checkCoverageCtrl = Bootstrapper.GetInstance<CheckCoverageController>();
                            var resultData = checkCoverageCtrl.FBSSCoverageResultQuery(
                               resultId: 0,
                               transactionId: TransactionID,
                               assetNumber: spData[0],
                               caseId: spData[1],
                               referenceNoStatus: "0"
                           );
                            //Command post service.
                            resultData.addressBuilding = "";
                            resultData.referenceNoStatus = statusUpdate;
                            _getLeaveMsgRefCommand.Handle(resultData);
                        }
                    }

                    //17.7 Member get Member
                    //Update status preregister
                    var reffno = TriplePlayFlag.Split('|');
                    if (reffno.Length > 1)
                    {
                        var preRegisterStatus = new UpdatePreregisterStatusPackageCommand
                        {
                            p_refference_no = reffno[1],
                            p_status = "Cancel (No Coverage)"
                        };
                        _updatePregisterCommand.Handle(preRegisterStatus);

                        //R22.06 Member get Member Send SMS Case Out of Coverage
                        if (Session["OfficerModel"] != null)
                        {
                            var model = Session["OfficerModel"] as QuickWinPanelModel;
                            var LovCampaignProjectNameMGM = GetLovCampaignProjectNameMGM("CAMPAIGN_PROJECT_NAME");
                            if (coverage == "N" && model.CoveragePanelModel.CoverageMemberGetMember.CampaignProjectName == LovCampaignProjectNameMGM)
                            {
                                var languageMGM = SiteSession.CurrentUICulture.IsThaiCulture() ? "T" : "E";
                                var fullUrl = this.Url.Action("", "fbbsaleportal", null, this.Request.Url.Scheme);
                                string ClientIP = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                                if (string.IsNullOrEmpty(ClientIP))
                                {
                                    ClientIP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                                }

                                var sendSmsMGM = new SendSmsMGMCommand
                                {
                                    p_refference_no = reffno[1],
                                    p_coverage_result = "NO",
                                    p_mgm_flag = model.MemberGetMemberFlag,
                                    p_language = languageMGM,
                                    ClientIP = ClientIP,
                                    FullUrl = fullUrl
                                };
                                _sendSmsMGMCommand.Handle(sendSmsMGM);
                            }
                        }
                    }

                    Session["ESRI"] = null;
                    Session["ESRI_COVERAGEMODEL"] = null;
                    Session["ESRI_BYPASSCOVERAGEMODEL"] = null;
                    Session["EOS_COVERAGEMODEL"] = null;
                }
                else /// coverage Y and TransactionID not null
                {
                    bool isWTTX = false;
                    string wttxGridId = "", wttxMaxBandWith = "", wttxCoverageResult = "";

                    var model = new QuickWinPanelModel();
                    model.TriplePlayFlag = TriplePlayFlag.IndexOf("$") != -1 ? "N" : TriplePlayFlag; /// add trilpleplay flag for old customer

                    if (Session["OfficerModel"] != null)  /// case officer
                    {
                        model = Session["OfficerModel"] as QuickWinPanelModel;
                        model.TriplePlayFlag = TriplePlayFlag;
                        base.Logger.Info("Process Register case OFFICER");
                    }

                    if (Session["StaffModel"] != null)  /// case staff
                    {
                        model = Session["StaffModel"] as QuickWinPanelModel;
                        model.TriplePlayFlag = "N";
                        base.Logger.Info("Process Register case STAFF");
                    }

                    if (Session["StaffPrivilegeBypass"] != null)
                    {
                        model = Session["StaffPrivilegeBypass"] as QuickWinPanelModel;
                    }

                    //20.2 : WTTx
                    if (!String.IsNullOrEmpty(AddressID) && siteCode == "WTTX")
                    {
                        isWTTX = true;

                        var ctrlCheckCoverage = Bootstrapper.GetInstance<CheckCoverageController>();
                        //Get WTTX Info
                        var resultWTTXInfo = ctrlCheckCoverage.GetWTTXInfo(AddressID, MobileNo);
                        if (resultWTTXInfo.RESULT_CODE == "0")
                        {
                            wttxGridId = resultWTTXInfo.GRIDID;
                            wttxMaxBandWith = resultWTTXInfo.MAXBANDWITH;
                            if (resultWTTXInfo.ONSERVICE == "YES" && resultWTTXInfo.NUMBEROFCUSTOMER < resultWTTXInfo.MAXCUSTOMER)
                                wttxCoverageResult = "YES";
                            else if (resultWTTXInfo.ONSERVICE == "YES" && resultWTTXInfo.NUMBEROFCUSTOMER >= resultWTTXInfo.MAXCUSTOMER)
                                wttxCoverageResult = "FULL";
                            else
                                wttxCoverageResult = "NO";
                        }
                        else
                            wttxCoverageResult = "ERROR";
                    }

                    model.SiteCode = siteCode;
                    model.FlowFlag = FlowFlag;

                    Stopwatch timer2 = Stopwatch.StartNew();


                    #region [FWA]

                    base.Logger.Info("Begin [FWA]");
                    var q = new GetCoverageAreaResultQuery
                    {
                        TRANSACTION_ID = TransactionID
                    };
                    var result = _queryProcessor.Execute(q);

                    timer2.Stop();
                    TimeSpan timespan2 = timer2.Elapsed;

                    model.CoveragePanelModel.RESULT_ID = result.RESULTID.ToSafeString();
                    base.Logger.Info("Result GetCoverageAreaResultQuery:" + result);

                    model.CoverageAreaResultModel = result;

                    model.CoveragePanelModel.Address.ZIPCODE_ID = result.ZIPCODE_ROWID.ToSafeString();
                    base.Logger.Info(" [FWA] ทำทุกครั้ง เพราะเอาดาต้าไปใช้ต่อ \r\n");

                    #endregion [FWA]

                    #region siteCode เป็น [FTTH ระดับตำบล]

                    if (siteCode != "" && siteCode != "FWA")
                    {
                        base.Logger.Info("Begin [FTTHTumBON]");
                        var FHHTumbol = base.LovData.Where(l => l.Name == "MAPPING_OWNER_PRODUCT" && l.LovValue1 == "FTTH").Select(o => o.LovValue3);

                        if (FHHTumbol.ToList().Contains(siteCode))
                        {
                            model.CallfeascheckFlag = "Y";
                            if (siteCode != "AWN")
                            {
                                model.FlowFlag = "0";
                            }
                        }
                        else
                        {
                            model.CallfeascheckFlag = "N";
                        }
                        base.Logger.Info("siteCode เป็น [FTTH ระดับตำบล]" + "model.CallfeascheckFlag:" + model.CallfeascheckFlag);
                    }

                    #endregion siteCode เป็น [FTTH ระดับตำบล]

                    base.Logger.Info("Begin Map model CoverageAreaResultModel");

                    var tempNameTH = base.LovData.Where(a => a.Name == "ESRI_REPLACE_NAME_TH").Select(a => new { a.LovValue1, a.LovValue2 });
                    var tempNameEN = base.LovData.Where(a => a.Name == "ESRI_REPLACE_NAME_EN").Select(a => new { a.LovValue1, a.LovValue2 });

                    if (language.ToSafeString() == "1") // TH
                    {
                        base.Logger.Info("Begin Replace Address TH");

                        var protemp = from a in tempNameTH
                                      where a.LovValue1 == province.ToSafeString()
                                      select a.LovValue2;
                        base.Logger.Info("Province ESRI LOV TH:" + protemp.FirstOrDefault() + "\r\n" + "Province ESRI Parameter TH:" + province);

                        if (protemp.FirstOrDefault() != "" && protemp.FirstOrDefault() != null) { province = protemp.FirstOrDefault(); base.Logger.Info("In REplace Province TH"); }

                        var aumtemp = from a in tempNameTH
                                      where a.LovValue1 == district.ToSafeString()
                                      select a.LovValue2;
                        base.Logger.Info("Aumphur ESRI LOV TH:" + aumtemp.FirstOrDefault() + "\r\n" + "Aumphur ESRI Parameter TH:" + district);

                        if (aumtemp.FirstOrDefault() != "" && aumtemp.FirstOrDefault() != null) { district = aumtemp.FirstOrDefault(); base.Logger.Info("In REplace Aumphur TH"); }

                        var tumtemp = from a in tempNameTH
                                      where a.LovValue1 == subDistrict.ToSafeString()
                                      select a.LovValue2;
                        base.Logger.Info("Tumbon ESRI LOV TH:" + tumtemp.FirstOrDefault() + "\r\n" + "Tumbon ESRI Parameter TH:" + subDistrict);

                        if (tumtemp.FirstOrDefault() != null && tumtemp.FirstOrDefault() != "") { subDistrict = tumtemp.FirstOrDefault(); base.Logger.Info("In REplace Tumbon TH"); }

                        base.Logger.Info("End Begin Replace Address TH");
                    }
                    else // EN
                    {
                        base.Logger.Info("Begin Replace Address EN");

                        #region province

                        var protemp = from a in tempNameEN
                                      where a.LovValue1.ToUpper() == province.ToSafeString().ToUpper()
                                      select a.LovValue2;

                        var weprovince = base.ZipCodeData(SiteSession.CurrentUICulture)
                        .Where(z => (!string.IsNullOrEmpty(z.Province) && z.Province.ToUpper().Equals(province.ToSafeString().ToUpper())))
                        .Select(z => z.Province).FirstOrDefault();

                        base.Logger.Info("Province ESRI LOV EN:" + protemp.FirstOrDefault()
                            + "\r\n" + "Province We EN:" + weprovince
                            + "\r\n" + "Province ESRI Parameter EN:" + province);

                        if (protemp.FirstOrDefault() != null && protemp.FirstOrDefault() != "")
                        {
                            province = protemp.FirstOrDefault();
                            base.Logger.Info("In REplace Province EN in Lov");
                        }
                        else
                        {
                            if (weprovince != null && weprovince != "")
                            {
                                province = weprovince;
                                base.Logger.Info("In REplace Province EN in WeZipcode");
                            }
                            else
                            {
                                base.Logger.Info("ESRI Province parameter not in WeZipcode");
                            }
                        }

                        #endregion province

                        #region aumphur

                        var aumtemp = from a in tempNameEN
                                      where a.LovValue1.ToUpper() == district.ToSafeString().ToUpper()
                                      select a.LovValue2;

                        var wedistrict = base.ZipCodeData(SiteSession.CurrentUICulture)
                       .Where(z => (!string.IsNullOrEmpty(z.Amphur) && z.Amphur.ToUpper().Equals(district.ToSafeString().ToUpper())))
                       .Select(z => z.Amphur).FirstOrDefault();

                        base.Logger.Info("Aumphur ESRI LOV TH:" + aumtemp.FirstOrDefault()
                            + "\r\n" + "Aumphur We EN:" + wedistrict
                            + "\r\n" + "Aumphur ESRI Parameter EN:" + district);

                        if (aumtemp.FirstOrDefault() != null && aumtemp.FirstOrDefault() != "")
                        {
                            district = aumtemp.FirstOrDefault();
                            base.Logger.Info("In REplace Aumphur EN");
                        }
                        else
                        {
                            if (wedistrict != null && wedistrict != "")
                            {
                                district = wedistrict;
                                base.Logger.Info("In REplace Aumphur EN in WeZipcode");
                            }
                            else
                            {
                                base.Logger.Info("ESRI Province Aumhpur not in WeZipcode");
                            }
                        }

                        #endregion aumphur

                        #region tumbon

                        var tumtemp = from a in tempNameEN
                                      where a.LovValue1.ToUpper() == subDistrict.ToSafeString().ToUpper()
                                      select a.LovValue2;

                        var wesubdistrict = base.ZipCodeData(SiteSession.CurrentUICulture)
                        .Where(z => (!string.IsNullOrEmpty(z.Tumbon) && z.Tumbon.ToUpper().Equals(subDistrict.ToSafeString().ToUpper())))
                        .Select(z => z.Tumbon).FirstOrDefault();

                        base.Logger.Info("Tumbon ESRI LOV EN:" + tumtemp.FirstOrDefault()
                            + "\r\n" + "Tumbon We EN:" + wesubdistrict
                            + "\r\n" + "Tumbon ESRI Parameter EN:" + subDistrict);

                        if (tumtemp.FirstOrDefault() != null && tumtemp.FirstOrDefault() != "")
                        {
                            subDistrict = tumtemp.FirstOrDefault();
                            base.Logger.Info("In REplace Tumbon EN");
                        }
                        else
                        {
                            if (wesubdistrict != null && wesubdistrict != "")
                            {
                                subDistrict = wesubdistrict;
                                base.Logger.Info("In REplace Tumbon EN in WeZipcode");
                            }
                            else
                            {
                                base.Logger.Info("ESRI Province Tumbon not in WeZipcode");
                            }
                        }

                        #endregion tumbon

                        base.Logger.Info("End Replace Address EN");
                    }

                    XDocument doc = XDocument.Parse(model.CoverageAreaResultModel.ACCESS_MODE_LIST.ToSafeString());

                    foreach (XElement hashElement in doc.Descendants("AccessMode"))
                    {
                        string hashValue = (string)hashElement;
                        model.CoverageAreaResultModel.ACCESS_MODE_LIST = hashValue;
                        base.Logger.Info("get accessmode from xml:" + hashValue);
                    }

                    base.Logger.Info("Begin Map model CoverageAreaResultModel");
                    model.CoverageAreaResultModel.PROVINCE = province.ToSafeString();
                    model.CoverageAreaResultModel.AUMPHUR = district.ToSafeString();
                    model.CoverageAreaResultModel.SUB_DISTRICT_NAME = subDistrict.ToSafeString();
                    model.CoverageAreaResultModel.TRANSACTION_ID = TransactionID.ToSafeString();
                    model.CoverageAreaResultModel.LANGUAGE = language.ToSafeString();
                    model.CoverageAreaResultModel.BUILDING_NAME = buildingName.ToSafeString();
                    model.CoverageAreaResultModel.ADDRESS_NO = houseNo.ToSafeString();
                    model.CoverageAreaResultModel.BUILDING_NO = "";
                    model.CoverageAreaResultModel.MOO = (moo.ToSafeString() == "") ? 0 : moo.ToSafeDecimal();
                    model.CoverageAreaResultModel.SOI = soi.ToSafeString();
                    model.CoverageAreaResultModel.ROAD = road.ToSafeString();
                    model.CoverageAreaResultModel.POSTAL_CODE = postCode.ToSafeString();
                    model.CoverageAreaResultModel.LATITUDE = latitude.ToSafeString();
                    model.CoverageAreaResultModel.LONGITUDE = longitude.ToSafeString();
                    model.CoverageAreaResultModel.PHONE_FLAG = phoneFlag.ToSafeString();
                    model.CoverageAreaResultModel.EXCLUSIVE_3BB = "";
                    model.address_flag = addressFlag;
                    model.UseMap = "True";

                    if (is3BBSit)
                    {
                        model.Is3bbCoverage = "Y";
                        if (!string.IsNullOrEmpty(SplitterList))
                        {
                            model.CoverageAreaResultModel.SPLITTER_LIST = new List<SPLITTER_INFO>();
                            model.CoverageAreaResultModel.SPLITTER_3BB_LIST = new List<SPLITTER_3BB_INFO>();
                            model.CoverageAreaResultModel.SPLITTER_3BB_JSON = SplitterList;
                            //model.CoverageAreaResultModel.SPLITTER_3BB_LIST = SplitterList3bbResult.Splitter.ConvertAll(x => new SPLITTER_3BB_INFO
                            //{
                            //    Splitter_Code = x.SplitterCode.ToSafeString(),
                            //    Splitter_Alias = x.SplitterAlias.ToSafeString(),
                            //    Distance = x.Distance.ToSafeDecimal(),
                            //    Distance_Type = x.DistanceType.ToSafeString(),
                            //    Splitter_Latitude = x.SplitterLatitude.ToSafeString(),
                            //    Splitter_Longitude = x.SplitterLongitude.ToSafeString(),
                            //    Splitter_Port = x.SplitterPort.ToSafeString()
                            //});
                        }
                        else
                        {
                            model.CoverageAreaResultModel.SPLITTER_3BB_JSON = "";
                            model.CoverageAreaResultModel.SPLITTER_3BB_LIST = new List<SPLITTER_3BB_INFO>();
                        }
                    }
                    else
                    {
                        model.CoverageAreaResultModel.SPLITTER_3BB_JSON = "";
                        model.CoverageAreaResultModel.SPLITTER_3BB_LIST = new List<SPLITTER_3BB_INFO>();
                        if (SplitterListResult.Splitter != null)
                        {
                            model.CoverageAreaResultModel.SPLITTER_LIST = SplitterListResult.Splitter.ConvertAll(x => new SPLITTER_INFO
                            {
                                Splitter_Name = x.Name.ToSafeString(),
                                Distance = x.Distance.ToSafeDecimal(),
                                Distance_Type = x.DistanceType.ToSafeString()
                            });
                        }
                        else
                        {
                            model.CoverageAreaResultModel.SPLITTER_LIST = new List<SPLITTER_INFO>();
                        }
                    }
                    base.Logger.Info("End Map model CoverageAreaResultModel");

                    base.Logger.Info("Start Splitter Management Get ResQuery");

                    //TODO: Splitter Management
                    model.SplitterTransactionId = TransactionID.ToSafeString();
                    model.SplitterFlagFirstTime = string.Empty;
                    model.SplitterFlag = string.Empty;
                    model.ReservationId = string.Empty;
                    model.SpecialRemark = string.Empty;
                    model.TimeSlotMessage = string.Empty;
                    model.SplitterListStr = WebUtility.HtmlEncode(SplitterList);

                    var listSiteCode = base.LovData.Where(l => l.Name == "MAPPING_OWNER_PRODUCT" && l.LovValue1 == "FTTH" && l.LovValue3 != "AWN" && l.LovValue3 == siteCode).ToList();

                    if (!is3BBSit && !string.IsNullOrEmpty(model.FlowFlag) && model.CoverageAreaResultModel.ACCESS_MODE_LIST == "FTTH" && listSiteCode.Count() <= 0)
                    {
                        var queryResQuery = new ZTEResQueryQuery
                        {
                            PRODUCT = model.CoverageAreaResultModel.ACCESS_MODE_LIST,
                            LISTOFSPLITTER = model.CoverageAreaResultModel.SPLITTER_LIST.ToArray(),
                            TRANSACTION_ID = model.SplitterTransactionId,
                            ADDRESS_ID = AddressID
                        };
                        var resultResQuery = _queryProcessor.Execute(queryResQuery);
                        if (resultResQuery != null)
                        {
                            model.SplitterFlagFirstTime = resultResQuery.RETURN_CASECODE;
                            model.SplitterFlag = resultResQuery.RETURN_CASECODE;
                        }

                        if ((model.SplitterFlagFirstTime == "2") || (model.SplitterFlagFirstTime == "3"))
                        {
                            var listTimeSlotMessage = LovData.FirstOrDefault(
                                item => item.Name == (model.SplitterFlagFirstTime == "2" ? "L_PORT_SPLITTER2_FULL" : "L_PORT_SPLITTER_FULL")
                                    && item.LovValue5 == "FBBOR004");

                            model.TimeSlotMessage = (SiteSession.CurrentUICulture.IsThaiCulture()) ? listTimeSlotMessage.LovValue1 : listTimeSlotMessage.LovValue2;
                        }
                    }
                    else if (is3BBSit && !string.IsNullOrEmpty(model.FlowFlag) && model.CoverageAreaResultModel.ACCESS_MODE_LIST == "FTTH")
                    {
                        model.SplitterFlagFirstTime = "1";
                        model.SplitterFlag = "1";
                    }

                    base.Logger.Info("End Splitter Management Get ResQuery");


                    #region siteCode เป็นค่า Site Code จริงๆ [FTTH] และมีกรณีที่ site code หาไม่เจอด้วย

                    if (!is3BBSit && siteCode != "" && siteCode != "FWA" && model.CallfeascheckFlag == "N")
                    {
                        string langStr = GetCurrentCulture().IsThaiCulture() ? "T" : "E";
                        base.Logger.Info("Begin TRUE [FTTH]");
                        var a = new GetDataESRIListBVQuery
                        {
                            sitecode = siteCode,
                            sub_district = subDistrict,
                            postcode = postCode,
                            language = langStr,
                            province = province,
                            addressid = AddressID
                        };
                        var result2 = _queryProcessor.Execute(a);

                        if (result2 == null) //// case haven't sitecode in listbv
                        {
                            base.Logger.Info("Result from GetDataESRIListBVQuery Case Cannot find in listBV \r\n");
                            base.Logger.Info("PARTNER_NAME: AWN \r\n");
                            base.Logger.Info("ACCESS_MODE_LIST:FTTH \r\n");
                            base.Logger.Info("ADDRESS_ID:" + AddressID + "\r\n");

                            model.TempBuildnameforlistpackage = "AWN";
                            model.CoverageAreaResultModel.PARTNER_NAME = "AWN";
                            model.CoverageAreaResultModel.ACCESS_MODE_LIST = "FTTH";
                            model.CoverageAreaResultModel.ADDRESS_ID = AddressID;
                        }
                        else
                        {
                            base.Logger.Info("Result from GetDataESRIListBVQuery \r\n");
                            base.Logger.Info("PARTNER_NAME:" + result2.PARTNER_NAME + "\r\n");
                            base.Logger.Info("ACCESS_MODE_LIST:" + result2.ACCESS_MODE_LIST + "\r\n");
                            base.Logger.Info("ADDRESS_ID:" + AddressID + "\r\n");
                            base.Logger.Info("BUILD_NAME:" + result2.BUILDING_NAME + "\r\n");

                            if (result2.PARTNER_NAME.ToSafeString() == "")
                            {
                                result2.PARTNER_NAME = "AWN";
                            }

                            model.TempBuildnameforlistpackage = result2.BUILDING_NAME;
                            model.CoverageAreaResultModel.PARTNER_NAME = result2.PARTNER_NAME;
                            model.CoverageAreaResultModel.ACCESS_MODE_LIST = result2.ACCESS_MODE_LIST;
                            model.CoverageAreaResultModel.ADDRESS_ID = AddressID;
                        }

                        base.Logger.Info("siteCode เป็นค่า Site Code จริงๆ [FTTH]");

                        base.Logger.Info("Update coverage result partnername");

                        #region Update Coverage Result

                        var up = Bootstrapper.GetInstance<CheckCoverageController>();
                        var chkCoverage = false;
                        if (Session["CheckCoverage"] != null)
                            chkCoverage = true;
                        else
                            chkCoverage = false;

                        up.FBSSCoverageResultCommand(

                            actionType: "Update",
                            interfaceId: model.CoverageAreaResultModel.TRANSACTION_ID,
                            partner: model.CoverageAreaResultModel.PARTNER_NAME,
                            addrId: model.CoverageAreaResultModel.ADDRESS_ID,
                            owner: model.CoverageAreaResultModel.PARTNER_NAME,
                            productType: model.CoverageAreaResultModel.PARTNER_NAME,
                            addrType: "ESRI",

                            addrNo: houseNo,
                            moo: moo.ToSafeDecimal(),
                            soi: soi,
                            road: road,
                            chkCoverage: chkCoverage,
                            coverage: coverage,
                            lcCode: model.CustomerRegisterPanelModel.L_LOC_CODE,
                            ascCode: model.CustomerRegisterPanelModel.L_ASC_CODE,
                            employeeID: model.CustomerRegisterPanelModel.L_STAFF_ID,
                            saleFirstname: model.OfficerInfoPanelModel.THFirstName,
                            saleLastname: model.OfficerInfoPanelModel.THLastName,
                            locationName: model.CustomerRegisterPanelModel.PartnerName,
                            subRegion: model.OfficerInfoPanelModel.outLocationSubRegion,
                            region: model.OfficerInfoPanelModel.outLocationRegion,
                            ascName: model.OfficerInfoPanelModel.outASCPartnerName,
                            channelName: model.CustomerRegisterPanelModel.PartnerName,
                            saleChannel: model.OfficerInfoPanelModel.outChnSales,
                            //R21.2
                            addressTypeDTL: "",
                            remark: "",
                            technology: "",
                            projectName: ""
                        );

                        #endregion Update Coverage Result
                    }
                    else if (is3BBSit)
                    {
                        // Get Lov wait
                        string tmp3bbAddressID = "";

                        //R23.01 dev by chist699
                        MapFBBAddressQuery query = new MapFBBAddressQuery()
                        {
                            p_province = province,
                            p_amphur = district,
                            p_tumbon = subDistrict,
                            p_zipcode = postCode,
                            FullUrl = "",
                            transaction_id = TransactionID
                        };
                        var resultData = _queryProcessor.Execute(query);
                        if (resultData != null && !string.IsNullOrEmpty(resultData.p_address_id))
                            tmp3bbAddressID = resultData.p_address_id.ToSafeString();

                        model.TempBuildnameforlistpackage = "AWN";
                        model.CoverageAreaResultModel.PARTNER_NAME = "AWN";
                        model.CoverageAreaResultModel.ACCESS_MODE_LIST = "FTTH";
                        model.CoverageAreaResultModel.ADDRESS_ID = tmp3bbAddressID;
                        model.CoverageAreaResultModel.IS_3BB_COVERAGE = true;
                    }

                    #endregion siteCode เป็นค่า Site Code จริงๆ [FTTH] และมีกรณีที่ site code หาไม่เจอด้วย

                    #region EXCLUSIVE_3BB
                    if (status == "ON_SERVICE" && subStatus == "3BB_EXCLUSIVE")
                    {
                        model.CoverageAreaResultModel.EXCLUSIVE_3BB = "Y";
                    }
                    #endregion

                    #region กรณี WTTX

                    if (isWTTX)
                    {
                        model.CoverageAreaResultModel.WTTX_COVERAGE_RESULT = wttxCoverageResult;

                        if (wttxCoverageResult == "YES")
                        {
                            model.CoverageAreaResultModel.WTTX_MAXBANDWITH = wttxMaxBandWith;
                            model.CoverageAreaResultModel.GRID_ID = wttxGridId;
                            //For control get list package.
                            model.FlowFlag = "0";
                            model.CoverageAreaResultModel.ACCESS_MODE_LIST = "WTTX";
                            model.CoverageAreaResultModel.PARTNER_NAME = "WTTX";
                        }
                        else if (wttxCoverageResult == "FULL" || wttxCoverageResult == "NO")
                        {
                            //Update coverage result
                            var up = Bootstrapper.GetInstance<CheckCoverageController>();
                            var chkCoverage = false;
                            if (Session["CheckCoverage"] != null)
                                chkCoverage = true;
                            else
                                chkCoverage = false;

                            up.FBSSCoverageResultCommand(
                                actionType: "Update",
                                interfaceId: model.CoverageAreaResultModel.TRANSACTION_ID,
                                partner: model.CoverageAreaResultModel.PARTNER_NAME,
                                addrId: model.CoverageAreaResultModel.ADDRESS_ID,
                                owner: model.CoverageAreaResultModel.PARTNER_NAME,
                                productType: model.CoverageAreaResultModel.PARTNER_NAME,
                                addrType: "ESRI",

                                addrNo: houseNo,
                                moo: moo.ToSafeDecimal(),
                                soi: soi,
                                road: road,
                                chkCoverage: chkCoverage,
                                coverage: "NO",
                                lcCode: model.CustomerRegisterPanelModel.L_LOC_CODE,
                                ascCode: model.CustomerRegisterPanelModel.L_ASC_CODE,
                                employeeID: model.CustomerRegisterPanelModel.L_STAFF_ID,
                                saleFirstname: model.OfficerInfoPanelModel.THFirstName,
                                saleLastname: model.OfficerInfoPanelModel.THLastName,
                                locationName: model.CustomerRegisterPanelModel.PartnerName,
                                subRegion: model.OfficerInfoPanelModel.outLocationSubRegion,
                                region: model.OfficerInfoPanelModel.outLocationRegion,
                                ascName: model.OfficerInfoPanelModel.outASCPartnerName,
                                channelName: model.CustomerRegisterPanelModel.PartnerName,
                                saleChannel: model.OfficerInfoPanelModel.outChnSales,
                                //R21.2
                                addressTypeDTL: "",
                                remark: "",
                                technology: "",
                                projectName: ""
                            );
                        }
                    }

                    #endregion

                    #region กรณี sitecode เป็นว่าง

                    if (siteCode == "")
                    {
                        base.Logger.Info("Case Site code is blank \r\n");

                        model.TempBuildnameforlistpackage = "AWN";
                        model.CoverageAreaResultModel.PARTNER_NAME = "AWN";
                        model.CoverageAreaResultModel.ACCESS_MODE_LIST = "FTTH";
                        model.CoverageAreaResultModel.ADDRESS_ID = "";

                        #region Update Coverage Result

                        var up = Bootstrapper.GetInstance<CheckCoverageController>();
                        var chkCoverage = false;
                        if (Session["CheckCoverage"] != null)
                            chkCoverage = true;
                        else
                            chkCoverage = false;
                        up.FBSSCoverageResultCommand(
                            actionType: "Update",
                            interfaceId: model.CoverageAreaResultModel.TRANSACTION_ID,
                            partner: model.CoverageAreaResultModel.PARTNER_NAME,
                            addrId: model.CoverageAreaResultModel.ADDRESS_ID,
                            owner: model.CoverageAreaResultModel.PARTNER_NAME,
                            productType: model.CoverageAreaResultModel.PARTNER_NAME,
                            addrType: "ESRI",

                            addrNo: houseNo,
                            moo: moo.ToSafeDecimal(),
                            soi: soi,
                            road: road,
                            chkCoverage: chkCoverage,
                            coverage: coverage,
                            lcCode: model.CustomerRegisterPanelModel.L_LOC_CODE,
                            ascCode: model.CustomerRegisterPanelModel.L_ASC_CODE,
                            employeeID: model.CustomerRegisterPanelModel.L_STAFF_ID,
                            saleFirstname: model.OfficerInfoPanelModel.THFirstName,
                            saleLastname: model.OfficerInfoPanelModel.THLastName,
                            locationName: model.CustomerRegisterPanelModel.PartnerName,
                            subRegion: model.OfficerInfoPanelModel.outLocationSubRegion,
                            region: model.OfficerInfoPanelModel.outLocationRegion,
                            ascName: model.OfficerInfoPanelModel.outASCPartnerName,
                            channelName: model.CustomerRegisterPanelModel.PartnerName,
                            saleChannel: model.OfficerInfoPanelModel.outChnSales,
                            //R21.2
                            addressTypeDTL: "",
                            remark: "",
                            technology: "",
                            projectName: ""
                        );

                        #endregion Update Coverage Result
                    }

                    #endregion กรณี sitecode เป็นว่าง

                    base.Logger.Info("Set session ESRI");
                    Session["ESRI"] = "true";
                    Session["ESRI_COVERAGEMODEL"] = model;
                    Session["ESRI_BYPASSCOVERAGEMODEL"] = model;
                    Session["EOS_COVERAGEMODEL"] = null;
                    if (TriplePlayFlag.IndexOf("|") != -1)
                    {
                        string[] CheckFromEOS = TriplePlayFlag.Split('|');
                        if (CheckFromEOS.Count() > 0)
                        {
                            if (CheckFromEOS[0].ToSafeString() == "EOS")
                            {
                                Session["EOS_COVERAGEMODEL"] = TriplePlayFlag;
                            }
                        }
                    }

                    //For test
                    if (Session["OfficerModel"] != null)  /// case officer
                    {
                        var officerInfo = Session["OfficerModel"] as QuickWinPanelModel;

                        model.CustomerRegisterPanelModel.L_ASC_CODE = officerInfo.CustomerRegisterPanelModel.L_ASC_CODE;
                        model.CustomerRegisterPanelModel.L_LOC_CODE = officerInfo.CustomerRegisterPanelModel.L_LOC_CODE;
                        model.CustomerRegisterPanelModel.outType = officerInfo.CustomerRegisterPanelModel.outType;
                        model.CustomerRegisterPanelModel.outSubType = officerInfo.CustomerRegisterPanelModel.outSubType;
                        model.CustomerRegisterPanelModel.outMobileNo = officerInfo.CustomerRegisterPanelModel.outMobileNo;
                        model.CustomerRegisterPanelModel.PartnerName = officerInfo.CustomerRegisterPanelModel.PartnerName;

                        Session["OfficerModel"] = model;
                        base.Logger.Info("Add model to OFFICER model");
                    }

                    if (Session["StaffModel"] != null)  /// case staff
                    {
                        var staffInfo = Session["StaffModel"] as QuickWinPanelModel;

                        model.TransactionID = TransactionID;
                        model.CustomerRegisterPanelModel.L_LOC_CODE = "STAFF";

                        model.CustomerRegisterPanelModel.L_FIRST_NAME = staffInfo.CustomerRegisterPanelModel.L_FIRST_NAME;
                        model.CustomerRegisterPanelModel.L_LAST_NAME = staffInfo.CustomerRegisterPanelModel.L_LAST_NAME;
                        model.CustomerRegisterPanelModel.L_STAFF_ID = staffInfo.CustomerRegisterPanelModel.L_STAFF_ID;

                        Session["StaffModel"] = model;
                        base.Logger.Info("Add model to STAFF model");
                    }

                    if (Session["ProcessLine1"] != null) /// case SSO
                    {
                        model.SummaryPanelModel.VAS_FLAG = "1";
                        Session["ProcessLine1"] = model;
                    }

                    feastCheckFlag = model.CallfeascheckFlag;
                }
                timer.Stop();
                TimeSpan timespan = timer.Elapsed;

                EndInterface("", log, TransactionID, "Success", "");
                if (Session["CheckCoverage"] != null)
                {
                    Session["CheckCoverage"] = null;
                    Session["ESRI"] = null;
                    Session["ESRI_COVERAGEMODEL"] = null;
                    if (coverage == "Y")
                    {
                        return Content("<html><script>parent.InCoveragePopUp(" + latitude + "," + longitude + ");</script></html>");
                    }
                    else
                    {
                        Session["ESRI_BYPASSCOVERAGEMODEL"] = null;
                        return Content("<html><script>window.top.location.href = '/process/CheckCoverage'; </script></html>");
                    }
                }
                Session["ESRI_BYPASSCOVERAGEMODEL"] = null;
                return Content("<html><script>window.top.location.href = 'Index'; </script></html>");
            }
            catch (Exception ex)
            {
                base.Logger.Info(ex.GetErrorMessage());
                EndInterface("", log, TransactionID, "ERROR", ex.GetErrorMessage());
                return Content("<html><script>window.top.location.href = 'Index'; </script></html>");
            }
        }

        [AuthorizeUserAttribute]
        public ActionResult ProcessTempVas()
        {
            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "AccountService");

            string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress))
            {
                ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }

            if (base.CurrentUser.SSOFields != null)
            {
                ViewBag.LC = base.CurrentUser.SSOFields.LocationCode.ToSafeString();
            }

            if (base.CurrentUser.SSOFields != null)
            {
                Logger.Info("Line 1 => User :" + base.CurrentUser.SSOFields.UserName.ToSafeString() + "Location code :" + base.CurrentUser.SSOFields.LocationCode);
                if (base.CurrentUser.SSOFields.EmployeeServiceWebRootUrl.Contains("partner"))
                {
                    SaveStatlog(base.CurrentUser.SSOFields.UserName.ToSafeString(), "SSO-PARTNER", ipAddress, "FBB REGISTER", base.CurrentUser.SSOFields.EmployeeServiceWebRootUrl, base.CurrentUser.SSOFields.LocationCode);
                }
                else
                {
                    SaveStatlog(base.CurrentUser.SSOFields.UserName.ToSafeString(), "SSO-EMPLOYEE", ipAddress, "FBB REGISTER", base.CurrentUser.SSOFields.EmployeeServiceWebRootUrl, base.CurrentUser.SSOFields.LocationCode);
                }
            }
            //else
            //{
            //    return RedirectToAction("Logout", "AccountService");
            //}

            ViewBag.User = base.CurrentUser;

            if (Session["EndProcessFlag"].ToSafeBoolean())
            {
                Session["PopupStatus"] = "Success";
                Session["EndProcessFlag"] = null;
            }
            else
                Session["PopupStatus"] = null;

            ViewBag.LabelFBBTR000 = GetGeneralScreenConfig();
            ViewBag.LabelFBBTR001 = GetCoverageScreenConfig();
            ViewBag.LabelFBBTR002 = GetDisplayPackageScreenConfig();
            ViewBag.LabelFBBTR003 = GetCustRegisterScreenConfig();
            ViewBag.LabelFBBTR004 = GetSummaryScreenConfig();
            ViewBag.LabelFBBTR010 = GetVas_Select_Package_ScreenConfig();
            ViewBag.LabelFBBORV11 = GetVasPopUpScreenConfig();
            ViewBag.LabelFBBOR015 = GetProfilePrePostPaid();
            ViewBag.FbbConstant = GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);
            ViewBag.FbbException = GetFbbConstantModel(WebConstants.LovConfigName.FbbExceptionIdCard);
            ViewBag.Fbb_Service = GetDisplay_Select_Type_Service();
            ViewBag.Vas = "1";
            Session["ProcessLine1"] = "1";
            Session["TripleLine"] = null;
            Session["StaffModel"] = null;
            Session["EnginerrLine"] = null;
            Session["OfficerModel"] = null;
            ViewBag.Version = GetVersion();
            ViewBag.MeshConfig = GetMeshConfig_ScreenConfig();

            //ViewBag.SFFProductName = outProductName;
            //ViewBag.SFFServiceYear = outServiceYear;

            //if (!string.IsNullOrEmpty(outProductName) && !string.IsNullOrEmpty(outServiceYear))
            //{
            //    ViewBag.Vas = "2";
            //    var q = new GetOwnerProductByNoQuery
            //    {
            //        No = no
            //    };
            //    ViewBag.OwnerProduct = _queryProcessor.Execute(q);
            //}
            //else
            //{
            //    ViewBag.Vas = "1";
            //}

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
                        ViewBag.LanguagePage = "1";
                    }
                }
                else
                {
                    if (data.LovValue2 != null)
                    {
                        ViewBag.LCLOSE = data.LovValue2.ToString();
                        ViewBag.LanguagePage = "2";
                    }
                }
            }
            else
            {
                ViewBag.LCLOSE = "";
            }
            QuickWinPanelModel model = new QuickWinPanelModel();
            model.SummaryPanelModel.VAS_FLAG = "1";
            model.ClientIP = ipAddress;
            return View("New_SearchProfilePrePostPaid", model);
        }

        public ActionResult ReloadPage()
        {
            return View();
        }

        public void Save(QuickWinPanelModel model, string SubProduct)
        {
        }

        [HttpPost]
        public ActionResult SaveSessionCardReader(FBB_IDCARDREADER model)
        {
            var result = false;
            Session["CARD_READER_NATIONALID"] = "";
            Session["CARD_READER_TITLE_TH"] = "";
            Session["CARD_READER_FIRSTNAME_TH"] = "";
            Session["CARD_READER_LASTNAME_TH"] = "";
            Session["CARD_READER_TITLE_EN"] = "";
            Session["CARD_READER_FIRSTNAME_EN"] = "";
            Session["CARD_READER_LASTNAME_EN"] = "";
            Session["CARD_READER_BIRTHDATE"] = "";
            Session["CARD_READER_SEX"] = "";
            Session["CARD_READER_ADDRESS"] = "";
            Session["CARD_READER_MOO"] = "";
            Session["CARD_READER_ALLEY"] = "";
            Session["CARD_READER_SOI"] = "";
            Session["CARD_READER_STREET"] = "";
            Session["CARD_READER_SUBDISTRICT"] = "";
            Session["CARD_READER_DISTRICT"] = "";
            Session["CARD_READER_PROVINCE"] = "";
            Session["CARD_READER_PICTURE"] = "";
            Session["CARD_READER_ISSUEPLACE"] = "";
            Session["CARD_READER_ISSUEDATE"] = "";
            Session["CARD_READER_EXPIREDATE"] = "";

            if (model != null)
            {
                result = true;
                Session["CARD_READER_NATIONALID"] = model.NATIONALID;
                Session["CARD_READER_TITLE_TH"] = model.TITLE_TH;
                Session["CARD_READER_FIRSTNAME_TH"] = model.FIRSTNAME_TH;
                Session["CARD_READER_LASTNAME_TH"] = model.LASTNAME_TH;
                Session["CARD_READER_TITLE_EN"] = model.TITLE_EN;
                Session["CARD_READER_FIRSTNAME_EN"] = model.FIRSTNAME_EN;
                Session["CARD_READER_LASTNAME_EN"] = model.LASTNAME_EN;
                Session["CARD_READER_BIRTHDATE"] = model.BIRTHDATE;
                Session["CARD_READER_SEX"] = model.SEX;
                Session["CARD_READER_ADDRESS"] = model.ADDRESS;
                Session["CARD_READER_MOO"] = model.MOO;
                Session["CARD_READER_ALLEY"] = model.ALLEY;
                Session["CARD_READER_SOI"] = model.SOI;
                Session["CARD_READER_STREET"] = model.STREET;
                Session["CARD_READER_SUBDISTRICT"] = model.SUBDISTRICT;
                Session["CARD_READER_DISTRICT"] = model.DISTRICT;
                Session["CARD_READER_PROVINCE"] = model.PROVINCE;
                Session["CARD_READER_PICTURE"] = model.PICTURE;
                Session["CARD_READER_ISSUEPLACE"] = model.ISSUEPLACE;
                Session["CARD_READER_ISSUEDATE"] = model.ISSUEDATE;
                Session["CARD_READER_EXPIREDATE"] = model.EXPIREDATE;

                Logger.Info("function SaveSessionCardReader ===> NATIONALID  :" + model.NATIONALID
                            + "\r\n" + "TITLE_TH:  " + model.TITLE_TH
                            + "\r\n" + "FIRSTNAME_TH:  " + model.FIRSTNAME_TH
                            + "\r\n" + "LASTNAME_TH:  " + model.LASTNAME_TH
                            + "\r\n" + "TITLE_EN:  " + model.TITLE_EN
                            + "\r\n" + "FIRSTNAME_EN:  " + model.FIRSTNAME_EN
                            + "\r\n" + "LASTNAME_EN:  " + model.LASTNAME_EN
                            + "\r\n" + "BIRTHDATE:  " + model.BIRTHDATE
                            + "\r\n" + "SEX:  " + model.SEX
                            + "\r\n" + "ADDRESS:  " + model.ADDRESS
                            + "\r\n" + "MOO:  " + model.MOO
                            + "\r\n" + "ALLEY:  " + model.ALLEY
                            + "\r\n" + "SOI:  " + model.SOI
                            + "\r\n" + "STREET:  " + model.STREET
                            + "\r\n" + "SUBDISTRICT:  " + model.SUBDISTRICT
                            + "\r\n" + "DISTRICT:  " + model.DISTRICT
                            + "\r\n" + "PROVINCE:  " + model.PROVINCE
                            + "\r\n" + "PICTURE:  " + model.PICTURE
                            + "\r\n" + "ISSUEPLACE:  " + model.ISSUEPLACE
                            + "\r\n" + "ISSUEDATE:  " + model.ISSUEDATE
                            + "\r\n" + "EXPIREDATE:  " + model.EXPIREDATE
                            );
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public void SaveStatlog(string username = "", string VisitType = "", string REQ_IPADDRESS = "", string SELECT_PAGE = "", string HOST = "", string LC = "")
        {
            try
            {
                var statcommand = new StatLogCommand
                {
                    Username = username,
                    VisitType = VisitType,
                    REQ_IPADDRESS = REQ_IPADDRESS,
                    SelectPage = SELECT_PAGE,
                    HOST = HOST,
                    LC = LC
                };
                _StatLogCommand.Handle(statcommand);
                Logger.Info("Statlogww: " + statcommand.ReturnDesc);
            }
            catch (Exception ex)
            {
                Logger.Info(ex.GetErrorMessage());
            }
        }

        [AuthorizeUserAttribute]
        public ActionResult SearchProfile()
        {
            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "AccountService");

            string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (string.IsNullOrEmpty(ipAddress))
            {
                ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
            Logger.Info("IP Address: " + ipAddress);

            if (base.CurrentUser.SSOFields != null)
            {
                Logger.Info("User: " + base.CurrentUser.SSOFields.UserName.ToSafeString() + "Location code :" + base.CurrentUser.SSOFields.LocationCode);
                if (base.CurrentUser.SSOFields.EmployeeServiceWebRootUrl.Contains("partner"))
                {
                    SaveStatlog(base.CurrentUser.SSOFields.UserName.ToSafeString(), "SSO-PARTNER", ipAddress, "FBB VAS", base.CurrentUser.SSOFields.EmployeeServiceWebRootUrl, base.CurrentUser.SSOFields.LocationCode);
                }
                else
                {
                    SaveStatlog(base.CurrentUser.SSOFields.UserName.ToSafeString(), "SSO-EMPLOYEE", ipAddress, "FBB VAS", base.CurrentUser.SSOFields.EmployeeServiceWebRootUrl, base.CurrentUser.SSOFields.LocationCode);
                }
            }
            Logger.Info("Statlogww: finish");
            ViewBag.User = base.CurrentUser;
            ViewBag.LabelFBBTR000 = GetGeneralScreenConfig();
            ViewBag.LabelFBBTR001 = GetCoverageScreenConfig();
            ViewBag.LabelFBBTR002 = GetDisplayPackageScreenConfig();
            ViewBag.LabelFBBTR003 = GetCustRegisterScreenConfig();
            ViewBag.LabelFBBTR004 = GetSummaryScreenConfig();
            ViewBag.LabelFBBTR010 = GetVas_Select_Package_ScreenConfig();
            ViewBag.LabelFBBOR015 = GetProfilePrePostPaid();
            ViewBag.LabelFBBORV11 = GetVasPopUpScreenConfig();
            ViewBag.FbbConstant = GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);
            ViewBag.Vas = "2";
            ViewBag.MeshConfig = GetMeshConfig_ScreenConfig();
            //R23.06 IP Camera
            ViewBag.IpCameraConfig = GetScreenConfig("FBBTR003");

            return View("New_SearchProfile2");
        }


        public JsonResult WatchListSendOneTimePW(string accountType)
        {
            if (null == Session["CONTRACTMOBILENO"])
            {
                return null;
            }

            try
            {
                var query = new SendOneTimePWQuery()
                {
                    msisdn = Session["CONTRACTMOBILENO"] as string,
                    accountType = accountType
                };
                var result = _queryProcessor.Execute(query);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public JsonResult SendOneTimePW(string msisdn, string accountType)
        {//send OTP
            // GssoSsoResponseModel result = new GssoSsoResponseModel();
            try
            {
                var query = new SendOneTimePWQuery()
                {
                    msisdn = msisdn,
                    accountType = accountType
                };
                //  result = new GssoSsoResponseModel();
                var result = _queryProcessor.Execute(query);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                GssoSsoResponseModel result = new GssoSsoResponseModel
                {
                    isSuccess = false
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult signature_64str(string base64str)
        {
            string base64str_ = "";
            base64str_ = base64str.Replace("data:image/png;base64,", "").ToString();
            var FileName = Guid.NewGuid().ToString() + ".jpg";
            var imgBytes = Convert.FromBase64String(base64str_);
            var path = @"D:\\" + FileName;
            System.IO.File.WriteAllBytes(path, imgBytes);
            return Json(new { result = true, }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult signature_flag(string status, string pic64signature)
        {
            Session["signature_flag"] = "";
            Session["pic64signature"] = "";
            if (status != "")
            {
                Session["signature_flag"] = status;
            }
            else
            {
                Session["signature_flag"] = "";
            }

            if (pic64signature != "")
            {
                Session["pic64signature"] = pic64signature;
            }
            else
            {
                Session["pic64signature"] = "";
            }

            return Json(new { result = Session["signature_flag"], }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult used_type_flag(string status)
        {
            Session["used_type_flag"] = "";
            if (status != "")
            {
                Session["used_type_flag"] = status;
            }
            else
            {
                Session["used_type_flag"] = "";
            }

            return Json(new { result = Session["used_type_flag"], }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult get_used_type_flag()
        {
            if (null == Session["used_type_flag"] || "not_flag" == Session["used_type_flag"])
            {
                Session["used_type_flag"] = "not_flag";
            }

            return Json(new { result = Session["used_type_flag"] }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult TopUp(bool SaveSuccess = false, string LSAVE = "", string LCLOSE = "",
            string LPOPUPSAVE = "", string LanguagePage = "", string SWiFi = "", string ExistingPopupFlag = "")
        {
            ViewBag.hasLoadingBlock = true;
            /*
            //R24.05 Add loading block screen by max kunlp885
            if (checkLoadingBlock("TopUp", ""))
            {
                Session["LoadingBlockSaveSuccess"] = SaveSuccess;
                Session["LoadingBlockLabelSave"] = LSAVE;
                Session["LoadingBlockLabelClose"] = LCLOSE;
                Session["LoadingBlockPopupSave"] = LPOPUPSAVE;
                Session["LoadingBlockLanguagePage"] = LanguagePage;
                Session["LoadingBlockSWiFi"] = SWiFi;
                Session["LoadingBlockExistingPopupFlag"] = ExistingPopupFlag;
                return View("ExistingFibreExtensions/_Loading");
            }
            SaveSuccess = (bool)Session["LoadingBlockSaveSuccess"];
            LSAVE = Session["LoadingBlockLabelSave"].ToSafeString();
            LCLOSE = Session["LoadingBlockLabelClose"].ToSafeString();
            LPOPUPSAVE = Session["LoadingBlockPopupSave"].ToSafeString();
            LanguagePage = Session["LoadingBlockLanguagePage"].ToSafeString();
            SWiFi = Session["LoadingBlockSWiFi"].ToSafeString();
            ExistingPopupFlag = Session["LoadingBlockExistingPopupFlag"].ToSafeString();
            //end R24.05 Add loading block screen by max kunlp885
            */

            Session["FullUrl"] = this.Url.Action("TopUp", "Process", null, this.Request.Url.Scheme);

            string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (string.IsNullOrEmpty(ipAddress))
            {
                ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
            Logger.Info("IP Address: " + ipAddress);

            if (Session["EndProcessFlag"].ToSafeBoolean())
            {
                Session["PopupStatus"] = "Success";
                Session["EndProcessFlag"] = null;
            }
            else
                Session["PopupStatus"] = null;
            ViewBag.TopUp = "1";
            ViewBag.LabelFBBTR000 = GetGeneralScreenConfig();
            ViewBag.LabelFBBTR001 = GetCoverageScreenConfig();
            ViewBag.LabelFBBTR002 = GetDisplayPackageScreenConfig();
            ViewBag.LabelFBBTR003 = GetCustRegisterScreenConfig();
            ViewBag.LabelFBBTR004 = GetSummaryScreenConfig();
            ViewBag.LabelFBBTR010 = GetVas_Select_Package_ScreenConfig();
            ViewBag.LabelFBBOR015 = GetProfilePrePostPaid();
            ViewBag.LabelFBBORV11 = GetVasPopUpScreenConfig();
            ViewBag.FbbException = GetFbbConstantModel(WebConstants.LovConfigName.FbbExceptionIdCard);
            ViewBag.Fbb_Service = GetDisplay_Select_Type_Service();
            ViewBag.Version = GetVersion();
            ViewBag.Vas = "";
            ViewBag.User = base.CurrentUser;
            ViewBag.MeshConfig = GetMeshConfig_ScreenConfig();

            ViewBag.LSAVE = LSAVE;
            ViewBag.SWiFi = SWiFi;
            ViewBag.LanguagePage = LanguagePage;
            ViewBag.LPOPUPSAVE = LPOPUPSAVE;
            ViewBag.LCLOSE = LCLOSE;
            ViewBag.SaveSuccess = SaveSuccess;
            ViewBag.FbbConstant = GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);
            ViewBag.ExistingFlag = ExistingPopupFlag == "Y" ? "PLAYBOX:MENU" : "PLAYBOX";
            ViewBag.ExistingPopupFlag = ExistingPopupFlag;
            return View("New_SearchProfile2");
        }

        [HttpPost]
        public ActionResult TopUp(string Data = "")
        {
            if (Session["PageLoadOntop"] == null)
            {
                PageLoadOntopModel tmpModel = new PageLoadOntopModel();
                tmpModel.Data = Data;
                ViewBag.PageGo = "TopUp";
                Session["PageLoadOntop"] = "HaveLoad";
                return View("PageLoadOntop", tmpModel);
            }
            else
            {
                Session["PageLoadOntop"] = null;
            }

            ViewBag.hasLoadingBlock = true;
            //R24.05 Add loading block screen by max kunlp885
            //if (checkLoadingBlock("TopUp", Data))
            //{
            //    return View("ExistingFibreExtensions/_Loading");
            //}
            //end R24.05 Add loading block screen by max kunlp885

            ViewBag.PageShow = "LoginFail";
            QuickWinPanelModel model = new QuickWinPanelModel();

            string FullUrl = "";
            if (Session["FullUrl"] != null)
                FullUrl = Session["FullUrl"].ToSafeString();
            Session["FullUrl"] = this.Url.Action("TopupInternet", "Process", null, this.Request.Url.Scheme);

            string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress))
            {
                ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }

            string NonMobileNo = "";
            string GetIdCardStatus = "";
            string lang = "";
            string timeStamp = "";
            string cardType = "";
            string cardNo = "";

            if (Data != "")
            {
                bool CheckInput = true;
                string DataDec = Decrypt(Data);

                //DataDec = string.IsNullOrEmpty(DataDec) ? "mobileNo=8850125473&lang=TH&timeStamp=202404041030123&cardType=ID_CARD&cardNo=3226178612217" : DataDec;//R24.04 Add loading block screen on /topup, /topupinternet and /topupmesh by max kunlp885

                string[] DataTemps = DataDec.Split('&');

                foreach (var item in DataTemps)
                {
                    string[] DataTemp = item.Split('=');
                    if (DataTemp != null)
                    {
                        if (DataTemp[0].ToSafeString() == "mobileNo")
                        {
                            NonMobileNo = DataTemp[1].ToSafeString();
                            ViewBag.NonMobileNo = NonMobileNo;
                            string checkNonMobileNo = NonMobileNo.Substring(0, 3);
                            var getLovForcheckMobileandFlag = GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);
                            var getLovCheckMobileNo = getLovForcheckMobileandFlag.FirstOrDefault(x => x.Field == "Existing_Check_Prefix_3BB").Validation;
                            var getLovFlagCheck3BB = getLovForcheckMobileandFlag.FirstOrDefault(x => x.Field == "Existing_Check_3BB_Flag").Validation;
                            if (getLovFlagCheck3BB == "Y")
                            {
                                if (checkNonMobileNo == getLovCheckMobileNo)
                                {
                                    if (Data == "")
                                        ViewBag.PageShow = "";
                                    ViewBag.FbbConstant = GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);
                                    ViewBag.TopUp = "1";
                                    ViewBag.LabelFBBTR000 = GetGeneralScreenConfig();
                                    ViewBag.LabelFBBTR001 = GetCoverageScreenConfig();
                                    ViewBag.LabelFBBOR015 = GetProfilePrePostPaid();
                                    ViewBag.LabelFBBTR010 = GetVas_Select_Package_ScreenConfig(); // 25.05.2023 deeflink
                                    ViewBag.MyAISBypass = "Y";
                                    ViewBag.Flag3BB = "3BB";
                                    ViewBag.PageShow = "LoginFail";
                                    return View("New_SearchProfile2");
                                }
                            }
                        }
                        if (DataTemp[0].ToSafeString() == "lang")
                        {
                            lang = DataTemp[1].ToSafeString();
                            if (lang == "TH")
                            {
                                ViewBag.LanguagePage = "1";
                                SiteSession.CurrentUICulture = 1;
                                Session["CurrentUICulture"] = 1;
                            }
                            else
                            {
                                ViewBag.LanguagePage = "2";
                                SiteSession.CurrentUICulture = 2;
                                Session["CurrentUICulture"] = 2;
                            }
                        }
                        if (DataTemp[0].ToSafeString() == "timeStamp")
                        {
                            timeStamp = DataTemp[1].ToSafeString();
                        }
                        if (DataTemp[0].ToSafeString() == "cardType")
                        {
                            cardType = DataTemp[1].ToSafeString();
                        }
                        if (DataTemp[0].ToSafeString() == "cardNo")
                        {
                            cardNo = DataTemp[1].ToSafeString();
                        }
                    }
                    else
                    {
                        // value in put ไม่ถูกต้อง
                        CheckInput = false;
                        break;

                    }
                }

                if (cardType == "" || cardNo == "")
                {
                    CheckInput = false;
                }
                else
                {
                    List<FbbConstantModel> cardtypeDatas = GetFbbConstantModel("ID_CARD_TYPE");
                    if (cardtypeDatas != null && cardtypeDatas.Count > 0)
                    {
                        List<FbbConstantModel> cardtypeData = cardtypeDatas.Where(t => t.Field == cardType).ToList();
                        if (cardtypeData == null || cardtypeData.Count == 0)
                        {
                            CheckInput = false;
                        }
                    }
                }

                if (CheckInput)
                {
                    GetIdCardStatus = GetInfoByNonMobileNo(NonMobileNo);
                    if (GetIdCardStatus == "" && !string.IsNullOrEmpty(IdCard))
                    {
                        ViewBag.IdCard = IdCard;
                        ViewBag.CardType = CardType;

                        ViewBag.User = base.CurrentUser;

                        /// GetData For Playbox

                        model = massCommonBypass(NonMobileNo, IdCard, CardType, "Playbox", ViewBag.User, FullUrl);
                        model.CoveragePanelModel.P_MOBILE = NonMobileNo;
                        ViewBag.AisAirNumberByPass = NonMobileNo;
                        var IDCardName = base.LovData
                            .Where(l => l.Type.Equals(WebConstants.LovConfigName.CardType))
                            .Select(l => new DropdownModel
                            {
                                Text = (lang == "TH" ? l.LovValue1 : l.LovValue2),
                                Value = l.Name,
                                DefaultValue = l.DefaultValue,
                            }).ToList().Find(x => x.Value == CardType);
                        model.CustomerRegisterPanelModel.L_CARD_TYPE = IDCardName.Text;

                        var query2 = new evOMQueryListServiceAndPromotionByPackageTypeQuery
                        {
                            mobileNo = NonMobileNo,
                            idCard = IdCard,
                            FullUrl = FullUrl
                        };

                        var data = _queryProcessor.Execute(query2);
                        if (data.access_mode == "")
                        {
                            ViewBag.FbbConstant = GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);
                            ViewBag.TopUp = "1";
                            ViewBag.LabelFBBTR000 = GetGeneralScreenConfig();
                            ViewBag.LabelFBBTR001 = GetCoverageScreenConfig();
                            ViewBag.LabelFBBOR015 = GetProfilePrePostPaid();
                            ViewBag.LabelFBBTR010 = GetVas_Select_Package_ScreenConfig(); // 25.05.2023 deeflink
                            ViewBag.MyAISBypass = "Y";
                            ViewBag.FlagAccessType = "AccessTypeFail";
                            ViewBag.PageShow = "LoginFail";
                            return View("New_SearchProfile2");
                        }
                        model.v_owner_product = data.v_owner_product;
                        model.v_package_subtype = data.v_package_subtype;
                        model.v_sff_main_promotionCD = data.v_sff_main_promotionCD;
                        model.v_number_of_pb_number = data.v_number_of_pb_number.ToSafeString();
                        model.v_number_of_playbox = data.L_NUMBER_OF_PLAYBOX;
                        model.CoverageAreaResultModel.ADDRESS_ID = data.addressId;
                        ViewBag.AddressID = data.addressId;
                        ViewBag.AccessModeByPass = data.access_mode;

                        model.CustomerRegisterPanelModel.v_installAddress = data.v_installAddress;
                        model.CustomerRegisterPanelModel.installAddress1 = data.v_installAddress1;
                        model.CustomerRegisterPanelModel.installAddress2 = data.v_installAddress2;
                        model.CustomerRegisterPanelModel.installAddress3 = data.v_installAddress3;
                        model.CustomerRegisterPanelModel.installAddress4 = data.v_installAddress4;
                        model.CustomerRegisterPanelModel.installAddress5 = data.v_installAddress5;
                        model.CustomerRegisterPanelModel.pbox_count = data.v_number_of_pb_number.ToString();



                        if (data.ListPromotion.Count > 0)
                        {

                            List<PackagePromotionModel> listPackagePromotion = new List<PackagePromotionModel>();

                            for (var index = 0; index < data.ListPromotion.Count; index++)
                            {

                                PackagePromotionModel pModel = new PackagePromotionModel();
                                pModel.ProductClass = data.ListPromotion[index].productClass;
                                pModel.ProductType = data.ListPromotion[index].productType;
                                pModel.ProductCd = data.ListPromotion[index].productCD;
                                pModel.EndDate = data.ListPromotion[index].endDate;

                                listPackagePromotion.Add(pModel);
                            }
                            model.PackagePromotionList = listPackagePromotion;
                        }

                        ViewBag.SFFProductName = model.outProductName;
                        ViewBag.SFFServiceYear = model.outDayOfServiceYear;
                        ViewBag.MobileAis = model.CoveragePanelModel.P_MOBILE;

                        var controller = DependencyResolver.Current.GetService<SelectPackageController>();

                        JsonNetResult IsMulitPlayboxSpeedPassTmp = controller.GetCheckPackageDownloadSpeed(model.v_sff_main_promotionCD, model.CoveragePanelModel.P_MOBILE) as JsonNetResult;
                        if (IsMulitPlayboxSpeedPassTmp != null)
                        {
                            if (IsMulitPlayboxSpeedPassTmp.Data != null)
                            {
                                CheckPackageDownloadSpeedModel checkPackageDownloadSpeedModel = IsMulitPlayboxSpeedPassTmp.Data as CheckPackageDownloadSpeedModel;
                                if (checkPackageDownloadSpeedModel != null && checkPackageDownloadSpeedModel.IsStatus)
                                {
                                    model.IsMulitPlayboxSpeedPass = "Y";
                                }
                                else // 25.05.2023 deeflink
                                {
                                    ViewBag.FlagMesh = "PBFail";
                                }
                            }
                        }

                        if (model.IsMulitPlayboxSpeedPass == "Y")
                        {
                            if (int.Parse(model.v_number_of_pb_number) > 0)
                            {
                                model.MulitPlaybox = new List<MulitPlayboxModel>();
                                JsonNetResult MulitPlayboxJson = controller.GetPlayboxExtensionInfo(model.CoveragePanelModel.P_MOBILE, model.v_number_of_pb_number, model.v_number_of_playbox) as JsonNetResult;
                                if (MulitPlayboxJson != null && MulitPlayboxJson.Data != null)
                                {
                                    EvOmPlayboxExtensionInfoModel evOmPlayboxExtensionInfoModel = MulitPlayboxJson.Data as EvOmPlayboxExtensionInfoModel;
                                    if (evOmPlayboxExtensionInfoModel != null)
                                    {
                                        if (evOmPlayboxExtensionInfoModel.AvailPlayboxList != null && evOmPlayboxExtensionInfoModel.AvailPlayboxList.Count > 0)
                                        {
                                            JsonNetResult listPackageTMP = controller.GetListPackagebySFFPromoV2(model.v_owner_product, model.v_package_subtype, "CallIn", model.CoveragePanelModel.P_MOBILE, model.v_sff_main_promotionCD, "PBOX", "CUSTOMER") as JsonNetResult;
                                            if (listPackageTMP != null)
                                            {
                                                List<PackageModel> listPackage = listPackageTMP.Data as List<PackageModel>;
                                                if (listPackage != null && listPackage.Count > 0)
                                                {
                                                    int indexMultiPlayboxNew = 0;
                                                    string tmpHTML = "";
                                                    foreach (var item in evOmPlayboxExtensionInfoModel.AvailPlayboxList)
                                                    {
                                                        string SelectVasPlayboxName = "";
                                                        string productCode = item.ProductCode.ToSafeString();
                                                        string productName = item.ProductName.ToSafeString();
                                                        string playBoxExt = item.PlayBoxExt.ToSafeString();
                                                        PackageModel packageInstall = null;
                                                        PackageModel packageMonthly = null;

                                                        SelectVasPlayboxName = "SelectVasPlaybox";
                                                        packageInstall = listPackage.Where(t => t.SERVICE_CODE == item.ProductCode
                                                                                                && t.PRODUCT_SUBTYPE == "PBOX"
                                                                                                && (t.PACKAGE_TYPE == "7" || t.PACKAGE_TYPE == "11")
                                                                                                && (t.PACKAGE_SERVICE_NAME == "PBOXEXT1" || t.PACKAGE_SERVICE_NAME == "PBOXEXT2")
                                                                                                && t.PRODUCT_SUBTYPE3.IndexOf("PBOXIN_EXT") > -1
                                                                                                ).FirstOrDefault();
                                                        packageMonthly = listPackage.Where(t => t.SERVICE_CODE == item.ProductCode
                                                                                                && t.PRODUCT_SUBTYPE == "PBOX"
                                                                                                && (t.PACKAGE_TYPE == "7" || t.PACKAGE_TYPE == "11")
                                                                                                && (t.PACKAGE_SERVICE_NAME == "PBOXEXT1" || t.PACKAGE_SERVICE_NAME == "PBOXEXT2")
                                                                                                && t.PRODUCT_SUBTYPE3.IndexOf("PBOXIN_EXT") == -1).FirstOrDefault();


                                                        if (packageInstall != null && packageMonthly != null)
                                                        {

                                                            string monthlyFee = packageMonthly.PRICE_CHARGE.ToSafeString() == "" ? "0" : packageMonthly.PRICE_CHARGE.ToSafeString();
                                                            string installFee = packageInstall.PRE_PRICE_CHARGE.ToSafeString() == "" ? "0" : packageInstall.PRE_PRICE_CHARGE.ToSafeString();
                                                            string InstallProductSubTypeTMP = packageInstall.PRODUCT_SUBTYPE3;
                                                            string MonthlyProductSubTypeTMP = packageMonthly.PRODUCT_SUBTYPE3;
                                                            string packageServiceName = packageInstall.PACKAGE_SERVICE_NAME;
                                                            string InstallDetaillTMP = "";
                                                            string MonthlyDetaillTMP = "";
                                                            if (lang == "TH")
                                                            {
                                                                InstallDetaillTMP = packageInstall.SFF_WORD_IN_STATEMENT_THA;
                                                                MonthlyDetaillTMP = packageMonthly.SFF_WORD_IN_STATEMENT_THA;
                                                            }
                                                            else
                                                            {
                                                                InstallDetaillTMP = packageInstall.SFF_WORD_IN_STATEMENT_ENG;
                                                                MonthlyDetaillTMP = packageMonthly.SFF_WORD_IN_STATEMENT_ENG;
                                                            }
                                                            model.MulitPlaybox.Add(new MulitPlayboxModel
                                                            {
                                                                RowNumber = indexMultiPlayboxNew.ToSafeString(),
                                                                PlayboxExt = playBoxExt,
                                                                ServiceCode = productCode,
                                                                InstallDetaill = InstallDetaillTMP,
                                                                MonthlyDetaill = MonthlyDetaillTMP,
                                                                InstallFee = installFee,
                                                                MonthlyFee = monthlyFee,
                                                                InstallProductSubType = InstallProductSubTypeTMP,
                                                                MonthlyProductSubType = MonthlyProductSubTypeTMP
                                                            });

                                                            tmpHTML += "<tr><td>";
                                                            tmpHTML += "<input type='text' name='" + SelectVasPlayboxName + "' value='" + indexMultiPlayboxNew + "'>";
                                                            tmpHTML += "<input type='text' id='MAPPING_CODE" + indexMultiPlayboxNew + "' value='" + packageInstall.MAPPING_CODE + "'>";
                                                            tmpHTML += "<input type='text' id='OWNER_PRODUCT" + indexMultiPlayboxNew + "' value='" + packageInstall.OWNER_PRODUCT + "'>";
                                                            tmpHTML += "<input type='text' id='PACKAGE_DISPLAY_ENG" + indexMultiPlayboxNew + "' value='" + packageInstall.PACKAGE_DISPLAY_ENG + "'>";
                                                            tmpHTML += "<input type='text' id='PACKAGE_DISPLAY_THA" + indexMultiPlayboxNew + "' value='" + packageInstall.PACKAGE_DISPLAY_THA + "'>";
                                                            tmpHTML += "<input type='text' id='PACKAGE_SERVICE_CODE" + indexMultiPlayboxNew + "' value='" + packageInstall.PACKAGE_SERVICE_CODE + "'>";
                                                            tmpHTML += "<input type='text' id='PACKAGE_SERVICE_NAME" + indexMultiPlayboxNew + "' value='" + packageInstall.PACKAGE_SERVICE_NAME + "'>";
                                                            tmpHTML += "<input type='text' id='PACKAGE_TYPE" + indexMultiPlayboxNew + "' value='" + packageInstall.PACKAGE_TYPE + "'>";
                                                            tmpHTML += "<input type='text' id='PACKAGE_TYPE_DESC" + indexMultiPlayboxNew + "' value='" + packageInstall.PACKAGE_TYPE_DESC + "'>";
                                                            tmpHTML += "<input type='text' id='PRE_PRICE_CHARGE" + indexMultiPlayboxNew + "' value='" + packageInstall.PRE_PRICE_CHARGE + "'>";
                                                            tmpHTML += "<input type='text' id='PRICE_CHARGE" + indexMultiPlayboxNew + "' value='" + packageInstall.PRICE_CHARGE + "'>";
                                                            tmpHTML += "<input type='text' id='PRODUCT_SUBTYPE" + indexMultiPlayboxNew + "' value='" + packageInstall.PRODUCT_SUBTYPE + "'>";
                                                            tmpHTML += "<input type='text' id='PRODUCT_SUBTYPE3" + indexMultiPlayboxNew + "' value='" + packageInstall.PRODUCT_SUBTYPE3 + "'>";
                                                            tmpHTML += "<input type='text' id='SFF_PRODUCT_NAME" + indexMultiPlayboxNew + "' value='" + packageInstall.SFF_PRODUCT_NAME + "'>";
                                                            tmpHTML += "<input type='text' id='SFF_PROMOTION_CODE" + indexMultiPlayboxNew + "' value='" + packageInstall.SFF_PROMOTION_CODE + "'>";
                                                            tmpHTML += "<input type='text' id='SFF_WORD_IN_STATEMENT_ENG" + indexMultiPlayboxNew + "' value='" + packageInstall.SFF_WORD_IN_STATEMENT_ENG + "'>";
                                                            tmpHTML += "<input type='text' id='SFF_WORD_IN_STATEMENT_THA" + indexMultiPlayboxNew + "' value='" + packageInstall.SFF_WORD_IN_STATEMENT_THA + "'>";
                                                            tmpHTML += "<input type='text' id='SERVICE_CODE" + indexMultiPlayboxNew + "' value='" + packageInstall.SERVICE_CODE + "'>";
                                                            tmpHTML += "<input type='text' id='DOWNLOAD_SPEED" + indexMultiPlayboxNew + "' value='" + packageInstall.DOWNLOAD_SPEED + "'>";
                                                            tmpHTML += "<input type='text' id='UPLOAD_SPEED" + indexMultiPlayboxNew + "' value='" + packageInstall.UPLOAD_SPEED + "'>";
                                                            tmpHTML += "<input type='text' id='AUTO_MAPPING_PROMOTION_CODE" + indexMultiPlayboxNew + "' value='" + packageInstall.AUTO_MAPPING_PROMOTION_CODE + "'>";//R22.08
                                                            tmpHTML += "<input type='text' id='PACKAGE_FOR_SALE_FLAG" + indexMultiPlayboxNew + "' value='" + packageInstall.PACKAGE_FOR_SALE_FLAG + "'>";//R22.08
                                                            tmpHTML += "</td></tr>";
                                                            indexMultiPlayboxNew++;

                                                            tmpHTML += "<tr><td>";
                                                            tmpHTML += "<input type='text' name='" + SelectVasPlayboxName + "' value='" + indexMultiPlayboxNew + "'>";
                                                            tmpHTML += "<input type='text' id='MAPPING_CODE" + indexMultiPlayboxNew + "' value='" + packageMonthly.MAPPING_CODE + "'>";
                                                            tmpHTML += "<input type='text' id='OWNER_PRODUCT" + indexMultiPlayboxNew + "' value='" + packageMonthly.OWNER_PRODUCT + "'>";
                                                            tmpHTML += "<input type='text' id='PACKAGE_DISPLAY_ENG" + indexMultiPlayboxNew + "' value='" + packageMonthly.PACKAGE_DISPLAY_ENG + "'>";
                                                            tmpHTML += "<input type='text' id='PACKAGE_DISPLAY_THA" + indexMultiPlayboxNew + "' value='" + packageMonthly.PACKAGE_DISPLAY_THA + "'>";
                                                            tmpHTML += "<input type='text' id='PACKAGE_SERVICE_CODE" + indexMultiPlayboxNew + "' value='" + packageMonthly.PACKAGE_SERVICE_CODE + "'>";
                                                            tmpHTML += "<input type='text' id='PACKAGE_SERVICE_NAME" + indexMultiPlayboxNew + "' value='" + packageMonthly.PACKAGE_SERVICE_NAME + "'>";
                                                            tmpHTML += "<input type='text' id='PACKAGE_TYPE" + indexMultiPlayboxNew + "' value='" + packageMonthly.PACKAGE_TYPE + "'>";
                                                            tmpHTML += "<input type='text' id='PACKAGE_TYPE_DESC" + indexMultiPlayboxNew + "' value='" + packageMonthly.PACKAGE_TYPE_DESC + "'>";
                                                            tmpHTML += "<input type='text' id='PRE_PRICE_CHARGE" + indexMultiPlayboxNew + "' value='" + packageMonthly.PRE_PRICE_CHARGE + "'>";
                                                            tmpHTML += "<input type='text' id='PRICE_CHARGE" + indexMultiPlayboxNew + "' value='" + packageMonthly.PRICE_CHARGE + "'>";
                                                            tmpHTML += "<input type='text' id='PRODUCT_SUBTYPE" + indexMultiPlayboxNew + "' value='" + packageMonthly.PRODUCT_SUBTYPE + "'>";
                                                            tmpHTML += "<input type='text' id='PRODUCT_SUBTYPE3" + indexMultiPlayboxNew + "' value='" + packageMonthly.PRODUCT_SUBTYPE3 + "'>";
                                                            tmpHTML += "<input type='text' id='SFF_PRODUCT_NAME" + indexMultiPlayboxNew + "' value='" + packageMonthly.SFF_PRODUCT_NAME + "'>";
                                                            tmpHTML += "<input type='text' id='SFF_PROMOTION_CODE" + indexMultiPlayboxNew + "' value='" + packageMonthly.SFF_PROMOTION_CODE + "'>";
                                                            tmpHTML += "<input type='text' id='SFF_WORD_IN_STATEMENT_ENG" + indexMultiPlayboxNew + "' value='" + packageMonthly.SFF_WORD_IN_STATEMENT_ENG + "'>";
                                                            tmpHTML += "<input type='text' id='SFF_WORD_IN_STATEMENT_THA" + indexMultiPlayboxNew + "' value='" + packageMonthly.SFF_WORD_IN_STATEMENT_THA + "'>";
                                                            tmpHTML += "<input type='text' id='SERVICE_CODE" + indexMultiPlayboxNew + "' value='" + packageMonthly.SERVICE_CODE + "'>";
                                                            tmpHTML += "<input type='text' id='DOWNLOAD_SPEED" + indexMultiPlayboxNew + "' value='" + packageMonthly.DOWNLOAD_SPEED + "'>";
                                                            tmpHTML += "<input type='text' id='UPLOAD_SPEED" + indexMultiPlayboxNew + "' value='" + packageMonthly.UPLOAD_SPEED + "'>";
                                                            tmpHTML += "<input type='text' id='AUTO_MAPPING_PROMOTION_CODE" + indexMultiPlayboxNew + "' value='" + packageMonthly.AUTO_MAPPING_PROMOTION_CODE + "'>";//R22.08
                                                            tmpHTML += "<input type='text' id='PACKAGE_FOR_SALE_FLAG" + indexMultiPlayboxNew + "' value='" + packageMonthly.PACKAGE_FOR_SALE_FLAG + "'>";//R22.08
                                                            tmpHTML += "</td></tr>";
                                                            indexMultiPlayboxNew++;
                                                        }
                                                        else // 25.05.2023 deeflink
                                                        {
                                                            ViewBag.FlagMesh = "PBFail";
                                                        }
                                                    }
                                                    if (tmpHTML != "")
                                                    {
                                                        ViewBag.tempV2ByPass = tmpHTML;
                                                        ViewBag.PageShow = "";
                                                    }

                                                }
                                            }
                                        }
                                        else // 25.05.2023 deeflink
                                        {
                                            ViewBag.FlagMesh = "AvailPB";
                                        }
                                    }
                                }
                            }
                            else
                            {
                                JsonNetResult listPackageTMP = controller.GetListPackagebySFFPromoV2(model.v_owner_product, model.v_package_subtype, "CallIn", model.CoveragePanelModel.P_MOBILE, model.v_sff_main_promotionCD, "PBOX", "CUSTOMER") as JsonNetResult;
                                if (listPackageTMP != null)
                                {
                                    List<PackageModel> listPackage = listPackageTMP.Data as List<PackageModel>;
                                    if (listPackage != null && listPackage.Count > 0)
                                    {
                                        int indexMultiPlayboxNew = 0;
                                        string tmpHTML = "";
                                        string SelectVasPlayboxName = "";
                                        List<PackageModel> packageForVasPlayboxCheckbox = new List<PackageModel>();
                                        SelectVasPlayboxName = "SelectVasPlayboxCheckbox";
                                        packageForVasPlayboxCheckbox = listPackage.Where(t => (t.PRODUCT_SUBTYPE == "PBOX"
                                                                                && (t.PACKAGE_TYPE == "7" || t.PACKAGE_TYPE == "11")
                                                                                && t.PACKAGE_SERVICE_NAME == "PBOX")
                                                                                ||
                                                                                //R22.08 Issue PBOX Ontop Content (ATV) by pass
                                                                                (t.PRODUCT_SUBTYPE == "PBOX"
                                                                                && t.PACKAGE_TYPE == "8"
                                                                                && t.PACKAGE_SERVICE_NAME == "PBOX"
                                                                                && t.PACKAGE_TYPE_DESC == "Ontop Content")
                                                                                ).ToList();

                                        if (packageForVasPlayboxCheckbox != null && packageForVasPlayboxCheckbox.Count > 0)
                                        {
                                            foreach (var itemPackage in packageForVasPlayboxCheckbox)
                                            {
                                                tmpHTML += "<tr><td>";
                                                tmpHTML += "<input type='text' name='" + SelectVasPlayboxName + "' value='" + indexMultiPlayboxNew + "'>";
                                                tmpHTML += "<input type='text' id='MAPPING_CODE" + indexMultiPlayboxNew + "' value='" + itemPackage.MAPPING_CODE + "'>";
                                                tmpHTML += "<input type='text' id='OWNER_PRODUCT" + indexMultiPlayboxNew + "' value='" + itemPackage.OWNER_PRODUCT + "'>";
                                                tmpHTML += "<input type='text' id='PACKAGE_DISPLAY_ENG" + indexMultiPlayboxNew + "' value='" + itemPackage.PACKAGE_DISPLAY_ENG + "'>";
                                                tmpHTML += "<input type='text' id='PACKAGE_DISPLAY_THA" + indexMultiPlayboxNew + "' value='" + itemPackage.PACKAGE_DISPLAY_THA + "'>";
                                                tmpHTML += "<input type='text' id='PACKAGE_SERVICE_CODE" + indexMultiPlayboxNew + "' value='" + itemPackage.PACKAGE_SERVICE_CODE + "'>";
                                                tmpHTML += "<input type='text' id='PACKAGE_SERVICE_NAME" + indexMultiPlayboxNew + "' value='" + itemPackage.PACKAGE_SERVICE_NAME + "'>";
                                                tmpHTML += "<input type='text' id='PACKAGE_TYPE" + indexMultiPlayboxNew + "' value='" + itemPackage.PACKAGE_TYPE + "'>";
                                                tmpHTML += "<input type='text' id='PACKAGE_TYPE_DESC" + indexMultiPlayboxNew + "' value='" + itemPackage.PACKAGE_TYPE_DESC + "'>";
                                                tmpHTML += "<input type='text' id='PRE_PRICE_CHARGE" + indexMultiPlayboxNew + "' value='" + itemPackage.PRE_PRICE_CHARGE + "'>";
                                                tmpHTML += "<input type='text' id='PRICE_CHARGE" + indexMultiPlayboxNew + "' value='" + itemPackage.PRICE_CHARGE + "'>";
                                                tmpHTML += "<input type='text' id='PRODUCT_SUBTYPE" + indexMultiPlayboxNew + "' value='" + itemPackage.PRODUCT_SUBTYPE + "'>";
                                                tmpHTML += "<input type='text' id='PRODUCT_SUBTYPE3" + indexMultiPlayboxNew + "' value='" + itemPackage.PRODUCT_SUBTYPE3 + "'>";
                                                tmpHTML += "<input type='text' id='SFF_PRODUCT_NAME" + indexMultiPlayboxNew + "' value='" + itemPackage.SFF_PRODUCT_NAME + "'>";
                                                tmpHTML += "<input type='text' id='SFF_PROMOTION_CODE" + indexMultiPlayboxNew + "' value='" + itemPackage.SFF_PROMOTION_CODE + "'>";
                                                tmpHTML += "<input type='text' id='SFF_WORD_IN_STATEMENT_ENG" + indexMultiPlayboxNew + "' value='" + itemPackage.SFF_WORD_IN_STATEMENT_ENG + "'>";
                                                tmpHTML += "<input type='text' id='SFF_WORD_IN_STATEMENT_THA" + indexMultiPlayboxNew + "' value='" + itemPackage.SFF_WORD_IN_STATEMENT_THA + "'>";
                                                tmpHTML += "<input type='text' id='SERVICE_CODE" + indexMultiPlayboxNew + "' value='" + itemPackage.SERVICE_CODE + "'>";
                                                tmpHTML += "<input type='text' id='DOWNLOAD_SPEED" + indexMultiPlayboxNew + "' value='" + itemPackage.DOWNLOAD_SPEED + "'>";
                                                tmpHTML += "<input type='text' id='UPLOAD_SPEED" + indexMultiPlayboxNew + "' value='" + itemPackage.UPLOAD_SPEED + "'>";
                                                tmpHTML += "<input type='text' id='AUTO_MAPPING_PROMOTION_CODE" + indexMultiPlayboxNew + "' value='" + itemPackage.AUTO_MAPPING_PROMOTION_CODE + "'>";//R22.08
                                                tmpHTML += "<input type='text' id='PACKAGE_FOR_SALE_FLAG" + indexMultiPlayboxNew + "' value='" + itemPackage.PACKAGE_FOR_SALE_FLAG + "'>";//R22.08
                                                tmpHTML += "</td></tr>";
                                                indexMultiPlayboxNew++;
                                            }
                                        }
                                        if (tmpHTML != "")
                                        {
                                            ViewBag.tempV2ByPass = tmpHTML;
                                            ViewBag.PageShow = "";
                                        }

                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        // Login fail
                        IdCard = "";
                        CardType = "";
                    }
                }
                else
                {
                    // value in put ไม่ถูกต้อง
                    ViewBag.LanguagePage = "1";
                    SiteSession.CurrentUICulture = 1;
                    Session["CurrentUICulture"] = 1;
                }
            }

            if (ViewBag.PageShow == "LoginFail")
            {
                if (Data == "")
                    ViewBag.PageShow = "";
                ViewBag.TopUp = "1";
                ViewBag.LabelFBBTR000 = GetGeneralScreenConfig();
                ViewBag.LabelFBBTR001 = GetCoverageScreenConfig();
                ViewBag.LabelFBBOR015 = GetProfilePrePostPaid();
                ViewBag.LabelFBBTR010 = GetVas_Select_Package_ScreenConfig(); // 25.05.2023 deeflink
                ViewBag.MyAISBypass = "Y";
                return View("New_SearchProfile2");
            }
            else
            {
                ViewBag.FbbConstant = GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);
                ViewBag.LabelFBBTR000 = GetGeneralScreenConfig();
                ViewBag.LabelFBBTR001 = GetCoverageScreenConfig();
                ViewBag.LabelFBBTR002 = GetDisplayPackageScreenConfig();
                ViewBag.LabelFBBTR003 = GetCustRegisterScreenConfig();
                ViewBag.LabelFBBTR004 = GetSummaryScreenConfig();
                ViewBag.LabelFBBTR010 = GetVas_Select_Package_ScreenConfig();
                ViewBag.LabelFBBORV11 = GetVasPopUpScreenConfig();
                ViewBag.LabelFBBORV41 = GetTopUpMesh_ScreenConfig();

                ViewBag.TopUp = "1";
                ViewBag.MyAISBypass = "Y";
                ViewBag.ExistingFlag = "PLAYBOX";
                return View("Index", model);
            }
        }

        public ActionResult TopUpFixedline(bool SaveSuccess = false, string LSAVE = "", string LCLOSE = "",
        string LPOPUPSAVE = "", string LanguagePage = "", string SWiFi = "", string ExistingPopupFlag = "")
        {
            ViewBag.hasLoadingBlock = true;
            /*
            //R24.05 Add loading block screen by max kunlp885
            if (checkLoadingBlock("TopUpFixedline", ""))
            {
                Session["LoadingBlockSaveSuccess"] = SaveSuccess;
                Session["LoadingBlockLabelSave"] = LSAVE;
                Session["LoadingBlockLabelClose"] = LCLOSE;
                Session["LoadingBlockPopupSave"] = LPOPUPSAVE;
                Session["LoadingBlockLanguagePage"] = LanguagePage;
                Session["LoadingBlockSWiFi"] = SWiFi;
                Session["LoadingBlockExistingPopupFlag"] = ExistingPopupFlag;
                return View("ExistingFibreExtensions/_Loading");
            }
            SaveSuccess = (bool)Session["LoadingBlockSaveSuccess"];
            LSAVE = Session["LoadingBlockLabelSave"].ToSafeString();
            LCLOSE = Session["LoadingBlockLabelClose"].ToSafeString();
            LPOPUPSAVE = Session["LoadingBlockPopupSave"].ToSafeString();
            LanguagePage = Session["LoadingBlockLanguagePage"].ToSafeString();
            SWiFi = Session["LoadingBlockSWiFi"].ToSafeString();
            ExistingPopupFlag = Session["LoadingBlockExistingPopupFlag"].ToSafeString();
            //end R24.05 Add loading block screen by max kunlp885
            */

            Session["FullUrl"] = this.Url.Action("TopUpFixedline", "Process", null, this.Request.Url.Scheme);

            string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (string.IsNullOrEmpty(ipAddress))
            {
                ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
            Logger.Info("IP Address: " + ipAddress);

            if (Session["EndProcessFlag"].ToSafeBoolean())
            {
                Session["PopupStatus"] = "Success";
                Session["EndProcessFlag"] = null;
            }
            else
                Session["PopupStatus"] = null;
            ViewBag.TopUp = "1";
            ViewBag.LabelFBBTR000 = GetGeneralScreenConfig();
            ViewBag.LabelFBBTR001 = GetCoverageScreenConfig();
            ViewBag.LabelFBBTR002 = GetDisplayPackageScreenConfig();
            ViewBag.LabelFBBTR003 = GetCustRegisterScreenConfig();
            ViewBag.LabelFBBTR004 = GetSummaryScreenConfig();
            ViewBag.LabelFBBTR010 = GetVas_Select_Package_ScreenConfig();
            ViewBag.LabelFBBOR015 = GetProfilePrePostPaid();
            ViewBag.LabelFBBORV11 = GetVasPopUpScreenConfig();
            ViewBag.LabelFBBTR023 = GetTopUpFixedlineScreenConfig();
            ViewBag.FbbException = GetFbbConstantModel(WebConstants.LovConfigName.FbbExceptionIdCard);
            ViewBag.Fbb_Service = GetDisplay_Select_Type_Service();
            ViewBag.Version = GetVersion();
            ViewBag.Vas = "";
            ViewBag.User = base.CurrentUser;
            ViewBag.MeshConfig = GetMeshConfig_ScreenConfig();

            ViewBag.LSAVE = LSAVE;
            ViewBag.SWiFi = SWiFi;
            ViewBag.LanguagePage = LanguagePage;
            //ViewBag.LCLOSE = LCLOSE;
            ViewBag.LPOPUPSAVE = LPOPUPSAVE;
            ViewBag.LCLOSE = LCLOSE;
            ViewBag.SaveSuccess = SaveSuccess;
            ViewBag.FbbConstant = GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);
            ViewBag.ExistingFlag = ExistingPopupFlag == "Y" ? "FIXLINE:MENU" : "FIXLINE";
            ViewBag.ExistingPopupFlag = ExistingPopupFlag;

            return View("New_SearchProfile2");
        }

        [HttpPost]
        public ActionResult TopUpFixedline(string Data = "")
        {
            if (Session["PageLoadTopUpFixedline"] == null)
            {
                PageLoadOntopModel tmpModel = new PageLoadOntopModel();
                tmpModel.Data = Data;
                ViewBag.PageGo = "TopUpFixedline";
                Session["PageLoadTopUpFixedline"] = "HaveLoad";
                return View("PageLoadOntop", tmpModel);
            }
            else
            {
                Session["PageLoadTopUpFixedline"] = null;
            }

            ViewBag.hasLoadingBlock = true;
            //R24.05 Add loading block screen by max kunlp885
            //if (checkLoadingBlock("TopUpFixedline", Data))
            //{
            //    return View("ExistingFibreExtensions/_Loading");
            //}
            //end R24.05 Add loading block screen by max kunlp885


            ViewBag.PageShow = "LoginFail";
            QuickWinPanelModel model = new QuickWinPanelModel();

            string FullUrl = "";
            if (Session["FullUrl"] != null)
                FullUrl = Session["FullUrl"].ToSafeString();
            Session["FullUrl"] = this.Url.Action("TopupInternet", "Process", null, this.Request.Url.Scheme);

            string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress))
            {
                ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }

            string NonMobileNo = "";
            string GetIdCardStatus = "";
            string lang = "";
            string timeStamp = "";
            string cardType = "";
            string cardNo = "";

            if (Data != "")
            {
                bool CheckInput = true;
                string DataDec = Decrypt(Data);
                string[] DataTemps = DataDec.Split('&');

                foreach (var item in DataTemps)
                {
                    string[] DataTemp = item.Split('=');
                    if (DataTemp != null)
                    {
                        if (DataTemp[0].ToSafeString() == "mobileNo")
                        {
                            NonMobileNo = DataTemp[1].ToSafeString();
                            ViewBag.NonMobileNo = NonMobileNo;
                            string checkNonMobileNo = NonMobileNo.Substring(0, 3);
                            var getLovForcheckMobileandFlag = GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);
                            var getLovCheckMobileNo = getLovForcheckMobileandFlag.FirstOrDefault(x => x.Field == "Existing_Check_Prefix_3BB").Validation;
                            var getLovFlagCheck3BB = getLovForcheckMobileandFlag.FirstOrDefault(x => x.Field == "Existing_Check_3BB_Flag").Validation;
                            if (getLovFlagCheck3BB == "Y")
                            {
                                if (checkNonMobileNo == getLovCheckMobileNo)
                                {
                                    if (Data == "")
                                        ViewBag.PageShow = "";
                                    ViewBag.FbbConstant = GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);
                                    ViewBag.TopUp = "1";
                                    ViewBag.LabelFBBTR000 = GetGeneralScreenConfig();
                                    ViewBag.LabelFBBTR001 = GetCoverageScreenConfig();
                                    ViewBag.LabelFBBOR015 = GetProfilePrePostPaid();
                                    ViewBag.LabelFBBTR010 = GetVas_Select_Package_ScreenConfig(); // 25.05.2023 deeflink
                                    ViewBag.MyAISBypass = "Y";
                                    ViewBag.Flag3BB = "3BB";
                                    ViewBag.PageShow = "LoginFail";
                                    return View("New_SearchProfile2");
                                }
                            }
                        }
                        if (DataTemp[0].ToSafeString() == "lang")
                        {
                            lang = DataTemp[1].ToSafeString();
                            if (lang == "TH")
                            {
                                ViewBag.LanguagePage = "1";
                                SiteSession.CurrentUICulture = 1;
                                Session["CurrentUICulture"] = 1;
                            }
                            else
                            {
                                ViewBag.LanguagePage = "2";
                                SiteSession.CurrentUICulture = 2;
                                Session["CurrentUICulture"] = 2;
                            }
                        }
                        if (DataTemp[0].ToSafeString() == "timeStamp")
                        {
                            timeStamp = DataTemp[1].ToSafeString();
                        }
                        if (DataTemp[0].ToSafeString() == "cardType")
                        {
                            cardType = DataTemp[1].ToSafeString();
                        }
                        if (DataTemp[0].ToSafeString() == "cardNo")
                        {
                            cardNo = DataTemp[1].ToSafeString();
                        }
                    }
                    else
                    {
                        // value in put ไม่ถูกต้อง
                        CheckInput = false;
                        break;

                    }
                }

                if (cardType == "" || cardNo == "")
                {
                    CheckInput = false;
                }
                else
                {
                    List<FbbConstantModel> cardtypeDatas = GetFbbConstantModel("ID_CARD_TYPE");
                    if (cardtypeDatas != null && cardtypeDatas.Count > 0)
                    {
                        List<FbbConstantModel> cardtypeData = cardtypeDatas.Where(t => t.Field == cardType).ToList();
                        if (cardtypeData == null || cardtypeData.Count == 0)
                        {
                            CheckInput = false;
                        }
                    }
                }

                if (CheckInput)
                {
                    GetIdCardStatus = GetInfoByNonMobileNo(NonMobileNo);
                    if (GetIdCardStatus == "" && !string.IsNullOrEmpty(IdCard))
                    {
                        ViewBag.IdCard = IdCard;
                        ViewBag.CardType = CardType;

                        ViewBag.User = base.CurrentUser;

                        /// GetData For Fixline

                        model = massCommonBypass(NonMobileNo, IdCard, CardType, "Fixline", ViewBag.User, FullUrl);
                        model.CoveragePanelModel.P_MOBILE = NonMobileNo;
                        ViewBag.AisAirNumberByPass = NonMobileNo;
                        var IDCardName = base.LovData
                            .Where(l => l.Type.Equals(WebConstants.LovConfigName.CardType))
                            .Select(l => new DropdownModel
                            {
                                Text = (lang == "TH" ? l.LovValue1 : l.LovValue2),
                                Value = l.Name,
                                DefaultValue = l.DefaultValue,
                            }).ToList().Find(x => x.Value == CardType);
                        model.CustomerRegisterPanelModel.L_CARD_TYPE = IDCardName.Text;

                        var query2 = new evOMQueryListServiceAndPromotionByPackageTypeQuery
                        {
                            mobileNo = NonMobileNo,
                            idCard = IdCard,
                            FullUrl = FullUrl
                        };

                        var data = _queryProcessor.Execute(query2);
                        if (data.access_mode == "")
                        {
                            ViewBag.FbbConstant = GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);
                            ViewBag.TopUp = "1";
                            ViewBag.LabelFBBTR000 = GetGeneralScreenConfig();
                            ViewBag.LabelFBBTR001 = GetCoverageScreenConfig();
                            ViewBag.LabelFBBOR015 = GetProfilePrePostPaid();
                            ViewBag.LabelFBBTR010 = GetVas_Select_Package_ScreenConfig(); // 25.05.2023 deeflink
                            ViewBag.MyAISBypass = "Y";
                            ViewBag.FlagAccessType = "AccessTypeFail";
                            ViewBag.PageShow = "LoginFail";
                            return View("New_SearchProfile2");
                        }
                        model.v_owner_product = data.v_owner_product;
                        model.v_package_subtype = data.v_package_subtype;
                        model.v_sff_main_promotionCD = data.v_sff_main_promotionCD;
                        model.v_number_of_pb_number = data.v_number_of_pb_number.ToSafeString();
                        model.v_number_of_playbox = data.L_NUMBER_OF_PLAYBOX;
                        model.CoverageAreaResultModel.ADDRESS_ID = data.addressId;
                        ViewBag.AddressID = data.addressId;
                        ViewBag.AccessModeByPass = data.access_mode;

                        model.CustomerRegisterPanelModel.v_installAddress = data.v_installAddress;
                        model.CustomerRegisterPanelModel.installAddress1 = data.v_installAddress1;
                        model.CustomerRegisterPanelModel.installAddress2 = data.v_installAddress2;
                        model.CustomerRegisterPanelModel.installAddress3 = data.v_installAddress3;
                        model.CustomerRegisterPanelModel.installAddress4 = data.v_installAddress4;
                        model.CustomerRegisterPanelModel.installAddress5 = data.v_installAddress5;
                        model.CustomerRegisterPanelModel.pbox_count = data.v_number_of_pb_number.ToString();



                        if (data.ListPromotion.Count > 0)
                        {

                            List<PackagePromotionModel> listPackagePromotion = new List<PackagePromotionModel>();

                            for (var index = 0; index < data.ListPromotion.Count; index++)
                            {

                                PackagePromotionModel pModel = new PackagePromotionModel();
                                pModel.ProductClass = data.ListPromotion[index].productClass;
                                pModel.ProductType = data.ListPromotion[index].productType;
                                pModel.ProductCd = data.ListPromotion[index].productCD;
                                pModel.EndDate = data.ListPromotion[index].endDate;

                                listPackagePromotion.Add(pModel);
                            }
                            model.PackagePromotionList = listPackagePromotion;
                        }

                        ViewBag.SFFProductName = model.outProductName;
                        ViewBag.SFFServiceYear = model.outDayOfServiceYear;
                        ViewBag.MobileAis = model.CoveragePanelModel.P_MOBILE;

                        var controller = DependencyResolver.Current.GetService<SelectPackageController>();
                        JsonNetResult listPackageTMP = controller.GetListPackagebySFFPromoV2(model.v_owner_product, model.v_package_subtype, "CallIn", model.CoveragePanelModel.P_MOBILE, model.v_sff_main_promotionCD, "VOIP", "CUSTOMER") as JsonNetResult;
                        if (listPackageTMP != null)
                        {
                            List<PackageModel> listPackage = listPackageTMP.Data as List<PackageModel>;
                            if (listPackage != null && listPackage.Count > 0)
                            {
                                int indexFixlineNew = 0;
                                string tmpHTML = "";
                                string SelectVasName = "";
                                List<PackageModel> packageForVasFixline = new List<PackageModel>();
                                SelectVasName = "SelectVasfixline";
                                packageForVasFixline = listPackage.Where(t => t.PRODUCT_SUBTYPE == "VOIP"
                                                                        && (t.PACKAGE_TYPE == "1" || t.PACKAGE_TYPE == "2")
                                                                        ).ToList();
                                if (packageForVasFixline != null && packageForVasFixline.Count > 0)
                                {
                                    foreach (var itemPackage in packageForVasFixline)
                                    {
                                        tmpHTML += "<tr><td>";
                                        tmpHTML += "<input type='text' name='" + SelectVasName + "' value='" + indexFixlineNew + "'>";
                                        tmpHTML += "<input type='text' id='MAPPING_CODE" + indexFixlineNew + "' value='" + itemPackage.MAPPING_CODE + "'>";
                                        tmpHTML += "<input type='text' id='OWNER_PRODUCT" + indexFixlineNew + "' value='" + itemPackage.OWNER_PRODUCT + "'>";
                                        tmpHTML += "<input type='text' id='PACKAGE_DISPLAY_ENG" + indexFixlineNew + "' value='" + itemPackage.PACKAGE_DISPLAY_ENG + "'>";
                                        tmpHTML += "<input type='text' id='PACKAGE_DISPLAY_THA" + indexFixlineNew + "' value='" + itemPackage.PACKAGE_DISPLAY_THA + "'>";
                                        tmpHTML += "<input type='text' id='PACKAGE_SERVICE_CODE" + indexFixlineNew + "' value='" + itemPackage.PACKAGE_SERVICE_CODE + "'>";
                                        tmpHTML += "<input type='text' id='PACKAGE_SERVICE_NAME" + indexFixlineNew + "' value='" + itemPackage.PACKAGE_SERVICE_NAME + "'>";
                                        tmpHTML += "<input type='text' id='PACKAGE_TYPE" + indexFixlineNew + "' value='" + itemPackage.PACKAGE_TYPE + "'>";
                                        tmpHTML += "<input type='text' id='PACKAGE_TYPE_DESC" + indexFixlineNew + "' value='" + itemPackage.PACKAGE_TYPE_DESC + "'>";
                                        tmpHTML += "<input type='text' id='PRE_PRICE_CHARGE" + indexFixlineNew + "' value='" + itemPackage.PRE_PRICE_CHARGE + "'>";
                                        tmpHTML += "<input type='text' id='PRICE_CHARGE" + indexFixlineNew + "' value='" + itemPackage.PRICE_CHARGE + "'>";
                                        tmpHTML += "<input type='text' id='PRODUCT_SUBTYPE" + indexFixlineNew + "' value='" + itemPackage.PRODUCT_SUBTYPE + "'>";
                                        tmpHTML += "<input type='text' id='PRODUCT_SUBTYPE3" + indexFixlineNew + "' value='" + itemPackage.PRODUCT_SUBTYPE3 + "'>";
                                        tmpHTML += "<input type='text' id='SFF_PRODUCT_NAME" + indexFixlineNew + "' value='" + itemPackage.SFF_PRODUCT_NAME + "'>";
                                        tmpHTML += "<input type='text' id='SFF_PROMOTION_CODE" + indexFixlineNew + "' value='" + itemPackage.SFF_PROMOTION_CODE + "'>";
                                        tmpHTML += "<input type='text' id='SFF_WORD_IN_STATEMENT_ENG" + indexFixlineNew + "' value='" + itemPackage.SFF_WORD_IN_STATEMENT_ENG + "'>";
                                        tmpHTML += "<input type='text' id='SFF_WORD_IN_STATEMENT_THA" + indexFixlineNew + "' value='" + itemPackage.SFF_WORD_IN_STATEMENT_THA + "'>";
                                        tmpHTML += "<input type='text' id='SERVICE_CODE" + indexFixlineNew + "' value='" + itemPackage.SERVICE_CODE + "'>";
                                        tmpHTML += "<input type='text' id='DOWNLOAD_SPEED" + indexFixlineNew + "' value='" + itemPackage.DOWNLOAD_SPEED + "'>";
                                        tmpHTML += "<input type='text' id='UPLOAD_SPEED" + indexFixlineNew + "' value='" + itemPackage.UPLOAD_SPEED + "'>";
                                        tmpHTML += "</td></tr>";
                                        indexFixlineNew++;
                                    }
                                }
                                if (tmpHTML != "")
                                {
                                    ViewBag.tempV2ByPass = tmpHTML;
                                    ViewBag.PageShow = "";
                                }

                            }
                        }
                    }
                    else
                    {
                        // Login fail
                        IdCard = "";
                        CardType = "";
                    }
                }
                else
                {
                    // value in put ไม่ถูกต้อง
                    ViewBag.LanguagePage = "1";
                    SiteSession.CurrentUICulture = 1;
                    Session["CurrentUICulture"] = 1;
                }

            }

            if (ViewBag.PageShow == "LoginFail")
            {
                if (Data == "")
                    ViewBag.PageShow = "";
                ViewBag.TopUp = "1";
                ViewBag.LabelFBBTR000 = GetGeneralScreenConfig();
                ViewBag.LabelFBBTR001 = GetCoverageScreenConfig();
                ViewBag.LabelFBBOR015 = GetProfilePrePostPaid();
                ViewBag.MyAISBypass = "Y";
                return View("New_SearchProfile2");
            }
            else
            {
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
                            ViewBag.LanguagePage = "1";
                        }
                    }
                    else
                    {
                        if (data.LovValue2 != null)
                        {
                            ViewBag.LCLOSE = data.LovValue2.ToString();
                            ViewBag.LanguagePage = "2";
                        }
                    }
                }
                else
                {
                    ViewBag.LCLOSE = "";
                }

                ViewBag.FbbConstant = GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);
                ViewBag.LabelFBBTR000 = GetGeneralScreenConfig();
                ViewBag.LabelFBBTR001 = GetCoverageScreenConfig();
                ViewBag.LabelFBBTR002 = GetDisplayPackageScreenConfig();
                ViewBag.LabelFBBTR003 = GetCustRegisterScreenConfig();
                ViewBag.LabelFBBTR004 = GetSummaryScreenConfig();
                ViewBag.LabelFBBTR010 = GetVas_Select_Package_ScreenConfig();
                ViewBag.LabelFBBORV11 = GetVasPopUpScreenConfig();
                ViewBag.LabelFBBTR023 = GetTopUpFixedlineScreenConfig();
                ViewBag.LabelFBBORV41 = GetTopUpMesh_ScreenConfig();

                ViewBag.TopUp = "1";
                ViewBag.MyAISBypass = "Y";
                ViewBag.ExistingFlag = "FIXLINE";
                return View("Index", model);
            }
        }

        //R17.11 Officer for Existing Customer
        public ActionResult TopupInternet(string PACKAGE_NAME = "", bool SaveSuccess = false, string LSAVE = "", string LCLOSE = "",
          string LPOPUPSAVE = "", string LanguagePage = "", string SWiFi = "", string DUP = "", string ExistingPopupFlag = "", string Data = "")
        {
            ViewBag.hasLoadingBlock = true;
            /*
            //R24.05 Add loading block screen by max kunlp885
            if (checkLoadingBlock("TopupInternet", Data))
            {
                Session["LoadingBlockPackageName"] = PACKAGE_NAME;
                Session["LoadingBlockSaveSuccess"] = SaveSuccess;
                Session["LoadingBlockLabelSave"] = LSAVE;
                Session["LoadingBlockLabelClose"] = LCLOSE;
                Session["LoadingBlockPopupSave"] = LPOPUPSAVE;
                Session["LoadingBlockLanguagePage"] = LanguagePage;
                Session["LoadingBlockSWiFi"] = SWiFi;
                Session["LoadingBlockDUP"] = DUP;
                Session["LoadingBlockExistingPopupFlag"] = ExistingPopupFlag;
                return View("ExistingFibreExtensions/_Loading");
            }
            PACKAGE_NAME = Session["LoadingBlockPackageName"].ToSafeString();
            SaveSuccess = (bool)Session["LoadingBlockSaveSuccess"];
            LSAVE = Session["LoadingBlockLabelSave"].ToSafeString();
            LCLOSE = Session["LoadingBlockLabelClose"].ToSafeString();
            LPOPUPSAVE = Session["LoadingBlockPopupSave"].ToSafeString();
            LanguagePage = Session["LoadingBlockLanguagePage"].ToSafeString();
            SWiFi = Session["LoadingBlockSWiFi"].ToSafeString();
            DUP = Session["LoadingBlockDUP"].ToSafeString();
            ExistingPopupFlag = Session["LoadingBlockExistingPopupFlag"].ToSafeString();
            //end R24.05 Add loading block screen by max kunlp885
            */

            QuickWinPanelModel model = new QuickWinPanelModel();

            string FullUrl = "";
            if (Session["FullUrl"] != null)
                FullUrl = Session["FullUrl"].ToSafeString();
            Session["FullUrl"] = this.Url.Action("TopupInternet", "Process", null, this.Request.Url.Scheme);

            string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress))
            {
                ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }

            string NonMobileNo = "";
            string GetIdCardStatus = "";
            string lang = "";
            string timeStamp = "";
            string cardType = "";
            string cardNo = "";

            if (Data != "")
            {
                ViewBag.MyAISBypass = "Y";
                bool CheckInput = true;
                string DataDec = Decrypt(Data);

                //DataDec = string.IsNullOrEmpty(DataDec) ? "mobileNo=8850125473&lang=TH&timeStamp=202404041030123&cardType=ID_CARD&cardNo=3226178612217" : DataDec;//R24.04 Add loading block screen on /topup, /topupinternet and /topupmesh by max kunlp885

                string[] DataTemps = DataDec.Split('&');

                foreach (var item in DataTemps)
                {
                    string[] DataTemp = item.Split('=');
                    if (DataTemp != null)
                    {
                        if (DataTemp[0].ToSafeString() == "mobileNo")
                        {
                            NonMobileNo = DataTemp[1].ToSafeString();
                            //NonMobileNo = "8800022021";
                            ViewBag.NonMobileNo = NonMobileNo;
                            string checkNonMobileNo = NonMobileNo.Substring(0, 3);
                            var getLovForcheckMobileandFlag = GetCoverageScreenConfig();
                            var getLovCheckMobileNo = getLovForcheckMobileandFlag.FirstOrDefault(x => x.Name == "Existing_Check_Prefix_3BB").DisplayValue;
                            var getLovFlagCheck3BB = getLovForcheckMobileandFlag.FirstOrDefault(x => x.Name == "Existing_Check_3BB_Flag").DisplayValue;
                            if (getLovFlagCheck3BB == "Y")
                            {
                                if (checkNonMobileNo == getLovCheckMobileNo)
                                {
                                    if (Data == "")
                                        ViewBag.PageShow = "";
                                    ViewBag.TopUp = "1";
                                    ViewBag.LabelFBBTR000 = GetGeneralScreenConfig();
                                    ViewBag.LabelFBBTR001 = GetCoverageScreenConfig();
                                    ViewBag.LabelFBBOR015 = GetProfilePrePostPaid();
                                    ViewBag.LabelFBBTR010 = GetVas_Select_Package_ScreenConfig(); // 25.05.2023 deeflink
                                    ViewBag.MyAISBypass = "Y";
                                    ViewBag.Flag3BB = "3BB";
                                    return View("New_SearchProfileTopupPlaybox");
                                }
                            }
                        }
                        if (DataTemp[0].ToSafeString() == "lang")
                        {
                            lang = DataTemp[1].ToSafeString();
                            if (lang == "TH")
                            {
                                ViewBag.LanguagePage = "1";
                                SiteSession.CurrentUICulture = 1;
                                Session["CurrentUICulture"] = 1;
                            }
                            else
                            {
                                ViewBag.LanguagePage = "2";
                                SiteSession.CurrentUICulture = 2;
                                Session["CurrentUICulture"] = 2;
                            }
                        }
                        if (DataTemp[0].ToSafeString() == "timeStamp")
                        {
                            timeStamp = DataTemp[1].ToSafeString();
                        }
                        if (DataTemp[0].ToSafeString() == "cardType")
                        {
                            cardType = DataTemp[1].ToSafeString();
                        }
                        if (DataTemp[0].ToSafeString() == "cardNo")
                        {
                            cardNo = DataTemp[1].ToSafeString();
                        }
                    }
                    else
                    {
                        // value in put ไม่ถูกต้อง
                        CheckInput = false;
                        break;

                    }
                }

                if (cardType == "" || cardNo == "")
                {
                    CheckInput = false;
                }
                else
                {
                    List<FbbConstantModel> cardtypeDatas = GetFbbConstantModel("ID_CARD_TYPE");
                    if (cardtypeDatas != null && cardtypeDatas.Count > 0)
                    {
                        List<FbbConstantModel> cardtypeData = cardtypeDatas.Where(t => t.Field == cardType).ToList();
                        if (cardtypeData == null || cardtypeData.Count == 0)
                        {
                            CheckInput = false;
                        }
                    }
                }

                if (CheckInput)
                {
                    GetIdCardStatus = GetInfoByNonMobileNo(NonMobileNo);
                    if (GetIdCardStatus == "" && !string.IsNullOrEmpty(IdCard))
                    {
                        ViewBag.IdCard = IdCard;
                        ViewBag.CardType = CardType;
                        ViewBag.PageShow = "Mobile";

                        ViewBag.User = base.CurrentUser;

                        model = massCommonBypass(NonMobileNo, IdCard, CardType, "TopupInternet", ViewBag.User, FullUrl);
                        model.CoveragePanelModel.P_MOBILE = NonMobileNo;

                        var query2 = new evOMQueryListServiceAndPromotionByPackageTypeQuery
                        {
                            mobileNo = NonMobileNo,//"8850001230",
                            idCard = IdCard,
                            FullUrl = FullUrl
                        };

                        var data = _queryProcessor.Execute(query2);
                        if (data.access_mode == "")
                        {
                            ViewBag.FbbConstant = GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);
                            ViewBag.TopUp = "1";
                            ViewBag.LabelFBBTR000 = GetGeneralScreenConfig();
                            ViewBag.LabelFBBTR001 = GetCoverageScreenConfig();
                            ViewBag.LabelFBBOR015 = GetProfilePrePostPaid();
                            ViewBag.LabelFBBTR010 = GetVas_Select_Package_ScreenConfig(); // 25.05.2023 deeflink
                            ViewBag.MyAISBypass = "Y";
                            ViewBag.FlagAccessType = "AccessTypeFail";
                            ViewBag.PageShow = "LoginFail";
                            return View("New_SearchProfileTopupPlaybox");
                        }
                        model.v_owner_product = data.v_owner_product;
                        model.v_package_subtype = data.v_package_subtype;
                        model.v_sff_main_promotionCD = data.v_sff_main_promotionCD;

                        var IDCardName = base.LovData
                            .Where(l => l.Type.Equals(WebConstants.LovConfigName.CardType))
                            .Select(l => new DropdownModel
                            {
                                Text = (lang == "TH" ? l.LovValue1 : l.LovValue2),
                                Value = l.Name,
                                DefaultValue = l.DefaultValue,
                            }).ToList().Find(x => x.Value == CardType);

                        model.CustomerRegisterPanelModel.L_CARD_TYPE = IDCardName.Text;


                        if (data.ListPromotion.Count > 0)
                        {

                            List<PackagePromotionModel> listPackagePromotion = new List<PackagePromotionModel>();

                            for (var index = 0; index < data.ListPromotion.Count; index++)
                            {

                                PackagePromotionModel pModel = new PackagePromotionModel();
                                pModel.ProductClass = data.ListPromotion[index].productClass;
                                pModel.ProductType = data.ListPromotion[index].productType;
                                pModel.ProductCd = data.ListPromotion[index].productCD;
                                pModel.EndDate = data.ListPromotion[index].endDate;

                                listPackagePromotion.Add(pModel);
                            }
                            model.PackagePromotionList = listPackagePromotion;
                        }

                        ViewBag.SFFProductName = model.outProductName;
                        ViewBag.SFFServiceYear = model.outDayOfServiceYear;
                        ViewBag.MobileAis = model.CoveragePanelModel.P_MOBILE;

                        //R17.9 Speed boost
                        if (model.PackagePromotionList.Any())
                        {
                            PackageTopupInternetNotUseModel NotUseData = new PackageTopupInternetNotUseModel();
                            List<CurrentPromotionData> ListCurrentPromotion = new List<CurrentPromotionData>();
                            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
                            foreach (var row in model.PackagePromotionList)
                            {
                                row.IsShowPacage = "Y";
                                if (!string.IsNullOrEmpty(row.ProductCd) && !string.IsNullOrEmpty(row.EndDate))
                                {
                                    if (row.EndDate.ToDateTime() > DateTime.Now)
                                    {
                                        row.IsShowPacage = "N";
                                    }
                                }
                                CurrentPromotionData currentPromotionData = new CurrentPromotionData()
                                {
                                    product_cd = row.ProductCd,
                                    product_class = row.ProductClass,
                                    start_date = row.StartDate,
                                    end_date = row.EndDate,
                                    product_status = row.ProductStatus
                                };
                                ListCurrentPromotion.Add(currentPromotionData);

                            }
                            PackageTopupInternetNotUseQuery queryNotUse = new PackageTopupInternetNotUseQuery
                            {
                                NonMobileNo = NonMobileNo,
                                ListCurrentPromotion = ListCurrentPromotion
                            };

                            NotUseData = _queryProcessor.Execute(queryNotUse);

                            if (NotUseData != null && NotUseData.ListPackageTopupInternetNotUse != null && NotUseData.ListPackageTopupInternetNotUse.Count > 0)
                            {
                                model.PackageTopupInternetNotUseList = NotUseData.ListPackageTopupInternetNotUse.Where(a => !string.IsNullOrEmpty(a.sff_promotion_code)).ToList();
                            }
                        }
                    }
                    else
                    {
                        // Login fail
                        IdCard = "";
                        CardType = "";
                        ViewBag.PageShow = "LoginFail";
                    }
                }
                else
                {
                    // value in put ไม่ถูกต้อง
                    ViewBag.LanguagePage = "1";
                    SiteSession.CurrentUICulture = 1;
                    Session["CurrentUICulture"] = 1;
                    ViewBag.PageShow = "LoginFail";
                }
            }

            else
            {
                ViewBag.PageShow = "LoginFail";
            }

            string TransactionID = NonMobileNo + ipAddress;

            InterfaceLogCommand log = null;
            log = StartInterface("DataEncrypt: " + Data + "\r\n NonMobileNo: " + NonMobileNo + "\r\n Language: " + lang + "\r\n timeStamp: " + timeStamp, "/process/TopupInternetPromotion", TransactionID, "", "TopupInternetGET");

            EndInterface("", log, TransactionID, "Success", "");

            ViewBag.Vas = "4";
            model.SummaryPanelModel.VAS_FLAG = "4";
            ViewBag.MobileFromBypass = NonMobileNo;

            if (Session["EndProcessFlag"].ToSafeBoolean())
            {
                Session["PopupStatus"] = "Success";
                Session["EndProcessFlag"] = null;
            }
            else
                Session["PopupStatus"] = null;

            SaveStatlog("CUSTOMER", "CUSTOMER", ipAddress, "FBB REGISTER ONTOP INTERNET", "", "");

            ViewBag.User = base.CurrentUser;

            ViewBag.LabelFBBTR000 = GetGeneralScreenConfig();
            ViewBag.LabelFBBTR001 = GetCoverageScreenConfig();
            ViewBag.LabelFBBTR002 = GetDisplayPackageScreenConfig();
            ViewBag.LabelFBBTR003 = GetCustRegisterScreenConfig();
            ViewBag.LabelFBBTR004 = GetSummaryScreenConfig();
            ViewBag.LabelFBBTR010 = GetVas_Select_Package_ScreenConfig();
            ViewBag.LabelFBBORV11 = GetVasPopUpScreenConfig();
            ViewBag.LabelFBBORV12 = GetTopUpPlaybox_ScreenConfig();
            ViewBag.LabelFBBORV24 = GetTopUpInternet_ScreenConfig();

            ViewBag.FbbConstant = GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);
            ViewBag.FbbException = GetFbbConstantModel(WebConstants.LovConfigName.FbbExceptionIdCard);
            ViewBag.Fbb_Service = GetDisplay_Select_Type_Service();

            ViewBag.MeshConfig = GetMeshConfig_ScreenConfig();

            ViewBag.Version = GetVersion();
            model.ClientIP = ipAddress;

            ViewBag.FbbConstant = GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);
            ViewBag.ContentPlaybox = GetLovConfigBytype("NEW_CONTENT_PLAYBOX");
            ViewBag.ExistingFlag = "INTERNET";

            if (ViewBag.PageShow == "LoginFail")
            {

                if (Data == "")
                    ViewBag.PageShow = "";

                ViewBag.LabelFBBOR015 = GetProfilePrePostPaid();

                ViewBag.NamePackageFail = PACKAGE_NAME;
                ViewBag.Vas = "";
                ViewBag.LSAVE = LSAVE;
                ViewBag.SWiFi = SWiFi;
                ViewBag.LanguagePage = LanguagePage;
                ViewBag.LPOPUPSAVE = LPOPUPSAVE;
                ViewBag.LCLOSE = LCLOSE;
                ViewBag.SaveSuccess = SaveSuccess;
                ViewBag.DUP = DUP;
                ViewBag.ExistingFlag = ExistingPopupFlag == "Y" ? "INTERNET:MENU" : "INTERNET";
                ViewBag.ExistingPopupFlag = ExistingPopupFlag;

                return View("New_SearchProfileTopupPlaybox");
            }
            else
            {
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
                            ViewBag.LanguagePage = "1";
                        }
                    }
                    else
                    {
                        if (data.LovValue2 != null)
                        {
                            ViewBag.LCLOSE = data.LovValue2.ToString();
                            ViewBag.LanguagePage = "2";
                        }
                    }
                }
                else
                {
                    ViewBag.LCLOSE = "";
                }

                return View("Index", model);
            }
        }

        [HttpPost]
        public ActionResult TopupInternet(string Data = "", string ExistingPopupFlag = "")
        {
            if (Session["PageLoadTopupInternet"] == null)
            {
                PageLoadOntopModel tmpModel = new PageLoadOntopModel();
                tmpModel.Data = Data;
                ViewBag.PageGo = "TopupInternet";
                Session["PageLoadTopupInternet"] = "HaveLoad";
                return View("PageLoadOntop", tmpModel);
            }
            else
            {
                Session["PageLoadTopupInternet"] = null;
            }

            ViewBag.hasLoadingBlock = true;
            //R24.05 Add loading block screen by max kunlp885
            //if (checkLoadingBlock("TopupInternet", Data))
            //{
            //    return View("ExistingFibreExtensions/_Loading");
            //}
            //end R24.05 Add loading block screen by max kunlp885


            QuickWinPanelModel model = new QuickWinPanelModel();

            string FullUrl = "";
            if (Session["FullUrl"] != null)
                FullUrl = Session["FullUrl"].ToSafeString();
            Session["FullUrl"] = this.Url.Action("TopupInternet", "Process", null, this.Request.Url.Scheme);

            string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress))
            {
                ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }

            string NonMobileNo = "";
            string GetIdCardStatus = "";
            string lang = "";
            string timeStamp = "";
            string cardType = "";
            string cardNo = "";

            if (Data != "")
            {
                bool CheckInput = true;
                string DataDec = Decrypt(Data);

                //DataDec = string.IsNullOrEmpty(DataDec) ? "mobileNo=8850125473&lang=TH&timeStamp=202404041030123&cardType=ID_CARD&cardNo=3226178612217" : DataDec;//R24.04 Add loading block screen on /topup, /topupinternet and /topupmesh by max kunlp885

                //string DataDec = "mobileNo=" + Data + "&lang="+ Data2; 
                string[] DataTemps = DataDec.Split('&');
                //string channel = "";
                //string transactionId = "";

                foreach (var item in DataTemps)
                {
                    string[] DataTemp = item.Split('=');
                    if (DataTemp != null)
                    {
                        //if (DataTemp[0].ToSafeString() == "channel")
                        //{
                        //    channel = DataTemp[1].ToSafeString();
                        //}
                        if (DataTemp[0].ToSafeString() == "mobileNo")
                        {
                            NonMobileNo = DataTemp[1].ToSafeString();
                            //NonMobileNo = "8800022021";
                            ViewBag.NonMobileNo = NonMobileNo;
                            string checkNonMobileNo = NonMobileNo.Substring(0, 3);
                            var getLovForcheckMobileandFlag = GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);
                            var getLovCheckMobileNo = getLovForcheckMobileandFlag.FirstOrDefault(x => x.Field == "Existing_Check_Prefix_3BB").Validation;
                            var getLovFlagCheck3BB = getLovForcheckMobileandFlag.FirstOrDefault(x => x.Field == "Existing_Check_3BB_Flag").Validation;
                            if (getLovFlagCheck3BB == "Y")
                            {
                                if (checkNonMobileNo == getLovCheckMobileNo)
                                {
                                    if (Data == "")
                                        ViewBag.PageShow = "";
                                    ViewBag.FbbConstant = GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);
                                    ViewBag.TopUp = "1";
                                    ViewBag.LabelFBBTR000 = GetGeneralScreenConfig();
                                    ViewBag.LabelFBBTR001 = GetCoverageScreenConfig();
                                    ViewBag.LabelFBBOR015 = GetProfilePrePostPaid();
                                    ViewBag.LabelFBBTR010 = GetVas_Select_Package_ScreenConfig(); // 25.05.2023 deeflink
                                    ViewBag.MyAISBypass = "Y";
                                    ViewBag.Flag3BB = "3BB";
                                    ViewBag.PageShow = "LoginFail";
                                    return View("New_SearchProfileTopupPlaybox");
                                }
                            }
                        }
                        if (DataTemp[0].ToSafeString() == "lang")
                        {
                            lang = DataTemp[1].ToSafeString();
                            if (lang == "TH")
                            {
                                ViewBag.LanguagePage = "1";
                                SiteSession.CurrentUICulture = 1;
                                Session["CurrentUICulture"] = 1;
                            }
                            else
                            {
                                ViewBag.LanguagePage = "2";
                                SiteSession.CurrentUICulture = 2;
                                Session["CurrentUICulture"] = 2;
                            }
                        }
                        if (DataTemp[0].ToSafeString() == "cardType")
                        {
                            cardType = DataTemp[1].ToSafeString();
                        }
                        if (DataTemp[0].ToSafeString() == "cardNo")
                        {
                            cardNo = DataTemp[1].ToSafeString();
                        }
                    }
                    else
                    {
                        // value in put ไม่ถูกต้อง
                        CheckInput = false;
                        break;

                    }
                }

                if (cardType == "" || cardNo == "")
                {
                    CheckInput = false;
                }
                else
                {
                    List<FbbConstantModel> cardtypeDatas = GetFbbConstantModel("ID_CARD_TYPE");
                    if (cardtypeDatas != null && cardtypeDatas.Count > 0)
                    {
                        List<FbbConstantModel> cardtypeData = cardtypeDatas.Where(t => t.Field == cardType).ToList();
                        if (cardtypeData == null || cardtypeData.Count == 0)
                        {
                            CheckInput = false;
                        }
                    }
                }

                if (CheckInput)
                {
                    GetIdCardStatus = GetInfoByNonMobileNo(NonMobileNo);
                    if (GetIdCardStatus == "" && !string.IsNullOrEmpty(IdCard))
                    {
                        ViewBag.IdCard = IdCard;
                        ViewBag.CardType = CardType;
                        ViewBag.PageShow = "Mobile";

                        ViewBag.User = base.CurrentUser;

                        model = massCommonBypass(NonMobileNo, IdCard, CardType, "TopupInternet", ViewBag.User, FullUrl);
                        model.CoveragePanelModel.P_MOBILE = NonMobileNo;

                        var query2 = new evOMQueryListServiceAndPromotionByPackageTypeQuery
                        {
                            mobileNo = NonMobileNo,//"8850001230",
                            idCard = IdCard,
                            FullUrl = FullUrl
                        };

                        var data = _queryProcessor.Execute(query2);
                        if (data.access_mode == "")
                        {
                            ViewBag.FbbConstant = GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);
                            ViewBag.TopUp = "1";
                            ViewBag.LabelFBBTR000 = GetGeneralScreenConfig();
                            ViewBag.LabelFBBTR001 = GetCoverageScreenConfig();
                            ViewBag.LabelFBBOR015 = GetProfilePrePostPaid();
                            ViewBag.LabelFBBTR010 = GetVas_Select_Package_ScreenConfig(); // 25.05.2023 deeflink
                            ViewBag.MyAISBypass = "Y";
                            ViewBag.FlagAccessType = "AccessTypeFail";
                            ViewBag.PageShow = "LoginFail";
                            return View("New_SearchProfileTopupPlaybox");
                        }
                        model.v_owner_product = data.v_owner_product;
                        model.v_package_subtype = data.v_package_subtype;
                        model.v_sff_main_promotionCD = data.v_sff_main_promotionCD;

                        var IDCardName = base.LovData
                            .Where(l => l.Type.Equals(WebConstants.LovConfigName.CardType))
                            .Select(l => new DropdownModel
                            {
                                Text = (lang == "TH" ? l.LovValue1 : l.LovValue2),
                                Value = l.Name,
                                DefaultValue = l.DefaultValue,
                            }).ToList().Find(x => x.Value == CardType);

                        model.CustomerRegisterPanelModel.L_CARD_TYPE = IDCardName.Text;


                        if (data.ListPromotion.Count > 0)
                        {

                            List<PackagePromotionModel> listPackagePromotion = new List<PackagePromotionModel>();

                            for (var index = 0; index < data.ListPromotion.Count; index++)
                            {

                                PackagePromotionModel pModel = new PackagePromotionModel();
                                pModel.ProductClass = data.ListPromotion[index].productClass.ToSafeString();
                                pModel.ProductType = data.ListPromotion[index].productType.ToSafeString();
                                pModel.ProductCd = data.ListPromotion[index].productCD.ToSafeString();
                                pModel.EndDate = data.ListPromotion[index].endDate.ToSafeString();
                                pModel.StartDate = data.ListPromotion[index].startDate.ToSafeString();
                                pModel.ProductStatus = data.ListPromotion[index].productStatus.ToSafeString();
                                listPackagePromotion.Add(pModel);
                            }
                            //List<PackagePromotionModel> PackagePromotionList
                            model.PackagePromotionList = listPackagePromotion;
                        }

                        ViewBag.SFFProductName = model.outProductName;
                        ViewBag.SFFServiceYear = model.outDayOfServiceYear;
                        ViewBag.MobileAis = model.CoveragePanelModel.P_MOBILE;

                        //R17.9 Speed boost
                        if (model.PackagePromotionList.Any())
                        {
                            PackageTopupInternetNotUseModel NotUseData = new PackageTopupInternetNotUseModel();
                            List<CurrentPromotionData> ListCurrentPromotion = new List<CurrentPromotionData>();
                            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
                            foreach (var row in model.PackagePromotionList)
                            {
                                row.IsShowPacage = "Y";
                                if (!string.IsNullOrEmpty(row.ProductCd) && !string.IsNullOrEmpty(row.EndDate))
                                {
                                    if (row.EndDate.ToDateTime() > DateTime.Now)
                                    {
                                        row.IsShowPacage = "N";
                                    }
                                }

                                CurrentPromotionData currentPromotionData = new CurrentPromotionData()
                                {
                                    product_cd = row.ProductCd,
                                    product_class = row.ProductClass,
                                    start_date = row.StartDate,
                                    end_date = row.EndDate,
                                    product_status = row.ProductStatus
                                };
                                ListCurrentPromotion.Add(currentPromotionData);
                            }
                            PackageTopupInternetNotUseQuery queryNotUse = new PackageTopupInternetNotUseQuery
                            {
                                NonMobileNo = NonMobileNo,
                                ListCurrentPromotion = ListCurrentPromotion
                            };

                            NotUseData = _queryProcessor.Execute(queryNotUse);

                            if (NotUseData != null && NotUseData.ListPackageTopupInternetNotUse != null && NotUseData.ListPackageTopupInternetNotUse.Count > 0)
                            {
                                model.PackageTopupInternetNotUseList = NotUseData.ListPackageTopupInternetNotUse.Where(a => !string.IsNullOrEmpty(a.sff_promotion_code)).ToList();
                            }
                        }
                    }
                    else
                    {
                        // Login fail
                        IdCard = "";
                        CardType = "";
                        ViewBag.PageShow = "LoginFail";
                    }
                }
                else
                {
                    // value in put ไม่ถูกต้อง
                    ViewBag.LanguagePage = "1";
                    SiteSession.CurrentUICulture = 1;
                    Session["CurrentUICulture"] = 1;
                    ViewBag.PageShow = "LoginFail";
                }
            }
            else
            {
                ViewBag.PageShow = "LoginFail";
            }

            string TransactionID = NonMobileNo + ipAddress;

            InterfaceLogCommand log = null;
            log = StartInterface("DataEncrypt: " + Data + "\r\n NonMobileNo: " + NonMobileNo + "\r\n Language: " + lang + "\r\n timeStamp: " + timeStamp, "/process/TopupInternetPromotion", TransactionID, "", "TopupInternetPOST");

            EndInterface("", log, TransactionID, "Success", "");

            ViewBag.Vas = "4";
            model.SummaryPanelModel.VAS_FLAG = "4";
            ViewBag.MobileFromBypass = NonMobileNo;

            if (Session["EndProcessFlag"].ToSafeBoolean())
            {
                Session["PopupStatus"] = "Success";
                Session["EndProcessFlag"] = null;
            }
            else
                Session["PopupStatus"] = null;

            SaveStatlog("CUSTOMER", "CUSTOMER", ipAddress, "FBB REGISTER ONTOP INTERNET", "", "");

            Logger.Info("Line 4 => Acess through Topup Playbox(BPL) Link");

            ViewBag.User = base.CurrentUser;

            ViewBag.LabelFBBTR000 = GetGeneralScreenConfig();
            ViewBag.LabelFBBTR001 = GetCoverageScreenConfig();
            ViewBag.LabelFBBTR002 = GetDisplayPackageScreenConfig();
            ViewBag.LabelFBBTR003 = GetCustRegisterScreenConfig();
            ViewBag.LabelFBBTR004 = GetSummaryScreenConfig();
            ViewBag.LabelFBBTR010 = GetVas_Select_Package_ScreenConfig();
            ViewBag.LabelFBBORV11 = GetVasPopUpScreenConfig();
            ViewBag.LabelFBBORV12 = GetTopUpPlaybox_ScreenConfig();
            ViewBag.LabelFBBORV24 = GetTopUpInternet_ScreenConfig();

            ViewBag.FbbConstant = GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);
            ViewBag.FbbException = GetFbbConstantModel(WebConstants.LovConfigName.FbbExceptionIdCard);
            ViewBag.Fbb_Service = GetDisplay_Select_Type_Service();

            ViewBag.MeshConfig = GetMeshConfig_ScreenConfig();

            ViewBag.Version = GetVersion();
            model.ClientIP = ipAddress;

            ViewBag.FbbConstant = GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);
            ViewBag.ContentPlaybox = GetLovConfigBytype("NEW_CONTENT_PLAYBOX");
            ViewBag.ExistingFlag = "INTERNET";
            ViewBag.MyAISBypass = "Y";

            if (ViewBag.PageShow == "LoginFail")
            {
                ViewBag.LabelFBBOR015 = GetProfilePrePostPaid();
                return View("New_SearchProfileTopupPlaybox");
            }
            else
            {
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
                            ViewBag.LanguagePage = "1";
                        }
                    }
                    else
                    {
                        if (data.LovValue2 != null)
                        {
                            ViewBag.LCLOSE = data.LovValue2.ToString();
                            ViewBag.LanguagePage = "2";
                        }
                    }
                }
                else
                {
                    ViewBag.LCLOSE = "";
                }
                return View("Index", model);
            }
        }

        //R19.05 TopupMesh
        public ActionResult TopupMesh(string PACKAGE_NAME = "", bool SaveSuccess = false, string LSAVE = "", string LCLOSE = "",
          string LPOPUPSAVE = "", string LanguagePage = "", string SWiFi = "", string DUP = "", string ExistingPopupFlag = "", string Data = "", string statusPay = "",
          string LcCode = "", string ASCCode = "", string EmployeeID = "", string outSubType = "", string NonMobileNo = "", string AddressID = "", string outServiceLevel = "",
          string outTitle = "", string outCompanyName = "", string outDistChn = "", string outChnSales = "", string outShopType = "",
          string outOperatorClass = "", string outASCTitleThai = "", string outASCPartnerName = "", string outMemberCategory = "", string outPosition = "",
          string outLocationRegion = "", string outLocationSubRegion = "", string THFirstName = "", string THLastName = "")
        {
            if (Data != "" && Session["PageLoadTopupMesh"] == null)
            {
                PageLoadOntopModel tmpModel = new PageLoadOntopModel();
                tmpModel.Data = Data;
                ViewBag.PageGo = "TopupMesh";
                Session["PageLoadTopupMesh"] = "HaveLoad";
                return View("PageLoadOntop", tmpModel);
            }
            else
            {
                Session["PageLoadTopupMesh"] = null;
            }

            ViewBag.hasLoadingBlock = true;
            string logStep = "";

            try
            {
                logStep = "Start";
                QuickWinPanelModel model = new QuickWinPanelModel();
                string FullUrl = "";

                Session["FullUrl"] = this.Url.Action("TopupMesh", "Process", null, this.Request.Url.Scheme);

                if (Session["FullUrl"] != null)
                    FullUrl = Session["FullUrl"].ToSafeString();

                string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (string.IsNullOrEmpty(ipAddress))
                {
                    ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                }

                var dataLOV = base.LovData.Where(l => !string.IsNullOrEmpty(l.Name) && l.Name.Equals("B_CLOSE"));
                if (SiteSession.CurrentUICulture.IsThaiCulture())
                { ViewBag.LCLOSE = dataLOV.FirstOrDefault().LovValue1; }
                else
                { ViewBag.LCLOSE = dataLOV.FirstOrDefault().LovValue2; }

                logStep = "Init Parameter for bypass";
                #region check bypass
                string GetIdCardStatus = "";
                string lang = "";
                string timeStamp = "";
                string Channel = "WEB";
                ViewBag.Channel = "WEB";
                bool CheckInput = true;

                logStep = "Start Check isBypass";
                if (Data != "" || NonMobileNo != "")
                {
                    logStep = "Bypass by Existing or others";
                    if (Data != "")
                    {
                        logStep = "Bypass by Subcontract or others";
                        ViewBag.URLRef = Request.UrlReferrer.ToSafeString();
                        logStep = "Decrypt";
                        string DataDec = Decrypt(Data);

                        //DataDec = string.IsNullOrEmpty(DataDec) ? "Channel=BYPASS&Internet_no=8850125378&lang=TH&timeStamp=202404031130111" : DataDec;//R24.04 Add loading block screen on /topup, /topupinternet and /topupmesh by max kunlp885

                        //string DataDec = "mobileNo=" + Data + "&lang="+ Data2; 
                        logStep = "Split Data Decrypted";
                        string[] DataTemps = DataDec.Split('&');
                        logStep = "Loop Input Parameter";
                        foreach (var item in DataTemps)
                        {
                            string[] DataTemp = item.Split('=');
                            if (DataTemp != null)
                            {
                                if (DataTemp[0].ToSafeString() == "Channel")
                                {
                                    ViewBag.Channel = DataTemp[1].ToSafeString();
                                    Channel = DataTemp[1].ToSafeString();
                                }
                                if (DataTemp[0].ToSafeString() == "Staff_id")
                                {
                                    ViewBag.Staff_id = DataTemp[1].ToSafeString();
                                }
                                if (DataTemp[0].ToSafeString() == "Location_code")
                                {
                                    ViewBag.Location_code = DataTemp[1].ToSafeString();
                                }
                                if (DataTemp[0].ToSafeString() == "Install_date")
                                {
                                    ViewBag.Install_date = DataTemp[1].ToSafeString();
                                }
                                if (DataTemp[0].ToSafeString() == "Time_slot")
                                {
                                    ViewBag.Time_slot = DataTemp[1].ToSafeString();
                                }
                                if (DataTemp[0].ToSafeString() == "Internet_no" || DataTemp[0].ToSafeString() == "mobileNo")
                                {
                                    NonMobileNo = DataTemp[1].ToSafeString();
                                    //NonMobileNo = "8800022021";
                                    ViewBag.NonMobileNo = NonMobileNo;
                                    string checkNonMobileNo = NonMobileNo.Substring(0, 3);
                                    var getLovForcheckMobileandFlag = GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);
                                    var getLovCheckMobileNo = getLovForcheckMobileandFlag.FirstOrDefault(x => x.Field == "Existing_Check_Prefix_3BB").Validation;
                                    var getLovFlagCheck3BB = getLovForcheckMobileandFlag.FirstOrDefault(x => x.Field == "Existing_Check_3BB_Flag").Validation;
                                    if (getLovFlagCheck3BB == "Y")
                                    {
                                        if (checkNonMobileNo == getLovCheckMobileNo)
                                        {
                                            if (Data == "")
                                                ViewBag.PageShow = "";
                                            ViewBag.TopUp = "1";
                                            ViewBag.FbbConstant = GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);
                                            ViewBag.LabelFBBTR000 = GetGeneralScreenConfig();
                                            ViewBag.LabelFBBTR001 = GetCoverageScreenConfig();
                                            ViewBag.LabelFBBOR015 = GetProfilePrePostPaid();
                                            ViewBag.LabelFBBTR010 = GetVas_Select_Package_ScreenConfig(); // 25.05.2023 deeflink
                                            ViewBag.MyAISBypass = "Y";
                                            ViewBag.Flag3BB = "3BB";
                                            ViewBag.PageShow = "LoginFail";
                                            return View("New_SearchProfile2");
                                        }
                                    }
                                }
                                if (DataTemp[0].ToSafeString() == "lang")
                                {
                                    lang = DataTemp[1].ToSafeString();
                                    if (lang == "TH")
                                    {
                                        ViewBag.LanguagePage = "1";
                                        SiteSession.CurrentUICulture = 1;
                                        Session["CurrentUICulture"] = 1;
                                    }
                                    else
                                    {
                                        ViewBag.LanguagePage = "2";
                                        SiteSession.CurrentUICulture = 2;
                                        Session["CurrentUICulture"] = 2;
                                    }
                                }
                                if (DataTemp[0].ToSafeString() == "timeStamp")
                                {
                                    timeStamp = DataTemp[1].ToSafeString();
                                }
                                //if (DataTemp[0].ToSafeString() == "transactionId")
                                //{
                                //    transactionId = DataTemp[1].ToSafeString();
                                //}
                            }
                            else
                            {
                                // value in put ไม่ถูกต้อง
                                CheckInput = false;
                                break;

                            }
                        }
                    }
                    else
                    {
                        logStep = "Bypass from Existing then set Parameter";
                        ViewBag.Channel = "Existing";
                        Channel = "Existing";
                        ViewBag.NonMobileNo = NonMobileNo;
                        ViewBag.Staff_id = EmployeeID;
                        ViewBag.Location_code = LcCode;
                        ViewBag.ASC_Code = ASCCode;
                        ViewBag.outSubType = outSubType;

                    }
                    logStep = "Check Data from Bypass";
                    if (CheckInput)
                    {
                        logStep = "Have Data from Bypass " + NonMobileNo;
                        GetIdCardStatus = GetInfoByNonMobileNo(NonMobileNo);
                        logStep = "Check Card Status";
                        if (GetIdCardStatus == "" && !string.IsNullOrEmpty(IdCard))
                        {
                            logStep = "Have IDCard " + IdCard;
                            ViewBag.IdCard = IdCard;
                            ViewBag.CardType = CardType;
                            ViewBag.PageShow = "Mobile";

                            ViewBag.User = base.CurrentUser;

                            logStep = "Call MassCommon Bypass";
                            model = massCommonBypass(NonMobileNo, IdCard, CardType, "TopupMesh", ViewBag.User, FullUrl);
                            model.CoveragePanelModel.P_MOBILE = NonMobileNo;

                            logStep = "Call PersonalPromotion";
                            ViewBag.personalPromotion = personalPromotionMesh(NonMobileNo);

                            logStep = "Call evOMQueryListServiceAndPromotionByPackageTypeQuery";
                            var query2 = new evOMQueryListServiceAndPromotionByPackageTypeQuery
                            {
                                mobileNo = NonMobileNo,//"8850001230",
                                idCard = IdCard,
                                FullUrl = FullUrl
                            };

                            var data = _queryProcessor.Execute(query2);
                            if (data.access_mode == "")
                            {
                                ViewBag.FbbConstant = GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);
                                ViewBag.TopUp = "1";
                                ViewBag.LabelFBBTR000 = GetGeneralScreenConfig();
                                ViewBag.LabelFBBTR001 = GetCoverageScreenConfig();
                                ViewBag.LabelFBBOR015 = GetProfilePrePostPaid();
                                ViewBag.LabelFBBTR010 = GetVas_Select_Package_ScreenConfig(); // 25.05.2023 deeflink
                                ViewBag.MyAISBypass = "Y";
                                ViewBag.FlagAccessType = "AccessTypeFail";
                                ViewBag.PageShow = "LoginFail";
                                return View("New_SearchProfile2");
                            }
                            logStep = "Store Data from evOMQueryListServiceAndPromotionByPackageTypeQuery";
                            model.v_owner_product = data.v_owner_product;
                            model.v_package_subtype = data.v_package_subtype;
                            model.v_sff_main_promotionCD = data.v_sff_main_promotionCD;
                            ViewBag.AddressID = data.addressId;

                            model.CustomerRegisterPanelModel.v_installAddress = data.v_installAddress;
                            model.CustomerRegisterPanelModel.installAddress1 = data.v_installAddress1;
                            model.CustomerRegisterPanelModel.installAddress2 = data.v_installAddress2;
                            model.CustomerRegisterPanelModel.installAddress3 = data.v_installAddress3;
                            model.CustomerRegisterPanelModel.installAddress4 = data.v_installAddress4;
                            model.CustomerRegisterPanelModel.installAddress5 = data.v_installAddress5;
                            model.CustomerRegisterPanelModel.pbox_count = data.v_number_of_pb_number.ToString();

                            logStep = "Card Type";
                            if (lang != "")
                            {
                                var IDCardName = base.LovData
                                    .Where(l => l.Type.Equals(WebConstants.LovConfigName.CardType))
                                    .Select(l => new DropdownModel
                                    {
                                        Text = (lang == "TH" ? l.LovValue1 : l.LovValue2),
                                        Value = l.Name,
                                        DefaultValue = l.DefaultValue,
                                    }).ToList().Find(x => x.Value == CardType);

                                model.CustomerRegisterPanelModel.L_CARD_TYPE = IDCardName.Text;
                            }
                            else
                            {
                                model.CustomerRegisterPanelModel.L_CARD_TYPE = model.IDCardType;
                            }

                            logStep = "Check ListPromotion";
                            if (data.ListPromotion.Count > 0)
                            {

                                List<PackagePromotionModel> listPackagePromotion = new List<PackagePromotionModel>();

                                for (var index = 0; index < data.ListPromotion.Count; index++)
                                {

                                    PackagePromotionModel pModel = new PackagePromotionModel();
                                    pModel.ProductClass = data.ListPromotion[index].productClass;
                                    pModel.ProductType = data.ListPromotion[index].productType;
                                    pModel.ProductCd = data.ListPromotion[index].productCD;
                                    pModel.EndDate = data.ListPromotion[index].endDate;

                                    listPackagePromotion.Add(pModel);
                                }
                                //List<PackagePromotionModel> PackagePromotionList
                                model.PackagePromotionList = listPackagePromotion;
                            }

                            logStep = "Store More Data";
                            ViewBag.SFFProductName = model.outProductName;
                            ViewBag.SFFServiceYear = model.outDayOfServiceYear;
                            ViewBag.MobileAis = model.CoveragePanelModel.P_MOBILE;

                            logStep = "Call Check Technology";
                            var checkTech = new GetMeshCheckTechnologyQuery
                            {
                                addressID = data.addressId
                            };

                            var modelCheckTech = _queryProcessor.Execute(checkTech);

                            /// Update PRODUCT_SUBTYPE R.0920
                            if (modelCheckTech != null && modelCheckTech.PRODUCT_SUBTYPE.ToSafeString() != "")
                            {
                                model.v_package_subtype = modelCheckTech.PRODUCT_SUBTYPE;
                            }

                            logStep = "Call Check AROrder";
                            var chkAROrder = new CheckAROrderQuery
                            {
                                BroadbandId = NonMobileNo,//"8800024585",
                                Option = "Mesh",
                                FullUrl = ""
                            };

                            var dataAROrder = _queryProcessor.Execute(chkAROrder);

                            logStep = "Check Show Error from Tech and AR";
                            if (modelCheckTech.RETURN_CODE != "0" || (dataAROrder.Count > 0 && dataAROrder.Where(l => l.installFlag.Equals("N")).ToList().Count > 0))
                            {
                                ViewBag.PageShow = "LoginFail";
                                if (modelCheckTech.RETURN_CODE != "0")
                                    ViewBag.showErrorText = "Technology";
                                else
                                    ViewBag.showErrorText = "AROrder";
                            }

                            model.CoverageAreaResultModel.ADDRESS_ID = AddressID;

                        }
                        else
                        {
                            // Login fail
                            logStep = "Not Found IDCard";
                            IdCard = "";
                            CardType = "";
                            ViewBag.PageShow = "LoginFail";
                        }
                    }
                    else
                    {
                        // value in put ไม่ถูกต้อง
                        logStep = "Missing Data";
                        ViewBag.LanguagePage = "1";
                        SiteSession.CurrentUICulture = 1;
                        Session["CurrentUICulture"] = 1;
                        ViewBag.PageShow = "LoginFail";
                    }
                }

                else
                {
                    logStep = "Not Bypass";
                    ViewBag.PageShow = "LoginFail";
                }

                #endregion

                logStep = "Log Data";
                string TransactionID = NonMobileNo + ipAddress;

                InterfaceLogCommand log = null;
                log = StartInterface("DataEncrypt: " + Data + "\r\n NonMobileNo: " + NonMobileNo + "\r\n Channel: " + Channel + "\r\n timeStamp: " + timeStamp, "/process/TopupMesh", TransactionID, "", "TopupMeshGET");

                EndInterface("", log, TransactionID, "Success", "");

                ViewBag.TopUp = "1";
                model.SummaryPanelModel.TOPUP = "1";
                ViewBag.MobileFromBypass = NonMobileNo;
                if (NonMobileNo != "")
                {
                    model.CoveragePanelModel.P_MOBILE = NonMobileNo;
                }

                logStep = "Check is from Bypass";
                if (Data == "")
                {
                    logStep = "Not from Bypass store some Data when from officer";
                    model.CustomerRegisterPanelModel.L_STAFF_ID = EmployeeID;
                    model.CustomerRegisterPanelModel.L_LOC_CODE = LcCode;
                    model.CustomerRegisterPanelModel.L_ASC_CODE = ASCCode;
                    model.CustomerRegisterPanelModel.outSubType = outSubType;

                }
                else
                {
                    logStep = "From Bypass";
                    model.CustomerRegisterPanelModel.L_STAFF_ID = ViewBag.Staff_id;
                    model.CustomerRegisterPanelModel.L_LOC_CODE = ViewBag.Location_code;
                }

                logStep = "Check Session EndProcessFlag";
                if (Session["EndProcessFlag"].ToSafeBoolean())
                {
                    Session["PopupStatus"] = "Success";
                    Session["EndProcessFlag"] = null;
                }
                else
                    Session["PopupStatus"] = null;

                logStep = "SaveStatlog";
                SaveStatlog("CUSTOMER", "CUSTOMER", ipAddress, "FBB REGISTER MESH", "", "");

                Logger.Info("Line 4 => Acess through TopupMesh Link");

                logStep = "Get base CurrentUser";
                ViewBag.User = base.CurrentUser;

                logStep = "Get a lot of LOV";
                ViewBag.LabelFBBTR000 = GetGeneralScreenConfig();
                ViewBag.LabelFBBTR001 = GetCoverageScreenConfig();
                ViewBag.LabelFBBTR002 = GetDisplayPackageScreenConfig();
                ViewBag.LabelFBBTR003 = GetCustRegisterScreenConfig();
                ViewBag.LabelFBBTR004 = GetSummaryScreenConfig();
                ViewBag.LabelFBBTR010 = GetVas_Select_Package_ScreenConfig();
                ViewBag.LabelFBBORV11 = GetVasPopUpScreenConfig();
                ViewBag.LabelFBBORV41 = GetTopUpMesh_ScreenConfig();
                ViewBag.MeshConfig = GetMeshConfig_ScreenConfig();
                //ViewBag.LabelFBBORV12 = GetTopUpPlaybox_ScreenConfig();

                logStep = "GET LOV Constant";
                ViewBag.FbbConstant = GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);
                ViewBag.FbbException = GetFbbConstantModel(WebConstants.LovConfigName.FbbExceptionIdCard);
                ViewBag.Fbb_Service = GetDisplay_Select_Type_Service();

                logStep = "Get Version and IP";
                ViewBag.Version = GetVersion();
                model.ClientIP = ipAddress;

                logStep = "Get LOV PLAYBOX, Sorry forget to delete this";
                ViewBag.ContentPlaybox = GetLovConfigBytype("NEW_CONTENT_PLAYBOX");
                ViewBag.ExistingFlag = "MESH";
                model.TopupMesh = "Y";
                //20.3 Service Level
                ViewBag.IsSetServiceLevel = false;

                //R20.5 add by Aware : Atipon Wiparsmongkol
                model.CustomerRegisterPanelModel.ServiceLevel = outServiceLevel;

                logStep = "Check Show ErrorPage";
                if (ViewBag.PageShow == "LoginFail")
                {
                    logStep = "Date is null it's bot error page";
                    if (Data == "")
                        ViewBag.PageShow = "";

                    logStep = "GetProfilePrePostPaid";
                    ViewBag.LabelFBBOR015 = GetProfilePrePostPaid();

                    logStep = "set ViewBag to show on Screen";
                    ViewBag.NamePackageFail = PACKAGE_NAME;
                    ViewBag.Vas = "";
                    ViewBag.LSAVE = LSAVE;
                    ViewBag.SWiFi = SWiFi;
                    ViewBag.LanguagePage = LanguagePage;
                    ViewBag.LPOPUPSAVE = LPOPUPSAVE;
                    ViewBag.SaveSuccess = SaveSuccess;
                    ViewBag.DUP = DUP;
                    ViewBag.ExistingFlag = ExistingPopupFlag == "Y" ? "MESH:MENU" : "MESH";
                    ViewBag.ExistingPopupFlag = ExistingPopupFlag;

                    logStep = "check statusPay";
                    if (statusPay != "")
                    {
                        ViewBag.BackFromPaymentMsg = statusPay;
                    }

                    logStep = "Return View SearchProfile2";
                    return View("New_SearchProfile2");

                }
                else
                {
                    logStep = "Return View Index";
                    //R24.05 Add loading block screen by max kunlp885
                    //if (checkLoadingBlock("TopupMesh", Data))
                    //{
                    //    return View("ExistingFibreExtensions/_Loading");
                    //}
                    //end R24.05 Add loading block screen by max kunlp885

                    return View("Index", model);
                }

            }
            catch (Exception ex)
            {
                //EndInterface("", log2, "TOPUPMESH_LOG", "ERROR", "logStep: " + logStep + "\r\n" + "ErrorMessage: " + ex.GetErrorMessage() + "\r\n" + "RenderExceptionMessage: " + ex.RenderExceptionMessage());
                return View("New_SearchProfile2");
            }

            //
        }

        //R22.03 TopupReplace
        public ActionResult TopupReplace(bool SaveTopupReplace = false, bool SaveSuccess = false, string LSAVE = "", string LCLOSE = "",
            string LPOPUPSAVE = "", string LanguagePage = "", string SWiFi = "", string ExistingPopupFlag = "")
        {
            ViewBag.hasLoadingBlock = true;
            /*
            //R24.05 Add loading block screen by max kunlp885
            if (checkLoadingBlock("TopupReplace", ""))
             {
                 Session["LoadingBlockSaveTopupReplace"] = SaveTopupReplace;
                 Session["LoadingBlockSaveSuccess"] = SaveSuccess;
                 Session["LoadingBlockLabelSave"] = LSAVE;
                 Session["LoadingBlockLabelClose"] = LCLOSE;
                 Session["LoadingBlockPopupSave"] = LPOPUPSAVE;
                 Session["LoadingBlockLanguagePage"] = LanguagePage;
                 Session["LoadingBlockSWiFi"] = SWiFi;
                 Session["LoadingBlockExistingPopupFlag"] = ExistingPopupFlag;
                 return View("ExistingFibreExtensions/_Loading");
             }
             SaveTopupReplace = (bool)Session["LoadingBlockSaveTopupReplace"];
             SaveSuccess = (bool)Session["LoadingBlockSaveSuccess"];
             LSAVE = Session["LoadingBlockLabelSave"].ToSafeString();
             LCLOSE = Session["LoadingBlockLabelClose"].ToSafeString();
             LPOPUPSAVE = Session["LoadingBlockPopupSave"].ToSafeString();
             LanguagePage = Session["LoadingBlockLanguagePage"].ToSafeString();
             SWiFi = Session["LoadingBlockSWiFi"].ToSafeString();
             ExistingPopupFlag = Session["LoadingBlockExistingPopupFlag"].ToSafeString();
             //end R24.05 Add loading block screen by max kunlp885
             */

            Session["FullUrl"] = this.Url.Action("TopupReplace", "Process", null, this.Request.Url.Scheme);

            string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (string.IsNullOrEmpty(ipAddress))
            {
                ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
            Logger.Info("IP Address: " + ipAddress);

            if (Session["EndProcessFlag"].ToSafeBoolean())
            {
                Session["PopupStatus"] = "Success";
                Session["EndProcessFlag"] = null;
            }
            else
                Session["PopupStatus"] = null;

            ViewBag.TopUp = "1";
            ViewBag.LabelFBBTR000 = GetGeneralScreenConfig();
            ViewBag.LabelFBBTR001 = GetCoverageScreenConfig();
            ViewBag.LabelFBBTR002 = GetDisplayPackageScreenConfig();
            ViewBag.LabelFBBTR003 = GetCustRegisterScreenConfig();
            ViewBag.LabelFBBTR004 = GetSummaryScreenConfig();
            ViewBag.LabelFBBTR010 = GetVas_Select_Package_ScreenConfig();
            ViewBag.LabelFBBORV11 = GetVasPopUpScreenConfig();
            ViewBag.LabelFBBOR015 = GetProfilePrePostPaid();
            ViewBag.LabelFBBOR050 = GetTopUpReplace_ScreenConfig();
            ViewBag.FbbException = GetFbbConstantModel(WebConstants.LovConfigName.FbbExceptionIdCard);
            ViewBag.Fbb_Service = GetDisplay_Select_Type_Service();
            ViewBag.Version = GetVersion();
            ViewBag.Vas = "7";
            ViewBag.User = base.CurrentUser;
            ViewBag.MeshConfig = GetMeshConfig_ScreenConfig();

            ViewBag.SaveTopupReplace = SaveTopupReplace;
            ViewBag.LSAVE = LSAVE;
            ViewBag.SWiFi = SWiFi;
            ViewBag.LanguagePage = LanguagePage;
            ViewBag.LPOPUPSAVE = LPOPUPSAVE;
            ViewBag.LCLOSE = LCLOSE;
            ViewBag.SaveSuccess = SaveSuccess;
            ViewBag.FbbConstant = GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);
            ViewBag.ExistingFlag = ExistingPopupFlag == "Y" ? "PLAYBOXREPLACE:MENU" : "PLAYBOXREPLACE";
            ViewBag.ExistingPopupFlag = ExistingPopupFlag;
            return View("New_SearchProfile2");
        }
        //IPCAMERA 23.06 Kong
        public ActionResult TopupIPCamera(string PACKAGE_NAME = "", bool SaveSuccess = false, string LSAVE = "", string LCLOSE = "",
          string LPOPUPSAVE = "", string LanguagePage = "", string SWiFi = "", string DUP = "", string ExistingPopupFlag = "", string Data = "", string statusPay = "",
          string LcCode = "", string ASCCode = "", string EmployeeID = "", string outSubType = "", string NonMobileNo = "", string AddressID = "", string outServiceLevel = "",
          string outTitle = "", string outCompanyName = "", string outDistChn = "", string outChnSales = "", string outShopType = "",
          string outOperatorClass = "", string outASCTitleThai = "", string outASCPartnerName = "", string outMemberCategory = "", string outPosition = "",
          string outLocationRegion = "", string outLocationSubRegion = "", string THFirstName = "", string THLastName = "")
        {
            if (Data != "" && Session["PageLoadTopupIPCamera"] == null)
            {
                PageLoadOntopModel tmpModel = new PageLoadOntopModel();
                tmpModel.Data = Data;
                ViewBag.PageGo = "TopupIPCamera";
                Session["PageLoadTopupIPCamera"] = "HaveLoad";
                return View("PageLoadOntop", tmpModel);
            }
            else
            {
                Session["PageLoadTopupIPCamera"] = null;
            }

            ViewBag.hasLoadingBlock = true;

            QuickWinPanelModel model = new QuickWinPanelModel();
            string FullUrl = "";

            Session["FullUrl"] = this.Url.Action("TopupIPCamera", "Process", null, this.Request.Url.Scheme);

            if (Session["FullUrl"] != null)
                FullUrl = Session["FullUrl"].ToSafeString();
            string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (string.IsNullOrEmpty(ipAddress))
            {
                ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
            string TransactionID = NonMobileNo + ipAddress;
            Logger.Info("IP Address: " + ipAddress);

            string GetIdCardStatus = "";
            string lang = "";
            string timeStamp = "";
            string Channel = "WEB";
            ViewBag.Channel = "WEB";
            bool CheckInput = true;
            ViewBag.IpCamera = GetIpCameraScreenConfig();//23.06 IPCAMERA


            if (Data != "" || NonMobileNo != "")
            {
                ViewBag.AisAirNumberByPass = NonMobileNo;
                if (Data != "")
                {
                    ViewBag.IsSubContact = true;
                    ViewBag.URLRef = Request.UrlReferrer.ToSafeString();
                    string DataDec = Decrypt(Data);
                    //string DataDec = "mobileNo=" + Data + "&lang="+ Data2; 
                    string[] DataTemps = DataDec.Split('&');
                    foreach (var item in DataTemps)
                    {
                        string[] DataTemp = item.Split('=');
                        if (DataTemp != null)
                        {
                            if (DataTemp[0].ToSafeString() == "Channel")
                            {
                                ViewBag.Channel = DataTemp[1].ToSafeString();
                                Channel = DataTemp[1].ToSafeString();
                                ViewBag.CheckChannel = DataTemp[1].ToSafeString();
                            }
                            if (DataTemp[0].ToSafeString() == "Staff_id")
                            {
                                ViewBag.Staff_id = DataTemp[1].ToSafeString();
                                EmployeeID = DataTemp[1].ToSafeString();
                            }
                            if (DataTemp[0].ToSafeString() == "Location_code")
                            {
                                ViewBag.Location_code = DataTemp[1].ToSafeString();
                                LcCode = DataTemp[1].ToSafeString();
                            }
                            if (DataTemp[0].ToSafeString() == "Install_date")
                            {
                                ViewBag.Install_date = DataTemp[1].ToSafeString();
                            }
                            if (DataTemp[0].ToSafeString() == "Time_slot")
                            {
                                ViewBag.Time_slot = DataTemp[1].ToSafeString();
                            }
                            if (DataTemp[0].ToSafeString() == "Internet_no" || DataTemp[0].ToSafeString() == "mobileNo")
                            {
                                NonMobileNo = DataTemp[1].ToSafeString();
                                //NonMobileNo = "8800022021";
                                ViewBag.NonMobileNo = NonMobileNo;
                                string checkNonMobileNo = NonMobileNo.Substring(0, 3);
                                var getLovForcheckMobileandFlag = GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);
                                var getLovCheckMobileNo = getLovForcheckMobileandFlag.FirstOrDefault(x => x.Field == "Existing_Check_Prefix_3BB").Validation;
                                var getLovFlagCheck3BB = getLovForcheckMobileandFlag.FirstOrDefault(x => x.Field == "Existing_Check_3BB_Flag").Validation;
                                if (getLovFlagCheck3BB == "Y")
                                {
                                    if (checkNonMobileNo == getLovCheckMobileNo)
                                    {
                                        if (Data == "")
                                            ViewBag.PageShow = "";
                                        ViewBag.FbbConstant = GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);
                                        ViewBag.TopUp = "1";
                                        ViewBag.LabelFBBTR000 = GetGeneralScreenConfig();
                                        ViewBag.LabelFBBTR001 = GetCoverageScreenConfig();
                                        ViewBag.LabelFBBOR015 = GetProfilePrePostPaid();
                                        ViewBag.LabelFBBTR010 = GetVas_Select_Package_ScreenConfig(); // 25.05.2023 deeflink
                                        ViewBag.MyAISBypass = "Y";
                                        ViewBag.Flag3BB = "3BB";
                                        ViewBag.PageShow = "LoginFail";
                                        return View("New_SearchProfile2");
                                    }
                                }
                            }
                            if (DataTemp[0].ToSafeString() == "lang")
                            {
                                lang = DataTemp[1].ToSafeString();
                                if (lang == "TH")
                                {
                                    ViewBag.LanguagePage = "1";
                                    SiteSession.CurrentUICulture = 1;
                                    Session["CurrentUICulture"] = 1;
                                }
                                else
                                {
                                    ViewBag.LanguagePage = "2";
                                    SiteSession.CurrentUICulture = 2;
                                    Session["CurrentUICulture"] = 2;
                                }
                            }
                            if (DataTemp[0].ToSafeString() == "timeStamp")
                            {
                                timeStamp = DataTemp[1].ToSafeString();
                            }
                        }
                        else
                        {
                            // value in put ไม่ถูกต้อง
                            CheckInput = false;
                            break;

                        }
                    }
                }
                if (CheckInput)
                {
                    GetIdCardStatus = GetInfoByNonMobileNo(NonMobileNo);
                    if (GetIdCardStatus == "" && !string.IsNullOrEmpty(IdCard))
                    {
                        ViewBag.IdCard = IdCard;
                        ViewBag.CardType = CardType;
                        //ViewBag.PageShow = "Mobile";

                        ViewBag.User = base.CurrentUser;
                        model = massCommonBypass(NonMobileNo, IdCard, CardType, "topupIPCamera", ViewBag.User, FullUrl);
                        if (Data == "")
                        {
                            model.CustomerRegisterPanelModel.L_STAFF_ID = EmployeeID;
                            model.CustomerRegisterPanelModel.L_LOC_CODE = LcCode;
                            model.CustomerRegisterPanelModel.L_ASC_CODE = ASCCode;
                            model.CustomerRegisterPanelModel.outSubType = outSubType;

                        }
                        model.CoveragePanelModel.P_MOBILE = NonMobileNo;
                        ViewBag.personalPromotion = personalPromotionMesh(NonMobileNo);

                        var query2 = new evOMQueryListServiceAndPromotionByPackageTypeQuery
                        {
                            mobileNo = NonMobileNo,//"8850001230",
                            idCard = IdCard,
                            FullUrl = FullUrl
                        };

                        var data = _queryProcessor.Execute(query2);
                        if (data.access_mode == "")
                        {
                            ViewBag.FbbConstant = GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);
                            ViewBag.TopUp = "1";
                            ViewBag.LabelFBBTR000 = GetGeneralScreenConfig();
                            ViewBag.LabelFBBTR001 = GetCoverageScreenConfig();
                            ViewBag.LabelFBBOR015 = GetProfilePrePostPaid();
                            ViewBag.LabelFBBTR010 = GetVas_Select_Package_ScreenConfig(); // 25.05.2023 deeflink
                            ViewBag.MyAISBypass = "Y";
                            ViewBag.FlagAccessType = "AccessTypeFail";
                            ViewBag.PageShow = "LoginFail";
                            return View("New_SearchProfile2");
                        }
                        model.v_owner_product = data.v_owner_product;
                        model.v_package_subtype = "IP_CAMERA";
                        var v_package_subtype = data.v_package_subtype;
                        model.v_sff_main_promotionCD = data.v_sff_main_promotionCD;
                        ViewBag.AddressID = data.addressId;
                        model.CoveragePanelModel.Address.AddressId = data.addressId;

                        model.CustomerRegisterPanelModel.v_installAddress = data.v_installAddress;
                        model.CustomerRegisterPanelModel.installAddress1 = data.v_installAddress1;
                        model.CustomerRegisterPanelModel.installAddress2 = data.v_installAddress2;
                        model.CustomerRegisterPanelModel.installAddress3 = data.v_installAddress3;
                        model.CustomerRegisterPanelModel.installAddress4 = data.v_installAddress4;
                        model.CustomerRegisterPanelModel.installAddress5 = data.v_installAddress5;
                        ViewBag.AccessModeByPass = data.access_mode;
                        //model.CustomerRegisterPanelModel.pbox_count = data.v_number_of_pb_number.ToString();
                        var controller = DependencyResolver.Current.GetService<SelectPackageController>();

                        JsonNetResult listPackageActionResult = controller.GetListPackagebySFFPromoV2(model.v_owner_product, model.v_package_subtype, "CallIn", model.CoveragePanelModel.P_MOBILE, model.v_sff_main_promotionCD, "IP_CAMERA", LcCode == "" || EmployeeID == "" ? "CUSTOMER" : "OFFICER") as JsonNetResult;
                        //JsonNetResult listPackageActionResult = GetListPackagebySFFPromoMockup();


                        model.v_package_subtype = data.v_package_subtype;
                        var tmpHTML = "";
                        if (listPackageActionResult != null)
                        {
                            JsonNetResult listPackageJsonNetResult = listPackageActionResult as JsonNetResult;
                            if (listPackageJsonNetResult != null && listPackageJsonNetResult.Data != null)
                            {
                                List<PackageModel> listPackageData = listPackageJsonNetResult.Data as List<PackageModel>;
                                List<PackageModel> listPackagetmp = listPackageData.Where(t => (t.PACKAGE_TYPE == "14" || t.PACKAGE_TYPE == "6") && t.PRODUCT_SUBTYPE == "IP_CAMERA").ToList();
                                //
                                if (listPackagetmp == null || listPackagetmp.Count == 0)
                                {
                                    // Login fail
                                    IdCard = "";
                                    CardType = "";
                                    ViewBag.PageShow = "LoginFail";
                                }
                                else
                                {
                                    for (int index = 0; index < listPackagetmp.Count(); index++)
                                    {
                                        tmpHTML += "<tr><td>";
                                        tmpHTML += "<input type='text' name='" + "SelectCamera" + "' value='" + index + "'>";
                                        tmpHTML += "<input type='text' id='MAPPING_CODE" + index + "' value='" + listPackagetmp[index].MAPPING_CODE + "'>";
                                        tmpHTML += "<input type='text' id='OWNER_PRODUCT" + index + "' value='" + listPackagetmp[index].OWNER_PRODUCT + "'>";
                                        tmpHTML += "<input type='text' id='PACKAGE_DISPLAY_ENG" + index + "' value='" + listPackagetmp[index].PACKAGE_DISPLAY_ENG + "'>";
                                        tmpHTML += "<input type='text' id='PACKAGE_DISPLAY_THA" + index + "' value='" + listPackagetmp[index].PACKAGE_DISPLAY_THA + "'>";
                                        tmpHTML += "<input type='text' id='PACKAGE_SERVICE_CODE" + index + "' value='" + listPackagetmp[index].PACKAGE_SERVICE_CODE + "'>";
                                        tmpHTML += "<input type='text' id='PACKAGE_SERVICE_NAME" + index + "' value='" + listPackagetmp[index].PACKAGE_SERVICE_NAME + "'>";
                                        tmpHTML += "<input type='text' id='PACKAGE_TYPE" + index + "' value='" + listPackagetmp[index].PACKAGE_TYPE + "'>";
                                        tmpHTML += "<input type='text' id='PACKAGE_TYPE_DESC" + index + "' value='" + listPackagetmp[index].PACKAGE_TYPE_DESC + "'>";
                                        tmpHTML += "<input type='text' id='PRE_PRICE_CHARGE" + index + "' value='" + listPackagetmp[index].PRE_PRICE_CHARGE + "'>";
                                        tmpHTML += "<input type='text' id='PRICE_CHARGE" + index + "' value='" + listPackagetmp[index].PRICE_CHARGE + "'>";
                                        tmpHTML += "<input type='text' id='PRODUCT_SUBTYPE" + index + "' value='" + listPackagetmp[index].PRODUCT_SUBTYPE + "'>";
                                        tmpHTML += "<input type='text' id='PRODUCT_SUBTYPE3" + index + "' value='" + listPackagetmp[index].PRODUCT_SUBTYPE3 + "'>";
                                        tmpHTML += "<input type='text' id='SFF_PRODUCT_NAME" + index + "' value='" + listPackagetmp[index].SFF_PRODUCT_NAME + "'>";
                                        tmpHTML += "<input type='text' id='SFF_PROMOTION_CODE" + index + "' value='" + listPackagetmp[index].SFF_PROMOTION_CODE + "'>";
                                        tmpHTML += "<input type='text' id='SFF_WORD_IN_STATEMENT_ENG" + index + "' value='" + listPackagetmp[index].SFF_WORD_IN_STATEMENT_ENG + "'>";
                                        tmpHTML += "<input type='text' id='SFF_WORD_IN_STATEMENT_THA" + index + "' value='" + listPackagetmp[index].SFF_WORD_IN_STATEMENT_THA + "'>";
                                        tmpHTML += "<input type='text' id='SERVICE_CODE" + index + "' value='" + listPackagetmp[index].SERVICE_CODE + "'>";
                                        tmpHTML += "<input type='text' id='DOWNLOAD_SPEED" + index + "' value='" + listPackagetmp[index].DOWNLOAD_SPEED + "'>";
                                        tmpHTML += "<input type='text' id='UPLOAD_SPEED" + index + "' value='" + listPackagetmp[index].UPLOAD_SPEED + "'>";
                                        tmpHTML += "<input type='text' id='AUTO_MAPPING_PROMOTION_CODE" + index + "' value='" + listPackagetmp[index].AUTO_MAPPING_PROMOTION_CODE + "'>";//R22.08
                                        tmpHTML += "<input type='text' id='PACKAGE_FOR_SALE_FLAG" + index + "' value='" + listPackagetmp[index].PACKAGE_FOR_SALE_FLAG + "'>";//R22.08
                                        tmpHTML += "</td></tr>";
                                    }
                                    if (tmpHTML != "")
                                    {
                                        ViewBag.tempV2ByPass = tmpHTML;
                                        ViewBag.PageShow = "";
                                    }
                                }

                            }
                        }
                        if (lang != "")
                        {
                            var IDCardName = base.LovData
                                .Where(l => l.Type.Equals(WebConstants.LovConfigName.CardType))
                                .Select(l => new DropdownModel
                                {
                                    Text = (lang == "TH" ? l.LovValue1 : l.LovValue2),
                                    Value = l.Name,
                                    DefaultValue = l.DefaultValue,
                                }).ToList().Find(x => x.Value == CardType);

                            model.CustomerRegisterPanelModel.L_CARD_TYPE = IDCardName.Text;
                        }
                        else
                        {
                            model.CustomerRegisterPanelModel.L_CARD_TYPE = model.IDCardType;
                        }

                        if (data.ListPromotion.Count > 0)
                        {

                            List<PackagePromotionModel> listPackagePromotion = new List<PackagePromotionModel>();

                            for (var index = 0; index < data.ListPromotion.Count; index++)
                            {

                                PackagePromotionModel pModel = new PackagePromotionModel();
                                pModel.ProductClass = data.ListPromotion[index].productClass;
                                pModel.ProductType = data.ListPromotion[index].productType;
                                pModel.ProductCd = data.ListPromotion[index].productCD;
                                pModel.EndDate = data.ListPromotion[index].endDate;

                                listPackagePromotion.Add(pModel);
                            }
                            model.PackagePromotionList = listPackagePromotion;
                        }

                        ViewBag.SFFProductName = model.outProductName;
                        ViewBag.SFFServiceYear = model.outDayOfServiceYear;
                        ViewBag.MobileAis = model.CoveragePanelModel.P_MOBILE;

                        //R24.01 Fix IP Camera AR order by Max kunlp885
                        /*var checkTech = new GetMeshCheckTechnologyQuery
                        {
                            addressID = data.addressId
                        };

                        var modelCheckTech = _queryProcessor.Execute(checkTech);

                        /// Update PRODUCT_SUBTYPE R.0920
                        if (modelCheckTech != null && modelCheckTech.PRODUCT_SUBTYPE.ToSafeString() != "")
                        {
                            model.v_package_subtype = modelCheckTech.PRODUCT_SUBTYPE;
                        }*/
                        //R24.01 Comment GetMeshCheckTechnologyQuery by Max kunlp885

                        var chkAROrder = new CheckAROrderQuery
                        {
                            BroadbandId = NonMobileNo,//"8800024585",
                            Option = "IP Camera",
                            FullUrl = ""
                        };

                        var dataAROrder = _queryProcessor.Execute(chkAROrder);

                        //if(modelCheckTech.RETURN_CODE != "0" ||
                        if ((dataAROrder.Count > 0 && dataAROrder
                            .Where(l => l.installFlag.Equals("N") || string.IsNullOrEmpty(l.installFlag))
                            .ToList().Count > 0))
                        {
                            ViewBag.PageShow = "LoginFail";
                            //if (modelCheckTech.RETURN_CODE != "0")
                            //    ViewBag.showErrorText = "Technology";
                            //else
                            ViewBag.showErrorText = "AROrderIPCamera";
                        }
                        //end R24.01 Fix IP Camera AR order by Max kunlp885

                        model.CoverageAreaResultModel.ADDRESS_ID = AddressID;

                    }
                    else
                    {
                        // Login fail
                        //logStep = "Not Found IDCard";
                        IdCard = "";
                        CardType = "";
                        ViewBag.PageShow = "LoginFail";
                    }
                }
                else
                {
                    // value in put ไม่ถูกต้อง
                    //logStep = "Missing Data";
                    ViewBag.LanguagePage = "1";
                    SiteSession.CurrentUICulture = 1;
                    Session["CurrentUICulture"] = 1;
                    ViewBag.PageShow = "LoginFail";
                }

                if (!AllowIPCamera())
                {
                    ViewBag.showErrorText = "ProhibitIPCamera";
                    IdCard = "";
                    CardType = "";
                    ViewBag.PageShow = "LoginFail";
                }

                if (ViewBag.PageShow == "LoginFail")
                {
                    if (Data == "")
                        ViewBag.PageShow = "";
                    ViewBag.TopUp = "1";
                    ViewBag.LabelFBBORV41 = GetTopUpMesh_ScreenConfig(); // R24.01 alert message checkAROrder
                    ViewBag.LabelFBBTR000 = GetGeneralScreenConfig();
                    ViewBag.LabelFBBTR001 = GetCoverageScreenConfig();
                    ViewBag.LabelFBBOR015 = GetProfilePrePostPaid();
                    ViewBag.LabelFBBTR010 = GetVas_Select_Package_ScreenConfig(); // 25.05.2023 deeflink
                    ViewBag.MyAISBypass = "Y";
                    return View("New_SearchProfile2");
                }

                else
                {
                    Session["PopupStatus"] = null;
                    ViewBag.Topup = "1";
                    ViewBag.LabelFBBORV41 = GetTopUpMesh_ScreenConfig(); // R24.01 alert message checkAROrder
                    ViewBag.LabelFBBTR000 = GetGeneralScreenConfig();
                    ViewBag.LabelFBBTR001 = GetCoverageScreenConfig();
                    ViewBag.LabelFBBTR002 = GetDisplayPackageScreenConfig();
                    ViewBag.LabelFBBTR003 = GetCustRegisterScreenConfig();
                    ViewBag.LabelFBBTR004 = GetSummaryScreenConfig();
                    ViewBag.LabelFBBTR010 = GetVas_Select_Package_ScreenConfig();
                    ViewBag.LabelFBBOR015 = GetProfilePrePostPaid();
                    ViewBag.LabelFBBORV11 = GetVasPopUpScreenConfig();
                    ViewBag.FbbException = GetFbbConstantModel(WebConstants.LovConfigName.FbbExceptionIdCard);
                    ViewBag.Fbb_Service = GetDisplay_Select_Type_Service();
                    ViewBag.Version = GetVersion();
                    ViewBag.Vas = "";
                    ViewBag.User = base.CurrentUser;
                    ViewBag.MeshConfig = GetMeshConfig_ScreenConfig();
                    ViewBag.LSAVE = LSAVE;
                    ViewBag.SWiFi = SWiFi;
                    ViewBag.LanguagePage = LanguagePage;
                    ViewBag.LPOPUPSAVE = LPOPUPSAVE;
                    ViewBag.LCLOSE = LCLOSE;
                    ViewBag.SaveSuccess = SaveSuccess;
                    ViewBag.FbbConstant = GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);
                    ViewBag.ExistingPopupFlag = ExistingPopupFlag;
                    ViewBag.MyAISBypass = "Y";
                    ViewBag.ExistingFlag = "IP_CAMERA";
                    model.TopUp = "1";
                    model.ExistingFlag = "IP_CAMERA";
                    return ProcessLine2(model);
                }
            }
            else
            {
                if (Session["EndProcessFlag"].ToSafeBoolean())
                {
                    Session["PopupStatus"] = "Success";
                    Session["EndProcessFlag"] = null;
                }
                else
                    Session["PopupStatus"] = null;
                ViewBag.Topup = "1";
                ViewBag.LabelFBBORV41 = GetTopUpMesh_ScreenConfig(); // R24.01 alert message checkAROrder
                ViewBag.LabelFBBTR000 = GetGeneralScreenConfig();
                ViewBag.LabelFBBTR001 = GetCoverageScreenConfig();
                ViewBag.LabelFBBTR002 = GetDisplayPackageScreenConfig();
                ViewBag.LabelFBBTR003 = GetCustRegisterScreenConfig();
                ViewBag.LabelFBBTR004 = GetSummaryScreenConfig();
                ViewBag.LabelFBBTR010 = GetVas_Select_Package_ScreenConfig();
                ViewBag.LabelFBBOR015 = GetProfilePrePostPaid();
                ViewBag.LabelFBBORV11 = GetVasPopUpScreenConfig();
                ViewBag.FbbException = GetFbbConstantModel(WebConstants.LovConfigName.FbbExceptionIdCard);
                ViewBag.Fbb_Service = GetDisplay_Select_Type_Service();
                ViewBag.Version = GetVersion();
                ViewBag.Vas = "";
                ViewBag.User = base.CurrentUser;
                ViewBag.MeshConfig = GetMeshConfig_ScreenConfig();
                ViewBag.LSAVE = LSAVE;
                ViewBag.SWiFi = SWiFi;
                ViewBag.LanguagePage = LanguagePage;
                ViewBag.LPOPUPSAVE = LPOPUPSAVE;
                ViewBag.LCLOSE = LCLOSE;
                ViewBag.SaveSuccess = SaveSuccess;
                ViewBag.FbbConstant = GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);
                ViewBag.ExistingFlag = ExistingPopupFlag == "Y" ? "IP_CAMERA:MENU" : "IP_CAMERA";
                ViewBag.ExistingPopupFlag = ExistingPopupFlag;
                if (!AllowIPCamera())
                {
                    ViewBag.showErrorText = "ProhibitIPCamera";
                    IdCard = "";
                    CardType = "";
                    ViewBag.PageShow = "LoginFail";
                }
                return View("New_SearchProfile2");
            }
        }

        public List<evESQueryPersonalInformationModel> evESQueryPersonalInformation(string mobileNo, string option)
        {
            // 17.6 Interface Log Add Url
            //Session["FullUrl"] = this.Url.Action("TopupMesh", "Process", null, this.Request.Url.Scheme); 23.06 IPCAMERA
            string FullUrl = "";
            if (Session["FullUrl"] != null)
                FullUrl = Session["FullUrl"].ToSafeString();

            List<evESQueryPersonalInformationModel> result = new List<evESQueryPersonalInformationModel>();
            try
            {
                var query = new evESQueryPersonalInformationQuery()
                {
                    mobileNo = mobileNo,
                    option = option,
                    FullUrl = FullUrl
                };
                result = _queryProcessor.Execute(query);
            }
            catch { }

            return result;
        }
        public JsonResult evESQueryPersonalInformationJson(string mobileNo, string option)
        {
            // 17.6 Interface Log Add Url
            Session["FullUrl"] = this.Url.Action("TopupMesh", "Process", null, this.Request.Url.Scheme);
            string FullUrl = "";
            if (Session["FullUrl"] != null)
                FullUrl = Session["FullUrl"].ToSafeString();

            List<evESQueryPersonalInformationModel> result = new List<evESQueryPersonalInformationModel>();
            try
            {
                var query = new evESQueryPersonalInformationQuery()
                {
                    mobileNo = mobileNo,
                    option = option,
                    FullUrl = FullUrl
                };
                result = _queryProcessor.Execute(query);
            }
            catch { }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public string evFBBGenerateFBBNo(string inMobileNo, string FullUrl)
        {
            string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress))
            {
                ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
            string resultData = "";
            evFBBGenerateFBBNoModel result = new evFBBGenerateFBBNoModel();
            evFBBGenerateFBBNoQuery query = new evFBBGenerateFBBNoQuery()
            {
                inMobileNo = inMobileNo,
                FullUrl = FullUrl,
                ClientIP = ipAddress
            };
            result = _queryProcessor.Execute(query);
            if (result != null)
            {
                resultData = result.FBBNo.ToSafeString();
            }

            return resultData;
        }

        protected string personalPromotionMesh(string NonMobileNo)
        {

            List<evESQueryPersonalInformationModel> PersonalInformation2 = evESQueryPersonalInformation(NonMobileNo, "2");
            StringBuilder htmlResult = new StringBuilder();
            foreach (var tmp in PersonalInformation2)
            {
                string grpdiv = "<div id={0}>" +
                    "<input type='hidden' name='productCd' value='{1}' />" +
                    "<input type='hidden' name='promotionName' value='{2}' />" +
                    "<input type='hidden' name='productClass' value='{3}' />" +
                    "<input type='hidden' name='productGroup' value='{4}' />" +
                    "<input type='hidden' name='productPkg' value='{5}' />" +
                    "<input type='hidden' name='descThai' value='{6}' />" +
                    "<input type='hidden' name='descEng' value='{7}' />" +
                    "<input type='hidden' name='inStatementThai' value='{8}' />" +
                    "<input type='hidden' name='inStatementEng' value='{9}' />" +
                    "<input type='hidden' name='startDt' value='{10}' />" +
                    "<input type='hidden' name='endDt' value='{11}' />" +
                    "<input type='hidden' name='productSeq' value='{12}' />" +
                    "</div>";

                htmlResult.Append(string.Format(grpdiv, tmp.productCd, tmp.productCd, tmp.promotionName, tmp.productClass, tmp.produuctGroup, tmp.productPkg, tmp.descThai, tmp.descEng, tmp.inStatementThai, tmp.inStatementEng, tmp.startDt, tmp.endDt, tmp.productSeq));
            }

            string personaInfo = htmlResult.ToString();

            return personaInfo;
        }

        protected QuickWinPanelModel massCommonBypass(string NonMobileNo, string IdCard, string CardType, string Page, string User, string FullUrl)
        {

            QuickWinPanelModel model = new QuickWinPanelModel();
            var query = new evESeServiceQueryMassCommonAccountInfoQuery
            {
                inOption = "2",
                inMobileNo = NonMobileNo,
                inCardNo = IdCard,
                inCardType = CardType,
                Page = Page,
                Username = User,
                FullUrl = FullUrl
            };
            var CustData = _queryProcessor.Execute(query);

            model.IDCardNo = IdCard;
            model.IDCardType = CardType;
            model.IDCardTypeENG = CardType;

            var controller = DependencyResolver.Current.GetService<MasterDataController>();
            List<DropdownModel> cardTypeData = controller.GetCustomerCardTypeDropdownModel("All");
            if (cardTypeData != null && cardTypeData.Count > 0)
            {
                model.IDCardType = cardTypeData.FirstOrDefault(t => t.Value == CardType).Text.ToSafeString();
            }

            model.CustomerRegisterPanelModel.L_CARD_NO = IdCard;

            model.CustomerRegisterPanelModel.CateType = CustData.cardType;
            model.outcardType = CustData.cardType;

            model.outPrimaryContactFirstName = CustData.outPrimaryContactFirstName;
            model.outContactLastName = CustData.outContactLastName;

            model.outHouseNumber = CustData.outHouseNumber;
            model.outMoo = CustData.outMoo;
            model.outMooban = CustData.outMooban;
            model.outBuildingName = CustData.outBuildingName;
            model.outFloor = CustData.outFloor;
            model.outRoom = CustData.outRoom;
            model.outSoi = CustData.outSoi;
            model.outStreetName = CustData.outStreetName;
            model.outtumbol = CustData.outTumbol;
            model.outAmphur = CustData.outAmphur;
            model.outProvince = CustData.outProvince;
            model.outPostalCode = CustData.outPostalCode;

            model.CustomerRegisterPanelModel.AddressPanelModelSetup.L_HOME_NUMBER_2 = CustData.outHouseNumber;
            model.CustomerRegisterPanelModel.AddressPanelModelSetup.L_MOO = CustData.outMoo;
            model.CustomerRegisterPanelModel.AddressPanelModelSetup.L_MOOBAN = CustData.outMooban;
            model.CustomerRegisterPanelModel.AddressPanelModelSetup.L_BUILD_NAME = CustData.outBuildingName;
            model.CustomerRegisterPanelModel.AddressPanelModelSetup.L_FLOOR = CustData.outFloor;
            model.CustomerRegisterPanelModel.AddressPanelModelSetup.L_ROOM = CustData.outRoom;
            model.CustomerRegisterPanelModel.AddressPanelModelSetup.L_SOI = CustData.outSoi;
            model.CustomerRegisterPanelModel.AddressPanelModelSetup.L_ROAD = CustData.outStreetName;
            model.CustomerRegisterPanelModel.AddressPanelModelSetup.L_TUMBOL = CustData.outTumbol;
            model.CustomerRegisterPanelModel.AddressPanelModelSetup.L_AMPHUR = CustData.outAmphur;
            model.CustomerRegisterPanelModel.AddressPanelModelSetup.L_PROVINCE = CustData.outProvince;
            model.CustomerRegisterPanelModel.AddressPanelModelSetup.L_ZIPCODE = CustData.outPostalCode;

            model.CustomerRegisterPanelModel.AddressPanelModelSendDoc.L_HOME_NUMBER_2 = CustData.outHouseNumber;
            model.CustomerRegisterPanelModel.AddressPanelModelSendDoc.L_MOO = CustData.outMoo;
            model.CustomerRegisterPanelModel.AddressPanelModelSendDoc.L_MOOBAN = CustData.outMooban;
            model.CustomerRegisterPanelModel.AddressPanelModelSendDoc.L_BUILD_NAME = CustData.outBuildingName;
            model.CustomerRegisterPanelModel.AddressPanelModelSendDoc.L_FLOOR = CustData.outFloor;
            model.CustomerRegisterPanelModel.AddressPanelModelSendDoc.L_ROOM = CustData.outRoom;
            model.CustomerRegisterPanelModel.AddressPanelModelSendDoc.L_SOI = CustData.outSoi;
            model.CustomerRegisterPanelModel.AddressPanelModelSendDoc.L_ROAD = CustData.outStreetName;
            model.CustomerRegisterPanelModel.AddressPanelModelSendDoc.L_TUMBOL = CustData.outTumbol;
            model.CustomerRegisterPanelModel.AddressPanelModelSendDoc.L_AMPHUR = CustData.outAmphur;
            model.CustomerRegisterPanelModel.AddressPanelModelSendDoc.L_PROVINCE = CustData.outProvince;
            model.CustomerRegisterPanelModel.AddressPanelModelSendDoc.L_ZIPCODE = CustData.outPostalCode;

            model.CustomerRegisterPanelModel.AddressPanelModelVat.L_HOME_NUMBER_2 = CustData.outHouseNumber;
            model.CustomerRegisterPanelModel.AddressPanelModelVat.L_MOO = CustData.outMoo;
            model.CustomerRegisterPanelModel.AddressPanelModelVat.L_MOOBAN = CustData.outMooban;
            model.CustomerRegisterPanelModel.AddressPanelModelVat.L_BUILD_NAME = CustData.outBuildingName;
            model.CustomerRegisterPanelModel.AddressPanelModelVat.L_FLOOR = CustData.outFloor;
            model.CustomerRegisterPanelModel.AddressPanelModelVat.L_ROOM = CustData.outRoom;
            model.CustomerRegisterPanelModel.AddressPanelModelVat.L_SOI = CustData.outSoi;
            model.CustomerRegisterPanelModel.AddressPanelModelVat.L_ROAD = CustData.outStreetName;
            model.CustomerRegisterPanelModel.AddressPanelModelVat.L_TUMBOL = CustData.outTumbol;
            model.CustomerRegisterPanelModel.AddressPanelModelVat.L_AMPHUR = CustData.outAmphur;
            model.CustomerRegisterPanelModel.AddressPanelModelVat.L_PROVINCE = CustData.outProvince;
            model.CustomerRegisterPanelModel.AddressPanelModelVat.L_ZIPCODE = CustData.outPostalCode;

            model.outBillLanguage = CustData.outBillLanguage;
            model.outBirthDate = CustData.outBirthDate;
            model.outEmail = CustData.outEmail;
            model.outparameter2 = CustData.outparameter2;
            model.outAccountName = CustData.outAccountName;
            model.outAccountNumber = CustData.outAccountNumber;
            model.outServiceAccountNumber = CustData.outServiceAccountNumber;
            model.outBillingAccountNumber = CustData.outBillingAccountNumber;
            model.outProductName = CustData.outProductName;
            model.outDayOfServiceYear = CustData.outDayOfServiceYear;
            model.outRegisteredDate = CustData.outRegisteredDate;
            model.outAccountSubCategory = CustData.outAccountSubCategory;

            model.CustomerRegisterPanelModel.outFullAddress = CustData.outFullAddress;
            model.outAccountCategory = CustData.outAccountCategory;

            model.CustomerRegisterPanelModel.L_FIRST_NAME = CustData.outPrimaryContactFirstName;
            model.CustomerRegisterPanelModel.L_LAST_NAME = CustData.outContactLastName;
            model.CustomerRegisterPanelModel.L_GOVERNMENT_NAME = CustData.outAccountName;
            model.CustomerRegisterPanelModel.L_CONTACT_PERSON = CustData.outPrimaryContactFirstName + " " + CustData.outContactLastName;
            model.CustomerRegisterPanelModel.SubCateType = CustData.outAccountSubCategory;
            model.CustomerRegisterPanelModel.L_CONTACT_PHONE = CustData.outparameter2;
            model.CustomerRegisterPanelModel.L_BIRTHDAY = CustData.outBirthDate;
            model.CustomerRegisterPanelModel.L_EMAIL = CustData.outEmail;

            model.CoveragePanelModel.CA_ID = CustData.outAccountNumber;
            model.CoveragePanelModel.SA_ID = CustData.outServiceAccountNumber;
            model.CoveragePanelModel.BA_ID = CustData.outBillingAccountNumber;
            model.CoveragePanelModel.SffProductName = CustData.outProductName;
            model.CoveragePanelModel.SffServiceYear = CustData.outDayOfServiceYear;

            model.CustomerRegisterPanelModel.L_TITLE = CustData.outTitle;
            model.SffProfileLogID = CustData.SffProfileLogID.GetValueOrDefault();

            model.CustomerRegisterPanelModel.vatAddress1 = CustData.vatAddress1;
            model.CustomerRegisterPanelModel.vatAddress2 = CustData.vatAddress2;
            model.CustomerRegisterPanelModel.vatAddress3 = CustData.vatAddress3;
            model.CustomerRegisterPanelModel.vatAddress4 = CustData.vatAddress4;
            model.CustomerRegisterPanelModel.vatAddress5 = CustData.vatAddress5;
            model.CustomerRegisterPanelModel.vatPostalCd = CustData.vatPostalCd;
            model.CustomerRegisterPanelModel.vatAddressFull = CustData.vatAddressFull;
            model.v_PackageCode = CustData.PackageCode;

            model.outServiceLevel = CustData.outServiceLevel;
            model.CustomerRegisterPanelModel.ServiceLevel = CustData.outServiceLevel;
            model.CoveragePanelModel.Address.AddressId = CustData.outAddressId;

            if (FullUrl.IndexOf("TopupMesh") != -1 || FullUrl.IndexOf("TopupIPCamera") != -1)
            {
                evOMCheckDeviceContractQuery checkDeviceContractQuery = new evOMCheckDeviceContractQuery()
                {
                    inCardNo = IdCard,
                    inCardType = CardType,
                    inMobileNo = NonMobileNo,
                    FullUrl = FullUrl
                };
                var checkDeviceContractData = _queryProcessor.Execute(checkDeviceContractQuery);
                if (checkDeviceContractData.errorMessage == "")
                {
                    model.CustomerRegisterPanelModel.CountContractFbb = checkDeviceContractData.countContractFbb;
                    model.CustomerRegisterPanelModel.ContractFlagFbb = checkDeviceContractData.contractFlagFbb;
                    model.CustomerRegisterPanelModel.FBBLimitContract = checkDeviceContractData.fbbLimitContract;
                    model.CustomerRegisterPanelModel.ContractProfileCountFbb = checkDeviceContractData.contractProfileCountFbb;
                }

            }

            return model;
        }

        public ActionResult TopupPlaybox(string PACKAGE_NAME = "", bool SaveSuccess = false, string LSAVE = "", string LCLOSE = "",
          string LPOPUPSAVE = "", string LanguagePage = "", string SWiFi = "", string DUP = "", string ExistingPopupFlag = "")
        {
            ViewBag.hasLoadingBlock = true;
            /*
            //R24.05 Add loading block screen by max kunlp885
            if (checkLoadingBlock("TopupPlaybox", ""))
            {
                Session["LoadingBlockPackageName"] = PACKAGE_NAME;
                Session["LoadingBlockSaveSuccess"] = SaveSuccess;
                Session["LoadingBlockLabelSave"] = LSAVE;
                Session["LoadingBlockLabelClose"] = LCLOSE;
                Session["LoadingBlockPopupSave"] = LPOPUPSAVE;
                Session["LoadingBlockLanguagePage"] = LanguagePage;
                Session["LoadingBlockSWiFi"] = SWiFi;
                Session["LoadingBlockDUP"] = DUP;
                Session["LoadingBlockExistingPopupFlag"] = ExistingPopupFlag;
                return View("ExistingFibreExtensions/_Loading");
            }
            PACKAGE_NAME = Session["LoadingBlockPackageName"].ToSafeString();
            SaveSuccess = (bool)Session["LoadingBlockSaveSuccess"];
            LSAVE = Session["LoadingBlockLabelSave"].ToSafeString();
            LCLOSE = Session["LoadingBlockLabelClose"].ToSafeString();
            LPOPUPSAVE = Session["LoadingBlockPopupSave"].ToSafeString();
            LanguagePage = Session["LoadingBlockLanguagePage"].ToSafeString();
            SWiFi = Session["LoadingBlockSWiFi"].ToSafeString();
            DUP = Session["LoadingBlockDUP"].ToSafeString();
            ExistingPopupFlag = Session["LoadingBlockExistingPopupFlag"].ToSafeString();
            //end R24.05 Add loading block screen by max kunlp885
            */

            Session["FullUrl"] = this.Url.Action("TopupPlaybox", "Process", null, this.Request.Url.Scheme);
            ViewBag.IsTopupPlaybox = "Y";
            ViewBag.Vas = "4";
            if (Session["EndProcessFlag"].ToSafeBoolean())
            {
                Session["PopupStatus"] = "Success";
                Session["EndProcessFlag"] = null;
            }
            else
                Session["PopupStatus"] = null;

            string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress))
            {
                ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
            SaveStatlog("CUSTOMER", "CUSTOMER", ipAddress, "FBB REGISTER ONTOP PLAYBOX", "", "");

            ViewBag.User = base.CurrentUser;

            ViewBag.LabelFBBTR000 = GetGeneralScreenConfig();
            ViewBag.LabelFBBTR001 = GetCoverageScreenConfig();
            ViewBag.LabelFBBTR002 = GetDisplayPackageScreenConfig();
            ViewBag.LabelFBBTR003 = GetCustRegisterScreenConfig();
            ViewBag.LabelFBBTR004 = GetSummaryScreenConfig();
            ViewBag.LabelFBBOR015 = GetProfilePrePostPaid();
            ViewBag.LabelFBBTR010 = GetVas_Select_Package_ScreenConfig();
            ViewBag.LabelFBBORV11 = GetVasPopUpScreenConfig();
            ViewBag.LabelFBBORV12 = GetTopUpPlaybox_ScreenConfig();
            ViewBag.FbbConstant = GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);
            ViewBag.FbbException = GetFbbConstantModel(WebConstants.LovConfigName.FbbExceptionIdCard);
            ViewBag.Fbb_Service = GetDisplay_Select_Type_Service();
            ViewBag.MeshConfig = GetMeshConfig_ScreenConfig();
            ViewBag.Version = GetVersion();

            ViewBag.NamePackageFail = PACKAGE_NAME;
            ViewBag.Vas = "";
            ViewBag.LSAVE = LSAVE;
            ViewBag.SWiFi = SWiFi;
            ViewBag.LanguagePage = LanguagePage;
            ViewBag.LPOPUPSAVE = LPOPUPSAVE;
            ViewBag.LCLOSE = LCLOSE;
            ViewBag.SaveSuccess = SaveSuccess;
            ViewBag.DUP = DUP;
            ViewBag.FbbConstant = GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);
            ViewBag.ExistingFlag = ExistingPopupFlag == "Y" ? "PLAYBOX:MENU" : "PLAYBOX";
            ViewBag.ExistingPopupFlag = ExistingPopupFlag;

            return View("New_SearchProfileTopupPlaybox");
        }

        [HttpPost]
        public ActionResult TopupPlaybox(string Data = "", string ExistingPopupFlag = "")
        {
            if (Session["PageLoadTopupPlaybox"] == null)
            {
                PageLoadOntopModel tmpModel = new PageLoadOntopModel();
                tmpModel.Data = Data;
                ViewBag.PageGo = "TopupPlaybox";
                Session["PageLoadTopupPlaybox"] = "HaveLoad";
                return View("PageLoadOntop", tmpModel);
            }
            else
            {
                Session["PageLoadTopupPlaybox"] = null;
            }

            ViewBag.hasLoadingBlock = true;
            //R24.05 Add loading block screen by max kunlp885
            //if (checkLoadingBlock("TopupPlaybox", Data))
            //{
            //    Session["LoadingBlockExistingPopupFlag"] = ExistingPopupFlag;
            //    return View("ExistingFibreExtensions/_Loading");
            //}
            //ExistingPopupFlag = Session["LoadingBlockExistingPopupFlag"].ToSafeString();
            //end R24.05 Add loading block screen by max kunlp885

            QuickWinPanelModel model = new QuickWinPanelModel();

            string FullUrl = "";
            if (Session["FullUrl"] != null)
                FullUrl = Session["FullUrl"].ToSafeString();
            Session["FullUrl"] = this.Url.Action("TopupInternet", "Process", null, this.Request.Url.Scheme);

            string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress))
            {
                ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }

            string NonMobileNo = "";
            string GetIdCardStatus = "";
            string lang = "";
            string timeStamp = "";
            string cardType = "";
            string cardNo = "";

            if (Data != "")
            {
                bool CheckInput = true;
                string DataDec = Decrypt(Data);
                string[] DataTemps = DataDec.Split('&');

                foreach (var item in DataTemps)
                {
                    string[] DataTemp = item.Split('=');
                    if (DataTemp != null)
                    {
                        if (DataTemp[0].ToSafeString() == "mobileNo")
                        {
                            NonMobileNo = DataTemp[1].ToSafeString();
                            ViewBag.NonMobileNo = NonMobileNo;
                            string checkNonMobileNo = NonMobileNo.Substring(0, 3);
                            var getLovForcheckMobileandFlag = GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);
                            var getLovCheckMobileNo = getLovForcheckMobileandFlag.FirstOrDefault(x => x.Field == "Existing_Check_Prefix_3BB").Validation;
                            var getLovFlagCheck3BB = getLovForcheckMobileandFlag.FirstOrDefault(x => x.Field == "Existing_Check_3BB_Flag").Validation;
                            if (getLovFlagCheck3BB == "Y")
                            {
                                if (checkNonMobileNo == getLovCheckMobileNo)
                                {
                                    if (Data == "")
                                        ViewBag.PageShow = "";
                                    ViewBag.FbbConstant = GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);
                                    ViewBag.TopUp = "1";
                                    ViewBag.LabelFBBTR000 = GetGeneralScreenConfig();
                                    ViewBag.LabelFBBTR001 = GetCoverageScreenConfig();
                                    ViewBag.LabelFBBOR015 = GetProfilePrePostPaid();
                                    ViewBag.LabelFBBTR010 = GetVas_Select_Package_ScreenConfig(); // 25.05.2023 deeflink
                                    ViewBag.MyAISBypass = "Y";
                                    ViewBag.Flag3BB = "3BB";
                                    ViewBag.PageShow = "LoginFail";
                                    return View("New_SearchProfileTopupPlaybox");
                                }
                            }
                        }
                        if (DataTemp[0].ToSafeString() == "lang")
                        {
                            lang = DataTemp[1].ToSafeString();
                            if (lang == "TH")
                            {
                                ViewBag.LanguagePage = "1";
                                SiteSession.CurrentUICulture = 1;
                                Session["CurrentUICulture"] = 1;
                            }
                            else
                            {
                                ViewBag.LanguagePage = "2";
                                SiteSession.CurrentUICulture = 2;
                                Session["CurrentUICulture"] = 2;
                            }
                        }
                        if (DataTemp[0].ToSafeString() == "cardType")
                        {
                            cardType = DataTemp[1].ToSafeString();
                        }
                        if (DataTemp[0].ToSafeString() == "cardNo")
                        {
                            cardNo = DataTemp[1].ToSafeString();
                        }
                    }
                    else
                    {
                        // value in put ไม่ถูกต้อง
                        CheckInput = false;
                        break;

                    }
                }

                if (cardType == "" || cardNo == "")
                {
                    CheckInput = false;
                }
                else
                {
                    List<FbbConstantModel> cardtypeDatas = GetFbbConstantModel("ID_CARD_TYPE");
                    if (cardtypeDatas != null && cardtypeDatas.Count > 0)
                    {
                        List<FbbConstantModel> cardtypeData = cardtypeDatas.Where(t => t.Field == cardType).ToList();
                        if (cardtypeData == null || cardtypeData.Count == 0)
                        {
                            CheckInput = false;
                        }
                    }
                }

                if (CheckInput)
                {
                    GetIdCardStatus = GetInfoByNonMobileNo(NonMobileNo);
                    if (GetIdCardStatus == "" && !string.IsNullOrEmpty(IdCard))
                    {
                        ViewBag.IdCard = IdCard;
                        ViewBag.CardType = CardType;
                        ViewBag.PageShow = "Mobile";

                        ViewBag.User = base.CurrentUser;

                        model = massCommonBypass(NonMobileNo, IdCard, CardType, "TopupPlaybox", ViewBag.User, FullUrl);
                        model.CoveragePanelModel.P_MOBILE = NonMobileNo;

                        var query2 = new evOMQueryListServiceAndPromotionByPackageTypeQuery
                        {
                            mobileNo = NonMobileNo,//"8850001230",
                            idCard = IdCard,
                            FullUrl = FullUrl
                        };

                        var data = _queryProcessor.Execute(query2);
                        if (data.access_mode == "")
                        {
                            ViewBag.FbbConstant = GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);
                            ViewBag.TopUp = "1";
                            ViewBag.LabelFBBTR000 = GetGeneralScreenConfig();
                            ViewBag.LabelFBBTR001 = GetCoverageScreenConfig();
                            ViewBag.LabelFBBOR015 = GetProfilePrePostPaid();
                            ViewBag.LabelFBBTR010 = GetVas_Select_Package_ScreenConfig(); // 25.05.2023 deeflink
                            ViewBag.MyAISBypass = "Y";
                            ViewBag.FlagAccessType = "AccessTypeFail";
                            ViewBag.PageShow = "LoginFail";
                            return View("New_SearchProfileTopupPlaybox");
                        }
                        model.v_owner_product = data.v_owner_product;
                        model.v_package_subtype = data.v_package_subtype;
                        model.v_sff_main_promotionCD = data.v_sff_main_promotionCD;

                        var IDCardName = base.LovData
                            .Where(l => l.Type.Equals(WebConstants.LovConfigName.CardType))
                            .Select(l => new DropdownModel
                            {
                                Text = (lang == "TH" ? l.LovValue1 : l.LovValue2),
                                Value = l.Name,
                                DefaultValue = l.DefaultValue,
                            }).ToList().Find(x => x.Value == CardType);

                        model.CustomerRegisterPanelModel.L_CARD_TYPE = IDCardName.Text;


                        if (data.ListPromotion.Count > 0)
                        {

                            List<PackagePromotionModel> listPackagePromotion = new List<PackagePromotionModel>();

                            for (var index = 0; index < data.ListPromotion.Count; index++)
                            {

                                PackagePromotionModel pModel = new PackagePromotionModel();
                                pModel.ProductClass = data.ListPromotion[index].productClass.ToSafeString();
                                pModel.ProductType = data.ListPromotion[index].productType.ToSafeString();
                                pModel.ProductCd = data.ListPromotion[index].productCD.ToSafeString();
                                pModel.EndDate = data.ListPromotion[index].endDate.ToSafeString();
                                pModel.StartDate = data.ListPromotion[index].startDate.ToSafeString();
                                pModel.ProductStatus = data.ListPromotion[index].productStatus.ToSafeString();
                                listPackagePromotion.Add(pModel);
                            }
                            model.PackagePromotionList = listPackagePromotion;
                        }


                        var selectPackageController = DependencyResolver.Current.GetService<SelectPackageController>();
                        ActionResult listPackageActionResult = selectPackageController.GetListPackagebySFFPromoV2(model.v_owner_product, model.v_package_subtype, "CallIn", model.CoveragePanelModel.P_MOBILE, model.v_sff_main_promotionCD, "PBOX_CONTENT", "CUSTOMER");
                        if (listPackageActionResult != null)
                        {
                            JsonNetResult listPackageJsonNetResult = listPackageActionResult as JsonNetResult;
                            if (listPackageJsonNetResult != null && listPackageJsonNetResult.Data != null)
                            {
                                List<PackageModel> listPackageData = listPackageJsonNetResult.Data as List<PackageModel>;
                                // 25.05.2023 deeflink
                                //List<PackageModel> listPackagetmp = listPackageData.Where(t => t.PACKAGE_TYPE == "8" && t.PRODUCT_SUBTYPE == "PBOX" && t.PACKAGE_SERVICE_NAME == "PBOX").ToList();
                                List<PackageModel> listPackagetmp = listPackageData.Where(t => t.PACKAGE_TYPE == "8" && t.PRODUCT_SUBTYPE == "PBOX" && t.PACKAGE_SERVICE_NAME == "INTERNET").ToList();
                                //
                                if (listPackagetmp == null || listPackagetmp.Count == 0)
                                {
                                    // 25.05.2023 deeflink
                                    ViewBag.FlagPB = "FailPlaybox";
                                    //
                                    // Login fail
                                    IdCard = "";
                                    CardType = "";
                                    ViewBag.PageShow = "LoginFail";
                                }
                            }
                        }


                        ViewBag.SFFProductName = model.outProductName;
                        ViewBag.SFFServiceYear = model.outDayOfServiceYear;
                        ViewBag.MobileAis = model.CoveragePanelModel.P_MOBILE;

                        if (data.v_number_of_pb_number == 0)
                        {
                            // 25.05.2023 deeflink
                            ViewBag.FlagPB = "FailPlaybox";
                            //
                            // Login fail
                            IdCard = "";
                            CardType = "";
                            ViewBag.PageShow = "LoginFail";
                        }
                    }
                    else
                    {
                        // Login fail
                        IdCard = "";
                        CardType = "";
                        ViewBag.PageShow = "LoginFail";
                    }
                }
                else
                {
                    // value in put ไม่ถูกต้อง
                    ViewBag.LanguagePage = "1";
                    SiteSession.CurrentUICulture = 1;
                    Session["CurrentUICulture"] = 1;
                    ViewBag.PageShow = "LoginFail";
                }
            }
            else
            {
                ViewBag.PageShow = "LoginFail";
            }

            string TransactionID = NonMobileNo + ipAddress;

            InterfaceLogCommand log = null;
            log = StartInterface("DataEncrypt: " + Data + "\r\n NonMobileNo: " + NonMobileNo + "\r\n Language: " + lang + "\r\n timeStamp: " + timeStamp, "/process/TopupInternetPromotion", TransactionID, "", "TopupInternetPOST");

            EndInterface("", log, TransactionID, "Success", "");

            ViewBag.Vas = "4";
            model.SummaryPanelModel.VAS_FLAG = "4";
            ViewBag.MobileFromBypass = NonMobileNo;

            if (Session["EndProcessFlag"].ToSafeBoolean())
            {
                Session["PopupStatus"] = "Success";
                Session["EndProcessFlag"] = null;
            }
            else
                Session["PopupStatus"] = null;

            ViewBag.User = base.CurrentUser;

            ViewBag.LabelFBBTR000 = GetGeneralScreenConfig();
            ViewBag.LabelFBBTR001 = GetCoverageScreenConfig();
            ViewBag.LabelFBBTR002 = GetDisplayPackageScreenConfig();
            ViewBag.LabelFBBTR003 = GetCustRegisterScreenConfig();
            ViewBag.LabelFBBTR004 = GetSummaryScreenConfig();
            ViewBag.LabelFBBTR010 = GetVas_Select_Package_ScreenConfig();
            ViewBag.LabelFBBORV11 = GetVasPopUpScreenConfig();
            ViewBag.LabelFBBORV12 = GetTopUpPlaybox_ScreenConfig();
            ViewBag.LabelFBBORV24 = GetTopUpInternet_ScreenConfig();

            ViewBag.FbbConstant = GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);
            ViewBag.FbbException = GetFbbConstantModel(WebConstants.LovConfigName.FbbExceptionIdCard);
            ViewBag.Fbb_Service = GetDisplay_Select_Type_Service();

            ViewBag.MeshConfig = GetMeshConfig_ScreenConfig();

            ViewBag.Version = GetVersion();
            model.ClientIP = ipAddress;

            ViewBag.FbbConstant = GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);
            ViewBag.ContentPlaybox = GetLovConfigBytype("NEW_CONTENT_PLAYBOX");
            ViewBag.ExistingFlag = "PLAYBOX";
            ViewBag.MyAISBypass = "Y";

            if (ViewBag.PageShow == "LoginFail")
            {
                ViewBag.LabelFBBOR015 = GetProfilePrePostPaid();
                return View("New_SearchProfileTopupPlaybox");
            }
            else
            {
                ViewBag.IsSetServiceLevel = false;

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
                            ViewBag.LanguagePage = "1";
                        }
                    }
                    else
                    {
                        if (data.LovValue2 != null)
                        {
                            ViewBag.LCLOSE = data.LovValue2.ToString();
                            ViewBag.LanguagePage = "2";
                        }
                    }
                }
                else
                {
                    ViewBag.LCLOSE = "";
                }
                return View("Index", model);
            }
        }

        public ActionResult TriplePlay(bool SaveSuccess = false, string LSAVE = "", string LCLOSE = "",
            string LPOPUPSAVE = "", string LanguagePage = "", string SWiFi = "")
        {
            if (Session["EndProcessFlag"].ToSafeBoolean())
            {
                Session["PopupStatus"] = "Success";
                Session["EndProcessFlag"] = null;
            }
            else
                Session["PopupStatus"] = null;

            string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress))
            {
                ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
            SaveStatlog("CUSTOMER", "CUSTOMER", ipAddress, "FBB REGISTER TriplePlay", "", "");

            ViewBag.User = base.CurrentUser;

            ViewBag.LabelFBBTR000 = GetGeneralScreenConfig();
            ViewBag.LabelFBBTR001 = GetCoverageScreenConfig();
            ViewBag.LabelFBBTR002 = GetDisplayPackageScreenConfig();
            ViewBag.LabelFBBTR003 = GetCustRegisterScreenConfig();
            ViewBag.LabelFBBTR004 = GetSummaryScreenConfig();
            ViewBag.LabelFBBTR010 = GetVas_Select_Package_ScreenConfig();
            ViewBag.LabelFBBORV11 = GetVasPopUpScreenConfig();
            ViewBag.FbbConstant = GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);
            ViewBag.FbbException = GetFbbConstantModel(WebConstants.LovConfigName.FbbExceptionIdCard);
            ViewBag.Fbb_Service = GetDisplay_Select_Type_Service();
            ViewBag.MeshConfig = GetMeshConfig_ScreenConfig();
            ViewBag.Version = GetVersion();

            ViewBag.Vas = "";
            ViewBag.LSAVE = LSAVE;
            ViewBag.SWiFi = SWiFi;
            ViewBag.LanguagePage = LanguagePage;
            ViewBag.LPOPUPSAVE = LPOPUPSAVE;
            ViewBag.LCLOSE = LCLOSE;
            ViewBag.SaveSuccess = SaveSuccess;
            ViewBag.FbbConstant = GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);

            return View("_SearchProfileTriple");
        }

        public JsonResult usedData_flag(string status)
        {
            Session["usedData_flag"] = "";
            if (status != "")
            {
                Session["usedData_flag"] = status;
            }
            else
            {
                Session["usedData_flag"] = "";
            }
            return Json(new { result = Session["usedData_flag"], }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ValidateCaptcha(string inputUser, string inputHash)
        {
            //var hash = 5381;
            //inputUser = inputUser.ToUpper().Trim() + "978692";
            //for(int i = 0; i < inputUser.Length; i++) {
            //    hash = ((hash << 5) + hash) + inputUser[i];
            //}
            string result = CreateToken(inputUser, "978692");

            if (result == inputHash)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }

        struct MyObj
        {
            public string success { get; set; }
            public string score { get; set; }
            public string serverSaveImgTime { get; set; }
            public string serverProcessinTime { get; set; }
            public string serverResponseTime { get; set; }
            public string errorMessage { get; set; }
            public string isMatched { get; set; }
            public string buildNumber { get; set; }
        }


        [HttpPost]
        public JsonResult compareFace(string cardBase64Imgs = "", string selfieBase64Imgs = "", string AisAirNumber = "")
        {

            string ClientIP = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ClientIP))
            {
                ClientIP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }

            string transactionId = AisAirNumber + ClientIP;
            Session["logForUpdateImage"] = null;
            InterfaceLogCommand log = null;
            log = StartInterface("", "compareFace", transactionId, "", "WEB");

            Session["logForUpdateImage"] = log;

            var result = new MyObj();

            string URL = base.LovData.Where(l => l.Type == "SCREEN" && l.Name == "URL_COMPAREFACE").FirstOrDefault().LovValue1;
            //string serverCert = base.LovData.Where(l => l.Type == "SCREEN" && l.Name == "TDM_SERVER_CERT").FirstOrDefault().LovValue1;
            //string URL = "https://10.104.240.135:8543/TDMWSRestful/resteasy/stockReport/queryStockTDM";
            try
            {
                //if (serverCert == "Y")
                //ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
                //ServicePointManager.ServerCertificateValidationCallback = TrustCertificate;
                //ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
                request.ContentType = "application/json";
                request.Method = "POST";

                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    string json = new JavaScriptSerializer().Serialize(new
                    {
                        cardBase64Imgs = cardBase64Imgs,
                        selfieBase64Imgs = selfieBase64Imgs

                    });

                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();
                }


                var httpResponse = (HttpWebResponse)request.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var resultJson = streamReader.ReadToEnd();
                    result = new JavaScriptSerializer().Deserialize<MyObj>(resultJson);

                    EndInterface(resultJson, log, transactionId, "Success", "");

                }

            }
            catch (Exception ex)
            {
                EndInterface("", log, transactionId, "ERROR", "ErrorMessage: " + ex.GetErrorMessage() + "\r\n" + "RenderExceptionMessage: " + ex.RenderExceptionMessage());

            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        public ActionResult ZTEResQuery(string ACCESS_MODE_LIST, string RESOURCE_LIST, string PHONE_FLAGE, string TRANSACTION_ID = "", string ADDRESS_ID = "")
        {
            string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress)) ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            if (TRANSACTION_ID == "SCPE")
            {
                TRANSACTION_ID = "|" + ipAddress;
            }
            else
            {
                TRANSACTION_ID = TRANSACTION_ID + ipAddress;
            }

            if (RESOURCE_LIST != null && RESOURCE_LIST != "")
            {
                JavaScriptSerializer json_serializer = new JavaScriptSerializer();
                var routes_list = (object[])json_serializer.DeserializeObject(RESOURCE_LIST);

                List<DSLAM_INFO> RESULTDSLAM_LIST;

                RESULTDSLAM_LIST = null;
                if (routes_list != null)
                {
                    RESULTDSLAM_LIST = new List<DSLAM_INFO>();
                    foreach (Dictionary<string, object> item in routes_list)
                    {
                        if (item.ContainsKey("ResourceName"))
                        {
                            DSLAM_INFO resource_info = new DSLAM_INFO();
                            resource_info.Dslam_Name = (string)item["ResourceName"];
                            RESULTDSLAM_LIST.Add(resource_info);
                        }
                    }
                }

                var queryResQuery = new ZTEResQueryQuery
                {
                    PHONE_FLAGE = PHONE_FLAGE,
                    PRODUCT = ACCESS_MODE_LIST,
                    LISTOFDSLAM = RESULTDSLAM_LIST.ToArray(),
                    TRANSACTION_ID = TRANSACTION_ID,
                    ADDRESS_ID = ADDRESS_ID
                };
                ZTEResQueryModel resultResQuery = _queryProcessor.Execute(queryResQuery);

                string RETURN_CASECODE = "";
                string CasecodeMessage = "";
                string Casecode = "3";
                string ShowTimeSlot = "N";
                string FlowFlag = "";

                RETURN_CASECODE = resultResQuery.RETURN_CASECODE;

                var CasecodeData = base.LovData.Where(l => l.Name == "ResQuery" && l.LovValue1 == RETURN_CASECODE).Select(t => t.LovValue2);
                var CasecodeMessageData = base.LovData.Where(l => l.Name == "L_PORT_DSLAM_FULL").Select(t => new { t.LovValue1, t.LovValue2 });

                if (CasecodeData != null && CasecodeData.Count() > 0)
                {
                    Casecode = CasecodeData.FirstOrDefault().ToString();
                }

                if (CasecodeMessageData != null && CasecodeMessageData.Count() > 0)
                {
                    if (SiteSession.CurrentUICulture == 1)
                    {
                        CasecodeMessage = CasecodeMessageData.FirstOrDefault().LovValue1.ToString();
                    }
                    else
                    {
                        CasecodeMessage = CasecodeMessageData.FirstOrDefault().LovValue2.ToString();
                    }
                }

                if (Casecode == "1" || Casecode == "4")
                {
                    if (PHONE_FLAGE == "Y")
                    {
                        ShowTimeSlot = "Y";
                        CasecodeMessage = "";
                    }
                    else
                    {
                        ShowTimeSlot = "Y";
                        CasecodeMessage = "";
                    }
                    FlowFlag = "0";
                }

                if (Casecode == "3")
                {
                    ShowTimeSlot = "N";
                }

                if (Casecode == "5")
                {
                    if (PHONE_FLAGE == "Y")
                    {
                        ShowTimeSlot = "N";
                    }
                    else
                    {
                        ShowTimeSlot = "N";
                    }
                }

                if (Casecode == "6" || Casecode == "7")
                {
                    if (PHONE_FLAGE == "Y")
                    {
                        Casecode = "6";
                        ShowTimeSlot = "N";
                    }
                    else
                    {
                        Casecode = "7";
                        ShowTimeSlot = "Y";
                        CasecodeMessage = "";
                    }
                }



                return Json(new { Casecode = Casecode, CasecodeMessage = CasecodeMessage, ShowTimeSlot = ShowTimeSlot, FlowFlag = FlowFlag }, JsonRequestBehavior.AllowGet);
            }
            else
            {

                return Json(new { Casecode = "", CasecodeMessage = "", ShowTimeSlot = "N", FlowFlag = "" }, JsonRequestBehavior.AllowGet);
            }

        }

        private static String GetTimestamp(DateTime value)
        {
            return value.ToString("yyyyMMddHHmmssffff", new CultureInfo("en-US"));
        }

        private ConfirmChangePromotionModelLine4 ConfirmChangePromotionModelLine4(string mobileNo, string promotionCd, string promotionCdOldContent
            , string locationCd, string ascCode, string employeeID, string employeeName)
        {
            var flag1 = base.LovData.Where(l => l.Type == "FBB_CONSTANT" && l.Name == "CONFIRM_PROMOTION" && l.Text == "evOMServiceConfirmChangePromotion").SingleOrDefault();
            var flag2 = base.LovData.Where(l => l.Type == "FBB_CONSTANT" && l.Name == "CONFIRM_PROMOTION" && l.Text == "evOMCreateOrderChangePromotion").SingleOrDefault();

            var query = new GetConfirmChangePromotionQuery()
            {
                mobileNo = mobileNo,
                promotionCode = promotionCd,
                FlagCallService_evOMServiceConfirmChangePromotion = flag1.LovValue1,
                FlagCallService_evOMCreateOrderChangePromotion = flag2.LovValue1,
                promotionCdOldContent = promotionCdOldContent,
                locationCd = locationCd,
                ascCode = ascCode,
                employeeID = employeeID,
                employeeName = employeeName
            };

            var result = _queryProcessor.Execute(query);
            return result;
        }

        private string CreateToken(string message, string secret)
        {
            secret = secret ?? "";
            var encoding = new System.Text.ASCIIEncoding();
            byte[] keyByte = encoding.GetBytes(secret);
            byte[] messageBytes = encoding.GetBytes(message);
            using (var hmacsha256 = new HMACSHA256(keyByte))
            {
                byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
                return Convert.ToBase64String(hashmessage);
            }
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

        private void InterfaceUpdateImage(InterfaceLogCommand dbIntfCmd, string imagepath)
        {
            if (null == dbIntfCmd)
                return;

            dbIntfCmd.ActionType = WBBContract.Commands.ActionType.UpdateImage;
            dbIntfCmd.IMAGE_PATH = imagepath;

            _intfLogCommand.Handle(dbIntfCmd);
        }

        private CheckChangePromotionModelLine4 GetCheckChangePromotionLine4(string mobileNo, string promotionType, string promotionCd, string orderChannel)
        {
            var query = new GetCheckChangePromotionQuery()
            {
                mobileNo = mobileNo,
                promotionType = promotionType,
                promotionCd = promotionCd,
                orderChannel = orderChannel
            };

            var result = _queryProcessor.Execute(query);
            return result;
        }

        public List<FbbConstantModel> GetFbbConstantModel(string fbbConstType)
        {
            var data = base.LovData
               .Where(l => l.Type.Equals(fbbConstType))
               .Select(l => new FbbConstantModel
               {
                   Field = l.Name,
                   DefaultValue = l.DefaultValue,
                   Validation = l.LovValue1,
                   SubValidation = l.LovValue2
               }).ToList();

            return data;
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

        public string GetLovCampaignProjectNameMGM(string lovName)
        {
            //R22.06 Member get Member Send SMS Case Out of Coverage
            var LovCampaignProjectNameMGM = base.LovData
                .Where(l => (l.Type == "FBB_CONSTANT" && l.Name == "CAMPAIGN_PROJECT_NAME" && l.Text == "MGM"))
                .Select(s => s.LovValue2)
                .FirstOrDefault();
            return LovCampaignProjectNameMGM;
        }

        private SaveOrderResp GetSaveOrderResp(QuickWinPanelModel model)
        {
            string FullUrl = "";
            if (Session["FullUrl"] != null)
                FullUrl = Session["FullUrl"].ToSafeString();

            var query = new GetSaveOrderRespQuery
            {
                CurrentCulture = SiteSession.CurrentUICulture,
                QuickWinPanelModel = model,
                FullUrl = FullUrl
            };
            SaveOrderResp data = _queryProcessor.Execute(query);
            return data;
        }

        private GetIMCaseFBBRestServiceModel GetSaveIMTopupReplace(SummaryPanelModel model)
        {
            string FullUrl = Session["FullUrl"] != null ? Session["FullUrl"].ToSafeString() : "";

            #region Get IP Address Interface Log

            // Get IP Address
            string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress)) ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            string transactionId = model.DetailCalliMPBReplace.FIBRENET_ID + ipAddress;

            #endregion

            DetailCalliMTopupReplaceModel dataSendIM = new DetailCalliMTopupReplaceModel();
            GetIMCaseFBBRestServiceModel reaultDataIM = new GetIMCaseFBBRestServiceModel();

            if (model.DetailCalliMPBReplace != null && model.DetailCalliMPBReplace.RESERVED_ID != "")
            {
                //Step1. Get Detail Data Send IM (WBB.PKG_FBBOR050.DETAIL_CALL_IM)
                var query = new GetDetailCalliMTopupReplaceQuery
                {
                    P_FIBRENET_ID = model.DetailCalliMPBReplace.FIBRENET_ID,
                    P_CONTRACT_NO = model.DetailCalliMPBReplace.CONTRACT_NO,
                    P_CUSTOMER_NAME = model.DetailCalliMPBReplace.CUSTOMER_NAME,
                    P_SERIAL_NO = model.DetailCalliMPBReplace.SERIAL_NO,
                    P_RESERVED_ID = model.DetailCalliMPBReplace.RESERVED_ID,
                    P_TIME_SLOT = model.DetailCalliMPBReplace.TIME_SLOT,
                    P_DATE_TIME_SLOT = model.DetailCalliMPBReplace.DATE_TIME_SLOT,
                    P_ACCESS_MODE = model.DetailCalliMPBReplace.ACCESS_MODE,
                    P_ADDRESS_ID = model.DetailCalliMPBReplace.ADDRESS_ID,
                    P_COUNT_PB = model.DetailCalliMPBReplace.COUNT_PB,
                    TransactionId = transactionId,
                    FullUrl = FullUrl
                };

                dataSendIM = _queryProcessor.Execute(query);

                if (dataSendIM != null && dataSendIM.RETURN_PRICE_CURROR != null && dataSendIM.RETURN_PRICE_CURROR.Count() > 0)
                {
                    //Step2. Call Service IM
                    var query2 = new GetIMCaseFBBRestServiceQuery
                    {
                        Body = dataSendIM.RETURN_PRICE_CURROR.FirstOrDefault(),
                        TransactionId = transactionId,
                        FullUrl = FullUrl
                    };
                    reaultDataIM = _queryProcessor.Execute(query2);
                }
            }

            return reaultDataIM;
        }

        public string GetVersion()
        {
            string version = "";

            var query = new WBBContract.Queries.Commons.Master.GetVersionQuery
            {
            };

            var versionModel = _queryProcessor.Execute(query);

            version = versionModel.InternalServiceVersion;

            return version;
        }

        [HttpPost]
        public ActionResult UploadImage(string cateType, string cardNo, string cardType, string register_dv, string AisAirNumber)
        {

            ViewBag.hasLoadingBlock = true;//R24.04 Add loading block screen by max kunlp885
            #region Get IP Address Interface Log (Update 17.2)
            // Get IP Address
            string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress)) ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            #endregion

            string browserCapa = "";
            var bc = Request.Browser;
            browserCapa = "Browser Capabilities:\r\n";
            browserCapa = browserCapa + "Type = " + bc.Type + "\r\n";
            browserCapa = browserCapa + "Name = " + bc.Browser + "\r\n";
            browserCapa = browserCapa + "Version = " + bc.Version + "\r\n";
            browserCapa = browserCapa + "Major Version = " + bc.MajorVersion + "\r\n";
            browserCapa = browserCapa + "Minor Version = " + bc.MinorVersion + "\r\n";
            browserCapa = browserCapa + "Platform = " + bc.Platform + "\r\n";
            browserCapa = browserCapa + "Is Beta = " + bc.Beta + "\r\n";
            browserCapa = browserCapa + "Is Crawler = " + bc.Crawler + "\r\n";
            browserCapa = browserCapa + "Is AOL = " + bc.AOL + "\r\n";
            browserCapa = browserCapa + "Is Win16 = " + bc.Win16 + "\r\n";
            browserCapa = browserCapa + "Is Win32 = " + bc.Win32 + "\r\n";
            browserCapa = browserCapa + "Supports Frames = " + bc.Frames + "\r\n";
            browserCapa = browserCapa + "Supports Tables = " + bc.Tables + "\r\n";
            browserCapa = browserCapa + "Supports Cookies = " + bc.Cookies + "\r\n";
            browserCapa = browserCapa + "Is Mobile = " + bc.IsMobileDevice + "\r\n";
            browserCapa = browserCapa + "MobileDeviceManufacturer = " + bc.MobileDeviceManufacturer + "\r\n";
            browserCapa = browserCapa + "MobileDeviceModel = " + bc.MobileDeviceModel + "\r\n";
            browserCapa = browserCapa + "Device From JS = " + register_dv + "\r\n";

            string transactionId = (AisAirNumber + ipAddress).ToSafeString();
            InterfaceLogCommand log = null;
            log = StartInterface("IdcardNo:" + cardNo + "\r\n" + browserCapa, "UploadImage", transactionId, cardNo, "WEB");

            var listOfBase64Photo = Session["base64photo"] as Dictionary<string, string>;

            if (Request.Files.Count > 0 || (listOfBase64Photo != null && listOfBase64Photo.Any()))
            {
                try
                {


                    List<string> Arr_files = new List<string>();
                    QuickWinPanelModel model = new QuickWinPanelModel();
                    HttpPostedFileBase[] filesPosted;

                    if (Request.Files.Count > 0)
                    {
                        HttpFileCollectionBase files = Request.Files;
                        filesPosted = new HttpPostedFileBase[files.Count];
                        for (int i = 0; i < files.Count; i++)
                        {
                            filesPosted[i] = files[i];
                        }
                    }
                    else
                    {
                        //TODO: get image from card reader
                        filesPosted = new HttpPostedFileBase[listOfBase64Photo.Count()];
                        var ikey = 0;
                        foreach (var item in listOfBase64Photo)
                        {
                            byte[] byteArray = Convert.FromBase64String(item.Value);
                            HttpPostedFile postfile = ConstructHttpPostedFile(byteArray, item.Key + ".jpg", "image/jpeg");
                            filesPosted[ikey] = new HttpPostedFileWrapper(postfile);
                            ikey = ikey + 1;
                        }

                    }

                    model.Register_device = register_dv;
                    model.CustomerRegisterPanelModel.CateType = cateType;
                    model.CustomerRegisterPanelModel.L_CARD_NO = cardNo;
                    model.CustomerRegisterPanelModel.L_CARD_TYPE = cardType;

                    model.CoveragePanelModel.L_CONTACT_PHONE = AisAirNumber;
                    model.ClientIP = ipAddress;

                    filesPostedRegisterTempStep = filesPosted;
                    model = SaveFileImage(filesPosted, model);

                    if (Session["logForUpdateImage"] != null)
                    {
                        InterfaceUpdateImage(((InterfaceLogCommand)Session["logForUpdateImage"]), model.CustomerRegisterPanelModel.ListImageFile[0].FileName);
                        Session["logForUpdateImage"] = null;
                    }

                    EndInterface("", log, transactionId, "Success", "");
                    if (model.CustomerRegisterPanelModel.ListImageFile.Any())
                        return Json(model.CustomerRegisterPanelModel.ListImageFile);
                    else
                        throw new Exception("Null ListImageFile");
                }
                catch (Exception ex)
                {
                    EndInterface("", log, transactionId, "ERROR", "Error Message: " + ex.GetErrorMessage() + "\r\nStack Trace: " + ex.RenderExceptionMessage());


                    Logger.Info("Error Upload Image:" + ex.GetErrorMessage());
                    Logger.Info("Error Upload Image With Stack Trace : " + ex.RenderExceptionMessage());
                    return Json(false);
                }
            }
            else
            {
                return Json(false);
            }
        }

        public HttpPostedFile ConstructHttpPostedFile(byte[] data, string filename, string contentType)
        {
            // Get the System.Web assembly reference
            Assembly systemWebAssembly = typeof(HttpPostedFileBase).Assembly;
            // Get the types of the two internal types we need
            Type typeHttpRawUploadedContent = systemWebAssembly.GetType("System.Web.HttpRawUploadedContent");
            Type typeHttpInputStream = systemWebAssembly.GetType("System.Web.HttpInputStream");

            // Prepare the signatures of the constructors we want.
            Type[] uploadedParams = { typeof(int), typeof(int) };
            Type[] streamParams = { typeHttpRawUploadedContent, typeof(int), typeof(int) };
            Type[] parameters = { typeof(string), typeof(string), typeHttpInputStream };

            // Create an HttpRawUploadedContent instance
            object uploadedContent = typeHttpRawUploadedContent
              .GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, uploadedParams, null)
              .Invoke(new object[] { data.Length, data.Length });

            // Call the AddBytes method
            typeHttpRawUploadedContent
              .GetMethod("AddBytes", BindingFlags.NonPublic | BindingFlags.Instance)
              .Invoke(uploadedContent, new object[] { data, 0, data.Length });

            // This is necessary if you will be using the returned content (ie to Save)
            typeHttpRawUploadedContent
              .GetMethod("DoneAddingBytes", BindingFlags.NonPublic | BindingFlags.Instance)
              .Invoke(uploadedContent, null);

            // Create an HttpInputStream instance
            object stream = (Stream)typeHttpInputStream
              .GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, streamParams, null)
              .Invoke(new object[] { uploadedContent, 0, data.Length });

            // Create an HttpPostedFile instance
            HttpPostedFile postedFile = (HttpPostedFile)typeof(HttpPostedFile)
              .GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, parameters, null)
              .Invoke(new object[] { filename, contentType, stream });

            return postedFile;
        }

        private string RegisterCustomer(QuickWinPanelModel model, string interfaceCode,
            string interfaceDesc, string interfaceOrder, string ClientIP)
        {
            var coverageResultId = "";
            if (null != model.CoveragePanelModel)
                coverageResultId = model.CoveragePanelModel.RESULT_ID;

            if (model.CustomerRegisterPanelModel.FlagTDM == "Y")
            {
                // call WS TDM
                string AisAirNumber = model.CoveragePanelModel.P_MOBILE.ToSafeString();
                string resultCode_tmp = "";
                string contractId_tmp = "";
                string contractName_tmp = "";
                string contractType_tmp = "";
                string contractRuleId_tmp = "";
                string penaltyType_tmp = "";
                string penaltyId_tmp = "";
                string limitContract_tmp = "";
                string countFlg_tmp = "";
                string duration_tmp = "";

                duration_tmp = model.CustomerRegisterPanelModel.Duration.ToSafeString();

                GetListQueryConfigContractQuery queryWS = new GetListQueryConfigContractQuery()
                {
                    TransactionId = AisAirNumber,
                    contract_id = model.CustomerRegisterPanelModel.ContractID.ToSafeString(),
                    contract_name = model.CustomerRegisterPanelModel.ContractName.ToSafeString()
                };
                GetListQueryConfigContractModel resultWS = _queryProcessor.Execute(queryWS);
                if (resultWS != null)
                {
                    resultCode_tmp = resultWS.resultCode;
                    if (resultWS.resultCode == "20000" || resultWS.resultCode == "50000")
                    {
                        if (resultWS.listConfigurationContract != null)
                        {
                            contractId_tmp = resultWS.listConfigurationContract[0].contractId;
                            contractName_tmp = resultWS.listConfigurationContract[0].contractName;
                            contractType_tmp = resultWS.listConfigurationContract[0].contractType;
                            contractRuleId_tmp = resultWS.listConfigurationContract[0].contractRuleId;

                            if (resultWS.listConfigurationContract[0].listPenPenaltyFeeBean.Count > 0)
                            {
                                penaltyType_tmp = resultWS.listConfigurationContract[0].listPenPenaltyFeeBean[0].penaltyChargeGroup;
                            }

                            penaltyId_tmp = resultWS.listConfigurationContract[0].penaltyId;
                            limitContract_tmp = resultWS.listConfigurationContract[0].limitContract;
                            countFlg_tmp = resultWS.listConfigurationContract[0].countFlg;
                        }

                        if (contractId_tmp == "")
                        {
                            contractId_tmp = model.CustomerRegisterPanelModel.ContractID.ToSafeString();
                        }
                    }
                    //// Insert TDM 

                    InsertMasterTDMContractDeviceMeshCommand insertMasterTDMContractDeviceMeshCommand = new InsertMasterTDMContractDeviceMeshCommand()
                    {
                        Transaction_Id = AisAirNumber,
                        P_RESULT_CODE_TDM = resultCode_tmp,
                        P_CONTRACT_ID = contractId_tmp,
                        P_CONTRACT_NAME = contractName_tmp,
                        P_CONTRACT_TYPE = contractType_tmp,
                        P_CONTRACT_RULE_ID = contractRuleId_tmp,
                        P_PENALTY_TYPE = penaltyType_tmp,
                        P_PENALTY_ID = penaltyId_tmp,
                        P_LIMIT_CONTRACT = limitContract_tmp,
                        P_COUNT_FLAG = countFlg_tmp,
                        P_DURATION = duration_tmp
                    };
                    _insertMasterTDMContractDeviceMeshCommand.Handle(insertMasterTDMContractDeviceMeshCommand);

                    if (insertMasterTDMContractDeviceMeshCommand.LIST_DEVICE_CONTRACT != null && insertMasterTDMContractDeviceMeshCommand.LIST_DEVICE_CONTRACT.Count() > 0)
                    {
                        DeviceContractMeshData tmpDeviceContractMeshData = insertMasterTDMContractDeviceMeshCommand.LIST_DEVICE_CONTRACT[0];

                        model.CustomerRegisterPanelModel.TDMContractId = tmpDeviceContractMeshData.CONTRACT_ID.ToSafeString();
                        model.CustomerRegisterPanelModel.TDMRuleId = tmpDeviceContractMeshData.CONTRACT_RULE_ID.ToSafeString();
                        model.CustomerRegisterPanelModel.TDMPenaltyId = tmpDeviceContractMeshData.PENALTY_ID.ToSafeString();
                        model.CustomerRegisterPanelModel.TDMPenaltyGroupId = tmpDeviceContractMeshData.PENALTY_TYPE.ToSafeString();
                        model.CustomerRegisterPanelModel.ContractFlag = tmpDeviceContractMeshData.CONTRACT_FLAG.ToSafeString();
                    }
                }
            }

            var command = new CustRegisterCommand
            {
                QuickWinPanelModel = model,
                CurrentCulture = SiteSession.CurrentUICulture,
                InterfaceCode = interfaceCode,
                InterfaceDesc = interfaceDesc,
                InterfaceOrder = interfaceOrder,
                CoverageResultId = coverageResultId.ToSafeDecimal(),
                ClientIP = ClientIP
            };

            _custRegCommand.Handle(command);

            return command.CustomerId;
        }

        public JsonResult CheckCondoPlugAndPlay(string address_id)
        {
            var result = true;
            var CondoPlugAndPlay = base.LovData
               .Where(l => (l.Type == "FBB_CONSTANT" && l.Name == "BUILDING_WAITING_LIST"
                            && l.LovValue1 == address_id))
               .Select(l => new FbbConstantModel
               {
                   Field = l.Name,
                   Validation = l.LovValue1,
                   SubValidation = l.LovValue2
               }).ToList();

            if (CondoPlugAndPlay.Any())
                result = false;

            return Json(result);
        }

        private void SetLovValueToViewBag(QuickWinPanelModel model)
        {
            if (SiteSession.CurrentUICulture.IsThaiCulture())
            { ViewBag.LanguagePage = "1"; model.SummaryPanelModel.PDFPackageModel.PDF_L_UNIT = "1"; }
            else
            { ViewBag.LanguagePage = "2"; model.SummaryPanelModel.PDFPackageModel.PDF_L_UNIT = "2"; }

            var lovData = base.LovData.Where(l => !string.IsNullOrEmpty(l.Name) && l.Name.Equals("B_CLOSE"));
            if (SiteSession.CurrentUICulture.IsThaiCulture())
            { ViewBag.LCLOSE = lovData.FirstOrDefault().LovValue1; }
            else
            { ViewBag.LCLOSE = lovData.FirstOrDefault().LovValue2; }

            lovData = base.LovData.Where(l => !string.IsNullOrEmpty(l.Name) && l.Name.Equals("B_SAVE"));
            if (SiteSession.CurrentUICulture.IsThaiCulture())
            { ViewBag.LSAVE = lovData.FirstOrDefault().LovValue1; }
            else
            { ViewBag.LSAVE = lovData.FirstOrDefault().LovValue2; }

            //R22.06 Modify E-App

            //Logic Old
            //var AISsubtype = LovData.FirstOrDefault(item => item.Name == "AIS_SHOP_SUB_TYPE" && item.LovValue5 == "FBBOR004").LovValue1;
            //bool isShop = model.TopUp != "5" || model.CustomerRegisterPanelModel.outSubType == AISsubtype;
            //if (model.TopUp == "5" && model.CustomerRegisterPanelModel.outSubType == AISsubtype)

            //Logic New
            var AISsubtype = LovData.Where(item => item.Name == "AIS_SHOP_SUB_TYPE" && item.LovValue5 == "FBBOR004").Select(s => s.LovValue1).ToList();
            bool isShop = model.TopUp != "5" || AISsubtype.Contains(model.CustomerRegisterPanelModel.outSubType);
            if (isShop)
            {
                lovData = base.LovData.Where(l => !string.IsNullOrEmpty(l.Name) && l.Name.Equals("L_POPUP_PLUG_AND_PLAY"));
            }
            else
            {
                lovData = base.LovData.Where(l => !string.IsNullOrEmpty(l.Name) && l.Name.Equals("L_POPUP_SAVE"));
            }

            if (SiteSession.CurrentUICulture.IsThaiCulture())
            { ViewBag.LPOPUPSAVE = HttpUtility.HtmlEncode(lovData.FirstOrDefault().LovValue1); }
            else
            { ViewBag.LPOPUPSAVE = HttpUtility.HtmlEncode(lovData.FirstOrDefault().LovValue2); }

            //lovData = base.LovData.Where(l => !string.IsNullOrEmpty(l.Name) && l.Name.Equals("L_USE_ADDR_CARD"));
            //if (SiteSession.CurrentUICulture.IsThaiCulture())
            //{ ViewBag.L_USE_ADDR_CARD = lovData.FirstOrDefault().LovValue1; }
            //else
            //{ ViewBag.L_USE_ADDR_CARD = lovData.FirstOrDefault().LovValue2; }

            //lovData = base.LovData.Where(l => !string.IsNullOrEmpty(l.Name) && l.Name.Equals("B_YES"));
            //if (SiteSession.CurrentUICulture.IsThaiCulture())
            //{ ViewBag.B_YES = lovData.FirstOrDefault().LovValue1; }
            //else
            //{ ViewBag.B_YES = lovData.FirstOrDefault().LovValue2; }

            //lovData = base.LovData.Where(l => !string.IsNullOrEmpty(l.Name) && l.Name.Equals("B_NO"));
            //if (SiteSession.CurrentUICulture.IsThaiCulture())
            //{ ViewBag.B_NO = lovData.FirstOrDefault().LovValue1; }
            //else
            //{ ViewBag.B_NO = lovData.FirstOrDefault().LovValue2; }
        }

        [HttpPost]
        [AjaxValidateAntiForgeryToken]
        public JsonResult writeLog(string message, string AisAirNumber, string method, string service)
        {

            string userAgent = "";
            if (Request.Headers["User-Agent"] != null)
            {
                var headers = Request.Headers.GetValues("User-Agent");

                StringBuilder sb = new StringBuilder();

                foreach (var header in headers)
                {
                    sb.Append(header);

                    // Re-add spaces stripped when user agent string was split up.
                    sb.Append(" ");
                }

                userAgent = sb.ToString().Trim();
            }

            string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress)) ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            string transactionId = AisAirNumber + ipAddress;

            base.Logger.Info("User Agent: " + userAgent + "\r\n" + "HTTP Response: " + message + "\r\n");

            InterfaceLogCommand log = null;
            log = StartInterface("User Agent: " + userAgent + "\r\n" + message + "\r\n", method, transactionId, message, service);

            EndInterface("", log, transactionId, "Success", "");

            return Json("Success: " + service + ", " + method);
        }

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
                IN_TRANSACTION_ID = transactionId,
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

        [AllowAnonymous]
        [CustomActionFilter(LogType = "Login")]
        public ActionResult Login(string returnUrl)
        {
            Session[WebConstants.FBBConfigSessionKeys.User] = null;
            try
            {
                var query = new SelectLovQuery
                {
                    LOV_TYPE = "CONFIG_TEMP_LOGIN_PAGE"
                };
                var data = _queryProcessor.Execute(query);

                //Logger.Info(data.DumpToString("TemporaryLogin"));

                string flg = "N";
                if (data.Any())
                    flg = data.FirstOrDefault().LOV_VAL1;

                if (flg == "Y")
                {
                    ViewBag.ReturnUrl = returnUrl;
                    var titleQuery = new GetLovQuery
                    {
                        LovType = "FBB_CONSTANT",
                        LovName = "TITLE_LOGIN_PAGE",
                    };
                    var titleLogin = _queryProcessor.Execute(titleQuery);
                    ViewBag.configscreen = titleLogin;
                    return View();
                }
                else
                {
                    ViewBag.ReturnUrl = returnUrl;
                    return RedirectToAction("TemporaryNotAvailable", "Process");
                }

            }
            catch (Exception ex)
            {
                Logger.Info(ex.GetErrorMessage());
                Logger.Info(ex.StackTrace);

                return RedirectToAction("TemporaryNotAvailable", "Process");
            }
        }

        public JsonResult CheckTechnology(string addressID)
        {

            var query2 = new GetMeshCheckTechnologyQuery
            {
                addressID = addressID
            };

            var model = _queryProcessor.Execute(query2);

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        //R24.01 Fix IP Camera AR order by Max kunlp885
        //public JsonResult CheckAROrder(string NonMobileNo)
        public JsonResult CheckAROrder(string NonMobileNo, string PackageType)
        {

            var chkAROrder = new CheckAROrderQuery
            {
                BroadbandId = NonMobileNo,//"8800024585",
                //Option = "Mesh",
                Option = PackageType,
                FullUrl = ""
            };

            var dataAROrder = _queryProcessor.Execute(chkAROrder);

            var result = true;
            if ((dataAROrder.Count > 0 && dataAROrder.Where(l => l.installFlag.Equals("N")).ToList().Count > 0))
                result = false;

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckAROrderMesh(string NonMobileNo)
        {

            var chkAROrder = new CheckAROrderQuery
            {
                BroadbandId = NonMobileNo,//"8800024585",
                //Option = "Mesh",
                Option = "Mesh",
                FullUrl = ""
            };

            var dataAROrder = _queryProcessor.Execute(chkAROrder);

            var result = false;
            if ((dataAROrder.Count > 0 && dataAROrder.Where(l => l.installFlag.Equals("N")).ToList().Count == 0)
                || dataAROrder.Count == 0
                )
                result = true;

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SavePreRegisterWTTx(QuickWinPanelModel model)
        {
            #region Get IP Address Interface Log : Edit 2017-01-30

            string ClientIP = "";

            // Get IP Address
            string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress)) ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            ClientIP = ipAddress;

            #endregion

            string FullUrl = "";
            if (Session["FullUrl"] != null)
                FullUrl = Session["FullUrl"].ToSafeString();

            TempData["isWTTx"] = model.CoveragePanelModel.WTTX_COVERAGE_RESULT == "YES" ? true : false;

            string WTTxMSG = "";
            string WTTxErrorMSG = "";
            LovValueModel WTTxMSGtmp = base.LovData.FirstOrDefault(t => t.Name == "WTTx_SUCCESS_OFF_1");
            LovValueModel WTTxErrorMSGtmp = base.LovData.FirstOrDefault(t => t.Name == "WTTx_ERROR_MSG");
            if (SiteSession.CurrentUICulture.IsThaiCulture())
            {
                WTTxMSG = WTTxMSGtmp != null ? WTTxMSGtmp.LovValue1.ToSafeString() : "";
                WTTxErrorMSG = WTTxErrorMSGtmp != null ? WTTxErrorMSGtmp.LovValue1.ToSafeString() : "";
            }
            else
            {
                WTTxMSG = WTTxMSGtmp != null ? WTTxMSGtmp.LovValue2.ToSafeString() : "";
                WTTxErrorMSG = WTTxErrorMSGtmp != null ? WTTxErrorMSGtmp.LovValue2.ToSafeString() : "";
            }

            string FBBNo = evFBBGenerateFBBNo(model.CoveragePanelModel.L_CONTACT_PHONE, FullUrl);
            if (FBBNo == "")
            {
                TempData["WTTxErrorMSG"] = WTTxErrorMSG;
                return RedirectToAction("IndexWithModel", new { model = "" });
            }
            model.CustomerRegisterPanelModel.FIBRE_ID = FBBNo;
            model.CoveragePanelModel.P_MOBILE = FBBNo;

            string orderId = "";
            //Gen orderId
            orderId = GetPaymentOrderID();

            if (orderId == "")
            {
                TempData["WTTxErrorMSG"] = WTTxErrorMSG;
                return RedirectToAction("IndexWithModel", new { model = "" });
            }
            model.PayMentOrderID = orderId;

            // Call AIR_ADMIN.PKG_FBBOR911.INSERT_SAVE_ORDER_NEW InsertSaveOrderNew911
            string orderNo = "";
            orderNo = InsertSaveOrderNew911(model);

            if (orderNo == "")
            {

                TempData["WTTxErrorMSG"] = WTTxErrorMSG;
                return RedirectToAction("IndexWithModel", new { model = "" });
            }

            // Check Entrry Fee
            PackageModel EntryFeePackage = model.SummaryPanelModel.PackageModelList.Where(t => t.PACKAGE_TYPE == "6" && t.PACKAGE_SERVICE_CODE == "00001").FirstOrDefault();
            decimal? EntrryFeePriceCharge = 0;
            if (EntryFeePackage != null)
            {
                EntrryFeePriceCharge = EntryFeePackage.PRICE_CHARGE;
            }

            if (EntrryFeePriceCharge > 0)
            {
                // Call RegisterCustomer
                string customerRowID = RegisterCustomer(model, "", "", "", ClientIP);

                // Get Lov payment_methods
                List<string> payment_methods = base.LovData.Where(t => t.Type == "FBB_PAYMENT_METHOD" && t.Name == "WEBREGISTER_WTTX").Select(t => t.LovValue1).ToList();
                LovValueModel typeData = base.LovData.FirstOrDefault(t => t.Type == "FBB_CONFIG_PAYMENT_WTTX" && t.Name == "PRODUCT_NAME");
                string type = typeData != null ? typeData.LovValue1.ToSafeString() : "";
                LovValueModel channelData = base.LovData.FirstOrDefault(t => t.Type == "FBB_CONFIG_PAYMENT_WTTX" && t.Name == "REGISTER_CHANNEL");
                string channel = channelData != null ? channelData.LovValue1.ToSafeString() : "";

                PaymentChannelModel paymentChannelModel = new PaymentChannelModel()
                {
                    fbb_id = FBBNo,
                    payment_transaction_id = orderId,
                    register_channel = channel,
                    product_name = type,
                    list_payment_method = payment_methods
                };
                string paymentChannelJsonStr = JsonConvert.SerializeObject(paymentChannelModel);
                paymentChannelModel.data = Encrypt(paymentChannelJsonStr);
                paymentChannelModel.fbb_id = "";
                paymentChannelModel.payment_transaction_id = "";
                paymentChannelModel.register_channel = "";
                paymentChannelModel.product_name = "";
                paymentChannelModel.list_payment_method = null;

                //Check From officer
                if (model.CustomerRegisterPanelModel.L_LOC_CODE.ToSafeString() != ""
                || model.CustomerRegisterPanelModel.L_ASC_CODE.ToSafeString() != ""
                 || model.CustomerRegisterPanelModel.L_STAFF_ID.ToSafeString() != "")
                {
                    // Set url For Bypass To payment
                    string protocol = "https";
                    string urlForBypass = this.Url.Action("PaymentChannel", "FBBPayment", null, protocol) + "?data=" + paymentChannelModel.data;

                    // Lov For SMS msgtxt
                    string msgtxt = "";
                    LovValueModel smsData = base.LovData.FirstOrDefault(t => t.Type == "SCREEN" && t.Name == "SMS_MESSAGE_WTTX_PAYMENT");
                    if (smsData != null && model.CoveragePanelModel.L_CONTACT_PHONE.ToSafeString() != "")
                    {
                        if (SiteSession.CurrentUICulture.IsThaiCulture())
                        {
                            msgtxt = smsData.LovValue1.ToSafeString();
                        }
                        else
                        {
                            msgtxt = smsData.LovValue2.ToSafeString();
                        }
                        msgtxt = msgtxt.Replace("[PaymentURL]", urlForBypass);
                        // Send url SMS 

                        var command = new SendSmsCommand();
                        command.FullUrl = FullUrl;
                        command.Source_Addr = "AISFIBRE";
                        command.Destination_Addr = model.CoveragePanelModel.L_CONTACT_PHONE;
                        command.Transaction_Id = model.CoveragePanelModel.L_CONTACT_PHONE;
                        command.Message_Text = msgtxt;
                        _sendSmsCommand.Handle(command);
                        WTTxMSG = WTTxMSG.Replace("{mobileNo}", "xxx-xxx-" + model.CoveragePanelModel.L_CONTACT_PHONE.Substring(6, 4));
                        TempData["WTTxMSG"] = WTTxMSG;
                    }
                    else
                    {
                        TempData["WTTxErrorMSG"] = WTTxErrorMSG;
                    }

                    return RedirectToAction("IndexWithModel", new { model = "" });
                }
                else
                {
                    return RedirectToAction("PaymentChannel", "FBBPayment", paymentChannelModel);
                }
            }
            else
            {
                LovValueModel WTTxMSGSuccesstmp = base.LovData.FirstOrDefault(t => t.Name == "WTTx_SUCCESS_MSG_1");
                if (SiteSession.CurrentUICulture.IsThaiCulture())
                {
                    WTTxMSG = WTTxMSGSuccesstmp != null ? WTTxMSGSuccesstmp.LovValue1.ToSafeString() : "";
                }
                else
                {
                    WTTxMSG = WTTxMSGSuccesstmp != null ? WTTxMSGSuccesstmp.LovValue2.ToSafeString() : "";
                }

                string reservedIdSTR = "";
                // get from lov RESERVE_EXPIRE_WTTX
                int reservedExpTime = 24;
                LovValueModel loveList = base.LovData.Where(lov => lov.Name == "RESERVE_EXPIRE_WTTX").FirstOrDefault();
                if (loveList != null && loveList.LovValue1 != null)
                {
                    int.TryParse(loveList.LovValue1, out reservedExpTime);
                }

                DateTime expTime = DateTime.Now.AddHours(reservedExpTime);
                GetWTTXReserveQuery getWTTXReserveQuery = new GetWTTXReserveQuery()
                {
                    gridId = model.CoveragePanelModel.GRID_ID,
                    reservedExpTime = expTime.ToString("dd/MM/yyyy HH:mm:ss"),
                    transaction_id = model.CoveragePanelModel.L_CONTACT_PHONE
                };

                WTTXReserveModel wttxReserveModel = _queryProcessor.Execute(getWTTXReserveQuery);
                if (wttxReserveModel != null)
                {
                    reservedIdSTR = wttxReserveModel.reservedId != null ? wttxReserveModel.reservedId : "";
                }
                model.ReservationId = reservedIdSTR;

                SaveOrderResp saveOrderResp = GetSaveOrderResp(model);

                LovValueModel PayMentMethodWttxNoEntryfee = base.LovData.FirstOrDefault(t => t.Name == "WTTX_BY_ENTRYFEE0");
                if (PayMentMethodWttxNoEntryfee != null)
                {
                    model.PayMentMethod = PayMentMethodWttxNoEntryfee.LovValue1.ToSafeString();
                }

                string customerRowID = RegisterCustomer(model,
                        saveOrderResp.RETURN_CODE.ToSafeString(),
                        saveOrderResp.RETURN_MESSAGE,
                        saveOrderResp.RETURN_ORDER_NO,
                        ClientIP);
                bool isCreateOrder = true;
                if (customerRowID == null || customerRowID == "")
                {
                    isCreateOrder = false;
                }

                if (isCreateOrder)
                {
                    if (model.SummaryPanelModel.L_SEND_EMAIL)
                    {
                        #region PDF

                        var newregisterFlag = true;
                        var running_no = InsertMailLog(customerRowID);
                        string uploadFileWebPath = Configurations.UploadFilePath;
                        string uploadFileAppPath = Configurations.UploadFileTempPath;

                        model.CustomerRegisterPanelModel.L_INSTALL_DATE = model.CustomerRegisterPanelModel.L_INSTALL_DATE + "  " + model.CustomerRegisterPanelModel.FBSSTimeSlot.TimeSlot;

                        System.IFormatProvider format = new System.Globalization.CultureInfo("en-US");
                        string filename = "Request" + DateTime.Now.ToString("ddMMyy", format) + "_" + running_no.ToSafeString();
                        string directoryPath = "";
                        string directoryPathApp = "";

                        //R22.06 Modify E-App
                        //Logic New
                        var AISsubtype = LovData.Where(item => item.Name == "AIS_SHOP_SUB_TYPE" && item.LovValue5 == "FBBOR004").Select(s => s.LovValue1).ToList();
                        bool isShop = model.TopUp != "5" || AISsubtype.Contains(model.CustomerRegisterPanelModel.outSubType);
                        //Logic Old
                        //var AISsubtype = LovData.FirstOrDefault(item => item.Name == "AIS_SHOP_SUB_TYPE" && item.LovValue5 == "FBBOR004").LovValue1;
                        //bool isShop = model.TopUp != "5" || model.CustomerRegisterPanelModel.outSubType == AISsubtype;

                        string isShopString = isShop ? "Y" : "N";

                        Logger.Info(model.DumpToXml());

                        directoryPath = GeneratePDF_HTML(model, @uploadFileWebPath, @uploadFileAppPath, filename, isShopString);

                        Session["FILENAME"] = filename;

                        if (isShop)
                        {
                            var langPDFAPP = "";
                            if (SiteSession.CurrentUICulture.IsThaiCulture())
                                langPDFAPP = "T";
                            else
                                langPDFAPP = "E";
                            @directoryPathApp = GeneratePDFApp(model.CustomerRegisterPanelModel.L_CARD_NO, model.CustomerRegisterPanelModel.OrderNo, langPDFAPP, model.CoveragePanelModel.L_CONTACT_PHONE, model);
                        }

                        #endregion PDF
                        #region SendEmail

                        try
                        {
                            Logger.Info(string.Format("Step 1 ,SendEmail ,running_no = {0},L_SEND_EMAIL = {1}", running_no, model.SummaryPanelModel.L_SEND_EMAIL));

                            if (model.SummaryPanelModel.L_SEND_EMAIL)
                            {
                                Logger.Info(string.Format("Step 2 ,SendEmail ,running_no = {0},L_EMAIL = {1}", running_no, model.CustomerRegisterPanelModel.L_EMAIL));

                                if (!string.IsNullOrEmpty(model.CustomerRegisterPanelModel.L_EMAIL))
                                {
                                    Logger.Info(string.Format("Step 3 ,SendEmail ,running_no = {0},newregisterFlag = {1}", running_no, newregisterFlag));

                                    if (!newregisterFlag)
                                    {
                                        SendEmail(customerRowID, running_no, model.CustomerRegisterPanelModel.L_EMAIL, @directoryPath, @directoryPathApp);
                                    }
                                    else
                                    {
                                        Logger.Info(string.Format("Step 4 ,SendEmail ,running_no = {0},ReceiveEmailFlag = {1}", running_no, model.CustomerRegisterPanelModel.ReceiveEmailFlag));
                                        if (model.CustomerRegisterPanelModel.ReceiveEmailFlag == "Y")
                                        {
                                            SendEmail(customerRowID, running_no, model.CustomerRegisterPanelModel.L_EMAIL, @directoryPath, @directoryPathApp);
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Info("Error SendEmail , Error message = " + ex.Message + " , Error Base" + ex.GetBaseException());
                        }

                        #endregion SendEmail
                    }

                    string msgtxt = "";
                    LovValueModel smsData = base.LovData.FirstOrDefault(t => t.Type == "SCREEN" && t.Name == "SMS_MESSAGE_WTTX_REGIS_SUCCESS");
                    if (smsData != null && model.CoveragePanelModel.L_CONTACT_PHONE.ToSafeString() != "")
                    {
                        if (SiteSession.CurrentUICulture.IsThaiCulture())
                        {
                            msgtxt = smsData.LovValue1.ToSafeString();
                        }
                        else
                        {
                            msgtxt = smsData.LovValue2.ToSafeString();
                        }

                        List<PackageModel> MainPackage = model.SummaryPanelModel.PackageModelList.Where(t => t.PACKAGE_TYPE == "1").ToList();
                        if (MainPackage != null)
                        {
                            List<AIR_CHANGE_OLD_PACKAGE_ARRAY> air_old_list = new List<AIR_CHANGE_OLD_PACKAGE_ARRAY>();
                            foreach (var item in MainPackage)
                            {
                                AIR_CHANGE_OLD_PACKAGE_ARRAY air = new AIR_CHANGE_OLD_PACKAGE_ARRAY()
                                {
                                    enddt = "",
                                    productSeq = "",
                                    sffPromotionCode = item.SFF_PROMOTION_CODE,
                                    startdt = ""
                                };
                                air_old_list.Add(air);
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
                                string packageMainName = "";
                                if (result != null && result.Count > 0)
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
                                            msgtxt = msgtxt.Replace("{Package}", packageMainName);

                                        }
                                    }
                                }
                            }
                        }

                        var command = new SendSmsCommand();
                        command.FullUrl = FullUrl;
                        command.Source_Addr = "AISFIBRE";
                        command.Destination_Addr = model.CoveragePanelModel.L_CONTACT_PHONE;
                        command.Transaction_Id = model.CoveragePanelModel.L_CONTACT_PHONE;
                        command.Message_Text = msgtxt;
                        _sendSmsCommand.Handle(command);
                    }
                    WTTxMSG = WTTxMSG.Replace("{non_mobile_no}", model.CoveragePanelModel.P_MOBILE);
                    TempData["WTTxMSG"] = WTTxMSG;
                }
                else
                {
                    TempData["WTTxErrorMSG"] = WTTxErrorMSG;
                }

                return RedirectToAction("IndexWithModel", new { model = "" });
            }
        }

        [HttpPost]
        public JsonResult SavePreRegisterCustomer(QuickWinPanelModel model)
        {
            InterfaceLogCommand log = null;
            log = StartInterface("PayMentOrderID: " + model.PayMentOrderID + "L_INSTALL_DATE: " + model.CustomerRegisterPanelModel.L_INSTALL_DATE, "SavePreRegisterCustomer", model.CoveragePanelModel.P_MOBILE, "", "SavePreRegisterCustomer");

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


            /// FBSSQueryOrderDoServiceConfirm
            FBSSQueryOrderDoServiceConfirm(model.CoveragePanelModel.P_MOBILE);

            //save RegisterCustomer

            string customerRowID = RegisterCustomer(model, "", "", "", ClientIP);
            string dataForMesh = "";
            dataForMesh = Encrypt(model.CoveragePanelModel.P_MOBILE + "&" + model.CustomerRegisterPanelModel.L_CONTACT_PHONE + "&" + orderId);
            EndInterface(orderId, log, "", "Success", "End Pocesss SavePreRegisterCustomer");
            return Json(new { data = orderId, dataForMesh = dataForMesh }, JsonRequestBehavior.AllowGet);
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

        [HttpPost]
        [AllowAnonymous]
        [CustomActionFilter(LogType = "Login")]
        public ActionResult Login(LoginPanelModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var userName = model.UserName.Replace("\"", "").Replace("'", "");
                var passWord = model.Password.Replace("'", "").Replace("'", "");
                List<LovValueModel> groupId;

                var grouIdSaleQuery = new GetLovQuery
                {
                    LovType = "FBB_CONSTANT",
                    LovName = "USER_SALE_MNG_GROUP_ID"
                };
                groupId = _queryProcessor.Execute(grouIdSaleQuery);
                var authenticatedUser = GetUser(userName, groupId[0].LovValue1);

                Session["isSaleManager"] = "true";

                if (null == authenticatedUser.UserName || (authenticatedUser.Groups == null))
                {
                    var grouIdQuery = new GetLovQuery
                    {
                        LovType = "FBB_CONSTANT",
                        LovName = "USER_EN_GROUP_ID"
                    };
                    groupId = _queryProcessor.Execute(grouIdQuery);
                    authenticatedUser = GetUser(userName, groupId[0].LovValue1);
                    Session["isSaleManager"] = "false";
                }

                var msgUserQuery = new GetLovQuery
                {
                    LovType = "FBB_CONSTANT",
                    LovName = "MSG_USER_FAILD"
                };
                var msgUser = _queryProcessor.Execute(msgUserQuery);

                var msgPassQuery = new GetLovQuery
                {
                    LovType = "FBB_CONSTANT",
                    LovName = "MSG_USER_PASS_FAILD"
                };
                var msgPass = _queryProcessor.Execute(msgPassQuery);

                var cfgLDAPQuery = new GetLovQuery
                {
                    LovType = "FBB_CONSTANT",
                    LovName = "URLAuthenLDAP"
                };
                var cfgLDAP = _queryProcessor.Execute(cfgLDAPQuery);
                bool useLDAP = false;
                if (cfgLDAP != null && cfgLDAP.Count > 0)
                {
                    useLDAP = true;
                }

                if (useLDAP)
                {

                    if (null != authenticatedUser.UserName
                        && (authenticatedUser.Groups != null)
                        && (authenticatedUser.Groups[0] == Convert.ToDecimal(groupId[0].LovValue1)))
                    {
                        var authenResultMessage = "";
                        if (AuthenLDAP(userName, passWord, out authenResultMessage))
                        {
                            //var authenticatedUser = GetUser(userName);
                            authenticatedUser.AuthenticateType = AuthenticateType.LDAP;
                            Response.AppendCookie(CreateAuthenticatedCookie(authenticatedUser.UserName));
                            base.CurrentUser = authenticatedUser;

                            return RedirectToAction("Engineers", "Process");
                        }
                        else
                        {
                            ModelState.AddModelError("", msgPass[0].LovValue1);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", userName + " " + msgUser[0].LovValue1);
                    }
                }
                else
                {
                    // bypass authen
                    //Session["userName"] = userName;
                    if (null != authenticatedUser.UserName && (authenticatedUser.Groups != null))
                    {
                        authenticatedUser.AuthenticateType = AuthenticateType.LDAP;
                        Response.AppendCookie(CreateAuthenticatedCookie(authenticatedUser.UserName));
                        base.CurrentUser = authenticatedUser;
                        return RedirectToAction("Engineers", "Process");
                    }
                    else
                    {
                        ModelState.AddModelError("", userName + " " + msgUser[0].LovValue1);
                    }
                }
            }
            var titleQuery = new GetLovQuery
            {
                LovType = "FBB_CONSTANT",
                LovName = "TITLE_LOGIN_PAGE",
            };
            var titleLogin = _queryProcessor.Execute(titleQuery);
            ViewBag.configscreen = titleLogin;
            return View(model);
        }

        public UserModel GetUser(string userName, string groupId)
        {
            var userQuery = new GetUserDataQuery
            {
                UserName = userName,
                GroupId = groupId
            };

            var authenticatedUser = _queryProcessor.Execute(userQuery);
            return authenticatedUser;
        }

        public UserModel GetUserPinCode(string pinCode, string groupId)
        {
            var userQuery = new GetUserDataQuery
            {
                UserName = "",
                PinCode = pinCode,
                GroupId = groupId
            };

            var authenticatedUser = _queryProcessor.Execute(userQuery);
            return authenticatedUser;
        }

        private System.Web.HttpCookie CreateAuthenticatedCookie(string userName)
        {
            var authTicket = new FormsAuthenticationTicket(2, userName, DateTime.Now,
                DateTime.Now.AddMinutes(FormsAuthentication.Timeout.TotalMinutes), false, "");

            var authCookie = new System.Web.HttpCookie(FormsAuthentication.FormsCookieName,
                FormsAuthentication.Encrypt(authTicket))
            { HttpOnly = true };

            return authCookie;
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult TemporaryNotAvailable()
        {
            return View();
        }

        public bool AuthenLDAP(string userName, string password, out string authenMessage)
        {
            var authLDAPQuery = new GetAuthenLDAPQuery
            {
                UserName = userName,
                Password = password,
                ProjectCode = Configurations.ProjectCodeLdapFBB,
            };

            var authenLDAPResult = _queryProcessor.Execute(authLDAPQuery);
            authenMessage = "";
            return authenLDAPResult;
        }

        public JsonResult CheckMobileNumberSerenade(string msisdn)
        {
            string FullUrl = "";
            if (Session["FullUrl"] != null)
                FullUrl = Session["FullUrl"].ToSafeString();

            var query = new CheckMobileNumberSerenadeQuery
            {
                MobileNo = msisdn,
                FullUrl = FullUrl
            };

            var model = _queryProcessor.Execute(query);

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        private string IdCard = "";
        private string CardType = "";

        private string GetInfoByNonMobileNo(string NonMobileNo = "")
        {
            string FullUrl = "";
            if (Session["FullUrl"] != null)
                FullUrl = Session["FullUrl"].ToSafeString();

            #region Get IP Address Interface Log (Update 17.2)

            // Get IP Address
            string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress)) ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            #endregion

            string user = "";
            string InOption = "2";
            if (base.CurrentUser != null) user = base.CurrentUser.UserName;

            var query = new evESeServiceQueryMassCommonAccountInfoQuery
            {
                inOption = InOption,
                inMobileNo = NonMobileNo,
                Page = "Check SearchProfilePrePostpaid",
                Username = user,
                ClientIP = ipAddress,
                FullUrl = FullUrl
            };
            var a = _queryProcessor.Execute(query);
            if (a != null)
            {
                var CustomerData = GetCustomerInfo(a.outAccountNumber);
                if (CustomerData != null && CustomerData.idCardNum != "")
                {
                    IdCard = CustomerData.idCardNum;
                    CardType = CustomerData.idCardType;

                    return "";
                }
                else
                {
                    IdCard = "";
                    CardType = "";
                    return "NodataCustomer";
                }
            }
            else
            {
                return "NodataCustomer";
            }
        }

        private evESeServiceQueryMassCommonAccountInfoModel GetMassCommonAccountInfo(string mobileNo = "", string cardNo = "", string cardType = "", string line = "", string SubNetworkType = "")
        {
            string FullUrl = "";
            if (Session["FullUrl"] != null)
                FullUrl = Session["FullUrl"].ToSafeString();

            string user = "";
            if (base.CurrentUser != null) user = base.CurrentUser.UserName;

            string InOption = "";
            if (SubNetworkType == "PREPAID")
            {
                InOption = "4";
            }
            else
            {
                InOption = "2";
            }

            var query = new evESeServiceQueryMassCommonAccountInfoQuery
            {
                //inOption = "1",
                inOption = InOption,//16.4 change use option2
                inMobileNo = mobileNo,
                inCardNo = cardNo,
                inCardType = cardType,
                Page = "Check Coverage",
                Username = user,
                FullUrl = FullUrl
            };
            evESeServiceQueryMassCommonAccountInfoModel a = _queryProcessor.Execute(query);





            return a;
        }

        // evAMQueryCustomerInfoQuery
        private evAMQueryCustomerInfoModel GetCustomerInfo(string accntNo = "")
        {
            var query = new evAMQueryCustomerInfoQuery
            {
                accntNo = accntNo
            };
            evAMQueryCustomerInfoModel a = _queryProcessor.Execute(query);

            return a;
        }

        //R17.11 Officer for Existing Customer
        private string GetRedirectTopupPage(string pageStr, string existingFlag)
        {
            try
            {
                if (pageStr.ToSafeString().ToLower() == "topupplaybox")
                {
                    if ((existingFlag.ToSafeString() == "INTERNET") || (existingFlag.ToSafeString() == "INTERNET:MENU"))
                    {
                        pageStr = "TopupInternet";
                    }
                }

                if (pageStr.ToSafeString().ToLower() == "topup")
                {
                    if ((existingFlag.ToSafeString() == "FIXLINE") || (existingFlag.ToSafeString() == "FIXLINE:MENU"))
                    {
                        pageStr = "TopUpFixedline";
                    }
                }

                if (pageStr.ToSafeString().ToLower() == "topupreplace")
                {//R22.03 TopupReplace
                    if ((existingFlag.ToSafeString() == "PLAYBOXREPLACE") || (existingFlag.ToSafeString() == "PLAYBOXREPLACE:MENU"))
                    {
                        pageStr = "TopupReplace";
                    }
                }
                return pageStr;
            }
            catch (Exception)
            {
                return pageStr;
            }
        }

        //R17.11 Officer for Existing Customer
        private string GetExistingTopupFlag(string existingFlag)
        {
            try
            {
                if ((existingFlag.ToSafeString() == "FIXLINE:MENU") ||
                    (existingFlag.ToSafeString() == "INTERNET:MENU") ||
                    (existingFlag.ToSafeString() == "PLAYBOX:MENU") ||
                    (existingFlag.ToSafeString() == "PLAYBOXREPLACE:MENU") ||
                    (existingFlag.ToSafeString() == "IPCAMERA:MENU"))
                {
                    return "Y";
                }
                return "N";
            }
            catch (Exception)
            {
                return "N";
            }
        }

        public string GetOntopContent(string mc, string ec, string proSubtype3, string recurringCharge)
        {
            string strBuilder = "";
            string topup = "";
            string imagebase64 = "";
            var ContentPlaybox = GetLovConfigBytype("NEW_CONTENT_PLAYBOX");
            LovScreenValueModel newContentPlayboxDetail = new LovScreenValueModel();
            newContentPlayboxDetail = ContentPlaybox.Where(p => p.Type == "NEW_CONTENT_PLAYBOX" && p.DisplayValueJing == "ICON_DES" && p.Name == proSubtype3).FirstOrDefault();
            LovScreenValueModel newContentPlayboxImage = new LovScreenValueModel();
            newContentPlayboxImage = ContentPlaybox.Where(p => p.Type == "NEW_CONTENT_PLAYBOX" && p.DisplayValueJing == "ICON_PATH" && p.Name == proSubtype3).FirstOrDefault();

            topup = newContentPlayboxDetail != null ? newContentPlayboxDetail.DisplayValue : "";
            imagebase64 = newContentPlayboxImage != null ? newContentPlayboxImage.Blob : "";
            topup = topup.Replace("{0}", recurringCharge);
            strBuilder += "<div><label>";
            strBuilder += "<div class='col-sm-1 col-xs-1' id='vasBoxSub_chkPlayBoxGold'>";
            strBuilder += "<input type='checkbox' indexValue='" + mc + ec + "' id='vasBoxSub" + ec + "_chkbox' onclick='onVasBoxFullHD_Click(checkboxtopup,this)'>";
            strBuilder += "</div>";
            strBuilder += "<div class='col-sm-5 col-xs-5'>";
            strBuilder += "<img src='data:image/png;base64," + imagebase64 + "' class='img-responsive center-block' style='vertical-align:middle'> ";
            strBuilder += "</div>";
            strBuilder += "<div class='col-sm-6 col-xs-6'>";
            strBuilder += topup;
            strBuilder += "</div>";
            strBuilder += "</label></div>";

            return strBuilder;
        }

        public string GetOntopContentDetail(string proSubtype3)
        {
            string strBuilder = "";
            string imageHead = "";
            string detailHead = "";
            string imageDetail = "";
            string detail = "";
            var ContentPlaybox = GetLovConfigBytype("NEW_CONTENT_PLAYBOX");
            LovScreenValueModel newContentPlayboxImageHead = new LovScreenValueModel();
            newContentPlayboxImageHead = ContentPlaybox.Where(p => p.Type == "NEW_CONTENT_PLAYBOX" && p.DisplayValueJing == "DETAIL_PLABOX_PATH" && p.Name == proSubtype3).FirstOrDefault();
            LovScreenValueModel newContentPlayboxDetailHead = new LovScreenValueModel();
            newContentPlayboxDetailHead = ContentPlaybox.Where(p => p.Type == "NEW_CONTENT_PLAYBOX" && p.DisplayValueJing == "DETAIL_HEADER" && p.Name == proSubtype3).FirstOrDefault();
            LovScreenValueModel newContentPlayboxImage = new LovScreenValueModel();
            newContentPlayboxImage = ContentPlaybox.Where(p => p.Type == "NEW_CONTENT_PLAYBOX" && p.DisplayValueJing == "DETAIL_CONTENT_PATH" && p.Name == proSubtype3).FirstOrDefault();
            //LovScreenValueModel newContentPlayboxDetail = new LovScreenValueModel();
            //newContentPlayboxDetail = ContentPlaybox.Where(p => p.Type == "NEW_CONTENT_PLAYBOX" && p.DisplayValueJing == "DETAIL_CONTENT_DESCRIPTION" && p.Name == proSubtype3).FirstOrDefault();
            List<LovScreenValueModel> newContentPlayboxDetailList = ContentPlaybox.Where(p => p.Type == "NEW_CONTENT_PLAYBOX" && p.DisplayValueJing == "DETAIL_CONTENT_DESCRIPTION" && p.Name == proSubtype3).OrderBy(p => p.OrderByPDF ?? 0).ToList();

            imageHead = newContentPlayboxImageHead != null ? newContentPlayboxImageHead.Blob : "";
            detailHead = newContentPlayboxDetailHead != null ? newContentPlayboxDetailHead.DisplayValue : "";
            imageDetail = newContentPlayboxImage != null ? newContentPlayboxImage.Blob : "";
            //detail = newContentPlayboxDetail != null ? newContentPlayboxDetail.DisplayValue : "";
            var sb = new StringBuilder();
            foreach (var item in newContentPlayboxDetailList)
            {
                sb.Append(item.DisplayValue);
            }
            detail = sb.ToString();

            strBuilder += "<div class='col-xs-12'>";
            strBuilder += "<img src='data:image/png;base64," + imageHead + "' class='img-responsive pull-left'>";
            strBuilder += "</div>";
            strBuilder += "<p class='clearfix'></p>";
            strBuilder += "<i class='fa fa-chevron-right tgreen'></i>" + detailHead;
            strBuilder += "<p class='clearfix'></p>";
            strBuilder += "<div class='col-xs-12 text-center'>";
            if (imageDetail != "")
            {
                strBuilder += "<img src='data:image/png;base64," + imageDetail + "' class='img-responsive center-block'>";
            }
            strBuilder += "</div>";
            strBuilder += "<p class='clearfix'></p>";
            strBuilder += "<p class='clearfix'></p>";
            strBuilder += "<div class='col-xs-12'>";
            strBuilder += "<div class='add-rounded-gray'>";
            strBuilder += "<p class='clearfix'></p>";
            strBuilder += "<span>" + detail + "</span>";
            strBuilder += "<p class='clearfix'></p>";
            strBuilder += "</div>";
            strBuilder += "</div>";

            return strBuilder;
        }

        public string GetOntopContentV2(string indexContent, string sffPromotionCode, string priceCharge)
        {
            string strBuilder = "";
            string topup = "";
            string imagebase64 = "";
            var ContentPlaybox = GetLovConfigBytype("NEW_CONTENT_PLAYBOX");
            LovScreenValueModel newContentPlayboxDetail = new LovScreenValueModel();
            newContentPlayboxDetail = ContentPlaybox.Where(p => p.Type == "NEW_CONTENT_PLAYBOX" && p.DisplayValueJing == "ICON_DES" && p.Name == sffPromotionCode).FirstOrDefault();
            LovScreenValueModel newContentPlayboxImage = new LovScreenValueModel();
            newContentPlayboxImage = ContentPlaybox.Where(p => p.Type == "NEW_CONTENT_PLAYBOX" && p.DisplayValueJing == "ICON_PATH" && p.Name == sffPromotionCode).FirstOrDefault();

            topup = newContentPlayboxDetail != null ? newContentPlayboxDetail.DisplayValue : "";
            imagebase64 = newContentPlayboxImage != null ? newContentPlayboxImage.Blob : "";
            topup = topup.Replace("{0}", priceCharge);
            strBuilder += "<div><label>";
            strBuilder += "<div class='col-sm-1 col-xs-1'>";
            strBuilder += "<input type='checkbox' id='contentSubPlaybox_" + sffPromotionCode + "' onclick='onContentPlayBox_Click(divCheckboxContentPlaybox,this)'>";
            strBuilder += "</div>";
            strBuilder += "<div class='col-sm-5 col-xs-5'>";
            strBuilder += "<img src='data:image/png;base64," + imagebase64 + "' class='img-responsive center-block' style='vertical-align:middle'> ";
            strBuilder += "</div>";
            strBuilder += "<div class='col-sm-6 col-xs-6'>";
            strBuilder += topup;
            strBuilder += "</div>";
            strBuilder += "</label></div>";

            return strBuilder;
        }

        public string GetOntopContentV2Ontop(string indexContent, string sffPromotionCode, string priceCharge, string divIdOntop)
        {// R21.11 ATV
            string strBuilder = "";
            string topup = "";
            string imagebase64 = "";
            var ContentPlaybox = GetLovConfigBytype("NEW_CONTENT_PLAYBOX");
            LovScreenValueModel newContentPlayboxDetail = new LovScreenValueModel();
            newContentPlayboxDetail = ContentPlaybox.Where(p => p.Type == "NEW_CONTENT_PLAYBOX" && p.DisplayValueJing == "ICON_DES" && p.Name == sffPromotionCode).FirstOrDefault();
            LovScreenValueModel newContentPlayboxImage = new LovScreenValueModel();
            newContentPlayboxImage = ContentPlaybox.Where(p => p.Type == "NEW_CONTENT_PLAYBOX" && p.DisplayValueJing == "ICON_PATH" && p.Name == sffPromotionCode).FirstOrDefault();

            topup = newContentPlayboxDetail != null ? newContentPlayboxDetail.DisplayValue : "";
            imagebase64 = newContentPlayboxImage != null ? newContentPlayboxImage.Blob : "";
            topup = topup.Replace("{0}", priceCharge);
            strBuilder += "<div class='checkbox'><label style='padding-left:5px;'>";
            strBuilder += "<div class='col-sm-1 col-xs-1'>";
            strBuilder += "<input name='contentPlayboxOntop'type='checkbox' id='contentPlayboxOntop_" + sffPromotionCode + "' onclick='onContentPlayBoxOntop_Click(" + divIdOntop + ",this)'>";
            strBuilder += "</div>";
            strBuilder += "<div class='col-sm-5 col-xs-5'>";
            strBuilder += "<img src='data:image/png;base64," + imagebase64 + "' class='img-responsive center-block' style='vertical-align:middle'> ";
            strBuilder += "</div>";
            strBuilder += "<div class='col-sm-6 col-xs-6'>";
            strBuilder += topup;
            strBuilder += "</div>";
            strBuilder += "</label></div>";

            return strBuilder;
        }

        public string GetOntopContentDetailV2(string sffPromotionCode)
        {
            string strBuilder = "";
            string imageHead = "";
            string detailHead = "";
            string imageDetail = "";
            string detail = "";
            var ContentPlaybox = GetLovConfigBytype("NEW_CONTENT_PLAYBOX");
            LovScreenValueModel newContentPlayboxImageHead = new LovScreenValueModel();
            newContentPlayboxImageHead = ContentPlaybox.Where(p => p.Type == "NEW_CONTENT_PLAYBOX" && p.DisplayValueJing == "DETAIL_PLABOX_PATH" && p.Name == sffPromotionCode).FirstOrDefault();
            LovScreenValueModel newContentPlayboxDetailHead = new LovScreenValueModel();
            newContentPlayboxDetailHead = ContentPlaybox.Where(p => p.Type == "NEW_CONTENT_PLAYBOX" && p.DisplayValueJing == "DETAIL_HEADER" && p.Name == sffPromotionCode).FirstOrDefault();
            LovScreenValueModel newContentPlayboxImage = new LovScreenValueModel();
            newContentPlayboxImage = ContentPlaybox.Where(p => p.Type == "NEW_CONTENT_PLAYBOX" && p.DisplayValueJing == "DETAIL_CONTENT_PATH" && p.Name == sffPromotionCode).FirstOrDefault();
            //LovScreenValueModel newContentPlayboxDetail = new LovScreenValueModel();
            //newContentPlayboxDetail = ContentPlaybox.Where(p => p.Type == "NEW_CONTENT_PLAYBOX" && p.DisplayValueJing == "DETAIL_CONTENT_DESCRIPTION" && p.Name == sffPromotionCode).FirstOrDefault();
            List<LovScreenValueModel> newContentPlayboxDetailList = ContentPlaybox.Where(p => p.Type == "NEW_CONTENT_PLAYBOX" && p.DisplayValueJing == "DETAIL_CONTENT_DESCRIPTION" && p.Name == sffPromotionCode).OrderBy(p => p.OrderByPDF ?? 0).ToList();

            imageHead = newContentPlayboxImageHead != null ? newContentPlayboxImageHead.Blob : "";
            detailHead = newContentPlayboxDetailHead != null ? newContentPlayboxDetailHead.DisplayValue : "";
            imageDetail = newContentPlayboxImage != null ? newContentPlayboxImage.Blob : "";
            //detail = newContentPlayboxDetail != null ? newContentPlayboxDetail.DisplayValue : "";
            var sb = new StringBuilder();
            foreach (var item in newContentPlayboxDetailList)
            {
                sb.Append(item.DisplayValue);
            }
            detail = sb.ToString();

            strBuilder += "<div class='col-xs-12'>";
            strBuilder += "<img src='data:image/png;base64," + imageHead + "' class='img-responsive pull-left'>";
            strBuilder += "</div>";
            strBuilder += "<p class='clearfix'></p>";
            strBuilder += "<i class='fa fa-chevron-right tgreen'></i>" + detailHead;
            strBuilder += "<p class='clearfix'></p>";
            strBuilder += "<div class='col-xs-12 text-center'>";
            if (imageDetail != "")
            {
                strBuilder += "<img src='data:image/png;base64," + imageDetail + "' class='img-responsive center-block'>";
            }
            strBuilder += "</div>";
            strBuilder += "<p class='clearfix'></p>";
            strBuilder += "<p class='clearfix'></p>";
            strBuilder += "<div class='col-xs-12'>";
            strBuilder += "<div class='add-rounded-gray'>";
            strBuilder += "<p class='clearfix'></p>";
            strBuilder += "<span>" + detail + "</span>";
            strBuilder += "<p class='clearfix'></p>";
            strBuilder += "</div>";
            strBuilder += "</div>";

            return strBuilder;
        }
        //IPcamera by First
        public string GetOntopContentDetailIPCAMERA(string sffPromotionCode)
        {
            string strBuilder = "";
            string imageHead = "";
            string detailHead = "";
            string imageDetail = "";
            string detail = "";
            var IPCameraLOV = GetScreenConfig("FBBTR003");

            var imageHeader = IPCameraLOV.Where(p => p.Name == "IPCamera_Detail_Picture").FirstOrDefault();
            imageHead = imageHeader.Blob;
            var detaillov = IPCameraLOV.Where(p => p.Name == "IPCamera_Detail_Info").ToList();

            //detail = detaillov.DisplayValue;

            var detailHeadlov = IPCameraLOV.Where(p => p.Name == "IPCamera_Detail_Topic").FirstOrDefault();
            detailHead = detailHeadlov.DisplayValue;

            strBuilder += "<div class='col-xs-12'>";
            strBuilder += "<img src='data:image/png;base64," + imageHead + "' width='50%' class='img-responsive pull-left'>";
            strBuilder += "</div>";
            strBuilder += "<p class='clearfix'></p>";
            strBuilder += "<i class='fa fa-chevron-right tgreen'></i>" + detailHead;
            strBuilder += "<p class='clearfix'></p>";
            strBuilder += "<div class='col-xs-12 text-center'>";
            if (imageDetail != "")
            {
                strBuilder += "<img src='data:image/png;base64," + imageDetail + "' class='img-responsive center-block'>";
            }
            strBuilder += "</div>";
            strBuilder += "<p class='clearfix'></p>";
            strBuilder += "<p class='clearfix'></p>";
            strBuilder += "<div class='col-xs-12'>";
            strBuilder += "<div class='add-rounded-gray'>";
            strBuilder += "<p class='clearfix'></p>";
            foreach (var detaillist in detaillov)
            {
                strBuilder += detaillist.DisplayValue;
            }
            //strBuilder += "<span>" + detail + "</span>";
            strBuilder += "<p class='clearfix'></p>";
            strBuilder += "</div>";
            strBuilder += "</div>";

            return strBuilder;
        }

        public JsonResult GetBuildingNoAll(string postcode, string build_name, string language)
        {
            var query = new GetBuildingNoAllQuery
            {
                Buildname = build_name,
                Postcode = "",
                Language = language,
            };

            var a = _queryProcessor.Execute(query);

            return Json(a, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveChangeStatusBuildingList(SaveChangeStatusBuildingModel dataList)
        {
            //R23.02_22022023
            InterfaceLogCommand log = null;
            log = StartInterface(dataList, "/process/SaveChangeStatusBuildingList", base.CurrentUser.UserName, "", "SaveChangeStatusBuildingList");

            SaveChangeStatusBuildingModel model = new SaveChangeStatusBuildingModel();
            List<SaveChangeStatusBuildingModel> saveResult = new List<SaveChangeStatusBuildingModel>();
            try
            {

                foreach (var d in dataList.SaveChangeStatusBuildingList)
                {

                    var reason = d.Reason.ToSafeString();
                    if (d.isSaveReason && reason.Count() > 300)
                    {
                        reason = d.Reason.Substring(300);
                    }

                    model = SaveChangeStatusBuilding(d.AddressId, d.ChkStatus, d.ChkFttrStatus, reason, d.isSaveReason);
                    if (model.ReturnCode == "X")
                    {
                        saveResult.Add(model);
                        return Json(saveResult, JsonRequestBehavior.AllowGet);
                    }

                    saveResult.Add(model);
                }

                #region call IM & SD
                var responseIM = callUpdateListBuildingVillageIM(dataList.SaveChangeStatusBuildingList);
                var responseSD = callUpdateListBuildingVillageSD(dataList.SaveChangeStatusBuildingList);

                saveResult.FirstOrDefault().isSuccessIM = responseIM.ResultCode == "0" && responseIM.ResultDesc == "Success" ? "Success" : "Failed";
                saveResult.FirstOrDefault().isSuccessSD = responseSD.resultCode == "0" && responseSD.resultDescription == "Success" ? "Success" : "Failed";
                #endregion call IM & SD

                EndInterface(saveResult, log, base.CurrentUser.UserName, "Success", "");

            }
            catch (Exception ex)
            {
                EndInterface("", log, base.CurrentUser.UserName, "Failed", ex.GetErrorMessage());
                throw ex;
            }
            return Json(saveResult, JsonRequestBehavior.AllowGet);
        }

        public SaveChangeStatusBuildingModel SaveChangeStatusBuilding(string addressID,
            string chkStatus,
            string chkFttrStatus,
            string reason = null,
            bool isSaveReason = false)
        {
            SaveChangeStatusBuildingModel model = new SaveChangeStatusBuildingModel();
            model.ReturnCode = "N";
            model.AddressId = addressID;

            if (null != base.CurrentUser)
            {
                SaveChangeStatusBuildingCommand command = new SaveChangeStatusBuildingCommand()
                {
                    ADDRESS_ID = addressID.ToSafeString(),
                    UPDATE_BY = base.CurrentUser.UserName.ToSafeString(),
                    ACTIVE_FLAG = chkStatus.ToSafeString(),
                    FTTR_FLAG = chkFttrStatus.ToSafeString().ToSafeString(),
                    REASON = reason.ToSafeString(),
                    IS_SAVE_REASON = isSaveReason
                };

                _saveChangeStatusBuildingCommand.Handle(command);

                if (command.Return_Code == 1)
                {
                    model.ReturnCode = "Y";
                    model.UpdateBy = command.UPDATE_BY;
                    model.UpdateDate = command.Return_UpdateDate;
                }

            }
            else
            {
                model.ReturnCode = "X";
            }

            return model;
        }

        public UpdateListBuildingVillageIMModel callUpdateListBuildingVillageIM(List<WBBEntity.PanelModels.SaveChangeStatusBuildingModel.StatusBuilding> dataList)
        {
            //R23.02_22022023 call api IM
            UpdateListBuildingVillageIMModel resultIM = new UpdateListBuildingVillageIMModel();

            try
            {
                List<ListBuildingVillageIM> listBuildingVillageIM = new List<ListBuildingVillageIM>();
                listBuildingVillageIM = dataList.ConvertAll(x => new ListBuildingVillageIM
                {
                    AddressID = x.AddressId,
                    ActiveFlag = x.ChkStatusText,
                    Remark = x.Reason
                });

                UpdateListBuildingVillageIMQuery updateIM = new UpdateListBuildingVillageIMQuery()
                {
                    buildingListIM = listBuildingVillageIM,
                    UpdateBy = base.CurrentUser.UserName
                };

                resultIM = _updateListBuildingVillageIMQuery.Handle(updateIM);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return resultIM;
        }

        public UpdateListBuildingVillageSDModel callUpdateListBuildingVillageSD(List<WBBEntity.PanelModels.SaveChangeStatusBuildingModel.StatusBuilding> dataList)
        {
            //R23.02_22022023 call api SD
            UpdateListBuildingVillageSDModel resultSD = new UpdateListBuildingVillageSDModel();

            try
            {
                List<ListBuildingVillageSD> listBuildingVillageSD = new List<ListBuildingVillageSD>();
                listBuildingVillageSD = dataList.ConvertAll(x => new ListBuildingVillageSD
                {
                    addressId = x.AddressId,
                    activeFlag = x.ChkStatusText,
                    reason = x.Reason
                });

                UpdateListBuildingVillageSDQuery updateSD = new UpdateListBuildingVillageSDQuery()
                {
                    buildingListSD = listBuildingVillageSD,
                    UpdateBy = base.CurrentUser.UserName
                };

                resultSD = _updateListBuildingVillageSDQuery.Handle(updateSD);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return resultSD;
        }


        [HttpPost]
        public JsonResult CheckFlagProcess(string Type, string SubType, string MobileType, string ServiceYearByDay, string Mobile)
        {
            string FullUrl = "";
            if (Session["FullUrl"] != null)
                FullUrl = Session["FullUrl"].ToSafeString();

            CheckFlagProcessQuery query = new CheckFlagProcessQuery()
            {
                P_TYPE = Type,
                P_SUB_TYPE = SubType,
                P_MOBILE_TYPE = MobileType,
                P_SERVICE_YEAR_BY_DAY = ServiceYearByDay,
                TRANSACTION_ID = Mobile,
                FULL_URL = FullUrl
            };

            CheckFlagProcessModel result = _queryProcessor.Execute(query);
            CheckFlagProcessData checkFlagProcessData = new CheckFlagProcessData();

            if (result != null)
            {
                if (result.RETURN_CODE == "0")
                {
                    checkFlagProcessData = result.checkFlagProcessData;
                }
            }

            return Json(checkFlagProcessData, JsonRequestBehavior.AllowGet);

        }

        public string PermissionCondoUpdateStatus(string userName)
        {
            string result = "N";

            GetPermissionCondoUpdateStatusQuery query = new GetPermissionCondoUpdateStatusQuery()
            {
                UserName = userName
            };

            result = _queryProcessor.Execute(query);

            return result;
        }

        [HttpPost]
        public JsonResult DisposeCache()
        {
            try
            {
                foreach (DictionaryEntry entry in HttpRuntime.Cache)
                {
                    HttpRuntime.Cache.Remove((string)entry.Key);
                }

                return Json("OK", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult Getdropdowndeveloper()
        {
            var query = new GetDeveloperQuery { };
            var result = _queryProcessor.Execute(query);

            List<DropdownModel> sDrop = new List<DropdownModel>();
            if (SiteSession.CurrentUICulture.IsThaiCulture())
            {
                var th = from r in result.P_RES_DATA
                         select new DropdownModel() { Value = string.IsNullOrEmpty(r.LOV_VAL_TH) ? "" : r.LOV_VAL_TH, Text = string.IsNullOrEmpty(r.LOV_VAL_TH) ? "&nbsp;" : r.LOV_VAL_TH };
                sDrop = th.ToList();
            }
            else
            {
                var en = from r in result.P_RES_DATA
                         select new DropdownModel() { Value = string.IsNullOrEmpty(r.LOV_VAL_EN) ? "" : r.LOV_VAL_EN, Text = string.IsNullOrEmpty(r.LOV_VAL_EN) ? "&nbsp;" : r.LOV_VAL_EN };
                sDrop = en.ToList();
            }

            return Json(sDrop, JsonRequestBehavior.AllowGet);
        }

        public List<LovScreenValueModel> GetListGroupMailSpecialAccount()
        {
            try
            {
                List<GroupMailSpecialAccount> config = null;

                GetListGroupMailSpecialAccountModel executeResults = new GetListGroupMailSpecialAccountModel();
                var query = new GetListGroupMailSpecialAccountQuery { };
                var result = _queryProcessor.Execute(query);

                var screenValue = new List<LovScreenValueModel>();

                if (result.RETURN_CODE == "0")
                {
                    if (SiteSession.CurrentUICulture.IsThaiCulture())
                    {
                        screenValue = result.LIST_MAIL_SPECIAL_ACCOUNT.Select(l => new LovScreenValueModel
                        {
                            DisplayValue = l.LOV_VAL1
                        }).ToList();
                    }
                    else
                    {
                        screenValue = result.LIST_MAIL_SPECIAL_ACCOUNT.Select(l => new LovScreenValueModel
                        {
                            DisplayValue = l.LOV_VAL2
                        }).ToList();
                    }
                    return screenValue;
                }
                else return new List<LovScreenValueModel>();
            }
            catch (Exception ex)
            {
                Logger.Info(ex.GetErrorMessage());
                return new List<LovScreenValueModel>();
            }
        }

        [HttpGet]
        public ActionResult TestPostAIChatBot()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AIChatBotBypass(string Data)
        {
            QuickWinPanelModel model = null;
            var TransactionID = "AIChatBotBypass";
            var logStatus = "ERROR";
            var logOutput = "";
            var log = StartInterface(Data, "/process/AIChatBotBypass", TransactionID, "", "ESRI");

            try
            {
                Session["AIChatBotBypass"] = null;
                base.Logger.Info("AIChatBotBypass Start");

                var bc = Request.Browser;
                if (TempData["ProcessLineOffier"] != null) //Set data from Fbbsaleportal/ProcessLineOfficer_IM for bypass order of IM.
                {
                    LeaveMessageDataModel data = TempData["ProcessLineOffier"] as LeaveMessageDataModel;
                    TempData["ProcessLineOffier"] = null;

                    return ProcessLineOfficer(LcCode: data.LC_CODE, outType: data.OUT_TYPE, outSubType: data.OUT_SUBTYPE, leaveMessageDataModel: data);
                }

                if (Session["OfficerModel"] == null)
                {
                    Session["FullUrl"] = this.Url.Action("Index", "Process", null, this.Request.Url.Scheme);
                }

                if (!string.IsNullOrEmpty(Data))
                {
                    var dataDecrypt = Decrypt(Data, "");
                    if (!string.IsNullOrEmpty(dataDecrypt))
                    {
                        logOutput = dataDecrypt;
                        var channel = "";
                        var mobileNo = "";
                        var chargeType = "";
                        var cardType = "";
                        var cardNo = "";
                        var language = "";

                        string[] DataTemps = dataDecrypt.Split('&');

                        if (DataTemps.Length > 0)
                        {
                            foreach (var item in DataTemps)
                            {
                                string[] DataTemp = item.Split('=');
                                if (DataTemp != null && DataTemp.Count() == 2)
                                {
                                    if (DataTemp[0].ToSafeString() == "channel")
                                    {
                                        channel = DataTemp[1].ToSafeString();
                                    }
                                    if (DataTemp[0].ToSafeString() == "mobileNo")
                                    {
                                        mobileNo = DataTemp[1].ToSafeString();
                                    }
                                    if (DataTemp[0].ToSafeString() == "chargeType")
                                    {
                                        chargeType = DataTemp[1].ToSafeString();
                                    }
                                    if (DataTemp[0].ToSafeString() == "cardType")
                                    {
                                        cardType = DataTemp[1].ToSafeString();
                                    }
                                    if (DataTemp[0].ToSafeString() == "cardNo")
                                    {
                                        cardNo = DataTemp[1].ToSafeString();
                                    }
                                    if (DataTemp[0].ToSafeString() == "language")
                                    {
                                        language = DataTemp[1].ToSafeString();
                                    }
                                }
                            }
                        }

                        model = new QuickWinPanelModel
                        {
                            CoveragePanelModel = new CoveragePanelModel
                            {
                                P_MOBILE = mobileNo
                            },
                            CustomerRegisterPanelModel = new CustomerRegisterPanelModel
                            {
                                RegisterChannelSaveOrder = channel,
                                SubNetworkType = chargeType
                            },
                            IDCardType = cardType,
                            IDCardNo = cardNo,
                            languageCulture = (language ?? string.Empty).Equals("TH") ? 1 : 2,
                            AIChatBotBypass_Channel = channel,
                            AIChatBotBypass_Flag = "Y"
                        };

                        ChangeCurrentCulture(model.languageCulture);
                        logStatus = "Success";
                        TempData["tempAIChatBotBypass"] = model;
                    }
                }
            }
            catch (Exception ex)
            {
                base.Logger.Info("AIChatBotBypass Exception : " + ex.GetErrorMessage());
                logStatus = "ERROR";
                logOutput = ex.GetErrorMessage();
            }
            finally
            {
                base.Logger.Info("AIChatBotBypass End");
                EndInterface(logOutput, log, TransactionID, logStatus, "");
            }

            return RedirectToAction("Index", "Process");
        }

        [HttpGet]
        public ActionResult AIChatBotBypass(string Data, string Value = "")
        {
            QuickWinPanelModel model = null;
            var TransactionID = "AIChatBotBypass";
            var logStatus = "ERROR";
            var logOutput = "";
            var log = StartInterface(Data, "/process/AIChatBotBypass", TransactionID, "", "ESRI");

            try
            {
                Session["AIChatBotBypass"] = null;
                base.Logger.Info("AIChatBotBypass Start");

                var bc = Request.Browser;
                if (TempData["ProcessLineOffier"] != null) //Set data from Fbbsaleportal/ProcessLineOfficer_IM for bypass order of IM.
                {
                    LeaveMessageDataModel data = TempData["ProcessLineOffier"] as LeaveMessageDataModel;
                    TempData["ProcessLineOffier"] = null;

                    return ProcessLineOfficer(LcCode: data.LC_CODE, outType: data.OUT_TYPE, outSubType: data.OUT_SUBTYPE, leaveMessageDataModel: data);
                }

                if (Session["OfficerModel"] == null)
                {
                    Session["FullUrl"] = this.Url.Action("Index", "Process", null, this.Request.Url.Scheme);
                }

                if (!string.IsNullOrEmpty(Data))
                {
                    var dataDecrypt = Decrypt(Data, "");
                    if (!string.IsNullOrEmpty(dataDecrypt))
                    {
                        logOutput = dataDecrypt;
                        var channel = "";
                        var mobileNo = "";
                        var chargeType = "";
                        var cardType = "";
                        var cardNo = "";
                        var language = "";

                        string[] DataTemps = dataDecrypt.Split('&');

                        if (DataTemps.Length > 0)
                        {
                            foreach (var item in DataTemps)
                            {
                                string[] DataTemp = item.Split('=');
                                if (DataTemp != null && DataTemp.Count() == 2)
                                {
                                    if (DataTemp[0].ToSafeString() == "channel")
                                    {
                                        channel = DataTemp[1].ToSafeString();
                                    }
                                    if (DataTemp[0].ToSafeString() == "mobileNo")
                                    {
                                        mobileNo = DataTemp[1].ToSafeString();
                                    }
                                    if (DataTemp[0].ToSafeString() == "chargeType")
                                    {
                                        chargeType = DataTemp[1].ToSafeString();
                                    }
                                    if (DataTemp[0].ToSafeString() == "cardType")
                                    {
                                        cardType = DataTemp[1].ToSafeString();
                                    }
                                    if (DataTemp[0].ToSafeString() == "cardNo")
                                    {
                                        cardNo = DataTemp[1].ToSafeString();
                                    }
                                    if (DataTemp[0].ToSafeString() == "language")
                                    {
                                        language = DataTemp[1].ToSafeString();
                                    }
                                }
                            }
                        }

                        model = new QuickWinPanelModel
                        {
                            CoveragePanelModel = new CoveragePanelModel
                            {
                                P_MOBILE = mobileNo
                            },
                            CustomerRegisterPanelModel = new CustomerRegisterPanelModel
                            {
                                RegisterChannelSaveOrder = channel,
                                SubNetworkType = chargeType
                            },
                            IDCardType = cardType,
                            IDCardNo = cardNo,
                            languageCulture = (language ?? string.Empty).Equals("TH") ? 1 : 2,
                            AIChatBotBypass_Channel = channel,
                            AIChatBotBypass_Flag = "Y"
                        };

                        ChangeCurrentCulture(model.languageCulture);
                        logStatus = "Success";
                        TempData["tempAIChatBotBypass"] = model;
                    }
                }
            }
            catch (Exception ex)
            {
                base.Logger.Info("AIChatBotBypass Exception : " + ex.GetErrorMessage());
                logStatus = "ERROR";
                logOutput = ex.GetErrorMessage();
            }
            finally
            {
                base.Logger.Info("AIChatBotBypass End");
                EndInterface(logOutput, log, TransactionID, logStatus, "");
            }

            return RedirectToAction("Index", "Process");
        }

        [HttpPost]
        public string AIChatBotEncrypt(string Data)
        {
            var response = "";
            try
            {
                if (!string.IsNullOrEmpty(Data))
                {
                    response = Encrypt(Data, "");
                }
            }
            catch (Exception ex)
            {
                response = "AIChatBotEncrypt Exception : " + ex.GetErrorMessage();
            }
            return response;
        }

        [HttpPost]
        public ActionResult StaffPrivilege(string Data)
        {
            QuickWinPanelModel model = null;

            var bc = Request.Browser;

            if (!string.IsNullOrEmpty(Data))
            {
                var dataDecrypt = Decrypt(Data, "");
                if (!string.IsNullOrEmpty(dataDecrypt))
                {
                    string transactionID = "";
                    string channel = "";
                    string language = "";
                    string cardType = "";
                    string cardNo = "";


                    string[] DataTemps = dataDecrypt.Split('&');

                    if (DataTemps.Length > 0)
                    {
                        foreach (var item in DataTemps)
                        {
                            string[] DataTemp = item.Split('=');
                            if (DataTemp != null && DataTemp.Count() == 2)
                            {
                                if (DataTemp[0].ToSafeString().ToUpper() == "LANGUAGE")
                                {
                                    language = DataTemp[1].ToSafeString();
                                }
                                if (DataTemp[0].ToSafeString().ToUpper() == "TRANSACTION_STAFF")
                                {
                                    transactionID = DataTemp[1].ToSafeString();
                                }
                                if (DataTemp[0].ToSafeString().ToUpper() == "CHANNEL")
                                {
                                    channel = DataTemp[1].ToSafeString();
                                }
                                if (DataTemp[0].ToSafeString().ToUpper() == "CARDTYPE")
                                {
                                    cardType = DataTemp[1].ToSafeString();
                                }
                                if (DataTemp[0].ToSafeString().ToUpper() == "CARDNO")
                                {
                                    cardNo = DataTemp[1].ToSafeString();
                                }
                            }
                        }

                        model = new QuickWinPanelModel
                        {
                            CustomerRegisterPanelModel = new CustomerRegisterPanelModel
                            {
                                L_MOBILE = "0000000000"
                            },
                            CoveragePanelModel = new CoveragePanelModel
                            {
                                P_MOBILE = "0000000000"
                            },
                            IDCardType = cardType,
                            IDCardNo = cardNo,
                            languageCulture = (language ?? string.Empty).Equals("TH") ? 1 : 2,
                            StaffPrivilegeBypass_TransactionID = transactionID,
                            StaffPrivilegeBypass_Channel = channel,
                            StaffPrivilegeBypass_Flag = "Y"
                        };
                        ChangeCurrentCulture(model.languageCulture);
                    }
                }
            }
            return SearchProfileIndex(model);
        }

        [HttpPost]
        public JsonResult APIMicrosite()
        {
            //R21.07 APIMicrosite 
            string TRANSACTION_ID = "";
            string APP_SOURCE = "";
            string APP_DESTINATION = "";
            string CONTENT_TYPE = "";
            string BODY_JSON = "";

            var re = Request;

            //Body
            Stream req = re.InputStream;
            req.Seek(0, System.IO.SeekOrigin.Begin);
            BODY_JSON = new StreamReader(req).ReadToEnd();
            if (!string.IsNullOrEmpty(BODY_JSON))
            {
                //Call API
                var query = new GetAPIMicrositeQuery
                {
                    Transaction_Id = TRANSACTION_ID,
                    App_Source = APP_SOURCE,
                    App_Destination = APP_DESTINATION,
                    Content_Type = CONTENT_TYPE,
                    BodyJson = BODY_JSON
                };
                var result = _queryProcessor.Execute(query);

                return Json(new { RESULT_CODE = result.RESULT_CODE, RESULT_DESC = result.RESULT_DESC, TRANSACTIONID = result.TRANSACTIONID }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { RESULT_CODE = "-1", RESULT_DESC = "Error : Request Body", TRANSACTIONID = "" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult APISendmail(GetAPISendmailQuery Data)
        {
            //R22.07 APISendmail

            if (Data != null)
            {
                string result_code = "-1";
                string result_desc = "Error : Request Field ";

                if (string.IsNullOrEmpty(Data.TransactionID))
                {
                    result_desc += "TransactionID.";
                }
                else if (string.IsNullOrEmpty(Data.firstname))
                {
                    result_desc += "firstname.";
                }
                else if (string.IsNullOrEmpty(Data.lastname))
                {
                    result_desc += "lastname.";
                }
                else if (string.IsNullOrEmpty(Data.telephone))
                {
                    result_desc += "telephone.";
                }
                else
                {
                    //API Send mail
                    var query = new GetAPISendmailQuery
                    {
                        TransactionID = Data.TransactionID,
                        firstname = Data.firstname,
                        lastname = Data.lastname,
                        telephone = Data.telephone,
                        email = Data.email,
                        corporate_name = Data.corporate_name,
                        message = Data.message
                    };

                    var result = _queryProcessor.Execute(query);
                    result_code = result.RESULT_CODE;
                    result_desc = result.RETURN_MESSAGE;
                }
                return Json(new { RESULT_CODE = result_code, RESULT_DESC = result_desc }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { RESULT_CODE = "-1", RESULT_DESC = "Error : Request Data" }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetContractDeviceNew(string AisAirNumber, string promotionCodeMain, string meshFlag, string p_duration)
        {
            //R21.08 Contract Device
            GetContractDeviceNewModel result = new GetContractDeviceNewModel();
            GetContractDeviceNewModel resultCon = new GetContractDeviceNewModel();
            GetListQueryConfigContractModel resultWS = new GetListQueryConfigContractModel();

            try
            {
                var queryCon = new GetContractDeviceNewQuery()
                {
                    TransactionId = AisAirNumber,
                    P_PROMOTION_CODE_MAIN = promotionCodeMain,
                    P_MESH_FLAG = meshFlag,
                    P_DURATION = p_duration
                };
                resultCon = _queryProcessor.Execute(queryCon);

                if (resultCon.RETURN_CODE == "1")
                {
                    result = resultCon;
                }
                else if (resultCon.RETURN_CODE == "2")
                {
                    var contractId = resultCon.LIST_MASTER[0].P_CONTRACT_ID;
                    var contractName = resultCon.LIST_MASTER[0].P_CONTRACT_NAME;

                    //Call WS ListQueryConfigContract
                    var queryWS = new GetListQueryConfigContractQuery()
                    {
                        TransactionId = AisAirNumber,
                        contract_id = contractId,
                        contract_name = contractName
                    };
                    resultWS = _queryProcessor.Execute(queryWS);

                    if (resultWS.resultCode == "20000" || resultWS.resultCode == "50000")
                    {
                        var contractId_tmp = "";
                        var contractName_tmp = "";
                        var contractType_tmp = "";
                        var contractRuleId_tmp = "";
                        var penaltyType_tmp = "";
                        var penaltyId_tmp = "";
                        var limitContract_tmp = "";
                        if (resultWS.listConfigurationContract != null)
                        {
                            contractId_tmp = resultWS.listConfigurationContract[0].contractId;
                            contractName_tmp = resultWS.listConfigurationContract[0].contractName;
                            contractType_tmp = resultWS.listConfigurationContract[0].contractType;
                            contractRuleId_tmp = resultWS.listConfigurationContract[0].contractRuleId;

                            if (resultWS.listConfigurationContract[0].listPenPenaltyFeeBean.Count > 0)
                            {
                                penaltyType_tmp = resultWS.listConfigurationContract[0].listPenPenaltyFeeBean[0].penaltyChargeGroup;
                            }

                            penaltyId_tmp = resultWS.listConfigurationContract[0].penaltyId;
                            limitContract_tmp = resultWS.listConfigurationContract[0].limitContract;
                        }

                        //Insert TDM
                        var TDMcommamd = new InsertMasterTDMContractDeviceCommand
                        {
                            Transaction_Id = AisAirNumber,
                            P_RESULT_CODE_TDM = resultWS.resultCode,
                            P_CONTRACT_ID = contractId_tmp,
                            P_CONTRACT_NAME = contractName_tmp,
                            P_CONTRACT_TYPE = contractType_tmp,
                            P_CONTRACT_RULE_ID = contractRuleId_tmp,
                            P_PENALTY_TYPE = penaltyType_tmp,
                            P_PENALTY_ID = penaltyId_tmp,
                            P_LIMIT_CONTRACT = limitContract_tmp,
                            P_DURATION = resultCon.LIST_MASTER[0].P_DURATION
                        };
                        _insertMasterTDMContractDeviceCommand.Handle(TDMcommamd);

                        if (Convert.ToInt16(TDMcommamd.RETURN_CODE) > -1)
                        {
                            List<ContractDeviceNewModel> tmp_list_master = new List<ContractDeviceNewModel>();
                            tmp_list_master = TDMcommamd.LIST_PARA_SAVE.Select(t =>
                                                new ContractDeviceNewModel()
                                                {
                                                    P_CONTRACT_ID = t.P_CONTRACT_ID.ToSafeString(),
                                                    P_CONTRACT_NAME = t.P_CONTRACT_NAME.ToSafeString(),
                                                    P_CONTRACT_RULE_ID = t.P_CONTRACT_RULE_ID.ToSafeString(),
                                                    P_PENALTY_TYPE = t.P_PENALTY_TYPE.ToSafeString(),
                                                    P_PENALTY_ID = t.P_PENALTY_ID.ToSafeString(),
                                                    P_CONTRACT_FLAG = t.P_CONTRACT_FLAG.ToSafeString(),
                                                    P_DURATION = t.P_DURATION.ToSafeString(),
                                                }).ToList();

                            GetContractDeviceNewModel resultCallWS = new GetContractDeviceNewModel
                            {
                                RETURN_CODE = TDMcommamd.RETURN_CODE.ToSafeString(),
                                RETURN_MESSAGE = TDMcommamd.RETURN_MESSAGE.ToSafeString(),
                                LIST_MASTER = tmp_list_master
                            };

                            result = resultCallWS;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private string GetCheckPendingOrderFbssFlag(string interetNo, string url)
        {
            //20.7 change promotion - change device
            var pendingOrderFbssFlag = string.Empty;
            try
            {

                var CheckPendingFlag = "";

                List<LovValueModel> CheckPendingOrderFbssFlag = base.LovData.Where(l => l.Name == "CHECK_PENDING_ORDER_FLAG" && l.Type == "CONFIG").ToList();

                if (CheckPendingOrderFbssFlag.Any())
                {
                    CheckPendingFlag = CheckPendingOrderFbssFlag.Select(i => i.LovValue1).FirstOrDefault();
                }

                if (CheckPendingFlag == "Y")
                {
                    var query = new CheckPendingOrderFbssQuery
                    {
                        InteretNo = interetNo.ToSafeString(),
                        FullUrl = url,
                    };
                    var result = _queryProcessor.Execute(query);
                    if (result != null)
                    {
                        pendingOrderFbssFlag = result.PendingOrderFbss_Flag.ToSafeString();
                    }
                }
            }
            catch (Exception ex)
            {
                pendingOrderFbssFlag = string.Empty;
                Logger.Info("Process/GetCheckPendingOrderFbssFlag Error : ", ex.GetBaseException());
            }
            return pendingOrderFbssFlag;
        }

        //20.8 Add by Aware : Atipon -> Add Method for get Expire Time QR Code
        public string GetExpireTimeQRPayment(string ProductName, string ServiceName)
        {
            var expireTimeQRPayment = "";

            try
            {
                GetConfigReqPaymentQuery getConfigReqPaymentQuery = new GetConfigReqPaymentQuery()
                {
                    p_product_name = ProductName,
                    p_service_name = ServiceName
                };

                GetConfigReqPaymentModel getConfigReqPaymentData = _queryProcessor.Execute(getConfigReqPaymentQuery);

                if (getConfigReqPaymentData.ret_code == "0" && getConfigReqPaymentData.list_config_req_payment != null && getConfigReqPaymentData.list_config_req_payment.Count > 0)
                {
                    List<ConfigReqPaymentData> configReqPaymentDatas = getConfigReqPaymentData.list_config_req_payment;

                    expireTimeQRPayment = (configReqPaymentDatas.FirstOrDefault(item => item.attr_name == "expire_time_seconds") ?? new ConfigReqPaymentData()).attr_value;
                }

                var expireTime = !string.IsNullOrEmpty(expireTimeQRPayment) ? expireTimeQRPayment.ToSafeInteger() : 1800;

                expireTime = expireTime / 60 >= 1 ? expireTime / 60 : 1;

                expireTimeQRPayment = expireTime.ToString();
            }
            catch (Exception ex)
            {
                Logger.Info("Error GetExpireTimeQRPayment : " + ex.Message);

                expireTimeQRPayment = "30";
            }

            return expireTimeQRPayment;
        }

        public bool checkSessionOfficerModel(QuickWinPanelModel model)
        {
            bool chkValue = false;
            string lcCode, ascCode, empId;

            try { lcCode = model?.CustomerRegisterPanelModel?.L_LOC_CODE.ToString(); } catch { lcCode = null; }
            try { ascCode = model?.CustomerRegisterPanelModel?.L_ASC_CODE.ToString(); } catch { ascCode = null; }
            try { empId = model?.CustomerRegisterPanelModel?.L_STAFF_ID.ToString(); } catch { empId = null; }

            if (string.IsNullOrEmpty(lcCode) != true
                || string.IsNullOrEmpty(ascCode) != true
                || string.IsNullOrEmpty(empId) != true)
            {
                if (Session["OfficerModel"] == null)
                {
                    chkValue = true;
                }
            }
            return chkValue;
        }

        public bool checkSessionOfficer()
        {
            bool chkValue = false;
            string lcCode = Request["LcCode"];
            string ascCode = Request["ASCCode"];
            string empId = Request["EmployeeID"];
            if (string.IsNullOrEmpty(lcCode) != true
                || string.IsNullOrEmpty(ascCode) != true
                || string.IsNullOrEmpty(empId) != true)
            {
                if (Session["OfficerModel"] == null)
                {
                    chkValue = true;
                }
            }
            return chkValue;
        }
        private string InsertSaveOrderNew911Test()
        {
            InsertSaveOrderNew911Command command = new InsertSaveOrderNew911Command();
            command.p_customer_type = "R";
            command.p_customer_subtype = "T";
            command.p_title_code = "127";
            command.p_first_name = "ทดสอบโคด1";
            command.p_last_name = "ทดสอบโคดS";
            command.p_contact_title_code = "127";
            command.p_contact_first_name = "ทดสอบโคด1";
            command.p_contact_last_name = "ทดสอบโคดS";
            command.p_id_card_type_desc = "บัตรประชาชน";
            command.p_id_card_no = "5418574945045";
            command.p_tax_id = "";
            command.p_gender = "หญิง";
            command.p_birth_date = "04/03/1956";
            command.p_mobile_no = "0962954015";
            command.p_mobile_no_2 = "";
            command.p_home_phone_no = "";
            command.p_email_address = "";
            command.p_contact_time = "08:00 - 17:00 น.";
            command.p_nationality_desc = "THAI";
            command.p_customer_remark = "";
            command.p_house_no = "1291/1";
            command.p_moo_no = "";
            command.p_building = "";
            command.p_floor = "";
            command.p_room = "";
            command.p_mooban = "";
            command.p_soi = "ซอยพหลโยธิน 9";
            command.p_road = "";
            command.p_zipcode_rowid = "926384188F362846E044000B5DE06DF2";
            command.p_latitude = "13.78645300";
            command.p_longtitude = "100.54672200";
            command.p_asc_code = "";
            command.p_employee_id = "";
            command.p_location_code = "";
            command.p_sale_represent = "";
            command.p_cs_note = "";
            command.p_wifi_access_point = "Y";
            command.p_install_status = "N";
            command.p_coverage = "";
            command.p_existing_airnet_no = "";
            command.p_gsm_mobile_no = "";
            command.p_contact_name_1 = "ทดสอบโคด1";
            command.p_contact_name_2 = "ทดสอบโคด2";
            command.p_contact_mobile_no_1 = "0962954015";
            command.p_contact_mobile_no_2 = "";
            command.p_condo_floor = "";
            command.p_condo_roof_top = "N";
            command.p_condo_balcony = "N";
            command.p_balcony_north = "N";
            command.p_balcony_south = "N";
            command.p_balcony_east = "N";
            command.p_balcony_wast = "N";
            command.p_high_building = "N";
            command.p_high_tree = "N";
            command.p_billboard = "N";
            command.p_expressway = "N";
            command.p_address_type_wire = "บ้านเดียว";
            command.p_address_type = "บ้านเดียว";
            command.p_floor_no = "";
            command.p_house_no_bl = "1291/1";
            command.p_moo_no_bl = "";
            command.p_building_bl = "";
            command.p_floor_bl = "";
            command.p_room_bl = "";
            command.p_mooban_bl = "";
            command.p_soi_bl = "ซอยพหลโยธิน 9";
            command.p_road_bl = "";
            command.p_zipcode_rowid_bl = "926384188F362846E044000B5DE06DF2";
            command.p_house_no_vt = "";
            command.p_moo_no_vt = "";
            command.p_building_vt = "";
            command.p_floor_vt = "";
            command.p_room_vt = "";
            command.p_mooban_vt = "";
            command.p_soi_vt = "";
            command.p_road_vt = "";
            command.p_zipcode_rowid_vt = "";
            command.p_cvr_id = "";
            command.p_cvr_node = "";
            command.p_cvr_tower = "";
            command.p_site_code = "WTTX";
            command.p_relate_mobile = "";
            command.p_relate_non_mobile = "8850155733";
            command.p_sff_ca_no = "";
            command.p_sff_sa_no = "";
            command.p_sff_ba_no = "";
            command.p_network_type = "";
            command.p_service_day = "0";
            command.p_expect_install_date = "-";
            command.p_fttx_vendor = "AWN";
            command.p_install_note = "";
            command.p_phone_flag = "";
            command.p_time_Slot = "";
            command.p_installation_Capacity = "";
            command.p_address_Id = "9999999";
            command.p_access_Mode = "WTTx";
            command.p_eng_flag = "N";
            command.p_event_code = "";
            command.p_installAddress1 = "";
            command.p_installAddress2 = "";
            command.p_installAddress3 = "";
            command.p_installAddress4 = "";
            command.p_installAddress5 = "";
            command.p_pbox_count = "";
            command.p_convergence_flag = "";
            command.p_time_slot_id = "";
            command.p_gift_voucher = "";
            command.p_sub_location_id = "";
            command.p_sub_contract_name = "";
            command.p_install_staff_id = "";
            command.p_install_staff_name = "";
            command.p_flow_flag = "";
            command.p_line_id = "";
            command.p_relate_project_name = "";
            command.p_plug_and_play_flag = "";
            command.p_reserved_id = "";
            command.p_job_order_type = "N";
            command.p_assign_rule = "PUSH";
            command.p_old_isp = "";
            command.p_splitter_flag = "";
            command.p_reserved_port_id = "";
            command.p_special_remark = "";
            command.p_order_no = "20250508162432657523";
            command.p_source_system = "";
            command.p_bill_media = "SMS and eBill";
            command.p_pre_order_no = "";
            command.p_voucher_desc = "";
            command.p_campaign_project_name = "";
            command.p_pre_order_chanel = "";
            command.p_rental_flag = "N";
            command.p_dev_project_code = "";
            command.p_dev_bill_to = "";
            command.p_dev_po_no = "";
            command.p_partner_type = "";
            command.p_partner_subtype = "";
            command.p_mobile_by_asc = "";
            command.p_location_name = "";
            command.p_paymentMethod = "";
            command.p_transactionId_in = "20250508162432657523";
            command.p_transactionId = "";
            command.p_sub_access_mode = "";
            command.p_request_sub_flag = "";
            command.p_premium_flag = "N";
            command.p_relate_mobile_segment = "Non AIS";
            command.p_ref_ur_no = "";
            command.p_location_email_by_region = "";
            command.p_sale_staff_name = "";
            command.p_dopa_flag = "N";
            command.p_service_year = "0";
            command.p_require_cs_verify_doc = "N";
            command.p_facerecog_flag = "N";
            command.p_special_account_name = "";
            command.p_special_account_no = "";
            command.p_special_account_enddate = "";
            command.p_special_account_group_email = "";
            command.p_special_account_flag = "";
            command.p_existing_mobile_flag = "";
            command.p_pre_survey_date = "";
            command.p_pre_survey_timeslot = "";
            command.p_register_channel = "FBBWF";
            command.p_auto_create_prospect_flag = "Y";
            command.p_order_verify = "";
            command.p_waiting_install_date = "";
            command.p_waiting_time_slot = "";
            command.p_sale_channel = "CUSTOMER";
            command.p_owner_product = "WTTx";
            command.p_package_for = "PUBLIC";
            command.p_sff_promotion_code = "";
            command.p_region = "BKK";
            command.p_province = "กรุงเทพ";
            command.p_district = "พญาไท";
            command.p_sub_district = "พญาไท";
            command.p_serenade_flag = "";
            command.p_fmpa_flag = "";
            command.p_cvm_flag = "";
            command.p_order_relate_change_pro = "";
            command.p_company_name = "";
            command.p_distribution_channel = "";
            command.p_channel_sales_group = "";
            command.p_shop_type = "";
            command.p_shop_segment = "";
            command.p_asc_name = "";
            command.p_asc_member_category = "";
            command.p_asc_position = "";
            command.p_location_region = "";
            command.p_location_sub_region = "";
            command.p_employee_name = "";
            command.p_service_level = "L";
            command.p_amendment_flag = "";
            command.p_customerpurge = "";
            command.p_exceptentryfee = "";
            command.p_secondinstallation = "";
            command.p_first_install_date = "";
            command.p_first_time_slot = "";
            command.p_line_temp_id = "WEB_20250508162329270029";
            command.p_fmc_special_flag = "";
            command.p_non_res_flag = "";
            command.p_criteria_mobile = "";
            command.p_remark_for_subcontract = "";
            command.p_mesh_count = "";
            command.p_online_flag = "";
            command.p_privilege_points = "";
            command.p_transaction_privilege_id = "";
            command.p_special_skill = "";
            command.p_tdm_contract_id = "";
            command.p_tdm_rule_id = "";
            command.p_tdm_penalty_id = "";
            command.p_tdm_penalty_group_id = "";
            command.p_duration = "";
            command.p_contract_flag = "";
            var airregists = new List<AirRegistPackage>();
            airregists.Add(new AirRegistPackage()
            {
                temp_ia = "00001",
                product_subtype = "WTTx",
                package_type = "1",
                package_code = "P200401180",
                package_price = 0,
                idd_flag = "",
                fax_flag = "",
                home_ip = "",
                home_port = "",
                mobile_forward = "",
                pbox_ext = ""

            });
            airregists.Add(new AirRegistPackage()
            {
                temp_ia = "00001",
                product_subtype = "WTTx",
                package_type = "2",
                package_code = "P200128064",
                package_price = 0,
                idd_flag = "",
                fax_flag = "",
                home_ip = "",
                home_port = "",
                mobile_forward = "",
                pbox_ext = ""

            });
            airregists.Add(new AirRegistPackage()
            {
                temp_ia = "00001",
                product_subtype = "WTTx",
                package_type = "2",
                package_code = "P200128063",
                package_price = 0,
                idd_flag = "",
                fax_flag = "",
                home_ip = "",
                home_port = "",
                mobile_forward = "",
                pbox_ext = ""

            });
            airregists.Add(new AirRegistPackage()
            {
                temp_ia = "00001",
                product_subtype = "WTTx",
                package_type = "4",
                package_code = "P19075809",
                package_price = 0,
                idd_flag = "",
                fax_flag = "",
                home_ip = "",
                home_port = "",
                mobile_forward = "",
                pbox_ext = ""
            });
            airregists.Add(new AirRegistPackage()
            {
                temp_ia = "00001",
                product_subtype = "WTTx",
                package_type = "6",
                package_code = "P19075866",
                package_price = 0,
                idd_flag = "",
                fax_flag = "",
                home_ip = "",
                home_port = "",
                mobile_forward = "",
                pbox_ext = ""
            });
            var airImage = new List<AirRegistFile>();
            airImage.Add(new AirRegistFile() { 
                file_name = "\\\\10.137.32.9\\fbb_idcard_ndev001b\\202505\\FBB_01_5045_2025050816240182569101.jpg"
            });

            var airSplitter = new List<AirRegistSplitter>();
            //airSplitter.Add(new AirRegistSplitter());

            var airCPE = new List<AirRegistCPESerial>();
            //airCPE.Add(new AirRegistCPESerial());

            var custInsightRecord = new List<AirRegistCustInsi>();
            //custInsightRecord.Add(new AirRegistCustInsi());

            var dcontract = new List<AirRegistDcontract>();
            //contract.Add(new AirRegistDcontract());

            command.p_air_regist_package_array = airregists;
            command.p_air_regist_file_array = airImage;
            command.p_air_regist_splitter_array = airSplitter;
            command.p_air_regist_cpe_serial_array = airCPE;
            command.p_air_regist_cust_insi_array = custInsightRecord;
            command.p_air_regist_dcontract_array = dcontract;

            try
            {
                _insertSaveOrderNew911Command.Handle(command);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return command.o_return_order_no.ToSafeString();
        }

        private string InsertSaveOrderNew911(QuickWinPanelModel model)
        {
            #region set birthDateString

            var birthDateString = string.Empty;
            var birthDate = new DateTime();

            if (string.IsNullOrEmpty(model.CustomerRegisterPanelModel.L_BIRTHDAY)) model.CustomerRegisterPanelModel.L_BIRTHDAY = "0/0/";

            var date = DateTime.TryParseExact(model.CustomerRegisterPanelModel.L_BIRTHDAY.ToSafeString(), "dd/MM/yyyy",
                                                CultureInfo.InvariantCulture, DateTimeStyles.None, out birthDate);

            if (SiteSession.CurrentUICulture.IsThaiCulture())
            {
                if (model.SummaryPanelModel.VAS_FLAG.ToSafeString() != "2" && model.SummaryPanelModel.TOPUP.ToSafeString() == "")
                {
                    if (birthDate > DateTime.MinValue.AddYears(543))
                        birthDateString = birthDate.AddYears(-543).ToDateDisplayText();
                }
                else
                {
                    if (birthDate > DateTime.MinValue)
                        birthDateString = birthDate.ToDateDisplayText();
                }
            }
            else
            {
                if (birthDate > DateTime.MinValue)
                    birthDateString = birthDate.ToDateDisplayText();
            }
            // Case Leap Year 29/02
            var split_BD = model.CustomerRegisterPanelModel.L_BIRTHDAY.ToSafeString().Split('/');
            if (!date && split_BD.Length > 2)
            {
                if (split_BD.Any())
                {
                    if (SiteSession.CurrentUICulture.IsThaiCulture())
                    {
                        if (split_BD[2] != "")
                        {
                            if (DateTime.IsLeapYear((split_BD[2].ToSafeInteger() - 543)))
                            {
                                if (split_BD[1] == "02")
                                    if (split_BD[0] == "29")
                                    {
                                        int tmpYear = split_BD[2].ToSafeInteger();
                                        if (tmpYear > 2450)
                                        {
                                            tmpYear = tmpYear - 543;
                                        }

                                        birthDate = new DateTime(tmpYear, 2, 29);
                                        birthDateString = birthDate.ToString("dd/MM/yyyy");
                                        model.CustomerRegisterPanelModel.L_BIRTHDAY = birthDateString;

                                    }
                            }
                        }
                        else
                        {
                            birthDateString = "";
                            model.CustomerRegisterPanelModel.L_BIRTHDAY = birthDateString;
                        }
                    }
                    else
                    {
                        if (split_BD[2] != "")
                        {
                            if (DateTime.IsLeapYear(split_BD[2].ToSafeInteger()))
                            {
                                if (split_BD[1] == "02")
                                    if (split_BD[0] == "29")
                                    {
                                        birthDate = new DateTime(split_BD[2].ToSafeInteger(), 2, 29);
                                        birthDateString = birthDate.ToString("dd/MM/yyyy");
                                        model.CustomerRegisterPanelModel.L_BIRTHDAY = birthDateString;
                                    }
                            }
                        }
                        else
                        {
                            birthDateString = "";
                            model.CustomerRegisterPanelModel.L_BIRTHDAY = birthDateString;
                        }
                    }
                }
                else
                {
                    birthDateString = "";
                    model.CustomerRegisterPanelModel.L_BIRTHDAY = birthDateString;
                }

            }

            #endregion

            string title = model.CustomerRegisterPanelModel.L_TITLE_CODE.ToSafeString();
            string firstName = (model.CustomerRegisterPanelModel.CateType.ToSafeString().Equals("R") ? model.CustomerRegisterPanelModel.L_FIRST_NAME : model.CustomerRegisterPanelModel.L_GOVERNMENT_NAME);
            string lastName = (model.CustomerRegisterPanelModel.CateType.ToSafeString().Equals("R") ? model.CustomerRegisterPanelModel.L_LAST_NAME : "");
            string phoneNo = model.CustomerRegisterPanelModel.L_MOBILE.ToSafeString();

            string cTitle = (model.CustomerRegisterPanelModel.CateType.ToSafeString().Equals("R") ? model.CustomerRegisterPanelModel.L_TITLE_CODE.ToSafeString() : "");
            string[] tempcontact = model.CustomerRegisterPanelModel.L_CONTACT_PERSON.ToSafeString().Split();
            string ctemtfirst = "";
            string ctemtlast = "";
            if (tempcontact.Count() > 1)
            {
                ctemtfirst = tempcontact[0];
                for (var i = 1; i < tempcontact.Count(); i++)
                {
                    ctemtlast = ctemtlast + tempcontact[i] + " ";
                }
            }
            else
            {
                ctemtfirst = tempcontact[0];
                ctemtlast = ".";
            }

            string cFirstName = (model.CustomerRegisterPanelModel.CateType.ToSafeString().Equals("R") ? model.CustomerRegisterPanelModel.L_FIRST_NAME : ctemtfirst);
            string cLastName = (model.CustomerRegisterPanelModel.CateType.ToSafeString().Equals("R") ? model.CustomerRegisterPanelModel.L_LAST_NAME : ctemtlast.Trim());
            string cPhoneNo = model.CustomerRegisterPanelModel.L_MOBILE.ToSafeString();

            if (model.ForCoverageResult == true)
            {
                title = model.CoveragePanelModel.L_FIRST_LAST.ToSafeString();
                firstName = model.CoveragePanelModel.L_FIRST_NAME.ToSafeString();
                lastName = model.CoveragePanelModel.L_LAST_NAME.ToSafeString();
                phoneNo = model.CoveragePanelModel.L_CONTACT_PHONE.ToSafeString();

                cTitle = title;
                cFirstName = firstName;
                cLastName = lastName;
                cPhoneNo = phoneNo;
            }

            string cardNo = "";
            string taxId = "";
            if (model.CustomerRegisterPanelModel.CateType.ToSafeString().Equals("R"))
            {
                cardNo = model.CustomerRegisterPanelModel.L_CARD_NO.ToSafeString();
            }
            else
            {
                taxId = model.CustomerRegisterPanelModel.L_CARD_NO.ToSafeString();
            }

            string email = model.CoveragePanelModel.L_CONTACT_EMAIL.ToSafeString();
            string tmp_mail = email;
            tmp_mail = string.IsNullOrEmpty(tmp_mail) ? model.CustomerRegisterPanelModel.L_EMAIL.ToSafeString() : email;

            string cusNat = (model.CustomerRegisterPanelModel.CateType.ToSafeString().Equals("R") ? model.CustomerRegisterPanelModel.L_NATIONALITY : "");

            if (model.SummaryPanelModel.VAS_FLAG.ToSafeString() == "2" || model.SummaryPanelModel.TOPUP.ToSafeString() == "1")
            { cusNat = "THAI"; }

            string condoFloor = model.CustomerRegisterPanelModel.L_NUM_OF_FLOOR.ToSafeString();

            if (model.SummaryPanelModel.VAS_FLAG.ToSafeString() == "2" || model.SummaryPanelModel.TOPUP.ToSafeString() == "1")
                condoFloor = model.CustomerRegisterPanelModel.AddressPanelModelVat.L_FLOOR;

            #region condo
            var CONDO_ROOF_TOP = "";
            var CONDO_BALCONY = "";
            var BALCONY_NORTH = "";
            var BALCONY_SOUTH = "";
            var BALCONY_EAST = "";
            var BALCONY_WAST = "";
            var HIGH_BUILDING = "";
            var HIGH_TREE = "";
            var BILLBOARD = "";
            var EXPRESSWAY = "";
            if (model.SummaryPanelModel.VAS_FLAG.ToSafeString() != "2")
            {
                // condo
                CONDO_ROOF_TOP = (string.IsNullOrEmpty(model.CustomerRegisterPanelModel.L_TOP_TERRACE) ? "N" : "Y");
                CONDO_BALCONY = (string.IsNullOrEmpty(model.CustomerRegisterPanelModel.L_TERRACE) ? "N" : "Y");
                BALCONY_NORTH = (string.IsNullOrEmpty(model.CustomerRegisterPanelModel.L_NORTH) ? "N" : "Y");
                BALCONY_SOUTH = (string.IsNullOrEmpty(model.CustomerRegisterPanelModel.L_SOUTH) ? "N" : "Y");
                BALCONY_EAST = (string.IsNullOrEmpty(model.CustomerRegisterPanelModel.L_EAST) ? "N" : "Y");
                BALCONY_WAST = (string.IsNullOrEmpty(model.CustomerRegisterPanelModel.L_WEST) ? "N" : "Y");
                HIGH_BUILDING = (string.IsNullOrEmpty(model.CustomerRegisterPanelModel.L_BUILDING) ? "N" : "Y");
                HIGH_TREE = (string.IsNullOrEmpty(model.CustomerRegisterPanelModel.L_TREE) ? "N" : "Y");

                // house
                BILLBOARD = (string.IsNullOrEmpty(model.CustomerRegisterPanelModel.L_BILLBOARD) ? "N" : "Y");
                EXPRESSWAY = (string.IsNullOrEmpty(model.CustomerRegisterPanelModel.L_EXPRESSWAY) ? "N" : "Y");

                try
                {
                    //if (!string.IsNullOrEmpty(cpm.Address.L_MOOBAN))
                    if (!string.IsNullOrEmpty(model.CoveragePanelModel.BuildingType) && model.CoveragePanelModel.BuildingType.Equals("V"))
                    {
                        var homeArea = model.CustomerRegisterPanelModel.L_HOUSE_AREA.ToSafeString();

                        var homeAreaSplitValues = homeArea.Split('|');
                        if (homeAreaSplitValues.Length > 3)
                        {
                            HIGH_BUILDING = homeAreaSplitValues[0].HaveValue().ToYesNoFlgString();
                            HIGH_TREE = homeAreaSplitValues[1].HaveValue().ToYesNoFlgString();
                            BILLBOARD = homeAreaSplitValues[2].HaveValue().ToYesNoFlgString();
                            EXPRESSWAY = homeAreaSplitValues[3].HaveValue().ToYesNoFlgString();
                        }
                    }
                    else if (!string.IsNullOrEmpty(model.CoveragePanelModel.BuildingType) && model.CoveragePanelModel.BuildingType.Equals("B"))
                    {
                        var condoType = model.CustomerRegisterPanelModel.L_BUILD_CONDO.ToSafeString();
                        var condoDirection = model.CustomerRegisterPanelModel.L_TERRACE_DIRECTION.ToSafeString();
                        var condoLimit = model.CustomerRegisterPanelModel.L_NUM_OF_FLOOR.ToSafeString();
                        var condoArea = model.CustomerRegisterPanelModel.L_CONDO_AREA.ToSafeString();

                        var condoTypeSplitValues = condoType.Split('|');
                        if (condoTypeSplitValues.Length > 1)
                        {
                            CONDO_ROOF_TOP = condoTypeSplitValues[0].HaveValue().ToYesNoFlgString();
                            CONDO_BALCONY = condoTypeSplitValues[1].HaveValue().ToYesNoFlgString();
                        }

                        var condoDirectionSplitValues = condoDirection.Split('|');
                        if (condoDirectionSplitValues.Length > 3)
                        {
                            BALCONY_NORTH = condoDirectionSplitValues[0].HaveValue().ToYesNoFlgString();
                            BALCONY_SOUTH = condoDirectionSplitValues[1].HaveValue().ToYesNoFlgString();
                            BALCONY_EAST = condoDirectionSplitValues[2].HaveValue().ToYesNoFlgString();
                            BALCONY_WAST = condoDirectionSplitValues[3].HaveValue().ToYesNoFlgString();
                        }

                        var condoAreaSplitValues = condoArea.Split('|');
                        if (condoAreaSplitValues.Length > 1)
                        {
                            HIGH_BUILDING = condoAreaSplitValues[0].HaveValue().ToYesNoFlgString();
                            HIGH_TREE = condoAreaSplitValues[1].HaveValue().ToYesNoFlgString();
                        }
                    }
                    else
                    {
                        var homeArea = model.CustomerRegisterPanelModel.L_HOUSE_AREA.ToSafeString();

                        var homeAreaSplitValues = homeArea.Split('|');
                        if (homeAreaSplitValues.Length > 3)
                        {
                            HIGH_BUILDING = homeAreaSplitValues[0].HaveValue().ToYesNoFlgString();
                            HIGH_TREE = homeAreaSplitValues[1].HaveValue().ToYesNoFlgString();
                            BILLBOARD = homeAreaSplitValues[2].HaveValue().ToYesNoFlgString();
                            EXPRESSWAY = homeAreaSplitValues[3].HaveValue().ToYesNoFlgString();
                        }
                    }
                }
                catch (IndexOutOfRangeException)
                {

                }
            }
            #endregion

            string addressTypeWire = "";
            if (model.SummaryPanelModel.VAS_FLAG.ToSafeString() != "2" && model.SummaryPanelModel.TOPUP.ToSafeString() == "")
            {
                if (!string.IsNullOrEmpty(model.CoveragePanelModel.BuildingType) && model.CoveragePanelModel.BuildingType.Equals("V"))
                {
                    addressTypeWire = "HOUSE";
                    if (!string.IsNullOrEmpty(model.CustomerRegisterPanelModel.L_TYPE_ADDR))
                    {
                        LovValueModel lovValue = new LovValueModel();
                        if (SiteSession.CurrentUICulture.IsThaiCulture())
                            addressTypeWire = model.CustomerRegisterPanelModel.L_TYPE_ADDR;
                        else
                            lovValue = base.LovData.FirstOrDefault(t => t.LovValue2 == model.CustomerRegisterPanelModel.L_TYPE_ADDR);

                        if (null != lovValue && lovValue.LovValue1 != null)
                            addressTypeWire = lovValue.LovValue1;
                    }
                }
                else if (!string.IsNullOrEmpty(model.CoveragePanelModel.BuildingType) && model.CoveragePanelModel.BuildingType.Equals("O"))
                {
                    addressTypeWire = "OTHER_SPECIFIC";
                }
                else if (!string.IsNullOrEmpty(model.CoveragePanelModel.BuildingType) && model.CoveragePanelModel.BuildingType.Equals("B"))
                {
                    addressTypeWire = "CONDO";
                }
                else
                {
                    addressTypeWire = "HOUSE";
                    if (!string.IsNullOrEmpty(model.CustomerRegisterPanelModel.L_TYPE_ADDR))
                    {
                        LovValueModel lovValue = new LovValueModel();
                        if (SiteSession.CurrentUICulture.IsThaiCulture())
                            addressTypeWire = model.CustomerRegisterPanelModel.L_TYPE_ADDR;
                        else
                            lovValue = base.LovData.FirstOrDefault(t => t.LovValue2 == model.CustomerRegisterPanelModel.L_TYPE_ADDR);

                        if (null != lovValue && lovValue.LovValue1 != null)
                            addressTypeWire = lovValue.LovValue1;
                    }
                }
            }

            string floorNo = (model.SummaryPanelModel.VAS_FLAG.ToSafeString().Equals("2") ? "" : model.CoveragePanelModel.Address.L_FLOOR);

            #region mobile,ca,sa,ba
            var CA_ID = "";
            var SA_ID = "";
            var BA_ID = "";
            var P_AIS_MOBILE = "";
            var P_AIS_NONMOBILE = "";
            var Productname = "";
            var ServiceYear = "0";

            if (model.CoveragePanelModel.P_MOBILE.ToSafeString() != "")
            {
                model.CoveragePanelModel.P_MOBILE = model.CoveragePanelModel.P_MOBILE.Replace("|", "");
                if (model.CoveragePanelModel.P_MOBILE != "")
                {
                    if (model.CoveragePanelModel.P_MOBILE.ToSafeString().Substring(0, 1) == "0")
                    {
                        P_AIS_NONMOBILE = "";
                        P_AIS_MOBILE = model.CoveragePanelModel.P_MOBILE.ToSafeString();
                    }
                    else if (model.CoveragePanelModel.P_MOBILE.ToSafeString().Substring(0, 1) != "0")
                    {
                        P_AIS_MOBILE = "";
                        P_AIS_NONMOBILE = model.CoveragePanelModel.P_MOBILE.ToSafeString();
                    }
                }
            }


            if (model.CustomerRegisterPanelModel.FIBRE_ID != null && model.CustomerRegisterPanelModel.FIBRE_ID != "")
            {
                P_AIS_NONMOBILE = model.CustomerRegisterPanelModel.FIBRE_ID.ToSafeString();
            }

            if (model.SummaryPanelModel.VAS_FLAG.ToSafeString() == "1" || (model.SummaryPanelModel.VAS_FLAG.ToSafeString() == "0" && model.SummaryPanelModel.TOPUP.ToSafeString() == ""))
            {
                Productname = model.CoveragePanelModel.SffProductName.ToSafeString();
                ServiceYear = model.CoveragePanelModel.SffServiceYear.ToSafeString();


                if (model.CoveragePanelModel.P_MOBILE.ToSafeString() == "")
                {
                    CA_ID = "";
                    SA_ID = "";
                    BA_ID = "";
                }
                else
                {
                    if (model.CoveragePanelModel.ChargeType == "PREPAID")
                    {
                        CA_ID = "";
                        SA_ID = "";
                        BA_ID = "";
                    }
                    else if (model.CoveragePanelModel.BillingSystem.ToSafeString() == "BOS" && model.CoveragePanelModel.ChargeType == "POSTPAID")
                    {
                        if (model.CoveragePanelModel.BundlingSpecialFlag == "Y" || model.CoveragePanelModel.BundlingMainFlag == "Y")
                        {
                            CA_ID = model.CoveragePanelModel.CA_ID.ToSafeString(); ;
                            SA_ID = model.CoveragePanelModel.SA_ID.ToSafeString(); ;
                            BA_ID = "";
                        }
                        else
                        {
                            CA_ID = "";
                            SA_ID = "";
                            BA_ID = "";
                        }
                    }
                    else
                    {
                        CA_ID = model.CoveragePanelModel.CA_ID.ToSafeString();
                        SA_ID = model.CoveragePanelModel.SA_ID.ToSafeString();
                        BA_ID = "";
                    }

                }
            }
            else if (model.SummaryPanelModel.VAS_FLAG.ToSafeString() == "2" || model.SummaryPanelModel.TOPUP.ToSafeString() == "1")
            {
                Productname = model.CoveragePanelModel.SffProductName;
                ServiceYear = model.CoveragePanelModel.SffServiceYear;
                CA_ID = model.CoveragePanelModel.CA_ID;
                SA_ID = model.CoveragePanelModel.SA_ID;
                BA_ID = model.CoveragePanelModel.BA_ID;
            }
            else if (model.SummaryPanelModel.VAS_FLAG.ToSafeString() == "3")
            {
                if (model.CoveragePanelModel.BillingSystem.ToSafeString() == "BOS" && model.CoveragePanelModel.ChargeType == "POSTPAID")
                {
                    if (model.CoveragePanelModel.BundlingSpecialFlag == "Y" || model.CoveragePanelModel.BundlingMainFlag == "Y")
                    {
                        CA_ID = model.CoveragePanelModel.CA_ID.ToSafeString(); ;
                        SA_ID = model.CoveragePanelModel.SA_ID.ToSafeString(); ;
                        BA_ID = "";
                    }
                    else
                    {
                        CA_ID = "";
                        SA_ID = "";
                        BA_ID = "";
                    }
                }
                else
                {
                    CA_ID = model.CoveragePanelModel.CA_ID.ToSafeString();//cpm.CA_ID.ToSafeString();
                    SA_ID = model.CoveragePanelModel.SA_ID.ToSafeString();//cpm.SA_ID.ToSafeString();
                    BA_ID = "";
                }
            }
            #endregion

            #region install date
            var tempdatefinal = model.CustomerRegisterPanelModel.FBSSTimeSlot.AppointmentDate.ToDisplayText("dd/MM/yyyy");
            #endregion

            #region vendor
            var vendor = "";
            if (model.SummaryPanelModel.VAS_FLAG == "2" || model.SummaryPanelModel.TOPUP == "1")
            {
                if (model.SummaryPanelModel.PackageModelList.Any())
                {
                    List<string> listowner = base.LovData.Where(t => t.Name == "MAPPING_OWNER_PRODUCT").Select(t => t.LovValue3).ToList();

                    if (listowner.Contains(model.SummaryPanelModel.PackageModelList[0].OWNER_PRODUCT.ToSafeString()))
                    {
                        vendor = model.SummaryPanelModel.PackageModelList[0].OWNER_PRODUCT.ToSafeString();
                    }

                }
            }
            else
            {
                PackageModel PackageMain = null;
                if (model.SummaryPanelModel.PackageModelList.Any())
                    PackageMain = model.SummaryPanelModel.PackageModelList.FirstOrDefault(t => t.PACKAGE_TYPE == "1");


                if (PackageMain != null)
                {
                    if (PackageMain.PRODUCT_SUBTYPE == "FTTx" || PackageMain.PRODUCT_SUBTYPE == "FTTR")
                    {
                        if (model.CoveragePanelModel.P_FTTX_VENDOR.ToSafeString().Contains("|"))
                        {
                            string[] tempowner = model.CoveragePanelModel.P_FTTX_VENDOR.Split('|');
                            List<string> listowner = base.LovData.Where(t => t.Name == "MAPPING_OWNER_PRODUCT").Select(t => t.LovValue3).ToList();
                            foreach (var a in tempowner)
                            {
                                if (listowner.Contains(a))
                                {
                                    vendor = a;
                                }

                            }
                        }
                        else if (PackageMain.PRODUCT_SUBTYPE == "FTTR")
                        {
                            vendor = PackageMain.OWNER_PRODUCT.ToSafeString();
                        }
                        else
                        {
                            vendor = PackageMain.OWNER_PRODUCT.ToSafeString();
                        }
                    }
                    else if (PackageMain.PRODUCT_SUBTYPE == "WTTx")
                    {
                        vendor = "AWN";
                    }
                }
            }
            #endregion

            string engFlag = "Y";
            if (SiteSession.CurrentUICulture.IsThaiCulture())
            {
                engFlag = "N";
            }

            string flowflag = model.FlowFlag.ToSafeString();
            string sitecode = model.SiteCode.ToSafeString();
            string lineid = model.CoveragePanelModel.L_CONTACT_LINE_ID.ToSafeString();

            string strPlug_and_play_flag = "";
            if (!string.IsNullOrEmpty(model.CustomerRegisterPanelModel.L_EVENT_CODE.ToSafeString()))
            {
                strPlug_and_play_flag = model.CustomerRegisterPanelModel.Plug_and_play_flag.ToSafeString();
            }

            //R18.1 FTTB Sell Router
            if (!string.IsNullOrEmpty(model.CustomerRegisterPanelModel.RouterFlag))
            {
                if (model.CustomerRegisterPanelModel.RouterFlag == "S")
                {
                    strPlug_and_play_flag = "4";
                }
                else if (model.CustomerRegisterPanelModel.RouterFlag == "M")
                {
                    strPlug_and_play_flag = "3";
                }
            }

            //TODO: Splitter Management
            string splitterFlag = string.Empty;
            string reservedPortId = string.Empty;
            string spacialRemark = string.Empty;
            if (!string.IsNullOrEmpty(model.FlowFlag)
                && model.CoveragePanelModel.AccessMode == "FTTH")
            {
                splitterFlag = model.SplitterFlag;
                reservedPortId = model.ReservationId;

                spacialRemark = base.LovData.FirstOrDefault(t => t.Name == "SaveOrderNew"
                                                            && t.LovValue1 == model.SplitterFlagFirstTime
                                                            && t.LovValue2 == model.SplitterFlag).LovValue3.ToSafeString();
            }
            else if (model.CoveragePanelModel.AccessMode == "VDSL")
            {
                splitterFlag = model.SplitterFlag;
                reservedPortId = model.ReservationId;
                spacialRemark = base.LovData.FirstOrDefault(t => t.Name == "SaveOrderNew"
                                                            && t.Text == "SPECIAL_REMARK_FTTB"
                                                            && t.LovValue1 == model.SplitterFlagFirstTime).LovValue3.ToSafeString();
            }
            else if (model.CoveragePanelModel.AccessMode == "WTTx")
            {
                splitterFlag = model.SplitterFlag;
                reservedPortId = model.ReservationId;
                spacialRemark = base.LovData.FirstOrDefault(t => t.Name == "SaveOrderNew"
                                                            && t.Text == "SPECIAL_REMARK_WTTX").LovValue1.ToSafeString();
            }

            LovValueModel lovEstatement = base.LovData.FirstOrDefault(t => t.Name == "ESTATEMENT_STATUS" && t.Type == "FBB_CONSTANT" && t.ActiveFlag == "Y");

            var billMedia = string.Empty;
            if (!string.IsNullOrEmpty(model.CustomerRegisterPanelModel.EBillFlag))
            {
                if (model.CustomerRegisterPanelModel.EBillFlag == "0")
                {
                    billMedia = lovEstatement.LovValue2;
                }
                else if (model.CustomerRegisterPanelModel.EBillFlag == "1")
                {
                    billMedia = lovEstatement.LovValue3;
                }
                else if (model.CustomerRegisterPanelModel.EBillFlag == "2")
                {
                    billMedia = lovEstatement.LovValue1;
                }
            }

            CoverageMemberGetMemberModel MemberGetMemberInfo = model.CoveragePanelModel.CoverageMemberGetMember;
            string reffernce_no = MemberGetMemberInfo.RefferenceNo.ToSafeString();
            string voucher_desc = MemberGetMemberInfo.VoucherDesc.ToSafeString();
            string campaign_project_name = MemberGetMemberInfo.CampaignProjectName.ToSafeString();
            string channel = model.RegisterChannel.ToSafeString();
            string dev_project_code = model.CustomerRegisterPanelModel.p_dev_project_code.ToSafeString();
            string dev_bill_to = model.CustomerRegisterPanelModel.p_dev_bill_to.ToSafeString();
            string dev_price = model.CustomerRegisterPanelModel.p_dev_price.ToSafeString();
            string dev_po_no = model.CustomerRegisterPanelModel.PO_NO.ToSafeString();


            string PayMentMethod = model.PayMentMethod.ToSafeString();
            string PayMentOrderID = model.PayMentOrderID.ToSafeString();
            string PayMentTranID = model.PayMentTranID.ToSafeString();

            var packageModelList = new List<PackageModel>();
            if (model.SummaryPanelModel.VAS_FLAG.ToSafeString() == "2" || model.SummaryPanelModel.TOPUP.ToSafeString() == "1")
            {
                if ((model.SummaryPanelModel.PackageModel.SelectVas_Flag == "1" && model.SummaryPanelModel.VOIP_FLAG == "1")
                    || (model.SummaryPanelModel.PackageModel.SelectPlayBox_Flag == "0" && model.SummaryPanelModel.PackageModel.SelectVas_Flag == "1"))
                {
                    //register VOIP Only
                    packageModelList = model.SummaryPanelModel.PackageModelList.Where(item => item.PRODUCT_SUBTYPE == "VOIP").ToList();
                }
                else
                {
                    //register Playbox Only

                    // remove package VOIP from list package
                    model.SummaryPanelModel.PackageModelList.RemoveAll(item => item.PRODUCT_SUBTYPE == "VOIP");

                    if (!string.IsNullOrEmpty(model.RegisterPlayboxNumber) &&
                        Convert.ToInt16(model.RegisterPlayboxNumber) > 0)
                    {
                        // register Playbox 2,3,....
                        for (var i = 0; i < Convert.ToInt16(model.RegisterPlayboxNumber); i++)
                        {
                            var iExt = i.ToSafeString();
                            var playboxItem =
                                model.MulitPlaybox.SingleOrDefault(
                                    item => item.RowNumber == iExt) ?? new MulitPlayboxModel();
                            var ext =
                                playboxItem.InstallProductSubType.Substring(
                                    playboxItem.InstallProductSubType.Length - 4);
                            foreach (
                                var packageModel in
                                    model.SummaryPanelModel.PackageModelList.Where(item => item.SERVICE_CODE == playboxItem.ServiceCode)
                                )
                            {
                                packageModel.PLAYBOX_EXT = ext;
                                packageModelList.Add(packageModel);
                            }
                        }
                    }
                    else
                    {
                        //register Playbox Main

                        //remove package Playbox 2,3,....  from list package
                        model.SummaryPanelModel.PackageModelList.RemoveAll(
                            item =>
                                !string.IsNullOrEmpty(item.MAPPING_PRODUCT) &&
                                item.MAPPING_PRODUCT.Substring(0, 1) == "E");

                        packageModelList = model.SummaryPanelModel.PackageModelList;
                    }

                }
            }
            else
            {
                packageModelList = model.SummaryPanelModel.PackageModelList;
                packageModelList = packageModelList.OrderBy(p => p.PACKAGE_SERVICE_CODE).ThenBy(p => p.PACKAGE_TYPE).ToList();
            }

            var temp = packageModelList;

            //R18.1 FTTB Sell Router
            string[] routerPackageCode = null;
            if (!string.IsNullOrEmpty(model.CustomerRegisterPanelModel.RouterFlag) && model.CustomerRegisterPanelModel.RouterFlag == "S")
            {
                routerPackageCode = base.LovData.Where(t => t.Type == "ROUTER_FIBRE" && t.Text == "SELECT_GROUP" && t.ActiveFlag == "Y").Select(t => t.LovValue1).ToArray();
            }

            List<AirRegistPackage> airregists = temp.Select(o => new AirRegistPackage()
            {
                fax_flag = o.FAX_FLAG.ToSafeString(),
                temp_ia = o.PACKAGE_SERVICE_CODE.ToSafeString(),
                home_ip = o.VOIP_IP.ToSafeString(),
                home_port = "",
                idd_flag = o.IDD_FLAG.ToSafeString(),
                package_code = o.SFF_PROMOTION_CODE.ToSafeString(),
                package_type = o.PACKAGE_TYPE.ToSafeString(),
                product_subtype = o.PRODUCT_SUBTYPE.ToSafeString(),
                mobile_forward = o.MOBILE_FORWARD.ToSafeString(),
                pbox_ext = o.PLAYBOX_EXT.ToSafeString(),
                package_price = (routerPackageCode != null)
                        ? (routerPackageCode.Contains(o.SFF_PROMOTION_CODE.ToSafeString())
                            ? o.PRICE_CHARGE.GetValueOrDefault()
                            : 0)
                        : 0
            }).ToList();

            List<UploadImage> temp2 = model.CustomerRegisterPanelModel.ListImageFile;
            List<AirRegistFile> airImage = temp2.Select(o => new AirRegistFile()
            {
                file_name = o.FileName
            }).ToList();

            List<AirRegistSplitter> airSplitter = new List<AirRegistSplitter>();
            if (model.CoverageAreaResultModel.SPLITTER_LIST != null)
            {
                airSplitter = model.CoverageAreaResultModel.SPLITTER_LIST.ConvertAll(x => new AirRegistSplitter
                {
                    SPLITTER_NAME = x.Splitter_Name.ToSafeString(),
                    DISTANCE = x.Distance,
                    DISTANCE_TYPE = x.Distance_Type.ToSafeString(),
                    RESOURCE_TYPE = "SPLITTER"
                });
            }
            else if (model.CoverageAreaResultModel.RESOURCE_LIST != null)
            {
                airSplitter = model.CoverageAreaResultModel.RESOURCE_LIST.ConvertAll(x => new AirRegistSplitter
                {
                    SPLITTER_NAME = x.Dslam_Name.ToSafeString(),
                    DISTANCE = 0,
                    DISTANCE_TYPE = "",
                    RESOURCE_TYPE = "DSLAM"
                });
            }

            List<AirRegistCPESerial> airCPE = new List<AirRegistCPESerial>();
            if (model.CoverageAreaResultModel.WTTX_COVERAGE_RESULT == "YES")
            {
                if (model.CustomerRegisterPanelModel.WTTx_Info != null)
                {
                    airCPE = model.CustomerRegisterPanelModel.WTTx_Info.ConvertAll(l => new AirRegistCPESerial()
                    {
                        SERIAL_NO = l.SN.ToSafeString(),
                        CPE_TYPE = l.cpe_type.ToSafeString(),
                        MAC_ADDRESS = l.CPE_MAC_ADDR.ToSafeString(),
                        STATUS_DESC = l.STATUS_DESC.ToSafeString(),
                        MODEL_NAME = l.CPE_MODEL_NAME.ToSafeString(),
                        COMPANY_CODE = l.CPE_COMPANY_CODE.ToSafeString(),
                        CPE_PLANT = l.CPE_PLANT.ToSafeString(),
                        STORAGE_LOCATION = l.CPE_STORAGE_LOCATION.ToSafeString(),
                        MATERIAL_CODE = l.CPE_MATERIAL_CODE.ToSafeString(),
                        REGISTER_DATE = l.REGISTER_DATE.ToSafeString(),
                        FIBRENET_ID = l.FIBRENET_ID.ToSafeString(),
                        SN_PATTERN = l.SN_PATTERN.ToSafeString(),
                        SHIP_TO = l.SHIP_TO.ToSafeString(),
                        WARRANTY_START_DATE = l.WARRANTY_START_DATE.ToSafeString(),
                        WARRANTY_END_DATE = l.WARRANTY_END_DATE.ToSafeString()
                    });
                }
            }
            else
            {
                if (model.CustomerRegisterPanelModel.CPE_Info != null)
                {
                    airCPE = model.CustomerRegisterPanelModel.CPE_Info.ConvertAll(l => new AirRegistCPESerial()
                    {
                        SERIAL_NO = l.SN.ToSafeString(),
                        CPE_TYPE = l.cpe_type.ToSafeString(),
                        MAC_ADDRESS = l.CPE_MAC_ADDR.ToSafeString(),
                        STATUS_DESC = l.STATUS_DESC.ToSafeString(),
                        MODEL_NAME = l.CPE_MODEL_NAME.ToSafeString(),
                        COMPANY_CODE = l.CPE_COMPANY_CODE.ToSafeString(),
                        CPE_PLANT = l.CPE_PLANT.ToSafeString(),
                        STORAGE_LOCATION = l.CPE_STORAGE_LOCATION.ToSafeString(),
                        MATERIAL_CODE = l.CPE_MATERIAL_CODE.ToSafeString(),
                        REGISTER_DATE = l.REGISTER_DATE.ToSafeString(),
                        FIBRENET_ID = l.FIBRENET_ID.ToSafeString(),
                        SN_PATTERN = l.SN_PATTERN.ToSafeString(),
                        SHIP_TO = l.SHIP_TO.ToSafeString(),
                        WARRANTY_START_DATE = l.WARRANTY_START_DATE.ToSafeString(),
                        WARRANTY_END_DATE = l.WARRANTY_END_DATE.ToSafeString()
                    });
                }
            }

            List<AirRegistCustInsi> custInsightRecord = new List<AirRegistCustInsi>();
            if (model.CustomerRegisterPanelModel.ListCustomerInsight != null && model.CustomerRegisterPanelModel.ListCustomerInsight.Count > 0)
            {
                custInsightRecord = model.CustomerRegisterPanelModel.ListCustomerInsight.Select(p => new AirRegistCustInsi
                {
                    GROUP_ID = p.GROUP_ID.ToSafeString(),
                    GROUP_NAME_TH = p.GROUP_NAME_TH.ToSafeString(),
                    GROUP_NAME_EN = p.GROUP_NAME_EN.ToSafeString(),
                    QUESTION_ID = p.QUESTION_ID.ToSafeString(),
                    QUESTION_TH = p.QUESTION_TH.ToSafeString(),
                    QUESTION_EN = p.QUESTION_EN.ToSafeString(),
                    ANSWER_ID = p.ANSWER_ID.ToSafeString(),
                    ANSWER_TH = p.ANSWER_TH.ToSafeString(),
                    ANSWER_EN = p.ANSWER_EN.ToSafeString(),
                    ANSWER_VALUE_TH = p.ANSWER_VALUE_TH.ToSafeString(),
                    ANSWER_VALUE_EN = p.ANSWER_VALUE_EN.ToSafeString(),
                    PARENT_ANSWER_ID = p.PARENT_ANSWER_ID.ToSafeString(),
                    ACTION_WFM = p.ACTION_WFM.ToSafeString(),
                    ACTION_FOA = p.ACTION_FOA.ToSafeString()
                }).ToList();
            }

            List<AirRegistDcontract> dcontract = new List<AirRegistDcontract>();

            InsertSaveOrderNew911Command command = new InsertSaveOrderNew911Command();
            command.p_customer_type = model.CustomerRegisterPanelModel.CateType.ToSafeString();
            command.p_customer_subtype = model.CustomerRegisterPanelModel.SubCateType.ToSafeString();
            command.p_title_code = title.ToSafeString();
            command.p_first_name = firstName.ToSafeString().Trim();
            command.p_last_name = lastName.ToSafeString().Trim();
            command.p_contact_title_code = cTitle.ToSafeString();
            command.p_contact_first_name = cFirstName.ToSafeString();
            command.p_contact_last_name = cLastName.ToSafeString();
            command.p_id_card_type_desc = model.CustomerRegisterPanelModel.L_CARD_TYPE.ToSafeString();
            command.p_id_card_no = cardNo.ToSafeString();
            command.p_tax_id = taxId.ToSafeString();
            command.p_gender = model.CustomerRegisterPanelModel.L_GENDER.ToSafeString();
            command.p_birth_date = birthDateString.ToSafeString();
            command.p_mobile_no = phoneNo.ToSafeString();
            command.p_mobile_no_2 = model.CustomerRegisterPanelModel.L_OR.ToSafeString();
            command.p_home_phone_no = model.CustomerRegisterPanelModel.L_HOME_PHONE.ToSafeString();
            command.p_email_address = tmp_mail.ToSafeString();
            command.p_contact_time = model.CustomerRegisterPanelModel.L_SPECIFIC_TIME.ToSafeString();
            command.p_nationality_desc = cusNat.ToSafeString();
            command.p_customer_remark = model.CustomerRegisterPanelModel.L_REMARK.ToSafeString();
            command.p_house_no = model.CustomerRegisterPanelModel.AddressPanelModelSetup.L_HOME_NUMBER_2.ToSafeString();
            command.p_moo_no = model.CustomerRegisterPanelModel.AddressPanelModelSetup.L_MOO.ToSafeString();
            command.p_building = model.CustomerRegisterPanelModel.AddressPanelModelSetup.L_BUILD_NAME.ToSafeString();
            command.p_floor = model.CustomerRegisterPanelModel.AddressPanelModelSetup.L_FLOOR.ToSafeString();
            command.p_room = model.CustomerRegisterPanelModel.AddressPanelModelSetup.L_ROOM.ToSafeString();
            command.p_mooban = model.CustomerRegisterPanelModel.AddressPanelModelSetup.L_MOOBAN.ToSafeString();
            command.p_soi = model.CustomerRegisterPanelModel.AddressPanelModelSetup.L_SOI.ToSafeString();
            command.p_road = model.CustomerRegisterPanelModel.AddressPanelModelSetup.L_ROAD.ToSafeString();
            command.p_zipcode_rowid = model.CustomerRegisterPanelModel.AddressPanelModelSetup.ZIPCODE_ID.ToSafeString();
            command.p_latitude = model.CoveragePanelModel.L_LAT.ToSafeString();
            command.p_longtitude = model.CoveragePanelModel.L_LONG.ToSafeString();
            command.p_asc_code = model.CustomerRegisterPanelModel.L_ASC_CODE.ToSafeString();
            command.p_employee_id = model.CustomerRegisterPanelModel.L_STAFF_ID.ToSafeString();
            command.p_location_code = model.CustomerRegisterPanelModel.L_LOC_CODE.ToSafeString();
            command.p_sale_represent = model.CustomerRegisterPanelModel.L_SALE_REP.ToSafeString();
            command.p_cs_note = model.CustomerRegisterPanelModel.L_FOR_CS_TEAM.ToSafeString();
            command.p_wifi_access_point = model.DisplayPackagePanelModel.WIFIAccessPoint.ToSafeString();
            command.p_install_status = "N";
            command.p_coverage = model.CoveragePanelModel.L_RESULT.ToSafeString();
            command.p_existing_airnet_no = "";
            command.p_gsm_mobile_no = "";
            command.p_contact_name_1 = cFirstName.ToSafeString();
            command.p_contact_name_2 = cLastName.ToSafeString();
            command.p_contact_mobile_no_1 = cPhoneNo.ToSafeString();
            command.p_contact_mobile_no_2 = model.CustomerRegisterPanelModel.L_OR.ToSafeString();
            command.p_condo_floor = condoFloor.ToSafeString();
            command.p_condo_roof_top = CONDO_ROOF_TOP.ToSafeString();
            command.p_condo_balcony = CONDO_BALCONY.ToSafeString();
            command.p_balcony_north = BALCONY_NORTH.ToSafeString();
            command.p_balcony_south = BALCONY_SOUTH.ToSafeString();
            command.p_balcony_east = BALCONY_EAST.ToSafeString();
            command.p_balcony_wast = BALCONY_WAST.ToSafeString();
            command.p_high_building = HIGH_BUILDING.ToSafeString();
            command.p_high_tree = HIGH_TREE.ToSafeString();
            command.p_billboard = BILLBOARD.ToSafeString();
            command.p_expressway = EXPRESSWAY.ToSafeString();
            command.p_address_type_wire = addressTypeWire.ToSafeString();
            command.p_address_type = model.CustomerRegisterPanelModel.L_TYPE_ADDR.ToSafeString();
            command.p_floor_no = floorNo.ToSafeString();
            command.p_house_no_bl = model.CustomerRegisterPanelModel.AddressPanelModelSendDoc.L_HOME_NUMBER_2.ToSafeString();
            command.p_moo_no_bl = model.CustomerRegisterPanelModel.AddressPanelModelSendDoc.L_MOO.ToSafeString();
            command.p_building_bl = model.CustomerRegisterPanelModel.AddressPanelModelSendDoc.L_BUILD_NAME.ToSafeString();
            command.p_floor_bl = model.CustomerRegisterPanelModel.AddressPanelModelSendDoc.L_FLOOR.ToSafeString();
            command.p_room_bl = model.CustomerRegisterPanelModel.AddressPanelModelSendDoc.L_ROOM.ToSafeString();
            command.p_mooban_bl = model.CustomerRegisterPanelModel.AddressPanelModelSendDoc.L_MOOBAN.ToSafeString();
            command.p_soi_bl = model.CustomerRegisterPanelModel.AddressPanelModelSendDoc.L_SOI.ToSafeString();
            command.p_road_bl = model.CustomerRegisterPanelModel.AddressPanelModelSendDoc.L_ROAD.ToSafeString();
            command.p_zipcode_rowid_bl = model.CustomerRegisterPanelModel.AddressPanelModelSendDoc.ZIPCODE_ID.ToSafeString();
            command.p_house_no_vt = model.CustomerRegisterPanelModel.AddressPanelModelVat.L_HOME_NUMBER_2.ToSafeString();
            command.p_moo_no_vt = model.CustomerRegisterPanelModel.AddressPanelModelVat.L_MOO.ToSafeString();
            command.p_building_vt = model.CustomerRegisterPanelModel.AddressPanelModelVat.L_BUILD_NAME.ToSafeString();
            command.p_floor_vt = model.CustomerRegisterPanelModel.AddressPanelModelVat.L_FLOOR.ToSafeString();
            command.p_room_vt = model.CustomerRegisterPanelModel.AddressPanelModelVat.L_ROOM.ToSafeString();
            command.p_mooban_vt = model.CustomerRegisterPanelModel.AddressPanelModelVat.L_MOOBAN.ToSafeString();
            command.p_soi_vt = model.CustomerRegisterPanelModel.AddressPanelModelVat.L_SOI.ToSafeString();
            command.p_road_vt = model.CustomerRegisterPanelModel.AddressPanelModelVat.L_ROAD.ToSafeString();
            command.p_zipcode_rowid_vt = model.CustomerRegisterPanelModel.AddressPanelModelVat.ZIPCODE_ID.ToSafeString();
            command.p_cvr_id = "";
            command.p_cvr_node = "";
            command.p_cvr_tower = "";
            command.p_relate_mobile = P_AIS_MOBILE.ToSafeString();
            command.p_relate_non_mobile = P_AIS_NONMOBILE.ToSafeString();
            command.p_sff_ca_no = CA_ID.ToSafeString();
            command.p_sff_sa_no = SA_ID.ToSafeString();
            command.p_sff_ba_no = BA_ID.ToSafeString();
            command.p_network_type = Productname.ToSafeString();
            command.p_service_day = ServiceYear.ToSafeString();
            command.p_expect_install_date = tempdatefinal.ToSafeString();
            command.p_fttx_vendor = vendor.ToSafeString();
            command.p_install_note = "";
            command.p_phone_flag = model.CoveragePanelModel.L_HAVE_FIXED_LINE.ToSafeString();
            command.p_time_Slot = model.CustomerRegisterPanelModel.FBSSTimeSlot.TimeSlot.ToSafeString();
            command.p_installation_Capacity = model.CustomerRegisterPanelModel.FBSSTimeSlot.InstallationCapacity.ToSafeString();
            command.p_address_Id = model.CoveragePanelModel.Address.AddressId.ToSafeString();
            command.p_access_Mode = model.CoveragePanelModel.AccessMode.ToSafeString();
            command.p_eng_flag = engFlag.ToSafeString();
            command.p_event_code = model.CustomerRegisterPanelModel.L_EVENT_CODE.ToSafeString();
            command.p_installAddress1 = model.CustomerRegisterPanelModel.installAddress1.ToSafeString();
            command.p_installAddress2 = model.CustomerRegisterPanelModel.installAddress2.ToSafeString();
            command.p_installAddress3 = model.CustomerRegisterPanelModel.installAddress3.ToSafeString();
            command.p_installAddress4 = model.CustomerRegisterPanelModel.installAddress4.ToSafeString();
            command.p_installAddress5 = model.CustomerRegisterPanelModel.installAddress5.ToSafeString();
            command.p_pbox_count = model.CustomerRegisterPanelModel.pbox_count.ToSafeString();
            command.p_convergence_flag = model.CustomerRegisterPanelModel.convergence_flag.ToSafeString();
            command.p_time_slot_id = model.CustomerRegisterPanelModel.FBSSTimeSlot.TimeSlotId.ToSafeString();
            command.p_gift_voucher = model.CustomerRegisterPanelModel.L_VOUCHER_PIN.ToSafeString();
            command.p_sub_location_id = model.CustomerRegisterPanelModel.AddressPanelModelVat.SUB_LOCATION_ID.ToSafeString();
            command.p_sub_contract_name = model.CustomerRegisterPanelModel.AddressPanelModelVat.SUB_CONTRACT_NAME.ToSafeString();
            command.p_install_staff_id = model.CustomerRegisterPanelModel.AddressPanelModelVat.INSTALL_STAFF_ID.ToSafeString();
            command.p_install_staff_name = model.CustomerRegisterPanelModel.AddressPanelModelVat.INSTALL_STAFF_NAME.ToSafeString();
            command.p_flow_flag = flowflag.ToSafeString();
            command.p_site_code = sitecode.ToSafeString();
            command.p_line_id = lineid.ToSafeString();
            command.p_relate_project_name = model.CustomerRegisterPanelModel.Project_name.ToSafeString();
            command.p_plug_and_play_flag = strPlug_and_play_flag;
            command.p_reserved_id = model.SummaryPanelModel.RESERVED_ID;
            command.p_job_order_type = model.CustomerRegisterPanelModel.JOB_ORDER_TYPE;
            command.p_assign_rule = model.CustomerRegisterPanelModel.ASSIGN_RULE;
            command.p_old_isp = model.CustomerRegisterPanelModel.L_OLD_ISP.ToSafeString();
            command.p_splitter_flag = splitterFlag;
            command.p_reserved_port_id = reservedPortId;
            command.p_special_remark = spacialRemark;
            command.p_order_no = model.PayMentOrderID.ToSafeString();
            command.p_source_system = "";
            command.p_bill_media = billMedia;
            command.p_pre_order_no = reffernce_no;
            command.p_voucher_desc = voucher_desc;
            command.p_campaign_project_name = campaign_project_name;
            command.p_pre_order_chanel = channel;
            command.p_rental_flag = model.CustomerRegisterPanelModel.RentalFlag.ToSafeString();
            command.p_dev_project_code = dev_project_code.ToSafeString();
            command.p_dev_bill_to = dev_bill_to.ToSafeString();
            command.p_dev_po_no = dev_po_no.ToSafeString();
            command.p_partner_type = model.CustomerRegisterPanelModel.outType.ToSafeString();
            command.p_partner_subtype = model.CustomerRegisterPanelModel.outSubType.ToSafeString();
            command.p_mobile_by_asc = model.CustomerRegisterPanelModel.outMobileNo.ToSafeString();
            command.p_location_name = model.CustomerRegisterPanelModel.PartnerName.ToSafeString();
            command.p_paymentMethod = PayMentMethod;
            command.p_transactionId_in = PayMentOrderID;
            command.p_transactionId = PayMentTranID;
            command.p_sub_access_mode = model.CustomerRegisterPanelModel.SUB_ACCESS_MODE.ToSafeString();
            command.p_request_sub_flag = model.CustomerRegisterPanelModel.REQUEST_SUB_FLAG.ToSafeString();
            command.p_premium_flag = model.CustomerRegisterPanelModel.PREMIUM_FLAG.ToSafeString();
            command.p_relate_mobile_segment = model.CustomerRegisterPanelModel.RELATE_MOBILE_SEGMENT.ToSafeString();
            command.p_ref_ur_no = model.CustomerRegisterPanelModel.REF_UR_NO.ToSafeString();
            command.p_location_email_by_region = model.CustomerRegisterPanelModel.LOCATION_EMAIL_BY_REGION.ToSafeString();
            command.p_sale_staff_name = model.CustomerRegisterPanelModel.EMP_NAME.ToSafeString();
            command.p_dopa_flag = model.CustomerRegisterPanelModel.FlagDopaSubmit.ToSafeString();
            command.p_service_year = model.CustomerRegisterPanelModel.SffServiceYear.ToSafeString();
            command.p_require_cs_verify_doc = model.CustomerRegisterPanelModel.FlagVarifyDocuments.ToSafeString();
            command.p_facerecog_flag = model.CustomerRegisterPanelModel.FlagFaceRecognitionSubmit.ToSafeString();
            command.p_special_account_name = model.CustomerRegisterPanelModel.SpecialAccountName.ToSafeString();
            command.p_special_account_no = model.CustomerRegisterPanelModel.SpecialAccountNo.ToSafeString();
            command.p_special_account_enddate = model.CustomerRegisterPanelModel.SpecialAccountEnddate.ToSafeString();
            command.p_special_account_group_email = model.CustomerRegisterPanelModel.SpecialAccountGroupEmail.ToSafeString();
            command.p_special_account_flag = model.CustomerRegisterPanelModel.SpecialAccountFlag.ToSafeString();
            command.p_existing_mobile_flag = model.CustomerRegisterPanelModel.Existing_Mobile.ToSafeString();
            command.p_pre_survey_date = model.CustomerRegisterPanelModel.PreSurveyDate.ToSafeString();
            command.p_pre_survey_timeslot = model.CustomerRegisterPanelModel.PreSurveyTimeslot.ToSafeString();
            command.p_register_channel = model.CustomerRegisterPanelModel.RegisterChannelSaveOrder.ToSafeString();
            command.p_auto_create_prospect_flag = model.CustomerRegisterPanelModel.AutoCreateProspectFlag.ToSafeString();
            command.p_order_verify = model.CustomerRegisterPanelModel.OrderVerify.ToSafeString();
            command.p_waiting_install_date = "";
            command.p_waiting_time_slot = "";
            command.p_sale_channel = model.CoveragePanelModel.SAVEORDER_SALE_CHANNEL.ToSafeString();
            command.p_owner_product = model.CoveragePanelModel.SAVEORDER_OWNER_PRODUCT.ToSafeString();
            command.p_package_for = model.CoveragePanelModel.SAVEORDER_PACKAGE_FOR.ToSafeString();
            command.p_sff_promotion_code = "";
            command.p_region = model.CoveragePanelModel.SAVEORDER_REGION.ToSafeString();
            command.p_province = model.CoveragePanelModel.SAVEORDER_PROVINCE.ToSafeString();
            command.p_district = model.CoveragePanelModel.SAVEORDER_DISTRICT.ToSafeString();
            command.p_sub_district = model.CoveragePanelModel.SAVEORDER_SUB_DISTRICT.ToSafeString();
            command.p_serenade_flag = model.CoveragePanelModel.SAVEORDER_SERENADE_FLAG.ToSafeString();
            command.p_fmpa_flag = model.CoveragePanelModel.SAVEORDER_FMPA_FLAG.ToSafeString();
            command.p_cvm_flag = model.CoveragePanelModel.SAVEORDER_CVM_FLAG.ToSafeString();
            command.p_order_relate_change_pro = "";
            command.p_company_name = (model.OfficerInfoPanelModel.outTitle.ToSafeString() + " " + model.OfficerInfoPanelModel.outCompanyName.ToSafeString()).Trim();
            command.p_distribution_channel = model.OfficerInfoPanelModel.outDistChn.ToSafeString();
            command.p_channel_sales_group = model.OfficerInfoPanelModel.outChnSales.ToSafeString();
            command.p_shop_type = model.OfficerInfoPanelModel.outShopType.ToSafeString();
            command.p_shop_segment = model.OfficerInfoPanelModel.outOperatorClass.ToSafeString();
            command.p_asc_name = model.OfficerInfoPanelModel.outASCTitleThai.ToSafeString() + model.OfficerInfoPanelModel.outASCPartnerName.ToSafeString();
            command.p_asc_member_category = model.OfficerInfoPanelModel.outMemberCategory.ToSafeString();
            command.p_asc_position = model.OfficerInfoPanelModel.outPosition.ToSafeString();
            command.p_location_region = model.OfficerInfoPanelModel.outLocationRegion.ToSafeString();
            command.p_location_sub_region = model.OfficerInfoPanelModel.outLocationSubRegion.ToSafeString();
            command.p_employee_name = (model.OfficerInfoPanelModel.THFirstName.ToSafeString() + " " + model.OfficerInfoPanelModel.THLastName.ToSafeString()).Trim();
            command.p_service_level = model.CustomerRegisterPanelModel.ServiceLevel.ToSafeString();
            command.p_amendment_flag = model.CustomerRegisterPanelModel.ServiceLevel_Flag.ToSafeString();
            command.p_customerpurge = "";
            command.p_exceptentryfee = "";
            command.p_secondinstallation = "";
            command.p_first_install_date = model.CustomerRegisterPanelModel.FBSSTimeSlot.FirstInstallDate.ToSafeString();
            command.p_first_time_slot = model.CustomerRegisterPanelModel.FBSSTimeSlot.FirstTimeSlot.ToSafeString();
            command.p_line_temp_id = model.CustomerRegisterPanelModel.LINE_TEMP_ID.ToSafeString();
            command.p_fmc_special_flag = model.CoveragePanelModel.SAVEORDER_FMC_SPECIAL_FLAG.ToSafeString();
            command.p_non_res_flag = model.CustomerRegisterPanelModel.Non_Res_Flag.ToSafeString();
            command.p_criteria_mobile = P_AIS_MOBILE.ToSafeString();
            command.p_remark_for_subcontract = model.CustomerRegisterPanelModel.Remark_For_Subcontract.ToSafeString();
            command.p_mesh_count = model.CustomerRegisterPanelModel.mesh_count;
            command.p_online_flag = model.CustomerRegisterPanelModel.Online_Flag.ToSafeString();
            command.p_privilege_points = "";
            command.p_transaction_privilege_id = model.CustomerRegisterPanelModel.StaffPrivilegeBypass_TransactionID.ToSafeString();
            command.p_special_skill = "";
            command.p_tdm_contract_id = model.CustomerRegisterPanelModel.TDMContractId.ToSafeString();
            command.p_tdm_rule_id = model.CustomerRegisterPanelModel.TDMRuleId.ToSafeString();
            command.p_tdm_penalty_id = model.CustomerRegisterPanelModel.TDMPenaltyId.ToSafeString();
            command.p_tdm_penalty_group_id = model.CustomerRegisterPanelModel.TDMPenaltyGroupId.ToSafeString();
            command.p_duration = model.CustomerRegisterPanelModel.Duration.ToSafeString();
            command.p_contract_flag = model.CustomerRegisterPanelModel.ContractFlag.ToSafeString();

            command.p_air_regist_package_array = airregists;
            command.p_air_regist_file_array = airImage;
            command.p_air_regist_splitter_array = airSplitter;
            command.p_air_regist_cpe_serial_array = airCPE;
            command.p_air_regist_cust_insi_array = custInsightRecord;
            command.p_air_regist_dcontract_array = dcontract;

            try
            {
                _insertSaveOrderNew911Command.Handle(command);
            }
            catch (Exception ex)
            {

            }

            return command.o_return_order_no.ToSafeString();
        }

        [HttpPost]
        public JsonResult GetAddressBilling(BillingAddressModel billingAddressParam)
        {
            //R23.04_18042023
            InterfaceLogCommand log = null;
            log = StartInterface(billingAddressParam, "/process/GetAddressBilling", billingAddressParam.GetBillingAddress.MobileNo, "", "GetAddressBilling");

            BillingAddressModel model = new BillingAddressModel();
            RelatedMobileServiceATNModel resultATN = new RelatedMobileServiceATNModel();

            if (!string.IsNullOrEmpty(billingAddressParam.GetBillingAddress.Mode))
            {
                try
                {
                    #region ConfigAddress
                    var lovAddress = base.LovData.Where(l => l.Type == "SCREEN" && l.Name == "L_BILLING_ADDR_NAME" && l.ActiveFlag == "Y").ToList();
                    string txtAddr = billingAddressParam.GetBillingAddress.languageCode == "1" ? lovAddress.Where(l => l.LovValue3 == "AddressNo").FirstOrDefault()?.LovValue1.ToSafeString() ?? "ที่อยู่"
                                        : lovAddress.Where(l => l.LovValue3 == "AddressNo").FirstOrDefault()?.LovValue2.ToSafeString() ?? "Address No.";
                    string txtMoo = billingAddressParam.GetBillingAddress.languageCode == "1" ? lovAddress.Where(l => l.LovValue3 == "MooNo").FirstOrDefault()?.LovValue1.ToSafeString() ?? "หมู่"
                                        : lovAddress.Where(l => l.LovValue3 == "MooNo").FirstOrDefault()?.LovValue2.ToSafeString() ?? "Moo No.";
                    string txtMooban = billingAddressParam.GetBillingAddress.languageCode == "1" ? lovAddress.Where(l => l.LovValue3 == "Mooban").FirstOrDefault()?.LovValue1.ToSafeString() ?? "หมู่บ้าน"
                                        : lovAddress.Where(l => l.LovValue3 == "Mooban").FirstOrDefault()?.LovValue2.ToSafeString() ?? "Village";
                    string txtBuilding = billingAddressParam.GetBillingAddress.languageCode == "1" ? lovAddress.Where(l => l.LovValue3 == "Building").FirstOrDefault()?.LovValue1.ToSafeString() ?? "อาคาร"
                                        : lovAddress.Where(l => l.LovValue3 == "Building").FirstOrDefault()?.LovValue2.ToSafeString() ?? "Building";
                    string txtFloor = billingAddressParam.GetBillingAddress.languageCode == "1" ? lovAddress.Where(l => l.LovValue3 == "Floor").FirstOrDefault()?.LovValue1.ToSafeString() ?? "ชั้น"
                                        : lovAddress.Where(l => l.LovValue3 == "Floor").FirstOrDefault()?.LovValue2.ToSafeString() ?? "Floor";
                    string txtRoom = billingAddressParam.GetBillingAddress.languageCode == "1" ? lovAddress.Where(l => l.LovValue3 == "Room").FirstOrDefault()?.LovValue1.ToSafeString() ?? "ห้อง"
                                        : lovAddress.Where(l => l.LovValue3 == "Room").FirstOrDefault()?.LovValue2.ToSafeString() ?? "Room";
                    string txtSoi = billingAddressParam.GetBillingAddress.languageCode == "1" ? lovAddress.Where(l => l.LovValue3 == "Soi").FirstOrDefault()?.LovValue1.ToSafeString() ?? "ซอย"
                                        : lovAddress.Where(l => l.LovValue3 == "Soi").FirstOrDefault()?.LovValue2.ToSafeString() ?? "Soi";
                    string txtStreet = billingAddressParam.GetBillingAddress.languageCode == "1" ? lovAddress.Where(l => l.LovValue3 == "Street").FirstOrDefault()?.LovValue1.ToSafeString() ?? "ถนน"
                                        : lovAddress.Where(l => l.LovValue3 == "Street").FirstOrDefault()?.LovValue2.ToSafeString() ?? "Road";
                    string txtTumbon = billingAddressParam.GetBillingAddress.languageCode == "1" ? lovAddress.Where(l => l.LovValue3 == "Subdistrict").FirstOrDefault()?.LovValue1.ToSafeString() ?? "แขวง/ตำบล"
                                        : lovAddress.Where(l => l.LovValue3 == "Subdistrict").FirstOrDefault()?.LovValue2.ToSafeString() ?? "Subdistrict";
                    string txtAmphur = billingAddressParam.GetBillingAddress.languageCode == "1" ? lovAddress.Where(l => l.LovValue3 == "District").FirstOrDefault()?.LovValue1.ToSafeString() ?? "เขต/อำเภอ"
                                        : lovAddress.Where(l => l.LovValue3 == "District").FirstOrDefault()?.LovValue2.ToSafeString() ?? "District";
                    string txtProvince = billingAddressParam.GetBillingAddress.languageCode == "1" ? lovAddress.Where(l => l.LovValue3 == "Province").FirstOrDefault()?.LovValue1.ToSafeString() ?? "จังหวัด"
                                        : lovAddress.Where(l => l.LovValue3 == "Province").FirstOrDefault()?.LovValue2.ToSafeString() ?? "Province";
                    string txtZipCode = billingAddressParam.GetBillingAddress.languageCode == "1" ? lovAddress.Where(l => l.LovValue3 == "Zipcode").FirstOrDefault()?.LovValue1.ToSafeString() ?? ""
                                        : lovAddress.Where(l => l.LovValue3 == "Zipcode").FirstOrDefault()?.LovValue2.ToSafeString() ?? "";
                    #endregion ConfigAddress

                    RelatedMobileServiceATNQuery queryServiceATN = new RelatedMobileServiceATNQuery()
                    {
                        channel = "FBB",
                        username = billingAddressParam.GetBillingAddress.MobileNo.ToSafeString(),
                        idCardNo = billingAddressParam.GetBillingAddress.IdCard.ToSafeString(),
                        mobileNo = billingAddressParam.GetBillingAddress.MobileNo.ToSafeString(),
                        mode = billingAddressParam.GetBillingAddress.Mode.ToSafeString(),
                        languageCode = billingAddressParam.GetBillingAddress.languageCode.ToSafeString()
                    };

                    resultATN = _queryProcessor.Execute(queryServiceATN);

                    if (resultATN.returnCode == "0" && resultATN.resultData != null && resultATN.resultData.Count > 0)
                    {
                        foreach (var itemBill in resultATN.resultData)
                        {
                            BillingAddressModel itemModel = new BillingAddressModel();

                            //Check Language Service ATN Return
                            int isLanguageResultATN = !string.IsNullOrEmpty(itemBill.Province) ? IsEnglishLanguage(itemBill.Province) : SiteSession.CurrentUICulture;

                            //Get ZipCode row id
                            string provinceFilter = itemBill.Province ?? "";
                            string amphurFilter = itemBill.Ampur ?? "";
                            string tumbonFilter = itemBill.Tumbon ?? "";
                            string zipCodeFilter = itemBill.ZipCode ?? "";

                            var zipCodeRowId = base.ZipCodeData(isLanguageResultATN) //fixed language because ATN returndata thai only  //base.ZipCodeData(SiteSession.CurrentUICulture)
                                 .Where(z => (!string.IsNullOrEmpty(z.Province) && z.Province.Equals(provinceFilter))
                                     && (!string.IsNullOrEmpty(z.Amphur) && z.Amphur.Contains(amphurFilter))
                                     && (!string.IsNullOrEmpty(z.Tumbon) && z.Tumbon.Equals(tumbonFilter))
                                     && (!string.IsNullOrEmpty(z.ZipCode) && z.ZipCode.Equals(zipCodeFilter)))
                                 .Select(z => z.ZipCodeId).FirstOrDefault();

                            itemModel.GetBillingAddressResponse.ZipCodeRowId = !string.IsNullOrEmpty(zipCodeRowId) ? zipCodeRowId : "";

                            if (billingAddressParam.GetBillingAddress.languageCode == "1")
                            {
                                itemModel.GetBillingAddressResponse.BaNo = itemBill.BaNo.ToSafeString();
                                itemModel.GetBillingAddressResponse.AddressDisplay = (!string.IsNullOrEmpty(itemBill.HomeId) ? txtAddr + " " + itemBill.HomeId + " " : "") + (!string.IsNullOrEmpty(itemBill.Moo) ? txtMoo + " " + itemBill.Moo + " " : "") +
                                    (!string.IsNullOrEmpty(itemBill.Mooban) ? txtMooban + itemBill.Mooban + " " : "") + (!string.IsNullOrEmpty(itemBill.Building) ? txtBuilding + " " + itemBill.Building + " " : "") +
                                    (!string.IsNullOrEmpty(itemBill.Floor) ? txtFloor + " " + itemBill.Floor + " " : "") + (!string.IsNullOrEmpty(itemBill.Room) ? txtRoom + " " + itemBill.Room + " " : "") +
                                    (!string.IsNullOrEmpty(itemBill.Soi) ? (itemBill.Soi.Contains("ซอย") || itemBill.Soi.Contains("ซ.") ? itemBill.Soi + " " : txtSoi + itemBill.Soi + " ") : "") +
                                    (!string.IsNullOrEmpty(itemBill.Street) ? (itemBill.Street.Contains("ถนน") || itemBill.Street.Contains("ถ.") ? itemBill.Street + " " : txtStreet + itemBill.Street + " ") : "") +
                                    (!string.IsNullOrEmpty(itemBill.Tumbon) ? txtTumbon + " " + itemBill.Tumbon + " " : "") + (!string.IsNullOrEmpty(itemBill.Ampur) ? txtAmphur + " " + itemBill.Ampur + " " : "") +
                                    (!string.IsNullOrEmpty(itemBill.Province) ? txtProvince + " " + itemBill.Province + " " : "") + (!string.IsNullOrEmpty(itemBill.ZipCode) ? itemBill.ZipCode : "");
                                itemModel.GetBillingAddressResponse.MobileNo = itemBill.MobileNo.ToSafeString();
                                itemModel.GetBillingAddressResponse.MobileNoGroup = itemBill.MobileNoGroup.ToSafeString();
                                itemModel.GetBillingAddressResponse.MobileNoDisplay = itemBill.MobileNoDisplay.ToSafeString();
                                itemModel.GetBillingAddressResponse.MobileNoGroupDisplay = itemBill.MobileNoGroupDisplay.ToSafeString();
                                itemModel.GetBillingAddressResponse.IdCard = billingAddressParam.GetBillingAddress.IdCard.ToSafeString();
                                itemModel.GetBillingAddressResponse.HomeId = !string.IsNullOrEmpty(itemBill.HomeId) ? itemBill.HomeId : "";
                                itemModel.GetBillingAddressResponse.Moo = !string.IsNullOrEmpty(itemBill.Moo) ? itemBill.Moo : "";
                                itemModel.GetBillingAddressResponse.Mooban = !string.IsNullOrEmpty(itemBill.Mooban) ? itemBill.Mooban : "";
                                itemModel.GetBillingAddressResponse.Building = !string.IsNullOrEmpty(itemBill.Building) ? itemBill.Building : "";
                                itemModel.GetBillingAddressResponse.Floor = !string.IsNullOrEmpty(itemBill.Floor) ? itemBill.Floor : "";
                                itemModel.GetBillingAddressResponse.Room = !string.IsNullOrEmpty(itemBill.Room) ? itemBill.Room : "";
                                itemModel.GetBillingAddressResponse.Soi = !string.IsNullOrEmpty(itemBill.Soi) ? itemBill.Soi : "";
                                itemModel.GetBillingAddressResponse.Street = !string.IsNullOrEmpty(itemBill.Street) ? itemBill.Street : "";
                                itemModel.GetBillingAddressResponse.Tumbon = !string.IsNullOrEmpty(itemBill.Tumbon) ? itemBill.Tumbon : "";
                                itemModel.GetBillingAddressResponse.Ampur = !string.IsNullOrEmpty(itemBill.Ampur) ? itemBill.Ampur : "";
                                itemModel.GetBillingAddressResponse.Province = !string.IsNullOrEmpty(itemBill.Province) ? itemBill.Province : "";
                                itemModel.GetBillingAddressResponse.ZipCode = !string.IsNullOrEmpty(itemBill.ZipCode) ? itemBill.ZipCode : "";

                                itemModel.GetBillingAddressResponse.BillCycleInfo = itemBill.BillCycleInfo; // "รอบบิลวันที่....";
                                itemModel.GetBillingAddressResponse.ChannelViewBill = itemBill.ChannelViewBill; // "TH SMS and eMail";
                                itemModel.GetBillingAddressResponse.BillMedia = itemBill.BillMedia; //R23.08 Billing eBill

                                List<LovValueModel> lovCycleBillInfo = base.LovData.Where(l => l.Name == "L_BILLING_ADDR_BILL_MEDIA" &&
                                                                            l.LovValue3.Contains(itemBill.BillMedia)).ToList();

                                if (string.IsNullOrEmpty(itemModel.GetBillingAddressResponse.ChannelViewBill))
                                {
                                    itemModel.GetBillingAddressResponse.ChannelViewBill = billingAddressParam.GetBillingAddress.languageCode == "1" ? lovCycleBillInfo[0].LovValue1 : lovCycleBillInfo[0].LovValue2;
                                }

                                itemModel.GetBillingAddressResponse.BillCycle = itemBill.BillCycle.ToSafeString();
                                if (itemModel.GetBillingAddressResponse.BillCycle != null)
                                {
                                    List<LovValueModel> lovCycleBill = base.LovData.Where(l => l.Name == itemModel.GetBillingAddressResponse.BillCycle
                                                                                  && l.Type == "BILL_CYCLE").ToList();

                                    itemModel.GetBillingAddressResponse.BillCycleFirstDay = lovCycleBill[0].LovValue1;
                                    itemModel.GetBillingAddressResponse.BillCycleLastDay = lovCycleBill[0].LovValue2;
                                }
                            }
                            else
                            {
                                itemModel.GetBillingAddressResponse.BaNo = itemBill.BaNo.ToSafeString();
                                itemModel.GetBillingAddressResponse.AddressDisplay = (!string.IsNullOrEmpty(itemBill.HomeId) ? txtAddr + itemBill.HomeId + " " : "") + (!string.IsNullOrEmpty(itemBill.Moo) ? txtMoo + itemBill.Moo + " " : "") +
                                    (!string.IsNullOrEmpty(itemBill.Mooban) ? itemBill.Mooban + " " + txtMooban + ", " : "") + (!string.IsNullOrEmpty(itemBill.Building) ? itemBill.Building + " " + txtBuilding + ", " : "") +
                                    (!string.IsNullOrEmpty(itemBill.Floor) ? txtFloor + " " + itemBill.Floor + " " : "") + (!string.IsNullOrEmpty(itemBill.Room) ? txtRoom + " " + itemBill.Room + " " : "") +
                                    (!string.IsNullOrEmpty(itemBill.Soi) ? txtSoi + " " + itemBill.Soi + " " : "") + (!string.IsNullOrEmpty(itemBill.Street) ? itemBill.Street + " " + txtStreet + ", " : "") +
                                    (!string.IsNullOrEmpty(itemBill.Tumbon) ? itemBill.Tumbon + " " + txtTumbon + ", " : "") + (!string.IsNullOrEmpty(itemBill.Ampur) ? itemBill.Ampur + " " + txtAmphur + ", " : "") +
                                    (!string.IsNullOrEmpty(itemBill.Province) ? itemBill.Province + " " + txtProvince + " " : "") + (!string.IsNullOrEmpty(itemBill.ZipCode) ? itemBill.ZipCode : "");
                                itemModel.GetBillingAddressResponse.MobileNo = itemBill.MobileNo.ToSafeString();
                                itemModel.GetBillingAddressResponse.MobileNoGroup = itemBill.MobileNoGroup.ToSafeString();
                                itemModel.GetBillingAddressResponse.MobileNoDisplay = itemBill.MobileNoDisplay.ToSafeString();
                                itemModel.GetBillingAddressResponse.MobileNoGroupDisplay = itemBill.MobileNoGroupDisplay.ToSafeString();
                                itemModel.GetBillingAddressResponse.IdCard = billingAddressParam.GetBillingAddress.IdCard.ToSafeString();
                                itemModel.GetBillingAddressResponse.HomeId = !string.IsNullOrEmpty(itemBill.HomeId) ? itemBill.HomeId : "";
                                itemModel.GetBillingAddressResponse.Moo = !string.IsNullOrEmpty(itemBill.Moo) ? itemBill.Moo : "";
                                itemModel.GetBillingAddressResponse.Mooban = !string.IsNullOrEmpty(itemBill.Mooban) ? itemBill.Mooban : "";
                                itemModel.GetBillingAddressResponse.Building = !string.IsNullOrEmpty(itemBill.Building) ? itemBill.Building : "";
                                itemModel.GetBillingAddressResponse.Floor = !string.IsNullOrEmpty(itemBill.Floor) ? itemBill.Floor : "";
                                itemModel.GetBillingAddressResponse.Room = !string.IsNullOrEmpty(itemBill.Room) ? itemBill.Room : "";
                                itemModel.GetBillingAddressResponse.Soi = !string.IsNullOrEmpty(itemBill.Soi) ? itemBill.Soi : "";
                                itemModel.GetBillingAddressResponse.Street = !string.IsNullOrEmpty(itemBill.Street) ? itemBill.Street : "";
                                itemModel.GetBillingAddressResponse.Tumbon = !string.IsNullOrEmpty(itemBill.Tumbon) ? itemBill.Tumbon : "";
                                itemModel.GetBillingAddressResponse.Ampur = !string.IsNullOrEmpty(itemBill.Ampur) ? itemBill.Ampur : "";
                                itemModel.GetBillingAddressResponse.Province = !string.IsNullOrEmpty(itemBill.Province) ? itemBill.Province : "";
                                itemModel.GetBillingAddressResponse.ZipCode = !string.IsNullOrEmpty(itemBill.ZipCode) ? itemBill.ZipCode : "";

                                itemModel.GetBillingAddressResponse.BillCycleInfo = itemBill.BillCycleInfo; // "รอบบิลวันที่....";
                                itemModel.GetBillingAddressResponse.ChannelViewBill = itemBill.ChannelViewBill; // "EN SMS and eMail";
                                itemModel.GetBillingAddressResponse.BillMedia = itemBill.BillMedia; //R23.08 Billing eBill

                                List<LovValueModel> lovCycleBillInfo = base.LovData.Where(l => l.Name == "L_BILLING_ADDR_BILL_MEDIA" &&
                                                                            l.LovValue3.Contains(itemBill.BillMedia)).ToList();

                                if (string.IsNullOrEmpty(itemModel.GetBillingAddressResponse.ChannelViewBill))
                                {
                                    itemModel.GetBillingAddressResponse.ChannelViewBill = billingAddressParam.GetBillingAddress.languageCode == "1" ? lovCycleBillInfo[0].LovValue1 : lovCycleBillInfo[0].LovValue2;
                                }

                                itemModel.GetBillingAddressResponse.BillCycle = itemBill.BillCycle.ToSafeString();
                                if (itemModel.GetBillingAddressResponse.BillCycle != null)
                                {
                                    List<LovValueModel> lovCycleBill = base.LovData.Where(l => l.Name == itemModel.GetBillingAddressResponse.BillCycle
                                                                                  && l.Type == "BILL_CYCLE").ToList();

                                    itemModel.GetBillingAddressResponse.BillCycleFirstDay = lovCycleBill[0].LovValue1;
                                    itemModel.GetBillingAddressResponse.BillCycleLastDay = lovCycleBill[0].LovValue2;
                                }
                            }

                            model.GetBillingAddressResponseList.Add(itemModel.GetBillingAddressResponse);
                        }
                    }

                    model.BillCycleLov = base.LovData.Where(l => l.Type == "BILL_CYCLE").ToList();

                    //ReturnMessage
                    if (resultATN.returnCode == "0")
                    {
                        model.ReturnCode = "0";
                        model.ReturnRelated = resultATN.returnRelated.ToSafeString();
                        model.ErrorMessage = string.Empty;
                    }
                    else
                    {
                        var lovMessage = base.LovData.Where(l => l.Type == "SCREEN" && l.Name == "L_BILLING_ADDR_API_MESSAGE" && l.ActiveFlag == "Y" && l.OrderBy == resultATN.returnCode.ToSafeDecimal()).FirstOrDefault();
                        model.ReturnCode = resultATN.returnCode;
                        model.ErrorMessage = billingAddressParam.GetBillingAddress.languageCode == "1" ? lovMessage.LovValue1 : lovMessage.LovValue2;
                    }

                    EndInterface(model, log, billingAddressParam.GetBillingAddress.MobileNo, "Success", "");
                }
                catch (Exception ex)
                {
                    var lovMessage = base.LovData.Where(l => l.Type == "SCREEN" && l.Name == "L_BILLING_ADDR_API_MESSAGE" && l.ActiveFlag == "Y" && l.OrderBy == -1).FirstOrDefault();
                    model.ReturnCode = "-1";
                    model.ErrorMessage = billingAddressParam.GetBillingAddress.languageCode == "1" ? lovMessage.LovValue1 : lovMessage.LovValue2; //ex.Message;
                    EndInterface("", log, billingAddressParam.GetBillingAddress.MobileNo, "Failed", ex.GetErrorMessage());
                }
            }
            else
            {
                model.ReturnCode = "-1";
                model.ErrorMessage = "Failed : Data Related or Non-Related is empty";
                EndInterface("", log, billingAddressParam.GetBillingAddress.MobileNo, "Failed", "Failed : Data Related or Non-Related is empty");
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        //R23.05.2023 Created: THOTST49
        private bool InsertConsentLog(string transacionId, string contactMobile, bool valueFlag, string airNo, string ipClient)
        {
            var command = new InsertConsentLogNewRegisterCommand()
            {
                InTransactionId = transacionId,
                ContactMobile = contactMobile,
                ValueFlag = valueFlag,
                RefOrderNo = airNo,
                ClientIp = ipClient
            };

            _insertConsentLogNewRegisterCommand.Handle(command);

            return command.ReturnCode == 200;
        }

        //R23.05 CheckFraud
        public JsonResult OnlineAuthen(GetOnlineAuthenRequestTokenQuery request) => Json(_queryProcessor.Execute(request), JsonRequestBehavior.AllowGet);

        public JsonResult GetCheckFraud(GetOnlineQueryCheckFraudQuery request) => Json(_queryProcessor.Execute(request), JsonRequestBehavior.AllowGet);

        public JsonResult GetLimitAssetWatchList(GetCustomerRiskLimitAssetWatchListQuery request)
        {
            var config = base.LovData.Where(l => l.Name.Contains("Athena_Limit_Asset_Watchlist") && l.ActiveFlag == "Y")
            .Select(s => new AthenaLimitAssetConfig { LOVName = s.Name, LOVVal = s.LovValue1 });

            request.Channel = GetConfigValue(config, "channel");
            request.UserId = GetConfigValue(config, "userId");
            request.Username = GetConfigValue(config, "username");
            request.AssetType = string.IsNullOrEmpty(request.AssetType) || string.IsNullOrEmpty(request.AssetType.Trim()) ? "postpaid" : request.AssetType.ToLower();

            return Json(_queryProcessor.Execute(request), JsonRequestBehavior.AllowGet);
        }

        private string GetConfigValue(IEnumerable<AthenaLimitAssetConfig> config, string target) => config.Where(w => w.LOVName.Contains(target)).Select(s => s.LOVVal).FirstOrDefault();

        [HttpPost]
        public JsonResult CheckFraudCentralize(CheckFraudCentralizeQuery request) => Json(_queryProcessor.Execute(request), JsonRequestBehavior.AllowGet);
        //end R23.05 CheckFraud

        //R23.06 IP Camera
        private string InsertRegisterCloudIPCamera(QuickWinPanelModel model, string p_customerRowID, string p_returnOrderNo)
        {
            var command = new InsertCloudIPCameraCommand
            {
                ActionType = ActionType.Insert,
                QuickWinPanelModel = model,
                p_cust_row_id = p_customerRowID,
                p_register_flag = model.SummaryPanelModel.VAS_FLAG == "8" ? "Existing" : "WEB",
                p_package_service_code = string.Empty,
                p_product_subtype = string.Empty,
                p_package_type = string.Empty,
                p_package_code = string.Empty,
                p_package_price = 0,
                p_package_count = 0,
                p_return_order = p_returnOrderNo,
                p_created_by = string.Empty,
                p_fibrenet_id = string.Empty
            };

            _insertRegIpCameraCommand.Handle(command);

            return command.ret_code.ToSafeString();
        }

        //R23.08 Check ATN Language
        private static int IsEnglishLanguage(string value)
        {
            bool isEngValue = Regex.IsMatch(value, @"[A-Za-z]+");
            return isEngValue != true ? 1 : 2;
        }

        //R23.06 kunlp885 Max - Ontop IP camera
        public bool AllowIPCamera() => base.LovData.Where(t => t.Name == "IPCamera_Use_Flag").Any(t => t.LovValue1 == "Y");

        //23.09 kunlp885 Max - Ontop IP camera
        public List<LovScreenValueModel> GetListByLOVName(string lovName)
            => base.LovData.Where(l => l.Name == lovName && l.ActiveFlag == "Y").Select(s => new LovScreenValueModel
            {
                DisplayValue = SiteSession.CurrentUICulture.IsThaiCulture() ? s.LovValue1 : s.LovValue2,
                OrderByPDF = s.OrderBy
            }).ToList();

        private JsonNetResult GetListPackagebySFFPromoMockup()
        {
            return new JsonNetResult
            {
                Data = new List<PackageModel>
                {
                    new PackageModel
                    {
                        SFF_PROMOTION_CODE = "P230619107",
                        PACKAGE_TYPE = "14",
                        PRODUCT_SUBTYPE = "IP_CAMERA",
                        PRICE_CHARGE = 99m
                    }
                }
            };
        }
        //end R23.06 kunlp885 Max - Ontop IP camera


        //R23.08 IDS Login officer
        public ActionResult MenuOfficerAuthen(string code) => RedirectToAction("MenuOfficerAuthen", "Officer", new { code });
        //end R23.08 IDS Login officer

        //R23.05 CheckFraud
        public JsonResult GetCheckFraudLocal(GetOnlineQueryCheckFraudQuery query)
        {
            try
            {
                var url0 = base.LovData.Where(l => l.Name == "URL_GET_CHECK_FRAUD" || l.Text == "URL_GET_CHECK_FRAUD" || l.DefaultValue == "URL_GET_CHECK_FRAUD");


                //var target = JsonConvert.DeserializeObject<List<string>>(query.OFFERINGJSON);
                var target = query.OFFERING;

                var lovConfigs = base.LovData.Where(l => l.Name == "LIST_SPECIAL_OFFER" && l.ActiveFlag == "Y" && target.Contains(l.Text)).Select(s => s.LovValue1);

                var request = RequestBuilder(query);
                string url = base.LovData.Where(l => l.Name == "URL_GET_CHECK_FRAUD" && l.ActiveFlag == "Y").Select(s => s.LovValue1).FirstOrDefault();
                url = url ?? "https://sit-f22raptor.az.intra.ais/onlinequery/v3/query/getcheckfraud";
                var client = new RestClient(url);
                var response = client.Execute(request);

                var ttt = MockCheckFraud(2);
                var jjj = new JsonNetResult
                {
                    Data = ttt,
                    Formatting = Newtonsoft.Json.Formatting.Indented
                };

                //return Json(jjj, JsonRequestBehavior.AllowGet) ;
                return Json(ttt, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex, JsonRequestBehavior.AllowGet);
            }
        }

        private RestRequest RequestBuilder(GetOnlineQueryCheckFraudQuery query)
        {
            var request = new RestRequest();
            request.Method = Method.POST;
            request.AddHeader("Authorization", "Bearer " + query.ONLINEAUTH_TOKEN);
            request.AddParameter("application/json", query, ParameterType.RequestBody);

            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback =
                (s, certificate, chain, sslPolicyErrors) => true;

            return request;
        }

        private GetOnlineQueryCheckFraudQueryModel MockCheckFraud(int number)
        {
            var result = new GetOnlineQueryCheckFraudQueryModel
            {
                RESULT_CODE = "20000",
                RESULT_DESC = "Success",
                TRANSACTION_ID = "trannsaction",
                CHECK_FRAUD_INFO = new checkFraudInfo()
            };
            switch (number)
            {
                case 1:
                    result.CHECK_FRAUD_INFO.NOTIFY_POPUP = "Y";
                    result.CHECK_FRAUD_INFO.NOTIFY_MESSAGE = "แนะนำ Renew AIS Fibre เดิม ให้ลูกค้าติดต่อ AIS Shop , Call Center 1175 กรณีลูกค้าไม่สะดวกแนบ empowerment เพิ่มเติม";
                    result.CHECK_FRAUD_INFO.VERIFY_REASON = "ID card + Address Dup +Status Order เดิม =Disconnect Customer Request,Terminate+ Terminated <90 วัน ";
                    result.CHECK_FRAUD_INFO.FLAG_GO_NOGO = "Go";
                    result.CHECK_FRAUD_INFO.AUTO_CREATE_PROSPECT = "S";
                    result.CHECK_FRAUD_INFO.FRAUD_SCORE = "218";
                    result.CHECK_FRAUD_INFO.FRAUD_REASONS
                        = new List<FRAUDREASONS>
                        {
                        new FRAUDREASONS()
                        {
                            REASON="ID Card Duplicated",
                            SCORE= 100},
                        new FRAUDREASONS
                        {
                            REASON="Contact Duplicated",
                            SCORE= 100
                        },
                        new FRAUDREASONS
                        {
                              REASON="Contact No FMC <= 2",
                            SCORE= 18
                        }};
                    result.CHECK_FRAUD_INFO.CEN_FRAUD_FLAG = "Y";
                    break;

                case 2:
                    result.CHECK_FRAUD_INFO.NOTIFY_POPUP = "Y";
                    result.CHECK_FRAUD_INFO.NOTIFY_MESSAGE = "Internet no. มียอดค้าง   \r\n\r\n-แนะนำชำระยอดค้างก่อนทำรายการสมัครใหม่";
                    result.CHECK_FRAUD_INFO.VERIFY_REASON = "ID card +Contract no. Dup +Status Order เดิม  = Suspend, Disconnect Customer Request,Terminate +AR balance (CA) (Over Due date) ";
                    result.CHECK_FRAUD_INFO.FLAG_GO_NOGO = "No Go";
                    result.CHECK_FRAUD_INFO.FRAUD_SCORE = "218";
                    result.CHECK_FRAUD_INFO.FRAUD_REASONS = new List<FRAUDREASONS>
                        {
                        new FRAUDREASONS()
                        {
                            REASON="ID Card Duplicated",
                            SCORE= 100},
                        new FRAUDREASONS
                        {
                            REASON="Contact Duplicated",
                            SCORE= 100
                        },
                        new FRAUDREASONS
                        {
                              REASON="Contact No FMC <= 2",
                            SCORE= 18
                        }};
                    result.CHECK_FRAUD_INFO.CEN_FRAUD_FLAG = "Y";
                    break;

                case 3:
                    result.CHECK_FRAUD_INFO.VERIFY_REASON = "โปร Serenade";
                    result.CHECK_FRAUD_INFO.AUTO_CREATE_PROSPECT = "S";
                    result.CHECK_FRAUD_INFO.FRAUD_SCORE = "0";
                    result.CHECK_FRAUD_INFO.CEN_FRAUD_FLAG = "Y";
                    break;

                case 4:
                    result.CHECK_FRAUD_INFO.VERIFY_REASON = "ลูกค้า Non-Residential";
                    result.CHECK_FRAUD_INFO.AUTO_CREATE_PROSPECT = "N";
                    result.CHECK_FRAUD_INFO.FRAUD_SCORE = "0";
                    result.CHECK_FRAUD_INFO.CEN_FRAUD_FLAG = "Y";
                    break;

                case 5:
                    result.RESULT_CODE = "40003";
                    result.RESULT_DESC = "Request parameter(s) should not be null or empty. (Parameter 'CONTACT_MOBILE_NO')";
                    result.CHECK_FRAUD_INFO.CEN_FRAUD_FLAG = "N";
                    break;

                default:
                    result.RESULT_CODE = "40001";
                    result.RESULT_DESC = "Data Not Found.";
                    result.CHECK_FRAUD_INFO.AUTO_CREATE_PROSPECT = "N";
                    result.CHECK_FRAUD_INFO.CEN_FRAUD_FLAG = "N";
                    break;
            }
            return result;
        }
        //R23.05 CheckFraud
        [HttpPost]
        public ActionResult UploadImageFraud(string cateType, string cardNo, string cardType, string register_dv, string AisAirNumber)
        {

            #region Get IP Address Interface Log (Update 17.2)
            // Get IP Address
            string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress)) ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            #endregion

            string browserCapa = "";
            var bc = Request.Browser;
            browserCapa = "Browser Capabilities:\r\n";
            browserCapa = browserCapa + "Type = " + bc.Type + "\r\n";
            browserCapa = browserCapa + "Name = " + bc.Browser + "\r\n";
            browserCapa = browserCapa + "Version = " + bc.Version + "\r\n";
            browserCapa = browserCapa + "Major Version = " + bc.MajorVersion + "\r\n";
            browserCapa = browserCapa + "Minor Version = " + bc.MinorVersion + "\r\n";
            browserCapa = browserCapa + "Platform = " + bc.Platform + "\r\n";
            browserCapa = browserCapa + "Is Beta = " + bc.Beta + "\r\n";
            browserCapa = browserCapa + "Is Crawler = " + bc.Crawler + "\r\n";
            browserCapa = browserCapa + "Is AOL = " + bc.AOL + "\r\n";
            browserCapa = browserCapa + "Is Win16 = " + bc.Win16 + "\r\n";
            browserCapa = browserCapa + "Is Win32 = " + bc.Win32 + "\r\n";
            browserCapa = browserCapa + "Supports Frames = " + bc.Frames + "\r\n";
            browserCapa = browserCapa + "Supports Tables = " + bc.Tables + "\r\n";
            browserCapa = browserCapa + "Supports Cookies = " + bc.Cookies + "\r\n";
            browserCapa = browserCapa + "Is Mobile = " + bc.IsMobileDevice + "\r\n";
            browserCapa = browserCapa + "MobileDeviceManufacturer = " + bc.MobileDeviceManufacturer + "\r\n";
            browserCapa = browserCapa + "MobileDeviceModel = " + bc.MobileDeviceModel + "\r\n";
            browserCapa = browserCapa + "Device From JS = " + register_dv + "\r\n";

            string transactionId = (AisAirNumber + ipAddress).ToSafeString();
            InterfaceLogCommand log = null;
            log = StartInterface("IdcardNo:" + cardNo + "\r\n" + browserCapa, "UploadImage", transactionId, cardNo, "WEB");

            var listOfBase64Photo = Session["base64photo"] as Dictionary<string, string>;

            if (Request.Files.Count > 0 || (listOfBase64Photo != null && listOfBase64Photo.Any()))
            {
                try
                {


                    List<string> Arr_files = new List<string>();
                    QuickWinPanelModel model = new QuickWinPanelModel();
                    HttpPostedFileBase[] filesPosted;

                    if (Request.Files.Count > 0)
                    {
                        HttpFileCollectionBase files = Request.Files;
                        filesPosted = new HttpPostedFileBase[files.Count];
                        for (int i = 0; i < files.Count; i++)
                        {
                            filesPosted[i] = files[i];
                        }
                    }
                    else
                    {
                        //TODO: get image from card reader
                        filesPosted = new HttpPostedFileBase[listOfBase64Photo.Count()];
                        var ikey = 0;
                        foreach (var item in listOfBase64Photo)
                        {
                            byte[] byteArray = Convert.FromBase64String(item.Value);
                            HttpPostedFile postfile = ConstructHttpPostedFile(byteArray, item.Key + ".jpg", "image/jpeg");
                            filesPosted[ikey] = new HttpPostedFileWrapper(postfile);
                            ikey = ikey + 1;
                        }

                    }

                    model.Register_device = register_dv;
                    model.CustomerRegisterPanelModel.CateType = cateType;
                    model.CustomerRegisterPanelModel.L_CARD_NO = cardNo;
                    model.CustomerRegisterPanelModel.L_CARD_TYPE = cardType;

                    model.CoveragePanelModel.L_CONTACT_PHONE = AisAirNumber;
                    model.ClientIP = ipAddress;

                    filesPostedRegisterTempStep = filesPosted;
                    model = SaveFileImageFraud(filesPosted, model);

                    if (Session["logForUpdateImage"] != null)
                    {
                        InterfaceUpdateImage(((InterfaceLogCommand)Session["logForUpdateImage"]), model.CustomerRegisterPanelModel.ListImageFile[0].FileName);
                        Session["logForUpdateImage"] = null;
                    }

                    EndInterface("", log, transactionId, "Success", "");
                    if (model.CustomerRegisterPanelModel.ListImageFile.Any())
                        return Json(model.CustomerRegisterPanelModel.ListImageFile);
                    else
                        throw new Exception("Null ListImageFile");
                }
                catch (Exception ex)
                {
                    EndInterface("", log, transactionId, "ERROR", "Error Message: " + ex.GetErrorMessage() + "\r\nStack Trace: " + ex.RenderExceptionMessage());


                    Logger.Info("Error Upload Image:" + ex.GetErrorMessage());
                    Logger.Info("Error Upload Image With Stack Trace : " + ex.RenderExceptionMessage());
                    return Json(false);
                }
            }
            else
            {
                return Json(false);
            }
        }
        public void SaveRegisterFraud(QuickWinPanelModel model, string cust_row_id)
        {
            try
            {
                var fraud_reason = "";
                if (model.CustomerRegisterPanelModel.CHECK_FRAUD_INFO.FRAUD_REASONS != null && model.CustomerRegisterPanelModel.CHECK_FRAUD_INFO.FRAUD_REASONS.Count > 0)
                {
                    for (var i = 0; model.CustomerRegisterPanelModel.CHECK_FRAUD_INFO.FRAUD_REASONS.Count() >= i; i++)
                    {
                        fraud_reason = fraud_reason + "REASON : " + model.CustomerRegisterPanelModel.CHECK_FRAUD_INFO.FRAUD_REASONS[i].REASON +
                            "SCORE : " + model.CustomerRegisterPanelModel.CHECK_FRAUD_INFO.FRAUD_REASONS[i].SCORE + "//";
                    }
                }
                var command = new SaveRegisterFraudCommand()
                {
                    p_cust_row_id = cust_row_id,
                    p_created_by = model.CustomerRegisterPanelModel.L_STAFF_ID,
                    p_cen_fraud_flag = model.CustomerRegisterPanelModel.CHECK_FRAUD_INFO.CEN_FRAUD_FLAG,
                    p_verify_reason_cen = model.CustomerRegisterPanelModel.CHECK_FRAUD_INFO.VERIFY_REASON,
                    p_fraud_score = model.CustomerRegisterPanelModel.CHECK_FRAUD_INFO.FRAUD_SCORE,
                    p_air_fraud_reason_array = fraud_reason,
                    p_auto_create_prospect_flag = model.CustomerRegisterPanelModel.CHECK_FRAUD_INFO.AUTO_CREATE_PROSPECT,
                    p_cs_note_popup = model.CustomerRegisterPanelModel.CS_NOTE_POPUP,
                    p_url_attach_popup = model.CustomerRegisterPanelModel.ListImageFile[7] != null ? model.CustomerRegisterPanelModel.ListImageFile[7].FileName : ""
                };
                _saveRegisterFraudCommand.Handle(command);
            }
            catch (Exception ex)
            {
                Logger.Info("Error SaveRegisterFraud :" + ex.GetErrorMessage());
            }
        }
        public void SaveRegisterFraudNoGo(QuickWinPanelModel model, string status, string message)
        {
            try
            {
                var fraud_reason = "";
                var p_address_duplicated_flag = "";
                var p_id_duplicated_flag = "";
                var p_contact_duplicated_flag = "";
                var p_contact_not_active_flag = "";
                var p_contact_no_fmc_flag = "";
                var p_watch_list_dealer_flag = "";
                var p_sale_dealer_direct_sale_flag = "";
                if (model.CustomerRegisterPanelModel.CHECK_FRAUD_INFO.FRAUD_REASONS != null && model.CustomerRegisterPanelModel.CHECK_FRAUD_INFO.FRAUD_REASONS.Count > 0)
                {
                    for (var i = 0; model.CustomerRegisterPanelModel.CHECK_FRAUD_INFO.FRAUD_REASONS.Count() >= i; i++)
                    {
                        fraud_reason = fraud_reason + "REASON : " + model.CustomerRegisterPanelModel.CHECK_FRAUD_INFO.FRAUD_REASONS[i].REASON +
                            "SCORE : " + model.CustomerRegisterPanelModel.CHECK_FRAUD_INFO.FRAUD_REASONS[i].SCORE + "//";
                    }
                    p_address_duplicated_flag = model.CustomerRegisterPanelModel.CHECK_FRAUD_INFO.FRAUD_REASONS.Count(x => x.REASON == model.CustomerRegisterPanelModel.address_duplicated_flag) > 0 ? "Y" : "N";
                    p_id_duplicated_flag = model.CustomerRegisterPanelModel.CHECK_FRAUD_INFO.FRAUD_REASONS.Count(x => x.REASON == model.CustomerRegisterPanelModel.id_duplicated_flag) > 0 ? "Y" : "N";
                    p_contact_duplicated_flag = model.CustomerRegisterPanelModel.CHECK_FRAUD_INFO.FRAUD_REASONS.Count(x => x.REASON == model.CustomerRegisterPanelModel.contact_duplicated_flag) > 0 ? "Y" : "N";
                    p_contact_not_active_flag = model.CustomerRegisterPanelModel.CHECK_FRAUD_INFO.FRAUD_REASONS.Count(x => x.REASON == model.CustomerRegisterPanelModel.contact_not_active_flag) > 0 ? "Y" : "N";
                    p_contact_no_fmc_flag = model.CustomerRegisterPanelModel.CHECK_FRAUD_INFO.FRAUD_REASONS.Count(x => x.REASON == model.CustomerRegisterPanelModel.contact_no_fmc_flag) > 0 ? "Y" : "N";
                    p_watch_list_dealer_flag = model.CustomerRegisterPanelModel.CHECK_FRAUD_INFO.FRAUD_REASONS.Count(x => x.REASON == model.CustomerRegisterPanelModel.watch_list_dealer_flag) > 0 ? "Y" : "N";
                    p_sale_dealer_direct_sale_flag = model.CustomerRegisterPanelModel.CHECK_FRAUD_INFO.FRAUD_REASONS.Count(x => x.REASON == model.CustomerRegisterPanelModel.sale_dealer_direct_sale_flag) > 0 ? "Y" : "N";
                }
                var install_address = "";
                if (model.CoveragePanelModel.Address != null)
                {
                    install_address = model.CoveragePanelModel.Address.L_HOME_NUMBER_1 + " " + model.CoveragePanelModel.Address.L_MOO + " " + model.CoveragePanelModel.Address.L_MOOBAN
                        + " " + model.CoveragePanelModel.Address.L_BUILD_NAME + " " + model.CoveragePanelModel.Address.L_FLOOR + " " + model.CoveragePanelModel.Address.L_ROOM
                        + " " + model.CoveragePanelModel.Address.L_SOI + " " + model.CoveragePanelModel.Address.L_ROAD + " " + model.CoveragePanelModel.Address.L_TUMBOL
                        + " " + model.CoveragePanelModel.Address.L_AMPHUR + " " + model.CoveragePanelModel.Address.L_PROVINCE + " " + model.CoveragePanelModel.Address.L_ZIPCODE;
                }
                decimal? p_entry_fee = 0;
                var p_promotion_name = "";
                decimal? p_promotion_price = 0;
                string p_promotion_ontop = "";
                decimal? p_price_net = 0;
                if (model.SummaryPanelModel.PackageModelList != null && model.SummaryPanelModel.PackageModelList.Count > 0)
                {
                    p_entry_fee = model.SummaryPanelModel.PackageModelList.Find(x => x.PACKAGE_TYPE == "1").RECURRING_CHARGE;
                    p_promotion_name = model.SummaryPanelModel.PackageModelList.Find(x => x.PACKAGE_TYPE == "1").SFF_PRODUCT_NAME;
                    p_promotion_price = model.SummaryPanelModel.PackageModelList.Find(x => x.PACKAGE_TYPE == "1").PRICE_CHARGE;
                    p_promotion_ontop = model.SummaryPanelModel.PackageModelList.Find(x => x.PACKAGE_TYPE == "4").SFF_PRODUCT_NAME;
                    p_price_net = model.SummaryPanelModel.PackageModelList.Find(x => x.PACKAGE_TYPE == "4").PRICE_CHARGE;
                }
                var command = new SaveRegisterFraudNoGoCommand()
                {
                    p_customer_type = model.CustomerRegisterPanelModel.CateType,
                    p_customer_name = model.CustomerRegisterPanelModel.L_FIRST_NAME + " " + model.CustomerRegisterPanelModel.L_LAST_NAME,
                    p_id_card_no = model.CustomerRegisterPanelModel.L_CARD_NO,
                    p_install_address = install_address,
                    p_product_subtype = model.CoveragePanelModel.AccessMode,
                    p_entry_fee = p_entry_fee,
                    p_operator = model.CoveragePanelModel.SAVEORDER_OWNER_PRODUCT,
                    p_promotion_name = p_promotion_name,
                    p_promotion_ontop = p_promotion_ontop,
                    p_promotion_price = p_promotion_price,
                    p_price_net = p_promotion_price - p_price_net,
                    p_cs_note = model.CustomerRegisterPanelModel.ServiceLevel + model.CustomerRegisterPanelModel.L_FOR_CS_TEAM,
                    p_location_code = model.CustomerRegisterPanelModel.L_LOC_CODE,
                    p_location_name = model.OfficerInfoPanelModel.outTitle + " " + model.OfficerInfoPanelModel.outCompanyName.Trim(),
                    p_chn_sales_name = model.OfficerInfoPanelModel.outChnSales,
                    p_asc_code = model.CustomerRegisterPanelModel.L_ASC_CODE,
                    p_asc_name = model.OfficerInfoPanelModel.outASCTitleThai + model.OfficerInfoPanelModel.outASCPartnerName,
                    p_region_customer = model.CoveragePanelModel.SAVEORDER_REGION,
                    p_region_sale = model.OfficerInfoPanelModel.outLocationRegion,
                    p_fraud_score = model.CustomerRegisterPanelModel.CHECK_FRAUD_INFO.FRAUD_SCORE.ToSafeDecimal(),
                    p_waiting_time_slot_flag = "N",
                    p_project_name = model.CustomerRegisterPanelModel.Project_name,
                    p_address_duplicated_flag = p_address_duplicated_flag,
                    p_id_duplicated_flag = p_id_duplicated_flag,
                    p_contact_duplicated_flag = p_contact_duplicated_flag,
                    p_contact_not_active_flag = p_contact_not_active_flag,
                    p_contact_no_fmc_flag = p_contact_no_fmc_flag,
                    p_watch_list_dealer_flag = p_watch_list_dealer_flag,
                    p_sale_dealer_direct_sale_flag = p_sale_dealer_direct_sale_flag,
                    p_relate_mobile_segment = model.CustomerRegisterPanelModel.RELATE_MOBILE_SEGMENT,
                    p_charge_type = model.CoveragePanelModel.ChargeType,
                    p_service_month = model.CoveragePanelModel.SffServiceYear,
                    p_use_id_card_address_flag = model.CustomerRegisterPanelModel.AddressPanelModelSendDoc.L_HOME_NUMBER_1 == model.CustomerRegisterPanelModel.AddressPanelModelSetup.L_HOME_NUMBER_1 ? "Y" : "N",
                    p_reason_verify = model.CustomerRegisterPanelModel.CHECK_FRAUD_INFO.VERIFY_REASON,
                    p_created_by = model.CustomerRegisterPanelModel.L_STAFF_ID,
                    p_updated_by = model.CustomerRegisterPanelModel.L_STAFF_ID,
                    p_flag_send_xml = status,
                    p_message_send_xml = message,
                };
                _saveRegisterFraudNoGoCommand.Handle(command);
            }
            catch (Exception ex)
            {
                Logger.Info("Error SaveRegisterFraudNoGo :" + ex.GetErrorMessage());
            }
        }
        //end R23.05 CheckFraud

        /*private bool checkLoadingBlock(string Caller, string Data)
        {
            ViewBag.hasLoadingBlock = true;
            if (string.IsNullOrEmpty(Session["hasLoadingBlock"].ToSafeString()))
            {
                Session["hasLoadingBlock"] = "Success";
                ViewBag.LoadingBlockCaller = Caller;
                ViewBag.LoadingBlockData = Data;
                return true;
            }
            else
            {
                Session["hasLoadingBlock"] = "";
                return false;
            }
        }
        */
    }
    public class File_Remove
    {
        public string file_name { get; set; }
    }

    [Serializable]
    public class SplitterInfo
    {
        [XmlElement("Distance")]
        public string Distance { get; set; }

        [XmlElement("DistanceType")]
        public string DistanceType { get; set; }

        [XmlElement("Name")]
        public string Name { get; set; }
    }

    [Serializable, XmlRoot("SplitterList")]
    public class SplitterInfoList
    {
        [XmlElement("Splitter")]
        public List<SplitterInfo> Splitter { get; set; }
    }

    [Serializable]
    public class SplitterInfo3bb
    {
        [XmlElement("Distance")]
        public string Distance { get; set; }

        [XmlElement("DistanceType")]
        public string DistanceType { get; set; }

        [XmlElement("SplitterCode")]
        public string SplitterCode { get; set; }

        [XmlElement("SplitterAlias")]
        public string SplitterAlias { get; set; }

        [XmlElement("SplitterPort")]
        public string SplitterPort { get; set; }

        [XmlElement("SplitterLatitude")]
        public string SplitterLatitude { get; set; }

        [XmlElement("SplitterLongitude")]
        public string SplitterLongitude { get; set; }
    }

    [Serializable, XmlRoot("SplitterList")]
    public class SplitterInfo3bbList
    {
        [XmlElement("Splitter")]
        public List<SplitterInfo3bb> Splitter { get; set; }
    }

}