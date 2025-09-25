using System.Collections.Generic;
using System.Linq;
using WBBContract;
using WBBContract.Queries.Commons.Master;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels;

namespace WBBBusinessLayer.QueryHandlers.Commons.Master
{
    public class GetCoverageAreaRelQueryHandler : IQueryHandler<GetCoverageAreaRelQuery, List<CoverageRelValueModel>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_COVERAGEAREA> _coverageService;
        private readonly IEntityRepository<FBB_COVERAGEAREA_RELATION> _coverageRelService;

        public GetCoverageAreaRelQueryHandler(ILogger logger,
            IEntityRepository<FBB_COVERAGEAREA> coverageService,
            IEntityRepository<FBB_COVERAGEAREA_RELATION> coverageRelService)
        {
            _logger = logger;
            _coverageService = coverageService;
            _coverageRelService = coverageRelService;
        }

        public List<CoverageRelValueModel> Handle(GetCoverageAreaRelQuery query)
        {
            return GetCoverageAreaRelHelper.GetCoverageAreaRel(_coverageService, _coverageRelService, query);
        }
    }

    public static class GetCoverageAreaRelHelper
    {
        public static List<CoverageRelValueModel> GetCoverageAreaRel(
            IEntityRepository<FBB_COVERAGEAREA> coverageService,
            IEntityRepository<FBB_COVERAGEAREA_RELATION> coverageRelService,
            GetCoverageAreaRelQuery query)
        {
            var coverageIds = query.CurrentCulture.IsThaiCulture() ?
                coverageService.Get(c => c.NODENAME_TH.Equals(query.NodeName) && c.ACTIVEFLAG == "Y" && c.NODESTATUS == "ON_SITE")
                .Select(c => c.CVRID).ToList() :
                coverageService.Get(c => c.NODENAME_EN.Equals(query.NodeName) && c.ACTIVEFLAG == "Y" && c.NODESTATUS == "ON_SITE")
                .Select(c => c.CVRID).ToList();

            var data = coverageRelService
                .Get(cr => coverageIds.Contains(cr.CVRID) && cr.ACTIVEFLAG == "Y");

            var coverageRelValueModel = data
            .Select(cr => new CoverageRelValueModel
            {
                CvrId = cr.CVRID,
                DslamId = cr.DSLAMID,
                TowerNameEn = cr.TOWERNAME_EN,
                TowerNameTh = cr.TOWERNAME_TH,

                Latitute = cr.LATITUDE,
                Longitude = cr.LONGITUDE,
            });

            if (query.CurrentCulture.IsThaiCulture())
                return coverageRelValueModel.DistinctBy(c => c.TowerNameTh).ToList();
            else
                return coverageRelValueModel.DistinctBy(c => c.TowerNameEn).ToList();
        }
    }
}
