using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class CheckCoverageMapServiceQuery : IQuery<CheckCoverageMapServiceDataModel>
    {
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string transactionId { get; set; }
        public string FullUrl { get; set; }
    }
}
