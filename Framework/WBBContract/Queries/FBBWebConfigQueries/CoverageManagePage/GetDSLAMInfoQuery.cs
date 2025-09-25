using System.Collections.Generic;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries.CoverageManagePage
{
    public class GetDSLAMInfoQuery : IQuery<List<DslamPanel>>
    {
        public decimal CVRID { get; set; }

    }
}
