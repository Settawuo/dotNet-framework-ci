using System.Collections.Generic;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBData.Repository;
using WBBEntity.Models;
using WBBEntity.PanelModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers
{
    public class SelectTumbonAirHandler : IQueryHandler<SelectTumbonAirQuery, List<DropdownModel>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_APCOVERAGE> _FBB_APCOVERAGE;

        public SelectTumbonAirHandler(ILogger logger, IEntityRepository<FBB_APCOVERAGE> FBB_APCOVERAGE)
        {
            _logger = logger;
            _FBB_APCOVERAGE = FBB_APCOVERAGE;
        }


        public List<DropdownModel> Handle(SelectTumbonAirQuery query)
        {
            var result = (from r in _FBB_APCOVERAGE.Get()
                          where r.PROVINCE == query.PROVINCE && r.DISTRICT == query.AUMPHUR && r.ACTIVE_FLAG == "Y"
                          group r by r.SUB_DISTRICT into g
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
