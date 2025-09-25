using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using WBBContract;
using WBBContract.Commands.FBBWebConfigCommands;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;

namespace WBBBusinessLayer.CommandHandlers.FBBWebConfigHandlers
{
    public class DeleteConfigurationRulePriorityCammandHandler : ICommandHandler<DeleteConfigurationRulePriorityCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<string> _objService;
        private readonly IEntityRepository<FBB_HISTORY_LOG> _historyLog;
        private readonly IWBBUnitOfWork _uow;

        public DeleteConfigurationRulePriorityCammandHandler(ILogger ILogger, IEntityRepository<string> objService, IEntityRepository<FBB_HISTORY_LOG> historyLog, IWBBUnitOfWork uow)
        {
            _logger = ILogger;
            _objService = objService;
            _historyLog = historyLog;
            _uow = uow;
        }
        public void Handle(DeleteConfigurationRulePriorityCommand command)
        {
            try
            {
                var p_rule_id = new OracleParameter();
                p_rule_id.ParameterName = "p_rule_id";
                p_rule_id.Size = 2000;
                p_rule_id.OracleDbType = OracleDbType.Varchar2;
                p_rule_id.Direction = ParameterDirection.Input;
                p_rule_id.Value = command.RULE_ID;

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

                var executeResult = _objService.ExecuteStoredProc("wbb.pkg_fixed_asset_prioritylookup.p_delete_priority",
                            out paramOut,
                            new
                            {
                                p_rule_id,
                                return_code,
                                return_msg

                            });
                command.return_code = return_code.Value.ToSafeString();
                command.return_msg = return_msg.Value.ToSafeString();
                _logger.Info("End wbb.pkg_fixed_asset_prioritylookup.p_delete_priority" + return_msg);
            }
            catch (Exception ex)
            {
                _logger.Info(ex.GetErrorMessage());
                command.return_code = "-1";
                command.return_msg = "Error DeleteConfigurationRulePriorityCommand : " + ex.GetErrorMessage();
            }

        }
    }
}

