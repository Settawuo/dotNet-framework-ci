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
    public class GetRegisterPendingDeductionHandler : IQueryHandler<GetRegisterPendingDeductionQuery, GetRegisterPendingDeductionModel>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<FBB_REGISTER_PENDING_DEDUCTION> _objDeduc;

        public GetRegisterPendingDeductionHandler(ILogger logger,
            IWBBUnitOfWork uow,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IEntityRepository<FBB_REGISTER_PENDING_DEDUCTION> objDeduc)
        {
            _logger = logger;
            _uow = uow;
            _intfLog = intfLog;
            _objDeduc = objDeduc;
        }

        public GetRegisterPendingDeductionModel Handle(GetRegisterPendingDeductionQuery query)
        {
            InterfaceLogCommand log = null;
            var result = new GetRegisterPendingDeductionModel();
            string StatusCode = "Failed";
            string StatusMessage = "Failed";
            try
            {
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.transaction_id,
                    "GetRegisterPendingDeduction", "GetRegisterPendingDeductionHandler", "", "FBB|" + query.Url, "");

                var deduc = _objDeduc.Get(x => x.TRANSACTION_ID == query.transaction_id).FirstOrDefault();
                if (deduc != null)
                {
                    result.Data = deduc;
                }

                StatusCode = "Success";
                StatusMessage = "";
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