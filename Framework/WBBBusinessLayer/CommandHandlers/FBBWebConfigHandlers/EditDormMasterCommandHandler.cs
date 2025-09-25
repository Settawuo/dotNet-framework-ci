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
    public class EditDormMasterCommandHandler : ICommandHandler<EditDormMasterCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<string> _objService;
        private readonly IEntityRepository<FBBDORM_DORMITORY_MASTER> _FBBDormMaster;

        public EditDormMasterCommandHandler(ILogger logger,
            IEntityRepository<string> objService, IEntityRepository<FBBDORM_DORMITORY_MASTER> FBBDormMaster)
        {
            _logger = logger;
            _objService = objService;
            _FBBDormMaster = FBBDormMaster;
        }
        public void Handle(EditDormMasterCommand command)
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

                var executePassword = _objService.ExecuteStoredProc("WBB.PKG_FBBDORM_ADMIN001.PROC_EDIT_DORMITORY",
                out paramOut,
                  new
                  {
                      p_dormitory_id = command.Edit_dormitory_ID.ToSafeString(),
                      p_building_th = command.Save_building_th.ToSafeString(),
                      p_dormitory_name_th = command.Edit_dormitory_name_th.ToSafeString(),
                      p_home_no_th = command.Edit_HOME_NO_TH.ToSafeString(),
                      p_moo_th = command.Edit_MOO_TH.ToSafeString(),
                      p_soi_th = command.Edit_SOI_TH.ToSafeString(),
                      p_Street_th = command.Edit_STREET_NAME_TH.ToSafeString(),
                      p_tumbol_th = command.Edit_TUMBON_TH.ToSafeString(),
                      p_amphur_th = command.Edit_AMPHUR_TH.ToSafeString(),
                      p_province_th = command.Edit_Province_TH.ToSafeString(),
                      p_zipcode_th = command.Edit_Postcode_TH.ToSafeString(),
                      p_dormitory_name_en = command.Edit_dormitory_name_en.ToSafeString(),
                      p_home_no_en = command.Edit_HOME_NO_EN.ToSafeString(),
                      p_moo_en = command.Edit_MOO_EN.ToSafeString(),
                      p_soi_en = command.Edit_SOI_EN.ToSafeString(),
                      p_Street_en = command.Edit_STREET_NAME_EN.ToSafeString(),
                      p_tumbol_en = command.Edit_TUMBON_EN.ToSafeString(),
                      p_amphur_en = command.Edit_AMPHUR_EN.ToSafeString(),
                      p_province_en = command.Edit_Province_EN.ToSafeString(),
                      p_zipcode_en = command.Edit_Postcode_EN.ToSafeString(),
                      p_user = command.User.ToSafeString(),
                      p_target_launch_dt = command.Edit_target_launch_dt.ToSafeString(),
                      p_launch_dt = command.Edit_launch_dt.ToSafeString(),
                      p_target_volumn = command.Edit_target_volumn.ToSafeString(),
                      p_volumn = command.Edit_volumn.ToSafeString(),
                      p_dorm_contract_name = command.Edit_dorm_contract_name.ToSafeString(),
                      p_dorm_contract_email = command.Edit_dorm_contract_email.ToSafeString(),
                      p_dorm_contract_phone = command.Edit_dorm_contract_phone.ToSafeString(),

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