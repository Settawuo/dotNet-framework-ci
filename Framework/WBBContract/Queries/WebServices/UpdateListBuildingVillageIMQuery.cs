using System.Collections.Generic;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class UpdateListBuildingVillageIMQuery : IQuery<UpdateListBuildingVillageIMModel>
    {
        //R23
        public string UpdateBy { get; set; }
        public List<ListBuildingVillageIM> buildingListIM { get; set; }
    }
}
