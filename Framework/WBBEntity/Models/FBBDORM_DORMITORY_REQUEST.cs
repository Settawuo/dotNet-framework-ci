using System;

namespace WBBEntity.Models
{
    public partial class FBBDORM_DORMITORY_REQUEST
    {
        public string ROW_ID { get; set; }
        public Nullable<System.DateTime> CREATED_DT { get; set; }
        public string CREATE_BY { get; set; }
        public Nullable<System.DateTime> LAST_UPDATE_DT { get; set; }
        public string LAST_UPDATE_BY { get; set; }
        public string TYPE_CUSTOMER_REQUEST { get; set; }
        public string CUSTOMER_FIRST_NAME { get; set; }
        public string CUSTOMER_LAST_NAME { get; set; }
        public string CONTRACT_PHONE { get; set; }
        public string DORMITORY_NAME { get; set; }
        public string TYPE_DORMITORY { get; set; }
        public string HOME_NO { get; set; }
        public string MOO { get; set; }
        public string SOI { get; set; }
        public string STREET { get; set; }
        public string ZIPCODE_ROW_ID { get; set; }
        public string A_BUILDING { get; set; }
        public string A_UNIT { get; set; }
        public string A_LIVING { get; set; }
        public string PHONE_CABLE { get; set; }
        public string PROBLEM_INTERNET { get; set; }
        public string A_UNIT_USE_INTERNET { get; set; }
        public string OLD_SYSTEM { get; set; }
        public string OLD_VENDOR_SERVICE { get; set; }
        public string FLAG_LANGUAGE { get; set; }
        public string FLAG_SEND_TO_SALE { get; set; }
        public Nullable<System.DateTime> SEND_TO_SALE_DT { get; set; }
        public string USER_APPROVE { get; set; }
        public Nullable<System.DateTime> USER_APPROVE_DT { get; set; }
        public string PROCESS_STATUS { get; set; }
        public string DORMITORY_MASTER_ID { get; set; }
        public string REMARK { get; set; }
    }
}
