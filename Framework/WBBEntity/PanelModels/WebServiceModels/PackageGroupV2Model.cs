using System.Collections.Generic;

namespace WBBEntity.PanelModels.WebServiceModels
{
    public class PackageDataV2Model
    {
        public List<PackageGroupV2Model> PackageGroupList { get; set; }
        public List<PackageV2Model> PackageList { get; set; }
    }

    public class PackageGroupV2Model
    {
        public string PACKAGE_GROUP { get; set; }
        public string PACKAGE_GROUP_DESC_ENG { get; set; }
        public string PACKAGE_GROUP_DESC_THA { get; set; }
        public string PACKAGE_REMARK_ENG { get; set; }
        public string PACKAGE_REMARK_THA { get; set; }
        public string PRODUCT_SUBTYPE { get; set; }

        public List<PackageV2Model> PackageItems { get; set; }
    }

    public class PackageV2Model
    {
        public string ACCESS_MODE { get; set; }
        public string AUTO_MAPPING_PROMOTION_CODE { get; set; }
        public string DISCOUNT_DAY { get; set; }
        public string DISCOUNT_TYPE { get; set; }
        public string DISCOUNT_VALUE { get; set; }
        public string DISPLAY_FLAG { get; set; }
        public string DISPLAY_SEQ { get; set; }
        public string DOWNLOAD_SPEED { get; set; }
        public string MAPPING_CODE { get; set; }
        public string OWNER_PRODUCT { get; set; }
        public string PACKAGE_DISPLAY_ENG { get; set; }
        public string PACKAGE_DISPLAY_THA { get; set; }
        public string PACKAGE_FOR_SALE_FLAG { get; set; }
        public string PACKAGE_GROUP { get; set; }
        public string PACKAGE_GROUP_DESC_ENG { get; set; }
        public string PACKAGE_GROUP_DESC_THA { get; set; }
        public string PACKAGE_GROUP_SEQ { get; set; }
        public string PACKAGE_REMARK_ENG { get; set; }
        public string PACKAGE_REMARK_THA { get; set; }
        public string PACKAGE_SERVICE_CODE { get; set; }
        public string PACKAGE_TYPE { get; set; }
        public string PACKAGE_TYPE_DESC { get; set; }
        public string PRE_PRICE_CHARGE { get; set; }
        public string PRICE_CHARGE { get; set; }
        public string PRODUCT_SUBTYPE { get; set; }
        public string PRODUCT_SUBTYPE3 { get; set; }
        public string SERVICE_CODE { get; set; }
        public string SFF_PRODUCT_CLASS { get; set; }
        public string SFF_PRODUCT_NAME { get; set; }
        public string SFF_PROMOTION_CODE { get; set; }
        public string SFF_WORD_IN_STATEMENT_ENG { get; set; }
        public string SFF_WORD_IN_STATEMENT_THA { get; set; }
        public string SUB_SEQ { get; set; }
        public string UPLOAD_SPEED { get; set; }
        public string MOBILE_PRICE { get; set; }
        public string EXISTING_MOBILE { get; set; }
        public string CUSTOMER_TYPE { get; set; }
        public string INSTALL_SHOW { get; set; }
        public string ENTRYFEE_SHOW { get; set; }
        public string PACKAGE_DURATION { get; set; }
        public string PRICE_CHARGE_VAT { get; set; }
    }

    public class PackageDataV2ToViewModel
    {
        public string owenerProduct { get; set; }
        public string fmpa_flag { get; set; }
        public string cvm_flag { get; set; }
        public string fmc_special_flag { get; set; }
        public string mou_flag { get; set; } //R21.10 MOU
        public List<PackageGroupV2Model> packageGroupList { get; set; }
        public List<PackageV2Model> packageMainList { get; set; } //R21.11 ATV
        public List<PackageV2Model> packageInstallList { get; set; }
        public List<PackageV2Model> packageInstallList2 { get; set; }
        public List<PackageV2Model> packageAutoList { get; set; }
        public List<PackageV2Model> packageDiscountList { get; set; }
        public List<PackageV2Model> packagePlayboxList { get; set; }
        public List<PackageV2Model> packageFixlineList { get; set; }
        public List<PackageV2Model> packageFixlineInstallList { get; set; }
        public List<PackageV2Model> packageContentPlayboxList { get; set; }
        public List<PackageV2Model> packageAutoContentPlayboxList { get; set; } //R21.11 ATV
        public List<PackageV2Model> packagePlayboxMonthlyFeeList { get; set; }
        public List<PackageV2Model> packagEntryFeeList { get; set; }
        public List<PackageV2Model> packagWiFiLogList { get; set; }
        public List<PackageV2Model> packageSuperMESHWifiList { get; set; }
        public List<PackageV2Model> packageAutoSuperMESHWifiList { get; set; } //R21.11 ATV
        public List<PackageV2Model> packageSpeedBoostList { get; set; }
        public List<PackageV2Model> packageAutoSuperMESHWifiListMeshArpu { get; set; } //mesh Arpu
        public List<PackageV2Model> packageIpCameraList { get; set; } //R23.06 IP Camera (Ontop IP Camera)
        public List<PackageV2Model> packageAutoIpCameraList { get; set; } //R23.08 IP Camera (Order Fee)
    }

    public class OnlineQueryConfigModel
    {
        public string Url { get; set; }
        public string UseSecurityProtocol { get; set; }
        public string ContentType { get; set; }
        public string Channel { get; set; }
        public string BodyStr { get; set; }
    }

    public class OnlineQueryConfigBody
    {
        public string SALE_CHANNEL { get; set; }
        public ProductSubtypeArray[] PRODUCT_SUBTYPE_ARRAY { get; set; }
        public string PACKAGE_FOR { get; set; }
        public string[] SFF_PROMOTION_ARRAY { get; set; }
        public string CUSTOMER_TYPE { get; set; }
        public string CUSTOMER_SUBTYPE { get; set; }
        public string PARTNER_TYPE { get; set; }
        public string PARTNER_SUBTYPE { get; set; }
        public string DISTRIBUTION_CHANNEL { get; set; }
        public string CHANNEL_SALES_GROUP { get; set; }
        public string SHOP_SEGMENT { get; set; }
        public string LOCATION_CODE { get; set; }
        public string ASC_CODE { get; set; }
        public string EMPLOYEE_ID { get; set; }
        public string REGION { get; set; }
        public string PROVINCE { get; set; }
        public string DISTRICT { get; set; }
        public string SUB_DISTRICT { get; set; }
        public string[] ADDRESS_ID_ARRAY { get; set; }
        public Project_Cond_Flag_Array[] PROJECT_COND_FLAG_ARRAY { get; set; }
        public string LOCATION_PROVINCE { get; set; }
    }

    public class ProductSubtypeArray
    {
        public string PRODUCT_SUBTYPE { get; set; }
        public string OWNER_PRODUCT { get; set; }
    }

    public class Project_Cond_Flag_Array
    {
        public string Flag { get; set; }
        public string Value { get; set; }
    }

    public class OnlineQueryConfigResult
    {
        public string RESULT_CODE { get; set; }
        public string RESULT_DESC { get; set; }
        public string TRANSACTION_ID { get; set; }
        public List<OnlineQueryPackage> LIST_PACKAGE_BY_SERVICE { get; set; }
    }

    public class OnlineQueryPackage
    {
        public string mapping_code { get; set; }
        public string package_service_code { get; set; }
        public string sff_promotion_code { get; set; }
        public string sff_product_name { get; set; }
        public string package_group { get; set; }
        public string package_group_desc_tha { get; set; }
        public string package_group_desc_eng { get; set; }
        public string package_remark_tha { get; set; }
        public string package_remark_eng { get; set; }
        public string package_group_seq { get; set; }
        public string price_charge { get; set; }
        public string pre_price_charge { get; set; }
        public string sff_word_in_statement_tha { get; set; }
        public string sff_word_in_statement_eng { get; set; }
        public string package_display_tha { get; set; }
        public string package_display_eng { get; set; }
        public string sff_product_class { get; set; }
        public string package_for_sale_flag { get; set; }
        public string sale_start_date { get; set; }
        public string sale_end_date { get; set; }
        public string download_speed { get; set; }
        public string upload_speed { get; set; }
        public string package_type { get; set; }
        public string package_type_desc { get; set; }
        public string product_subtype { get; set; }
        public string owner_product { get; set; }
        public string access_mode { get; set; }
        public string service_code { get; set; }
        public string product_subtype3 { get; set; }
        public string discount_type { get; set; }
        public string discount_value { get; set; }
        public string discount_day { get; set; }
        public string auto_mapping_promotion_code { get; set; }
        public string display_flag { get; set; }
        public string display_seq { get; set; }
        public string sub_seq { get; set; }
        public string customer_type { get; set; }
        public string package_duration { get; set; }
        public string price_charge_vat { get; set; }
    }

    public class ProductSubtypeCursor
    {
        public string ADDRESS_ID { get; set; }
        public string PRODUCT_SUBTYPE { get; set; }
        public string ADDRESS_TYPE { get; set; }
        public string ACCESS_MODE { get; set; }
        public string FTTR_FLAG { get; set; }
    }

    public class GetPackageListbySFFPromoOnlineRestult
    {
        public string RESULT_CODE { get; set; }
        public string RESULT_DESC { get; set; }
        public string TRANSACTION_ID { get; set; }
        public List<GetPackageListbySFFPromoOnlineQueryPackage> LIST_PACKAGE_BY_SFFPROMO { get; set; }
    }

    public class GetPackageListbySFFPromoOnlineQueryPackage
    {
        public string mapping_code { get; set; }
        public string package_service_code { get; set; }
        public string package_service_name { get; set; }
        public string sff_promotion_code { get; set; }
        public string package_name { get; set; }
        public string package_group { get; set; }
        public string package_group_desc_tha { get; set; }
        public string package_group_desc_eng { get; set; }
        public string package_remark_tha { get; set; }
        public string package_remark_eng { get; set; }
        public string package_group_seq { get; set; }
        public string price_charge { get; set; }
        public string pre_price_charge { get; set; }
        public string sff_word_in_statement_tha { get; set; }
        public string sff_word_in_statement_eng { get; set; }
        public string package_display_tha { get; set; }
        public string package_display_eng { get; set; }
        public string sff_product_class { get; set; }
        public string package_for_sale_flag { get; set; }
        public string sale_start_date { get; set; }
        public string sale_end_date { get; set; }
        public string download_speed { get; set; }
        public string upload_speed { get; set; }
        public string package_type { get; set; }
        public string package_type_desc { get; set; }
        public string product_subtype { get; set; }
        public string sff_product_name { get; set; }
        public string sff_product_package { get; set; }
        public string owner_product { get; set; }
        public string access_mode { get; set; }
        public string service_code { get; set; }
        public string product_subtype3 { get; set; }
        public string discount_type { get; set; }
        public string discount_value { get; set; }
        public string discount_day { get; set; }
        public string auto_mapping_promotion_code { get; set; }
        public string display_flag { get; set; }
        public string display_seq { get; set; }
        public string sub_seq { get; set; }
        public string customer_type { get; set; }

    }



}
