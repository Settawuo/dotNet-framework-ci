using System.Collections.Generic;

namespace WBBEntity.PanelModels.FBBWebConfigModels
{
    public class RegisterBulkCorpInstallModel
    {
        public string p_no { get; set; }
        public string p_installaddress1 { get; set; }
        public string p_installaddress2 { get; set; }
        public string p_installaddress3 { get; set; }
        public string p_installaddress4 { get; set; }
        public string p_installaddress5 { get; set; }
        public string p_latitude { get; set; }
        public string p_longitude { get; set; }
        public string p_dpname { get; set; }
        public string p_installationcapacity { get; set; }
        public string p_ia { get; set; }
        public string p_password { get; set; }

        public string p_user { get; set; }
        public string p_file_name { get; set; }
        public int p_file_size { get; set; }
        public int p_total_row { get; set; }
        public string p_bulk_no { get; set; }

        public string p_technology_install { get; set; }
        public string p_address_id { get; set; }
        public string p_install_date { get; set; }
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

        public UploadExcelBulk ExcelBulk { get; set; }
        public string Register_device { get; set; }
        public string account_category { get; set; }
        public string id_card_no { get; set; }
        public string id_card_type { get; set; }
        public string ca_main_mobile { get; set; }
        public string ClientIP { get; set; }

        public string output_bulk_no { get; set; }
        public string output_return_code { get; set; }
        public string output_return_message { get; set; }

    }

    public class ReturnInsertExcelData
    {
        public string output_bulk_no { get; set; }
        public string output_return_code { get; set; }
        public string output_return_message { get; set; }
    }

    public class UploadExcelBulk
    {
        public string FileExcelBulk { get; set; }
    }

    public class BulkExcelData
    {
        public string No { get; set; }
        public string installAddress1 { get; set; }
        public string installAddress2 { get; set; }
        public string installAddress3 { get; set; }
        public string installAddress4 { get; set; }
        public string installAddress5 { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string install_date { get; set; }
        //public string dpName { get; set; }
        //public string installationCapacity { get; set; }
        //public string ia { get; set; }
        //public string password { get; set; }
    }

    public class BulkExcelDataResend
    {
        public string No { get; set; }
        public string OrderNumber { get; set; }
        public string installAddress1 { get; set; }
        public string installAddress2 { get; set; }
        public string installAddress3 { get; set; }
        public string installAddress4 { get; set; }
        public string installAddress5 { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string install_date { get; set; }

    }

    public class BulkInsertExcel
    {
        public string No { get; set; }
        public string installAddress1 { get; set; }
        public string installAddress2 { get; set; }
        public string installAddress3 { get; set; }
        public string installAddress4 { get; set; }
        public string installAddress5 { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string p_install_date { get; set; }
        //public string dpName { get; set; }
        //public string installationCapacity { get; set; }
        //public string ia { get; set; }
        //public string password { get; set; }

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

    }

    public class BulkInsertExcelResend
    {
        public string bulk_number { get; set; }
        public string p_user { get; set; }
        public string No { get; set; }
        public string OrderNumber { get; set; }
        public string installAddress1 { get; set; }
        public string installAddress2 { get; set; }
        public string installAddress3 { get; set; }
        public string installAddress4 { get; set; }
        public string installAddress5 { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string install_date { get; set; }
        public string p_file_name { get; set; }
    }

    public class ReturnPackageList
    {
        public string sff_promotion_code { get; set; }
        public string package_class { get; set; }
        public string sff_promotion_bill_tha { get; set; }
        public string sff_promotion_bill_eng { get; set; }
        public string package_name_tha { get; set; }
        public string package_name_eng { get; set; }
        public decimal recurring_charge { get; set; }
        public decimal pre_initiation_charge { get; set; }
        public decimal initiation_charge { get; set; }
        public string download_speed { get; set; }
        public string upload_speed { get; set; }
        public string product_type { get; set; }
        public string owner_product { get; set; }
        public string product_subtype { get; set; }
        public string product_subtype2 { get; set; }
        public string technology { get; set; }
        public string package_group { get; set; }
        public string package_code { get; set; }
        public string network_type { get; set; }
        public string customer_type { get; set; }
    }

    public class ReturnOntopPackageList
    {
        public string sff_promotion_code { get; set; }
        public string package_class { get; set; }
        public string sff_promotion_bill_tha { get; set; }
        public string sff_promotion_bill_eng { get; set; }
        public string package_name_tha { get; set; }
        public string package_name_eng { get; set; }
        public decimal recurring_charge { get; set; }
        public decimal pre_initiation_charge { get; set; }
        public decimal initiation_charge { get; set; }
        public string download_speed { get; set; }
        public string upload_speed { get; set; }
        public string product_type { get; set; }
        public string owner_product { get; set; }
        public string product_subtype { get; set; }
        public string product_subtype2 { get; set; }
        public string technology { get; set; }
        public string package_group { get; set; }
        public string package_code { get; set; }
        public string customer_type { get; set; }
    }

    public class ReturnServicePackageList
    {
        public string service_code { get; set; }
        public string product_name { get; set; }
    }

    public class ReturnDocumentSum
    {
        public string output_bulk_no { get; set; }
        public string output_return_code { get; set; }
        public string output_return_message { get; set; }
        public List<DocumentSumList> NewDocSumList { get; set; }
    }

    public class DocumentSumList
    {
        public string p_package_main_name { get; set; }
        public string p_install_charge { get; set; }
        public string p_discount_charge { get; set; }
        public string p_package_charge { get; set; }
        public string p_charge { get; set; }
        public string p_charge_by_one { get; set; }
        public string p_summary_charge { get; set; }
    }

    public class DetailWorkflow
    {
        public string CUSTOMER_TYPE { get; set; }
        public string CUSTOMER_SUBTYPE { get; set; }
        public string TITLE_CODE { get; set; }
        public string FIRST_NAME { get; set; }
        public string LAST_NAME { get; set; }
        public string CONTACT_TITLE_CODE { get; set; }
        public string CONTACT_FIRST_NAME { get; set; }
        public string CONTACT_LAST_NAME { get; set; }
        public string ID_CARD_TYPE_DESC { get; set; }
        public string ID_CARD_NO { get; set; }
        public string TAX_ID { get; set; }
        public string GENDER { get; set; }
        public string BIRTH_DATE { get; set; }
        public string MOBILE_NO { get; set; }
        public string MOBILE_NO_2 { get; set; }
        public string HOME_PHONE_NO { get; set; }
        public string EMAIL_ADDRESS { get; set; }
        public string CONTACT_TIME { get; set; }
        public string NATIONALITY_DESC { get; set; }
        public string REMARK { get; set; }
        public string HOUSE_NO { get; set; }
        public string MOO_NO { get; set; }
        public string BUILDING { get; set; }
        public string FLOOR { get; set; }
        public string ROOM { get; set; }
        public string MOOBAN { get; set; }
        public string SOI { get; set; }
        public string ROAD { get; set; }
        public string ZIPCODE_ROWID { get; set; }
        public string LATITUDE { get; set; }
        public string LONGTITUDE { get; set; }
        public string ASC_CODE { get; set; }
        public string EMPLOYEE_ID { get; set; }
        public string LOCATION_CODE { get; set; }
        public string SALE_REPRESENT { get; set; }
        public string CS_NOTE { get; set; }
        public string WIFI_ACCESS_POINT { get; set; }
        public string INSTALL_STATUS { get; set; }
        public string COVERAGE { get; set; }
        public string EXISTING_AIRNET_NO { get; set; }
        public string GSM_MOBILE_NO { get; set; }
        public string CONTACT_NAME_1 { get; set; }
        public string CONTACT_NAME_2 { get; set; }
        public string CONTACT_MOBILE_NO_1 { get; set; }
        public string CONTACT_MOBILE_NO_2 { get; set; }
        public string CONDO_FLOOR { get; set; }
        public string CONDO_ROOF_TOP { get; set; }
        public string CONDO_BALCONY { get; set; }
        public string BALCONY_NORTH { get; set; }
        public string BALCONY_SOUTH { get; set; }
        public string BALCONY_EAST { get; set; }
        public string BALCONY_WAST { get; set; }
        public string HIGH_BUILDING { get; set; }
        public string HIGH_TREE { get; set; }
        public string BILLBOARD { get; set; }
        public string EXPRESSWAY { get; set; }
        public string ADDRESS_TYPE_WIRE { get; set; }
        public string ADDRESS_TYPE { get; set; }
        public string FLOOR_NO { get; set; }
        public string HOUSE_NO_BI { get; set; }
        public string MOO_NO_BI { get; set; }
        public string MOOBAN_BI { get; set; }
        public string BUILDING_BI { get; set; }
        public string FLOOR_BI { get; set; }
        public string ROOM_BI { get; set; }
        public string SOI_BI { get; set; }
        public string ROAD_BI { get; set; }
        public string ZIPCODE_ROWID_BI { get; set; }
        public string HOUSE_NO_VT { get; set; }
        public string MOO_NO_VT { get; set; }
        public string MOOBAN_VT { get; set; }
        public string BUILDING_VT { get; set; }
        public string FLOOR_VT { get; set; }
        public string ROOM_VT { get; set; }
        public string SOI_VT { get; set; }
        public string ROAD_VT { get; set; }
        public string ZIPCODE_ROWID_VT { get; set; }
        public string CVR_ID { get; set; }
        public string CVR_NODE { get; set; }
        public string CVR_TOWER { get; set; }
        public string SITE_CODE { get; set; }
        public string RELATE_MOBILE { get; set; }
        public string RELATE_NON_MOBILE { get; set; }
        public string SFF_CA_NO { get; set; }
        public string SFF_SA_NO { get; set; }
        public string SFF_BA_NO { get; set; }
        public string NETWORK_TYPE { get; set; }
        public string SERVICE_DAY { get; set; }
        public string EXPECT_INSTALL_DATE { get; set; }
        public string FTTX_VENDOR { get; set; }
        public string INSTALL_NOTE { get; set; }
        public string PHONE_FLAG { get; set; }
        public string TIME_SLOT { get; set; }
        public string INSTALLATION_CAPACITY { get; set; }
        public string ADDRESS_ID { get; set; }
        public string ACCESS_MODE { get; set; }
        public string ENG_FLAG { get; set; }
        public string EVENT_CODE { get; set; }
        public string INSTALL_ADDRESS_1 { get; set; }
        public string INSTALL_ADDRESS_2 { get; set; }
        public string INSTALL_ADDRESS_3 { get; set; }
        public string INSTALL_ADDRESS_4 { get; set; }
        public string INSTALL_ADDRESS_5 { get; set; }
        public string PBOX_COUNT { get; set; }
        public string CONVERGENCE_FLAG { get; set; }
        public string TIME_SLOT_ID { get; set; }
        public string GIFT_VOUCHER { get; set; }
        public string SUB_LOCATION_ID { get; set; }
        public string SUB_CONTRACT_NAME { get; set; }
        public string INSTALL_STAFF_ID { get; set; }
        public string INSTALL_STAFF_NAME { get; set; }
        public string FLOW_FLAG { get; set; }
        public string LINE_ID { get; set; }
        public string RELATE_PROJECT_NAME { get; set; }
        public string PLUG_AND_PLAY_FLAG { get; set; }
        public string RESERVED_ID { get; set; }
        public string JOB_ORDER_TYPE { get; set; }
        public string ASSIGN_RULE { get; set; }
        public string OLD_ISP { get; set; }
        public string SPLITTER_FLAG { get; set; }
        public string RESERVED_PORT_ID { get; set; }
        public string SPECIAL_REMARK { get; set; }
        public string ORDER_NO { get; set; }
        public string SOURCE_SYSTEM { get; set; }
        public string BILL_MEDIA { get; set; }
        public string RENTAL_FLAG { get; set; } // new R17.9
        // R18.7
        public string PARTNER_TYPE { get; set; }
        public string PARTNER_SUBTYPE { get; set; }
        public string MOBILE_BY_ASC { get; set; }
        public string LOCATION_NAME { get; set; }
        public string PAYMENTMETHOD { get; set; }
        public string TRANSACTIONID_IN { get; set; }
        public string TRANSACTIONID { get; set; }
        // R18.8
        public string SUB_ACCESS_MODE { get; set; }
        public string REQUEST_SUB_FLAG { get; set; }
        public string PREMIUM_FLAG { get; set; }
        public string RELATE_MOBILE_SEGMENT { get; set; }
        public string REF_UR_NO { get; set; }
        public string LOCATION_EMAIL_BY_REGION { get; set; }
    }

    public class DetailSFF
    {
        public string REFERENCENO { get; set; }
        public string ACCOUNTCAT { get; set; }
        public string ACCOUNTSUBCAT { get; set; }
        public string IDCARDTYPE { get; set; }
        public string IDCARDNO { get; set; }
        public string TITLENAME { get; set; }
        public string FIRSTNAME { get; set; }
        public string LASTNAME { get; set; }
        public string SANAME { get; set; }
        public string BANAME { get; set; }
        public string CANUMBER { get; set; }
        public string BANUMBER { get; set; }
        public string SANUMBER { get; set; }
        public string BIRTHDATE { get; set; }
        public string GENDER { get; set; }
        public string BILLNAME { get; set; }
        public string BILLCYCLE { get; set; }
        public string BILLLANGUAGE { get; set; }
        public string ENGFLAG { get; set; }
        public string ACCHOMENO { get; set; }
        public string ACCBUILDINGNAME { get; set; }
        public string ACCFLOOR { get; set; }
        public string ACCROOM { get; set; }
        public string ACCMOO { get; set; }
        public string ACCMOOBAN { get; set; }
        public string ACCSOI { get; set; }
        public string ACCSTREET { get; set; }
        public string ACCTUMBOL { get; set; }
        public string ACCAMPHUR { get; set; }
        public string ACCPROVINCE { get; set; }
        public string ACCZIPCODE { get; set; }
        public string BILLHOMENO { get; set; }
        public string BILLBUILDINGNAME { get; set; }
        public string BILLFLOOR { get; set; }
        public string BILLROOM { get; set; }
        public string BILLMOO { get; set; }
        public string BILLMOOBAN { get; set; }
        public string BILLSOI { get; set; }
        public string BILLSTREET { get; set; }
        public string BILLTUMBOL { get; set; }
        public string BILLAMPHUR { get; set; }
        public string BILLZIPCODE { get; set; }
        public string BILLPROVINCE { get; set; }
        public string USERID { get; set; }
        public string DEALERLOCATIONCODE { get; set; }
        public string ASCCODE { get; set; }
        public string ORDERREASON { get; set; }
        public string REMARK { get; set; }
        public string SAVATNAME { get; set; }
        public string SAVATADDRESS1 { get; set; }
        public string SAVATADDRESS2 { get; set; }
        public string SAVATADDRESS3 { get; set; }
        public string SAVATADDRESS4 { get; set; }
        public string SAVATADDRESS5 { get; set; }
        public string SAVATADDRESS6 { get; set; }
        public string CONTACTFIRSTNAME { get; set; }
        public string CONTACTLASTNAME { get; set; }
        public string CONTACTTITLE { get; set; }
        public string MOBILENUMBERCONTACT { get; set; }
        public string PHONENUMBERCONTACT { get; set; }
        public string EMAILADDRESS { get; set; }
        public string SAHOMENO { get; set; }
        public string SABUILDINGNAME { get; set; }
        public string SAFLOOR { get; set; }
        public string SAROOM { get; set; }
        public string SAMOO { get; set; }
        public string SAMOOBAN { get; set; }
        public string SASOI { get; set; }
        public string SASTREET { get; set; }
        public string SATUMBOL { get; set; }
        public string SAAMPHUR { get; set; }
        public string SAZIPCODE { get; set; }
        public string SAPROVINCE { get; set; }
        public string ORDERTYPE { get; set; }
        public string CHANNEL { get; set; }
        public string PROJECTNAME { get; set; }
        public string CABRANCHNO { get; set; }
        public string SABRANCHNO { get; set; }
        public string CHARGETYPE { get; set; }
        public string SOURCESYSTEM { get; set; }
        public string SUBCONTRACTOR { get; set; }
        public string INSTALLSTAFFID { get; set; }
        public string EMPLOYEEID { get; set; }
        public string BILLMEDIA { get; set; }

    }

    public class listServiceVdsl
    {
        public string PACKAGE_CODE { get; set; }
        public string DPNAME { get; set; }
        public string DPPORT { get; set; }
        public string DSLAMNAME { get; set; }
        public string DSLAMPORT { get; set; }
        public string IA { get; set; }
        public string INSTALLADDRESS1 { get; set; }
        public string INSTALLADDRESS2 { get; set; }
        public string INSTALLADDRESS3 { get; set; }
        public string INSTALLADDRESS4 { get; set; }
        public string INSTALLADDRESS5 { get; set; }
        public string LATITUDE { get; set; }
        public string LONGITUDE { get; set; }
        public string NETWORKPROVIDER { get; set; }
        public string ORDERNO { get; set; }
        public string PASSWORD { get; set; }
        public string RELATENUMBER { get; set; }
        public string TYPE { get; set; }
        public string CONTACTNAME { get; set; }
        public string CONTACTMOBILEPHONE { get; set; }
        public string ADDRESSID { get; set; }
        public string FLOWFLAG { get; set; }

    }

    public class listServiceVdslRouter
    {
        public string PACKAGE_CODE { get; set; }
        public string ACCESSTYPE { get; set; }
        public string BRAND { get; set; }
        public string MACADDRESS { get; set; }
        public string METERIALCODE { get; set; }
        public string MODEL { get; set; }
        public string SERIALNO { get; set; }
        public string SUBCONTRACTOR { get; set; }

    }

    public class PromoMainlist
    {
        public string PACKAGE_CODE { get; set; }
    }

    public class PromoOntoplist
    {
        public string PACKAGE_CODE { get; set; }
    }

    public class returnDetailWorkflowData
    {
        public string output_bulk_no { get; set; }
        public string output_return_code { get; set; }
        public string output_return_message { get; set; }
        public List<DetailWorkflow> WorkflowList { get; set; }

    }

    public class returnDetailSFFData
    {
        public string output_bulk_no { get; set; }
        public string output_return_code { get; set; }
        public string output_return_message { get; set; }
        public List<DetailSFF> SFFList { get; set; }

    }

    public class returnServiceVDSLData
    {
        public string output_bulk_no { get; set; }
        public string output_return_code { get; set; }
        public string output_return_message { get; set; }
        public List<listServiceVdsl> ServiceVDSL { get; set; }

    }

    public class returnServiceVDSLRouterData
    {
        public string output_bulk_no { get; set; }
        public string output_return_code { get; set; }
        public string output_return_message { get; set; }
        public List<listServiceVdslRouter> ServiceVDSLRouter { get; set; }

    }

    public class returnPromoMainData
    {
        public string output_bulk_no { get; set; }
        public string output_return_code { get; set; }
        public string output_return_message { get; set; }
        public List<PromoMainlist> ListProMain { get; set; }

    }

    public class returnPromoOntopData
    {
        public string output_bulk_no { get; set; }
        public string output_return_code { get; set; }
        public string output_return_message { get; set; }
        public List<PromoOntoplist> ListProOntop { get; set; }

    }

    public class returnInstanceData
    {
        public string output_bulk_no { get; set; }
        public string output_return_code { get; set; }
        public string output_return_message { get; set; }
        public List<InstanceList> ListInstance { get; set; }

    }

    public class InstanceList
    {
        public string PRODUCTINSTANCE { get; set; }
        public string MOBILENO { get; set; }
        public string SIMSERIALNO { get; set; }
        public string PROVINCECODE { get; set; }

    }

    public class returnregisterpkgData
    {
        public string output_bulk_no { get; set; }
        public string output_return_code { get; set; }
        public string output_return_message { get; set; }
        public List<airregistpkgList> airpkgList { get; set; }

    }

    public class airregistpkgList
    {
        public string TEMP_IA { get; set; }
        public string PRODUCT_SUBTYPE { get; set; }
        public string PACKAGE_TYPE { get; set; }
        public string PACKAGE_CODE { get; set; }
        public decimal PACKAGE_PRICE { get; set; }
        public string IDD_FLAG { get; set; }
        public string FAX_FLAG { get; set; }
        public string HOME_IP { get; set; }
        public string HOME_PORT { get; set; }
        public string MOBILE_FORWARD { get; set; }

    }

    public class airregistfileList
    {
        public string registFile { get; set; }
    }

    public class ReturnUploadData
    {
        public string output_bulk_no { get; set; }
        public string output_return_code { get; set; }
        public string output_return_message { get; set; }
    }

    public class ReturnCompletewf
    {
        public string output_bulk_no { get; set; }
        public string output_return_code { get; set; }
        public string output_return_message { get; set; }
    }

    public class ReturnCompletesff
    {
        public string output_bulk_no { get; set; }
        public string output_return_code { get; set; }
        public string output_return_message { get; set; }
    }

    public class ScreenBulkCorpSFFModel
    {
        public string OUTPUT_return_code { get; set; }
        public string OUTPUT_return_message { get; set; }
        public List<DetailSFF> P_CALL_SFF { get; set; }
        public List<listServiceVdsl> P_LIST_SERVICE_VDSL { get; set; }
        public List<listServiceVdslRouter> P_LIST_SERVICE_VDSL_ROUTER { get; set; }
        public List<PromoMainlist> P_SFF_PROMOTION_CUR { get; set; }
        public List<PromoOntoplist> P_SFF_PROMOTION_ONTOP_CUR { get; set; }
        public List<InstanceList> P_LIST_INSTANCE_CUR { get; set; }

    }

    public class returnDelExcel
    {
        public string output_bulk_no { get; set; }
        public string output_return_code { get; set; }
        public string output_return_message { get; set; }
    }

    public class RetPackageList
    {
        public List<ReturnPackageList> ReturnPackageList { get; set; }
        public string output_return_code { get; set; }
        public string output_return_message { get; set; }
    }

    public class RetGetAddrID
    {
        public string output_return_code { get; set; }
        public string output_return_message { get; set; }
    }
}
