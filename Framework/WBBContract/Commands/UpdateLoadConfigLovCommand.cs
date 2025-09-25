namespace WBBContract.Commands
{
    public class UpdateLoadConfigLovCommand
    {
        public string EVENT_NAME { get; set; }
        public string FLAG_NUMBER { get; set; }
        public string IP { get; set; }

        //return
        public int return_code { get; set; }
        public string return_message { get; set; }
    }
}
