using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class CheckNotiProductQuery : IQuery<CheckNotiProductModel>
    {
        public string FullUrl { get; set; }
        public string transaction_id { get; set; }
        public string order_transaction_id { get; set; }
    }
}