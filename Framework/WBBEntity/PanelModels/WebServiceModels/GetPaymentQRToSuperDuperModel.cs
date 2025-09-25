namespace WBBEntity.PanelModels.WebServiceModels
{
    public class GetPaymentQRToSuperDuperModel
    {
        public string order_id { get; set; }
        public string txn_id { get; set; }
        public string qr_code { get; set; }
        public string amount { get; set; }
        public string amount_net { get; set; }
        public string amount_cust_fee { get; set; }

        //public string status_code { get; set; }
        public string status { get; set; }
        //public string status_message { get; set; }
        public string message { get; set; }
        public string message_remark { get; set; }
    }
}
