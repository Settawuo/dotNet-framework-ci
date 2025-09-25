namespace WBBContract.Commands.FBBWebConfigCommands
{
    public class UpdateFOAResendStatusCommand
    {
        public string P_TRANS_ID { get; set; }
        public string P_ORDER_NO { get; set; }
        public string P_SERIAL_NO { get; set; }
        public string P_INTERNET_NO { get; set; }
        public string P_SUBNUMBER { get; set; }
        public string P_ASSET_CODE { get; set; }
        public string P_MATERIAL_DOC { get; set; }
        public string P_DOC_YEAR { get; set; }
        public string P_COM_CODE { get; set; }
        public string P_ERR_CODE { get; set; }
        public string P_ERR_MSG { get; set; }
        public string P_REMARK { get; set; }

        public string ret_code { get; set; }
        public string ret_msg { get; set; }
    }
}
