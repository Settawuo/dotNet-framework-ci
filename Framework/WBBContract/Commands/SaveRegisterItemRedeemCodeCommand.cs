namespace WBBContract.Commands
{
    public class SaveRegisterItemRedeemCodeCommand
    {
        public string p_non_mobile_no { get; set; }
        public string p_id_card_no { get; set; }
        public string p_contact_mobile_no { get; set; }
        public string p_game_name { get; set; }
        public string p_redeem_code_1 { get; set; }
        public string p_redeem_code_2 { get; set; }
        public string p_redeem_code_3 { get; set; }
        public string p_redeem_code_4 { get; set; }
        public string p_redeem_code_5 { get; set; }
        public string p_redeem_status { get; set; }

        public string ClientIP { get; set; }
        public string FullUrl { get; set; }

        public string ret_code { get; set; }
        public string ret_message { get; set; }

    }
}
