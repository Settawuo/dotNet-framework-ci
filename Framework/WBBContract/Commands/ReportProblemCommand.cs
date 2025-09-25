namespace WBBContract.Commands
{
    public class ReportProblemCommand
    {
        public decimal? ReportProblemId { get; set; }
        public string CustInternetNum { get; set; }
        public string CustIdCardType { get; set; }
        public string CustIdCardNum { get; set; }
        public string ProblemType { get; set; }
        public string ProblemTypeDes { get; set; }
        public string ProblemDetails { get; set; }
        public string ContactInfo { get; set; }
        public string ContactEmail { get; set; }
        public string ContactNumber { get; set; }
        public string ReturnCode { get; set; }
        public string ReturnDesc { get; set; }
    }
}
