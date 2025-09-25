namespace WBBEntity.PanelModels.WebServiceModels
{
    public class FBSSChangedAddressInfo
    {
        public string Language { get; set; }
        public string AddressType { get; set; }
        public string PostalCode { get; set; }
        public string AddressId { get; set; }
        public bool AddressIdSpecified { get; set; }
        public string BuildingName { get; set; }
        public string BuildingNo { get; set; }
        public string ChangedAction { get; set; }
        public string SUBDISTRICT { get; set; }
        public string ACCESS_MODE { get; set; }
        public string SITE_CODE { get; set; }
        public string PARTNER { get; set; }
        public string LATITUDE { get; set; }
        public string LONGTITUDE { get; set; }
        //R21.1
        public string FTTR_FLAG { get; set; }
        public string SPECIFIC_TEAM_1 { get; set; }
        public string SPECIFIC_TEAM_2 { get; set; }
    }
}
