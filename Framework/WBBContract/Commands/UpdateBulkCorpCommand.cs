namespace WBBContract.Commands
{
    public class UpdateBulkCorpCommand
    {
        public string P_USER { get; set; }
        public string P_BULK_NO { get; set; }
        public string P_ORDER_NO { get; set; }
        public string P_STATUS_TYPE { get; set; }
        public string P_STATUS { get; set; }
        public string P_MSG_ERROR { get; set; }

        public string P_RETURN_CODE { get; set; }
        public string P_RETURN_MESSAGE { get; set; }
    }
}
