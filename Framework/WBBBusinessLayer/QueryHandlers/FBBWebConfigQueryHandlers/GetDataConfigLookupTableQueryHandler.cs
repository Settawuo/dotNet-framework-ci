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
    public class GetDataConfigLookupTableQueryHandler : IQueryHandler<GetDataConfigLookupTableQuery, ConfigurationLookupView> //
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<object> _objService;

        public GetDataConfigLookupTableQueryHandler(ILogger logger, IEntityRepository<object> objService)
        {
            _logger = logger;
            _objService = objService;
        }
        public ConfigurationLookupView Handle(GetDataConfigLookupTableQuery query) //
        {
            try
            {
                ConfigLookupResponse executeResults = new ConfigLookupResponse();
                var returnForm = new ConfigurationLookupView();

                var p_lookup_name = new OracleParameter();
                p_lookup_name.ParameterName = "p_lookup_name";
                p_lookup_name.Size = 2000;
                p_lookup_name.OracleDbType = OracleDbType.Varchar2;
                p_lookup_name.Direction = ParameterDirection.Input;
                p_lookup_name.Value = query.LOOKUP_NAME;

                var return_code = new OracleParameter();
                return_code.ParameterName = "return_code";
                return_code.OracleDbType = OracleDbType.Decimal;
                return_code.Direction = ParameterDirection.Output;

                var return_msg = new OracleParameter();
                return_msg.ParameterName = "return_msg";
                return_msg.OracleDbType = OracleDbType.Varchar2;
                return_msg.Size = 2000;
                return_msg.Direction = ParameterDirection.Output;

                var result_lookup_name_cur = new OracleParameter
                {
                    ParameterName = "result_lookup_name_cur",
                    OracleDbType = OracleDbType.RefCursor,
                    Direction = ParameterDirection.Output
                };

                var executeResult = _objService.ExecuteStoredProcMultipleCursor(
                        "wbb.pkg_fixed_asset_prioritylookup.p_get_config_lookup",
                        new object[]
                        {
                            p_lookup_name,
                            return_code,
                            return_msg,
                            result_lookup_name_cur
                        });

                if (executeResult != null)
                {
                    DataTable dtTableRespones = (DataTable)executeResult[2];
                    List<DataConfigLookupTable> ListDataLookupRespones = new List<DataConfigLookupTable>();
                    ListDataLookupRespones = dtTableRespones.DataTableToList<DataConfigLookupTable>();
                    executeResults.result_lookup_search_cur = ListDataLookupRespones;
                    executeResults.ret_code = return_code.Value.ToSafeString();
                    executeResults.ret_msg = return_msg.Value.ToSafeString();
                    returnForm.dataConfigLookup = executeResults.result_lookup_search_cur;
                }
                return returnForm;
                //return executeResults;
            }
            catch (Exception ex)
            {
                _logger.Info("Error call pkg_fixed_asset_prioritylookup.p_get_config_lookup handles : " + ex.Message);
                return null;
            }            
        }
    }
}
