using System.Collections.Generic;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries.CoverageManagePage
{

    public class GetCradPortdata : IQuery<List<CoveragePortPanelGrid>>
    {
        public decimal CVRID { get; set; }

    }
}
