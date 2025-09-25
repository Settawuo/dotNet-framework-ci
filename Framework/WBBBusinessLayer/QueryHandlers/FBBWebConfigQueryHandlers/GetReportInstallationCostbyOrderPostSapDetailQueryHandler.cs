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
    public class GetReportInstallationCostbyOrderPostSapDetailQueryHandler : IQueryHandler<GetReportInstallationCostbyOrderPostSapDetailQuery, List<reportInstallPostSapDetail>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<reportInstallPostSapDetail> _objService;

        public GetReportInstallationCostbyOrderPostSapDetailQueryHandler(ILogger logger, IEntityRepository<reportInstallPostSapDetail> objService)
        {
            _logger = logger;
            _objService = objService;
        }

        public List<reportInstallPostSapDetail> Handle(GetReportInstallationCostbyOrderPostSapDetailQuery query)
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
                    "WBB.PKG_PAYG_INSTALL_COST_RPT.p_post_sap_detail",
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
                query.ret_code = "PKG_FIXED_ASSET_LASTMILE.p_post_sap_detail Error : " + ex.Message;

                return null;
            }
        }
    }
}
