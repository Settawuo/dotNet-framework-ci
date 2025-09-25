using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class GetMeshCompareDeviceOrderQuery : IQuery<MeshCompareDeviceOrderModel>
    {
        public string FibrenetID { get; set; }
        public string Channel { get; set; }
        public string MeshBrandName { get; set; }
        public string BuyMesh { get; set; }
        public string PenaltyInstall { get; set; }
        public string ContractID { get; set; }
        public string ContractName { get; set; }
        public string Duration { get; set; }
        //R22.05 Device Contract Mesh
        public string ContractRuleId { get; set; }
        public string PenaltyType { get; set; }
        public string PenaltyId { get; set; }
        public string CountFlag { get; set; }
    }
}
