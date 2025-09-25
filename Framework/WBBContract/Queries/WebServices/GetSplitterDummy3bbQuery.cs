using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class GetSplitterDummy3bbQuery : IQuery<GetSplitterDummy3bbQueryModel>
    {
        public string TransactionId { get; set; } = "";
        public string p_address_id { get; set; }
    }
}
