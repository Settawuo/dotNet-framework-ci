using System.Collections.Generic;
using WBBEntity.Minions;

namespace WBBContract.Minions
{
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(ElementName = "GetListProfileContactQuery", Namespace = "", IsNullable = false)]
    public class MinionGetListProfileContactQuery : IQuery<List<MinionGetListProfileContactQueryModel>>
    {
        public string Order_No { get; set; }
    }
}