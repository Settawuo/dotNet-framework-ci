using System;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries.CoverageManagePage;
using WBBData.Repository;
using WBBEntity.Models;
using WBBEntity.PanelModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers.CoverageManagePage
{
    public class GetModelAndBrandHandler : IQueryHandler<GetModelAndBrandQuery, DropdownModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_DSLAM_INFO> _FBB_DSLAM_INFO;
        private readonly IEntityRepository<FBB_DSLAMMODEL> _FBB_DSLAMMODEL;
        private readonly IEntityRepository<FBB_COVERAGEAREA_RELATION> _FBB_COVERAGEAREA_RELATION;

        public GetModelAndBrandHandler(ILogger logger, IEntityRepository<FBB_DSLAM_INFO> FBB_DSLAM_INFO,
        IEntityRepository<FBB_DSLAMMODEL> FBB_DSLAMMODEL, IEntityRepository<FBB_COVERAGEAREA_RELATION> FBB_COVERAGEAREA_RELATION)
        {
            _logger = logger;
            _FBB_DSLAM_INFO = FBB_DSLAM_INFO;
            _FBB_DSLAMMODEL = FBB_DSLAMMODEL;
            _FBB_COVERAGEAREA_RELATION = FBB_COVERAGEAREA_RELATION;
        }

        public DropdownModel Handle(GetModelAndBrandQuery query)
        {
            IQueryable<decimal> cvr = from r in _FBB_COVERAGEAREA_RELATION.Get()
                                      where r.ACTIVEFLAG == "Y"
                                      select r.DSLAMID;

            return (from r in _FBB_DSLAM_INFO.Get()
                    join m in _FBB_DSLAMMODEL.Get() on r.DSLAMMODELID equals m.DSLAMMODELID
                    where !cvr.Contains(r.DSLAMID) && r.ACTIVEFLAG == "Y" && r.REGION_CODE == query.RegionCode && r.LOT_NUMBER == query.LotNo
                    && r.NODEID == null
                    orderby r.DSLAMID
                    select new DropdownModel
                    {
                        Value = m.MODEL,
                        Value2 = m.SH_BRAND,
                        ID = r.DSLAMID
                    }).FirstOrDefault();
        }
    }
}
