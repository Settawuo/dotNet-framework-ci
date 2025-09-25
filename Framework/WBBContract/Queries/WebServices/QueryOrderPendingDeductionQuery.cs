using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class QueryOrderPendingDeductionQuery : IQuery<QueryOrderPendingDeductionModel>
    {
        public string FullUrl { get; set; }
    }
}
