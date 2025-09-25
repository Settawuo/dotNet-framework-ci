using System.Collections.Generic;

namespace WBBEntity.PanelModels.WebServiceModels
{
    public class GetListQueryConfigContractModel
    {
        public string resultCode { get; set; }
        public string resultDescription { get; set; }
        public string developerMessage { get; set; }
        public List<ConfigurationContract> listConfigurationContract { get; set; }
    }

    public class ConfigurationContract
    {
        public string contractId { get; set; }
        public string contractType { get; set; }
        public string contractName { get; set; }
        public string contractRuleId { get; set; }
        public string penaltyId { get; set; }
        public string penaltyType { get; set; }
        public string limitContract { get; set; }
        public string countFlg { get; set; }
        public List<PenContractRuleBean> listPenContractRuleBean { get; set; }
        public List<PenPenaltyFeeBean> listPenPenaltyFeeBean { get; set; }
    }

    public class PenContractRuleBean
    {
        public string contractSeq { get; set; }
        public string contractRuleName { get; set; }
        public string contractRuleValue { get; set; }
        public string limitType { get; set; }
    }

    public class PenPenaltyFeeBean
    {
        public string penaltySeq { get; set; }
        public string penaltyFee { get; set; }
        public decimal? penaltyCharge { get; set; }
        public decimal? penaltyAmt { get; set; }
        public string penaltyStart { get; set; }
        public string penaltyEnd { get; set; }
        public string penaltyChargeGroup { get; set; }
    }

    public class InsertMasterTDMModel
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
