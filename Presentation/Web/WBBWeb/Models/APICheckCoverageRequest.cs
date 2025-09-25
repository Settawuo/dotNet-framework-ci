using System.Collections.Generic;

namespace WBBWeb.Models
{
    public class APICheckCoverageRequest
    {
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string buildingAddressID { get; set; }
        public string transactionId { get; set; }

    }

    public class APICheckCoverageResult
    {
        public string returnCode { get; set; }
        public string returnMessage { get; set; }
        public string coverage { get; set; }
        public List<APICheckCoverageSplitterResult> splitterList { get; set; }
        public string inServiceDate { get; set; }
    }

    public class APICheckCoverageSplitterResult
    {
        public string distance { get; set; }
        public string splitterAlias { get; set; }
        public string splitterCode { get; set; }
        public string splitterLatitude { get; set; }
        public string splitterLongitude { get; set; }
        public string splitterPort { get; set; }
    }
}