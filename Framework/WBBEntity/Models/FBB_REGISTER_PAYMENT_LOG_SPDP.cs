using System;

namespace WBBEntity.Models
{
    public partial class FBB_REGISTER_PAYMENT_LOG_SPDP
    {
        public string NON_MOBILE_NO { get; set; }
        public string SERVICE_NAME { get; set; }
        public string ENDPOINT { get; set; }
        public string ORDER_ID { get; set; }
        public string TXN_ID { get; set; }
        public string STATUS { get; set; }
        public string STATUS_CODE { get; set; }
        public string STATUS_MESSAGE { get; set; }
        public string CHANNEL { get; set; }
        public string AMOUNT { get; set; }
        public string REQ_XML_PARAM { get; set; }
        public string RES_XML_PARAM { get; set; }
        public string CREATED_BY { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
        public string UPDATED_BY { get; set; }
        public Nullable<System.DateTime> UPDATED_DATE { get; set; }
        public string ORDER_TRANSACTION_ID { get; set; }
    }
}