using System.Collections.Generic;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBData.Repository;
using WBBEntity.Models;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers
{
    public class SelectAmphurDormENQueryHandler : IQueryHandler<SelectAmphurDormENQuery, List<LovModel>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_ZIPCODE> _FBB_ZIPCODE;

        public SelectAmphurDormENQueryHandler(ILogger logger, IEntityRepository<FBB_ZIPCODE> FBB_ZIPCODE)
        {
            _logger = logger;
            _FBB_ZIPCODE = FBB_ZIPCODE;
        }

        public List<LovModel> Handle(SelectAmphurDormENQuery query)
        {
            if (query.PROVINCE == "")
            {
                return (from r in _FBB_ZIPCODE.Get()
                        where r.LANG_FLAG == query.Lang_Flag && r.STATUS == "A"
                        group r by r.AMPHUR into g
                        orderby g.Key
                        select new LovModel
                        {
                            LOV_NAME = g.Key,
                            DISPLAY_VAL = g.Key
                        }).ToList();
            }
            else
            {
                return (from r in _FBB_ZIPCODE.Get()
                        where r.LANG_FLAG == query.Lang_Flag && r.STATUS == "A" && r.PROVINCE == query.PROVINCE
                        group r by r.AMPHUR into g
                        orderby g.Key
                        select new LovModel
                        {
                            LOV_NAME = g.Key,
                            DISPLAY_VAL = g.Key
                        }).ToList();
            }
        }


    }

}
