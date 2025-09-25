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
    public class ImportPaidStatusCommandHandler : ICommandHandler<ImportPaidStatusCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<string> _objService;

        public ImportPaidStatusCommandHandler(ILogger logger,
            IEntityRepository<string> objService)
        {
            _logger = logger;
            _objService = objService;
        }

        public void Handle(ImportPaidStatusCommand command)
        {
            try
            {
                var ret_code = new OracleParameter();
                ret_code.OracleDbType = OracleDbType.Decimal;
                ret_code.Direction = ParameterDirection.Output;

                var e_msg = new OracleParameter();
                e_msg.OracleDbType = OracleDbType.Varchar2;
                e_msg.Size = 2000;
                e_msg.Direction = ParameterDirection.Output;

                var outp = new List<object>();
                var paramOut = outp.ToArray();
                _logger.Info("StartPROC_IMPORT_PAID_STATUS");

                var executePassword = _objService.ExecuteStoredProc("WBB.PKG_FBBPAYG_UPDATEPAID_STD.Import_file_csv",
                out paramOut,
                  new
                  {
                      //result = result,
                      p_fibrenet_id = command.FIBRENET_ID.ToSafeString(),
                      p_invoice_no = command.INVOICE_NO.ToSafeString(),
                      p_invoice_dt = (command.INVOICE_DT != null) ? command.INVOICE_DT.Value.Date.ToString("dd/MM/yyyy") : "",
                      p_po_no = command.PO_NO.ToSafeString(),
                      p_paid_st = command.PAID_ST.ToSafeString(),
                      p_device_type = command.DEVICE_TYPE.ToSafeString(),
                      p_remark = command.REMARK.ToSafeString(),
                      ret_code = ret_code,
                      e_msg = e_msg

                  });

                command.Return_Code = ret_code.Value.ToSafeString();
                command.Return_Message = e_msg.Value.ToSafeString();

                _logger.Info("EndPROC_IMPORT_PAID_STATUS");

            }
            catch (Exception ex)
            {
                _logger.Info(ex.GetErrorMessage());
            }
        }
    }
}
