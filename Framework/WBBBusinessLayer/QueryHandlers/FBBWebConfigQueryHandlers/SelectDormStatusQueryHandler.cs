using System.Collections.Generic;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBData.Repository;
using WBBEntity.Models;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers
{
    public class SelectDormStatusQueryHandler : IQueryHandler<SelectDormStatusQuery, List<LovModel>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_CFG_LOV> _FBB_CFG_LOV;

        public SelectDormStatusQueryHandler(ILogger logger, IEntityRepository<FBB_CFG_LOV> FBB_CFG_LOV)
        {
            _logger = logger;
            _FBB_CFG_LOV = FBB_CFG_LOV;
        }


        public List<LovModel> Handle(SelectDormStatusQuery query)
        {
            List<LovModel> List, ddllist;

            if (query.FlagSearch)
            {
                List = (from r in _FBB_CFG_LOV.Get()
                        where r.LOV_TYPE == "FBBDORM_ADMIN_SCREEN" && r.LOV_NAME == "D_STATUS"
                        group r by r.LOV_VAL1 into g
                        orderby g.Key
                        select new LovModel
                        {
                            DISPLAY_VAL = g.Key,
                            LOV_NAME = g.Key

                        }).Distinct().ToList();

            }
            else
            {
                List = (from r in _FBB_CFG_LOV.Get()
                        where r.LOV_TYPE == "FBBDORM_ADMIN_SCREEN" && r.LOV_NAME == "D_STATUS" && r.LOV_VAL2 != null
                        group r by r.LOV_VAL2 into g
                        orderby g.Key
                        select new LovModel
                        {
                            DISPLAY_VAL = g.Key,
                            LOV_NAME = g.Key

                        }).Distinct().ToList();
            }

            ddllist = List.OrderBy(x => x.DISPLAY_VAL).ToList();

            return ddllist;


            //(from r in _FBB_CFG_LOV.Get()
            //      where r.LOV_TYPE == "FBBDORM_ADMIN_SCREEN" && r.LOV_NAME == "D_STATUS" 
            //      group r by r.LOV_VAL1 into g
            //       orderby g.Key
            //        select new LovModel
            //        {
            //            DISPLAY_VAL = g.Key,
            //            LOV_NAME = g.Key

            //        }).Distinct().ToList();
        }
    }
}
