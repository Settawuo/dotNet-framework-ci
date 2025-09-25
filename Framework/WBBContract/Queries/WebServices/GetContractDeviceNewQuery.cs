using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class GetContractDeviceNewQuery : IQuery<GetContractDeviceNewModel>
    {
        public string TransactionId { get; set; }
        public string P_PROMOTION_CODE_MAIN { get; set; }
        public string P_MESH_FLAG { get; set; }
        public string P_DURATION { get; set; }
    }
}
