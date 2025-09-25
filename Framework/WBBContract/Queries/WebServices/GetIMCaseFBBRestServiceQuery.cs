using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class GetIMCaseFBBRestServiceQuery : IQuery<GetIMCaseFBBRestServiceModel>
    {
        public string FullUrl { get; set; }
        public string TransactionId { get; set; }

        public DetailCalliMTopupReplace Body { get; set; }
        public string BodyJson { get; set; }
        public string Request_Url { get; set; }
    }
}
