using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.FBBWebConfigQueries
{
    public class OrderPaymentQuery : IQuery<OrderPaymentModel>
    {
        public string p_order_id { get; set; }
    }
}
