namespace WBBContract.Commands
{
    public class UpdatePreregisterCommand
    {
        public string p_refference_no { get; set; }
        public string p_is_contact_cust { get; set; }
        public string p_remark_for_contact_cust { get; set; }
        public string p_is_in_cov { get; set; }
        public string p_remark_for_no_cov { get; set; }
        public string p_closing_sale { get; set; }
        public string p_remark_for_no_reg { get; set; }
        public string p_user_name { get; set; }
        public string p_compleated_flag { get; set; }
        //R20.12
        //public string p_house_no { get; set; }
        //public string p_village_name { get; set; }
        //public string p_soi { get; set; }
        //public string p_road { get; set; }

        public decimal return_code { get; set; }
        public string return_message { get; set; }

    }
}
