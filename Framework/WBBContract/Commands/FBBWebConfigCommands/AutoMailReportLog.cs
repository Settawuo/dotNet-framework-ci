namespace WBBContract.Commands.FBBWebConfigCommands
{
    public class AutoMailReportLog
    {
        public decimal p_log_id { get; set; }
        public decimal p_report_id { get; set; }
        public string p_status { get; set; }
        public string p_status_message { get; set; }
        public string p_path_file { get; set; }
        public string p_create_by { get; set; }

        public string p_return_code { get; set; }
        public string p_return_message { get; set; }

    }
}
