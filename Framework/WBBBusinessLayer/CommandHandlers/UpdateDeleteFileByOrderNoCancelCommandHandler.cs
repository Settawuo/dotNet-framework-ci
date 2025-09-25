using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using WBBContract;
using WBBContract.Commands;
using WBBData.Repository;

namespace WBBBusinessLayer.CommandHandlers
{
    public class UpdateDeleteFileByOrderNoCancelCommandHandler : ICommandHandler<UpdateDeleteFileByOrderNoCancelCommand>
    {
        private readonly ILogger _logger;
        private readonly IAirNetEntityRepository<string> _airnetRepository;

        public UpdateDeleteFileByOrderNoCancelCommandHandler(ILogger logger, IAirNetEntityRepository<string> airnetRepository)
        {
            _logger = logger;
            _airnetRepository = airnetRepository;
        }

        public void Handle(UpdateDeleteFileByOrderNoCancelCommand command)
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

                var execute = _airnetRepository.ExecuteStoredProc("AIR_ADMIN.PKG_FBBOR022_BATCH.PROC_DELETE_FILE_PATH",
                    out paramOut,
                    new
                    {
                        P_ORDER_NO = command.P_ORDER_NO,
                        RES_CODE = ret_code,
                        RES_MESSAGE = ret_msg
                    });

                command.RES_CODE = ret_code.Value.ToString();
                command.RES_MESSAGE = ret_msg.Value == null ? "" : ret_msg.Value.ToString();
            }
            catch (Exception ex)
            {
                _logger.Info(ex.Message);
                command.RES_CODE = "-1";
                command.RES_MESSAGE = "Error call UpdateDeleteFileByOrderNoCancelCommandHandler: " + ex.Message;
            }
        }
    }
}
