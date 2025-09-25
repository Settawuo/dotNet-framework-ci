using System.Collections.Generic;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries
{
    public class GetPackageGroupDescriptionByPackageGroupQuery : IQuery<List<PackageGroupDesc>>
    {
        public string PackageGroupName { get; set; }
    }
}
