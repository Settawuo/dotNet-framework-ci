namespace WBBContract.Commands
{
    public class SaveOutgoingMessageCommand
    {
        public string MethodName { get; set; }
        public ActionType Action { get; set; }
        public string SoapXml { get; set; }
        public string OrderRowId { get; set; }
        public string MobileNo { get; set; }
        public string AirOrderNo { get; set; }
        public string TransactionId { get; set; }
    }
}
