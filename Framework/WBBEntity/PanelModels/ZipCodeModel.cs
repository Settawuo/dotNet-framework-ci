namespace WBBEntity.PanelModels
{
    public class ZipCodeModel : PanelModelBase
    {
        public string ZipCodeId { get; set; }
        public bool IsThai { get; set; }
        public string ZipCode { get; set; }
        public string Tumbon { get; set; }
        public string Amphur { get; set; }
        public string Province { get; set; }
        public string RegionCode { get; set; }
        public string lang_flag { get; set; }
    }
}