using System;

namespace WBBEntity.Models
{
    public partial class FBB_AP_INFO
    {
        public decimal AP_ID { get; set; }
        public string AP_NAME { get; set; }
        public string SECTOR { get; set; }
        public decimal SITE_ID { get; set; }
        public string ACTIVE_FLAG { get; set; }
        public string CREATED_BY { get; set; }
        public Nullable<System.DateTime> CREATED_DATE { get; set; }
        public string UPDATED_BY { get; set; }
        public Nullable<System.DateTime> UPDATED_DATE { get; set; }
        public string IP_ADDRESS { get; set; }
        public string STATUS { get; set; }
        public string IMPLEMENT_PHASE { get; set; }
        public string PO_NUMBER { get; set; }
        public string AP_COMPANY { get; set; }
        public string AP_LOT { get; set; }
        public Nullable<System.DateTime> IMPLEMENT_DATE { get; set; }
        public Nullable<System.DateTime> ON_SERVICE_DATE { get; set; }
    }
}
