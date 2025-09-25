using System;
using System.Collections.Generic;
using System.Linq;
using WBBContract;
using WBBContract.Queries.WebServices;
using WBBData.Repository;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class GetConfigurationReportQueryHandler : IQueryHandler<GetConfigurationReportQuery, List<ConfigurationReportModel>>
    {

        private readonly IEntityRepository<ConfigurationReportModel> _objService;
        public GetConfigurationReportQueryHandler(IEntityRepository<ConfigurationReportModel> objService)
        {
            _objService = objService;
        }

        public List<ConfigurationReportModel> Handle(GetConfigurationReportQuery query)
        {
            try
            {
                var stringQuery =
                    string.Format(
                        "SELECT * FROM TABLE( PKG_FBB_AUTOMAIL_REPORT.Search(iReportName => '{0}',iScheduler => '{1}', iSortColumn => '{2}', iAscDesc => '{3}', iPageNo => '{4}', iRecordsPerPage => '{5}'))"
                        , query.ReportName
                        , query.Scheduler
                        , query.SortColumn
                        , query.SortBy
                        , query.PageNo
                        , query.RecordsPerPage);
                List<ConfigurationReportModel> executeResult = _objService.SqlQuery(stringQuery).ToList();

                query.ReturnCode = "0";
                query.ReturnDesc = "Success";
                return executeResult;

            }
            catch (Exception ex)
            {
                query.ReturnCode = "-1";
                query.ReturnDesc = "PKG_FBB_AUTOMAIL_REPORT.Search() Error : " + ex.Message;

                return new List<ConfigurationReportModel>();
            }

        }
    }
}
