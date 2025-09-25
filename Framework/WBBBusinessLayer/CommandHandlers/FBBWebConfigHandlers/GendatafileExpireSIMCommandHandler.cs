using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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
    public class GendatafileExpireSIMCommandHandler : ICommandHandler<GendatafileExpireSIMCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_INTERFACE_LOG_PAYG> _intfLog;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<listReturnData> _objService;
        public GendatafileExpireSIMCommandHandler(ILogger logger,
           IWBBUnitOfWork uow, IEntityRepository<listReturnData> objService,
           IEntityRepository<FBB_INTERFACE_LOG_PAYG> intfLog)
        {
            _logger = logger;
            _uow = uow;
            _intfLog = intfLog;
            _objService = objService;
        }
        public void Handle(GendatafileExpireSIMCommand command)
        {
            InterfaceLogPayGCommand log = new InterfaceLogPayGCommand();
            log = InterfaceLogPayGServiceHelper.StartInterfaceLogPayG(_uow, _intfLog, command, "in_report_name [" + command.in_report_name + "]", "call package : PKG_FBBPAYG_REPORT.p_get_datafile", "GenfileViewOnHandsCommandHandler", "", "FBBViewOnHandsBatch", "FBB_BATCH");

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


                var return_report_data_list_cur = new OracleParameter();
                return_report_data_list_cur.ParameterName = "return_report_data_list_cur";
                return_report_data_list_cur.OracleDbType = OracleDbType.RefCursor;
                return_report_data_list_cur.Size = 2000;
                return_report_data_list_cur.Direction = ParameterDirection.Output;

                //var outp = new List<object>();
                //var paramOut = outp.ToArray();
                List<listReturnData> executeResults = _objService.ExecuteReadStoredProc("WBB.PKG_FBBPAYG_REPORT.p_get_datafile",
                    new
                    {
                        in_report_name,
                        ret_code,
                        ret_msg,
                        return_report_data_list_cur
                    }).ToList();



                command.ret_code = ret_code.Value.ToSafeString();
                command.ret_msg = ret_msg.Value.ToSafeString();
                command.return_report_data_list_cur = executeResults;



                //+InterfaceLogPayGServiceHelper.EndInterfaceLogPayG(_uow, _intfLog, executeResults, log, ret_code.Value.ToSafeString().Equals("0") ? "Success" : ret_msg.Value.ToSafeString(), "", "FBB_BATCH");
            }
            catch (Exception ex)
            {
                InterfaceLogPayGServiceHelper.EndInterfaceLogPayG(_uow, _intfLog, ex, log, "Error", ex.ToSafeString(), "FBB_BATCH");
                _logger.Info(ex.GetErrorMessage());
            }

        }
    }
}
