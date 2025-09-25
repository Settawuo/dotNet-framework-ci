using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using WBBContract;
using WBBContract.Commands;
using WBBData.Repository;

namespace WBBBusinessLayer.CommandHandlers
{
    public class UpdateBulkCorpCommandHandler : ICommandHandler<UpdateBulkCorpCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<string> _objService;

        public UpdateBulkCorpCommandHandler(ILogger logger, IEntityRepository<string> objService)
        {
            _logger = logger;
            _objService = objService;
        }

        public void Handle(UpdateBulkCorpCommand command)
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

                var execute = _objService.ExecuteStoredProc("WBB.PKG_FBBBULK_CORP_BATCH.PROC_UPDATE_BULKCORP_REGISTER",
                out paramOut,
                  new
                  {
                      P_USER = command.P_USER,
                      P_BULK_NO = command.P_BULK_NO,
                      P_ORDER_NO = command.P_ORDER_NO,
                      P_STATUS_TYPE = command.P_STATUS_TYPE,
                      P_STATUS = command.P_STATUS,
                      P_MSG_ERROR = command.P_MSG_ERROR,
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
                command.P_RETURN_MESSAGE = "Error call UpdateBulkCorpCommandHandler: " + ex.Message;
            }
        }

    }
}
