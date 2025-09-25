using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using WBBContract;
using WBBContract.Commands;
using WBBEntity.Extensions;
using WBBEntity.PanelModels;
using WBBEntity.PanelModels.WebServiceModels;
using WBBWeb.Extension;

namespace WBBWeb.Controllers
{
    [CustomActionFilter]
    [CustomHandleError]
    [IENoCache]
    public class FBBPaymentController : WBBController
    {
        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<SavePaymentLogCommand> _savePaymentLogCommand;
        private readonly ICommandHandler<CreateOrderMeshPromotionCommand> _createOrderMeshPromotionCommand;
        private readonly ICommandHandler<InterfaceLogCommand> _intfLogCommand;

        public FBBPaymentController(IQueryProcessor queryProcessor,
            ICommandHandler<SavePaymentLogCommand> savePaymentLogCommand,
            ICommandHandler<CreateOrderMeshPromotionCommand> createOrderMeshPromotionCommand,
            ICommandHandler<InterfaceLogCommand> intfLogCommand)
        {
            _queryProcessor = queryProcessor;
            _savePaymentLogCommand = savePaymentLogCommand;
            _createOrderMeshPromotionCommand = createOrderMeshPromotionCommand;
            _intfLogCommand = intfLogCommand;
        }

        #region PaymentChannel
        public ActionResult PaymentChannel(PaymentChannelModel model)
        {
            if (model.data != null)
            {
                string tmpData = Decrypt(model.data);
                var value = JsonConvert.DeserializeObject<PaymentChannelModel>(tmpData);

                if (value != null)
                {
                    value.data = model.data;
                    var controller = DependencyResolver.Current.GetService<ProcessController>();
                    ViewBag.LabelFBBOR041 = controller.GetScreenConfig("FBBOR041");
                    ViewBag.LabelFBBTR001 = controller.GetCoverageScreenConfig();
                    ViewBag.FbbConstant = controller.GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);
                    ViewBag.InternetNo = value.fbb_id;
                    ViewBag.MobileNo = "";
                    ViewBag.PayMentOrderID = value.payment_transaction_id;
                    ViewBag.PayMentRecurringChargeVAT = "";
                    ViewBag.SffPromotionCode = "";
                    ViewBag.MsgInfo = "";
                    ViewBag.MsgERROR = "";
                    ViewBag.IsSucces = false;
                    ViewBag.Ba = "";

                    var listmethod = new List<LovScreenValueModel>();
                    foreach (var item in value.list_payment_method)
                    {
                        listmethod.AddRange(controller.GetScreenConfig(item));
                    }

                    string ExpireTimeQRPayment = controller.GetExpireTimeQRPayment(value.product_name, "Generate QR");
                    if (!string.IsNullOrEmpty(ExpireTimeQRPayment) && listmethod.Where(x => x.GroupByPDF == "QR_Code") != null)
                    {
                        ViewBag.QRTimeout = ExpireTimeQRPayment;
                        listmethod.FirstOrDefault(x => x.GroupByPDF == "QR_Code").DisplayValue = listmethod.FirstOrDefault(x => x.GroupByPDF == "QR_Code").DisplayValue.Replace("{0}", ExpireTimeQRPayment);
                    }

                    ViewBag.PaymentMethod = listmethod;
                    ViewBag.Channel = value.register_channel;
                    ViewBag.SuperDuperProductName = value.product_name;
                    ViewBag.Vas = (value.product_name != "WTTx" ? "2" : "3");
                }
                else
                {
                    var controllerProcess = DependencyResolver.Current.GetService<ProcessController>();
                    var LovData = controllerProcess.GetScreenConfig("FBBOR041");
                    ViewBag.MsgERROR = LovData.Where(t => t.Name == "L_POPUP_PAYMENT_SMS_ERROR").Select(t => t.DisplayValue).Count() > 0 ? LovData.Where(t => t.Name == "L_POPUP_PAYMENT_SMS_ERROR").Select(t => t.DisplayValue).FirstOrDefault() : "";
                }
            }
            else
            {
                var controllerProcess = DependencyResolver.Current.GetService<ProcessController>();
                var LovData = controllerProcess.GetScreenConfig("FBBOR041");
                ViewBag.MsgERROR = LovData.Where(t => t.Name == "L_POPUP_PAYMENT_SMS_ERROR").Select(t => t.DisplayValue).Count() > 0 ? LovData.Where(t => t.Name == "L_POPUP_PAYMENT_SMS_ERROR").Select(t => t.DisplayValue).FirstOrDefault() : "";
            }

            return View("PaymentChannel");
        }
        #endregion
        #region LinePayRequest
        public ActionResult LinePayRequest(PaymentChannelModel model)
        {
            try
            {
                string address = "https://sandbox-api-pay.line.me";
                string channelId = "1656556853";
                string channelSecret = "58318c33fdb49dac69012d8623184ec2";
                string uri = "/v3/payments/request";
                string nonce = Guid.NewGuid().ToString();

                var lineform = new LinePayAutomaticPaymentModel
                {
                    amount = 200,
                    currency = "THB",
                    orderId = Guid.NewGuid().ToString(), //model.payment_transaction_id,
                    packages = new List<LinePayPackagesModel>
                    {
                        new LinePayPackagesModel
                        {
                            id = "1",
                            amount = 200,
                            products = new List<LinePayProductsModel>
                            {
                                new LinePayProductsModel
                                {
                                    id = "01",
                                    name = "Product 001",
                                    imageUrl = "https://cdn-img.wemall.com/943745/w_1400,h_1400,c_thumb/93a3354b89ef9d10471ed471bc6a9d05/iphone_12_mini_black_pdp_image1.jpg",
                                    quantity = 1,
                                    price = 50
                                },
                                new LinePayProductsModel
                                {
                                    id = "02",
                                    name = "Product 002",
                                    imageUrl = "https://cdn-img.wemall.com/943745/w_1400,h_1400,c_thumb/cddf6292a73fe343f255233ecca834a8/iphone_12_pro_max_silver_pdp_image1.jpg",
                                    quantity = 5,
                                    price = 30
                                }
                            }
                        }
                    },
                    redirectUrls = new LinePayRedirectUrlsModel
                    {
                        confirmUrl = "http://localhost:50960/order/payment/authorizel",
                        cancelUrl = "http://localhost:50960/order/payment/cancel"
                    },
                    options = new LinePayOptionsModel
                    {
                        payment = new LinePayOptionsPaymentModel
                        {
                            payType = "PREAPPROVED"
                        }
                    }
                };

                string queryOrBody = JsonConvert.SerializeObject(lineform);
                string signature = GetAuthSignature(channelSecret, uri, queryOrBody, nonce);

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(address);
                    client.DefaultRequestHeaders.Add("X-LINE-ChannelId", channelId);
                    client.DefaultRequestHeaders.Add("X-LINE-Authorization-Nonce", nonce);
                    client.DefaultRequestHeaders.Add("X-LINE-Authorization", signature);

                    var httpContent = new StringContent(queryOrBody, Encoding.UTF8, "application/json");
                    var response = client.PostAsync(uri, httpContent).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var result = response.Content.ReadAsStringAsync().Result;
                        var output = JsonConvert.DeserializeObject<LinePayResponseModel>(result);

                        if (output.returnCode == "0000")
                        {
                            return Redirect(output.info.paymentUrl.web);
                        }
                        else
                        {
                            var controllerProcess = DependencyResolver.Current.GetService<ProcessController>();
                            var LovData = controllerProcess.GetScreenConfig("FBBOR041");
                            ViewBag.MsgERROR = LovData.Where(t => t.Name == "L_POPUP_PAYMENT_SMS_ERROR").Select(t => t.DisplayValue).Count() > 0 ? LovData.Where(t => t.Name == "L_POPUP_PAYMENT_SMS_ERROR").Select(t => t.DisplayValue).FirstOrDefault() : "";
                            return View("PaymentChannel");
                        }
                    }
                    else
                    {
                        var controllerProcess = DependencyResolver.Current.GetService<ProcessController>();
                        var LovData = controllerProcess.GetScreenConfig("FBBOR041");
                        ViewBag.MsgERROR = LovData.Where(t => t.Name == "L_POPUP_PAYMENT_SMS_ERROR").Select(t => t.DisplayValue).Count() > 0 ? LovData.Where(t => t.Name == "L_POPUP_PAYMENT_SMS_ERROR").Select(t => t.DisplayValue).FirstOrDefault() : "";
                        return View("PaymentChannel");
                    }
                }
            }
            catch (Exception ex)
            {
                var controllerProcess = DependencyResolver.Current.GetService<ProcessController>();
                var LovData = controllerProcess.GetScreenConfig("FBBOR041");
                ViewBag.MsgERROR = LovData.Where(t => t.Name == "L_POPUP_PAYMENT_SMS_ERROR").Select(t => t.DisplayValue).Count() > 0 ? LovData.Where(t => t.Name == "L_POPUP_PAYMENT_SMS_ERROR").Select(t => t.DisplayValue).FirstOrDefault() : "";
                return View("PaymentChannel");
            }
        }
        #endregion
        #region GetAuthSignature
        private string GetAuthSignature(string channelSecret, string uri, string queryOrBody, string nonce)
        {
            Encoding ascii = Encoding.ASCII;
            HMACSHA256 hmac = new HMACSHA256(ascii.GetBytes(channelSecret));
            string authMacText = channelSecret + uri + queryOrBody + nonce;
            return Convert.ToBase64String(hmac.ComputeHash(ascii.GetBytes(authMacText)));
        }
        #endregion
        #region LinePayHMACSHA256
        private string LinePayHMACSHA256(string key, string message)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            byte[] keyByte = encoding.GetBytes(key);

            HMACSHA256 hmacsha256 = new HMACSHA256(keyByte);

            byte[] messageBytes = encoding.GetBytes(message);
            byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);

            return Convert.ToBase64String(hashmessage);
        }
        #endregion
        #region StartInterface
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
        #endregion
        #region EndInterface
        private void EndInterface<T>(T output, InterfaceLogCommand dbIntfCmd, string transactionId, string result, string reason)
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
        #endregion
        #region GetByForm
        public ActionResult GetByForm(string InternetNo = "", string MobileNo = "", string PayMentOrderID = "", string PayMentRecurringChargeVAT = "", string SffPromotionCode = "", string MsgInfo = "", string MsgERROR = "", bool IsSucces = false, string ba = "")
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

            return View("PaymentChannel");
        }
        #endregion
    }
}
