namespace WBBEntity.PanelModels.WebServiceModels
{
    public class GetPaymentEnquiryToSuperDuperModel
    {
        public string order_id { get; set; }
        public string merchant_id { get; set; }
        public string txn_id { get; set; }
        public string status { get; set; }
        public string status_code { get; set; }
        public string status_message { get; set; }
        public string amount { get; set; }
        public string amount_net { get; set; }
        public string amount_cust_fee { get; set; }
        public string currency { get; set; }
        public string service_id { get; set; }
        //20.8 delete channel 
        public string channel_type { get; set; }
        public string ref_1 { get; set; }
        public string ref_2 { get; set; }
        public string ref_3 { get; set; }
        public string ref_4 { get; set; }
        public string ref_5 { get; set; }
        public GetPaymentEnquiryToSuperDuperCard card { get; set; }
        public GetPaymentEnquiryToSuperDuperInstallment installment { get; set; }
        public GetPaymentEnquiryToSuperDuperBank bank { get; set; }
        public string sof_txn_id { get; set; }
        public string created_at { get; set; }
        public string success_at { get; set; }

    }

    public class GetPaymentEnquiryToSuperDuperCard
    {
        public string card_holder_name { get; set; }
        public string card_no { get; set; }
        public string card_type { get; set; }
        public string card_expire { get; set; }
        public string card_country { get; set; }
        public string card_ref { get; set; }
    }

    public class GetPaymentEnquiryToSuperDuperInstallment
    {
        public string bank_issuer { get; set; }
        public string term { get; set; }
        public string amount_per_term { get; set; }
    }

    public class GetPaymentEnquiryToSuperDuperBank
    {
        public string account_last_digits { get; set; }
        public string account_name { get; set; }
        public string bank_code { get; set; }
    }
}
