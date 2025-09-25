using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class GetServicePlayboxQuery : IQuery<GetServicePlayboxModels>
    {
        public string SERVICE_CODE { get; set; }
        public string SFF_PROMOTION_CODE { get; set; }
    }
}
