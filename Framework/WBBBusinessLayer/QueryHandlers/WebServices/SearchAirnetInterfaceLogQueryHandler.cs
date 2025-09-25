using AIRNETEntity.PanelModels;
using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using WBBContract;
using WBBContract.Queries.WebServices;
using WBBData.Repository;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    class SearchAirnetInterfaceLogQueryHandler : IQueryHandler<SearchAirnetInterfaceLogQuery, SearchAirnetInterfaceLogData>
    {
        private readonly ILogger _logger;
        private readonly IAirNetEntityRepository<SearchAirnetInterfaceLog> _repositoryAirInterfacelog;

        public SearchAirnetInterfaceLogQueryHandler(ILogger logger,
            IAirNetEntityRepository<SearchAirnetInterfaceLog> repositoryAirInterfacelog)
        {
            _logger = logger;
            _repositoryAirInterfacelog = repositoryAirInterfacelog;
        }

        public SearchAirnetInterfaceLogData Handle(SearchAirnetInterfaceLogQuery query)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            SearchAirnetInterfaceLogData executeResult;
            try
            {
                var stringQuery =
                    string.Format(
                        "SELECT * FROM TABLE( PKG_AIR_REPORT_INTERFACE_LOG.Search(idatefrom => '{0}'" +
                        ",idateto => '{1}'" +
                        ",iorderno => '{2}'" +
                        ",isortcolumn => '{3}'" +
                        ",iascdesc => '{4}'" +
                        ",ipageno => '{5}'" +
                        ",irecordsperpage => '{6}'))"
                        , query.DateFrom
                        , query.DateTo
                        , query.OrderNo
                        , query.SortColumn
                        , query.OrderBy
                        , query.PageNo
                        , query.RecordsPerPage);

                var result = _repositoryAirInterfacelog.SqlQuery(stringQuery).ToList();

                executeResult = new SearchAirnetInterfaceLogData
                {
                    AirInterfaceLogData = result,
                    ReturnCode = "0",
                    ReturnMessage = "Success"
                };
            }
            catch (Exception ex)
            {
                _logger.Info(ex.Message);

                executeResult = new SearchAirnetInterfaceLogData
                {
                    ReturnCode = "-1",
                    ReturnMessage = "SearchInterfaceLog Error :  " + ex.Message
                };
            }

            return executeResult;
        }
    }
}
