namespace WBBContract.Commands.FBBWebConfigCommands
{
    public class SaveLeavemessageCommand
    {
        public SaveLeavemessageCommand()
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

        //20.2
        public bool p_wttx_full { get; set; }

        //20.4
        public string p_relate_mobile_no { get; set; }
        public string p_fbb_percent_discount { get; set; }

        public string p_order_mc_no { get; set; }
        public string p_address_mc { get; set; }

        // for return
        public decimal return_code { get; set; }
        public string return_msg { get; set; }

        //012024_Add Parameter
        public string p_coveragearea { get; set; }
        public string p_networkprovider { get; set; }
        public string p_region { get; set; }
        public string p_coveragesubstatus { get; set; }
        public string p_coveragegroupowner { get; set; }
        public string p_coveragecontactname { get; set; }
        public string p_coveragecontactemail { get; set; }
        public string p_coveragecontacttel { get; set; }
        public string p_coveragestatus { get; set; }
        public string p_coverage { get; set; }
    }
}
