using System.Collections.Generic;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBData.Repository;
using WBBEntity.Models;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers
{
    public class SelectDormitoryNameQueryHandler : IQueryHandler<SelectDormitoryNameQuery, List<LovModel>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBBDORM_DORMITORY_MASTER> _FBB_DORMMASTER;

        public SelectDormitoryNameQueryHandler(ILogger logger, IEntityRepository<FBBDORM_DORMITORY_MASTER> FBB_DORMMASTER)
        {
            _logger = logger;
            _FBB_DORMMASTER = FBB_DORMMASTER;
        }


        public List<LovModel> Handle(SelectDormitoryNameQuery query)
        {
            return (from r in _FBB_DORMMASTER.Get()
                    where r.STATE == query.State && r.ADDRESS_ID == "XXX"
                    group r by r.DORMITORY_NAME_TH into g
                    orderby g.Key
                    select new LovModel
                    {
                        LOV_NAME = g.Key,
                        DISPLAY_VAL = g.Key
                    }).ToList();
        }
    }

    public class SelectAllDormitoryNameQueryHandler : IQueryHandler<SelectAllDormitoryNameQuery, List<LovModel>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBBDORM_DORMITORY_MASTER> _FBB_DORMMASTER;
        private readonly IEntityRepository<FBBDORM_DORMITORY_DTL> _FBB_DORMDTL;
        private readonly IEntityRepository<FBB_ZIPCODE> _FBB_ZIPCODE;

        public SelectAllDormitoryNameQueryHandler(ILogger logger, IEntityRepository<FBBDORM_DORMITORY_MASTER> FBB_DORMMASTER
            , IEntityRepository<FBBDORM_DORMITORY_DTL> FBB_DORMDTL
            , IEntityRepository<FBB_ZIPCODE> FBB_ZIPCODE)
        {
            _logger = logger;
            _FBB_DORMMASTER = FBB_DORMMASTER;
            _FBB_DORMDTL = FBB_DORMDTL;
            _FBB_ZIPCODE = FBB_ZIPCODE;
        }


        public List<LovModel> Handle(SelectAllDormitoryNameQuery query)
        {
            if (query.Province == "")
            {
                return (from r1 in _FBB_DORMMASTER.Get()
                        group r1 by r1.DORMITORY_NAME_TH into g
                        orderby g.Key
                        select new LovModel
                        {
                            LOV_NAME = g.Key,
                            DISPLAY_VAL = g.Key
                        }).ToList();
            }
            else if (query.Region == "" && query.Province != "")
            {
                return (from r1 in _FBB_DORMMASTER.Get()
                        join r2 in _FBB_ZIPCODE.Get()
                        on r1.ZIPCODE_ROW_ID_TH equals r2.ZIPCODE_ROWID
                        where r2.PROVINCE == query.Province
                        group r1 by r1.DORMITORY_NAME_TH into g
                        orderby g.Key
                        select new LovModel
                        {
                            LOV_NAME = g.Key,
                            DISPLAY_VAL = g.Key
                        }).ToList();

            }
            else
            {
                return (from r1 in _FBB_DORMMASTER.Get()
                        join r2 in _FBB_ZIPCODE.Get()
                        on r1.ZIPCODE_ROW_ID_TH equals r2.ZIPCODE_ROWID
                        where r2.PROVINCE == query.Province && r2.REGION_CODE == query.Region
                        group r1 by r1.DORMITORY_NAME_TH into g
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
