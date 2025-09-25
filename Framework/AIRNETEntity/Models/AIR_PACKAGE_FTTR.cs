using System;

namespace AIRNETEntity.Models
{
    public partial class AIR_PACKAGE_FTTR
    {
        public string ADDRESS_ID { get; set; }
        public string SFF_PROMOTION_CODE { get; set; }
        public string SFF_PROMOTION_BILL_THA { get; set; }
        public string SFF_PROMOTION_BILL_ENG { get; set; }
        public string PRODUCT_SUBTYPE { get; set; }
        public string OWNER_PRODUCT { get; set; }
        public string CUSTOMER_TYPE { get; set; }
        public DateTime? EFFECTIVE_DTM { get; set; }
        public DateTime? EXPIRE_DTM { get; set; }
        public DateTime? UPD_DTM { get; set; }
        public string UPD_BY { get; set; }
    }
}
