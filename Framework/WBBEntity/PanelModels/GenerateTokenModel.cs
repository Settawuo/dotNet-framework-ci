namespace WBBEntity.PanelModels.WebServiceModels
{
    public class GenerateTokenModel
    {
        public string ret_code { get; set; }
        public string ret_mes { get; set; }
        public object resultData { get; set; }
        public bool IsSuccessStatusCode { get; set; }
    }

}
