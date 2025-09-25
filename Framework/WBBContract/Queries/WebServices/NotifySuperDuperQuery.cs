using WBBEntity.PanelModels.WebServiceModels;
using WBBEntity.PanelModels.WebServices;

namespace WBBContract.Queries.WebServices
{
    public class NotifySuperDuperQuery : IQuery<NotifySuperDuperModel>
    {
        public string FullUrl { get; set; }
        public string TransactionId { get; set; }
        public WebhookResponseModel DataResult { get; set; }
    }
}