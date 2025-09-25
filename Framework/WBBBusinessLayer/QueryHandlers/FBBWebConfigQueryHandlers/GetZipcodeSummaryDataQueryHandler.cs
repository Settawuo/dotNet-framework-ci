using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBData.Repository;
using WBBEntity.Models;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers
{


    public class GetZipcodeSummaryDataQueryHandler : IQueryHandler<GetoutPostalCodeQuery, string>
    {
        private readonly ILogger _logger;

        private readonly IEntityRepository<FBB_ZIPCODE> _FBB_ZIPCODE;

        public GetZipcodeSummaryDataQueryHandler(ILogger logger,
            IEntityRepository<FBB_ZIPCODE> FBB_ZIPCODE)
        {
            _logger = logger;
            _FBB_ZIPCODE = FBB_ZIPCODE;
        }

        public string Handle(GetoutPostalCodeQuery query)
        {

            var a = (from r in _FBB_ZIPCODE.Get()
                     where r.ZIPCODE == query.outPostalCode && r.TUMBON == query.outTumbol && r.AMPHUR == query.outAmphur && r.PROVINCE == query.outProvince && r.STATUS == "A"

                     select r.ZIPCODE_ROWID
                    ).ToList().FirstOrDefault();

            return a;
        }
    }
}
