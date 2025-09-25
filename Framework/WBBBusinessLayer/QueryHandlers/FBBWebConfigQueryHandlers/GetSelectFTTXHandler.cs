using System.Collections.Generic;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBData.Repository;
using WBBEntity.Models;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers
{


    public class GetSelectFTTXHandler : IQueryHandler<SelectFTTXQuery, List<LovModel>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_CFG_LOV> _FBB_CFG_LOV;

        public GetSelectFTTXHandler(ILogger logger, IEntityRepository<FBB_CFG_LOV> FBB_CFG_LOV)
        {
            _logger = logger;
            _FBB_CFG_LOV = FBB_CFG_LOV;
        }

        public List<LovModel> Handle(SelectFTTXQuery query)
        {


            var zipcodes = from z in _FBB_CFG_LOV.Get()
                           where z.LOV_TYPE == "FTTx"
                           select new LovModel()
                           {
                               LOV_NAME = z.LOV_VAL2,
                               DISPLAY_VAL = z.LOV_VAL1
                           };

            return zipcodes.ToList();
        }
    }
}
