using Npgsql;
using NpgsqlTypes;
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

    public class FBBPAYGReconcileFBSSHVRCommandHandler : ICommandHandler<FBBPAYGReconcileFBSSHVRCommand>
    {
        private readonly ILogger _logger;
        private readonly IFBBHVREntityRepository<FBBPAYGReconcileFBSSHVRCommand> _objService;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;


        public FBBPAYGReconcileFBSSHVRCommandHandler(ILogger logger,
            IFBBHVREntityRepository<FBBPAYGReconcileFBSSHVRCommand> objService,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IWBBUnitOfWork uow)
        {
            _logger = logger;
            _objService = objService;
            _intfLog = intfLog;
            _uow = uow;
        }

        public void Handle(FBBPAYGReconcileFBSSHVRCommand command)
        {
            InterfaceLogCommand log = null;

            try
            {
                _logger.Info("Start FBBPAYGReconcileFBSSCommandHandler.");

                var p_report_name = new NpgsqlParameter();
                p_report_name.ParameterName = "p_report_name";
                p_report_name.NpgsqlDbType = NpgsqlDbType.Text;
                p_report_name.Direction = ParameterDirection.InputOutput;
                p_report_name.Value = "FBBPAYG_RECONCILEFBSS_CPE";

                var multi_ret_code = new NpgsqlParameter();
                multi_ret_code.ParameterName = "multi_ret_code";
                multi_ret_code.NpgsqlDbType = NpgsqlDbType.Refcursor;
                multi_ret_code.Direction = ParameterDirection.InputOutput;
                multi_ret_code.Value = "multi_ret_code";

                var data_cur = new NpgsqlParameter();
                data_cur.ParameterName = "data_cur";
                data_cur.NpgsqlDbType = NpgsqlDbType.Refcursor;
                data_cur.Direction = ParameterDirection.InputOutput;
                data_cur.Value = "data_cur";

                var result = new List<string>();
                var outp = new List<object>();
                var paramOut = outp.ToArray();

                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, command, "", "P_RECONCILE_REPORT", "FBBPAYGReconcileFBSS", "", "FBB", "JOB");

                var execute = _objService.ExecuteStoredProcMultipleCursorNpgsql("fbbadm.pkg_fbb_reconcile_p_reconcile_report",
                  new object[]
                   {
                       //List
                       p_report_name,
                       data_cur,
                       multi_ret_code

                       //Return
                       //cur,
                       //multi_ret_code,
                       //ret_msg
                   });

                if (execute != null)
                {
                    DataTable resp = (DataTable)execute[1];
                    List<FBBPAYGReconcileFBSSHVRReturn> respList = resp.DataTableToList<FBBPAYGReconcileFBSSHVRReturn>();
                    command.RET_CUR_FILE = respList;
                }

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
