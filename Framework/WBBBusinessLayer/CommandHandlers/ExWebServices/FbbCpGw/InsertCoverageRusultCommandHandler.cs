using AIRNETEntity.Extensions;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using WBBBusinessLayer.QueryHandlers;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Commands.ExWebServices.FbbCpGw;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Models;

namespace WBBBusinessLayer.CommandHandlers.ExWebServices.FbbCpGw
{
    public class InsertCoverageRusultCommandHandler : ICommandHandler<InsertCoverageRusultCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<string> _objService;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;

        public InsertCoverageRusultCommandHandler(ILogger logger,
            IEntityRepository<string> objService,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IWBBUnitOfWork uow)
        {
            _logger = logger;
            _objService = objService;
            _intfLog = intfLog;
            _uow = uow;
        }

        public void Handle(InsertCoverageRusultCommand command)
        {
            InterfaceLogCommand log = null;
            try
            {
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, command, command.p_channel, "InsertCoverageRusultCommandHandler", "InsertCoverageRusultCommandHandler", command.p_channel, "FBB", "WEB");

                //Input
                var p_channel = new OracleParameter();
                p_channel.ParameterName = "p_channel";
                p_channel.OracleDbType = OracleDbType.Varchar2;
                p_channel.Size = 1000;
                p_channel.Direction = ParameterDirection.Input;
                p_channel.Value = command.p_channel.ToSafeString();

                var p_address_type = new OracleParameter();
                p_address_type.ParameterName = "p_address_type";
                p_address_type.OracleDbType = OracleDbType.Varchar2;
                p_address_type.Size = 1000;
                p_address_type.Direction = ParameterDirection.Input;
                p_address_type.Value = command.p_address_type.ToSafeString();

                var p_postal_code = new OracleParameter();
                p_postal_code.ParameterName = "p_postal_code";
                p_postal_code.OracleDbType = OracleDbType.Varchar2;
                p_postal_code.Size = 1000;
                p_postal_code.Direction = ParameterDirection.Input;
                p_postal_code.Value = command.p_postal_code.ToSafeString();

                var p_district = new OracleParameter();
                p_district.ParameterName = "p_district";
                p_district.OracleDbType = OracleDbType.Varchar2;
                p_district.Size = 1000;
                p_district.Direction = ParameterDirection.Input;
                p_district.Value = command.p_district.ToSafeString();

                var p_sub_district = new OracleParameter();
                p_sub_district.ParameterName = "p_sub_district";
                p_sub_district.OracleDbType = OracleDbType.Varchar2;
                p_sub_district.Size = 1000;
                p_sub_district.Direction = ParameterDirection.Input;
                p_sub_district.Value = command.p_sub_district.ToSafeString();

                var p_language = new OracleParameter();
                p_language.ParameterName = "p_language";
                p_language.OracleDbType = OracleDbType.Varchar2;
                p_language.Size = 1000;
                p_language.Direction = ParameterDirection.Input;
                p_language.Value = command.p_language.ToSafeString();

                var p_building_name = new OracleParameter();
                p_building_name.ParameterName = "p_building_name";
                p_building_name.OracleDbType = OracleDbType.Varchar2;
                p_building_name.Size = 1000;
                p_building_name.Direction = ParameterDirection.Input;
                p_building_name.Value = command.p_building_name.ToSafeString();

                var p_building_no = new OracleParameter();
                p_building_no.ParameterName = "p_building_no";
                p_building_no.OracleDbType = OracleDbType.Varchar2;
                p_building_no.Size = 1000;
                p_building_no.Direction = ParameterDirection.Input;
                p_building_no.Value = command.p_building_no.ToSafeString();

                var p_phone_flag = new OracleParameter();
                p_phone_flag.ParameterName = "p_phone_flag";
                p_phone_flag.OracleDbType = OracleDbType.Varchar2;
                p_phone_flag.Size = 1000;
                p_phone_flag.Direction = ParameterDirection.Input;
                p_phone_flag.Value = command.p_phone_flag.ToSafeString();

                var p_floor_no = new OracleParameter();
                p_floor_no.ParameterName = "p_floor_no";
                p_floor_no.OracleDbType = OracleDbType.Varchar2;
                p_floor_no.Size = 1000;
                p_floor_no.Direction = ParameterDirection.Input;
                p_floor_no.Value = command.p_floor_no.ToSafeString();

                var p_address_no = new OracleParameter();
                p_address_no.ParameterName = "p_address_no";
                p_address_no.OracleDbType = OracleDbType.Varchar2;
                p_address_no.Size = 1000;
                p_address_no.Direction = ParameterDirection.Input;
                p_address_no.Value = command.p_address_no.ToSafeString();

                var p_moo = new OracleParameter();
                p_moo.ParameterName = "p_moo";
                p_moo.OracleDbType = OracleDbType.Varchar2;
                p_moo.Size = 1000;
                p_moo.Direction = ParameterDirection.Input;
                p_moo.Value = command.p_moo.ToSafeString();

                var p_soi = new OracleParameter();
                p_soi.ParameterName = "p_soi";
                p_soi.OracleDbType = OracleDbType.Varchar2;
                p_soi.Size = 1000;
                p_soi.Direction = ParameterDirection.Input;
                p_soi.Value = command.p_soi.ToSafeString();

                var p_road = new OracleParameter();
                p_road.ParameterName = "p_road";
                p_road.OracleDbType = OracleDbType.Varchar2;
                p_road.Size = 50;
                p_road.Direction = ParameterDirection.Input;
                p_road.Value = command.p_road.ToSafeString();

                var p_latitude = new OracleParameter();
                p_latitude.ParameterName = "p_latitude";
                p_latitude.OracleDbType = OracleDbType.Varchar2;
                p_latitude.Size = 1000;
                p_latitude.Direction = ParameterDirection.Input;
                p_latitude.Value = command.p_latitude.ToSafeString();

                var p_longitude = new OracleParameter();
                p_longitude.ParameterName = "p_longitude";
                p_longitude.OracleDbType = OracleDbType.Varchar2;
                p_longitude.Size = 1000;
                p_longitude.Direction = ParameterDirection.Input;
                p_longitude.Value = command.p_longitude.ToSafeString();

                var p_unit_no = new OracleParameter();
                p_unit_no.ParameterName = "p_unit_no";
                p_unit_no.OracleDbType = OracleDbType.Varchar2;
                p_unit_no.Size = 1000;
                p_unit_no.Direction = ParameterDirection.Input;
                p_unit_no.Value = command.p_unit_no.ToSafeString();

                var p_coverage_flag = new OracleParameter();
                p_coverage_flag.ParameterName = "p_coverage_flag";
                p_coverage_flag.OracleDbType = OracleDbType.Varchar2;
                p_coverage_flag.Size = 1000;
                p_coverage_flag.Direction = ParameterDirection.Input;
                p_coverage_flag.Value = command.p_coverage_flag.ToSafeString();

                var p_address_id = new OracleParameter();
                p_address_id.ParameterName = "p_address_id";
                p_address_id.OracleDbType = OracleDbType.Varchar2;
                p_address_id.Size = 1000;
                p_address_id.Direction = ParameterDirection.Input;
                p_address_id.Value = command.p_address_id.ToSafeString();

                var p_is_partner = new OracleParameter();
                p_is_partner.ParameterName = "p_is_partner";
                p_is_partner.OracleDbType = OracleDbType.Varchar2;
                p_is_partner.Size = 1000;
                p_is_partner.Direction = ParameterDirection.Input;
                p_is_partner.Value = command.p_is_partner.ToSafeString();

                var p_partner_name = new OracleParameter();
                p_partner_name.ParameterName = "p_partner_name";
                p_partner_name.OracleDbType = OracleDbType.Varchar2;
                p_partner_name.Size = 1000;
                p_partner_name.Direction = ParameterDirection.Input;
                p_partner_name.Value = command.p_partner_name.ToSafeString();

                var p_firstname = new OracleParameter();
                p_firstname.ParameterName = "p_firstname";
                p_firstname.OracleDbType = OracleDbType.Varchar2;
                p_firstname.Size = 1000;
                p_firstname.Direction = ParameterDirection.Input;
                p_firstname.Value = command.p_firstname.ToSafeString();

                var p_lastname = new OracleParameter();
                p_lastname.ParameterName = "p_lastname";
                p_lastname.OracleDbType = OracleDbType.Varchar2;
                p_lastname.Size = 1000;
                p_lastname.Direction = ParameterDirection.Input;
                p_lastname.Value = command.p_lastname.ToSafeString();

                var p_contactnumber = new OracleParameter();
                p_contactnumber.ParameterName = "p_contactnumber";
                p_contactnumber.OracleDbType = OracleDbType.Varchar2;
                p_contactnumber.Size = 1000;
                p_contactnumber.Direction = ParameterDirection.Input;
                p_contactnumber.Value = command.p_contactnumber.ToSafeString();

                var p_producttype = new OracleParameter();
                p_producttype.ParameterName = "p_producttype";
                p_producttype.OracleDbType = OracleDbType.Varchar2;
                p_producttype.Size = 1000;
                p_producttype.Direction = ParameterDirection.Input;
                p_producttype.Value = command.p_producttype.ToSafeString();

                var p_owner_product = new OracleParameter();
                p_owner_product.ParameterName = "p_owner_product";
                p_owner_product.OracleDbType = OracleDbType.Varchar2;
                p_owner_product.Size = 1000;
                p_owner_product.Direction = ParameterDirection.Input;
                p_owner_product.Value = command.p_owner_product.ToSafeString();

                var p_splitter_name = new OracleParameter();
                p_splitter_name.ParameterName = "p_splitter_name";
                p_splitter_name.OracleDbType = OracleDbType.Varchar2;
                p_splitter_name.Size = 1000;
                p_splitter_name.Direction = ParameterDirection.Input;
                p_splitter_name.Value = command.p_splitter_name.ToSafeString();

                var p_distance = new OracleParameter();
                p_distance.ParameterName = "p_distance";
                p_distance.OracleDbType = OracleDbType.Varchar2;
                p_distance.Size = 1000;
                p_distance.Direction = ParameterDirection.Input;
                p_distance.Value = command.p_distance.ToSafeString();

                var p_contact_email = new OracleParameter();
                p_contact_email.ParameterName = "p_contact_email";
                p_contact_email.OracleDbType = OracleDbType.Varchar2;
                p_contact_email.Size = 2000;
                p_contact_email.Direction = ParameterDirection.Input;
                p_contact_email.Value = command.p_contact_email.ToSafeString();

                var p_contact_line_id = new OracleParameter();
                p_contact_line_id.ParameterName = "p_contact_line_id";
                p_contact_line_id.OracleDbType = OracleDbType.Varchar2;
                p_contact_line_id.Size = 2000;
                p_contact_line_id.Direction = ParameterDirection.Input;
                p_contact_line_id.Value = command.p_contact_line_id.ToSafeString();

                var p_location_code = new OracleParameter();
                p_location_code.ParameterName = "p_location_code";
                p_location_code.OracleDbType = OracleDbType.Varchar2;
                p_location_code.Size = 1000;
                p_location_code.Direction = ParameterDirection.Input;
                p_location_code.Value = command.p_location_code.ToSafeString();

                var p_asc_code = new OracleParameter();
                p_asc_code.ParameterName = "p_asc_code";
                p_asc_code.OracleDbType = OracleDbType.Varchar2;
                p_asc_code.Size = 1000;
                p_asc_code.Direction = ParameterDirection.Input;
                p_asc_code.Value = command.p_asc_code.ToSafeString();

                var p_employee_id = new OracleParameter();
                p_employee_id.ParameterName = "p_employee_id";
                p_employee_id.OracleDbType = OracleDbType.Varchar2;
                p_employee_id.Size = 1000;
                p_employee_id.Direction = ParameterDirection.Input;
                p_employee_id.Value = command.p_employee_id.ToSafeString();

                var p_location_name = new OracleParameter();
                p_location_name.ParameterName = "p_location_name";
                p_location_name.OracleDbType = OracleDbType.Varchar2;
                p_location_name.Size = 4000;
                p_location_name.Direction = ParameterDirection.Input;
                p_location_name.Value = command.p_location_name.ToSafeString();

                var p_sub_region = new OracleParameter();
                p_sub_region.ParameterName = "p_sub_region";
                p_sub_region.OracleDbType = OracleDbType.Varchar2;
                p_sub_region.Size = 1000;
                p_sub_region.Direction = ParameterDirection.Input;
                p_sub_region.Value = command.p_sub_region.ToSafeString();

                var p_region_name = new OracleParameter();
                p_region_name.ParameterName = "p_region_name";
                p_region_name.OracleDbType = OracleDbType.Varchar2;
                p_region_name.Size = 1000;
                p_region_name.Direction = ParameterDirection.Input;
                p_region_name.Value = command.p_region_name.ToSafeString();

                var p_asc_name = new OracleParameter();
                p_asc_name.ParameterName = "p_asc_name";
                p_asc_name.OracleDbType = OracleDbType.Varchar2;
                p_asc_name.Size = 4000;
                p_asc_name.Direction = ParameterDirection.Input;
                p_asc_name.Value = command.p_asc_name.ToSafeString();

                var p_sale_name = new OracleParameter();
                p_sale_name.ParameterName = "p_sale_name";
                p_sale_name.OracleDbType = OracleDbType.Varchar2;
                p_sale_name.Size = 4000;
                p_sale_name.Direction = ParameterDirection.Input;
                p_sale_name.Value = command.p_sale_name.ToSafeString();

                var p_channel_name = new OracleParameter();
                p_channel_name.ParameterName = "p_channel_name";
                p_channel_name.OracleDbType = OracleDbType.Varchar2;
                p_channel_name.Size = 4000;
                p_channel_name.Direction = ParameterDirection.Input;
                p_channel_name.Value = command.p_channel_name.ToSafeString();

                var p_sale_channel = new OracleParameter();
                p_sale_channel.ParameterName = "p_sale_channel";
                p_sale_channel.OracleDbType = OracleDbType.Varchar2;
                p_sale_channel.Size = 4000;
                p_sale_channel.Direction = ParameterDirection.Input;
                p_sale_channel.Value = command.p_sale_channel.ToSafeString();

                var P_ADDRESS_TYPE_DTL = new OracleParameter();
                P_ADDRESS_TYPE_DTL.ParameterName = "P_ADDRESS_TYPE_DTL";
                P_ADDRESS_TYPE_DTL.OracleDbType = OracleDbType.Varchar2;
                P_ADDRESS_TYPE_DTL.Size = 2000;
                P_ADDRESS_TYPE_DTL.Direction = ParameterDirection.Input;
                P_ADDRESS_TYPE_DTL.Value = command.P_ADDRESS_TYPE_DTL.ToSafeString();

                var P_REMARK = new OracleParameter();
                P_REMARK.ParameterName = "P_REMARK";
                P_REMARK.OracleDbType = OracleDbType.Varchar2;
                P_REMARK.Size = 5000;
                P_REMARK.Direction = ParameterDirection.Input;
                P_REMARK.Value = command.P_REMARK.ToSafeString();

                var P_TECHNOLOGY = new OracleParameter();
                P_TECHNOLOGY.ParameterName = "P_TECHNOLOGY";
                P_TECHNOLOGY.OracleDbType = OracleDbType.Varchar2;
                P_TECHNOLOGY.Size = 1000;
                P_TECHNOLOGY.Direction = ParameterDirection.Input;
                P_TECHNOLOGY.Value = command.P_TECHNOLOGY.ToSafeString();

                var P_PROJECTNAME = new OracleParameter();
                P_PROJECTNAME.ParameterName = "P_PROJECTNAME";
                P_PROJECTNAME.OracleDbType = OracleDbType.Varchar2;
                P_PROJECTNAME.Size = 1000;
                P_PROJECTNAME.Direction = ParameterDirection.Input;
                P_PROJECTNAME.Value = command.P_PROJECTNAME.ToSafeString();

                //R24.01 Add coverage
                var P_CoverageArea = new OracleParameter();
                P_CoverageArea.ParameterName = "P_CoverageArea";
                P_CoverageArea.OracleDbType = OracleDbType.Varchar2;
                P_CoverageArea.Size = 1000;
                P_CoverageArea.Direction = ParameterDirection.Input;
                P_CoverageArea.Value = command.P_CoverageArea.ToSafeString();

                var P_NetworkProvider = new OracleParameter();
                P_NetworkProvider.ParameterName = "P_NetworkProvider";
                P_NetworkProvider.OracleDbType = OracleDbType.Varchar2;
                P_NetworkProvider.Size = 1000;
                P_NetworkProvider.Direction = ParameterDirection.Input;
                P_NetworkProvider.Value = command.P_NetworkProvider.ToSafeString();

                var P_Region = new OracleParameter();
                P_Region.ParameterName = "P_Region";
                P_Region.OracleDbType = OracleDbType.Varchar2;
                P_Region.Size = 1000;
                P_Region.Direction = ParameterDirection.Input;
                P_Region.Value = command.P_Region.ToSafeString();

                var P_CoverageStatus = new OracleParameter();
                P_CoverageStatus.ParameterName = "P_CoverageStatus";
                P_CoverageStatus.OracleDbType = OracleDbType.Varchar2;
                P_CoverageStatus.Size = 1000;
                P_CoverageStatus.Direction = ParameterDirection.Input;
                P_CoverageStatus.Value = command.P_CoverageStatus.ToSafeString();

                var P_CoverageSubstatus = new OracleParameter();
                P_CoverageSubstatus.ParameterName = "P_CoverageSubstatus";
                P_CoverageSubstatus.OracleDbType = OracleDbType.Varchar2;
                P_CoverageSubstatus.Size = 1000;
                P_CoverageSubstatus.Direction = ParameterDirection.Input;
                P_CoverageSubstatus.Value = command.P_CoverageSubstatus.ToSafeString();

                var P_CoverageGroupOwner = new OracleParameter();
                P_CoverageGroupOwner.ParameterName = "P_CoverageGroupOwner";
                P_CoverageGroupOwner.OracleDbType = OracleDbType.Varchar2;
                P_CoverageGroupOwner.Size = 1000;
                P_CoverageGroupOwner.Direction = ParameterDirection.Input;
                P_CoverageGroupOwner.Value = command.P_CoverageGroupOwner.ToSafeString();

                var P_CoverageContactName = new OracleParameter();
                P_CoverageContactName.ParameterName = "P_CoverageContactName";
                P_CoverageContactName.OracleDbType = OracleDbType.Varchar2;
                P_CoverageContactName.Size = 1000;
                P_CoverageContactName.Direction = ParameterDirection.Input;
                P_CoverageContactName.Value = command.P_CoverageContactName.ToSafeString();

                var P_CoverageContactEmail = new OracleParameter();
                P_CoverageContactEmail.ParameterName = "P_CoverageContactEmail";
                P_CoverageContactEmail.OracleDbType = OracleDbType.Varchar2;
                P_CoverageContactEmail.Size = 1000;
                P_CoverageContactEmail.Direction = ParameterDirection.Input;
                P_CoverageContactEmail.Value = command.P_CoverageContactEmail.ToSafeString();

                var P_CoverageContactTel = new OracleParameter();
                P_CoverageContactTel.ParameterName = "P_CoverageContactTel";
                P_CoverageContactTel.OracleDbType = OracleDbType.Varchar2;
                P_CoverageContactTel.Size = 1000;
                P_CoverageContactTel.Direction = ParameterDirection.Input;
                P_CoverageContactTel.Value = command.P_CoverageContactTel.ToSafeString();

                //Return
                var ret_code = new OracleParameter();
                ret_code.ParameterName = "ret_code";
                ret_code.OracleDbType = OracleDbType.Varchar2;
                ret_code.Size = 2000;
                ret_code.Direction = ParameterDirection.Output;

                var ret_message = new OracleParameter();
                ret_message.ParameterName = "ret_message";
                ret_message.OracleDbType = OracleDbType.Varchar2;
                ret_message.Size = 2000;
                ret_message.Direction = ParameterDirection.Output;

                var executeResult = _objService.ExecuteStoredProc("WBB.PKG_FBBOR004.PROC_INS_COVERAGE",
                    new
                    {
                        //Input
                        p_channel,
                        p_address_type,
                        p_postal_code,
                        p_district,
                        p_sub_district,
                        p_language,
                        p_building_name,
                        p_building_no,
                        p_phone_flag,
                        p_floor_no,
                        p_address_no,
                        p_moo,
                        p_soi,
                        p_road,
                        p_latitude,
                        p_longitude,
                        p_unit_no,
                        p_coverage_flag,
                        p_address_id,
                        p_is_partner,
                        p_partner_name,
                        p_firstname,
                        p_lastname,
                        p_contactnumber,
                        p_producttype,
                        p_owner_product,
                        p_splitter_name,
                        p_distance,
                        p_contact_email,
                        p_contact_line_id,
                        p_location_code,
                        p_asc_code,
                        p_employee_id,
                        p_location_name,
                        p_sub_region,
                        p_region_name,
                        p_asc_name,
                        p_sale_name,
                        p_channel_name,
                        p_sale_channel,
                        P_ADDRESS_TYPE_DTL,
                        P_REMARK,
                        P_TECHNOLOGY,
                        P_PROJECTNAME,
                        //R24.01 Add coverage
                        P_CoverageArea,
                        P_NetworkProvider,
                        P_Region,
                        P_CoverageStatus,
                        P_CoverageSubstatus,
                        P_CoverageGroupOwner,
                        P_CoverageContactName,
                        P_CoverageContactEmail,
                        P_CoverageContactTel,

                        //Return
                        ret_code,
                        ret_message

                    });

                command.ret_code = ret_code.Value.ToSafeString();
                command.ret_message = ret_message.Value.ToSafeString();

                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, command.ret_code, log, "Success", "", "");

            }
            catch (Exception ex)
            {
                _logger.Info(ex.Message);

                if (null != log)
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex, log, "Failed", ex.GetErrorMessage(), "");
                }

                command.ret_code = "-1";
                command.ret_message = "Error Service InsertCoverageRusultCommandHandler" + ex.Message;
            }
        }
    }
}
