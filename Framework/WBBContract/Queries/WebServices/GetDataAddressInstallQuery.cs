using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class GetDataAddressInstallQuery : IQuery<GetDataAddressInstallModel>
    {
        public string p_fibrenet_id { get; set; }
        public string transaction_Id { get; set; }
        public string fullUrl { get; set; }
    }
}
