using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using WBBContract;
using WBBContract.Commands;
using WBBData.Repository;

namespace WBBBusinessLayer.CommandHandlers
{
    public class UpdateFilePathByOrderNoCommandHandler : ICommandHandler<UpdateFilePathByOrderNoCommand>
    {
        private readonly ILogger _logger;
        private readonly IAirNetEntityRepository<string> _airnetRepository;

        public UpdateFilePathByOrderNoCommandHandler(ILogger logger, IAirNetEntityRepository<string> airnetRepository)
        {
            _logger = logger;
            _airnetRepository = airnetRepository;
        }

        public void Handle(UpdateFilePathByOrderNoCommand command)
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

                var execute = _airnetRepository.ExecuteStoredProc("AIR_ADMIN.PKG_FBBOR006.PROC_UPDATE_FILE_PATH",
                    out paramOut,
                    new
                    {
                        P_Order_No = command.P_Order_No,
                        P_File_Path = command.P_File_Path,
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
                command.RES_MESSAGE = "Error call UpdateFilePathByOrderNoCommandHandler: " + ex.Message;
            }
        }
    }
}
