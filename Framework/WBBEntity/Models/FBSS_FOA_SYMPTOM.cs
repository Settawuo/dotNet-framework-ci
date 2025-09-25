using System;

namespace WBBEntity.Models
{
    public partial class FBSS_FOA_SYMPTOM
    {
        public string SYMPTOM_CODE { get; set; }

        public string DISPLAY_VALUE { get; set; }

        public string LONG_VALUE { get; set; }

        public string SYMPTOM_GROUP { get; set; }

        public string LANGUAGE_INDEPENDENT_CODE { get; set; }

        public string LANGUAGE_NAME { get; set; }

        public string PARENT_LIC { get; set; }

        public string SYMPTOM_ORDER { get; set; }

        public string ACTIVE { get; set; }

        public string TRANSLATE1 { get; set; }

        public string MULTILINGUAL { get; set; }

        public string DESCRIPTION_CAT { get; set; }

        public string SYMPTOM_REPLICATION { get; set; }

        public string SYMPTOM_LEVEL { get; set; }

        public Nullable<System.DateTime> CREATED_DATE { get; set; }
        public Nullable<System.DateTime> UPDATED_DATE { get; set; }
    }
}
