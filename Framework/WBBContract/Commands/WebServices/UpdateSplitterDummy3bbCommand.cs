namespace WBBContract.Commands.WebServices
{
    public class UpdateSplitterDummy3bbCommand
    {
        // input
        public string Transaction_Id { get; set; } = "";
        public string p_address_id { get; set; }
        public string p_splitter_no { get; set; }
        // output
        public string ReturnCode { get; set; } = "-1";
        public string ReturnMsg { get; set; } = "";
    }
}
