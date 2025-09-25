using System;

namespace WBBEntity.PanelModels.FBBWebConfigModels
{
    public class SubContractEmailInfoModel
    {
        public int ORG_ID { get; set; }
        public string REGION { get; set; }
        public string SUB_CONTRACTOR_NAME_TH { get; set; }
        public string SUB_CONTRACTOR_NAME_EN { get; set; }
        public Nullable<DateTime> MODIFY_DT { get; set; }
        public Nullable<DateTime> CREATE_DT { get; set; }
        public string ROW_ID { get; set; }
        public string ACTION_FLAG { get; set; }
        public string STORAGE_LOCATION { get; set; }
        public string SUBCONTRACT_EMAIL { get; set; }
        public string PHASE { get; set; }
        public string SUB_CONTRACTOR_CODE { get; set; }
        public string SUB_CONTRACTOR_FOR_MAIL { get; set; }
        public string PRODUCT_NAME { get; set; }
        public string PRODUCT_FOR_CHECK { get; set; }
        public string POST_INSTALL { get; set; }
        public string ADDRESS_ID { get; set; }
        public string MODIFY_DT_TEXT { get; set; }
        public string CREATE_DT_TEXT { get; set; }

    }
    public class SearchSubContractEmailInfoModel
    {

        public string subcontract_name { get; set; }
        public string storage { get; set; }
        public string subcontract_code { get; set; }
        public string action_flag { get; set; }

    }
}
