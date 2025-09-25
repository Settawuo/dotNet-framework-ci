using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Commands.WebServices;
using WBBContract.Queries.WebServices;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.WebServiceModels;
using WBBEntity.PanelModels.WebServices;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class NotifySuperDuperHandler : IQueryHandler<NotifySuperDuperQuery, NotifySuperDuperModel>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<string> _objService;
        private readonly IEntityRepository<OrderPendingPaymentModel> _orderCreateReository;
        private readonly IQueryHandler<CreateOrderMeshQuery, CreateOrderMeshModel> _createOrderMeshHandler;
        private readonly IQueryHandler<CreateOrderSCPEQuery, CreateOrderSCPEModel> _createOrderScpeHandler;

        public NotifySuperDuperHandler(ILogger logger, IWBBUnitOfWork uow, IEntityRepository<FBB_INTERFACE_LOG> intfLog, IEntityRepository<string> objService, IEntityRepository<OrderPendingPaymentModel> orderCreateReository, IQueryHandler<CreateOrderMeshQuery, CreateOrderMeshModel> createOrderMeshHandler, IQueryHandler<CreateOrderSCPEQuery, CreateOrderSCPEModel> createOrderScpeHandler)
        {
            _logger = logger;
            _uow = uow;
            _intfLog = intfLog;
            _objService = objService;
            _orderCreateReository = orderCreateReository;
            _createOrderMeshHandler = createOrderMeshHandler;
            _createOrderScpeHandler = createOrderScpeHandler;
        }

        public NotifySuperDuperModel Handle(NotifySuperDuperQuery query)
        {
            InterfaceLogCommand log = null;
            var result = new NotifySuperDuperModel();
            try
            {
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.TransactionId, "NotifySuperDuper", "NotifySuperDuperHandler", "", "FBB|" + query.FullUrl, "");

                var channel = query.DataResult.channel_type.ToSafeString();
                var txn_id = query.DataResult.txn_id.ToSafeString();
                var order_transaction_id = query.DataResult.order_id.ToSafeString();
                var status = query.DataResult.status.ToSafeString();
                var status_code = query.DataResult.status_code.ToSafeString();
                var status_message = query.DataResult.status_message.ToSafeString();

                var command = new SavePaymentSPDPLogCommand()
                {
                    p_action = "New",
                    p_service_name = "WebHook Notify",
                    p_user_name = "WebHook Notify",
                    p_txn_id = txn_id,
                    p_status = status,
                    p_status_code = status_code,
                    p_status_message = status_message,
                    p_channel = channel,
                    p_order_transaction_id = order_transaction_id,
                };
                InterfaceLogServiceHelper.SavePaymentSPDPLog(_objService, command, query);

                //1.Get data จาก Transaction ID (TXN_ID) 
                //var resSpDpLog = GetPaymentOldDataSuperDuper(query);
                var orderCreateList = GetPendingOrderPayment(txn_id);
                if (orderCreateList != null && orderCreateList.Count() > 0)
                {
                    //2: function Verify Create type Mesh or SCPE
                    var orderCreate = orderCreateList.FirstOrDefault();
                    CreateOrder(orderCreate, channel, txn_id, query.FullUrl);
                }

                result.RESULT_CODE = "0";
                result.RESULT_DESC = "Success";
                return result;
            }
            catch (Exception ex)
            {
                result.RESULT_CODE = "-1";
                result.RESULT_DESC = ex.GetErrorMessage();
                _logger.Info("Error call NotifySuperDuperHandler : " + ex.GetErrorMessage());
                return result;
            }
            finally
            {
                var resultLog = (result ?? new NotifySuperDuperModel()).RESULT_CODE == "0" ? "Success" : "Failed";
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, resultLog, result.RESULT_DESC, "");
            }
        }

        private List<OrderPendingPaymentModel> GetPendingOrderPayment(string txn_id)
        {
            var data = new List<OrderPendingPaymentModel>();
            try
            {
                var p_transaction_id = new OracleParameter();
                p_transaction_id.ParameterName = "p_transaction_id";
                p_transaction_id.OracleDbType = OracleDbType.Varchar2;
                p_transaction_id.Direction = ParameterDirection.Input;
                p_transaction_id.Value = txn_id;

                var ret_code = new OracleParameter();
                ret_code.ParameterName = "ret_code";
                ret_code.OracleDbType = OracleDbType.Varchar2;
                ret_code.Size = 2000;
                ret_code.Direction = ParameterDirection.Output;

                var ret_msgerr = new OracleParameter();
                ret_msgerr.ParameterName = "ret_message";
                ret_msgerr.OracleDbType = OracleDbType.Varchar2;
                ret_msgerr.Size = 2000;
                ret_msgerr.Direction = ParameterDirection.Output;

                var ioResults = new OracleParameter();
                ioResults.ParameterName = "list_order_pending_spdp";
                ioResults.OracleDbType = OracleDbType.RefCursor;
                ioResults.Direction = ParameterDirection.Output;

                List<OrderPendingPaymentModel> executeResult = _orderCreateReository.ExecuteReadStoredProc("WBB.PKG_FBB_REGISTER_PAYMENT.PROC_LIST_ORD_PENDING_SPDP",
                    new
                    {
                        p_transaction_id = p_transaction_id,
                        //  return code
                        ret_code = ret_code,
                        ret_message = ret_msgerr,

                        list_order_paending_payment = ioResults
                    }).ToList();

                if (ret_code.Value.ToSafeString() != "0")
                {
                    throw new Exception(ret_msgerr.Value.ToSafeString());
                }
                else
                {
                    if (executeResult != null && executeResult.Count > 0)
                    {
                        data = executeResult;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Info("GetPendingOrderPayment : Error.");
                _logger.Info(ex.Message);
            }

            return data;

        }

        private void CreateOrder(OrderPendingPaymentModel order, string channel, string txn_id, string FullUrl)
        {
            if (order.product_name == "TOPUP_MESH")
            {
                var resultMesh = _createOrderMeshHandler.Handle(new CreateOrderMeshQuery()
                {
                    Channel = channel,
                    InternetNo = order.non_mobile_no,
                    OrderId = order.order_id,
                    txn_id = txn_id,
                    FullUrl = FullUrl
                });

                if (resultMesh.RESULT_CODE != "0")
                {
                    //TODO: Rollback payment status
                    var commandRollback = new SavePaymentSPDPLogCommand()
                    {
                        p_action = "Rollback",
                        p_service_name = "WebHook Notify",
                        p_user_name = "WebHook Notify Rollback",
                        p_order_id = order.order_id
                    };
                    InterfaceLogServiceHelper.SavePaymentSPDPLog(_objService, commandRollback, string.Empty);
                }
            }
            else if (order.product_name == "SELL_ROUTER")
            {
                var resultScpe = _createOrderScpeHandler.Handle(new CreateOrderSCPEQuery()
                {
                    Channel = channel,
                    OrderId = order.order_id,
                    txn_id = txn_id,
                    FullUrl = FullUrl
                });
                if (resultScpe.RESULT_CODE != "0")
                {
                    //TODO: Rollback payment status
                    var commandRollback = new SavePaymentSPDPLogCommand()
                    {
                        p_action = "Rollback",
                        p_service_name = "WebHook Notify",
                        p_user_name = "WebHook Notify Rollback",
                        p_order_id = order.order_id
                    };
                    InterfaceLogServiceHelper.SavePaymentSPDPLog(_objService, commandRollback, string.Empty);
                }
            }
        }
    }
}