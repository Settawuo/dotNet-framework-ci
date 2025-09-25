using System.Collections.Generic;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBEntity.PanelModels
{
    public class CustomerRegisterPanelModel : PanelModelBase
    {
        public CustomerRegisterPanelModel()
        {
            this.AddressPanelModelSendDoc = new AddressPanelModel();
            this.AddressPanelModelSetup = new AddressPanelModel();
            this.AddressPanelModelVat = new AddressPanelModel();
            this.ListImageFile = new List<UploadImage>();
            this.FBSSTimeSlot = new FBSSTimeSlot();
            this.CPE_Info = new List<CPEINFO>();
            this.WTTx_Info = new List<WTTxINFO>();

            this.AddressPanelModelSendDocIDCard = new AddressPanelModel();

            this.ListCustomerInsight = new List<CustomerInsight>();
        }
        public AddressPanelModel AddressPanelModelSendDocIDCard { get; set; }
        public AddressPanelModel AddressPanelModelSendDoc { get; set; }
        public AddressPanelModel AddressPanelModelSetup { get; set; }
        public AddressPanelModel AddressPanelModelVat { get; set; }

        public string H_FBB003 { get; set; }
        public string L_YOUR_PACK { get; set; }
        public string L_MAIN_PACK { get; set; }
        public string L_ONTOP_PACK { get; set; }
        public string L_RESIDENTIAL { get; set; }
        public string L_GOVERNMENT { get; set; }
        public string L_BUSINESS { get; set; }
        public string L_TITLE { get; set; }
        public string L_TITLE_CODE { get; set; }
        public string L_FIRST_NAME { get; set; }
        public string L_LAST_NAME { get; set; }
        public string L_CARD_TYPE { get; set; }
        public string L_CARD_NO { get; set; }
        public string L_GENDER { get; set; }
        public string L_BIRTHDAY { get; set; }
        public string L_CONTACT_PHONE { get; set; }
        public string L_HOME_PHONE { get; set; }
        public string L_MOBILE { get; set; }
        public string L_OR { get; set; }
        public string L_SPECIFIC_TIME { get; set; }
        public string L_EMAIL { get; set; }
        public string L_REMARK { get; set; }
        public string L_FOR_OFFICER_ONLY { get; set; }
        public string L_FOR_OFFICER { get; set; }
        public string L_LOC_CODE { get; set; }
        public string L_ASC_CODE { get; set; }
        public string L_STAFF_ID { get; set; }
        public string L_SALE_REP { get; set; }
        public string L_FOR_CS_TEAM { get; set; }
        public string L_GOVERNMENT_NAME { get; set; }
        public string L_FILL_VAT { get; set; }
        public string L_CONTACT_PERSON { get; set; }
        public string L_NATIONALITY { get; set; }
        public string L_ADDING_ADDR { get; set; }
        public string L_BUILD_CONDO { get; set; }
        public string L_TOP_TERRACE { get; set; }
        public string L_TERRACE { get; set; }
        public string L_TERRACE_DIRECTION { get; set; }
        public string L_NORTH { get; set; }
        public string L_SOUTH { get; set; }
        public string L_EAST { get; set; }
        public string L_WEST { get; set; }
        public string L_TYPE_ADDR { get; set; }
        public string L_HOUSE { get; set; }
        public string L_TOWN_HOME { get; set; }
        public string L_OFF_BUILD { get; set; }
        public string L_INSTALL_ADDR { get; set; }
        public string L_BILLING_ADDR { get; set; }
        public string L_SAME_AS_ADDR { get; set; }
        public string L_NUM_OF_FLOOR { get; set; }
        public string L_FLOOR_8 { get; set; }
        public string L_FLOOR_12 { get; set; }
        public string L_FLOOR_13 { get; set; }
        public string L_CONDO_AREA { get; set; }
        public string L_HOUSE_AREA { get; set; }
        public string L_BUILDING { get; set; }
        public string L_TREE { get; set; }
        public string L_BILLBOARD { get; set; }
        public string L_EXPRESSWAY { get; set; }
        public string L_VAT_ADDR { get; set; }
        public string L_INSTALL_DATE { get; set; }
        public string L_VOUCHER_PIN { get; set; }
        public string CateType { get; set; }
        public string SubCateType { get; set; }
        public string DocType { get; set; }

        public string BillChecked { get; set; }
        public string VatChecked { get; set; }

        // Update 15.3
        public FBSSTimeSlot FBSSTimeSlot { get; set; }

        // Update R15.6
        public string DeleteOrderNo { get; set; }
        public string L_EVENT_CODE { get; set; }
        public string L_UPLOAD_FILE_NAME { get; set; }

        // Update R15.7
        public string installAddress1 { get; set; }
        public string installAddress2 { get; set; }
        public string installAddress3 { get; set; }
        public string installAddress4 { get; set; }
        public string installAddress5 { get; set; }
        public string pbox_count { get; set; }
        public string convergence_flag { get; set; }
        public string v_installAddress { get; set; }
        public string outFullAddress { get; set; }

        public string vatAddress1 { get; set; }
        public string vatAddress2 { get; set; }
        public string vatAddress3 { get; set; }
        public string vatAddress4 { get; set; }
        public string vatAddress5 { get; set; }
        public string vatPostalCd { get; set; }
        public string vatAddressFull { get; set; }

        //15.9
        public string outType { get; set; }
        public string outSubType { get; set; }

        public List<UploadImage> ListImageFile { get; set; }

        //16.3
        public string Project_name { get; set; }

        //16.4
        public List<CPEINFO> CPE_Info { get; set; }
        public string CPE_Serial { get; set; }
        public string PlayBox_Serial { get; set; }
        public string Plug_and_play_flag { get; set; }

        //16.5
        public string AccountCategory { get; set; }

        //16.7
        public string OrderNo { get; set; }

        //16.10
        public string L_OLD_ISP { get; set; }

        //16.11
        public string JOB_ORDER_TYPE { get; set; }
        public string ASSIGN_RULE { get; set; }

        //17.4 eStatement value Y/N
        public string EStatmentFlag { get; set; }
        public string ReceiveEmailFlag { get; set; }

        //17.9
        public string RentalFlag { get; set; }

        public string L_SAME_AS_ADDR_IDCARD { get; set; }
        public string L_SAME_AS_IDCARD_ADDR { get; set; }
        //17.10
        public string CustomerSubtype { get; set; }

        //18.1 FTTB Sell Router
        public string RouterFlag { get; set; } //กรณีซื้อ router = "S", กรณีใช้ของตังเอง = "M", กรณียืม router = "B"

        //18.2 eBill
        public string EBillFlag { get; set; } //ไม่เลือก = '0', sms+ebill = '1' , sms+email = '2'

        //18.3 deverloper

        public string DEVELOPER { get; set; }
        public string p_dev_project_code { get; set; }
        public string p_dev_bill_to { get; set; }
        public string p_dev_price { get; set; }
        public string PO_NO { get; set; }

        //18.6 SCPE

        public string SCPE_USE_LOC_CODE { get; set; }
        public string SCPE_LOC_CODE { get; set; }
        public string SCPE_USE_ASC_CODE { get; set; }
        public string SCPE_ASC_CODE { get; set; }

        //18.6
        public string SelectFlagCheckPlugAndPlayFlow { get; set; }

        //18.7
        public string outMobileNo { get; set; }
        public string PartnerName { get; set; }

        //18.8
        public string REQUEST_SUB_FLAG { get; set; }
        public string SUB_ACCESS_MODE { get; set; }
        public string PREMIUM_FLAG { get; set; }
        public string RELATE_MOBILE_SEGMENT { get; set; }
        public string REF_UR_NO { get; set; }
        public string LOCATION_EMAIL_BY_REGION { get; set; }

        //18.11
        public string EMP_NAME { get; set; }


        //R18.11 IdCard Reader on PC
        public string L_CARD_NO_LASER { get; set; }

        //18.12
        public string SffServiceYear { get; set; }
        public string SffRegisteredDate { get; set; }
        public string SubNetworkType { get; set; }
        public string FlagDopa { get; set; }
        public string FlagFaceRecognition { get; set; }
        public string FlagTakePhoto { get; set; }
        public string FlagBrowseFile { get; set; }
        public string FlagVarifyDocuments { get; set; }

        public string FlagDopaSubmit { get; set; }
        public string FlagFaceRecognitionSubmit { get; set; }

        //19.1 Special Account
        public string SpecialAccountName { get; set; }
        public string SpecialAccountNo { get; set; }
        public string SpecialAccountEnddate { get; set; }
        public string SpecialAccountGroupEmail { get; set; }
        public string SpecialAccountFlag { get; set; }

        //19.3
        public string MobilePrice { get; set; }
        public string Existing_Mobile { get; set; }

        //19.4 Redcap
        public string PreSurveyDate { get; set; }
        public string PreSurveyTimeslot { get; set; }

        public string replace_onu { get; set; }
        public string replace_wifi { get; set; }
        public string mesh_count { get; set; }
        //19.6 Redcap
        public string DownloadSpeed { get; set; }
        //19.7
        public string RegisterChannelSaveOrder { get; set; }
        public string AutoCreateProspectFlag { get; set; }
        public string OrderVerify { get; set; }
        //20.2
        public string WTTx_Serial { get; set; }
        public List<WTTxINFO> WTTx_Info { get; set; }
        //20.3 Service Level
        public string ServiceLevel { get; set; }
        public string ServiceLevel_Flag { get; set; }
        //20.5 Non-Res Register Residential
        public string Non_Res_Flag { get; set; }
        public string LINE_TEMP_ID { get; set; }
        //20.6
        public string Remark_For_Subcontract { get; set; }
        public List<CustomerInsight> ListCustomerInsight { get; set; }
        public string Online_Flag { get; set; }
        //21.3
        public string StaffPrivilegeBypass_TransactionID { get; set; }
        //21.6 eApplication
        public string Package_Duration { get; set; }
        //21.8 Contract Device
        public string TDMContractId { get; set; }
        public string TDMRuleId { get; set; }
        public string TDMPenaltyId { get; set; }
        public string TDMPenaltyGroupId { get; set; }
        public string Duration { get; set; }
        public string ContractFlag { get; set; }
        // MESH evOMCheckDeviceContract
        public string ContractFlagFbb { get; set; }
        public string CountContractFbb { get; set; }
        public string FBBLimitContract { get; set; }
        public string FlagTDM { get; set; }
        public string ContractID { get; set; }
        public string ContractName { get; set; }
        public string PenaltyInstall { get; set; }
        public string ContractProfileCountFbb { get; set; }
        //21.10 MOU
        public string FIBRE_ID { get; set; }
        public List<Dcontract> ListDcontract { get; set; }

        //R23.04 Billing Address
        //ช่องทางการรับเอกสาร
        public string CHANNEL_RECEIVE_BILL { get; set; }
        //เก็บค่าเงื่อนไขใน Drop Down list
        public string CONDITION_NEW_DOC { get; set; }
        //เก็บเเบอร์โทรสำหรับบิลที่เลือก
        public string BILL_MOBILE_NO { get; set; }
        //ข้อมูลรอบบิล
        public string BILL_CYCLE_INFO { get; set; }
        //ข้อมูลการรับบิล
        public string BILL_CHANNEL_INFO { get; set; }

        //R23.08 จำนวนวันที่คิดค่าเฉลี่ย
        public string BILL_SUM_AVG_DAY { get; set; }
        //R23.08 จำนวนเงินรวมตามวันเฉลี่ย
        public string BILL_SUM_AVG { get; set; }
        //R23.08
        public string BILL_AVG_PER_DAY { get; set; }
        //R23.08
        public string BILL_TOTAL { get; set; }
        //R23.08
        public string BILL_SUM_TOTAL { get; set; }

        //R23.05 CheckFraud
        public checkFraudInfo CHECK_FRAUD_INFO { get; set; }
        //R23.04
        public string BILL_ADDRESS_NO { get; set; }
        //R23.09 IPCAMERA
        public string DELIVERY_METHOD { get; set; }
        public string DIY_FLAG { get; set; }

        //R24.01 checkFraud
        public string CUST_ROW_ID { get; set; }
        public string CREATED_BY { get; set; }
        public string CEN_FRAUD_FLAG { get; set; }
        public string VERIFY_REASON_CEN_FRAUD { get; set; }
        public string FRAUD_SCORE { get; set; }
        public string AIR_FRAUD_REASON_ARRAY { get; set; }
        public string AUTO_CREATE_PROSPECT_FLAG { get; set; }
        public string CS_NOTE_POPUP { get; set; }
        public string URL_ATTACH_POPUP { get; set; }
        public string address_duplicated_flag { get; set; }
        public string id_duplicated_flag { get; set; }
        public string contact_duplicated_flag { get; set; }
        public string contact_not_active_flag { get; set; }
        public string contact_no_fmc_flag { get; set; }
        public string watch_list_dealer_flag { get; set; }
        public string sale_dealer_direct_sale_flag { get; set; }
    }

    public class OrderDupModel
    {
        public string CAN_DELETE { get; set; }
        public string ID_CARD_NO { get; set; }
        public string ORDER_CREATED_DTM { get; set; }
        public string CUSTOMER_ADDRESS { get; set; }
        public string INSTALL_DATE { get; set; }
        public string ORDER_STATUS { get; set; }
        public string PACKAGE_NAME { get; set; }
        public string ORDER_NO { get; set; }
    }


    public class UploadImage
    {
        public string FileName { get; set; }
    }

    public class CPEINFO
    {
        public string cpe_type { get; set; }
        public string Plug_and_play_flag { get; set; }

        public string Check_Result_Code { get; set; }
        public string Check_Result_Desc { get; set; }
        public string SN { get; set; }
        public string Status_ID { get; set; }
        public string STATUS_DESC { get; set; }
        public string CPE_MAC_ADDR { get; set; }
        public string CPE_TYPE_ERR { get; set; }
        public string SN_PATTERN { get; set; }

        //20.4
        public string CPE_MODEL_NAME { get; set; }
        public string CPE_COMPANY_CODE { get; set; }
        public string CPE_PLANT { get; set; }
        public string CPE_STORAGE_LOCATION { get; set; }
        public string CPE_MATERIAL_CODE { get; set; }
        public string REGISTER_DATE { get; set; }
        public string FIBRENET_ID { get; set; }
        public string SHIP_TO { get; set; }
        public string WARRANTY_START_DATE { get; set; }
        public string WARRANTY_END_DATE { get; set; }
        public string MAC_ADDRESS { get; set; }
    }
    public class WTTxINFO
    {
        public string cpe_type { get; set; }
        public string Plug_and_play_flag { get; set; }

        public string Check_Result_Code { get; set; }
        public string Check_Result_Desc { get; set; }
        public string SN { get; set; }
        public string Status_ID { get; set; }
        public string STATUS_DESC { get; set; }
        public string CPE_MAC_ADDR { get; set; }
        public string CPE_TYPE_ERR { get; set; }
        public string SN_PATTERN { get; set; }

        //20.4
        public string CPE_MODEL_NAME { get; set; }
        public string CPE_COMPANY_CODE { get; set; }
        public string CPE_PLANT { get; set; }
        public string CPE_STORAGE_LOCATION { get; set; }
        public string CPE_MATERIAL_CODE { get; set; }
        public string REGISTER_DATE { get; set; }
        public string FIBRENET_ID { get; set; }
        public string SHIP_TO { get; set; }
        public string WARRANTY_START_DATE { get; set; }
        public string WARRANTY_END_DATE { get; set; }
        public string MAC_ADDRESS { get; set; }
    }

    public class CustomerInsight
    {
        public string GROUP_ID { get; set; }
        public string GROUP_NAME_TH { get; set; }
        public string GROUP_NAME_EN { get; set; }
        public string QUESTION_ID { get; set; }
        public string QUESTION_TH { get; set; }
        public string QUESTION_EN { get; set; }
        public string ANSWER_ID { get; set; }
        public string ANSWER_TH { get; set; }
        public string ANSWER_EN { get; set; }
        public string ANSWER_VALUE_TH { get; set; }
        public string ANSWER_VALUE_EN { get; set; }
        public string PARENT_ANSWER_ID { get; set; }
        public string ACTION_WFM { get; set; }
        public string ACTION_FOA { get; set; }
    }

    public class Dcontract
    {
        public string PRODUCT_SUBTYPE { get; set; }
        public string PBOX_EXT { get; set; }
        public string TDM_CONTRACT_ID { get; set; }
        public string TDM_RULE_ID { get; set; }
        public string TDM_PENALTY_ID { get; set; }
        public string TDM_PENALTY_GROUP_ID { get; set; }
        public string DURATION { get; set; }
        public string CONTRACT_FLAG { get; set; }
        public string DEVICE_COUNT { get; set; }
    }
}
