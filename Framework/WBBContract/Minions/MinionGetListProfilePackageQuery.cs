
using System.Collections.Generic;
using WBBEntity.Minions;

namespace WBBContract.Minions
{
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(ElementName = "GetListProfilePackageQuery", Namespace = "", IsNullable = false)]
    public class MinionGetListProfilePackageQuery : IQuery<List<MinionGetListProfilePackageQueryModel>>
    {
        public string Order_No { get; set; }
    }
}
