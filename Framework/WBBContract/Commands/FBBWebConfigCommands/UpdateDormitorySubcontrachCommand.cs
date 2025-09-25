namespace WBBContract.Commands.FBBWebConfigCommands
{
    public class UpdateDormitorySubcontrachCommand
    {

        public string DORMITORY_NAME_TH { get; set; }
        public string SUB_CONTRACT_LOCATION_CODE { get; set; }
        public string SUB_CONTRACT_NAME_TH { get; set; }
        public string SUB_CONTRACT_NAME_EN { get; set; }
        public string PRICE_INSTALL { get; set; }
        public decimal Result { get; set; }
        public string User { get; set; }
    }

    public class UpdatePrepaidNonMobileStatusDataModel
    {
        public string FibrenetID { get; set; }
        public string Status { get; set; }
        public string User { get; set; }
    }

    public class UpdatePrepaidNonMobileStatusCommand
    {
        public string FibrenetID { get; set; }
        public string Status { get; set; }
        public string User { get; set; }

        public decimal Return_Code { get; set; }
        public string Return_Message { get; set; }
    }
}
