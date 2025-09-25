using System.Collections.Generic;

namespace WBBContract.Commands
{
    public class ConfigurationAutoMailCommand
    {
        public ConfigurationAutoMailCommand()
        {
            this.return_code = -1;
            this.return_msg = "";

            if (ConfigurationQueryList == null)
                ConfigurationQueryList = new List<ConfigurationQueryArrayModel>();
        }

        public decimal report_id { get; set; }
        public string report_name { get; set; }
        public string scheduler { get; set; }
        public string day_of_week { get; set; }
        public string month_of_year { get; set; }
        public string day_of_month { get; set; }
        public string email_to { get; set; }
        public string email_from { get; set; }
        public string email_cc { get; set; }
        public string email_subject { get; set; }
        public string email_content { get; set; }
        public string email_to_admin { get; set; }
        public string active_flag { get; set; }
        public string ip_mail_server { get; set; }
        public string report_type { get; set; }
        public string created_by { get; set; }

        //for return
        public decimal return_code { get; set; }
        public string return_msg { get; set; }

        public List<ConfigurationQueryArrayModel> ConfigurationQueryList { get; set; }

    }

    public class ConfigurationQueryArrayModel
    {
        public decimal query_id { get; set; }
        public string sheet_name { get; set; }
        public string owner_db { get; set; }
        public string query_1 { get; set; }
        public string query_2 { get; set; }
        public string query_3 { get; set; }
        public string query_4 { get; set; }
        public string query_5 { get; set; }
        public string query_type { get; set; }
    }

}
