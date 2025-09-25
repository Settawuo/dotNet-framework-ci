using WBBBusinessLayer.QueryHandlers.ExWebServices.FbbCpGw;
using WBBContract;
using WBBContract.Queries.Commons.Master;
using WBBData.Repository;
using WBBEntity.Models;

namespace WBBBusinessLayer.QueryHandlers.Commons.Master
{
    public class GetMappingSbnOwnerHandler : IQueryHandler<GetMappingSbnOwnerProd, string>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_CFG_LOV> _lov;

        public GetMappingSbnOwnerHandler(ILogger logger, IEntityRepository<FBB_CFG_LOV> lov)
        {
            _logger = logger;
            _lov = lov;
        }

        public string Handle(GetMappingSbnOwnerProd query)
        {
            return GetPackageListHelper.AirnetWfAccessModeMapper(_logger, _lov, query);
        }
    }
}
