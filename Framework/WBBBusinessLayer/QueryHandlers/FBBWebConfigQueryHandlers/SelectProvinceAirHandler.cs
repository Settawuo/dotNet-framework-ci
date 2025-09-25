using System.Collections.Generic;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBData.Repository;
using WBBEntity.Models;
using WBBEntity.PanelModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers
{
    public class SelectProvinceAirHandler : IQueryHandler<SelecProvinceAirQuery, List<DropdownModel>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_APCOVERAGE> _FBB_APCOVERAGE;

        public SelectProvinceAirHandler(ILogger logger, IEntityRepository<FBB_APCOVERAGE> FBB_APCOVERAGE)
        {
            _logger = logger;
            _FBB_APCOVERAGE = FBB_APCOVERAGE;
        }


        public List<DropdownModel> Handle(SelecProvinceAirQuery query)
        {

            var result = (from r in _FBB_APCOVERAGE.Get()
                          where r.ZONE == query.REGION_CODE && r.ACTIVE_FLAG == "Y"
                          group r by r.PROVINCE into g
                          orderby g.Key
                          select new DropdownModel
                          {
                              Text = g.Key,
                              Value = g.Key
                          }).ToList();
            return result;

        }
    }
}
