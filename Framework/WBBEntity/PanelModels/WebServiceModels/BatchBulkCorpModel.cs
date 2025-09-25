using System;
using System.Collections.Generic;

namespace WBBEntity.PanelModels.WebServiceModels
{
    public class BatchBulkCorpModel
    {
        public List<DetailWfCallWorkFlow> P_CALL_WORKFLOW { get; set; }
        public List<DetailWfAirRegistFileArray> AIR_REGIST_FILE_ARRAY { get; set; }
        public List<DetailWfAirRegistPackageArray> AIR_REGIST_PACKAGE_ARRAY { get; set; }
        public List<DetailWfAirRegistSplitterArray> AIR_REGIST_SPLITTER_ARRAY { get; set; }
        public List<DetailWfAirRegistCpeSerialArray> AIR_REGIST_CPE_SERIAL_ARRAY { get; set; }
        public string OUTPUT_BULK_NO { get; set; }
        public string OUTPUT_RETURN_CODE { get; set; }
        public string OUTPUT_RETURN_MESSAGE { get; set; }
    }
    public class DetailWfCallWorkFlow
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
        public string CUSTOMER_REMARK { get; set; }
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
        public string HOUSE_NO_BL { get; set; }
        public string MOO_NO_BL { get; set; }
        public string MOOBAN_BL { get; set; }
        public string BUILDING_BL { get; set; }
        public string FLOOR_BL { get; set; }
        public string ROOM_BL { get; set; }
        public string SOI_BL { get; set; }
        public string ROAD_BL { get; set; }
        public string ZIPCODE_ROWID_BL { get; set; }
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
        public string INSTALLADDRESS1 { get; set; }
        public string INSTALLADDRESS2 { get; set; }
        public string INSTALLADDRESS3 { get; set; }
        public string INSTALLADDRESS4 { get; set; }
        public string INSTALLADDRESS5 { get; set; }
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
        public string PRE_ORDER_NO { get; set; }
        public string VOUCHER_DESC { get; set; }
        public string CAMPAIGN_PROJECT_NAME { get; set; }
        public string PRE_ORDER_CHANEL { get; set; }
        public string RENTAL_FLAG { get; set; }  //new R17.9
        public string DEV_PROJECT_CODE { get; set; }
        public string DEV_BILL_TO { get; set; }
        public string DEV_PO_NO { get; set; }
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
    public class DetailWfAirRegistFileArray
    {
        public string PATH_FILE { get; set; }
    }
    public class DetailWfAirRegistPackageArray
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

    public class DetailWfAirRegistSplitterArray
    {
        public string SPLITTER_NAME { get; set; }
        public decimal DISTANCE { get; set; }
        public string DISTANCE_TYPE { get; set; }
        public string RESOURCE_TYPE { get; set; }
    }

    public class DetailWfAirRegistCpeSerialArray
    {
        public string CPE_TYPE { get; set; }
        public string SERIAL_NO { get; set; }
        public string MAC_ADDRESS { get; set; }
    }

    public class BatchBulkCorpSFFModel
    {
        public List<DetailBulkCorpSFF> P_CALL_SFF { get; set; }
        public List<DetailBulkCorpListServiceVdsl> P_LIST_SERVICE_VDSL { get; set; }
        public List<DetailBulkCorpListServiceVdslRouter> P_LIST_SERVICE_VDSL_ROUTER { get; set; }
        public List<DetailBulkCorpListServiceAppoint> P_LIST_SERVICE_APPOINT { get; set; }  //new
        public List<DetailBulkCorpSffPromotionCur> P_SFF_PROMOTION_CUR { get; set; }
        public List<DetailBulkCorpSffPromotionOntopCur> P_SFF_PROMOTION_ONTOP_CUR { get; set; }
        public List<DetailBulkCorpListInstanceCur> P_LIST_INSTANCE_CUR { get; set; }
        public string OUTPUT_return_code { get; set; }
        public string OUTPUT_return_message { get; set; }
    }

    public class BatchBulkCorpSFFReturnModel
    {
        public List<DetailSffReturn> P_OUTPUT_SFF_DETAIL { get; set; }
        public string OUTPUT_return_code { get; set; }
        public string OUTPUT_return_message { get; set; }
    }
    public class BatchBulkCorpUpdateSFFReturnModel
    {
        public string P_RETURN_CODE { get; set; }
        public string P_RETURN_MESSAGE { get; set; }
    }
    public class DetailSffReturn
    {
        public string P_ORDER_NO { get; set; }
        public string P_CA_NO { get; set; }
        public string P_BA_NO { get; set; }
        public string P_SA_NO { get; set; }
        public string P_MOBILE_NO { get; set; }
        public string p_error_reason { get; set; }
        public string p_interface_result { get; set; }
    }
    public class DetailBulkCorpSFF
    {
        public string p_referenceNo { get; set; }
        public string P_accountCat { get; set; }
        public string P_accountSubCat { get; set; }
        public string P_idCardType { get; set; }
        public string P_idCardNo { get; set; }
        public string P_titleName { get; set; }
        public string P_firstName { get; set; }
        public string P_lastName { get; set; }
        public string P_saName { get; set; }
        public string P_baName { get; set; }
        public string P_caNumber { get; set; }
        public string P_baNumber { get; set; }
        public string P_saNumber { get; set; }
        public string P_birthdate { get; set; }
        public string P_gender { get; set; }
        public string P_billName { get; set; }
        public string P_billCycle { get; set; }
        public string P_billLanguage { get; set; }
        public string P_engFlag { get; set; }
        public string P_accHomeNo { get; set; }
        public string P_accBuildingName { get; set; }
        public string P_accFloor { get; set; }
        public string P_accRoom { get; set; }
        public string P_accMoo { get; set; }
        public string P_accMooBan { get; set; }
        public string P_accSoi { get; set; }
        public string P_accStreet { get; set; }
        public string P_accTumbol { get; set; }
        public string P_accAmphur { get; set; }
        public string P_accProvince { get; set; }
        public string P_accZipCode { get; set; }
        public string P_billHomeNo { get; set; }
        public string P_billBuildingName { get; set; }
        public string P_billFloor { get; set; }
        public string P_billRoom { get; set; }
        public string P_billMoo { get; set; }
        public string P_billMooBan { get; set; }
        public string P_billSoi { get; set; }
        public string P_billStreet { get; set; }
        public string P_billTumbol { get; set; }
        public string P_billAmphur { get; set; }
        public string P_billZipCode { get; set; }
        public string P_billProvince { get; set; }
        public string P_userId { get; set; }
        public string P_dealerLocationCode { get; set; }
        public string P_ascCode { get; set; }
        public string P_orderReason { get; set; }
        public string P_remark { get; set; }
        public string P_saVatName { get; set; }
        public string P_saVatAddress1 { get; set; }
        public string P_saVatAddress2 { get; set; }
        public string P_saVatAddress3 { get; set; }
        public string P_saVatAddress4 { get; set; }
        public string P_saVatAddress5 { get; set; }
        public string P_saVatAddress6 { get; set; }
        public string P_contactFirstName { get; set; }
        public string P_contactLastName { get; set; }
        public string P_contactTitle { get; set; }
        public string P_mobileNumberContact { get; set; }
        public string P_phoneNumberContact { get; set; }
        public string P_emailAddress { get; set; }
        public string P_saHomeNo { get; set; }
        public string P_saBuildingName { get; set; }
        public string P_saFloor { get; set; }
        public string P_saRoom { get; set; }
        public string P_saMoo { get; set; }
        public string P_saMooBan { get; set; }
        public string P_saSoi { get; set; }
        public string P_saStreet { get; set; }
        public string P_saTumbol { get; set; }
        public string P_saAmphur { get; set; }
        public string P_saProvince { get; set; }
        public string P_saZipCode { get; set; }
        public string P_orderType { get; set; }
        public string P_channel { get; set; }
        public string P_projectName { get; set; }
        public string P_caBranchNo { get; set; }
        public string P_saBranchNo { get; set; }
        public string P_chargeType { get; set; }
        public string P_sourceSystem { get; set; }
        public string P_subcontractor { get; set; }
        public string P_installStaffID { get; set; }
        public string P_employeeID { get; set; }
        public string P_billmedia { get; set; }

    }
    public class DetailBulkCorpListServiceVdsl
    {
        public string p_sff_product_cd { get; set; }
        public string p_fixip { get; set; }
        public string p_addressId { get; set; }
        public string p_appointmentdate { get; set; }
        public string p_contactMobilePhone { get; set; }
        public string p_contactName { get; set; }
        public string p_dpName { get; set; }
        public string p_dpPort { get; set; }
        public string p_dslamName { get; set; }
        public string p_dslamPort { get; set; }
        public string p_flowFlag { get; set; }
        public string p_ia { get; set; }
        public string p_installAddress1 { get; set; }
        public string p_installAddress2 { get; set; }
        public string p_installAddress3 { get; set; }
        public string p_installAddress4 { get; set; }
        public string p_installAddress5 { get; set; }
        public string p_installationCapacity { get; set; }
        public string p_latitude { get; set; }
        public string p_longitude { get; set; }
        public string p_networkProvider { get; set; }
        public string p_orderNo { get; set; }
        public string p_password { get; set; }
        public string p_phoneFlag { get; set; }
        public string p_relateNumber { get; set; }
        public string p_reservedId { get; set; }
        public string p_timeSlot { get; set; }
        public string p_type { get; set; }


    }
    public class DetailBulkCorpListServiceVdslRouter
    {
        public string p_sff_product_cd { get; set; }
        public string p_accessType { get; set; }
        public string p_brand { get; set; }
        public string p_macAddress { get; set; }
        public string p_meterialCode { get; set; }
        public string p_model { get; set; }
        public string p_serialNo { get; set; }
        public string p_subContractor { get; set; }

    }

    public class DetailBulkCorpListServiceAppoint
    {
        public string p_sff_product_cd { get; set; }
        public string p_sysmptom { get; set; }
        public string p_appointmentDate { get; set; }
        public string p_fbbContactNo1 { get; set; }
        public string p_fbbContactNo2 { get; set; }
        public string p_installationCapacity { get; set; }
        public string p_playBoxAmount { get; set; }
        public string p_remarkForSubcontract { get; set; }
        public string p_reservedId { get; set; }
        public string p_reservedPort { get; set; }
        public string p_timeslot { get; set; }
        public string p_urgentFlag { get; set; }
    }

    public class DetailBulkCorpSffPromotionCur
    {
        public string p_sff_product_cd { get; set; }
    }
    public class DetailBulkCorpSffPromotionOntopCur
    {
        public string p_sff_product_cd { get; set; }
    }
    public class DetailBulkCorpListInstanceCur
    {
        public string p_productInstance { get; set; }
        public string p_mobileNo { get; set; }
        public string p_simSerialNo { get; set; }
        public string p_provinceCode { get; set; }

    }
    public class GetDetailBulkCorpRegister
    {
        public string ROW_ID { get; set; }
        public string CUSTOMER_TYPE { get; set; }
        public string CUSTOMER_SUBTYPE { get; set; }
        public string TITLE_CODE { get; set; }
        public string TITLENAME { get; set; }
        public string FIRST_NAME { get; set; }
        public string LAST_NAME { get; set; }
        public string CONTACT_TITLE_CODE { get; set; }
        public string CONTACT_FIRST_NAME { get; set; }
        public string CONTACT_LAST_NAME { get; set; }
        public string ID_CARD_TYPE_DESC { get; set; }
        public string ID_CARD_NO { get; set; }
        public string TAX_ID { get; set; }
        public string GENDER { get; set; }
        public DateTime? BIRTH_DATE { get; set; }
        public string MOBILE_NO { get; set; }
        public string MOBILE_NO_2 { get; set; }
        public string HOME_PHONE_NO { get; set; }
        public string EMAIL_ADDRESS { get; set; }
        public string CONTACT_TIME { get; set; }
        public string NATIONALITY_DESC { get; set; }
        public string CUSTOMER_REMARK { get; set; }
        public string HOUSE_NO { get; set; }
        public string MOO_NO { get; set; }
        public string BUILDING { get; set; }
        public string FLOOR { get; set; }
        public string ROOM { get; set; }
        public string MOOBAN { get; set; }
        public string SOI { get; set; }
        public string ROAD { get; set; }
        public string TUMBON { get; set; }
        public string AMPHUR { get; set; }
        public string PROVINCE { get; set; }
        public string ZIPCODE { get; set; }
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
        public string HOUSE_NO_BL { get; set; }
        public string MOO_NO_BL { get; set; }
        public string MOOBAN_BL { get; set; }
        public string BUILDING_BL { get; set; }
        public string FLOOR_BL { get; set; }
        public string ROOM_BL { get; set; }
        public string SOI_BL { get; set; }
        public string ROAD_BL { get; set; }
        public string TUMBON_BL { get; set; }
        public string AMPHUR_BL { get; set; }
        public string PROVINCE_BL { get; set; }
        public string ZIPCODE_BL { get; set; }
        public string ZIPCODE_ROWID_BL { get; set; }
        public string HOUSE_NO_VT { get; set; }
        public string MOO_NO_VT { get; set; }
        public string MOOBAN_VT { get; set; }
        public string BUILDING_VT { get; set; }
        public string FLOOR_VT { get; set; }
        public string ROOM_VT { get; set; }
        public string SOI_VT { get; set; }
        public string ROAD_VT { get; set; }
        public string TUMBON_VT { get; set; }
        public string AMPHUR_VT { get; set; }
        public string PROVINCE_VT { get; set; }
        public string ZIPCODE_VT { get; set; }
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
        public DateTime? EXPECT_INSTALL_DATE { get; set; }
        public string FTTX_VENDOR { get; set; }
        public string INSTALL_NOTE { get; set; }
        public string JUMTYPE { get; set; }
        public string TIME_SLOT { get; set; }
        public string INSTALLATION_CAPACITY { get; set; }
        public string ADDRESS_ID { get; set; }
        public string ACCESS_MODE { get; set; }
        public string ENG_FLAG { get; set; }
        public string EVENT_CODE { get; set; }
        public string INSTALLADDRESS1 { get; set; }
        public string INSTALLADDRESS2 { get; set; }
        public string INSTALLADDRESS3 { get; set; }
        public string INSTALLADDRESS4 { get; set; }
        public string INSTALLADDRESS5 { get; set; }
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
        public string SOURCE_SYSTEM { get; set; }
        public string ORDER_NO { get; set; }
        public string BULK_NO { get; set; }
        //SFF
        public string RETURN_ORDER { get; set; }
        public string VAT_ADDRESS_1 { get; set; }
        public string VAT_ADDRESS_2 { get; set; }
        public string VAT_ADDRESS_3 { get; set; }
        public string VAT_ADDRESS_4 { get; set; }
        public string VAT_ADDRESS_5 { get; set; }
        public string VAT_ADDRESS_6 { get; set; }
        //public string IA { get; set; }
        //public string PASSWORD { get; set; }
        // public string DPNAME { get; set; }
        public string billLanguage { get; set; }
        public string userId { get; set; }
        public string orderReason { get; set; }
        public string productInstance { get; set; }
        public string serviceCode { get; set; }
        public string dpName { get; set; }
        public string dpPort { get; set; }
        public string dslamName { get; set; }
        public string dslamPort { get; set; }
        public string ia { get; set; }
        public string password { get; set; }
        public string types { get; set; }
        public string contactName { get; set; }
        public string contactMobilePhone { get; set; }
        // public string addressId { get; set; }
        public string accessType { get; set; }
        public string brand { get; set; }
        public string macAddress { get; set; }
        public string meterialCode { get; set; }
        public string model { get; set; }
        public string serialNo { get; set; }
        public string subContractor { get; set; }
        public string promotionMain { get; set; }
        public string promotionOntop { get; set; }

        public List<DetailAIR_REGIST_PACKAGE_ARRAY> P_AIR_REGIST_PACKAGE_ARRAY { get; set; }
        public List<CPEINFO> CPE_Info { get; set; }
        public List<DetailAIR_REGIST_FILE_ARRAY> P_AIR_REGIST_FILE_ARRAY { get; set; }
        public List<DetailAIR_REGIST_SPLITTER_ARRAY> P_AIR_REGIST_SPLITTER_ARRAY { get; set; }
        //   public List<DetailAIR_REGIST_CPE_SERIAL_ARRAY> P_AIR_REGIST_CPE_SERIAL_ARRAY { get; set; }
    }

    public class GetBulkCorpPackage
    {
        public List<DetailAIR_REGIST_PACKAGE_ARRAY> P_RES_DATA { get; set; }
        public string P_RETURN_CODE { get; set; }
        public string P_RETURN_MESSAGE { get; set; }
    }
    public class DetailAIR_REGIST_PACKAGE_ARRAY
    {
        public string cust_row_id { get; set; }
        public string package_code { get; set; }
        public string package_class { get; set; }
        public string package_group { get; set; }
        public string product_type { get; set; }
        public string product_subtype { get; set; }
        public string technology { get; set; }
        public string package_name { get; set; }
        public decimal recurring_charge { get; set; }
        public decimal initiation_charge { get; set; }
        public decimal discount_initiation { get; set; }
        public string package_bill_tha { get; set; }
        public string package_bill_eng { get; set; }
        public string download_speed { get; set; }
        public string upload_speed { get; set; }
        public string owner_product { get; set; }
        public string voip_ip { get; set; }
        public string idd_flag { get; set; }
        public string fax_flag { get; set; }
        public string mobile_forward { get; set; }
        public string TEMP_IA { get; set; }


    }

    public class DetailAIR_REGIST_FILE_ARRAY
    {
        public string file_name { get; set; }

    }

    public class DetailAIR_REGIST_SPLITTER_ARRAY
    {
        public string P_SPLITTER_NAME { get; set; }
        public decimal P_DISTANCE { get; set; }
        public string P_DISTANCE_TYPE { get; set; }
    }

    public class DetailAIR_REGIST_CPE_SERIAL_ARRAY
    {
        public string P_CPE_TYPE { get; set; }
        public string P_SERIAL_NO { get; set; }
        public string P_MAC_ADDRESS { get; set; }
    }

    public class GetWFAndSFFStatus
    {
        public string P_RETURN_CODE { get; set; }
        public string P_RETURN_MESSAGE { get; set; }
        public List<DetailWFAndSFFStatus> P_RES_DATA { get; set; }

    }
    public class DetailWFAndSFFStatus
    {
        public string BULK_NO { get; set; }
        public string ORDER_NO { get; set; }
        public decimal SEQ_ID { get; set; }
        public string STATUS_SEND_WORKFLOW { get; set; }
        public string STATUS_SEND_SFF { get; set; }
        public string BA_NO { get; set; }
        public string CA_NO { get; set; }
        public string SA_NO { get; set; }
        public string FIBRE_NET_ID { get; set; }

    }
    public class GetBlukCorpRegisterModel
    {
        public string P_RETURN_CODE { get; set; }
        public string P_RETURN_MESSAGE { get; set; }
        public List<DetialBlukCorpRegister> P_RES_DATA { get; set; }
    }
    public class DetialBlukCorpRegister
    {
        public string BULK_NO { get; set; }
        public string ORDER_NO { get; set; }
        public decimal SEQ_ID { get; set; }
        public string STATUS_SEND_WORKFLOW { get; set; }
        public string STATUS_SEND_SFF { get; set; }

    }

    public class GetEmailStatusModel
    {
        public string P_RETURN_CODE { get; set; }
        public string P_RETURN_MESSAGE { get; set; }
        public List<DetailGetEmailStatus> P_RES_DATA { get; set; }
    }

    public class DetailGetEmailStatus
    {
        public string BULK_NO { get; set; }
        public string EMAIL { get; set; }
    }



}
