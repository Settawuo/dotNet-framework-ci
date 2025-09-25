using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class CheckOrderCancelQuery : IQuery<CheckOrderCancelModel>
    {
        public string P_ORDER_ID { get; set; }
        public string P_ID_CARD { get; set; }
    }
}
