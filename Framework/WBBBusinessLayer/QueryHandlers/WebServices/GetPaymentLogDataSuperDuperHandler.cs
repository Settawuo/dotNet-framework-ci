using System;
using System.Linq;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.WebServices;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class GetPaymentLogDataSuperDuperHandler : IQueryHandler<GetPaymentLogDataSuperDuperQuery, GetPaymentLogDataSuperDuperModel>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<FBB_REGISTER_PAYMENT_LOG_SPDP> _objLspdp;

        public GetPaymentLogDataSuperDuperHandler(ILogger logger,
            IWBBUnitOfWork uow,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IEntityRepository<FBB_REGISTER_PAYMENT_LOG_SPDP> objLspdp)
        {
            _logger = logger;
            _uow = uow;
            _intfLog = intfLog;
            _objLspdp = objLspdp;
        }

        public GetPaymentLogDataSuperDuperModel Handle(GetPaymentLogDataSuperDuperQuery query)
        {
            InterfaceLogCommand log = null;
            var result = new GetPaymentLogDataSuperDuperModel();
            string StatusCode = "Failed";
            string StatusMessage = "Failed";
            try
            {
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.transaction_id,
                    "GetPaymentLogDataSuperDuper", "GetPaymentLogDataSuperDuperHandler", "", "FBB|" + query.Url, "");

                var spdp = _objLspdp.Get(x => x.TXN_ID == query.transaction_id).FirstOrDefault();
                if (spdp != null)
                {
                    result.DataLog = spdp;
                }

                StatusCode = "Success";
                StatusMessage = "Success";
            }
            catch (Exception ex)
            {
                StatusCode = "Failed";
                StatusMessage = ex.GetBaseException().ToString();
                _logger.Info(ex.GetErrorMessage());
            }
            finally
            {
                result.code = StatusCode;
                result.message = StatusMessage;
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, StatusCode, StatusMessage, "");
            }

            return result;
        }

    }
}