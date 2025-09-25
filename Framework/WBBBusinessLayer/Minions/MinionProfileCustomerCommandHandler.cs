using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Minions;
using WBBData.Repository;
using WBBEntity.Extensions;

namespace WBBBusinessLayer.Minions
{
    public class MinionProfileCustomerCommandHandler : ICommandHandler<MinionProcMainCommand>
    {
        private readonly IEntityRepository<string> _objService;

        public MinionProfileCustomerCommandHandler(IEntityRepository<string> objService)
        {
            _objService = objService;
        }

        public void Handle(MinionProcMainCommand command)
        {
            try
            {

                var profileModel = new WBBBusinessLayer.CommandHandlers.ProfileCustomerCommandHandler.ProfileObjectModel();
                var addrModel = new WBBBusinessLayer.CommandHandlers.ProfileCustomerCommandHandler.AddressObjectModel();
                var contactModel = new WBBBusinessLayer.CommandHandlers.ProfileCustomerCommandHandler.ContactObjectModel();
                var packageModel = new WBBBusinessLayer.CommandHandlers.ProfileCustomerCommandHandler.PackageObjectModel();
                var splitterModel = new WBBBusinessLayer.CommandHandlers.ProfileCustomerCommandHandler.SplitterObjectModel();

                addrModel.REC_CUST_ADDRESS = command.FbbCustAddressList.Select(a => new WBBBusinessLayer.CommandHandlers.ProfileCustomerCommandHandler.Rec_Cust_AddressOracleTypeMapping
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

                profileModel.REC_CUST_PROFILE = command.FbbCustProfilesList.Select(p => new WBBBusinessLayer.CommandHandlers.ProfileCustomerCommandHandler.Rec_Cust_ProfileOracleTypeMapping
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

                contactModel.REC_CUST_CONTACT = command.FbbCustContactList.Select(c => new WBBBusinessLayer.CommandHandlers.ProfileCustomerCommandHandler.Rec_Cust_ContactOracleTypeMapping
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

                packageModel.REC_CUST_PACKAGE = command.FbbCustPackageList.Select(p => new WBBBusinessLayer.CommandHandlers.ProfileCustomerCommandHandler.Rec_Cust_PackageOracleTypeMapping
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

                splitterModel.REC_CUST_SPLITTER = command.FbbCustSplitterList.Select(p => new WBBBusinessLayer.CommandHandlers.ProfileCustomerCommandHandler.Rec_Cust_SplitterOracleTypeMapping
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
                var package = OracleCustomTypeUtilities.CreateUDTArrayParameter("p_REC_CUST_PACKAGE", "FBB_CUST_PACKAGE_ARRAY", packageModel);
                var splitter = OracleCustomTypeUtilities.CreateUDTArrayParameter("p_REC_CUST_SPLITTER", "FBB_CUST_SPLITTER_ARRAY", splitterModel);

                _objService.ExecuteStoredProc("WBB.PKG_FBB_INS_PROFILE.PROC_MAIN",

                out paramOut,
                   new
                   {
                       p_REC_CUST_PROFILE = profile,
                       p_REC_CUST_CONTACT = contact,
                       p_REC_CUST_ADDRESS = address,
                       p_REC_CUST_PACKAGE = package,
                       p_REC_CUST_SPLITTER = splitter,


                       //return code
                       p_Return_code = ret_code,
                       Return_msg = v_error_msg
                   });

                command.Return_Code = ret_code.Value != null ? Convert.ToInt32(ret_code.Value.ToSafeString()) : -1;
                command.Return_Desc = v_error_msg.Value.ToSafeString();


            }
            catch (Exception ex)
            {
                command.Return_Code = -1;
                command.Return_Desc = "Error call WBB.PKG_FBB_INS_PROFILE.PROC_MAIN " + ex.GetErrorMessage();
            }
        }
    }
}
