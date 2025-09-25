namespace WBBContract.Commands.FBBWebConfigCommands
{
    public class UpdateSubContractEmailInfoCommand
    {
        public string p_row_id { get; set; }
        public string p_subcontrac_email { get; set; }
        public string p_phase { get; set; }
        public string p_subcontract_for_email { get; set; }
        //
        public string ret_code { get; set; }
        public string ret_msg { get; set; }
    }
}
