using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class GetPaymentLogDataSuperDuperQuery : IQuery<GetPaymentLogDataSuperDuperModel>
    {
        public string Url { get; set; }
        public string product_name { get; set; }
        public string service_name { get; set; }
        public string non_mobile_no { get; set; }
        public string order_id { get; set; }
        public string order_transaction_id { get; set; }
        public string transaction_id { get; set; }
    }
}
