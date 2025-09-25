using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class GetPaymentQRToSuperDuperQuery : IQuery<GetPaymentQRToSuperDuperModel>
    {
        public string Url { get; set; }
        public string ProductName { get; set; }
        public string ServiceName { get; set; }
        public string p_mobile_no { get; set; }
        public string Secret { get; set; }
        public string payment_method_id { get; set; }

        //Header
        public string ContentType { get; set; }
        public string MerchantID { get; set; }
        public string Signature { get; set; }
        public string Nonce { get; set; }

        //Body
        public PaymentQRToSuperDuperBody Body { get; set; }

        public string BodyStr { get; set; }

        public string FullUrl { get; set; }
    }

    public class PaymentQRToSuperDuperBody
    {
        public string order_id { get; set; }
        public string product_name { get; set; }
        public string sof { get; set; }
        public string service_id { get; set; }
        public string terminal_id { get; set; }
        public string location_name { get; set; }
        public string amount { get; set; }
        public string currency { get; set; }
        public string expire_time_seconds { get; set; }
        public string ref_1 { get; set; }
        public string ref_2 { get; set; }
        public string ref_3 { get; set; }
        public string ref_4 { get; set; }
        public string ref_5 { get; set; }
        public MetaData metadata { get; set; }
    }
}
