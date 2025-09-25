using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Commands.ExWebServices.FbbCpGw;
using WBBData.Repository;
using WBBEntity.Extensions;

namespace WBBBusinessLayer.CommandHandlers.ExWebServices.FbbCpGw
{
    public class CustRegisterCommandHandler : ICommandHandler<CustRegisterCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<string> _objService;

        public CustRegisterCommandHandler(ILogger logger,
            IEntityRepository<string> objService)
        {
            _logger = logger;
            _objService = objService;
        }

        public void Handle(CustRegisterCommand command)
        {
            try
            {
                var custInfoModel = command.CustRegisterInfoModel;

                if (null == custInfoModel)
                {
                    throw new System.Exception("CustRegisterModel Is Null");
                }
                var isThai = custInfoModel.Language.ToCultureCode().IsThaiCulture();

                //register
                var packgaemodel = new PackageObjectModel();
                packgaemodel.REC_REG_PACKAGE = custInfoModel.SelectPackage.Select(c => new Rec_Reg_PackageOracleTypeMapping
                {
                    package_code = c.PACKAGE_CODE.ToSafeString(),
                    package_class = c.PACKAGE_CLASS.ToSafeString(),
                    package_group = c.PACKAGE_GROUP.ToSafeString(),
                    product_subtype = c.PRODUCT_SUBTYPE.ToSafeString(),
                    technology = c.TECHNOLOGY.ToSafeString(),
                    package_name = c.PACKAGE_NAME.ToSafeString(),
                    recurring_charge = c.RECURRING_CHARGE.GetValueOrDefault(),
                    initiation_charge = c.INITIATION_CHARGE.GetValueOrDefault(),
                    discount_initiation = c.DISCOUNT_INITIATION_CHARGE.GetValueOrDefault(),
                    package_bill_tha = c.SFF_PROMOTION_BILL_THA.ToSafeString(),
                    package_bill_eng = c.SFF_PROMOTION_BILL_ENG.ToSafeString(),
                    download_speed = c.DOWNLOAD_SPEED.ToSafeString(),
                    upload_speed = c.UPLOAD_SPEED.ToSafeString(),
                    owner_product = c.OWNER_PRODUCT.ToSafeString(),
                    voip_ip = c.VOIP_IP.ToSafeString(),
                    idd_flag = c.IDD_FLAG.ToSafeString(),
                    fax_flag = c.FAX_FLAG.ToSafeString(),
                    mobile_forward = c.MOBILE_FORWARD.ToSafeString()
                }).ToArray();
                var register = OracleCustomTypeUtilities.CreateUDTArrayParameter("p_REC_REG_PACKAGE", "FBB_REG_PACKAGE_ARRAY", packgaemodel);

                //listfilename // initial fixed null value
                var picObjModel = new PictureObjectModel();
                picObjModel.REC_REG_PACKAGE = new List<PicturePackageOracleTypeMapping>(){
                    new PicturePackageOracleTypeMapping{
                        file_name = "DUMMY_FILE.jpg",
                    },
                }.ToArray();

                var listfilename = OracleCustomTypeUtilities.CreateUDTArrayParameter("p_REC_UPLOAD_FILE", "FBB_REG_UPLOAD_FILE_ARRAY", picObjModel);

                //splitter
                var splitterModel = new SplitterObjectModel();
                if (command.CoveragePanelModel.splitter != null)
                {
                    splitterModel.REC_CUST_SPLITTER = command.CoveragePanelModel.splitter.Select(p => new Rec_Cust_SplitterOracleTypeMapping
                    {
                        splitter_name = p.Splitter_Name.ToSafeString(),
                        distance = p.Distance,
                        distance_type = p.Distance_Type.ToSafeString()
                    }).ToArray();
                }
                else
                {
                    List<Rec_Cust_SplitterOracleTypeMapping> ltmp = new List<Rec_Cust_SplitterOracleTypeMapping>()
                    {
                        //new Rec_Cust_SplitterOracleTypeMapping() { splitter_name = "", distance = 0, distance_type = ""}
                    };
                    splitterModel.REC_CUST_SPLITTER = ltmp.ToArray();
                }
                var splitter = OracleCustomTypeUtilities.CreateUDTArrayParameter("p_REC_SPLITTER_LIST", "FBB_REG_SPLITTER_LIST_ARRAY", splitterModel);

                //listcpe
                var cpeModel = new CPEListObjectModel();
                cpeModel.CPE_LIST_PACKAGE = new List<CPE_List_PackageOracleTypeMapping>(){
                    new CPE_List_PackageOracleTypeMapping{ },
                }.ToArray();
                var listcpe = OracleCustomTypeUtilities.CreateUDTArrayParameter("p_REC_CPE_LIST", "FBB_REG_CPE_LIST_ARRAY", cpeModel);

                var ret_code = new OracleParameter();
                ret_code.OracleDbType = OracleDbType.Decimal;
                ret_code.Direction = ParameterDirection.Output;

                var v_error_msg = new OracleParameter();
                v_error_msg.OracleDbType = OracleDbType.Varchar2;
                v_error_msg.Size = 2000;
                v_error_msg.Direction = ParameterDirection.Output;

                var v_cust_id = new OracleParameter();
                v_cust_id.OracleDbType = OracleDbType.Varchar2;
                v_cust_id.Size = 2000;
                v_cust_id.Direction = ParameterDirection.Output;

                var outp = new List<object>();
                var paramOut = outp.ToArray();

                var executeResult = _objService.ExecuteStoredProc("WBB.PKG_FBBOR004.FBBOR004_TR",
                    out paramOut,
                       new
                       {
                           #region T.FBB_REGISTER

                           p_cust_name = string.Format("{0} {1}", custInfoModel.CustFirstName, custInfoModel.CustLastName).ToSafeString(),
                           p_cust_id_card_type = isThai ? "บัตรประชาชน" : "ID_CARD",
                           p_cust_id_card_num = custInfoModel.IDCardNo.ToSafeString(),
                           p_cust_category = "R",
                           p_cust_sub_category = "T",
                           p_cust_gender = isThai ? "หญิง" : "Female",
                           p_cust_birth_dt = custInfoModel.CustBirthDate.ToDate(),
                           p_cust_nationality = "THAI",
                           p_cust_title = "คุณ",
                           p_contact_first_name = custInfoModel.CustFirstName.ToSafeString(),
                           p_contact_last_name = custInfoModel.CustLastName.ToSafeString(),
                           p_contact_home_phone = custInfoModel.ContactHomeNo.ToSafeString(),
                           p_contact_mobile_phone1 = custInfoModel.ContactHomeNo.ToSafeString(),
                           p_contact_mobile_phone2 = "",
                           p_contact_email = custInfoModel.ContactEmail.ToSafeString(),
                           p_contact_time = custInfoModel.PreferInstallTime.ToSafeString(),
                           p_sale_rep = custInfoModel.SaleRep.ToSafeString(),
                           p_asc_code = custInfoModel.ASCCode.ToSafeString(),
                           p_employee_id = custInfoModel.StaffID.ToSafeString(),
                           p_location_code = custInfoModel.LocationCode.ToSafeString(),
                           p_cs_note = "",

                           p_condo_type = "",
                           p_condo_direction = "",
                           p_condo_limit = "",
                           p_condo_area = "",
                           p_home_type = "",
                           p_home_area = "",

                           p_document_type = custInfoModel.DocType.ToSafeString(),
                           p_remark = command.TransactionID,
                           p_cvr_id = "",//custInfoModel.CvrId,
                           p_cvr_node = "",//custInfoModel.InstallBuilding.ToSafeString(),
                           p_cvr_tower = "",//custInfoModel.InstallTower.ToSafeString(),

                           p_return_code = custInfoModel.AirReturnCode.ToSafeString(),
                           p_return_message = custInfoModel.AirReturnMessage.ToSafeString(),
                           p_return_order = custInfoModel.AirReturnOrder.ToSafeString(),

                           //R15.3
                           p_Phone_Flag = command.PhoneFlag.ToSafeString(),
                           p_Time_Slot = command.TimeSlot.ToSafeString(),
                           p_Installation_Capacity = command.InstallCapacity.ToSafeString(),
                           p_Address_Id = command.CoveragePanelModel.Address.AddressId.ToSafeString(),
                           p_access_mode = command.CoveragePanelModel.AccessMode.ToSafeString(),
                           p_service_code = command.CoveragePanelModel.ServiceCode.ToSafeString(),

                           p_event_code = "", // initial fixed null value

                           #endregion

                           #region T.FBB_ADDRESS
                           // PARAMETER FOR T.FBB_ADDRESS
                           p_lang = custInfoModel.Language.ToSafeString(),
                           #endregion

                           #region install address
                           // Install address
                           p_install_house_no = custInfoModel.InstallHouseNo.ToSafeString(),
                           p_install_soi = custInfoModel.InstallSoi.ToSafeString(),
                           p_install_moo = custInfoModel.InstallMoo.ToSafeString(),
                           p_install_mooban = custInfoModel.InstallVillage.ToSafeString(),
                           p_install_building_name = (custInfoModel.InstallBuilding.ToSafeString() + custInfoModel.InstallTower.ToSafeString()).ToSafeString(),
                           p_install_floor = custInfoModel.InstallFloor.ToSafeString(),
                           p_install_room = "",
                           p_install_street_name = custInfoModel.InstallRoad.ToSafeString(),
                           p_install_zipcode_id = custInfoModel.InstallZipCodeId.ToSafeString(),
                           #endregion

                           #region Billing address
                           // Billing address
                           p_bill_house_no = custInfoModel.BillHouseNo.ToSafeString(),
                           p_bill_soi = custInfoModel.BillSoi.ToSafeString(),
                           p_bill_moo = custInfoModel.BillMoo.ToSafeString(),
                           p_bill_mooban = custInfoModel.BillVillage.ToSafeString(),
                           p_bill_building_name = (custInfoModel.BillBuilding.ToSafeString() + custInfoModel.BillTower.ToSafeString()).ToSafeString(),
                           p_bill_floor = custInfoModel.BillFloor.ToSafeString(),
                           p_bill_room = "",
                           p_bill_street_name = custInfoModel.BillRoad,
                           p_bill_zipcode_id = custInfoModel.BillZipCodeId,
                           p_bill_ckecked = "",
                           #endregion

                           #region vat address

                           // Vat address
                           p_vat_house_no = "",
                           p_vat_soi = "",
                           p_vat_moo = "",
                           p_vat_mooban = "",
                           p_vat_building_name = "",
                           p_vat_floor = "",
                           p_vat_room = "",
                           p_vat_street_name = "",
                           p_vat_zipcode_id = "",
                           p_vat_ckecked = "",

                           p_result_id = command.OrderRefId, //custInfoModel.CoverageResultId.ToSafeDecimal(),

                           #endregion

                           #region for new vas

                           p_ca_id = custInfoModel.SffCaNo.ToSafeString(),
                           p_sa_id = custInfoModel.SffSaNo.ToSafeString(),
                           p_ba_id = custInfoModel.SffBaNo.ToSafeString(),

                           p_ais_mobile = custInfoModel.IsNonMobile ? "" : custInfoModel.MobileNo.ToSafeString(),
                           p_ais_non_mobile = custInfoModel.IsNonMobile ? custInfoModel.MobileNo.ToSafeString() : "",
                           p_network_type = custInfoModel.NetworkType.ToSafeString(),
                           p_service_year = custInfoModel.ServiceYear.ToSafeString(),
                           p_request_install_date = custInfoModel.PreferInstallDate.ToDate(),
                           p_register_type = "MOBILE_REGISTER",
                           p_install_address_1 = "",
                           p_install_address_2 = "",
                           p_install_address_3 = "",
                           p_install_address_4 = "",
                           p_install_address_5 = "",
                           p_number_of_pb = "",
                           p_convergence_flag = "",
                           p_single_bill_flag = "",
                           p_time_slot_id = command.TimeSlotID.ToSafeString(),
                           p_guid = command.Guid.ToSafeString(),
                           p_voucher_pin = "", // initial fixed null value               
                           p_sub_location_id = "", // initial fixed null value
                           p_sub_contract_name = "", // initial fixed null value           
                           p_install_staff_id = "", // initial fixed null value
                           p_install_staff_name = "", // initial fixed null value
                           p_site_code = "",
                           p_flow_flag = "",
                           p_vat_address_1 = "",
                           p_vat_address_2 = "",
                           p_vat_address_3 = "",
                           p_vat_address_4 = "",
                           p_vat_address_5 = "",
                           p_vat_postcode = "",
                           p_address_flag = "",
                           p_relate_project_name = "",
                           p_register_device = "",
                           p_browser_type = "",
                           p_reserved_id = "",
                           p_job_order_type = "",
                           p_assign_job = "",
                           p_old_isp = "",
                           p_client_ip = "",
                           p_splitter_flag = "",
                           p_reserved_port_id = "",
                           p_special_remark = "",
                           p_source_system = "",
                           p_bill_media = "",
                           p_pre_order_no = "",
                           p_voucher_desc = "",
                           p_campaign_project_name = "",
                           p_pre_order_chanel = "",
                           p_rental_flag = "",
                           p_plug_and_play_flag = "",
                           p_dev_project_code = "",
                           p_dev_bill_to = "",
                           p_dev_po_no = "",
                           p_tmp_location_code = "",
                           p_tmp_asc_code = "",
                           p_partner_type = "",
                           p_partner_subtype = "",
                           p_mobile_by_asc = "",
                           p_location_name = "",
                           P_PAYMENTMETHOD = "",
                           P_TRANSACTIONID_IN = "",
                           P_TRANSACTIONID = "",
                           p_sub_access_mode = "",
                           p_request_sub_flag = "",
                           p_premium_flag = "",
                           p_relate_mobile_segment = "",
                           p_ref_ur_no = "",
                           p_location_email_by_region = "",
                           p_sale_staff_name = "",
                           p_dopa_flag = "",
                           p_request_cs_verify_doc = "",
                           p_facereccog_flag = "",
                           p_special_account_name = "",
                           p_special_account_no = "",
                           p_special_account_enddate = "",
                           p_special_account_group_email = "",
                           p_special_account_flag = "",
                           p_existing_mobile_flag = "",
                           p_pre_survey_date = "",
                           p_pre_survey_timeslot = "",
                           p_replace_onu = "",
                           p_replace_wifi = "",
                           p_number_of_mesh = "",
                           p_company_name = "",
                           p_distribution_channel = "",
                           p_channel_sales_group = "",
                           p_shop_type = "",
                           p_shop_segment = "",
                           p_asc_name = "",
                           p_asc_member_category = "",
                           p_asc_position = "",
                           p_location_region = "",
                           p_location_sub_region = "",
                           p_employee_name = "",
                           p_customerpurge = "",
                           p_exceptentryfee = "",
                           p_secondinstallation = "",
                           p_amendment_flag = "",
                           p_service_level = "",
                           p_first_install_date = "",
                           p_first_time_slot = "",
                           p_line_temp_id = "",
                           p_non_res_flag = "",
                           p_fmc_special_flag = "",
                           p_criteria_mobile = "",
                           p_remark_for_subcontract = "",
                           p_online_flag = "",
                           p_transaction_staff = "",
                           P_TDM_CONTRACT_ID = "",
                           P_TDM_DURATION = "",
                           P_TDM_CONTRACT_Flag = "",
                           P_TDM_PENALTY_GROUP_ID = "",
                           P_TDM_PENALTY_ID = "",
                           P_TDM_RULE_ID = "",
                           P_SPECIAL_SKILL = "",

                           #endregion

                           p_REC_REG_PACKAGE = register,
                           p_REC_UPLOAD_FILE = listfilename,
                           p_REC_SPLITTER_LIST = splitter,
                           p_REC_CPE_LIST = listcpe,

                           // Return Code
                           ret_code = ret_code,
                           v_error_msg = v_error_msg,
                           v_cust_id = v_cust_id
                       });

                command.CustRegisterInfoModel.RegisterResult.CustomerRowId = ((Oracle.ManagedDataAccess.Client.OracleParameter)(paramOut[paramOut.Count() - 1])).Value.ToSafeString();

            }
            catch (Exception ex)
            {
                _logger.Info(ex.GetErrorMessage());
            }
        }
    }
}
