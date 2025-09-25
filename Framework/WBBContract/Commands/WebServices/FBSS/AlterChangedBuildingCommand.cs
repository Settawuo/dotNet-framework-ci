using System.Collections.Generic;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Commands.WebServices.FBSS
{
    public class AlterChangedBuildingCommand : CommandBase
    {
        public List<FBSSChangedAddressInfo> FBSSChangedAddressInfos { get; set; }
        public string result { get; set; }
        public string errormsg { get; set; }
        public string numRecInsert { get; set; }
        public string numRecUpdate { get; set; }
        public string numRecDelete { get; set; }
    }
}
