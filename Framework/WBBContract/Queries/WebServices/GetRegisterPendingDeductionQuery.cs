using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class GetRegisterPendingDeductionQuery : IQuery<GetRegisterPendingDeductionModel>
    {
        public string Url { get; set; }
        public string transaction_id { get; set; }
    }
}