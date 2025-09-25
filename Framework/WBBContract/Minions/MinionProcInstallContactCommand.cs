using System.Xml.Serialization;

namespace WBBContract.Minions
{
    [XmlTypeAttribute(AnonymousType = true)]
    [XmlRootAttribute(ElementName = "REC_CUST_CONTACT", Namespace = "", IsNullable = false)]
    public class MinionProcInstallContactCommand
    {
        public string ba_id { get; set; }
        public string Contact_Seq { get; set; }
        public string Contact_Addr_id { get; set; }
        public string Contact_First_Name { get; set; }
        public string Contact_Last_Name { get; set; }
        public string Contact_Home_Phone { get; set; }
        public string Contact_Mobile_Phone1 { get; set; }
        public string Contact_Mobile_Phone2 { get; set; }
        public string Contact_Email { get; set; }

        [SkipProperty]
        public int Return_Code { get; set; }
        [SkipProperty]
        public string Return_Desc { get; set; }
    }


}
