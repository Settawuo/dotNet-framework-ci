namespace WBBContract.Commands
{
    public class SendSmsCommand
    {
        public string Source_Addr { get; set; }
        public string Destination_Addr { get; set; }
        public string Message_Text { get; set; }

        public string return_status { get; set; }

        // Update 17.2
        public string Transaction_Id { get; set; }

        // Update 17.5
        public string FullUrl { get; set; }
    }

    public class MeshSmsCommand
    {

    }
}
