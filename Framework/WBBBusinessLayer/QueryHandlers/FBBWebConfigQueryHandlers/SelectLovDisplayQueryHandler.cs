using System.Collections.Generic;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBData.Repository;
using WBBEntity.Models;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers
{
    public class SelectLovDisplayQueryHandler : IQueryHandler<SelectLovDisplayQuery, List<LovModel>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_CFG_LOV> _FBB_CFG_LOV;

        public SelectLovDisplayQueryHandler(ILogger logger, IEntityRepository<FBB_CFG_LOV> FBB_CFG_LOV)
        {
            _logger = logger;
            _FBB_CFG_LOV = FBB_CFG_LOV;
        }


        public List<LovModel> Handle(SelectLovDisplayQuery query)
        {
            List<LovModel> datalov;
            if (query.DISPLAY_VAL == "")
            {
                var sss = (from r in _FBB_CFG_LOV.Get()
                           where r.LOV_TYPE == query.LOV_TYPE && r.LOV_VAL5 == query.LOV_VAL5 && r.ACTIVEFLAG == "Y"
                           orderby r.ORDER_BY
                           select new LovModel
                           {
                               LOV_NAME = r.LOV_NAME,
                               LOV_VAL1 = r.LOV_VAL1,
                               LOV_VAL2 = r.LOV_VAL2,
                               DISPLAY_VAL = r.DISPLAY_VAL
                           });
                datalov = sss.ToList();
                return datalov;
            }
            else
            {
                var sss = (from r in _FBB_CFG_LOV.Get()
                           where r.LOV_TYPE == query.LOV_TYPE && r.LOV_VAL5 == query.LOV_VAL5
                                  && r.DISPLAY_VAL == query.DISPLAY_VAL && r.ACTIVEFLAG == "Y"
                           orderby r.ORDER_BY
                           select new LovModel
                           {
                               LOV_NAME = r.LOV_NAME,
                               LOV_VAL1 = r.LOV_VAL1,
                               LOV_VAL2 = r.LOV_VAL2,
                               DISPLAY_VAL = r.DISPLAY_VAL
                           });
                datalov = sss.ToList();
                return datalov;
            }
        }
    }
}
