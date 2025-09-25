using System.Collections.Generic;

namespace WBBEntity.PanelModels.WebServiceModels
{
    public class UpdateListBuildingVillageSDModel
    {
        public string resultCode { get; set; }
        public string resultDescription { get; set; }
        public string developerMessage { get; set; }
        public SDListResponse data { get; set; }
    }

    public class BuildingVillageSDConfigUrlModel
    {
        public string Url { get; set; }
        public string UseSecurityProtocol { get; set; }
        public string BodyStr { get; set; }
        public string Channel { get; set; }
        public string ContentType { get; set; }
        public string Authorization { get; set; }
    }

    public class ListBuildingVillageSD
    {
        public string addressId { get; set; }
        public string activeFlag { get; set; }
        public string reason { get; set; }
    }

    public class BuildingVillageSDConfigBody
    {
        public string channel { get; set; }
        public List<ListBuildingVillageSD> buildingList { get; set; }
    }

    public class SDListResponse
    {
        public string isSuccess { get; set; }
        public string code { get; set; }
        public string exception { get; set; }
        public string fieldErrors { get; set; }
        public string errorDetail { get; set; }
    }
}
