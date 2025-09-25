using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using WBBContract;
using WBBContract.Commands.FBBWebConfigCommands;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
namespace WBBBusinessLayer.CommandHandlers.FBBWebConfigHandlers
{
    public class EditAddBuildingCommandHandler : ICommandHandler<EditAddBuildingCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<string> _objService;
        private readonly IEntityRepository<FBBDORM_DORMITORY_MASTER> _FBBDormMaster;

        public EditAddBuildingCommandHandler(ILogger logger,
            IEntityRepository<string> objService, IEntityRepository<FBBDORM_DORMITORY_MASTER> FBBDormMaster)
        {
            _logger = logger;
            _objService = objService;
            _FBBDormMaster = FBBDormMaster;
        }
        public void Handle(EditAddBuildingCommand command)
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
                _logger.Info("StartPROC_INS_MASTER");

                var executePassword = _objService.ExecuteStoredProc("WBB.PKG_FBBDORM_ADMIN001.PROC_EDIT_ADD_BUILDING",
                out paramOut,
                  new
                  {
                      p_dormitory_id = command.EditAdd_dormitory_ID.ToSafeString(),
                      p_building_th = command.EditAdd_building_th.ToSafeString(),
                      p_building_en = command.EditAdd_building_en.ToSafeString(),
                      p_room_amount = command.EditAdd_room_amount.ToSafeString(),
                      p_user = command.User.ToSafeString(),
                      p_return_code = ret_code,
                      p_return_message = ret_msg

                  });
                command.Edit_return_code = ret_code.Value.ToSafeString();
                command.Edit_return_msg = ret_msg.Value.ToSafeString();
                _logger.Info("EndPROC_INS_MASTER" + ret_msg);

            }
            catch (Exception ex)
            {
                _logger.Info(ex.GetErrorMessage());
                command.Edit_return_code = "-1";
                command.Edit_return_msg = "Error call SavePreregister Handler: " + ex.GetErrorMessage();
            }
        }
    }
}
