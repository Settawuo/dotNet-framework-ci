namespace WBBEntity.PanelModels.WebServiceModels
{
    public class GetPrivilegeBarcodeModel
    {
        public string TransactionID { get; set; }
        public string HttpStatus { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public string Msg { get; set; }
        public string RegID { get; set; }
        public string MsgBarcode { get; set; }
        public string BarcodeType { get; set; }
        public string Ssid { get; set; }
    }
}
