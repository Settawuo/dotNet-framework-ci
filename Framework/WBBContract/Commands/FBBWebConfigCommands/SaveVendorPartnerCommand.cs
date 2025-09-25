namespace WBBContract.Commands.FBBWebConfigCommands
{
    public class SaveVendorPartnerCommand
    {
        public string vendor_partner { get; set; }
        public string user { get; set; }
        public string return_msg { get; set; }
        public decimal return_code { get; set; }
    }
}
