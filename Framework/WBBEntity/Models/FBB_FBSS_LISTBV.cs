using System;

namespace WBBEntity.Models
{
    public partial class FBB_FBSS_LISTBV
    {
        public decimal LISTBV_ID { get; set; }
        public string LANGUAGE { get; set; }
        public string ADDRESS_TYPE { get; set; }
        public string POSTAL_CODE { get; set; }
        public string ADDRESS_ID { get; set; }
        public string BUILDING_NAME { get; set; }
        public string BUILDING_NO { get; set; }
        public string ACTIVE_FLAG { get; set; }
        public string SUB_DISTRICT { get; set; }
        public string SITE_CODE { get; set; }
        public string ACCESS_MODE { get; set; }
        public string PARTNER { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public string UPDATED_BY { get; set; }
        public DateTime? UPDATED_DATE { get; set; }
        public string LATITUDE { get; set; }
        public string LONGTITUDE { get; set; }
        public string FTTR_FLAG { get; set; }
        public string SPECIFIC_TEAM_1 { get; set; }
        public string SPECIFIC_TEAM_2 { get; set; }
        public string FTTR_TYPE { get; set; }
        public string REASON { get; set; }
    }
}
