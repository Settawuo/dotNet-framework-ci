using System.Collections.Generic;

namespace WBBEntity.PanelModels.FBBWebConfigModels
{
    public class GetORDPendingPaymentModel
    {
        public string ret_code { get; set; }
        public string ret_msgerr { get; set; }
        public List<GetORDPendingPayment> GetORDPendingPaymentList { get; set; }
    }

    public class GetORDPendingPayment
    {
        public string url { get; set; }
        public string project_code { get; set; }
        public string command { get; set; }
        public string merchant_id { get; set; }
        public string order_id { get; set; }
        public string purchase_amt { get; set; }
        public string sale_id { get; set; }
        public string qrcommand { get; set; }
        public string appid { get; set; }
        public string appsecret { get; set; }
        public string internet_no { get; set; }
        public string order_type { get; set; }
    }

    public class GetListORDDetailCreateModel
    {
        public string ret_code { get; set; }
        public string ret_message { get; set; }
        public string return_ia_no { get; set; }
        public string return_order_no { get; set; }
        public List<ODRDetailCustomer> ODRDetailCustomerList { get; set; }
        public List<ODRDetailPackage> ODRDetailPackageList { get; set; }
        public List<ODRDetailFile> ODRDetailFileList { get; set; }
        public List<ODRDetailSplitter> ODRDetailSplitterList { get; set; }
        public List<ODRDetailCPE> ODRDetailCPEList { get; set; }

    }

    public class ODRDetailCustomer
    {
        public string customer_type { get; set; }
        public string customer_subtype { get; set; }
        public string title_code { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string contact_title_code { get; set; }
        public string contact_first_name { get; set; }
        public string contact_last_name { get; set; }
        public string id_card_type_desc { get; set; }
        public string id_card_no { get; set; }
        public string tax_id { get; set; }
        public string gender { get; set; }
        public string birth_date { get; set; }
        public string mobile_no { get; set; }
        public string mobile_no_2 { get; set; }
        public string home_phone_no { get; set; }
        public string email_address { get; set; }
        public string contact_time { get; set; }
        public string nationality_desc { get; set; }
        public string customer_remark { get; set; }

        public string house_no { get; set; }
        public string moo_no { get; set; }
        public string building { get; set; }
        public string floor { get; set; }
        public string room { get; set; }
        public string mooban { get; set; }
        public string soi { get; set; }
        public string road { get; set; }
        public string zipcode_rowid { get; set; }

        public string latitude { get; set; }
        public string longtitude { get; set; }
        public string asc_code { get; set; }
        public string employee_id { get; set; }
        public string location_code { get; set; }
        public string sale_represent { get; set; }
        public string cs_note { get; set; }
        public string wifi_access_point { get; set; }
        public string install_status { get; set; }
        public string coverage { get; set; }
        public string existing_airnet_no { get; set; }
        public string gsm_mobile_no { get; set; }
        public string contact_name_1 { get; set; }
        public string contact_name_2 { get; set; }
        public string contact_mobile_no_1 { get; set; }
        public string contact_mobile_no_2 { get; set; }
        public string condo_floor { get; set; }
        public string condo_roof_top { get; set; }
        public string condo_balcony { get; set; }
        public string balcony_north { get; set; }
        public string balcony_south { get; set; }
        public string balcony_east { get; set; }
        public string balcony_wast { get; set; }
        public string high_building { get; set; }
        public string high_tree { get; set; }
        public string billboard { get; set; }
        public string expressway { get; set; }
        public string address_type_wire { get; set; }
        public string address_type { get; set; }
        public string floor_no { get; set; }

        public string house_no_bl { get; set; }
        public string moo_no_bl { get; set; }
        public string building_bl { get; set; }
        public string floor_bl { get; set; }
        public string room_bl { get; set; }
        public string mooban_bl { get; set; }
        public string soi_bl { get; set; }
        public string road_bl { get; set; }
        public string zipcode_rowid_bl { get; set; }

        public string house_no_vt { get; set; }
        public string moo_no_vt { get; set; }
        public string building_vt { get; set; }
        public string floor_vt { get; set; }
        public string room_vt { get; set; }
        public string mooban_vt { get; set; }
        public string soi_vt { get; set; }
        public string road_vt { get; set; }
        public string zipcode_rowid_vt { get; set; }

        public string cvr_id { get; set; }
        public string cvr_node { get; set; }
        public string cvr_tower { get; set; }

        public string relate_mobile { get; set; }
        public string relate_non_mobile { get; set; }
        public string sff_ca_no { get; set; }
        public string sff_sa_no { get; set; }
        public string sff_ba_no { get; set; }
        public string network_type { get; set; }
        public string service_day { get; set; }
        public string expect_install_date { get; set; }
        public bool service_dayspecified { get; set; }
        public string fttx_vendor { get; set; }
        public string install_note { get; set; }

        public string phone_flag { get; set; }
        public string time_slot { get; set; }
        public string installation_capacity { get; set; }
        public string address_id { get; set; }
        public string access_mode { get; set; }

        public string eng_flag { get; set; }
        public string event_code { get; set; }
        public string installaddress1 { get; set; }
        public string installaddress2 { get; set; }
        public string installaddress3 { get; set; }
        public string installaddress4 { get; set; }
        public string installaddress5 { get; set; }
        public string pbox_count { get; set; }
        public string convergence_flag { get; set; }
        public string time_slot_id { get; set; }

        public string gift_voucher { get; set; }
        public string sub_location_id { get; set; }
        public string sub_contract_name { get; set; }
        public string install_staff_id { get; set; }
        public string install_staff_name { get; set; }

        public string flow_flag { get; set; }
        public string site_code { get; set; }

        public string line_id { get; set; }
        public string relate_project_name { get; set; }
        public string plug_and_play_flag { get; set; }

        public string reserved_id { get; set; }
        public string job_order_type { get; set; }
        public string assign_rule { get; set; }
        public string old_isp { get; set; }

        public string splitter_flag { get; set; }
        public string reserved_port_id { get; set; }
        public string special_remark { get; set; }
        public string order_no { get; set; }
        public string source_system { get; set; }

        public string bill_media { get; set; }

        public string pre_order_no { get; set; }
        public string voucher_desc { get; set; }
        public string campaign_project_name { get; set; }
        public string pre_order_chanel { get; set; }

        public string rental_flag { get; set; }
        public string dev_project_code { get; set; }
        public string dev_bill_to { get; set; }
        public string dev_po_no { get; set; }

        public string partner_type { get; set; }
        public string partner_subtype { get; set; }
        public string mobile_by_asc { get; set; }
        public string location_name { get; set; }
        public string paymentmethod { get; set; }
        public string transactionid_in { get; set; }
        public string transactionid { get; set; }

        public string sub_access_mode { get; set; }
        public string request_sub_flag { get; set; }
        public string premium_flag { get; set; }
        public string relate_mobile_segment { get; set; }
        public string ref_ur_no { get; set; }
        public string location_email_by_region { get; set; }
        //20.3
        public string sale_staff_name { get; set; }
        public string dopa_flag { get; set; }
        public string request_cs_verify_doc { get; set; }
        public string facerecog_flag { get; set; }
        public string special_account_name { get; set; }
        public string special_account_no { get; set; }
        public string special_account_enddate { get; set; }
        public string special_account_group_email { get; set; }
        public string special_account_flag { get; set; }
        public string existing_mobile_flag { get; set; }
        public string pre_survey_date { get; set; }
        public string pre_survey_timeslot { get; set; }
        public string replace_onu { get; set; }
        public string replace_wifi { get; set; }
        public string number_of_mesh { get; set; }
        public string company_name { get; set; }
        public string distribution_channel { get; set; }
        public string channel_sales_group { get; set; }
        public string shop_type { get; set; }
        public string shop_segment { get; set; }
        public string asc_name { get; set; }
        public string asc_member_category { get; set; }
        public string asc_position { get; set; }
        public string location_region { get; set; }
        public string location_sub_region { get; set; }
        public string employee_name { get; set; }
        public string customerpurge { get; set; }
        public string exceptentryfee { get; set; }
        public string secondinstallation { get; set; }
        public string amendment_flag { get; set; }
        public string service_level { get; set; }
        public string first_install_date { get; set; }
        public string first_time_slot { get; set; }
        public string line_temp_id { get; set; }
        public string service_year { get; set; }
    }

    public class ODRDetailPackage
    {
        public string temp_ia { get; set; }
        public string product_subtype { get; set; }
        public string package_type { get; set; }
        public string package_code { get; set; }
        public decimal package_price { get; set; }
        public string idd_flag { get; set; }
        public string fax_flag { get; set; }
        public string home_ip { get; set; }
        public string home_port { get; set; }
        public string mobile_forward { get; set; }
        public string pbox_ext { get; set; }
        public string sff_promotion_code { get; set; }
    }

    public class ODRDetailFile
    {
        public string file_name { get; set; }
    }

    public class ODRDetailSplitter
    {
        public string splitter_name { get; set; }
        public decimal distance { get; set; }
        public string distance_type { get; set; }
        public string resource_type { get; set; }
    }

    public class ODRDetailCPE
    {
        public string cpe_type { get; set; }
        public string serial_no { get; set; }
        public string mac_address { get; set; }
    }

    public class GetORDPendingPaymentTimeOutModel
    {
        public string ret_code { get; set; }
        public string ret_msgerr { get; set; }
        public List<GetORDPendingPaymentTimeOut> GetORDPendingPaymentTimeOutList { get; set; }
    }

    public class GetORDPendingPaymentTimeOut
    {
        public string order_id { get; set; }
        public string reserve_timeslot_id { get; set; }
        public string reserve_port_id { get; set; }
        public string mobile_no { get; set; }
        public string eng_flag { get; set; }
        public string order_type { get; set; }
        public string internet_no { get; set; }
    }



}
