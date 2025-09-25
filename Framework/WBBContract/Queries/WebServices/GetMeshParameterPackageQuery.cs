using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class GetMeshParameterPackageQuery : IQuery<MeshParameterPackageModel>
    {
        public string FibrenetID { get; set; }
        public string PromotionCode { get; set; }
    }
}
