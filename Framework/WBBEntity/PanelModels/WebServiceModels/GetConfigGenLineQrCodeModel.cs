namespace WBBEntity.PanelModels.WebServiceModels
{
    public class GetConfigGenLineQrCodeModel
    {
        public string payload_ch { get; set; }
        public string payload_linetempid { get; set; }
        public string verify_signature { get; set; }
        public string url { get; set; }
        public string ret_code { get; set; }
        public string ret_msg { get; set; }
    }
}
