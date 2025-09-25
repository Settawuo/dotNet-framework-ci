using System.Collections.Generic;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBData.Repository;
using WBBEntity.Models;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers
{
    public class SelectLovHandler : IQueryHandler<SelectLovQuery, List<LovModel>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_CFG_LOV> _FBB_CFG_LOV;

        public SelectLovHandler(ILogger logger, IEntityRepository<FBB_CFG_LOV> FBB_CFG_LOV)
        {
            _logger = logger;
            _FBB_CFG_LOV = FBB_CFG_LOV;
        }


        public List<LovModel> Handle(SelectLovQuery query)
        {
            List<LovModel> datalov;

            var result = (from r in _FBB_CFG_LOV.Get()
                          where r.LOV_TYPE == query.LOV_TYPE && r.ACTIVEFLAG == "Y"
                          orderby r.ORDER_BY
                          select new LovModel
                          {
                              LOV_NAME = r.LOV_NAME,
                              DISPLAY_VAL = r.DISPLAY_VAL,
                              LOV_VAL1 = r.LOV_VAL1,
                              LOV_VAL2 = r.LOV_VAL2
                          });
            datalov = result.ToList();
            return datalov;
        }
    }

    public class SelectLovByTypeAndLovVal5QueryHandler : IQueryHandler<SelectLovByTypeAndLovVal5Query, List<LovModel>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_CFG_LOV> _FBB_CFG_LOV;

        public SelectLovByTypeAndLovVal5QueryHandler(ILogger logger, IEntityRepository<FBB_CFG_LOV> FBB_CFG_LOV)
        {
            _logger = logger;
            _FBB_CFG_LOV = FBB_CFG_LOV;
        }


        public List<LovModel> Handle(SelectLovByTypeAndLovVal5Query query)
        {
            List<LovModel> datalov;

            var result = (from r in _FBB_CFG_LOV.Get()
                          where r.LOV_TYPE == query.LOV_TYPE
                          && r.ACTIVEFLAG == "Y"
                          && r.LOV_VAL5 == query.LOV_VAL5
                          orderby r.ORDER_BY
                          select new LovModel
                          {
                              LOV_NAME = r.LOV_NAME,
                              DISPLAY_VAL = r.DISPLAY_VAL,
                              LOV_VAL1 = r.LOV_VAL1,
                              LOV_VAL2 = r.LOV_VAL2,
                              LOV_VAL3 = r.LOV_VAL3,
                              LOV_VAL4 = r.LOV_VAL4
                          });
            datalov = result.ToList();
            return datalov;
        }
    }



    //----------------------------- Start : Add ListData FBB_CFG_LOV------------------------
    //------------------------------Date : 17/12/2020 -----------------------------

    public class SelectLovVal5QueryHandler : IQueryHandler<SelectLovVal5Query, List<LovModel>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_CFG_LOV> _FBB_CFG_LOV;

        public SelectLovVal5QueryHandler(ILogger logger, IEntityRepository<FBB_CFG_LOV> FBB_CFG_LOV)
        {
            _logger = logger;
            _FBB_CFG_LOV = FBB_CFG_LOV;
        }


        public List<LovModel> Handle(SelectLovVal5Query query)
        {
            List<LovModel> datalov;

            var result = (from r in _FBB_CFG_LOV.Get()
                          where r.ACTIVEFLAG == "Y"
                          && r.LOV_VAL5 == query.LOV_VAL5
                          orderby r.ORDER_BY
                          select new LovModel
                          {
                              LOV_TYPE = r.LOV_TYPE,
                              LOV_NAME = r.LOV_NAME,
                              DISPLAY_VAL = r.DISPLAY_VAL,
                              LOV_VAL1 = r.LOV_VAL1,
                              LOV_VAL2 = r.LOV_VAL2,
                              LOV_VAL3 = r.LOV_VAL3,
                              LOV_VAL4 = r.LOV_VAL4
                          });
            datalov = result.ToList();
            return datalov;
        }
    }
    //----------------------------- End : Add ListData FBB_CFG_LOV------------------------
    //------------------------------Date : 17/12/2020 -----------------------------
}
