using System.Collections.Generic;

namespace WBBEntity.PanelModels.WebServiceModels
{
    public class ConfigurationReportModel
    {
        public decimal REPORT_ID { get; set; }
        public string REPORT_NAME { get; set; }
        public string STATUS_REPORT { get; set; }
        public string SCHEDULER { get; set; }
        public string DAY_OF_WEEK { get; set; }
        public string MONTH_OF_YEAR { get; set; }
        public string DAY_OF_MONTH { get; set; }
        public string EMAIL_TO { get; set; }
        public string EMAIL_FROM { get; set; }
        public string EMAIL_CC { get; set; }
        public string EMAIL_SUBJECT { get; set; }
        public string EMAIL_CONTENT { get; set; }
        public string EMAIL_TO_ADMIN { get; set; }
        public string ACTIVE_FLAG { get; set; }
        public string IP_MAIL_SERVER { get; set; }
        public string LAST_DATE_SEND_MAIL { get; set; }
        public string EMAIL_STATUS { get; set; }
        public string MESSAGE_FAILED { get; set; }
        public string REPORT_TYPE { get; set; }
        public string CREATED_BY { get; set; }
        public string ALL_RECORDS { get; set; }

        public List<ConfigurationReportQueryModel> ConfigurationQueryList { get; set; }

    }
}
