using System;


namespace WBBEntity.Models
{
    public partial class FBSS_FIXED_ASSET_SYMPTOM
    {


        public string SYMPTOM_CODE { get; set; }

        public string CATEGORY { get; set; }

        public string SUB_CATEGORY { get; set; }

        public string SYMPTOM_NAME { get; set; }

        public string SR_TT { get; set; }

        public int SLA_HOURS { get; set; }

        public string SEARCH_SPEC_EXPR { get; set; }

        public string SYMPTOM_TYPE { get; set; }

        public string SYMPTOM_GROUP { get; set; }

        public string OWNER { get; set; }

        public string REQUIRE_TYPE { get; set; }

        public string OLD_SN_OLD_TYPE { get; set; }
        public string NEW_SN_OLD_TYPE { get; set; }

        public string NEW_SN_NEW_TYPE { get; set; }


        public Nullable<System.DateTime> CREATE_DATE { get; set; }
        public Nullable<System.DateTime> MODIFY_DATE { get; set; }

    }
}
