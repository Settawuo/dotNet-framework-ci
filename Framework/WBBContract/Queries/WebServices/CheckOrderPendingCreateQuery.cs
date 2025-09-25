using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class CheckOrderPendingCreateQuery : IQuery<CheckOrderPendingCreateModel>
    {
        public string OrderId { get; set; }
        public string UpdateBy { get; set; }
        public string InternetNo { get; set; }
    }
}
