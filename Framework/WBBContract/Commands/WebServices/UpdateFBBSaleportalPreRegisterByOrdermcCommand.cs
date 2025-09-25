namespace WBBContract.Commands.WebServices
{
    public class UpdateFBBSaleportalPreRegisterByOrdermcCommand
    {
        // input
        public string RefferenceNo { get; set; }
        public string OrderMC { get; set; }
        // output
        public string ReturnCode { get; set; }
        public string ReturnMessage { get; set; }
    }
}
