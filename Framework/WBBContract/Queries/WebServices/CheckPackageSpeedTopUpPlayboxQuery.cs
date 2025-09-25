using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class CheckPackageSpeedTopUpPlayboxQuery : IQuery<CheckPackageDownloadSpeedModel>
    {
        public string ProductCode { get; set; }
        public string TransactionId { get; set; }
    }
}
