using AIRNETEntity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries.AirWirelessCoverage;
using WBBData.Repository;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers.AirNetWirelessCoverage
{
    public class GetAWChannelGroupQueryHandler : IQueryHandler<GetAWChannelGroupQuery, List<ChannelGroup>>
    {
        private readonly ILogger _logger;
        private readonly IAirNetEntityRepository<AIR_CHANNEL_GROUP> _airchannelgroup;

        public GetAWChannelGroupQueryHandler(ILogger logger, IAirNetEntityRepository<AIR_CHANNEL_GROUP> airchannelgroup)
        {
            _logger = logger;
            _airchannelgroup = airchannelgroup;
        }

        public List<ChannelGroup> Handle(GetAWChannelGroupQuery query)
        {
            List<ChannelGroup> result;
            var ttp = (from c in _airchannelgroup.Get()
                       where c.EFFECTIVE_DATE <= DateTime.Now
                       select new ChannelGroup()
                       {
                           CatalogAndAuthorizeName = c.CATALOG_AUTHORIZE

                       }).Distinct();
            result = ttp.ToList();

            return result;
        }
    }
}
