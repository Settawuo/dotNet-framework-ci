namespace WBBEntity.PanelModels.WebServiceModels
{
    public class OrderPendingPaymentModel
    {
        public string endpoint { get; set; }
        public string channel_secret { get; set; }
        public string non_mobile_no { get; set; }
        public string merchant_id { get; set; }
        public string txn_id { get; set; }
        public string product_name { get; set; }
        public string order_id { get; set; }
        public string payment_method { get; set; }
        public string payment_transaction_date { get; set; }
        public string address_id { get; set; }
    }
}
