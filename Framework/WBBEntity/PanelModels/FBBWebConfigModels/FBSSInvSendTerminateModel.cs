using System.Collections.Generic;

namespace WBBEntity.PanelModels.FBBWebConfigModels
{
    public class FBSSInvSendTerminateModel
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

    }
    public class FBSSInvSendTerminateWriteLog
    {
        public string Access_No { get; set; }
        public string Trans_id { get; set; }
        public string Run_Group { get; set; }
        public string Action { get; set; }
        public string Main_Asset { get; set; }
        public string SubNumber { get; set; }
        public string CompanyCode { get; set; }
        public string DocDate { get; set; }
        public string PostDate { get; set; }
        public string AssetDate { get; set; }
        public string OrderNumber { get; set; }
        public string ItemText { get; set; }
    }
    public class FBSSInvSendTerminateReturn
    {
        public string ret_code { get; set; }
        public string ret_msg { get; set; }

        public List<FBSSInvSendTerminateModel> p_ws_revalue_cur { get; set; }
    }
}
