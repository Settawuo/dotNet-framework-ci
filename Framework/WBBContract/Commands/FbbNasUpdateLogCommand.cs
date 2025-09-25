namespace WBBContract.Commands
{
    public class FbbNasUpdateLogCommand
    {
        public FbbNasUpdateLogCommand()
        {
            this.ret_code = -1;
        }

        public string file_name { get; set; }
        public string file_owner { get; set; }
        public string nas_path { get; set; }
        public string action { get; set; }
        // out
        public decimal ret_code { get; set; }
    }
}
