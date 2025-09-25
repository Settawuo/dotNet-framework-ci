using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries.DSLAMMasterPage;
using WBBData.Repository;
using WBBEntity.Models;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers.DSLAMMasterPage
{
    public class GetDSLAMModelHandler : IQueryHandler<GetDSLAMModelQuery, List<LovModel>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_DSLAMMODEL> _FBB_DSLAMMODEL;

        public GetDSLAMModelHandler(ILogger logger, IEntityRepository<FBB_DSLAMMODEL> FBB_DSLAMMODEL)
        {
            _logger = logger;
            _FBB_DSLAMMODEL = FBB_DSLAMMODEL;
        }

        public List<LovModel> Handle(GetDSLAMModelQuery query)
        {
            return (from r in _FBB_DSLAMMODEL.Get()
                    where r.ACTIVEFLAG == "Y"
                    select new LovModel
                    {
                        LOV_NAME = r.MODEL,
                        DISPLAY_VAL = r.MODEL
                    }).ToList();
        }
    }
}
