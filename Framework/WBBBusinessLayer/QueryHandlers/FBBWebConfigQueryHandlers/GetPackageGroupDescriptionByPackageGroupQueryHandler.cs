using System.Collections.Generic;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBData.Repository;
using WBBEntity.Models;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers
{
    public class GetPackageGroupDescriptionByPackageGroupQueryHandler : IQueryHandler<GetPackageGroupDescriptionByPackageGroupQuery, List<PackageGroupDesc>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_CFG_LOV> _fbbConfigLov;

        public GetPackageGroupDescriptionByPackageGroupQueryHandler(ILogger logger, IEntityRepository<FBB_CFG_LOV> fbbConfigLov)
        {
            _logger = logger;
            _fbbConfigLov = fbbConfigLov;
        }

        public List<PackageGroupDesc> Handle(GetPackageGroupDescriptionByPackageGroupQuery query)
        {
            List<PackageGroupDesc> result;
            var ttp = (from c in _fbbConfigLov.Get()
                       where c.LOV_NAME == "DESCRIPTION_BY_PACKAGE_GROUP" && c.DISPLAY_VAL == query.PackageGroupName
                       select new PackageGroupDesc()
                       {
                           PackageGroupName = c.DISPLAY_VAL,
                           PackageGroupDescriptionThai = c.LOV_VAL1,
                           PackageGroupDescriptionEng = c.LOV_VAL2

                       });
            result = ttp.ToList();

            return result;
        }
    }
}
