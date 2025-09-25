using Notify;
using System.Collections.Generic;
using System.Linq;
using WBBBusinessLayer.Extension;
using WBBContract;
using WBBContract.Queries.Commons.Masters;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels;

namespace WBBBusinessLayer.QueryHandlers.Commons.Master
{
    public class GetLovQueryV2Handler : IQueryHandler<GetLovV2Query, List<LovValueModel>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_CFG_LOV> _lovService;

        public GetLovQueryV2Handler(ILogger logger, IEntityRepository<FBB_CFG_LOV> lovService)
        {
            _logger = logger;
            _lovService = lovService;
        }

        public List<LovValueModel> Handle(GetLovV2Query query)
        {
            List<FBB_CFG_LOV> loveList = null;
            if (string.IsNullOrEmpty(query.LovType) && string.IsNullOrEmpty(query.LovVal5))
            {
                loveList = _lovService
                    .Get(lov => lov.ACTIVEFLAG.Equals("Y"))
                    .OrderBy(o => o.ORDER_BY).ToList();
            }
            else
            {
                loveList = _lovService
                    .Get(lov => (!string.IsNullOrEmpty(query.LovType) && lov.LOV_TYPE.Equals(query.LovType))
                            && (string.IsNullOrEmpty(query.LovVal5) || lov.LOV_VAL5.Equals(query.LovVal5))
                            && (string.IsNullOrEmpty(query.LovVal3) || lov.LOV_VAL3.Equals(query.LovVal3))
                            && lov.ACTIVEFLAG.Equals("Y"))
                            .OrderBy(o => o.ORDER_BY).ToList();
            }

            var loveValueModelList = loveList.ToList().Select(l => new LovValueModel
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
