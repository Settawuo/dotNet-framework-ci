using System;
using System.Collections.Generic;
using System.Linq;
using WBBContract;
using WBBContract.Queries.WebServices;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class GetSalePortalChannelHandler : IQueryHandler<GetSalePortalChannelQuery, List<SalePortalChannelModel>>
    {
        private ILogger _logger;
        private IEntityRepository<FBB_PRE_REGISTER> _FBB_PRE_REGISTER;
        private IEntityRepository<FBB_CFG_LOV> _FBB_CFG_LOV;

        public GetSalePortalChannelHandler(
            ILogger logger,
            IEntityRepository<FBB_PRE_REGISTER> FBB_PREGISTER,
            IEntityRepository<FBB_CFG_LOV> FBB_CFG_LOV
            )
        {
            _logger = logger;
            _FBB_PRE_REGISTER = FBB_PREGISTER;
            _FBB_CFG_LOV = FBB_CFG_LOV;
        }

        public List<SalePortalChannelModel> Handle(GetSalePortalChannelQuery query)
        {
            try
            {
                List<string> exceptChannelList = _FBB_CFG_LOV.Get(o => o.LOV_TYPE == "CONFIG_REPORT_LEAVE").Select(o => o.LOV_VAL1).ToList();
                List<string> channelList = _FBB_PRE_REGISTER.Get(o => !exceptChannelList.Any(s => s == o.CHANNEL)).Select(o => o.CHANNEL).Distinct().ToList();

                query.ret_code = 0;
                query.ret_msg = "Success";

                return channelList.Select(o => new SalePortalChannelModel() { Name = o }).ToList();
            }
            catch (Exception ex)
            {
                _logger.Info(ex.GetErrorMessage());

                query.ret_code = -1;
                query.ret_msg = "Error";

                return null;
            }
        }
    }
}
