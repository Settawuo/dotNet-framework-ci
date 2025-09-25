namespace WBBEntity.PanelModels.WebServiceModels
{
    public class CheckCardByLaserModel
    {
        public string resultCode { get; set; }
        public string developerMessage { get; set; }
        public Results result { get; set; }
    }

    public class Results
    {
        public bool isError { get; set; }
        public string errorDesc { get; set; }
        public DataInfo dataInfo { get; set; }
    }

    public class DataInfo
    {
        public string stCode { get; set; }
        public string stDesc { get; set; }
    }
}
