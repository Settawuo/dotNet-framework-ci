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
    public class SaveDormMasterCommandHandler : ICommandHandler<SaveDormMasterCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<string> _objService;
        private readonly IEntityRepository<FBBDORM_DORMITORY_MASTER> _FBBDormMaster;

        public SaveDormMasterCommandHandler(ILogger logger,
            IEntityRepository<string> objService, IEntityRepository<FBBDORM_DORMITORY_MASTER> FBBDormMaster)
        {
            _logger = logger;
            _objService = objService;
            _FBBDormMaster = FBBDormMaster;
        }

        public void Handle(SaveDormMasterCommand command)
        {
            string Save_complete = "";
            string Save_fail = "";
            decimal Save_return_code = 0;
            string Save_return_msg = "Success";
            foreach (var item in command.DormMasterDataList)
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

                    var executeResult = _objService.ExecuteStoredProc("WBB.PKG_FBBDORM_ADMIN001.PROC_INSERT_DORMITORY",
                    out paramOut,
                      new
                      {
                          p_dormitory_name_th = item.Save_dormitory_name_th.ToSafeString(),
                          p_home_no_th = item.Save_HOME_NO_TH.ToSafeString(),
                          p_moo_th = item.Save_MOO_TH.ToSafeString(),
                          p_soi_th = item.Save_SOI_TH.ToSafeString(),
                          p_Street_th = item.Save_STREET_NAME_TH.ToSafeString(),
                          p_tumbol_th = item.Save_TUMBON_TH.ToSafeString(),
                          p_amphur_th = item.Save_AMPHUR_TH.ToSafeString(),
                          p_province_th = item.Save_Province_TH.ToSafeString(),
                          p_zipcode_th = item.Save_Postcode_TH.ToSafeString(),


                          p_dormitory_name_en = item.Save_dormitory_name_en.ToSafeString(),
                          p_home_no_en = item.Save_HOME_NO_EN.ToSafeString(),
                          p_moo_en = item.Save_MOO_EN.ToSafeString(),
                          p_soi_en = item.Save_SOI_EN.ToSafeString(),
                          p_Street_en = item.Save_STREET_NAME_EN.ToSafeString(),
                          p_tumbol_en = item.Save_TUMBON_EN.ToSafeString(),
                          p_amphur_en = item.Save_AMPHUR_EN.ToSafeString(),
                          p_province_en = item.Save_Province_EN.ToSafeString(),
                          p_zipcode_en = item.Save_Postcode_EN.ToSafeString(),
                          p_user = item.User.ToSafeString(),


                          p_building_th = item.Save_building_th.ToSafeString(),
                          p_building_en = item.Save_building_en.ToSafeString(),
                          p_room_amount = item.Save_room_amount.ToSafeString(),

                          p_target_launch_dt = item.Save_target_launch_dt.ToSafeString(),
                          p_launch_dt = item.Save_launch_dt.ToSafeString(),
                          p_target_volumn = item.Save_target_volumn.ToSafeString(),
                          p_volumn = item.Save_volumn.ToSafeString(),
                          p_dorm_contract_name = item.Save_dorm_contract_name.ToSafeString(),
                          p_dorm_contract_email = item.Save_dorm_contract_email.ToSafeString(),
                          p_dorm_contract_phone = item.Save_dorm_contract_phone.ToSafeString(),

                          p_return_code = p_return_code,
                          p_return_message = ret_msg

                      });
                    decimal Return_code = p_return_code.Value.ToSafeString() != "null" ? decimal.Parse(p_return_code.Value.ToSafeString()) : 0;

                    if (Save_complete == "")
                    {
                        Save_complete = item.Save_building_en;
                    }
                    else
                    {
                        Save_complete = Save_complete + ", " + item.Save_building_en;
                    }

                    _logger.Info("EndPROC_INS_MASTER" + command.Save_return_msg);

                }
                catch (Exception ex)
                {
                    _logger.Info(ex.GetErrorMessage());
                    if (Save_fail == "")
                    {
                        Save_fail = item.Save_building_en;
                    }
                    else
                    {
                        Save_fail = Save_fail + ", " + item.Save_building_en;
                    }
                    if (Save_return_code == 1)
                    {
                        Save_return_code = -1;
                    }
                    if (Save_return_msg == "Success")
                    {
                        Save_return_msg = "Error call SavePreregister Handler: " + ex.GetErrorMessage();
                    }
                }
            }
            command.Save_return_code = Save_return_code;
            command.Save_return_msg = Save_return_msg;
            command.Save_complete = Save_complete;
            command.Save_fail = Save_fail;


        }
    }
}