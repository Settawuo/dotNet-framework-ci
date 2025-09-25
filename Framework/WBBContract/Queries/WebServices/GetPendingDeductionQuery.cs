using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class GetPendingDeductionQuery : IQuery<GetPendingDeductionModel>
    {
        public string p_transaction_id { get; set; }
        public string p_mobile_no { get; set; }
    }
}
