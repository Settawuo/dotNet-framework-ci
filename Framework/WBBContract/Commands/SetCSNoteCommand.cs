namespace WBBContract.Commands
{
    public class SetCSNoteCommand
    {
        public SetCSNoteCommand()
        {
            this.out_return_code = "-1";
            this.out_return_error = "";
        }

        public string in_order_no { get; set; }
        public string in_cs_note { get; set; }
        public string in_p_user { get; set; }
        // for return
        public string out_return_code { get; set; }
        public string out_return_error { get; set; }
    }

    public class SetCustomerVerificationCommand
    {
        public SetCustomerVerificationCommand()
        {
            this.return_code = "-1";
            this.return_msg = "";
        }

        public string OrderNo { get; set; }
        public string CustomerPurge { get; set; }
        public string ExceptEntryFee { get; set; }
        public string SecondInstallation { get; set; }
        // for return
        public string return_code { get; set; }
        public string return_msg { get; set; }
    }
}
