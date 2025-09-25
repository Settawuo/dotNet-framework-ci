using System.Collections.Generic;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries.CoverageManagePage
{
    public class GetGridDSLAMRestockQuery : IQuery<List<GridDSLAMRestockModel>>
    {
        public decimal CVRID { get; set; }

    }
}
