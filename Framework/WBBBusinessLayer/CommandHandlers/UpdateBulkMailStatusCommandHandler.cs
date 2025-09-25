using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using WBBContract;
using WBBContract.Commands;
using WBBData.Repository;

namespace WBBBusinessLayer.CommandHandlers
{
    public class UpdateBulkMailStatusCommandHandler : ICommandHandler<BatchUpdateMailStatusCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<string> _objService;

        public UpdateBulkMailStatusCommandHandler(ILogger logger, IEntityRepository<string> objService)
        {
            _logger = logger;
            _objService = objService;
        }

        public void Handle(BatchUpdateMailStatusCommand command)
        {
            try
            {
                var ret_code = new OracleParameter();
                ret_code.OracleDbType = OracleDbType.Varchar2;
                ret_code.Size = 1000;
                ret_code.Direction = ParameterDirection.Output;

                var ret_msg = new OracleParameter();
                ret_msg.OracleDbType = OracleDbType.Varchar2;
                ret_msg.Size = 2000;
                ret_msg.Direction = ParameterDirection.Output;

                var outp = new List<object>();
                var paramOut = outp.ToArray();

                var execute = _objService.ExecuteStoredProc("WBB.PKG_FBBBULK_CORP_BATCH.PROC_UPDATE_STATUS_MAIL",
                out paramOut,
                  new
                  {
                      P_BULK_NO = command.P_BULK_NO,

                      P_RETURN_CODE = ret_code,
                      P_RETURN_MESSAGE = ret_msg

                  });

                command.P_RETURN_CODE = ret_code.Value.ToString();
                command.P_RETURN_MESSAGE = ret_msg.Value == null ? "" : ret_msg.Value.ToString();


            }
            catch (Exception ex)
            {
                _logger.Info(ex.Message);
                command.P_RETURN_CODE = "-1";
                command.P_RETURN_MESSAGE = "Error call UpdateBulkMailStatusCommandHandler: " + ex.Message;
            }
        }




    }
}
