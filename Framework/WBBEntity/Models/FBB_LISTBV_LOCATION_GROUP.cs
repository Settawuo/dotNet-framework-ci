using System;

namespace WBBEntity.Models
{
    public partial class FBB_LISTBV_LOCATION_GROUP
    {
        public decimal LOCATION_GROUP_ID { get; set; }
        public string LOCATION_CODE { get; set; }
        public string LOCATION_NAME { get; set; }
        public string SPECIFIC_FLAG { get; set; }
        public string ACTIVE_FLAG { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public string UPDATED_BY { get; set; }
        public DateTime? UPDATED_DATE { get; set; }
    }
}
