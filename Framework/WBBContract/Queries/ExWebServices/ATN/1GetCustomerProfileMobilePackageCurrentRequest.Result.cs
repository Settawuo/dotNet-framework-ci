using System.Collections.Generic;
using System.Runtime.Serialization;

namespace WBBContract.Queries.ExWebServices.ATN
{

    [DataContract]
    public class GetCustomerProfileMobilePackageCurrentRequest : IQuery<GetCustomerProfileMobilePackageCurrentResult>
    {
        [DataMember(Order = 0)]
        public string transactionId { get; set; }
        [DataMember(Order = 1)]
        public string channel { get; set; }
        [DataMember(Order = 2)]
        public string username { get; set; }
        [DataMember(Order = 3)]
        public string msisdn { get; set; }
        [DataMember(Order = 4)]
        public string filter { get; set; }
    }

    public class GetCustomerProfileMobilePackageCurrentResult : ResultBaseATN
    {
        public GetCustomerProfileMobilePackageCurrentData resultData { get; set; }
    }

    public class GetCustomerProfileMobilePackageCurrentData
    {
        public GetCustomerProfileMobilePackageCurrentItem mobilePackageCurrent { get; set; }
    }

    public class GetCustomerProfileMobilePackageCurrentItem
    {
        public CustomerProfileMobilePackageCurrentMain main { get; set; }
        public List<CustomerProfileMobilePackageCurrentOntop> ontop { get; set; }
    }
}
