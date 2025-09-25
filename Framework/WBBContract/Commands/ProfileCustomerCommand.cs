using System.Collections.Generic;

namespace WBBContract.Commands
{
    public class ProfileCustomerCommand
    {
        public ProfileCustomerCommand()
        {
            this.Return_Code = -1;
            this.Return_Desc = "";
        }

        // for return
        public int Return_Code { get; set; }
        public string Return_Desc { get; set; }

        private List<REC_CUST_PROFILE> _Rec_Cust_Profile;
        public List<REC_CUST_PROFILE> Rec_Cust_Profile
        {
            get { return _Rec_Cust_Profile; }
            set { _Rec_Cust_Profile = value; }
        }

        private List<REC_CUST_CONTACT> _Rec_Cust_Contact;
        public List<REC_CUST_CONTACT> Rec_Cust_Contact
        {
            get { return _Rec_Cust_Contact; }
            set { _Rec_Cust_Contact = value; }
        }

        private List<REC_CUST_ADDRESS> _Rec_Cust_Address;
        public List<REC_CUST_ADDRESS> Rec_Cust_Address
        {
            get { return _Rec_Cust_Address; }
            set { _Rec_Cust_Address = value; }
        }

        //private List<REC_CUST_ASSET> _Rec_Cust_Asset;
        //public List<REC_CUST_ASSET> Rec_Cust_Asset
        //{
        //    get { return _Rec_Cust_Asset; }
        //    set { _Rec_Cust_Asset = value; }
        //}

        private List<REC_CUST_PACKAGE> _Rec_Cust_Package;
        public List<REC_CUST_PACKAGE> Rec_Cust_Package
        {
            get { return _Rec_Cust_Package; }
            set { _Rec_Cust_Package = value; }
        }

        //Update 16.2
        private List<REC_CUST_SPLITTER> _Rec_Cust_Splitter;
        public List<REC_CUST_SPLITTER> Rec_Cust_Splitter
        {
            get { return _Rec_Cust_Splitter; }
            set { _Rec_Cust_Splitter = value; }
        }

    }

    public class REC_CUST_PROFILE
    {
        public string Cust_Non_Mobile { get; set; }
        public string Ca_Id { get; set; }
        public string Sa_Id { get; set; }
        public string Ba_Id { get; set; }
        public string Ia_Id { get; set; }
        public string Cust_Name { get; set; }
        public string Cust_Surname { get; set; }
        public string Cust_Id_Card_Type { get; set; }
        public string Cust_Id_Card_Num { get; set; }
        public string Cust_Category { get; set; }
        public string Cust_Sub_Category { get; set; }
        public string Cust_Gender { get; set; }
        public string Cust_Birthday { get; set; }
        public string Cust_Nationality { get; set; }
        public string Cust_Title { get; set; }
        public string Online_Number { get; set; }
        public string Condo_Type { get; set; }
        public string Condo_Direction { get; set; }
        public string Condo_Limit { get; set; }
        public string Condo_Area { get; set; }
        public string Home_Type { get; set; }
        public string Home_Area { get; set; }
        public string Document_Type { get; set; }
        public string Cvr_Id { get; set; }
        public string Port_Id { get; set; }
        public string Order_No { get; set; }
        public string Remark { get; set; }
        public string Port_Active_Date { get; set; }
        public string Installation_Date { get; set; }
        public string Relate_Mobile { get; set; }
        public string Relate_Non_Mobile { get; set; }
        public string Network_Type { get; set; }
        public int? Service_Day { get; set; }

        // Update 15.3
        public string Phone_Flag { get; set; }
        public string Time_Slot { get; set; }
        public string Address_Id { get; set; }
        public string Access_Mode { get; set; }
        public string Service_Code { get; set; }

        //Update 16.1 - Gift Voucher & Event flow
        public string event_code { get; set; }
        public string install_staff_id { get; set; }
        public string install_staff_name { get; set; }
        public string sub_contract_name { get; set; }
        public string sub_location_id { get; set; }
        public string gift_voucher { get; set; }

        //update 16.2
        public string site_code { get; set; }
        public string flow_flag { get; set; }
    }

    public class REC_CUST_CONTACT
    {
        public string Address_In_Seq { get; set; }
        public string Contact_First_Name { get; set; }
        public string Contact_Last_Name { get; set; }
        public string Contact_Home_Phone { get; set; }
        public string Contact_Mobile_Phone1 { get; set; }
        public string Contact_Mobile_Phone2 { get; set; }
        public string Contact_Email { get; set; }
        public string ca_id { get; set; }
        public string ba_id { get; set; }
    }

    public class REC_CUST_ADDRESS
    {
        public string Addr_Type { get; set; }
        public string House_No { get; set; }
        public string Soi { get; set; }
        public string Moo { get; set; }
        public string Mooban { get; set; }
        public string Building_Name { get; set; }
        public string Floor { get; set; }
        public string Room { get; set; }
        public string Street_Name { get; set; }
        public string Zipcode_Id { get; set; }
        public string Address_Vat { get; set; }
        public string Address_In_Seq { get; set; }
        public string ca_id { get; set; }
        public string ba_id { get; set; }
    }

    //public class REC_CUST_ASSET
    //{
    //    public string Asset_Code { get; set; }
    //    public string Package_Code { get; set; }
    //    public string Equipment_Type { get; set; }
    //    public string Asset_Status { get; set; }
    //    public string Asset_Name { get; set; }
    //    public string Asset_Charge { get; set; }
    //    public string Asset_Discount { get; set; }
    //    public string Asset_Start_Dt { get; set; }
    //    public string Asset_End_Dt { get; set; }
    //    public string Serial_Number { get; set; }
    //    public string Asset_Model { get; set; }
    //}

    public class REC_CUST_PACKAGE
    {
        public string Package_Code { get; set; }
        public string Package_Class { get; set; }
        public string Package_Type { get; set; }
        public string Package_Group { get; set; }
        public string Package_Subtype { get; set; }
        public string Package_Owner { get; set; }
        public string Technology { get; set; }
        public string package_Status { get; set; }
        public string package_Name { get; set; }
        public string Recurring_Charge { get; set; }
        //public string Recurring_Discount { get; set; }
        public string Pre_Recurring_Charge { get; set; }
        public string Recurring_Discount_Exp { get; set; }
        public string Recurring_Start_Dt { get; set; }
        public string Recurring_End_Dt { get; set; }
        public string Initiation_Charge { get; set; }
        //public string Initiation_Discount { get; set; }
        public string Pre_Initiation_Charge { get; set; }
        public string Package_Bill_Tha { get; set; }
        public string Download_Speed { get; set; }
        public string Upload_Speed { get; set; }
        public string Home_Ip { get; set; }
        public string Home_Port { get; set; }
        public string Idd_Flag { get; set; }
        public string Fax_Flag { get; set; }
        public string Cust_Non_Mobile { get; set; }
        public string Mobile_Forward { get; set; }

    }

    public class REC_CUST_SPLITTER
    {
        public string Cust_Non_Mobile { get; set; }
        public string Splitter_Name { get; set; }
        public string Distance { get; set; }
        public string Distance_Type { get; set; }
    }
}
