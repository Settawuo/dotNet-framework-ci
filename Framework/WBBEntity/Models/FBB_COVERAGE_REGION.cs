using System;
using System.ComponentModel;

namespace WBBEntity.Models
{
    public class FBB_COVERAGE_REGION
    {
        public decimal FTTX_ID { get; set; }
        public string GROUP_AMPHUR { get; set; }
        [DisplayName("Owner_Product")]
        public string OWNER_PRODUCT { get; set; }
        [DisplayName("Owner Type")]
        public string OWNER_TYPE { get; set; }
        public string ACTIVEFLAG { get; set; }

        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string UPDATED_BY { get; set; }
        [DisplayName("Tower Thai")]
        public string TOWER_TH { get; set; }
        [DisplayName("Tower English")]
        public string TOWER_EN { get; set; }
        [DisplayName("Service Type")]
        public string SERVICE_TYPE { get; set; }
        public DateTime? UPDATED_DATE { get; set; }
        ///new
        public DateTime? ONTARGET_DATE_IN { get; set; }
        public DateTime? ONTARGET_DATE_EX { get; set; }
        public string COVERAGE_STATUS { get; set; }

        public string LATITUDE { get; set; }
        public string LONGITUDE { get; set; }

        public string ZIPCODE_ROWID_TH { get; set; }
        public string ZIPCODE_ROWID_EN { get; set; }

    }
}
