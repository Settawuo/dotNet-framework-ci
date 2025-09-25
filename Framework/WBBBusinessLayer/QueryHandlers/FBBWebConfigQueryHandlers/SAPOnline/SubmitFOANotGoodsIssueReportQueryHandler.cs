using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries.SAPOnline;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers.SAPOnline
{
    class SubmitFOANotGoodsIssueReportQueryHandler : IQueryHandler<SubmitFOANotGoodsIssueReportQuery, List<SubmitFOAEquipment>>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<SubmitFOAEquipmentReport> _submitOrderLog;
        private readonly IEntityRepository<SubmitFOAEquipment> _submitEquipmentLog;

        public SubmitFOANotGoodsIssueReportQueryHandler(
            ILogger logger,
            IEntityRepository<SubmitFOAEquipmentReport> submitOrderLog,
            IEntityRepository<SubmitFOAEquipment> submitEquipmentLog)
        {
            _logger = logger;
            _submitOrderLog = submitOrderLog;
            _submitEquipmentLog = submitEquipmentLog;
        }

        public List<SubmitFOAEquipment> Handle(SubmitFOANotGoodsIssueReportQuery query)
        {
            var p_ret_code = new OracleParameter();
            p_ret_code.ParameterName = "ret_code";
            p_ret_code.OracleDbType = OracleDbType.Decimal;
            p_ret_code.Direction = ParameterDirection.Output;

            var p_equip_cur = new OracleParameter();
            p_equip_cur.ParameterName = "notgoodsissue_cur";
            p_equip_cur.OracleDbType = OracleDbType.RefCursor;
            p_equip_cur.Direction = ParameterDirection.Output;

            try
            {
                List<SubmitFOAEquipment> executeResult = _submitEquipmentLog.ExecuteReadStoredProc("WBB.PKG_FBB_FOA_ORDER_MANAGEMENT.P_GET_ASSET_NOTGOODSISSUE",
                  new
                  {
                      // return code
                      //0 = Success/Send, 1 = Error/Not Send, 2 = Success/Not Send
                      ret_code = p_ret_code,
                      cur = p_equip_cur
                  }).ToList();

                return executeResult;
            }
            catch (Exception ex)
            {
                _logger.Info(ex.GetErrorMessage());
                return null;
            }
        }
    }
}
