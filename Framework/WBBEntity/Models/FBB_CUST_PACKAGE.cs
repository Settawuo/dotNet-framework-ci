using System;

namespace WBBEntity.Models
{
    public partial class FBB_CUST_PACKAGE
    {
        public string CUST_NON_MOBILE { get; set; }
        public string PACKAGE_CODE { get; set; }
        public string PACKAGE_CLASS { get; set; }
        public string PACKAGE_TYPE { get; set; }
        public string PACKAGE_GROUP { get; set; }
        public string PACKAGE_SUBTYPE { get; set; }
        public string PACKAGE_OWNER { get; set; }
        public string TECHNOLOGY { get; set; }
        public string PACKAGE_STATUS { get; set; }
        public string PACKAGE_NAME { get; set; }
        public decimal? RECURRING_CHARGE { get; set; }
        public decimal? RECURRING_DISCOUNT { get; set; }
        public DateTime? RECURRING_DISCOUNT_EXP { get; set; }
        public DateTime? RECURRING_START_DT { get; set; }
        public DateTime? RECURRING_END_DT { get; set; }
        public decimal? INITIATION_CHARGE { get; set; }
        public decimal? INITIATION_DISCOUNT { get; set; }
        public string PACKAGE_BILL_THA { get; set; }
        public string DOWNLOAD_SPEED { get; set; }
        public string UPLOAD_SPEED { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string UPDATED_BY { get; set; }
        public DateTime UPDATED_DATE { get; set; }
        public string PRE_RECURRING_CHARGE { get; set; }
        public string PRE_INITIATION_CHARGE { get; set; }
        public string HOME_IP { get; set; }
        public string HOME_PORT { get; set; }
        public string IDD_FLAG { get; set; }
        public string FAX_FLAG { get; set; }

    }
}
