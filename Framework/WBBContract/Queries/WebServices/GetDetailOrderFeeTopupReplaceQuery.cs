using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class GetDetailOrderFeeTopupReplaceQuery : IQuery<DetailOrderFeeTopupReplaceModel>
    {
        public string TransactionId { get; set; }
        public string FullUrl { get; set; }
        public string P_FLAG_LANG { get; set; }
    }
}
