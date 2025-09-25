using System.Xml.Serialization;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class WsLdapQueryIdQuery : IQuery<WsLdapQueryIdModel>
    {
        [XmlIgnore]
        public string Username { get; set; }
        [XmlIgnore]
        public string TransactionId { get; set; }
        [XmlIgnore]
        public string FullUrl { get; set; }

        public AisEmployeeUser AisEmployeeUser { get; set; }
        public ProjectQuery ProjectQuery { get; set; }

    }

    public class AisEmployeeUser
    {
        public string Username { get; set; }
    }

    public class ProjectQuery
    {
        public string ProjectCode { get; set; }
        public int ProjectLevel { get; set; }
    }


}
