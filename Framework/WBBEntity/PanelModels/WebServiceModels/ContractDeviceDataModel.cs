using System.Collections.Generic;

namespace WBBEntity.PanelModels.WebServiceModels
{
    public class ContractDeviceDataModel
    {
        public List<ContractDeviceModel> ContractDeviceList { get; set; }
    }

    public class ContractDeviceModel
    {
        public string SFF_PROMOTION_CODE { get; set; }
        public string PRODUCT_SUBTYPE { get; set; }
        public string CONTRACT_ID { get; set; }
        public string CONTRACT_NAME { get; set; }
        public string DURATION_BY_MONTH { get; set; }
        public string DURATION { get; set; }
        public string CONTRACT_DISPLAY_TH_1 { get; set; }
        public string CONTRACT_DISPLAY_TH_2 { get; set; }
        public string CONTRACT_DISPLAY_EN_1 { get; set; }
        public string CONTRACT_DISPLAY_EN_2 { get; set; }
        public string CONTRACT_FLAG { get; set; }
        public string PENALTY_CHARGE { get; set; }
        public string CONTRACT_TYPE { get; set; }
        public string CONTRACT_RULE_ID { get; set; }
        public string PENALTY_TYPE { get; set; }
        public string PENALTY_ID { get; set; }
        public string LIMIT_CONTRACT { get; set; }
        public string COUNT_FLAG { get; set; }
        public string DEFAULT_FLAG { get; set; }
        public string CALL_TDM_FLAG { get; set; }
    }

    public class ContractDeviceOnlineQueryConfigBody
    {
        public string CHANNEL { get; set; }
        public string EVENT { get; set; }
        public string PRODUCT_SUBTYPE { get; set; } //R22.07
        public string OWNER_PRODUCT { get; set; } //R22.07
        public string SALE_CHANNEL { get; set; } //R22.07
        public PROMOTION_LIST[] PROMOTION_LIST { get; set; }
    }

    public class PROMOTION_LIST
    {
        public string PROMOTION_CODE { get; set; }
    }

    public class ContractDeviceOnlineQueryConfigResult
    {
        public string RESULT_CODE { get; set; }
        public string RESULT_DESC { get; set; }
        public string TRANSACTION_ID { get; set; }
        public List<ContractDeviceOnlineQueryPackage> ListDeviceContract { get; set; }
    }

    public class ContractDeviceOnlineQueryPackage
    {
        public string SFF_PROMOTION_CODE { get; set; }
        public string PRODUCT_SUBTYPE { get; set; }
        public string CONTRACT_ID { get; set; }
        public string CONTRACT_NAME { get; set; }
        public string DURATION_BY_MONTH { get; set; }
        public string DURATION { get; set; }
        public string CONTRACT_DISPLAY_TH_1 { get; set; }
        public string CONTRACT_DISPLAY_TH_2 { get; set; }
        public string CONTRACT_DISPLAY_EN_1 { get; set; }
        public string CONTRACT_DISPLAY_EN_2 { get; set; }
        public string CONTRACT_FLAG { get; set; }
        public string PENALTY_CHARGE { get; set; }
        public string CONTRACT_TYPE { get; set; }
        public string CONTRACT_RULE_ID { get; set; }
        public string PENALTY_TYPE { get; set; }
        public string PENALTY_ID { get; set; }
        public string LIMIT_CONTRACT { get; set; }
        public string COUNT_FLAG { get; set; }
        public string DEFAULT_FLAG { get; set; }
        public string CALL_TDM_FLAG { get; set; }

    }
}
