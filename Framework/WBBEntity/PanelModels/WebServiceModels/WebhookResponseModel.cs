namespace WBBEntity.PanelModels.WebServiceModels
{
    public class WebhookResponseModel
    {
        public string order_id { get; set; }
        public string merchant_id { get; set; }
        public string txn_id { get; set; }
        public string status { get; set; }
        public string status_code { get; set; }
        public string status_message { get; set; }
        public decimal amount { get; set; }
        public decimal amount_net { get; set; }
        public decimal amount_cust_fee { get; set; }
        public string currency { get; set; }
        public string service_id { get; set; }
        public string channel_type { get; set; }
        public string ref_1 { get; set; }
        public string ref_2 { get; set; }
        public string ref_3 { get; set; }
        public string ref_4 { get; set; }
        public string ref_5 { get; set; }
        public WebhookResponseModelCard card { get; set; }
        public WebhookResponseModelInstallment installment { get; set; }
        public WebhookResponseModelBank bank { get; set; }
        public WebhookResponseModelRlp rlp { get; set; }
        public string sof_txn_id { get; set; }
        public string created_at { get; set; }
        public string success_at { get; set; }
        public string Header_Nonce { get; set; }
        public string Header_MerchantID { get; set; }
        public string Header_Signature { get; set; }
        public string Json_RawData { get; set; }

    }

    public class WebhookResponseModelCard
    {
        public string card_holder_name { get; set; }
        public string card_no { get; set; }
        public string card_type { get; set; }
        public string card_expire { get; set; }
        public string card_country { get; set; }
        public string card_ref { get; set; }
    }

    public class WebhookResponseModelInstallment
    {
        public string bank_issuer { get; set; }
        public int term { get; set; }
        public string amount_per_term { get; set; }
    }

    public class WebhookResponseModelBank
    {
        public string account_last_digits { get; set; }
        public string account_name { get; set; }
        public string bank_code { get; set; }
    }

    public class WebhookResponseModelRlp
    {
        public string pay_type { get; set; }
        public string token_id { get; set; }
        public PayInfo[] pay_info { get; set; }
        public Packages[] packages { get; set; }
    }

    public class PayInfo
    {
        public string method { get; set; }
        public decimal amount { get; set; }
        public string credit_card_nickname { get; set; }
        public string credit_card_brand { get; set; }
    }

    public class Packages
    {
        public string id { get; set; }
        public decimal amount { get; set; }
        public Products[] products { get; set; }
    }

    public class Products
    {
        public string id { get; set; }
        public string name { get; set; }
        public string image_url { get; set; }
        public int quantity { get; set; }
        public decimal price { get; set; }
    }
}
