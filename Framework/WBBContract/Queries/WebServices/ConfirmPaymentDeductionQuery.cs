using WBBEntity.PanelModels.WebServices;

namespace WBBContract.Queries.WebServices
{
    public class ConfirmPaymentDeductionQuery : IQuery<ConfirmPaymentDeductionModel>
    {
        public string FullUrl { get; set; }
        public string txn_id { get; set; }
        public string non_mobile_no { get; set; }
        public string order_transaction_id { get; set; }
        public string update_by { get; set; }
    }
}