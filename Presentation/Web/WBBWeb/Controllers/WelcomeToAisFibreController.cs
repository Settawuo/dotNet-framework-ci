using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
//using System.Xml.Linq;
//using System.Xml.Linq;
using WBBBusinessLayer;
using WBBBusinessLayer.Extension.Security;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.ExWebServices;
using WBBContract.Queries.WebServices;
using WBBContract.QueryModels.WebServices;
using WBBEntity.Extensions;
using WBBEntity.PanelModels;
using WBBEntity.PanelModels.ExWebServiceModels;
using WBBEntity.PanelModels.WebServiceModels;
using WBBWeb.Extension;
using WBBWeb.Models;

namespace WBBWeb.Controllers
{
    public class WelcomeToAisFibreController : WBBController
    {
        #region Properties

        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<InsertLogDataPrivacyCommand> _insertLogDataPrivacyCommand;

        #endregion

        #region Constuctor
        //
        // GET: /WelcomeToAisFibre/
        public WelcomeToAisFibreController(IQueryProcessor queryProcessor,
            ICommandHandler<InsertLogDataPrivacyCommand> InsertMasterTDMContractDeviceCommand,
            ILogger logger)
        {
            _queryProcessor = queryProcessor;
            _insertLogDataPrivacyCommand = InsertMasterTDMContractDeviceCommand;
            base.Logger = logger;
        }

        #endregion

        #region ActionResult

        public ActionResult Index()
        {
            SiteSession.CurrentUICulture = 1; //TH
            Session[WebConstants.SessionKeys.CurrentUICulture] = 1;
            Session["WELCOME_MOBILEDATA"] = null;
            Session["FullUrl"] = this.Url.Action("Index", "WelcomeToAisFibre", null, this.Request.Url.Scheme);
            ViewBag.LabelFBBOR051 = GetScreenConfig(WebConstants.LovConfigName.GetScreenWelcomeToAisFibre);
            ViewBag.FbbConstant = GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);
            ViewBag.Config = GetConfigModel();

            return View();
        }

        #endregion

        #region JsonResult Login

        public JsonResult getCheckMobileNumberSentOTP(string mobileNo = "", string option = "")
        {
            string outServiceMobileNo = "";
            string FullUrl = Session["FullUrl"] != null ? Session["FullUrl"].ToSafeString() : "";

            #region Get IP Address Interface Log
            // Get IP Address
            string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress)) ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            #endregion

            try
            {
                string outMobileNumber = "";
                var query = new evESeServiceQueryMassCommonAccountInfoQuery
                {
                    inOption = option,
                    inMobileNo = mobileNo,
                    inCardNo = "",
                    inCardType = "",
                    Page = "WelcomeToAisFibre",
                    Username = "Customer",
                    ClientIP = ipAddress,
                    FullUrl = FullUrl
                };
                var a = _queryProcessor.Execute(query);

                if (a.outServiceMobileNo != null)
                {
                    outServiceMobileNo = a.outServiceMobileNo.ToSafeString();
                }

                if (a.outMobileNumber != null)
                {
                    outMobileNumber = a.outMobileNumber.ToSafeString();
                }

                Session["WELCOME_MOBILEDATA"] = outServiceMobileNo;

                if (!string.IsNullOrEmpty(outServiceMobileNo))
                {
                    var resultSendOneTime = SendOneTimePWPrivate(outServiceMobileNo, "All");
                    if (resultSendOneTime != null && resultSendOneTime.code == "000")
                    {
                        var EnServiceMobileNo = WBBEncrypt.textEncrpyt(outServiceMobileNo);
                        var ShowServiceMobileNo = string.IsNullOrEmpty(outServiceMobileNo) ? "-" : outServiceMobileNo.Substring(0, 3) + "XXX" + outServiceMobileNo.Substring(outServiceMobileNo.Length - 4);
                        //var ShowMobileNumber = string.IsNullOrEmpty(outMobileNumber) ? "-" : outMobileNumber.Substring(0, 3) + "XXX" + outMobileNumber.Substring(outMobileNumber.Length - 4);
                        //R23.05 fixed welcome not show mask of mobile number
                        var ShowMobileNumber = string.IsNullOrEmpty(outMobileNumber) ? "-" : outMobileNumber;
                        var transactionID = resultSendOneTime.transactionID.ToSafeString();

                        return Json(new
                        {
                            data = new
                            {
                                EnServiceMobileNo = EnServiceMobileNo,
                                ShowServiceMobileNo = ShowServiceMobileNo,
                                ShowMobileNumber = ShowMobileNumber,
                                transactionID = transactionID,
                                errorMessage = ""
                            }
                        }, JsonRequestBehavior.AllowGet);
                    }
                    else if (resultSendOneTime == null)
                    {
                        string messageError = getErrorCodeOTP(null, outServiceMobileNo);

                        return Json(new
                        {
                            data = new
                            {
                                EnServiceMobileNo = "",
                                ShowServiceMobileNo = "",
                                ShowMobileNumber = "",
                                transactionID = "",
                                errorMessage = messageError
                            }
                        }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var errorMessageOTP = getErrorCodeOTP(resultSendOneTime.code, outServiceMobileNo);
                        string errorMessage = errorMessageOTP.ToSafeString();
                        return Json(new
                        {
                            data = new
                            {
                                EnServiceMobileNo = "",
                                ShowServiceMobileNo = "",
                                ShowMobileNumber = "",
                                transactionID = "",
                                errorMessage = errorMessage
                            }
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new
                    {
                        data = new
                        {
                            EnServiceMobileNo = "",
                            ShowServiceMobileNo = "",
                            ShowMobileNumber = "",
                            transactionID = "",
                            errorMessage = "No Profile"
                        }
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                string messageError = getErrorCodeOTP(null, outServiceMobileNo);

                return Json(new
                {
                    data = new
                    {
                        EnServiceMobileNo = "",
                        ShowServiceMobileNo = "",
                        ShowMobileNumber = "",
                        transactionID = "",
                        errorMessage = messageError
                    }
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult evESeServiceQueryMassCommonAccountInfoforWelcome(string mobileNo = "", string option = "")
        {
            string FullUrl = Session["FullUrl"] != null ? Session["FullUrl"].ToSafeString() : "";

            #region Get IP Address Interface Log
            // Get IP Address
            string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress)) ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            #endregion

            try
            {
                var query = new evESeServiceQueryMassCommonAccountInfoQuery
                {
                    inOption = option,
                    inMobileNo = mobileNo,
                    inCardNo = "",
                    inCardType = "",
                    Page = "WelcomeToAisFibre",
                    Username = "Customer",
                    ClientIP = ipAddress,
                    FullUrl = FullUrl
                };

                var sffData = _queryProcessor.Execute(query);

                if (sffData != null && sffData.errorMessage == "")
                {
                    return Json(
                        new
                        {
                            data = new
                            {
                                errorMessage = sffData.errorMessage.ToSafeString(),

                                outPrimaryContactFirstName = sffData.outPrimaryContactFirstName.ToSafeString(),
                                outContactLastName = sffData.outContactLastName.ToSafeString(),
                                outAmphur = sffData.outAmphur.ToSafeString(),
                                outBuildingName = sffData.outBuildingName.ToSafeString(),
                                outFloor = sffData.outFloor.ToSafeString(),
                                outHouseNumber = sffData.outHouseNumber.ToSafeString(),
                                outMoo = sffData.outMoo.ToSafeString(),
                                outMooban = sffData.outMooban.ToSafeString(),
                                outProvince = sffData.outProvince.ToSafeString(),
                                outRoom = sffData.outRoom.ToSafeString(),
                                outSoi = sffData.outSoi.ToSafeString(),
                                outStreetName = sffData.outStreetName.ToSafeString(),
                                outBillLanguage = sffData.outBillLanguage.ToSafeString(),
                                outTumbol = sffData.outTumbol.ToSafeString(),
                                outBirthDate = sffData.outBirthDate.ToSafeString(),
                                outEmail = sffData.outEmail.ToSafeString(),
                                outAccountName = sffData.outAccountName.ToSafeString(),
                                outAccountNumber = sffData.outAccountNumber.ToSafeString(),
                                outServiceAccountNumber = sffData.outServiceAccountNumber.ToSafeString(),
                                outBillingAccountNumber = sffData.outBillingAccountNumber.ToSafeString(),
                                outProductName = sffData.outProductName.ToSafeString(),
                                outDayOfServiceYear = sffData.outDayOfServiceYear.ToSafeString(),
                                cardType = sffData.cardType.ToSafeString(),
                                outAccountSubCategory = sffData.outAccountSubCategory.ToSafeString(),
                                outPostalCode = sffData.outPostalCode.ToSafeString(),
                                outTitle = sffData.outTitle.ToSafeString(),
                                OwnerProduct = sffData.OwnerProduct.ToSafeString(),
                                PackageCode = sffData.PackageCode.ToSafeString(),
                                outFullAddress = sffData.outFullAddress.ToSafeString(),
                                outAccountCategory = sffData.outAccountCategory.ToSafeString(),
                                outparameter1 = sffData.outparameter1.ToSafeString(),
                                outparameter2 = sffData.outparameter2.ToSafeString(),
                                vatAddress1 = sffData.vatAddress1.ToSafeString(),
                                vatAddress2 = sffData.vatAddress2.ToSafeString(),
                                vatAddress3 = sffData.vatAddress3.ToSafeString(),
                                vatAddress4 = sffData.vatAddress4.ToSafeString(),
                                vatAddress5 = sffData.vatAddress5.ToSafeString(),
                                vatPostalCd = sffData.vatPostalCd.ToSafeString(),
                                vatAddressFull = sffData.vatAddressFull.ToSafeString()
                            }
                        }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { data = new { errorMessage = "No Profile" } }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { data = new { errorMessage = "ERROR" } }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult insertLogDataPrivacy(string channel = "", string fibrenetId = "", string mobileNo = "",
            string confirmMkt = "", string confirmPrivilege = "")
        {
            string FullUrl = Session["FullUrl"] != null ? Session["FullUrl"].ToSafeString() : "";
            #region Get IP Address Interface Log (Update 17.2)

            // Get IP Address
            string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress)) ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            string transactionId = fibrenetId + ipAddress;

            #endregion

            //Insert LogData Privacy
            var TDMcommamd = new InsertLogDataPrivacyCommand
            {
                Transaction_Id = transactionId.ToSafeString(),
                FullUrl = FullUrl.ToSafeString(),
                P_CHANNEL = channel.ToSafeString(),
                P_FIBRENET_ID = fibrenetId.ToSafeString(),
                P_MOBILE_NO = mobileNo.ToSafeString(),
                P_CONFIRM_MKT = confirmMkt.ToSafeString(),
                P_CONFIRM_PRIVILEGE = confirmPrivilege.ToSafeString()
            };
            _insertLogDataPrivacyCommand.Handle(TDMcommamd);

            bool saveSuccess = false;
            if (TDMcommamd.RETURN_CODE != "-1")
            {
                saveSuccess = true;
            }

            return Json(saveSuccess, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SendOneTimePW(string msisdn)
        {
            //send OTP
            var mobileNo = WBBDecrypt.textDecrpyt(msisdn);
            var mobileData = Session["WELCOME_MOBILEDATA"].ToSafeString();
            if (mobileData == mobileNo)
            {
                try
                {
                    string transactionID = "";
                    GssoSsoResponseModel resultSendOneTime = SendOneTimePWPrivate(mobileNo, "All");
                    if (resultSendOneTime != null && resultSendOneTime.code == "000")
                    {
                        transactionID = resultSendOneTime.transactionID.ToSafeString();
                        return Json(new { data = new { transactionID = transactionID, errorMessage = "" } }, JsonRequestBehavior.AllowGet);
                    }
                    else if (resultSendOneTime == null)
                    {
                        var errorMessageOTP = getErrorCodeOTP(null, mobileNo);
                        string errorMessage = errorMessageOTP.ToSafeString();
                        return Json(new { data = new { transactionID = "", errorMessage = errorMessage } }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var errorMessageOTP = getErrorCodeOTP(resultSendOneTime.code, mobileNo);
                        string errorMessage = errorMessageOTP.ToSafeString();
                        return Json(new { data = new { transactionID = "", errorMessage = errorMessage } }, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception ex)
                {
                    return Json(new { data = new { transactionID = "", errorMessage = "Error" } }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { data = new { transactionID = "", errorMessage = "Error" } }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ConfirmOneTimePW(string nonMobile, string msisdn, string pwd, string transactionID)
        {
            //send OTP
            var mobileNo = WBBDecrypt.textDecrpyt(msisdn);
            var mobileData = Session["WELCOME_MOBILEDATA"].ToSafeString();
            GssoSsoResponseModel result = new GssoSsoResponseModel();
            if (mobileData == mobileNo)
            {
                try
                {
                    var query = new ConfirmOneTimePWQuery()
                    {
                        msisdn = mobileNo,
                        pwd = pwd,
                        transactionID = transactionID
                    };
                    result = new GssoSsoResponseModel();
                    result = _queryProcessor.Execute(query);

                    if (result != null && result.code == "000" && result.transactionID == transactionID)
                    {
                        transactionID = result.transactionID.ToSafeString();

                        return Json(new { data = new { MobileNo = mobileNo, status = true, errorMessage = "" } }, JsonRequestBehavior.AllowGet);
                    }
                    else if (result == null)
                    {
                        var errorMessageOTP = getErrorCodeOTP(null, mobileNo);
                        string errorMessage = errorMessageOTP.ToSafeString();
                        return Json(new { data = new { MobileNo = "", status = false, errorMessage = errorMessage } }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var errorMessageOTP = getErrorCodeOTP(result.code, mobileNo);
                        string errorMessage = errorMessageOTP.ToSafeString();
                        return Json(new { data = new { MobileNo = "", status = false, errorMessage = errorMessage } }, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception ex)
                {
                    return Json(new { data = new { MobileNo = "", status = false, errorMessage = "ERROR" } }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { data = new { MobileNo = "", status = false, errorMessage = "ERROR" } }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region JsonResult Profile

        public JsonResult evESQueryPersonalInformationfoForWelcome(string fibrenetId, string option)
        {

            string FullUrl = "";
            if (Session["FullUrl"] != null)
                FullUrl = Session["FullUrl"].ToSafeString();

            List<evESQueryPersonalInformationModel> result = new List<evESQueryPersonalInformationModel>();
            try
            {
                var query = new evESQueryPersonalInformationQuery()
                {
                    mobileNo = fibrenetId,
                    option = option,
                    FullUrl = FullUrl
                };
                result = _queryProcessor.Execute(query);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json("ERROR", JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult GetDataAddressInstallProfile(string fibrenetId = "")
        {

            string FullUrl = Session["FullUrl"] != null ? Session["FullUrl"].ToSafeString() : "";
            #region Get IP Address Interface Log (Update 17.2)

            // Get IP Address
            string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress)) ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            string transactionId = fibrenetId + ipAddress;

            #endregion


            try
            {
                var query = new GetDataAddressInstallQuery
                {
                    p_fibrenet_id = fibrenetId,
                    transaction_Id = transactionId.ToSafeString(),
                    fullUrl = FullUrl.ToSafeString()
                };
                var result = _queryProcessor.Execute(query);

                return Json(new { data = new { install_curror = result.install_curror } }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(new { data = new { install_curror = "ERROR" } }, JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult GetDataAddressBillProfile(string fibrenetId = "", string houseNo = "", string Moo = "", string Mooban = "", string Room = "", string Floor = "", string buildingName = "", string Soi = "", string streetName = "", string Tumbol = "", string Amphur = "", string provinceName = "", string zipCode = "")
        {

            string FullUrl = Session["FullUrl"] != null ? Session["FullUrl"].ToSafeString() : "";
            #region Get IP Address Interface Log (Update 17.2)

            // Get IP Address
            string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress)) ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            string transactionId = fibrenetId + ipAddress;

            #endregion


            try
            {
                var query = new GetDataAddressBillQuery
                {
                    transaction_Id = transactionId.ToSafeString(),
                    fullUrl = FullUrl.ToSafeString(),
                    p_houseNo = houseNo,
                    p_Moo = Moo,
                    p_Mooban = Mooban,
                    p_Room = Room,
                    p_Floor = Floor,
                    p_buildingName = buildingName,
                    p_Soi = Soi,
                    p_streetName = streetName,
                    p_Tumbol = Tumbol,
                    p_Amphur = Amphur,
                    p_provinceName = provinceName,
                    p_zipCode = zipCode

                };
                var result = _queryProcessor.Execute(query);

                return Json(new { data = new { address_curror = result.address_curror } }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { data = new { address_curror = "ERROR" } }, JsonRequestBehavior.AllowGet);
            }



        }

        public JsonResult evCorpESQueryAccountInformationListInForWelcome(string inAccountNo,string inAccountName)
        {
            evCorpESQueryAccountInformationListInfoForWelcomeQuery query = new evCorpESQueryAccountInformationListInfoForWelcomeQuery()
            {

                inAccountNo = inAccountNo.ToSafeString(),
            };

            evCorpESQueryAccountInformationListInfoForWelcomeModel result = _queryProcessor.Execute(query);
            if (result == null)
            {
                result = new evCorpESQueryAccountInformationListInfoForWelcomeModel()
                {
                    errorCode = "0001",
                    errorMessage = "No Data Found"
                };
            }
            else
            {
                if(!result.billName.Contains(inAccountName))
                {
                    result = new evCorpESQueryAccountInformationListInfoForWelcomeModel()
                    {
                        errorCode = "0001",
                        errorMessage = "No Data Found"
                    };
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        #endregion


        #region JsonResult Package


        public JsonResult GetPackageDetailByNonMobile(string mobileNo = "", string option = "")
        {
            string FullUrl = Session["FullUrl"] != null ? Session["FullUrl"].ToSafeString() : "";

            #region Get IP Address Interface Log
            // Get IP Address
            string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress)) ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            #endregion

            GetPackageDetailByNonMobileQueryModel result = new GetPackageDetailByNonMobileQueryModel();
            result.RETURN_CODE = "-1";

            try
            {
                var promotionByPackageQuery = new evOMQueryListServiceAndPromotionByPackageTypeQuery
                {
                    mobileNo = mobileNo.ToSafeString(),
                    idCard = mobileNo.ToSafeString(),
                    FullUrl = FullUrl.ToSafeString()
                };

                var promotionByPackageData = _queryProcessor.Execute(promotionByPackageQuery);

                if (promotionByPackageData != null && promotionByPackageData.ListPromotion != null
                    && promotionByPackageData.ListPromotion.Count > 0)
                {

                    List<PackageFbbor051> sentData = promotionByPackageData.ListPromotion.Select(c => new PackageFbbor051
                    {
                        fibrenetId = mobileNo.ToSafeString(),
                        productCD = c.productCD,
                        productClass = c.productClass,
                        productType = c.productType,
                        startDate = c.startDate,
                        endDate = c.endDate
                    }).ToList();

                    GetPackageDetailByNonMobileQuery getPackageDetailByNonMobileQuery = new GetPackageDetailByNonMobileQuery()
                    {
                        P_FIBRENET_ID = mobileNo.ToSafeString(),
                        P_FBBOR051_PACKAGE_ARRAY = sentData
                    };

                    result = _queryProcessor.Execute(getPackageDetailByNonMobileQuery);

                    #region SFFService Option 2
                    var query02 = new evESQueryPersonalInformationQuery()
                    {
                        mobileNo = mobileNo,
                        option = "2",
                        FullUrl = FullUrl,

                    };
                    var sffresult = _queryProcessor.Execute(query02).Where(s => s.productClass != null).ToList();

                    //----------------กล่อง แพ็กเกจหลัก-------------------
                    Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US"); //ADD : 25/05/2023
                    var dtnow = DateTime.Now;
                    try
                    {
                        var productClass = new[] { "Main" };
                        var packmainsff = (from pm in sffresult
                                           where productClass.Contains(pm.productClass)
                                           let startdt = pm.startDt != null ? DateTime.ParseExact(pm.startDt.Split(' ').FirstOrDefault(), "dd/MM/yyyy", null) : DateTime.Now
                                           let enddt = pm.endDt != null ? DateTime.ParseExact(pm.endDt.Split(' ').FirstOrDefault(), "dd/MM/yyyy", null) : DateTime.Now
                                           let diff = enddt - startdt
                                           where dtnow >= startdt && dtnow <= enddt
                                           where pm.shortNameThai != null
                                           where pm.inStatementThai != null
                                           orderby enddt descending
                                           select new WelcomePackageMain
                                           {
                                               PROMOTION_DISPLAY = pm.shortNameThai,
                                               PROMOTION_NAME = pm.inStatementThai,
                                               FIBRENET_ID = mobileNo,
                                               PROMOTION_EXPIRE = pm.endDt.ToDisplayThaiDate(),
                                           }).Take(1).ToList();

                        result.PACKAGE_MAIN = (from sp in result.PACKAGE_MAIN
                                               join sff in packmainsff on sp.FIBRENET_ID equals sff.FIBRENET_ID
                                               select new WelcomePackageMain
                                               {
                                                   PROMOTION_DISPLAY = sff.PROMOTION_DISPLAY,
                                                   PROMOTION_NAME = sff.PROMOTION_NAME,
                                                   FIBRENET_ID = mobileNo,
                                                   PROMOTION_EXPIRE = sff.PROMOTION_EXPIRE,
                                                   DOWNLOAD_SPEED = sp.DOWNLOAD_SPEED,
                                                   UPLOAD_SPEED = sp.UPLOAD_SPEED
                                               }).Take(1).ToList();
                    }
                    catch
                    {
                    }

                    //----------------กล่อง ส่วนลด-------------------
                    var discount = new[] { "FBB_PAY_ADVANCE", "FBB_DEVICE_DISCOUNT", "FBB_DEVICE_DISCOUNT_BAHT", "FBB_DISCOUNT", "FBB_DISCOUNT_BAHT", "FBB_DISCOUNT_SAVE_CHURN", "FBB_DISCOUNT_SAVE_CHURN_BAHT", "FBB_DISCOUNT_SAVE_CHURN_PERCENT", "FMB_DISCOUNT", "FMB_SERENADE", "FBB_DISCOUNT_ENTRYFEE" };
                    var discountblob = LovData.Where(w => w.Name == "DISCOUNT" && w.ActiveFlag == "Y").FirstOrDefault();
                    var prodcless = new[] { "On-Top", "On-Top Extra" };

                    try
                    {

                        result.PACKAGE_DISCOUNT = (from dc in sffresult
                                                   where discount.Contains(dc.productPkg)
                                                   where prodcless.Contains(dc.productClass) //(on top, on top extra)
                                                   let startdt = dc.startDt != null ? DateTime.ParseExact(dc.startDt.Split(' ').FirstOrDefault(), "dd/MM/yyyy", null) : DateTime.Now
                                                   let enddt = dc.endDt != null ? DateTime.ParseExact(dc.endDt.Split(' ').FirstOrDefault(), "dd/MM/yyyy", null) : DateTime.Now
                                                   //let diff = enddt - startdt
                                                   where dtnow >= startdt && dtnow <= enddt
                                                   where dc.shortNameThai != null
                                                   where dc.inStatementThai != null
                                                   orderby enddt descending
                                                   select new WelcomePackageDiscount
                                                   {
                                                       FIBRENET_ID = mobileNo,
                                                       PROMOTION_PIC = discountblob != null ? Convert.ToBase64String(discountblob.Image_blob, 0, discountblob.Image_blob.Length) : "",
                                                       PROMOTION_DISPLAY = dc.shortNameThai,
                                                       PROMOTION_DETALIL = dc.inStatementThai
                                                   }).ToList();

                    }
                    catch
                    {
                    }

                    try
                    {
                        //----------------กล่องแพ็คเกจเสริม-------------------
                        var notInData = new[] { "FBB_PAY_ADVANCE", "FBB_DEVICE_DISCOUNT", "FBB_DEVICE_DISCOUNT_BAHT", "FBB_DISCOUNT", "FBB_DISCOUNT_BAHT", "FBB_DISCOUNT_SAVE_CHURN", "FBB_DISCOUNT_SAVE_CHURN_BAHT", "FBB_DISCOUNT_SAVE_CHURN_PERCENT", "FMB_DISCOUNT", "FMB_SERENADE", "FBB_DISCOUNT_ENTRYFEE", "Penalty Package", "FBB Penalty Charge" };
                        var contentpiblob = LovData.Where(w => w.Name == "CONTENTPI" && w.ActiveFlag == "Y").FirstOrDefault();
                        result.PACKAGE_CONTENT = (from ct in sffresult
                                                  where prodcless.Contains(ct.productClass)
                                                  where !notInData.Contains(ct.productPkg)
                                                  let startdt = ct.startDt != null ? DateTime.ParseExact(ct.startDt.Split(' ').FirstOrDefault(), "dd/MM/yyyy", null) : DateTime.Now
                                                  let enddt = ct.endDt != null ? DateTime.ParseExact(ct.endDt.Split(' ').FirstOrDefault(), "dd/MM/yyyy", null) : DateTime.Now
                                                  //let diff = enddt - startdt
                                                  where dtnow >= startdt && dtnow <= enddt
                                                  where ct.shortNameThai != null
                                                  where ct.inStatementThai != null
                                                  orderby enddt descending
                                                  select new WelcomePackageContent
                                                  {
                                                      FIBRENET_ID = mobileNo,
                                                      PROMOTION_PIC = contentpiblob != null ? Convert.ToBase64String(contentpiblob.Image_blob, 0, contentpiblob.Image_blob.Length) : "",
                                                      PROMOTION_DISPLAY = ct.shortNameThai,
                                                      PROMOTION_DETALIL = ct.inStatementThai
                                                  }).ToList();
                    }
                    catch
                    {
                    }

                    try
                    {
                        //----------------กล่องเงื่อนไข-------------------

                        var discountins = new[] { "Penalty Package", "FBB Penalty Charge" };
                        result.PACKAGE_INSTALL = (from ins in sffresult
                                                  where ins.productClass == "On-Top Extra"
                                                  where discountins.Contains(ins.productPkg)
                                                  let startdt = ins.startDt != null ? DateTime.ParseExact(ins.startDt.Split(' ').FirstOrDefault(), "dd/MM/yyyy", null) : DateTime.Now
                                                  let enddt = ins.endDt != null ? DateTime.ParseExact(ins.endDt.Split(' ').FirstOrDefault(), "dd/MM/yyyy", null) : DateTime.Now
                                                  //let diff = enddt - startdt
                                                  where dtnow >= startdt && dtnow <= enddt
                                                  where ins.shortNameThai != null
                                                  where ins.inStatementThai != null
                                                  orderby enddt descending
                                                  select new WelcomePackageInstall
                                                  {
                                                      FIBRENET_ID = mobileNo,
                                                      PROMOTION_DETALIL = ins.inStatementThai,
                                                      PROMOTION_DISPLAY = ins.shortNameThai
                                                  }).ToList();


                    }
                    catch
                    {
                    }
                    #endregion

                    #region SFFService Option 3     

                    var query03 = new evESQueryPersonalInformationQuery()
                    {
                        mobileNo = mobileNo,
                        option = "3",
                        FullUrl = FullUrl,
                        sourceSystem = "MYAIS"
                    };
                    //var sffresult03 = _queryProcessor.Execute(query03).Where(s=>s.productClass != null).ToList();
                    var sffresult03 = _queryProcessor.Execute(query03);

                    //-----------------กล่องบริการเสริม ---------------
                    try
                    {
                        var meshpic = LovData.Where(w => w.Name == "MESHPIC" && w.ActiveFlag == "Y").FirstOrDefault();
                        var playbox = LovData.Where(w => w.Name == "PLAYBOX" && w.ActiveFlag == "Y").FirstOrDefault();
                        string blobmeshpic = meshpic != null ? Convert.ToBase64String(meshpic.Image_blob, 0, meshpic.Image_blob.Length) : "";
                        string blobplaybox = playbox != null ? Convert.ToBase64String(playbox.Image_blob, 0, playbox.Image_blob.Length) : "";

                        result.PACKAGE_VAS = (from vas in sffresult03
                                                  //where prodcless.Contains(vas.productClass)  //in ("on top, on top extra")
                                              let startdt = vas.startDt != null ? DateTime.ParseExact(vas.startDt.Split(' ').FirstOrDefault(), "dd/MM/yyyy", null) : DateTime.Now
                                              let enddt = vas.endDt != null ? DateTime.ParseExact(vas.endDt.Split(' ').FirstOrDefault(), "dd/MM/yyyy", null) : DateTime.Now
                                              //let diff = enddt - startdt
                                              where dtnow >= startdt && dtnow <= enddt
                                              where vas.shortNameThai != null
                                              where vas.inStatementThai != null
                                              orderby enddt descending
                                              select new WelcomePackageVas
                                              {
                                                  FIBRENET_ID = mobileNo,
                                                  PROMOTION_PIC = vas.productCd == "00006" ? blobmeshpic : blobplaybox,
                                                  PROMOTION_DETALIL = vas.inStatementThai,
                                                  PROMOTION_DISPLAY = vas.shortNameThai
                                              }).ToList();
                    }
                    catch
                    {
                    }

                    #endregion

                    if (result != null && result.RETURN_CODE != "-1")
                    {
                        //PackageYouData = getPackageYouData
                        return Json(new { PackageYouData = result }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json("ERROR", JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json("ERROR", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json("ERROR", JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region method private

        private GssoSsoResponseModel SendOneTimePWPrivate(string msisdn, string accountType)
        {
            //send OTP
            try
            {
                var query = new SendOneTimePWQuery()
                {
                    msisdn = msisdn,
                    accountType = accountType
                };
                var result = _queryProcessor.Execute(query);

                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private string getErrorCodeOTP(string Error_Code = "", string Mobile_No = "")
        {
            try
            {
                string Massage = null;

                IEnumerable<LovValueModel> Error_Massage;
                string[] errorCodes = LovData.Where(d => d.Text != null && d.Name == "L_ERROR_OTP").Select(d => d.Text).ToArray();

                if (!errorCodes.Contains(Error_Code))
                    Error_Massage = base.LovData.Where(l => l.Name == "L_ERROR_OTP" && l.Text == null);
                else
                    Error_Massage = base.LovData.Where(l => l.Name == "L_ERROR_OTP" && l.Text == Error_Code);

                if (Error_Massage.Any())
                {
                    var tmp = Error_Massage.FirstOrDefault();
                    Massage = tmp.LovValue1; //TH
                    if (Massage.IndexOf("{MobileNo}") > 0)
                    {
                        Massage = Massage.Replace("{MobileNo}", Mobile_No.Substring(0, 3) + "XXX" + Mobile_No.Substring(Mobile_No.Length - 4));
                    }
                }

                return Massage;
            }
            catch (Exception ex)
            {
                return null;
            }
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

        private List<ConfigModel> GetConfigModel()
        {
            var lovs = LovData
                .Where(d => d.Type == WebConstants.LovConfigName.GetConfig)
                .Select(d => new ConfigModel()
                {
                    Key = d.Name,
                    Value1 = d.LovValue1,
                    Value2 = d.LovValue2,
                    Value3 = d.LovValue3,
                    Value4 = d.LovValue4,
                    Value5 = d.LovValue5
                }).ToList();

            return lovs;
        }

        #endregion

        #region package page New version(Welcome)!!!

        public JsonResult CheckPackageDetailByNonMobile(string mobileNo = "", string option = "")
        {

            string FullUrl = Session["FullUrl"] != null ? Session["FullUrl"].ToSafeString() : "";

            #region Get IP Address Interface Log
            // Get IP Address
            string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress)) ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            #endregion

            var sffresult = new List<evESQueryPersonalInformationModel>();
            try
            {
                if (option == "2")
                {
                    #region SFFService Option 2
                    var query02 = new evESQueryPersonalInformationQuery()
                    {
                        mobileNo = mobileNo,
                        option = "2",
                        FullUrl = FullUrl,

                    };
                    sffresult = _queryProcessor.Execute(query02).Where(s => s.productClass != null).ToList();
                    //return Json(new { PackageYouData = sffresult_ }, JsonRequestBehavior.AllowGet);
                    #endregion

                }
                else if (option == "3")
                {
                    #region SFFService Option 3     

                    var query03 = new evESQueryPersonalInformationQuery()
                    {
                        mobileNo = mobileNo,
                        option = "3",
                        FullUrl = FullUrl,
                        sourceSystem = "MYAIS"
                    };

                    sffresult = _queryProcessor.Execute(query03);
                    // return Json(new { PackageYouData = sffresult03 }, JsonRequestBehavior.AllowGet);

                    //-----------------กล่องบริการเสริม ---------------

                    #endregion
                }

                return Json(new { PackageYouData = sffresult }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json("ERROR", JsonRequestBehavior.AllowGet);
            }
        }


        public JsonResult GetPackageDetailByNonMobileNewVersion(string mobileNo = "", string option = "")
        {
            var test = "step 1";
            string FullUrl = Session["FullUrl"] != null ? Session["FullUrl"].ToSafeString() : "";

            #region Get IP Address Interface Log
            // Get IP Address
            string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress)) ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            #endregion

            GetPackageDetailByNonMobileQueryModel result = new GetPackageDetailByNonMobileQueryModel();
            result.RETURN_CODE = "-1";

            try
            {
                var promotionByPackageQuery = new evOMQueryListServiceAndPromotionByPackageTypeQuery
                {
                    mobileNo = mobileNo.ToSafeString(),
                    idCard = mobileNo.ToSafeString(),
                    FullUrl = FullUrl.ToSafeString()
                };

                var promotionByPackageData = _queryProcessor.Execute(promotionByPackageQuery);

                if (promotionByPackageData != null && promotionByPackageData.ListPromotion != null
                && promotionByPackageData.ListPromotion.Count > 0)
                {

                    List<PackageFbbor051> sentData = promotionByPackageData.ListPromotion.Select(c => new PackageFbbor051
                    {
                        fibrenetId = mobileNo.ToSafeString(),
                        productCD = c.productCD,
                        productClass = c.productClass,
                        productType = c.productType,
                        startDate = c.startDate,
                        endDate = c.endDate
                    }).ToList();

                    GetPackageDetailByNonMobileQuery getPackageDetailByNonMobileQuery = new GetPackageDetailByNonMobileQuery()
                    {
                        P_FIBRENET_ID = mobileNo.ToSafeString(),
                        P_FBBOR051_PACKAGE_ARRAY = sentData
                    };
                    test += " , step 2 : call GetPackageDetailByNonMobileQuery";
                    result = _queryProcessor.Execute(getPackageDetailByNonMobileQuery);



                    #region SFFService Option 2

                    test += " , step 3 : call evESQueryPersonalInformationQuery option 2";
                    var query02 = new evESQueryPersonalInformationQuery()
                    {
                        mobileNo = mobileNo,
                        option = "2",
                        FullUrl = FullUrl,

                    };
                    //var sffresult_ = _queryProcessor.Execute(query02).Where(s => s.productClass != null).ToList();
                    var sffresult = _queryProcessor.Execute(query02);

                    //List<evESQueryPersonalInformationModel> sffresult = new List<evESQueryPersonalInformationModel>(); var xml = @"<?xml version='1.0' encoding='utf - 16'?> <ArrayOfEvESQueryPersonalInformationModel xmlns:xsd='http://www.w3.org/2001/XMLSchema' xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'> <evESQueryPersonalInformationModel> <monthlyFee>0</monthlyFee> <priceExclVat>0</priceExclVat> </evESQueryPersonalInformationModel> <evESQueryPersonalInformationModel> <billCycle/> <promotionName>AIS PLAYBOX ATV Bundling 24 months for box no.1</promotionName> <productClass>On-Top Extra</productClass> <produuctGroup>Monthly Fee</produuctGroup> <productCd>P211009229</productCd> <endDt>16/03/2024 00:00:00</endDt> <shortNameThai>ฟรีค่าบริการรายเดือน AIS PLAYBOX จุดที่ 1</shortNameThai> <shortNameEng>AIS PLAYBOX Bundling for box No. 1</shortNameEng> <startDt>15/03/2022 17:43:30</startDt> <descThai>ฟรีค่าบริการรายเดือน AIS PLAYBOX จุดที่ 1 นาน 24 รอบบิล</descThai> <descEng>AIS PLAYBOX Bundling for box No. 1 for 24 Bill Cycles</descEng> <inStatementThai>ฟรีค่าบริการรายเดือน AIS PLAYBOX จุดที่ 1 นาน 24 รอบบิล</inStatementThai> <inStatementEng>AIS PLAYBOX Bundling for box No. 1 for 24 Bill Cycles</inStatementEng> <priceType>Recurring</priceType> <productSeq>109</productSeq> <monthlyFee>0.00</monthlyFee> <nextBillCycle/> <crmFlg>N</crmFlg> <paymentMode>Post-paid</paymentMode> <priceExclVat>0.00</priceExclVat> </evESQueryPersonalInformationModel> <evESQueryPersonalInformationModel> <billCycle/> <promotionName>Installation Promotion Charge FTTH 4,800 THB (Prorate) - Contract 24 months</promotionName> <productClass>On-Top Extra</productClass> <produuctGroup>Monthly Fee</produuctGroup> <productPkg>Penalty Package</productPkg> <productCd>P19087344</productCd> <endDt>15/03/2024 17:43:27</endDt> <shortNameThai>ส่วนลดค่าติดตั้ง 4,800 บาท (Prorate) เมื่อใช้บริการครบ 24 รอบบิล</shortNameThai> <shortNameEng>Discount for installation fee 4,800 THB (Prorate) for contract 24 months.</shortNameEng> <startDt>15/03/2022 17:43:40</startDt> <descThai>เรียกคืนส่วนลดค่าติดตั้ง 4,800 บาท กรณียกเลิกก่อนครบอายุสัญญา 24 เดือน (Prorate)</descThai> <descEng>Installation promotion charge 4,800 baht in case terminate the contract before 24 months. (Prorate)</descEng> <inStatementThai>เรียกคืนส่วนลดค่าติดตั้ง 4,800 บาท กรณียกเลิกก่อนครบอายุสัญญา 24 เดือน (Prorate)</inStatementThai> <inStatementEng>Installation promotion charge 4,800 baht in case terminate the contract before 24 months. (Prorate)</inStatementEng> <priceType>Usage</priceType> <monthlyFee>4800.00</monthlyFee> <nextBillCycle/> <crmFlg>N</crmFlg> <paymentMode>Post-paid</paymentMode> <priceExclVat>4485.98</priceExclVat> </evESQueryPersonalInformationModel> <evESQueryPersonalInformationModel> <billCycle/> <promotionName>FBB Purge Discount 50% for 24 months</promotionName> <productClass>On-Top</productClass> <produuctGroup>Monthly Fee</produuctGroup> <productPkg>FBB_DISCOUNT</productPkg> <productCd>P211109718</productCd> <endDt>16/03/2024 00:00:00</endDt> <shortNameThai>FBB Purge Discount 50% for 24 months</shortNameThai> <shortNameEng>FBB Purge Discount 50% for 24 months</shortNameEng> <startDt>15/03/2022 17:43:30</startDt> <descThai>ส่วนลดแพ็กเกจ AIS Fibre 50% นาน 24 เดือน</descThai> <descEng>AIS Fibre discount 50% for 24 months</descEng> <inStatementThai>ส่วนลดแพ็กเกจ AIS Fibre 50% นาน 24 เดือน</inStatementThai> <inStatementEng>AIS Fibre discount 50% for 24 months</inStatementEng> <priceType>Recurring</priceType> <productSeq>107#108</productSeq> <monthlyFee>0.00</monthlyFee> <nextBillCycle/> <crmFlg>N</crmFlg> <paymentMode>Post-paid</paymentMode> <priceExclVat>0.00</priceExclVat> </evESQueryPersonalInformationModel> <evESQueryPersonalInformationModel> <billCycle/> <promotionName>NGB_PLAY FAMILY_ATV_Free_24Months</promotionName> <productClass>On-Top</productClass> <produuctGroup>VAS Promotion</produuctGroup> <productPkg>AIS_ON_AIR</productPkg> <productCd>P211109688</productCd> <endDt>15/03/2024 00:00:00</endDt> <shortNameThai>แพ็กเกจ PLAY FAMILY ฟรีนาน 24 เดือน</shortNameThai> <shortNameEng>PLAY FAMILY Package free 24 months</shortNameEng> <startDt>15/03/2022 17:43:30</startDt> <descThai>แพ็กเกจ PLAY FAMILY ฟรีนาน 24 เดือน ดูหนัง ซีรีส์ วาไรตี้ การ์ตูน และ ข่าว จากช่องดังระดับโลก จาก AIS PLAY</descThai> <descEng>PLAY FAMILY Package free 24 months. Enjoy movies, series, varieties, cartoons and news from premium channels from AIS PLAY.</descEng> <inStatementThai>แพ็กเกจ PLAY FAMILY ฟรีนาน 24 เดือน ดูหนัง ซีรีส์ วาไรตี้ การ์ตูน และ ข่าว จากช่องดังระดับโลก จาก AIS PLAY</inStatementThai> <inStatementEng>PLAY FAMILY Package free 24 months. Enjoy movies, series, varieties, cartoons and news from premium channels from AIS PLAY.</inStatementEng> <priceType>Recurring</priceType> <productSeq>100</productSeq> <monthlyFee>0.00</monthlyFee> <nextBillCycle/> <crmFlg>N</crmFlg> <paymentMode>Post-paid</paymentMode> <priceExclVat>0.00</priceExclVat> </evESQueryPersonalInformationModel> <evESQueryPersonalInformationModel> <billCycle/> <promotionName>POWER4 GIGA Special II 1000/500 Mbps 1,059 THB 24 months</promotionName> <productClass>Main</productClass> <produuctGroup>Monthly Fee</produuctGroup> <productCd>P220110464</productCd> <endDt>16/03/2024 00:00:00</endDt> <shortNameThai>POWER4 GIGA Special II 1000/500 Mbps 1,059/M 24M</shortNameThai> <shortNameEng>POWER4 GIGA Special II 1000/500 Mbps 1,059/M 24M</shortNameEng> <startDt>15/03/2022 17:43:30</startDt> <descThai>แพ็กเกจ POWER4 GIGA Special II 1000/500 Mbps ค่าบริการรายเดือน 1,059 บาท พร้อมกล่อง AIS PLAYBOX และเน็ตมือถือไม่จำกัด ความเร็วสูงสุดไม่เกิน 10 Mbps, โทรฟรี 200 นาที</descThai> <descEng>POWER4 GIGA Special II 1000/500 Mbps Monthly fee 1,059 THB bundled with AIS PLAYBOX, get AIS postpaid unlimited Internet at max speed of 10 Mbps, voice 200 mins</descEng> <inStatementThai>แพ็กเกจ POWER4 GIGA Special II 1000/500 Mbps ค่าบริการรายเดือน 1,059 บาท พร้อมกล่อง AIS PLAYBOX และเน็ตมือถือไม่จำกัด ความเร็วสูงสุดไม่เกิน 10 Mbps, โทรฟรี 200 นาที</inStatementThai> <inStatementEng>POWER4 GIGA Special II 1000/500 Mbps Monthly fee 1,059 THB bundled with AIS PLAYBOX, get AIS postpaid unlimited Internet at max speed of 10 Mbps, voice 200 mins</inStatementEng> <priceType>Recurring</priceType> <productSeq>106</productSeq> <monthlyFee>1133.13</monthlyFee> <nextBillCycle/> <crmFlg>N</crmFlg> <paymentMode>Post-paid</paymentMode> <priceExclVat>1059.00</priceExclVat> </evESQueryPersonalInformationModel> </ArrayOfEvESQueryPersonalInformationModel> ";
                    //var custRegisterCommand = (evESQueryPersonalInformationModel)xml.DeserializedToObj<evESQueryPersonalInformationModel>();
                    //XDocument doc = XDocument.Parse(xml);
                    //if (doc.Root != null)
                    //{
                    //    sffresult = (from r in doc.Root.Elements("evESQueryPersonalInformationModel")
                    //                 select new evESQueryPersonalInformationModel
                    //                 {
                    //                     productClass = (string)r.Element("productClass"),
                    //                     startDt = (string)r.Element("startDt"),
                    //                     endDt = (string)r.Element("endDt"),
                    //                     shortNameThai = (string)r.Element("shortNameThai"),
                    //                     inStatementThai = (string)r.Element("inStatementThai"),
                    //                     mobileNo = (string)r.Element("mobileNo"),
                    //                     productPkg = (string)r.Element("productPkg"),
                    //                     inStatementEng = (string)r.Element("inStatementEng"),
                    //                     shortNameEng = (string)r.Element("shortNameEng")
                    //                 }).ToList();
                    //}

                    //----------------กล่อง แพ็กเกจหลัก-------------------
                    Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US"); //ADD : 25/05/2023
                    var dtnow = DateTime.Now;

                    try
                    {
                        test += " , step 4 : แพ็กเกจหลัก from option 2";
                        var productClass = new[] { "Main" };
                        var packmainsff = (from pm in sffresult
                                           where productClass.Contains(pm.productClass)
                                           let startdt = pm.startDt != null ? DateTime.ParseExact(pm.startDt.Split(' ').FirstOrDefault(), "dd/MM/yyyy", null) : DateTime.Now
                                           let enddt = pm.endDt != null ? DateTime.ParseExact(pm.endDt.Split(' ').FirstOrDefault(), "dd/MM/yyyy", null) : DateTime.Now
                                           let diff = enddt - startdt
                                           where dtnow >= startdt && dtnow <= enddt
                                           where pm.shortNameThai != null
                                           where pm.inStatementThai != null
                                           orderby enddt descending
                                           select new WelcomePackageMain
                                           {
                                               PROMOTION_DISPLAY = pm.shortNameThai,
                                               PROMOTION_NAME = pm.inStatementThai,
                                               FIBRENET_ID = mobileNo,
                                               PROMOTION_EXPIRE = pm.endDt.ToDisplayThaiDate(),
                                           }).Take(1).ToList();



                        result.PACKAGE_MAIN = (from sp in result.PACKAGE_MAIN
                                               join sff in packmainsff on sp.FIBRENET_ID equals sff.FIBRENET_ID
                                               select new WelcomePackageMain
                                               {
                                                   PROMOTION_DISPLAY = sff.PROMOTION_DISPLAY,
                                                   PROMOTION_NAME = sff.PROMOTION_NAME,
                                                   FIBRENET_ID = mobileNo,
                                                   PROMOTION_EXPIRE = sff.PROMOTION_EXPIRE,
                                                   DOWNLOAD_SPEED = sp.DOWNLOAD_SPEED,
                                                   UPLOAD_SPEED = sp.UPLOAD_SPEED
                                               }).Take(1).ToList();
                    }
                    catch
                    {
                    }

                    test += " , step 5 :end  แพ็กเกจหลัก from option 2";
                    result.PACKAGE_ALL_OPTION2 = sffresult;

                    //----------------กล่อง ส่วนลด-------------------
                    test += " , step 5.1 ";
                    var discountblob = LovData.Where(w => w.Name == "DISCOUNT" && w.ActiveFlag == "Y").FirstOrDefault();

                    result.discountblob = "";
                    if (discountblob != null && discountblob.Image_blob != null)
                    {
                        result.discountblob = Convert.ToBase64String(discountblob.Image_blob, 0, discountblob.Image_blob.Length);
                    }


                    //----------------กล่องแพ็คเกจเสริม-------------------
                    test += " , step 5.2 ";
                    var contentpiblob = LovData.Where(w => w.Name == "CONTENTPI" && w.ActiveFlag == "Y").FirstOrDefault();
                    result.contentpiblob = "";
                    if (contentpiblob != null && contentpiblob.Image_blob != null)
                    {
                        result.contentpiblob = Convert.ToBase64String(contentpiblob.Image_blob, 0, contentpiblob.Image_blob.Length);
                    }

                    //-----------------กล่องเงื่อนไข---------------


                    //-----------------กล่องบริการเสริม ---------------
                    test += " , step 5.2 ";
                    var meshpic = LovData.Where(w => w.Name == "MESHPIC" && w.ActiveFlag == "Y").FirstOrDefault();
                    var playbox = LovData.Where(w => w.Name == "PLAYBOX" && w.ActiveFlag == "Y").FirstOrDefault();
                    string blobmeshpic = "";
                    if (meshpic != null && meshpic.Image_blob != null)
                    {
                        blobmeshpic = Convert.ToBase64String(meshpic.Image_blob, 0, meshpic.Image_blob.Length);
                    }
                    result.blobmeshpic = blobmeshpic;

                    string blobplaybox = "";
                    if (playbox != null && playbox.Image_blob != null)
                    {
                        blobplaybox = Convert.ToBase64String(playbox.Image_blob, 0, playbox.Image_blob.Length);
                    }

                    result.blobplaybox = blobplaybox;

                    #endregion

                    #region SFFService Option 3     
                    test += " , step 6 :call  แพ็กเกจหลัก from option 3";
                    var query03 = new evESQueryPersonalInformationQuery()
                    {
                        mobileNo = mobileNo,
                        option = "3",
                        FullUrl = FullUrl,
                        sourceSystem = "MYAIS"
                    };
                    //var sffresult03 = _queryProcessor.Execute(query03).Where(s=>s.productClass != null).ToList();
                    var sffresult03 = _queryProcessor.Execute(query03);
                    result.PACKAGE_ALL_OPTION3 = sffresult03;

                    #endregion

                    test += " , step 7 : end call  แพ็กเกจหลัก from option 3";
                    if (result != null && result.RETURN_CODE != "-1")
                    {
                        //PackageYouData = getPackageYouData
                        return Json(new { PackageYouData = result }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json("ERROR : " + test, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json("ERROR : " + test, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                var msg = "ERROR: catch" + ex.Message + test;
                return Json(msg, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
    }
}
