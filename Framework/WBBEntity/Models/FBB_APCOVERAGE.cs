using System;

namespace WBBEntity.Models
{
    public partial class FBB_APCOVERAGE
    {
        public string BASEL2 { get; set; }
        public string SITENAME { get; set; }
        public string SUB_DISTRICT { get; set; }
        public string DISTRICT { get; set; }
        public string PROVINCE { get; set; }
        public string ZONE { get; set; }
        public string LAT { get; set; }
        public string LNG { get; set; }
        public string ACTIVE_FLAG { get; set; }
        public string CREATED_BY { get; set; }
        public Nullable<System.DateTime> CREATED_DATE { get; set; }
        public string UPDATED_BY { get; set; }
        public Nullable<System.DateTime> UPDATED_DATE { get; set; }
        public decimal APPID { get; set; }

        public string TOWER_TYPE { get; set; }
        public string TOWER_HEIGHT { get; set; }
        public string VLAN { get; set; }
        public string SUBNET_MASK_26 { get; set; }
        public string GATEWAY { get; set; }
        public string AP_COMMENT { get; set; }

        public string COVERAGE_STATUS { get; set; }
        public DateTime? ONTARGET_DATE_EX { get; set; }
        public DateTime? ONTARGET_DATE_IN { get; set; }
    }
}
