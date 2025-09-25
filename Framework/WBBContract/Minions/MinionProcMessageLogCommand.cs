using System.Xml.Serialization;

namespace WBBContract.Minions
{
    [XmlType(AnonymousType = true)]
    [XmlRoot(ElementName = "REC_MESSAGE_LOG", Namespace = "", IsNullable = false)]
    public class MinionProcMessageLogCommand
    {
        public string Cust_Non_Mobile { get; set; }
        public string Process_Name { get; set; }
        public string Create_User { get; set; }
        public string Create_Date { get; set; }
        public string Return_Code { get; set; }
        public string Return_Desc { get; set; }
        public string Remark { get; set; }

        [SkipProperty]
        public int Response_Code { get; set; }
        [SkipProperty]
        public string Response_Message { get; set; }
    }
}
