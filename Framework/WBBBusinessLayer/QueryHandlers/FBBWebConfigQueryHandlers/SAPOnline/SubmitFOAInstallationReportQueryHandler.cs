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

/* change history
 *ch0001 29/01/2020 --Get data revalue-- validate Accessno  แทนการ validate date from and date to
 */

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers.SAPOnline
{
    public class SubmitFOAInstallationReportQueryHandler : IQueryHandler<SubmitFOAInstallationReportQuery, List<SubmitFOAInstallation>>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<SubmitFOAInstallationReport> _submitOrderLog;
        private readonly IEntityRepository<SubmitFOAInstallation> _submitInstallLog;

        public SubmitFOAInstallationReportQueryHandler(
            ILogger logger,
            IEntityRepository<SubmitFOAInstallationReport> submitOrderLog,
            IEntityRepository<SubmitFOAInstallation> submitInstallLog)
        {
            _logger = logger;
            _submitOrderLog = submitOrderLog;
            _submitInstallLog = submitInstallLog;
        }

        public List<SubmitFOAInstallation> Handle(SubmitFOAInstallationReportQuery query)
        {
            //ch0001 start
            //string[] subStr = query.dateFrom.Split('/');
            //string dateFrom = subStr[0].ToSafeString() + subStr[1].ToSafeString() + subStr[2].ToSafeString();

            //subStr = query.dateTo.Split('/');
            //var dateTo = subStr[0].ToSafeString() + subStr[1].ToSafeString() + subStr[2].ToSafeString();

            string[] subStr = null;

            string dateFrom = "";
            string dateTo = "";

            if (query.dateFrom != "" && query.dateTo != "")
            {
                subStr = query.dateFrom.Split('/');
                dateFrom = subStr[0].ToSafeString() + subStr[1].ToSafeString() + subStr[2].ToSafeString();

                subStr = query.dateTo.Split('/');
                dateTo = subStr[0].ToSafeString() + subStr[1].ToSafeString() + subStr[2].ToSafeString();
            }

            //ch0001 end


            var p_ret_code = new OracleParameter();
            p_ret_code.ParameterName = "ret_code";
            p_ret_code.OracleDbType = OracleDbType.Decimal;
            p_ret_code.Direction = ParameterDirection.Output;

            var p_install_cur = new OracleParameter();
            p_install_cur.ParameterName = "cur";
            p_install_cur.OracleDbType = OracleDbType.RefCursor;
            p_install_cur.Direction = ParameterDirection.Output;

            try
            {
                List<SubmitFOAInstallation> executeResult = _submitInstallLog.ExecuteReadStoredProc("WBB.PKG_FBB_FOA_ORDER_MANAGEMENT.P_GET_INSTALL",
                  new
                  {
                      p_ORDER_NO = query.orderNo.ToSafeString(),
                      p_INTERNET_NO = query.internetNo.ToSafeString(),
                      p_PRODUCT_NAME = query.productName.ToSafeString(),
                      p_SERVICE_NAME = query.serviceName.ToSafeString(),
                      p_STATUS = query.status.ToSafeString(),
                      p_ORDER_FROM = dateFrom,
                      p_ORDER_TO = dateTo,
                      p_PRODUCT_OWNER = query.productOwner.ToSafeString(),
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
