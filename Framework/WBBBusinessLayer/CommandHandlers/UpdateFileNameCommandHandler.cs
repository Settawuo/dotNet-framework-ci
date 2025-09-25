using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using WBBBusinessLayer.QueryHandlers;
using WBBContract;
using WBBContract.Commands;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Models;

namespace WBBBusinessLayer.CommandHandlers
{
    class UpdateFileNameCommandHandler : ICommandHandler<UpdateFileNameCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<string> _objService;
        private readonly IAirNetEntityRepository<string> _objAirService;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _interfaceLog;

        public UpdateFileNameCommandHandler(ILogger logger
            , IEntityRepository<string> objService
            , IAirNetEntityRepository<string> objAirService
            , IWBBUnitOfWork uow
            , IEntityRepository<FBB_INTERFACE_LOG> interfaceLog)
        {
            _logger = logger;
            _objService = objService;
            _objAirService = objAirService;
            _uow = uow;
            _interfaceLog = interfaceLog;
        }

        public void Handle(UpdateFileNameCommand command)
        {
            InterfaceLogCommand log = null;
            log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _interfaceLog, command, command.Transaction_Id, "UpdateFileNameCommand", "UpdateFileName", null, "FBB|" + command.FullUrl, "");
            try
            {
                var outp = new List<object>();
                var paramOut = outp.ToArray();

                var ret_code = new OracleParameter();
                ret_code.OracleDbType = OracleDbType.Decimal;
                ret_code.Direction = ParameterDirection.Output;

                var ret_msg = new OracleParameter();
                ret_msg.OracleDbType = OracleDbType.Varchar2;
                ret_msg.Size = 2000;
                ret_msg.Direction = ParameterDirection.Output;

                var executeResult = _objService.ExecuteStoredProc("WBB.PKG_FBBOR004.PROC_UPDATE_FILE_LOG",
                    out paramOut,
                       new
                       {
                           p_order_no = command.OrderNo,
                           p_file_name = command.FileName,

                           ret_code = ret_code,
                           ret_msg = ret_msg
                       });
                command.WBB_returnCode = Convert.ToInt32(ret_code.Value == null ? "0" : ret_code.Value.ToString());
                command.WBB_returnMsg = ret_msg.Value == null ? "" : ret_msg.Value.ToString();

                executeResult = _objAirService.ExecuteStoredProc("AIR_ADMIN.PKG_AIROR905.UPDATE_FILE",
                    out paramOut,
                       new
                       {
                           p_order_no = command.OrderNo,
                           p_file_name = command.FileName,

                           ret_code = ret_code,
                           ret_msg = ret_msg
                       });
                command.AIRadmin_returnCode = Convert.ToInt32(ret_code.Value == null ? "0" : ret_code.Value.ToString());
                command.AIRadmin_returnMsg = ret_msg.Value == null ? "" : ret_msg.Value.ToString();

                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _interfaceLog, command, log, "Success", "", "");
            }
            catch (Exception ex)
            {
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _interfaceLog, command, log, "Failed", ex.Message, "");
            }
        }

    }
}
