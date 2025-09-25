using System.Collections.Generic;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries.AirWirelessCoverage;
using WBBData.Repository;
using WBBEntity.Models;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers.AirNetWirelessCoverage
{
    public class GetAWCAllCQueryHandler : IQueryHandler<GetAWCAllC, List<AWCexportlist>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_APCOVERAGE> _apcoverage;
        private readonly IEntityRepository<FBB_AP_INFO> _apifo;

        public GetAWCAllCQueryHandler(ILogger logger, IEntityRepository<FBB_APCOVERAGE> apcoverage, IEntityRepository<FBB_AP_INFO> apifo)
        {
            _logger = logger;
            _apcoverage = apcoverage;
            _apifo = apifo;
        }

        public List<AWCexportlist> Handle(GetAWCAllC query)
        {
            List<AWCexportlist> result;
            var ttp = (from c in _apcoverage.Get()
                       join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                       where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y"
                       select new AWCexportlist()
                       {
                           Base_L2 = c.BASEL2,
                           Site_Name = c.SITENAME,
                           Aumphur = c.DISTRICT,
                           Province = c.PROVINCE,
                           Tumbon = c.SUB_DISTRICT,
                           Lat = c.LAT,
                           Lon = c.LNG,
                           AP_Name = ifo.AP_NAME,
                           Sector = ifo.SECTOR,
                           Zone = c.ZONE

                       }).Distinct();

            //var aa = (from msg in _apcoverage.Get()
            //          join pp in _apifo on msg.
            //          select new AWCexportlist()
            //          {
            //              Base_L2 = msg.BASEL2,
            //              Site_Name = msg.SITENAME,
            //              Aumphur = msg.DISTRICT,
            //              Province = msg.PROVINCE,
            //              Tumbon = msg.SUB_DISTRICT,

            //          });
            result = ttp.ToList();

            return result;
        }
    }
}
