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
    public class UpdateDormitorySubcontrachCommandHandler : ICommandHandler<UpdateDormitorySubcontrachCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<string> _objService;
        private readonly IEntityRepository<FBBDORM_DORMITORY_MASTER> _FBBDormMaster;

        public UpdateDormitorySubcontrachCommandHandler(ILogger logger,
            IEntityRepository<string> objService, IEntityRepository<FBBDORM_DORMITORY_MASTER> FBBDormMaster)
        {
            _logger = logger;
            _objService = objService;
            _FBBDormMaster = FBBDormMaster;
        }

        public void Handle(UpdateDormitorySubcontrachCommand command)
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

                var executeResult = _objService.ExecuteStoredProc("WBB.PKG_FBBDORM_ADMIN001.PROC_EDIT_SUBCONTRACT",
                out paramOut,
                  new
                  {
                      p_user = command.User.ToSafeString(),
                      p_dormitory_mane_th = command.DORMITORY_NAME_TH.ToSafeString(),
                      p_subcontract_code = command.SUB_CONTRACT_LOCATION_CODE.ToSafeString(),
                      p_subcontract_name_th = command.SUB_CONTRACT_NAME_TH.ToSafeString(),
                      p_subcontract_name_en = command.SUB_CONTRACT_NAME_EN.ToSafeString(),
                      p_price_sub = command.PRICE_INSTALL.ToSafeString(),

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