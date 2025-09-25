using System.Collections.Generic;

namespace WBBEntity.PanelModels.ShareplexModels
{
    public class PremiumAreaModel
    {
        public PremiumAreaModel()
        {
            if (ReturnPremiumConfig == null)
                ReturnPremiumConfig = new List<PremiumConfigModel>();
        }

        public string ReturnCode { get; set; }
        public string ReturnMessage { get; set; }
        public List<PremiumConfigModel> ReturnPremiumConfig { get; set; }
    }

    public class PremiumConfigModel
    {
        public string Region { get; set; }
        public string ProvinceTh { get; set; }
        public string DistrictTh { get; set; }
        public string SubdistrictTh { get; set; }
        public string ProvinceEn { get; set; }
        public string DistrictEn { get; set; }
        public string SubdistrictEn { get; set; }
        public string Postcode { get; set; }
    }
}
