using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class CreateOrderSCPEQuery : IQuery<CreateOrderSCPEModel>
    {
        public string FullUrl { get; set; }
        public string OrderId { get; set; }
        public string txn_id { get; set; }
        public string Channel { get; set; }
        public string IdCardNo { get; set; }
    }
}
