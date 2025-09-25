using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using WBBBusinessLayer.QueryHandlers;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Commands.FBBWebConfigCommands;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;

namespace WBBBusinessLayer.CommandHandlers.FBBWebConfigHandlers
{
    public class SaveLeavemessageIMCommandHandler : ICommandHandler<SaveLeavemessageIMCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<string> _objService;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;

        public SaveLeavemessageIMCommandHandler(ILogger logger,
            IEntityRepository<string> objService,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IWBBUnitOfWork uow)
        {
            _logger = logger;
            _objService = objService;
            _intfLog = intfLog;
            _uow = uow;
        }

        public void Handle(SaveLeavemessageIMCommand command)
        {
            InterfaceLogCommand log = null;
            try
            {
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, command, command.p_contact_mobile_no, "SaveLeavemessageIMCommandHandler", "SaveLeavemessageIMCommandHandler", command.p_contact_mobile_no, "FBB", "WEB");

                var o_return_code = new OracleParameter();
                o_return_code.OracleDbType = OracleDbType.Decimal;
                o_return_code.Direction = ParameterDirection.Output;

                var o_return_msg = new OracleParameter();
                o_return_msg.OracleDbType = OracleDbType.Varchar2;
                o_return_msg.Size = 2000;
                o_return_msg.Direction = ParameterDirection.Output;

                var outp = new List<object>();
                var paramOut = outp.ToArray();


                var executeResult = _objService.ExecuteStoredProc("WBB.PKG_FBBOR021.PROC_REG_FROM_IM",
                out paramOut,
                   new
                   {
                       p_language = command.p_language,
                       p_service_speed = command.p_service_speed,
                       p_cust_name = command.p_cust_name,
                       p_cust_surname = command.p_cust_surname,
                       p_contact_mobile_no = command.p_contact_mobile_no,
                       p_is_ais_mobile = command.p_is_ais_mobile,
                       p_contact_email = command.p_contact_email,
                       p_address_type = command.p_address_type,
                       p_building_name = command.p_building_name,
                       p_house_no = command.p_house_no,
                       p_soi = command.p_soi,
                       p_road = command.p_road,
                       p_sub_district = command.p_sub_district,
                       p_district = command.p_district,
                       p_province = command.p_province,
                       p_postal_code = command.p_postal_code,
                       p_contact_time = command.p_contact_time,

                       //v17.3
                       p_location_code = command.p_location_code,
                       p_asc_code = command.p_asc_code,
                       p_channel = command.p_channel,

                       //v17.5
                       p_internet_no = command.p_internet_no,

                       //v17.7
                       p_line_id = command.p_line_id,
                       p_voucher_desc = command.p_voucher_desc,
                       p_campaign_project_name = command.p_campaign_project_name,

                       //v17.9
                       p_rental_flag = command.p_rental_flag,
                       p_location_check_coverage = command.p_location_check_coverage,
                       p_full_address = command.p_full_address,

                       //v18.8
                       p_emp_id = command.p_emp_id,
                       p_sales_rep = command.p_sales_rep,

                       //v18.9
                       p_in_coverage = command.p_in_coverage,
                       p_playbox_flag = command.p_playbox_flag,
                       p_latitude = command.p_latitude,
                       p_longitude = command.p_longitude,

                       //v18.10
                       p_url = command.p_url,

                       //v19.2
                       p_asset_number = command.p_asset_number,
                       p_service_case_id = command.p_service_case_id,

                       //v19.3
                       p_floor = command.p_floor,
                       p_building_no = command.p_building_no,
                       p_moo = command.p_moo,

                       /// Out
                       o_return_code = o_return_code,
                       o_return_msg = o_return_msg

                   });
                command.return_msg = o_return_msg.Value.ToSafeString();
                string return_code = o_return_code.Value.ToSafeString();
                command.return_code = o_return_code.Value.ToSafeString() != "null" ? decimal.Parse(o_return_code.Value.ToSafeString()) : 0;

                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, return_code, log, "Success", "", "");

            }
            catch (Exception ex)
            {
                _logger.Info(ex.Message);

                if (null != log)
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex, log, "Failed", ex.GetErrorMessage(), "");
                }

                command.return_code = -1;
                command.return_msg = "Error save Leavemessage service " + ex.Message;
            }
        }
    }
}
