using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Queries.ExWebServices;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.ExWebServices.SAPOnline
{
    public class MonitorLmrQueryHandler : IQueryHandler<MonitorLmrQuery, List<SubmitFOAEquipment>>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<SubmitFOAEquipmentReport> _submitOrderLog;
        private readonly IEntityRepository<SubmitFOAEquipment> _submitEquipmentLog;
        public MonitorLmrQueryHandler(
              ILogger logger,
            IEntityRepository<SubmitFOAEquipmentReport> submitOrderLog,
            IEntityRepository<SubmitFOAEquipment> submitEquipmentLog)
        {
            _logger = logger;
            _submitOrderLog = submitOrderLog;
            _submitEquipmentLog = submitEquipmentLog;
        }
        public List<SubmitFOAEquipment> Handle(MonitorLmrQuery query)
        {
            var p_ret_code = new OracleParameter();
            p_ret_code.ParameterName = "ret_code";
            p_ret_code.OracleDbType = OracleDbType.Decimal;
            p_ret_code.Direction = ParameterDirection.Output;

            var cur_equip_lmr = new OracleParameter();
            cur_equip_lmr.ParameterName = "cur_equip_lmr";
            cur_equip_lmr.OracleDbType = OracleDbType.RefCursor;
            cur_equip_lmr.Direction = ParameterDirection.Output;

            try
            {
                List<SubmitFOAEquipment> executeResult = _submitEquipmentLog.ExecuteReadStoredProc("WBB.PKG_FBB_FOA_ORDER_MANAGEMENT.p_get_equip_monitor_lmr",
                  new
                  {
                      p_ORDER_FROM = query.dateFrom.ToSafeString(),
                      p_ORDER_TO = query.dateTo.ToSafeString(),

                      // return code
                      //0 = Success/Send, 1 = Error/Not Send, 2 = Success/Not Send
                      ret_code = p_ret_code,
                      cur = cur_equip_lmr
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
