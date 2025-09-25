using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using WBBContract;
using WBBContract.Commands;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;

namespace WBBBusinessLayer.CommandHandlers.FBSS
{
    public class UpdateRegisterCommandHandler : ICommandHandler<UpdateRegisterCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<string> _objService;
        private readonly IEntityRepository<FBB_REGISTER> _FBBRegister;

        public UpdateRegisterCommandHandler(ILogger logger, IEntityRepository<string> objService, IEntityRepository<FBB_REGISTER> FBBRegister)
        {
            _logger = logger;
            _objService = objService;
            _FBBRegister = FBBRegister;
        }

        public void Handle(UpdateRegisterCommand command)
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
                _logger.Info("StartPROC_INS_REGISTER");

                var executePassword = _objService.ExecuteStoredProc("WBB.PKG_FBBDORM_FBBDORM008.PROC_INS_REGISTER",
                out paramOut,
                  new
                  {
                      p_customer_name = command.p_customer_name.ToSafeString(),
                      p_customer_lastname = command.p_customer_lastname.ToSafeString(),
                      p_card_type = command.p_card_type.ToSafeString(),
                      p_card_no = command.p_card_no.ToSafeString(),
                      p_mobile_no = command.p_mobile_no.ToSafeString(),
                      P_fbbdorm_order_no = command.p_fbbdorm_order_no.ToSafeString(),
                      p_prepaid_non_mobile = command.p_prepaid_non_mobile.ToSafeString(),
                      P_TIME_SLOT = command.p_time_slot.ToSafeString(),
                      P_address_id = command.p_address_id.ToSafeString(),
                      P_service_code = command.p_service_code.ToSafeString(),
                      P_EVENT_CODE = command.p_event_code.ToSafeString(),
                      P_UPDATED_BY = command.p_updated_by.ToSafeString(),
                      P_IN_NO = command.p_in_no.ToSafeString(),
                      ret_code = ret_code,
                      ret_msg = ret_msg

                  });
                command.ret_code = 1;
                command.ret_msg = "Success";
                ////
                _logger.Info("EndPROC_INS_REGISTER" + command.ret_msg);


            }
            catch (Exception ex)
            {
                _logger.Info(ex.GetErrorMessage());
                command.ret_code = -1;
                command.ret_msg = "Error call SavePreregister Handler: " + ex.GetErrorMessage();
            }
        }
    }
}
