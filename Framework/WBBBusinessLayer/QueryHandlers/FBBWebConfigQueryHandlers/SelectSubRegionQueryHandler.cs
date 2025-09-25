using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers
{
    public class SelectSubRegionQueryHandler : IQueryHandler<SelectSubRegionQuery, string>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_ZIPCODE> _FBB_ZIPCODE;

        public SelectSubRegionQueryHandler(ILogger logger, IEntityRepository<FBB_ZIPCODE> FBB_ZIPCODE)
        {
            _logger = logger;
            _FBB_ZIPCODE = FBB_ZIPCODE;
        }

        public string Handle(SelectSubRegionQuery query)
        {
            if (query.currentculture.IsThaiCulture())
            {

                return (from sub in _FBB_ZIPCODE.Get()
                        where sub.ZIPCODE_ROWID == query.rowid && sub.LANG_FLAG == "N" && sub.STATUS == "A"
                        select sub.SUB_REGION).FirstOrDefault();
            }
            else
            {
                return (from sub in _FBB_ZIPCODE.Get()
                        where sub.ZIPCODE_ROWID == query.rowid && sub.LANG_FLAG == "Y" && sub.STATUS == "A"
                        select sub.SUB_REGION).FirstOrDefault();
            }


        }

    }
}
