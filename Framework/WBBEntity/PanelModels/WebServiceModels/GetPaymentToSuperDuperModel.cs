using System.Xml.Serialization;

namespace WBBEntity.PanelModels.WebServiceModels
{
    public class GetPaymentToSuperDuperModel
    {
        public string order_id { get; set; }
        public string txn_id { get; set; }
        public string txn_token { get; set; }
        public string status { get; set; }
        public string status_code { get; set; }
        public string status_message { get; set; }
        public string create_at { get; set; }
        public string currency { get; set; }
        public string cust_id { get; set; }
        public string amount { get; set; }
        public string amount_net { get; set; }
        public string amount_cust_fee { get; set; }
        public string form_url { get; set; }

        [XmlElement("3ds")]
        public EdsDataModel Eds { get; set; }
    }

    public class EdsDataModel
    {
        [XmlElement("3ds_required")]
        public string Eds_required { get; set; }
    }
}
