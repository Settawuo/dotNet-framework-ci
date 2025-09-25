namespace WBBContract.Commands.ExWebServices.FbbCpGw
{
    public class UpdateCoverageRusultCommand
    {
        public string p_order_no { get; set; }
        public string p_channel { get; set; }
        public string p_status_plan { get; set; }
        public string p_user_verify { get; set; }
        public string p_flag_verify { get; set; }
        public string p_date_verify { get; set; }
        public string p_remark { get; set; }
        //Output
        public string o_return_code { get; set; }
        public string o_return_message { get; set; }
    }
}
