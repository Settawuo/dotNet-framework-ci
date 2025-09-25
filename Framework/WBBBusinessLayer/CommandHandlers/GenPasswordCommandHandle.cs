using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using WBBContract;
using WBBContract.Commands;
using WBBData.Repository;
using WBBEntity.Extensions;
namespace WBBBusinessLayer.CommandHandlers
{
    public class GenPasswordCommandHandle : ICommandHandler<GenPasswordCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<string> _objService;
        private readonly IAirNetEntityRepository<string> _objAirService;
        public GenPasswordCommandHandle(ILogger logger, IEntityRepository<string> objService, IAirNetEntityRepository<string> objAirService)
        {
            _logger = logger;
            _objService = objService;
            _objAirService = objAirService;
        }
        public void Handle(GenPasswordCommand command)
        {
            try
            {
                var o_return_code = new OracleParameter();
                o_return_code.OracleDbType = OracleDbType.Decimal;
                o_return_code.Direction = ParameterDirection.Output;

                var o_return_message = new OracleParameter();
                o_return_message.OracleDbType = OracleDbType.Varchar2;
                o_return_message.Size = 2000;
                o_return_message.Direction = ParameterDirection.Output;

                var o_password = new OracleParameter();
                o_password.OracleDbType = OracleDbType.Varchar2;
                o_password.Size = 2000;
                o_password.Direction = ParameterDirection.Output;

                var result = new OracleParameter();
                result.OracleDbType = OracleDbType.Varchar2;
                result.Size = 2000;
                result.Direction = ParameterDirection.Output;

                var outp = new List<object>();
                var paramOut = outp.ToArray();

                var executeResult = _objAirService.ExecuteStoredProc("AIR_ADMIN.PKG_AIROR901.GEN_PASSWORD",
                out paramOut,
                  new
                  {
                      o_return_code = o_return_code,
                      o_return_message = o_return_message,
                      o_password = o_password
                  });

                command.Genpassword = o_password.Value.ToSafeString();

                var executeDecrypt = _objService.ExecuteStoredProc("WBB.PKG_FBBDORM_PASSWORD.DECRYPT",
                out paramOut,
                 new
                 {
                     data = command.Genpassword,
                     result = result
                 });
                command.PasswordDec = result.Value.ToSafeString();

                command.ret_code = 1;
                command.ret_msg = "Success";
            }
            catch (Exception ex)
            {
                _logger.Info(ex.GetErrorMessage());
                command.ret_code = -1;
                command.ret_msg = "Error call SavePreregister Handler: " + ex.GetErrorMessage();
            }
        }
    }
}
