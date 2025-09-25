namespace WBBContract.Commands
{
    public class UpdatePreregisterStatusCommand
    {
        public string p_refference_no { get; set; }
        public string p_status { get; set; }
        public string p_user_name { get; set; }


        //return
        public int return_code { get; set; }
        public string return_message { get; set; }
    }
}
