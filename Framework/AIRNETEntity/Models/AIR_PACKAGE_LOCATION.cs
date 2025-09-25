using System;

namespace AIRNETEntity.Models
{
    public class AIR_PACKAGE_LOCATION
    {
        public string PACKAGE_CODE { get; set; }
        public string REGION { get; set; }
        public string PROVINCE { get; set; }
        public string DISTRICT { get; set; }
        public string SUB_DISTRICT { get; set; }
        public string ADDRESS_TYPE { get; set; }
        public string BUILDING_NAME { get; set; }
        public string BUILDING_NO { get; set; }
        public Nullable<DateTime> EFFECTIVE_DTM { get; set; }
        public Nullable<DateTime> EXPIRE_DTM { get; set; }
        public DateTime UPD_DTM { get; set; }
        public string UPD_BY { get; set; }
        public string ADDRESS_ID { get; set; }
    }
}
