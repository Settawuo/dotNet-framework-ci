namespace WBBContract.Commands.FBBWebConfigCommands
{
    public class SaveLeavemessageIMCommand
    {
        public SaveLeavemessageIMCommand()
        {
            this.return_code = -1;
            this.return_msg = "";
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

        //V17.3
        public string p_location_code { get; set; }
        public string p_asc_code { get; set; }
        public string p_channel { get; set; }

        //V17.5
        public string p_internet_no { get; set; }

        //v17.7
        public string p_line_id { get; set; }
        public string p_voucher_desc { get; set; }
        public string p_campaign_project_name { get; set; }

        //v17.9
        public string p_rental_flag { get; set; }

        //v18.4
        public string p_location_check_coverage { get; set; }

        public string p_full_address { get; set; }

        //v18.8
        public string p_emp_id { get; set; }
        public string p_sales_rep { get; set; }

        //v18.9
        public string p_in_coverage { get; set; }
        public string p_playbox_flag { get; set; }
        public string p_latitude { get; set; }
        public string p_longitude { get; set; }

        //v18.10
        public string p_url { get; set; }

        //v19.2
        public string p_asset_number { get; set; }
        public string p_service_case_id { get; set; }

        //v19.3
        public string p_building_no { get; set; }
        public string p_floor { get; set; }
        public string p_moo { get; set; }

        // for return
        public decimal return_code { get; set; }
        public string return_msg { get; set; }
    }
}
