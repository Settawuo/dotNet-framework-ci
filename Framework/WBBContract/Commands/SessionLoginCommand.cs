namespace WBBContract.Commands
{
    public class SessionLoginCommand
    {
        public string CustInternetNum { get; set; }
        public string SessionId { get; set; }
        public int ReturnCode { get; set; }
        public string ReturnDesc { get; set; }
    }
}
