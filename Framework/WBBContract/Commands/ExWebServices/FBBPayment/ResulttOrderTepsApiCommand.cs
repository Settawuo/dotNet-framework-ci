using System.Xml.Serialization;

namespace WBBContract.Commands.ExWebServices.FBBPayment
{
    [XmlRoot(ElementName = "response")]
    public class ResulttOrderTepsApiCommand
    {
        public string status { get; set; }
        public string respCode { get; set; }
        public string respDesc { get; set; }
        public string endPointUrl { get; set; }
        public string saleId { get; set; }
        public string detail1 { get; set; }
        public string detail2 { get; set; }
        public string detail3 { get; set; }
    }
}
