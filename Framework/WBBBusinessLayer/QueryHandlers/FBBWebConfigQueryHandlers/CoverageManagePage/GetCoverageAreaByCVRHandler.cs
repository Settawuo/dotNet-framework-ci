using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries.CoverageManagePage;
using WBBData.Repository;
using WBBEntity.Models;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers.CoverageManagePage
{
    public class GetCoverageAreaByCVRHandler : IQueryHandler<GetCoverageAreaByCVRQuery, CoverageAreaModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_COVERAGEAREA> _coverageArea;
        private readonly IEntityRepository<FBB_CFG_LOV> _lov;

        public GetCoverageAreaByCVRHandler(ILogger logger, IEntityRepository<FBB_COVERAGEAREA> coverageArea, IEntityRepository<FBB_CFG_LOV> lov)
        {
            _logger = logger;
            _coverageArea = coverageArea;
            _lov = lov;
        }

        public CoverageAreaModel Handle(GetCoverageAreaByCVRQuery query)
        {
            var a = (from r in _coverageArea.Get()
                     where r.CVRID == query.CVRID
                     select new CoverageAreaModel
                     {
                         CVRID = r.CVRID,
                         ContactID = r.CONTACT_ID,
                         BuildingCode = r.BUILDINGCODE,
                         RegionCode = r.REGION_CODE,
                         CondoCode = r.LOCATIONCODE,
                         IPRANCode = r.IPRAN_CODE
                     }).FirstOrDefault();

            var l = (from r in _lov.Get()
                     where r.LOV_NAME == a.RegionCode && r.LOV_TYPE == "REGION_CODE" && r.ACTIVEFLAG == "Y"
                     select r).FirstOrDefault();

            a.Region = l.DISPLAY_VAL;
            a.RegionLOV = l.LOV_VAL1;

            return a;
        }

    }
}
