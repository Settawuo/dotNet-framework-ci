using System.Collections.Generic;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBData.Repository;
using WBBEntity.Models;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers
{
    public class GetLastMileByDistanceConfigQueryHandler : IQueryHandler<GetLastMileByDistanceConfigQuery, List<LovModel>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_CFG_LOV> _FBB_CFG_LOV;

        public GetLastMileByDistanceConfigQueryHandler(ILogger logger, IEntityRepository<FBB_CFG_LOV> _FBBCFGLOV)
        {
            _logger = logger;
            _FBB_CFG_LOV = _FBBCFGLOV;
        }
        public List<LovModel> Handle(GetLastMileByDistanceConfigQuery query)
        {
            List<LovModel> a = new List<LovModel>();
            //return (from r in _FBSS_FIXASSET.Get()

            //        where r.PROGRAM_NAME == query.Program
            //        orderby r.ASSET_CLASS_GI
            //        select new LovModel
            //        {
            //            LOV_NAME = r.COM_CODE,
            //            DISPLAY_VAL = r.COM_CODE
            //        }).ToList();
            return a;

        }
    }
}
