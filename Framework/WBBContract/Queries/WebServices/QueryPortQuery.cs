using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class QueryPortQuery : IQuery<QueryPortModel>
    {
        public string RESOURCE_NO { get; set; }
        public string RESOURCE_TYPE { get; set; }
        public string SERVICE_STATE { get; set; }
        public string TRANSACTION_ID { get; set; }
        public string FullUrl { get; set; }
    }
}
