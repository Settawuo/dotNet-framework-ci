using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WBBContract;
using WBBContract.Commands.FBBWebConfigCommands;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Models;
using WBBEntity.Extensions;
using WBBEntity.PanelModels.FBBWebConfigModels;
using Oracle.DataAccess.Client;
using System.Data;
using Oracle.DataAccess.Types;

namespace WBBBusinessLayer.CommandHandlers.FBBWebConfigHandlers
{
    public class UpdateFbssConfigTblWithOMCommandHandler : ICommandHandler<UpdateFbssConfigTblWithOMCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<string> _objService;

        public UpdateFbssConfigTblWithOMCommandHandler(
            ILogger logger,
            IEntityRepository<string> objService)
        {
            _logger = logger;
            _objService = objService;

        }
        public void Handle(UpdateFbssConfigTblWithOMCommand command)
        {
            UpdateFbssConfigTblWithOM_Result result = new UpdateFbssConfigTblWithOM_Result();
            _logger.Info("Start PKG_FBB_FOA_ORDER_MANAGEMENT.p_update_fbss_config_tbl");
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

                var executePassword = _objService.ExecuteStoredProc("WBB.PKG_FBB_FOA_ORDER_MANAGEMENT.p_update_fbss_config_tbl",
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
                _logger.Info("ERROR PKG_FBB_FOA_ORDER_MANAGEMENT.p_update_fbss_config_tbl: " + ex.GetErrorMessage());

            }
        }
    }
}
