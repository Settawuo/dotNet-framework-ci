using System.Xml.Serialization;

namespace WBBContract.Minions
{
    [XmlTypeAttribute(AnonymousType = true)]
    [XmlRootAttribute(ElementName = "REC_CUST_SPLITTER", Namespace = "", IsNullable = false)]
    public class MinionProcInstallSplitterCommand
    {
        public string Cust_Non_Mobile { get; set; }
        public string Splitter_Seq { get; set; }
        public string Splitter_Name { get; set; }
        public string Distance { get; set; }
        public string Distance_Type { get; set; }

        [SkipProperty]
        public int Return_Code { get; set; }
        [SkipProperty]
        public string Return_Desc { get; set; }
    }
}
