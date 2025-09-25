using System.Collections.Generic;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries.SAPOnline;
using WBBData.Repository;
using WBBEntity.Models;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers
{
    public class GetBuildingCostInstallationQueryHandler : IQueryHandler<GetBuildingCostInstallationQuery, List<LovModel>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBBPAYG_STANDARD_ADDRESS> _building;

        public GetBuildingCostInstallationQueryHandler(ILogger logger, IEntityRepository<FBBPAYG_STANDARD_ADDRESS> building)
        {
            _logger = logger;
            _building = building;
        }

        public List<LovModel> Handle(GetBuildingCostInstallationQuery query)
        {
            if (query.SERVICE_TYPE == "")
            {
                query.SERVICE_TYPE = "XDSL";
            }
            if (query.TYPE == "")
            {
                query.TYPE = "Building";
            }

            string _service = query.SERVICE_TYPE ?? null;
            string _vendor = query.TYPE ?? null;

            int _ADDRESS_ID_count = (query.ADDRESS_ID == null) ? 0 : query.ADDRESS_ID.Count();
            var resultBuilding = (_ADDRESS_ID_count > 0) ? _building.Get().Where(b => query.ADDRESS_ID.Contains(b.ADDRESS_ID)) : _building.Get();
            List<LovModel> result = (from b in resultBuilding
                                     select new LovModel
                                     {
                                         LOV_NAME = b.BUILDING_NAME_TH,
                                         LOV_VAL1 = b.ADDRESS_ID
                                     }).ToList();
            return result;
        }
    }
}
