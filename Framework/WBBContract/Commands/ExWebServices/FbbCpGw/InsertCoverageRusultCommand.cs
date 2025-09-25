namespace WBBContract.Commands.ExWebServices.FbbCpGw
{
    public class InsertCoverageRusultCommand
    {
        public string p_channel { get; set; }
        public string p_address_type { get; set; }
        public string p_postal_code { get; set; }
        public string p_district { get; set; }
        public string p_sub_district { get; set; }
        public string p_language { get; set; }
        public string p_building_name { get; set; }
        public string p_building_no { get; set; }
        public string p_phone_flag { get; set; }
        public string p_floor_no { get; set; }
        public string p_address_no { get; set; }
        public string p_moo { get; set; }
        public string p_soi { get; set; }
        public string p_road { get; set; }
        public string p_latitude { get; set; }
        public string p_longitude { get; set; }
        public string p_unit_no { get; set; }
        public string p_coverage_flag { get; set; }
        public string p_address_id { get; set; }
        public string p_is_partner { get; set; }
        public string p_partner_name { get; set; }
        public string p_firstname { get; set; }
        public string p_lastname { get; set; }
        public string p_contactnumber { get; set; }
        public string p_producttype { get; set; }
        public string p_owner_product { get; set; }
        public string p_splitter_name { get; set; }
        public string p_distance { get; set; }
        public string p_contact_email { get; set; }
        public string p_contact_line_id { get; set; }
        public string p_location_code { get; set; }
        public string p_asc_code { get; set; }
        public string p_employee_id { get; set; }
        public string p_location_name { get; set; }
        public string p_sub_region { get; set; }
        public string p_region_name { get; set; }
        public string p_asc_name { get; set; }
        public string p_sale_name { get; set; }
        public string p_channel_name { get; set; }
        public string p_sale_channel { get; set; }
        public string P_ADDRESS_TYPE_DTL { get; set; }
        public string P_REMARK { get; set; }
        public string P_TECHNOLOGY { get; set; }
        public string P_PROJECTNAME { get; set; }

        //R24.01 Add coverage
        public string P_CoverageArea { get; set; }
        public string P_NetworkProvider { get; set; }
        public string P_Region { get; set; }
        public string P_CoverageStatus { get; set; }
        public string P_CoverageSubstatus { get; set; }
        public string P_CoverageGroupOwner { get; set; }
        public string P_CoverageContactName { get; set; }
        public string P_CoverageContactEmail { get; set; }
        public string P_CoverageContactTel { get; set; }

        //Return
        public string ret_code { get; set; }
        public string ret_message { get; set; }
    }
}
