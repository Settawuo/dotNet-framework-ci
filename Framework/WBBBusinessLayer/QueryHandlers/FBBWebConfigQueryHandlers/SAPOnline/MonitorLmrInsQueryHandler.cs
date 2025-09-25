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
    public class MonitorLmrInsQueryHandler : IQueryHandler<MonitorLmrInsQuery, List<SubmitFOAInstallation>>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<SubmitFOAInstallationReport> _submitOrderLog;
        private readonly IEntityRepository<SubmitFOAInstallation> _submitInstallLog;

        public MonitorLmrInsQueryHandler(
            ILogger logger,
            IEntityRepository<SubmitFOAInstallationReport> submitOrderLog,
            IEntityRepository<SubmitFOAInstallation> submitInstallLog)
        {
            _logger = logger;
            _submitOrderLog = submitOrderLog;
            _submitInstallLog = submitInstallLog;
        }

        public List<SubmitFOAInstallation> Handle(MonitorLmrInsQuery query)
        {

            var p_ret_code = new OracleParameter();
            p_ret_code.ParameterName = "ret_code";
            p_ret_code.OracleDbType = OracleDbType.Decimal;
            p_ret_code.Direction = ParameterDirection.Output;

            var cur_install_lmr = new OracleParameter();
            cur_install_lmr.ParameterName = "cur_install_lmr";
            cur_install_lmr.OracleDbType = OracleDbType.RefCursor;
            cur_install_lmr.Direction = ParameterDirection.Output;

            try
            {
                List<SubmitFOAInstallation> executeResult = _submitInstallLog.ExecuteReadStoredProc("WBB.PKG_FBB_FOA_ORDER_MANAGEMENT.p_get_install_monitor_lmr",
                  new
                  {
                      p_ORDER_FROM = query.dateFrom.ToSafeString(),
                      p_ORDER_TO = query.dateTo.ToSafeString(),

                      // return code
                      //0 = Success/Send, 1 = Error/Not Send, 2 = Success/Not Send
                      ret_code = p_ret_code,
                      cur = cur_install_lmr
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
