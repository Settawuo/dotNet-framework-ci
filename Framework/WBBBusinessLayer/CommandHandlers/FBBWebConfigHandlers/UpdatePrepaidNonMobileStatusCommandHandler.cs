using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using WBBContract;
using WBBContract.Commands.FBBWebConfigCommands;
using WBBData.Repository;
using WBBEntity.Extensions;

namespace WBBBusinessLayer.CommandHandlers.FBBWebConfigHandlers
{
    public class UpdatePrepaidNonMobileStatusCommandHandler : ICommandHandler<UpdatePrepaidNonMobileStatusCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<string> _objService;

        public UpdatePrepaidNonMobileStatusCommandHandler(ILogger logger,
            IEntityRepository<string> objService)
        {
            _logger = logger;
            _objService = objService;
        }

        public void Handle(UpdatePrepaidNonMobileStatusCommand command)
        {
            try
            {
                var p_return_code = new OracleParameter();
                p_return_code.OracleDbType = OracleDbType.Decimal;
                p_return_code.Direction = ParameterDirection.Output;

                var p_return_message = new OracleParameter();
                p_return_message.OracleDbType = OracleDbType.Varchar2;
                p_return_message.Size = 2000;
                p_return_message.Direction = ParameterDirection.Output;

                var outp = new List<object>();
                var paramOut = outp.ToArray();
                _logger.Info("StartPROC_UPDATE_PrepaidNonMobile_STATUS");

                var executePassword = _objService.ExecuteStoredProc("WBB.PKG_FBBDORM_ADMIN001.PROC_EDIT_STATUS",
                out paramOut,
                  new
                  {
                      p_fibre = command.FibrenetID.ToSafeString(),
                      p_status = command.Status.ToSafeString(),
                      p_user = command.User.ToSafeString(),

                      //Return value
                      p_return_code = p_return_code,
                      p_return_message = p_return_message
                  });

                command.Return_Code = p_return_code.Value != null ? Convert.ToInt32(p_return_code.Value.ToSafeString()) : 1;
                command.Return_Message = p_return_message.Value.ToSafeString();
                _logger.Info("EndPROC_UPDATE_PrepaidNonMobile_STATUS : " + p_return_message);
            }
            catch (Exception ex)
            {
                command.Return_Code = 1;
                command.Return_Message = ex.GetErrorMessage();
                _logger.Info(ex.GetErrorMessage());
            }
        }
    }
}
