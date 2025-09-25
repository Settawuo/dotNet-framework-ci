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

    public class FBBPAYGReconcileFBSSCommandHandler : ICommandHandler<FBBPAYGReconcileFBSSCommand>
    {
        private readonly ILogger _logger;
        private readonly IFBBShareplexEntityRepository<FBBPAYGReconcileFBSSCommand> _objService;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;


        public FBBPAYGReconcileFBSSCommandHandler(ILogger logger,
            IFBBShareplexEntityRepository<FBBPAYGReconcileFBSSCommand> objService,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IWBBUnitOfWork uow)
        {
            _logger = logger;
            _objService = objService;
            _intfLog = intfLog;
            _uow = uow;
        }

        public void Handle(FBBPAYGReconcileFBSSCommand command)
        {
            InterfaceLogCommand log = null;

            try
            {
                _logger.Info("Start FBBPAYGReconcileFBSSCommandHandler.");

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

                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, command, "", "P_RECONCILE_REPORT", "FBBPAYGReconcileFBSS", "", "FBB", "JOB");

                var execute = _objService.ExecuteStoredProc("FBBADM.PKG_FBB_RECONCILE.P_RECONCILE_REPORT",
                out paramOut,
                  new
                  {
                      p_report_name = command.p_report_name,
                      ret_code = ret_code,
                      ret_msg = ret_msg
                  });

                result.Add(ret_code.Value != null ? ret_code.Value.ToSafeString() : "-1");
                result.Add(ret_msg.Value != null ? ret_msg.Value.ToSafeString() : "Failed");
                command.RET_CODE = ret_code.Value != null ? ret_code.Value.ToSafeString() : "-1";
                command.RET_MSG = ret_msg.Value != null ? ret_msg.Value.ToSafeString() : "Failed";
                _logger.Info(string.Format("package -> {0} : {1}", ret_code.Value.ToSafeString(), ret_msg.Value.ToSafeString()));

                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, "Success", "", "");

                _logger.Info("End FBBPAYGReconcileFBSSCommandHandler.");

            }
            catch (Exception ex)
            {
                if (log != null)
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex, log, "Failed", ex.GetErrorMessage(), "");
                }
                _logger.Info(ex.GetErrorMessage());
            }
        }
    }
}
