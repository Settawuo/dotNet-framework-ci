using WBBEntity.Minions;

namespace WBBContract.Minions
{
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(ElementName = "GetListPackageChangeQuery", Namespace = "", IsNullable = false)]
    public class MinionGetListPackageByChangeQuery : IQuery<MinionGetListPackageByChangeQueryModel>
    {
        public string non_mobile_no { get; set; }
        public string owner_product { get; set; }
        public string package_for { get; set; }
        public string serenade_flag { get; set; }
        public string ref_row_id { get; set; }
        public string customer_type { get; set; }
        public string address_id { get; set; }
    }
}
