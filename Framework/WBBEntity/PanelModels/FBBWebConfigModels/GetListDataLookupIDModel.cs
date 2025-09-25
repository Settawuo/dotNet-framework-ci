using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WBBEntity.PanelModels.FBBWebConfigModels
{
    public class GetListDataLookupIDModel
    {
        public GetListDataLookupIDModel()
        {
            if (result_lookup_id_cur_data == null)
            {
                result_lookup_id_cur_data = new List<result_lookup_id_cur>();
            }

        }
        public string return_param_name { get; set; }
        public string return_ontop_flag { get; set; }
        public string return_ontop_lookup { get; set; }

        public string return_code { get; set; }
        public string return_msg { get; set; }

        public List<result_lookup_id_cur> result_lookup_id_cur_data { get; set; }
    }
    public class result_lookup_id_cur
    {
        // column
        public string LOOKUP_ID { get; set; }
        public string LOOKUP_NAME { get; set; }
        public string p_ORDER_TYPE { get; set; }
        public string p_PRODUCT_NAME { get; set; }
        public string p_Reject_reason { get; set; }
        public string p_product_owner { get; set; }
        public string v_same_day { get; set; }
        public string p_event_flow_flag { get; set; }
        public string p_request_sub_flag { get; set; }
        public string v_province { get; set; }
        public string v_district { get; set; }
        public string v_subdistrict { get; set; }
        public string p_addess_id { get; set; }
        public string v_fttr_flag { get; set; }
        public string p_subcontract_type { get; set; }
        public string p_subcontract_sub_type { get; set; }
        public string v_region { get; set; }
        public string p_org_id { get; set; }
        public string p_SUBCONTRACT_CODE { get; set; }
        public string p_SUBCONTRACT_NAME { get; set; }
        public string v_reused_flag { get; set; }
        public string distance_from { get; set; }
        public string distance_to { get; set; }
        public string v_subcontract_location { get; set; }
        public string indoor_cost { get; set; }
        public string outdoor_cost { get; set; }
        public string v_over_cost_pm { get; set; }
        public string v_max_distance { get; set; }
        public string base_price { get; set; }
        public string effective_date_start { get; set; }
        public string effective_date_to { get; set; }
        public string v_symptom_group { get; set; }
        public string v_same_subs { get; set; }
        public string v_same_team { get; set; }
        public string p_main_promo_code { get; set; }
    }

}
