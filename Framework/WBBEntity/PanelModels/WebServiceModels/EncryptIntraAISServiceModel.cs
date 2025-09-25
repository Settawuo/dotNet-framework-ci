using System.Collections.Generic;

namespace WBBEntity.PanelModels.WebServiceModels
{
    public class EncryptIntraAISServiceModel
    {
        public string resultcode { get; set; }
        public string errmsg { get; set; }
        public string output { get; set; }
    }

    public class PaymentChannelModel
    {
        public string data { get; set; }
        public string product_name { get; set; }
        public string fbb_id { get; set; }
        public string payment_transaction_id { get; set; }
        public List<string> list_payment_method { get; set; }
        public string register_channel { get; set; }

    }
}
