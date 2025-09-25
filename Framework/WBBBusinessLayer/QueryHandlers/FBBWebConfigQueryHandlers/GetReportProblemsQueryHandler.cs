using System;
using System.Collections.Generic;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBData.Repository;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers
{

    public class GetReportProblemsQueryHandler : IQueryHandler<GetReportProblemsQuery, List<ReportProblemsModel>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<ReportProblemsModel> _objService;

        public GetReportProblemsQueryHandler(ILogger logger, IEntityRepository<ReportProblemsModel> objService)
        {
            _logger = logger;
            _objService = objService;
        }

        public List<ReportProblemsModel> Handle(GetReportProblemsQuery query)
        {
            try
            {
                var stringQuery =
                    string.Format(
                        "SELECT * FROM TABLE( PKG_FBB_REPORT_PROBLEM.Search(iCreatedDateFrom => '{0}',iCreatedDateTo => '{1}', iSortColumn => '{2}', iAscDesc => '{3}', iProblemType => '{4}', iPageNo => '{5}', iRecordsPerPage => '{6}'))"
                        , (query.DateFrom != null) ? query.DateFrom.Value.Date.ToString("dd/MM/yyyy") : ""
                        , (query.DateTo != null) ? query.DateTo.Value.Date.ToString("dd/MM/yyyy") : ""
                        , query.SortColumn
                        , query.SortBy
                        , query.ProblemType
                        , query.PageNo
                        , query.RecordsPerPage);
                List<ReportProblemsModel> executeResult = _objService.SqlQuery(stringQuery).ToList();

                query.ReturnCode = "0";
                query.ReturnDesc = "Success";
                return executeResult;

            }
            catch (Exception ex)
            {
                _logger.Info(ex.Message);

                query.ReturnCode = "-1";
                query.ReturnDesc = "PKG_FBB_REPORT_PROBLEM.Search() Error : " + ex.Message;

                return null;
            }

        }
    }

}
