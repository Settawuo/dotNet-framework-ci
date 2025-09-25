using System;
using WBBContract;
using WBBContract.Queries.ExWebServices;
using WBBEntity.PanelModels.ExWebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.ExWebServices.SAPInventory
{
    public class TerminateServiceQueryHandler : IQueryHandler<TerminateServiceQuery, TerminateServiceResponse>
    {
        private readonly ILogger _logger;

        public TerminateServiceQueryHandler(ILogger logger)
        {
            _logger = logger;
        }

        public TerminateServiceResponse Handle(TerminateServiceQuery model)
        {
            try
            {
                var request = new SFFServices.SffRequest();
                TerminateServiceResponse result = new TerminateServiceResponse();
                //code
                return result;
            }
            catch (Exception ex)
            {
                _logger.Info("SAPRegisterResponse - exeption message: " + ex.Message + " ,Inner Exeption: " + ex.InnerException);
            }

            return null;
        }
    }
}
