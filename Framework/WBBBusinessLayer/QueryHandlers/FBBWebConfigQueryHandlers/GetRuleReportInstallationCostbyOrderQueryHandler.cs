using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBData.Repository;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandler
{
    class GetRuleReportInstallationCostbyOrderQueryHandler : IQueryHandler<GetRuleReportInstallationCostbyOrderQuery, List<RuleReportInstallationCostbyOrderModel>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<RuleReportInstallationCostbyOrderModel> _objService;

        public GetRuleReportInstallationCostbyOrderQueryHandler(ILogger logger, IEntityRepository<RuleReportInstallationCostbyOrderModel> objService)
        {
            _logger = logger;
            _objService = objService;
        }

        public List<RuleReportInstallationCostbyOrderModel> Handle(GetRuleReportInstallationCostbyOrderQuery query)
        {
            try
            {
                var p_table_name = new OracleParameter();
                p_table_name.ParameterName = "p_table_name";
                p_table_name.Size = 2000;
                p_table_name.OracleDbType = OracleDbType.Varchar2;
                p_table_name.Direction = ParameterDirection.Input;
                p_table_name.Value = query.p_table_name;

                var ret_code = new OracleParameter();
                ret_code.ParameterName = "ret_code";
                ret_code.OracleDbType = OracleDbType.Decimal;
                ret_code.Direction = ParameterDirection.Output;

                var ret_msg = new OracleParameter();
                ret_msg.ParameterName = "ret_msg";
                ret_msg.OracleDbType = OracleDbType.Varchar2;
                ret_msg.Size = 2000;
                ret_msg.Direction = ParameterDirection.Output;

                var cur = new OracleParameter
                {
                    ParameterName = "cur",
                    OracleDbType = OracleDbType.RefCursor,
                    Direction = ParameterDirection.Output
                };

                var executeResult = _objService.ExecuteReadStoredProc(
                    "WBB.PKG_PAYG_INSTALL_COST_RPT.p_get_rule_id",
                    new
                    {
                        ret_code,
                        ret_msg,
                        cur,
                        p_table_name
                    }).ToList();

                return executeResult;
            }
            catch (Exception ex)
            {
                _logger.Info(ex.Message);

                query.ret_code = "-1";
                query.ret_code = "PKG_FIXED_ASSET_LASTMILE.p_get_rule_id Error : " + ex.Message;

                return null;
            }
        }
    }
}
