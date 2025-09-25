

using System.Collections.Generic;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Commands.WebServices
{
    public class InsertSaveOrderNew911Command
    {
        public InsertSaveOrderNew911Command()
        {
            this.p_air_regist_package_array = new List<AirRegistPackage>();
            this.p_air_regist_file_array = new List<AirRegistFile>();
            this.p_air_regist_splitter_array = new List<AirRegistSplitter>();
            this.p_air_regist_cpe_serial_array = new List<AirRegistCPESerial>();
            this.p_air_regist_cust_insi_array = new List<AirRegistCustInsi>();
            this.p_air_regist_dcontract_array = new List<AirRegistDcontract>();
            this.o_return_code = -1;
            this.o_return_message = "";
        }

        public string p_customer_type { get; set; }
        public string p_customer_subtype { get; set; }
        public string p_title_code { get; set; }
        public string p_first_name { get; set; }
        public string p_last_name { get; set; }
        public string p_contact_title_code { get; set; }
        public string p_contact_first_name { get; set; }
        public string p_contact_last_name { get; set; }
        public string p_id_card_type_desc { get; set; }
        public string p_id_card_no { get; set; }
        public string p_tax_id { get; set; }
        public string p_gender { get; set; }
        public string p_birth_date { get; set; }
        public string p_mobile_no { get; set; }
        public string p_mobile_no_2 { get; set; }
        public string p_home_phone_no { get; set; }
        public string p_email_address { get; set; }
        public string p_contact_time { get; set; }
        public string p_nationality_desc { get; set; }
        public string p_customer_remark { get; set; }
        public string p_house_no { get; set; }
        public string p_moo_no { get; set; }
        public string p_building { get; set; }
        public string p_floor { get; set; }
        public string p_room { get; set; }
        public string p_mooban { get; set; }
        public string p_soi { get; set; }
        public string p_road { get; set; }
        public string p_zipcode_rowid { get; set; }
        public string p_latitude { get; set; }
        public string p_longtitude { get; set; }
        public string p_asc_code { get; set; }
        public string p_employee_id { get; set; }
        public string p_location_code { get; set; }
        public string p_sale_represent { get; set; }
        public string p_cs_note { get; set; }
        public string p_wifi_access_point { get; set; }
        public string p_install_status { get; set; }
        public string p_coverage { get; set; }
        public string p_existing_airnet_no { get; set; }
        public string p_gsm_mobile_no { get; set; }
        public string p_contact_name_1 { get; set; }
        public string p_contact_name_2 { get; set; }
        public string p_contact_mobile_no_1 { get; set; }
        public string p_contact_mobile_no_2 { get; set; }
        public string p_condo_floor { get; set; }
        public string p_condo_roof_top { get; set; }
        public string p_condo_balcony { get; set; }
        public string p_balcony_north { get; set; }
        public string p_balcony_south { get; set; }
        public string p_balcony_east { get; set; }
        public string p_balcony_wast { get; set; }
        public string p_high_building { get; set; }
        public string p_high_tree { get; set; }
        public string p_billboard { get; set; }
        public string p_expressway { get; set; }
        public string p_address_type_wire { get; set; }
        public string p_address_type { get; set; }
        public string p_floor_no { get; set; }
        public string p_house_no_bl { get; set; }
        public string p_moo_no_bl { get; set; }
        public string p_mooban_bl { get; set; }
        public string p_building_bl { get; set; }
        public string p_floor_bl { get; set; }
        public string p_room_bl { get; set; }
        public string p_soi_bl { get; set; }
        public string p_road_bl { get; set; }
        public string p_zipcode_rowid_bl { get; set; }
        public string p_house_no_vt { get; set; }
        public string p_moo_no_vt { get; set; }
        public string p_mooban_vt { get; set; }
        public string p_building_vt { get; set; }
        public string p_floor_vt { get; set; }
        public string p_room_vt { get; set; }
        public string p_soi_vt { get; set; }
        public string p_road_vt { get; set; }
        public string p_zipcode_rowid_vt { get; set; }
        public string p_cvr_id { get; set; }
        public string p_cvr_node { get; set; }
        public string p_cvr_tower { get; set; }
        public string p_site_code { get; set; }
        public string p_relate_mobile { get; set; }
        public string p_relate_non_mobile { get; set; }
        public string p_sff_ca_no { get; set; }
        public string p_sff_sa_no { get; set; }
        public string p_sff_ba_no { get; set; }
        public string p_network_type { get; set; }
        public string p_service_day { get; set; }
        public string p_expect_install_date { get; set; }
        public string p_fttx_vendor { get; set; }
        public string p_install_note { get; set; }
        public string p_phone_flag { get; set; }
        public string p_time_Slot { get; set; }
        public string p_installation_Capacity { get; set; }
        public string p_address_Id { get; set; }
        public string p_access_Mode { get; set; }
        public string p_eng_flag { get; set; }
        public string p_event_code { get; set; }
        public string p_installAddress1 { get; set; }
        public string p_installAddress2 { get; set; }
        public string p_installAddress3 { get; set; }
        public string p_installAddress4 { get; set; }
        public string p_installAddress5 { get; set; }
        public string p_pbox_count { get; set; }
        public string p_convergence_flag { get; set; }
        public string p_time_slot_id { get; set; }
        public string p_gift_voucher { get; set; }
        public string p_sub_location_id { get; set; }
        public string p_sub_contract_name { get; set; }
        public string p_install_staff_id { get; set; }
        public string p_install_staff_name { get; set; }
        public string p_flow_flag { get; set; }
        public string p_line_id { get; set; }
        public string p_relate_project_name { get; set; }
        public string p_plug_and_play_flag { get; set; }
        public string p_reserved_id { get; set; }
        public string p_job_order_type { get; set; }
        public string p_assign_rule { get; set; }
        public string p_old_isp { get; set; }
        public string p_splitter_flag { get; set; }
        public string p_reserved_port_id { get; set; }
        public string p_special_remark { get; set; }
        public string p_order_no { get; set; }
        public string p_source_system { get; set; }
        public string p_bill_media { get; set; }
        public string p_pre_order_no { get; set; }
        public string p_voucher_desc { get; set; }
        public string p_campaign_project_name { get; set; }
        public string p_pre_order_chanel { get; set; }
        public string p_rental_flag { get; set; }
        public string p_dev_project_code { get; set; }
        public string p_dev_bill_to { get; set; }
        public string p_dev_po_no { get; set; }
        public string p_partner_type { get; set; }
        public string p_partner_subtype { get; set; }
        public string p_mobile_by_asc { get; set; }
        public string p_location_name { get; set; }
        public string p_paymentMethod { get; set; }
        public string p_transactionId_in { get; set; }
        public string p_transactionId { get; set; }
        public string p_sub_access_mode { get; set; }
        public string p_request_sub_flag { get; set; }
        public string p_premium_flag { get; set; }
        public string p_relate_mobile_segment { get; set; }
        public string p_ref_ur_no { get; set; }
        public string p_location_email_by_region { get; set; }
        public string p_sale_staff_name { get; set; }
        public string p_dopa_flag { get; set; }
        public string p_service_year { get; set; }
        public string p_require_cs_verify_doc { get; set; }
        public string p_facerecog_flag { get; set; }
        public string p_special_account_name { get; set; }
        public string p_special_account_no { get; set; }
        public string p_special_account_enddate { get; set; }
        public string p_special_account_group_email { get; set; }
        public string p_special_account_flag { get; set; }
        public string p_existing_mobile_flag { get; set; }
        public string p_pre_survey_date { get; set; }
        public string p_pre_survey_timeslot { get; set; }
        public string p_register_channel { get; set; }
        public string p_auto_create_prospect_flag { get; set; }
        public string p_order_verify { get; set; }
        public string p_waiting_install_date { get; set; }
        public string p_waiting_time_slot { get; set; }
        public string p_sale_channel { get; set; }
        public string p_owner_product { get; set; }
        public string p_package_for { get; set; }
        public string p_sff_promotion_code { get; set; }
        public string p_region { get; set; }
        public string p_province { get; set; }
        public string p_district { get; set; }
        public string p_sub_district { get; set; }
        public string p_serenade_flag { get; set; }
        public string p_fmpa_flag { get; set; }
        public string p_cvm_flag { get; set; }
        public string p_order_relate_change_pro { get; set; }
        public string p_company_name { get; set; }
        public string p_distribution_channel { get; set; }
        public string p_channel_sales_group { get; set; }
        public string p_shop_type { get; set; }
        public string p_shop_segment { get; set; }
        public string p_asc_name { get; set; }
        public string p_asc_member_category { get; set; }
        public string p_asc_position { get; set; }
        public string p_location_region { get; set; }
        public string p_location_sub_region { get; set; }
        public string p_employee_name { get; set; }
        public string p_customerpurge { get; set; }
        public string p_exceptentryfee { get; set; }
        public string p_secondinstallation { get; set; }
        public string p_amendment_flag { get; set; }
        public string p_service_level { get; set; }
        public string p_first_install_date { get; set; }
        public string p_first_time_slot { get; set; }
        public string p_line_temp_id { get; set; }
        public string p_fmc_special_flag { get; set; }
        public string p_non_res_flag { get; set; }
        public string p_criteria_mobile { get; set; }
        public string p_remark_for_subcontract { get; set; }
        public string p_mesh_count { get; set; }
        public string p_online_flag { get; set; }
        public string p_privilege_points { get; set; }
        public string p_transaction_privilege_id { get; set; }
        public string p_special_skill { get; set; }
        public string p_tdm_contract_id { get; set; }
        public string p_tdm_rule_id { get; set; }
        public string p_tdm_penalty_id { get; set; }
        public string p_tdm_penalty_group_id { get; set; }
        public string p_duration { get; set; }
        public string p_contract_flag { get; set; }
        public string p_national_id { get; set; }
        public string p_non_mobile_no { get; set; }
        public string p_regist_paymentId { get; set; }
        public string p_regist_paymentDate { get; set; }
        public string p_regist_paymentMethod { get; set; }
        public List<AirRegistPackage> p_air_regist_package_array { get; set; }
        public List<AirRegistFile> p_air_regist_file_array { get; set; }
        public List<AirRegistSplitter> p_air_regist_splitter_array { get; set; }
        public List<AirRegistCPESerial> p_air_regist_cpe_serial_array { get; set; }
        public List<AirRegistCustInsi> p_air_regist_cust_insi_array { get; set; }
        public List<AirRegistDcontract> p_air_regist_dcontract_array { get; set; }

        // out
        public decimal o_return_code { get; set; }
        public string o_return_message { get; set; }
        public string o_return_order_no { get; set; }
        public string o_return_Multi_Instance_flag { get; set; }
        public string o_return_product_subtype { get; set; }
        public string o_return_event_code { get; set; }
    }
}
