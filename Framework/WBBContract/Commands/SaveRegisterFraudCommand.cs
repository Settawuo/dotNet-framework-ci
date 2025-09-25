namespace WBBContract.Commands
{
    public class SaveRegisterFraudCommand
    {
        public string p_cust_row_id { get; set; }
        public string p_created_by { get; set; }
        public string p_cen_fraud_flag { get; set; }
        public string p_verify_reason_cen { get; set; }
        public string p_fraud_score { get; set; }
        public string p_air_fraud_reason_array { get; set; }
        public string p_auto_create_prospect_flag { get; set; }
        public string p_cs_note_popup { get; set; }
        public string p_url_attach_popup { get; set; }
        public string ret_code { get; set; }
        public string ret_message { get; set; }
    }
}
