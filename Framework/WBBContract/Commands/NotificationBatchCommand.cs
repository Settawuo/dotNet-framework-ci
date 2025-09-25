namespace WBBContract.Commands
{
    public class NotificationBatchCommand
    {
        public string Cause { get; set; }
        public string result { get; set; }
        public string errormsg { get; set; }
        public string numRecInsert { get; set; }
        public string numRecUpdate { get; set; }
        public string numRecDelete { get; set; }


    }
}
