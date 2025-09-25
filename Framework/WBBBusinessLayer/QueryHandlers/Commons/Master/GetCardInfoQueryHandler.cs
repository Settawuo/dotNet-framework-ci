using System.Collections.Generic;
using System.Linq;
using WBBContract;
using WBBContract.Queries.Commons.Master;
using WBBData.Repository;
using WBBEntity.Models;

namespace WBBBusinessLayer.QueryHandlers.Commons.Master
{
    public class GetCardInfoQueryHandler : IQueryHandler<GetCardInfoQuery, List<FBB_CARD_INFO>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_CARD_INFO> _cardinfo;

        public GetCardInfoQueryHandler(ILogger logger, IEntityRepository<FBB_CARD_INFO> cardinfo)
        {
            _logger = logger;
            _cardinfo = cardinfo;
        }


        public List<FBB_CARD_INFO> Handle(GetCardInfoQuery query)
        {
            var listCardInfo = (from cardinfo in _cardinfo.Get()
                                where cardinfo.ACTIVEFLAG == "Y"
                                && cardinfo.DSLAMID == query.DSLAMId
                                select cardinfo).ToList();

            return listCardInfo;
        }
    }
}
