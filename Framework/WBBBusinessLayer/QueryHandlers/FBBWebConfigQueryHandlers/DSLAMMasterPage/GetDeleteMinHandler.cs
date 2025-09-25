using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBData.Repository;
using WBBEntity.Models;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHanlders
{
    public class GetDeleteMinHandler : IQueryHandler<GetDeleteMinQuery, decimal>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_DSLAM_INFO> _FBB_DSLAM_INFO;
        //private readonly IEntityRepository<FBB_COVERAGEAREA_RELATION> _FBB_COVERAGEAREA_RELATION;
        //private readonly IEntityRepository<FBB_CFG_LOV> _FBB_CFG_LOV;

        public GetDeleteMinHandler(ILogger logger, IEntityRepository<FBB_DSLAM_INFO> FBB_DSLAM_INFO)
        {
            _logger = logger;
            _FBB_DSLAM_INFO = FBB_DSLAM_INFO;
        }

        public decimal Handle(GetDeleteMinQuery query)
        {
            return (from r in _FBB_DSLAM_INFO.Get()
                    where r.LOT_NUMBER == query.Lot && r.REGION_CODE == query.RegionCode && r.NODEID == null && r.ACTIVEFLAG == "Y"
                    select r).Count();
        }
    }
}
