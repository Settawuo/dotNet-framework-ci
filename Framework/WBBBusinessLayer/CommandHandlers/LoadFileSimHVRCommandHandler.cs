using System;
using System.Data;
using System.Linq;
using WBBBusinessLayer.QueryHandlers;
using WBBContract;
using WBBContract.Commands;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using Npgsql;
using NpgsqlTypes;
using Oracle.ManagedDataAccess.Client;

namespace WBBBusinessLayer.CommandHandlers
{
    public class LoadFileSimHVRCommandHandler : ICommandHandler<LoadFileSimHVRCommand>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<object> _objService;
        private readonly IEntityRepository<FBB_INTERFACE_LOG_PAYG> _intfLog;

        public LoadFileSimHVRCommandHandler(ILogger logger,
            IWBBUnitOfWork uow,
            IEntityRepository<object> objService,
           IEntityRepository<FBB_INTERFACE_LOG_PAYG> intfLog)
        {
            _logger = logger;
            _uow = uow;
            _objService = objService;
            _intfLog = intfLog;
        }

        public void Handle(LoadFileSimHVRCommand command)
        {
            var ret_code = new OracleParameter();
            ret_code.ParameterName = "ret_code";
            ret_code.Size = 2000;
            ret_code.OracleDbType = OracleDbType.Varchar2;
            ret_code.Direction = ParameterDirection.Output;

            var ret_msg = new OracleParameter();
            ret_msg.ParameterName = "ret_msg";
            ret_msg.Size = 2000;
            ret_msg.OracleDbType = OracleDbType.Varchar2;
            ret_msg.Direction = ParameterDirection.Output;

            #region call pk p_load_file_sim
            InterfaceLogPayGCommand log3 = new InterfaceLogPayGCommand();
            log3 = InterfaceLogPayGServiceHelper.StartInterfaceLogPayG(_uow, _intfLog, command, "in_report_name [LoadFileSimHVRCommand]", "call package : PKG_FBBPAYG_LOAD_SIM.p_load_file_sim", "LoadFileSimHVRCommand", "", "FBBPAYGLoadSIM", "FBB_BATCH");
            try
            {

                var executeResults = _objService.ExecuteReadStoredProc("WBB.PKG_FBBPAYG_LOAD_SIM.p_load_file_sim",
                    new
                    {
                        //Parameter Output
                        ret_code,
                        ret_msg
                    }).FirstOrDefault();

                //Return
                command.RET_CODE = ret_code.Value.ToSafeString();
                command.RET_MSG = ret_msg.Value.ToSafeString();

                InterfaceLogPayGServiceHelper.EndInterfaceLogPayG(_uow, _intfLog, executeResults, log3, ret_code.Value.ToSafeString().Equals("0") ? "Success" : ret_msg.Value.ToSafeString(), "", "FBB_BATCH");
            }
            catch (Exception ex)
            {
                InterfaceLogPayGServiceHelper.EndInterfaceLogPayG(_uow, _intfLog, ex, log3, "Error", ex.ToSafeString(), "FBB_BATCH");
                _logger.Info("PKG_FBBPAYG_LOAD_SIM.p_load_file_sim : " + ex.GetErrorMessage());
            }
            #endregion
        }
    }
}