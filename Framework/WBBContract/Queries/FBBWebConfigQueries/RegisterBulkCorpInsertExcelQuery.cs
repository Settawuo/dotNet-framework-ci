using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries
{
    public class RegisterBulkCorpInsertExcelQuery : IQuery<ReturnInsertExcelData>
    {
        public string p_no { get; set; }
        public string p_installaddress1 { get; set; }
        public string p_installaddress2 { get; set; }
        public string p_installaddress3 { get; set; }
        public string p_installaddress4 { get; set; }
        public string p_installaddress5 { get; set; }
        public string p_latitude { get; set; }
        public string p_longitude { get; set; }
        public string p_install_date { get; set; }//change from web in excel
        //public string p_dpname { get; set; }
        //public string p_installationcapacity { get; set; }
        //public string p_ia { get; set; }
        //public string p_password { get; set; }

        public string p_user { get; set; }
        public string p_file_name { get; set; }
        public int p_file_size { get; set; }
        public int p_total_row { get; set; }
        public string p_bulk_no { get; set; }

        public string p_technology_install { get; set; }
        public string p_address_id { get; set; }
        //public string p_install_date { get; set; }
        public string p_event_code { get; set; }
        public string p_contact_first_name { get; set; }
        public string p_contact_last_name { get; set; }
        public string p_contact_phone { get; set; }
        public string p_contact_mobile { get; set; }
        public string p_contact_email { get; set; }

        public string pm_sff_promotion_code { get; set; }
        public string pm_package_class { get; set; }
        public string pm_sff_promotion_bill_tha { get; set; }
        public string pm_sff_promotion_bill_eng { get; set; }
        public string pm_package_name_tha { get; set; }
        public decimal pm_recurring_charge { get; set; }
        public decimal pm_pre_initiation_charge { get; set; }
        public decimal pm_initiation_charge { get; set; }
        public string pm_download_speed { get; set; }
        public string pm_upload_speed { get; set; }
        public string pm_product_type { get; set; }
        public string pm_owner_product { get; set; }
        public string pm_product_subtype { get; set; }
        public string pm_product_subtype2 { get; set; }
        public string pm_technology { get; set; }
        public string pm_package_group { get; set; }
        public string pm_package_code { get; set; }

        public string pi_sff_promotion_code { get; set; }
        public string pi_package_class { get; set; }
        public string pi_sff_promotion_bill_tha { get; set; }
        public string pi_sff_promotion_bill_eng { get; set; }
        public string pi_package_name_tha { get; set; }
        public decimal pi_recurring_charge { get; set; }
        public decimal pi_pre_initiation_charge { get; set; }
        public decimal pi_initiation_charge { get; set; }
        public string pi_download_speed { get; set; }
        public string pi_upload_speed { get; set; }
        public string pi_product_type { get; set; }
        public string pi_owner_product { get; set; }
        public string pi_product_subtype { get; set; }
        public string pi_product_subtype2 { get; set; }
        public string pi_technology { get; set; }
        public string pi_package_group { get; set; }
        public string pi_package_code { get; set; }

        public string pv_sff_promotion_code { get; set; }
        public string pv_package_class { get; set; }
        public string pv_sff_promotion_bill_tha { get; set; }
        public string pv_sff_promotion_bill_eng { get; set; }
        public string pv_package_name_tha { get; set; }
        public decimal pv_recurring_charge { get; set; }
        public decimal pv_pre_initiation_charge { get; set; }
        public decimal pv_initiation_charge { get; set; }
        public string pv_download_speed { get; set; }
        public string pv_upload_speed { get; set; }
        public string pv_product_type { get; set; }
        public string pv_owner_product { get; set; }
        public string pv_product_subtype { get; set; }
        public string pv_product_subtype2 { get; set; }
        public string pv_technology { get; set; }
        public string pv_package_group { get; set; }
        public string pv_package_code { get; set; }

        public string s1_service_code { get; set; }
        public string s1_product_name { get; set; }
        public string s2_service_code { get; set; }
        public string s2_product_name { get; set; }
        public string s3_service_code { get; set; }
        public string s3_product_name { get; set; }

        public string pod_sff_promotion_code { get; set; }
        public string pod_package_class { get; set; }
        public string pod_sff_promotion_bill_tha { get; set; }
        public string pod_sff_promotion_bill_eng { get; set; }
        public string pod_package_name_tha { get; set; }
        public decimal pod_recurring_charge { get; set; }
        public decimal pod_pre_initiation_charge { get; set; }
        public decimal pod_initiation_charge { get; set; }
        public string pod_download_speed { get; set; }
        public string pod_upload_speed { get; set; }
        public string pod_product_type { get; set; }
        public string pod_owner_product { get; set; }
        public string pod_product_subtype { get; set; }
        public string pod_product_subtype2 { get; set; }
        public string pod_technology { get; set; }
        public string pod_package_group { get; set; }
        public string pod_package_code { get; set; }

        //return
        public string output_bulk_no { get; set; }
        public string output_return_code { get; set; }
        public string output_return_message { get; set; }

    }
}
