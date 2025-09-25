using System;
using System.Collections.Generic;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBData.Repository;
using WBBEntity.Models;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHanlders
{
    public class GetDSLAMMasterHandler : IQueryHandler<GetDSLAMMasterQuery, List<GridDSLAMModel>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_DSLAM_INFO> _FBB_DSLAM_INFO;
        private readonly IEntityRepository<FBB_COVERAGEAREA_RELATION> _FBB_COVERAGEAREA_RELATION;
        private readonly IEntityRepository<FBB_CFG_LOV> _FBB_CFG_LOV;

        public GetDSLAMMasterHandler(ILogger logger
            , IEntityRepository<FBB_DSLAM_INFO> FBB_DSLAM_INFO
            , IEntityRepository<FBB_COVERAGEAREA_RELATION> FBB_COVERAGEAREA_RELATION
            , IEntityRepository<FBB_CFG_LOV> FBB_CFG_LOV)
        {
            _logger = logger;
            _FBB_DSLAM_INFO = FBB_DSLAM_INFO;
            _FBB_COVERAGEAREA_RELATION = FBB_COVERAGEAREA_RELATION;
            _FBB_CFG_LOV = FBB_CFG_LOV;
        }

        public List<GridDSLAMModel> Handle(GetDSLAMMasterQuery query)
        {
            IQueryable<decimal> cvr = from r in _FBB_COVERAGEAREA_RELATION.Get()
                                      where r.ACTIVEFLAG == "Y"
                                      select r.DSLAMID;

            var a = (from r in _FBB_DSLAM_INFO.Get()
                     where !cvr.Contains(r.DSLAMID) && r.ACTIVEFLAG == "Y"
                     && r.NODEID == null
                     group r by new { r.REGION_CODE, r.LOT_NUMBER } into g
                     select new GridDSLAMModel
                     {
                         Region = "",
                         RegionCode = g.Key.REGION_CODE,
                         LotNo = g.Key.LOT_NUMBER,
                         CurrentNo = g.Select(s => s.DSLAMID).Count(),
                         CreatedDate = g.Select(s => s.CREATED_DATE).Max()
                     });

            var t = (from r in a
                     join l in _FBB_CFG_LOV.Get() on r.RegionCode equals l.LOV_NAME
                     where l.LOV_TYPE == "REGION_CODE" && l.ACTIVEFLAG == "Y"
                     select new GridDSLAMModel
                     {
                         Region = l.DISPLAY_VAL,
                         RegionCode = r.RegionCode,
                         LotNo = r.LotNo,
                         CurrentNo = r.CurrentNo,
                         CreatedDate = r.CreatedDate
                     }).OrderByDescending(o => o.CreatedDate);

            var list = new List<GridDSLAMModel>();
            foreach (var x in t)
            {
                var temp = (from r in _FBB_DSLAM_INFO.Get()
                            where r.ACTIVEFLAG == "Y" && r.LOT_NUMBER == x.LotNo && r.REGION_CODE == x.RegionCode
                            group r by new { r.LOT_NUMBER, r.REGION_CODE } into g
                            select g.Count());

                var totalLot = (from r in _FBB_DSLAM_INFO.Get()
                                where r.LOT_NUMBER == x.LotNo
                                group r by r.LOT_NUMBER into g
                                select g.Count());

                list.Add(new GridDSLAMModel
                {
                    Region = x.Region,
                    RegionCode = x.RegionCode,
                    LotNo = x.LotNo,
                    CurrentNo = x.CurrentNo,
                    CreatedDate = x.CreatedDate,
                    CurrentCount = temp.FirstOrDefault(),
                    TotalLot = totalLot.FirstOrDefault()
                });
            }

            return list;

        }
    }
}
