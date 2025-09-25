namespace WBBContract.Commands
{
    public class SaveRegisterFraudNoGoCommand
    {
        public string p_customer_type { get; set; }
        public string p_customer_name { get; set; }
        public string p_id_card_no { get; set; }
        public string p_install_address { get; set; }
        public string p_product_subtype { get; set; }
        public decimal? p_entry_fee { get; set; }
        public string p_operator { get; set; }
        public string p_promotion_name { get; set; }
        public string p_promotion_ontop { get; set; }
        public decimal? p_promotion_price { get; set; }
        public decimal? p_price_net { get; set; }
        public string p_cs_note { get; set; }
        public string p_location_code { get; set; }
        public string p_location_name { get; set; }
        public string p_chn_sales_name { get; set; }
        public string p_asc_code { get; set; }
        public string p_asc_name { get; set; }
        public string p_region_customer { get; set; }
        public string p_region_sale { get; set; }
        public decimal? p_fraud_score { get; set; }
        public string p_waiting_time_slot_flag { get; set; }
        public string p_project_name { get; set; }
        public string p_address_duplicated_flag { get; set; }
        public string p_id_duplicated_flag { get; set; }
        public string p_contact_duplicated_flag { get; set; }
        public string p_contact_not_active_flag { get; set; }
        public string p_contact_no_fmc_flag { get; set; }
        public string p_watch_list_dealer_flag { get; set; }
        public string p_sale_dealer_direct_sale_flag { get; set; }
        public string p_relate_mobile_segment { get; set; }
        public string p_charge_type { get; set; }
        public string p_service_month { get; set; }
        public string p_use_id_card_address_flag { get; set; }
        public string p_reason_verify { get; set; }
        public string p_created_by { get; set; }
        public string p_updated_by { get; set; }
        public string p_flag_send_xml { get; set; }
        public string p_message_send_xml { get; set; }
        public string ret_code { get; set; }
        public string ret_message { get; set; }
    }
}
