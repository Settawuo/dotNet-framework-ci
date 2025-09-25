using System;

namespace WBBEntity.Models
{
    public partial class FBB_EMAIL_PROCESSING
    {
        public string PROCESS_NAME { get; set; }
        public string SEND_TO { get; set; }
        public string SEND_CC { get; set; }
        public string SEND_BCC { get; set; }
        public Nullable<System.DateTime> EFFECTIVE_DATE { get; set; }
        public Nullable<System.DateTime> EXPIRE_DATE { get; set; }
        public string CREATE_BY { get; set; }
        public Nullable<System.DateTime> CREATE_DATE { get; set; }
        public string SEND_FROM { get; set; }
        public string ATTACHED_FILE { get; set; }
        public string IP_MAIL_SERVER { get; set; }
    }
}
