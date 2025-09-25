namespace WBBContract.Commands
{
    public class UpdateFileNameCommand
    {
        public string OrderNo { get; set; }
        public string FileName { get; set; }

        public int WBB_returnCode { get; set; }
        public string WBB_returnMsg { get; set; }
        public int AIRadmin_returnCode { get; set; }
        public string AIRadmin_returnMsg { get; set; }

        // Update 17.2
        public string Transaction_Id { get; set; }

        // Update 17.5
        public string FullUrl { get; set; }
    }
}
