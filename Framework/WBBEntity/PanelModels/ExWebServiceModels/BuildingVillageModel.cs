using System.Collections.Generic;

namespace WBBEntity.PanelModels.ExWebServiceModels
{
    public class BuildingVillageModel
    {
        public string BuildingType { get; set; }
        public string BuildingName { get; set; }
        public string Tower { get; set; }
    }

    public class CoverageAreaModel
    {
        public decimal CvrId { get; set; }
        public string SiteCode { get; set; }
        public string NodeNameEn { get; set; }
        public string NodeNameTh { get; set; }
        public string NodeType { get; set; }
        public string NodeStatus { get; set; }

        // node address.
        public string Moo { get; set; }
        public string Soi_Th { get; set; }
        public string Road_Th { get; set; }
        public string Soi_En { get; set; }
        public string Road_En { get; set; }
        public string Zipcode { get; set; }

        public List<CoverageRelValueModel> CoverageRelValueModelList { get; set; }
    }
    //19.6
    public class ListBuildingVillageModel
    {
        public string AddressId { get; set; }
        public string BuildingName { get; set; }
        public string BuildingNo { get; set; }
        public string SubDistrict { get; set; }
        public string District { get; set; }
        public string Province { get; set; }
        public string Postcode { get; set; }
        public string AccessMode { get; set; }
        public string PartnerName { get; set; }
        public string SiteCode { get; set; }
        public string Latitude { get; set; }
        public string Longtitude { get; set; }
    }
}
