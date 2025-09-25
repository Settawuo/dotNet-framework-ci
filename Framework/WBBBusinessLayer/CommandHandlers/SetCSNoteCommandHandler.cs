using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
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
    public class SetCSNoteCommandHandler : ICommandHandler<SetCSNoteCommand>
    {
        private readonly ILogger _logger;
        private readonly IAirNetEntityRepository<string> _objService;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;

        public SetCSNoteCommandHandler(ILogger logger,
            IAirNetEntityRepository<string> objService,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IWBBUnitOfWork uow)
        {
            _logger = logger;
            _objService = objService;
            _intfLog = intfLog;
            _uow = uow;
        }

        public void Handle(SetCSNoteCommand command)
        {

            InterfaceLogCommand log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, command, command.in_order_no,
                    "SetCSNoteCommand", "SetCSNoteCommandHandler", command.in_order_no,
                    "FBB|", "WEB");
            try
            {
                var out_return_code = new OracleParameter();
                out_return_code.OracleDbType = OracleDbType.Decimal;
                out_return_code.ParameterName = "out_return_code";
                out_return_code.Direction = ParameterDirection.Output;

                var out_return_error = new OracleParameter();
                out_return_error.OracleDbType = OracleDbType.Varchar2;
                out_return_error.ParameterName = "out_return_error";
                out_return_error.Size = 2000;
                out_return_error.Direction = ParameterDirection.Output;

                var outp = new List<object>();
                var paramOut = outp.ToArray();

                var executeResult = _objService.ExecuteStoredProc("AIR_ADMIN.PKG_AIRDOCNOTI.SET_CS_NOTE",
                    out paramOut,
                    new
                    {
                        //in 
                        in_order_no = command.in_order_no.ToSafeString(),
                        in_cs_note = command.in_cs_note.ToSafeString(),
                        p_user = command.in_p_user.ToSafeString(),
                        /// Out
                        out_return_code = out_return_code,
                        out_return_error = out_return_error

                    });
                command.out_return_code = out_return_code.Value.ToString() == "null" ? "0" : out_return_code.Value.ToString();
                command.out_return_error = out_return_error.Value.ToString() == "null" ? "" : out_return_error.Value.ToSafeString();

                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, command, log, "Success", "", "");

            }
            catch (Exception ex)
            {
                _logger.Info(ex.Message);

                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex, log, "Failed", ex.GetErrorMessage(),
                    "");

                command.out_return_code = "-1";
                command.out_return_error = "No data found.";
            }
        }
    }

    public class SetCustomerVerificationCommandHandler : ICommandHandler<SetCustomerVerificationCommand>
    {
        private readonly ILogger _logger;
        private readonly IAirNetEntityRepository<string> _objService;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;

        public SetCustomerVerificationCommandHandler(ILogger logger,
            IAirNetEntityRepository<string> objService,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IWBBUnitOfWork uow)
        {
            _logger = logger;
            _objService = objService;
            _intfLog = intfLog;
            _uow = uow;
        }

        public void Handle(SetCustomerVerificationCommand command)
        {

            InterfaceLogCommand log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, command, command.OrderNo,
                    "SetCustomerVerification", "SetCustomerVerificationCommandHandler", command.OrderNo,
                    "FBB|", "WEB");
            try
            {
                var return_code = new OracleParameter();
                return_code.OracleDbType = OracleDbType.Varchar2;
                return_code.ParameterName = "return_code";
                return_code.Size = 2000;
                return_code.Direction = ParameterDirection.Output;

                var return_msg = new OracleParameter();
                return_msg.OracleDbType = OracleDbType.Varchar2;
                return_msg.ParameterName = "return_msg";
                return_msg.Size = 2000;
                return_msg.Direction = ParameterDirection.Output;

                var outp = new List<object>();
                var paramOut = outp.ToArray();

                var executeResult = _objService.ExecuteStoredProc("AIR_ADMIN.PKG_AIRDOCNOTI.SET_CUSTOMER_VERIFICATION",
                    out paramOut,
                    new
                    {
                        //in 
                        p_OrderNo = command.OrderNo.ToSafeString(),
                        p_CustomerPurge = command.CustomerPurge.ToSafeString(),
                        p_ExceptEntryFee = command.ExceptEntryFee.ToSafeString(),
                        p_SecondInstallation = command.SecondInstallation.ToSafeString(),

                        /// Out
                        return_code = return_code,
                        return_msg = return_msg

                    });
                command.return_code = return_code.Value.ToString() == "null" ? "0" : return_code.Value.ToString();
                command.return_msg = return_msg.Value.ToString() == "null" ? "" : return_msg.Value.ToSafeString();

                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, command, log, "Success", "", "");

            }
            catch (Exception ex)
            {
                _logger.Info(ex.Message);

                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex, log, "Failed", ex.GetErrorMessage(),
                    "");

                command.return_code = "-1";
                command.return_msg = "No data found.";
            }
        }
    }
}
