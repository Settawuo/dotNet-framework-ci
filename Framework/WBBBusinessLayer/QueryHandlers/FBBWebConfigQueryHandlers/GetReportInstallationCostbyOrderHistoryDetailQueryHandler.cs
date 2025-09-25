using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBData.Repository;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers
{
    public class GetReportInstallationCostbyOrderHistoryDetailQueryHandler : IQueryHandler<GetReportInstallationCostbyOrderHistoryDetailQuery, List<reportInstallOrderHistoryDetailModel>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<reportInstallOrderHistoryDetailModel> _objService;

        public GetReportInstallationCostbyOrderHistoryDetailQueryHandler(ILogger logger, IEntityRepository<reportInstallOrderHistoryDetailModel> objService)
        {
            _logger = logger;
            _objService = objService;
        }

        public List<reportInstallOrderHistoryDetailModel> Handle(GetReportInstallationCostbyOrderHistoryDetailQuery query)
        {
            try
            {
                var ret_code = new OracleParameter
                {
                    OracleDbType = OracleDbType.Decimal,
                    Direction = ParameterDirection.Output
                };

                var cur = new OracleParameter
                {
                    ParameterName = "cur",
                    OracleDbType = OracleDbType.RefCursor,
                    Direction = ParameterDirection.Output
                };

                var executeResult = _objService.ExecuteReadStoredProc(
                    "WBB.PKG_PAYG_INSTALL_COST_RPT.p_order_history_detail",
                    new
                    {
                        query.p_ORDER_NO,
                        query.p_ACCESS_NO,
                        ret_code,
                        cur
                    }).ToList();

                return executeResult;
            }
            catch (Exception ex)
            {
                _logger.Info(ex.Message);

                query.ret_code = "-1";
                query.ret_code = "PKG_FIXED_ASSET_LASTMILE.p_order_history_detail Error : " + ex.Message;

                return null;
            }
        }
    }
}
