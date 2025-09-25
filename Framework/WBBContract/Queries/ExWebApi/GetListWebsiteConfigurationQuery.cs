using WBBEntity.PanelModels.ExWebApiModel;

namespace WBBContract.Queries.ExWebApi
{
    public class GetListWebsiteConfigurationQuery : IQuery<GetListWebsiteConfigurationQueryModel>
    {
        public string TransactionId { set; get; }
        public string ColumnName { set; get; }
        public string LovType { set; get; }
        public string LovName { set; get; }
        public string PackageCode { set; get; }
    }
}
