using System.Collections.Generic;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries.AirWirelessCoverage;
using WBBData.Repository;
using WBBEntity.Models;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers.AirNetWirelessCoverage
{
    public class GetAWZipCodeQueryHandler : IQueryHandler<GetAWZipCodeQuery, List<ZipCode>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_ZIPCODE> _fbbZipCode;

        public GetAWZipCodeQueryHandler(ILogger logger, IEntityRepository<FBB_ZIPCODE> fbbZipCode)
        {
            _logger = logger;
            _fbbZipCode = fbbZipCode;
        }

        public List<ZipCode> Handle(GetAWZipCodeQuery query)
        {
            List<ZipCode> result;
            var ttp = (from c in _fbbZipCode.Get()
                       select new ZipCode()
                       {
                           SUB_REGION = c.SUB_REGION

                       }).Distinct().OrderBy(t => t.SUB_REGION);
            result = ttp.ToList();

            return result;
        }
    }
}
