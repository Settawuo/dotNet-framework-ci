namespace WBBEntity.PanelModels.WebServiceModels
{
    public class GetRedirectIDSQueryModel
    {
        public string SERVICE_PROVIDER_NAME { get; set; }
        public string CALLBACK_URL { get; set; }
        public string CLIENT_ID { get; set; }
        public string CLIENT_SECRET { get; set; }
        public string FULL_URL_IDS { get; set; }
    }
}
