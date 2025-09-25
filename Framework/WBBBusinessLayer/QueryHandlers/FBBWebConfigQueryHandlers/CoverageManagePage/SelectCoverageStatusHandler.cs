using System.Collections.Generic;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries.CoverageManagePage;
using WBBData.Repository;
using WBBEntity.Models;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers.CoverageManagePage
{
    public class SelectCoverageStatusHandler : IQueryHandler<SelectCoverageStatusQuery, List<LovModel>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_CFG_LOV> _FBB_CFG_LOV;

        public SelectCoverageStatusHandler(ILogger logger, IEntityRepository<FBB_CFG_LOV> FBB_CFG_LOV)
        {
            _logger = logger;
            _FBB_CFG_LOV = FBB_CFG_LOV;
        }


        public List<LovModel> Handle(SelectCoverageStatusQuery query)
        {
            if (query.Status == "ON_PROGRESS" || query.Status == "CLOSE_SITE")
            {
                return (from r in _FBB_CFG_LOV.Get()
                        where r.LOV_TYPE == query.LOV_TYPE && r.ACTIVEFLAG == "Y" && r.LOV_NAME == query.Status
                        orderby r.ORDER_BY
                        select new LovModel
                        {
                            LOV_NAME = r.LOV_NAME,
                            DISPLAY_VAL = r.DISPLAY_VAL,
                            LOV_VAL1 = r.LOV_VAL1
                        }).ToList();
            }
            else
            {
                return (from r in _FBB_CFG_LOV.Get()
                        where r.LOV_TYPE == query.LOV_TYPE && r.ACTIVEFLAG == "Y" && r.LOV_NAME != "ON_PROGRESS"
                        orderby r.ORDER_BY
                        select new LovModel
                        {
                            LOV_NAME = r.LOV_NAME,
                            DISPLAY_VAL = r.DISPLAY_VAL,
                            LOV_VAL1 = r.LOV_VAL1
                        }).ToList();
            }


        }
    }
}
