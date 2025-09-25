using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using WBBContract;
using WBBContract.Minions;
using WBBData.Repository;
using WBBEntity.Extensions;

namespace WBBBusinessLayer.Minions
{
    public class MinionProcInstallPackageCommandHandler : ICommandHandler<MinionProcInstallPackageCommand>
    {
        private readonly IEntityRepository<string> _objService;

        public MinionProcInstallPackageCommandHandler(IEntityRepository<string> objService)
        {
            _objService = objService;
        }

        public void Handle(MinionProcInstallPackageCommand command)
        {
            try
            {
                var ret_code = new OracleParameter();
                ret_code.OracleDbType = OracleDbType.Decimal;
                ret_code.Direction = ParameterDirection.Output;

                var v_error_msg = new OracleParameter();
                v_error_msg.OracleDbType = OracleDbType.Varchar2;
                v_error_msg.Size = 2000;
                v_error_msg.Direction = ParameterDirection.Output;

                var outp = new List<object>();
                var paramOut = outp.ToArray();

                _objService.ExecuteStoredProc("WBB.PKG_FBB_INS_PROFILE.PROC_INS_PACKAGE",

                out paramOut,
                   new
                   {
                       p_cust_non_mobile = command.Cust_Non_Mobile,
                       p_package_code = command.Package_Code,
                       p_package_class = command.Package_Class,
                       p_package_type = command.Package_Type,
                       p_package_group = command.Package_Group,
                       p_package_subtype = command.Package_Subtype,
                       p_package_owner = command.Package_Owner,
                       p_technology = command.Technology,
                       p_package_status = command.package_Status,
                       p_package_name = command.package_Name,
                       p_recurring_charge = command.Recurring_Charge,
                       p_pre_recurring_charge = command.Pre_Recurring_Charge,
                       p_recurring_discount_exp = command.Recurring_Discount_Exp,
                       p_recurring_start_dt = command.Recurring_Start_Dt,
                       p_recurring_end_dt = command.Recurring_End_Dt,
                       p_initiation_charge = command.Initiation_Charge,
                       p_pre_initiation_charge = command.Pre_Initiation_Charge,
                       p_package_bill_tha = command.Package_Bill_Tha,
                       p_download_speed = command.Download_Speed,
                       p_upload_speed = command.Upload_Speed,
                       p_home_ip = command.Home_Ip,
                       p_home_port = command.Home_Port,
                       p_idd_flag = command.Idd_Flag,
                       p_fax_flag = command.Fax_Flag,
                       p_mobile_forward = command.Mobile_Forward,

                       //return code
                       out_return_code = ret_code,
                       out_return_message = v_error_msg
                   });


                command.Return_Code = ret_code.Value != null ? Convert.ToInt32(ret_code.Value.ToSafeString() == "null" ? "0" : ret_code.Value.ToSafeString()) : -1;
                command.Return_Desc = v_error_msg.Value.ToSafeString();

            }
            catch (Exception ex)
            {
                command.Return_Code = -1;
                command.Return_Desc = "Error call WBB.PKG_FBB_INS_PROFILE.PROC_INS_PACKAGE " + ex.GetErrorMessage();
            }
        }
    }
}
