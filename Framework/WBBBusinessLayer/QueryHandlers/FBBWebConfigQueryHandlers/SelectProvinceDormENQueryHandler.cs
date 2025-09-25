using System.Collections.Generic;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBData.Repository;
using WBBEntity.Models;
using WBBEntity.PanelModels.FBBWebConfigModels;
namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers
{
    public class SelectProvinceDormENQueryHandler : IQueryHandler<SelectProvinceDormENQuery, List<LovModel>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_ZIPCODE> _FBB_ZIPCODE;

        public SelectProvinceDormENQueryHandler(ILogger logger, IEntityRepository<FBB_ZIPCODE> FBB_ZIPCODE)
        {
            _logger = logger;
            _FBB_ZIPCODE = FBB_ZIPCODE;
        }


        public List<LovModel> Handle(SelectProvinceDormENQuery query)
        {

            return (from r in _FBB_ZIPCODE.Get()
                    where r.LANG_FLAG == query.Lang_Flag && r.STATUS == "A"
                    group r by r.PROVINCE into g
                    orderby g.Key
                    select new LovModel
                    {
                        LOV_NAME = g.Key,
                        DISPLAY_VAL = g.Key
                    }).ToList();
        }
    }
}
