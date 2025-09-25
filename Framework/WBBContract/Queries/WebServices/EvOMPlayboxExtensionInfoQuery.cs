using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class EvOmPlayboxExtensionInfoQuery : IQuery<EvOmPlayboxExtensionInfoModel>
    {
        public string FbbId { get; set; }
        public string TransactionId { get; set; }
    }
}
