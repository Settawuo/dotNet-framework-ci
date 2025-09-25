using System.Collections.Generic;
using System.Runtime.Serialization;

namespace WBBContract.Queries.ExWebServices.ATN
{
    [DataContract]
    public class GetCustomerProfileSubScriptionAccountRequest : IQuery<GetCustomerProfileSubScriptionAccountResult>
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
        public string status { get; set; }
        [DataMember(Order = 5)]
        public string filter { get; set; }
    }

    public class GetCustomerProfileSubScriptionAccountResult : ResultBaseATN
    {
        public GetCustomerProfileSubScriptionAccountData resultData { get; set; }
    }
    public class GetCustomerProfileSubScriptionAccountData
    {
        public GetCustomerProfileSubscriptionAccountItem subscriptionAccount { get; set; }
        public CustomerProfileNafaProfile nafaProfile { get; set; }
    }

    public class GetCustomerProfileSubscriptionAccountItem
    {
        public string msisdn { get; set; }
        public string subscriptionState { get; set; }
        public string subscriptionStateDate { get; set; }
        public List<CustomerProfileCustomerAccount> customerAccount { get; set; }
        public List<CustomerProfileBillingAccount> billingAccount { get; set; }
        public List<CustomerProfileServiceAccount> serviceAccount { get; set; }
        public List<CustomerProfileSubscriptionHolder> subscriptionHolder { get; set; }
    }
}
