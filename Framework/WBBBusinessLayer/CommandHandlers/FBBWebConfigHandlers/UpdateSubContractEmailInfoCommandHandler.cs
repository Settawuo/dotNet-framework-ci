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
    public class UpdateSubContractEmailInfoCommandHandler : ICommandHandler<UpdateSubContractEmailInfoCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_HISTORY_LOG> _historyLog;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<string> _objService;
        public UpdateSubContractEmailInfoCommandHandler(ILogger logger,
           IWBBUnitOfWork uow, IEntityRepository<string> objService,
           IEntityRepository<FBB_HISTORY_LOG> historyLog)
        {
            _logger = logger;
            _uow = uow;
            _historyLog = historyLog;
            _objService = objService;
        }
        public void Handle(UpdateSubContractEmailInfoCommand command)
        {
            try
            {
                var ret_code = new OracleParameter();
                ret_code.OracleDbType = OracleDbType.Decimal;
                ret_code.Direction = ParameterDirection.Output;

                var ret_msg = new OracleParameter();
                ret_msg.OracleDbType = OracleDbType.Varchar2;
                ret_msg.Size = 2000;
                ret_msg.Direction = ParameterDirection.Output;

                var outp = new List<object>();
                var paramOut = outp.ToArray();
                var executePassword = _objService.ExecuteStoredProc("WBB.PKG_SUBCONTRACT_EMAIL_INFO.p_edit_subcontract",
                    out paramOut,
                    new
                    {

                        command.p_row_id,
                        command.p_subcontrac_email,
                        command.p_phase,
                        command.p_subcontract_for_email,
                        ret_code,
                        ret_msg
                    });
                command.ret_code = ret_code.Value.ToSafeString();
                command.ret_msg = ret_msg.Value.ToSafeString();

                var historyLogItem = new FBB_HISTORY_LOG();
                historyLogItem.ACTION = ActionHistory.DELETE.ToString();
                historyLogItem.APPLICATION = "FBBConfig";
                historyLogItem.CREATED_BY = "GUISubContractEmailInfo";
                historyLogItem.CREATED_DATE = DateTime.Now;
                historyLogItem.DESCRIPTION = "success";
                _historyLog.Create(historyLogItem);
                _uow.Persist();

            }
            catch (Exception ex)
            {
                var historyLogItem = new FBB_HISTORY_LOG();
                historyLogItem.ACTION = ActionHistory.DELETE.ToString();
                historyLogItem.APPLICATION = "FBBConfig";
                historyLogItem.CREATED_BY = "GUISubContractEmailInfo";
                historyLogItem.CREATED_DATE = DateTime.Now;
                historyLogItem.DESCRIPTION = "" + ex.GetErrorMessage().ToSafeString();
                _historyLog.Create(historyLogItem);
                _uow.Persist();

                _logger.Info(ex.GetErrorMessage());
            }

        }
    }
}
