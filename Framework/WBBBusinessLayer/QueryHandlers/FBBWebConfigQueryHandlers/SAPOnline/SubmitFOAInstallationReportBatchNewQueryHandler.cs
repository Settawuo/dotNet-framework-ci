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
    public class SubmitFOAInstallationReportBatchNewQueryHandler : IQueryHandler<SubmitFOAInstallationReportBatchNewQuery, List<SubmitFOAInstallationNew>>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<SubmitFOAInstallationNew> _submitInstallLog;

        public SubmitFOAInstallationReportBatchNewQueryHandler(
            ILogger logger,
            IEntityRepository<SubmitFOAInstallationNew> submitInstallLog)
        {
            _logger = logger;
            _submitInstallLog = submitInstallLog;
        }

        public List<SubmitFOAInstallationNew> Handle(SubmitFOAInstallationReportBatchNewQuery query)
        {
            //string[] subStr = query.dateFrom.Split('/');
            //string dateFrom = subStr[0].ToSafeString() + subStr[1].ToSafeString() + subStr[2].ToSafeString();

            //subStr = query.dateTo.Split('/');
            //var dateTo = subStr[0].ToSafeString() + subStr[1].ToSafeString() + subStr[2].ToSafeString();

            var p_ret_code = new OracleParameter();
            p_ret_code.ParameterName = "ret_code";
            p_ret_code.OracleDbType = OracleDbType.Decimal;
            p_ret_code.Direction = ParameterDirection.Output;

            var p_install_cur = new OracleParameter();
            p_install_cur.ParameterName = "install_cur";
            p_install_cur.OracleDbType = OracleDbType.RefCursor;
            p_install_cur.Direction = ParameterDirection.Output;

            try
            {
                List<SubmitFOAInstallationNew> executeResult = _submitInstallLog.ExecuteReadStoredProc("WBB.P_FBB_GET_INS_BATCH",
                  new
                  {
                      //p_ORDER_NO = query.orderNo.ToSafeString(),
                      //p_INTERNET_NO = query.internetNo.ToSafeString(),
                      //p_PRODUCT_NAME = query.productName.ToSafeString(),
                      //p_SERVICE_NAME = query.serviceName.ToSafeString(),
                      //p_STATUS = query.status.ToSafeString(),
                      p_ORDER_FROM = query.dateFrom.ToSafeString(),
                      p_ORDER_TO = query.dateTo.ToSafeString(),

                      // return code
                      //0 = Success/Send, 1 = Error/Not Send, 2 = Success/Not Send
                      ret_code = p_ret_code,
                      cur = p_install_cur
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
