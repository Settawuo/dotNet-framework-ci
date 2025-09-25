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
    public class UpdateFbssConfigTblCommandHandler : ICommandHandler<UpdateFbssConfigTblCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<string> _objService;

        public UpdateFbssConfigTblCommandHandler(
            ILogger logger,
            IEntityRepository<string> objService)
        {
            _logger = logger;
            _objService = objService;

        }
        public void Handle(UpdateFbssConfigTblCommand command)
        {
            UpdateFbssConfigTbl_Result result = new UpdateFbssConfigTbl_Result();
            _logger.Info("Start PKG_FIXED_ASSET_LASTMILE.p_update_fbss_config_tbl");
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

                var executePassword = _objService.ExecuteStoredProc("WBB.PKG_FIXED_ASSET_LASTMILE.p_update_fbss_config_tbl",
             out paramOut,
               new
               {
                   command.con_type,
                   command.con_name,
                   command.display_val,
                   command.val1,
                   command.val2,
                   command.flag,
                   command.updated_by,

                   ret_code,
                   ret_msg

               });
                result.ret_code = ret_code.Value.ToSafeString();
                result.ret_message = ret_msg.Value.ToSafeString();
                _logger.Info("End p_update_foa_resend_error_log" + ret_msg);


            }
            catch (Exception ex)
            {
                result.ret_code = "1";
                result.ret_message = "ERROR" + ":" + ex.Message.ToString();
                _logger.Info("ERROR PKG_FIXED_ASSET_LASTMILE.p_update_fbss_config_tbl: " + ex.GetErrorMessage());

            }
        }
    }
}
