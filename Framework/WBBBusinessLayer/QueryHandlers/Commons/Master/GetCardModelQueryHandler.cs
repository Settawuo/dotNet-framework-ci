using System.Collections.Generic;
using System.Linq;
using WBBContract;
using WBBContract.Queries.Commons.Master;
using WBBData.Repository;
using WBBEntity.Models;

namespace WBBBusinessLayer.QueryHandlers.Commons.Master
{
    public class GetCardModelQueryHandler : IQueryHandler<GetCardModelQuery, List<FBB_CARDMODEL>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_CARDMODEL> _carmodel;

        public GetCardModelQueryHandler(ILogger logger, IEntityRepository<FBB_CARDMODEL> carmodel)
        {
            _logger = logger;
            _carmodel = carmodel;
        }


        public List<FBB_CARDMODEL> Handle(GetCardModelQuery query)
        {
            var listCardInfo = (from cardinfo in _carmodel.Get()
                                where cardinfo.ACTIVEFLAG == "Y"
                                select cardinfo).ToList();

            return listCardInfo;








        }
    }
}
