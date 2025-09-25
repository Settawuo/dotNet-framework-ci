using System.Collections.Generic;
using WBBContract.QueryModels.FBSS;
using WBBEntity.PanelModels;

namespace WBBContract.Queries.FBSS
{
    public class GetBuildingNoQuery : IQuery<List<DropdownModel>>
    {
        public string Postcode { get; set; }
        public string Buildname { get; set; }
        public string Language { get; set; }
    }

    public class GetBuildingNoAllQuery : IQuery<List<DropdownModel>>
    {
        public string Postcode { get; set; }
        public string Buildname { get; set; }
        public string Language { get; set; }
    }

    public class GetPermissionCondoUpdateStatusQuery : IQuery<string>
    {
        public string UserName { get; set; }
    }

    public class GetBuildingByBuildingNameAndNoQuery : IQuery<GetBuildingByBuildingNameAndNoQueryModel>
    {
        public string Buildname { get; set; }
        public string Buildno { get; set; }
    }
}
