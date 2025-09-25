using System.Collections.Generic;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBData.Repository;
using WBBEntity.Models;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers
{
    public class GetFixedAssetConfigQueryHandler : IQueryHandler<GetFixedAssetConfigQuery, List<LovModel>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBSS_FIXED_ASSET_CONFIG> _FBSS_FIXASSET;
        public GetFixedAssetConfigQueryHandler(ILogger logger, IEntityRepository<FBSS_FIXED_ASSET_CONFIG> FBSS_FIXASSET)
        {
            _logger = logger;
            _FBSS_FIXASSET = FBSS_FIXASSET;
        }
        public List<LovModel> Handle(GetFixedAssetConfigQuery query)
        {

            return (from r in _FBSS_FIXASSET.Get()

                    where r.PROGRAM_NAME == query.Program
                    orderby r.ASSET_CLASS_GI
                    select new LovModel
                    {
                        LOV_NAME = r.COM_CODE,
                        LOV_VAL1 = r.ASSET_CLASS_GI,
                        LOV_VAL2 = r.ASSET_CLASS_INS,
                        DISPLAY_VAL = r.COM_CODE
                    }).ToList();

        }
    }
}
