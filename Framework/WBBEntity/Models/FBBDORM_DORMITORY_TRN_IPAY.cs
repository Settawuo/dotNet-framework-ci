namespace WBBEntity.Models
{
    public class FBBDORM_DORMITORY_TRN_IPAY
    {
        public string RESPONSE_CODE { get; set; }
        public string RESPONSE_MES { get; set; }
        public int CHANNEL_ID { get; set; }
        public string CHANNEL_NAME { get; set; }
        public string CHANNEL_REF_ID { get; set; }
        public string COMMAND { get; set; }
        public string MSISDN { get; set; }
        public string AIS_REF_DATE { get; set; }
        public string AIS_REF_NO { get; set; }
        public string BANK_REF_NO { get; set; }
        public string PAY_BANK_FLAG { get; set; }
        public string ISSUER_BANK_CODE { get; set; }
        public string ACQUIRER_BANK_CODE { get; set; }
        public long CARD_NO { get; set; }
        public string CARD_TYPE { get; set; }
        public string ISSUER_BANK_NAME { get; set; }
        public string SHOP_ID { get; set; }
        public int PAY_TERM { get; set; }
        public long INSTALLMENT_RATE { get; set; }
        public long AMOUNT_PER_MONTH { get; set; }
        public long TOTAL_AMOUNT { get; set; }
        public string BANK_REPONSE_CODE { get; set; }
        public string SESSION_ID { get; set; }
        public decimal BANK_RETURN_AMOUNT { get; set; }
        public decimal IPAY_RETURN_AMOUNT { get; set; }
        public string BOS_RESULT_CODE { get; set; }
    }
}
