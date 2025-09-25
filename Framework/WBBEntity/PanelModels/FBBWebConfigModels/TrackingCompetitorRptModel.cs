namespace WBBEntity.PanelModels.FBBWebConfigModels
{
    public class TrackingCompetitorRptModel
    {
        public string DateFrom { get; set; }
        public string DateTo { get; set; }

        public string OrderStatus { get; set; }
    }

    public class TrackingCompetitorRptList
    {
        public string LOCATION_CODE { get; set; }
        public string ASC_CODE { get; set; }
        public string EMPLOYEE_ID { get; set; }
        public string FBB_ID { get; set; }
        public string PACKAGE { get; set; }
        public string ISP { get; set; }
        public string REGISTERDATE { get; set; }
        public string ACTIVEDATE { get; set; }

    }
}
