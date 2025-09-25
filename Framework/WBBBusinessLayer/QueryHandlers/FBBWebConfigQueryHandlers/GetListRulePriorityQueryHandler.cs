using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBData.Repository;
using WBBEntity.PanelModels.FBBWebConfigModels;
using WBBEntity.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Data;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Commands.FBBWebConfigCommands;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBData.DbIteration;
using WBBBusinessLayer.QueryHandlers;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers
{
    public class GetListRulePriorityQueryHandler : IQueryHandler<GetListRulePriorityQuery, ConfigurationRulePriorityView>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<object> _objService;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;

        public GetListRulePriorityQueryHandler(ILogger logger, IEntityRepository<object> objService ,
               IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IWBBUnitOfWork uow)
        {
            _logger = logger;
            _objService = objService;
            _intfLog = intfLog;
            _uow = uow;
        }

        public ConfigurationRulePriorityView Handle(GetListRulePriorityQuery query)
        {
            InterfaceLogCommand log = null;
            try
            {
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, "", "GetListRulePriorityQueryHandler", "GetListRulePriorityQueryHandler", "", "FBB", "WEB_CONFIG");
                ConfigRulePriorityResponse executeResults = new ConfigRulePriorityResponse();
                var returnForm = new ConfigurationRulePriorityView();

                var p_rule_name = new OracleParameter();
                p_rule_name.ParameterName = "p_rule_name";
                p_rule_name.Size = 2000;
                p_rule_name.OracleDbType = OracleDbType.Varchar2;
                p_rule_name.Direction = ParameterDirection.Input;
                p_rule_name.Value = query.RULE_NAME;

                var p_priority = new OracleParameter();
                p_priority.ParameterName = "p_priority";
                p_priority.Size = 2000;
                p_priority.OracleDbType = OracleDbType.BinaryFloat;
                p_priority.Direction = ParameterDirection.Input;
                p_priority.Value = query.PRIORITY;

                var p_lookup_name = new OracleParameter();
                p_lookup_name.ParameterName = "p_lookup_name";
                p_lookup_name.Size = 2000;
                p_lookup_name.OracleDbType = OracleDbType.Varchar2;
                p_lookup_name.Direction = ParameterDirection.Input;
                p_lookup_name.Value = query.LOOKUP_NAME;

                var p_effective_start = new OracleParameter();
                p_effective_start.ParameterName = "p_effective_start";
                p_effective_start.Size = 2000;
                p_effective_start.OracleDbType = OracleDbType.Varchar2;
                p_effective_start.Direction = ParameterDirection.Input;
                p_effective_start.Value = query.EFFECTIVE_DATE_START;

                var p_effective_end = new OracleParameter();
                p_effective_end.ParameterName = "p_effective_end";
                p_effective_end.Size = 2000;
                p_effective_end.OracleDbType = OracleDbType.Varchar2;
                p_effective_end.Direction = ParameterDirection.Input;
                p_effective_end.Value = query.EFFECTIVE_DATE_END;

                var ret_code = new OracleParameter();
                ret_code.ParameterName = "return_code";
                ret_code.OracleDbType = OracleDbType.BinaryFloat;
                ret_code.Direction = ParameterDirection.Output;

                var return_msg = new OracleParameter();
                return_msg.ParameterName = "return_msg";
                return_msg.OracleDbType = OracleDbType.Varchar2;
                return_msg.Size = 2000;
                return_msg.Direction = ParameterDirection.Output;

                var result_priority_search_cur = new OracleParameter
                {
                    ParameterName = "result_priority_search_cur",
                    OracleDbType = OracleDbType.RefCursor,
                    Direction = ParameterDirection.Output
                };

                var executeResult = _objService.ExecuteStoredProcMultipleCursor(
                        "wbb.pkg_fixed_asset_prioritylookup.p_get_config_priority",
                        new[]
                        {
                            p_rule_name,
                            p_priority,
                            p_lookup_name,
                            p_effective_start,
                            p_effective_end,
                            ret_code,
                            return_msg,
                            result_priority_search_cur
                        });

                if (executeResult != null)
                {
                    DataTable dtTableRespones = (DataTable)executeResult[2];
                    List<DataConfigRulePriorityTable> ListDataRulePriorityRespones = new List<DataConfigRulePriorityTable>();
                    ListDataRulePriorityRespones = dtTableRespones.DataTableToList<DataConfigRulePriorityTable>();
                    executeResults.result_priority_search_cur = ListDataRulePriorityRespones;
                    returnForm.dataConfigRulePriority = executeResults.result_priority_search_cur;
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, "", log, "Success", "", "");
                }
                return returnForm;
            }
            catch (Exception ex)
            {
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, "", log, "Failed ex", ex.Message, "");
                _logger.Info("Error call pkg_fixed_asset_prioritylookup.p_get_config_priority handles : " + ex.Message);
                return null;
            }


        }
    }
}
