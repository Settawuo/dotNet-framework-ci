namespace WBBEntity.Models
{
    public partial class FBB_SENDMAIL_LOG
    {
        public string CUST_ROW_ID { get; set; }
        public string PROCESS_NAME { get; set; }
        public string RETURN_CODE { get; set; }
        public string RETURN_DESC { get; set; }
        public string CREATE_USER { get; set; }
        public System.DateTime CREATE_DATE { get; set; }
        public decimal RUNNING_NO { get; set; }
        public string FILE_NAME { get; set; }
    }
}
