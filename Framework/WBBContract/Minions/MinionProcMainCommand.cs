using System.Collections.Generic;
using System.Xml.Serialization;

namespace WBBContract.Minions
{
    [XmlTypeAttribute(AnonymousType = true)]
    [XmlRootAttribute(ElementName = "ProfileCustomerCommand", Namespace = "", IsNullable = false)]
    public class MinionProcMainCommand
    {
        [XmlArray("Rec_Cust_Profile"), XmlArrayItem(typeof(MinionFbbCustProfileRec), ElementName = "REC_CUST_PROFILE")]
        public List<MinionFbbCustProfileRec> FbbCustProfilesList { get; set; }
        [XmlArray("Rec_Cust_Contact"), XmlArrayItem(typeof(MinionFbbCustContactRec), ElementName = "REC_CUST_CONTACT")]
        public List<MinionFbbCustContactRec> FbbCustContactList { get; set; }
        [XmlArray("Rec_Cust_Address"), XmlArrayItem(typeof(MinionFbbCustAddressRec), ElementName = "REC_CUST_ADDRESS")]
        public List<MinionFbbCustAddressRec> FbbCustAddressList { get; set; }
        [XmlArray("Rec_Cust_Package"), XmlArrayItem(typeof(MinionFbbCustPackageRec), ElementName = "REC_CUST_PACKAGE")]
        public List<MinionFbbCustPackageRec> FbbCustPackageList { get; set; }
        [XmlArray("Rec_Cust_Splitter"), XmlArrayItem(typeof(MinionFbbCustSplitterRec), ElementName = "REC_CUST_SPLITTER")]
        public List<MinionFbbCustSplitterRec> FbbCustSplitterList { get; set; }
        public int Return_Code { get; set; }
        public string Return_Desc { get; set; }

        public MinionProcMainCommand()
        {
            FbbCustProfilesList = new List<MinionFbbCustProfileRec>();
            FbbCustContactList = new List<MinionFbbCustContactRec>();
            FbbCustAddressList = new List<MinionFbbCustAddressRec>();
            FbbCustPackageList = new List<MinionFbbCustPackageRec>();
            FbbCustSplitterList = new List<MinionFbbCustSplitterRec>();
            Return_Code = -1;
            Return_Desc = "";
        }
    }

    [XmlTypeAttribute(AnonymousType = true)]
    [XmlRootAttribute(ElementName = "REC_CUST_PROFILE", Namespace = "", IsNullable = false)]
    public class MinionFbbCustProfileRec
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
        public string Service_Day { get; set; }
        public string Phone_Flag { get; set; }
        public string Time_Slot { get; set; }
        public string Access_Mode { get; set; }
        public string Address_Id { get; set; }
        public string Service_Code { get; set; }
        public string gift_voucher { get; set; }
        public string event_code { get; set; }
        public string sub_contract_name { get; set; }
        public string sub_location_id { get; set; }
        public string install_staff_id { get; set; }
        public string install_staff_name { get; set; }
        public string site_code { get; set; }
        public string flow_flag { get; set; }
        public string status { get; set; }
    }

    [XmlTypeAttribute(AnonymousType = true)]
    [XmlRootAttribute(ElementName = "REC_CUST_CONTACT", Namespace = "", IsNullable = false)]
    public class MinionFbbCustContactRec
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
        public string status { get; set; }
    }

    [XmlTypeAttribute(AnonymousType = true)]
    [XmlRootAttribute(ElementName = "REC_CUST_ADDRESS", Namespace = "", IsNullable = false)]
    public class MinionFbbCustAddressRec
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
        public string status { get; set; }
    }

    [XmlTypeAttribute(AnonymousType = true)]
    [XmlRootAttribute(ElementName = "REC_CUST_PACKAGE", Namespace = "", IsNullable = false)]
    public class MinionFbbCustPackageRec
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
        public string Pre_Recurring_Charge { get; set; }
        public string Recurring_Discount_Exp { get; set; }
        public string Recurring_Start_Dt { get; set; }
        public string Recurring_End_Dt { get; set; }
        public string Initiation_Charge { get; set; }
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
        public string status { get; set; }
    }

    [XmlTypeAttribute(AnonymousType = true)]
    [XmlRootAttribute(ElementName = "REC_CUST_SPLITTER", Namespace = "", IsNullable = false)]
    public class MinionFbbCustSplitterRec
    {
        public string Cust_Non_Mobile { get; set; }
        public string Splitter_Name { get; set; }
        public string Distance { get; set; }
        public string Distance_Type { get; set; }
        public string status { get; set; }
    }
}
