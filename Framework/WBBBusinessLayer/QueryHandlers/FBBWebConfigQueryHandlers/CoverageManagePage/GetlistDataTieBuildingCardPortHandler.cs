using System.Collections.Generic;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries.CoverageManagePage;
using WBBData.Repository;
using WBBEntity.Models;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers.CoverageManagePage
{



    public class GetlistDataTieBuildingCardPortHandler : IQueryHandler<GetBulidingCardPortTie, List<BuailTieQuery>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_COVERAGEAREA_RELATION> _FBB_COVERAGEAREA_RELATION;
        private readonly IEntityRepository<FBB_COVERAGEAREA_BUILDING> _FBB_COVERAGEAREA_BUILDING;
        private readonly IEntityRepository<FBB_COVERAGEAREA> _FBB_COVERAGEAREA;


        public GetlistDataTieBuildingCardPortHandler(ILogger logger,
         IEntityRepository<FBB_COVERAGEAREA_RELATION> FBB_COVERAGEAREA_RELATION,
           IEntityRepository<FBB_COVERAGEAREA_BUILDING> FBB_COVERAGEAREA_BUILDING,
            IEntityRepository<FBB_COVERAGEAREA> FBB_COVERAGEAREA)
        {
            _logger = logger;
            _FBB_COVERAGEAREA_RELATION = FBB_COVERAGEAREA_RELATION;
            _FBB_COVERAGEAREA_BUILDING = FBB_COVERAGEAREA_BUILDING;
            _FBB_COVERAGEAREA = FBB_COVERAGEAREA;

        }



        public List<BuailTieQuery> Handle(GetBulidingCardPortTie query)
        {
            List<BuailTieQuery> result;

            var coveragearea_buildingquery = (from coveragearea in _FBB_COVERAGEAREA.Get()
                                              join coveragearea_building in _FBB_COVERAGEAREA_BUILDING.Get()
                                              on coveragearea.CONTACT_ID equals coveragearea_building.CONTACT_ID
                                              join coveragearea_relation in _FBB_COVERAGEAREA_RELATION.Get()
                                              on coveragearea.CVRID equals coveragearea_relation.CVRID
                                              where coveragearea_relation.DSLAMID == query.DSALMID
                                              && coveragearea_relation.ACTIVEFLAG == "Y" && coveragearea_relation.ACTIVEFLAG == "Y"
                                              && coveragearea.ACTIVEFLAG == "Y" && coveragearea_building.ACTIVE_FLAG == "Y"
                                              && coveragearea_relation.TOWERNAME_EN == coveragearea_building.BUILDING_EN
                                              orderby coveragearea_building.BUILDING ascending

                                              select new BuailTieQuery()
                                              {
                                                  building = coveragearea_building.BUILDING,
                                                  Dispay = coveragearea_building.BUILDING


                                              }).Distinct();

            if (coveragearea_buildingquery.Any())
            {
                result = coveragearea_buildingquery.ToList();

                return result;
            }
            else
            {
                result = null;

                return result;
            }



        }
    }
}
