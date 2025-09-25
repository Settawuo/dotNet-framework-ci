using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Queries.WebServices;
using WBBData.Repository;
using WBBEntity.Models;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class GetOwnerByBuildingHandler : IQueryHandler<GetOwnerByBuildingQuery, OwnerByBuildingModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_COVERAGE_REGION> _FBB_COVERAGE_REGION;

        public GetOwnerByBuildingHandler(ILogger logger, IEntityRepository<FBB_COVERAGE_REGION> FBB_COVERAGE_REGION)
        {
            _logger = logger;
            _FBB_COVERAGE_REGION = FBB_COVERAGE_REGION;
        }

        public OwnerByBuildingModel Handle(GetOwnerByBuildingQuery query)
        {
            var result = GetOwnerByBuildingHelper.GetOwnerByBuilding(_logger, _FBB_COVERAGE_REGION, query);
            return result;
        }
    }

    public static class GetOwnerByBuildingHelper
    {
        public static OwnerByBuildingModel GetOwnerByBuilding(ILogger logger,
            IEntityRepository<FBB_COVERAGE_REGION> FBB_COVERAGE_REGION,
            GetOwnerByBuildingQuery query)
        {
            if (query.LanguageFlag == "1")
            {
                var a = from c in FBB_COVERAGE_REGION.Get()
                        where c.ACTIVEFLAG == "Y" && c.TOWER_TH == query.Building
                        select new OwnerByBuildingModel
                        {
                            OWNER_PRODUCT = c.OWNER_PRODUCT,
                            LATITUDE = c.LATITUDE,
                            LONGITUDE = c.LONGITUDE
                        };

                if (a.Any())

                    return a.FirstOrDefault();
            }
            else
            {
                var a = from c in FBB_COVERAGE_REGION.Get()
                        where c.ACTIVEFLAG == "Y" && c.TOWER_EN == query.Building
                        select new OwnerByBuildingModel
                        {
                            OWNER_PRODUCT = c.OWNER_PRODUCT,
                            LATITUDE = c.LATITUDE,
                            LONGITUDE = c.LONGITUDE
                        };

                if (a.Any())
                    return a.FirstOrDefault();
            }

            return new OwnerByBuildingModel();
        }
    }
}
