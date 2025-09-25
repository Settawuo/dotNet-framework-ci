using Newtonsoft.Json;

namespace WBBContract.Queries.ExWebServices.ATN
{
    public class CustomerProfileNafaProfile
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string msisdn { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string firstName { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string lastName { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string email { get; set; }
    }
}
