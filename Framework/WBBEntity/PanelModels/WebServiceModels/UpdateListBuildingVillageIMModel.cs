using System.Collections.Generic;

namespace WBBEntity.PanelModels.WebServiceModels
{
    public class UpdateListBuildingVillageIMModel
    {
        public string ResultDesc { get; set; }
        public string ResultCode { get; set; }
    }

    public class BuildingVillageIMConfigUrlModel
    {
        public string Url { get; set; }
        public string UseSecurityProtocol { get; set; }
        public string BodyStr { get; set; }
    }

    public class ListBuildingVillageIM
    {
        public string AddressID { get; set; }
        public string ActiveFlag { get; set; }
        public string Remark { get; set; }
    }

    public class BuildingVillageIMConfigBody
    {
        public string UpdateBy { get; set; }
        public List<ListBuildingVillageIM> buildingList { get; set; }
    }
}
