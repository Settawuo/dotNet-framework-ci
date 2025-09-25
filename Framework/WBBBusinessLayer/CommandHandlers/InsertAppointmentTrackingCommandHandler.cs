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
    public class InsertAppointmentTrackingCommandHandler : ICommandHandler<InsertAppointmentTrackingCommand>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IAirNetEntityRepository<string> _objService;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _interfaceLog;

        public InsertAppointmentTrackingCommandHandler(ILogger logger, IWBBUnitOfWork uow, IAirNetEntityRepository<string> objService, IEntityRepository<FBB_INTERFACE_LOG> interfaceLog)
        {
            _logger = logger;
            _uow = uow;
            _objService = objService;
            _interfaceLog = interfaceLog;
        }

        public void Handle(InsertAppointmentTrackingCommand command)
        {
            InterfaceLogCommand log = null;
            _logger.Info("InsertAppointmentTrackingCommandHandler Start");

            try
            {
                var output_return_code = new OracleParameter();
                output_return_code.OracleDbType = OracleDbType.Decimal;
                output_return_code.Direction = ParameterDirection.Output;

                var output_return_message = new OracleParameter();
                output_return_message.OracleDbType = OracleDbType.Varchar2;
                output_return_message.Size = 2000;
                output_return_message.Direction = ParameterDirection.Output;

                var outp = new List<object>();
                var paramOut = outp.ToArray();

                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _interfaceLog, command, "", "PROC_GET_APPOINTMENT", "InsertAppointmentTrackingCommand", command.id_card_no, "FBB|" + command.FullUrl, "");

                var execute = _objService.ExecuteStoredProc("AIR_ADMIN.PKG_APPOINTMENT_DISPLAY.PROC_GET_APPOINTMENT",
                out paramOut,
                  new
                  {
                      p_order_no = command.order_no,
                      p_id_card_no = command.id_card_no,
                      p_non_mobile_no = command.non_mobile_no,
                      p_create_date_zte = command.create_date_zte,
                      p_appointment_date = command.appointment_date,
                      p_appointment_timeslot = command.appointment_timeslot,
                      /// Out
                      output_return_code = output_return_code,
                      output_return_message = output_return_message
                  });

                command.output_return_code = Convert.ToInt32(output_return_code.Value == null ? "0" : output_return_code.Value.ToString());
                command.output_return_message = output_return_message.Value == null ? "" : output_return_message.Value.ToString();

                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _interfaceLog, command, log, "Success", "", "");
                _logger.Info("End InsertAppointmentTrackingCommandHandler output msg : " + command.output_return_message);

            }
            catch (Exception ex)
            {
                _logger.Info(ex.Message);
                _logger.Info("Error call service AIR_ADMIN.PKG_APPOINTMENT_DISPLAY.PROC_GET_APPOINTMENT : " + ex.Message);
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _interfaceLog, ex, log, "Failed", ex.Message, "");
                command.output_return_code = -1;
                command.output_return_message = "Error call service AIR_ADMIN.PKG_APPOINTMENT_DISPLAY.PROC_GET_APPOINTMENT : " + ex.Message;
            }
        }
    }
}
