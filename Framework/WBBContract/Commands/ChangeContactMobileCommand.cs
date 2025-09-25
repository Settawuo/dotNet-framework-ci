namespace WBBContract.Commands
{
    public class ChangeContactMobileCommand
    {
        public string OrderNo { get; set; }
        public string NewMobileContact { get; set; }

        public int WBB_returnCode { get; set; }
        public string WBB_returnMsg { get; set; }
        public int AIRadmin_returnCode { get; set; }
        public string AIRadmin_returnMsg { get; set; }
    }
}
