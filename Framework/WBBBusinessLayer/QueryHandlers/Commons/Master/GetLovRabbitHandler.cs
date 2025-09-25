using System.Collections.Generic;
using System.Linq;
using WBBContract;
using WBBContract.Queries.Commons.Master;
using WBBData.Repository;
using WBBEntity.Models;
using WBBEntity.PanelModels;

namespace WBBBusinessLayer.QueryHandlers.Commons.Master
{
    public class GetLovRabbitHandler : IQueryHandler<GetLovRabbitQuery, List<LovValueModel>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_CFG_LOV> _lovService;

        public GetLovRabbitHandler(ILogger logger, IEntityRepository<FBB_CFG_LOV> lovService)
        {
            _logger = logger;
            _lovService = lovService;
        }

        public List<LovValueModel> Handle(GetLovRabbitQuery query)
        {
            return (from r in _lovService.Get()
                    where r.LOV_VAL5 == "FBBOR008" && r.ACTIVEFLAG == "Y"
                    select new LovValueModel
                    {
                        Name = r.LOV_NAME,
                        Text = r.LOV_VAL1
                    }).ToList();
        }


    }
}
