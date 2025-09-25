using System;

namespace WBBEntity.Models
{
    public class FBB_HISTORY_LOG
    {
        public decimal HISTORY_ID { get; set; }
        public string DESCRIPTION { get; set; }
        public string ACTION { get; set; }
        public string APPLICATION { get; set; }
        public string REF_NAME { get; set; }
        public string REF_KEY { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
    }

    public enum ActionHistory
    {
        NONE = 0,
        ADD = 1,
        UPDATE = 2,
        DELETE = 3
    }
}
