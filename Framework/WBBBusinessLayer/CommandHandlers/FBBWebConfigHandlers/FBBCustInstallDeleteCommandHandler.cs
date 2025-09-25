using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using WBBBusinessLayer.QueryHandlers;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Commands.FBBWebConfigCommands;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;

namespace WBBBusinessLayer.CommandHandlers.FBBWebConfigHandlers
{
    class FBBCustInstallDeleteCommandHandler : ICommandHandler<FBBCustInstallDeleteCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBBCustInstallDeleteCommand> _objService;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;


        public FBBCustInstallDeleteCommandHandler(ILogger logger,
            IEntityRepository<FBBCustInstallDeleteCommand> objService,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IWBBUnitOfWork uow)
        {
            _logger = logger;
            _objService = objService;
            _intfLog = intfLog;
            _uow = uow;
        }

        public void Handle(FBBCustInstallDeleteCommand command)
        {
            InterfaceLogCommand log = null;

            try
            {
                _logger.Info("Start FBBCustInstallDeleteCommandHandler.");

                var ret_code = new OracleParameter();
                ret_code.OracleDbType = OracleDbType.Decimal;
                ret_code.Direction = ParameterDirection.Output;

                var ret_msg = new OracleParameter();
                ret_msg.OracleDbType = OracleDbType.Varchar2;
                ret_msg.Size = 2000;
                ret_msg.Direction = ParameterDirection.Output;

                var result = new List<string>();
                var outp = new List<object>();
                var paramOut = outp.ToArray();

                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, command, "", "p_get_ins_addr", "FBBCustInstallDelete", "", "FBB", "JOB");

                var execute = _objService.ExecuteStoredProc("WBB.pkg_fbb_cust_install_delete.p_get_ins_addr",
                out paramOut,
                  new
                  {
                      ret_code = ret_code,
                      ret_msg = ret_msg
                  });

                result.Add(ret_code.Value != null ? ret_code.Value.ToSafeString() : "-1");
                result.Add(ret_msg.Value != null ? ret_msg.Value.ToSafeString() : "Failed");
                _logger.Info(string.Format("package -> {0} : {1}", ret_code.Value.ToSafeString(), ret_msg.Value.ToSafeString()));

                command.RES_CODE = ret_code.Value != null ? ret_code.Value.ToSafeString() : "-1";
                command.RES_MESSAGE = ret_msg.Value != null ? ret_msg.Value.ToSafeString() : "-1";
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, "Success", "", "");

                _logger.Info("End FBBCustInstallDeleteCommandHandler.");

            }
            catch (Exception ex)
            {
                command.RES_CODE = "2";
                command.RES_MESSAGE = ex.Message;
                if (log != null)
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex, log, "Failed", ex.GetErrorMessage(), "");
                }
                _logger.Info(ex.GetErrorMessage());
            }
        }
    }
}
