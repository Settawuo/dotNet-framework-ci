using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class GetDeliveryIPCameraQuery : IQuery<GetDeliveryIPCameraModel>
    {
        public string province { get; set; }
    }
}
