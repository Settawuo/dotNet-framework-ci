using AIRNETEntity.Extensions;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.WebServices;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Models;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class CheckOrderPendingDeductionHandler : IQueryHandler<CheckOrderPendingDeductionQuery, CheckOrderPendingDeductionModel>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<string> _objService;

        public CheckOrderPendingDeductionHandler(ILogger logger, IWBBUnitOfWork uow, IEntityRepository<FBB_INTERFACE_LOG> intfLog, IEntityRepository<string> objService)
        {
            _logger = logger;
            _uow = uow;
            _intfLog = intfLog;
            _objService = objService;
        }

        public CheckOrderPendingDeductionModel Handle(CheckOrderPendingDeductionQuery query)
        {
            InterfaceLogCommand log = null;
            var data = new CheckOrderPendingDeductionModel();
            try
            {
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.TransactionId, "CheckOrderPendingCreate", "CheckOrderPendingCreateHandler", "", "FBB|" + query.FullUrl, "WEB");

                var p_transaction_id = new OracleParameter();
                p_transaction_id.ParameterName = "p_transaction_id";
                p_transaction_id.OracleDbType = OracleDbType.Varchar2;
                p_transaction_id.Direction = ParameterDirection.Input;
                p_transaction_id.Value = query.TransactionId;

                var p_order_transaction_id = new OracleParameter();
                p_order_transaction_id.ParameterName = "p_order_transaction_id";
                p_order_transaction_id.OracleDbType = OracleDbType.Varchar2;
                p_order_transaction_id.Direction = ParameterDirection.Input;
                p_order_transaction_id.Value = query.OrderTransactionId;

                var ret_code = new OracleParameter();
                ret_code.ParameterName = "ret_code";
                ret_code.OracleDbType = OracleDbType.Varchar2;
                ret_code.Size = 2000;
                ret_code.Direction = ParameterDirection.Output;

                var ret_message = new OracleParameter();
                ret_message.ParameterName = "ret_message";
                ret_message.OracleDbType = OracleDbType.Varchar2;
                ret_message.Size = 2000;
                ret_message.Direction = ParameterDirection.Output;

                var ret_ord_deduction = new OracleParameter();
                ret_ord_deduction.ParameterName = "ret_ord_deduction";
                ret_ord_deduction.OracleDbType = OracleDbType.Varchar2;
                ret_ord_deduction.Size = 2000;
                ret_ord_deduction.Direction = ParameterDirection.Output;

                var result = _objService.ExecuteStoredProc("WBB.PKG_FBB_REGISTER_PAYMENT.PROC_CHECK_ORD_DEDUCTION",
                     new
                     {
                         p_transaction_id = p_transaction_id,
                         p_order_transaction_id = p_order_transaction_id,
                         //  return code
                         ret_code = ret_code,
                         ret_message = ret_message,
                         ret_ord_deduction = ret_ord_deduction

                     });

                if (ret_code != null && ret_code.Value.ToSafeString() == "0")
                {
                    data.OrderDeduction = ret_ord_deduction.Value.ToSafeString();
                }

                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, data, log, ret_message.Value.ToSafeString(), string.Empty, "");

            }
            catch (Exception ex)
            {
                _logger.Info("CheckOrderPendingDeduction : Error.");
                _logger.Info(ex.Message);

                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, "", log, "Error", ex.StackTrace, "");

                throw;
            }

            return data;
        }
    }
}
