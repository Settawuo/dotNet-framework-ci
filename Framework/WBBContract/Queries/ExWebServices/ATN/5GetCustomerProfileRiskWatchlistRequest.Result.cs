using System.Runtime.Serialization;

namespace WBBContract.Queries.ExWebServices.ATN
{
    [DataContract]
    public class GetCustomerProfileRiskWatchlistRequest : IQuery<GetCustomerProfileRiskWatchlistResult>
    {
        [DataMember(Order = 0)]
        public string transactionId { get; set; }
        [DataMember(Order = 1)]
        public string key_name { get; set; }
        [DataMember(Order = 2)]
        public string key_value { get; set; }
    }

    public class GetCustomerProfileRiskWatchlistResult : ResultBaseATN
    {
        public GetCustomerProfileRiskWatchlistResultData resultData { get; set; }
    }

    public class GetCustomerProfileRiskWatchlistResultData
    {
        public GetCustomerProfileRiskWatchlistResultStatus customerRisk { get; set; }
    }

    public class GetCustomerProfileRiskWatchlistResultStatus
    {
        public string watchlistStatus { get; set; }
        public string watchlistCode { get; set; }
    }
}
