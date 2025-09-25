using System.Collections.Generic;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class GetMeshCompareDeviceQuery : IQuery<MeshCompareDeviceModel>
    {
        public string FibrenetID { get; set; }
        public string Channel { get; set; }
        public string flagPenalty { get; set; }
        public string lang { get; set; }
        public string RegisterDate { get; set; }
        public string ContractFlagFbb { get; set; }
        public string CountContractFbb { get; set; }
        public string FBBLimitContract { get; set; }
        public List<FBSSQueryCPEPenaltyModel> CompareArray { get; set; }
    }
}
