using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using WBBBusinessLayer.QueryHandlers;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Commands.FBBWebConfigCommands;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;

namespace WBBBusinessLayer.CommandHandlers.FBBWebConfigHandlers
{
    public class DeleteConfigurationLookupCommandHandler : ICommandHandler<DeleteConfigurationLookupCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<string> _objService;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;

        public DeleteConfigurationLookupCommandHandler(ILogger ILogger, IEntityRepository<string> objService, IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IWBBUnitOfWork uow)
        {
            _logger = ILogger;
            _objService = objService;
            _intfLog = intfLog;
            _uow = uow;
        }
        public void Handle(DeleteConfigurationLookupCommand command)
        {
            InterfaceLogCommand log = null;
            try
            {
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, command, "", "DeleteConfigurationLookupCommandHandler", "DeleteConfigurationLookupCommandHandler", "", "FBB", "WEB_CONFIG");


                var p_lookup_name = new OracleParameter();
                p_lookup_name.ParameterName = "p_lookup_name";
                p_lookup_name.Size = 2000;
                p_lookup_name.OracleDbType = OracleDbType.Varchar2;
                p_lookup_name.Direction = ParameterDirection.Input;
                p_lookup_name.Value = command.LOOKUP_NAME;

                var p_modified_by = new OracleParameter();
                p_modified_by.ParameterName = "p_modified_by";
                p_modified_by.Size = 2000;
                p_modified_by.OracleDbType = OracleDbType.Varchar2;
                p_modified_by.Direction = ParameterDirection.Input;
                p_modified_by.Value = command.USER;

                var return_code = new OracleParameter();
                return_code.ParameterName = "return_code";
                return_code.OracleDbType = OracleDbType.BinaryFloat;
                return_code.Direction = ParameterDirection.Output;

                var return_msg = new OracleParameter();
                return_msg.ParameterName = "return_msg";
                return_msg.OracleDbType = OracleDbType.Varchar2;
                return_msg.Size = 2000;
                return_msg.Direction = ParameterDirection.Output;

                var outp = new List<object>();
                var paramOut = outp.ToArray();

                var executeResult = _objService.ExecuteStoredProc("wbb.pkg_fixed_asset_prioritylookup.p_delete_lookup",
                            out paramOut,
                            new
                            {
                                p_lookup_name,
                                p_modified_by,
                                return_code,
                                return_msg

                            });
                command.return_code = return_code.Value.ToSafeString();
                command.return_msg = return_msg.Value.ToSafeString();
                _logger.Info("End wbb.pkg_fixed_asset_prioritylookup.p_delete_lookup" + return_msg);
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, return_code, log, "Success", "", "");
            }
            catch (Exception ex)
            {
                _logger.Info(ex.GetErrorMessage());
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex, log, "Failed", ex.GetErrorMessage(), "");
                command.return_code = "-1";
                command.return_msg = "Error DeleteConfigurationLookupCommand : " + ex.GetErrorMessage();
            }
            
        }
    }
}
