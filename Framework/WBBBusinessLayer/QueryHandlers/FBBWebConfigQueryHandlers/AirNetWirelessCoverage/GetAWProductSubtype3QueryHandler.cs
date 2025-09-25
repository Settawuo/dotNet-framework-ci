using AIRNETEntity.Models;
using System.Collections.Generic;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries.AirWirelessCoverage;
using WBBData.Repository;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers.AirNetWirelessCoverage
{
    public class GetAWProductSubtype3QueryHandler : IQueryHandler<GetAWProductSubtype3Query, List<ProductSubtype3>>
    {
        private readonly ILogger _logger;
        private readonly IAirNetEntityRepository<AIR_PACKAGE_MASTER_DETAIL> _airpackagemasterdetail;

        public GetAWProductSubtype3QueryHandler(ILogger logger, IAirNetEntityRepository<AIR_PACKAGE_MASTER_DETAIL> airpackagemasterdetail)
        {
            _logger = logger;
            _airpackagemasterdetail = airpackagemasterdetail;
        }

        public List<ProductSubtype3> Handle(GetAWProductSubtype3Query query)
        {
            List<ProductSubtype3> result;
            var ttp = (from c in _airpackagemasterdetail.Get()
                       where c.PRODUCT_SUBTYPE3 != null
                       select new ProductSubtype3()
                       {
                           PRODUCT_SUBTYPE3 = c.PRODUCT_SUBTYPE3

                       }).Distinct().OrderBy(t => t.PRODUCT_SUBTYPE3);
            result = ttp.ToList();

            return result;
        }

    }
}
