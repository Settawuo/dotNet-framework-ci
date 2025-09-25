using System;
using System.Collections.Generic;
namespace WBBEntity.PanelModels.FBBWebConfigModels
{
    public class ReturnReverseSapModel
    {

        public List<ReturnCurSapmodel> p_ws_revalue_cur { get; set; }


        public string ret_code { get; set; }
        public string ret_msg { get; set; }


        public int ResSuccess { get; set; }
        public int ResError { get; set; }


    }
    public class ReverAssetWriteHisLog
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

    public class ReturnCurSapmodel
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

        public string NUMBER_MESSAGE { get; set; }
        public string ERROR_MESSAGE { get; set; }
    }
    public class ReverseAssetModel
    {
        public string ACCESS_NUMBER { get; set; }
        public string ASSET_CODE { get; set; }
        public string SUB_NUMBER { get; set; }
        public string ASSET_TYPE { get; set; }
        public DateTime? ORDER_DATE { get; set; }
        public string PRODUCT_NAME { get; set; }
        public string SERVICE_NAME { get; set; }
        public double INSTALLATION_COST { get; set; }
        public string ASSET_STATUS { get; set; }
        public string INTERNET_STATUS { get; set; }
        public DateTime? MODIFY_DATE { get; set; }
        public DateTime? ASSET_DATE { get; set; }
        //  public string REASON { get; set; }

    }
    public class ReverseAssetSearchModel
    {
        public string ACCESS_NUMBER { get; set; }
        public string ASSET_CODE { get; set; }
        public string ASSET_STATUS { get; set; }
        public string MODIFY_DATE_FORM { get; set; }
        public string ASSET_DATE_FORM { get; set; }
        public string MODIFY_DATE_TO { get; set; }
        public string ASSET_DATE_TO { get; set; }
        public string FILE_NAME { get; set; }

    }
    public class ReverseAssetAcessNumber
    {
        public string ACCESS_NUMBER { get; set; }
        public string ASSET_CODE { get; set; }
        public string ACTION { get; set; }

    }


}
