using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using System.Xml.Serialization;
using WBBContract.Commands;
using WBBContract.Commands.WebServices;
using WBBContract.Queries.Commons.Masters;
using WBBContract.Queries.WebServices;
using WBBEntity.Extensions;
using WBBEntity.PanelModels;
using WBBEntity.PanelModels.WebServiceModels;
using WBBWeb.Hubs;
using WBBWeb.Models;

namespace WBBWeb.Controllers
{
    public partial class ProcessController : WBBController
    {
        [HttpPost]
        public ActionResult PaymentWithPoint(QuickWinPanelModel model)
        {
            ActionResult action = null;
            string fullUrlStr = this.Url.Action("TopupMesh", "Process", null, this.Request.Url.Scheme);

            #region Get IP Address Interface Log : Edit 2017-01-30

            string transactionId = "";

            // Get IP Address
            string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress)) ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            transactionId = ipAddress + "|";

            #endregion

            var LovData = GetScreenConfig("FBBOR041");
            string ErrorMsgNotPaid = LovData.Where(t => t.Name == "L_POPUP_PAYMENT_ERROR").Select(t => t.DisplayValue).Count() > 0 ? LovData.Where(t => t.Name == "L_POPUP_PAYMENT_ERROR").Select(t => t.DisplayValue).FirstOrDefault() : "";
            string ErrorMsg = LovData.Where(t => t.Name == "L_POPUP_ERROR").Select(t => t.DisplayValue).Count() > 0 ? LovData.Where(t => t.Name == "L_POPUP_ERROR").Select(t => t.DisplayValue).FirstOrDefault() : "";

            model.PayMentMethod = "";
            string MobileRedeemPoint = "";
            if (model.PayMentPointType == "1")
            {
                MobileRedeemPoint = model.CustomerRegisterPanelModel.L_CONTACT_PHONE;
            }
            else if (model.PayMentPointType == "2")
            {
                MobileRedeemPoint = model.CoveragePanelModel.P_MOBILE;
            }

            string transactionIDPayPoint = PrivilegeRedeemPoint(model.PayMentOrderID.ToSafeString(), MobileRedeemPoint, model.SffPromotiontCodeMeshSelect);

            if (transactionIDPayPoint != "")
            {

                bool CreateOrderStatus = false;
                /// Getvalue for sent to sff
                GetOrderChangeServiceModel DataForSentToSff = null;

                DataForSentToSff = GetOrderChangeService(model.CoveragePanelModel.P_MOBILE, model.PayMentOrderID);

                if (DataForSentToSff != null && DataForSentToSff.RespCode != null && DataForSentToSff.RespCode != "-1")
                {
                    /// Sent data to sff
                    CreateOrderMeshPromotionResult result = null;
                    result = CreateOrderMeshPromotion(DataForSentToSff, model.CoveragePanelModel.P_MOBILE);
                    if (result != null && result.order_no.ToSafeString() != "")
                    {

                        /// Update CustRegister 
                        var customerRowID = RegisterCustomerMesh(result.order_no.ToSafeString(), model.PayMentOrderID.ToSafeString(), transactionIDPayPoint, model.PayMentMethod.ToSafeString());

                        /// Get value info
                        string informationData = "";
                        informationData = LovData.Where(t => t.Name == "L_POPUP_SUCCESS").Select(t => t.DisplayValue).Count() > 0 ? LovData.Where(t => t.Name == "L_POPUP_SUCCESS").Select(t => t.DisplayValue).FirstOrDefault() : "";
                        GetMeshCustomerProfileModel informationDataModel = GetMeshCustomerProfile(model.CoveragePanelModel.P_MOBILE, model.PayMentOrderID);
                        informationData = informationData.Replace("[OrderList]", informationDataModel.order_list);
                        informationData = informationData.Replace("[OrderNo]", informationDataModel.amount);
                        informationData = informationData.Replace("[PurchaseAmt]", informationDataModel.purchase_amt);
                        informationData = informationData.Replace("[TranId]", informationDataModel.tran_id);
                        informationData = informationData.Replace("[OrderDate]", informationDataModel.order_date);
                        informationData = informationData.Replace("[CustomerName]", informationDataModel.customer_name);
                        informationData = informationData.Replace("[NonMobileNo]", informationDataModel.non_mobile_no);
                        informationData = informationData.Replace("[ContactMobile]", informationDataModel.contact_mobile);
                        informationData = informationData.Replace("[InstallDate]", informationDataModel.install_date);


                        /// Sent SMS

                        MeshSendSMS(model.CustomerRegisterPanelModel.L_CONTACT_PHONE, informationDataModel.purchase_amt, informationDataModel.install_date, informationDataModel.non_mobile_no, informationDataModel.tran_id, "1", ipAddress, fullUrlStr, GetCurrentCulture().IsThaiCulture(), "");
                        action = action = TopupMesh("", true, "", "", "", "", "", "", "", "", informationData, "", "", "", "", "");
                        CreateOrderStatus = true;
                    }
                }
                if (!CreateOrderStatus)
                {
                    action = TopupMesh("", false, "", "", "", "", "", "", "", "", ErrorMsg, "", "", "", "", "");
                }
            }
            else
            {
                MeshSendSMS(model.CustomerRegisterPanelModel.L_CONTACT_PHONE, "", "", "", "", "2", ipAddress, fullUrlStr, GetCurrentCulture().IsThaiCulture(), "");
                action = TopupMesh("", false, "", "", "", "", "", "", "", "", ErrorMsg, "", "", "", "", "");
            }

            return action;

        }

        public ActionResult PayWithSMS(QuickWinPanelModel model)
        {
            string fullUrlStr = this.Url.Action("TopupMesh", "Process", null, this.Request.Url.Scheme);

            // Get IP Address
            string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress)) ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            string orderId = model.PayMentOrderID;
            Session[orderId] = model;

            string informationData = "";
            ActionResult action;

            /// GetLov data
            var controller = DependencyResolver.Current.GetService<ProcessController>();
            var LovData = controller.GetScreenConfig("FBBOR041");
            string L_POPUP_SEND_SMS = LovData.Where(t => t.Name == "L_POPUP_SEND_SMS").Select(t => t.DisplayValue).Count() > 0 ? LovData.Where(t => t.Name == "L_POPUP_SEND_SMS").Select(t => t.DisplayValue).FirstOrDefault() : "";

            string ContactPhoneStr = "xxx-xxx-";
            if (model.CustomerRegisterPanelModel.L_CONTACT_PHONE != null && model.CustomerRegisterPanelModel.L_CONTACT_PHONE.Length >= 10)
            {
                ContactPhoneStr = ContactPhoneStr + model.CustomerRegisterPanelModel.L_CONTACT_PHONE.Substring(6, 4);
            }

            informationData = L_POPUP_SEND_SMS.Replace("[ContactMobile]", ContactPhoneStr);
            /// Sent SMS
            //string PaymentOrderIDEncrypt = Encrypt(model.CoveragePanelModel.P_MOBILE + "&" + model.PayMentOrderID);
            //string UrlForSentSMS = this.Url.Action("SelectPayment", "PaymentMesh", new { data = PaymentOrderIDEncrypt }, this.Request.Url.Scheme);
            //base.Logger.Info("Call MeshSendSMS");
            //MeshSendSMS(model.CustomerRegisterPanelModel.L_CONTACT_PHONE, "", "", "", "", "3", ipAddress, fullUrlStr, GetCurrentCulture().IsThaiCulture(), UrlForSentSMS);
            //base.Logger.Info("End MeshSendSMS");
            action = TopupMesh("", true, "", "", "", "", "", "", "", "", informationData, "", "", "", "", "");
            return action;
        }

        [HttpPost]
        public JsonResult SendLinkMeshSMS(string Data = "")
        {
            string result = "N";
            string fullUrlStr = this.Url.Action("TopupMesh", "Process", null, this.Request.Url.Scheme);

            // Get IP Address
            string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress)) ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            /// Nonmobile Mobile PayMentOrderID
            string Nonmobile = "";
            string Mobile = "";
            string PayMentOrderID = "";
            if (Data != "")
            {
                string tmpData = Decrypt(Data);
                string[] DataTemps = tmpData.Split('&');
                if (DataTemps != null && DataTemps.Count() == 3)
                {
                    Nonmobile = DataTemps[0].ToSafeString();
                    Mobile = DataTemps[1].ToSafeString();
                    PayMentOrderID = DataTemps[2].ToSafeString();
                }

                /// Sent SMS
                string PaymentOrderIDEncrypt = Encrypt(Nonmobile + "&" + PayMentOrderID);
                //string UrlForSentSMS = this.Url.Action("SelectPayment", "PaymentMesh", new { data = PaymentOrderIDEncrypt }, this.Request.Url.Scheme);
                string RequestStr = "https";
                if (Request.IsLocal)
                {
                    RequestStr = this.Request.Url.Scheme;
                }
                string UrlForSentSMS = this.Url.Action("SelectPayment", "PaymentMesh", new { data = PaymentOrderIDEncrypt }, RequestStr);
                result = MeshSendSMS(Mobile, "", "", "", "", "3", ipAddress, fullUrlStr, GetCurrentCulture().IsThaiCulture(), UrlForSentSMS);
            }

            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> PaymentToMPayGateway(QuickWinPanelModel model)
        {
            string fullUrlStr = this.Url.Action("TopupMesh", "Process", null, this.Request.Url.Scheme);

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

            var LovConfigData = base.LovData.Where(t => t.Name == "RequestOrderTepsApi" && t.LovValue5 == "FBBOR041").ToList();

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
            string reqRef1 = model.CoveragePanelModel.P_MOBILE.ToSafeString();

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

            // ForDev
            if (Request.IsLocal)
            {
                redirectUrl = "http://localhost:50960/Process/PaymentToMPayGatewayResult";
            }

            var result = new response();
            string responseBody = "";
            try
            {
                string sidDecode = System.Web.HttpUtility.UrlDecode(sid);

                SavePaymentLogModel savePaymentLogModel = new SavePaymentLogModel()
                {
                    ACTION = "New",
                    PROCESS_NAME = "RequestPayment",
                    PAYMENT_ORDER_ID = orderId,
                    ENDPOINT = url,
                    REQ_PROJECT_CODE = projectCode,
                    REQ_COMMAND = command,
                    REQ_SID = sidDecode,
                    REQ_REDIRECT_URL = redirectUrl,
                    REQ_MERCHANT_ID = merchantId,
                    REQ_ORDER_ID = orderId,
                    REQ_PURCHASE_AMT = tmpPurchaseAmt,
                    REQ_CURRENCY = currency,
                    REQ_PAYMENT_METHOD = model.PayMentMethod,
                    REQ_SMS_FLAG = smsFlag,
                    REQ_ORDER_EXPIRE = orderExpire,
                    REQ_INTEGRITY_STR = integrityStr,
                    REQ_REF1 = reqRef1,
                    REQ_REF5 = reqRef5
                };
                SavePaymentLog(savePaymentLogModel);
                var contents = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("projectCode", projectCode),
                        new KeyValuePair<string, string>("command", command),
                        new KeyValuePair<string, string>("sid", sidDecode),
                        new KeyValuePair<string, string>("redirectUrl", redirectUrl),
                        new KeyValuePair<string, string>("merchantId", merchantId),
                        new KeyValuePair<string, string>("orderId", orderId),
                        new KeyValuePair<string, string>("purchaseAmt", tmpPurchaseAmt),
                        new KeyValuePair<string, string>("currency", currency),
                        new KeyValuePair<string, string>("paymentMethod", model.PayMentMethod),
                        new KeyValuePair<string, string>("smsFlag", smsFlag),
                        new KeyValuePair<string, string>("orderExpire", orderExpire),
                        new KeyValuePair<string, string>("integrityStr", integrityStr),
                        new KeyValuePair<string, string>("ref1", model.CoveragePanelModel.P_MOBILE.ToSafeString()),
                        new KeyValuePair<string, string>("ref1", reqRef1),
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
                        responseBody = await response.Content.ReadAsStringAsync();

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
                var LovData = GetScreenConfig("FBBOR041");
                MeshSendSMS(model.CustomerRegisterPanelModel.L_CONTACT_PHONE, "", "", "", "", "2", ipAddress, fullUrlStr, GetCurrentCulture().IsThaiCulture(), "");
                string ErrorMsg = LovData.Where(t => t.Name == "L_POPUP_PAYMENT_ERROR").Select(t => t.DisplayValue).Count() > 0 ? LovData.Where(t => t.Name == "L_POPUP_PAYMENT_ERROR").Select(t => t.DisplayValue).FirstOrDefault() : "";
                ActionResult action = TopupMesh("", false, "", "", "", "", "", "", "", "", ErrorMsg, "", "", "", "", "");
                return action;
            }
        }

        [HttpPost]
        public JsonResult GetPaymentToMerchantQrCode(string payMentOrderId, string payMentRecurringChargeVat, string nonMobile)
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

                var lovConfigDataQr = LovData.Where(t => t.Name == "RequestOrderQrcodeApi" && t.LovValue5 == "FBBOR041").ToList();
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

                /// Set NonMobile in ref1
                ref1 = nonMobile;

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
            ActionResult action = null;
            bool CreateOrderStatus = false;
            string fullUrl = this.Url.Action("TopupMesh", "Process", null, this.Request.Url.Scheme);

            #region Get IP Address Interface Log : Edit 2017-01-30

            // Get IP Address
            string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress)) ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            #endregion

            var controller = DependencyResolver.Current.GetService<ProcessController>();
            var LovData = controller.GetScreenConfig("FBBOR041");
            string ErrorMsgNotPaid = LovData.Where(t => t.Name == "L_POPUP_PAYMENT_ERROR").Select(t => t.DisplayValue).Count() > 0 ? LovData.Where(t => t.Name == "L_POPUP_PAYMENT_ERROR").Select(t => t.DisplayValue).FirstOrDefault() : "";
            string ErrorMsg = LovData.Where(t => t.Name == "L_POPUP_ERROR").Select(t => t.DisplayValue).Count() > 0 ? LovData.Where(t => t.Name == "L_POPUP_ERROR").Select(t => t.DisplayValue).FirstOrDefault() : "";

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

            List<string> Statuss = new List<string>();
            Statuss.Add("S");
            Statuss.Add("Success");
            Session[orderId] = null;
            model.PayMentTranID = tranId.ToSafeString();
            if (Statuss.Contains(status) && respCode == "0000")
            {
                /// Getvalue for sent to sff
                GetOrderChangeServiceModel DataForSentToSff = null;

                DataForSentToSff = GetOrderChangeService(model.CoveragePanelModel.P_MOBILE, model.PayMentOrderID);

                if (DataForSentToSff != null && DataForSentToSff.RespCode != null && DataForSentToSff.RespCode != "-1")
                {
                    /// Sent data to sff
                    CreateOrderMeshPromotionResult result = null;
                    result = CreateOrderMeshPromotion(DataForSentToSff, model.CoveragePanelModel.P_MOBILE);
                    if (result != null && result.order_no.ToSafeString() != "")
                    {

                        /// Update CustRegister 
                        var customerRowID = RegisterCustomerMesh(result.order_no.ToSafeString(), orderId.ToSafeString(), tranId.ToSafeString(), model.PayMentMethod.ToSafeString());

                        /// Get value info
                        string informationData = "";
                        informationData = LovData.Where(t => t.Name == "L_POPUP_SUCCESS").Select(t => t.DisplayValue).Count() > 0 ? LovData.Where(t => t.Name == "L_POPUP_SUCCESS").Select(t => t.DisplayValue).FirstOrDefault() : "";
                        GetMeshCustomerProfileModel informationDataModel = GetMeshCustomerProfile(model.CoveragePanelModel.P_MOBILE, model.PayMentOrderID);
                        informationData = informationData.Replace("[OrderList]", informationDataModel.order_list);
                        informationData = informationData.Replace("[OrderNo]", informationDataModel.amount);
                        informationData = informationData.Replace("[PurchaseAmt]", informationDataModel.purchase_amt);
                        informationData = informationData.Replace("[TranId]", tranId.ToSafeString());
                        informationData = informationData.Replace("[OrderDate]", informationDataModel.order_date);
                        informationData = informationData.Replace("[CustomerName]", informationDataModel.customer_name);
                        informationData = informationData.Replace("[NonMobileNo]", informationDataModel.non_mobile_no);
                        informationData = informationData.Replace("[ContactMobile]", informationDataModel.contact_mobile);
                        informationData = informationData.Replace("[InstallDate]", informationDataModel.install_date);

                        /// Sent SMS

                        MeshSendSMS(model.CustomerRegisterPanelModel.L_CONTACT_PHONE, informationDataModel.purchase_amt, informationDataModel.install_date, informationDataModel.non_mobile_no, informationDataModel.tran_id, "1", ipAddress, fullUrl, GetCurrentCulture().IsThaiCulture(), "");
                        action = TopupMesh("", true, "", "", "", "", "", "", "", "", informationData, "", "", "", "", "");
                        CreateOrderStatus = true;
                    }
                }
                if (!CreateOrderStatus)
                {
                    action = TopupMesh("", false, "", "", "", "", "", "", "", "", ErrorMsg, "", "", "", "", "");
                }
            }
            else
            {
                action = TopupMesh("", false, "", "", "", "", "", "", "", "", ErrorMsgNotPaid, "", "", "", "", "");
            }

            if (!CreateOrderStatus)
            {
                /// Sent SMS ErrorMsg
                MeshSendSMS(model.CustomerRegisterPanelModel.L_CONTACT_PHONE, "", "", "", "", "2", ipAddress, fullUrl, GetCurrentCulture().IsThaiCulture(), "");
            }

            return action;
        }

        [HttpPost]
        public ActionResult PayQRResult(string orderId, string tranDtm, string tranId, string serviceId, string terminalId,
            string locationName, string amount, string status, string sof, string qrType, string ref1, string ref2,
            string ref3, string ref4, string ref5)
        {
            try
            {
                Logger.Info(string.Format(
                    "START QR-CODE-PayResult : orderId = {0},tranDtm = {1},tranId = {2},serviceId = {3},terminalId = {4},locationName = {5},amount = {6},status = {7},sof = {8},qrType = {9},ref1 = {10},ref2 = {11},ref3 = {12},ref4 = {13},ref5 = {14}",
                    orderId, tranDtm, tranId, serviceId, terminalId, locationName, amount, status, sof, qrType, ref1,
                    ref2, ref3, ref4, ref5));

                var fullUrl = this.Url.Action("PayResult", "Process", null, Request.Url.Scheme);

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

        [HttpPost]
        public JsonResult CheckSentSMS(string Subtype = "")
        {
            string result = "N";
            if (Subtype != "")
            {
                var LovConfigData = base.LovData.Where(t => t.Name == "MESH_PAY_BY_SEND_SMS" && t.LovValue5 == "FBBOR041" && t.DefaultValue == Subtype).ToList();
                if (LovConfigData != null && LovConfigData.Count() > 0)
                {
                    result = "Y";
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
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

        public void SavePaymentLog(SavePaymentLogModel model)
        {
            string FullUrl = "";
            try
            {
                if (Session["FullUrl"] != null)
                    FullUrl = Session["FullUrl"].ToSafeString();
            }
            catch { }

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

        public GetOrderChangeServiceModel GetOrderChangeService(string internetNo, string paymentOrderID)
        {
            GetOrderChangeServiceQuery changeServiceQuery = new GetOrderChangeServiceQuery()
            {
                p_internet_no = internetNo,
                p_payment_order_id = paymentOrderID
            };

            GetOrderChangeServiceModel result = _queryProcessor.Execute(changeServiceQuery);

            return result;
        }

        public CreateOrderMeshPromotionResult CreateOrderMeshPromotion(GetOrderChangeServiceModel data, string NonMobileNo)
        {
            CreateOrderMeshPromotionResult result = new CreateOrderMeshPromotionResult();
            string FullUrl = "";
            if (Session["FullUrl"] != null)
                FullUrl = Session["FullUrl"].ToSafeString();

            // Get IP Address
            string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress)) ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            CreateOrderMeshPromotionCommand createOrderMeshPromotionCommand = new CreateOrderMeshPromotionCommand()
            {
                GetOrderChangeService = data,
                NonMobileNo = NonMobileNo,
                FullUrl = FullUrl,
                client_ip = ipAddress
            };

            _createOrderMeshPromotionCommand.Handle(createOrderMeshPromotionCommand);

            if (createOrderMeshPromotionCommand.ERROR_MSG == "")
            {
                result.ret_code = createOrderMeshPromotionCommand.VALIDATE_FLAG.ToSafeString();
                result.ret_message = createOrderMeshPromotionCommand.ERROR_MSG.ToSafeString();
                result.order_no = createOrderMeshPromotionCommand.sffOrderNo.ToSafeString();
            }

            return result;
        }

        public string RegisterCustomerMesh(string SffOrder, string oderID, string transactionID, string paymentMethod)
        {
            CustRegisterJobCommand command = new CustRegisterJobCommand()
            {
                RETURN_IA_NO = SffOrder,
                RETURN_ORDER_NO = SffOrder,
                TRANSACTIONID_IN = oderID,
                TRANSACTIONID = transactionID,
                PAYMENTMETHOD = paymentMethod,
                PLUG_AND_PLAY_FLAG = "",
                REGISTER_TYPE = "TOPUP_MESH",
                ClientIP = "",
                FullUrl = ""
            };
            _custRegJobCommand.Handle(command);

            return command.CUSTOMERID;
        }

        public GetMeshCustomerProfileModel GetMeshCustomerProfile(string internetNo, string paymentOrderID)
        {
            GetMeshCustomerProfileModel result = null;
            GetMeshCustomerProfileQuery query = new GetMeshCustomerProfileQuery()
            {
                p_internet_no = internetNo,
                p_payment_order_id = paymentOrderID
            };
            result = _queryProcessor.Execute(query);
            return result;
        }

        public string PrivilegeRedeemPoint(string PaymentOrderID, string MobileNo, string SffPromotionCode)
        {
            string result = "";

            string url = "";
            List<LovValueModel> lovConfigData = new List<LovValueModel>();
            lovConfigData = LovData.Where(t => t.Name == "getPointSet" && t.LovValue5 == "FBBOR041" && t.Type == "FBB_CONSTANT").ToList();
            if (lovConfigData.Count > 0)
            {
                url = lovConfigData.Where(t => t.Text == "url").Select(t => t.LovValue1).Any()
                    ? lovConfigData.Where(t => t.Text == "url").Select(t => t.LovValue1).FirstOrDefault()
                    : "";
            }

            MobileNo = MobileNo.Replace("-", "");

            SavePaymentLogModel savePaymentLogModel = new SavePaymentLogModel()
            {
                ACTION = "New",
                PROCESS_NAME = "requestPrivilegeBarcode",
                PAYMENT_ORDER_ID = PaymentOrderID,
                ENDPOINT = url,
                REQ_COMMAND = "requestPrivilegeBarcode",
                REQ_MOBILE_NO = MobileNo,
                REQ_PAYMENT_METHOD = ""
            };
            SavePaymentLog(savePaymentLogModel);

            PrivilegeRedeemPointQuery query = new PrivilegeRedeemPointQuery()
            {
                SFFPromotioncode = SffPromotionCode,
                PaymentOrderID = PaymentOrderID,
                MobileNo = MobileNo,
                FullURL = ""
            };

            var resultData = _queryProcessor.Execute(query);

            SavePaymentLogModel savePaymentLogModelUpdate = new SavePaymentLogModel();

            if (resultData != null)
            {
                if (resultData.HttpStatus == "200" && resultData.Status == "20000")
                {
                    result = resultData.MsgBarcode.ToSafeString();
                }
                savePaymentLogModelUpdate = new SavePaymentLogModel()
                {
                    ACTION = "Modify",
                    PROCESS_NAME = "requestPrivilegeBarcode",
                    PAYMENT_ORDER_ID = PaymentOrderID,
                    RESP_STATUS = resultData.HttpStatus.ToSafeString(),
                    RESP_RESP_CODE = resultData.Status.ToSafeString(),
                    RESP_RESP_DESC = resultData.Description.ToSafeString(),
                    RESP_REF1 = resultData.MsgBarcode.ToSafeString()
                };
            }
            else
            {
                savePaymentLogModelUpdate = new SavePaymentLogModel()
                {
                    ACTION = "Modify",
                    PROCESS_NAME = "requestPrivilegeBarcode",
                    PAYMENT_ORDER_ID = PaymentOrderID,
                    RESP_STATUS = "",
                    RESP_RESP_CODE = "",
                    RESP_RESP_DESC = "",
                    RESP_REF1 = "No Data"
                };
            }

            SavePaymentLog(savePaymentLogModelUpdate);

            return result;
        }

        [HttpPost]
        public ActionResult PaymentToSuperDuper(QuickWinPanelModel model)
        {
            string fullUrlStr = this.Url.Action("PaymentToSuperDuper", "Process", null, this.Request.Url.Scheme);

            #region Get IP Address Interface Log : Edit 2017-01-30

            string transactionId = "";

            // Get IP Address
            string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress)) ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            transactionId = ipAddress + "|";

            #endregion

            string endPointUrl = "";

            var PaymentToSuperDuperData = new GetPaymentToSuperDuperModel();

            if (!string.IsNullOrEmpty(model.CoveragePanelModel.P_MOBILE))
            {
                PaymentToSuperDuperData = GetPaymentToSuperDuper(model.CoveragePanelModel.P_MOBILE.ToSafeString(), model.SuperDuperProductName.ToSafeString(), model.SuperDuperServiceName.ToSafeString(), model.PayMentOrderID.ToSafeString(), model.PayMentRecurringChargeVAT.ToSafeString(), model.outBillingAccountNumber);
            }
            else
            {
                PaymentToSuperDuperData = GetPaymentToSuperDuper(model.CustomerRegisterPanelModel.L_CARD_NO.ToSafeString(), model.SuperDuperProductName.ToSafeString(), model.SuperDuperServiceName.ToSafeString(), model.PayMentOrderID.ToSafeString(), model.PayMentRecurringChargeVAT.ToSafeString(), "");
            }

            if (PaymentToSuperDuperData != null)
            {
                endPointUrl = PaymentToSuperDuperData.form_url.ToSafeString();
            }

            if (endPointUrl != "")
            {
                return Redirect(endPointUrl);
            }
            else
            {
                if (Session[model.PayMentOrderID] != null)
                {
                    Session[model.PayMentOrderID] = null;
                }

                if (model.SuperDuperProductName.ToSafeString() == "BYOD")
                {
                    var LovData = GetScreenConfig("FBBOR028");

                    string ErrorMsg = LovData.Where(t => t.Name == "L_POPUP_PAYMENT_ERROR")
                                             .Select(t => t.DisplayValue).Count() > 0
                                             ? LovData.Where(t => t.Name == "L_POPUP_PAYMENT_ERROR").Select(t => t.DisplayValue).FirstOrDefault() : "";

                    return RedirectToAction("ErrorUrlEmptyPaymentScpe", "Scpe", new { ErrorMsg = ErrorMsg, FullUrl = fullUrlStr });
                }
                else if (model.SuperDuperProductName.ToSafeString() == "WTTx")
                {
                    // WTTx_ERROR_PAYMENT_MSG
                    string WTTxErrorMSG = "";
                    LovValueModel WTTxErrorMSGtmp = base.LovData.FirstOrDefault(t => t.Name == "WTTx_ERROR_PAYMENT_MSG");
                    if (SiteSession.CurrentUICulture.IsThaiCulture())
                    {
                        WTTxErrorMSG = WTTxErrorMSGtmp != null ? WTTxErrorMSGtmp.LovValue1.ToSafeString() : "";
                    }
                    else
                    {
                        WTTxErrorMSG = WTTxErrorMSGtmp != null ? WTTxErrorMSGtmp.LovValue2.ToSafeString() : "";
                    }
                    TempData["WTTxErrorMSG"] = WTTxErrorMSG;
                    return RedirectToAction("IndexWithModel", new { model = "" });
                }
                else
                {
                    var LovData = GetScreenConfig("FBBOR041");
                    MeshSendSMS(model.CustomerRegisterPanelModel.L_CONTACT_PHONE, "", "", "", "", "2", ipAddress, fullUrlStr, GetCurrentCulture().IsThaiCulture(), "");
                    string ErrorMsg = LovData.Where(t => t.Name == "L_POPUP_PAYMENT_ERROR").Select(t => t.DisplayValue).Count() > 0 ? LovData.Where(t => t.Name == "L_POPUP_PAYMENT_ERROR").Select(t => t.DisplayValue).FirstOrDefault() : "";
                    ActionResult action = TopupMesh("", false, "", "", "", "", "", "", "", "", ErrorMsg, "", "", "", "", "");

                    return action;
                }
            }
        }

        [HttpGet]
        public ActionResult MeshPaymentToSuperDuperSuccessResult(string transactionId = "")
        {
            ActionResult action = null;
            var logStatus = "ERROR";
            var logOutput = "";
            InterfaceLogCommand log = null;
            try
            {
                log = StartInterface(transactionId, "MeshPaymentToSuperDuperSuccessResult", transactionId, "", "TOPUP_MESH");
            }
            catch (Exception ex)
            {
                base.Logger.Info("MeshPaymentToSuperDuperSuccessResult StartInterface Exception : " + ex.GetErrorMessage());
            }

            try
            {
                string fullUrl = this.Url.Action("MeshPaymentToSuperDuperSuccessResult", "Process", null, this.Request.Url.Scheme);

                // Get IP Address
                string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (string.IsNullOrEmpty(ipAddress))
                    ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                var controller = DependencyResolver.Current.GetService<ProcessController>();
                var LovData = controller.GetScreenConfig("FBBOR041");
                string ErrorMsg = LovData.Where(t => t.Name == "L_POPUP_PAYMENT_ERROR").Select(t => t.DisplayValue).Count() > 0 ? LovData.Where(t => t.Name == "L_POPUP_PAYMENT_ERROR").Select(t => t.DisplayValue).FirstOrDefault() : "";
                action = TopupMesh("", false, "", "", "", "", "", "", "", "", ErrorMsg, "", "", "", "", "");

                if (!string.IsNullOrEmpty(transactionId))
                {
                    var logSpdp = GetPaymentLogDataSuperDuper("", "", "", "", "", transactionId);

                    if (logSpdp != null && logSpdp.DataLog != null)
                    {
                        var payMentTranId = logSpdp.DataLog.ORDER_TRANSACTION_ID.ToSafeString();
                        var payMentOrderID = logSpdp.DataLog.ORDER_ID.ToSafeString();
                        var nonMobile = logSpdp.DataLog.NON_MOBILE_NO.ToSafeString();

                        string informationData = "";

                        informationData = LovData.Where(t => t.Name == "L_POPUP_SUCCESS").Select(t => t.DisplayValue).Count() > 0 ? LovData.Where(t => t.Name == "L_POPUP_SUCCESS").Select(t => t.DisplayValue).FirstOrDefault() : "";
                        GetMeshCustomerProfileModel informationDataModel = GetMeshCustomerProfile(nonMobile, payMentOrderID);
                        if (informationDataModel != null && !string.IsNullOrEmpty(informationDataModel.order_list))
                        {
                            informationData = informationData.Replace("[OrderList]", informationDataModel.order_list);
                            informationData = informationData.Replace("[OrderNo]", informationDataModel.amount);
                            informationData = informationData.Replace("[PurchaseAmt]", informationDataModel.purchase_amt);
                            informationData = informationData.Replace("[TranId]", transactionId.ToSafeString());
                            informationData = informationData.Replace("[OrderDate]", informationDataModel.order_date);
                            informationData = informationData.Replace("[CustomerName]", informationDataModel.customer_name);
                            informationData = informationData.Replace("[NonMobileNo]", informationDataModel.non_mobile_no);
                            informationData = informationData.Replace("[ContactMobile]", informationDataModel.contact_mobile);
                            informationData = informationData.Replace("[InstallDate]", informationDataModel.install_date);

                            action = TopupMesh("", true, "", "", "", "", "", "", "", "", informationData, "", "", "", "", "");
                        }
                    }
                }

                logStatus = "Success";
            }
            catch (Exception ex)
            {
                base.Logger.Info("MeshPaymentToSuperDuperSuccessResult Exception : " + ex.GetErrorMessage());
                logStatus = "ERROR";
                logOutput = ex.GetErrorMessage();
                var controllerEx = DependencyResolver.Current.GetService<ProcessController>();
                var LovDataEx = controllerEx.GetScreenConfig("FBBOR041");
                string ErrorMsg = LovDataEx.Where(t => t.Name == "L_POPUP_PAYMENT_ERROR").Select(t => t.DisplayValue).Count() > 0 ? LovDataEx.Where(t => t.Name == "L_POPUP_PAYMENT_ERROR").Select(t => t.DisplayValue).FirstOrDefault() : "";
                action = TopupMesh("", false, "", "", "", "", "", "", "", "", ErrorMsg, "", "", "", "", "");
            }
            finally
            {
                base.Logger.Info("MeshPaymentToSuperDuperSuccessResult End");
                EndInterface(logOutput, log, transactionId, logStatus, "");
            }

            return action;
        }

        [HttpGet]
        public ActionResult MeshPaymentToSuperDuperFailResult(string Data = "")
        {
            ActionResult action = null;
            InterfaceLogCommand log = null;
            var orderTransactionId = "";
            var logStatus = "Failed";
            var logMessage = "";
            try
            {
                orderTransactionId = DecodeOrderTransactionId(Data);
                log = StartInterface(Data, "MeshPaymentToSuperDuperFailResult", orderTransactionId, "", "TOPUP_MESH");
            }
            catch (Exception ex)
            {
                base.Logger.Info("MeshPaymentToSuperDuperFailResult StartInterface Exception : " + ex.GetErrorMessage());
            }
            try
            {
                UpdateCancelPendingOrderSuperDuper(orderTransactionId, "TOPUP_MESH");
                var controller = DependencyResolver.Current.GetService<ProcessController>();
                var LovData = controller.GetScreenConfig("FBBOR041");
                string ErrorMsgNotPaid = LovData.Where(t => t.Name == "L_POPUP_PAYMENT_ERROR").Select(t => t.DisplayValue).Count() > 0 ? LovData.Where(t => t.Name == "L_POPUP_PAYMENT_ERROR").Select(t => t.DisplayValue).FirstOrDefault() : "";
                action = TopupMesh("", false, "", "", "", "", "", "", "", "", ErrorMsgNotPaid, "", "", "", "", "");
                logStatus = "Success";
                logMessage = "";
            }
            catch (Exception ex)
            {
                base.Logger.Info("MeshPaymentToSuperDuperFailResult Exception : " + ex.GetErrorMessage());
                logStatus = "Failed";
                logMessage = ex.GetErrorMessage();
            }
            finally
            {
                EndInterface("", log, Data, logStatus, logMessage);
            }
            return action;
        }

        [HttpGet]
        public ActionResult ScpePaymentToSuperDuperSuccessResult(string transactionId = "")
        {
            InterfaceLogCommand log = null;
            var logStatus = "ERROR";
            var logOutput = "";
            try
            {
                log = StartInterface(transactionId, "ScpePaymentToSuperDuperSuccessResult", transactionId, "", "SELL_ROUTER");
            }
            catch (Exception ex)
            {
                base.Logger.Info("ScpePaymentToSuperDuperSuccessResult StartInterface Exception : " + ex.GetErrorMessage());
            }
            try
            {
                string fullUrl = this.Url.Action("ScpePaymentToSuperDuperSuccessResult", "Process", null, this.Request.Url.Scheme);

                // Get IP Address
                string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

                if (string.IsNullOrEmpty(ipAddress))
                {
                    ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                }

                TempData["SaveStatus"] = "Y";

                logStatus = "Success";
            }
            catch (Exception ex)
            {
                base.Logger.Info("ScpePaymentToSuperDuperSuccessResult Exception : " + ex.GetErrorMessage());
                logStatus = "ERROR";
                logOutput = ex.GetErrorMessage();

                var controller = DependencyResolver.Current.GetService<ProcessController>();
                var LovData = controller.GetScreenConfig("FBBOR028");
                string ErrorMsg = LovData.Where(t => t.Name == "L_POPUP_PAYMENT_ERROR").Select(t => t.DisplayValue).Count() > 0 ? LovData.Where(t => t.Name == "L_POPUP_PAYMENT_ERROR").Select(t => t.DisplayValue).FirstOrDefault() : "";
                return RedirectToAction("Index", "Scpe", new { statusPay = ErrorMsg });
            }
            finally
            {
                base.Logger.Info("ScpePaymentToSuperDuperSuccessResult End");
                EndInterface(logOutput, log, transactionId, logStatus, "");
            }

            return RedirectToAction("Index", "Scpe");
        }

        [HttpGet]
        public ActionResult ScpePaymentToSuperDuperFailResult(string Data = "")
        {
            string ErrorMsg = "";
            var orderTransactionId = "";
            var logStatus = "ERROR";
            var logOutput = "";
            InterfaceLogCommand log = null;
            try
            {
                orderTransactionId = DecodeOrderTransactionId(Data);
                log = StartInterface("Data = " + Data, "ScpePaymentToSuperDuperFailResult", orderTransactionId, "", "SELL_ROUTER");
            }
            catch (Exception ex)
            {
                base.Logger.Info("ScpePaymentToSuperDuperFailResult StartInterface Exception : " + ex.GetErrorMessage());
            }
            try
            {
                UpdateCancelPendingOrderSuperDuper(orderTransactionId, "SELL_ROUTER");

                string fullUrl = this.Url.Action("ScpePaymentToSuperDuperFailResult", "Process", null, this.Request.Url.Scheme);

                // Get IP Address
                string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

                if (string.IsNullOrEmpty(ipAddress))
                {
                    ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                }

                var controller = DependencyResolver.Current.GetService<ProcessController>();
                var LovData = controller.GetScreenConfig("FBBOR028");
                ErrorMsg = LovData.Where(t => t.Name == "L_POPUP_PAYMENT_ERROR").Select(t => t.DisplayValue).Count() > 0 ? LovData.Where(t => t.Name == "L_POPUP_PAYMENT_ERROR").Select(t => t.DisplayValue).FirstOrDefault() : "";

                logStatus = "Success";
            }
            catch (Exception ex)
            {
                base.Logger.Info("ScpePaymentToSuperDuperFailResult Exception : " + ex.GetErrorMessage());
                logStatus = "ERROR";
                logOutput = ex.GetErrorMessage();

                ErrorMsg = "ไม่สามารถชำระค่าบริการได้";
            }
            finally
            {
                TempData["statusPay"] = ErrorMsg;

                base.Logger.Info("ScpePaymentToSuperDuperFailResult End");
                EndInterface(logOutput, log, Data, logStatus, logOutput);
            }

            return RedirectToAction("Index", "Scpe");
        }

        [HttpGet]
        public ActionResult WTTxRabbitPaymentToSuperDuperSuccessResult(string txn_id = "")
        {
            string transactionId = txn_id;
            ActionResult action = RedirectToAction("IndexWithModel", new { model = "" });
            var logStatus = "ERROR";
            var logOutput = "";
            InterfaceLogCommand log = null;
            string WTTxMSG = "";
            string WTTxErrorMSG = "";
            TempData["isWTTx"] = true;
            LovValueModel WTTxMSGtmp = base.LovData.FirstOrDefault(t => t.Name == "WTTx_SUCCESS_MSG_1");
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

            try
            {
                log = StartInterface(transactionId, "WTTxPaymentToSuperDuperSuccessResult", transactionId, "", "WTTx");
            }
            catch (Exception ex)
            {
                base.Logger.Info("WTTxPaymentToSuperDuperSuccessResult StartInterface Exception : " + ex.GetErrorMessage());
            }

            try
            {
                string fullUrl = this.Url.Action("WTTxPaymentToSuperDuperSuccessResult", "Process", null, this.Request.Url.Scheme);

                // Get IP Address
                string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (string.IsNullOrEmpty(ipAddress))
                    ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];




                if (!string.IsNullOrEmpty(transactionId))
                {
                    var logSpdp = GetPaymentLogDataSuperDuper("", "", "", "", "", transactionId);

                    if (logSpdp != null && logSpdp.DataLog != null)
                    {
                        var nonMobile = logSpdp.DataLog.NON_MOBILE_NO.ToSafeString();
                        WTTxMSG = WTTxMSG.Replace("{non_mobile_no}", nonMobile);
                        TempData["WTTxMSG"] = WTTxMSG;
                        TempData["WTTxErrorMSG"] = null;
                    }
                }
                else
                {
                    if (TempData["txndata"] != null)
                    {
                        var logSpdp = GetPaymentLogDataSuperDuper("", "", "", "", "", TempData["txndata"].ToSafeString());

                        if (logSpdp != null && logSpdp.DataLog != null)
                        {
                            var nonMobile = logSpdp.DataLog.NON_MOBILE_NO.ToSafeString();
                            WTTxMSG = WTTxMSG.Replace("{non_mobile_no}", nonMobile);
                            TempData["WTTxMSG"] = WTTxMSG;
                            TempData["WTTxErrorMSG"] = null;
                        }
                        else
                        {
                            if (SiteSession.CurrentUICulture.IsThaiCulture())
                            {
                                WTTxMSG = WTTxMSG.Replace("<br>หมายเลขอินเทอร์เน็ตของท่านคือ {non_mobile_no}", "");
                            }
                            else
                            {
                                WTTxMSG = WTTxMSG.Replace("<br>your internet number is {non_mobile_no}", "");
                            }

                            TempData["WTTxMSG"] = "TempData[txndata] is not null but logSpdp or logSpdp.DataLog is null";
                            TempData["WTTxErrorMSG"] = null;
                        }
                    }
                    else
                    {
                        if (SiteSession.CurrentUICulture.IsThaiCulture())
                        {
                            WTTxMSG = WTTxMSG.Replace("<br>หมายเลขอินเทอร์เน็ตของท่านคือ {non_mobile_no}", "");
                        }
                        else
                        {
                            WTTxMSG = WTTxMSG.Replace("<br>your internet number is {non_mobile_no}", "");
                        }

                        TempData["WTTxMSG"] = WTTxMSG;
                        TempData["WTTxErrorMSG"] = null;
                    }
                }

                logStatus = "Success";
            }
            catch (Exception ex)
            {
                base.Logger.Info("WTTxPaymentToSuperDuperSuccessResult Exception : " + ex.GetErrorMessage());
                logStatus = "ERROR";
                logOutput = ex.GetErrorMessage();
                TempData["WTTxMSG"] = null;
                TempData["WTTxErrorMSG"] = WTTxErrorMSG;
            }
            finally
            {
                base.Logger.Info("WTTxPaymentToSuperDuperSuccessResult End");
                EndInterface(logOutput, log, transactionId, logStatus, "");
            }

            return action;
        }

        [HttpGet]
        public ActionResult WTTxPaymentToSuperDuperSuccessResult(string transactionId = "")
        {
            ActionResult action = RedirectToAction("IndexWithModel", new { model = "" });
            var logStatus = "ERROR";
            var logOutput = "";
            InterfaceLogCommand log = null;
            string WTTxMSG = "";
            string WTTxErrorMSG = "";
            TempData["isWTTx"] = true;
            LovValueModel WTTxMSGtmp = base.LovData.FirstOrDefault(t => t.Name == "WTTx_SUCCESS_MSG_1");
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

            try
            {
                log = StartInterface(transactionId, "WTTxPaymentToSuperDuperSuccessResult", transactionId, "", "WTTx");
            }
            catch (Exception ex)
            {
                base.Logger.Info("WTTxPaymentToSuperDuperSuccessResult StartInterface Exception : " + ex.GetErrorMessage());
            }

            try
            {
                string fullUrl = this.Url.Action("WTTxPaymentToSuperDuperSuccessResult", "Process", null, this.Request.Url.Scheme);

                // Get IP Address
                string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (string.IsNullOrEmpty(ipAddress))
                    ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];




                if (!string.IsNullOrEmpty(transactionId))
                {
                    var logSpdp = GetPaymentLogDataSuperDuper("", "", "", "", "", transactionId);

                    if (logSpdp != null && logSpdp.DataLog != null)
                    {
                        var nonMobile = logSpdp.DataLog.NON_MOBILE_NO.ToSafeString();
                        WTTxMSG = WTTxMSG.Replace("{non_mobile_no}", nonMobile);
                        TempData["WTTxMSG"] = WTTxMSG;
                        TempData["WTTxErrorMSG"] = null;
                    }
                }
                else
                {
                    if (TempData["txndata"] != null)
                    {
                        var logSpdp = GetPaymentLogDataSuperDuper("", "", "", "", "", TempData["txndata"].ToSafeString());

                        if (logSpdp != null && logSpdp.DataLog != null)
                        {
                            var nonMobile = logSpdp.DataLog.NON_MOBILE_NO.ToSafeString();
                            WTTxMSG = WTTxMSG.Replace("{non_mobile_no}", nonMobile);
                            TempData["WTTxMSG"] = WTTxMSG;
                            TempData["WTTxErrorMSG"] = null;
                        }
                        else
                        {
                            if (SiteSession.CurrentUICulture.IsThaiCulture())
                            {
                                WTTxMSG = WTTxMSG.Replace("<br>หมายเลขอินเทอร์เน็ตของท่านคือ {non_mobile_no}", "");
                            }
                            else
                            {
                                WTTxMSG = WTTxMSG.Replace("<br>your internet number is {non_mobile_no}", "");
                            }

                            TempData["WTTxMSG"] = "TempData[txndata] is not null but logSpdp or logSpdp.DataLog is null";
                            TempData["WTTxErrorMSG"] = null;
                        }
                    }
                    else
                    {
                        if (SiteSession.CurrentUICulture.IsThaiCulture())
                        {
                            WTTxMSG = WTTxMSG.Replace("<br>หมายเลขอินเทอร์เน็ตของท่านคือ {non_mobile_no}", "");
                        }
                        else
                        {
                            WTTxMSG = WTTxMSG.Replace("<br>your internet number is {non_mobile_no}", "");
                        }

                        TempData["WTTxMSG"] = WTTxMSG;
                        TempData["WTTxErrorMSG"] = null;
                    }
                }

                logStatus = "Success";
            }
            catch (Exception ex)
            {
                base.Logger.Info("WTTxPaymentToSuperDuperSuccessResult Exception : " + ex.GetErrorMessage());
                logStatus = "ERROR";
                logOutput = ex.GetErrorMessage();
                TempData["WTTxMSG"] = null;
                TempData["WTTxErrorMSG"] = WTTxErrorMSG;
            }
            finally
            {
                base.Logger.Info("WTTxPaymentToSuperDuperSuccessResult End");
                EndInterface(logOutput, log, transactionId, logStatus, "");
            }

            return action;
        }

        [HttpGet]
        public ActionResult WTTxPaymentToSuperDuperFailResult(string Data = "")
        {
            ActionResult action = null;
            InterfaceLogCommand log = null;
            var orderTransactionId = "";
            var logStatus = "Failed";
            var logMessage = "";
            string WTTxErrorMSG = "";

            TempData["isWTTx"] = true;

            LovValueModel WTTxErrorMSGtmp = base.LovData.FirstOrDefault(t => t.Name == "WTTx_ERROR_MSG");
            if (SiteSession.CurrentUICulture.IsThaiCulture())
            {
                WTTxErrorMSG = WTTxErrorMSGtmp != null ? WTTxErrorMSGtmp.LovValue1.ToSafeString() : "";
            }
            else
            {
                WTTxErrorMSG = WTTxErrorMSGtmp != null ? WTTxErrorMSGtmp.LovValue2.ToSafeString() : "";
            }

            TempData["WTTxErrorMSG"] = WTTxErrorMSG;
            action = RedirectToAction("IndexWithModel", new { model = "" });

            try
            {
                orderTransactionId = DecodeOrderTransactionId(Data);
                log = StartInterface(Data, "WTTxPaymentToSuperDuperFailResult", orderTransactionId, "", "WTTx");
            }
            catch (Exception ex)
            {
                base.Logger.Info("WTTxPaymentToSuperDuperFailResult StartInterface Exception : " + ex.GetErrorMessage());
            }
            try
            {
                UpdateCancelPendingOrderSuperDuper(orderTransactionId, "WTTx");
                logStatus = "Success";
                logMessage = "";
            }
            catch (Exception ex)
            {
                base.Logger.Info("WTTxPaymentToSuperDuperFailResult Exception : " + ex.GetErrorMessage());
                logStatus = "Failed";
                logMessage = ex.GetErrorMessage();

            }
            finally
            {
                EndInterface("", log, Data, logStatus, logMessage);
            }
            return action;
        }

        [HttpPost]
        public JsonResult PaymentQRCodeToSuperDuper(QuickWinPanelModel model)
        {
            GetPaymentQRToSuperDuperModel result = new GetPaymentQRToSuperDuperModel();
            try
            {
                if (!string.IsNullOrEmpty(model.CoveragePanelModel.P_MOBILE))
                {
                    result = GetPaymentQRToSuperDuper(model.CoveragePanelModel.P_MOBILE.ToSafeString(), model.SuperDuperProductName.ToSafeString(), model.SuperDuperServiceName.ToSafeString(), model.PayMentOrderID.ToSafeString(), model.PayMentRecurringChargeVAT.ToSafeString(), model.outBillingAccountNumber);
                }
                else
                {
                    result = GetPaymentQRToSuperDuper(model.CustomerRegisterPanelModel.L_CARD_NO.ToSafeString(), model.SuperDuperProductName.ToSafeString(), model.SuperDuperServiceName.ToSafeString(), model.PayMentOrderID.ToSafeString(), model.PayMentRecurringChargeVAT.ToSafeString(), "");
                }

            }
            catch (Exception ex)
            {
                result = new GetPaymentQRToSuperDuperModel
                {
                    //status_code = "-1",
                    //status_message = ex.Message
                    status = "-1",
                    message = ex.Message
                };
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult PaymentRabbitToSuperDuper(QuickWinPanelModel model)
        {
            string fullUrlStr = this.Url.Action("PaymentRabbitToSuperDuper", "Process", null, this.Request.Url.Scheme);

            #region Get IP Address Interface Log : Edit 2017-01-30

            string transactionId = "";

            // Get IP Address
            string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress)) ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            transactionId = ipAddress + "|";

            #endregion

            var resData = new ResponseDataModel();
            var PaymentRabbitToSuperDuperData = new GetPaymentRabbitToSuperDuperModel();

            if (!string.IsNullOrEmpty(model.CoveragePanelModel.P_MOBILE))
            {
                PaymentRabbitToSuperDuperData = GetPaymentRabbitToSuperDuper(model.CoveragePanelModel.P_MOBILE.ToSafeString(), model.SuperDuperProductName.ToSafeString(), model.SuperDuperServiceName.ToSafeString(), model.PayMentOrderID.ToSafeString(), model.PayMentRecurringChargeVAT.ToSafeString(), model.outBillingAccountNumber);
            }
            else
            {
                PaymentRabbitToSuperDuperData = GetPaymentRabbitToSuperDuper(model.CustomerRegisterPanelModel.L_CARD_NO.ToSafeString(), model.SuperDuperProductName.ToSafeString(), model.SuperDuperServiceName.ToSafeString(), model.PayMentOrderID.ToSafeString(), model.PayMentRecurringChargeVAT.ToSafeString(), "");
            }

            if (PaymentRabbitToSuperDuperData.data != null)
            {
                TempData["txndata"] = PaymentRabbitToSuperDuperData.data.transaction_id.ToSafeString();
                resData.web_redirect = PaymentRabbitToSuperDuperData.data.web_redirect.ToSafeString();
            }

            if (!string.IsNullOrEmpty(resData.web_redirect))
            {
                return Redirect(resData.web_redirect);
            }
            else
            {
                if (Session[model.PayMentOrderID] != null)
                {
                    Session[model.PayMentOrderID] = null;
                }

                if (model.SuperDuperProductName.ToSafeString() == "BYOD")
                {
                    var LovData = GetScreenConfig("FBBOR028");

                    string ErrorMsg = LovData.Where(t => t.Name == "L_POPUP_PAYMENT_ERROR")
                                             .Select(t => t.DisplayValue).Count() > 0
                                             ? LovData.Where(t => t.Name == "L_POPUP_PAYMENT_ERROR").Select(t => t.DisplayValue).FirstOrDefault() : "";

                    return RedirectToAction("ErrorUrlEmptyPaymentScpe", "Scpe", new { ErrorMsg = ErrorMsg, FullUrl = fullUrlStr });
                }
                else if (model.SuperDuperProductName.ToSafeString() == "WTTx")
                {
                    // WTTx_ERROR_PAYMENT_MSG
                    string WTTxErrorMSG = "";
                    LovValueModel WTTxErrorMSGtmp = base.LovData.FirstOrDefault(t => t.Name == "WTTx_ERROR_PAYMENT_MSG");
                    if (SiteSession.CurrentUICulture.IsThaiCulture())
                    {
                        WTTxErrorMSG = WTTxErrorMSGtmp != null ? WTTxErrorMSGtmp.LovValue1.ToSafeString() : "";
                    }
                    else
                    {
                        WTTxErrorMSG = WTTxErrorMSGtmp != null ? WTTxErrorMSGtmp.LovValue2.ToSafeString() : "";
                    }

                    TempData["isWTTx"] = true;
                    TempData["WTTxMSG"] = null;
                    TempData["WTTxErrorMSG"] = WTTxErrorMSG;
                    return RedirectToAction("IndexWithModel", new { model = "" });
                }
                else
                {
                    var LovData = GetScreenConfig("FBBOR041");
                    MeshSendSMS(model.CustomerRegisterPanelModel.L_CONTACT_PHONE, "", "", "", "", "2", ipAddress, fullUrlStr, GetCurrentCulture().IsThaiCulture(), "");
                    string ErrorMsg = LovData.Where(t => t.Name == "L_POPUP_PAYMENT_ERROR").Select(t => t.DisplayValue).Count() > 0 ? LovData.Where(t => t.Name == "L_POPUP_PAYMENT_ERROR").Select(t => t.DisplayValue).FirstOrDefault() : "";
                    ActionResult action = TopupMesh("", false, "", "", "", "", "", "", "", "", ErrorMsg, "", "", "", "", "");

                    return action;
                }
            }
        }

        public GetPaymentToSuperDuperModel GetPaymentToSuperDuper(string NonMobileNO, string ProductName, string ServiceName, string TransactionID, string PayMentRecurringChargeVAT, string BillingAccount)
        {
            string fullUrl = this.Url.Action("GetPaymentToSuperDuper", "Process", null, this.Request.Url.Scheme);

            GetPaymentToSuperDuperModel PaymentToSuperDuperData = new GetPaymentToSuperDuperModel();

            GetConfigReqPaymentQuery getConfigReqPaymentQuery = new GetConfigReqPaymentQuery()
            {
                p_product_name = ProductName,
                p_service_name = ServiceName,
                p_transaction_id = TransactionID,
                p_non_mobile_no = NonMobileNO
            };

            GetConfigReqPaymentModel getConfigReqPaymentData = _queryProcessor.Execute(getConfigReqPaymentQuery);

            if (getConfigReqPaymentData.ret_code == "0" && getConfigReqPaymentData.list_config_req_payment != null && getConfigReqPaymentData.list_config_req_payment.Count > 0)
            {
                string Nonce = Guid.NewGuid().ToString();

                string endpoint = "";
                string channel_secret = "";
                string Content_Type = "";
                string X_sdpg_merchant_id = "";
                string order_id = "";
                string product_name = "";
                string service_id = "";
                string channel_type = "";
                string amount = "";
                string currency = "";
                string form_type = "";
                string is_remember = "";
                string ref_1 = "";
                string ref_2 = "";
                string payment_method_id = "";
                string cust_id = NonMobileNO;

                bool tmp_is_remember = false;

                List<ConfigReqPaymentData> configReqPaymentDatas = getConfigReqPaymentData.list_config_req_payment;
                // set config
                foreach (var item in configReqPaymentDatas)
                {

                    if (item.attr_name == "endpoint")
                    {
                        endpoint = item.attr_value.ToSafeString();
                    }
                    else if (item.attr_name == "channel_secret")
                    {
                        channel_secret = item.attr_value.ToSafeString();
                    }
                    else if (item.attr_name == "Content-Type")
                    {
                        Content_Type = item.attr_value.ToSafeString();
                    }
                    else if (item.attr_name == "X-sdpg-merchant-id")
                    {
                        X_sdpg_merchant_id = item.attr_value.ToSafeString();
                    }
                    else if (item.attr_name == "order_id")
                    {
                        order_id = item.attr_value.ToSafeString();
                    }
                    else if (item.attr_name == "product_name")
                    {
                        product_name = item.attr_value.ToSafeString();
                    }
                    else if (item.attr_name == "service_id")
                    {
                        service_id = item.attr_value.ToSafeString();
                    }
                    else if (item.attr_name == "channel_type")
                    {
                        channel_type = item.attr_value.ToSafeString();
                    }
                    else if (item.attr_name == "amount")
                    {
                        amount = ProductName == "BYOD" ? PayMentRecurringChargeVAT.ToSafeString() : item.attr_value.ToSafeString();
                    }
                    else if (item.attr_name == "currency")
                    {
                        currency = item.attr_value.ToSafeString();
                    }
                    else if (item.attr_name == "form_type")
                    {
                        form_type = item.attr_value.ToSafeString();
                    }
                    else if (item.attr_name == "is_remember")
                    {
                        is_remember = item.attr_value.ToSafeString();
                    }
                    else if (item.attr_name == "ref_1")
                    {
                        ref_1 = item.attr_value.ToSafeString();
                    }
                    else if (item.attr_name == "ref_2")
                    {
                        ref_2 = item.attr_value.ToSafeString();
                    }
                    else if (item.attr_name == "payment_method_id")
                    {
                        payment_method_id = item.attr_value.ToSafeString();
                    }

                }

                if (is_remember == "true")
                {
                    tmp_is_remember = true;
                }

                string bank_code = "";
                string company_account_no = "";
                string company_account_name = "";
                string service_id_Metadat = "";
                string transaction_code = "";
                string billing_system = "";
                string merchant_type = "";
                string billing_account = "";
                string master_mobile_no = "";
                string batch_no = "";


                List<ConfigReqPaymentData> ReqPaymentMetadat = getConfigReqPaymentData.list_req_payment_metadata;
                // set config
                foreach (var item in ReqPaymentMetadat)
                {
                    if (item.attr_name == "bank_code")
                    {
                        bank_code = item.attr_value.ToSafeString();
                    }
                    else if (item.attr_name == "company_account_no")
                    {
                        company_account_no = item.attr_value.ToSafeString();
                    }
                    else if (item.attr_name == "company_account_name")
                    {
                        company_account_name = item.attr_value.ToSafeString();
                    }
                    else if (item.attr_name == "service_id")
                    {
                        service_id_Metadat = item.attr_value.ToSafeString();
                    }
                    else if (item.attr_name == "transaction_code")
                    {
                        transaction_code = item.attr_value.ToSafeString();
                    }
                    else if (item.attr_name == "billing_system")
                    {
                        billing_system = item.attr_value.ToSafeString();
                    }
                    else if (item.attr_name == "merchant_type")
                    {
                        merchant_type = item.attr_value.ToSafeString();
                    }
                    else if (item.attr_name == "billing_account")
                    {
                        billing_account = item.attr_value.ToSafeString();
                    }
                    else if (item.attr_name == "master_mobile_no")
                    {
                        master_mobile_no = item.attr_value.ToSafeString();
                    }
                    else if (item.attr_name == "batch_no")
                    {
                        batch_no = item.attr_value.ToSafeString();
                    }
                }

                bool TMP_Eds_required = false;
                string Eds_required = "";
                string Eds_url_success = "";
                string Eds_url_fail = "";

                List<ConfigReqPaymentData> ReqPayment3ds = getConfigReqPaymentData.list_req_payment_3ds;
                // set config
                foreach (var item in ReqPayment3ds)
                {
                    if (item.attr_name == "3ds_required")
                    {
                        Eds_required = item.attr_value.ToSafeString();
                    }
                    else if (item.attr_name == "3ds_url_success")
                    {
                        Eds_url_success = item.attr_value.ToSafeString();
                    }
                    else if (item.attr_name == "3ds_url_fail")
                    {
                        Eds_url_fail = item.attr_value.ToSafeString();
                    }
                }
                if (Eds_required == "true")
                {
                    TMP_Eds_required = true;
                }

                CustomerMobileNo[] customer_mobile_no = new CustomerMobileNo[1];
                customer_mobile_no[0] = new CustomerMobileNo()
                {
                    billing_account = BillingAccount,
                    mobile_no = NonMobileNO,
                    amount = amount
                };

                MetaData metaData = new MetaData
                {
                    bank_code = "",
                    company_account_no = company_account_no,
                    company_account_name = company_account_name,
                    service_id = service_id_Metadat,
                    transaction_code = transaction_code,
                    billing_system = billing_system,
                    merchant_type = merchant_type,
                    master_mobile_no = NonMobileNO,
                    customer_mobile_no = customer_mobile_no,
                    batch_no = batch_no
                };

                EdsData edsData = new EdsData
                {
                    Eds_required = TMP_Eds_required,
                    Eds_url_success = Eds_url_success,
                    Eds_url_fail = Eds_url_fail
                };

                PaymentToSuperDuperBody body = new PaymentToSuperDuperBody
                {
                    order_id = order_id,
                    product_name = product_name,
                    service_id = service_id,
                    channel_type = channel_type,
                    cust_id = cust_id,
                    amount = amount,
                    currency = currency,
                    ref_1 = BillingAccount,
                    ref_2 = ProductName == "BYOD" ? ref_2 : NonMobileNO,
                    ref_3 = "",
                    ref_4 = "",
                    ref_5 = "",
                    form_type = form_type,
                    is_remember = tmp_is_remember,
                    metadata = metaData,
                    Eds = edsData
                };
                GetPaymentToSuperDuperQuery getPaymentInquiryToSuperDuperQuery = new GetPaymentToSuperDuperQuery
                {
                    Url = endpoint,
                    ProductName = product_name,
                    ServiceName = ServiceName,
                    p_mobile_no = NonMobileNO,
                    Secret = channel_secret,
                    payment_method_id = payment_method_id,

                    ContentType = Content_Type,
                    MerchantID = X_sdpg_merchant_id,
                    Signature = "",
                    Nonce = Nonce,
                    Body = body,
                    FullUrl = fullUrl
                };

                PaymentToSuperDuperData = _queryProcessor.Execute(getPaymentInquiryToSuperDuperQuery);
            }
            else
            {
                PaymentToSuperDuperData.status_code = "-1";
                PaymentToSuperDuperData.status_message = "getConfigReqPayment No Data.";
            }
            return PaymentToSuperDuperData;
        }

        public GetPaymentQRToSuperDuperModel GetPaymentQRToSuperDuper(string NonMobileNO, string ProductName, string ServiceName, string TransactionID, string PayMentRecurringChargeVAT, string BillingAccount)
        {
            string fullUrl = this.Url.Action("GetPaymentQRToSuperDuper", "Process", null, this.Request.Url.Scheme);

            var PaymentQRToSuperDuperData = new GetPaymentQRToSuperDuperModel();

            GetConfigReqPaymentQuery getConfigReqPaymentQuery = new GetConfigReqPaymentQuery()
            {
                p_product_name = ProductName,
                p_service_name = ServiceName,
                p_transaction_id = TransactionID,
                p_non_mobile_no = NonMobileNO
            };

            GetConfigReqPaymentModel getConfigReqPaymentData = _queryProcessor.Execute(getConfigReqPaymentQuery);

            if (getConfigReqPaymentData.ret_code == "0" && getConfigReqPaymentData.list_config_req_payment != null && getConfigReqPaymentData.list_config_req_payment.Count > 0)
            {
                string Nonce = Guid.NewGuid().ToString();
                string endpoint = "";
                string channel_secret = "";
                string Content_Type = "";
                string X_sdpg_merchant_id = "";
                string order_id = "";
                string product_name = "";
                string sof = "";
                string service_id = "";
                string terminal_id = "";
                string location_name = "";
                string amount = "";
                string currency = "";
                string expire_time_seconds = "";
                string payment_method_id = "";

                List<ConfigReqPaymentData> configReqPaymentDatas = getConfigReqPaymentData.list_config_req_payment;
                // set config
                foreach (var item in configReqPaymentDatas)
                {
                    if (item.attr_name == "endpoint")
                    {
                        endpoint = item.attr_value.ToSafeString();
                    }
                    else if (item.attr_name == "channel_secret")
                    {
                        channel_secret = item.attr_value.ToSafeString();
                    }
                    else if (item.attr_name == "Content-Type")
                    {
                        Content_Type = item.attr_value.ToSafeString();
                    }
                    else if (item.attr_name == "X-sdpg-merchant-id")
                    {
                        X_sdpg_merchant_id = item.attr_value.ToSafeString();
                    }
                    else if (item.attr_name == "order_id")
                    {
                        order_id = item.attr_value.ToSafeString();
                    }
                    else if (item.attr_name == "product_name")
                    {
                        product_name = item.attr_value.ToSafeString();
                    }
                    else if (item.attr_name == "sof")
                    {
                        sof = item.attr_value.ToSafeString();
                    }
                    else if (item.attr_name == "service_id")
                    {
                        service_id = item.attr_value.ToSafeString();
                    }
                    else if (item.attr_name == "terminal_id")
                    {
                        terminal_id = item.attr_value.ToSafeString();
                    }
                    else if (item.attr_name == "location_name")
                    {
                        location_name = item.attr_value.ToSafeString();
                    }
                    else if (item.attr_name == "amount")
                    {
                        amount = ProductName == "BYOD" ? PayMentRecurringChargeVAT.ToSafeString() : item.attr_value.ToSafeString();
                    }
                    else if (item.attr_name == "currency")
                    {
                        currency = item.attr_value.ToSafeString();
                    }
                    else if (item.attr_name == "expire_time_seconds")
                    {
                        expire_time_seconds = item.attr_value.ToSafeString();
                    }
                    else if (item.attr_name == "payment_method_id")
                    {
                        payment_method_id = item.attr_value.ToSafeString();
                    }
                }

                string bank_code = "";
                string company_account_no = "";
                string company_account_name = "";
                string service_id_Metadat = "";
                string transaction_code = "";
                string billing_system = "";
                string merchant_type = "";
                string billing_account = "";
                string master_mobile_no = "";
                string batch_no = "";


                List<ConfigReqPaymentData> ReqPaymentMetadat = getConfigReqPaymentData.list_req_payment_metadata;
                // set config
                foreach (var item in ReqPaymentMetadat)
                {
                    if (item.attr_name == "bank_code")
                    {
                        bank_code = item.attr_value.ToSafeString();
                    }
                    else if (item.attr_name == "company_account_no")
                    {
                        company_account_no = item.attr_value.ToSafeString();
                    }
                    else if (item.attr_name == "company_account_name")
                    {
                        company_account_name = item.attr_value.ToSafeString();
                    }
                    else if (item.attr_name == "service_id")
                    {
                        service_id_Metadat = item.attr_value.ToSafeString();
                    }
                    else if (item.attr_name == "transaction_code")
                    {
                        transaction_code = item.attr_value.ToSafeString();
                    }
                    else if (item.attr_name == "billing_system")
                    {
                        billing_system = item.attr_value.ToSafeString();
                    }
                    else if (item.attr_name == "merchant_type")
                    {
                        merchant_type = item.attr_value.ToSafeString();
                    }
                    else if (item.attr_name == "billing_account")
                    {
                        billing_account = item.attr_value.ToSafeString();
                    }
                    else if (item.attr_name == "master_mobile_no")
                    {
                        master_mobile_no = item.attr_value.ToSafeString();
                    }
                    else if (item.attr_name == "batch_no")
                    {
                        batch_no = item.attr_value.ToSafeString();
                    }
                }

                CustomerMobileNo[] customer_mobile_no = new CustomerMobileNo[1];
                customer_mobile_no[0] = new CustomerMobileNo()
                {
                    billing_account = BillingAccount,
                    mobile_no = NonMobileNO,
                    amount = amount
                };

                MetaData metaData = new MetaData
                {
                    bank_code = "",
                    company_account_no = company_account_no,
                    company_account_name = company_account_name,
                    service_id = service_id_Metadat,
                    transaction_code = transaction_code,
                    billing_system = billing_system,
                    merchant_type = merchant_type,
                    master_mobile_no = master_mobile_no,
                    customer_mobile_no = customer_mobile_no,
                    batch_no = batch_no
                };

                PaymentQRToSuperDuperBody body = new PaymentQRToSuperDuperBody
                {
                    order_id = order_id,
                    product_name = product_name,
                    sof = sof,
                    service_id = service_id,
                    terminal_id = terminal_id,
                    location_name = location_name,
                    amount = amount,
                    currency = currency,
                    expire_time_seconds = expire_time_seconds,
                    ref_1 = BillingAccount,
                    ref_2 = ProductName == "BYOD" ? "" : NonMobileNO,
                    ref_3 = "",
                    ref_4 = "",
                    ref_5 = "",
                    metadata = metaData
                };



                GetPaymentQRToSuperDuperQuery getPaymentQRToSuperDuperQuery = new GetPaymentQRToSuperDuperQuery()
                {
                    Url = endpoint,
                    ProductName = product_name,
                    ServiceName = ServiceName,
                    p_mobile_no = NonMobileNO,
                    Secret = channel_secret,
                    payment_method_id = payment_method_id,

                    ContentType = Content_Type,
                    MerchantID = X_sdpg_merchant_id,
                    Signature = "",
                    Nonce = Nonce,

                    //20.8 Payment - mPay Super Duper
                    Body = body,
                    FullUrl = fullUrl
                };

                PaymentQRToSuperDuperData = _queryProcessor.Execute(getPaymentQRToSuperDuperQuery);

                string RemarkQR = "";

                if (ProductName == "BYOD")
                {
                    var LovData028 = GetScreenConfig("FBBOR028");
                    RemarkQR = LovData028.Where(x => x.Name == "L_MPAY_QR_CODE_FOOTER_REMARK").Any() ? LovData028.FirstOrDefault(x => x.Name == "L_MPAY_QR_CODE_FOOTER_REMARK").DisplayValue : "";
                }
                else if (ProductName == "MESH")
                {
                    var LovData041 = GetScreenConfig("FBBOR041");
                    RemarkQR = LovData041.Where(x => x.Name == "L_PAY_WITH_QR_CODE_2").Any() ? LovData041.FirstOrDefault(x => x.Name == "L_PAY_WITH_QR_CODE_2").DisplayValue : "";
                }


                if (!string.IsNullOrEmpty(RemarkQR))
                {
                    var expireTime = expire_time_seconds.ToInt();
                    var expireTimeStr = "";

                    expireTime = expireTime / 60 >= 1 ? expireTime / 60 : 1;
                    expireTimeStr = expireTime.ToString();

                    RemarkQR = RemarkQR.Replace("{0}", expireTimeStr);
                }

                PaymentQRToSuperDuperData.message_remark = RemarkQR;
            }
            else
            {
                //PaymentQRToSuperDuperData.status_code = "-1";
                //PaymentQRToSuperDuperData.status_message = "getConfigReqPayment No Data.";
                PaymentQRToSuperDuperData.status = "-1";
                PaymentQRToSuperDuperData.message = "getConfigReqPayment No Data.";
            }

            return PaymentQRToSuperDuperData;
        }

        public GetPaymentRabbitToSuperDuperModel GetPaymentRabbitToSuperDuper(string NonMobileNO, string ProductName, string ServiceName, string TransactionID, string PayMentRecurringChargeVAT, string BillingAccount)
        {
            string fullUrl = this.Url.Action("GetPaymentRabbitToSuperDuper", "Process", null, this.Request.Url.Scheme);

            GetPaymentRabbitToSuperDuperModel PaymentRabbitToSuperDuperData = new GetPaymentRabbitToSuperDuperModel();

            GetConfigReqPaymentQuery getConfigReqPaymentQuery = new GetConfigReqPaymentQuery()
            {
                p_product_name = ProductName,
                p_service_name = ServiceName,
                p_transaction_id = TransactionID,
                p_non_mobile_no = NonMobileNO
            };

            GetConfigReqPaymentModel getConfigReqPaymentData = _queryProcessor.Execute(getConfigReqPaymentQuery);

            if (getConfigReqPaymentData.ret_code == "0" && getConfigReqPaymentData.list_config_req_payment != null && getConfigReqPaymentData.list_config_req_payment.Count > 0)
            {
                #region config
                string Nonce = Guid.NewGuid().ToString();

                string endpoint = "";
                string channel_secret = "";
                string Content_Type = "";
                string X_sdpg_merchant_id = "";
                string order_id = "";
                string product_name = "";
                string service_id = "";
                string channel_type = "";
                string amount = "";
                string currency = "";
                string form_type = "";
                string is_pre_approve = "";
                string ref_1 = "";
                string ref_2 = "";
                string payment_method_id = "";
                string cust_id = NonMobileNO;
                //R22.04 WTTx------------------
                string packages_id = "";
                string products_id = "";
                string products_name = "";
                string image_url = "";
                string quantity = "";
                string price = "";
                //-----------------------------

                bool tmp_is_pre_approve = false;

                List<ConfigReqPaymentData> configReqPaymentDatas = getConfigReqPaymentData.list_config_req_payment;
                // set config
                foreach (var item in configReqPaymentDatas)
                {

                    if (item.attr_name == "endpoint")
                    {
                        endpoint = item.attr_value.ToSafeString();
                    }
                    else if (item.attr_name == "channel_secret")
                    {
                        channel_secret = item.attr_value.ToSafeString();
                    }
                    else if (item.attr_name == "Content-Type")
                    {
                        Content_Type = item.attr_value.ToSafeString();
                    }
                    else if (item.attr_name == "X-sdpg-merchant-id")
                    {
                        X_sdpg_merchant_id = item.attr_value.ToSafeString();
                    }
                    else if (item.attr_name == "order_id")
                    {
                        order_id = item.attr_value.ToSafeString();
                    }
                    else if (item.attr_name == "product_name")
                    {
                        product_name = item.attr_value.ToSafeString();
                    }
                    else if (item.attr_name == "service_id")
                    {
                        service_id = item.attr_value.ToSafeString();
                    }
                    else if (item.attr_name == "channel_type")
                    {
                        channel_type = item.attr_value.ToSafeString();
                    }
                    else if (item.attr_name == "amount")
                    {
                        amount = item.attr_value.ToSafeString();
                    }
                    else if (item.attr_name == "currency")
                    {
                        currency = item.attr_value.ToSafeString();
                    }
                    else if (item.attr_name == "ref_1")
                    {
                        ref_1 = item.attr_value.ToSafeString();
                    }
                    else if (item.attr_name == "ref_2")
                    {
                        ref_2 = item.attr_value.ToSafeString();
                    }
                    else if (item.attr_name == "payment_method_id")
                    {
                        payment_method_id = item.attr_value.ToSafeString();
                    }
                    else if (item.attr_name == "form_type")
                    {
                        form_type = item.attr_value.ToSafeString();
                    }
                    else if (item.attr_name == "is_pre_approve")
                    {
                        is_pre_approve = item.attr_value.ToSafeString();
                    }
                    else if (item.attr_name == "packages_id")
                    {
                        packages_id = item.attr_value.ToSafeString();
                    }
                    else if (item.attr_name == "products_id")
                    {
                        products_id = item.attr_value.ToSafeString();
                    }
                    else if (item.attr_name == "products_name")
                    {
                        products_name = item.attr_value.ToSafeString();
                    }
                    else if (item.attr_name == "image_url")
                    {
                        image_url = item.attr_value.ToSafeString();
                    }
                    else if (item.attr_name == "quantity")
                    {
                        quantity = item.attr_value.ToSafeString();
                    }
                    else if (item.attr_name == "price")
                    {
                        price = item.attr_value.ToSafeString();
                    }
                }

                if (is_pre_approve == "true")
                {
                    tmp_is_pre_approve = true;
                }
                #endregion
                #region metadata 
                //R22.04 WTTx
                string bank_code = "";
                string company_account_no = "";
                string company_account_name = "";
                string service_id_Metadat = "";
                string transaction_code = "";
                string billing_system = "";
                string merchant_type = "";
                string billing_account = "";
                string master_mobile_no = "";
                string batch_no = "";

                List<ConfigReqPaymentData> ReqPaymentMetadat = getConfigReqPaymentData.list_req_payment_metadata;
                // set config
                foreach (var item in ReqPaymentMetadat)
                {
                    if (item.attr_name == "bank_code")
                    {
                        bank_code = item.attr_value.ToSafeString();
                    }
                    else if (item.attr_name == "company_account_no")
                    {
                        company_account_no = item.attr_value.ToSafeString();
                    }
                    else if (item.attr_name == "company_account_name")
                    {
                        company_account_name = item.attr_value.ToSafeString();
                    }
                    else if (item.attr_name == "service_id")
                    {
                        service_id_Metadat = item.attr_value.ToSafeString();
                    }
                    else if (item.attr_name == "transaction_code")
                    {
                        transaction_code = item.attr_value.ToSafeString();
                    }
                    else if (item.attr_name == "billing_system")
                    {
                        billing_system = item.attr_value.ToSafeString();
                    }
                    else if (item.attr_name == "merchant_type")
                    {
                        merchant_type = item.attr_value.ToSafeString();
                    }
                    else if (item.attr_name == "billing_account")
                    {
                        billing_account = item.attr_value.ToSafeString();
                    }
                    else if (item.attr_name == "master_mobile_no")
                    {
                        master_mobile_no = item.attr_value.ToSafeString();
                    }
                    else if (item.attr_name == "batch_no")
                    {
                        batch_no = item.attr_value.ToSafeString();
                    }
                }

                CustomerMobileNoRabbit[] customer_mobile_no = new CustomerMobileNoRabbit[1];
                customer_mobile_no[0] = new CustomerMobileNoRabbit()
                {
                    billing_account = BillingAccount,
                    mobile_no = NonMobileNO,
                    amount = amount
                };

                MetaDataRabbit metadata = new MetaDataRabbit
                {
                    bank_code = bank_code,
                    company_account_no = company_account_no,
                    company_account_name = company_account_name,
                    service_id = service_id_Metadat,
                    transaction_code = transaction_code,
                    billing_system = billing_system,
                    merchant_type = merchant_type,
                    master_mobile_no = master_mobile_no,
                    customer_mobile_no = customer_mobile_no,
                    batch_no = batch_no
                };

                #endregion
                #region packages
                //R22.04 WTTx
                var packages = new List<Packages>() {
                    new Packages {
                        id = packages_id,
                        amount = (string.IsNullOrEmpty(amount) ? 0 : Convert.ToDecimal(amount)),
                        products = new Products[] {
                            new Products {
                                id = products_id,
                                name = products_name,
                                image_url = image_url,
                                quantity = (string.IsNullOrEmpty(quantity) ? 0 : Convert.ToInt32(quantity)),
                                price = (string.IsNullOrEmpty(price) ? 0 : Convert.ToDecimal(price))
                            }
                        }
                    }
                };

                #endregion
                #region redirect_urls
                var redirect_urls = new RedirectUrls();
                List<ConfigReqPaymentData> ReqPayment3ds = getConfigReqPaymentData.list_req_payment_3ds;
                // set config
                foreach (var item in ReqPayment3ds)
                {
                    if (item.attr_name == "confirm_url")
                    {
                        redirect_urls.confirm_url = item.attr_value.ToSafeString();
                    }
                    else if (item.attr_name == "cancel_url")
                    {
                        redirect_urls.cancel_url = item.attr_value.ToSafeString();
                    }
                }
                #endregion

                PaymentRabbitToSuperDuperBody body = new PaymentRabbitToSuperDuperBody
                {
                    amount = amount,
                    currency = currency,
                    order_id = order_id,
                    is_pre_approve = tmp_is_pre_approve,
                    service_id = service_id,
                    channel_type = channel_type,
                    ref_1 = ref_1,
                    ref_2 = ref_2,
                    ref_3 = "",
                    ref_4 = "",
                    ref_5 = "",
                    metadata = metadata,
                    packages = packages,
                    redirect_urls = redirect_urls
                };

                GetPaymentRabbitToSuperDuperQuery getPaymentInquiryToSuperDuperQuery = new GetPaymentRabbitToSuperDuperQuery
                {
                    Url = endpoint,
                    ProductName = product_name,
                    ServiceName = ServiceName,
                    p_mobile_no = NonMobileNO,
                    Secret = channel_secret,
                    payment_method_id = payment_method_id,

                    ContentType = Content_Type,
                    MerchantID = X_sdpg_merchant_id,
                    Signature = "",
                    Nonce = Nonce,
                    Body = body,
                    FullUrl = fullUrl
                };

                PaymentRabbitToSuperDuperData = _queryProcessor.Execute(getPaymentInquiryToSuperDuperQuery);
            }
            else
            {
                PaymentRabbitToSuperDuperData.status_code = "-1";
                PaymentRabbitToSuperDuperData.message = "getConfigReqPayment No Data.";
            }
            return PaymentRabbitToSuperDuperData;
        }

        [HttpPost]
        public string GetPaymentInquiryToSuperDuper()
        {
            string result = "";

            PaymentInquiryToSuperDuperBody body = new PaymentInquiryToSuperDuperBody
            {
                service_id = "19100061935133847",
                card_ref = "dcc217ebdcd4430b9b0fd64cf2838533"
            };
            string SecKey = "1a2a1d4ecce34686";
            string Nonce = Guid.NewGuid().ToString();
            string Body = "";
            string Signature = "";
            string tmpData = "";
            Body = JsonConvert.SerializeObject(body);
            tmpData = Body + Nonce;

            Signature = hmacsha256(tmpData, SecKey);

            GetPaymentInquiryToSuperDuperQuery query = new GetPaymentInquiryToSuperDuperQuery
            {
                Url = "http://api-dev.apps.ocp4-dev.dmifco.com/dev/",

                ContentType = "application/json; charset=UTF-8",
                MerchantID = "5396508",
                Signature = "MjdFRkZCNzZDOTcwMjI0OTdFMjVEM0E1RDdFODIzNDJCQTM1RjE3OTA3MTU2OEIwQzMzNQ==",
                Nonce = "4572616e48616d6d65724c61686176",
                Body = body
            };

            var xxx = _queryProcessor.Execute(query);

            return result;
        }

        [HttpPost]
        public string GetPaymentEnquiryToSuperDuper()
        {
            string result = "";

            PaymentEnquiryToSuperDuperBody body = new PaymentEnquiryToSuperDuperBody
            {
                txn_id = "T19317181124303024707"
            };
            string SecKey = "1a2a1d4ecce34686";
            string Nonce = Guid.NewGuid().ToString();
            string Body = "";
            string Signature = "";
            string tmpData = "";
            Body = JsonConvert.SerializeObject(body);
            tmpData = Body + Nonce;

            Signature = hmacsha256(tmpData, SecKey);


            GetPaymentEnquiryToSuperDuperQuery query = new GetPaymentEnquiryToSuperDuperQuery
            {
                Url = "http://api-dev.apps.ocp4-dev.dmifco.com/dev/service-txn-gateway/v1/enquiry",

                ContentType = "application/json",
                MerchantID = "2032832",
                Signature = Signature,
                Nonce = Nonce,
                Body = body
            };

            var xxx = _queryProcessor.Execute(query);

            return result;
        }

        [HttpPost]
        public ActionResult WebHookNotify()
        {
            var result = new object();

            var transactionId = "";
            var OrdertransactionId = "";
            var status = "";
            var merchantId = "";
            var channel = "";
            var log = new InterfaceLogCommand();
            var logStatus = "ERROR";
            var logMessage = "";
            var channelSecret = "";
            var DataNotify = new WebhookResponseModel();

            string fullUrl = this.Url.Action("WebHookNotify", "Process", null, this.Request.Url.Scheme);
            Session["FullUrl"] = fullUrl;

            try
            {
                Stream req = Request.InputStream;
                req.Seek(0, System.IO.SeekOrigin.Begin);
                string json = new StreamReader(req).ReadToEnd();
                var objResult = JObject.Parse(json);
                DataNotify = JsonConvert.DeserializeObject<WebhookResponseModel>(json);
                DataNotify.Json_RawData = json;

                OrdertransactionId = DataNotify.order_id.ToSafeString();
                transactionId = DataNotify.txn_id.ToSafeString();
                status = DataNotify.status.ToSafeString();
                channel = DataNotify.channel_type.ToSafeString();

                //objResult

                var NonceQ = Request.Headers.GetValues("X-sdpg-nonce") ?? new[] { "" };
                var MerchantIDQ = Request.Headers.GetValues("X-sdpg-merchant-id") ?? new[] { "" };
                var SignatureQ = Request.Headers.GetValues("X-sdpg-signature") ?? new[] { "" };

                DataNotify.Header_Nonce = NonceQ[0];
                DataNotify.Header_MerchantID = MerchantIDQ[0];
                DataNotify.Header_Signature = SignatureQ[0];

                //TODO: Insert interface log
                log = StartInterface(DataNotify, "WebHookNotify", transactionId, "", "MPAY");

                //TODO: get secret from db channel_secret,X-sdpg-merchant-id
                channelSecret = GetChannelSecret(channel, transactionId);
                merchantId = GetMerchantID(channel, transactionId);

                //verify merchantId
                if (MerchantIDQ[0] != merchantId)
                {
                    result = new
                    {
                        error_code = "VALIDATE_REQUEST_MERCHANTID_FAILED",
                        error = "error secret-key not found for merchant " + MerchantIDQ[0],
                        context_path = "[POST] /process/WebHookNotify",
                        timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")
                    };

                    logMessage = "VALIDATE_REQUEST_MERCHANTID_FAILED";

                    return Json(result);
                }

                var signature = hmacsha256(json + NonceQ[0].ToString(), channelSecret);
                var arrSignatureRequest = StringToByteArray(SignatureQ[0].ToString());
                var arrSignatureWeb = StringToByteArray(signature);
                //compare signature
                var resultcompare = arrSignatureRequest.SequenceEqual(arrSignatureWeb);

                if (!resultcompare)
                {
                    result = new
                    {
                        error_code = "VALIDATE_REQUEST_SIGNATURE_FAILED",
                        error = "error invalid hash signature " + arrSignatureRequest,
                        context_path = "[POST] /process/WebHookNotify",
                        timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")
                    };

                    logMessage = "VALIDATE_REQUEST_SIGNATURE_FAILED";

                    return Json(result);
                }

                if (status == "SUCCESS")
                {
                    //TODO: create order aysn
                    var objReqTask = new Dictionary<string, string>();
                    var taskCreateOrder = Task.Factory.StartNew(new Func<Task>(async () => await CreateOrder(DataNotify, transactionId, OrdertransactionId))).ConfigureAwait(false);
                    taskCreateOrder.GetAwaiter();

                    //QR code ยิง signalR
                    if (channel == "QR")
                    {
                        var logSpdp = GetPaymentLogDataSuperDuper("", "", "", "", "", transactionId);
                        if (logSpdp != null && logSpdp.DataLog != null)
                        {
                            //modal message qr code
                            var orderId = logSpdp.DataLog.ORDER_ID.ToSafeString();
                            var tranId = transactionId.ToSafeString();
                            var hubContext = GlobalHost.ConnectionManager.GetHubContext<BaseHub>();
                            hubContext.Clients.All.addNewMessageToPage(orderId, tranId);
                        }
                    }
                }

                result = new
                {
                    status_code = "0000",
                    status_message = "Success",
                    timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")
                };

                logStatus = "Success";
            }
            catch (Exception ex)
            {
                base.Logger.Info("WebHookNotify Exception : " + ex.GetErrorMessage());

                logMessage = ex.GetErrorMessage();

                result = ex.StackTrace;
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            finally
            {
                base.Logger.Info("WebHookNotify End");
                //TODO: Update interface log
                EndInterface(result.ToSafeString(), log, transactionId, logStatus, logMessage);

            }

            return Json(result);
        }

        [HttpPost]
        public ActionResult MeshOrderRent(QuickWinPanelModel model)
        {
            string fullUrlStr = this.Url.Action("TopupMesh", "Process", null, this.Request.Url.Scheme);

            #region Get IP Address Interface Log : Edit 2017-01-30

            string transactionId = "";

            // Get IP Address
            string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress)) ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            transactionId = ipAddress + "|";

            #endregion

            var LovData = GetScreenConfig("FBBOR041");
            string ErrorMsgNotPaid = LovData.Where(t => t.Name == "L_POPUP_PAYMENT_ERROR").Select(t => t.DisplayValue).Count() > 0 ? LovData.Where(t => t.Name == "L_POPUP_PAYMENT_ERROR").Select(t => t.DisplayValue).FirstOrDefault() : "";
            string ErrorMsg = LovData.Where(t => t.Name == "L_POPUP_ERROR").Select(t => t.DisplayValue).Count() > 0 ? LovData.Where(t => t.Name == "L_POPUP_ERROR").Select(t => t.DisplayValue).FirstOrDefault() : "";
            ActionResult action = TopupMesh("", false, "", "", "", "", "", "", "", "", ErrorMsg, "", "", "", "", "");

            /// FBSSQueryOrderDoServiceConfirm
            FBSSQueryOrderDoServiceConfirm(model.CoveragePanelModel.P_MOBILE);

            //R22.11 Mesh with arpu //R23.01 Mesh with arpu Check RedeemPoint
            string meshArpuPoints = "";
            int meshPointsSelect = int.Parse(model.MESH_ARPU_POINTS_SELECT);
            string transactionIDPayPoint = "0";
            bool RedeemPointFlag = false;
            int countRedeemPoint = 0;
            int countRedeemPointFail = 0;
            if (model.MESH_ARPU_FLAG_OPTION == "P" && meshPointsSelect > 0)
            {
                RedeemPointFlag = true;

                for (int num = 0; num < meshPointsSelect; num++)
                {
                    string MsgBarcode = PrivilegeRedeemPoint(model.PayMentOrderID.ToSafeString(), model.CoveragePanelModel.P_MOBILE.ToSafeString(), "P19052018");

                    if (!string.IsNullOrEmpty(MsgBarcode))
                    {
                        countRedeemPoint += 1;
                        transactionIDPayPoint = transactionIDPayPoint == "0" ? MsgBarcode : transactionIDPayPoint + "|" + MsgBarcode;
                    }
                    else
                    {
                        countRedeemPointFail += 1;
                    }
                }

                if (countRedeemPoint > 0)
                {
                    var LOV_MESH_POINT_DISCOUNT = base.LovData
                    .Where(l => (l.Type == "FBB_CONSTANT" && l.Name == "MESH_POINT_DISCOUNT" && l.LovValue5 == "FBBOR041"))
                    .Select(s => s.LovValue1).FirstOrDefault();

                    meshArpuPoints = (countRedeemPoint * int.Parse(LOV_MESH_POINT_DISCOUNT)).ToSafeString();
                }
            }

            if (RedeemPointFlag && countRedeemPointFail > 0 && countRedeemPointFail.Equals(meshPointsSelect))
            {
                //Case RedeemPoint Fail All //R23.01 Mesh with arpu Check RedeemPoint
                string lovRedeemPointFail = "";
                string tmpLovName = "L_POPUP_ERROR_RENTAL_REDEEMPOINT";
                lovRedeemPointFail = LovData.Where(t => t.Name == tmpLovName).Select(t => t.DisplayValue).Count() > 0 ? LovData.Where(t => t.Name == tmpLovName).Select(t => t.DisplayValue).FirstOrDefault() : "";

                action = TopupMesh("", true, "", "", "", "", "", "", "", "", lovRedeemPointFail, "", "", "", "", "");
            }
            else
            {
                    /// Sent Sevice
                    GetCreateOrderMeshRentalQuery getCreateOrderMeshRentalQuery = new GetCreateOrderMeshRentalQuery()
                    {
                        p_internet_no = model.CoveragePanelModel.P_MOBILE,
                        p_payment_order_id = model.PayMentOrderID,
                        p_penalty_install = model.CustomerRegisterPanelModel.PenaltyInstall,
                        p_point = meshArpuPoints, //R23.01 Mesh with arpu Check RedeemPoint //model.MESH_ARPU_POINTS, //R22.11 Mesh with arpu
                        p_flag_option = model.MESH_ARPU_FLAG_OPTION, //R22.11 Mesh with arpu
                        p_flag_mesh = model.MESH_ARPU_FLAG_MESH //R22.11 Mesh with arpu
                    };
                    GetCreateOrderMeshRentalModel getCreateOrderMeshRentalModel = _queryProcessor.Execute(getCreateOrderMeshRentalQuery);

                    if (getCreateOrderMeshRentalModel.sffOrderNo != null && getCreateOrderMeshRentalModel.sffOrderNo != "")
                    {
                        /// Update CustRegister 
                        var customerRowID = RegisterCustomerMesh(getCreateOrderMeshRentalModel.sffOrderNo.ToSafeString(), model.PayMentOrderID, transactionIDPayPoint, "R");
                        //var customerRowID = RegisterCustomerMesh(getCreateOrderMeshRentalModel.sffOrderNo.ToSafeString(), model.PayMentOrderID, "0", "R");

                        if (customerRowID != null && customerRowID != "")
                        {
                            string Channel = model.SpecialRemark;
                            /// Get value info
                            string informationData = "";
                            string tmpLovName = "L_POPUP_SUCCESS_RENTAL_" + Channel.ToUpper();
                            informationData = LovData.Where(t => t.Name == tmpLovName).Select(t => t.DisplayValue).Count() > 0 ? LovData.Where(t => t.Name == tmpLovName).Select(t => t.DisplayValue).FirstOrDefault() : "";

                            if (informationData == "")
                            {
                                informationData = LovData.Where(t => t.Name == "L_POPUP_SUCCESS_RENTAL").Select(t => t.DisplayValue).Count() > 0 ? LovData.Where(t => t.Name == "L_POPUP_SUCCESS_RENTAL").Select(t => t.DisplayValue).FirstOrDefault() : "";
                            }

                            GetMeshCustomerProfileModel informationDataModel = GetMeshCustomerProfile(model.CoveragePanelModel.P_MOBILE, model.PayMentOrderID);
                            informationData = informationData.Replace("[OrderList]", informationDataModel.order_list);
                            informationData = informationData.Replace("[OrderNo]", informationDataModel.amount);
                            informationData = informationData.Replace("[PurchaseAmt]", informationDataModel.purchase_amt);
                            informationData = informationData.Replace("[TranId]", informationDataModel.tran_id);
                            informationData = informationData.Replace("[OrderDate]", informationDataModel.order_date);
                            informationData = informationData.Replace("[CustomerName]", informationDataModel.customer_name);
                            informationData = informationData.Replace("[NonMobileNo]", informationDataModel.non_mobile_no);
                            informationData = informationData.Replace("[ContactMobile]", informationDataModel.contact_mobile);
                            informationData = informationData.Replace("[InstallDate]", informationDataModel.install_date);

                            /// Sent SMS

                            MeshSendSMS(model.CustomerRegisterPanelModel.L_CONTACT_PHONE, informationDataModel.purchase_amt, informationDataModel.install_date, informationDataModel.non_mobile_no, informationDataModel.tran_id, "1", ipAddress, fullUrlStr, GetCurrentCulture().IsThaiCulture(), "");
                            action = TopupMesh("", true, "", "", "", "", "", "", "", "", informationData, "", "", "", "", "");
                        }
                    }
            }
            return action;
        }

        [HttpPost]
        public ActionResult SendOneTimePasswordGSSOService(string MobileNo = "")
        {
            string transactionID = "";
            string fullUrl = "";
            string msisdn = "";
            string otpChannel = base.LovData.FirstOrDefault(t => t.Name == "CONFIG_CHANNEL_SEND_OTP") != null ? base.LovData.FirstOrDefault(t => t.Name == "CONFIG_CHANNEL_SEND_OTP").LovValue1 : "";
            string service = base.LovData.FirstOrDefault(t => t.Name == "CONFIG_SERVICE_SEND_OTP") != null ? base.LovData.FirstOrDefault(t => t.Name == "CONFIG_SERVICE_SEND_OTP").LovValue1 : "";
            if (MobileNo != "")
            {
                msisdn = "66" + MobileNo.Substring(1);
            }

            SendOneTimePWModel sendOneTimePWModel = new SendOneTimePWModel();
            SendOneTimePW sendOneTimePW = new SendOneTimePW()
            {
                msisdn = msisdn,
                otpChannel = otpChannel,
                service = service,
                accountType = "all",
                lifeTimeoutMins = "5",
                waitDR = "false",
                otpDigit = "4",
                refDigit = "4"
            };
            sendOneTimePWModel.sendOneTimePW = sendOneTimePW;

            string url = base.LovData.FirstOrDefault(t => t.Name == "JSON_OTP") != null ? base.LovData.FirstOrDefault(t => t.Name == "JSON_OTP").LovValue1 : "";
            if (url != "" && msisdn != "")
            {
                try
                {
                    string BodyStr = JsonConvert.SerializeObject(sendOneTimePWModel);

                    RequestsGSSOServiceQuery query = new RequestsGSSOServiceQuery
                    {
                        p_endpoint = url,
                        p_mobile_no = MobileNo,
                        p_bodyJsonStr = BodyStr,
                        FullUrl = fullUrl
                    };
                    RequestsGSSOServiceModel requestsGSSOServiceModel = _queryProcessor.Execute(query);
                    if (requestsGSSOServiceModel.StatusCode == "Success" && requestsGSSOServiceModel.GSSOContent != "")
                    {
                        SendOneTimePWResponseModel responseModel = JsonConvert.DeserializeObject<SendOneTimePWResponseModel>(requestsGSSOServiceModel.GSSOContent) ?? new SendOneTimePWResponseModel();
                        if (responseModel != null && responseModel.sendOneTimePWResponse != null && responseModel.sendOneTimePWResponse.code == "2000")
                            transactionID = responseModel.sendOneTimePWResponse.transactionID;
                    }

                }
                catch
                {

                }
            }
            return Json(new { transactionID = transactionID }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ConfirmOneTimePasswordGSSOService(string MobileNo = "", string pwd = "", string transactionID = "", int GSSOServiceFlag = 0)
        {
            string status = "N";
            string fullUrl = "";
            string msisdn = "";
            string serviceGSSO = "";

            switch (GSSOServiceFlag)
            {
                case 0:
                    serviceGSSO = "CONFIG_SERVICE_SEND_OTP";
                    break;
                case 1:
                    serviceGSSO = "CONFIG_SERVICE_SEND_OTP_IPCAMERA";
                    break;
                default:
                    break;
            }
            string service = base.LovData.FirstOrDefault(t => t.Name == serviceGSSO) != null ? base.LovData.FirstOrDefault(t => t.Name == serviceGSSO).LovValue1 : "";

            if (MobileNo != "")
            {
                msisdn = "66" + MobileNo.Substring(1);
            }

            ConfirmOneTimePWModel confirmOneTimePWModel = new ConfirmOneTimePWModel();
            ConfirmOneTimePW confirmOneTimePW = new ConfirmOneTimePW()
            {
                msisdn = msisdn,
                pwd = pwd,
                service = service,
                transactionID = transactionID
            };
            confirmOneTimePWModel.confirmOneTimePW = confirmOneTimePW;

            string url = base.LovData.FirstOrDefault(t => t.Name == "JSON_CON") != null ? base.LovData.FirstOrDefault(t => t.Name == "JSON_CON").LovValue1 : "";
            if (url != "" && msisdn != "")
            {
                try
                {
                    string BodyStr = JsonConvert.SerializeObject(confirmOneTimePWModel);

                    RequestsGSSOServiceQuery query = new RequestsGSSOServiceQuery
                    {
                        p_endpoint = url,
                        p_mobile_no = MobileNo,
                        p_bodyJsonStr = BodyStr,
                        FullUrl = fullUrl
                    };
                    RequestsGSSOServiceModel requestsGSSOServiceModel = _queryProcessor.Execute(query);
                    if (requestsGSSOServiceModel.StatusCode == "Success" && requestsGSSOServiceModel.GSSOContent != "")
                    {
                        ConfirmOneTimePWResponseModel responseModel = JsonConvert.DeserializeObject<ConfirmOneTimePWResponseModel>(requestsGSSOServiceModel.GSSOContent) ?? new ConfirmOneTimePWResponseModel();
                        if (responseModel != null && responseModel.confirmOneTimePWResponse != null && responseModel.confirmOneTimePWResponse.code == "2000")
                            status = "Y";
                    }

                }
                catch
                {

                }
            }

            return Json(new { status = status }, JsonRequestBehavior.AllowGet);
        }

        public DoServiceConfirmModel DoServiceConfirm(DoServiceConfirmBody doServiceConfirmBody)
        {
            DoServiceConfirmModel result = null;

            DoServiceConfirmQuery query = new DoServiceConfirmQuery()
            {
                BodyData = doServiceConfirmBody,
                FullUrl = ""
            };

            result = _queryProcessor.Execute(query);

            return result;
        }

        public void FBSSQueryOrderDoServiceConfirm(string FIBRENET_ID = "", string ORDER_TYPE = "0")
        {
            InterfaceLogCommand log = null;
            log = StartInterface("FIBRENET_ID: " + FIBRENET_ID + "ORDER_TYPE: " + ORDER_TYPE, "FBSSQueryOrderDoServiceConfirm", FIBRENET_ID, "", "FBSSQueryOrderDoServiceConfirm");

            List<string> eventOrders = new List<string>();
            GetLovQuery eventOrderQuery = new GetLovQuery
            {
                LovType = "FBB_CONSTANT",
                LovName = "EVENT_ORDER"
            };
            List<LovValueModel> eventOrder = _queryProcessor.Execute(eventOrderQuery);
            if (eventOrder != null && eventOrder.Count > 0)
            {
                eventOrders = eventOrder.Select(t => t.LovValue1.ToSafeString()).ToList();
            }

            DateTime endDate = DateTime.Now;
            DateTime startDate = endDate.AddDays(-90);
            List<FIBRENetID> FIBRENetID_List = new List<FIBRENetID>();
            FIBRENetID FIBRENetIDData = new FIBRENetID()
            {
                FIBRENET_ID = FIBRENET_ID,
                START_DATE = startDate.Day.ToString("0#") + "/" + startDate.Month.ToString("0#") + "/" + startDate.Year + " " + startDate.Hour.ToString("0#") + ":" + startDate.Minute.ToString("0#") + ":" + startDate.Second.ToString("0#"),
                END_DATE = endDate.Day.ToString("0#") + "/" + endDate.Month.ToString("0#") + "/" + endDate.Year + " " + endDate.Hour.ToString("0#") + ":" + endDate.Minute.ToString("0#") + ":" + endDate.Second.ToString("0#")
            };
            FIBRENetID_List.Add(FIBRENetIDData);

            var query = new QueryOrderQuery();
            query.ORDER_TYPE = ORDER_TYPE;
            query.FIBRENetID_List = FIBRENetID_List;

            QueryOrderModel result = _queryProcessor.Execute(query);

            if (result != null && result.RESULT == "0")
            {
                foreach (var item in result.Order_Details_List)
                {

                    if (item.TRANSACTION_STATE == "Handling" && eventOrders.IndexOf(item.EVENT) != -1)
                    {
                        Activity_Details tmpDetail = new Activity_Details();


                        DateTime MaxDateTime = new DateTime();
                        string ACTIVITY = "";
                        string WORK_ORDER_STATE = "";
                        if (item.ACTIVITY_DETAILS != null && item.ACTIVITY_DETAILS.Count > 0)
                        {
                            bool firstloop = true;
                            foreach (var itemDetail in item.ACTIVITY_DETAILS)
                            {
                                if (firstloop)
                                {
                                    string createDate = itemDetail.CREATED_DATE.ToSafeString();
                                    Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
                                    System.Globalization.CultureInfo enUS = new System.Globalization.CultureInfo("en-US");
                                    DateTime.TryParseExact(createDate, "dd/MM/yyyy HH:mm:ss", enUS, System.Globalization.DateTimeStyles.None, out MaxDateTime);

                                    ACTIVITY = itemDetail.ACTIVITY;
                                    WORK_ORDER_STATE = itemDetail.WORK_ORDER_STATE;
                                    firstloop = false;
                                }
                                else
                                {
                                    string createDate = itemDetail.CREATED_DATE.ToSafeString();
                                    Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
                                    System.Globalization.CultureInfo enUS = new System.Globalization.CultureInfo("en-US");
                                    DateTime tmpDateTime = new DateTime();
                                    DateTime.TryParseExact(createDate, "dd/MM/yyyy HH:mm:ss", enUS, System.Globalization.DateTimeStyles.None, out tmpDateTime);

                                    if (tmpDateTime > MaxDateTime)
                                    {
                                        MaxDateTime = tmpDateTime;
                                        ACTIVITY = itemDetail.ACTIVITY;
                                        WORK_ORDER_STATE = itemDetail.WORK_ORDER_STATE;
                                    }
                                }
                            }
                        }

                        /// DoServiceConfirm                      
                        if (ACTIVITY == "Service Confirm" && WORK_ORDER_STATE == "successful")
                        {
                            DoServiceConfirmBody doServiceConfirmBody = new DoServiceConfirmBody()
                            {
                                FIBRENET_ID = item.FIBRENET_ID,
                                //ORDER_TYPE = item.ORDER_TYPE,
                                ORDER_TYPE = "1",
                                ORDER_NO = item.TRANSACTION_NUMBER,
                                ACTION = "A",
                                REASON = "",
                                SUB_REASON = "",
                                TAG_CODE = "",
                                STAFF_ID = "",
                                REMARK = "Bill Approve form WS."
                            };
                            DoServiceConfirm(doServiceConfirmBody);
                        }
                    }
                }
            }
            EndInterface("", log, "", "Success", "End Pocesss FBSSQueryOrderDoServiceConfirm");
        }

        private string MeshSendSMS(string mobileNo, string PurchaseAmt, string InstallDate, string NonMobileNo, string TranID, string MsgWay, string ipAddress, string FullUrl, bool IsThaiCulture, string UrlForSentSMS)
        {
            string result = "N";
            base.Logger.Info("Wait MeshSendSMS");
            try
            {
                string transactionId = "";
                transactionId = ipAddress + "|";

                var sourc_addr = "";

                sourc_addr = LovData.Where(x => x.Type == "CONFIG" && x.Name == "SENDER_SMS").Any() ? LovData.FirstOrDefault(x => x.Type == "CONFIG" && x.Name == "SENDER_SMS").LovValue1 : "AISFIBRE";

                var query = new MeshSmsQuery();
                query.FullUrl = FullUrl;
                query.Source_Addr = sourc_addr;
                query.mobileNo = mobileNo;
                query.PurchaseAmt = PurchaseAmt;
                query.Transaction_Id = transactionId;
                query.InstallDate = InstallDate;
                query.NonMobileNo = NonMobileNo;
                query.TranID = TranID;
                query.MsgWay = MsgWay;
                query.FullUrl = FullUrl;
                query.IsThaiCulture = IsThaiCulture;
                query.UrlForSentSMS = UrlForSentSMS;
                result = _queryProcessor.Execute(query);
                base.Logger.Info("End Wait MeshSendSMS");
            }
            catch (Exception ex)
            {
                base.Logger.Info("Error MeshSendSMS : " + ex.Message);
            }
            return result;

        }

        private GetPaymentLogDataSuperDuperModel GetPaymentLogDataSuperDuper(string NonMobileNO, string ProductName, string ServiceName, string OrderId, string OrderTransactionId, string TransactionId)
        {
            var result = new GetPaymentLogDataSuperDuperModel();
            var getConfigReqPaymentQuery = new GetPaymentLogDataSuperDuperQuery()
            {
                Url = this.Url.Action("GetPaymentLogDataSuperDuper", "Process", null, this.Request.Url.Scheme),
                product_name = ProductName,
                service_name = ServiceName,
                non_mobile_no = NonMobileNO,
                order_id = OrderId,
                order_transaction_id = OrderTransactionId,
                transaction_id = TransactionId,
            };
            result = _queryProcessor.Execute(getConfigReqPaymentQuery);
            return result;
        }

        private async Task CreateOrder(WebhookResponseModel objResult, string transactionId, string ordertransactionId)
        {
            string fullUrl = this.Url.Action("WebHookNotify", "Process", null, this.Request.Url.Scheme);
            var query = new WebHookNotifyQuery
            {
                FullUrl = fullUrl,
                TransactionId = transactionId,
                OrderTransactionId = ordertransactionId,
                DataResult = objResult
            };
            var result = _queryProcessor.Execute(query);
        }

        private string GetChannelSecret(string ServiceName, string transactionId)
        {
            var channelSecret = "";

            try
            {
                GetConfigReqPaymentQuery getConfigReqPaymentQuery = new GetConfigReqPaymentQuery()
                {
                    p_service_name = ServiceName,
                    p_transaction_id = transactionId
                };

                GetConfigReqPaymentModel getConfigReqPaymentData = _queryProcessor.Execute(getConfigReqPaymentQuery);

                if (getConfigReqPaymentData.ret_code == "0" && getConfigReqPaymentData.list_config_req_payment != null && getConfigReqPaymentData.list_config_req_payment.Count > 0)
                {
                    List<ConfigReqPaymentData> configReqPaymentDatas = getConfigReqPaymentData.list_config_req_payment;

                    channelSecret = (configReqPaymentDatas.FirstOrDefault(item => item.attr_name == "channel_secret") ?? new ConfigReqPaymentData()).attr_value;
                }
            }
            catch (Exception ex)
            {
                base.Logger.Info("Error GetChannelSecret : " + ex.Message);
            }

            return channelSecret;
        }

        private void UpdateCancelPendingOrderSuperDuper(string orderTransactionId, string pageRequest)
        {
            InterfaceLogCommand log = null;
            var logStatus = "Failed";
            var logMessage = "";
            var dataDecrypt = "";
            try
            {
                log = StartInterface(orderTransactionId, "UpdateCancelPendingOrderSuperDuper", orderTransactionId, "", pageRequest);
            }
            catch (Exception ex)
            {
                base.Logger.Info("UpdateCancelPendingOrderSuperDuper StartInterface Exception : " + ex.GetErrorMessage());
            }
            try
            {
                var commandCancelSpdp = new SavePaymentSPDPLogCommand()
                {
                    p_action = "Cancel",
                    p_service_name = "WebHook Notify",
                    p_user_name = "WebHook Notify Cancel",
                    p_order_transaction_id = orderTransactionId,
                };
                _savePaymentSPDPLogCommand.Handle(commandCancelSpdp);

                logStatus = "Success";
                logMessage = "";
            }
            catch (Exception ex)
            {
                base.Logger.Info("UpdateCancelPendingOrderSuperDuper Exception : " + ex.GetErrorMessage());
                logStatus = "Failed";
                logMessage = ex.GetErrorMessage();
            }
            finally
            {
                EndInterface(dataDecrypt, log, "", logStatus, logMessage);
            }
        }

        private string DecodeOrderTransactionId(string Data = "")
        {
            var result = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(Data))
                {
                    var dataDecrypt = EncryptionUtility.Base64Decode(Data);
                    if (!string.IsNullOrEmpty(dataDecrypt))
                    {
                        var arrData = dataDecrypt.Split('=');
                        if (arrData.Length == 2)
                        {
                            var orderTransactionId = arrData[1];
                            if (!string.IsNullOrEmpty(orderTransactionId))
                            {
                                result = orderTransactionId;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = string.Empty;
                base.Logger.Info("DecodeOrderTransactionId Exception : " + ex.GetErrorMessage());
            }
            return result;
        }

        private string GetMerchantID(string ServiceName, string transactionId)
        {
            var merchantID = "";

            try
            {
                GetConfigReqPaymentQuery getConfigReqPaymentQuery = new GetConfigReqPaymentQuery()
                {
                    p_service_name = ServiceName,
                    p_transaction_id = transactionId
                };

                GetConfigReqPaymentModel getConfigReqPaymentData = _queryProcessor.Execute(getConfigReqPaymentQuery);

                if (getConfigReqPaymentData.ret_code == "0" && getConfigReqPaymentData.list_config_req_payment != null && getConfigReqPaymentData.list_config_req_payment.Count > 0)
                {
                    List<ConfigReqPaymentData> configReqPaymentDatas = getConfigReqPaymentData.list_config_req_payment;

                    merchantID = (configReqPaymentDatas.FirstOrDefault(item => item.attr_name == "X-sdpg-merchant-id") ?? new ConfigReqPaymentData()).attr_value;
                }
            }
            catch (Exception ex)
            {
                base.Logger.Info("Error GetMerchantID : " + ex.Message);
            }
            return merchantID;
        }

        [HttpPost]
        public JsonResult SMSIPCamera(string MobileNo) => Json(GSSOIPCamera(new SendOneTimePW { msisdn = MobileNo }), JsonRequestBehavior.AllowGet);

        private string GSSOIPCamera(SendOneTimePW sendOneTimePW)
        {
            string url = base.LovData.Where(t => t.Name == "JSON_OTP").Select(s => s.LovValue1).FirstOrDefault();
            if (string.IsNullOrEmpty(sendOneTimePW.msisdn) || url == "") return "";
            var query = GSSOIPCameraQuery(sendOneTimePW);
            query.p_endpoint = url;
            var serviceModel = _queryProcessor.Execute(query);
            var responseModel = JsonConvert.DeserializeObject<SendOneTimePWResponseModel>(serviceModel.GSSOContent) ?? new SendOneTimePWResponseModel();
            return responseModel?.sendOneTimePWResponse?.code == "2000" ? responseModel.sendOneTimePWResponse.transactionID : "";
        }

        private RequestsGSSOServiceQuery GSSOIPCameraQuery(SendOneTimePW req)
        {
            req.msisdn = new StringBuilder("66").Append(req.msisdn.Substring(1)).ToString();
            req.service = LovData.Where(t => t.Name == "CONFIG_SERVICE_SEND_OTP_IPCAMERA").Select(t => t.LovValue1).FirstOrDefault();
            req.otpChannel = "sms";
            req.accountType = "all";
            req.lifeTimeoutMins = "5";
            req.waitDR = "false";
            req.otpDigit = "4";
            req.refDigit = "4";
            return new RequestsGSSOServiceQuery
            {
                p_mobile_no = req.msisdn,
                p_bodyJsonStr = JsonConvert.SerializeObject(
                    new SendOneTimePWModel
                    {
                        sendOneTimePW = req
                    })
            };
        }
    }
}