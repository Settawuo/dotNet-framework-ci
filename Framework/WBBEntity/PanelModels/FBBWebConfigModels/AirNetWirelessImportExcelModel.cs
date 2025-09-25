using System.Collections.Generic;

namespace WBBEntity.PanelModels.FBBWebConfigModels
{
    public class AirNetWirelessImportExcelModel
    {


        public string UrlPatxtext { get; set; }
        public string Base_L2 { get; set; }
        public string Sitename { get; set; }
        public string Sub_district { get; set; }
        public string District { get; set; }
        public string Province { get; set; }
        public string Lat { get; set; }
        public string Long { get; set; }
        public string AP_Name { get; set; }
        public string sector { get; set; }

        public List<AirNetWirelessImportExcelModel> listAirmodelexecl { get; set; }

    }
}
