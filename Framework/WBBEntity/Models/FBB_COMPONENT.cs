using System;

namespace WBBEntity.Models
{
    public partial class FBB_COMPONENT
    {
        public decimal COMPONENT_ID { get; set; }
        public string PROGRAM_ID { get; set; }
        public string COMPONENT_NAME { get; set; }
        public string ACTIVE_FLG { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public string UPDATED_BY { get; set; }
        public DateTime? UPDATED_DATE { get; set; }
        public string COMPONENT_TYPE { get; set; }
    }
}
