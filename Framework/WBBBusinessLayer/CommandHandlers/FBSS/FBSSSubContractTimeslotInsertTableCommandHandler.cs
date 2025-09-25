using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using WBBContract;
using WBBContract.Commands.FBSS;
using WBBData.Repository;
using WBBEntity.Extensions;

namespace WBBBusinessLayer.CommandHandlers.FBSS
{
    public class FBSSSubContractTimeslotInsertTableCommandHandler : ICommandHandler<FBSSSubContractTimeslotInsertTableCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<string> _objService;

        public FBSSSubContractTimeslotInsertTableCommandHandler(ILogger logger, IEntityRepository<string> objService)
        {
            _logger = logger;
            _objService = objService;
        }

        public void Handle(FBSSSubContractTimeslotInsertTableCommand command)
        {
            var ret_code = new OracleParameter();
            ret_code.OracleDbType = OracleDbType.Decimal;
            ret_code.Direction = ParameterDirection.Output;

            var ret_message = new OracleParameter();
            ret_message.OracleDbType = OracleDbType.Varchar2;
            ret_message.Direction = ParameterDirection.Output;

            object[] paramOut;

            var buffetData = new OracleParameter();
            buffetData.OracleDbType = OracleDbType.Clob;
            buffetData.Value = command.Bufferdata;
            buffetData.Direction = ParameterDirection.Input;

            var executeResult = _objService.ExecuteStoredProc("WBB.PKG_FBB_TIME_SLOT.INSERT_TIME_SLOT",
            out paramOut,
                new
                {
                    p_file_path = command.Filepath,
                    p_file_name = command.Filename,
                    p_user = "FBB_BATCH",
                    p_buffer = buffetData,
                    p_data_date = command.DataDate,

                    //// return 

                    p_return_code = ret_code,
                    p_return_message = ret_message

                });

            command.Recode = ret_code.Value != null ? Convert.ToInt32(ret_code.Value.ToSafeString()) : -1;
            command.Remessage = ret_message.Value.ToSafeString();
        }
    }

}
