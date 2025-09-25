using AIRNETEntity.Models;
using System.Collections.Generic;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries.AirWirelessCoverage;
using WBBData.Repository;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers.AirNetWirelessCoverage
{
    public class GetAWPackageGroupQueryHandler : IQueryHandler<GetAWPackageGroupQuery, List<PackageGroup>>
    {
        private readonly ILogger _logger;
        private readonly IAirNetEntityRepository<AIR_PACKAGE_MASTER_DETAIL> _airpackagemasterdetail;

        public GetAWPackageGroupQueryHandler(ILogger logger, IAirNetEntityRepository<AIR_PACKAGE_MASTER_DETAIL> airpackagemasterdetail)
        {
            _logger = logger;
            _airpackagemasterdetail = airpackagemasterdetail;
        }

        public List<PackageGroup> Handle(GetAWPackageGroupQuery query)
        {
            List<PackageGroup> result;
            var ttp = (from c in _airpackagemasterdetail.Get()
                       where c.PACKAGE_GROUP != null && c.PRODUCT_SUBTYPE == query.ProductType
                       select new PackageGroup()
                       {
                           PACKAGE_GROUP = c.PACKAGE_GROUP

                       }).Distinct().OrderBy(t => t.PACKAGE_GROUP);
            result = ttp.ToList();

            return result;
        }
    }
}
