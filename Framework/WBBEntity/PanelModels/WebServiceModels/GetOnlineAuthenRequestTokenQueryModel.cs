namespace WBBEntity.PanelModels.WebServiceModels
{
    //R23.05 CheckFraud
    public class GetOnlineAuthenRequestTokenQueryModel
    {
        public ReturnData returnData { get; set; }
        public string returnCode { get; set; }
        public string returnMessage { get; set; }
    }

    public class ReturnData
    {
        public string appToken { get; set; }
        public string validFrom { get; set; }
        public string validTo { get; set; }
    }

}
