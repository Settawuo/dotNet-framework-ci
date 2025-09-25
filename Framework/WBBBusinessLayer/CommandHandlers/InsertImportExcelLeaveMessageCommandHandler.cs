using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using WBBContract;
using WBBContract.Commands;
using WBBData.Repository;
using WBBEntity.Extensions;

namespace WBBBusinessLayer.CommandHandlers
{
    public class InsertImportExcelLeaveMessageCommandHandler : ICommandHandler<InsertImportExcelLeaveMessageCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<string> _objService;

        public InsertImportExcelLeaveMessageCommandHandler(ILogger logger, IEntityRepository<string> objService)
        {
            _logger = logger;
            _objService = objService;
        }

        public void Handle(InsertImportExcelLeaveMessageCommand command)
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

                var execute = _objService.ExecuteStoredProc("WBB.PKG_FBBOR021.PROC_IMPORT_EXCEL",
                out paramOut,
                  new
                  {
                      p_file_name = command.p_file_name.ToSafeString(),
                      p_username = command.p_username.ToSafeString(),
                      p_service_speed = command.p_service_speed.ToSafeString(),
                      p_cust_name = command.p_cust_name.ToSafeString(),
                      p_cust_surname = command.p_cust_surname.ToSafeString(),
                      p_contact_mobile_no = command.p_contact_mobile_no.ToSafeString(),
                      p_is_ais_mobile = command.p_is_ais_mobile.ToSafeString(),
                      p_contact_time = command.p_contact_time.ToSafeString(),
                      p_contact_email = command.p_contact_email.ToSafeString(),
                      p_address_type = command.p_address_type.ToSafeString(),
                      p_building_name = command.p_building_name.ToSafeString(),
                      p_village_name = command.p_village_name.ToSafeString(),
                      p_house_no = command.p_house_no.ToSafeString(),
                      p_soi = command.p_soi.ToSafeString(),
                      p_road = command.p_road.ToSafeString(),
                      p_sub_district = command.p_sub_district.ToSafeString(),
                      p_district = command.p_district.ToSafeString(),
                      p_province = command.p_province.ToSafeString(),
                      p_postal_code = command.p_postal_code.ToSafeString(),
                      p_campaign_project_name = command.p_campaign_project_name.ToSafeString(),

                      return_code = ret_code,
                      return_message = ret_msg
                  });

                command.return_code = Convert.ToInt32(ret_code.Value == null ? "0" : ret_code.Value.ToString());
                command.return_message = ret_msg.Value == null ? "" : ret_msg.Value.ToString();
            }
            catch (Exception ex)
            {
                _logger.Info(ex.Message);
                command.return_code = -1;
                command.return_message = "Error call InsertImportExcelLeaveMessageCommandHandler: " + ex.Message;
            }
        }
    }
}
