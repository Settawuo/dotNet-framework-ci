using WBBContract;
using WBBContract.Queries.ExWebServices;
using WBBEntity.PanelModels.ExWebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.ExWebServices.SAPInventory
{
    public class JoinOrderQueryHandler : IQueryHandler<JoinOrderQuery, JoinOrderResponse>
    {
        private readonly ILogger _logger;

        public JoinOrderQueryHandler(ILogger logger)
        {
            _logger = logger;
        }

        public JoinOrderResponse Handle(JoinOrderQuery model)
        {
            return null;
        }
    }
}
