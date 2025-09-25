using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBBusinessLayer.QueryHandlers;
using WBBContract;
using WBBContract.Commands;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;

namespace WBBBusinessLayer.CommandHandlers
{
    public class ProfileCustomerCommandHandler : ICommandHandler<ProfileCustomerCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<string> _objService;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;

        public ProfileCustomerCommandHandler(ILogger logger,
            IEntityRepository<string> objService,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IWBBUnitOfWork uow)
        {
            _logger = logger;
            _objService = objService;
            _intfLog = intfLog;
            _uow = uow;
        }

        public void Handle(ProfileCustomerCommand command)
        {
            InterfaceLogCommand log = null;
            try
            {
                //log = GetPackageListHelper.StartInterfaceAirWfLog(_uow, _intfLog, command,
                //  "", "SaveProfileCustomer", "ProfileCustomerCommandHandler");
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, command, "", "SaveProfileCustomer", "ProfileCustomerCommandHandler", null, "FBB", "");

                #region if want to write log open this comment

                //string logPackage = "";

                //foreach (var item in command.Rec_Cust_Package)
                //{
                //    logPackage += ", Package_Code : " + item.Package_Code;
                //    logPackage += ", Package_Class : " + item.Package_Class;
                //    logPackage += ", Package_Type : " + item.Package_Type;
                //    logPackage += ", Package_Group : " + item.Package_Group;
                //    logPackage += ", Package_Subtype : " + item.Package_Subtype;
                //    logPackage += ", Package_Owner : " + item.Package_Owner;
                //    logPackage += ", Technology : " + item.Technology;
                //    logPackage += ", package_Status : " + item.package_Status;
                //    logPackage += ", package_Name : " + item.package_Name;
                //    logPackage += ", Recurring_Charge : " + item.Recurring_Charge;
                //    logPackage += ", Pre_Recurring_Charge : " + item.Pre_Recurring_Charge;
                //    logPackage += ", Recurring_Discount_Exp : " + item.Recurring_Discount_Exp;
                //    logPackage += ", Recurring_Start_Dt : " + item.Recurring_Start_Dt;
                //    logPackage += ", Recurring_End_Dt : " + item.Recurring_End_Dt;
                //    logPackage += ", Initiation_Charge : " + item.Initiation_Charge;
                //    logPackage += ", Pre_Initiation_Charge : " + item.Pre_Initiation_Charge;
                //    logPackage += ", Package_Bill_Tha : " + item.Package_Bill_Tha;
                //    logPackage += ", Download_Speed : " + item.Download_Speed;
                //    logPackage += ", Upload_Speed : " + item.Upload_Speed;
                //    logPackage += ", Home_Ip : " + item.Home_Ip;
                //    logPackage += ", Home_Port : " + item.Home_Port;
                //    logPackage += ", Idd_Flag : " + item.Idd_Flag;
                //    logPackage += ", Fax_Flag : " + item.Fax_Flag;
                //    logPackage += ", Cust_Non_Mobile : " + item.Cust_Non_Mobile;
                //    logPackage += ", Mobile_Forward : " + item.Mobile_Forward;
                //}

                //_logger.Info(logPackage);

                #endregion if want to write log open this comment

                var profileModel = new ProfileObjectModel();
                var addrModel = new AddressObjectModel();
                var contactModel = new ContactObjectModel();
                //var assetModel = new AssetObjectModel();
                var packageModel = new PackageObjectModel();
                var splitterModel = new SplitterObjectModel();

                addrModel.REC_CUST_ADDRESS = command.Rec_Cust_Address.Select(a => new Rec_Cust_AddressOracleTypeMapping
                {
                    addr_type = a.Addr_Type.ToSafeString(),
                    address_IN_seq = a.Address_In_Seq.ToSafeString(),
                    address_vat = a.Address_Vat.ToSafeString(),
                    building_name = a.Building_Name.ToSafeString(),
                    floor = a.Floor.ToSafeString(),
                    house_no = a.House_No.ToSafeString(),
                    moo = a.Moo.ToSafeString(),
                    mooban = a.Mooban.ToSafeString(),
                    room = a.Room.ToSafeString(),
                    soi = a.Soi.ToSafeString(),
                    street_name = a.Street_Name.ToSafeString(),
                    zipcode_id = a.Zipcode_Id.ToSafeString(),
                    ca_id = a.ca_id.ToSafeString(),
                    ba_id = a.ba_id.ToSafeString()
                }).ToArray();

                profileModel.REC_CUST_PROFILE = command.Rec_Cust_Profile.Select(p => new Rec_Cust_ProfileOracleTypeMapping
                {
                    cust_non_mobile = p.Cust_Non_Mobile.ToSafeString(),
                    ca_id = p.Ca_Id.ToSafeString(),
                    sa_id = p.Sa_Id.ToSafeString(),
                    ba_id = p.Ba_Id.ToSafeString(),
                    ia_id = p.Ia_Id.ToSafeString(),
                    cust_name = p.Cust_Name.ToSafeString(),
                    cust_surname = p.Cust_Surname.ToSafeString(),
                    cust_id_card_type = p.Cust_Id_Card_Type.ToSafeString(),
                    cust_id_card_num = p.Cust_Id_Card_Num.ToSafeString(),
                    cust_category = p.Cust_Category.ToSafeString(),
                    cust_sub_category = p.Cust_Sub_Category.ToSafeString(),
                    cust_gender = p.Cust_Gender.ToSafeString(),
                    cust_birthday = p.Cust_Birthday.ToSafeString(),
                    cust_nationality = p.Cust_Nationality.ToSafeString(),
                    cust_title = p.Cust_Title.ToSafeString(),
                    online_number = p.Online_Number.ToSafeString(),
                    condo_type = p.Condo_Type.ToSafeString(),
                    condo_direction = p.Condo_Direction.ToSafeString(),
                    condo_limit = p.Condo_Limit.ToSafeString(),
                    condo_area = p.Condo_Area.ToSafeString(),
                    home_type = p.Home_Type.ToSafeString(),
                    home_area = p.Home_Area.ToSafeString(),
                    document_type = p.Document_Type.ToSafeString(),
                    cvr_id = p.Cvr_Id.ToSafeString(),
                    port_id = p.Port_Id.ToSafeString(),
                    order_no = p.Order_No.ToSafeString(),
                    remark = p.Remark.ToSafeString(),
                    port_active_date = p.Port_Active_Date.ToSafeString(),
                    installation_date = p.Installation_Date.ToSafeString(),
                    relate_mobile = p.Relate_Mobile.ToSafeString(),
                    relate_non_mobile = p.Relate_Non_Mobile.ToSafeString(),
                    network_type = p.Network_Type.ToSafeString(),
                    service_day = p.Service_Day.ToSafeInteger(),
                    phone_flag = p.Phone_Flag.ToSafeString(),
                    time_slot = p.Time_Slot.ToSafeString(),
                    access_mode = p.Access_Mode.ToSafeString(),
                    address_id = p.Address_Id.ToSafeString(),
                    service_code = p.Service_Code.ToSafeString(),
                    voucher_pin = p.gift_voucher.ToSafeString(),
                    event_code = p.event_code.ToSafeString(),
                    sub_location_id = p.sub_location_id.ToSafeString(),
                    sub_contract_name = p.sub_contract_name.ToSafeString(),
                    install_staff_id = p.install_staff_id.ToSafeString(),
                    install_staff_name = p.install_staff_name.ToSafeString(),
                    site_code = p.site_code.ToSafeString(),
                    flow_flag = p.flow_flag.ToSafeString()
                }).ToArray();

                contactModel.REC_CUST_CONTACT = command.Rec_Cust_Contact.Select(c => new Rec_Cust_ContactOracleTypeMapping
                {
                    address_IN_seq = c.Address_In_Seq.ToSafeString(),
                    contact_email = c.Contact_Email.ToSafeString(),
                    contact_first_name = c.Contact_First_Name.ToSafeString(),
                    contact_home_phone = c.Contact_Home_Phone.ToSafeString(),
                    contact_last_name = c.Contact_Last_Name.ToSafeString(),
                    contact_mobile_phone1 = c.Contact_Mobile_Phone1.ToSafeString(),
                    contact_mobile_phone2 = c.Contact_Mobile_Phone2.ToSafeString(),
                    ca_id = c.ca_id.ToSafeString(),
                    ba_id = c.ba_id.ToSafeString()
                }).ToArray();

                //assetModel.REC_CUST_ASSET = command.Rec_Cust_Asset.Select(a => new Rec_Cust_AssetOracleTypeMapping
                //    {
                //        asset_charge = a.Asset_Charge.ToSafeString(),
                //        asset_code = a.Asset_Code.ToSafeString(),
                //        asset_discount = a.Asset_Discount.ToSafeString(),
                //        asset_end_dt = a.Asset_End_Dt.ToSafeString(),
                //        asset_model = a.Asset_Model.ToSafeString(),
                //        asset_name = a.Asset_Name.ToSafeString(),
                //        asset_start_dt = a.Asset_Start_Dt.ToSafeString(),
                //        asset_status = a.Asset_Status.ToSafeString(),
                //        equipment_type = a.Equipment_Type.ToSafeString(),
                //        package_code = a.Package_Code.ToSafeString(),
                //        serial_number = a.Serial_Number.ToSafeString()
                //    }).ToArray();

                packageModel.REC_CUST_PACKAGE = command.Rec_Cust_Package.Select(p => new Rec_Cust_PackageOracleTypeMapping
                {
                    download_speed = p.Download_Speed.ToSafeString(),
                    initiation_charge = p.Initiation_Charge.ToSafeString(),
                    pre_recurring_charge = p.Pre_Recurring_Charge.ToSafeString(),
                    package_bill_tha = p.Package_Bill_Tha.ToSafeString(),
                    package_class = p.Package_Class.ToSafeString(),
                    package_code = p.Package_Code.ToSafeString(),
                    package_group = p.Package_Group.ToSafeString(),
                    package_name = p.package_Name.ToSafeString(),
                    package_owner = p.Package_Owner.ToSafeString(),
                    package_status = p.package_Status.ToSafeString(),
                    package_subtype = p.Package_Subtype.ToSafeString(),
                    package_type = p.Package_Type.ToSafeString(),
                    recurring_charge = p.Recurring_Charge.ToSafeString(),
                    pre_initiation_charge = p.Pre_Initiation_Charge.ToSafeString(),
                    recurring_discount_exp = p.Recurring_Discount_Exp.ToSafeString(),
                    recurring_end_dt = p.Recurring_End_Dt.ToSafeString(),
                    recurring_start_dt = p.Recurring_Start_Dt.ToSafeString(),
                    technology = p.Technology.ToSafeString(),
                    upload_speed = p.Upload_Speed.ToSafeString(),
                    home_ip = p.Home_Ip.ToSafeString(),
                    home_port = p.Home_Port.ToSafeString(),
                    idd_flag = p.Idd_Flag.ToSafeString(),
                    fax_flag = p.Fax_Flag.ToSafeString(),
                    cust_non_mobile = p.Cust_Non_Mobile.ToSafeString(),
                    mobile_forward = p.Mobile_Forward.ToSafeString()
                }).ToArray();

                splitterModel.REC_CUST_SPLITTER = command.Rec_Cust_Splitter.Select(p => new Rec_Cust_SplitterOracleTypeMapping
                {
                    cust_non_mobile = p.Cust_Non_Mobile.ToSafeString(),
                    splitter_name = p.Splitter_Name.ToSafeString(),
                    distance = p.Distance.ToSafeDecimal(),
                    distance_type = p.Distance_Type.ToSafeString()
                }).ToArray();

                var ret_code = new OracleParameter();
                ret_code.OracleDbType = OracleDbType.Decimal;
                ret_code.Direction = ParameterDirection.Output;

                var v_error_msg = new OracleParameter();
                v_error_msg.OracleDbType = OracleDbType.Varchar2;
                v_error_msg.Size = 2000;
                v_error_msg.Direction = ParameterDirection.Output;

                var outp = new List<object>();
                var paramOut = outp.ToArray();

                var profile = OracleCustomTypeUtilities.CreateUDTArrayParameter("p_REC_CUST_PROFILE", "FBB_CUST_PROFILE_ARRAY", profileModel);
                var contact = OracleCustomTypeUtilities.CreateUDTArrayParameter("p_REC_CUST_CONTACT", "FBB_CUST_CONTACT_ARRAY", contactModel);
                var address = OracleCustomTypeUtilities.CreateUDTArrayParameter("p_REC_CUST_ADDRESS", "FBB_CUST_ADDRESS_ARRAY", addrModel);
                //var asset = OracleCustomTypeUtilities.CreateUDTArrayParameter("p_REC_CUST_ASSET", "FBB_CUST_ASSET_ARRAY", assetModel);
                var package = OracleCustomTypeUtilities.CreateUDTArrayParameter("p_REC_CUST_PACKAGE", "FBB_CUST_PACKAGE_ARRAY", packageModel);
                var splitter = OracleCustomTypeUtilities.CreateUDTArrayParameter("p_REC_CUST_SPLITTER", "FBB_CUST_SPLITTER_ARRAY", splitterModel);

                var executeResult = _objService.ExecuteStoredProc("WBB.PKG_FBB_INS_PROFILE.PROC_MAIN",

                out paramOut,
                   new
                   {
                       p_REC_CUST_PROFILE = profile,
                       p_REC_CUST_CONTACT = contact,
                       p_REC_CUST_ADDRESS = address,
                       //p_REC_CUST_ASSET = asset,
                       p_REC_CUST_PACKAGE = package,
                       p_REC_CUST_SPLITTER = splitter,

                       //p_port_active_date = "11/09/2014",
                       //p_installation_date = "23/11/2014",

                       //return code
                       Return_code = ret_code,
                       Return_msg = v_error_msg
                   });

                command.Return_Code = ret_code.Value != null ? Convert.ToInt32(ret_code.Value.ToSafeString()) : -1;
                command.Return_Desc = v_error_msg.Value.ToSafeString();

                //GetPackageListHelper.EndInterfaceAirWfLog(_uow, _intfLog, command.Return_Code, log, "Success", command.Return_Desc);
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, command.Return_Code, log, "Success", command.Return_Desc, "");

            }
            catch (Exception ex)
            {
                _logger.Info(ex.GetErrorMessage());

                if (null != log)
                {
                    //GetPackageListHelper.EndInterfaceAirWfLog(_uow, _intfLog, ex, log,
                    //    "Failed", ex.GetErrorMessage());
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex, log, "Failed", ex.GetErrorMessage(), "");
                }

                command.Return_Code = -1;
                command.Return_Desc = "Error call save profile customer service " + ex.GetErrorMessage();
            }
        }

        #region Mapping Rec_Cust_Profile Type Oracle

        public class ProfileObjectModel : Oracle.ManagedDataAccess.Types.INullable, IOracleCustomType
        {
            [OracleArrayMapping()]
            public Rec_Cust_ProfileOracleTypeMapping[] REC_CUST_PROFILE { get; set; }

            private bool objectIsNull;

            public bool IsNull
            {
                get { return objectIsNull; }
            }

            public static ProfileObjectModel Null
            {
                get
                {
                    ProfileObjectModel obj = new ProfileObjectModel();
                    obj.objectIsNull = true;
                    return obj;
                }
            }

            public void FromCustomObject(OracleConnection con, object udt)
            {
                OracleUdt.SetValue(con, udt, 0, REC_CUST_PROFILE);
            }

            public void ToCustomObject(OracleConnection con, object udt)
            {
                REC_CUST_PROFILE = (Rec_Cust_ProfileOracleTypeMapping[])OracleUdt.GetValue(con, udt, 0);
            }
        }

        [OracleCustomTypeMappingAttribute("FBB_CUST_PROFILE_RECORD")]
        public class Rec_Cust_ProfileOracleTypeMappingFactory : IOracleCustomTypeFactory
        {
            public IOracleCustomType CreateObject()
            {
                return new Rec_Cust_ProfileOracleTypeMapping();
            }
        }

        [OracleCustomTypeMapping("FBB_CUST_PROFILE_ARRAY")]
        public class ProfileObjectModelFactory : IOracleCustomTypeFactory, IOracleArrayTypeFactory
        {
            #region IOracleCustomTypeFactory Members

            public IOracleCustomType CreateObject()
            {
                return new ProfileObjectModel();
            }

            #endregion IOracleCustomTypeFactory Members

            #region IOracleArrayTypeFactory Members

            public Array CreateArray(int numElems)
            {
                return new Rec_Cust_ProfileOracleTypeMapping[numElems];
            }

            public Array CreateStatusArray(int numElems)
            {
                return null;
            }

            #endregion IOracleArrayTypeFactory Members
        }

        public class Rec_Cust_ProfileOracleTypeMapping : Oracle.ManagedDataAccess.Types.INullable, IOracleCustomType
        {
            private bool objectIsNull;

            #region Attribute Mapping

            [OracleObjectMappingAttribute("CUST_NON_MOBILE")]
            public string cust_non_mobile { get; set; }

            [OracleObjectMappingAttribute("CA_ID")]
            public string ca_id { get; set; }

            [OracleObjectMappingAttribute("SA_ID")]
            public string sa_id { get; set; }

            [OracleObjectMappingAttribute("BA_ID")]
            public string ba_id { get; set; }

            [OracleObjectMappingAttribute("IA_ID")]
            public string ia_id { get; set; }

            [OracleObjectMappingAttribute("CUST_NAME")]
            public string cust_name { get; set; }

            [OracleObjectMappingAttribute("CUST_SURNAME")]
            public string cust_surname { get; set; }

            [OracleObjectMappingAttribute("CUST_ID_CARD_TYPE")]
            public string cust_id_card_type { get; set; }

            [OracleObjectMappingAttribute("CUST_ID_CARD_NUM")]
            public string cust_id_card_num { get; set; }

            [OracleObjectMappingAttribute("CUST_CATEGORY")]
            public string cust_category { get; set; }

            [OracleObjectMappingAttribute("CUST_SUB_CATEGORY")]
            public string cust_sub_category { get; set; }

            [OracleObjectMappingAttribute("CUST_GENDER")]
            public string cust_gender { get; set; }

            [OracleObjectMappingAttribute("CUST_BIRTHDAY")]
            public string cust_birthday { get; set; }

            [OracleObjectMappingAttribute("CUST_NATIONALITY")]
            public string cust_nationality { get; set; }

            [OracleObjectMappingAttribute("CUST_TITLE")]
            public string cust_title { get; set; }

            [OracleObjectMappingAttribute("ONLINE_NUMBER")]
            public string online_number { get; set; }

            [OracleObjectMappingAttribute("CONDO_TYPE")]
            public string condo_type { get; set; }

            [OracleObjectMappingAttribute("CONDO_DIRECTION")]
            public string condo_direction { get; set; }

            [OracleObjectMappingAttribute("CONDO_LIMIT")]
            public string condo_limit { get; set; }

            [OracleObjectMappingAttribute("CONDO_AREA")]
            public string condo_area { get; set; }

            [OracleObjectMappingAttribute("HOME_TYPE")]
            public string home_type { get; set; }

            [OracleObjectMappingAttribute("HOME_AREA")]
            public string home_area { get; set; }

            [OracleObjectMappingAttribute("DOCUMENT_TYPE")]
            public string document_type { get; set; }

            [OracleObjectMappingAttribute("CVR_ID")]
            public string cvr_id { get; set; }

            [OracleObjectMappingAttribute("PORT_ID")]
            public string port_id { get; set; }

            [OracleObjectMappingAttribute("ORDER_NO")]
            public string order_no { get; set; }

            [OracleObjectMappingAttribute("REMARK")]
            public string remark { get; set; }

            [OracleObjectMappingAttribute("PORT_ACTIVE_DATE")]
            public string port_active_date { get; set; }

            [OracleObjectMappingAttribute("INSTALLATION_DATE")]
            public string installation_date { get; set; }

            [OracleObjectMappingAttribute("RELATE_MOBILE")]
            public string relate_mobile { get; set; }

            [OracleObjectMappingAttribute("RELATE_NON_MOBILE")]
            public string relate_non_mobile { get; set; }

            [OracleObjectMappingAttribute("NETWORK_TYPE")]
            public string network_type { get; set; }

            [OracleObjectMappingAttribute("SERVICE_DAY")]
            public decimal? service_day { get; set; }

            [OracleObjectMappingAttribute("PHONE_FLAG")]
            public string phone_flag { get; set; }

            [OracleObjectMappingAttribute("TIME_SLOT")]
            public string time_slot { get; set; }

            [OracleObjectMappingAttribute("ACCESS_MODE")]
            public string access_mode { get; set; }

            [OracleObjectMappingAttribute("ADDRESS_ID")]
            public string address_id { get; set; }

            [OracleObjectMappingAttribute("SERVICE_CODE")]
            public string service_code { get; set; }

            [OracleObjectMappingAttribute("VOUCHER_PIN")]
            public string voucher_pin { get; set; }

            [OracleObjectMappingAttribute("EVENT_CODE")]
            public string event_code { get; set; }

            [OracleObjectMappingAttribute("SUB_LOCATION_ID")]
            public string sub_location_id { get; set; }

            [OracleObjectMappingAttribute("SUB_CONTRACT_NAME")]
            public string sub_contract_name { get; set; }

            [OracleObjectMappingAttribute("INSTALL_STAFF_ID")]
            public string install_staff_id { get; set; }

            [OracleObjectMappingAttribute("INSTALL_STAFF_NAME")]
            public string install_staff_name { get; set; }

            [OracleObjectMappingAttribute("SITE_CODE")]
            public string site_code { get; set; }

            [OracleObjectMappingAttribute("FLOW_FLAG")]
            public string flow_flag { get; set; }

            #endregion Attribute Mapping

            public static Rec_Cust_ProfileOracleTypeMapping Null
            {
                get
                {
                    Rec_Cust_ProfileOracleTypeMapping obj = new Rec_Cust_ProfileOracleTypeMapping();
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
                OracleUdt.SetValue(con, udt, "CUST_NON_MOBILE", cust_non_mobile);
                OracleUdt.SetValue(con, udt, "CA_ID", ca_id);
                OracleUdt.SetValue(con, udt, "SA_ID", sa_id);
                OracleUdt.SetValue(con, udt, "BA_ID", ba_id);
                OracleUdt.SetValue(con, udt, "IA_ID", ia_id);
                OracleUdt.SetValue(con, udt, "CUST_NAME", cust_name);
                OracleUdt.SetValue(con, udt, "CUST_SURNAME", cust_surname);
                OracleUdt.SetValue(con, udt, "CUST_ID_CARD_TYPE", cust_id_card_type);
                OracleUdt.SetValue(con, udt, "CUST_ID_CARD_NUM", cust_id_card_num);
                OracleUdt.SetValue(con, udt, "CUST_CATEGORY", cust_category);
                OracleUdt.SetValue(con, udt, "CUST_SUB_CATEGORY", cust_sub_category);
                OracleUdt.SetValue(con, udt, "CUST_GENDER", cust_gender);
                OracleUdt.SetValue(con, udt, "CUST_BIRTHDAY", cust_birthday);
                OracleUdt.SetValue(con, udt, "CUST_NATIONALITY", cust_nationality);
                OracleUdt.SetValue(con, udt, "CUST_TITLE", cust_title);
                OracleUdt.SetValue(con, udt, "ONLINE_NUMBER", online_number);
                OracleUdt.SetValue(con, udt, "CONDO_TYPE", condo_type);
                OracleUdt.SetValue(con, udt, "CONDO_DIRECTION", condo_direction);
                OracleUdt.SetValue(con, udt, "CONDO_LIMIT", condo_limit);
                OracleUdt.SetValue(con, udt, "CONDO_AREA", condo_area);
                OracleUdt.SetValue(con, udt, "HOME_TYPE", home_type);
                OracleUdt.SetValue(con, udt, "HOME_AREA", home_area);
                OracleUdt.SetValue(con, udt, "DOCUMENT_TYPE", document_type);
                OracleUdt.SetValue(con, udt, "CVR_ID", cvr_id);
                OracleUdt.SetValue(con, udt, "PORT_ID", port_id);
                OracleUdt.SetValue(con, udt, "ORDER_NO", order_no);
                OracleUdt.SetValue(con, udt, "REMARK", remark);
                OracleUdt.SetValue(con, udt, "PORT_ACTIVE_DATE", port_active_date);
                OracleUdt.SetValue(con, udt, "INSTALLATION_DATE", installation_date);
                OracleUdt.SetValue(con, udt, "RELATE_MOBILE", relate_mobile);
                OracleUdt.SetValue(con, udt, "RELATE_NON_MOBILE", relate_non_mobile);
                OracleUdt.SetValue(con, udt, "NETWORK_TYPE", network_type);
                OracleUdt.SetValue(con, udt, "SERVICE_DAY", service_day);
                OracleUdt.SetValue(con, udt, "PHONE_FLAG", phone_flag);
                OracleUdt.SetValue(con, udt, "TIME_SLOT", time_slot);
                OracleUdt.SetValue(con, udt, "ACCESS_MODE", access_mode);
                OracleUdt.SetValue(con, udt, "ADDRESS_ID", address_id);
                OracleUdt.SetValue(con, udt, "SERVICE_CODE", service_code);
                OracleUdt.SetValue(con, udt, "VOUCHER_PIN", voucher_pin);
                OracleUdt.SetValue(con, udt, "EVENT_CODE", event_code);
                OracleUdt.SetValue(con, udt, "SUB_LOCATION_ID", sub_location_id);
                OracleUdt.SetValue(con, udt, "SUB_CONTRACT_NAME", sub_contract_name);
                OracleUdt.SetValue(con, udt, "INSTALL_STAFF_ID", install_staff_id);
                OracleUdt.SetValue(con, udt, "INSTALL_STAFF_NAME", install_staff_name);
                OracleUdt.SetValue(con, udt, "SITE_CODE", site_code);
                OracleUdt.SetValue(con, udt, "FLOW_FLAG", flow_flag);
            }

            public void ToCustomObject(OracleConnection con, object udt)
            {
                throw new NotImplementedException();
            }
        }

        #endregion Mapping Rec_Cust_Profilet Type Oracle

        #region Mapping Rec_Cust_Contact Type Oracle

        public class ContactObjectModel : Oracle.ManagedDataAccess.Types.INullable, IOracleCustomType
        {
            [OracleArrayMapping()]
            public Rec_Cust_ContactOracleTypeMapping[] REC_CUST_CONTACT { get; set; }

            private bool objectIsNull;

            public bool IsNull
            {
                get { return objectIsNull; }
            }

            public static ContactObjectModel Null
            {
                get
                {
                    ContactObjectModel obj = new ContactObjectModel();
                    obj.objectIsNull = true;
                    return obj;
                }
            }

            public void FromCustomObject(OracleConnection con, object udt)
            {
                OracleUdt.SetValue(con, udt, 0, REC_CUST_CONTACT);
            }

            public void ToCustomObject(OracleConnection con, object udt)
            {
                REC_CUST_CONTACT = (Rec_Cust_ContactOracleTypeMapping[])OracleUdt.GetValue(con, udt, 0);
            }
        }

        [OracleCustomTypeMappingAttribute("FBB_CUST_CONTACT_RECORD")]
        public class Rec_Cust_ContactOracleTypeMappingFactory : IOracleCustomTypeFactory
        {
            public IOracleCustomType CreateObject()
            {
                return new Rec_Cust_ContactOracleTypeMapping();
            }
        }

        [OracleCustomTypeMapping("FBB_CUST_CONTACT_ARRAY")]
        public class ContactObjectModelFactory : IOracleCustomTypeFactory, IOracleArrayTypeFactory
        {
            #region IOracleCustomTypeFactory Members

            public IOracleCustomType CreateObject()
            {
                return new ContactObjectModel();
            }

            #endregion IOracleCustomTypeFactory Members

            #region IOracleArrayTypeFactory Members

            public Array CreateArray(int numElems)
            {
                return new Rec_Cust_ContactOracleTypeMapping[numElems];
            }

            public Array CreateStatusArray(int numElems)
            {
                return null;
            }

            #endregion IOracleArrayTypeFactory Members
        }

        public class Rec_Cust_ContactOracleTypeMapping : Oracle.ManagedDataAccess.Types.INullable, IOracleCustomType
        {
            private bool objectIsNull;

            #region Attribute Mapping

            [OracleObjectMappingAttribute("ADDRESS_IN_SEQ")]
            public string address_IN_seq { get; set; }

            [OracleObjectMappingAttribute("CONTACT_FIRST_NAME")]
            public string contact_first_name { get; set; }

            [OracleObjectMappingAttribute("CONTACT_LAST_NAME")]
            public string contact_last_name { get; set; }

            [OracleObjectMappingAttribute("CONTACT_HOME_PHONE")]
            public string contact_home_phone { get; set; }

            [OracleObjectMappingAttribute("CONTACT_MOBILE_PHONE1")]
            public string contact_mobile_phone1 { get; set; }

            [OracleObjectMappingAttribute("CONTACT_MOBILE_PHONE2")]
            public string contact_mobile_phone2 { get; set; }

            [OracleObjectMappingAttribute("CONTACT_EMAIL")]
            public string contact_email { get; set; }

            [OracleObjectMappingAttribute("CA_ID")]
            public string ca_id { get; set; }

            [OracleObjectMappingAttribute("BA_ID")]
            public string ba_id { get; set; }

            #endregion Attribute Mapping

            public static Rec_Cust_ContactOracleTypeMapping Null
            {
                get
                {
                    Rec_Cust_ContactOracleTypeMapping obj = new Rec_Cust_ContactOracleTypeMapping();
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
                OracleUdt.SetValue(con, udt, "ADDRESS_IN_SEQ", address_IN_seq);
                OracleUdt.SetValue(con, udt, "CONTACT_FIRST_NAME", contact_first_name);
                OracleUdt.SetValue(con, udt, "CONTACT_LAST_NAME", contact_last_name);
                OracleUdt.SetValue(con, udt, "CONTACT_HOME_PHONE", contact_home_phone);
                OracleUdt.SetValue(con, udt, "CONTACT_MOBILE_PHONE1", contact_mobile_phone1);
                OracleUdt.SetValue(con, udt, "CONTACT_MOBILE_PHONE2", contact_mobile_phone2);
                OracleUdt.SetValue(con, udt, "CONTACT_EMAIL", contact_email);
                OracleUdt.SetValue(con, udt, "CA_ID", ca_id);
                OracleUdt.SetValue(con, udt, "BA_ID", ba_id);
            }

            public void ToCustomObject(OracleConnection con, object udt)
            {
                throw new NotImplementedException();
            }
        }

        #endregion Mapping Rec_Cust_Contact Type Oracle

        #region Mapping Rec_Cust_Address Type Oracle

        public class AddressObjectModel : Oracle.ManagedDataAccess.Types.INullable, IOracleCustomType
        {
            [OracleArrayMapping()]
            public Rec_Cust_AddressOracleTypeMapping[] REC_CUST_ADDRESS { get; set; }

            private bool objectIsNull;

            public bool IsNull
            {
                get { return objectIsNull; }
            }

            public static AddressObjectModel Null
            {
                get
                {
                    AddressObjectModel obj = new AddressObjectModel();
                    obj.objectIsNull = true;
                    return obj;
                }
            }

            public void FromCustomObject(OracleConnection con, object udt)
            {
                OracleUdt.SetValue(con, udt, 0, REC_CUST_ADDRESS);
            }

            public void ToCustomObject(OracleConnection con, object udt)
            {
                REC_CUST_ADDRESS = (Rec_Cust_AddressOracleTypeMapping[])OracleUdt.GetValue(con, udt, 0);
            }
        }

        [OracleCustomTypeMappingAttribute("FBB_CUST_ADDRESS_RECORD")]
        public class Rec_Cust_AddressOracleTypeMappingFactory : IOracleCustomTypeFactory
        {
            public IOracleCustomType CreateObject()
            {
                return new Rec_Cust_AddressOracleTypeMapping();
            }
        }

        [OracleCustomTypeMapping("FBB_CUST_ADDRESS_ARRAY")]
        public class AddressObjectModelFactory : IOracleCustomTypeFactory, IOracleArrayTypeFactory
        {
            #region IOracleCustomTypeFactory Members

            public IOracleCustomType CreateObject()
            {
                return new AddressObjectModel();
            }

            #endregion IOracleCustomTypeFactory Members

            #region IOracleArrayTypeFactory Members

            public Array CreateArray(int numElems)
            {
                return new Rec_Cust_AddressOracleTypeMapping[numElems];
            }

            public Array CreateStatusArray(int numElems)
            {
                return null;
            }

            #endregion IOracleArrayTypeFactory Members
        }

        public class Rec_Cust_AddressOracleTypeMapping : Oracle.ManagedDataAccess.Types.INullable, IOracleCustomType
        {
            private bool objectIsNull;

            #region Attribute Mapping

            [OracleObjectMappingAttribute("ADDR_TYPE")]
            public string addr_type { get; set; }

            [OracleObjectMappingAttribute("HOUSE_NO")]
            public string house_no { get; set; }

            [OracleObjectMappingAttribute("SOI")]
            public string soi { get; set; }

            [OracleObjectMappingAttribute("MOO")]
            public string moo { get; set; }

            [OracleObjectMappingAttribute("MOOBAN")]
            public string mooban { get; set; }

            [OracleObjectMappingAttribute("BUILDING_NAME")]
            public string building_name { get; set; }

            [OracleObjectMappingAttribute("FLOOR")]
            public string floor { get; set; }

            [OracleObjectMappingAttribute("ROOM")]
            public string room { get; set; }

            [OracleObjectMappingAttribute("STREET_NAME")]
            public string street_name { get; set; }

            [OracleObjectMappingAttribute("ZIPCODE_ID")]
            public string zipcode_id { get; set; }

            [OracleObjectMappingAttribute("ADDRESS_VAT")]
            public string address_vat { get; set; }

            [OracleObjectMappingAttribute("ADDRESS_IN_SEQ")]
            public string address_IN_seq { get; set; }

            [OracleObjectMappingAttribute("CA_ID")]
            public string ca_id { get; set; }

            [OracleObjectMappingAttribute("BA_ID")]
            public string ba_id { get; set; }

            #endregion Attribute Mapping

            public static Rec_Cust_AddressOracleTypeMapping Null
            {
                get
                {
                    Rec_Cust_AddressOracleTypeMapping obj = new Rec_Cust_AddressOracleTypeMapping();
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
                OracleUdt.SetValue(con, udt, "ADDR_TYPE", addr_type);
                OracleUdt.SetValue(con, udt, "HOUSE_NO", house_no);
                OracleUdt.SetValue(con, udt, "SOI", soi);
                OracleUdt.SetValue(con, udt, "MOO", moo);
                OracleUdt.SetValue(con, udt, "MOOBAN", mooban);
                OracleUdt.SetValue(con, udt, "BUILDING_NAME", building_name);
                OracleUdt.SetValue(con, udt, "FLOOR", floor);
                OracleUdt.SetValue(con, udt, "ROOM", room);
                OracleUdt.SetValue(con, udt, "STREET_NAME", street_name);
                OracleUdt.SetValue(con, udt, "ZIPCODE_ID", zipcode_id);
                OracleUdt.SetValue(con, udt, "ADDRESS_VAT", address_vat);
                OracleUdt.SetValue(con, udt, "ADDRESS_IN_SEQ", address_IN_seq);
                OracleUdt.SetValue(con, udt, "CA_ID", ca_id);
                OracleUdt.SetValue(con, udt, "BA_ID", ba_id);
            }

            public void ToCustomObject(OracleConnection con, object udt)
            {
                throw new NotImplementedException();
            }
        }

        #endregion Mapping Rec_Cust_Address Type Oracle

        //#region Mapping Rec_Cust_Asset Type Oracle
        //public class AssetObjectModel : Oracle.ManagedDataAccess.Types.INullable, IOracleCustomType
        //{
        //    [OracleArrayMapping()]
        //    public Rec_Cust_AssetOracleTypeMapping[] REC_CUST_ASSET { get; set; }

        //    private bool objectIsNull;

        //    public bool IsNull
        //    {
        //        get { return objectIsNull; }
        //    }

        //    public static AssetObjectModel Null
        //    {
        //        get
        //        {
        //            AssetObjectModel obj = new AssetObjectModel();
        //            obj.objectIsNull = true;
        //            return obj;
        //        }
        //    }

        //    public void FromCustomObject(OracleConnection con, IntPtr pUdt)
        //    {
        //        OracleUdt.SetValue(con, pUdt, 0, REC_CUST_ASSET);
        //    }

        //    public void ToCustomObject(OracleConnection con, IntPtr pUdt)
        //    {
        //        REC_CUST_ASSET = (Rec_Cust_AssetOracleTypeMapping[])OracleUdt.GetValue(con, pUdt, 0);
        //    }
        //}
        //[OracleCustomTypeMappingAttribute("FBB_CUST_ASSET_RECORD")]
        //public class Rec_Cust_AssetOracleTypeMappingFactory : IOracleCustomTypeFactory
        //{
        //    #region IOracleCustomTypeFactory Members

        //    public IOracleCustomType CreateObject()
        //    {
        //        return new Rec_Cust_AssetOracleTypeMapping();
        //    }

        //    #endregion
        //}
        //[OracleCustomTypeMapping("FBB_CUST_ASSET_ARRAY")]
        //public class AssetObjectModelFactory : IOracleCustomTypeFactory, IOracleArrayTypeFactory
        //{
        //    #region IOracleCustomTypeFactory Members
        //    public IOracleCustomType CreateObject()
        //    {
        //        return new AssetObjectModel();
        //    }

        //    #endregion

        //    #region IOracleArrayTypeFactory Members
        //    public Array CreateArray(int numElems)
        //    {
        //        return new Rec_Cust_AssetOracleTypeMapping[numElems];
        //    }

        //    public Array CreateStatusArray(int numElems)
        //    {
        //        return null;
        //    }

        //    #endregion
        //}

        //public class Rec_Cust_AssetOracleTypeMapping : Oracle.ManagedDataAccess.Types.INullable, IOracleCustomType
        //{
        //    private bool objectIsNull;

        //    #region Attribute Mapping

        //    [OracleObjectMappingAttribute("ASSET_CODE")]
        //    public string asset_code { get; set; }

        //    [OracleObjectMappingAttribute("PACKAGE_CODE")]
        //    public string package_code { get; set; }

        //    [OracleObjectMappingAttribute("EQUIPMENT_TYPE")]
        //    public string equipment_type { get; set; }

        //    [OracleObjectMappingAttribute("ASSET_STATUS")]
        //    public string asset_status { get; set; }

        //    [OracleObjectMappingAttribute("ASSET_NAME")]
        //    public string asset_name { get; set; }

        //    [OracleObjectMappingAttribute("ASSET_CHARGE")]
        //    public string asset_charge { get; set; }

        //    [OracleObjectMappingAttribute("ASSET_DISCOUNT")]
        //    public string asset_discount { get; set; }

        //    [OracleObjectMappingAttribute("ASSET_START_DT")]
        //    public string asset_start_dt { get; set; }

        //    [OracleObjectMappingAttribute("ASSET_END_DT")]
        //    public string asset_end_dt { get; set; }

        //    [OracleObjectMappingAttribute("SERIAL_NUMBER")]
        //    public string serial_number { get; set; }

        //    [OracleObjectMappingAttribute("ASSET_MODEL")]
        //    public string asset_model { get; set; }
        //    #endregion

        //    public static Rec_Cust_AssetOracleTypeMapping Null
        //    {
        //        get
        //        {
        //            Rec_Cust_AssetOracleTypeMapping obj = new Rec_Cust_AssetOracleTypeMapping();
        //            obj.objectIsNull = true;
        //            return obj;
        //        }
        //    }

        //    public bool IsNull
        //    {
        //        get { return objectIsNull; }
        //    }

        //    public void FromCustomObject(Oracle.ManagedDataAccess.Client.OracleConnection con, IntPtr pUdt)
        //    {
        //        OracleUdt.SetValue(con, pUdt, "ASSET_CODE", asset_code);
        //        OracleUdt.SetValue(con, pUdt, "PACKAGE_CODE", package_code);
        //        OracleUdt.SetValue(con, pUdt, "EQUIPMENT_TYPE", equipment_type);
        //        OracleUdt.SetValue(con, pUdt, "ASSET_STATUS", asset_status);
        //        OracleUdt.SetValue(con, pUdt, "ASSET_NAME", asset_name);
        //        OracleUdt.SetValue(con, pUdt, "ASSET_CHARGE", asset_charge);
        //        OracleUdt.SetValue(con, pUdt, "ASSET_DISCOUNT", asset_discount);
        //        OracleUdt.SetValue(con, pUdt, "ASSET_START_DT", asset_start_dt);
        //        OracleUdt.SetValue(con, pUdt, "ASSET_END_DT", asset_end_dt);
        //        OracleUdt.SetValue(con, pUdt, "SERIAL_NUMBER", serial_number);
        //        OracleUdt.SetValue(con, pUdt, "ASSET_MODEL", asset_model);
        //    }

        //    public void ToCustomObject(Oracle.ManagedDataAccess.Client.OracleConnection con, IntPtr pUdt)
        //    {
        //        throw new NotImplementedException();
        //    }
        //}
        //#endregion

        #region Mapping Rec_Cust_Package Type Oracle

        public class PackageObjectModel : Oracle.ManagedDataAccess.Types.INullable, IOracleCustomType
        {
            [OracleArrayMapping()]
            public Rec_Cust_PackageOracleTypeMapping[] REC_CUST_PACKAGE { get; set; }

            private bool objectIsNull;

            public bool IsNull
            {
                get { return objectIsNull; }
            }

            public static PackageObjectModel Null
            {
                get
                {
                    PackageObjectModel obj = new PackageObjectModel();
                    obj.objectIsNull = true;
                    return obj;
                }
            }

            public void FromCustomObject(OracleConnection con, object udt)
            {
                OracleUdt.SetValue(con, udt, 0, REC_CUST_PACKAGE);
            }

            public void ToCustomObject(OracleConnection con, object udt)
            {
                REC_CUST_PACKAGE = (Rec_Cust_PackageOracleTypeMapping[])OracleUdt.GetValue(con, udt, 0);
            }
        }

        [OracleCustomTypeMappingAttribute("FBB_CUST_PACKAGE_RECORD")]
        public class Rec_Cust_PackageOracleTypeMappingFactory : IOracleCustomTypeFactory
        {
            #region IOracleCustomTypeFactory Members

            public IOracleCustomType CreateObject()
            {
                return new Rec_Cust_PackageOracleTypeMapping();
            }

            #endregion IOracleCustomTypeFactory Members
        }

        [OracleCustomTypeMapping("FBB_CUST_PACKAGE_ARRAY")]
        public class PackageObjectModelFactory : IOracleCustomTypeFactory, IOracleArrayTypeFactory
        {
            #region IOracleCustomTypeFactory Members

            public IOracleCustomType CreateObject()
            {
                return new PackageObjectModel();
            }

            #endregion IOracleCustomTypeFactory Members

            #region IOracleArrayTypeFactory Members

            public Array CreateArray(int numElems)
            {
                return new Rec_Cust_PackageOracleTypeMapping[numElems];
            }

            public Array CreateStatusArray(int numElems)
            {
                return null;
            }

            #endregion IOracleArrayTypeFactory Members
        }

        public class Rec_Cust_PackageOracleTypeMapping : Oracle.ManagedDataAccess.Types.INullable, IOracleCustomType
        {
            private bool objectIsNull;

            #region Attribute Mapping

            [OracleObjectMappingAttribute("PACKAGE_CODE")]
            public string package_code { get; set; }

            [OracleObjectMappingAttribute("PACKAGE_CLASS")]
            public string package_class { get; set; }

            [OracleObjectMappingAttribute("PACKAGE_TYPE")]
            public string package_type { get; set; }

            [OracleObjectMappingAttribute("PACKAGE_GROUP")]
            public string package_group { get; set; }

            [OracleObjectMappingAttribute("PACKAGE_SUBTYPE")]
            public string package_subtype { get; set; }

            [OracleObjectMappingAttribute("PACKAGE_OWNER")]
            public string package_owner { get; set; }

            [OracleObjectMappingAttribute("TECHNOLOGY")]
            public string technology { get; set; }

            [OracleObjectMappingAttribute("PACKAGE_STATUS")]
            public string package_status { get; set; }

            [OracleObjectMappingAttribute("PACKAGE_NAME")]
            public string package_name { get; set; }

            [OracleObjectMappingAttribute("RECURRING_CHARGE")]
            public string recurring_charge { get; set; }

            [OracleObjectMappingAttribute("PRE_RECURRING_CHARGE")]
            public string pre_recurring_charge { get; set; }

            [OracleObjectMappingAttribute("RECURRING_DISCOUNT_EXP")]
            public string recurring_discount_exp { get; set; }

            [OracleObjectMappingAttribute("RECURRING_START_DT")]
            public string recurring_start_dt { get; set; }

            [OracleObjectMappingAttribute("RECURRING_END_DT")]
            public string recurring_end_dt { get; set; }

            [OracleObjectMappingAttribute("INITIATION_CHARGE")]
            public string initiation_charge { get; set; }

            [OracleObjectMappingAttribute("PRE_INITIATION_CHARGE")]
            public string pre_initiation_charge { get; set; }

            [OracleObjectMappingAttribute("PACKAGE_BILL_THA")]
            public string package_bill_tha { get; set; }

            [OracleObjectMappingAttribute("DOWNLOAD_SPEED")]
            public string download_speed { get; set; }

            [OracleObjectMappingAttribute("UPLOAD_SPEED")]
            public string upload_speed { get; set; }

            [OracleObjectMappingAttribute("HOME_IP")]
            public string home_ip { get; set; }

            [OracleObjectMappingAttribute("HOME_PORT")]
            public string home_port { get; set; }

            [OracleObjectMappingAttribute("IDD_FLAG")]
            public string idd_flag { get; set; }

            [OracleObjectMappingAttribute("FAX_FLAG")]
            public string fax_flag { get; set; }

            [OracleObjectMappingAttribute("CUST_NON_MOBILE")]
            public string cust_non_mobile { get; set; }

            [OracleObjectMappingAttribute("MOBILE_FORWARD")]
            public string mobile_forward { get; set; }

            #endregion Attribute Mapping

            public static Rec_Cust_PackageOracleTypeMapping Null
            {
                get
                {
                    Rec_Cust_PackageOracleTypeMapping obj = new Rec_Cust_PackageOracleTypeMapping();
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
                OracleUdt.SetValue(con, udt, "PACKAGE_CODE", package_code);
                OracleUdt.SetValue(con, udt, "PACKAGE_CLASS", package_class);
                OracleUdt.SetValue(con, udt, "PACKAGE_TYPE", package_type);
                OracleUdt.SetValue(con, udt, "PACKAGE_GROUP", package_group);
                OracleUdt.SetValue(con, udt, "PACKAGE_SUBTYPE", package_subtype);
                OracleUdt.SetValue(con, udt, "PACKAGE_OWNER", package_owner);
                OracleUdt.SetValue(con, udt, "TECHNOLOGY", technology);
                OracleUdt.SetValue(con, udt, "PACKAGE_STATUS", package_status);
                OracleUdt.SetValue(con, udt, "PACKAGE_NAME", package_name);
                OracleUdt.SetValue(con, udt, "RECURRING_CHARGE", recurring_charge);
                OracleUdt.SetValue(con, udt, "PRE_RECURRING_CHARGE", pre_recurring_charge);
                OracleUdt.SetValue(con, udt, "RECURRING_DISCOUNT_EXP", recurring_discount_exp);
                OracleUdt.SetValue(con, udt, "RECURRING_START_DT", recurring_start_dt);
                OracleUdt.SetValue(con, udt, "RECURRING_END_DT", recurring_end_dt);
                OracleUdt.SetValue(con, udt, "INITIATION_CHARGE", initiation_charge);
                OracleUdt.SetValue(con, udt, "PRE_INITIATION_CHARGE", pre_initiation_charge);
                OracleUdt.SetValue(con, udt, "PACKAGE_BILL_THA", package_bill_tha);
                OracleUdt.SetValue(con, udt, "DOWNLOAD_SPEED", download_speed);
                OracleUdt.SetValue(con, udt, "UPLOAD_SPEED", upload_speed);
                OracleUdt.SetValue(con, udt, "HOME_IP", home_ip);
                OracleUdt.SetValue(con, udt, "HOME_PORT", home_port);
                OracleUdt.SetValue(con, udt, "IDD_FLAG", idd_flag);
                OracleUdt.SetValue(con, udt, "FAX_FLAG", fax_flag);
                OracleUdt.SetValue(con, udt, "CUST_NON_MOBILE", cust_non_mobile);
                OracleUdt.SetValue(con, udt, "MOBILE_FORWARD", mobile_forward);
            }

            public void ToCustomObject(OracleConnection con, object udt)
            {
                throw new NotImplementedException();
            }
        }

        #endregion Mapping Rec_Cust_Package Type Oracle

        #region Mapping Rec_Cust_Splitter Type Oracle
        public class SplitterObjectModel : Oracle.ManagedDataAccess.Types.INullable, IOracleCustomType
        {
            [OracleArrayMapping()]
            public Rec_Cust_SplitterOracleTypeMapping[] REC_CUST_SPLITTER { get; set; }

            private bool objectIsNull;

            public bool IsNull
            {
                get { return objectIsNull; }
            }

            public static SplitterObjectModel Null
            {
                get
                {
                    SplitterObjectModel obj = new SplitterObjectModel();
                    obj.objectIsNull = true;
                    return obj;
                }
            }

            public void FromCustomObject(OracleConnection con, object udt)
            {
                OracleUdt.SetValue(con, udt, 0, REC_CUST_SPLITTER);
            }

            public void ToCustomObject(OracleConnection con, object udt)
            {
                REC_CUST_SPLITTER = (Rec_Cust_SplitterOracleTypeMapping[])OracleUdt.GetValue(con, udt, 0);
            }
        }

        [OracleCustomTypeMappingAttribute("FBB_CUST_SPLITTER_RECORD")]
        public class Rec_Cust_SplitterOracleTypeMappingFactory : IOracleCustomTypeFactory
        {
            #region IOracleCustomTypeFactory Members

            public IOracleCustomType CreateObject()
            {
                return new Rec_Cust_SplitterOracleTypeMapping();
            }

            #endregion IOracleCustomTypeFactory Members
        }

        [OracleCustomTypeMapping("FBB_CUST_SPLITTER_ARRAY")]
        public class SplitterObjectModelFactory : IOracleCustomTypeFactory, IOracleArrayTypeFactory
        {
            #region IOracleCustomTypeFactory Members

            public IOracleCustomType CreateObject()
            {
                return new SplitterObjectModel();
            }

            #endregion IOracleCustomTypeFactory Members

            #region IOracleArrayTypeFactory Members

            public Array CreateArray(int numElems)
            {
                return new Rec_Cust_SplitterOracleTypeMapping[numElems];
            }

            public Array CreateStatusArray(int numElems)
            {
                return null;
            }

            #endregion IOracleArrayTypeFactory Members
        }

        public class Rec_Cust_SplitterOracleTypeMapping : Oracle.ManagedDataAccess.Types.INullable, IOracleCustomType
        {
            private bool objectIsNull;

            #region Attribute Mapping

            [OracleObjectMappingAttribute("CUST_NON_MOBILE")]
            public string cust_non_mobile { get; set; }

            [OracleObjectMappingAttribute("SPLITTER_NAME")]
            public string splitter_name { get; set; }

            [OracleObjectMappingAttribute("DISTANCE")]
            public decimal distance { get; set; }

            [OracleObjectMappingAttribute("DISTANCE_TYPE")]
            public string distance_type { get; set; }

            #endregion Attribute Mapping

            public static Rec_Cust_SplitterOracleTypeMapping Null
            {
                get
                {
                    Rec_Cust_SplitterOracleTypeMapping obj = new Rec_Cust_SplitterOracleTypeMapping();
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
                OracleUdt.SetValue(con, udt, "CUST_NON_MOBILE", cust_non_mobile);
                OracleUdt.SetValue(con, udt, "SPLITTER_NAME", splitter_name);
                OracleUdt.SetValue(con, udt, "DISTANCE", distance);
                OracleUdt.SetValue(con, udt, "DISTANCE_TYPE", distance_type);
            }

            public void ToCustomObject(OracleConnection con, object udt)
            {
                throw new NotImplementedException();
            }
        }
        #endregion Mapping Rec_Cust_Splitter Type Oracle
    }
}