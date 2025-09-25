using System;

namespace WBBEntity.Models
{
    public partial class FBB_COMPONENT_PERMISSION
    {
        public decimal COMPONENT_PERMISSION_ID { get; set; }
        public decimal? COMPONENT_ID { get; set; }
        public decimal GROUP_ID { get; set; }
        public string ENABLE_FLG { get; set; }
        public string ACTIVE_FLG { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public string UPDATED_BY { get; set; }
        public DateTime? UPDATED_DATE { get; set; }
        public string READ_ONLY_FLG { get; set; }
    }
}
