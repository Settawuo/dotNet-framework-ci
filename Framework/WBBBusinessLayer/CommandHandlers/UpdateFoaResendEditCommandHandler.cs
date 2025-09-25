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
    public class UpdateFoaResendEditCommandHandler : ICommandHandler<UpdateFoaResendEditCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<string> _objService;

        public UpdateFoaResendEditCommandHandler(ILogger logger, IEntityRepository<string> objService)
        {
            _logger = logger;
            _objService = objService;
        }

        public void Handle(UpdateFoaResendEditCommand command)
        {
            try
            {

                _logger.Info("== Start Call UpdateFoaResendEditCommandHandler ==");
                var ret_code = new OracleParameter();
                ret_code.OracleDbType = OracleDbType.Decimal;
                ret_code.Direction = ParameterDirection.Output;

                var ret_msg = new OracleParameter();
                ret_msg.OracleDbType = OracleDbType.Varchar2;
                ret_msg.Size = 2000;
                ret_msg.Direction = ParameterDirection.Output;

                var outp = new List<object>();
                var paramOut = outp.ToArray();

                var execute = _objService.ExecuteStoredProc("WBB.PKG_FBB_FOA_ORDER_MANAGEMENT.p_update_foa_resend_edit",
                out paramOut,
                  new
                  {
                      p_serial_no = command.p_serial_no.ToSafeString(),
                      p_order_no = command.p_order_no.ToSafeString(),
                      p_trans_id = command.p_trans_id.ToSafeString(),
                      p_plant = command.p_plant.ToSafeString(),
                      p_storage_location = command.p_storage_location.ToSafeString(),
                      p_access_number = command.p_access_number.ToSafeString(),

                      ret_code = ret_code,
                      ret_msg = ret_msg
                  });

                command.return_code = Convert.ToInt32(ret_code.Value == null ? "0" : ret_code.Value.ToString());
                command.return_message = ret_msg.Value == null ? "" : ret_msg.Value.ToString();

                _logger.Info("== End Call UpdateFoaResendEditCommandHandler ==");

            }
            catch (Exception ex)
            {
                _logger.Info(ex.GetErrorMessage());
                command.return_code = -1;
                command.return_message = "Error call UpdateFoaResendEditCommandHandler: " + ex.GetErrorMessage();
            }
        }
    }
}
