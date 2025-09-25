namespace WBBContract.Commands
{
    public class GenPasswordCommand
    {
        public GenPasswordCommand()
        {
            this.ret_code = -1;
            this.ret_msg = "";
        }
        public string p_prepaid_non_mobile { get; set; }
        public string p_service_status { get; set; }
        public string p_updated_by { get; set; }
        public string p_Address { get; set; }
        public string p_TimeSlot { get; set; }
        public int ret_code { get; set; }
        public string ret_msg { get; set; }
        public string PasswordEnc { get; set; }
        public string PasswordDec { get; set; }
        public string Genpassword { get; set; }
        public string IA_NO { get; set; }
        public string ProductType { get; set; }
        public string CustID { get; set; }
        public string UserName { get; set; }
        public string IANO { get; set; }

    }
}
