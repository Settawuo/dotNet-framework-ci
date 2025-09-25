using System.Linq;
using WBBContract;
using WBBContract.Queries.Commons.Masters;
using WBBData.Repository;
using WBBEntity.Models;
using WBBEntity.PanelModels;

namespace WBBBusinessLayer.QueryHandlers.Commons.Master
{
    public class GetLoadConfigLovHandler : IQueryHandler<GetLoadConfigLovQuery, LoadConfigLovModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_LOAD_CONFIG_LOV> _lovService;

        public GetLoadConfigLovHandler(ILogger logger, IEntityRepository<FBB_LOAD_CONFIG_LOV> lovService)
        {
            _logger = logger;
            _lovService = lovService;
        }

        public LoadConfigLovModel Handle(GetLoadConfigLovQuery query)
        {
            IQueryable<FBB_LOAD_CONFIG_LOV> loveList = null;
            loveList = _lovService.Get(l => l.EVENT_NAME == query.EVENT_NAME);

            var loveValueModelList = loveList.ToList().Select(l => new LoadConfigLovModel
            {
                EVENT_NAME = l.EVENT_NAME,
                FLAG = l.FLAG,

            }).FirstOrDefault();

            return loveValueModelList;
        }
    }
}
