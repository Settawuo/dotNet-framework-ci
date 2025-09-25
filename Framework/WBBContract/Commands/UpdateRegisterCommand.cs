namespace WBBContract.Commands
{
    public class UpdateRegisterCommand
    {
        public string p_customer_name { get; set; }
        public string p_customer_lastname { get; set; }
        public string p_card_type { get; set; }
        public string p_card_no { get; set; }
        public string p_mobile_no { get; set; }
        public string p_fbbdorm_order_no { get; set; }
        public string p_prepaid_non_mobile { get; set; }
        public string p_time_slot { get; set; }
        public string p_address_id { get; set; }
        public string p_service_code { get; set; }
        public string p_event_code { get; set; }
        public string p_time_slot_id { get; set; }
        public string p_updated_by { get; set; }
        public string p_in_no { get; set; }
        public int ret_code { get; set; }
        public string ret_msg { get; set; }

    }
}