using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
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
    public class GetConfigReqPaymentHandler : IQueryHandler<GetConfigReqPaymentQuery, GetConfigReqPaymentModel>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<object> _objService;

        public GetConfigReqPaymentHandler(ILogger logger, IWBBUnitOfWork uow, IEntityRepository<FBB_INTERFACE_LOG> intfLog, IEntityRepository<object> objService)
        {
            _logger = logger;
            _uow = uow;
            _intfLog = intfLog;
            _objService = objService;
        }

        public GetConfigReqPaymentModel Handle(GetConfigReqPaymentQuery query)
        {
            var log = new InterfaceLogCommand();
            GetConfigReqPaymentModel results = new GetConfigReqPaymentModel();

            try
            {
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow,
                    _intfLog,
                    query,
                    string.IsNullOrEmpty(query.p_non_mobile_no) ? query.p_transaction_id : query.p_non_mobile_no,
                    "GetConfigReqPayment",
                    "GetConfigReqPaymentHandler",
                    null,
                    "FBB",
                    "");

                var p_product_name = new OracleParameter();
                p_product_name.ParameterName = "p_product_name";
                p_product_name.Size = 2000;
                p_product_name.OracleDbType = OracleDbType.Varchar2;
                p_product_name.Direction = ParameterDirection.Input;
                p_product_name.Value = query.p_product_name;

                var p_service_name = new OracleParameter();
                p_service_name.ParameterName = "p_service_name";
                p_service_name.Size = 2000;
                p_service_name.OracleDbType = OracleDbType.Varchar2;
                p_service_name.Direction = ParameterDirection.Input;
                p_service_name.Value = query.p_service_name;

                var p_transaction_id = new OracleParameter();
                p_transaction_id.ParameterName = "p_transaction_id";
                p_transaction_id.Size = 2000;
                p_transaction_id.OracleDbType = OracleDbType.Varchar2;
                p_transaction_id.Direction = ParameterDirection.Input;
                p_transaction_id.Value = query.p_transaction_id;

                var ret_code = new OracleParameter();
                ret_code.ParameterName = "ret_code";
                ret_code.Size = 2000;
                ret_code.OracleDbType = OracleDbType.Varchar2;
                ret_code.Direction = ParameterDirection.Output;

                var ret_message = new OracleParameter();
                ret_message.ParameterName = "ret_message";
                ret_message.Size = 2000;
                ret_message.OracleDbType = OracleDbType.Varchar2;
                ret_message.Direction = ParameterDirection.Output;

                var list_config_req_payment = new OracleParameter();
                list_config_req_payment.ParameterName = "list_config_req_payment";
                list_config_req_payment.OracleDbType = OracleDbType.RefCursor;
                list_config_req_payment.Direction = ParameterDirection.Output;

                var list_req_payment_metadata = new OracleParameter();
                list_req_payment_metadata.ParameterName = "list_req_payment_metadata";
                list_req_payment_metadata.OracleDbType = OracleDbType.RefCursor;
                list_req_payment_metadata.Direction = ParameterDirection.Output;

                var list_req_payment_3ds = new OracleParameter();
                list_req_payment_3ds.ParameterName = "list_req_payment_3ds";
                list_req_payment_3ds.OracleDbType = OracleDbType.RefCursor;
                list_req_payment_3ds.Direction = ParameterDirection.Output;

                var result = _objService.ExecuteStoredProcMultipleCursor("WBB.PKG_FBB_REGISTER_PAYMENT.PROC_LIST_CONFIG_REQ_PAYMENT",
                    new object[]
                    {
                        p_product_name,
                        p_service_name,
                        p_transaction_id,
                         //return
                        ret_code,
                        ret_message,
                        list_config_req_payment,
                        list_req_payment_metadata,
                        list_req_payment_3ds
                    });

                results.ret_code = result[0] != null ? result[0].ToSafeString() : "-1";
                results.ret_message = result[1] != null ? result[1].ToSafeString() : "error";
                if (results.ret_code != "-1")
                {
                    DataTable data1 = (DataTable)result[2];
                    results.list_config_req_payment = data1.DataTableToList<ConfigReqPaymentData>();
                    DataTable data2 = (DataTable)result[3];
                    results.list_req_payment_metadata = data2.DataTableToList<ConfigReqPaymentData>();
                    DataTable data3 = (DataTable)result[4];
                    results.list_req_payment_3ds = data3.DataTableToList<ConfigReqPaymentData>();
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, results, log, "Success", "", "");
                }
                else
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, results, log, "Failed", results.ret_message, "");
                }

            }
            catch (Exception ex)
            {
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex.Message, log, "Failed", ex.Message, "");
                results.ret_code = "-1";
                results.ret_message = "Error";

                return null;
            }
            return results;
        }
    }
}
