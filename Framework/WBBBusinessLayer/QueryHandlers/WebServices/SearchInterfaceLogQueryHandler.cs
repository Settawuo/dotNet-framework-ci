using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using WBBContract;
using WBBContract.Queries.WebServices;
using WBBData.Repository;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class SearchInterfaceLogQueryHandler : IQueryHandler<SearchInterfaceLogQuery, SearchInterfaceLogData>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<SearchInterfaceLog> _repositoryInterfaceLog;

        public SearchInterfaceLogQueryHandler(ILogger logger,
            IEntityRepository<SearchInterfaceLog> repositoryInterfaceLog)
        {
            _logger = logger;
            _repositoryInterfaceLog = repositoryInterfaceLog;
        }

        public SearchInterfaceLogData Handle(SearchInterfaceLogQuery query)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            SearchInterfaceLogData executeResult;

            try
            {
                var stringQuery =
                    string.Format(
                        "SELECT * FROM TABLE( PKG_FBB_REPORT_INTERFACE_LOG.Search(idatefrom => '{0}'" +
                        ",idateto => '{1}'" +
                        ",iintransactionid => '{2}'" +
                        ",imethodname => '{3}'" +
                        ",iinidcardno => '{4}'" +
                        ",isortcolumn => '{5}'" +
                        ",iascdesc => '{6}'" +
                        ",ipageno => '{7}'" +
                        ",irecordsperpage => '{8}'))"
                        , query.DateFrom
                        , query.DateTo
                        , query.TransactionId
                        , query.MethodName
                        , query.IdCardNo
                        , query.SortColumn
                        , query.OrderBy
                        , query.PageNo
                        , query.RecordsPerPage);

                var result = _repositoryInterfaceLog.SqlQuery(stringQuery).ToList();

                executeResult = new SearchInterfaceLogData
                {
                    InterfaceLogData = result,
                    ReturnCode = "0",
                    ReturnMessage = "Success"
                };
            }
            catch (Exception ex)
            {
                _logger.Info(ex.Message);

                executeResult = new SearchInterfaceLogData
                {
                    ReturnCode = "-1",
                    ReturnMessage = "SearchInterfaceLog Error :  " + ex.Message
                };
            }

            return executeResult;
        }
    }

    public class InterfaceLogMethodNameQueryHandler : IQueryHandler<GetInterfaceLogMethodNameQuery, List<string>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<MethodNameLog> _repositoryInterfaceLog;

        public InterfaceLogMethodNameQueryHandler(ILogger logger,
            IEntityRepository<MethodNameLog> repositoryInterfaceLog)
        {
            _logger = logger;
            _repositoryInterfaceLog = repositoryInterfaceLog;
        }

        public List<string> Handle(GetInterfaceLogMethodNameQuery query)
        {
            var searchInterfaceLogData = new List<string>();
            try
            {

                var stringQuery =
                   string.Format("SELECT * FROM TABLE(PKG_FBB_REPORT_INTERFACE_LOG.GetMethodName)");

                var result = _repositoryInterfaceLog.SqlQuery(stringQuery);
                searchInterfaceLogData = result.Select(item => item.METHOD_NAME).ToList();
            }
            catch (Exception ex)
            {
                _logger.Info(ex.Message);
            }
            return searchInterfaceLogData;
        }
    }
}
