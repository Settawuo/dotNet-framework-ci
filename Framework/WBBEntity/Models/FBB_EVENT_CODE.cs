using System;

namespace WBBEntity.Models
{
    public partial class FBB_EVENT_CODE
    {
        public string EVENT_CODE { get; set; }
        public Nullable<System.DateTime> EFFECTIVE_DATE { get; set; }
        public Nullable<System.DateTime> EXPIRE_DATE { get; set; }
        public string CREATED_BY { get; set; }
        public Nullable<System.DateTime> CREATED_DATE { get; set; }
        public string UPDATED_BY { get; set; }
        public Nullable<System.DateTime> UPDATED_DATE { get; set; }
        public string TECHNOLOGY { get; set; }
        public string PLUG_AND_PLAY_FLAG { get; set; }
    }
}
