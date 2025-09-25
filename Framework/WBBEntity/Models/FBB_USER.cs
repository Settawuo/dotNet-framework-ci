using System;

namespace WBBEntity.Models
{
    public partial class FBB_USER
    {
        public decimal USER_ID { get; set; }
        public string USER_NAME { get; set; }
        public string CREATED_BY { get; set; }
        public Nullable<System.DateTime> CREATED_DATE { get; set; }
        public string UPDATED_BY { get; set; }
        public Nullable<System.DateTime> UPDATED_DATE { get; set; }
        public string FIRST_NAME { get; set; }
        public string LAST_NAME { get; set; }
        public string PIN_CODE { get; set; }
        public string POSITION { get; set; }
        public string MOBILE_NUMBER { get; set; }
        public string EMAIL { get; set; }
        public string ACTIVE_FLAG { get; set; }
    }
}
