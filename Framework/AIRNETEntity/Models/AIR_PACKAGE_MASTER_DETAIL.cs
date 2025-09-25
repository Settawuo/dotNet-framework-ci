using System;

namespace AIRNETEntity.Models
{
    public class AIR_PACKAGE_MASTER_DETAIL
    {
        public string PACKAGE_CODE { get; set; }
        public string PRODUCT_TYPE { get; set; }
        public string PRODUCT_SUBTYPE { get; set; }
        public string PRODUCT_SUBTYPE2 { get; set; }
        public string PRODUCT_SUBTYPE3 { get; set; }
        public string OWNER_PRODUCT { get; set; }
        public string TECHNOLOGY { get; set; }
        public string PACKAGE_GROUP { get; set; }
        public string NETWORK_TYPE { get; set; }
        public int SERVICE_DAY_START { get; set; }
        public int SERVICE_DAY_END { get; set; }
        public DateTime UPD_DTM { get; set; }
        public string UPD_BY { get; set; }
        public string SERENADE_FLAG { get; set; }
        public string SERENADE_PACKAGE_GROUP { get; set; }
        public string NON_SERENADE_PACKAGE_GROUP { get; set; }
        public string PLUG_AND_PLAY_FLAG { get; set; }
        public string CUSTOMER_TYPE { get; set; }
    }
}
