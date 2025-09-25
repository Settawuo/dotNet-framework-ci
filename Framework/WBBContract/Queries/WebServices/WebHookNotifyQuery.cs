using WBBEntity.PanelModels.WebServiceModels;
using WBBEntity.PanelModels.WebServices;

namespace WBBContract.Queries.WebServices
{
    public class WebHookNotifyQuery : IQuery<WebHookNotifyModel>
    {
        public string FullUrl { get; set; }
        public string OrderTransactionId { get; set; }
        public string TransactionId { get; set; }
        public WebhookResponseModel DataResult { get; set; }
    }
}