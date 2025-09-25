namespace WBBContract.Commands
{
    public class UpdateFilePathByOrderNoCommand
    {
        public string P_Order_No { get; set; }
        public string P_File_Path { get; set; }

        public string RES_CODE { get; set; }
        public string RES_MESSAGE { get; set; }
    }
}
