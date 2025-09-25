using System.Collections.Generic;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries.AirWirelessCoverage
{
    public class GetAWPackageGroupQuery : IQuery<List<PackageGroup>>
    {
        public string ProductType { get; set; }
    }
}
