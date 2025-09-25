using System.Collections.Generic;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBData.Repository;
using WBBEntity.Models;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers
{
    public class SelectOwnerProductHandler : IQueryHandler<SelectOwnerProductQuery, List<LovModel>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_COVERAGE_REGION> _FBB_COVERAGE_REGION;

        public SelectOwnerProductHandler(ILogger logger, IEntityRepository<FBB_COVERAGE_REGION> FBB_COVERAGE_REGION)
        {
            _logger = logger;
            _FBB_COVERAGE_REGION = FBB_COVERAGE_REGION;
        }


        public List<LovModel> Handle(SelectOwnerProductQuery query)
        {
            return (from r in _FBB_COVERAGE_REGION.Get()
                    where r.ACTIVEFLAG == "Y"
                    group r by r.OWNER_PRODUCT into g
                    orderby g.Key
                    select new LovModel
                    {
                        LOV_NAME = g.Key,
                        DISPLAY_VAL = g.Key
                    }).ToList();
        }
    }
}
