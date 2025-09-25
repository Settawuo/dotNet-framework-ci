namespace WBBContract.Commands
{
    public class InsertImportExcelLeaveMessageCommand
    {
        public string p_file_name { get; set; }
        public string p_username { get; set; }
        public string p_service_speed { get; set; }
        public string p_cust_name { get; set; }
        public string p_cust_surname { get; set; }
        public string p_contact_mobile_no { get; set; }
        public string p_is_ais_mobile { get; set; }
        public string p_contact_time { get; set; }
        public string p_contact_email { get; set; }
        public string p_address_type { get; set; }
        public string p_building_name { get; set; }
        public string p_village_name { get; set; }
        public string p_house_no { get; set; }
        public string p_soi { get; set; }
        public string p_road { get; set; }
        public string p_sub_district { get; set; }
        public string p_district { get; set; }
        public string p_province { get; set; }
        public string p_postal_code { get; set; }
        public string p_campaign_project_name { get; set; }

        public decimal return_code { get; set; }
        public string return_message { get; set; }
    }
}
