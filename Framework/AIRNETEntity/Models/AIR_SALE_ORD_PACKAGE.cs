using System;

namespace AIRNETEntity.Models
{
    public partial class AIR_SALE_ORD_PACKAGE
    {


        public string ORDER_NO { get; set; }
        public string PRODUCT_TYPE { get; set; }
        public string PRODUCT_SUBTYPE { get; set; }
        public string IA_NO { get; set; }
        public string PACKAGE_CODE { get; set; }
        public Nullable<DateTime> EFFECTIVE_DATE { get; set; }
        public Nullable<DateTime> EXPIRE_DATE { get; set; }
        public Nullable<decimal> QUANTITY { get; set; }
        public Nullable<decimal> PARTIAL_MONTH { get; set; }
        public Nullable<decimal> PARTIAL_MNY { get; set; }
        public string PARTIAL_INVOICE { get; set; }
        public string OLD_PACKAGE_CODE { get; set; }
        public Nullable<decimal> OLD_PARTIAL_MNY { get; set; }
        public Nullable<DateTime> UPD_DTM { get; set; }
        public string UPD_BY { get; set; }
        public string PAYMENT_TYPE { get; set; }
        public string PACKAGE_GROUP { get; set; }

    }
}
