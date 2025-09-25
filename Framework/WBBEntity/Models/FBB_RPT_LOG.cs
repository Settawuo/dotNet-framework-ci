namespace WBBEntity.Models
{
    public partial class FBB_RPT_LOG
    {
        public decimal REPORT_ID { get; set; }
        public string REPORT_CODE { get; set; }
        public string REPORT_NAME { get; set; }
        public string REPORT_CONDITION { get; set; }
        public string REPORT_DESC { get; set; }
        public string CREATED_BY { get; set; }
        public System.DateTime? CREATED_DATE { get; set; }
        public string UPDATED_BY { get; set; }
        public System.DateTime? UPDATED_DATE { get; set; }
        public string FILE_PATH { get; set; }
        public string REPORT_STATUS { get; set; }
        public string REPORT_STATUS_DESC { get; set; }
        public string REPORT_PARAMETER { get; set; }
        public string FILE_NAME { get; set; }

    }
}
