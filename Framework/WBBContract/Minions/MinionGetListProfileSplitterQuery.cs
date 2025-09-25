using System.Collections.Generic;
using WBBEntity.Minions;

namespace WBBContract.Minions
{
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(ElementName = "GetListProfileSplitterQuery", Namespace = "", IsNullable = false)]
    public class MinionGetListProfileSplitterQuery : IQuery<List<MinionGetListProfileSplitterQueryModel>>
    {
        public string Order_No { get; set; }
    }
}
