using System;
using System.Collections.Generic;
using System.Linq;
using WBBContract;
using WBBContract.Queries.WebServices;
using WBBData.Repository;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers
{
    public class GetConfigurationReportByIdQueryHandlers : IQueryHandler<GetConfigurationReportByIdQuery, ConfigurationReportModel>
    {
        private readonly IEntityRepository<ConfigurationReportModel> _objService;
        private readonly IEntityRepository<ConfigurationReportQueryModel> _objServiceQuery;
        public GetConfigurationReportByIdQueryHandlers(IEntityRepository<ConfigurationReportModel> objService, IEntityRepository<ConfigurationReportQueryModel> objServiceQuery)
        {
            _objService = objService;
            _objServiceQuery = objServiceQuery;
        }

        public ConfigurationReportModel Handle(GetConfigurationReportByIdQuery query)
        {
            try
            {
                var stringQuery =
                    string.Format(
                        "SELECT * FROM TABLE( PKG_FBB_AUTOMAIL_REPORT.GetListConfigReport(iReportId => '{0}'))"
                        , query.ReportId
                        );
                ConfigurationReportModel executeResult = _objService.SqlQuery(stringQuery).FirstOrDefault();

                var strQuery = string.Format(
                        "SELECT * FROM TABLE( PKG_FBB_AUTOMAIL_REPORT.GetQueryReport(iReportId => '{0}'))"
                        , query.ReportId
                        );
                List<ConfigurationReportQueryModel> listQuery = _objServiceQuery.SqlQuery(strQuery).ToList();

                if (executeResult != null)
                {
                    executeResult.ConfigurationQueryList = new List<ConfigurationReportQueryModel>();
                    foreach (var item in listQuery)
                    {
                        executeResult.ConfigurationQueryList.Add(item);
                    }
                }


                return executeResult;

            }
            catch (Exception ex)
            {
                return null;
            }

        }
    }
}
