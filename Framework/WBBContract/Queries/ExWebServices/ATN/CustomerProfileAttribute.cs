using System.Collections.Generic;

namespace WBBContract.Queries.ExWebServices.ATN
{
    public class CustomerProfileAttribute
    {
        public string propertyCode { get; set; }
        public string propertyType { get; set; }
        public string propertyValue { get; set; }
        public List<CustomerProfileAttributeSubInst> productSubInstProperty { get; set; }
        public string productInstEffectiveTime { get; set; }
        public string productInstExpireTime { get; set; }
    }

    public class CustomerProfileAttributeSubInst
    {
        public string subPropertyCode { get; set; }
        public string subPropertyValue { get; set; }
    }
}
