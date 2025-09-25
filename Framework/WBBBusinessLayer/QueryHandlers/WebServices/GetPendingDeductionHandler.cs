using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using WBBContract;
using WBBContract.Queries.WebServices;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class GetPendingDeductionHandler : IQueryHandler<GetPendingDeductionQuery, GetPendingDeductionModel>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<object> _objService;
        public GetPendingDeductionHandler(ILogger logger, IWBBUnitOfWork uow, IEntityRepository<FBB_INTERFACE_LOG> intfLog, IEntityRepository<object> objService)
        {
            _logger = logger;
            _uow = uow;
            _intfLog = intfLog;
            _objService = objService;
        }

        public GetPendingDeductionModel Handle(GetPendingDeductionQuery query)
        {
            var log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, !string.IsNullOrEmpty(query.p_mobile_no) ? query.p_mobile_no : query.p_transaction_id, "GetPendingDeduction", "GetPendingDeductionHandler", null, "FBB", "");

            GetPendingDeductionModel results = new GetPendingDeductionModel();
            try
            {
                var p_transaction_id = new OracleParameter();
                p_transaction_id.ParameterName = "p_transaction_id";
                p_transaction_id.Size = 2000;
                p_transaction_id.OracleDbType = OracleDbType.Varchar2;
                p_transaction_id.Direction = ParameterDirection.Input;
                p_transaction_id.Value = query.p_transaction_id;

                var p_mobile_no = new OracleParameter();
                p_mobile_no.ParameterName = "p_mobile_no";
                p_mobile_no.Size = 2000;
                p_mobile_no.OracleDbType = OracleDbType.Varchar2;
                p_mobile_no.Direction = ParameterDirection.Input;
                p_mobile_no.Value = query.p_mobile_no;

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

                var list_order_paending_deduction = new OracleParameter();
                list_order_paending_deduction.ParameterName = "list_order_paending_deduction";
                list_order_paending_deduction.OracleDbType = OracleDbType.RefCursor;
                list_order_paending_deduction.Direction = ParameterDirection.Output;

                var result = _objService.ExecuteStoredProcMultipleCursor("WBB.PKG_FBB_REGISTER_PAYMENT.PROC_LIST_PENDING_DEDUCTION",
                    new object[]
                    {
                        p_transaction_id,
                        p_mobile_no,
                         //return code
                         ret_code,
                         ret_message,
                         list_order_paending_deduction
                    });

                results.ret_code = result[0] != null ? result[0].ToSafeString() : "-1";
                results.ret_message = result[1] != null ? result[1].ToSafeString() : "error";
                if (results.ret_code != "-1")
                {
                    DataTable data1 = (DataTable)result[2];
                    results.orderPaendingDeductionDatas = data1.DataTableToList<OrderPaendingDeduction>();
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
            }
            return results;
        }
    }
}
