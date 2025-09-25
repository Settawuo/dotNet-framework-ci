using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class GetSendSMSDeductionPaymentQuery : IQuery<GetSendSMSDeductionPaymentModel>
    {
        public string p_transaction_id { get; set; }
        public string p_mobile_no { get; set; }
    }
}
