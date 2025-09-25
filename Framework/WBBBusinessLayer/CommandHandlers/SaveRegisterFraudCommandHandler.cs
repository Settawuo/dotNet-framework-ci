using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using WBBBusinessLayer.QueryHandlers;
using WBBContract;
using WBBContract.Commands;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;

namespace WBBBusinessLayer.CommandHandlers
{
    public class SaveRegisterFraudCommandHandler : ICommandHandler<SaveRegisterFraudCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<string> _objService;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;

        public SaveRegisterFraudCommandHandler(ILogger logger
            , IEntityRepository<string> objService
            , IWBBUnitOfWork uow
            , IEntityRepository<FBB_INTERFACE_LOG> intfLog)
        {
            _logger = logger;
            _objService = objService;
            _uow = uow;
            _intfLog = intfLog;
        }

        public void Handle(SaveRegisterFraudCommand command)
        {

            InterfaceLogCommand log = null;
            log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, command, "", "SaveRegisterFraud", "SaveRegisterFraudCommand", "", "", "WEB");

            try
            {
                var RETURN_CODE = new OracleParameter();
                RETURN_CODE.OracleDbType = OracleDbType.Varchar2;
                RETURN_CODE.Size = 1000;
                RETURN_CODE.Direction = ParameterDirection.Output;

                var RETURN_MESSAGE = new OracleParameter();
                RETURN_MESSAGE.OracleDbType = OracleDbType.Varchar2;
                RETURN_MESSAGE.Size = 2000;
                RETURN_MESSAGE.Direction = ParameterDirection.Output;

                var outp = new List<object>();
                var paramOut = outp.ToArray();

                var execute = _objService.ExecuteStoredProc("WBB.PKG_FBBOR004_FRAUD.PROC_INSERT_FRAUD",
                out paramOut,
                  new
                  {
                      p_cust_row_id = command.p_cust_row_id.ToSafeString(),
                      p_created_by = command.p_created_by.ToSafeString(),
                      p_cen_fraud_flag = command.p_cen_fraud_flag.ToSafeString(),
                      p_verify_reason_cen_fraud = command.p_verify_reason_cen.ToSafeString(),
                      p_fraud_score = command.p_fraud_score.ToSafeString(),
                      p_air_fraud_reason_array = command.p_air_fraud_reason_array.ToSafeString(),
                      p_auto_create_prospect_flag = command.p_auto_create_prospect_flag.ToSafeString(),
                      p_cs_note_popup = command.p_cs_note_popup.ToSafeString(),
                      p_url_attach_popup = command.p_url_attach_popup.ToSafeString(),

                      RETURN_CODE = RETURN_CODE,
                      RETURN_MESSAGE = RETURN_MESSAGE

                  });

                if (RETURN_CODE.Value != "-1")
                {
                    command.ret_code = RETURN_CODE.Value.ToString();
                    command.ret_message = RETURN_MESSAGE.Value.ToString();
                    _logger.Info("End WBB.PKG_FBBOR004_FRAUD.PROC_INSERT_FRAUD output msg: " + command.ret_code);
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, command, log, "Success", "", "");
                }
                else
                {
                    command.ret_code = "-1";
                    command.ret_message = "Error return -1 call service WBB.PKG_FBBOR004_FRAUD.PROC_INSERT_FRAUD output msg: " + "Error";
                    _logger.Info("Error return -1 call service WBB.PKG_FBBOR004_FRAUD.PROC_INSERT_FRAUD output msg: " + "Error");
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, command, log, "Failed", "Failed", "");
                }
            }
            catch (Exception ex)
            {
                command.ret_code = "-1";
                command.ret_message = "Error call SaveRegisterFraudCommand: " + ex.Message;
                _logger.Info(ex.Message);
                _logger.Info("Error call service WBB.PKG_FBBOR004_FRAUD.PROC_INSERT_FRAUD" + ex.Message);
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, command, log, "Failed", ex.Message, "");
            }
        }
    }
}
