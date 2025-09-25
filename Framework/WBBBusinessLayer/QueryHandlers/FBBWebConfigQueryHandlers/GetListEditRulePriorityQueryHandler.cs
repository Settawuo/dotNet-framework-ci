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

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers
{
    
    public class GetListEditRulePriorityQueryHandler : IQueryHandler<GetListEditRulePriorityQuery, ConfigurationEditRulePriorityView>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<object> _objService;

        public GetListEditRulePriorityQueryHandler(ILogger logger, IEntityRepository<object> objService)
        {
            _logger = logger;
            _objService = objService;
        }

        public ConfigurationEditRulePriorityView Handle(GetListEditRulePriorityQuery query)
        {

            try
            {
                ConfigEditRulePriorityResponse executeResults = new ConfigEditRulePriorityResponse();
                var returnForm = new ConfigurationEditRulePriorityView();

                var p_rule_id = new OracleParameter();
                p_rule_id.ParameterName = "p_rule_id";
                p_rule_id.Size = 2000;
                p_rule_id.OracleDbType = OracleDbType.Varchar2;
                p_rule_id.Direction = ParameterDirection.Input;
                p_rule_id.Value = query.RULE_ID;

                var ret_rule_id = new OracleParameter();
                ret_rule_id.ParameterName = "ret_rule_id";
                ret_rule_id.Size = 2000;
                ret_rule_id.OracleDbType = OracleDbType.Varchar2;
                ret_rule_id.Direction = ParameterDirection.Output;

                var ret_rule_name = new OracleParameter();
                ret_rule_name.ParameterName = "ret_rule_name";
                ret_rule_name.Size = 2000;
                ret_rule_name.OracleDbType = OracleDbType.Varchar2;
                ret_rule_name.Direction = ParameterDirection.Output;

                var ret_priority = new OracleParameter();
                ret_priority.ParameterName = "ret_priority";
                ret_priority.Size = 2000;
                ret_priority.OracleDbType = OracleDbType.BinaryFloat;
                ret_priority.Direction = ParameterDirection.Output;

                var ret_lmr_flag = new OracleParameter();
                ret_lmr_flag.ParameterName = "ret_lmr_flag";
                ret_lmr_flag.Size = 2000;
                ret_lmr_flag.OracleDbType = OracleDbType.Varchar2;
                ret_lmr_flag.Direction = ParameterDirection.Output;

                var ret_lookup_name = new OracleParameter();
                ret_lookup_name.ParameterName = "ret_lookup_name";
                ret_lookup_name.Size = 2000;
                ret_lookup_name.OracleDbType = OracleDbType.Varchar2;
                ret_lookup_name.Direction = ParameterDirection.Output;

                var ret_lookup_parameter = new OracleParameter();
                ret_lookup_parameter.ParameterName = "ret_lookup_parameter";
                ret_lookup_parameter.Size = 2000;
                ret_lookup_parameter.OracleDbType = OracleDbType.Varchar2;
                ret_lookup_parameter.Direction = ParameterDirection.Output;

                var ret_rule_param_id = new OracleParameter();
                ret_rule_param_id.ParameterName = "ret_rule_param_id";
                ret_rule_param_id.Size = 2000;
                ret_rule_param_id.OracleDbType = OracleDbType.Varchar2;
                ret_rule_param_id.Direction = ParameterDirection.Output;

                var ret_effective_date_start = new OracleParameter();
                ret_effective_date_start.ParameterName = "ret_effective_date_start";
                ret_effective_date_start.Size = 2000;
                ret_effective_date_start.OracleDbType = OracleDbType.Varchar2;
                ret_effective_date_start.Direction = ParameterDirection.Output;

                var ret_effective_date_end = new OracleParameter();
                ret_effective_date_end.ParameterName = "ret_effective_date_end";
                ret_effective_date_end.Size = 2000;
                ret_effective_date_end.OracleDbType = OracleDbType.Varchar2;
                ret_effective_date_end.Direction = ParameterDirection.Output;

                var return_code = new OracleParameter();
                return_code.ParameterName = "return_code";
                return_code.OracleDbType = OracleDbType.BinaryFloat;
                return_code.Direction = ParameterDirection.Output;

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
                        "wbb.pkg_fixed_asset_prioritylookup.p_get_condition_priority",
                        new[]
                        {
                            p_rule_id,
                            ret_rule_id,
                            ret_rule_name,
                            ret_priority,
                            ret_lmr_flag,
                            ret_lookup_name,
                            ret_lookup_parameter,
                            ret_rule_param_id,
                            ret_effective_date_start,
                            ret_effective_date_end,
                            return_code,
                            return_msg,
                            result_priority_search_cur
                        });

                if (executeResult != null)
                {
                    DataTable dtTableRespones = (DataTable)executeResult[11];
                    List<DataConfigEDITRulePriorityTable> ListDataEditRulePriorityRespones = new List<DataConfigEDITRulePriorityTable>();
                    ListDataEditRulePriorityRespones = dtTableRespones.DataTableToList<DataConfigEDITRulePriorityTable>();
                    executeResults.result_priority_condition_cur = ListDataEditRulePriorityRespones;
                    returnForm.dataConfigEditRulePriority = executeResults.result_priority_condition_cur;
                    returnForm.ret_rule_id = executeResult[0].ToSafeString();
                    returnForm.ret_rule_name = executeResult[1].ToSafeString();
                    returnForm.ret_priority = executeResult[2].ToSafeString();
                    returnForm.ret_lmr_flag = executeResult[3].ToSafeString();
                    returnForm.ret_lookup_name = executeResult[4].ToSafeString();
                    returnForm.ret_lookup_parameter = executeResult[5].ToSafeString();
                    returnForm.ret_rule_param_id = executeResult[6].ToSafeString();
                    returnForm.ret_effective_date_start = executeResult[7].ToSafeString();
                    returnForm.ret_effective_date_end = executeResult[8].ToSafeString();
                    returnForm.ret_code = executeResult[8].ToSafeString();
                    returnForm.ret_msg = executeResult[9].ToSafeString();

                }
                return returnForm;
            }
            catch (Exception ex)
            {
                _logger.Info("Error call pkg_fixed_asset_prioritylookup.p_get_condition_priority handles : " + ex.Message);
                return null;
            }


        }
    }
}
