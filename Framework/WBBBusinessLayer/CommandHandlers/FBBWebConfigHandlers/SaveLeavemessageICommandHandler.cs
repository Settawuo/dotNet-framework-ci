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
    public class SaveLeavemessageICommandHandler : ICommandHandler<SaveLeavemessageIICommand>
    {

        private readonly ILogger _logger;
        private readonly IEntityRepository<string> _objService;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;

        public SaveLeavemessageICommandHandler(ILogger logger,
            IEntityRepository<string> objService,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IWBBUnitOfWork uow)
        {
            _logger = logger;
            _objService = objService;
            _intfLog = intfLog;
            _uow = uow;
        }

        public void Handle(SaveLeavemessageIICommand command)
        {
            InterfaceLogCommand log = null;
            try
            {
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, command, command.p_contact_mobile_no, "SaveLeavemessageICommandHandler", "SaveLeavemessageICommandHandler", command.p_contact_mobile_no, "FBB", "WEB");

                var o_return_code = new OracleParameter();
                o_return_code.OracleDbType = OracleDbType.Decimal;
                o_return_code.Direction = ParameterDirection.Output;

                var o_return_msg = new OracleParameter();
                o_return_msg.OracleDbType = OracleDbType.Varchar2;
                o_return_msg.Size = 2000;
                o_return_msg.Direction = ParameterDirection.Output;

                var outp = new List<object>();
                var paramOut = outp.ToArray();

                var executeResult = _objService.ExecuteStoredProc("PKG_FBBOR021.PROC_REG_LINE_BOT",
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
                       p_rental_flag = command.p_rental_flag,

                       p_location_code = command.p_location_code,
                       p_asc_code = command.p_asc_code,
                       p_emp_id = command.p_emp_id,
                       p_sales_rep = command.p_sales_rep,

                       p_channal = command.p_channal,
                       p_campaign = command.p_campaign,
                       p_url = command.p_url,
                       p_latitude = command.p_latitude,
                       p_longitude = command.p_longitude,

                       /// Out
                       o_return_code = o_return_code,
                       o_return_msg = o_return_msg

                   });
                command.return_message = o_return_msg.Value.ToSafeString();
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
                command.return_message = "Error save Leavemessage service " + ex.Message;
            }
        }


    }
}
