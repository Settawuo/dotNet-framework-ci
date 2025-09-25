using System;
using System.Collections.Generic;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBData.Repository;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers
{

    public class GetExportReportProblemsQueryHandler : IQueryHandler<GetExportReportProblemsQuery, List<ExportReportProblemsModel>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<ExportReportProblemsModel> _objService;

        public GetExportReportProblemsQueryHandler(ILogger logger, IEntityRepository<ExportReportProblemsModel> objService)
        {
            _logger = logger;
            _objService = objService;
        }

        public List<ExportReportProblemsModel> Handle(GetExportReportProblemsQuery command)
        {
            try
            {
                var stringQuery =
                    string.Format(
                        "SELECT * FROM TABLE( PKG_FBB_REPORT_PROBLEM.Export(iCreatedDateFrom => '{0}',iCreatedDateTo => '{1}', iSortColumn => '{2}', iAscDesc => '{3}', iProblemType => '{4}'))"
                        , (command.DateFrom != null) ? command.DateFrom.Value.Date.ToString("dd/MM/yyyy") : ""
                        , (command.DateTo != null) ? command.DateTo.Value.Date.ToString("dd/MM/yyyy") : ""
                        , command.SortColumn
                        , command.SortBy
                        , command.ProblemType);
                List<ExportReportProblemsModel> executeResult = _objService.SqlQuery(stringQuery).ToList();

                command.ReturnCode = "0";
                command.ReturnDesc = "Success";
                return executeResult;

            }
            catch (Exception ex)
            {
                _logger.Info(ex.Message);

                command.ReturnCode = "-1";
                command.ReturnDesc = "Error call save event service " + ex.Message;

                return null;
            }

        }
    }

}

