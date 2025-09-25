using System.Linq;
using WBBContract;
using WBBContract.Queries.Commons.Master;
using WBBData.Repository;
using WBBEntity.Models;

namespace WBBBusinessLayer.QueryHandlers.Commons.Master
{
    public class ValidateNonMobilePinQueryHandler : IQueryHandler<ValidateNonMobilePinQuery, string>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBBDORM_DORMITORY_DTL> _FBB_DORM;

        public ValidateNonMobilePinQueryHandler(ILogger logger, IEntityRepository<FBBDORM_DORMITORY_DTL> FBB_DORM)
        {
            _logger = logger;
            _FBB_DORM = FBB_DORM;
        }

        public string Handle(ValidateNonMobilePinQuery query)
        {
            string result = "";
            if (query.NonMobilePin != "")
            {
                result = (from r in _FBB_DORM.Get()
                          where r.PREPAID_NON_MOBILE == query.NonMobilePin
                          select r.SERVICE_STATUS).FirstOrDefault();
            }
            else
            {

            }

            return result;
        }
    }

}
