using System;

namespace WBBEntity.Models
{
    public class FBB_PACKAGE_TRAN
    {
        public string ROW_ID { get; set; }
        public System.DateTime CREATED { get; set; }
        public string CREATED_BY { get; set; }
        public System.DateTime LAST_UPD { get; set; }
        public string LAST_UPD_BY { get; set; }
        public string CUST_ROW_ID { get; set; }
        public string PACKAGE_CODE { get; set; }
        public string PACKAGE_CLASS { get; set; }
        public string PACKAGE_GROUP { get; set; }
        public string PRODUCT_TYPE { get; set; }
        public string PRODUCT_SUBTYPE { get; set; }
        public string TECHNOLOGY { get; set; }
        public string PACKAGE_NAME { get; set; }
        public Nullable<decimal> RECURRING_CHARGE { get; set; }
        public Nullable<decimal> INITIATION_CHARGE { get; set; }
        public Nullable<decimal> DISCOUNT_INITIATION { get; set; }
        public string PACKAGE_BILL_THA { get; set; }
        public string PACKAGE_BILL_ENG { get; set; }
        public string DOWNLOAD_SPEED { get; set; }
        public string UPLOAD_SPEED { get; set; }
        public string OWNER_PRODUCT { get; set; }
    }
}
