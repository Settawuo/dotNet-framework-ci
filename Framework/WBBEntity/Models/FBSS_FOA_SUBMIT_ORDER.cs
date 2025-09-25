using System;

namespace WBBEntity.Models
{
    public partial class FBSS_FOA_SUBMIT_ORDER
    {
        public string ACCESS_NUMBER { get; set; }
        public string ORDER_NO { get; set; }
        public string ORDER_TYPE { get; set; }
        public string FLAG_TYPE { get; set; }
        public string SUBCONTRACT_CODE { get; set; }
        public string SUBCONTRACT_NAME { get; set; }
        public string PRODUCT_NAME { get; set; }
        public string SERVICE_LIST { get; set; }
        public string SUBMIT_FLAG { get; set; }
        public string REJECT_REASON { get; set; }
        public string OLT_NAME { get; set; }
        public string BUILDING_NAME { get; set; }
        public string MOBILE_CONTACT { get; set; }
        public Nullable<System.DateTime> FOA_SUBMIT_DATE { get; set; }
        public Nullable<System.DateTime> MODIFY_DATE { get; set; }
        public decimal? INSTALLATION_COST { get; set; }
        public string ADDRESS_ID { get; set; }
        public string ORG_ID { get; set; }
        public string REUSE_FLAG { get; set; }
        public string EVENT_FLOW_FLAG { get; set; }
        public string SUBCONTRACT_TYPE { get; set; }
        public string SUBCONTRACT_SUB_TYPE { get; set; }
        public string REQUEST_SUB_FLAG { get; set; }
        public string SUB_ACCESS_MODE { get; set; }
        public string ORDER_NO_SFF { get; set; }

        // 3BB Integration Track OSS Multi Company
        public string PRODUCT_OWNER { get; set; }
        public string MAIN_PROMO_CODE { get; set; }
        public string TEAM_ID { get; set; }
    }
}
