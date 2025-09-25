using System;
using WBBContract;
using WBBContract.Queries.ExWebServices;
using WBBEntity.PanelModels.ExWebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.ExWebServices
{
    public class NewRegisterQueryHandler : IQueryHandler<NewRegisterQuery, NewRegisterResponse>
    {
        private readonly ILogger _logger;

        public NewRegisterQueryHandler(ILogger logger)
        {
            _logger = logger;
        }

        public NewRegisterResponse Handle(NewRegisterQuery model)
        {
            try
            {
                var request = new SFFServices.SffRequest();
                NewRegisterResponse result = new NewRegisterResponse();
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
