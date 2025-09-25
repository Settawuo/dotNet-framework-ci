using System;
using System.Collections.Generic;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries.AirWirelessCoverage;
using WBBData.Repository;
using WBBEntity.Models;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers.AirNetWirelessCoverage
{
    public class GetAWCinformationQueryHandler : IQueryHandler<GetAWCinformationQuery, List<AWCinformation>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_APCOVERAGE> _coverage;

        public GetAWCinformationQueryHandler(ILogger logger, IEntityRepository<FBB_APCOVERAGE> coverage)
        {
            _logger = logger;
            _coverage = coverage;
        }

        public List<AWCinformation> Handle(GetAWCinformationQuery query)
        {
            var result = new List<AWCinformation>();
            try
            {
                var qq = (from info in _coverage.Get()
                          select new AWCinformation()
                          {
                              Base_L2 = info.BASEL2,
                              Site_Name = info.SITENAME,
                              Aumphur = info.DISTRICT,
                              Tumbon = info.SUB_DISTRICT,
                              Province = info.PROVINCE,
                              Zone = info.ZONE,
                              Lat = info.LAT,
                              Lon = info.LNG,
                              ACTIVE_FLAGAPPC = info.ACTIVE_FLAG,
                              APP_ID = info.APPID

                          });
                result = qq.ToList();
            }
            catch (Exception)
            {
                throw;
            }
            return result;
        }
    }
}
