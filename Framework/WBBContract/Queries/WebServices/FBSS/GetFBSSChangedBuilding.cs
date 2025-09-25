using System.Collections.Generic;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices.FBSS
{
    public class GetFBSSChangedBuilding : IQuery<List<FBSSChangedAddressInfo>>
    {
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }
}
