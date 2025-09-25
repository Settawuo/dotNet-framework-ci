namespace WBBEntity.PanelModels.WebServiceModels
{
    public class GetPaymentInquiryToSuperDuperModel
    {
        public string status { get; set; }
        public string api_ref_id { get; set; }
        public string status_message { get; set; }
        public string status_code { get; set; }
        public GetPaymentInquiryToSuperDuperData data { get; set; }
    }

    public class GetPaymentInquiryToSuperDuperData
    {
        public string card_ref { get; set; }
        public string service_id { get; set; }
        public string cust_id { get; set; }
        public string ba { get; set; }
        public string status { get; set; }
        public string channel_name { get; set; }
        public string create_at { get; set; }
    }
}
