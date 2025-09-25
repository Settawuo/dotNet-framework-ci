using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries.SAPOnline;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.PanelModels.FBBWebConfigModels;
namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers.SAPOnline
{
    class FBBReconcileInvRPTBatchQueryHandler : IQueryHandler<FBBReconcileInvRPTBatchQuery, FBBReconcileInvRPTBatchModel>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBBReconcileInvRPTBatchModel> _submitEquipmentLog;

        public FBBReconcileInvRPTBatchQueryHandler(
            ILogger logger,
            IEntityRepository<FBBReconcileInvRPTBatchModel> submitEquipmentLog)
        {
            _logger = logger;
            _submitEquipmentLog = submitEquipmentLog;
        }

        public FBBReconcileInvRPTBatchModel Handle(FBBReconcileInvRPTBatchQuery query)
        {
            FBBReconcileInvRPTBatchModel executeResults = new FBBReconcileInvRPTBatchModel();

            var p_ret_code = new OracleParameter();
            p_ret_code.ParameterName = "ret_code";
            p_ret_code.OracleDbType = OracleDbType.Int64;
            p_ret_code.Direction = ParameterDirection.Output;

            var p_ret_msg = new OracleParameter();
            p_ret_msg.ParameterName = "ret_msg";
            p_ret_msg.Size = 2000;
            p_ret_msg.OracleDbType = OracleDbType.Varchar2;
            p_ret_msg.Direction = ParameterDirection.Output;

            var p_cur = new OracleParameter();
            p_cur.ParameterName = "cur";
            p_cur.OracleDbType = OracleDbType.RefCursor;
            p_cur.Direction = ParameterDirection.Output;

            var p_cur2 = new OracleParameter();
            p_cur2.ParameterName = "cur2";
            p_cur2.OracleDbType = OracleDbType.RefCursor;
            p_cur2.Direction = ParameterDirection.Output;

            try
            {
                _logger.Info("Start WBB.PKG_FBBRECONCILE_INVRPT.P_CHECK_OM_FOA");

                var result = _submitEquipmentLog.ExecuteStoredProcMultipleCursor("WBB.PKG_FBBRECONCILE_INVRPT.P_CHECK_OM_FOA",
                       new object[]
                  {
                      // return code
                      //0 = Success/Send, 1 = Error/Not Send, 2 = Success/Not Send
                        p_ret_code,
                        p_ret_msg,
                        p_cur,
                        p_cur2
                  });

                executeResults.ret_code = result[0] != null ? result[0].ToString() : "-1";
                executeResults.ret_msg = result[1].ToString();
                DataTable cur = (DataTable)result[2];
                List<FBBReconcileInvRPTBatchModel_MSG> p_list_cur = cur.DataTableToList<FBBReconcileInvRPTBatchModel_MSG>();
                executeResults.cur = p_list_cur;

                DataTable cur2 = (DataTable)result[3];
                List<FBBReconcileInvRPTBatchModel_Cur2> p_list_cur2 = cur2.DataTableToList<FBBReconcileInvRPTBatchModel_Cur2>();
                executeResults.cur2 = p_list_cur2;
                _logger.Info("End  " + executeResults.ret_code);

                return executeResults;

            }
            catch (Exception ex)
            {
                _logger.Info("Error " + ex.GetErrorMessage());
                return null;
            }
        }
    }
}
