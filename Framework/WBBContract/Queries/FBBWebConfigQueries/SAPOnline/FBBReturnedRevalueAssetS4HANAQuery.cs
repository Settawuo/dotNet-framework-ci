using System;
using System.Collections.Generic;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries.SAPOnline
{
    public class FBBReturnedRevalueAssetS4HANAQuery : IQuery<FBBReturnedRevalueAssetS4HANAReturn>
    {
        //Date
        public string p_date_start { get; set; }
        public string p_date_to { get; set; }



        public string p_term_date { get; set; }
        public string P_ACCESS_NUMBER { get; set; }
        public string P_ORDER_NO { get; set; }
        public string P_ORDER_TYPE { get; set; }
        public string P_SUBCONTRACT_CODE { get; set; }
        public string P_SUBCONTRACT_NAME { get; set; }
        public string P_PRODUCT_NAME { get; set; }

        public List<ProcutListRec> P_PRODUCT_LIST { get; set; }
        //public List<string> P_SERVICE_LIST { get; set; } // Adjust the type if necessary
        public string P_SUBMIT_FLAG { get; set; }
        public string P_REJECT_REASON { get; set; }
        public string P_FOA_SUBMIT_DATE { get; set; }
        public string P_POST_DATE { get; set; }
        public string P_OLT_NAME { get; set; }
        public string P_BUILDING_NAME { get; set; }
        public string P_MOBILE_CONTACT { get; set; }
        public string P_ADDESS_ID { get; set; }
        public string P_ORG_ID { get; set; }
        public string P_REUSE_FLAG { get; set; }
        public string P_EVENT_FLOW_FLAG { get; set; }
        public string P_SUBCONTRACT_TYPE { get; set; }
        public string P_SUBCONTRACT_SUB_TYPE { get; set; }
        public string P_REQUEST_SUB_FLAG { get; set; }
        public string P_SUB_ACCESS_MODE { get; set; }

    }

    public class ProcutListRec
    {
        // P_PRODUCT_LIST
        public string SN { get; set; }
        public string MATERIAL_CODE { get; set; }
        public string COMPANY_CODE { get; set; }
        public string PLANT { get; set; }
        public string STORAGE_LOCATION { get; set; }
        public string SN_PATTERN { get; set; }
        public string MOVEMENT_TYPE { get; set; }
    }


    public class FBBReturnedRevalueAssetS4HANAQueryReturn
    {
        public string TRANS_ID { get; set; }
        public string INTERNET_NO { get; set; }
        public string RUN_GROUP { get; set; }
        public string ACTION { get; set; }
        public string MAIN_ASSET { get; set; }
        public string SUBNUMBER { get; set; }
        public string COM_CODE { get; set; }
        public string DOC_DATE { get; set; }
        public string POST_DATE { get; set; }
        public string ASSET_VALUE_DATE { get; set; }
        public string REF_DOC_NO { get; set; }
        public string ITEM_TEXT { get; set; }
        public string ASSIGNMENT { get; set; }

    }


    public class POSTSAPResponseRev
    {
        public string MessageID { get; set; }
        public string PartnerName { get; set; }
        public string PartnerMessageID { get; set; }
        public string MessageType { get; set; }
        public string MessageDesc { get; set; }
        public List<POSTSAPItemRev> ITEMS { get; set; }
    }

    public class POSTSAPItemRev
    {
        public string ItemNumber { get; set; }
        public string MessageType { get; set; }
        public string MessageDesc { get; set; }
        public string ItemID { get; set; }
        public string CompanyCode { get; set; }
        public string AssetNumber { get; set; }
        public string AssetSubnumber { get; set; }
        public string Reference { get; set; }
        public string EntryDate { get; set; }
    }


    public class OrderRevalue
    {
        public string ACCESS_NUMBER { get; set; }
        public string ORDER_NO { get; set; }

    }
}
