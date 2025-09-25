namespace WBBEntity.PanelModels.WebServiceModels
{
    public class FBSSCheckCPEModel
    {
        public string Check_Result_Code { get; set; }
        public string Check_Result_Desc { get; set; }
        public string SN { get; set; }
        public string Status_ID { get; set; }
        public string STATUS_DESC { get; set; }
        public string CPE_MAC_ADDR { get; set; }
        public string CPE_TYPE { get; set; }
        public string CPE_COMPANY_CODE { get; set; }
        public string CPE_PLANT { get; set; }
        public string CPE_STORAGE_LOCATION { get; set; }
        public string CPE_MATERIAL_CODE { get; set; }
        public string SN_PATTERN { get; set; }

        //R20.4
        public string CPE_MODEL_NAME { get; set; }
        public string REGISTER_DATE { get; set; }
        public string FIBRENET_ID { get; set; }
        public string SHIP_TO { get; set; }
        public string WARRANTY_START_DATE { get; set; }
        public string WARRANTY_END_DATE { get; set; }
        public string MAC_ADDRESS { get; set; }
    }
}
