using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBData.Repository;
using WBBEntity.Models;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers
{
    public class GetTriplePlayDupQueryHandler : IQueryHandler<GetTriplePlayDupQuery, string>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_CUST_PROFILE> _FBB_CUST_PROFILE;
        private readonly IEntityRepository<FBB_CUST_PACKAGE> _FBB_CUST_PACKAGE;
        private readonly IEntityRepository<FBB_REGISTER> _FBB_REGISTER;
        private readonly IEntityRepository<FBB_PACKAGE_TRAN> _FBB_PACKAGE_TRAN;

        public GetTriplePlayDupQueryHandler(ILogger logger, IEntityRepository<FBB_CUST_PROFILE> FBB_CUST_PROFILE, IEntityRepository<FBB_REGISTER> FBB_REGISTER,
            IEntityRepository<FBB_CUST_PACKAGE> FBB_CUST_PACKAGE, IEntityRepository<FBB_PACKAGE_TRAN> FBB_PACKAGE_TRAN)
        {
            _logger = logger;
            _FBB_CUST_PROFILE = FBB_CUST_PROFILE;
            _FBB_REGISTER = FBB_REGISTER;
            _FBB_CUST_PACKAGE = FBB_CUST_PACKAGE;
            _FBB_PACKAGE_TRAN = FBB_PACKAGE_TRAN;
        }

        public string Handle(GetTriplePlayDupQuery query)
        {
            var result1 = 0;
            var result2 = 0;


            var result = (from r in _FBB_CUST_PROFILE.Get()
                          join p in _FBB_CUST_PACKAGE.Get() on r.CUST_NON_MOBILE equals p.CUST_NON_MOBILE
                          where r.RELATE_MOBILE == query.MobileNo && r.CUST_NON_MOBILE.StartsWith("8")
                          && p.PACKAGE_GROUP.Contains("Triple") && p.PACKAGE_CLASS == "Main"
                          select r);

            if (result.Any())
            {
                result1 = result.Count();
            }

            var temp = (from r in _FBB_REGISTER.Get()
                        join p in _FBB_PACKAGE_TRAN.Get() on r.ROW_ID equals p.CUST_ROW_ID
                        where r.AIS_MOBILE == query.MobileNo
                        && p.PACKAGE_GROUP.Contains("Triple") && p.PACKAGE_CLASS == "Main"
                        select r);

            if (temp.Any())
            {
                result2 = temp.Count();
            }

            if (result1 == 0 && result2 == 0)
            {
                return "True";
            }



            return "False";
        }
    }
}
