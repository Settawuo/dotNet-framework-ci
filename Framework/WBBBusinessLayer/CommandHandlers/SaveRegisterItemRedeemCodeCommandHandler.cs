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
    public class SaveRegisterItemRedeemCodeCommandHandler : ICommandHandler<SaveRegisterItemRedeemCodeCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<string> _objService;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;

        public SaveRegisterItemRedeemCodeCommandHandler(ILogger logger
            , IEntityRepository<string> objService
            , IWBBUnitOfWork uow
            , IEntityRepository<FBB_INTERFACE_LOG> intfLog)
        {
            _logger = logger;
            _objService = objService;
            _uow = uow;
            _intfLog = intfLog;
        }

        public void Handle(SaveRegisterItemRedeemCodeCommand command)
        {

            InterfaceLogCommand log = null;
            log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, command, command.p_non_mobile_no + command.ClientIP, "REGISTER_ITEM_REDEEM_CODE", "SaveRegisterItemRedeemCodeCommand", command.p_id_card_no, "FBB|" + command.FullUrl, "WEB");

            try
            {
                var ret_code = new OracleParameter();
                ret_code.OracleDbType = OracleDbType.Varchar2;
                ret_code.Size = 1000;
                ret_code.Direction = ParameterDirection.Output;

                var ret_message = new OracleParameter();
                ret_message.OracleDbType = OracleDbType.Varchar2;
                ret_message.Size = 2000;
                ret_message.Direction = ParameterDirection.Output;

                var outp = new List<object>();
                var paramOut = outp.ToArray();

                var execute = _objService.ExecuteStoredProc("WBB.PKG_FBBOR042.REGISTER_ITEM_REDEEM_CODE",
                out paramOut,
                  new
                  {
                      p_non_mobile_no = command.p_non_mobile_no.ToSafeString(),
                      p_id_card_no = command.p_id_card_no.ToSafeString(),
                      p_contact_mobile_no = command.p_contact_mobile_no.ToSafeString(),
                      p_game_name = command.p_game_name.ToSafeString(),
                      p_redeem_code_1 = command.p_redeem_code_1.ToSafeString(),
                      p_redeem_code_2 = command.p_redeem_code_2.ToSafeString(),
                      p_redeem_code_3 = command.p_redeem_code_3.ToSafeString(),
                      p_redeem_code_4 = command.p_redeem_code_4.ToSafeString(),
                      p_redeem_code_5 = command.p_redeem_code_5.ToSafeString(),
                      p_redeem_status = command.p_redeem_status.ToSafeString(),

                      ret_code = ret_code,
                      ret_message = ret_message

                  });

                if (ret_code.Value != "-1")
                {
                    command.ret_code = ret_code.Value.ToString();
                    command.ret_message = ret_message.Value.ToString();
                    _logger.Info("End WBB.PKG_FBBOR042.REGISTER_ITEM_REDEEM_CODE output msg: " + command.ret_code);
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, command, log, "Success", "", "");
                }
                else
                {
                    command.ret_code = "-1";
                    command.ret_message = "Error return -1 call service WBB.PKG_FBBOR042.REGISTER_ITEM_REDEEM_CODE output msg: " + "Error";
                    _logger.Info("Error return -1 call service WBB.PKG_FBBOR042.REGISTER_ITEM_REDEEM_CODE output msg: " + "Error");
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, command, log, "Failed", "Failed", "");
                }
            }
            catch (Exception ex)
            {
                command.ret_code = "-1";
                command.ret_message = "Error call SaveRegisterItemRedeemCodeCommandHandler: " + ex.Message;
                _logger.Info(ex.Message);
                _logger.Info("Error call service WBB.PKG_FBBOR042.QUERY_CONFIG_REDEEM_CODE" + ex.Message);
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, command, log, "Failed", ex.Message, "");
            }
        }
    }
}
