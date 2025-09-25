using WBBEntity.Minions;

namespace WBBContract.Minions
{
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(ElementName = "GetListPackageByServiceQuery", Namespace = "", IsNullable = false)]
    public class MinionGetListPackageByServiceQuery : IQuery<MinionGetListPackageByServiceQueryModel>
    {
        public string P_OWNER_PRODUCT { get; set; }
        public string P_PRODUCT_SUBTYPE { get; set; }
        public string P_NETWORK_TYPE { get; set; }
        public string P_SERVICE_DAY { get; set; }
        public string P_PACKAGE_FOR { get; set; }
        public string P_PACKAGE_CODE { get; set; }
        public string PartnerName { get; set; }
        public string P_Location_Code { get; set; }
        public string P_Asc_Code { get; set; }
        public string P_Partner_Type { get; set; }
        public string P_Partner_SubType { get; set; }
        public string P_Region { get; set; }
        public string P_Province { get; set; }
        public string P_District { get; set; }
        public string P_Sub_District { get; set; }
        public string P_Address_Type { get; set; }
        public string P_Building_Name { get; set; }
        public string P_Building_No { get; set; }
        public string P_Serenade_Flag { get; set; }
        public string P_Customer_Type { get; set; }
        public string P_Address_Id { get; set; }
        public string P_Plug_And_Play_Flag { get; set; }
        public string P_rental_flag { get; set; }
        public string P_customer_subtype { get; set; }
    }
}
