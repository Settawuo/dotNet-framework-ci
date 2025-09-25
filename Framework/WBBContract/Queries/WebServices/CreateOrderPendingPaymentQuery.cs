using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class CreateOrderPendingPaymentQuery : IQuery<CreateOrderPendingPaymentModel>
    {
        public string payment_method { get; set; }
        public string UpdateBy { get; set; }
    }
}
