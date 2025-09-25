using System.Xml.Serialization;

namespace WBBContract.Minions
{
    [XmlType(AnonymousType = true)]
    [XmlRoot(ElementName = "REC_CUST_PACKAGE", Namespace = "", IsNullable = false)]
    public class MinionProcInstallPackageCommand
    {
        public string Cust_Non_Mobile { get; set; }
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
        public string Mobile_Forward { get; set; }
        [SkipProperty]
        public int Return_Code { get; set; }
        [SkipProperty]
        public string Return_Desc { get; set; }
    }
}
