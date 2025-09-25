using System.Collections.Generic;

namespace WBBEntity.PanelModels.WebServiceModels
{
    public class GoodsMovementKAFKAQueryModel
    {
        public string ret_code { get; set; }
        public string ret_msg { get; set; }

        public string ret_code_pending_asst { get; set; }
        public string ret_msg_pending_asst { get; set; }
        public string return_transactions { get; set; }
        public List<string> item_json { get; set; }
    }

    public class GoodsMovementKAFKAList
    {
        public string p_TRANS_ID { get; set; }
        public string p_REC_TYPE { get; set; }
        public string p_RUN_GROUP { get; set; }
        public string p_INTERNET_NO { get; set; }
        public string p_ORDER_NO { get; set; }
        public string p_COM_CODE { get; set; }
        public string p_ASSET_CODE { get; set; }
        public string p_SUBNUMBER { get; set; }
        public string p_MATERIAL_NO { get; set; }
        public string p_SERIAL_NO { get; set; }
        public string p_MATERIAL_DOC { get; set; }
        public string p_DOC_YEAR { get; set; }
        public string p_ERR_STATUS { get; set; }
        public string p_ERR_CODE { get; set; }
        public string p_ERR_MSG { get; set; }
        public string p_REF_DOC_NO { get; set; }
    }

}
