namespace WBBContract.Commands.ExWebServices.FbbCpGw
{
    public class MicrositeActionCommand
    {
        public string P_ORDER_NO { get; set; }
        public string P_ORDER_CHANNEL { get; set; }
        public string P_IS_CONTACT_CUSTOMER { get; set; }
        public string P_IS_IN_COVERAGE { get; set; }
        public string P_USER_ACTION { get; set; }
        public string P_DATE_ACTION { get; set; }
        public string P_ORDER_PRE_REGISTER { get; set; }
        public string P_STATUS_ORDER { get; set; }
        public string P_REMARK_NOTE { get; set; }
        //Return
        public string p_return_code { get; set; }
        public string p_return_message { get; set; }
    }
}
