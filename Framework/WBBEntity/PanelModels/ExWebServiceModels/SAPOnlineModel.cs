using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace WBBEntity.PanelModels.ExWebServiceModels
{
    public class NewRegistFOAList
    {
        [Required(ErrorMessage = "Access_No is required.")]
        [RegularExpression(@"^(8{2})[0-9]{8}", ErrorMessage = "The Access_No is not in the correct format!")]
        public string Access_No { get; set; }

        [Required(ErrorMessage = "OrderNumber is required.")]
        public string OrderNumber { get; set; }

        [Required(ErrorMessage = "SubcontractorCode is required.")]
        public string SubcontractorCode { get; set; }

        [Required(ErrorMessage = "SubcontractorName is required.")]
        public string SubcontractorName { get; set; }

        [Required(ErrorMessage = "ProductName is required.")]
        public string ProductName { get; set; }

        [Required(ErrorMessage = "ServiceName is required.")]
        public string ServiceName { get; set; }

        [Required(ErrorMessage = "OrderType is required.")]
        public string OrderType { get; set; }

        [Required(ErrorMessage = "ProductList is required.")]
        public List<NewRegistFOAProductList> ProductList { get; set; }

        [Required(ErrorMessage = "submitFlag is required.")]
        public string submitFlag { get; set; }
    }
    //[XmlRoot(ElementName = "entry")]

    [DataContract]
    public class NewRegistFOA
    {
        [DataMember(Order = 1)]
        public string Access_No { get; set; }
        [DataMember(Order = 2)]
        public string BUILDING_NAME { get; set; }
        [DataMember(Order = 3)]
        public string FOA_Submit_date { get; set; }
        [DataMember(Order = 4)]
        public string Mobile_Contact { get; set; }
        [DataMember(Order = 5)]
        public string OLT_NAME { get; set; }
        [DataMember(Order = 6)]
        public string OrderNumber { get; set; }
        [DataMember(Order = 7)]
        public string OrderType { get; set; }
        [DataMember(Order = 8)]
        public List<NewRegistFOAProductList> ProductList { get; set; }
        [DataMember(Order = 9)]
        public string ProductName { get; set; }
        [DataMember(Order = 10)]
        public string RejectReason { get; set; }
        [DataMember(Order = 11)]
        public List<NewRegistFOAServiceList> ServiceList { get; set; }
        [DataMember(Order = 12)]
        public string SubcontractorCode { get; set; }
        [DataMember(Order = 13)]
        public string SubcontractorName { get; set; }
        [DataMember(Order = 14)]
        public string SubmitFlag { get; set; }
        [DataMember(Order = 15)]
        public string Post_Date { get; set; }
        [DataMember(Order = 16)]
        public string Address_ID { get; set; }
        [DataMember(Order = 17)]
        public string ORG_ID { get; set; }

        [DataMember(Order = 18)]
        public string Reuse_Flag { get; set; }

        [DataMember(Order = 19)]
        public string Event_Flow_Flag { get; set; }
        //add new 18.06.28


        [DataMember(Order = 20)]
        public string UserName { get; set; }
        //add new 31.07.2018
        [DataMember(Order = 21)]
        public string Subcontract_Type { get; set; }

        [DataMember(Order = 22)]
        public string Subcontract_Sub_Type { get; set; }

        [DataMember(Order = 23)]
        public string Request_Sub_Flag { get; set; }

        [DataMember(Order = 24)]
        public string Sub_Access_Mode { get; set; }

        [DataMember(Order = 25)]
        public string Product_Owner { get; set; }

        [DataMember(Order = 26)]
        public string Main_Promo_Code { get; set; }

        [DataMember(Order = 27)]
        public string Team_ID { get; set; }
        public string ReturnMessage { get; set; }

    }

    public class NewRegistFOAServiceList
    {
        public string ServiceName { get; set; }
    }
    public class NewRegistFOAProductList
    {
        public string SerialNumber { get; set; }
        public string MaterialCode { get; set; }
        public string CompanyCode { get; set; }
        public string Plant { get; set; }
        public string StorageLocation { get; set; }
        public string SNPattern { get; set; }
        public string MovementType { get; set; }
    }
    public class NewRegistFOAResult
    {
        public string result { get; set; }
        public string errorReason { get; set; }
    }





    public class Register
    {
        public string REC_TYPE { get; set; }
        public string RUN_GROUP { get; set; }
        public string DOC_DATE { get; set; }
        public string POST_DATE { get; set; }
        public string COM_CODE { get; set; }
        public string ASSET_CLASS { get; set; }
        public string MAIN_ASSET { get; set; }
        public string SUBNUMBER { get; set; }
        public string ASSET_DESC1 { get; set; }
        public string ASSET_DESC2 { get; set; }
        public string INV_NO { get; set; }
        public string COSTCENTER { get; set; }
        public string EVA4 { get; set; }
        public string EVA5 { get; set; }
        public string XREF1_HD { get; set; }
    }
    public class JoinOrder
    {
        public string REC_TYPE { get; set; }
        public string RUN_GROUP { get; set; }
        public string REF_DOC_NO { get; set; }
        public string DOC_HEADER_TXT { get; set; }
        public string REF_DOC_ITEM { get; set; }
        public string MOVEMENT_TYPE { get; set; }
        public string MATERIAL_NO { get; set; }
        public string PLANT_FROM { get; set; }
        public string SLOC_FROM { get; set; }
        public string QUANTITY { get; set; }
        public string GL_ACCT { get; set; }
        public string ITEM_TEXT { get; set; }
        public string SERIAL_NO { get; set; }
    }
    public class InstallationCost
    {
        public string REC_TYPE { get; set; }
        public string RUN_GROUP { get; set; }
        public string Document_Type { get; set; }
        public string Currency { get; set; }
        public string Rate { get; set; }
        public string Translation_Date { get; set; }
        public string Doc_Header_Text { get; set; }
        public string Account { get; set; }
        public string Amount_in_Document_Currency { get; set; }
        public string Reference_Key_1 { get; set; }
        public string Reference_Key_2 { get; set; }
        public string Reference_Key_3 { get; set; }
    }
    public class TerminateService
    {
        public string Internet_Number { get; set; }
        public string Send_Type { get; set; }
        public string Asset_Code_List { get; set; }
        public string Order_Status { get; set; }
    }
    public class RenewService
    {
        public string Internet_Number { get; set; }
        public string Send_Type { get; set; }
        public List<ProductList> ProductList { get; set; }
        public string Create_datetime { get; set; }
    }
    //public class RevaluePending
    //{
    //    public string ACCESS_NUMBER { get; set; }
    //    public string ORDER_NO { get; set; }
    //    public string ORDER_TYPE { get; set; }
    //    public string RUN_GROUP { get; set; }
    //    public string ACTION { get; set; }
    //    public string MAIN_ASSET { get; set; }
    //    public string SUB_NUMBER { get; set; }
    //    public string COM_CODE { get; set; }
    //    public string DOC_DATE { get; set; }
    //    public string ERR_CODE { get; set; }
    //    public string ERR_MSG { get; set; }
    //    public string STATUS { get; set; }
    //    public string TRANS_ID { get; set; }
    //    public string ITEM_TEXT { get; set; }
    //}
    public class ProductList
    {
        public string Product_Name { get; set; }
        public string Product_Code { get; set; }
        public string CPE_Type { get; set; }
    }

    public class PkgFbbFoaOrderManagementResponse
    {
        public PkgFbbFoaOrderManagementResponse()
        {
            if (p_ws_main_cur == null)
            {
                p_ws_main_cur = new List<FBSS_SubmitFOAMainRespones>();
            }

            if (p_ws_inv_cur == null)
            {
                p_ws_inv_cur = new List<FBSS_SubmitFOAInvRespones>();
            }

            if (p_ws_ins_cur == null)
            {
                p_ws_ins_cur = new List<FBSS_SubmitFOAInsRespones>();
            }

            if (p_ws_sff_cur == null)
            {
                p_ws_sff_cur = new List<FBSS_SFFRespones>();
            }

            if (p_ws_revalue_cur == null)
            {
                p_ws_revalue_cur = new List<FBSS_SubmitFOARevalueResponse>();
            }

            if (p_ws_maintain_cur == null)
            {
                p_ws_maintain_cur = new List<FBSS_SubmitFOAMaintainResponse>();
            }
        }

        public string ret_code { get; set; }
        public string ret_msg { get; set; }
        public List<FBSS_SubmitFOAMainRespones> p_ws_main_cur { get; set; }
        public List<FBSS_SubmitFOAInvRespones> p_ws_inv_cur { get; set; }
        public List<FBSS_SubmitFOAInsRespones> p_ws_ins_cur { get; set; }
        public List<FBSS_SFFRespones> p_ws_sff_cur { get; set; }
        public List<FBSS_SubmitFOARevalueResponse> p_ws_revalue_cur { get; set; }
        public List<FBSS_SubmitFOAMaintainResponse> p_ws_maintain_cur { get; set; }
    }

    public class SAPResponse
    {
        public int RETURN_CODE { get; set; }
        public string RETURN_MESSAGE { get; set; }
    }

    public class NewRegistForSubmitFOAResponse
    {
        public string result { get; set; }
        public string errorReason { get; set; }
        public string new_response { get; set; }
        public string new_ErrorMsg { get; set; }
    }

    public class NewRegistForSubmitFOAS4HANAResponse
    {
        public string result { get; set; }
        public string errorReason { get; set; }
        public string new_response { get; set; }
        public string new_ErrorMsg { get; set; }
    }

    public class NewRegisterResponse
    {
        public string REC_TYPE { get; set; }
        public string RUN_GROUP { get; set; }
        public string REFERENCE { get; set; }
        public string TYPE { get; set; }
        public string COMPANY { get; set; }
        public string MAIN_ASSET { get; set; }
        public string SUBNUMBER { get; set; }
        public string NUMBER { get; set; }
        public string MESSAGE { get; set; }
    }
    public class JoinOrderResponse
    {
        public string REC_TYPE { get; set; }
        public string RUN_GROUP { get; set; }
        public string REF_DOC_NO { get; set; }
        public string REF_DOC_ITEM { get; set; }
        public string MATERIAL_NO { get; set; }
        public string MATERIAL_DOC { get; set; }
        public string ASSET_NO { get; set; }
        public string ASSET_SUB { get; set; }
        public string DOC_YEAR { get; set; }
        //*!error
        public string SERIAL_NO { get; set; }
        public string ERR_CODE { get; set; }
    }
    public class InstallationCostResponse
    {
        public string REC_TYPE { get; set; }
        public string RUN_GROUP { get; set; }
        public string REF_LEGACY { get; set; }
        public string COMPANY { get; set; }
        public string NUMBER { get; set; }
        public string MESSAGE { get; set; }
        //*!success
        public string DOCNO { get; set; }
        public string YEAR { get; set; }
    }

    public class TerminateServiceResponse : SAPResponse
    {
    }

    public class RenewServiceResponse : SAPResponse
    {
        public string ASSET_CODE { get; set; }
    }

    public class FBSS_SubmitFOARevalueResponse
    {

        public string FLAG_TYPE { get; set; }
        public string TRANS_ID { get; set; }
        public string REC_TYPE { get; set; }
        public string RUN_GROUP { get; set; }
        public string COM_CODE { get; set; }
        public string ASSET_CLASS { get; set; }
        public string MAIN_ASSET { get; set; }
        public string SUBNUMBER { get; set; }
        public string PRODUCT_NAME { get; set; }
        public string ASSET_DESC2 { get; set; }
        public string INTERNET_NO { get; set; }
        public string COSTCENTER { get; set; }
        public string EVA4 { get; set; }
        public string EVA5 { get; set; }
        public string ORDER_NO { get; set; }

        public string ACTION { get; set; }
        public string ENTRY_DATE { get; set; }
        public string ENTRY_TIME { get; set; }
        public string ENTRY_BY { get; set; }
        public string DOC_DATE { get; set; }
        public string POST_DATE { get; set; }
        public string ASSET_VALUE_DATE { get; set; }
        public string REF_DOC_NO { get; set; }
        public string ITEM_TEXT { get; set; }
        public string ASSIGNMENT { get; set; }

        public string NUMBER_MESSAGE { get; set; }
        public string ERROR_MESSAGE { get; set; }
        public string ITEM_NUMBER { get; set; }


    }

    public class FBSS_SubmitFOAMaintainResponse
    {
        public string DOC_DATE { get; set; }
        public string POST_DATE { get; set; }
        public string TRANS_ID { get; set; }
        public string REF_DOC { get; set; }
        public string RUN_GROUP { get; set; }
        public string MOVEMENT_TYPE { get; set; }
        public string MATERIAL_NO { get; set; }
        public string PLANT_FROM { get; set; }
        public string SLOC_FROM { get; set; }
        public string PLANT_TO { get; set; }
        public string SLOC_TO { get; set; }
        public string QUANTITY { get; set; }
        public string UOM { get; set; }
        public string AMOUNT { get; set; }
        public string COST_CENTER { get; set; }
        public string GL_ACCT { get; set; }
        public string GOODS_RECIPIENT { get; set; }
        public string SERIAL_NO { get; set; }
        public string MATERIAL_DOC { get; set; }
        public string DOC_YEAR { get; set; }
        public string ITEM_TEXT { get; set; }
        public string REF_DOC_FBSS { get; set; }
        public string XREF1_HD { get; set; }

    }

    public class FBSS_SubmitFOAMainRespones
    {
        public string FLAG_TYPE { get; set; }
        public string TRANS_ID { get; set; }
        public string REC_TYPE { get; set; }
        public string RUN_GROUP { get; set; }
        public string COM_CODE { get; set; }
        public string ASSET_CLASS { get; set; }
        public string MAIN_ASSET { get; set; }
        public string SUBNUMBER { get; set; }
        public string PRODUCT_NAME { get; set; }
        public string ASSET_DESC2 { get; set; }
        public string INTERNET_NO { get; set; }
        //  public string REF_DOC_NO { get; set; }
        public string COSTCENTER { get; set; }
        public string EVA4 { get; set; }
        public string EVA5 { get; set; }

        public string ORDER_NO { get; set; }
    }
    public class FBSS_SubmitFOAInvRespones
    {
        //GI
        public string FLAG_TYPE { get; set; }
        public string TRANS_ID { get; set; }
        public string REC_TYPE { get; set; }
        public string RUN_GROUP { get; set; }
        public string DOC_DATE { get; set; }
        public string POST_DATE { get; set; }
        public string INTERNET_NO { get; set; }
        public string REF_DOC_NO { get; set; }
        public string COM_CODE { get; set; }
        public string ORDER_NO { get; set; }
        public string MAIN_ASSET { get; set; }
        public string SUBNUMBER { get; set; }

        //Product List
        public string MATERIAL_NO { get; set; }
        public string PLANT { get; set; }
        public string STORAGE_LOCATION { get; set; }
        public string QUANTITY { get; set; }
        public string MOVEMENT_TYPE { get; set; }
        public string ITEM_TEXT { get; set; }
        public string SERIAL_NO { get; set; }
        public string XREF1_HD { get; set; }
    }
    public class FBSS_SubmitFOAInsRespones
    {
        //INS 
        public string FLAG_TYPE { get; set; }
        public string TRANS_ID { get; set; }
        public string REC_TYPE { get; set; }
        public string RUN_GROUP { get; set; }
        public string COM_CODE { get; set; }
        public string ASSET_CLASS { get; set; }
        public string MAIN_ASSET { get; set; }
        public string SUBNUMBER { get; set; }
        public string ASSET_DESC1 { get; set; }
        public string ASSET_DESC2 { get; set; }
        public string INTERNET_NO { get; set; }
        public string COSTCENTER { get; set; }
        public string EVA4 { get; set; }
        public string EVA5 { get; set; }
        public string DOCUMENT_TYPE { get; set; }
        public string DOC_DATE { get; set; }
        public string POST_DATE { get; set; }
        public string REF_DOC_NO { get; set; }
        public string XREF1_HD { get; set; }
        public string CURRENCY { get; set; }
        public string RATE { get; set; }
        public string TRANSLATION_DATE { get; set; }
        public string ACCOUNT { get; set; }
        public string AMOUNT { get; set; }
        public string REFERENCE_KEY1 { get; set; }
        public string REFERENCE_KEY2 { get; set; }
        public string REFERENCE_KEY3 { get; set; }
        public string ITEM_TEXT { get; set; }
        public string ASSIGNMENT { get; set; }
    }

    public class FBSS_SFFRespones
    {
        public string SN { get; set; }
        public string ACCESS { get; set; }
    }

    public class FBB_product_list
    {
        public string SN { get; set; }
        public string MATERIAL_CODE { get; set; }
        public string COMPANY_CODE { get; set; }
        public string PLANT { get; set; }
        public string STORAGE_LOCATION { get; set; }
        public string SNPattern { get; set; }
        public string MOVEMENT_TYPE { get; set; }
    }

    public class HistorySubmitFOAResend
    {
        public string ORDER_NO { get; set; }
        public string XML_DATA { get; set; }
        public string FLAG { get; set; }
    }

    public class SYMPTOM
    {
        public string SYMPTOM_CODE { get; set; }
        public string SYMPTOM_GROUP { get; set; }
        public string LONG_VALUE { get; set; }
        public string DISPLAY_VALUE { get; set; }

    }
    public class lostTranQueryResponse
    {
        public string XMLRESULT { get; set; }

    }
}
