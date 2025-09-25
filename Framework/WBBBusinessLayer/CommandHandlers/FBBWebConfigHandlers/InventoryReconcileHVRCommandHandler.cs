//using Oracle.ManagedDataAccess.Client;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBBusinessLayer.QueryHandlers;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Commands.FBBWebConfigCommands;
using WBBContract.Queries.FBBWebConfigQueries.SAPOnline;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.CommandHandlers.FBBWebConfigHandlers
{

    public class InventoryReconcileHVRCommandHandler : ICommandHandler<ReconcileCPEHVRCommand>
    {
        private readonly ILogger _logger;
        private readonly IFBBHVREntityRepository<ReconcileCPEHVRCommand> _objService;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;


        public InventoryReconcileHVRCommandHandler(ILogger logger,
            IFBBHVREntityRepository<ReconcileCPEHVRCommand> objService,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IWBBUnitOfWork uow)
        {
            _logger = logger;
            _objService = objService;
            _intfLog = intfLog;
            _uow = uow;
        }


        public void Handle(ReconcileCPEHVRCommand command)
        {
            InterfaceLogCommand log = null;
            try
            {
                _logger.Info("Start InventoryReconcileHVRCommandHandler.");
                var p_report_name = new NpgsqlParameter();
                p_report_name.ParameterName = "p_report_name";
                p_report_name.NpgsqlDbType = NpgsqlDbType.Text;
                p_report_name.Direction = ParameterDirection.InputOutput;
                p_report_name.Value = "p_report_name";

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


                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, command, "", "P_RECONCILE_CPE", "InventoryReconcile", "", "FBB", "JOB");

                var execute = _objService.ExecuteStoredProcMultipleCursorNpgsql("fbbadm.pkg_fbb_reconcile_p_reconcile_cpe",
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
                    List<ReconcileCPEHVRReturn> respList = resp.DataTableToList<ReconcileCPEHVRReturn>();
                    command.RET_CUR_FILE = respList;
                }
                
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, execute, log, "Success", "", "");

                _logger.Info("End InventoryReconcileHVRCommandHandler.");

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
