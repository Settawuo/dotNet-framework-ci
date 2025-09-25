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
    public class DeleteFileNameLeaveMessageCommandHandler : ICommandHandler<DeleteFileNameLeaveMessageCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<string> _objService;

        public DeleteFileNameLeaveMessageCommandHandler(ILogger logger, IEntityRepository<string> objService)
        {
            _logger = logger;
            _objService = objService;
        }

        public void Handle(DeleteFileNameLeaveMessageCommand command)
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

                var execute = _objService.ExecuteStoredProc("WBB.PKG_FBBOR021.PROC_DELETED_FILE_NAME",
                out paramOut,
                  new
                  {
                      p_file_name = command.p_file_name.ToSafeString(),
                      p_username = command.p_username.ToSafeString(),

                      return_code = ret_code,
                      return_message = ret_msg
                  });

                command.return_code = Convert.ToInt32(ret_code.Value == null ? "0" : ret_code.Value.ToString());
                command.return_message = ret_msg.Value == null ? "" : ret_msg.Value.ToString();
            }
            catch (Exception ex)
            {
                _logger.Info(ex.Message);
                command.return_code = -1;
                command.return_message = "Error call DeleteFileNameLeaveMessageCommandHandler: " + ex.Message;
            }
        }
    }
}
