using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class GetListQueryConfigContractQuery : IQuery<GetListQueryConfigContractModel>
    {
        public string FullUrl { get; set; }
        public string Request_Url { get; set; }
        public string TransactionId { get; set; }
        public string BodyJson { get; set; }

        public string contract_id { get; set; }
        public string contract_name { get; set; }
    }
}
