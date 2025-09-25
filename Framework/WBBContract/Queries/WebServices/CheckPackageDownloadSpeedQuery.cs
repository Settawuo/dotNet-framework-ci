using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class CheckPackageDownloadSpeedQuery : IQuery<CheckPackageDownloadSpeedModel>
    {
        public string ProductCode { get; set; }
        public string TransactionId { get; set; }
    }
}
