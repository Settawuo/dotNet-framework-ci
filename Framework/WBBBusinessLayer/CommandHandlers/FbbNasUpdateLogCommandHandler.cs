using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using WBBBusinessLayer.QueryHandlers;
using WBBContract;
using WBBContract.Commands;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;

namespace WBBBusinessLayer.CommandHandlers
{
    public class FbbNasUpdateLogCommandHandler : ICommandHandler<FbbNasUpdateLogCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<string> _objService;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;
        private readonly ICommandHandler<SendSmsCommand> _sendSmsCommandHandler;
        private readonly IEntityRepository<FBB_CFG_LOV> _lov;
        private readonly Object _thisLock = new Object();

        public FbbNasUpdateLogCommandHandler(ILogger logger,
            IEntityRepository<string> objService,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IWBBUnitOfWork uow,
            ICommandHandler<SendSmsCommand> sendSmsCommandHandler,
            IEntityRepository<FBB_CFG_LOV> lov)
        {
            _logger = logger;
            _objService = objService;
            _intfLog = intfLog;
            _uow = uow;
            _sendSmsCommandHandler = sendSmsCommandHandler;
            _lov = lov;
        }

        public void Handle(FbbNasUpdateLogCommand command)
        {

            InterfaceLogCommand log = null;
            try
            {
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, command, command.file_name, "FbbNasUpdateLogCommandHandler", "FbbNasUpdateLogCommandHandler", command.file_name, "FBB", "WEB");

                var ret_code = new OracleParameter();
                ret_code.OracleDbType = OracleDbType.Int32;
                ret_code.ParameterName = "ret_code";
                ret_code.Direction = ParameterDirection.Output;

                var outp = new List<object>();
                var paramOut = outp.ToArray();

                var executeResult = _objService.ExecuteStoredProc("WBB.P_FBB_NAS_UPDATE_LOG",
                out paramOut,
                   new
                   {
                       //in 
                       p_FILE_NAME = command.file_name,
                       p_NAS_PATH = command.nas_path,
                       p_USERNAME = command.file_owner,
                       p_ACTION = command.action,
                       //out
                       ret_code = ret_code,


                   });
                //command.ret_msg = return_message.Value.ToSafeString();
                command.ret_code = ret_code.Value.ToSafeString() != "null" ? decimal.Parse(ret_code.Value.ToSafeString()) : 0;

                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ret_code, log, "Success", "", "");

            }
            catch (Exception ex)
            {
                _logger.Info(ex.Message);

                if (null != log)
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex, log, "Failed", ex.GetErrorMessage(), "");
                }

                command.ret_code = -1;
                //command.ret_msg = "Error save Campaign service " + ex.Message;
            }
        }
    }
}
