namespace WBBContract.Commands
{
    public class MailLogCommand
    {
        public string CustomerId { get; set; }
        public decimal RunningNo { get; set; }

        public decimal ReturnCode { get; set; }
        public string ReturnDesc { get; set; }
    }
}
