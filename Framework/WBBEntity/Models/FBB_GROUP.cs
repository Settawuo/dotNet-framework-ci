using System;

namespace WBBEntity.Models
{
    public partial class FBB_GROUP
    {
        public decimal GROUP_ID { get; set; }
        public string GROUP_NAME { get; set; }
        public string GROUP_DESC { get; set; }
        public string ACTIVE_FLAG { get; set; }
        public string CREATED_BY { get; set; }
        public Nullable<System.DateTime> CREATED_DATE { get; set; }
        public string UPDATED_BY { get; set; }
        public Nullable<System.DateTime> UPDATED_DATE { get; set; }
    }
}
