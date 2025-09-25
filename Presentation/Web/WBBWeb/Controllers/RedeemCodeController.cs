using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.ExWebServices;
using WBBContract.Queries.WebServices;
using WBBEntity.Extensions;
using WBBEntity.PanelModels;
using WBBEntity.PanelModels.ExWebServiceModels;
using WBBEntity.PanelModels.WebServiceModels;
using WBBWeb.Extension;
using WBBWeb.Models;

namespace WBBWeb.Controllers
{
    public class RedeemCodeController : WBBController
    {
        #region Properties

        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<SaveRegisterItemRedeemCodeCommand> _saveRegisterItemRedeemCodeCommand;
        private readonly ICommandHandler<SendSmsCommand> _sendSmsCommand;

        #endregion

        #region Constructor

        public RedeemCodeController(IQueryProcessor queryProcessor, ILogger logger,
            ICommandHandler<SaveRegisterItemRedeemCodeCommand> saveRegisterItemRedeemCodeCommand,
            ICommandHandler<SendSmsCommand> sendSmsCommand)
        {
            _queryProcessor = queryProcessor;
            _saveRegisterItemRedeemCodeCommand = saveRegisterItemRedeemCodeCommand;
            _sendSmsCommand = sendSmsCommand;
            base.Logger = logger;
        }

        #endregion

        #region ActionResult

        //
        // GET: /RedeemCode/

        public ActionResult Index(string lang = "")
        {
            Session["FullUrl"] = this.Url.Action("Index", "RedeemCode", null, this.Request.Url.Scheme);

            if (lang == "th" || lang == "")
            {
                ViewBag.LanguagePage = "th";
                SiteSession.CurrentUICulture = 1;
                Session["CurrentUICulture"] = 1;
            }
            else
            {
                ViewBag.LanguagePage = "en";
                SiteSession.CurrentUICulture = 2;
                Session["CurrentUICulture"] = 2;
            }

            var LovScreenFBBOR042 = GetScreenConfig("FBBOR042");
            ViewBag.LabelFBBOR042 = LovScreenFBBOR042;
            ViewBag.FbbConstant = GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);

            return View();
        }

        #endregion

        #region JsonResult

        [HttpPost]
        public JsonResult CheckRedeem(string inMobileNo, string inIDCardNo)
        {
            #region Get Full Url , IP Address Interface Log

            string FullUrl = Session["FullUrl"] != null ? Session["FullUrl"].ToSafeString() : "";
            string Page = string.IsNullOrEmpty(FullUrl) ? "" : FullUrl.Split('/')[FullUrl.Split('/').Count() - 1].ToSafeString();
            // Get IP Address
            string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress)) ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            #endregion

            string outMessage = "ERROR";
            string outContactMobileNo = "";
            var outProductMainData = new PersonalInfoRedeemCodeModel();

            var CustData = evESeServiceQueryMassCommonAccountInfomation
                (
                    "2", inMobileNo, inIDCardNo, "ID_CARD", Page, ipAddress, FullUrl
                );

            if (CustData.outErrorMessage == "")
            {
                var SMSData = evESeServiceQueryMassCommonAccountInfomation
                    (
                        "4", inMobileNo, "", "", Page, ipAddress, FullUrl
                    );

                if (!string.IsNullOrEmpty(SMSData.outServiceMobileNo))
                {
                    var PersonalInfoData = evESQueryPersonalInformation("2", inMobileNo, FullUrl.ToSafeString());

                    if (PersonalInfoData.Count > 0)
                    {
                        var ProductMainData = serchProductClassMain(PersonalInfoData);

                        if (!string.IsNullOrEmpty(ProductMainData.productClass))
                        {
                            outMessage = "SUCCESS";
                            outContactMobileNo = SMSData.outServiceMobileNo.ToSafeString();
                            outProductMainData = ProductMainData;
                        }
                    }
                }
            }

            return Json(new
            {
                data = new
                {
                    outMessage = outMessage,
                    outContactMobileNo = outContactMobileNo,
                    outProductMainData = outProductMainData
                },
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetConfigRedeemCode(string inLanguage, string inMobileNo, string inIDCardNo)
        {
            #region Get Full Url , IP Address Interface Log

            string FullUrl = Session["FullUrl"] != null ? Session["FullUrl"].ToSafeString() : "";
            string Page = string.IsNullOrEmpty(FullUrl) ? "" : FullUrl.Split('/')[FullUrl.Split('/').Count() - 1].ToSafeString();
            // Get IP Address
            string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress)) ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            #endregion

            string strLanguage = inLanguage == "en" ? "E" : "T";

            var query = new GetConfigRedeemCodeQuery
            {
                p_language = strLanguage.ToSafeString(),
                p_mobileno = inMobileNo.ToSafeString(),
                p_idcardno = inIDCardNo.ToSafeString(),

                ClientIP = ipAddress,
                FullUrl = FullUrl
            };

            var result = _queryProcessor.Execute(query);
            var outRedeemCodeConfigData = new List<RedeemCodeConfigDataModel>();

            if (result.list_config_redeem_code != null)
            {
                outRedeemCodeConfigData = result.list_config_redeem_code;
                foreach (var m in outRedeemCodeConfigData)
                {
                    string base64String = Convert.ToBase64String(m.game_item_img, 0, m.game_item_img.Length);
                    m.game_item_img_base64 = "data:image/png;base64," + base64String;
                    m.game_item_img = null;

                    string base64String2 = Convert.ToBase64String(m.game_popup_img, 0, m.game_popup_img.Length);
                    m.game_popup_img_base64 = "data:image/png;base64," + base64String2;
                    m.game_popup_img = null;
                }
                GC.Collect();
            }

            return Json(outRedeemCodeConfigData, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetCodePackRedeem(string inUrlReqBarcode, string inUsername, string inPassword, string inIPaddress, string inMobileNo, string inShortCode,
                                            string inReqBarcode, string inIDCardNo, string inGameName, string inContactMobileNo)
        {
            #region Get Full Url , IP Address Interface Log

            string FullUrl = Session["FullUrl"] != null ? Session["FullUrl"].ToSafeString() : "";
            string Page = string.IsNullOrEmpty(FullUrl) ? "" : FullUrl.Split('/')[FullUrl.Split('/').Count() - 1].ToSafeString();
            // Get IP Address
            string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress)) ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            #endregion

            string outCheckMsg = "ERROR";
            string inRedeemStatus = "ERROR";
            var outPrivilegeBarcodeData = new List<GetPrivilegeBarcodeModel>();

            int ReqBarcode = int.Parse(inReqBarcode);
            string[] arrShortCode = inShortCode.ToSafeString().Split('|');

            for (int i = 0; i < ReqBarcode; i++)
            {
                var privilegeBarcodemodel = new GetPrivilegeBarcodeModel();
                try
                {
                    // Call Service
                    var query = new GetPrivilegeBarcodeQuery
                    {
                        UrlReqPrivilegeBarcode = inUrlReqBarcode.ToSafeString(),
                        Username = inUsername.ToSafeString(),
                        Password = inPassword.ToSafeString(),
                        IpAddress = inIPaddress.ToSafeString(),
                        MobileNo = inMobileNo.ToSafeString(),
                        ShortCode = arrShortCode[i].ToSafeString(),

                        IDCardNo = inIDCardNo.ToSafeString(),
                        ClientIP = ipAddress.ToSafeString(),
                        FullURL = FullUrl.ToSafeString()
                    };

                    privilegeBarcodemodel = _queryProcessor.Execute(query);

                    if (privilegeBarcodemodel.Description == "SUCCESS" && privilegeBarcodemodel.HttpStatus == "200" && privilegeBarcodemodel.Status == "20000")
                    {
                        outCheckMsg = "SUCCESS";
                        inRedeemStatus = privilegeBarcodemodel.Description;
                        outPrivilegeBarcodeData.Add(privilegeBarcodemodel);
                    }
                    else
                    {
                        outCheckMsg = privilegeBarcodemodel.Description == "DUPLICATE" ? "ERR_ALREADY" : "ERROR";
                        inRedeemStatus = privilegeBarcodemodel.Description;
                        break;
                    }
                }
                catch
                {
                    outCheckMsg = "ERROR";
                    inRedeemStatus = "ERROR";
                }
            }

            // Insert Register Item RedeemCode
            RegisterItemRedeemCode(inMobileNo, inIDCardNo, inContactMobileNo, inGameName, outPrivilegeBarcodeData, inRedeemStatus, ipAddress, FullUrl);

            return Json(new
            {
                data = new
                {
                    outCheckMsg = outCheckMsg,
                    outDescription = inRedeemStatus,
                    outPrivilegeBarcodeData = outPrivilegeBarcodeData
                },
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SendSMSRedeemCodeItem(string inMobileNo, string inContactMobileNo, string inMessageText)
        {
            #region Get Full Url , IP Address Interface Log

            string FullUrl = Session["FullUrl"] != null ? Session["FullUrl"].ToSafeString() : "";
            string Page = string.IsNullOrEmpty(FullUrl) ? "" : FullUrl.Split('/')[FullUrl.Split('/').Count() - 1].ToSafeString();
            // Get IP Address
            string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress)) ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            #endregion

            var command = new SendSmsCommand()
            {
                Source_Addr = "AISFIBRE",
                Destination_Addr = inContactMobileNo.ToSafeString(),
                Message_Text = inMessageText.ToSafeString(),
                Transaction_Id = inMobileNo.ToSafeString() + ipAddress.ToSafeString(),
                FullUrl = FullUrl.ToSafeString()
            };

            _sendSmsCommand.Handle(command);

            return Json(new { data = new { return_status = command.return_status } }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region  method private

        private PersonalInfoRedeemCodeModel serchProductClassMain(List<evESQueryPersonalInformationModel> PersonalInfoData)
        {
            var model = new PersonalInfoRedeemCodeModel();

            var LovEsportsPackage = base.LovData.Where(p => p.LovValue5 == "FBBOR042" && p.Type == "SCREEN" && p.Name == "ESPORTS_PACKAGE").Select(p => p.LovValue1).ToList();

            var listProductClassMain = PersonalInfoData.Where(m => m.productClass == "Main" && LovEsportsPackage.Contains(m.productCd) == true).First();

            if (!string.IsNullOrEmpty(listProductClassMain.productClass))
            {
                model.productCd = listProductClassMain.productCd;
                model.productClass = listProductClassMain.productClass;
            }

            return model;
        }

        private void RegisterItemRedeemCode(string inMobileNo, string inIDCardNo, string inContactMobileNo, string inGameName, List<GetPrivilegeBarcodeModel> barcodeList,
                                            string inRedeemStatus, string clientIP, string fullUrl)
        {
            string[] inItemCode = new string[5];
            if (barcodeList.Any())
                for (int i = 0; i < barcodeList.Count; i++)
                {
                    inItemCode[i] = barcodeList[i].MsgBarcode.ToSafeString();
                }

            var command = new SaveRegisterItemRedeemCodeCommand
            {
                p_non_mobile_no = inMobileNo.ToSafeString(),
                p_id_card_no = inIDCardNo.ToSafeString(),
                p_contact_mobile_no = inContactMobileNo.ToSafeString(),
                p_game_name = inGameName.ToSafeString(),
                p_redeem_code_1 = inItemCode[0].ToSafeString(),
                p_redeem_code_2 = inItemCode[1].ToSafeString(),
                p_redeem_code_3 = inItemCode[2].ToSafeString(),
                p_redeem_code_4 = inItemCode[3].ToSafeString(),
                p_redeem_code_5 = inItemCode[4].ToSafeString(),
                p_redeem_status = inRedeemStatus.ToSafeString(),

                ClientIP = clientIP.ToSafeString(),
                FullUrl = fullUrl.ToSafeString()
            };

            _saveRegisterItemRedeemCodeCommand.Handle(command);
        }

        #endregion

        #region method public

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

        public List<FbbConstantModel> GetFbbConstantModel(string fbbConstType)
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

        public evESeServiceQueryMassCommonAccountInfoModel evESeServiceQueryMassCommonAccountInfomation(string option, string mobileNo, string idCardNo, string CardType,
                                                                                                        string Page, string clientIP, string fullUrl)
        {
            evESeServiceQueryMassCommonAccountInfoModel result = new evESeServiceQueryMassCommonAccountInfoModel();
            try
            {
                var query = new evESeServiceQueryMassCommonAccountInfoQuery
                {
                    inOption = option.ToSafeString(),
                    inMobileNo = mobileNo.ToSafeString(),
                    inCardNo = idCardNo.ToSafeString(),
                    inCardType = CardType.ToSafeString(),
                    Page = Page.ToSafeString(),
                    ClientIP = clientIP.ToSafeString(),
                    FullUrl = fullUrl.ToSafeString()
                };
                result = _queryProcessor.Execute(query);
            }
            catch { }

            return result;
        }

        public List<evESQueryPersonalInformationModel> evESQueryPersonalInformation(string option, string mobileNo, string fullUrl)
        {
            List<evESQueryPersonalInformationModel> result = new List<evESQueryPersonalInformationModel>();
            try
            {
                var query = new evESQueryPersonalInformationQuery()
                {
                    mobileNo = mobileNo.ToSafeString(),
                    option = option.ToSafeString(),
                    FullUrl = fullUrl.ToSafeString()
                };
                result = _queryProcessor.Execute(query);
            }
            catch { }

            return result;
        }

        #endregion
    }
}
