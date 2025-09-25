using System;

namespace WBBEntity.Models
{
    public partial class FBB_LISTBV_BY_LOCATION
    {
        public decimal LISTBV_LOC_ID { get; set; }
        public string LOCATION_CODE { get; set; }
        public string ADDRESS_ID { get; set; }
        public string ACTIVE_FLAG { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public string UPDATED_BY { get; set; }
        public DateTime? UPDATED_DATE { get; set; }
    }
}
