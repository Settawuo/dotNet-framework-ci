using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices.WTTX
{
    public class GetWTTXInfoQuery : IQuery<WTTXInfoModel>
    {
        public string grid_id { get; set; }
        public string transaction_id { get; set; }
        public string ref_id { get; set; }
    }
}
