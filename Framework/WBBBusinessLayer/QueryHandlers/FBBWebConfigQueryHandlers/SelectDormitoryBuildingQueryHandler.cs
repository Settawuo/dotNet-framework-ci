using System.Collections.Generic;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBData.Repository;
using WBBEntity.Models;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers
{
    public class SelectDormitoryBuildingQueryHandler : IQueryHandler<SelectDormitoryBuildingQuery, List<LovModel>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBBDORM_DORMITORY_MASTER> _FBB_DORMMASTER;

        public SelectDormitoryBuildingQueryHandler(ILogger logger, IEntityRepository<FBBDORM_DORMITORY_MASTER> FBB_DORMMASTER)
        {
            _logger = logger;
            _FBB_DORMMASTER = FBB_DORMMASTER;
        }

        public List<LovModel> Handle(SelectDormitoryBuildingQuery query)
        {
            return (from r in _FBB_DORMMASTER.Get()
                    where r.STATE == query.State && r.DORMITORY_NAME_TH == query.DormitoryName
                    group r by r.DORMITORY_NO_TH into g
                    orderby g.Key
                    select new LovModel
                    {
                        LOV_NAME = g.Key,
                        DISPLAY_VAL = g.Key
                    }).ToList();
        }
    }
    public class SelectAllDormitoryBuildingQueryHandler : IQueryHandler<SelectAllDormitoryBuildingQuery, List<LovModel>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBBDORM_DORMITORY_MASTER> _FBB_DORMMASTER;
        private readonly IEntityRepository<FBBDORM_DORMITORY_DTL> _FBB_DORMDTL;
        private readonly IEntityRepository<FBB_ZIPCODE> _FBB_ZIPCODE;

        public SelectAllDormitoryBuildingQueryHandler(ILogger logger, IEntityRepository<FBBDORM_DORMITORY_MASTER> FBB_DORMMASTER
            , IEntityRepository<FBBDORM_DORMITORY_DTL> FBB_DORMDTL
            , IEntityRepository<FBB_ZIPCODE> FBB_ZIPCODE)
        {
            _logger = logger;
            _FBB_DORMMASTER = FBB_DORMMASTER;
            _FBB_DORMDTL = FBB_DORMDTL;
            _FBB_ZIPCODE = FBB_ZIPCODE;
        }


        public List<LovModel> Handle(SelectAllDormitoryBuildingQuery query)
        {
            if (query.Province == "" && query.DormitoryName == "")
            {
                return (from r in _FBB_DORMMASTER.Get()
                        group r by r.DORMITORY_NO_TH into g
                        orderby g.Key
                        select new LovModel
                        {
                            LOV_NAME = g.Key,
                            DISPLAY_VAL = g.Key
                        }).ToList();
            }

            else if (query.Province != "" && query.DormitoryName == "")
            {
                return (from r in _FBB_DORMMASTER.Get()
                        join r2 in _FBB_ZIPCODE.Get()
                        on r.ZIPCODE_ROW_ID_TH equals r2.ZIPCODE_ROWID
                        where r2.PROVINCE == query.Province
                        group r by r.DORMITORY_NO_TH into g
                        orderby g.Key
                        select new LovModel
                        {
                            LOV_NAME = g.Key,
                            DISPLAY_VAL = g.Key
                        }).ToList();
            }
            else if (query.Province == "" && query.DormitoryName != "")
            {
                return (from r in _FBB_DORMMASTER.Get()
                        join r2 in _FBB_ZIPCODE.Get()
                        on r.ZIPCODE_ROW_ID_TH equals r2.ZIPCODE_ROWID
                        where r.DORMITORY_NAME_TH == query.DormitoryName
                        group r by r.DORMITORY_NO_TH into g
                        orderby g.Key
                        select new LovModel
                        {
                            LOV_NAME = g.Key,
                            DISPLAY_VAL = g.Key
                        }).ToList();
            }
            else
            {
                return (from r in _FBB_DORMMASTER.Get()
                        join r2 in _FBB_ZIPCODE.Get()
                      on r.ZIPCODE_ROW_ID_TH equals r2.ZIPCODE_ROWID
                        where r2.PROVINCE == query.Province && r.DORMITORY_NAME_TH == query.DormitoryName
                        group r by r.DORMITORY_NO_TH into g
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

