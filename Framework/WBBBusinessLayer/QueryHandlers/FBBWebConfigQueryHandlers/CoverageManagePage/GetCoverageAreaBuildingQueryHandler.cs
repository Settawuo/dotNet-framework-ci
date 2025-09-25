using System;
using System.Collections.Generic;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries.CoverageManagePage;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers.CoverageManagePage
{
    public class GetCoverageAreaBuildingQueryHandler : IQueryHandler<GetCoverageAreaBuildingQuery, List<BuildingPanel>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_COVERAGEAREA_BUILDING> _building;
        private readonly IEntityRepository<FBB_COVERAGEAREA> _coverageArea;

        public GetCoverageAreaBuildingQueryHandler(ILogger logger, IEntityRepository<FBB_COVERAGEAREA_BUILDING> building,
            IEntityRepository<FBB_COVERAGEAREA> coverageArea)
        {
            _logger = logger;
            _building = building;
            _coverageArea = coverageArea;
        }
        public List<BuildingPanel> Handle(GetCoverageAreaBuildingQuery query)
        {
            var listBuildingPanel = new List<BuildingPanel>();
            try
            {
                #region sql
                //select b.building,b.building_th,b.building_en 
                //from wbb.fbb_coveragearea_building b
                //where b.contact_id = '@contact_id'
                //order by b.building
                #endregion

                //select Building from fbb_coveragearea_building where contact_id = 1 
                //and building not in (select buildingcode from fbb_coveragearea  where contact_id = 1) 
                //and active_flag = 'Y'
                //order by building

                if (query.NotIn == true)
                {
                    var buildingCode = (from r in _coverageArea.Get()
                                        where r.CONTACT_ID == query.ContactId && r.ACTIVEFLAG == "Y"
                                        select r.BUILDINGCODE);

                    return (from r in _building.Get()
                            where r.CONTACT_ID == query.ContactId &&
                            !buildingCode.Contains(r.BUILDING) && r.ACTIVE_FLAG == "Y"
                            orderby r.BUILDING
                            select new BuildingPanel
                            {
                                Text = r.BUILDING,
                                Value = r.BUILDING
                            }).ToList();
                }
                else
                {
                    var building = from b in _building.Get()
                                   where b.CONTACT_ID == query.ContactId
                                   && b.ACTIVE_FLAG == "Y"
                                   orderby b.BUILDING
                                   select new BuildingPanel()
                                   {
                                       ContactId = b.CONTACT_ID,
                                       Tower = b.BUILDING,
                                       TowerTH = b.BUILDING_TH,
                                       TowerEN = b.BUILDING_EN,
                                       InstallNote = b.INSTALL_NOTE,
                                       UpdateDate = b.UPDATED_DATE
                                   };

                    if (building.Any())
                        listBuildingPanel = building.ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.GetErrorMessage());
            }

            return listBuildingPanel;
        }
    }
}
