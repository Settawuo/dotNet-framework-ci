namespace WBBContract.Commands.FBBWebConfigCommands
{

    public class ConfigutationTypeCommand
    {
        public string p_name { get; set; }
        public string p_type { get; set; }
        public string p_username { get; set; }
        public string ret_code { get; set; }
        public string ret_msg { get; set; }

    }

    public class ConfigurationInstallCostCommand
    {
        public string P_COMMAND { get; set; }
        public string P_TABLE { get; set; }
        public string P_RULE_ID { get; set; }
        public string P_RULE_NAME { get; set; }
        public string P_ORDER_TYPE { get; set; }
        public string P_SUBCONTRACT_TYPE { get; set; }
        public string P_SUBCONTRACT_SUB_TYPE { get; set; }
        public string P_VENDOR_CODE { get; set; }
        public string P_TECHNOLOGY { get; set; }
        public string P_TOTAL_PRICE { get; set; }
        public string P_EVENT_CODE { get; set; }
        public string P_ROOM_FLAG { get; set; }
        public string P_REUSE_FLAG { get; set; }
        public string P_DISTANCE_FROM { get; set; }
        public string P_DISTANCE_TO { get; set; }
        public string P_INDOOR_PRICE { get; set; }
        public string P_OUTDOOR_PRICE { get; set; }
        public string P_INTERNET_PRICE { get; set; }
        public string P_VOIP_PRICE { get; set; }
        public string P_PLAYBOX_PRICE { get; set; }
        public string P_MECH_PRICE { get; set; }
        public string P_ADDRESS_ID { get; set; }
        public string P_EVENT_TYPE { get; set; }
        public string P_EFFECTIVE_DATE { get; set; }
        public string P_EXPIRE_DATE { get; set; }
        public string P_SAME_DAY { get; set; }
        public string P_USERNAME { get; set; }
        public string P_COMPANY_NAME { get; set; }
        public string P_SUB_LOCATION { get; set; }
        public string ret_code { get; set; }
        public string ret_msg { get; set; }


    }

}
