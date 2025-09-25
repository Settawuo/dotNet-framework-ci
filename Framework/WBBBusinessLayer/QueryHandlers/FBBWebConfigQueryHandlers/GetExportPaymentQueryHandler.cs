using System;
using System.Collections.Generic;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBData.Repository;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers
{

    public class GetExportPaymentQueryHandler : IQueryHandler<GetExportPaymentQuery, List<ExportPaymentModel>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<ExportPaymentModel> _objService;

        public GetExportPaymentQueryHandler(ILogger logger, IEntityRepository<ExportPaymentModel> objService)
        {
            _logger = logger;
            _objService = objService;
        }

        public List<ExportPaymentModel> Handle(GetExportPaymentQuery command)
        {
            try
            {
                var stringQuery =
                    string.Format(
                        "SELECT * FROM TABLE( PKG_FBB_PAYMENT.Export(iCreatedDateFrom => '{0}',iCreatedDateTo => '{1}',iInternetNo => '{2}', iSortColumn => '{3}', iAscDesc => '{4}'))"
                        , (command.DateFrom != null) ? command.DateFrom.Value.Date.ToString("dd/MM/yyyy") : ""
                        , (command.DateTo != null) ? command.DateTo.Value.Date.ToString("dd/MM/yyyy") : ""
                        , command.InternetNo
                        , command.SortColumn
                        , command.SortBy);
                List<ExportPaymentModel> executeResult = _objService.SqlQuery(stringQuery).ToList();

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
