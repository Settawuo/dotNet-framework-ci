using System.Collections.Generic;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries.DSLAMMasterPage;
using WBBData.Repository;
using WBBEntity.Models;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers.DSLAMMasterPage
{
    public class GetCardModelHandler : IQueryHandler<GetCardModelQuery, List<LovModel>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_CARDMODEL> _FBB_CARDMODEL;

        public GetCardModelHandler(ILogger logger, IEntityRepository<FBB_CARDMODEL> FBB_CARDMODEL)
        {
            _logger = logger;
            _FBB_CARDMODEL = FBB_CARDMODEL;
        }

        public List<LovModel> Handle(GetCardModelQuery query)
        {
            var a = (from r in _FBB_CARDMODEL.Get()
                     where r.ACTIVEFLAG == "Y"
                     select new LovModel
                     {
                         DISPLAY_VAL = r.MODEL,
                         LOV_NAME = r.DATAONLY_FLAG == "Y" ? "Data" : "Data+Voice"
                     }).ToList();
            return a;
        }
    }
}
