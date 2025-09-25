namespace WBBContract.Commands
{
    public class InsertLogDataPrivacyCommand
    {
        public string Transaction_Id { get; set; }
        public string FullUrl { get; set; }
        public string P_CHANNEL { get; set; }
        public string P_FIBRENET_ID { get; set; }
        public string P_MOBILE_NO { get; set; }
        public string P_CONFIRM_MKT { get; set; }
        public string P_CONFIRM_PRIVILEGE { get; set; }
        public string RETURN_CODE { get; set; }
        public string RETURN_MESSAGE { get; set; }

    }
}
