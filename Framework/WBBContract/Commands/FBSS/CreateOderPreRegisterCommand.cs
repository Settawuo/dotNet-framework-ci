namespace WBBContract.Commands.FBSS
{
    public class CreateOderPreRegisterCommand : CommandBase
    {
        public string TimeSlot { get; set; }
        public string TimeSlots { get; set; }
        public string AppointmentDate { get; set; }
        public string AddressId { get; set; }
        public string Address { get; set; }
        public string PasswordDec { get; set; }
        public string Cus_Name { get; set; }
        public string Cus_Surname { get; set; }
        public string Subcontract { get; set; }
        public string PreNonmobile { get; set; }
        public string Upsubcontractid { get; set; }
        public string Phone { get; set; }
        public string Customername { get; set; }
        public string Cus_ID { get; set; }
        public string IA_NO { get; set; }
        public int Cus_Id { get; set; }
        public string Up_FBBDORM_Order_No { get; set; }
        public string SubcontractID { get; set; }
        public int ret_code { get; set; }
        public string ret_msg { get; set; }
        public string Pre_Total_Results { get; set; }
    }
}
