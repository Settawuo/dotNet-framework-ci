using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class QueryDBDProfileQuery : IQuery<QueryDBDProfileModel>
    {
        public string TAX_ID { get; set; }
    }
}
