using System;

namespace WBBEntity.Models
{
    public partial class FBB_EVENT_SUBCONTRACT
    {
        public string EVENT_CODE { get; set; }
        public string SUB_LOCATION_ID { get; set; }
        public string SUB_CONTRACT_NAME { get; set; }
        public string SUB_TEAM_ID { get; set; }
        public string SUB_TEAM_NAME { get; set; }
        public string INSTALL_STAFF_ID { get; set; }
        public string INSTALL_STAFF_NAME { get; set; }
        public Nullable<System.DateTime> EVENT_START_DATE { get; set; }
        public Nullable<System.DateTime> EVENT_END_DATE { get; set; }
        public string CREATED_BY { get; set; }
        public Nullable<System.DateTime> CREATED_DATE { get; set; }
        public string UPDATED_BY { get; set; }
        public Nullable<System.DateTime> UPDATED_DATE { get; set; }
    }
}
