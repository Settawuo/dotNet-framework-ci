using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Xml.Serialization;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBContract.Queries.WebServices;
using WBBEntity.Extensions;
using WBBEntity.PanelModels;
using WBBEntity.PanelModels.WebServiceModels;
using WBBWeb.Extension;

namespace WBBWeb.Controllers
{
    [CustomActionFilter]
    [CustomHandleError]
    [IENoCache]
    public class PaymentMeshController : WBBController
    {
        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<SavePaymentLogCommand> _savePaymentLogCommand;
        private readonly ICommandHandler<CreateOrderMeshPromotionCommand> _createOrderMeshPromotionCommand;
        private readonly ICommandHandler<InterfaceLogCommand> _intfLogCommand;

        public PaymentMeshController(IQueryProcessor queryProcessor,
            ICommandHandler<SavePaymentLogCommand> savePaymentLogCommand,
            ICommandHandler<CreateOrderMeshPromotionCommand> createOrderMeshPromotionCommand,
            ICommandHandler<InterfaceLogCommand> intfLogCommand)
        {
            _queryProcessor = queryProcessor;
            _savePaymentLogCommand = savePaymentLogCommand;
            _createOrderMeshPromotionCommand = createOrderMeshPromotionCommand;
            _intfLogCommand = intfLogCommand;
        }

        public ActionResult Index(string InternetNo = "", string MobileNo = "", string PayMentOrderID = "", string PayMentRecurringChargeVAT = "", string SffPromotionCode = "", string MsgInfo = "", string MsgERROR = "", bool IsSucces = false, string ba = "")
        {
            var controller = DependencyResolver.Current.GetService<ProcessController>();
            ViewBag.LabelFBBOR041 = controller.GetScreenConfig("FBBOR041");
            ViewBag.FbbConstant = controller.GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);
            ViewBag.InternetNo = InternetNo;
            ViewBag.MobileNo = MobileNo;
            ViewBag.PayMentOrderID = PayMentOrderID;
            ViewBag.PayMentRecurringChargeVAT = PayMentRecurringChargeVAT;
            ViewBag.SffPromotionCode = SffPromotionCode;
            ViewBag.MsgInfo = MsgInfo;
            ViewBag.MsgERROR = MsgERROR;
            ViewBag.IsSucces = IsSucces;
            //R20.10 ref1 = ba
            ViewBag.Ba = ba;

            return View("Index");
        }

        public ActionResult SelectPayment(string data = "")
        {
            Session["FullUrl"] = this.Url.Action("SelectPayment", "PaymentMesh", null, this.Request.Url.Scheme);

            var controllerProcess = DependencyResolver.Current.GetService<ProcessController>();
            var controllerSCPE = DependencyResolver.Current.GetService<ScpeController>();

            var LovData = controllerProcess.GetScreenConfig("FBBOR041");
            string ErrorMsgNotPaid = LovData.Where(t => t.Name == "L_POPUP_PAYMENT_SMS_ERROR").Select(t => t.DisplayValue).Count() > 0 ? LovData.Where(t => t.Name == "L_POPUP_PAYMENT_SMS_ERROR").Select(t => t.DisplayValue).FirstOrDefault() : "";

            ActionResult action = Index("", "", "", "", "", "", ErrorMsgNotPaid);

            GetMeshCustomerProfileModel informationDataModel = null;
            string InternetNo = "";
            string OrderPayment = "";
            if (data != "")
            {
                string tmpData = Decrypt(data);
                string[] DataTemps = tmpData.Split('&');
                if (DataTemps != null && DataTemps.Count() == 2)
                {
                    InternetNo = DataTemps[0].ToSafeString();
                    OrderPayment = DataTemps[1].ToSafeString();

                    /// check order paid
                    var GetOrderPaymentData = controllerSCPE.GetOrderPayment(OrderPayment);
                    if (GetOrderPaymentData != null)
                    {
                        JsonResult GetOrderPaymentDataJson = (JsonResult)GetOrderPaymentData;

                        if (GetOrderPaymentDataJson.Data.ToSafeString() == "N")
                        {
                            /// not paid
                            /// get order detail
                            informationDataModel = controllerProcess.GetMeshCustomerProfile(InternetNo, OrderPayment);

                            if (informationDataModel != null && informationDataModel.language != null)
                            {
                                string InformationStr = "";
                                // set Language
                                ChangeCurrentCulture(int.Parse(informationDataModel.language));
                                LovData = controllerProcess.GetScreenConfig("FBBOR041");
                                InformationStr = LovData.Where(t => t.Name == "L_SUMMARY_PAYMENT").Select(t => t.DisplayValue).Count() > 0 ? LovData.Where(t => t.Name == "L_SUMMARY_PAYMENT").Select(t => t.DisplayValue).FirstOrDefault() : "";
                                InformationStr = InformationStr.Replace("[OrderList]", informationDataModel.order_list);
                                InformationStr = InformationStr.Replace("[OrderNo]", informationDataModel.amount);
                                InformationStr = InformationStr.Replace("[PurchaseAmt]", informationDataModel.purchase_amt);
                                InformationStr = InformationStr.Replace("[CustomerName]", informationDataModel.customer_name);
                                InformationStr = InformationStr.Replace("[NonMobileNo]", informationDataModel.non_mobile_no);
                                InformationStr = InformationStr.Replace("[ContactMobile]", informationDataModel.contact_mobile);
                                InformationStr = InformationStr.Replace("[InstallDate]", informationDataModel.install_date);

                                action = Index(InternetNo, informationDataModel.contact_mobile, OrderPayment, informationDataModel.purchase_amt, informationDataModel.sff_promotion_code, InformationStr, "", false, informationDataModel.ba);
                            }
                        }
                    }
                }
            }

            return action;
        }

        [HttpPost]
        public async Task<ActionResult> PaymentToMPayGateway(string PayMentOrderID, string PayMentMethod, string PayMentRecurringChargeVAT)
        {

            string fullUrlStr = this.Url.Action("SelectPayment", "PaymentMesh", null, this.Request.Url.Scheme);

            #region Get IP Address Interface Log : Edit 2017-01-30

            string transactionId = "";

            // Get IP Address
            string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress)) ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            transactionId = ipAddress + "|";

            #endregion

            QuickWinPanelModel model = new QuickWinPanelModel();
            ///GetRegisterPendingPaymentByTransactionID HaveTimeOut
            RegisterPendingPaymentData registerPendingPaymentData = GetRegisterPendingPaymentByTransactionID(PayMentOrderID, true);
            if (registerPendingPaymentData != null)
            {
                model.PayMentOrderID = PayMentOrderID;
                model.PayMentRecurringChargeVAT = PayMentRecurringChargeVAT;
                model.CoveragePanelModel.P_MOBILE = registerPendingPaymentData.ais_non_mobile;
                model.PayMentMethod = registerPendingPaymentData.payment_method;
                model.CustomerRegisterPanelModel.L_CONTACT_PHONE = registerPendingPaymentData.contact_mobile_phone1;
            }
            else
            {
                var controller = DependencyResolver.Current.GetService<ProcessController>();
                var LovData = controller.GetScreenConfig("FBBOR041");
                string ErrorMsg = LovData.Where(t => t.Name == "L_POPUP_PAYMENT_SMS_ERROR").Select(t => t.DisplayValue).Count() > 0 ? LovData.Where(t => t.Name == "L_POPUP_PAYMENT_SMS_ERROR").Select(t => t.DisplayValue).FirstOrDefault() : "";
                ActionResult action = Index("", "", "", "", "", "", ErrorMsg);
                return action;
            }

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
            string reqRef5 = "64";
            string reqRef1 = model.CoveragePanelModel.P_MOBILE.ToSafeString();


            if (LovConfigData != null && LovConfigData.Count > 0)
            {
                url = LovConfigData.Where(t => t.Text == "url").Select(t => t.LovValue1).Count() > 0 ? LovConfigData.Where(t => t.Text == "url").Select(t => t.LovValue1).FirstOrDefault() : "";
                projectCode = LovConfigData.Where(t => t.Text == "projectCode").Select(t => t.LovValue1).Count() > 0 ? LovConfigData.Where(t => t.Text == "projectCode").Select(t => t.LovValue1).FirstOrDefault() : "";
                command = LovConfigData.Where(t => t.Text == "command").Select(t => t.LovValue1).Count() > 0 ? LovConfigData.Where(t => t.Text == "command").Select(t => t.LovValue1).FirstOrDefault() : "";
                sid = LovConfigData.Where(t => t.Text == "sid").Select(t => t.LovValue1).Count() > 0 ? LovConfigData.Where(t => t.Text == "sid").Select(t => t.LovValue1).FirstOrDefault() : "";
                redirectUrl = LovConfigData.Where(t => t.Text == "redirectUrlForSMS").Select(t => t.LovValue1).Count() > 0 ? LovConfigData.Where(t => t.Text == "redirectUrlForSMS").Select(t => t.LovValue1).FirstOrDefault() : "";
                merchantId = LovConfigData.Where(t => t.Text == "merchantId").Select(t => t.LovValue1).Count() > 0 ? LovConfigData.Where(t => t.Text == "merchantId").Select(t => t.LovValue1).FirstOrDefault() : "";
                currency = LovConfigData.Where(t => t.Text == "currency").Select(t => t.LovValue1).Count() > 0 ? LovConfigData.Where(t => t.Text == "currency").Select(t => t.LovValue1).FirstOrDefault() : "";
                smsFlag = LovConfigData.Where(t => t.Text == "smsFlag").Select(t => t.LovValue1).Count() > 0 ? LovConfigData.Where(t => t.Text == "smsFlag").Select(t => t.LovValue1).FirstOrDefault() : "";
                orderExpire = LovConfigData.Where(t => t.Text == "orderExpire").Select(t => t.LovValue1).Count() > 0 ? LovConfigData.Where(t => t.Text == "orderExpire").Select(t => t.LovValue1).FirstOrDefault() : "";
                SecretKey = LovConfigData.Where(t => t.Text == "SecretKEY").Select(t => t.LovValue1).Count() > 0 ? LovConfigData.Where(t => t.Text == "SecretKEY").Select(t => t.LovValue1).FirstOrDefault() : "";

                //cal integrityStr
                integrityStr = HashToHex(sid + merchantId + PayMentOrderID + tmpPurchaseAmt + SecretKey);
            }

            // ForDev
            if (Request.IsLocal)
            {
                redirectUrl = "http://localhost:50960/PaymentMesh/PaymentToMPayGatewayResult";
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
                    PAYMENT_ORDER_ID = PayMentOrderID,
                    ENDPOINT = url,
                    REQ_PROJECT_CODE = projectCode,
                    REQ_COMMAND = command,
                    REQ_SID = sidDecode,
                    REQ_REDIRECT_URL = redirectUrl,
                    REQ_MERCHANT_ID = merchantId,
                    REQ_ORDER_ID = PayMentOrderID,
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
                        new KeyValuePair<string, string>("orderId", PayMentOrderID),
                        new KeyValuePair<string, string>("purchaseAmt", tmpPurchaseAmt),
                        new KeyValuePair<string, string>("currency", currency),
                        new KeyValuePair<string, string>("paymentMethod", model.PayMentMethod),
                        new KeyValuePair<string, string>("smsFlag", smsFlag),
                        new KeyValuePair<string, string>("orderExpire", orderExpire),
                        new KeyValuePair<string, string>("integrityStr", integrityStr),
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
                PAYMENT_ORDER_ID = PayMentOrderID,
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
                var controller = DependencyResolver.Current.GetService<ProcessController>();
                var LovData = controller.GetScreenConfig("FBBOR041");
                string ErrorMsg = LovData.Where(t => t.Name == "L_POPUP_PAYMENT_SMS_ERROR").Select(t => t.DisplayValue).Count() > 0 ? LovData.Where(t => t.Name == "L_POPUP_PAYMENT_SMS_ERROR").Select(t => t.DisplayValue).FirstOrDefault() : "";
                MeshSendSMS(model.CustomerRegisterPanelModel.L_CONTACT_PHONE, "", "", "", "", "2", ipAddress, fullUrlStr, GetCurrentCulture().IsThaiCulture(), "");
                ActionResult action = Index("", "", "", "", "", "", ErrorMsg);
                return action;
            }
        }

        [HttpPost]
        public ActionResult PaymentToMPayGatewayResult(string status, string respCode, string respDesc, string tranId, int saleId = 0, string orderId = "", string currency = "", int exchangeRate = 0)
        {
            bool CreateOrderStatus = false;
            ActionResult action = null;
            string fullUrl = this.Url.Action("PaymentToMPayGatewayResult", "PaymentMesh", null, this.Request.Url.Scheme);

            #region Get IP Address Interface Log : Edit 2017-01-30

            // Get IP Address
            string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress)) ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            #endregion

            var controller = DependencyResolver.Current.GetService<ProcessController>();
            var LovData = controller.GetScreenConfig("FBBOR041");
            string ErrorMsgNotPaid = LovData.Where(t => t.Name == "L_POPUP_PAYMENT_SMS_ERROR").Select(t => t.DisplayValue).Count() > 0 ? LovData.Where(t => t.Name == "L_POPUP_PAYMENT_SMS_ERROR").Select(t => t.DisplayValue).FirstOrDefault() : "";

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
            ///GetRegisterPendingPaymentByTransactionID
            RegisterPendingPaymentData registerPendingPaymentData = GetRegisterPendingPaymentByTransactionID(orderId, false);
            if (registerPendingPaymentData != null)
            {
                model.PayMentOrderID = orderId;
                model.CoveragePanelModel.P_MOBILE = registerPendingPaymentData.ais_non_mobile;
                model.PayMentMethod = registerPendingPaymentData.payment_method;
                model.CustomerRegisterPanelModel.L_CONTACT_PHONE = registerPendingPaymentData.contact_mobile_phone1;
                model.PayMentTranID = tranId.ToSafeString();
            }

            List<string> Statuss = new List<string>();
            Statuss.Add("S");
            Statuss.Add("Success");
            if (Statuss.Contains(status) && respCode == "0000")
            {
                /// Getvalue for sent to sff
                GetOrderChangeServiceModel DataForSentToSff = null;

                DataForSentToSff = controller.GetOrderChangeService(model.CoveragePanelModel.P_MOBILE, model.PayMentOrderID);

                if (DataForSentToSff != null && DataForSentToSff.RespCode != null && DataForSentToSff.RespCode != "-1")
                {
                    /// Sent data to sff
                    CreateOrderMeshPromotionResult result = null;
                    result = CreateOrderMeshPromotion(DataForSentToSff, model.CoveragePanelModel.P_MOBILE);
                    if (result != null && result.order_no.ToSafeString() != "")
                    {

                        /// Update CustRegister 
                        var customerRowID = controller.RegisterCustomerMesh(result.order_no.ToSafeString(), orderId.ToSafeString(), tranId.ToSafeString(), model.PayMentMethod.ToSafeString());

                        /// Get value info
                        string informationData = "";
                        informationData = LovData.Where(t => t.Name == "L_POPUP_SUCCESS").Select(t => t.DisplayValue).Count() > 0 ? LovData.Where(t => t.Name == "L_POPUP_SUCCESS").Select(t => t.DisplayValue).FirstOrDefault() : "";
                        GetMeshCustomerProfileModel informationDataModel = controller.GetMeshCustomerProfile(model.CoveragePanelModel.P_MOBILE, model.PayMentOrderID);
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

                        MeshSendSMS(model.CustomerRegisterPanelModel.L_CONTACT_PHONE, informationDataModel.purchase_amt, informationDataModel.install_date, informationDataModel.non_mobile_no, informationDataModel.tran_id, "1", ipAddress, fullUrl, GetCurrentCulture().IsThaiCulture(), "");
                        action = Index("", "", "", "", "", informationData, "", true);
                        CreateOrderStatus = true;
                    }
                }
            }

            if (!CreateOrderStatus)
            {
                /// Sent SMS
                MeshSendSMS(model.CustomerRegisterPanelModel.L_CONTACT_PHONE, "", "", "", "", "2", ipAddress, fullUrl, GetCurrentCulture().IsThaiCulture(), "");
                action = Index("", "", "", "", "", "", ErrorMsgNotPaid);
            }

            return action;
        }

        [HttpPost]
        public ActionResult PaymentWithPoint(string InternetNo = "", string MobileNo = "", string PayMentOrderID = "", string PayMentPointType = "", string PayMentRecurringChargeVAT = "", string SffPromotionCode = "", string MsgInfo = "", string MsgERROR = "", bool IsSucces = false)
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
            var controller = DependencyResolver.Current.GetService<ProcessController>();

            var LovData = controller.GetScreenConfig("FBBOR041");
            string ErrorMsg = LovData.Where(t => t.Name == "L_POPUP_PAYMENT_SMS_ERROR").Select(t => t.DisplayValue).Count() > 0 ? LovData.Where(t => t.Name == "L_POPUP_PAYMENT_SMS_ERROR").Select(t => t.DisplayValue).FirstOrDefault() : "";

            string PayMentMethod = "";

            string MobileRedeemPoint = "";
            if (PayMentPointType == "1")
            {
                MobileRedeemPoint = MobileNo;
            }
            else if (PayMentPointType == "2")
            {
                MobileRedeemPoint = InternetNo;
            }

            string transactionIDPayPoint = controller.PrivilegeRedeemPoint(PayMentOrderID.ToSafeString(), MobileRedeemPoint, SffPromotionCode);

            if (transactionIDPayPoint != "")
            {
                bool CreateOrderStatus = false;
                /// Getvalue for sent to sff
                GetOrderChangeServiceModel DataForSentToSff = null;

                DataForSentToSff = controller.GetOrderChangeService(InternetNo, PayMentOrderID.ToSafeString());

                if (DataForSentToSff != null && DataForSentToSff.RespCode != null && DataForSentToSff.RespCode != "-1")
                {
                    /// Sent data to sff
                    CreateOrderMeshPromotionResult result = null;
                    result = CreateOrderMeshPromotion(DataForSentToSff, InternetNo);
                    if (result != null && result.order_no.ToSafeString() != "")
                    {

                        /// Update CustRegister 
                        var customerRowID = controller.RegisterCustomerMesh(result.order_no.ToSafeString(), PayMentOrderID, transactionIDPayPoint, PayMentMethod.ToSafeString());

                        /// Get value info
                        string informationData = "";
                        informationData = LovData.Where(t => t.Name == "L_POPUP_SUCCESS").Select(t => t.DisplayValue).Count() > 0 ? LovData.Where(t => t.Name == "L_POPUP_SUCCESS").Select(t => t.DisplayValue).FirstOrDefault() : "";
                        GetMeshCustomerProfileModel informationDataModel = controller.GetMeshCustomerProfile(InternetNo, PayMentOrderID);
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

                        MeshSendSMS(MobileNo, informationDataModel.purchase_amt, informationDataModel.install_date, informationDataModel.non_mobile_no, informationDataModel.tran_id, "1", ipAddress, fullUrlStr, GetCurrentCulture().IsThaiCulture(), "");
                        action = Index("", "", "", "", "", informationData, "", true);
                        CreateOrderStatus = true;
                    }
                }
                if (!CreateOrderStatus)
                {
                    action = controller.TopupMesh("", false, "", "", "", "", "", "", "", "", ErrorMsg, "", "", "", "", "");
                }
            }
            else
            {
                MeshSendSMS(MobileNo, "", "", "", "", "2", ipAddress, fullUrlStr, GetCurrentCulture().IsThaiCulture(), "");
                action = controller.TopupMesh("", false, "", "", "", "", "", "", "", "", ErrorMsg, "", "", "", "", "");
            }

            return action;

        }

        [HttpPost]
        public JsonResult GetPaymentToMerchantQrCode(string payMentOrderId, string payMentRecurringChargeVat, string nonMobile = "")
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

                List<LovValueModel> lovConfigDataQr = new List<LovValueModel>();
                lovConfigDataQr = LovData.Where(t => t.Name == "RequestOrderQrcodeApi" && t.LovValue5 == "FBBOR041").ToList();

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

                /// Set Non Mobile in ref1
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

        //R22.08 Order Deadlock Change Service not allow
        public JsonResult MeshCheckOrderDeadlock(string mobileNo = "")
        {
            bool isOrderDeadlock = false;
            var lovTransactionState = base.LovData.Where(l => l.Name == "TRANSACTION_STATE").Select(s => s.LovValue1).ToList();
            DateTime endDate = DateTime.Now;
            DateTime startDate = endDate.AddDays(-90);
            List<FIBRENetID> FIBRENetID_List = new List<FIBRENetID>();
            FIBRENetID FIBRENetIDData = new FIBRENetID()
            {
                FIBRENET_ID = mobileNo,
                START_DATE = startDate.Day.ToString("0#") + "/" + startDate.Month.ToString("0#") + "/" + startDate.Year + " " + startDate.Hour.ToString("0#") + ":" + startDate.Minute.ToString("0#") + ":" + startDate.Second.ToString("0#"),
                END_DATE = endDate.Day.ToString("0#") + "/" + endDate.Month.ToString("0#") + "/" + endDate.Year + " " + endDate.Hour.ToString("0#") + ":" + endDate.Minute.ToString("0#") + ":" + endDate.Second.ToString("0#")
            };
            FIBRENetID_List.Add(FIBRENetIDData);

            var query = new QueryOrderQuery();
            query.ORDER_TYPE = "2";
            query.FIBRENetID_List = FIBRENetID_List;

            QueryOrderModel result = _queryProcessor.Execute(query);

            if (result != null && result.RESULT == "0")
            {
                foreach (var item in result.Order_Details_List)
                {
                    if (item.EVENT == "Join" && !(lovTransactionState.Contains(item.TRANSACTION_STATE)))
                    {
                        isOrderDeadlock = true;
                        break;
                    }
                }
            }

            return Json(new { isOrderDeadlock = isOrderDeadlock }, JsonRequestBehavior.AllowGet);
        }

        //R22.08 Order Deadlock Change Service not allow
        public JsonResult MeshCreateCaseGenericFlow(string mobileNo = "", string contactPhone = "", string installAddress = "")
        {
            bool isStatusCreate = false;

            var lovData = base.LovData.Where(l => l.LovValue5 == "FBBOR041" && l.Type == "FBB_CONSTANT").ToList();

            string interactionType = lovData.Where(l => l.Name == "InteractionType").Select(s => s.LovValue1).FirstOrDefault();
            string ownerName = lovData.Where(l => l.Name == "OwnerName").Select(s => s.LovValue1).FirstOrDefault();
            string status = lovData.Where(l => l.Name == "Status").Select(s => s.LovValue1).FirstOrDefault();
            string topicName = lovData.Where(l => l.Name == "TopicName").Select(s => s.LovValue1).FirstOrDefault();
            string subTopic = lovData.Where(l => l.Name == "SubTopic").Select(s => s.LovValue1).FirstOrDefault();
            string assignedType = lovData.Where(l => l.Name == "AssignedType").Select(s => s.LovValue1).FirstOrDefault();
            string assignedTo = lovData.Where(l => l.Name == "AssignedTo").Select(s => s.LovValue1).FirstOrDefault();
            string comments = lovData.Where(l => l.Name == "Comments").Select(s => s.LovValue1).FirstOrDefault();
            string fieldName1 = lovData.Where(l => l.Name == "FieldName_1").Select(s => s.LovValue1).FirstOrDefault();
            string fieldName2 = lovData.Where(l => l.Name == "FieldName_2").Select(s => s.LovValue1).FirstOrDefault();
            string fieldName3 = lovData.Where(l => l.Name == "FieldName_3").Select(s => s.LovValue1).FirstOrDefault();

            CreateCaseGenericFlowConfigCapturingList[] capturingList = new CreateCaseGenericFlowConfigCapturingList[3];
            capturingList[0] = new CreateCaseGenericFlowConfigCapturingList() { FieldName = fieldName1, Value = contactPhone.ToSafeString() };
            capturingList[1] = new CreateCaseGenericFlowConfigCapturingList() { FieldName = fieldName2, Value = installAddress.ToSafeString() };
            capturingList[2] = new CreateCaseGenericFlowConfigCapturingList() { FieldName = fieldName3, Value = string.Empty };

            var query = new CreateCaseGenericFlowQuery
            {
                MobileNo = mobileNo.ToSafeString(),
                InteractionType = interactionType.ToSafeString(),
                OwnerName = ownerName.ToSafeString(),
                Status = status.ToSafeString(),
                TopicName = topicName.ToSafeString(),
                SubTopic = subTopic.ToSafeString(),
                AssignedType = assignedType.ToSafeString(),
                AssignedTo = assignedTo.ToSafeString(),
                Comments = comments.ToSafeString(),
                CapturingList = capturingList
            };

            CreateCaseGenericFlowModel result = _queryProcessor.Execute(query);

            if (result != null && result.ErrorCode == "0")
            {
                isStatusCreate = true;
            }

            return Json(new { isStatusCreate = isStatusCreate }, JsonRequestBehavior.AllowGet);
        }

        //R22.11 Mesh with arpu
        public JsonResult GenTransactionIDWithArpu()
        {
            string data = "";
            try
            {
                GetPaymentOrderIDQuery query = new GetPaymentOrderIDQuery();
                data = _queryProcessor.Execute(query);
            }
            catch (Exception ex)
            {
                data = "";
            }
            return Json(data, JsonRequestBehavior.AllowGet);
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

        public RegisterPendingPaymentData GetRegisterPendingPaymentByTransactionID(string PaymentTransactionID, bool checkTimeout)
        {
            RegisterPendingPaymentData registerPendingPaymentData = null;
            GetRegisterPendingPaymentByTransactionIDInQuery query = new GetRegisterPendingPaymentByTransactionIDInQuery()
            {
                payment_transaction_id_in = PaymentTransactionID
            };
            DateTime tmpTime = DateTime.Now;

            var RegisterPendingPayment = _queryProcessor.Execute(query);
            if (RegisterPendingPayment != null && RegisterPendingPayment.RegisterPendingPaymentList != null && RegisterPendingPayment.RegisterPendingPaymentList.Count > 0)
            {
                foreach (var item in RegisterPendingPayment.RegisterPendingPaymentList)
                {
                    //DiffDate
                    double totalMinutes = (tmpTime - item.created).TotalMinutes;
                    if (totalMinutes < 30 || !checkTimeout)
                    {
                        if (item.payment_status != "SUCCESS" || !checkTimeout)
                        {
                            registerPendingPaymentData = new RegisterPendingPaymentData();
                            registerPendingPaymentData = item;
                        }
                    }
                }
            }

            return registerPendingPaymentData;
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

        private void MeshSendSMS(string mobileNo, string PurchaseAmt, string InstallDate, string NonMobileNo, string TranID, string MsgWay, string ipAddress, string FullUrl, bool IsThaiCulture, string UrlForSentSMS)
        {
            string transactionId = "";
            transactionId = ipAddress + "|";

            var query = new MeshSmsQuery();
            query.FullUrl = FullUrl;
            query.Source_Addr = "AISFIBRE";
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
            _queryProcessor.Execute(query);

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

    }
}
