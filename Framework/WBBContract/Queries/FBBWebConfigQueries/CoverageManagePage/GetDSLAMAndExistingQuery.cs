using System.Collections.Generic;
using WBBEntity.PanelModels;

namespace WBBContract.Queries.FBBWebConfigQueries.CoverageManagePage
{
    public class GetDSLAMAndExistingQuery : IQuery<List<DropdownModel>>
    {
        public decimal CVRID { get; set; }
        public string RegionCode { get; set; }
        public bool Existing { get; set; }
    }
}
