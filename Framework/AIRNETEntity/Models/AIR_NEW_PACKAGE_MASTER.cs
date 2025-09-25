using System;

namespace AIRNETEntity.Models
{
    public class AIR_NEW_PACKAGE_MASTER
    {
        public string PACKAGE_CODE { get; set; }
        public string PACKAGE_TYPE { get; set; }
        public string PACKAGE_CLASS { get; set; }
        public Nullable<DateTime> SALE_START_DATE { get; set; }
        public Nullable<DateTime> SALE_END_DATE { get; set; }
        public int DURATION_MONTH { get; set; }
        public decimal PRE_INITIATION_CHARGE { get; set; }
        public decimal INITIATION_CHARGE { get; set; }
        public decimal PRE_RECURRING_CHARGE { get; set; }
        public decimal RECURRING_CHARGE { get; set; }
        public decimal TERMINATION_CHARGE { get; set; }
        public decimal SUSPENSION_CHARGE { get; set; }
        public decimal SUSPEND_RECURRING_CHARGE { get; set; }
        public decimal REACTIVATION_CHARGE { get; set; }
        public string PACKAGE_NAME_THA { get; set; }
        public string PACKAGE_NAME_ENG { get; set; }
        public string SFF_PROMOTION_CODE { get; set; }
        public string SFF_PROMOTION_BILL_THA { get; set; }
        public string SFF_PROMOTION_BILL_ENG { get; set; }
        public string DOWNLOAD_SPEED { get; set; }
        public string UPLOAD_SPEED { get; set; }
        public string DISCOUNT_TYPE { get; set; }
        public decimal DISCOUNT_VALUE { get; set; }
        public int DISCOUNT_DAY { get; set; }
        public string VAS_SERVICE { get; set; }
        public DateTime UPD_DTM { get; set; }
        public string UPD_BY { get; set; }
        public DateTime CHANGE_START_DATE { get; set; }
        public DateTime CHANGE_END_DATE { get; set; }
        public string NO_PLAY_BOX_FLAG { get; set; }
    }
}
