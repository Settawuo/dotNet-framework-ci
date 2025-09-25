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
    public class MinionProcInstallContactCommandHandler : ICommandHandler<MinionProcInstallContactCommand>
    {
        private readonly IEntityRepository<string> _objService;

        public MinionProcInstallContactCommandHandler(IEntityRepository<string> objService)
        {
            _objService = objService;
        }

        public void Handle(MinionProcInstallContactCommand command)
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

                _objService.ExecuteStoredProc("WBB.PKG_FBB_INS_PROFILE.PROC_INS_CONTACT",
                out paramOut,
                   new
                   {
                       p_ba_id = command.ba_id,
                       p_contact_seq = command.Contact_Seq,
                       p_contact_addr_id = command.Contact_Addr_id,
                       p_contact_first_name = command.Contact_First_Name,
                       p_contact_last_name = command.Contact_Last_Name,
                       p_contact_home_phone = command.Contact_Home_Phone,
                       p_contact_mobile_phone1 = command.Contact_Mobile_Phone1,
                       p_contact_mobile_phone2 = command.Contact_Mobile_Phone2,
                       p_contact_email = command.Contact_Email,

                       //return code
                       return_code = ret_code,
                       return_message = v_error_msg
                   });


                command.Return_Code = ret_code.Value != null ? Convert.ToInt32(ret_code.Value.ToSafeString() == "null" ? "0" : ret_code.Value.ToSafeString()) : -1;
                command.Return_Desc = v_error_msg.Value.ToSafeString();

            }
            catch (Exception ex)
            {
                command.Return_Code = -1;
                command.Return_Desc = "Error call WBB.PKG_FBB_INS_PROFILE.PROC_INS_CONTACT " + ex.GetErrorMessage();
            }
        }
    }
}
