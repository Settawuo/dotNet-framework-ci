using System.Xml.Serialization;
namespace WBBContract.Minions
{
    [XmlTypeAttribute(AnonymousType = true)]
    [XmlRootAttribute(ElementName = "REC_CUST_ADDRESS", Namespace = "", IsNullable = false)]
    public class MinionProcInstallAddressCommand
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

        [SkipProperty]
        public int Return_Code { get; set; }
        [SkipProperty]
        public string Return_Desc { get; set; }
        [SkipProperty]
        public string Return_Address_RowId { get; set; }

    }

}
