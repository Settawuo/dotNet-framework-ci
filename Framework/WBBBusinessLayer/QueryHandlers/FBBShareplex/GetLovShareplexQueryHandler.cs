using Notify;
using System.Collections.Generic;
using System.Linq;
using WBBBusinessLayer.Extension;
using WBBContract;
using WBBContract.Queries.Commons.FBBShareplex;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels;

namespace WBBBusinessLayer.QueryHandlers.Commons.FBBShareplex
{
    public class GetLovShareplexQueryHandler : IQueryHandler<GetLovShareplexQuery, List<LovValueModel>>
    {
        private readonly ILogger _logger;
        private readonly IFBBShareplexEntityRepository<FBB_CFG_LOV> _lovService;

        public GetLovShareplexQueryHandler(ILogger logger, IFBBShareplexEntityRepository<FBB_CFG_LOV> lovService)
        {
            _logger = logger;
            _lovService = lovService;
        }

        public List<LovValueModel> Handle(GetLovShareplexQuery query)
        {
            var stringQuery = string.Format("SELECT * FROM WBB.fbb_cfg_lov where lov_type = '" + query.LovType + "' and lov_name ='" + query.LovName + "' and activeflag = 'Y'");
            var LovValueModel = _lovService.SqlQuery(stringQuery).ToList();

            var loveValueModelList = LovValueModel.ToList().Select(l => new LovValueModel
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
                ActiveFlag = l.ACTIVEFLAG

            }).ToList();

            LineNotify.SendMessage(ServicesConstants.NotifyKey.LineNotifyFBB, this.GetType().Name, "LOV Length: " + loveValueModelList.Count);

            return loveValueModelList;
        }
    }
}
