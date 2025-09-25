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
    public class UpdatePreregisterCommandHandler : ICommandHandler<UpdatePreregisterCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<string> _objService;

        public UpdatePreregisterCommandHandler(ILogger logger, IEntityRepository<string> objService)
        {
            _logger = logger;
            _objService = objService;
        }

        public void Handle(UpdatePreregisterCommand command)
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

                var execute = _objService.ExecuteStoredProc("WBB.PKG_FBBSALEPORTAL_PRE_REGISTER.PROC_UPD_INFO_PRE_REGISTER",
                out paramOut,
                  new
                  {
                      p_refference_no = command.p_refference_no.ToSafeString(),
                      p_is_contact_cust = command.p_is_contact_cust.ToSafeString(),
                      p_remark_for_contact_cust = command.p_remark_for_contact_cust.ToSafeString(),
                      p_is_in_cov = command.p_is_in_cov.ToSafeString(),
                      p_remark_for_no_cov = command.p_remark_for_no_cov.ToSafeString(),
                      p_closing_sale = command.p_closing_sale.ToSafeString(),
                      p_remark_for_no_reg = command.p_remark_for_no_reg.ToSafeString(),
                      p_user_name = command.p_user_name.ToSafeString(),
                      p_compleated_flag = command.p_compleated_flag.ToSafeString(),
                      //R20.12
                      //p_house_no = command.p_house_no.ToSafeString(),
                      //p_village_name = command.p_village_name.ToSafeString(),
                      //p_soi = command.p_soi.ToSafeString(),
                      //p_road = command.p_road.ToSafeString(),

                      return_code = ret_code,
                      return_message = ret_msg
                  });

                command.return_code = Convert.ToInt32(ret_code.Value == null ? "0" : ret_code.Value.ToString());
                command.return_message = ret_msg.Value == null ? "" : ret_msg.Value.ToString();


            }
            catch (Exception ex)
            {
                _logger.Info(ex.GetErrorMessage());
                command.return_code = -1;
                command.return_message = "Error call UpdatePreregisterCommandHandler: " + ex.GetErrorMessage();
            }
        }
    }
}
