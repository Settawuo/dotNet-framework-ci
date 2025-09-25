namespace WBBEntity.PanelModels.WebServiceModels
{
    public class GetPaymentRabbitToSuperDuperModel
    {
        public string status { get; set; }
        public string status_code { get; set; }
        public string message { get; set; }
        public string message_code { get; set; }
        public ResponseDataModel data { get; set; }
    }

    public class ResponseDataModel
    {
        public string web_redirect { get; set; }
        public string app_redirect { get; set; }
        public string transaction_id { get; set; }
        public string payment_access_token { get; set; }
    }
}
