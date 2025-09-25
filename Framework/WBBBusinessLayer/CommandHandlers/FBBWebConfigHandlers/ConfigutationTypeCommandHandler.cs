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
    public class ConfigutationTypeCommandHandler : ICommandHandler<ConfigutationTypeCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<string> _objService;
        private readonly IEntityRepository<FBB_HISTORY_LOG> _historyLog;
        private readonly IWBBUnitOfWork _uow;
        public ConfigutationTypeCommandHandler(ILogger ILogger, IWBBUnitOfWork uow, IEntityRepository<string> objService, IEntityRepository<FBB_HISTORY_LOG> historyLog)
        {
            _logger = ILogger;
            _objService = objService;
            _historyLog = historyLog;
            _uow = uow;
        }
        public void Handle(ConfigutationTypeCommand command)
        {
            var historyLog = new FBB_HISTORY_LOG();
            var ret_code = new OracleParameter();
            ret_code.OracleDbType = OracleDbType.Decimal;
            ret_code.Direction = ParameterDirection.Output;

            var ret_msg = new OracleParameter();
            ret_msg.OracleDbType = OracleDbType.Varchar2;
            ret_msg.Size = 2000;
            ret_msg.Direction = ParameterDirection.Output;

            var outp = new List<object>();
            var paramOut = outp.ToArray();

            try
            {
                #region Add Cost Installasion

                _logger.Info("StartPKG_FIXED_ASSET_CONFIG_INSTALL.p_adddataconfig_type");
                var executeResult = _objService.ExecuteStoredProc("WBB.PKG_FIXED_ASSET_CONFIG_INSTALL.p_adddataconfig_type",
                    out paramOut,
                    new
                    {
                        command.p_name,
                        command.p_type,
                        command.p_username,

                        ret_code,
                        ret_msg

                    });
                command.ret_code = ret_code.Value.ToSafeString();
                command.ret_msg = ret_msg.Value.ToSafeString();
                _logger.Info("EndPKG_FIXED_ASSET_CONFIG_INSTALL.p_adddataconfig_type" + ret_msg);
                if (command.ret_code != "0")
                {
                    historyLog.HISTORY_ID = historyLog.HISTORY_ID;
                    historyLog.ACTION = ActionHistory.ADD.ToString();
                    historyLog.APPLICATION = "ADD Data Config Type";
                    historyLog.CREATED_BY = "FBB-FIXEDADMIN";
                    historyLog.CREATED_DATE = DateTime.Now;
                    historyLog.DESCRIPTION = command.ret_msg + " ret_code!=0";
                    historyLog.REF_KEY = "FBB-FIXEDADMIN";
                    historyLog.REF_NAME = "NODEID";
                    _historyLog.Create(historyLog);
                    _uow.Persist();
                }

                #endregion


            }
            catch (Exception ex)
            {
                historyLog.HISTORY_ID = historyLog.HISTORY_ID;
                historyLog.ACTION = ActionHistory.ADD.ToString();
                historyLog.APPLICATION = "ADD Data Config Type";
                historyLog.CREATED_BY = "FBB-FIXEDADMIN";
                historyLog.CREATED_DATE = DateTime.Now;
                historyLog.DESCRIPTION = command.ret_msg + " ret_code!=0";
                historyLog.REF_KEY = "FBB-FIXEDADMIN";
                historyLog.REF_NAME = "NODEID";
                _historyLog.Create(historyLog);
                _uow.Persist();

                _logger.Info(ex.GetErrorMessage());
                command.ret_code = "-1";
                command.ret_msg = "Error ADD Data Config Type CommandHandler : " + ex.GetErrorMessage();

            }
        }
    }
}
