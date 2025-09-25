using System.Collections.Generic;

namespace WBBWeb.Models
{

    public class CheckCoverageSpecialRequest
    {
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string mobileno { get; set; }
        public string buildingAddressID { get; set; }
    }

    public class CheckCoverageSpecialResponse
    {
        public string returnCode { get; set; }
        public string returnMessage { get; set; }
        public string coverage { get; set; }
        public List<CheckCoverage3bbSplitter> splitterList { get; set; }
        public string splitterJson { get; set; }
        public int splitterCount { get; set; }
        public string inServiceDate { get; set; }
    }

    public class CheckCoverage3bbSplitter
    {
        public string splitterCode { get; set; }
        public string splitterAlias { get; set; }
        public string distance { get; set; }
        public string splitterPort { get; set; }
        public string splitterLatitude { get; set; }
        public string splitterLongitude { get; set; }
    }

    public class CheckCoverageForWorkflowRequest
    {
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string transactionId { get; set; }
    }

    public class CheckCoverageForWorkflowResponse
    {
        public string returnCode { get; set; }
        public string returnMessage { get; set; }
        public string coverage { get; set; }
        public List<CheckCoverageForWorkflowSplitter> splitterList { get; set; }
        public string inServiceDate { get; set; }
    }

    public class CheckCoverageForWorkflowSplitter
    {
        public string distance { get; set; }
        public string splitterAlias { get; set; }
        public string splitterCode { get; set; }
        public string splitterLatitude { get; set; }
        public string splitterLongitude { get; set; }
        public string splitterPort { get; set; }
    }
}
