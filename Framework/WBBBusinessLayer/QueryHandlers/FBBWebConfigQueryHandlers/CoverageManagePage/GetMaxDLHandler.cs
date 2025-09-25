using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries.CoverageManagePage;
using WBBData.Repository;
using WBBEntity.Models;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers.CoverageManagePage
{
    public class GetMaxDLHandler : IQueryHandler<GetMaxDLQuery, decimal>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_DSLAM_INFO> _FBB_DSLAM_INFO;

        public GetMaxDLHandler(ILogger logger, IEntityRepository<FBB_DSLAM_INFO> FBB_DSLAM_INFO)
        {
            _logger = logger;
            _FBB_DSLAM_INFO = FBB_DSLAM_INFO;
        }

        public decimal Handle(GetMaxDLQuery query)
        {
            var a = (from r in _FBB_DSLAM_INFO.Get()
                     where r.CVRID == query.CVRID && r.ACTIVEFLAG == "Y"
                     orderby r.DSLAMNUMBER descending
                     select r.DSLAMNUMBER).FirstOrDefault();

            return a + 1;
        }

    }
}
