using Newtonsoft.Json;
using System.Linq;

namespace WBBContract.Queries.ExWebServices.ATN
{
    public class ResultBaseATN
    {
        public static readonly string[] DefSuccessCodes = new string[] { "20000", "20001" };

        [JsonProperty(Order = -5)]
        public string resultCode { get; set; }
        [JsonProperty(Order = -4)]
        public string resultDescription { get; set; }
        [JsonProperty(Order = -3)]
        public string developerMessage { get; set; }

        public bool IsSuccess(string[] successCodes = null)
        {
            successCodes = successCodes ?? DefSuccessCodes;
            var success = resultCode != null && successCodes.Contains(resultCode);
            return success;
        }
    }
}
