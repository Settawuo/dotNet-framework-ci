using System;
using System.Collections.Generic;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries.Report;
using WBBData.Repository;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers.Report
{
    public class GetExportLogInterfaceQueryHandler : IQueryHandler<GetExportLogInterfaceQuery, List<LogInterfaceModel>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<LogInterfaceModel> _interfaceLog;

        public GetExportLogInterfaceQueryHandler(ILogger logger, IEntityRepository<LogInterfaceModel> interfaceLog)
        {
            _logger = logger;
            _interfaceLog = interfaceLog;
        }

        public List<LogInterfaceModel> Handle(GetExportLogInterfaceQuery query)
        {
            var inTransactionId = query.IN_TRANSACTION_ID ?? "";
            var methodName = query.METHOD_NAME ?? "";
            var serviceName = query.SERVICE_NAME ?? "";
            var inIdCardNo = query.IN_ID_CARD_NO ?? "";
            var sortColumn = query.SortColumn ?? "";
            var sortBy = query.SortBy ?? "";
            try
            {
                var stringQuery =
                    string.Format(
                        "SELECT * FROM TABLE( PKG_FBB_REPORT_LOG_INTERFACE.Export(iInTransactionId => '{0}', iMethodName => '{1}', iServiceName => '{2}', iInIdCardNo => '{3}', iCreatedDateFrom => '{4}',iCreatedDateTo => '{5}', iSortColumn => '{6}', iAscDesc => '{7}'))"
                        , inTransactionId
                        , methodName
                        , serviceName
                        , inIdCardNo
                        , (query.CREATE_DATE_FROM != null) ? query.CREATE_DATE_FROM.Value.Date.ToString("dd/MM/yyyy") : ""
                        , (query.CREATE_DATE_TO != null) ? query.CREATE_DATE_TO.Value.Date.ToString("dd/MM/yyyy") : ""
                        , sortColumn
                        , sortBy);
                List<LogInterfaceModel> executeResult = _interfaceLog.SqlQuery(stringQuery).ToList();

                query.ReturnCode = "0";
                query.ReturnDesc = "Success";
                return executeResult;

            }
            catch (Exception ex)
            {
                _logger.Info(ex.Message);

                query.ReturnCode = "-1";
                query.ReturnDesc = "PKG_FBB_REPORT_LOG_INTERFACE.Export() Error : " + ex.Message;

                return null;
            }
        }
    }
}
