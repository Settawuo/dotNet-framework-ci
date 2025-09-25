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
    public class MinionProcInstallAddressCommandHandler : ICommandHandler<MinionProcInstallAddressCommand>
    {
        private readonly IEntityRepository<string> _objService;

        public MinionProcInstallAddressCommandHandler(IEntityRepository<string> objService)
        {
            _objService = objService;
        }

        public void Handle(MinionProcInstallAddressCommand command)
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

                var ret_addr_rowId = new OracleParameter();
                ret_addr_rowId.OracleDbType = OracleDbType.Varchar2;
                ret_addr_rowId.Size = 2000;
                ret_addr_rowId.Direction = ParameterDirection.Output;

                var outp = new List<object>();
                var paramOut = outp.ToArray();

                _objService.ExecuteStoredProc("WBB.PKG_FBB_INS_PROFILE.PROC_INS_ADDR",

                out paramOut,
                   new
                   {
                       p_house_no = command.House_No,
                       p_soi = command.Soi,
                       p_moo = command.Moo,
                       p_mooban = command.Mooban,
                       p_building_name = command.Building_Name,
                       p_floor = command.Floor,
                       p_room = command.Room,
                       p_street_name = command.Street_Name,
                       p_zipcode_id = command.Zipcode_Id,
                       p_address_vat = command.Address_Vat,

                       //return code
                       addr_rowid = ret_addr_rowId,
                       return_code = ret_code,
                       return_message = v_error_msg
                   });


                command.Return_Code = ret_code.Value != null ? Convert.ToInt32(ret_code.Value.ToSafeString() == "null" ? "0" : ret_code.Value.ToSafeString()) : -1;
                command.Return_Desc = v_error_msg.Value.ToSafeString();
                command.Return_Address_RowId = ret_addr_rowId.Value.ToSafeString();

            }
            catch (Exception ex)
            {
                command.Return_Code = -1;
                command.Return_Desc = "Error call WBB.PKG_FBB_INS_PROFILE.PROC_INS_ADDR " + ex.GetErrorMessage();
            }
        }
    }
}
