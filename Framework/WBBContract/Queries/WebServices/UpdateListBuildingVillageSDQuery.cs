using System.Collections.Generic;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class UpdateListBuildingVillageSDQuery : IQuery<UpdateListBuildingVillageSDModel>
    {
        //R23
        public string UpdateBy { get; set; }
        public List<ListBuildingVillageSD> buildingListSD { get; set; }
    }
}
