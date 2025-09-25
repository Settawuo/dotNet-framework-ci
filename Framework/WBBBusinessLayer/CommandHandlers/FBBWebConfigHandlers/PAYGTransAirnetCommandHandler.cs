using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using WBBContract;
using WBBContract.Commands.FBBWebConfigCommands;
using WBBData.Repository;
using WBBEntity.Extensions;
//using WBBEntity.Models;

namespace WBBBusinessLayer.CommandHandlers.FBBWebConfigHandlers
{
    public class PAYGTransAirnetCommandHandler : ICommandHandler<PAYGTransAirnetCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<PAYGTransAirnetCommand> _objService;

        public PAYGTransAirnetCommandHandler(ILogger logger,
            IEntityRepository<PAYGTransAirnetCommand> objService)
        {
            _logger = logger;
            _objService = objService;
        }

        public void Handle(PAYGTransAirnetCommand command)
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
                _logger.Info("StartPROC_PAYG_AIRNET_INV_REC");

                var executePassword = _objService.ExecuteStoredProc("WBB.PKG_FBBPAYG_AIRNET_INV_REC.p_fetch_data",
                out paramOut,
                  new
                  {
                      ret_code = ret_code,
                      ret_msg = ret_msg

                  });

                command.Return_Code = ret_code.Value.ToSafeString();
                command.Return_Message = ret_msg.Value.ToSafeString();

                _logger.Info("EndPROC_PAYG_AIRNET_INV_REC");

            }
            catch (Exception ex)
            {
                _logger.Info(ex.GetErrorMessage());
                command.Return_Code = "1";
                command.Return_Message = ex.GetErrorMessage();
            }
        }

    }
}
