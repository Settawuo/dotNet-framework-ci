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
    public class GetAWRegionQueryHandler : IQueryHandler<GetAWRegionQuery, List<RegionTable>>
    {
        private readonly ILogger _logger;
        private readonly IAirNetEntityRepository<AIR_PACKAGE_LOCATION> _airpackagelocation;

        public GetAWRegionQueryHandler(ILogger logger, IAirNetEntityRepository<AIR_PACKAGE_LOCATION> airpackagelocation)
        {
            _logger = logger;
            _airpackagelocation = airpackagelocation;
        }

        public List<RegionTable> Handle(GetAWRegionQuery query)
        {

            List<RegionTable> result = new List<RegionTable>();
            try
            {
                var ttp = (from c in _airpackagelocation.Get()
                           where c.PACKAGE_CODE == query.PackageCode
                           && c.ADDRESS_TYPE == null
                           select new RegionTable()
                           {
                               RegionTableName = c.REGION,
                               EffectiveDtm = null,
                               ExpireDtm = null
                           }).Distinct().OrderBy(t => t.RegionTableName);

                result = ttp.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(" GetAWRegionQueryHandler " + ex.Message);
            }

            return result;
        }
    }
}
