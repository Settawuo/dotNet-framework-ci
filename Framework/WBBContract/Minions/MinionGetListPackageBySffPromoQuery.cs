using WBBEntity.Minions;

namespace WBBContract.Minions
{
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(ElementName = "GetPackageListBySFFPromoQuery", Namespace = "", IsNullable = false)]
    public class MinionGetListPackageBySffPromoQuery : IQuery<MinionGetListPackageBySffPromoQueryModel>
    {
        public string P_SFF_PROMOCODE { get; set; }
        public string P_PRODUCT_SUBTYPE { get; set; }
        public string P_OWNER_PRODUCT { get; set; }
    }
}
