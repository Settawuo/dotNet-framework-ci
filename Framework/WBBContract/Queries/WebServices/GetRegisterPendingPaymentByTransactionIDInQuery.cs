using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class GetRegisterPendingPaymentByTransactionIDInQuery : IQuery<GetRegisterPendingPaymentByTransactionIDInModel>
    {
        public string payment_transaction_id_in { get; set; }
    }
}
