using AIRNETEntity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries.AirWirelessCoverage;
using WBBData.Repository;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers.AirNetWirelessCoverage
{
    public class GetAWPackageUserGroupQueryHandler : IQueryHandler<GetAWPackageUserGroupQuery, List<PackageUserGroup>>
    {
        private readonly ILogger _logger;
        private readonly IAirNetEntityRepository<AIR_PACKAGE_USER_GROUP> _airpackageusergroup;

        public GetAWPackageUserGroupQueryHandler(ILogger logger, IAirNetEntityRepository<AIR_PACKAGE_USER_GROUP> airpackageusergroup)
        {
            _logger = logger;
            _airpackageusergroup = airpackageusergroup;
        }

        public List<PackageUserGroup> Handle(GetAWPackageUserGroupQuery query)
        {
            List<PackageUserGroup> result;
            var ttp1 = (from c in _airpackageusergroup.Get()
                        where c.EFFECTIVE_DTM <= DateTime.Now
                        && c.PACKAGE_CODE == query.PackageCode
                        select new
                        {
                            c.USER_GROUP,
                            c.EXPIRE_DTM
                        }).ToList();
            List<PackageUserGroup> ListPackageUserGroup = new List<PackageUserGroup>();
            foreach (var item in ttp1)
            {
                if (item.EXPIRE_DTM == null)
                {
                    PackageUserGroup packageUserGroup = new PackageUserGroup();
                    packageUserGroup.CatalogAndAuthorizeName = item.USER_GROUP;
                    ListPackageUserGroup.Add(packageUserGroup);
                }
                else if (item.EXPIRE_DTM >= DateTime.Now)
                {
                    PackageUserGroup packageUserGroup = new PackageUserGroup();
                    packageUserGroup.CatalogAndAuthorizeName = item.USER_GROUP;
                    ListPackageUserGroup.Add(packageUserGroup);
                }
            }
            result = ListPackageUserGroup.Distinct().OrderBy(t => t.CatalogAndAuthorizeName).ToList();
            return result;
        }
    }
}
