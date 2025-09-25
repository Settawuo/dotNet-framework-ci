using System;

namespace WBBEntity.PanelModels
{
    public class FbbCFGReportModel
    {
        public decimal report_id { get; set; }
        public string created_by { get; set; }
        public System.DateTime created_date { get; set; }
        public string updated_by { get; set; }
        public Nullable<System.DateTime> updated_date { get; set; }
        public string report_name { get; set; }
        public string day_of_week { get; set; }
        public string month_of_year { get; set; }
        public string day_of_month { get; set; }
        public string email_to { get; set; }
        public string email_from { get; set; }
        public string email_cc { get; set; }
        public string email_subject { get; set; }
        public string email_content { get; set; }
        public string acitive_flag { get; set; }
    }
}
