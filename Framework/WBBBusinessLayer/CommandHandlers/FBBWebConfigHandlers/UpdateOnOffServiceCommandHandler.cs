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
    public class UpdateOnOffServiceCommandHandler : ICommandHandler<UpdateOnOffServiceCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<string> _objService;
        //   private readonly IEntityRepository<FBBDORM_DORMITORY_MASTER> _FBBDormMaster;

        public UpdateOnOffServiceCommandHandler(ILogger logger, IEntityRepository<string> objService)
        {
            _logger = logger;
            _objService = objService;
            //   _FBBDormMaster = FBBDormMaster;
        }

        public void Handle(UpdateOnOffServiceCommand command)
        {
            try
            {
                var p_return_code = new OracleParameter();
                p_return_code.OracleDbType = OracleDbType.Decimal;
                p_return_code.Direction = ParameterDirection.Output;

                var ret_msg = new OracleParameter();
                ret_msg.OracleDbType = OracleDbType.Varchar2;
                ret_msg.Size = 2000;
                ret_msg.Direction = ParameterDirection.Output;

                var outp = new List<object>();
                var paramOut = outp.ToArray();
                _logger.Info("StartPROC_INS_MASTER");

                var executeResult = _objService.ExecuteStoredProc("WBB.PKG_FBBDORM_ADMIN001.PROC_ON_OFF_WEB_DORMITORY",
                out paramOut,
                  new
                  {

                      p_dormitory_name = command.p_dormitory_name.ToSafeString(),
                      p_building = command.p_building.ToSafeString(),
                      p_status = command.p_status.ToSafeString(),
                      p_user = command.User.ToSafeString(),
                      p_return_code = p_return_code,
                      p_return_message = ret_msg

                  });
                command.Result = p_return_code.Value != null ? Convert.ToInt32(p_return_code.Value.ToSafeString()) : -1;
                _logger.Info("EndPROC_INS_MASTER" + ret_msg);

            }

            catch (Exception ex)
            {
                command.Result = -1;
                _logger.Info(ex.GetErrorMessage());

            }
        }
    }
}