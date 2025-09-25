using WBBEntity.PanelModels.WebServices;

namespace WBBContract.Queries.WebServices
{
    public class CreateOrderMeshQuery : IQuery<CreateOrderMeshModel>
    {
        public string FullUrl { get; set; }
        public string OrderId { get; set; }
        public string txn_id { get; set; }
        public string Channel { get; set; }

        public string InternetNo { get; set; }
    }
}
