using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class CheckCoverage3bbQuery : IQuery<CheckCoverage3bbQueryModel>
    {
        public string TRANSACTION_ID { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
    }
}
