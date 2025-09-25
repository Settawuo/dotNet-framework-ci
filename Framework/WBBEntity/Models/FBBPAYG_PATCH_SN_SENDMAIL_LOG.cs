using System;

namespace WBBEntity.Models
{
    public partial class FBBPAYG_PATCH_SN_SENDMAIL_LOG
    {
        public string FILE_NAME { get; set; }
        public string MAIL_TO { get; set; }
        public string MAIL_CONTENT { get; set; }
        public string MAIL_STATUS { get; set; }
        public DateTime CREATE_DATE { get; set; }
        public string CREATE_BY { get; set; }
        public DateTime UPDATE_DATE { get; set; }
        public string UPDATE_BY { get; set; }
        public string ERROR_MESSAGE { get; set; }
    }
}
