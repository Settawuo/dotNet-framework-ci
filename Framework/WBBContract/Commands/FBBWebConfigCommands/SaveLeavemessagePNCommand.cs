namespace WBBContract.Commands.FBBWebConfigCommands
{
    public class SaveLeavemessagePNCommand
    {
        public SaveLeavemessagePNCommand()
        {
            this.return_code = -1;
            this.return_message = "";
        }

        public string p_language { get; set; }
        public string p_service_speed { get; set; }
        public string p_cust_name { get; set; }
        public string p_cust_surname { get; set; }
        public string p_contact_mobile_no { get; set; }
        public string p_is_ais_mobile { get; set; }
        public string p_contact_email { get; set; }
        public string p_address_type { get; set; }
        public string p_building_name { get; set; }
        public string p_house_no { get; set; }
        public string p_soi { get; set; }
        public string p_road { get; set; }
        public string p_sub_district { get; set; }
        public string p_district { get; set; }
        public string p_province { get; set; }
        public string p_postal_code { get; set; }
        public string p_contact_time { get; set; }
        public string p_rental_flag { get; set; }

        public string p_location_code { get; set; }
        public string p_asc_code { get; set; }
        public string p_emp_id { get; set; }
        public string p_sales_rep { get; set; }

        //v18.10
        public string p_channal { get; set; }
        public string p_campaign { get; set; }
        public string p_url { get; set; }

        public string p_latitude { get; set; }
        public string p_longitude { get; set; }
        //New leavemessage
        public string p_company_name { get; set; }

        // for return
        public decimal return_code { get; set; }
        public string return_message { get; set; }
    }
}
