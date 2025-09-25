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

    public class InventoryReconcileCommandHandler : ICommandHandler<InventoryReconcileCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<InventoryReconcileCommand> _objService;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;


        public InventoryReconcileCommandHandler(ILogger logger,
            IEntityRepository<InventoryReconcileCommand> objService,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IWBBUnitOfWork uow)
        {
            _logger = logger;
            _objService = objService;
            _intfLog = intfLog;
            _uow = uow;
        }

        public void Handle(InventoryReconcileCommand command)
        {
            InterfaceLogCommand log = null;

            try
            {
                _logger.Info("Start InventoryReconcileCommandHandler.");

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

                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, command, "", "p_get_data", "InventoryReconcile", "", "FBB", "JOB");

                var execute = _objService.ExecuteStoredProc("WBB.PKG_FBBINVENTORY_RECONCILE.p_get_data",
                out paramOut,
                  new
                  {
                      ret_code = ret_code,
                      ret_msg = ret_msg
                  });

                result.Add(ret_code.Value != null ? ret_code.Value.ToSafeString() : "-1");
                result.Add(ret_msg.Value != null ? ret_msg.Value.ToSafeString() : "Failed");
                _logger.Info(string.Format("package -> {0} : {1}", ret_code.Value.ToSafeString(), ret_msg.Value.ToSafeString()));

                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, "Success", "", "");

                _logger.Info("End InventoryReconcileCommandHandler.");

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
