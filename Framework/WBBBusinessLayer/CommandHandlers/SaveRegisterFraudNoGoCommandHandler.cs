using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using WBBBusinessLayer.QueryHandlers;
using WBBContract;
using WBBContract.Commands;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;

namespace WBBBusinessLayer.CommandHandlers
{
    public class SaveRegisterFraudNoGoCommandHandler : ICommandHandler<SaveRegisterFraudNoGoCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<string> _objService;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;

        public SaveRegisterFraudNoGoCommandHandler(ILogger logger
            , IEntityRepository<string> objService
            , IWBBUnitOfWork uow
            , IEntityRepository<FBB_INTERFACE_LOG> intfLog)
        {
            _logger = logger;
            _objService = objService;
            _uow = uow;
            _intfLog = intfLog;
        }

        public void Handle(SaveRegisterFraudNoGoCommand command)
        {

            InterfaceLogCommand log = null;
            log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, command, "", "SaveRegisterFraudNoGo", "SaveRegisterFraudNoGoCommand", "", "", "WEB");

            try
            {
                var RETURN_CODE = new OracleParameter();
                RETURN_CODE.OracleDbType = OracleDbType.Varchar2;
                RETURN_CODE.Size = 1000;
                RETURN_CODE.Direction = ParameterDirection.Output;

                var RETURN_MESSAGE = new OracleParameter();
                RETURN_MESSAGE.OracleDbType = OracleDbType.Varchar2;
                RETURN_MESSAGE.Size = 2000;
                RETURN_MESSAGE.Direction = ParameterDirection.Output;

                var outp = new List<object>();
                var paramOut = outp.ToArray();

                var execute = _objService.ExecuteStoredProc("WBB.PKG_FBBOR004_FRAUD.PROC_INSERT_FRAUD_NOGO",
                out paramOut,
                  new
                  {
                      p_customer_type = command.p_customer_type.ToSafeString(),
                      p_customer_name = command.p_customer_name.ToSafeString(),
                      p_id_card_no = command.p_id_card_no.ToSafeString(),
                      p_install_address = command.p_install_address.ToSafeString(),
                      p_product_subtype = command.p_product_subtype.ToSafeString(),
                      p_entry_fee = command.p_entry_fee,
                      p_operator = command.p_operator.ToSafeString(),
                      p_promotion_name = command.p_promotion_name.ToSafeString(),
                      p_promotion_ontop = command.p_promotion_ontop.ToSafeString(),
                      p_promotion_price = command.p_promotion_price,
                      p_price_net = command.p_price_net,
                      p_cs_note = command.p_cs_note.ToSafeString(),
                      p_location_code = command.p_location_code.ToSafeString(),
                      p_location_name = command.p_location_name.ToSafeString(),
                      p_chn_sales_name = command.p_chn_sales_name.ToSafeString(),
                      p_asc_code = command.p_asc_code.ToSafeString(),
                      p_asc_name = command.p_asc_name.ToSafeString(),
                      p_region_customer = command.p_region_customer.ToSafeString(),
                      p_region_sale = command.p_region_sale.ToSafeString(),
                      p_fraud_score = command.p_fraud_score,
                      p_waiting_time_slot_flag = command.p_waiting_time_slot_flag.ToSafeString(),
                      p_project_name = command.p_project_name.ToSafeString(),
                      p_address_duplicated_flag = command.p_address_duplicated_flag.ToSafeString(),
                      p_id_duplicated_flag = command.p_id_duplicated_flag.ToSafeString(),
                      p_contact_duplicated_flag = command.p_contact_duplicated_flag.ToSafeString(),
                      p_contact_not_active_flag = command.p_contact_not_active_flag.ToSafeString(),
                      p_contact_no_fmc_flag = command.p_contact_no_fmc_flag.ToSafeString(),
                      p_watch_list_dealer_flag = command.p_watch_list_dealer_flag.ToSafeString(),
                      p_sale_dealer_direct_sale_flag = command.p_sale_dealer_direct_sale_flag.ToSafeString(),
                      p_relate_mobile_segment = command.p_relate_mobile_segment.ToSafeString(),
                      p_charge_type = command.p_charge_type.ToSafeString(),
                      p_service_month = command.p_service_month.ToSafeString(),
                      p_use_id_card_address_flag = command.p_use_id_card_address_flag.ToSafeString(),
                      p_reason_verify = command.p_reason_verify.ToSafeString(),
                      p_created_by = command.p_created_by.ToSafeString(),
                      p_updated_by = command.p_updated_by.ToSafeString(),
                      p_flag_send_xml = command.p_flag_send_xml.ToSafeString(),
                      p_message_send_xml = command.p_message_send_xml.ToSafeString(),

                      RETURN_CODE = RETURN_CODE,
                      RETURN_MESSAGE = RETURN_MESSAGE

                  });

                if (RETURN_CODE.Value != "-1")
                {
                    command.ret_code = RETURN_CODE.Value.ToString();
                    command.ret_message = RETURN_MESSAGE.Value.ToString();
                    _logger.Info("End WBB.PKG_FBBOR004_FRAUD.PROC_INSERT_FRAUD_NOGO output msg: " + command.ret_code);
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, command, log, "Success", "", "");
                }
                else
                {
                    command.ret_code = "-1";
                    command.ret_message = "Error return -1 call service WBB.PKG_FBBOR004_FRAUD.PROC_INSERT_FRAUD_NOGO output msg: " + "Error";
                    _logger.Info("Error return -1 call service WBB.PKG_FBBOR004_FRAUD.PROC_INSERT_FRAUD output msg: " + "Error");
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, command, log, "Failed", "Failed", "");
                }
            }
            catch (Exception ex)
            {
                command.ret_code = "-1";
                command.ret_message = "Error call SaveRegisterFraudNoGoCommand: " + ex.Message;
                _logger.Info(ex.Message);
                _logger.Info("Error call service WBB.PKG_FBBOR004_FRAUD.PROC_INSERT_FRAUD_NOGO" + ex.Message);
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, command, log, "Failed", ex.Message, "");
            }
        }
    }
}
