using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WBBContract;
using WBBContract.Commands;
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
    public class GenQRCodeController : WBBController
    {
        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<SavePaymentLogCommand> _savePaymentLogCommand;

        public GenQRCodeController(IQueryProcessor queryProcessor,
            ICommandHandler<SavePaymentLogCommand> savePaymentLogCommand)
        {
            _queryProcessor = queryProcessor;
            _savePaymentLogCommand = savePaymentLogCommand;
        }

        [HttpPost]
        public ActionResult Index(string InternetNo = "")
        {
            string PayMentOrderID = "";
            string PayMentRecurringChargeVAT = "";
            // getData By InternetNO

            var InputGenQRCodeData = GetValueQrCode(InternetNo);
            if (InputGenQRCodeData != null && InputGenQRCodeData.RespCode != "-1")
            {
                PayMentOrderID = InputGenQRCodeData.PaymentOrderID.ToSafeString();
                PayMentRecurringChargeVAT = InputGenQRCodeData.PurchaseAmt.ToSafeString();
            }

            var controller = DependencyResolver.Current.GetService<ProcessController>();
            ViewBag.LabelFBBOR041 = controller.GetScreenConfig("FBBOR041");
            ViewBag.PayMentOrderID = PayMentOrderID;
            ViewBag.PayMentRecurringChargeVAT = PayMentRecurringChargeVAT;

            return View();
        }

        [HttpPost]
        public JsonResult GetPaymentToMerchantQrCode(string payMentOrderId, string payMentRecurringChargeVat, string payFromFOA = "")
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

        private GetValueQrCodeModel GetValueQrCode(string InternetNo)
        {
            GetValueQrCodeQuery query = new GetValueQrCodeQuery
            {
                InternetNo = InternetNo
            };

            GetValueQrCodeModel result = _queryProcessor.Execute(query);

            return result;
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

    }
}
