using System.Collections.Generic;
using WBBEntity.Minions;

namespace WBBContract.Minions
{
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(ElementName = "GetListProfileAddressQuery", Namespace = "", IsNullable = false)]
    public class MinionGetListProfileAddressQuery : IQuery<List<MinionGetListProfileAddressQueryModel>>
    {
        public string Order_No { get; set; }
    }
}
