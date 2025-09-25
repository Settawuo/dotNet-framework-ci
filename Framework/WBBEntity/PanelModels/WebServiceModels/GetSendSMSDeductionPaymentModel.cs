using System.Collections.Generic;

namespace WBBEntity.PanelModels.WebServiceModels
{
    public class GetSendSMSDeductionPaymentModel
    {
        public string ret_code { get; set; }
        public string ret_message { get; set; }
        public List<OrderSendSMSPayment> list_order_send_sms_payment { get; set; }
    }

    public class OrderSendSMSPayment
    {
        public string endpoint { get; set; }
        public string ssid { get; set; }
        public string command { get; set; }
        public string input { get; set; }
        public string receipt_url { get; set; }
        public string message_th { get; set; }
        public string message_en { get; set; }
        public string message_eReceipt_th { get; set; }
        public string message_eReceipt_en { get; set; }
    }

}
