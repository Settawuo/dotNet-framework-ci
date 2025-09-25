using System;

namespace AIRNETEntity.Models
{
    public partial class AIR_PACKAGE_USER_GROUP
    {
        public string PACKAGE_CODE { get; set; }
        public string LOCATION_CODE { get; set; }
        public string ASC_CODE { get; set; }
        public string USER_GROUP { get; set; }
        public DateTime? EFFECTIVE_DTM { get; set; }
        public DateTime? EXPIRE_DTM { get; set; }
        public DateTime? UPD_DTM { get; set; }
        public string UPD_BY { get; set; }
    }
}
