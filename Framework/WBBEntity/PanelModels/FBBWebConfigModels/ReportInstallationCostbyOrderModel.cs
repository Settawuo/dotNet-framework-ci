using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using WBBEntity.Extensions;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBEntity.PanelModels.FBBWebConfigModels
{

    public class reportInstallFLSUpdateModel
    {
        public string UPDATESTATUS { get; set; }
        public string WF_STATUS { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public string INVOICE_NO { get; set; }
        public string INVOICE_DATE { get; set; }
        public int PAGE_INDEX { get; set; }
        public int PAGE_SIZE { get; set; }
        //End R19.03
    }
    public class ReportInstallationCostbyOrderModel
    {
        public string INV_GRP { get; set; }
        public string ORDER_NO { get; set; }
        public string ACCESS_NO { get; set; }
        public string[] PRODUCT_NAME { get; set; }
        public string SUBCONT_CODE { get; set; }
        public string ORG_ID { get; set; }
        //public string SUBCONT_NAME { get; set; }
        public string IR_DOC { get; set; }
        public string INVOICE_NO { get; set; }
        public string[] ORDER_STATUS { get; set; }
        public string REGION { get; set; }
        public string SUBCONT_TYPE { get; set; }
        public string SUBCONTSUB_TYPE { get; set; }
        public string ORD_STATUS { get; set; }
        public string ORD_TYPE { get; set; }
        public string FOA_FM { get; set; }
        public string FOA_TO { get; set; }
        public string APPROVE_FM { get; set; }
        public string APPROVE_TO { get; set; }
        public string PERIOD_FM { get; set; }
        public string PERIOD_TO { get; set; }
        public string TRANS_FM { get; set; }
        public string TRANS_TO { get; set; }
        public string UPDATE_BY { get; set; }
        public string INTERFACE { get; set; }
        public string USER { get; set; }
        public string STATUS { get; set; }
        public string INVOICE_DT { get; set; }
        public string VALIDATE_DIS { get; set; }
        public string REASON { get; set; }
        public string REMARK { get; set; }
        public string REMARK_FOR_SUB { get; set; }
        public string TRANSFER_DT { get; set; }
        public int PAGE_INDEX { get; set; }
        public int PAGE_SIZE { get; set; }
        //R19.03  Add model
        public string EXISTING_RULE { get; set; }
        public string NEW_RULE { get; set; }
        //End R19.03
        public string TOTAL_PRICE { get; set; }

        public string[] WORK_STATUS { get; set; }
        public string PRODUCT_OWNER { get; set; }
        public string report_type { get; set; }
    }

    public class reportInstallSelectSubContractorModel
    {
        public string ORG_ID { get; set; }
        public string SUB_CONTRACTOR_CODE { get; set; }
        public string SUB_CONTRACTOR_NAME { get; set; }
    }

    public class ReportInstallationCostbyOrderListReturn
    {
        public string ret_code { get; set; }
        public string ret_msgg { get; set; }
        public List<ReportInstallationCostbyOrderListModel_Binding> install_cur { get; set; }
        public List<ReportInstallationCostbyOrderListModel_Binding> ma_cur { get; set; }
        public List<ReportInstallationCostbyOrderListModel_Binding> cur { get; set; }
    }

    public class ExistReportInstallationCostbyOrderReturn
    {
        public string RET_OVER_COST { get; set; }
        public string RET_TOTAL_PRICE { get; set; }
        public string RET_CODE { get; set; }
        public string RET_MSG { get; set; }
        public List<ExistCurReportInstallationCostbyOrderListModel> RETURN_EXISTS_LOOKUP_CUR { get; set; }
        public List<ExistCurReportInstallationCostbyOrderListModel> RETURN_MAIN_LOOKUP_CUR { get; set; }
        public List<ExistCurReportInstallationCostbyOrderListModel> RETURN_CUR { get; set; }
    }
    public class ExistCurReportInstallationCostbyOrderListModel
    {
        public string LOOKUP_NAME { get; set; }
        public string TYPE { get; set; }
        public string LOOKUP_ID { get; set; }
        public decimal PARAMETER_VALUE { get; set; }
        public string ONTOP_LOOKUP { get; set; }
    }



    public class SelectedLookupReportInstallationCostbyOrderReturn
    {
        public string ret_code { get; set; }
        public string ret_msg { get; set; }
        public List<SelectedLookupCurReportInstallationCostbyOrderListModel> result_lookup_id_cur { get; set; }

    }
    public class SelectedLookupCurReportInstallationCostbyOrderListModel
    {
        public string ontop_flag { get; set; }
        public string lookup_name { get; set; }
        public string lookup_list { get; set; }
    }


    public class ReportInstallationCostbyOrderListModel_Binding
    {
        public string CONCATONTOP_LOOKUP { get; set; }
        public string L_UP_TYPE { get; set; }
        public decimal? L_UP_PRICE { get; set; }
        //public string WORK_STATUS { get; set; }
        //public string ACCESS_NO { get; set; }

        public string ACCESS_NUMBER { get; set; }
        public string INVOICE_DATE_TEXT { get; set; }
        public string SBC_CPY { get; set; }
        //public string INVOICE_NO { get; set; }
        public string ORDER_NO { get; set; }
        public string ORDER_NO_SFF { get; set; }

        public DateTime? CS_APPROVE_DATE { get; set; }
        public DateTime? ORDER_STATUS_DT { get; set; }
        public DateTime INVOICE_DATE { get; set; }
        public decimal? TOTAL_PAID_AMOUNT { get; set; }//
        public decimal? TOTAL_VAT { get; set; }//
        public decimal? TOTAL_AMOUNT { get; set; }//
        public string RULE_ID { get; set; }
        public string SUBCONTRACT_NAME { get; set; }
        public DateTime? PAID_DATE { get; set; }
        public decimal CNT { get; set; }//
        public string INVOICE_AMOUNT_BFVAT { get; set; }
        public decimal? INVOICE_AMOUNT_VAT { get; set; }//
        public decimal? TOTAL_PAID { get; set; }//
        public string TOTAL_INVOICE_AMOUNT_VAT { get; set; }
        public string CS_APPROVE_DATE_TEXT { get; set; }
        public string LAST_UPDATE_DATE_TEXT { get; set; }
        public DateTime LAST_UPDATE_DATE { get; set; }
        public string ORDER_STATUS_DT_TEXT { get; set; }
        public string COMPLETE_DATE_TEXT { get; set; }
        public string APPOINTMENNT_DT_TEXT { get; set; }
        public DateTime? APPOINTMENNT_DT { get; set; }
        public string EFFECTIVE_END_DT_TEXT { get; set; }
        public DateTime? EFFECTIVE_END_DT { get; set; }
        public DateTime? SFF_ACTIVE_DATE { get; set; }
        public string SFF_ACTIVE_DATE_TEXT { get; set; }
        public DateTime? SFF_SUBMITTED_DATE { get; set; }
        public DateTime FOA_SUBMIT_DATE { get; set; }
        public string PAID_DATE_TEXT { get; set; }
        public string FOA_SUBMIT_DATE_TEXT { get; set; }
        public string SFF_SUBMITTED_DATE_TEXT { get; set; }
        public string APPROVE_FLAG { get; set; }
        //public string ORDER_STATUS { get; set; }
        public string PERIOD { get; set; }
        public string ORG_ID { get; set; }
        public string SUB_MAIL { get; set; }
        public string ACCOUNT_NAME { get; set; }
        public string PROMOTION_NAME { get; set; }
        public decimal? DISTANCE_TOTAL { get; set; }//
        public decimal? ENTRY_FEE { get; set; }//




        public string WORK_STATUS { get; set; }
        public string ACCESS_NO { get; set; }
        public string ORDER_TYPE { get; set; }
        public DateTime COMPLETE_DATE { get; set; }
        public DateTime SOA_SUBMIT_DATE { get; set; }
        public string SUBCONTRACT_TYPE { get; set; }
        public string SOA_SUBMIT_DATE_TEXT { get; set; }
        public string MODIFY_DATE_TEXT { get; set; }
        public string SUBCONTRACT_SUB_TYPE { get; set; }
        public string LOOKUP_ID { get; set; }
        public string LOOKUP_NAME { get; set; }
        public string LOOKUP_COST { get; set; }
        public string ONTOP_LOOKUP_ID { get; set; }
        public string ONTOP_LOOKUP_NAME { get; set; }
        
        public string ONTOP_COST { get; set; }
        public DateTime MODIFY_DATE { get; set; }
        public string MODIFY_BY { get; set; }
        public string LAST_UPDATE_BY { get; set; }
        public string SYMPTOM_GROUP { get; set; }
        public string SYMPTOM_NAME { get; set; }
        public string REUSED_FLAG { get; set; }
        public string REQUEST_SUB_FLAG { get; set; }
        public string RECAL_LOOKUP_ID { get; set; }
        public string RECAL_ONTOP_LOOKUP_ID { get; set; }
        public decimal TOTAL_DISTANCE { get; set; }
        public decimal BASE_COST { get; set; }
        public decimal TOTAL_SOA { get; set; }
        public decimal RowNumber { get; set; }
        public decimal TOTAL_FEE { get; set; }
        public decimal TOTAL_RECAL { get; set; }
        public decimal RECAL_DISTANCE { get; set; }
        public decimal RECAL_COST { get; set; }
        public decimal RECAL_OVER_LENGTH { get; set; }
        public decimal RECAL_OVER_COST { get; set; }
        public decimal OVER_LENGTH { get; set; }
        public decimal OVER_COST { get; set; }

        public string ACCESS_NUMBER_MASKING { get; set; }
        public string NOTE { get; set; }

        public string action_flag { get; set; }



        //New requirement 17/10/2024
        public string TEAM_ID { get; set; }
        public string MAIN_PROMO_CODE { get; set; }
        public string REGION { get; set; }
        public string PRODUCT_NAME { get; set; }
        public string SUBCONTRACT_CODE { get; set; }
        public string IR_DOC { get; set; }
        public string INVOICE_NO { get; set; }
        public string PRODUCT_OWNER { get; set; }
        public string ORDER_STATUS { get; set; }
        public DateTime TRANSFER_DATE { get; set; }
        public string TRANSFER_DATE_TEXT { get; set; }
        public DateTime PERIOD_DATE { get; set; }
        public string PERIOD_DATE_TEXT { get; set; }





    }

    public class ReportInstallationCostbyOrderListModel
    {


        public string WORK_STATUS { get; set; }
        public string ACCESS_NUMBER_MASKING { get; set; }
        public string ACCESS_NUMBER { get; set; }
        public string ORDER_NO { get; set; }
        public string REUSED_FLAG { get; set; }
        public string REQUEST_SUB_FLAG { get; set; }

        public decimal? REQUEST_DISTANCE { get; set; }
        public decimal? APPROVE_DISTANCE { get; set; }
        public string APPROVE_STAFF { get; set; }
        public string APPROVE_STATUS { get; set; }
        public string RULE_ID { get; set; }
        public decimal? OVER_LENGTH { get; set; }
        public decimal? OVER_COST { get; set; }



        public string SUBCONTRACT_CODE { get; set; }
        public string SUBCONTRACT_NAME { get; set; }
        public string PAY_PERIOD { get; set; }
        public DateTime? CS_APPROVE_DATE { get; set; }
        public string CS_APPROVE_DATE_TEXT { get; set; }
        public string INVOICE_NO { get; set; }
        public DateTime? INVOICE_DATE { get; set; }
        public string INVOICE_DATE_TEXT { get; set; }
        public string IR_DOC { get; set; }
        public string REMARK { get; set; }
        public string ORDER_STATUS { get; set; }
        public string NOTE { get; set; }
        public decimal? DISTANCE_LASTMILE_APP { get; set; }
        public decimal? DISTANCE_ESRI { get; set; }
        public decimal? DISTANCE_GMAP { get; set; }
        public decimal? DISTANCE_STRAIT { get; set; }
        public decimal? DISTANCE_VALIDATE { get; set; }
        public decimal? DISTANCE_TOTAL { get; set; }
        public decimal? DISTANCE_PAID { get; set; }
        public decimal? Distance_MORE_325 { get; set; }
        public decimal? LASTMILE_PRICE { get; set; }
        public decimal? ORDER_FEE { get; set; }
        public decimal? TOTAL_PAID { get; set; }
        public DateTime? LAST_UPDATE_DATE { get; set; }
        public string LAST_UPDATE_DATE_TEXT { get; set; }
        public string LAST_UPDATE_BY { get; set; }
        public decimal? DISPUTE_DISTANCE { get; set; }
        public decimal? TOTAL_DISTANCE { get; set; }
        public decimal? MAPPING_COST { get; set; }
        public string PERIOD { get; set; }
        public DateTime? ORDER_STATUS_DT { get; set; }
        public string ORDER_STATUS_DT_TEXT { get; set; }
        public string ACCOUNT_NAME { get; set; }
        public DateTime? APPOINTMENNT_DT { get; set; }
        public string APPOINTMENNT_DT_TEXT { get; set; }
        public string PROMOTION_NAME { get; set; }
        public decimal? LENGTH_DISTANCE { get; set; }
        public decimal? OUTDOOR_COST { get; set; }
        public decimal? INDOOR_COST { get; set; }
        public decimal? TOTAL_COST { get; set; }
        public decimal? ENTRY_FEE { get; set; }
        public string PRODUCT_NAME { get; set; }
        public DateTime? EFFECTIVE_END_DT { get; set; }
        public string EFFECTIVE_END_DT_TEXT { get; set; }
        public string CREAETED_BY { get; set; }
        public string LAST_UPDATED_BY { get; set; }
        public string SUB_MAIL { get; set; }
        public List<ReportInstallationCostbyOrderListModel> SENDMAIL_LIST { get; set; }
        public string SBC_CPY { get; set; }
        public string ON_TOP1 { get; set; }
        public string ON_TOP2 { get; set; }
        public string VOIP_NUMBER { get; set; }
        public string ORD_TYPE { get; set; }
        public string ORDER_SFF { get; set; }
        public DateTime? SFF_ACTIVE_DATE { get; set; }
        public string SFF_ACTIVE_DATE_TEXT { get; set; }
        public string OM_ORDER_STATUS { get; set; }
        public string REJECT_REASON { get; set; }
        public string MATERIAL_CODE_CPESN { get; set; }
        public string CPE_SN { get; set; }
        public string CPE_MODE { get; set; }
        public string MATERIAL_CODE_STBSN { get; set; }
        public string STB_SN { get; set; }
        public string MATERIAL_CODE_ATASN { get; set; }
        public string ATA_SN { get; set; }
        public string MATERIAL_CODE_WIFIROUTESN { get; set; }
        public string WIFI_ROUTER_SN { get; set; }
        public string STO_LOCATION { get; set; }
        public string FOA_REJECT_REASON { get; set; }
        public string RE_APPOINTMENT_REASON { get; set; }
        public string PHASE_PO { get; set; }
        public DateTime? SFF_SUBMITTED_DATE { get; set; }
        public string SFF_SUBMITTED_DATE_TEXT { get; set; }
        public string EVENT_CODE { get; set; }
        public string REGION { get; set; }
        public string FEE_CODE { get; set; }
        public string ADDR_ID { get; set; }
        public string ADDR_NAME_TH { get; set; }
        public string ORG_ID { get; set; }
        public DateTime? FOA_SUBMIT_DATE { get; set; }
        public string FOA_SUBMIT_DATE_TEXT { get; set; }
        public string INSTALLATION_ADDRESS { get; set; }
        public string SUBCONTRACT_TYPE { get; set; }

        public string SUBCONTRACT_SUB_TYPE { get; set; }
        public string APPROVE_FLAG { get; set; }
        public int RowNumber { get; set; }
        public decimal CNT { get; set; }

        //Add For PhaseIII

        public string DIFF_DISTANCE { get; set; }
        public decimal? RECAL_DIS { get; set; }
        public decimal? RECAL_RATE { get; set; }
        public decimal? RECAL_OVER_LENGTH { get; set; }
        public decimal? RECAL_OVER_COST { get; set; }
        public decimal? MAX_LENGTH { get; set; }

        public decimal? RECAL_MAPPING_COST { get; set; }
        public decimal? INVOICE_AMOUNT_VAT { get; set; }
        public string INVOICE_AMOUNT_BFVAT { get; set; }

        public string TOTAL_INVOICE_AMOUNT_VAT { get; set; }

        public DateTime? PAID_DATE { get; set; }
        public string PAID_DATE_TEXT { get; set; }

        public decimal? TOTAL_PAID_AMOUNT { get; set; }
        public decimal? TOTAL_VAT { get; set; }
        public decimal? TOTAL_AMOUNT { get; set; }
        public string USER_ID { get; set; }
        public string INV_GRP { get; set; }
        public string SUBCONTRACT_LOCATION { get; set; }



        public string ORDER_TYPE { get; set; }
        public DateTime? COMPLETE_DATE { get; set; }
        public string LOOKUP_ID { get; set; }
        public string LOOKUP_NAME { get; set; }
        public string ONTOP_LOOKUP_NAME { get; set; }
        public string ONTOP_LOOKUP_ID { get; set; }
        public decimal? BASE_COST { get; set; }
        public decimal? TOTAL_SOA { get; set; }
        public string RECAL_LOOKUP_ID { get; set; }
        public string RECAL_ONTOP_LOOKUP_ID { get; set; }
        public decimal? RECAL_DISTANCE { get; set; }
        public decimal? RECAL_COST { get; set; }
        public decimal? TOTAL_RECAL { get; set; }
        public string TOTAL_DISPUTE { get; set; }
        public decimal? TOTAL_FEE { get; set; }

        public string SYMPTOM_GROUP { get; set; }
        public string SYMPTOM_NAME { get; set; }
        public DateTime? MODIFY_DATE { get; set; }
        public string MODIFY_BY { get; set; }
        public string PRODUCT_OWNER { get; set; }
        public string MAIN_PROMO_CODE { get; set; }
        public string TEAM_ID { get; set; }


        public DateTime? SOA_SUBMIT_DATE { get; set; }
        public decimal? LOOKUP_COST { get; set; }
        public decimal? ONTOP_COST { get; set; }
    }

    public class reportInstallScmOrderListModel
    {
        public string ACCESS_NUMBER { get; set; }
        public string ORDER_NO { get; set; }
        public string REUSE_FLAG { get; set; }
        public string SUBCONTRACT_CODE { get; set; }
        public string SUBCONTRACT_NAME { get; set; }
        public string PAY_PERIOD { get; set; }
        public string CS_APPROVE_DATE { get; set; }
        public string INVOICE_NO { get; set; }
        public string INVOICE_DATE { get; set; }
        public string IR_DOC { get; set; }
        public string REMARK { get; set; }
        public string STATUS { get; set; }
        public string ORDER_CASE { get; set; }

    }

    public class reportInstallScmOrderListModelBySendMail
    {
        public byte[] FileData;
        //public MemoryStream msExcel;
        public string CS_APPROVE_DATE { get; set; }
        public string CS_APPROVE_DATE_TO { get; set; }
        public string CS_APPROVE_DATE_FROM { get; set; }
        public string REMARK { get; set; }
        public string STRING_CS_APPROVE_DATE { get; set; }
        public string CS_DOC_DATE { get; set; }
        public string CS_PAYMENT_DATE { get; set; }

    }
    public class reportInstallGenerateExcelModel
    {
        public string physicalPath { get; set; }
        public MemoryStream msExcel { get; set; }
    }


    public class reportInstallFapoOrderListModel
    {

        public string PAY_PERIOD { get; set; }
        public string ORDER_STATUS_DT { get; set; }
        public string INTERNET_NO { get; set; }
        public string ACCOUNT_NAME { get; set; }
        public string APPOINTMENNT_DT { get; set; }
        public string PROMOTION_NAME { get; set; }
        public string SUBCONTRACT_NAME { get; set; }
        public string DISTANCE_TOTAL { get; set; }
        public string OUTDOOR_COST { get; set; }
        public string INDOOR_COST { get; set; }
        public string TOTAL_COST { get; set; }
        public string ENTRY_FEE { get; set; }
        public string ORDER_NO { get; set; }
        public string PRODUCT_NAME { get; set; }
        public string VENDOR_CODE { get; set; }
        public string EFFECTIVE_END_DT { get; set; }
        public string CREAETED_BY { get; set; }
        public string LAST_UPDATED_BY { get; set; }


    }

    public class reportInstallAcctOrderListModel
    {
        public string PAY_PERIOD { get; set; }
        public string ORDER_STATUS_DT { get; set; }
        public string INTERNET_NO { get; set; }
        public string ACCOUNT_NAME { get; set; }
        public string APPOINTMENNT_DT { get; set; }
        public string PROMOTION_NAME { get; set; }
        public string SUBCONTRACT_NAME { get; set; }
        public string DISTANCE_TOTAL { get; set; }
        public string OUTDOOR_COST { get; set; }
        public string INDOOR_COST { get; set; }
        public string TOTAL_COST { get; set; }
        public string ENTRY_FEE { get; set; }
        public string ORDER_NO { get; set; }
        public string PRODUCT_NAME { get; set; }
        public string VENDOR_CODE { get; set; }
        public string EFFECTIVE_END_DT { get; set; }
        public string CREAETED_BY { get; set; }
        public string LAST_UPDATED_BY { get; set; }
    }

    public class reportInstallOrderDetailModel
    {
        public string ACC_NBR { get; set; }
        public string USER_NAME { get; set; }
        public string SBC_CPY { get; set; }
        public string PRODUCT_NAME { get; set; }
        public string PHASE_PO { get; set; }
        public string EVENT_CODE { get; set; }
        public string ON_TOP1 { get; set; }
        public string ON_TOP2 { get; set; }
        public string VOIP_NUMBER { get; set; }
        public string SERVICE_PACK_NAME { get; set; }
        public string ORD_NO { get; set; }
        public string ORD_TYPE { get; set; }
        public string ORDER_SFF { get; set; }
        public DateTime? SFF_SUBMITTED_DATE { get; set; }
        public DateTime? APPOINTMENT_DATE { get; set; }
        public DateTime? SFF_ACTIVE_DATE { get; set; }
        public DateTime? APPROVE_JOB_FBSS_DATE { get; set; }
        public DateTime? COMPLETED_DATE { get; set; }
        public string ORDER_STATUS { get; set; }
        public string FOA_ORDER_STATUS { get; set; }
        public string REJECT_REASON { get; set; }
        public string MATERIAL_CODE_CPESN { get; set; }
        public string CPE_SN { get; set; }
        public string CPE_MODE { get; set; }
        public string MATERIAL_CODE_STBSN { get; set; }
        public string STB_SN { get; set; }
        public string MATERIAL_CODE_ATASN { get; set; }
        public string ATA_SN { get; set; }
        public string MATERIAL_CODE_WIFIROUTESN { get; set; }
        public string WIFI_ROUTER_SN { get; set; }
        public string STO_LOCATION { get; set; }
        public string VENDOR_CODE { get; set; }
        public string FOA_REJECT_REASON { get; set; }
        public string RE_APPOINTMENT_REASON { get; set; }
        public string REGION { get; set; }
        public decimal? TOTAL_FEE { get; set; }
        public string FEE_CODE { get; set; }
    }

    public class reportInstallOrderHistoryDetailModel
    {
        public string WORK_STATUS { get; set; }
        public string UPDATE_BY { get; set; }
        public DateTime? UPDATE_DATE { get; set; }
        public string UPDATE_DATE_TEXT { get; set; }
        public string REMARK { get; set; }
        public string REMARK_FOR_SUB { get; set; }
        public decimal? OVER_LENGTH { get; set; }
        public decimal? OVER_COST { get; set; }
        public decimal? LENGTH_DISTANCE { get; set; }
        public decimal? TOTAL_COST { get; set; }
        public string RULE_ID { get; set; }

    }

    public class reportInstallDistanceDetailModel
    {
        public string TRANSACTION_ID { get; set; }
        public decimal? REAL_DISTANCE { get; set; }
        public decimal? MAP_DISTANCE { get; set; }
        public decimal? DISP_DISTANCE { get; set; }
        public decimal? ESRI_DISTANCE { get; set; }
        public decimal? X1 { get; set; }
        public decimal? X2 { get; set; }
        public decimal? X2_1 { get; set; }
        public decimal? X2_2 { get; set; }
        public decimal? X2_3 { get; set; }
        public decimal? X2_4 { get; set; }
        public decimal? X3 { get; set; }
        public decimal? X4 { get; set; }
        public decimal? TotalPaid { get; set; }
        public string STATUS { get; set; }
        public string USER_NAME { get; set; }
        public DateTime? ACTION_DATE { get; set; }
        public string ACTION_DATE_TEXT { get; set; }
        public string POST_SAP { get; set; }
    }


    public class reportInstallPostSapDetail
    {
        public string ORDER_NO { get; set; }
        public DateTime? ORDER_DATE { get; set; }
        public string ORDER_TYPE { get; set; }
        public decimal? INSTALL_COST { get; set; }
        public string REUSE_FLAG { get; set; }
        public string PRODUCT_NAME { get; set; }
        public string PHASE { get; set; }
        public string VENDOR_NAME { get; set; }
        public string ORDER_STATUS { get; set; }




        public string SUBCONTRACT_TYPE { get; set; }
        public string SUBCONTRACT_SUB_TYPE { get; set; }


        public string RULE_ID { get; set; }
        public string ADDRESS_ID { get; set; }
        public string BUILDING_NAME { get; set; }
    }

    public class ReportInstallationCostbyOrderUserGroupModel
    {
        public string GROUP_NAME { get; set; }
    }

    public class reportInstallFileModel
    {
        public static string csv = "";
    }

    public class reportInstallCFGQueryreportReturn
    {
        public string ret_code { get; set; }
        public List<CFGQueryreportModel> install_cur { get; set; }
        public List<CFGQueryreportModel> ma_cur { get; set; }
        public List<CFGQueryreportModel> cur { get; set; }
    }

    public class reportInstallOrderPackageDetailModel
    {
        public int ORDER_PKG_ID { get; set; }
        public string PACKAGE_CODE { get; set; }
        public string PACKAGE_NAME { get; set; }
        public string PACKAGE_CLASS { get; set; }
        public string IS_NEW { get; set; }
        public string NEW_PACKAGE { get { return IS_NEW == "Y" ? "Yes" : "No"; } }
        public string ORDER_NO { get; set; }
        public string ACCESS_NO { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public string CREATED_DATE_TEXT { get { return CREATED_DATE.ToDateTimeDisplayText(); } }
        public string CREATED_BY { get; set; }
        public DateTime? UPDATED_DATE { get; set; }
        public string UPDATED_DATE_TEXT { get { return UPDATED_DATE.ToDateTimeDisplayText(); } }
        public string UPDATED_BY { get; set; }

    }

    public class reportInstallOrderFeeDetailModel
    {
        public int ORDER_FEE_ID { get; set; }
        public string FEE_ID { get; set; }
        public string FEE_ID_TYPE { get; set; }
        public string FEE_NAME { get; set; }
        public decimal? ORDER_FEE_PRICE { get; set; }
        public string FEE_ACTION { get; set; }
        public string ORDER_NO { get; set; }
        public string ACCESS_NO { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public string CREATED_DATE_TEXT { get { return CREATED_DATE.ToDateTimeDisplayText(); } }
        public string CREATED_BY { get; set; }
        public DateTime? UPDATED_DATE { get; set; }
        public string UPDATED_DATE_TEXT { get { return UPDATED_DATE.ToDateTimeDisplayText(); } }
        public string UPDATED_BY { get; set; }

    }
}
