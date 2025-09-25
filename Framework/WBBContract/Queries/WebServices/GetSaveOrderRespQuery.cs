using System.Collections.Generic;
using WBBEntity.PanelModels;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class GetSaveOrderRespQuery : IQuery<SaveOrderResp>
    {
        private QuickWinPanelModel _QuickWinPanelModel;
        public QuickWinPanelModel QuickWinPanelModel
        {
            get { return _QuickWinPanelModel ?? (_QuickWinPanelModel = new QuickWinPanelModel()); }
            set { _QuickWinPanelModel = value; }
        }

        public int CurrentCulture { get; set; }

        // Update 17.5
        public string FullUrl { get; set; }
    }

    public class GetSaveOrderRespJobQuery : IQuery<SaveOrderResp>
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
        public string BUILDING_BL { get; set; }
        public string FLOOR_BL { get; set; }
        public string ROOM_BL { get; set; }
        public string MOOBAN_BL { get; set; }
        public string SOI_BL { get; set; }
        public string ROAD_BL { get; set; }
        public string ZIPCODE_ROWID_BL { get; set; }

        public string HOUSE_NO_VT { get; set; }
        public string MOO_NO_VT { get; set; }
        public string BUILDING_VT { get; set; }
        public string FLOOR_VT { get; set; }
        public string ROOM_VT { get; set; }
        public string MOOBAN_VT { get; set; }
        public string SOI_VT { get; set; }
        public string ROAD_VT { get; set; }
        public string ZIPCODE_ROWID_VT { get; set; }
        public string CVR_ID { get; set; }
        public string CVR_NODE { get; set; }
        public string CVR_TOWER { get; set; }
        public string RELATE_MOBILE { get; set; }
        public string RELATE_NON_MOBILE { get; set; }
        public string SFF_CA_NO { get; set; }
        public string SFF_SA_NO { get; set; }
        public string SFF_BA_NO { get; set; }
        public string NETWORK_TYPE { get; set; }
        public string SERVICE_DAY { get; set; }
        public string EXPECT_INSTALL_DATE { get; set; }
        public bool SERVICE_DAYSpecified { get; set; }
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
        public string SITE_CODE { get; set; }
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
        public string RENTAL_FLAG { get; set; }
        public string DEV_PROJECT_CODE { get; set; }
        public string DEV_BILL_TO { get; set; }
        public string DEV_PO_NO { get; set; }
        public string PARTNER_TYPE { get; set; }
        public string PARTNER_SUBTYPE { get; set; }
        public string MOBILE_BY_ASC { get; set; }
        public string LOCATION_NAME { get; set; }
        public string PAYMENTMETHOD { get; set; }
        public string TRANSACTIONID_IN { get; set; }
        public string TRANSACTIONID { get; set; }
        public string SUB_ACCESS_MODE { get; set; }
        public string REQUEST_SUB_FLAG { get; set; }
        public string PREMIUM_FLAG { get; set; }
        public string RELATE_MOBILE_SEGMENT { get; set; }
        public string REF_UR_NO { get; set; }
        public string LOCATION_EMAIL_BY_REGION { get; set; }
        public string SALE_STAFF_NAME { get; set; }
        public List<REGIST_PACKAGE> REGIST_PACKAGE_LIST { get; set; }
        public List<REGIST_FILE> REGIST_FILE_LIST { get; set; }
        public List<REGIST_SPLITTER> REGIST_SPLITTER_LIST { get; set; }
        public List<REGIST_CPE_SERIAL> REGIST_CPE_SERIAL_LIST { get; set; }
        public string FullUrl { get; set; }
        public string ClientIP { get; set; }

        //20.3
        public string ORDER_RELATE_CHANGE_PRO { get; set; }
        public string COMPANY_NAME { get; set; }
        public string DISTRIBUTION_CHANNEL { get; set; }
        public string CHANNEL_SALES_GROUP { get; set; }
        public string SHOP_TYPE { get; set; }
        public string SHOP_SEGMENT { get; set; }
        public string ASC_NAME { get; set; }
        public string ASC_MEMBER_CATEGORY { get; set; }
        public string ASC_POSITION { get; set; }
        public string LOCATION_REGION { get; set; }
        public string LOCATION_SUB_REGION { get; set; }
        public string EMPLOYEE_NAME { get; set; }
        public string SERVICE_LEVEL { get; set; }
        public string AMENDMENT_FLAG { get; set; }
        public string CUSTOMERPURGE { get; set; }
        public string EXCEPTENTRYFEE { get; set; }
        public string SECONDINSTALLATION { get; set; }
        public string FIRST_INSTALL_DATE { get; set; }
        public string FIRST_TIME_SLOT { get; set; }
        public string LINE_TEMP_ID { get; set; }
        public string NON_MOBILE_NO { get; set; }
        public string REGIS_PAYMENT_ID { get; set; }
        public string REGIS_PAYMENTDATE { get; set; }
        public string REGIS_PAYMENTMETHOD { get; set; }
        public string REQUIRE_CS_VERIFY_DOC { get; set; }
        public string FACERECOG_FLAG { get; set; }
        public string SPECIAL_ACCOUNT_FLAG { get; set; }
        public string SERVICE_YEAR { get; set; }
    }

    public class REGIST_PACKAGE
    {
        public string faxFlag { get; set; }
        public string homeIp { get; set; }
        public string homePort { get; set; }
        public string iddFlag { get; set; }
        public string mobileForward { get; set; }
        public string packageCode { get; set; }
        public decimal packagePrice { get; set; }
        public bool packagePriceSpecified { get; set; }
        public string packageType { get; set; }
        public string pboxExt { get; set; }
        public string productSubtype { get; set; }
        public string tempIa { get; set; }
    }

    public class REGIST_FILE
    {
        public string fileName { get; set; }
    }

    public class REGIST_SPLITTER
    {
        public decimal distance { get; set; }
        public bool distanceSpecified { get; set; }
        public string distanceType { get; set; }
        public string resourceType { get; set; }
        public string splitterName { get; set; }
    }

    public class REGIST_CPE_SERIAL
    {
        public string cpeType { get; set; }
        public string macAddress { get; set; }
        public string serialNo { get; set; }
        //20.4
        public string status_desc { get; set; }
        public string model_name { get; set; }
        public string company_code { get; set; }
        public string cpe_plant { get; set; }
        public string storage_location { get; set; }
        public string material_code { get; set; }
        public string register_date { get; set; }
        public string fibrenet_id { get; set; }
        public string sn_pattern { get; set; }
        public string ship_to { get; set; }
        public string warranty_start_date { get; set; }
        public string warranty_end_date { get; set; }
    }
}
