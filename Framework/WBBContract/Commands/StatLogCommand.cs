namespace WBBContract.Commands
{
    public class StatLogCommand
    {
        public string Username { get; set; }
        public string VisitType { get; set; }
        public string SelectPage { get; set; }
        public string REQ_IPADDRESS { get; set; }
        public string HOST { get; set; }
        public string LC { get; set; }

        public decimal ReturnCode { get; set; }
        public string ReturnDesc { get; set; }
    }
}
