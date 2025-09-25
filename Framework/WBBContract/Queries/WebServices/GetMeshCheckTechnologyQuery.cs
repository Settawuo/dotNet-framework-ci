using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class GetMeshCheckTechnologyQuery : IQuery<MeshCheckTechnologyModel>
    {
        public string addressID { get; set; }
    }

    public class GetCheckTechnologyQuery : IQuery<CheckTechnologyModel>
    {
        public string P_OWNER_PRODUCT { get; set; }
        public string P_ADDRESS_ID { get; set; }
    }
}
