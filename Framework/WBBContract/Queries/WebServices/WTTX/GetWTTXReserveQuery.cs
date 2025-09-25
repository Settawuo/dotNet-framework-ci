using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices.WTTX
{
    public class GetWTTXReserveQuery : IQuery<WTTXReserveModel>
    {
        public string gridId { get; set; }
        public string reservedExpTime { get; set; }
        public string transaction_id { get; set; }
        public string ref_id { get; set; }
    }
}
