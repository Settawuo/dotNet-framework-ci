using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBData.Repository;
using WBBEntity.Models;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers.DSLAMMasterPage
{
    public class GetModelMaxSlotHandler : IQueryHandler<GetModelMaxSlotQuery, decimal>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_DSLAMMODEL> _FBB_DSLAMMODEL;

        public GetModelMaxSlotHandler(ILogger logger, IEntityRepository<FBB_DSLAMMODEL> FBB_DSLAMMODEL)
        {
            _logger = logger;
            _FBB_DSLAMMODEL = FBB_DSLAMMODEL;
        }

        public decimal Handle(GetModelMaxSlotQuery query)
        {
            return (from r in _FBB_DSLAMMODEL.Get()
                    where r.MODEL == query.Model
                    select r.MAXSLOT).FirstOrDefault();
        }
    }
}
