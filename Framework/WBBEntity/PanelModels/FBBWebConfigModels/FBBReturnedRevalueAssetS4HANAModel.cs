using System;
using System.Collections.Generic;

namespace WBBEntity.PanelModels.FBBWebConfigModels
{
    public class FBBReturnedRevalueAssetS4HANAModel
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
        public List<FBBReturnedGetOrderRevalueListModel> ret_cur_get_order_revalue { get; set; }
    }





    public class FBBReturnedRevalueAssetProductList
    {
        public string AccessNo { get; set; }
        public string OrderNumber { get; set; }
        public string SubcontractorCode { get; set; }
        public string SubcontractorName { get; set; }
        public string ProductName { get; set; }
        public string ServiceName { get; set; }
        public string OrderType { get; set; }
        public List<FBBReturnedRevalueAssetProduct> ProductList { get; set; }
        public List<FBBReturnedRevalueAssetInstall> InstallList { get; set; }
        public string ServiceList { get; set; }
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
    }

    public class FBBReturnedRevalueAssetProduct
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

    public class FBBReturnedRevalueAssetInstall
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


    public class FBBReturnedRevalueAssetServiceList
    {
        public string ServiceName { get; set; }
    }



    public class FBBReturnedGetOrderRevalueModel
    {
        //INPUT
        string P_DATE_START { get; set; }
        string P_DATE_TO { get; set; }
        string P_ORDER_TYPE { get; set; }


        //OUTPUT
        public List<FBBReturnedGetOrderRevalueListModel> ret_cur_get_order_revalue { get; set; }

    }

    public class FBBReturnedGetOrderRevalueListModel
    {
        string ACCESS_NUMBER { get; set; }
        string ORDER_NO { get; set; }

    }
    public class FBBReturnedRevalueAssetS4HANAWriteLog
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
    public class FBBReturnedRevalueAssetS4HANAReturn
    {
        public string ret_code { get; set; }
        public string ret_msg { get; set; }

        public List<FBBReturnedRevalueAssetS4HANAModel> p_ws_revalue_cur { get; set; }
    }
}
