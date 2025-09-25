using System;

namespace WBBEntity.Models
{
    public class FBB_REGISTER_PENDING_PAYMENT
    {
        public string ROW_ID { get; set; }
        public string AIS_NON_MOBILE { get; set; }
        public string REGISTER_TYPE { get; set; }
        public string PAYMENT_STATUS { get; set; }
        public string WEB_PAYMENT_STATUS { get; set; }
        public string PAYMENT_TRANSACTION_ID_IN { get; set; }
        public string PAYMENT_METHOD { get; set; }
        public string CONTACT_MOBILE_PHONE1 { get; set; }
        public string SEND_SMS_ERROR_FLAG { get; set; }
        public string SEND_SMS_SUCCESS_FLAG { get; set; }
        public DateTime CREATED { get; set; }
        public string RETURN_ORDER { get; set; }
    }
}
