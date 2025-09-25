namespace WBBEntity.PanelModels.WebServiceModels
{
    public class WTTXInfoModel
    {
        public string RESULT_CODE { get; set; }
        public string RESULT_MESSAGE { get; set; }
        public string TRANSACTION_ID { get; set; }

        public string GRIDID { get; set; }
        public string LATITUDE { get; set; }
        public string LONGITUDE { get; set; }
        public string REGION { get; set; }
        public string PROVINCEENG { get; set; }
        public string PROVINCETHA { get; set; }
        public string SCALEXKM { get; set; }
        public string SCALEYKM { get; set; }
        public string ONSERVICE { get; set; }
        public string MAXBANDWITH { get; set; }
        public int MAXCUSTOMER { get; set; }
        public int NUMBEROFCUSTOMER { get; set; }
        public string UTILIZATION { get; set; }
        public string LASTUPDATETIME { get; set; }
    }
}
