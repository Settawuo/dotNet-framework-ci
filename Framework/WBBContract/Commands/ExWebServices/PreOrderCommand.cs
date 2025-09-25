namespace WBBContract.Commands.ExWebServices
{
    public class PreOrderCommand
    {
        public string PreOrderNo { get; set; }
        public string Status { get; set; }

        public int ReturnCode { get; set; }
        public string ReturnMessage { get; set; }
    }
}
