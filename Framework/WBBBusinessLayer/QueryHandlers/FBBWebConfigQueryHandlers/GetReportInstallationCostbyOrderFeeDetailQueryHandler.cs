using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers
{
    public class GetReportInstallationCostbyOrderFeeDetailQueryHandler : IQueryHandler<GetReportInstallationCostbyOrderFeeDetailQuery, List<reportInstallOrderFeeDetailModel>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<reportInstallOrderFeeDetailModel> _objService;

        public GetReportInstallationCostbyOrderFeeDetailQueryHandler(ILogger logger, IEntityRepository<reportInstallOrderFeeDetailModel> objService)
        {
            _logger = logger;
            _objService = objService;
        }
        public List<reportInstallOrderFeeDetailModel> Handle(GetReportInstallationCostbyOrderFeeDetailQuery query)
        {
            try
            {
                var ret_code = new OracleParameter
                {
                    OracleDbType = OracleDbType.Decimal,
                    Direction = ParameterDirection.Output
                };              
                var ret_cursor = new OracleParameter
                {
                    ParameterName = "ret_cursor",
                    OracleDbType = OracleDbType.RefCursor,
                    Direction = ParameterDirection.Output
                };


                _logger.Info("Start GetReportInstallationCostbyOrderFeeDetailQueryHandler Call  : ");
                var executeResult = _objService.ExecuteReadStoredProc("WBB.PKG_PAYG_INSTALL_COST_RPT.p_order_fee_detail",
                new
                {
                    //In
                    query.p_ORDER_NO,
                    query.p_ACCESS_NO,

                    //Out
                    ret_cursor,
                    ret_code,

                }).ToList();

                _logger.Info("End GetReportInstallationCostbyOrderFeeDetailQueryHandler Call  : ");

                return executeResult;

            }
            catch (Exception ex)
            {
                _logger.Info(ex.Message);

                query.ret_code = "-1";
                query.ret_code = "p_order_fee_detail Error : " + ex.Message;

                _logger.Info("Error GetReportInstallationCostbyOrderFeeDetailQueryHandler Call  : " + ex.Message);
                return null;
            }
        }
    }
}
