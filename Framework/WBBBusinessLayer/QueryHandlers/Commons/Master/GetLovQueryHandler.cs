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
    public class GetLovQueryHandler : IQueryHandler<GetLovQuery, List<LovValueModel>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_CFG_LOV> _lovService;

        public GetLovQueryHandler(ILogger logger, IEntityRepository<FBB_CFG_LOV> lovService)
        {
            _logger = logger;
            _lovService = lovService;
        }

        public List<LovValueModel> Handle(GetLovQuery query)
        {
            List<FBB_CFG_LOV> loveList = null;
            if (string.IsNullOrEmpty(query.LovType) && string.IsNullOrEmpty(query.LovName))
            {
                loveList = _lovService
                    .Get(lov => lov.ACTIVEFLAG.Equals("Y"))
                    .OrderBy(o => o.ORDER_BY).ToList();
            }
            else
            {
                if (query.IgonreFlag)
                {
                    loveList = _lovService
                        .Get(lov => (!string.IsNullOrEmpty(query.LovType) && lov.LOV_TYPE.Equals(query.LovType))
                        && (string.IsNullOrEmpty(query.LovName) || lov.LOV_NAME.Equals(query.LovName)))
                        .OrderBy(o => o.ORDER_BY).ToList();
                }
                else
                {
                    loveList = _lovService
                        .Get(lov => (!string.IsNullOrEmpty(query.LovType) && lov.LOV_TYPE.Equals(query.LovType))
                        && (string.IsNullOrEmpty(query.LovName) || lov.LOV_NAME.Equals(query.LovName))
                        && lov.ACTIVEFLAG.Equals("Y"))
                        .OrderBy(o => o.ORDER_BY).ToList();
                }
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
