using WBBContract;
using WBBContract.Queries.ExWebServices;
using WBBEntity.PanelModels.ExWebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.ExWebServices.SAPInventory
{
    public class InstallationCostQueryHandler : IQueryHandler<InstallationCostQuery, InstallationCostResponse>
    {
        private readonly ILogger _logger;

        public InstallationCostQueryHandler(ILogger logger)
        {
            _logger = logger;
        }

        public InstallationCostResponse Handle(InstallationCostQuery model)
        {
            return null;
        }
    }
}
