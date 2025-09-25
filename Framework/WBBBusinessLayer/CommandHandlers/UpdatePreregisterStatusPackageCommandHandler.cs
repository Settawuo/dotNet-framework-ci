using Oracle.ManagedDataAccess.Client;
using System;
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
    public class UpdatePreregisterStatusPackageCommandHandler : ICommandHandler<UpdatePreregisterStatusPackageCommand>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<string> _objService;

        public UpdatePreregisterStatusPackageCommandHandler(ILogger logger,
            IWBBUnitOfWork uow,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IEntityRepository<string> objService)
        {
            _logger = logger;
            _uow = uow;
            _objService = objService;
            _intfLog = intfLog;
        }

        public void Handle(UpdatePreregisterStatusPackageCommand command)
        {
            InterfaceLogCommand log = null;
            try
            {
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, command, command.p_refference_no, "UpdatePreregisterStatus", "WBB", command.p_refference_no, "FBB", "WEB");

                var retCode = new OracleParameter
                {
                    OracleDbType = OracleDbType.Decimal,
                    Direction = ParameterDirection.Output
                };

                var retMessage = new OracleParameter
                {
                    OracleDbType = OracleDbType.Varchar2,
                    Size = 200,
                    Direction = ParameterDirection.Output
                };

                object[] paramOut;

                _objService.ExecuteStoredProc("WBB.PKG_FBBOR021.PROC_UPD_ORDER_STATUS",
                    out paramOut,
                    new
                    {
                        command.p_refference_no,
                        command.p_status,

                        //return code
                        return_code = retCode,
                        return_message = retMessage
                    });

                command.return_code = retCode.Value != null ? Convert.ToInt32(retCode.Value.ToSafeString()) : -1;
                command.return_message = retMessage.Value.ToSafeString();

                if (command.return_code == 0)
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, command, log, "Success", "", "");
                else
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, command, log, "Failed", command.return_message, "");
            }
            catch (Exception ex)
            {

                _logger.Info("Error occured when handle UpdatePreregisterStatusPackageCommandHandler");
                _logger.Info(ex.GetErrorMessage());
                _logger.Info(ex.StackTrace);
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex, log, "Failed", ex.GetErrorMessage(), "");

            }
        }
    }
}
