using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using WBBBusinessLayer;
using WBBBusinessLayer.Extension.Security;
using WBBContract;
using WBBContract.Queries.ExWebServices;
using WBBContract.Queries.WebServices;
using WBBEntity.Extensions;
using WBBEntity.PanelModels;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBWeb.Controllers
{
    public class IPCameraController : WBBController
    {
        private readonly IQueryProcessor _queryProcessor;

        public IPCameraController(IQueryProcessor queryProcessor, ILogger ilogger)
        {
            _queryProcessor = queryProcessor;
            base.Logger = ilogger;
  
        }

        //
        // GET: /CustomerVerification?transactionID=20230613110641236
        public ActionResult CustomerVerification(string transactionID, string email)
        {
            try
            {
                DateTime.ParseExact(transactionID, "yyyyMMddHHmmssfff", CultureInfo.InvariantCulture);
                Session["transactionID"] = transactionID;
                Session["email"] = email;

                var config3BB = base.LovData.FirstOrDefault(lov => lov.Type == "FBB_CONFIG" && lov.Name == "3BB_IPCAMERA");

                ViewData["IsLogoAIS3BB"] = config3BB.LovValue5 == "Y";

                ViewData["session_transactionID"] = transactionID;
                ViewData["session_email"] = email;

                return View();
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized, "You are not authorized to access this controller action.");
            }
        }


        public JsonResult CheckInternetNo(string internetNo, string option)
        {
            string outServiceMobileNo = "";
            string ShowServiceMobileNo = "";
            string outMobileNumber = "";
            string outAccountUuid = "";
            string outServiceName = "AIS";
            string FullUrl = Session["FullUrl"] != null ? Session["FullUrl"].ToSafeString() : "";
            string originalOutServiceMobileNo = "";
            string originalOutMobileNo = "";
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
                    inMobileNo = internetNo,
                    inCardNo = "",
                    inCardType = "",
                    Page = "Customer Verification",
                    Username = "Customer",
                    ClientIP = ipAddress,
                    FullUrl = FullUrl
                };
                var a = _queryProcessor.Execute(query);

                if (a.outServiceMobileNo != null)
                {
                    outServiceMobileNo = a.outServiceMobileNo.ToSafeString();
                    originalOutServiceMobileNo = a.outServiceMobileNo.ToSafeString();
                    outServiceMobileNo = outServiceMobileNo.Trim();
                }
                    

                if (a.outMobileNumber != null)
                {
                    outMobileNumber = a.outMobileNumber.ToSafeString();
                    originalOutMobileNo = a.outMobileNumber.ToSafeString();
                    outMobileNumber = outMobileNumber.Trim();

                }
                Session["CUSTOMER_VERIFICATION_MOBILEDATA"] = outServiceMobileNo;

                if (!string.IsNullOrEmpty(a.outIPCamera3BBAccountUuid))
                {
                    outServiceName = "3BB";
                    outAccountUuid = a.outIPCamera3BBAccountUuid.ToSafeString();
                }

                Session["uuid"] = outAccountUuid;

                if (!string.IsNullOrEmpty(outServiceMobileNo))
                {
                    ShowServiceMobileNo = string.IsNullOrEmpty(outServiceMobileNo) ? "-" : outServiceMobileNo.Substring(0, 3) + "xxx" + outServiceMobileNo.Substring(outServiceMobileNo.Length - 4);
                    var hideinternetNo = internetNo.Substring(0, 3) + "xxx" + internetNo.Substring(internetNo.Length - 4);
                    return Json(new
                    {
                        data = new
                        {
                            internetNo = internetNo,
                            hideinternetNo = hideinternetNo,
                            originalOutServiceMobileNo = originalOutServiceMobileNo,
                            MobileNo = outServiceMobileNo,
                            hideMobileNo = ShowServiceMobileNo,
                            errorMessage = "",
                            serviceName = outServiceName,
                        }
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new
                    {
                        data = new
                        {
                            internetNo = "",
                            hideMobileNo = "",
                            MobileNo = "",
                            //errorMessage = "No Profile",
                            //errorMessage =  a.outIPCamera3bbErrorMessage ?? "No Profile",
                            errorMessage =  a.outIPCamera3bbErrorMessage,
                            serviceName = outServiceName,
                        }
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                string messageError = getErrorCodeOTP3BB("OTHER", outServiceMobileNo);
                return Json(new
                {
                    data = new
                    {
                        internetNo = "",
                        hideMobileNo = "",
                        MobileNo = "",
                        errorMessage = messageError,
                        serviceName = outServiceName,
                    }
                }, JsonRequestBehavior.AllowGet);
            }

        }


        public JsonResult SendOneTimePW(string internetNo, string nonMobile, string transactionID)
        {
            var hideinternetNo = internetNo.Substring(0, 3) + "xxx" + internetNo.Substring(internetNo.Length - 4);
            var ShowServiceMobileNo = string.IsNullOrEmpty(nonMobile) ? "-" : nonMobile.Substring(0, 3) + "xxx" + nonMobile.Substring(nonMobile.Length - 4);
            //string transactionID = Session["transactionID"].ToSafeString();
            string uuid = Session["uuid"].ToSafeString();

            var getLOV = base.LovData.Where(w => w.Type == "IPCAMERA" && w.Name == "FBBWEB_CUSTOMER_VERIFICATION").FirstOrDefault();

            if (getLOV.ActiveFlag != "Y" || getLOV.LovValue5 != "Y")
            {
                bool use3bbService = !string.IsNullOrEmpty(uuid);
                var resultSendOneTime = SendOneTimePWPrivate(internetNo, "All", use3bbService);
                if (resultSendOneTime != null && resultSendOneTime.code == "000")
                {
                    var transactionIDs = resultSendOneTime.transactionID.ToSafeString();

                    return Json(new
                    {
                        data = new
                        {
                            code = resultSendOneTime.code,
                            description = resultSendOneTime.description,
                            internetNo = internetNo,
                            hideinternetNo = hideinternetNo,
                            MobileNo = nonMobile,
                            hideMobileNo = ShowServiceMobileNo,
                            transactionID = transactionIDs,
                            timer = 5,
                            errorMessage = ""
                        }
                    }, JsonRequestBehavior.AllowGet);
                }
                else if (resultSendOneTime == null)
                {
                    string messageError = getErrorCodeOTP(null, nonMobile);
                    return Json(new
                    {
                        data = new
                        {
                            code = "",
                            description = "",
                            internetNo = "",
                            hideMobileNo = "",
                            MobileNo = "",
                            timer = 5,
                            errorMessage = messageError
                        }
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var errorMessageOTP = getErrorCodeOTP(resultSendOneTime.code, nonMobile);
                    string messageError = errorMessageOTP.ToSafeString();
                    return Json(new
                    {
                        data = new
                        {
                            code = resultSendOneTime.code,
                            description = resultSendOneTime.description,
                            internetNo = "",
                            hideMobileNo = "",
                            MobileNo = "",
                            timer = 5,
                            errorMessage = messageError
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
                        internetNo = internetNo,
                        hideinternetNo = hideinternetNo,
                        MobileNo = nonMobile,
                        hideMobileNo = ShowServiceMobileNo,
                        transactionID = transactionID,
                        timer = 5,
                        errorMessage = ""
                    }
                }, JsonRequestBehavior.AllowGet);
            }

        }



        public JsonResult OTPVerification(string nonMobile, string msisdn, string pwd, string transactionID)
        {
            //var mobileNo = WBBDecrypt.textDecrpyt(msisdn);
            var mobileData = Session["CUSTOMER_VERIFICATION_MOBILEDATA"].ToSafeString();
            GssoSsoResponseModel result = new GssoSsoResponseModel();
            var errorMessageOTP ="";
            string errorMessage = "";
            var textResendNew = "";
            string resendNew = "";
            if (mobileData == msisdn)
            {
                try
                {

                    var getLOV = base.LovData.Where(w => w.Type == "IPCAMERA" && w.Name == "FBBWEB_CUSTOMER_VERIFICATION").FirstOrDefault();

                    if (getLOV.ActiveFlag != "Y" || getLOV.LovValue5 != "Y")
                    {
                        var query = new ConfirmOneTimePWQuery()
                        {
                            msisdn = msisdn,
                            pwd = pwd,
                            transactionID = transactionID
                        };
                        result = _queryProcessor.Execute(query);
                    }
                    else
                    {
                        result.code = "000";
                        result.transactionID = transactionID;
                    }

                    if (result != null && result.code == "000" && result.transactionID == transactionID)
                    {
                        transactionID = result.transactionID.ToSafeString();

                        return Json(new { data = new { MobileNo = msisdn, status = true, errorMessage = "" } }, JsonRequestBehavior.AllowGet);
                    }
                    else if (result == null)
                    {
                        errorMessageOTP = getErrorCodeOTP3BB(null, msisdn);
                        errorMessage = errorMessageOTP.ToSafeString();
                        return Json(new { data = new { MobileNo = "", status = false, errorMessage = errorMessage } }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        errorMessageOTP = getErrorCodeOTP3BB(result.code, msisdn);
                        errorMessage = errorMessageOTP.ToSafeString();
                        if(result.code == "003")
                        {
                            textResendNew = getErrorCodeOTP3BB("RESENTNEW", msisdn);
                            resendNew = textResendNew.ToSafeString();
                        }
                        else
                        {
                            resendNew = "";
                        }
                        return Json(new { data = new { MobileNo = "", status = false, errorMessage = errorMessage, resendNewOTP = resendNew } }, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception ex)
                {
                    errorMessageOTP = getErrorCodeOTP3BB("ERROR", msisdn);
                    errorMessage = errorMessageOTP.ToSafeString();
                    textResendNew = getErrorCodeOTP3BB("RESENTNEW", msisdn);
                    resendNew = textResendNew.ToSafeString();
                    return Json(new { data = new { MobileNo = "", status = false, errorMessage = errorMessage,resendNewOTP = resendNew } }, JsonRequestBehavior.AllowGet);
                }
            }
            errorMessageOTP = getErrorCodeOTP3BB("ERROR", msisdn);
            errorMessage = errorMessageOTP.ToSafeString();
            textResendNew = getErrorCodeOTP3BB("RESENTNEW", msisdn);
            resendNew = textResendNew.ToSafeString();
            return Json(new { data = new { MobileNo = "", status = false, errorMessage = errorMessage, resendNewOTP = resendNew } }, JsonRequestBehavior.AllowGet);
        }




        public JsonResult getUserAccount(string mobileNo, string transactionID, string email)
        {
            string FullUrl = Session["FullUrl"] != null ? Session["FullUrl"].ToSafeString() : "";
            //string transactionID = Session["transactionID"].ToSafeString();
            //string email = Session["email"].ToSafeString();
            string uuid = Session["uuid"].ToSafeString();

            #region Get IP Address Interface Log
            // Get IP Address
            string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress)) ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            #endregion

            try
            {
                var getLOV = base.LovData.Where(w => w.Type == "IPCAMERA" && w.Name == "FBBWEB_CUSTOMER_VERIFICATION").FirstOrDefault();

                if (getLOV.ActiveFlag != "Y" || getLOV.LovValue5 != "Y")
                {
                    if (!string.IsNullOrEmpty(uuid)) //Uuid 3bb
                    {
                        var resSuccess = new ResponseIpcameraSuccess
                        {
                            result = "SUCCESS",
                            code = "200",
                            payload = new IpcameraSuccess { transactionID = transactionID, uuid = uuid, email = email }
                        };

                        return Json(new { data = resSuccess }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var promotionByPackageQuery = new evOMQueryListServiceAndPromotionByPackageTypeQuery
                        {
                            mobileNo = mobileNo.ToSafeString(),
                            serviceType = "All",
                            idCard = mobileNo.ToSafeString(),
                            FullUrl = FullUrl.ToSafeString()
                        };

                        var promotionByPackageData = _queryProcessor.Execute(promotionByPackageQuery);

                        if (promotionByPackageData?.userAccount != null)
                        {
                            var result = new IpcameraSuccess
                            {
                                transactionID = transactionID,
                                uuid = promotionByPackageData.userAccount,
                                email = email
                            };

                            var resSuccess = new ResponseIpcameraSuccess
                            {
                                result = "SUCCESS",
                                code = "200",
                                payload = result
                            };

                            return Json(new { data = resSuccess }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {

                            var resFail = new ResponseIpcameraFail
                            {
                                result = "FAIL",
                                code = "101",
                                errorMessage = "ไม่พบบริการ Cloud IP Camera สำหรับหมายเลขอินเทอร์เน็ตนี้",
                                payload = new IpcameraFail
                                {
                                    status = "Authentication Fail",
                                }
                            };
                            return Json(new { data = resFail }, JsonRequestBehavior.AllowGet);
                            //return Json(new { data = resFail}, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                else
                {
                    var resSuccess = new ResponseIpcameraSuccess
                    {
                        result = "SUCCESS",
                        code = "200",
                        payload = new IpcameraSuccess
                        {
                            transactionID = transactionID,
                            email = email,
                            uuid = getLOV.LovValue2
                        }
                    };

                    var resFail = new ResponseIpcameraFail
                    {
                        result = "FAIL",
                        code = "101",
                        payload = new IpcameraFail
                        {
                            status = "Authentication Fail"
                        }
                    };
                    if (getLOV.LovValue1 == "Success")
                        return Json(new { data = resSuccess }, JsonRequestBehavior.AllowGet);
                    else
                        return Json(new { data = resFail }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                return Json("ERROR", JsonRequestBehavior.AllowGet);
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

        private string getErrorCodeOTP3BB(string Error_Code = "", string Mobile_No = "")
        {
            try
            {
                string Massage = null;

                IEnumerable<LovValueModel> Error_Massage;
                string[] errorCodes = LovData.Where(d => d.Text != null && d.Name == "L_ERROR_OTP_3BB").Select(d => d.Text).ToArray();

                if (!errorCodes.Contains(Error_Code))
                    Error_Massage = base.LovData.Where(l => l.Name == "L_ERROR_OTP_3BB" && l.Text == null);
                else
                    Error_Massage = base.LovData.Where(l => l.Name == "L_ERROR_OTP_3BB" && l.Text == Error_Code);

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

        private GssoSsoResponseModel SendOneTimePWPrivate(string msisdn, string accountType, bool use3BBService)
        {
            //send OTP
            try
            {
                var query = new SendOneTimePWQuery();
                query.msisdn = msisdn;
                query.accountType = accountType;
                query.use3BBService = use3BBService;
              
                var result = _queryProcessor.Execute(query);

                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public JsonResult ConfirmOneTimePW(string nonMobile, string msisdn, string pwd, string transactionID)
        {
            //send OTP
            var mobileNo = WBBDecrypt.textDecrpyt(msisdn);
            var mobileData = Session["CUSTOMER_VERIFICATION_MOBILEDATA"].ToSafeString();
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
    }
}
