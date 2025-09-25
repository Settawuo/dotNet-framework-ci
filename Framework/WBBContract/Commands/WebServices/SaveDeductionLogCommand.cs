namespace WBBContract.Commands.WebServices
{
    public class SaveDeductionLogCommand
    {
        public SaveDeductionLogCommand()
        {
            this.ret_code = "-1";
            this.ret_message = "";
        }
        public string p_action { get; set; }
        public string p_user_name { get; set; }
        public string p_transaction_id { get; set; }
        public string p_mobile_no { get; set; }
        public string p_service_name { get; set; }
        public string p_endpoint { get; set; }
        public string p_pm_tux_code { get; set; }
        public string p_pm_receipt_num { get; set; }
        public string p_enq_status { get; set; }
        public string p_enq_status_code { get; set; }
        public string p_req_xml_param { get; set; }
        public string p_res_xml_param { get; set; }
        public string p_order_transaction_id { get; set; }
        public string ret_code { get; set; }
        public string ret_message { get; set; }
    }
}
