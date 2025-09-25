using System.Collections.Generic;

namespace WBBEntity.PanelModels.WebServiceModels
{
    public class GetDataAddressInstallModel
    {
        public string ret_code { get; set; }
        public string ret_message { get; set; }
        public List<InstallAddressList> install_curror { get; set; }
    }

    public class InstallAddressList
    {
        public string Address_Name { get; set; }
        public string Address_Install { get; set; }

    }

}
