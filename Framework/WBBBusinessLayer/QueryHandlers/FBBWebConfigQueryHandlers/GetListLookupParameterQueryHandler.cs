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
    public class GetListLookupParameterQueryHandler : IQueryHandler<GetListLookupParameterQuery, ConfigurationLookupParamView>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<object> _objService;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;

        public GetListLookupParameterQueryHandler(ILogger logger, IEntityRepository<object> objService, IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IWBBUnitOfWork uow)
        {
            _logger = logger;
            _objService = objService;
            _intfLog = intfLog;
            _uow = uow;
        }

        public ConfigurationLookupParamView Handle(GetListLookupParameterQuery query)
        {
            InterfaceLogCommand log = null;
            try
            {
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, "", "GetListLookupParameterQueryHandler", "GetListLookupParameterQueryHandler", "", "FBB", "WEB_CONFIG");

                ConfigLookupParamResponse executeResults = new ConfigLookupParamResponse();
                var returnForm = new ConfigurationLookupParamView();

                var p_lookup_name = new OracleParameter();
                p_lookup_name.ParameterName = "p_lookup_name";
                p_lookup_name.Size = 2000;
                p_lookup_name.OracleDbType = OracleDbType.Varchar2;
                p_lookup_name.Direction = ParameterDirection.Input;
                p_lookup_name.Value = query.p_lookup_name;

                var ret_code = new OracleParameter();
                ret_code.ParameterName = "return_code";
                ret_code.OracleDbType = OracleDbType.BinaryFloat;
                ret_code.Direction = ParameterDirection.Output;

                var return_msg = new OracleParameter();
                return_msg.ParameterName = "return_msg";
                return_msg.OracleDbType = OracleDbType.Varchar2;
                return_msg.Size = 2000;
                return_msg.Direction = ParameterDirection.Output;

                var result_lookup_param_cur = new OracleParameter
                {
                    ParameterName = "result_lookup_param_cur",
                    OracleDbType = OracleDbType.RefCursor,
                    Direction = ParameterDirection.Output
                };

                var executeResult = _objService.ExecuteStoredProcMultipleCursor(
                    "wbb.pkg_fixed_asset_prioritylookup.p_get_lookup_param",
                    new[]
                    {
                        p_lookup_name,
                        ret_code,
                        return_msg,
                        result_lookup_param_cur
                    });
                
                    if (executeResult != null)
                    {
                        DataTable dtTableRespones = (DataTable)executeResult[2];
                        List<DataConfigLookupParamTable> ListDataLookupParamRespones = new List<DataConfigLookupParamTable>();
                        ListDataLookupParamRespones = dtTableRespones.DataTableToList<DataConfigLookupParamTable>();
                        executeResults.result_lookup_param_cur = ListDataLookupParamRespones;
                        returnForm.dataConfigLookupParam = executeResults.result_lookup_param_cur;
                        InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, "", log, "Success", "", "");
                    }
                    return returnForm;
            }
            catch (Exception ex)
            {
                //_logger.Info(ex.Message);
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, "", log, "Failed ex", ex.Message , "");
                _logger.Info("Error call pkg_fixed_asset_prioritylookup.p_get_lookup_param handles : " + ex.Message);
                return null;
            }
        }
    }
}
