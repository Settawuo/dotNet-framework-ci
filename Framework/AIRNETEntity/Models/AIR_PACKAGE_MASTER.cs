using System;

namespace AIRNETEntity.Models
{
    public partial class AIR_PACKAGE_MASTER
    {

        public string PACKAGE_CODE { get; set; }
        public string PACKAGE_TYPE { get; set; }
        public string PACKAGE_CLASS { get; set; }
        public DateTime SALE_START_DATE { get; set; }
        public DateTime? SALE_END_DATE { get; set; }
        public string RATE_PLAN_ID { get; set; }
        public decimal INITIATION_CHARGE { get; set; }
        public int INC_REVENUE_CODE { get; set; }
        public decimal RECURRING_CHARGE { get; set; }
        public int RCC_REVENUE_CODE { get; set; }
        public decimal TERMINATION_CHARGE { get; set; }
        public int TMC_REVENUE_CODE { get; set; }
        public decimal SUSPENSION_CHARGE { get; set; }
        public int SSC_REVENUE_CODE { get; set; }
        public decimal SUSPEND_RECURRING_CHARGE { get; set; }
        public int SRC_REVENUE_CODE { get; set; }
        public decimal REACTIVATION_CHARGE { get; set; }
        public int RAC_REVENUE_CODE { get; set; }
        public DateTime UPD_DTM { get; set; }
        public string UPD_BY { get; set; }
        public string PACKAGE_NAME_THA { get; set; }
        public string PACKAGE_NAME_ENG { get; set; }
        public int DURATION_MONTH { get; set; }
        public string PRORATE_FLAG { get; set; }
        public string AGGREGATE_LEVEL { get; set; }
        public string PACKAGE_TARIFF_TYPE { get; set; }
        public string PRODUCT_TYPE { get; set; }
        public string PRODUCT_SUBTYPE { get; set; }
        public decimal? PRIORITY_NUM { get; set; }
        public string SPEED { get; set; }
        public decimal? MINIMUM_CHARGE { get; set; }
        public string PRODUCT_SUBTYPE2 { get; set; }
        public string SFF_PROMOTION_CODE { get; set; }
        public string SFF_PROMOTION_NAME_THA { get; set; }
        public string SFF_PROMOTION_NAME_ENG { get; set; }
        public string SFF_PROMOTION_BILL_THA { get; set; }
        public string SFF_PROMOTION_BILL_ENG { get; set; }
        public string TECHNOLOGY { get; set; }
        public string DOWNLOAD_SPEED { get; set; }
        public string UPLOAD_SPEED { get; set; }
        public decimal? DISCOUNT_INITIATION_CHARGE { get; set; }
        public string OWNER_PRODUCT { get; set; }
    }
}
