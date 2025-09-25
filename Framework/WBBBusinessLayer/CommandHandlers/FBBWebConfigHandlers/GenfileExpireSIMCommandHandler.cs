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
    public class GenfileExpireSIMCommandHandler : ICommandHandler<GenfileExpireSIMCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_INTERFACE_LOG_PAYG> _intfLog;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<string> _objService;
        public GenfileExpireSIMCommandHandler(ILogger logger,
           IWBBUnitOfWork uow, IEntityRepository<string> objService,
           IEntityRepository<FBB_INTERFACE_LOG_PAYG> intfLog)
        {
            _logger = logger;
            _uow = uow;
            _intfLog = intfLog;
            _objService = objService;
        }
        public void Handle(GenfileExpireSIMCommand command)
        {
            InterfaceLogPayGCommand log = new InterfaceLogPayGCommand();
            log = InterfaceLogPayGServiceHelper.StartInterfaceLogPayG(_uow, _intfLog, command, "in_report_name [" + command.in_report_name + "]", "call package : PKG_FBBPAYG_REPORT.p_gen_file", "GenfileViewOnHandsCommandHandler", "", "FBBViewOnHandsBatch", "FBB_BATCH");
            try
            {
                var in_report_name = new OracleParameter();
                in_report_name.ParameterName = "in_report_name";
                in_report_name.OracleDbType = OracleDbType.Varchar2;
                in_report_name.Size = 1000;
                in_report_name.Direction = ParameterDirection.Input;
                in_report_name.Value = command.in_report_name.ToSafeString();

                var ret_code = new OracleParameter();
                ret_code.OracleDbType = OracleDbType.Decimal;
                ret_code.Direction = ParameterDirection.Output;

                var ret_msg = new OracleParameter();
                ret_msg.OracleDbType = OracleDbType.Varchar2;
                ret_msg.Size = 2000;
                ret_msg.Direction = ParameterDirection.Output;

                var outp = new List<object>();
                var paramOut = outp.ToArray();
                var executeResults = _objService.ExecuteStoredProc("WBB.PKG_FBBPAYG_REPORT.p_gen_file",
                    out paramOut,
                    new
                    {
                        in_report_name,
                        ret_code,
                        ret_msg
                    });
                command.ret_code = ret_code.Value.ToSafeString();
                command.ret_msg = ret_msg.Value.ToSafeString();

                InterfaceLogPayGServiceHelper.EndInterfaceLogPayG(_uow, _intfLog, executeResults, log, ret_code.Value.ToSafeString().Equals("0") ? "Success" : ret_msg.Value.ToSafeString(), "", "FBB_BATCH");
            }
            catch (Exception ex)
            {
                InterfaceLogPayGServiceHelper.EndInterfaceLogPayG(_uow, _intfLog, ex, log, "Error", ex.ToSafeString(), "FBB_BATCH");
                _logger.Info(ex.GetErrorMessage());
            }

        }
    }
}
