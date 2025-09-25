using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBBusinessLayer.QueryHandlers;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Commands.WebServices;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;

namespace WBBBusinessLayer.CommandHandlers.WebServices
{
    public class InsertSaveOrderNew911Handler : ICommandHandler<InsertSaveOrderNew911Command>
    {
        private readonly ILogger _logger;
        private readonly IAirNetEntityRepository<string> _objService;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;

        public InsertSaveOrderNew911Handler(
            ILogger logger,
            IAirNetEntityRepository<string> objService,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IWBBUnitOfWork uow
            )
        {
            _logger = logger;
            _objService = objService;
            _intfLog = intfLog;
            _uow = uow;
        }

        public void Handle(InsertSaveOrderNew911Command command)
        {
            InterfaceLogCommand log = null;

            try
            {
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, command, command.p_mobile_no, "InsertSaveOrderNew911Handler", "InsertSaveOrderNew911", command.p_mobile_no, "FBB", "WEB");

                // in

                AirRegistPackageObjectModel air_regist_package_array = new AirRegistPackageObjectModel();
                air_regist_package_array.REC_REG_PACKAGE = command.p_air_regist_package_array.Select(p => new Air_Regist_PackageOracleTypeMapping
                {
                    temp_ia = p.temp_ia.ToSafeString(),
                    product_subtype = p.product_subtype.ToSafeString(),
                    package_type = p.package_type.ToSafeString(),
                    package_code = p.package_code.ToSafeString(),
                    package_price = p.package_price,
                    idd_flag = p.idd_flag.ToSafeString(),
                    fax_flag = p.fax_flag.ToSafeString(),
                    home_ip = p.home_ip.ToSafeString(),
                    home_port = p.home_port.ToSafeString(),
                    mobile_forward = p.mobile_forward.ToSafeString(),
                    pbox_ext = p.pbox_ext.ToSafeString()
                }).ToArray();

                if (air_regist_package_array.REC_REG_PACKAGE.Length == 0)
                {
                    var tmp_regist_package_array = new List<Air_Regist_PackageOracleTypeMapping>();
                    air_regist_package_array.REC_REG_PACKAGE = tmp_regist_package_array.ToArray();
                }

                var p_air_regist_package_array = OracleCustomTypeUtilities.CreateUDTArrayParameter("p_AIR_REGIST_PACKAGE_ARRAY", "AIR_REGIST_PACKAGE_ARRAY", air_regist_package_array);

                AirRegistFileObjectModel air_regist_file_array = new AirRegistFileObjectModel();
                air_regist_file_array.REC_REG_PACKAGE = command.p_air_regist_file_array.Select(p => new Air_Regist_FileOracleTypeMapping
                {
                    file_name = p.file_name.ToSafeString()
                }).ToArray();

                if (air_regist_file_array.REC_REG_PACKAGE.Length == 0)
                {
                    var tmp_regist_file_array = new List<Air_Regist_FileOracleTypeMapping>();
                    air_regist_file_array.REC_REG_PACKAGE = tmp_regist_file_array.ToArray();
                }

                var p_air_regist_file_array = OracleCustomTypeUtilities.CreateUDTArrayParameter("p_AIR_REGIST_FILE_ARRAY", "AIR_REGIST_FILE_ARRAY", air_regist_file_array);

                AirRegistSplitterObjectModel air_regist_splitter_array = new AirRegistSplitterObjectModel();
                air_regist_splitter_array.REC_REG_PACKAGE = command.p_air_regist_splitter_array.Select(p => new Air_Regist_SplitterOracleTypeMapping
                {
                    SPLITTER_NAME = p.SPLITTER_NAME.ToSafeString(),
                    DISTANCE = p.DISTANCE,
                    DISTANCE_TYPE = p.DISTANCE_TYPE.ToSafeString(),
                    RESOURCE_TYPE = p.RESOURCE_TYPE.ToSafeString()
                }).ToArray();

                if (air_regist_splitter_array.REC_REG_PACKAGE.Length == 0)
                {
                    var tmp_regist_splitter_array = new List<Air_Regist_SplitterOracleTypeMapping>();
                    air_regist_splitter_array.REC_REG_PACKAGE = tmp_regist_splitter_array.ToArray();
                }

                var p_air_regist_splitter_array = OracleCustomTypeUtilities.CreateUDTArrayParameter("p_AIR_REGIST_SPLITTER_ARRAY", "AIR_REGIST_SPLITTER_ARRAY", air_regist_splitter_array);

                AirRegistCPESerialObjectModel air_regist_cpe_serial_array = new AirRegistCPESerialObjectModel();
                air_regist_cpe_serial_array.REC_REG_PACKAGE = command.p_air_regist_cpe_serial_array.Select(p => new Air_Regist_CPE_SerialOracleTypeMapping
                {
                    CPE_TYPE = p.CPE_TYPE.ToSafeString(),
                    SERIAL_NO = p.SERIAL_NO.ToSafeString(),
                    MAC_ADDRESS = p.MAC_ADDRESS.ToSafeString(),
                    STATUS_DESC = p.STATUS_DESC.ToSafeString(),
                    MODEL_NAME = p.MODEL_NAME.ToSafeString(),
                    COMPANY_CODE = p.COMPANY_CODE.ToSafeString(),
                    CPE_PLANT = p.CPE_PLANT.ToSafeString(),
                    STORAGE_LOCATION = p.STORAGE_LOCATION.ToSafeString(),
                    MATERIAL_CODE = p.MATERIAL_CODE.ToSafeString(),
                    REGISTER_DATE = p.REGISTER_DATE.ToSafeString(),
                    FIBRENET_ID = p.FIBRENET_ID.ToSafeString(),
                    SN_PATTERN = p.SN_PATTERN.ToSafeString(),
                    SHIP_TO = p.SHIP_TO.ToSafeString(),
                    WARRANTY_START_DATE = p.WARRANTY_START_DATE.ToSafeString(),
                    WARRANTY_END_DATE = p.WARRANTY_END_DATE.ToSafeString()
                }).ToArray();

                if (air_regist_cpe_serial_array.REC_REG_PACKAGE.Length == 0)
                {
                    var tmp_regist_cpe_serial_array = new List<Air_Regist_CPE_SerialOracleTypeMapping>();
                    air_regist_cpe_serial_array.REC_REG_PACKAGE = tmp_regist_cpe_serial_array.ToArray();
                }

                var p_air_regist_cpe_serial_array = OracleCustomTypeUtilities.CreateUDTArrayParameter("p_AIR_REGIST_CPE_SERIAL_ARRAY", "AIR_REGIST_CPE_SERIAL_ARRAY", air_regist_cpe_serial_array);

                AirRegistCustInsightObjectModel air_regist_cust_insi_array = new AirRegistCustInsightObjectModel();
                air_regist_cust_insi_array.REC_REG_PACKAGE = command.p_air_regist_cust_insi_array.Select(p => new Air_Regist_Cust_InsightOracleTypeMapping
                {
                    GROUP_ID = p.GROUP_ID.ToSafeString(),
                    GROUP_NAME_TH = p.GROUP_NAME_TH.ToSafeString(),
                    GROUP_NAME_EN = p.GROUP_NAME_EN.ToSafeString(),
                    QUESTION_ID = p.GROUP_ID.ToSafeString(),
                    QUESTION_TH = p.GROUP_ID.ToSafeString(),
                    QUESTION_EN = p.GROUP_ID.ToSafeString(),
                    ANSWER_ID = p.GROUP_ID.ToSafeString(),
                    ANSWER_TH = p.GROUP_ID.ToSafeString(),
                    ANSWER_EN = p.GROUP_ID.ToSafeString(),
                    ANSWER_VALUE_TH = p.GROUP_ID.ToSafeString(),
                    ANSWER_VALUE_EN = p.GROUP_ID.ToSafeString(),
                    PARENT_ANSWER_ID = p.GROUP_ID.ToSafeString(),
                    ACTION_WFM = p.GROUP_ID.ToSafeString(),
                    ACTION_FOA = p.GROUP_ID.ToSafeString()
                 }).ToArray();

                if (air_regist_cust_insi_array.REC_REG_PACKAGE.Length == 0)
                {
                    var tmp_regist_cust_insi_array = new List<Air_Regist_Cust_InsightOracleTypeMapping>();
                    air_regist_cust_insi_array.REC_REG_PACKAGE = tmp_regist_cust_insi_array.ToArray();
                }

                var p_air_regist_cust_insi_array = OracleCustomTypeUtilities.CreateUDTArrayParameter("p_AIR_REGIST_CUST_INSI_ARRAY", "AIR_REGIST_CUST_INSIGHT_ARRAY", air_regist_cust_insi_array);

                AirRegistDcontractRecordObjectModel air_regist_dcontract_array = new AirRegistDcontractRecordObjectModel();
                air_regist_dcontract_array.REC_REG_PACKAGE = command.p_air_regist_dcontract_array.Select(p => new Air_Regist_Dcontract_RecordOracleTypeMapping
                {
                    PRODUCT_SUBTYPE = p.PRODUCT_SUBTYPE.ToSafeString(),
                    PBOX_EXT = p.PBOX_EXT.ToSafeString(),
                    TDM_CONTRACT_ID = p.TDM_CONTRACT_ID.ToSafeString(),
                    TDM_RULE_ID = p.TDM_RULE_ID.ToSafeString(),
                    TDM_PENALTY_ID = p.TDM_PENALTY_ID.ToSafeString(),
                    TDM_PENALTY_GROUP_ID = p.TDM_PENALTY_GROUP_ID.ToSafeString(),
                    DURATION = p.DURATION.ToSafeString(),
                    CONTRACT_FLAG = p.CONTRACT_FLAG.ToSafeString(),
                    DEVICE_COUNT = p.DEVICE_COUNT.ToSafeString()
                }).ToArray();

                if (air_regist_dcontract_array.REC_REG_PACKAGE.Length == 0)
                {
                    var tmp_dcontract_array = new List<Air_Regist_Dcontract_RecordOracleTypeMapping>();
                    air_regist_dcontract_array.REC_REG_PACKAGE = tmp_dcontract_array.ToArray();
                }                   

                var p_air_regist_dcontract_array = OracleCustomTypeUtilities.CreateUDTArrayParameter("p_AIR_REGIST_DCONTRACT_ARRAY", "AIR_REGIST_DCONTRACT_ARRAY", air_regist_dcontract_array);

                // out

                OracleParameter o_return_code = new OracleParameter();
                o_return_code.ParameterName = "o_return_code";
                o_return_code.OracleDbType = OracleDbType.Decimal;
                o_return_code.Direction = ParameterDirection.Output;

                OracleParameter o_return_message = new OracleParameter();
                o_return_message.ParameterName = "o_return_message";
                o_return_message.OracleDbType = OracleDbType.Varchar2;
                o_return_message.Size = 2000;
                o_return_message.Direction = ParameterDirection.Output;

                OracleParameter o_return_order_no = new OracleParameter();
                o_return_order_no.ParameterName = "o_return_order_no";
                o_return_order_no.OracleDbType = OracleDbType.Varchar2;
                o_return_order_no.Size = 2000;
                o_return_order_no.Direction = ParameterDirection.Output;

                OracleParameter o_return_Multi_Instance_flag = new OracleParameter();
                o_return_Multi_Instance_flag.ParameterName = "o_return_Multi_Instance_flag";
                o_return_Multi_Instance_flag.OracleDbType = OracleDbType.Varchar2;
                o_return_Multi_Instance_flag.Size = 2000;
                o_return_Multi_Instance_flag.Direction = ParameterDirection.Output;

                OracleParameter o_return_product_subtype = new OracleParameter();
                o_return_product_subtype.ParameterName = "o_return_product_subtype";
                o_return_product_subtype.OracleDbType = OracleDbType.Varchar2;
                o_return_product_subtype.Size = 2000;
                o_return_product_subtype.Direction = ParameterDirection.Output;

                OracleParameter o_return_event_code = new OracleParameter();
                o_return_event_code.ParameterName = "o_return_event_code";
                o_return_event_code.OracleDbType = OracleDbType.Varchar2;
                o_return_event_code.Size = 2000;
                o_return_event_code.Direction = ParameterDirection.Output;

                var outp = new List<object>();
                var paramOut = outp.ToArray();

                var executePassword = _objService.ExecuteStoredProc("AIR_ADMIN.PKG_AIROR911.INSERT_SAVE_ORDER_NEW",
                    out paramOut,
                  new
                  {
                      // in
                      p_customer_type = command.p_customer_type.ToSafeString(),
                      p_customer_subtype = command.p_customer_subtype.ToSafeString(),
                      p_title_code = command.p_title_code.ToSafeString(),
                      p_first_name = command.p_first_name.ToSafeString(),
                      p_last_name = command.p_last_name.ToSafeString(),
                      p_contact_title_code = command.p_contact_title_code.ToSafeString(),
                      p_contact_first_name = command.p_contact_first_name.ToSafeString(),
                      p_contact_last_name = command.p_contact_last_name.ToSafeString(),
                      p_id_card_type_desc = command.p_id_card_type_desc.ToSafeString(),
                      p_id_card_no = command.p_id_card_no.ToSafeString(),
                      p_tax_id = command.p_tax_id.ToSafeString(),
                      p_gender = command.p_gender.ToSafeString(),
                      p_birth_date = command.p_birth_date.ToSafeString(),
                      p_mobile_no = command.p_mobile_no.ToSafeString(),
                      p_mobile_no_2 = command.p_mobile_no_2.ToSafeString(),
                      p_home_phone_no = command.p_home_phone_no.ToSafeString(),
                      p_email_address = command.p_email_address.ToSafeString(),
                      p_contact_time = command.p_contact_time.ToSafeString(),
                      p_nationality_desc = command.p_nationality_desc.ToSafeString(),
                      p_customer_remark = command.p_customer_remark.ToSafeString(),
                      p_house_no = command.p_house_no.ToSafeString(),
                      p_moo_no = command.p_moo_no.ToSafeString(),
                      p_building = command.p_building.ToSafeString(),
                      p_floor = command.p_floor.ToSafeString(),
                      p_room = command.p_room.ToSafeString(),
                      p_mooban = command.p_mooban.ToSafeString(),
                      p_soi = command.p_soi.ToSafeString(),
                      p_road = command.p_road.ToSafeString(),
                      p_zipcode_rowid = command.p_zipcode_rowid.ToSafeString(),
                      p_latitude = command.p_latitude.ToSafeString(),
                      p_longtitude = command.p_longtitude.ToSafeString(),
                      p_asc_code = command.p_asc_code.ToSafeString(),
                      p_employee_id = command.p_employee_id.ToSafeString(),
                      p_location_code = command.p_location_code.ToSafeString(),
                      p_sale_represent = command.p_sale_represent.ToSafeString(),
                      p_cs_note = command.p_cs_note.ToSafeString(),
                      p_wifi_access_point = command.p_wifi_access_point.ToSafeString(),
                      p_install_status = command.p_install_status.ToSafeString(),
                      p_coverage = command.p_coverage.ToSafeString(),
                      p_existing_airnet_no = command.p_existing_airnet_no.ToSafeString(),
                      p_gsm_mobile_no = command.p_gsm_mobile_no.ToSafeString(),
                      p_contact_name_1 = command.p_contact_name_1.ToSafeString(),
                      p_contact_name_2 = command.p_contact_name_2.ToSafeString(),
                      p_contact_mobile_no_1 = command.p_contact_mobile_no_1.ToSafeString(),
                      p_contact_mobile_no_2 = command.p_contact_mobile_no_2.ToSafeString(),
                      p_condo_floor = command.p_condo_floor.ToSafeString(),
                      p_condo_roof_top = command.p_condo_roof_top.ToSafeString(),
                      p_condo_balcony = command.p_condo_balcony.ToSafeString(),
                      p_balcony_north = command.p_balcony_north.ToSafeString(),
                      p_balcony_south = command.p_balcony_south.ToSafeString(),
                      p_balcony_east = command.p_balcony_east.ToSafeString(),
                      p_balcony_wast = command.p_balcony_wast.ToSafeString(),
                      p_high_building = command.p_high_building.ToSafeString(),
                      p_high_tree = command.p_high_tree.ToSafeString(),
                      p_billboard = command.p_billboard.ToSafeString(),
                      p_expressway = command.p_expressway.ToSafeString(),
                      p_address_type_wire = command.p_address_type_wire.ToSafeString(),
                      p_address_type = command.p_address_type.ToSafeString(),
                      p_floor_no = command.p_floor_no.ToSafeString(),
                      p_house_no_bl = command.p_house_no_bl.ToSafeString(),
                      p_moo_no_bl = command.p_moo_no_bl.ToSafeString(),
                      p_mooban_bl = command.p_mooban_bl.ToSafeString(),
                      p_building_bl = command.p_building_bl.ToSafeString(),
                      p_floor_bl = command.p_floor_bl.ToSafeString(),
                      p_room_bl = command.p_room_bl.ToSafeString(),
                      p_soi_bl = command.p_soi_bl.ToSafeString(),
                      p_road_bl = command.p_road_bl.ToSafeString(),
                      p_zipcode_rowid_bl = command.p_zipcode_rowid_bl.ToSafeString(),
                      p_house_no_vt = command.p_house_no_vt.ToSafeString(),
                      p_moo_no_vt = command.p_moo_no_vt.ToSafeString(),
                      p_mooban_vt = command.p_mooban_vt.ToSafeString(),
                      p_building_vt = command.p_building_vt.ToSafeString(),
                      p_floor_vt = command.p_floor_vt.ToSafeString(),
                      p_room_vt = command.p_room_vt.ToSafeString(),
                      p_soi_vt = command.p_soi_vt.ToSafeString(),
                      p_road_vt = command.p_road_vt.ToSafeString(),
                      p_zipcode_rowid_vt = command.p_zipcode_rowid_vt.ToSafeString(),
                      p_cvr_id = command.p_cvr_id.ToSafeString(),
                      p_cvr_node = command.p_cvr_node.ToSafeString(),
                      p_cvr_tower = command.p_cvr_tower.ToSafeString(),
                      p_site_code = command.p_site_code.ToSafeString(),
                      p_relate_mobile = command.p_relate_mobile.ToSafeString(),
                      p_relate_non_mobile = command.p_relate_non_mobile.ToSafeString(),
                      p_sff_ca_no = command.p_sff_ca_no.ToSafeString(),
                      p_sff_sa_no = command.p_sff_sa_no.ToSafeString(),
                      p_sff_ba_no = command.p_sff_ba_no.ToSafeString(),
                      p_network_type = command.p_network_type.ToSafeString(),
                      p_service_day = command.p_service_day.ToSafeString(),
                      p_expect_install_date = command.p_expect_install_date.ToSafeString(),
                      p_fttx_vendor = command.p_fttx_vendor.ToSafeString(),
                      p_install_note = command.p_install_note.ToSafeString(),
                      p_phone_flag = command.p_phone_flag.ToSafeString(),
                      p_time_Slot = command.p_time_Slot.ToSafeString(),
                      p_installation_Capacity = command.p_installation_Capacity.ToSafeString(),
                      p_address_Id = command.p_address_Id.ToSafeString(),
                      p_access_Mode = command.p_access_Mode.ToSafeString(),
                      p_eng_flag = command.p_eng_flag.ToSafeString(),
                      p_event_code = command.p_event_code.ToSafeString(),
                      p_installAddress1 = command.p_installAddress1.ToSafeString(),
                      p_installAddress2 = command.p_installAddress2.ToSafeString(),
                      p_installAddress3 = command.p_installAddress3.ToSafeString(),
                      p_installAddress4 = command.p_installAddress4.ToSafeString(),
                      p_installAddress5 = command.p_installAddress5.ToSafeString(),
                      p_pbox_count = command.p_pbox_count.ToSafeString(),
                      p_convergence_flag = command.p_convergence_flag.ToSafeString(),
                      p_time_slot_id = command.p_time_slot_id.ToSafeString(),
                      p_gift_voucher = command.p_gift_voucher.ToSafeString(),
                      p_sub_location_id = command.p_sub_location_id.ToSafeString(),
                      p_sub_contract_name = command.p_sub_contract_name.ToSafeString(),
                      p_install_staff_id = command.p_install_staff_id.ToSafeString(),
                      p_install_staff_name = command.p_install_staff_name.ToSafeString(),
                      p_flow_flag = command.p_flow_flag.ToSafeString(),
                      p_line_id = command.p_line_id.ToSafeString(),
                      p_relate_project_name = command.p_relate_project_name.ToSafeString(),
                      p_plug_and_play_flag = command.p_plug_and_play_flag.ToSafeString(),
                      p_reserved_id = command.p_reserved_id.ToSafeString(),
                      p_job_order_type = command.p_job_order_type.ToSafeString(),
                      p_assign_rule = command.p_assign_rule.ToSafeString(),
                      p_old_isp = command.p_old_isp.ToSafeString(),
                      p_splitter_flag = command.p_splitter_flag.ToSafeString(),
                      p_reserved_port_id = command.p_reserved_port_id.ToSafeString(),
                      p_special_remark = command.p_special_remark.ToSafeString(),
                      p_order_no = command.p_order_no.ToSafeString(),
                      p_source_system = command.p_source_system.ToSafeString(),
                      p_bill_media = command.p_bill_media.ToSafeString(),
                      p_pre_order_no = command.p_pre_order_no.ToSafeString(),
                      p_voucher_desc = command.p_voucher_desc.ToSafeString(),
                      p_campaign_project_name = command.p_campaign_project_name.ToSafeString(),
                      p_pre_order_chanel = command.p_pre_order_chanel.ToSafeString(),
                      p_rental_flag = command.p_rental_flag.ToSafeString(),
                      p_dev_project_code = command.p_dev_project_code.ToSafeString(),
                      p_dev_bill_to = command.p_dev_bill_to.ToSafeString(),
                      p_dev_po_no = command.p_dev_po_no.ToSafeString(),
                      p_partner_type = command.p_partner_type.ToSafeString(),
                      p_partner_subtype = command.p_partner_subtype.ToSafeString(),
                      p_mobile_by_asc = command.p_mobile_by_asc.ToSafeString(),
                      p_location_name = command.p_location_name.ToSafeString(),
                      p_paymentMethod = command.p_paymentMethod.ToSafeString(),
                      p_transactionId_in = command.p_transactionId_in.ToSafeString(),
                      p_transactionId = command.p_transactionId.ToSafeString(),
                      p_sub_access_mode = command.p_sub_access_mode.ToSafeString(),
                      p_request_sub_flag = command.p_request_sub_flag.ToSafeString(),
                      p_premium_flag = command.p_premium_flag.ToSafeString(),
                      p_relate_mobile_segment = command.p_relate_mobile_segment.ToSafeString(),
                      p_ref_ur_no = command.p_ref_ur_no.ToSafeString(),
                      p_location_email_by_region = command.p_location_email_by_region.ToSafeString(),
                      p_sale_staff_name = command.p_sale_staff_name.ToSafeString(),
                      p_dopa_flag = command.p_dopa_flag.ToSafeString(),
                      p_service_year = command.p_service_year.ToSafeString(),
                      p_require_cs_verify_doc = command.p_require_cs_verify_doc.ToSafeString(),
                      p_facerecog_flag = command.p_facerecog_flag.ToSafeString(),
                      p_special_account_name = command.p_special_account_name.ToSafeString(),
                      p_special_account_no = command.p_special_account_no.ToSafeString(),
                      p_special_account_enddate = command.p_special_account_enddate.ToSafeString(),
                      p_special_account_group_email = command.p_special_account_group_email.ToSafeString(),
                      p_special_account_flag = command.p_special_account_flag.ToSafeString(),
                      p_existing_mobile_flag = command.p_existing_mobile_flag.ToSafeString(),
                      p_pre_survey_date = command.p_pre_survey_date.ToSafeString(),
                      p_pre_survey_timeslot = command.p_pre_survey_timeslot.ToSafeString(),
                      p_register_channel = command.p_register_channel.ToSafeString(),
                      p_auto_create_prospect_flag = command.p_auto_create_prospect_flag.ToSafeString(),
                      p_order_verify = command.p_order_verify.ToSafeString(),
                      p_waiting_install_date = command.p_waiting_install_date.ToSafeString(),
                      p_waiting_time_slot = command.p_waiting_time_slot.ToSafeString(),
                      p_sale_channel = command.p_sale_channel.ToSafeString(),
                      p_owner_product = command.p_owner_product.ToSafeString(),
                      p_package_for = command.p_package_for.ToSafeString(),
                      p_sff_promotion_code = command.p_sff_promotion_code.ToSafeString(),
                      p_region = command.p_region.ToSafeString(),
                      p_province = command.p_province.ToSafeString(),
                      p_district = command.p_district.ToSafeString(),
                      p_sub_district = command.p_sub_district.ToSafeString(),
                      p_serenade_flag = command.p_serenade_flag.ToSafeString(),
                      p_fmpa_flag = command.p_fmpa_flag.ToSafeString(),
                      p_cvm_flag = command.p_cvm_flag.ToSafeString(),
                      p_order_relate_change_pro = command.p_order_relate_change_pro.ToSafeString(),
                      p_company_name = command.p_company_name.ToSafeString(),
                      p_distribution_channel = command.p_distribution_channel.ToSafeString(),
                      p_channel_sales_group = command.p_channel_sales_group.ToSafeString(),
                      p_shop_type = command.p_shop_type.ToSafeString(),
                      p_shop_segment = command.p_shop_segment.ToSafeString(),
                      p_asc_name = command.p_asc_name.ToSafeString(),
                      p_asc_member_category = command.p_asc_member_category.ToSafeString(),
                      p_asc_position = command.p_asc_position.ToSafeString(),
                      p_location_region = command.p_location_region.ToSafeString(),
                      p_location_sub_region = command.p_location_sub_region.ToSafeString(),
                      p_employee_name = command.p_employee_name.ToSafeString(),
                      p_customerpurge = command.p_customerpurge.ToSafeString(),
                      p_exceptentryfee = command.p_exceptentryfee.ToSafeString(),
                      p_secondinstallation = command.p_secondinstallation.ToSafeString(),
                      p_amendment_flag = command.p_amendment_flag.ToSafeString(),
                      p_service_level = command.p_service_level.ToSafeString(),
                      p_first_install_date = command.p_first_install_date.ToSafeString(),
                      p_first_time_slot = command.p_first_time_slot.ToSafeString(),
                      p_line_temp_id = command.p_line_temp_id.ToSafeString(),
                      p_fmc_special_flag = command.p_fmc_special_flag.ToSafeString(),
                      p_non_res_flag = command.p_non_res_flag.ToSafeString(),
                      p_criteria_mobile = command.p_criteria_mobile.ToSafeString(),
                      p_remark_for_subcontract = command.p_remark_for_subcontract.ToSafeString(),
                      p_mesh_count = command.p_mesh_count.ToSafeString(),
                      p_online_flag = command.p_online_flag.ToSafeString(),
                      p_privilege_points = command.p_privilege_points.ToSafeString(),
                      p_transaction_privilege_id = command.p_transaction_privilege_id.ToSafeString(),
                      p_special_skill = command.p_special_skill.ToSafeString(),
                      p_tdm_contract_id = command.p_tdm_contract_id.ToSafeString(),
                      p_tdm_rule_id = command.p_tdm_rule_id.ToSafeString(),
                      p_tdm_penalty_id = command.p_tdm_penalty_id.ToSafeString(),
                      p_tdm_penalty_group_id = command.p_tdm_penalty_group_id.ToSafeString(),
                      p_duration = command.p_duration.ToSafeString(),
                      p_contract_flag = command.p_contract_flag.ToSafeString(),
                      p_national_id = command.p_national_id.ToSafeString(),
                      p_non_mobile_no = command.p_non_mobile_no.ToSafeString(),
                      p_regist_paymentId = command.p_regist_paymentId.ToSafeString(),
                      p_regist_paymentDate = command.p_regist_paymentDate.ToSafeString(),
                      p_regist_paymentMethod = command.p_regist_paymentMethod.ToSafeString(),

                      // in array
                      p_air_regist_package_array = p_air_regist_package_array,
                      p_air_regist_file_array = p_air_regist_file_array,
                      p_air_regist_splitter_array = p_air_regist_splitter_array,
                      p_air_regist_cpe_serial_array = p_air_regist_cpe_serial_array,
                      p_air_regist_cust_insi_array = p_air_regist_cust_insi_array,
                      p_air_regist_dcontract_array = p_air_regist_dcontract_array,

                      // out
                      o_return_code = o_return_code,
                      o_return_message = o_return_message,
                      o_return_order_no = o_return_order_no,
                      o_return_Multi_Instance_flag = o_return_Multi_Instance_flag,
                      o_return_product_subtype = o_return_product_subtype,
                      o_return_event_code = o_return_event_code
                  });

                if (paramOut.Count() > 0)
                {
                    if ((OracleParameter)(paramOut[paramOut.Count() - 6]) != null)
                    {
                        command.o_return_code = decimal.Parse(((OracleParameter)(paramOut[paramOut.Count() - 6])).Value.ToSafeString());
                    }
                    if ((OracleParameter)(paramOut[paramOut.Count() - 5]) != null)
                    {
                        command.o_return_message = ((OracleParameter)(paramOut[paramOut.Count() - 5])).Value.ToSafeString();
                    }
                    if ((OracleParameter)(paramOut[paramOut.Count() - 4]) != null)
                    {
                        command.o_return_order_no = ((OracleParameter)(paramOut[paramOut.Count() - 4])).Value.ToSafeString();
                    }
                    if ((OracleParameter)(paramOut[paramOut.Count() - 3]) != null)
                    {
                        command.o_return_Multi_Instance_flag = ((OracleParameter)(paramOut[paramOut.Count() - 3])).Value.ToSafeString();
                    }
                    if ((OracleParameter)(paramOut[paramOut.Count() - 2]) != null)
                    {
                        command.o_return_product_subtype = ((OracleParameter)(paramOut[paramOut.Count() - 2])).Value.ToSafeString();
                    }
                    if ((OracleParameter)(paramOut[paramOut.Count() - 1]) != null)
                    {
                        command.o_return_event_code = ((OracleParameter)(paramOut[paramOut.Count() - 1])).Value.ToSafeString();
                    }
                }

                if (command.o_return_code == 0)
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, command, log, "Success", "", "");
                }
                else
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, command, log, "Failed", command.o_return_message, "");
                }

            }
            catch (Exception ex)
            {
                _logger.Info(ex.GetErrorMessage());
                command.o_return_code = -1;
                command.o_return_message = "Error call SavePreregister Handler: " + ex.GetErrorMessage();
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, command, log, "Failed", ex.GetErrorMessage(), "");
            }
        }
    }

    #region Mapping AIR_REGIST_PACKAGE Type Oracle
    public class AirRegistPackageObjectModel : Oracle.ManagedDataAccess.Types.INullable, IOracleCustomType
    {
        [OracleArrayMapping()]
        public Air_Regist_PackageOracleTypeMapping[] REC_REG_PACKAGE { get; set; }

        private bool objectIsNull;

        public bool IsNull
        {
            get { return objectIsNull; }
        }

        public static AirRegistPackageObjectModel Null
        {
            get
            {
                AirRegistPackageObjectModel obj = new AirRegistPackageObjectModel();
                obj.objectIsNull = true;
                return obj;
            }
        }

        public void FromCustomObject(OracleConnection con, object udt)
        {
            OracleUdt.SetValue(con, udt, 0, REC_REG_PACKAGE);
        }

        public void ToCustomObject(OracleConnection con, object udt)
        {
            REC_REG_PACKAGE = (Air_Regist_PackageOracleTypeMapping[])OracleUdt.GetValue(con, udt, 0);
        }
    }

    [OracleCustomTypeMappingAttribute("AIR_REGIST_PACKAGE_RECORD")]
    public class Air_Regist_PackageOracleTypeMappingFactory : IOracleCustomTypeFactory
    {
        #region IOracleCustomTypeFactory Members

        public IOracleCustomType CreateObject()
        {
            return new Air_Regist_PackageOracleTypeMapping();
        }

        #endregion
    }
    [OracleCustomTypeMapping("AIR_REGIST_PACKAGE_ARRAY")]
    public class AirRegistPackageObjectModelFactory : IOracleCustomTypeFactory, IOracleArrayTypeFactory
    {
        #region IOracleCustomTypeFactory Members
        public IOracleCustomType CreateObject()
        {
            return new AirRegistPackageObjectModel();
        }

        #endregion

        #region IOracleArrayTypeFactory Members
        public Array CreateArray(int numElems)
        {
            return new Air_Regist_PackageOracleTypeMapping[numElems];
        }

        public Array CreateStatusArray(int numElems)
        {
            return null;
        }

        #endregion
    }

    public class Air_Regist_PackageOracleTypeMapping : Oracle.ManagedDataAccess.Types.INullable, IOracleCustomType
    {
        private bool objectIsNull;

        #region Attribute Mapping
        [OracleObjectMappingAttribute("TEMP_IA")]
        public string temp_ia { get; set; }
        [OracleObjectMappingAttribute("PRODUCT_SUBTYPE")]
        public string product_subtype { get; set; }
        [OracleObjectMappingAttribute("PACKAGE_TYPE")]
        public string package_type { get; set; }
        [OracleObjectMappingAttribute("PACKAGE_CODE")]
        public string package_code { get; set; }
        [OracleObjectMappingAttribute("PACKAGE_PRICE")]
        public decimal package_price { get; set; }
        [OracleObjectMappingAttribute("IDD_FLAG")]
        public string idd_flag { get; set; }
        [OracleObjectMappingAttribute("FAX_FLAG")]
        public string fax_flag { get; set; }
        [OracleObjectMappingAttribute("HOME_IP")]
        public string home_ip { get; set; }
        [OracleObjectMappingAttribute("HOME_PORT")]
        public string home_port { get; set; }
        [OracleObjectMappingAttribute("MOBILE_FORWARD")]
        public string mobile_forward { get; set; }
        [OracleObjectMappingAttribute("PBOX_EXT")]
        public string pbox_ext { get; set; }



        #endregion

        public static Air_Regist_PackageOracleTypeMapping Null
        {
            get
            {
                Air_Regist_PackageOracleTypeMapping obj = new Air_Regist_PackageOracleTypeMapping();
                obj.objectIsNull = true;
                return obj;
            }
        }

        public bool IsNull
        {
            get { return objectIsNull; }
        }

        public void FromCustomObject(OracleConnection con, object udt)
        {
            OracleUdt.SetValue(con, udt, "TEMP_IA", temp_ia);
            OracleUdt.SetValue(con, udt, "PRODUCT_SUBTYPE", product_subtype);
            OracleUdt.SetValue(con, udt, "PACKAGE_TYPE", package_type);
            OracleUdt.SetValue(con, udt, "PACKAGE_CODE", package_code);
            OracleUdt.SetValue(con, udt, "PACKAGE_PRICE", package_price);
            OracleUdt.SetValue(con, udt, "IDD_FLAG", idd_flag);
            OracleUdt.SetValue(con, udt, "FAX_FLAG", fax_flag);
            OracleUdt.SetValue(con, udt, "FAX_FLAG", home_ip);
            OracleUdt.SetValue(con, udt, "FAX_FLAG", home_port);
            OracleUdt.SetValue(con, udt, "MOBILE_FORWARD", mobile_forward);
            OracleUdt.SetValue(con, udt, "PBOX_EXT", pbox_ext);
        }

        public void ToCustomObject(OracleConnection con, object udt)
        {
            throw new NotImplementedException();
        }
    }
    #endregion

    #region Mapping AIR_REGIST_FILE Type Oracle
    public class AirRegistFileObjectModel : Oracle.ManagedDataAccess.Types.INullable, IOracleCustomType
    {
        [OracleArrayMapping()]
        public Air_Regist_FileOracleTypeMapping[] REC_REG_PACKAGE { get; set; }

        private bool objectIsNull;

        public bool IsNull
        {
            get { return objectIsNull; }
        }

        public static AirRegistFileObjectModel Null
        {
            get
            {
                AirRegistFileObjectModel obj = new AirRegistFileObjectModel();
                obj.objectIsNull = true;
                return obj;
            }
        }

        public void FromCustomObject(OracleConnection con, object udt)
        {
            OracleUdt.SetValue(con, udt, 0, REC_REG_PACKAGE);
        }

        public void ToCustomObject(OracleConnection con, object udt)
        {
            REC_REG_PACKAGE = (Air_Regist_FileOracleTypeMapping[])OracleUdt.GetValue(con, udt, 0);
        }
    }

    [OracleCustomTypeMappingAttribute("AIR_REGIST_FILE_RECORD")]
    public class Air_Regist_FileOracleTypeMappingFactory : IOracleCustomTypeFactory
    {
        #region IOracleCustomTypeFactory Members

        public IOracleCustomType CreateObject()
        {
            return new Air_Regist_FileOracleTypeMapping();
        }

        #endregion
    }
    [OracleCustomTypeMapping("AIR_REGIST_FILE_ARRAY")]
    public class AirRegistFileObjectModelFactory : IOracleCustomTypeFactory, IOracleArrayTypeFactory
    {
        #region IOracleCustomTypeFactory Members
        public IOracleCustomType CreateObject()
        {
            return new AirRegistFileObjectModel();
        }

        #endregion

        #region IOracleArrayTypeFactory Members
        public Array CreateArray(int numElems)
        {
            return new Air_Regist_FileOracleTypeMapping[numElems];
        }

        public Array CreateStatusArray(int numElems)
        {
            return null;
        }

        #endregion
    }

    public class Air_Regist_FileOracleTypeMapping : Oracle.ManagedDataAccess.Types.INullable, IOracleCustomType
    {
        private bool objectIsNull;

        #region Attribute Mapping
        [OracleObjectMappingAttribute("FILE_NAME")]
        public string file_name { get; set; }
        #endregion

        public static Air_Regist_FileOracleTypeMapping Null
        {
            get
            {
                Air_Regist_FileOracleTypeMapping obj = new Air_Regist_FileOracleTypeMapping();
                obj.objectIsNull = true;
                return obj;
            }
        }

        public bool IsNull
        {
            get { return objectIsNull; }
        }

        public void FromCustomObject(OracleConnection con, object udt)
        {
            OracleUdt.SetValue(con, udt, "FILE_NAME", file_name);
        }

        public void ToCustomObject(OracleConnection con, object udt)
        {
            throw new NotImplementedException();
        }
    }
    #endregion

    #region Mapping AIR_REGIST_SPLITTER Type Oracle
    public class AirRegistSplitterObjectModel : Oracle.ManagedDataAccess.Types.INullable, IOracleCustomType
    {
        [OracleArrayMapping()]
        public Air_Regist_SplitterOracleTypeMapping[] REC_REG_PACKAGE { get; set; }

        private bool objectIsNull;

        public bool IsNull
        {
            get { return objectIsNull; }
        }

        public static AirRegistSplitterObjectModel Null
        {
            get
            {
                AirRegistSplitterObjectModel obj = new AirRegistSplitterObjectModel();
                obj.objectIsNull = true;
                return obj;
            }
        }

        public void FromCustomObject(OracleConnection con, object udt)
        {
            OracleUdt.SetValue(con, udt, 0, REC_REG_PACKAGE);
        }

        public void ToCustomObject(OracleConnection con, object udt)
        {
            REC_REG_PACKAGE = (Air_Regist_SplitterOracleTypeMapping[])OracleUdt.GetValue(con, udt, 0);
        }
    }

    [OracleCustomTypeMappingAttribute("AIR_REGIST_SPLITTER_RECORD")]
    public class Air_Regist_SplitterOracleTypeMappingFactory : IOracleCustomTypeFactory
    {
        #region IOracleCustomTypeFactory Members

        public IOracleCustomType CreateObject()
        {
            return new Air_Regist_SplitterOracleTypeMapping();
        }

        #endregion
    }
    [OracleCustomTypeMapping("AIR_REGIST_SPLITTER_ARRAY")]
    public class Air_Regist_SplitterObjectModelFactory : IOracleCustomTypeFactory, IOracleArrayTypeFactory
    {
        #region IOracleCustomTypeFactory Members
        public IOracleCustomType CreateObject()
        {
            return new AirRegistSplitterObjectModel();
        }

        #endregion

        #region IOracleArrayTypeFactory Members
        public Array CreateArray(int numElems)
        {
            return new Air_Regist_SplitterOracleTypeMapping[numElems];
        }

        public Array CreateStatusArray(int numElems)
        {
            return null;
        }

        #endregion
    }

    public class Air_Regist_SplitterOracleTypeMapping : Oracle.ManagedDataAccess.Types.INullable, IOracleCustomType
    {
        private bool objectIsNull;

        #region Attribute Mapping
        [OracleObjectMappingAttribute("SPLITTER_NAME")]
        public string SPLITTER_NAME { get; set; }
        [OracleObjectMappingAttribute("DISTANCE")]
        public decimal DISTANCE { get; set; }
        [OracleObjectMappingAttribute("DISTANCE_TYPE")]
        public string DISTANCE_TYPE { get; set; }
        [OracleObjectMappingAttribute("RESOURCE_TYPE")]
        public string RESOURCE_TYPE { get; set; }



        #endregion

        public static Air_Regist_SplitterOracleTypeMapping Null
        {
            get
            {
                Air_Regist_SplitterOracleTypeMapping obj = new Air_Regist_SplitterOracleTypeMapping();
                obj.objectIsNull = true;
                return obj;
            }
        }

        public bool IsNull
        {
            get { return objectIsNull; }
        }

        public void FromCustomObject(OracleConnection con, object udt)
        {
            OracleUdt.SetValue(con, udt, "SPLITTER_NAME", SPLITTER_NAME);
            OracleUdt.SetValue(con, udt, "DISTANCE", DISTANCE);
            OracleUdt.SetValue(con, udt, "DISTANCE_TYPE", DISTANCE_TYPE);
            OracleUdt.SetValue(con, udt, "RESOURCE_TYPE", RESOURCE_TYPE);
        }

        public void ToCustomObject(OracleConnection con, object udt)
        {
            throw new NotImplementedException();
        }
    }
    #endregion

    #region Mapping AIR_REGIST_CPE_SERIAL_ Type Oracle
    public class AirRegistCPESerialObjectModel : Oracle.ManagedDataAccess.Types.INullable, IOracleCustomType
    {
        [OracleArrayMapping()]
        public Air_Regist_CPE_SerialOracleTypeMapping[] REC_REG_PACKAGE { get; set; }

        private bool objectIsNull;

        public bool IsNull
        {
            get { return objectIsNull; }
        }

        public static AirRegistCPESerialObjectModel Null
        {
            get
            {
                AirRegistCPESerialObjectModel obj = new AirRegistCPESerialObjectModel();
                obj.objectIsNull = true;
                return obj;
            }
        }

        public void FromCustomObject(OracleConnection con, object udt)
        {
            OracleUdt.SetValue(con, udt, 0, REC_REG_PACKAGE);
        }

        public void ToCustomObject(OracleConnection con, object udt)
        {
            REC_REG_PACKAGE = (Air_Regist_CPE_SerialOracleTypeMapping[])OracleUdt.GetValue(con, udt, 0);
        }
    }

    [OracleCustomTypeMappingAttribute("AIR_REGIST_CPE_SERIAL_RECORD")]
    public class Air_Regist_CPE_SerialOracleTypeMappingFactory : IOracleCustomTypeFactory
    {
        #region IOracleCustomTypeFactory Members

        public IOracleCustomType CreateObject()
        {
            return new Air_Regist_CPE_SerialOracleTypeMapping();
        }

        #endregion
    }
    [OracleCustomTypeMapping("AIR_REGIST_CPE_SERIAL_ARRAY")]
    public class Air_Regist_CPE_SerialObjectModelFactory : IOracleCustomTypeFactory, IOracleArrayTypeFactory
    {
        #region IOracleCustomTypeFactory Members
        public IOracleCustomType CreateObject()
        {
            return new AirRegistCPESerialObjectModel();
        }

        #endregion

        #region IOracleArrayTypeFactory Members
        public Array CreateArray(int numElems)
        {
            return new Air_Regist_CPE_SerialOracleTypeMapping[numElems];
        }

        public Array CreateStatusArray(int numElems)
        {
            return null;
        }

        #endregion
    }

    public class Air_Regist_CPE_SerialOracleTypeMapping : Oracle.ManagedDataAccess.Types.INullable, IOracleCustomType
    {
        private bool objectIsNull;

        #region Attribute Mapping
        [OracleObjectMappingAttribute("CPE_TYPE")]
        public string CPE_TYPE { get; set; }
        [OracleObjectMappingAttribute("SERIAL_NO")]
        public string SERIAL_NO { get; set; }
        [OracleObjectMappingAttribute("MAC_ADDRESS")]
        public string MAC_ADDRESS { get; set; }
        [OracleObjectMappingAttribute("STATUS_DESC")]
        public string STATUS_DESC { get; set; }
        [OracleObjectMappingAttribute("MODEL_NAME")]
        public string MODEL_NAME { get; set; }
        [OracleObjectMappingAttribute("COMPANY_CODE")]
        public string COMPANY_CODE { get; set; }
        [OracleObjectMappingAttribute("CPE_PLANT")]
        public string CPE_PLANT { get; set; }
        [OracleObjectMappingAttribute("STORAGE_LOCATION")]
        public string STORAGE_LOCATION { get; set; }
        [OracleObjectMappingAttribute("MATERIAL_CODE")]
        public string MATERIAL_CODE { get; set; }
        [OracleObjectMappingAttribute("REGISTER_DATE")]
        public string REGISTER_DATE { get; set; }
        [OracleObjectMappingAttribute("FIBRENET_ID")]
        public string FIBRENET_ID { get; set; }
        [OracleObjectMappingAttribute("SN_PATTERN")]
        public string SN_PATTERN { get; set; }
        [OracleObjectMappingAttribute("SHIP_TO")]
        public string SHIP_TO { get; set; }
        [OracleObjectMappingAttribute("WARRANTY_START_DATE")]
        public string WARRANTY_START_DATE { get; set; }
        [OracleObjectMappingAttribute("WARRANTY_END_DATE")]
        public string WARRANTY_END_DATE { get; set; }
        #endregion

        public static Air_Regist_CPE_SerialOracleTypeMapping Null
        {
            get
            {
                Air_Regist_CPE_SerialOracleTypeMapping obj = new Air_Regist_CPE_SerialOracleTypeMapping();
                obj.objectIsNull = true;
                return obj;
            }
        }

        public bool IsNull
        {
            get { return objectIsNull; }
        }

        public void FromCustomObject(OracleConnection con, object udt)
        {
            OracleUdt.SetValue(con, udt, "CPE_TYPE", CPE_TYPE);
            OracleUdt.SetValue(con, udt, "SERIAL_NO", SERIAL_NO);
            OracleUdt.SetValue(con, udt, "MAC_ADDRESS", MAC_ADDRESS);
            OracleUdt.SetValue(con, udt, "STATUS_DESC", STATUS_DESC);
            OracleUdt.SetValue(con, udt, "MODEL_NAME", MODEL_NAME);
            OracleUdt.SetValue(con, udt, "COMPANY_CODE", COMPANY_CODE);
            OracleUdt.SetValue(con, udt, "CPE_PLANT", CPE_PLANT);
            OracleUdt.SetValue(con, udt, "STORAGE_LOCATION", STORAGE_LOCATION);
            OracleUdt.SetValue(con, udt, "MATERIAL_CODE", MATERIAL_CODE);
            OracleUdt.SetValue(con, udt, "REGISTER_DATE", REGISTER_DATE);
            OracleUdt.SetValue(con, udt, "FIBRENET_ID", FIBRENET_ID);
            OracleUdt.SetValue(con, udt, "SN_PATTERN", SN_PATTERN);
            OracleUdt.SetValue(con, udt, "SHIP_TO", SHIP_TO);
            OracleUdt.SetValue(con, udt, "WARRANTY_START_DATE", WARRANTY_START_DATE);
            OracleUdt.SetValue(con, udt, "WARRANTY_END_DATE", WARRANTY_END_DATE);
        }

        public void ToCustomObject(OracleConnection con, object udt)
        {
            throw new NotImplementedException();
        }
    }
    #endregion

    #region Mapping AIR_REGIST_CUST_INSIGHT Type Oracle
    public class AirRegistCustInsightObjectModel : Oracle.ManagedDataAccess.Types.INullable, IOracleCustomType
    {
        [OracleArrayMapping()]
        public Air_Regist_Cust_InsightOracleTypeMapping[] REC_REG_PACKAGE { get; set; }

        private bool objectIsNull;

        public bool IsNull
        {
            get { return objectIsNull; }
        }

        public static AirRegistCustInsightObjectModel Null
        {
            get
            {
                AirRegistCustInsightObjectModel obj = new AirRegistCustInsightObjectModel();
                obj.objectIsNull = true;
                return obj;
            }
        }

        public void FromCustomObject(OracleConnection con, object udt)
        {
            OracleUdt.SetValue(con, udt, 0, REC_REG_PACKAGE);
        }

        public void ToCustomObject(OracleConnection con, object udt)
        {
            REC_REG_PACKAGE = (Air_Regist_Cust_InsightOracleTypeMapping[])OracleUdt.GetValue(con, udt, 0);
        }
    }

    [OracleCustomTypeMappingAttribute("AIR_REGIST_CUST_INSIGHT_RECORD")]
    public class Air_Regist_Cust_InsightOracleTypeMappingFactory : IOracleCustomTypeFactory
    {
        #region IOracleCustomTypeFactory Members

        public IOracleCustomType CreateObject()
        {
            return new Air_Regist_Cust_InsightOracleTypeMapping();
        }

        #endregion
    }
    [OracleCustomTypeMapping("AIR_REGIST_CUST_INSIGHT_ARRAY")]
    public class Air_Regist_Cust_InsightObjectModelFactory : IOracleCustomTypeFactory, IOracleArrayTypeFactory
    {
        #region IOracleCustomTypeFactory Members
        public IOracleCustomType CreateObject()
        {
            return new AirRegistCustInsightObjectModel();
        }

        #endregion

        #region IOracleArrayTypeFactory Members
        public Array CreateArray(int numElems)
        {
            return new Air_Regist_Cust_InsightOracleTypeMapping[numElems];
        }

        public Array CreateStatusArray(int numElems)
        {
            return null;
        }

        #endregion
    }

    public class Air_Regist_Cust_InsightOracleTypeMapping : Oracle.ManagedDataAccess.Types.INullable, IOracleCustomType
    {
        private bool objectIsNull;

        #region Attribute Mapping
        [OracleObjectMappingAttribute("GROUP_ID")]
        public string GROUP_ID { get; set; }
        [OracleObjectMappingAttribute("GROUP_NAME_TH")]
        public string GROUP_NAME_TH { get; set; }
        [OracleObjectMappingAttribute("GROUP_NAME_EN")]
        public string GROUP_NAME_EN { get; set; }
        [OracleObjectMappingAttribute("QUESTION_ID")]
        public string QUESTION_ID { get; set; }
        [OracleObjectMappingAttribute("QUESTION_TH")]
        public string QUESTION_TH { get; set; }
        [OracleObjectMappingAttribute("QUESTION_EN")]
        public string QUESTION_EN { get; set; }
        [OracleObjectMappingAttribute("ANSWER_ID")]
        public string ANSWER_ID { get; set; }
        [OracleObjectMappingAttribute("ANSWER_TH")]
        public string ANSWER_TH { get; set; }
        [OracleObjectMappingAttribute("ANSWER_EN")]
        public string ANSWER_EN { get; set; }
        [OracleObjectMappingAttribute("ANSWER_VALUE_TH")]
        public string ANSWER_VALUE_TH { get; set; }
        [OracleObjectMappingAttribute("ANSWER_VALUE_EN")]
        public string ANSWER_VALUE_EN { get; set; }
        [OracleObjectMappingAttribute("PARENT_ANSWER_ID")]
        public string PARENT_ANSWER_ID { get; set; }
        [OracleObjectMappingAttribute("ACTION_WFM")]
        public string ACTION_WFM { get; set; }
        [OracleObjectMappingAttribute("ACTION_FOA")]
        public string ACTION_FOA { get; set; }

        #endregion

        public static Air_Regist_Cust_InsightOracleTypeMapping Null
        {
            get
            {
                Air_Regist_Cust_InsightOracleTypeMapping obj = new Air_Regist_Cust_InsightOracleTypeMapping();
                obj.objectIsNull = true;
                return obj;
            }
        }

        public bool IsNull
        {
            get { return objectIsNull; }
        }

        public void FromCustomObject(OracleConnection con, object udt)
        {
            OracleUdt.SetValue(con, udt, "GROUP_ID", GROUP_ID);
            OracleUdt.SetValue(con, udt, "GROUP_NAME_TH", GROUP_NAME_TH);
            OracleUdt.SetValue(con, udt, "GROUP_NAME_EN", GROUP_NAME_EN);
            OracleUdt.SetValue(con, udt, "QUESTION_ID", QUESTION_ID);
            OracleUdt.SetValue(con, udt, "QUESTION_TH", QUESTION_TH);
            OracleUdt.SetValue(con, udt, "QUESTION_EN", QUESTION_EN);
            OracleUdt.SetValue(con, udt, "ANSWER_ID", ANSWER_ID);
            OracleUdt.SetValue(con, udt, "ANSWER_TH", ANSWER_TH);
            OracleUdt.SetValue(con, udt, "ANSWER_EN", ANSWER_EN);
            OracleUdt.SetValue(con, udt, "ANSWER_VALUE_TH", ANSWER_VALUE_TH);
            OracleUdt.SetValue(con, udt, "ANSWER_VALUE_EN", ANSWER_VALUE_EN);
            OracleUdt.SetValue(con, udt, "PARENT_ANSWER_ID", PARENT_ANSWER_ID);
            OracleUdt.SetValue(con, udt, "ACTION_WFM", ACTION_WFM);
            OracleUdt.SetValue(con, udt, "ACTION_FOA", ACTION_FOA);
        }

        public void ToCustomObject(OracleConnection con, object udt)
        {
            throw new NotImplementedException();
        }
    }
    #endregion

    #region Mapping AIR_REGIST_DCONTRACT Type Oracle
    public class AirRegistDcontractRecordObjectModel : Oracle.ManagedDataAccess.Types.INullable, IOracleCustomType
    {
        [OracleArrayMapping()]
        public Air_Regist_Dcontract_RecordOracleTypeMapping[] REC_REG_PACKAGE { get; set; }

        private bool objectIsNull;

        public bool IsNull
        {
            get { return objectIsNull; }
        }

        public static AirRegistDcontractRecordObjectModel Null
        {
            get
            {
                AirRegistDcontractRecordObjectModel obj = new AirRegistDcontractRecordObjectModel();
                obj.objectIsNull = true;
                return obj;
            }
        }

        public void FromCustomObject(OracleConnection con, object udt)
        {
            OracleUdt.SetValue(con, udt, 0, REC_REG_PACKAGE);
        }

        public void ToCustomObject(OracleConnection con, object udt)
        {
            REC_REG_PACKAGE = (Air_Regist_Dcontract_RecordOracleTypeMapping[])OracleUdt.GetValue(con, udt, 0);
        }
    }

    [OracleCustomTypeMappingAttribute("AIR_REGIST_DCONTRACT_RECORD")]
    public class Air_Regist_Dcontract_RecordOracleTypeMappingFactory : IOracleCustomTypeFactory
    {
        #region IOracleCustomTypeFactory Members

        public IOracleCustomType CreateObject()
        {
            return new Air_Regist_Dcontract_RecordOracleTypeMapping();
        }

        #endregion
    }
    [OracleCustomTypeMapping("AIR_REGIST_DCONTRACT_ARRAY")]
    public class Air_Regist_Dcontract_RecordObjectModelFactory : IOracleCustomTypeFactory, IOracleArrayTypeFactory
    {
        #region IOracleCustomTypeFactory Members
        public IOracleCustomType CreateObject()
        {
            return new AirRegistDcontractRecordObjectModel();
        }

        #endregion

        #region IOracleArrayTypeFactory Members
        public Array CreateArray(int numElems)
        {
            return new Air_Regist_Dcontract_RecordOracleTypeMapping[numElems];
        }

        public Array CreateStatusArray(int numElems)
        {
            return null;
        }

        #endregion
    }

    public class Air_Regist_Dcontract_RecordOracleTypeMapping : Oracle.ManagedDataAccess.Types.INullable, IOracleCustomType
    {
        private bool objectIsNull;

        #region Attribute Mapping
        [OracleObjectMappingAttribute("PRODUCT_SUBTYPE")]
        public string PRODUCT_SUBTYPE { get; set; }
        [OracleObjectMappingAttribute("PBOX_EXT")]
        public string PBOX_EXT { get; set; }
        [OracleObjectMappingAttribute("TDM_CONTRACT_ID")]
        public string TDM_CONTRACT_ID { get; set; }
        [OracleObjectMappingAttribute("TDM_RULE_ID")]
        public string TDM_RULE_ID { get; set; }
        [OracleObjectMappingAttribute("TDM_PENALTY_ID")]
        public string TDM_PENALTY_ID { get; set; }
        [OracleObjectMappingAttribute("TDM_PENALTY_GROUP_ID")]
        public string TDM_PENALTY_GROUP_ID { get; set; }
        [OracleObjectMappingAttribute("DURATION")]
        public string DURATION { get; set; }
        [OracleObjectMappingAttribute("CONTRACT_FLAG")]
        public string CONTRACT_FLAG { get; set; }
        [OracleObjectMappingAttribute("DEVICE_COUNT")]
        public string DEVICE_COUNT { get; set; }

        #endregion

        public static Air_Regist_Dcontract_RecordOracleTypeMapping Null
        {
            get
            {
                Air_Regist_Dcontract_RecordOracleTypeMapping obj = new Air_Regist_Dcontract_RecordOracleTypeMapping();
                obj.objectIsNull = true;
                return obj;
            }
        }

        public bool IsNull
        {
            get { return objectIsNull; }
        }

        public void FromCustomObject(OracleConnection con, object udt)
        {
            OracleUdt.SetValue(con, udt, "PRODUCT_SUBTYPE", PRODUCT_SUBTYPE);
            OracleUdt.SetValue(con, udt, "PBOX_EXT", PBOX_EXT);
            OracleUdt.SetValue(con, udt, "TDM_CONTRACT_ID", TDM_CONTRACT_ID);
            OracleUdt.SetValue(con, udt, "TDM_RULE_ID", TDM_RULE_ID);
            OracleUdt.SetValue(con, udt, "TDM_PENALTY_ID", TDM_PENALTY_ID);
            OracleUdt.SetValue(con, udt, "TDM_PENALTY_GROUP_ID", TDM_PENALTY_GROUP_ID);
            OracleUdt.SetValue(con, udt, "DURATION", DURATION);
            OracleUdt.SetValue(con, udt, "CONTRACT_FLAG", CONTRACT_FLAG);
            OracleUdt.SetValue(con, udt, "DEVICE_COUNT", DEVICE_COUNT); ;
        }

        public void ToCustomObject(OracleConnection con, object udt)
        {
            throw new NotImplementedException();
        }
    }
    #endregion

}
