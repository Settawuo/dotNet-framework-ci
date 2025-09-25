using System.Collections.Generic;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Commands
{
    public class InsertMasterTDMContractDeviceCommand
    {
        public string Transaction_Id { get; set; }
        public string P_RESULT_CODE_TDM { get; set; }
        public string P_CONTRACT_ID { get; set; }
        public string P_CONTRACT_NAME { get; set; }
        public string P_CONTRACT_TYPE { get; set; }
        public string P_CONTRACT_RULE_ID { get; set; }
        public string P_PENALTY_TYPE { get; set; }
        public string P_PENALTY_ID { get; set; }
        public string P_LIMIT_CONTRACT { get; set; }
        public string P_DURATION { get; set; }
        public string RETURN_CODE { get; set; }
        public string RETURN_MESSAGE { get; set; }
        public List<InsertMasterTDMModel> LIST_PARA_SAVE { get; set; }
    }

    public class InsertMasterTDMContractDeviceMeshCommand
    {
        public string Transaction_Id { get; set; }
        public string P_RESULT_CODE_TDM { get; set; }
        public string P_CONTRACT_ID { get; set; }
        public string P_CONTRACT_NAME { get; set; }
        public string P_CONTRACT_TYPE { get; set; }
        public string P_CONTRACT_RULE_ID { get; set; }
        public string P_PENALTY_TYPE { get; set; }
        public string P_PENALTY_ID { get; set; }
        public string P_LIMIT_CONTRACT { get; set; }
        public string P_COUNT_FLAG { get; set; }
        public string P_DURATION { get; set; }

        public string RETURN_CODE { get; set; }
        public string RETURN_MESSAGE { get; set; }
        public List<DeviceContractMeshData> LIST_DEVICE_CONTRACT { get; set; }
    }

    public class DeviceContractMeshData
    {
        public string CONTRACT_ID { get; set; }
        public string CONTRACT_RULE_ID { get; set; }
        public string PENALTY_TYPE { get; set; }
        public string PENALTY_ID { get; set; }
        public string CONTRACT_FLAG { get; set; }
        public string DURATION { get; set; }
    }
}
