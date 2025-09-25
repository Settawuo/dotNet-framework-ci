using System.Collections.Generic;

namespace WBBEntity.PanelModels.WebServiceModels
{
    public class MeshCompareDeviceOrderModel
    {
        public MeshCompareDeviceOrderModel()
        {
            if (RES_COMPLETE_CUR == null)
                RES_COMPLETE_CUR = new List<CompareDeviceOrder>();
        }
        public string RETURN_FLAG_TDM { get; set; }
        public string RETURN_CONTRACT_ID { get; set; }
        public string RETURN_CONTRACT_NAME { get; set; }
        public string RETURN_CODE { get; set; }
        public string RETURN_MESSAGE { get; set; }
        public List<CompareDeviceOrder> RES_COMPLETE_CUR { get; set; }
    }

    public class CompareDeviceOrder
    {
        public string REPLACE_ONU { get; set; }
        public string REPLACE_WIFI { get; set; }
        public string NUMBER_OF_MESH { get; set; }
        public string CONTRACT_ID { get; set; }
        public string CONTRACT_RULE_ID { get; set; }
        public string PENALTY_TYPE { get; set; }
        public string PENALTY_ID { get; set; }
        public string CONTRACT_FLAG { get; set; }
        public string DURATION { get; set; }

    }

}
