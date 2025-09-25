using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class CreateOrderNewRegisterWTTxQuery : IQuery<CreateOrderNewRegisterWTTxModel>
    {
        public string FullUrl { get; set; }
        public string OrderId { get; set; }
        public string txn_id { get; set; }
        public string Channel { get; set; }
        public string IdCardNo { get; set; }
        public string payment_method { get; set; }
        public string payment_transaction_date { get; set; }
        public string address_id { get; set; }
        public string access_mode { get; set; }
    }
}
