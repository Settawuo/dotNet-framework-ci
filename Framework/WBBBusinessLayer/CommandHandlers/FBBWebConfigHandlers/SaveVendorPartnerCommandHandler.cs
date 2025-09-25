using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Commands.FBBWebConfigCommands;
using WBBData.Repository;
using WBBEntity.Extensions;

namespace WBBBusinessLayer.CommandHandlers.FBBWebConfigHandlers
{
    public class SaveVendorPartnerCommandHandler : ICommandHandler<SaveVendorPartnerCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<string> _objService;

        public SaveVendorPartnerCommandHandler(ILogger logger,
            IEntityRepository<string> objService)
        {
            _logger = logger;
            _objService = objService;
        }

        public void Handle(SaveVendorPartnerCommand command)
        {
            InterfaceLogCommand log = null;
            try
            {
                var ret_code = new OracleParameter();
                ret_code.OracleDbType = OracleDbType.Decimal;
                ret_code.Direction = ParameterDirection.Output;

                var v_error_msg = new OracleParameter();
                v_error_msg.OracleDbType = OracleDbType.Varchar2;
                v_error_msg.Size = 2000;
                v_error_msg.Direction = ParameterDirection.Output;

                var outp = new List<object>();
                var paramOut = outp.ToArray();

                var executeResult = _objService.ExecuteStoredProc("WBB.PKG_FBBOR012.save_new_vendor_partner",
                out paramOut,
                  new
                  {
                      p_vendor_partner = command.vendor_partner.ToSafeString(),
                      p_user = command.user.ToSafeString(),

                      //  return code
                      o_return_code = ret_code,
                      o_return_msg = v_error_msg
                  });

            }
            catch (Exception ex)
            {
                _logger.Info(ex.Message);

                command.return_code = -1;
                command.return_msg = "Error call save VendorPartner service " + ex.Message;
            }
        }

    }
}
