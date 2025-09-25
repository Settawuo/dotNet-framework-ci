using System.Collections.Generic;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries.CoverageManagePage
{
    public class SelectCoverageStatusQuery : IQuery<List<LovModel>>
    {
        public string LOV_TYPE { get; set; }
        public string Status { get; set; }
    }
}
