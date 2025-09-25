using System;

namespace WBBEntity.Models
{
    public partial class FBB_ORD_CHANGE_PACKAGE
    {
        public string ORDER_NO { get; set; }
        public string NON_MOBILE_NO { get; set; }
        public string RELATE_MOBILE { get; set; }
        public string SFF_PROMOTION_CODE { get; set; }
        public string ACTION_STATUS { get; set; }
        public string PACKAGE_STATE { get; set; }
        public string PROJECT_NAME { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public string UPDATED_BY { get; set; }
        public DateTime? UPDATED_DATE { get; set; }
        public string PRODUCT_SEQ { get; set; }
        public string BUNDLING_ACTION { get; set; }
        public string OLD_RELATE_MOBILE { get; set; }
        public string MOBILE_CONTACT { get; set; }
    }
}
