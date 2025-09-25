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
    public class UpdateOLTStatusCommandHandler : ICommandHandler<UpdateOLTStatusCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<string> _objService;

        public UpdateOLTStatusCommandHandler(ILogger logger,
            IEntityRepository<string> objService)
        {
            _logger = logger;
            _objService = objService;
        }

        public void Handle(UpdateOLTStatusCommand command)
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
                _logger.Info("StartPROC_UPDATE_OLT_STATUS");

                var executePassword = _objService.ExecuteStoredProc("WBB.PKG_FBBPAYG_REPORT_OLT.p_update_olt_status",
                out paramOut,
                  new
                  {
                      p_olt_name = command.OLT_NAME.ToSafeString(),
                      p_olt_status = command.OLT_STATUS.ToSafeString(),
                      ret_code = ret_code,
                      ret_msg = ret_msg

                  });

                command.Return_Code = ret_code.Value.ToSafeString();
                command.Return_Message = ret_msg.Value.ToSafeString();

                _logger.Info("EndPROC_UPDATE_OLT_STATUS" + ret_msg);

            }
            catch (Exception ex)
            {
                command.Return_Code = "0";
                command.Return_Message = ex.GetErrorMessage();

                _logger.Info(ex.GetErrorMessage());
            }
        }
    }
}
