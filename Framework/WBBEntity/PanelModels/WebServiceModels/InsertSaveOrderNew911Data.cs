namespace WBBEntity.PanelModels.WebServiceModels
{
    class InsertSaveOrderNew911Data
    {
    }

    public class AirRegistPackage
    {
        public string temp_ia { get; set; }
        public string product_subtype { get; set; }
        public string package_type { get; set; }
        public string package_code { get; set; }
        public decimal package_price { get; set; }
        public string idd_flag { get; set; }
        public string fax_flag { get; set; }
        public string home_ip { get; set; }
        public string home_port { get; set; }
        public string mobile_forward { get; set; }
        public string pbox_ext { get; set; }
    }

    public class AirRegistFile
    {
        public string file_name { get; set; }
    }

    public class AirRegistSplitter
    {
        public string SPLITTER_NAME { get; set; }
        public decimal DISTANCE { get; set; }
        public string DISTANCE_TYPE { get; set; }
        public string RESOURCE_TYPE { get; set; }
    }

    public class AirRegistCPESerial
    {
        public string CPE_TYPE { get; set; }
        public string SERIAL_NO { get; set; }
        public string MAC_ADDRESS { get; set; }
        public string STATUS_DESC { get; set; }
        public string MODEL_NAME { get; set; }
        public string COMPANY_CODE { get; set; }
        public string CPE_PLANT { get; set; }
        public string STORAGE_LOCATION { get; set; }
        public string MATERIAL_CODE { get; set; }
        public string REGISTER_DATE { get; set; }
        public string FIBRENET_ID { get; set; }
        public string SN_PATTERN { get; set; }
        public string SHIP_TO { get; set; }
        public string WARRANTY_START_DATE { get; set; }
        public string WARRANTY_END_DATE { get; set; }
    }

    public class AirRegistCustInsi
    {
        public string GROUP_ID { get; set; }
        public string GROUP_NAME_TH { get; set; }
        public string GROUP_NAME_EN { get; set; }
        public string QUESTION_ID { get; set; }
        public string QUESTION_TH { get; set; }
        public string QUESTION_EN { get; set; }
        public string ANSWER_ID { get; set; }
        public string ANSWER_TH { get; set; }
        public string ANSWER_EN { get; set; }
        public string ANSWER_VALUE_TH { get; set; }
        public string ANSWER_VALUE_EN { get; set; }
        public string PARENT_ANSWER_ID { get; set; }
        public string ACTION_WFM { get; set; }
        public string ACTION_FOA { get; set; }
    }

    public class AirRegistDcontract
    {
        public string PRODUCT_SUBTYPE { get; set; }
        public string PBOX_EXT { get; set; }
        public string TDM_CONTRACT_ID { get; set; }
        public string TDM_RULE_ID { get; set; }
        public string TDM_PENALTY_ID { get; set; }
        public string TDM_PENALTY_GROUP_ID { get; set; }
        public string DURATION { get; set; }
        public string CONTRACT_FLAG { get; set; }
        public string DEVICE_COUNT { get; set; }
    }
}
