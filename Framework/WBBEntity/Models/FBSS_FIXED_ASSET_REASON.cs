using System;

namespace WBBEntity.Models
{
    public partial class FBSS_FIXED_ASSET_REASON
    {
        public string REASON_CODE { get; set; }

        public string DISPLAY_VALUE { get; set; }

        public string LONG_VALUE { get; set; }

        public string SYMPTOM_GROUP { get; set; }

        public string MOVEMENT_OLD { get; set; }

        public string MOVEMENT_NEW { get; set; }

        public string STATUS_OLD { get; set; }

        public string STATUS_NEW { get; set; }

        public string ACTIVE { get; set; }

        public string DESCRIPTION_CAT { get; set; }

        public string SYMPTOM_REPLICATION { get; set; }

        public string SYMPTOM_LEVEL { get; set; }

        public Nullable<System.DateTime> CREATE_DATE { get; set; }
        public Nullable<System.DateTime> MODIFY_DATE { get; set; }

        public string REASON_GROUP { get; set; }
    }
}
