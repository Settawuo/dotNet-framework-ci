using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using WBBContract;
using WBBContract.Commands.FBBWebConfigCommands;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
namespace WBBBusinessLayer.CommandHandlers.FBBWebConfigHandlers
{
    public class ImportExcelDormCommandHandler : ICommandHandler<ImportExcelDormCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<string> _objService;
        private readonly IEntityRepository<FBB_HISTORY_LOG> _historyLog;
        private readonly IWBBUnitOfWork _uow;


        public ImportExcelDormCommandHandler(ILogger logger, IEntityRepository<string> objService, IEntityRepository<FBB_HISTORY_LOG> historyLog, IWBBUnitOfWork uow)
        {
            _logger = logger;
            _objService = objService;
            _historyLog = historyLog;
            _uow = uow;
        }

        public void Handle(ImportExcelDormCommand command)
        {
            try
            {
                var p_return_code = new OracleParameter();
                p_return_code.OracleDbType = OracleDbType.Varchar2;
                p_return_code.Direction = ParameterDirection.Output;

                var p_return_message = new OracleParameter();
                p_return_message.OracleDbType = OracleDbType.Varchar2;
                p_return_message.Size = 2000;
                p_return_message.Direction = ParameterDirection.Output;

                var outp = new List<object>();
                var paramOut = outp.ToArray();
                _logger.Info("StartPROC_INS_MOBILE");

                var execute = _objService.ExecuteStoredProc("WBB.PKG_FBBDORM_ADMIN001.PROC_INSERT_MOBILE",
                out paramOut,
                  new
                  {
                      p_dormitory_name_th = command.Dormitory_NameTH.ToSafeString(),
                      p_Building = command.Building_Name.ToSafeString(),
                      p_Floor_no = command.Floor.ToSafeString(),
                      p_Room_no = command.Room.ToSafeString(),
                      p_Fiber_ID = command.Fibrenet_id.ToSafeString(),
                      p_PIN_ID = command.Pin.ToSafeString(),
                      p_user_import = command.user.ToSafeString(),
                      p_dormitory_name = command.Dormitory_Name.ToSafeString(),
                      p_file_name = command.filename.ToSafeString(),
                      p_return_code = p_return_code,
                      p_return_message = p_return_message

                  });
                var Return_Code = p_return_code.Value.ToSafeString();
                var Return_Message = p_return_message.Value.ToSafeString();
                _logger.Info("ENDPROC_PROC_INS_MOBILE");

            }
            catch (Exception ex)
            {
                _logger.Info(ex.GetErrorMessage());
                command.Return_Code = "-1";

            }
        }
    }
}
