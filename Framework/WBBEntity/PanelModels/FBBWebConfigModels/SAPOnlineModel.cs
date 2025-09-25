using System;
using System.Collections.Generic;
using System.Data;

namespace WBBEntity.PanelModels.FBBWebConfigModels
{
    public class CostInstallation
    {
        public Nullable<decimal> ID { get; set; }
        public string SERVICE { get; set; }
        public string CUSTOMER { get; set; }
        public string CUSTOMER_NAME { get; set; }
        public Nullable<decimal> INTERNET_RATE { get; set; }
        public Nullable<decimal> PLAYBOX_RATE { get; set; }
        public Nullable<decimal> VOIP_RATE { get; set; }
        public string ORDER_TYPE { get; set; }
        public System.DateTime EFFECTIVE_DATE { get; set; }
        public Nullable<System.DateTime> EXPIRE_DATE { get; set; }
        public string REMARK { get; set; }
        public string CHK_PLAYBOX { get; set; }

        public string TMP_CODE_CUSTOMER_NAME { get; set; }
        public Nullable<decimal> LENGTH_FR { get; set; }
        public Nullable<decimal> LENGTH_TO { get; set; }
        public string TMP_LENGTH_FR_TO { get; set; }

        public Nullable<decimal> OUT_DOOR_PRICE { get; set; }
        public Nullable<decimal> IN_DOOR_PRICE { get; set; }

        public Nullable<System.DateTime> CREATE_DATE { get; set; }
        public string CREATE_BY { get; set; }
        public Nullable<System.DateTime> UPDATED_DATE { get; set; }
        public string UPDATED_BY { get; set; }
        public string ADDRESS_ID { get; set; }
        public Nullable<decimal> TOTAL_PRICE { get; set; }
        public string IS_DELETE_FIXASSCONFIG { get; set; }
    }
    public class FbssConfigTBL
    {
        public int CON_ID { get; set; }
        public string CON_TYPE { get; set; }
        public string CON_NAME { get; set; }
        public string DISPLAY_VAL { get; set; }
        public string VAL1 { get; set; }
        public string VAL2 { get; set; }
        public string VAL3 { get; set; }
        public string VAL4 { get; set; }
        public string VAL5 { get; set; }
        public string ACTIVEFLAG { get; set; }
        public int? ORDER_BY { get; set; }
        public string DEFAULT_VALUE { get; set; }
        public string CREATED_BY { get; set; }
        public Nullable<System.DateTime> CREATED_DATE { get; set; }
        public string UPDATED_BY { get; set; }
        public Nullable<System.DateTime> UPDATED_DATE { get; set; }
    }
    public class ReturnCreate
    {
        public Nullable<decimal> RETURN_CODE { get; set; }
        public string RETURN_MSG { get; set; }
    }
    public class SubmitFOAReport
    {
        public string TRANS_ID { get; set; }
        public string ACCESS_NUMBER { get; set; }
        public string ORDER_NO { get; set; }
        public string SUBCONTRACT_CODE { get; set; }
        public string SUBCONTRACT_NAME { get; set; }
        public string PRODUCT_NAME { get; set; }
        public string SERVICE_LIST { get; set; }
        public string ORDER_TYPE { get; set; }
        public string SUBMIT_FLAG { get; set; }
        public string STATUS { get; set; }
        public string MESSAGE { get; set; }
        public string REC_TYPE_I { get; set; }
        public string REC_TYPE_F { get; set; }
        public string REC_TYPE_A { get; set; }
        public string ERROR_MSG { get; set; }
        public List<SubmitFOAProduct> ProductList { get; set; }
        public List<SubmitFOAInstall> InstallList { get; set; }
        //public string createdDate { get; set; }
    }

    // R17.11
    public class SubmitFOAEquipmentReport
    {
        public string ACCESS_NUMBER { get; set; }
        public string ORDER_NO { get; set; }
        public string ORDER_TYPE { get; set; }
        public string SUBCONTRACT_CODE { get; set; }
        public string SUBCONTRACT_NAME { get; set; }
        public string PRODUCT_NAME { get; set; }
        public string SERVICE_NAME { get; set; }
        public string SUBMIT_FLAG { get; set; }
        public string REJECT_REASON { get; set; }
        public string SUBMIT_DATE { get; set; }
        public string OLT_NAME { get; set; }
        public string BUILDING_NAME { get; set; }
        public string MOBILE_CONTACT { get; set; }
        public string SN { get; set; }
        public string MATERIAL_CODE { get; set; }
        public string COMPANY_CODE { get; set; }
        public string PLANT { get; set; }
        public string STORAGE_LOCATION { get; set; }
        public string SN_TYPE { get; set; }
        public string MOVEMENT_TYPE { get; set; }
        public string STATUS { get; set; }
        public string ERR_CODE { get; set; }
        public string ERR_MSG { get; set; }
        public string MOVEMENT_TYPE_OUT { get; set; }
        public string MOVEMENT_TYPE_IN { get; set; }
        public List<SubmitFOAEquipment> ListEquipment { get; set; }

        public string SERIAL_NUMBER { get; set; }
        public string flag_auto_resend { get; set; }
        public string REC_TYPE { get; set; }

        // 3BB Integration Track OSS Multi Company
        public string PRODUCT_OWNER { get; set; }
        public string MAIN_PROMO_CODE { get; set; }
        public string TEAM_ID { get; set; }
        public string TRANS_ID { get; set; }
    }

    // R17.11
    public class SubmitFOAInstallationReport
    {
        public string ACCESS_NUMBER { get; set; }
        public string ASSET_CODE { get; set; }
        public string SUB_NUMBER { get; set; }
        public string PRODUCT_NAME { get; set; }
        public string SERVICE_NAME { get; set; }
        public decimal? INSTALLATION_COST { get; set; }
        //public double INSTALLATION_COST { get; set; }
        public string ORDER_DATE { get; set; }
        public string MODIFY_DATE { get; set; }
        public string TRAN_STATUS { get; set; }
        public List<SubmitFOAInstallation> ListInstallation { get; set; }
    }

    // R17.11
    public class SubmitFOAEquipment
    {
        public string ACCESS_NUMBER { get; set; }
        public string ORDER_NO { get; set; }
        public string ORDER_TYPE { get; set; }
        public string SUBCONTRACT_CODE { get; set; }
        public string SUBCONTRACT_NAME { get; set; }
        public string PRODUCT_NAME { get; set; }
        public string SERVICE_NAME { get; set; }
        public string SUBMIT_FLAG { get; set; }
        public string REJECT_REASON { get; set; }
        public string SUBMIT_DATE { get; set; }
        public string OLT_NAME { get; set; }
        public string BUILDING_NAME { get; set; }
        public string MOBILE_CONTACT { get; set; }
        public string SN { get; set; }
        public string MATERIAL_CODE { get; set; }
        public string COMPANY_CODE { get; set; }
        public string PLANT { get; set; }
        public string STORAGE_LOCATION { get; set; }
        public string SN_TYPE { get; set; }
        public string MOVEMENT_TYPE { get; set; }
        public string STATUS { get; set; }
        public string ERR_CODE { get; set; }
        public string ERR_MSG { get; set; }
        public string MOVEMENT_TYPE_OUT { get; set; }
        public string MOVEMENT_TYPE_IN { get; set; }
        public string ERR_DESC { get; set; }
        public string ADDRESS_ID { get; set; }
        public string ORG_ID { get; set; }
        public string REUSE_FLAG { get; set; }
        public string EVENT_FLOW_FLAG { get; set; }
        public string MATERIAL_DOC { get; set; }
        public string DOC_YEAR { get; set; }
        public string SUBCONTRACT_TYPE { get; set; }
        public string SUBCONTRACT_SUB_TYPE { get; set; }
        public string REQUEST_SUB_FLAG { get; set; }
        public string SUB_ACCESS_MODE { get; set; }
        public string TRANS_ID { get; set; }
        public decimal RowNumber { get; set; }
        public decimal CNT { get; set; }
        public string SERIAL_NUMBER { get; set; }
        public string flag_auto_resend { get; set; }

        //R21.04.2021
        public string REC_TYPE { get; set; }
        public string MODIFY_DATE { get; set; }

        // 3BB Integration Track OSS Multi Company
        public string PRODUCT_OWNER { get; set; }
        public string MAIN_PROMO_CODE { get; set; }
        public string TEAM_ID { get; set; }
        public  string TEMP_STAUS { get; set; }




    }

    public class SubmitFOAEquipmentListReturn
    {
        public string ret_code { get; set; }
        public List<SubmitFOAEquipment> cur { get; set; }
    }

    //R17.11
    public class SubmitFOAInstallation
    {
        public string ACCESS_NUMBER { get; set; }
        public string ASSET_CODE { get; set; }
        public string SUB_NUMBER { get; set; }
        public string PRODUCT_NAME { get; set; }
        public string SERVICE_NAME { get; set; }
        public decimal? INSTALLATION_COST { get; set; }
        //public double INSTALLATION_COST { get; set; }
        public string ORDER_DATE { get; set; }
        public string MODIFY_DATE { get; set; }
        public string TRAN_STATUS { get; set; }

        public string ORDER_NO { get; set; }

        // 3BB Integration Track OSS Multi Company
        public string PRODUCT_OWNER { get; set; }
        public string MAIN_PROMO_CODE { get; set; }
        public string TEAM_ID { get; set; }
        public string TRANS_ID { get; set; }
    }

    //R21.03.2021
    public class SubmitFOAInstallationNew
    {
        public string ACCESS_NUMBER { get; set; }
        public string ORDER_NO { get; set; }
        public string ORDER_TYPE { get; set; }
        public string SUBCONTRACT_CODE { get; set; }
        public string SUBCONTRACT_NAME { get; set; }
        public string PRODUCT_NAME { get; set; }
        public string SERVICE_NAME { get; set; }
        public string SUBMIT_FLAG { get; set; }
        public string REJECT_REASON { get; set; }
        public string SUBMIT_DATE { get; set; }
        public string OLT_NAME { get; set; }
        public string BUILDING_NAME { get; set; }
        public string MOBILE_CONTACT { get; set; }
        public string SN { get; set; }
        public string MATERIAL_CODE { get; set; }
        public string COMPANY_CODE { get; set; }
        public string PLANT { get; set; }
        public string STORAGE_LOCATION { get; set; }
        public string SN_TYPE { get; set; }
        public string MOVEMENT_TYPE { get; set; }
        public string STATUS { get; set; }
        public string ERR_CODE { get; set; }
        public string ERR_MSG { get; set; }
        public string MOVEMENT_TYPE_OUT { get; set; }
        public string MOVEMENT_TYPE_IN { get; set; }
        public string ERR_DESC { get; set; }
        public string ADDRESS_ID { get; set; }
        public string ORG_ID { get; set; }
        public string REUSE_FLAG { get; set; }
        public string EVENT_FLOW_FLAG { get; set; }
        public string MATERIAL_DOC { get; set; }
        public string DOC_YEAR { get; set; }
        public string SUBCONTRACT_TYPE { get; set; }
        public string SUBCONTRACT_SUB_TYPE { get; set; }
        public string REQUEST_SUB_FLAG { get; set; }
        public string SUB_ACCESS_MODE { get; set; }

        // 3BB Integration Track OSS Multi Company
        public string PRODUCT_OWNER { get; set; }
        public string MAIN_PROMO_CODE { get; set; }
        public string TEAM_ID { get; set; }
    }

    //begin 16.07.2018 main asset
    public class SubmitFOAMainAsset
    {
        public string ACCESS_NUMBER { get; set; }
        public string ORDER_NO { get; set; }
        public string ORDER_TYPE { get; set; }
        public string FLAG_TYPE { get; set; }
        public string SUBCONTRACT_CODE { get; set; }
        public string SUBCONTRACT_NAME { get; set; }
        public string PRODUCT_NAME { get; set; }
        public string COM_CODE_OLD { get; set; }
        public string COM_CODE_NEW { get; set; }
        public string ASSET_CLASS_GI { get; set; }
        public string COST_CENTER { get; set; }
        public string FOA_SUBMIT_DATE { get; set; }
        public string ASSET_CODE { get; set; }
        public string ASSET_TYPE { get; set; }
        public string ASSET_STATUS { get; set; }
        public string STATUS { get; set; }
    }

    //end 16.07.2018 main asset
    //begin  2.11.2018 Revalue
    public class SubmitFOARevalue
    {
        public string ACCESS_NUMBER { get; set; }
        public string ORDER_NO { get; set; }
        public string ORDER_TYPE { get; set; }
        public string RUN_GROUP { get; set; }
        public string ACTION { get; set; }
        public string MAIN_ASSET { get; set; }
        public string SUB_NUMBER { get; set; }
        public string COM_CODE { get; set; }
        public string DOC_DATE { get; set; }
        public string ERR_CODE { get; set; }
        public string ERR_MSG { get; set; }
        public string STATUS { get; set; }
        public string TRANS_ID { get; set; }
        public string ITEM_TEXT { get; set; }

        // 3BB Integration Track OSS Multi Company
        public string PRODUCT_OWNER { get; set; }
    }

    //end  2.11.2018 Revalue
    //R17.11
    public class ReplaceEquipmentModel
    {
        public string materialCode { get; set; }
        public string subcontractorCode { get; set; }
        public string companyCode { get; set; }
        public string postDate { get; set; }
        public string storLocation { get; set; }
        public string plant { get; set; }
        public string serial_number { get; set; }
        public string access_no { get; set; }
        public string order_no { get; set; }
        public string ch_status { get; set; }
        public string product_owner { get; set; }
        public string Tarns_id { get; set; }

    }

    public class SubmitFOAResend
    {
        public string AccessNo { get; set; }
        public string OrderNumber { get; set; }
        public string SubcontractorCode { get; set; }
        public string SubcontractorName { get; set; }
        public string ProductName { get; set; }
        public string ServiceName { get; set; }
        public string OrderType { get; set; }
        public List<SubmitFOAProduct> ProductList { get; set; }
        public List<SubmitFOAInstall> InstallList { get; set; }
        public string SubmitFlag { get; set; }
        public string RejectReason { get; set; }
        //public string FOA_Submit_date { get; set; }
        public string OLT_NAME { get; set; }
        public string BUILDING_NAME { get; set; }
        public string Mobile_Contact { get; set; }

        public DateTime? _SetFOA_Submit_date;
        public DateTime? FOA_Submit_date_value { set { _SetFOA_Submit_date = value; } }
        public string FOA_Submit_date { get { return _SetFOA_Submit_date != null ? _SetFOA_Submit_date.Value.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz") : ""; } }
        public string ADDRESS_ID { get; set; }
        public string ORG_ID { get; set; }
        public string REUSE_FLAG { get; set; }
        public string EVENT_FLOW_FLAG { get; set; }

        public string SUBCONTRACT_TYPE { get; set; }
        public string SUBCONTRACT_SUB_TYPE { get; set; }
        public string REQUEST_SUB_FLAG { get; set; }
        public string SUB_ACCESS_MODE { get; set; }
        public string PRODUCT_OWNER { get; set; }
        public string MAIN_PROMO_CODE { get; set; }
        public string TEAM_ID { get; set; }
    }

    public class SubmitFOAProduct
    {
        public string SerialNumber { get; set; }
        public string MaterialCode { get; set; }
        public string CompanyCode { get; set; }
        public string Plant { get; set; }
        public string StorageLocation { get; set; }
        public string SNPattern { get; set; }
        public string MovementType { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMassage { get; set; }
        public string RETURN_MSG { get; set; }
        public string Status { get; set; }
    }


    public class SubmitFOAResenorderdata
    {
        public string SerialNumber { get; set; }
        public string MaterialCode { get; set; }
        public string CompanyCode { get; set; }
        public string Plant { get; set; }
        public string StorageLocation { get; set; }
        public string trans_id { get; set; }
  
    }

    public class SubmitFOAInstall
    {
        public string MAIN_ASSET { get; set; }
        public string SUB_NUMBER { get; set; }
        public string SUBCONTRACT_CODE { get; set; }
        public string SUBCONTRACT_NAME { get; set; }
        public string COM_CODE { get; set; }
        public decimal? INSTALLATION_COST { get; set; }
        public string ERR_MSG { get; set; }
        public string PRODUCT_NAME { get; set; }
        public string ORDER_DATE { get; set; }
        public string MODIFY_DATE { get; set; }
        public string TRAN_STATUS { get; set; }
        public string RETURN_MSG { get; set; }
    }

    public class ListPendingSFFSubmitFOA
    {
        public string ACCESS_NUMBER { get; set; }
        public string ORDER_NO { get; set; }
        public string SN { get; set; }
        public string ORDER_DATE { get; set; }
    }

    public class ListResendToSFFSubmitFOA
    {
        public string ACCESS_NUMBER { get; set; }
        public string SIM_SN { get; set; }
    }

    public class ResultResendToSFFSubmitFOA
    {
        public Nullable<decimal> RETURN_CODE { get; set; }
        public string RETURN_MSG { get; set; }
    }

    public class CostInstallationExportList
    {
        public string ORDER_TYPE { get; set; }
        public string SERVICE { get; set; }
        public string CUSTOMER { get; set; }
        public string TYPE { get; set; }
        public string TMP_LENGTH_FR_TO { get; set; }
        public string INTERNET_RATE { get; set; }
        public string PLAYBOX_RATE { get; set; }
        public string VOIP_RATE { get; set; }
        public string EFFECTIVE_DATE { get; set; }
        public string EXPIRE_DATE { get; set; }
        public string REMARK { get; set; }

    }

    public class UpdateStatusSubmitFOAEquipmentModel
    {
        public string TRANS_ID { get; set; }
        public string ORDER_NO { get; set; }
        public string SERIAL_NO { get; set; }
        public string INTERNET_NO { get; set; }
        public string SUBNUMBER { get; set; }
        public string ASSET_CODE { get; set; }
        public string MATERIAL_DOC { get; set; }
        public string DOC_YEAR { get; set; }
        public string COM_CODE { get; set; }
        public string ERR_CODE { get; set; }
        public string ERR_MSG { get; set; }
        public string REMARK { get; set; }
    }
    public class SubmitFOAByFileModel
    {
        public static string csv = "";
        public static string xls = "";
        public static string xlxs = "";
    }
    public static class TempDataTableResendOrder
    {
        public static DataTable dt { get; set; }

    }
    public class ResendOrderFileList
    {
        public string ACCESS_NO { get; set; }
        public string ORDER_NO { get; set; }
        public string SUBCONTRACTOR_CODE { get; set; }
        public string SUBCONTRACTOR_NAME { get; set; }
        public string PRODUCT_NAME { get; set; }
        public string ORDER_TYPE { get; set; }
        public string SUBMIT_DATE { get; set; }
        public string SERIAL_NUMBER { get; set; }
        public string MATERIAL_CODE { get; set; }
        public string COMPANY_CODE { get; set; }
        public string PLANT { get; set; }
        public string STORAGE_LOCATION { get; set; }
        public string SN_PATTERN { get; set; }
        public string MOVEMENT_TYPE { get; set; }
        public string ERROR_CODE { get; set; }
        public string ERROR_MESSAGE { get; set; }
        public string STATUS { get; set; }
        public string MAIN_ASSET { get; set; }
        public string MATERIAL_DOC { get; set; }
        public string DOC_YEAR { get; set; }
        public string REMARK { get; set; }
        public string CHANGE_STATUS { get; set; }
        public string PRODUCT_OWNER { get; set; }
        public string TRANS_ID { get; set; }
    }

    public class PatchCPEModel
    {
        public string NO { get; set; }
        public string INTERNET_NO { get; set; }
        public string SERIAL_NUMBER { get; set; }
        public string SERIAL_STATUS { get; set; }
        public string FOA_CODE { get; set; }
        public string CREATE_DATE { get; set; }
        public string POST_DATE { get; set; }
        public string MOVEMENT_TYPE { get; set; }
        public string LOCATION_CODE { get; set; }
        public string REMARK { get; set; }
    }

    public class ResendOrderFileListIN
    {
        public string ACCESS_NO { get; set; }
        public string ASSET_CODE { get; set; }
        public string SUBCONTRACT_CODE { get; set; }
        public string SUBCONTRACT_NAME { get; set; }
        public string COM_CODE { get; set; }
        public string INSTALLATION_COST { get; set; }
      
        public string PRODUCT_NAME { get; set; }
        public string ORDER_DATE { get; set; }
        public string MODIFY_DATE { get; set; }
        public string TRAN_STATUS { get; set; }
        public string PRODUCT_OWNER { get; set; }
        public string ORDER_NO { get; set; }
        public string TRANS_ID { get; set; }
    }

    public class ResendOrderNewHandler
    {
        public string TRANS_ID { get; set; }
        public string MATERIAL_CODE { get; set; }
        public string COMPANY_CODE { get; set; }
        public string PLANT { get; set; }
        public string STORAGE_LOCATION { get; set; }
        public string SERIAL_NUMBER { get; set; }
        public short Return_Code { get; set; }
        public string Return_Desc { get; set; }
        public List<Resend_Oder_list_get> Data_list_get { get; set; }
    }

    public class Resend_Oder_list_get
    {
        public string ACCESS_NO { get; set; }
        public string ASSET_CODE { get; set; }
        public string SUBCONTRACT_CODE { get; set; }
        public string SUBCONTRACT_NAME { get; set; }
        public string COM_CODE { get; set; }
        public string INSTALLATION_COST { get; set; }

        public string PRODUCT_NAME { get; set; }
        public string ORDER_DATE { get; set; }
        public string MODIFY_DATE { get; set; }
        public string TRAN_STATUS { get; set; }
        public string PRODUCT_OWNER { get; set; }
        public string ORDER_NO { get; set; }
        public string TRANS_ID { get; set; }
    }



    public class FBSSConfig
    {
        public bool PROGRAM_PROCESS { get; set; }
        public List<FbssConfigTBL> VAL_LIST { get; set; }
    }
}
