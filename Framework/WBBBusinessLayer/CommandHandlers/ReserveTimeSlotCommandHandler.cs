using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using WBBContract;
using WBBContract.Commands;
using WBBData.Repository;
using WBBEntity.Extensions;

namespace WBBBusinessLayer.CommandHandlers
{
    public class ReserveTimeSlotCommandHandler : ICommandHandler<ReserveTimeSlotCommand>
    {

        private readonly ILogger _logger;
        private readonly IEntityRepository<string> _objService;

        public ReserveTimeSlotCommandHandler(ILogger logger, IEntityRepository<string> objService)
        {
            _logger = logger;
            _objService = objService;
        }

        public void Handle(ReserveTimeSlotCommand command)
        {
            try
            {
                var ret_code = new OracleParameter();
                ret_code.OracleDbType = OracleDbType.Decimal;
                ret_code.Direction = ParameterDirection.Output;

                var outp = new List<object>();
                var paramOut = outp.ToArray();

                var executeResult = _objService.ExecuteStoredProc("WBB.PKG_FBB_TIME_SLOT.RESERVE_TIME_SLOT",
                out paramOut,
                  new
                  {
                      p_guid = command.IdReserve.ToString(),
                      p_time_slot_id = command.TimeSlotId,
                      p_reserve_dtm = command.ReserveDTM,

                      //  return code
                      ret_code = ret_code

                  });

                command.Return_Code = 1;
            }
            catch (Exception ex)
            {
                _logger.Info(ex.GetErrorMessage());
                _logger.Info(ex.RenderExceptionMessage());

                command.Return_Message = ex.GetErrorMessage();
                command.Return_Code = -1;
            }
        }
    }
}
