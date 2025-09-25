using System;
using System.Collections.Generic;

namespace WBBEntity.PanelModels.FBBWebConfigModels
{
    public class ConfigurationPackagesModel
    {
        public int StatusPage { get; set; }
        public string SaveStatus { get; set; }
        public string SaveToPage2 { get; set; }
        public string SaveToPage3 { get; set; }
        public string SaveToPage4 { get; set; }
        public string SaveResult { get; set; }
        // Page Search

        public string SFFProductCodeSearch { get; set; }
        public string SFFProductNameThaiSearch { get; set; }
        public string SFFProductNameEngSearch { get; set; }

        // Page Save1

        public string SFFProductCode { get; set; }
        public string SFFProductNameThai { get; set; }
        public string SFFProductNameEng { get; set; }

        public string PACKAGE_TYPE { get; set; }
        public string VAS_SERVICE { get; set; }
        public string PACKAGE_FOR { get; set; }
        public string PACKAGE_CODE { get; set; }
        public string PACKAGE_CLASS { get; set; }
        public string PACKAGE_DISPLAY_WEB { get; set; }
        public string PACKAGE_NAME_THAI { get; set; }
        public string PACKAGE_NAME_ENG { get; set; }
        public string DOWNLOAD_SPEED { get; set; }
        public string UPLOAD_SPEED { get; set; }
        public string SALE_START_DATE { get; set; }
        public string SALE_END_DATE { get; set; }
        public decimal PRE_INITIATION_CHARGE { get; set; }
        public decimal INITIATION_CHARGE { get; set; }
        public decimal PRE_RECURRING_CHARGE { get; set; }
        public decimal RECURRING_CHARGE { get; set; }
        public string DISCOUNT_TYPE { get; set; }
        public decimal DISCOUNT_VALUE { get; set; }
        public decimal DISCOUNT_DAY { get; set; }
        public string PRODUCT_TYPE { get; set; }

        // Page Save2
        public string SFFProductCodeMapping { get; set; }
        public string PACKAGE_CODE_MAPPING { get; set; }


        public List<PackageType> ListPackageType { get; set; }

        public string VendorOrPartnerShow { get; set; }
        public List<VendorOrPartner> ListVendorOrPartner { get; set; }

        public string PackageTypeSearchShow { get; set; }
        public string PackageTypeSearch { get; set; }
        public List<PackageTypeSearch> ListPackageTypeSearchShow { get; set; }

        // Page Save3
        public string SFFProductCodeCatalog { get; set; }
        public string PACKAGE_CODE_CATALOG { get; set; }
        public List<CatalogAndAuthorizeTable> ListCatalogAndAuthorizeTable { get; set; }

        // Page Save4
        public string SFFProductCodeLocation { get; set; }
        public string PACKAGE_CODE_LOCATION { get; set; }
        public string BuildingSearch { get; set; }
        public List<RegionTable> RegionUse { get; set; }
        public List<RegionTable> ListRegionTable { get; set; }
        public List<ProvinceTable> ListProvinceTable { get; set; }
        public List<TechnologyTable> ListTechnologyTable { get; set; }

        // Page Report
        public string PACKAGE_CODE_REPORT { get; set; }
        public string SFFProductCodeReport { get; set; }



    }

    public class NewPackageMaster
    {
        public string sff_promotion_code { get; set; }
        public string sff_promotion_bill_tha { get; set; }
        public string sff_promotion_bill_eng { get; set; }
        public string package_code { get; set; }
        public string package_type { get; set; }
        public string package_class { get; set; }
        public string sale_start_date { get; set; }
        public string sale_end_date { get; set; }
        public decimal pre_initiation_charge { get; set; }
        public decimal initiation_charge { get; set; }
        public decimal pre_recurring_charge { get; set; }
        public decimal recurring_charge { get; set; }
        public string package_name_tha { get; set; }
        public string package_name_eng { get; set; }
        public string download_speed { get; set; }
        public string upload_speed { get; set; }
        public string discount_type { get; set; }
        public decimal discount_value { get; set; }
        public decimal discount_day { get; set; }
        public string vas_service { get; set; }
    }

    public class PackageType
    {
        public bool PACKAGE_SELECTOld { get; set; }
        public bool PACKAGE_SELECT { get; set; }
        public string PACKAGE_TYPE { get; set; }
        public string PACKAGE_TYPE_DISPLAY { get; set; }
        public string PACKAGE_GROUP { get; set; }
        public string PACKAGE_GROUP_DESCRIPTION_THAI { get; set; }
        public string PACKAGE_GROUP_DESCRIPTION_THAI_OLD { get; set; }
        public string PACKAGE_GROUP_DESCRIPTION_ENG { get; set; }
        public string PACKAGE_GROUP_DESCRIPTION_ENG_OLD { get; set; }
        public string PRODUCT_SUBTYPE { get; set; }
        public string PRODUCT_SUBTYPE3 { get; set; }
        public string NETWORK_TYPE { get; set; }
        public decimal SERVICE_DAY_STARY { get; set; }
        public decimal SERVICE_DAY_END { get; set; }

    }

    public class PackageTypeSearch
    {
        public bool package_type_select { get; set; }
        public string package_type { get; set; }
        public string package_code { get; set; }
        public string sff_promotion_code { get; set; }
        public string package_name_tha { get; set; }
        public string sff_promotion_bill_tha { get; set; }
        public string sff_promotion_bill_eng { get; set; }
        public string sale_start_date { get; set; }
        public string sale_end_date { get; set; }
        public string effective_date { get; set; }
        public string expire_date { get; set; }
        public string mapping_code { get; set; }
        public string mapping_product { get; set; }
    }

    public class PackageTypeUse
    {
        public bool package_type_select { get; set; }
        public string package_type { get; set; }
        public string package_code { get; set; }
        public string sff_promotion_code { get; set; }
        public string package_name_tha { get; set; }
        public string sff_promotion_bill_tha { get; set; }
        public string sff_promotion_bill_eng { get; set; }
        public string sale_start_date { get; set; }
        public string sale_end_date { get; set; }
        public string effective_dtm { get; set; }
        public string expire_dtm { get; set; }
        public string mapping_code { get; set; }
        public string mapping_product { get; set; }
    }

    public class VendorOrPartner
    {
        public bool VendorOrPartnerSelectOld { get; set; }
        public bool VendorOrPartnerSelect { get; set; }
        public string VendorOrPartnerName { get; set; }
        public string VendorOrPartnerValue { get; set; }

    }

    public class PackageGroup
    {
        public string PACKAGE_GROUP { get; set; }
    }

    public class PackageGroupDesc
    {
        public string PackageGroupName { get; set; }
        public string PackageGroupDescriptionThai { get; set; }
        public string PackageGroupDescriptionEng { get; set; }
    }

    public class ProductSubtype3
    {
        public string PRODUCT_SUBTYPE3 { get; set; }

    }

    public class ConfigurationPackageDetail
    {
        public decimal return_code { get; set; }
        public string package_code { get; set; }
        public string package_type { get; set; }
        public string package_class { get; set; }
        public string sale_start_date { get; set; }
        public string sale_end_date { get; set; }
        public decimal pre_initiation_charge { get; set; }
        public decimal initiation_charge { get; set; }
        public decimal pre_recurring_charge { get; set; }
        public decimal recurring_charge { get; set; }
        public string package_name_tha { get; set; }
        public string package_name_eng { get; set; }
        public string sff_promotion_code { get; set; }
        public string sff_promotion_bill_tha { get; set; }
        public string sff_promotion_bill_eng { get; set; }
        public string download_speed { get; set; }
        public string upload_speed { get; set; }
        public string discount_type { get; set; }
        public decimal discount_value { get; set; }
        public decimal discount_day { get; set; }
        public string vas_service { get; set; }
        public string product_subtype2 { get; set; }
        public string technology { get; set; }
    }

    public class ProductTypePackage
    {
        public string package_code { get; set; }
        public string product_type { get; set; }
        public string product_subtype { get; set; }
        public string product_subtype3 { get; set; }
        public string package_group { get; set; }
        public string network_type { get; set; }
        public decimal service_day_start { get; set; }
        public decimal service_day_end { get; set; }
    }

    public class VendorPartner
    {
        public string owner_product { get; set; }
    }

    public class CatalogAndAuthorizeTable
    {
        public bool CatalogAndAuthorizeTableSelectOld { get; set; }
        public bool CatalogAndAuthorizeTableSelect { get; set; }
        public string CatalogAndAuthorizeTableName { get; set; }
        public string CatalogAndAuthorizeTableValue { get; set; }
        public string CatalogAndAuthorizeTableEffective { get; set; }
        public string CatalogAndAuthorizeTableExpire { get; set; }
    }

    public class ChannelGroup
    {
        public string CatalogAndAuthorizeName { get; set; }
    }

    public class PackageUserGroup
    {
        public string CatalogAndAuthorizeName { get; set; }
    }

    public class RegionTable
    {
        public string RegionTableName { get; set; }
        public DateTime? EffectiveDtm { get; set; }
        public DateTime? ExpireDtm { get; set; }
    }

    public class RegionTables
    {
        public string[] RegionTableName { get; set; }
    }

    public class ZipCode
    {
        public string SUB_REGION { get; set; }
    }

    public class ProvinceTable
    {
        public bool ProvinceSelect { get; set; }
        public string ProvinceName { get; set; }
        public string SubRegion { get; set; }
        public DateTime? EffectiveDtm { get; set; }
        public DateTime? ExpireDtm { get; set; }
    }

    public class ZipCodeProvince
    {
        public bool ProvinceSelect { get; set; }
        public string ProvinceName { get; set; }
        public string SUB_REGION { get; set; }

    }

    public class TechnologyTable
    {
        public string TechnologyName { get; set; }
    }

    public class BuildingDetail
    {
        public string building_name { get; set; }
        public string building_no { get; set; }
        public string address_id { get; set; }
        public string address_type { get; set; }
        public string building_name_eng { get; set; }
        public string building_no_eng { get; set; }
        public DateTime? EffectiveDtm { get; set; }
        public DateTime? ExpireDtm { get; set; }
    }

    public class BuildingTable
    {
        public bool BuildingSelect { get; set; }
        public string BuildingName { get; set; }
        public string BuildingNo { get; set; }
        public string AddressID { get; set; }
        public string AddressType { get; set; }
        public string BuildingNameEng { get; set; }
        public string BuildingNoEng { get; set; }
        public DateTime? EffectiveDtm { get; set; }
        public DateTime? ExpireDtm { get; set; }
    }

    public class BuildingSearch
    {
        public string Building { get; set; }
        public string[] Technology { get; set; }
    }

    public class SummaryPackageSearch
    {
        public string PACKAGE_CODE { get; set; }
    }

    public class SummaryPackageMaster
    {
        public string package_code { get; set; }
        public string package_type { get; set; }
        public string product_subtype { get; set; }
        public string product_subtype2 { get; set; }
        public string owner_product { get; set; }
        public string technology { get; set; }
        public string package_group { get; set; }
        public string sale_start_date { get; set; }
        public string sale_end_date { get; set; }
        public decimal pre_initiation_charge { get; set; }
        public decimal initiation_charge { get; set; }
        public decimal pre_recurring_charge { get; set; }
        public decimal recurring_charge { get; set; }
        public string package_name_tha { get; set; }
        public string package_name_eng { get; set; }
        public string sff_promotion_code { get; set; }
        public string sff_promotion_bill_tha { get; set; }
        public string sff_promotion_bill_eng { get; set; }
        public string download_speed { get; set; }
        public string upload_speed { get; set; }
        public string discount_type { get; set; }
        public decimal discount_value { get; set; }
        public decimal discount_day { get; set; }
        public string vas_service { get; set; }
    }

    public class SummaryPackageMapping
    {
        public string mapping_code { get; set; }
        public string mapping_product { get; set; }
        public string sff_promotion_code { get; set; }
        public string sff_promotion_bill_tha { get; set; }
        public string sff_promotion_bill_eng { get; set; }
        public string effective_dtm { get; set; }
        public string expire_dtm { get; set; }
    }

    public class SummaryPackageUser
    {
        public string user_group { get; set; }
        public string sff_promotion_code { get; set; }
        public string sff_promotion_bill_tha { get; set; }
        public string sff_promotion_bill_eng { get; set; }
        public string effective_dtm { get; set; }
        public string expire_dtm { get; set; }
    }

    public class SummaryPackageLoc
    {
        public string region { get; set; }
        public string province { get; set; }
        public string building_name { get; set; }
        public string building_no { get; set; }
        public string sff_promotion_code { get; set; }
        public string sff_promotion_bill_tha { get; set; }
        public string sff_promotion_bill_eng { get; set; }
        public string effective_dtm { get; set; }
        public string expire_dtm { get; set; }
    }
}
