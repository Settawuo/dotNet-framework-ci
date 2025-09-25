using System;
using System.Collections.Generic;

namespace WBBEntity.PanelModels.FBBWebConfigModels
{
    public class FBBReturnedFixedAssetModel
    {
        public string ret_code { get; set; }
        public List<FBBReturnedCpeModel> cur { get; set; }
    }

    public class FBBReturnedCpeModel
    {
        public string ACCESS_NUMBER { get; set; }
        public string ORDER_NO { get; set; }
        public string ORDER_TYPE { get; set; }
        public string SUBCONTRACT_CODE { get; set; }
        public string SUBCONTRACT_NAME { get; set; }
        public string PRODUCT_NAME { get; set; }
        public string SERVICE_LIST { get; set; }
        public string PRODUCT_LIST { get; set; }
        public string SUBMIT_FLAG { get; set; }
        public string REJECT_REASON { get; set; }
        public DateTime? FOA_SUBMIT_DATE { get; set; }
        public string POST_DATE { get; set; }
        public string OLT_NAME { get; set; }
        public string BUILDING_NAME { get; set; }
        public string MOBILE_CONTACT { get; set; }
        public string ADDESS_ID { get; set; }
        public string ORG_ID { get; set; }
        public string REUSE_FLAG { get; set; }
        public string EVENT_FLOW_FLAG { get; set; }
        public string SUBCONTRACT_TYPE { get; set; }
        public string SUBCONTRACT_SUB_TYPE { get; set; }
        public string REQUEST_SUB_FLAG { get; set; }
        public string SUB_ACCESS_MODE { get; set; }

        // 3BB Integration Track OSS Multi Company
        public string PRODUCT_OWNER { get; set; }
        public string MAIN_PROMO_CODE { get; set; }
        public string TEAM_ID { get; set; }
    }

    public class FBBProductModel
    {
        public string SN { get; set; }
        public string MATERIAL_CODE { get; set; }
        public string COMPANY_CODE { get; set; }
        public string PLANT { get; set; }
        public string STORAGE_LOCATION { get; set; }
        public string SN_PATTERN { get; set; }
        public string MOVEMENT_TYPE { get; set; }
    }

    public class SendReturnedMAModel
    {
        public List<WSReturnedMaintainModel> p_ws_maintain_cur { get; set; }
        public string ret_code { get; set; }
        public string ret_msg { get; set; }
    }

    public class WSReturnedMaintainModel
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
}
