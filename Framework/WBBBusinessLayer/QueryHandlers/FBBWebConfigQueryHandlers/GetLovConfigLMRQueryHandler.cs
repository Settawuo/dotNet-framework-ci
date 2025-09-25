using System.Collections.Generic;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers
{
    public class GetLovConfigLMRQueryHandler : IQueryHandler<GetLovConfigLMRQuery, List<ConfigurationLMREmailModel>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_CFG_LOV> _lovService;

        public GetLovConfigLMRQueryHandler(ILogger logger, IEntityRepository<FBB_CFG_LOV> lovService)
        {
            _logger = logger;
            _lovService = lovService;
        }
        public List<ConfigurationLMREmailModel> Handle(GetLovConfigLMRQuery query)
        {
            List<FBB_CFG_LOV> loveList = null;
            if (!string.IsNullOrEmpty(query.LovType) && string.IsNullOrEmpty(query.LovValue5))
            {
                loveList = _lovService
                    .Get(lov => lov.ACTIVEFLAG.Equals("Y") && lov.LOV_TYPE == query.LovType)
                    .OrderBy(o => o.ORDER_BY).ToList();
            }
            else
            {
                loveList = _lovService
                   .Get(lov => lov.ACTIVEFLAG.Equals("Y") && lov.LOV_TYPE == query.LovType && lov.LOV_VAL5 == query.LovValue5)
                   .OrderBy(o => o.ORDER_BY).ToList();
            }

            var loveValueModelList = loveList.ToList().Select(l => new ConfigurationLMREmailModel
            {
                Text = l.DISPLAY_VAL,
                Type = l.LOV_TYPE,
                Name = l.LOV_NAME,
                Id = l.LOV_ID,
                ParId = l.PAR_LOV_ID.ToSafeDecimal(),
                LovValue1 = l.LOV_VAL1,
                LovValue2 = l.LOV_VAL2,
                LovValue3 = l.LOV_VAL3,
                LovValue4 = l.LOV_VAL4,
                LovValue5 = l.LOV_VAL5,
                OrderBy = l.ORDER_BY,
                DefaultValue = l.DEFAULT_VALUE,
                Image_blob = l.IMAGE_BLOB,
                Update_by = l.UPDATED_BY,
                Update_date = l.UPDATED_DATE

            }).ToList();

            return loveValueModelList;
        }
    }
}
