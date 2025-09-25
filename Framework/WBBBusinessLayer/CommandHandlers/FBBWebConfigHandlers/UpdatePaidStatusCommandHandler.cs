using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using WBBContract;
using WBBContract.Commands.FBBWebConfigCommands;
using WBBData.Repository;
using WBBEntity.Extensions;

namespace WBBBusinessLayer.CommandHandlers.FBBWebConfigHandlers
{
    public class UpdatePaidStatusCommandHandler : ICommandHandler<UpdatePaidStatusDataCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<string> _objService;

        public UpdatePaidStatusCommandHandler(ILogger logger,
            IEntityRepository<string> objService)
        {
            _logger = logger;
            _objService = objService;
        }

        public void Handle(UpdatePaidStatusDataCommand command)
        {
            try
            {
                var ret_code = new OracleParameter();
                ret_code.OracleDbType = OracleDbType.Decimal;
                ret_code.Direction = ParameterDirection.Output;

                var ret_msg = new OracleParameter();
                ret_msg.OracleDbType = OracleDbType.Varchar2;
                ret_msg.Size = 2000;
                ret_msg.Direction = ParameterDirection.Output;

                var outp = new List<object>();
                var paramOut = outp.ToArray();
                _logger.Info("StartPROC_UPDATE_PAID_STATUS");

                var executePassword = _objService.ExecuteStoredProc("WBB.PKG_FBBPAYG_UPDATEPAID_STD.p_update_paid_status",
                out paramOut,
                  new
                  {
                      p_fibrenet_id = command.FIBRENET_ID.ToSafeString(),
                      p_inv_no = command.INVOICE_NO.ToSafeString(),
                      p_po_no = command.PO_NO.ToSafeString(),
                      p_device_type = command.DEVICE_TYPE.ToSafeString(),
                      p_paid_st = command.PAID_ST.ToSafeString(),
                      p_remark = command.REMARK.ToSafeString(),
                      ret_code = ret_code,
                      ret_msg = ret_msg

                  });

                _logger.Info("EndPROC_UPDATE_PAID_STATUS" + ret_msg);

            }
            catch (Exception ex)
            {
                _logger.Info(ex.GetErrorMessage());
            }
        }
    }
}
