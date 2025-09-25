namespace WBBContract.Commands.WebServices
{
    public class SavePaymentSPDPLogCommand
    {
        public SavePaymentSPDPLogCommand()
        {
            this.ret_code = "-1";
            this.ret_message = "";
        }

        public string p_action { get; set; }
        public string p_user_name { get; set; }
        public string p_non_mobile_no { get; set; }
        public string p_service_name { get; set; }
        public string p_endpoint { get; set; }
        public string p_order_id { get; set; }
        public string p_txn_id { get; set; }
        public string p_status { get; set; }
        public string p_status_code { get; set; }
        public string p_status_message { get; set; }
        public string p_channel { get; set; }
        public string p_amount { get; set; }
        public string p_req_xml_param { get; set; }
        public string p_res_xml_param { get; set; }
        public string p_order_transaction_id { get; set; }
        /// out
        public string ret_code { get; set; }
        public string ret_message { get; set; }
    }
}
