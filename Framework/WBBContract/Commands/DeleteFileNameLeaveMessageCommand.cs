namespace WBBContract.Commands
{
    public class DeleteFileNameLeaveMessageCommand
    {
        public string p_file_name { get; set; }
        public string p_username { get; set; }

        public decimal return_code { get; set; }
        public string return_message { get; set; }
    }
}
