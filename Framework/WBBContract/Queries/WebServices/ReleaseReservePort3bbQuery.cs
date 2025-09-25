using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class ReleaseReservePort3bbQuery : IQuery<ReleaseReservePort3bbQueryModel>
    {
        public string referenceId { get; set; }
        public string transactionId { get; set; }
    }
}
