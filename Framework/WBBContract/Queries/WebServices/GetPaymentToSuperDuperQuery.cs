using Newtonsoft.Json;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class GetPaymentToSuperDuperQuery : IQuery<GetPaymentToSuperDuperModel>
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
        public PaymentToSuperDuperBody Body { get; set; }

        public string BodyStr { get; set; }

        public string FullUrl { get; set; }
    }

    public class PaymentToSuperDuperBody
    {
        public string order_id { get; set; }
        public string product_name { get; set; }
        public string service_id { get; set; }
        public string channel_type { get; set; }
        public string cust_id { get; set; }
        public string amount { get; set; }
        public string currency { get; set; }
        public string ref_1 { get; set; }
        public string ref_2 { get; set; }
        public string ref_3 { get; set; }
        public string ref_4 { get; set; }
        public string ref_5 { get; set; }
        public MetaData metadata { get; set; }
        public string form_type { get; set; }
        public bool is_remember { get; set; }
        public CardData card { get; set; }

        [JsonProperty("3ds")]
        public EdsData Eds { get; set; }
    }

    public class MetaData
    {
        public string bank_code { get; set; }
        public string company_account_no { get; set; }
        public string company_account_name { get; set; }
        public string service_id { get; set; }
        public string transaction_code { get; set; }
        public string billing_system { get; set; }
        public string merchant_type { get; set; }
        public string billing_account { get; set; }
        public string master_mobile_no { get; set; }
        public CustomerMobileNo[] customer_mobile_no { get; set; }
        public string batch_no { get; set; }


    }

    public class CustomerMobileNo
    {
        public string billing_account { get; set; }
        public string mobile_no { get; set; }
        public string amount { get; set; }
    }

    public class CardData
    {
        public string card_ref { get; set; }
        public string card_cvv { get; set; }
    }

    public class EdsData
    {
        [JsonProperty("3ds_required")]
        public bool Eds_required { get; set; }

        [JsonProperty("3ds_url_success")]
        public string Eds_url_success { get; set; }

        [JsonProperty("3ds_url_fail")]
        public string Eds_url_fail { get; set; }
    }
}
