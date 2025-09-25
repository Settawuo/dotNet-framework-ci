using System;
using System.Collections.Generic;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries.SAPOnline
{
    public class FBBReturnedFixedAssetQuery : IQuery<FBBReturnedFixedAssetModel>
    {
    }

    public class SendReturnedMAQuery : IQuery<SendReturnedMAModel>
    {
        public string p_ACCESS_NUMBER { get; set; }
        public string p_ORDER_NO { get; set; }
        public string p_ORDER_TYPE { get; set; }
        public string p_SUBCONTRACT_CODE { get; set; }
        public string p_SUBCONTRACT_NAME { get; set; }
        public string p_PRODUCT_NAME { get; set; }
        public string p_SERVICE_LIST { get; set; }
        public List<FBBProductModel> p_Product_List { get; set; }
        public string p_SUBMIT_FLAG { get; set; }
        public string p_Reject_reason { get; set; }
        public DateTime? p_foa_submit_date { get; set; }
        public string p_post_date { get; set; }
        public string p_olt_name { get; set; }
        public string p_building_name { get; set; }
        public string p_mobile_contact { get; set; }
        public string p_addess_id { get; set; }
        public string p_org_id { get; set; }
        public string p_reuse_flag { get; set; }
        public string p_event_flow_flag { get; set; }
        public string p_subcontract_type { get; set; }
        public string p_subcontract_sub_type { get; set; }
        public string p_request_sub_flag { get; set; }
        public string p_sub_access_mode { get; set; }
    }
}
