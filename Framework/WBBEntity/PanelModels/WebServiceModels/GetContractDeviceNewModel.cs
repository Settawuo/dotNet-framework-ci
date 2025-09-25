using System.Collections.Generic;

namespace WBBEntity.PanelModels.WebServiceModels
{
    public class GetContractDeviceNewModel
    {
        public string RETURN_CODE { get; set; }
        public string RETURN_MESSAGE { get; set; }
        public List<ContractDeviceNewModel> LIST_MASTER { get; set; }
    }

    public class ContractDeviceNewModel
    {
        public string P_CONTRACT_ID { get; set; }
        public string P_CONTRACT_NAME { get; set; }
        public string P_CONTRACT_RULE_ID { get; set; }
        public string P_PENALTY_TYPE { get; set; }
        public string P_PENALTY_ID { get; set; }
        public string P_CONTRACT_FLAG { get; set; }
        public string P_DURATION { get; set; }
    }
}
