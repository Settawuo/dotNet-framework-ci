using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBData.Repository;
using WBBEntity.Models;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers
{
    public class SelectLovDefaultValueHandler : IQueryHandler<SelectLovDefaultValueQuery, LovModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_CFG_LOV> _FBB_CFG_LOV;

        public SelectLovDefaultValueHandler(ILogger logger, IEntityRepository<FBB_CFG_LOV> FBB_CFG_LOV)
        {
            _logger = logger;
            _FBB_CFG_LOV = FBB_CFG_LOV;
        }


        public LovModel Handle(SelectLovDefaultValueQuery query)
        {
            var a = (from r in _FBB_CFG_LOV.Get()
                     where r.LOV_TYPE == query.LOV_TYPE && r.ACTIVEFLAG == "Y" && r.DEFAULT_VALUE == "Y"
                     orderby r.ORDER_BY
                     select new LovModel
                     {
                         LOV_NAME = r.LOV_NAME,
                         DISPLAY_VAL = r.DISPLAY_VAL,
                         LOV_VAL1 = r.LOV_VAL1
                     });

            if (a.Any())
                return a.FirstOrDefault();
            else
                return new LovModel();
        }
    }
}
