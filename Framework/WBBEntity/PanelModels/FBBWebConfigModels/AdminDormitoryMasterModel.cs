using System;
using System.Collections.Generic;

namespace WBBEntity.PanelModels.FBBWebConfigModels
{
    public class AdminDormitoryMasterModel
    {
        public string Admin_dormitory_id { get; set; }
        public string Admin_dormitory_name_th { get; set; }
        public string Admin_dormitory_name_en { get; set; }
        public string Admin_dormitory_no_th { get; set; }
        public string Admin_dormitory_no_en { get; set; }
        public string Admin_floor_no { get; set; }
        public string Admin_room_no { get; set; }
        public string Admin_dormitory_name_th_display { get; set; }
        public string Admin_netnumber { get; set; }
        public string Admin_SubcontractID { get; set; }
        public string Admin_SubcontractTH { get; set; }
        public string Admin_SubcontractEN { get; set; }
        public string Admin_HOME_NO_TH { get; set; }
        public string Admin_MOO_TH { get; set; }
        public string Admin_SOI_TH { get; set; }
        public string Admin_STREET_NAME_TH { get; set; }
        public string Admin_HOME_NO_EN { get; set; }
        public string Admin_MOO_EN { get; set; }
        public string Admin_SOI_EN { get; set; }
        public string Admin_STREET_NAME_EN { get; set; }
        public string Admin_AddressID { get; set; }
        public string Admin_AddressTH { get; set; }
        public string Admin_AddressEN { get; set; }
        public string Admin_ZipcodeID_TH { get; set; }
        public string Admin_ZipcodeID_EN { get; set; }
        public string Admin_TUMBON_TH { get; set; }
        public string Admin_AMPHUR_TH { get; set; }
        public string Admin_Province_TH { get; set; }
        public string Admin_TUMBON_EN { get; set; }
        public string Admin_AMPHUR_EN { get; set; }
        public string Admin_Province_EN { get; set; }
        public string Admin_Language { get; set; }
        public string Admin_Postcode_TH { get; set; }
        public string Admin_Postcode_EN { get; set; }
        public string Admin_SubcontractId { get; set; }
        public string Admin_Contact_Name { get; set; }
        public string Admin_contract_email { get; set; }
        public string Admin_contract_phone { get; set; }
        public string Admin_target_launch_dt { get; set; }
        public string Admin_launch_dt { get; set; }
        public string Admin_target_volumn { get; set; }
        public string Admin_volumn { get; set; }

    }

    public class DormitorySearchPara
    {
        public string Region { get; set; }
        public string Province { get; set; }
        public string DormitoryName { get; set; }
        public string Building { get; set; }
        public string Status { get; set; }
    }

    public class DormitorySearchModel
    {
        public string DormitoryProvincr { get; set; }
        public string DormitoryName { get; set; }
        public string Building { get; set; }
        public string Status { get; set; }
    }

    public class ConfigurationDormitoryData
    {
        public string region_code { get; set; }
        public string dormitory_id { get; set; }
        public string province { get; set; }
        public string dormitory_name_th { get; set; }
        public decimal Room_amount { get; set; }
    }

    public class ConfigurationPrepaidNonMobileData
    {
        public string region_code { get; set; }
        public string province { get; set; }
        public string dormitory_name_th { get; set; }
        public string dormitory_no_th { get; set; }
        public string floor_no { get; set; }
        public string room_no { get; set; }
        public string prepaid_non_mobile { get; set; }
        public string pin_code { get; set; }
        public string service_status { get; set; }

    }
    public class AddnewPrepaidNonMobile
    {
        public string indexPrefix { get; set; }
        public string prefix { get; set; }
        public string floor { get; set; }
        public string from { get; set; }
        public string to { get; set; }
    }
    public class ConfigurationDormitorySubcontract
    {
        public string dormitory_id { get; set; }
        public string region_code { get; set; }
        public string province { get; set; }
        public string DORMITORY_NAME_TH { get; set; }
        public string SUB_CONTRACT_LOCATION_CODE { get; set; }
        public string SUB_CONTRACT_NAME_TH { get; set; }
        public string SUB_CONTRACT_NAME_EN { get; set; }
        public string PRICE_INSTALL { get; set; }
    }
    public class ConfigurationAddressID
    {
        public string Region { get; set; }
        public string dormitory_id { get; set; }
        public string province { get; set; }
        public string DORMITORY_NAME_TH { get; set; }
        public string DORMITORY_NO_TH { get; set; }
        public string ADDRESS_ID { get; set; }
    }
    public class ConfigurationOnOffService
    {
        public string province { get; set; }
        public string DORMITORY_NAME_TH { get; set; }
        public string BUILDING_NO_TH { get; set; }
        public string Room_Amount { get; set; }
        public string Description { get; set; }
        public string ADDRESS_ID { get; set; }
        public string Subcontract { get; set; }
        public string Status { get; set; }


    }
    public class ConfigurationOnOffServices
    {
        public decimal dormitory_id { get; set; }
        public string region_code { get; set; }
        public string province { get; set; }
        public string dormitory_name { get; set; }
        public string BUILDING { get; set; }
        public string Room_Amount { get; set; }
        public string Detail_dormitory { get; set; }
        public string ADDRESS_ID { get; set; }
        public string Subcontract { get; set; }
        public string Status { get; set; }
        public string StatusOld { get; set; }
        public string StatusResult { get; set; }
        public decimal result { get; set; }

    }

    public class ConfigurationDormitorySubcontractModel
    {
        public string dormitory_id { get; set; }
        public string province { get; set; }
        public string dormitory_name_th { get; set; }
        public string subcontract_code { get; set; }
        public string subcontract_name_th { get; set; }
        public string subcontract_name_en { get; set; }
        public decimal price_sub { get; set; }
    }

    public class ConfigurationDormitoryModel
    {
        public string p_dormitory_id { get; set; }

        public string p_return_code { get; set; }
        public string p_return_message { get; set; }

        public string p_dormitory_name_th { get; set; }
        public string p_home_no_th { get; set; }
        public string p_moo_th { get; set; }
        public string p_soi_th { get; set; }
        public string p_Street_th { get; set; }
        public string p_tumbol_th { get; set; }
        public string p_amphur_th { get; set; }
        public string p_province_th { get; set; }
        public string p_zipcode_th { get; set; }

        public string p_dormitory_name_en { get; set; }
        public string p_home_no_en { get; set; }
        public string p_moo_en { get; set; }
        public string p_soi_en { get; set; }
        public string p_Street_en { get; set; }
        public string p_tumbol_en { get; set; }
        public string p_amphur_en { get; set; }
        public string p_province_en { get; set; }
        public string p_zipcode_en { get; set; }
        public string p_target_launch_dt { get; set; }
        public string p_launch_dt { get; set; }
        public string p_target_volumn { get; set; }
        public string p_volumn { get; set; }
        public string p_dorm_contract_name { get; set; }
        public string p_dorm_contract_email { get; set; }
        public string p_dorm_contract_phone { get; set; }
        public List<ConfigurationDormitoryBuildingData> ConfigurationDormitoryBuildingDataList { get; set; }
    }

    public class ConfigurationDormitoryBuildingData
    {
        public string dormitory_id { get; set; }
        public string dormitory_no_th { get; set; }
        public string dormitory_no_en { get; set; }
        public string number_of_room { get; set; }
        public string state { get; set; }
        public decimal result { get; set; }
    }

    public class DormitoryMasterModel
    {
        public string Mode { get; set; }
        public string dormitory_id { get; set; }
        public string dormitory_name_th { get; set; }
        public string dormitory_name_en { get; set; }
        public string dormitory_name_th_display { get; set; }
        public string netnumber { get; set; }
        public string SubcontractID { get; set; }
        public string SubcontractTH { get; set; }
        public string SubcontractEN { get; set; }
        public string HOME_NO_TH { get; set; }
        public string MOO_TH { get; set; }
        public string SOI_TH { get; set; }
        public string STREET_NAME_TH { get; set; }
        public string HOME_NO_EN { get; set; }
        public string MOO_EN { get; set; }
        public string SOI_EN { get; set; }
        public string STREET_NAME_EN { get; set; }
        public string AddressID { get; set; }
        public string AddressTH { get; set; }
        public string AddressEN { get; set; }
        public string ZipcodeID_TH { get; set; }
        public string ZipcodeID_EN { get; set; }
        public string TUMBON_TH { get; set; }
        public string AMPHUR_TH { get; set; }
        public string Province_TH { get; set; }
        public string TUMBON_EN { get; set; }
        public string AMPHUR_EN { get; set; }
        public string Province_EN { get; set; }
        public string Language { get; set; }
        public string Postcode_TH { get; set; }
        public string Postcode_EN { get; set; }
        public string SubcontractId { get; set; }
        public string Contact_Name { get; set; }
        public string contract_email { get; set; }
        public string contract_phone { get; set; }
        public DateTime target_launch_dt { get; set; }
        public DateTime launch_dt { get; set; }
        public string target_volumn { get; set; }
        public string volumn { get; set; }

    }

    public class DormitoryBuilding
    {
        public string dormitory_id { get; set; }
        public string dormitory_no_th { get; set; }
        public string dormitory_no_en { get; set; }
        public string dormitory_no_th_old { get; set; }
        public string number_of_room { get; set; }
        public string state { get; set; }
        public string mode { get; set; }
        public string indexBuilding { get; set; }
    }
    public class DormitorySubcontract
    {
        public string DORMITORY_NAME_TH { get; set; }
        public string SUB_CONTRACT_LOCATION_CODE { get; set; }
        public string SUB_CONTRACT_NAME_TH { get; set; }
        public string SUB_CONTRACT_NAME_EN { get; set; }
        public string PRICE_INSTALL { get; set; }
        public string indexSubcontract { get; set; }
    }

    public class DuplicateMobileList
    {
        public string NON_MOBILE_NO { get; set; }
        public string Customer_Name { get; set; }
        public string Order_Status { get; set; }
        public string MOBILE_NO { get; set; }
        public string RELATE_MOBILE { get; set; }
        public string Email_Address { get; set; }
        public string Location_Code { get; set; }
        public string ASC_Code { get; set; }
    }
    public class DuplicateMobileModel
    {
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
    }

    public class ConfigurationFibrenetID
    {
        public string FibrenetID { get; set; }
        public string Region { get; set; }
        public string Province { get; set; }
        public string DormitoryName { get; set; }
        public string Building { get; set; }
        public string FloorNo { get; set; }
        public string RoomNo { get; set; }
        public string Status { get; set; }
    }


    public class DormitorySearchFibrenetID
    {
        public string Region { get; set; }
        public string Province { get; set; }
        public string DormitoryName { get; set; }
        public string FibrenetIDFlag { get; set; }
    }
}
