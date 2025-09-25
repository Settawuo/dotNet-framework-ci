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
    public class MinionProcInstallSplitterCommandHandler : ICommandHandler<MinionProcInstallSplitterCommand>
    {
        private readonly IEntityRepository<string> _objService;

        public MinionProcInstallSplitterCommandHandler(IEntityRepository<string> objService)
        {
            _objService = objService;
        }

        public void Handle(MinionProcInstallSplitterCommand command)
        {
            try
            {
                var v_code = new OracleParameter();
                v_code.OracleDbType = OracleDbType.Decimal;
                v_code.Direction = ParameterDirection.Output;

                var v_error_msg = new OracleParameter();
                v_error_msg.OracleDbType = OracleDbType.Varchar2;
                v_error_msg.Size = 2000;
                v_error_msg.Direction = ParameterDirection.Output;

                var outp = new List<object>();
                var paramOut = outp.ToArray();

                _objService.ExecuteStoredProc("WBB.PKG_FBB_INS_PROFILE.PROC_INS_SPLITTER",
                out paramOut,
                   new
                   {
                       p_cust_non_mobile = command.Cust_Non_Mobile,
                       p_splitter_seq = command.Splitter_Seq,
                       p_splitter_name = command.Splitter_Name,
                       p_distance = command.Distance,
                       p_distance_type = command.Distance_Type,

                       //return code
                       ret_code = v_code,
                       ret_message = v_error_msg
                   });


                command.Return_Code = v_code.Value != null ? Convert.ToInt32(v_code.Value.ToSafeString() == "null" ? "0" : v_code.Value.ToSafeString()) : -1;
                command.Return_Desc = v_error_msg.Value.ToSafeString();

            }
            catch (Exception ex)
            {
                command.Return_Code = -1;
                command.Return_Desc = "Error call WBB.PKG_FBB_INS_PROFILE.PROC_INS_SPLITTER " + ex.GetErrorMessage();
            }
        }
    }
}
