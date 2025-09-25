using System;
using System.Collections.Generic;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries.CoverageManagePage;
using WBBData.Repository;
using WBBEntity.Models;
using WBBEntity.PanelModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers.CoverageManagePage
{
    public class GetDSLAMAndExistingHandler : IQueryHandler<GetDSLAMAndExistingQuery, List<DropdownModel>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_DSLAM_INFO> _FBB_DSLAM_INFO;
        private readonly IEntityRepository<FBB_DSLAMMODEL> _FBB_DSLAMMODEL;
        private readonly IEntityRepository<FBB_COVERAGEAREA_RELATION> _FBB_COVERAGEAREA_RELATION;

        public GetDSLAMAndExistingHandler(ILogger logger, IEntityRepository<FBB_DSLAM_INFO> FBB_DSLAM_INFO,
        IEntityRepository<FBB_DSLAMMODEL> FBB_DSLAMMODEL, IEntityRepository<FBB_COVERAGEAREA_RELATION> FBB_COVERAGEAREA_RELATION)
        {
            _logger = logger;
            _FBB_DSLAM_INFO = FBB_DSLAM_INFO;
            _FBB_DSLAMMODEL = FBB_DSLAMMODEL;
            _FBB_COVERAGEAREA_RELATION = FBB_COVERAGEAREA_RELATION;
        }

        public List<DropdownModel> Handle(GetDSLAMAndExistingQuery query)
        {
            if (query.Existing)
            {
                return (from r in _FBB_DSLAM_INFO.Get().ToList()
                        join m in _FBB_DSLAMMODEL.Get() on r.DSLAMMODELID equals m.DSLAMMODELID
                        where r.CVRID == query.CVRID && r.ACTIVEFLAG == "Y" && m.ACTIVEFLAG == "Y"
                        select new DropdownModel
                        {
                            Text = r.NODEID,
                            Value = m.MODEL + "," + m.BRAND + "," + r.DSLAMID.ToString()
                        }).ToList();
            }
            else
            {
                IQueryable<decimal> cvr = from r in _FBB_COVERAGEAREA_RELATION.Get()
                                          where r.ACTIVEFLAG == "Y"
                                          select r.DSLAMID;

                return (from r in _FBB_DSLAM_INFO.Get()
                        where !cvr.Contains(r.DSLAMID) && r.ACTIVEFLAG == "Y" && r.NODEID == null && r.REGION_CODE == query.RegionCode
                        group r by new { r.REGION_CODE, r.LOT_NUMBER } into g
                        select new DropdownModel
                        {
                            Text = g.Key.LOT_NUMBER,
                            Value = g.Key.LOT_NUMBER
                        }).ToList();
            }

        }
    }
}
