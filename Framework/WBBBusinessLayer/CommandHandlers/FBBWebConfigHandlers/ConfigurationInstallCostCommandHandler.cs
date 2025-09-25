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
    public class ConfigurationInstallCostCommandHandler : ICommandHandler<ConfigurationInstallCostCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<string> _objService;
        private readonly IEntityRepository<FBB_HISTORY_LOG> _historyLog;
        private readonly IWBBUnitOfWork _uow;
        public ConfigurationInstallCostCommandHandler(ILogger ILogger, IEntityRepository<string> objService, IEntityRepository<FBB_HISTORY_LOG> historyLog, IWBBUnitOfWork uow)
        {
            _logger = ILogger;
            _objService = objService;
            _historyLog = historyLog;
            _uow = uow;
        }
        public void Handle(ConfigurationInstallCostCommand command)
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

                if (command.P_COMMAND == "A")
                {

                    _logger.Info("StartPKG_FIXED_ASSET_CONFIG_INSTALL.p_add_configuration_cost");
                    var executeResult = _objService.ExecuteStoredProc("WBB.PKG_FIXED_ASSET_CONFIG_INSTALL.p_add_configuration_cost",
                        out paramOut,
                        new
                        {
                            command.P_TABLE,
                            command.P_RULE_NAME,
                            command.P_ORDER_TYPE,
                            command.P_SUBCONTRACT_TYPE,
                            command.P_SUBCONTRACT_SUB_TYPE,
                            command.P_VENDOR_CODE,
                            command.P_TECHNOLOGY,
                            command.P_TOTAL_PRICE,
                            command.P_EVENT_CODE,
                            command.P_ROOM_FLAG,
                            command.P_REUSE_FLAG,
                            command.P_DISTANCE_FROM,
                            command.P_DISTANCE_TO,
                            command.P_INDOOR_PRICE,
                            command.P_OUTDOOR_PRICE,
                            command.P_INTERNET_PRICE,
                            command.P_VOIP_PRICE,
                            command.P_PLAYBOX_PRICE,
                            command.P_MECH_PRICE,
                            command.P_ADDRESS_ID,
                            command.P_EVENT_TYPE,
                            command.P_EFFECTIVE_DATE,
                            command.P_EXPIRE_DATE,
                            command.P_SAME_DAY,
                            command.P_USERNAME,
                            command.P_SUB_LOCATION,
                            ret_code,
                            ret_msg

                        });
                    command.ret_code = ret_code.Value.ToSafeString();
                    command.ret_msg = ret_msg.Value.ToSafeString();
                    _logger.Info("EndPKG_FIXED_ASSET_CONFIG_INSTALL.p_add_configuration_cost" + ret_msg);
                    if (command.ret_code != "0")
                    {
                        historyLog.HISTORY_ID = historyLog.HISTORY_ID;
                        historyLog.ACTION = ActionHistory.ADD.ToString();
                        historyLog.APPLICATION = "ADD Config_COST_TABLE";
                        historyLog.CREATED_BY = "FBB-FIXEDADMIN";
                        historyLog.CREATED_DATE = DateTime.Now;
                        historyLog.DESCRIPTION = command.ret_msg + " ret_code!=0";
                        historyLog.REF_KEY = "FBB-FIXEDADMIN";
                        historyLog.REF_NAME = "NODEID";
                        _historyLog.Create(historyLog);
                        _uow.Persist();
                        command.ret_msg = "Save failed.Please check data again.";
                    }
                }
                #endregion
                #region Uppdate Cost Installasion

                else if (command.P_COMMAND == "E")
                {


                    var executeResult = _objService.ExecuteStoredProc("WBB.PKG_FIXED_ASSET_CONFIG_INSTALL.p_update_configuration_cost",
                        out paramOut,
                        new
                        {
                            command.P_TABLE,
                            command.P_RULE_ID,
                            command.P_RULE_NAME,
                            command.P_ORDER_TYPE,
                            command.P_SUBCONTRACT_TYPE,
                            command.P_SUBCONTRACT_SUB_TYPE,
                            command.P_VENDOR_CODE,
                            command.P_TECHNOLOGY,
                            command.P_TOTAL_PRICE,
                            command.P_EVENT_CODE,
                            command.P_ROOM_FLAG,
                            command.P_REUSE_FLAG,
                            command.P_DISTANCE_FROM,
                            command.P_DISTANCE_TO,
                            command.P_INDOOR_PRICE,
                            command.P_OUTDOOR_PRICE,
                            command.P_INTERNET_PRICE,
                            command.P_VOIP_PRICE,
                            command.P_PLAYBOX_PRICE,
                            command.P_MECH_PRICE,
                            command.P_ADDRESS_ID,
                            command.P_EVENT_TYPE,
                            command.P_EFFECTIVE_DATE,
                            command.P_EXPIRE_DATE,
                            command.P_SAME_DAY,
                            command.P_USERNAME,
                            command.P_SUB_LOCATION,
                            ret_code,
                            ret_msg

                        });
                    command.ret_code = ret_code.Value.ToSafeString();
                    command.ret_msg = ret_msg.Value.ToSafeString();
                    _logger.Info("EndPKG_FIXED_ASSET_CONFIG_INSTALL.p_add_configuration_cost" + ret_msg);
                    if (command.ret_code != "0")
                    {
                        historyLog.HISTORY_ID = historyLog.HISTORY_ID;
                        historyLog.ACTION = ActionHistory.ADD.ToString();
                        historyLog.APPLICATION = "UpdateConfig_COST_TABLE";
                        historyLog.CREATED_BY = "FBB-FIXEDADMIN";
                        historyLog.CREATED_DATE = DateTime.Now;
                        historyLog.DESCRIPTION = command.ret_msg + " ret_code!=0";
                        historyLog.REF_KEY = "FBB-FIXEDADMIN";
                        historyLog.REF_NAME = "NODEID";
                        _historyLog.Create(historyLog);
                        _uow.Persist();
                        command.ret_msg = "Save failed.Please check data again.";
                    }

                }
                #endregion
                #region Delete Cost Installasion
                else if (command.P_COMMAND == "D")
                {

                    var executeResult = _objService.ExecuteStoredProc("WBB.PKG_FIXED_ASSET_CONFIG_INSTALL.p_delete_configuration_cost",
                        out paramOut,
                        new
                        {
                            command.P_TABLE,
                            command.P_RULE_ID,
                            command.P_USERNAME,
                            ret_code,
                            ret_msg

                        });
                    command.ret_code = ret_code.Value.ToSafeString();
                    command.ret_msg = ret_msg.Value.ToSafeString();
                    _logger.Info("EndPKG_FIXED_ASSET_CONFIG_INSTALL.p_delete_configuration_cost" + ret_msg);
                    if (command.ret_code != "0")
                    {
                        historyLog.HISTORY_ID = historyLog.HISTORY_ID;
                        historyLog.ACTION = ActionHistory.ADD.ToString();
                        historyLog.APPLICATION = "Delete Config_COST_TABLE";
                        historyLog.CREATED_BY = "FBB-FIXEDADMIN";
                        historyLog.CREATED_DATE = DateTime.Now;
                        historyLog.DESCRIPTION = command.ret_msg + " ret_code!=0";
                        historyLog.REF_KEY = "FBB-FIXEDADMIN";
                        historyLog.REF_NAME = "NODEID";
                        _historyLog.Create(historyLog);
                        _uow.Persist();
                        command.ret_msg = "Save failed.Please check data again.";
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                historyLog.HISTORY_ID = historyLog.HISTORY_ID;
                historyLog.ACTION = ActionHistory.ADD.ToString();
                historyLog.APPLICATION = "Config_COST_TABLE";
                historyLog.CREATED_BY = "FBB-FIXEDADMIN";
                historyLog.CREATED_DATE = DateTime.Now;
                historyLog.DESCRIPTION = "Exception Pk Lastmile Re-cal By ORder CommandHandler " + ex.GetErrorMessage().ToSafeString();
                historyLog.REF_KEY = "FBB-FIXEDADMIN";
                historyLog.REF_NAME = "NODEID";
                _historyLog.Create(historyLog);
                _uow.Persist();

                _logger.Info(ex.GetErrorMessage());
                command.ret_code = "-1";
                command.ret_msg = "Error call LastMileBy Distance Re-cal By ORder CommandHandler : " + ex.GetErrorMessage();

            }
        }


    }
}
