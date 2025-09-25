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
    public class SaveDormMasterDetailCommandHandler : ICommandHandler<SaveDormMasterDetailCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<string> _objService;
        private readonly IEntityRepository<FBBDORM_DORMITORY_MASTER> _FBBDormMaster;

        public SaveDormMasterDetailCommandHandler(ILogger logger,
            IEntityRepository<string> objService, IEntityRepository<FBBDORM_DORMITORY_MASTER> FBBDormMaster)
        {
            _logger = logger;
            _objService = objService;
            _FBBDormMaster = FBBDormMaster;
        }
        public void Handle(SaveDormMasterDetailCommand command)
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

                var executePassword = _objService.ExecuteStoredProc("WBB.PKG_FBBDORM_FBBDORM12.PROC_IN_MASTER",
                out paramOut,
                  new
                  {
                      p_dormitory_name = command.SaveDT_dormitory_name.ToSafeString(),
                      p_building_name = command.SaveDT_dormitory_no_th.ToSafeString(),
                      p_address_id = command.SaveDT_addressid.ToSafeString(),
                      p_subcontract_code = command.SaveDT_subcontract_code.ToSafeString(),
                      p_subcontract_name_th = command.SaveDT_subcontractTH.ToSafeString(),
                      p_subcontract_name_en = command.SaveDT_subcontractEN.ToSafeString(),
                      p_user = command.SaveDT_User.ToSafeString(),
                      p_return_code = ret_code,
                      p_return_message = ret_msg

                  });
                command.SaveDT_return_code = 1;
                command.SaveDT_return_msg = "Success";

                _logger.Info("EndPROC_INS_MASTER" + command.SaveDT_return_msg);

            }
            catch (Exception ex)
            {
                _logger.Info(ex.GetErrorMessage());
                command.SaveDT_return_code = -1;
                command.SaveDT_return_msg = "Error call SavePreregister Handler: " + ex.GetErrorMessage();
            }
        }
    }
}
