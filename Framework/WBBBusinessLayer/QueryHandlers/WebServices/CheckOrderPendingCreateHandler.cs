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
    public class CheckOrderPendingCreateHandler : IQueryHandler<CheckOrderPendingCreateQuery, CheckOrderPendingCreateModel>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<string> _objService;

        public CheckOrderPendingCreateHandler(ILogger logger, IWBBUnitOfWork uow, IEntityRepository<FBB_INTERFACE_LOG> intfLog, IEntityRepository<string> objService)
        {
            _logger = logger;
            _uow = uow;
            _intfLog = intfLog;
            _objService = objService;
        }

        public CheckOrderPendingCreateModel Handle(CheckOrderPendingCreateQuery query)
        {
            InterfaceLogCommand log = null;
            var data = new CheckOrderPendingCreateModel();
            try
            {
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow,
                    _intfLog,
                    query,
                    string.IsNullOrEmpty(query.InternetNo) ? query.OrderId : query.InternetNo,
                    "CheckOrderPendingCreate",
                    "CheckOrderPendingCreateHandler",
                    "",
                    "WBB",
                    query.UpdateBy);

                var p_order_id = new OracleParameter();
                p_order_id.ParameterName = "p_order_id";
                p_order_id.OracleDbType = OracleDbType.Varchar2;
                p_order_id.Direction = ParameterDirection.Input;
                p_order_id.Value = query.OrderId;

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

                var ret_ord_created = new OracleParameter();
                ret_ord_created.ParameterName = "ret_ord_created";
                ret_ord_created.OracleDbType = OracleDbType.Varchar2;
                ret_ord_created.Size = 2000;
                ret_ord_created.Direction = ParameterDirection.Output;


                var result = _objService.ExecuteStoredProc("WBB.PKG_FBB_REGISTER_PAYMENT.PROC_CHECK_ORD_CREATED",
                     new
                     {
                         p_order_id = p_order_id,
                         //  return code
                         ret_code = ret_code,
                         ret_message = ret_message,
                         ret_ord_created = ret_ord_created

                     });

                if (ret_code != null && ret_code.Value.ToSafeString() == "0")
                {
                    data.OrderCreated = ret_ord_created.Value.ToSafeString();
                }

                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, data, log, ret_message.Value.ToSafeString(), string.Empty, "");

            }
            catch (Exception ex)
            {
                _logger.Info("CheckOrderPendingCreate : Error.");
                _logger.Info(ex.Message);

                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, "", log, "Error", ex.StackTrace, "");

                throw;
            }

            return data;
        }
    }
}
