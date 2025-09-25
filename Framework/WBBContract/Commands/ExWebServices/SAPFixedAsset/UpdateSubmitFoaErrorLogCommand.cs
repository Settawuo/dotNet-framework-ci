namespace WBBContract.Commands.ExWebServices.SAPFixedAsset
{
    public class UpdateSubmitFoaErrorLogCommand
    {
        public string order_no { get; set; }
        public string order_type { get; set; }
        public string reject_reason { get; set; }
        public string serial_no { get; set; }
        public string access_number { get; set; }
        public string resend_status { get; set; }
        public string updated_by { get; set; }
        public string updated_desc { get; set; }
        public string ret_code { get; set; }
        public string ret_msg { get; set; }

    }
}
