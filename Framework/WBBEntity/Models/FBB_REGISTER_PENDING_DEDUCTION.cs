using System;

namespace WBBEntity.Models
{
    public partial class FBB_REGISTER_PENDING_DEDUCTION
    {
        public string TRANSACTION_ID { get; set; }
        public string NON_MOBILE_NO { get; set; }
        public string BA_NO { get; set; }
        public string PAID_AMT { get; set; }
        public string CHANNEL { get; set; }
        public string MERCHANT_ID { get; set; }
        public string DEDUCTION_STATUS { get; set; }
        public string PAYMENT_STATUS { get; set; }
        public string SEND_SMS_FLAG { get; set; }
        public string PAYMENT_METHOD_ID { get; set; }
        public string RECEIPT_NO { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string UPDATED_BY { get; set; }
        public DateTime UPDATED_DATE { get; set; }
    }
}
