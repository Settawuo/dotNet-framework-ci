using System;

namespace WBBEntity.Models
{
    public class FBB_CFG_REPORT
    {
        public decimal REPORT_ID { get; set; }
        public string CREATED_BY { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
        public string UPDATED_BY { get; set; }
        public Nullable<System.DateTime> UPDATED_DATE { get; set; }
        public string REPORT_NAME { get; set; }
        public string MAIL_TYPE { get; set; }
        public string DAY_OF_WEEK { get; set; }
        public string MONTH_OF_YEAR { get; set; }
        public string DAY_OF_MONTH { get; set; }
        public string EMAIL_TO { get; set; }
        public string EMAIL_FROM { get; set; }
        public string EMAIL_CC { get; set; }
        public string EMAIL_SUBJECT { get; set; }
        public string EMAIL_CONTENT { get; set; }
        public string EMAIL_TO_NOTIFICATION { get; set; }
        public string ACTIVE_FLAG { get; set; }
    }
}
