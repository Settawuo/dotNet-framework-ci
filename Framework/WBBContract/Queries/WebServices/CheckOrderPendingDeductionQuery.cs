using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class CheckOrderPendingDeductionQuery : IQuery<CheckOrderPendingDeductionModel>
    {
        public string TransactionId { get; set; }
        public string OrderTransactionId { get; set; }
        public string FullUrl { get; set; }
    }
}
