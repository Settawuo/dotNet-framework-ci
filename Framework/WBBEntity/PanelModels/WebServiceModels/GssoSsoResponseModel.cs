namespace WBBEntity.PanelModels.WebServiceModels
{
    public class GssoSsoResponseModel
    {
        public string code { get; set; }
        public string description { get; set; }
        public bool isSuccess { get; set; }
        public string operName { get; set; }
        public string orderRef { get; set; }
        public string pwd { get; set; }
        public string transactionID { get; set; }
    }
}
