using System.Collections.Generic;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class GetPaymentRabbitToSuperDuperQuery : IQuery<GetPaymentRabbitToSuperDuperModel>
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
        public PaymentRabbitToSuperDuperBody Body { get; set; }

        public string BodyStr { get; set; }

        public string FullUrl { get; set; }
    }

    public class PaymentRabbitToSuperDuperBody
    {
        public string amount { get; set; }
        public string currency { get; set; }
        public string order_id { get; set; }
        public bool is_pre_approve { get; set; }
        public string service_id { get; set; }
        public string channel_type { get; set; }
        public string ref_1 { get; set; }
        public string ref_2 { get; set; }
        public string ref_3 { get; set; }
        public string ref_4 { get; set; }
        public string ref_5 { get; set; }
        public MetaDataRabbit metadata { get; set; }
        public List<Packages> packages { get; set; }
        public RedirectUrls redirect_urls { get; set; }
    }

    public class CustomerMobileNoRabbit
    {
        public string billing_account { get; set; }
        public string mobile_no { get; set; }
        public string amount { get; set; }
    }

    public class MetaDataRabbit
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
        public CustomerMobileNoRabbit[] customer_mobile_no { get; set; }
        public string batch_no { get; set; }
        public string ref_1 { get; set; }
        public string ref_2 { get; set; }
    }

    public class RedirectUrls
    {
        public string confirm_url { get; set; }
        public string cancel_url { get; set; }
    }
}
