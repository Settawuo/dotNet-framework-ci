using System.Collections.Generic;
using WBBEntity.Minions;

namespace WBBContract.Minions
{
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(ElementName = "GetListProfileCustomerQuery", Namespace = "", IsNullable = false)]
    public class MinionGetListProfileCustomerQuery : IQuery<List<MinionGetListProfileCustomerQueryModel>>
    {
        public string Order_No { get; set; }
    }
}
