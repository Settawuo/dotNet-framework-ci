using System.Collections.Generic;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries.CoverageManagePage
{
    public class GetCoverageAreaBuildingQuery : IQuery<List<BuildingPanel>>
    {
        public decimal ContactId { get; set; }
        public bool? NotIn { get; set; }
    }
}
