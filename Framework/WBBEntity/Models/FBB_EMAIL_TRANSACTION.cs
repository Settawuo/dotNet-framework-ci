namespace WBBEntity.Models
{
    public partial class FBB_EMAIL_TRANSACTION
    {
        public string PROCESS_NAME { get; set; }
        public string EMAIL_TO { get; set; }
        public string EMAIL_CONTENT { get; set; }
        public string EMAIL_ATTACH { get; set; }
        public string STATUS { get; set; }
        public string STATUS_DESC { get; set; }
        public string CREATE_BY { get; set; }
        public System.DateTime CREATE_DATE { get; set; }

    }
}
