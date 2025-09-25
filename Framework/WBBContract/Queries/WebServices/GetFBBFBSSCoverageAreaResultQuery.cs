using WBBContract.Commands;

namespace WBBContract.Queries.WebServices
{
    public class GetFBBFBSSCoverageAreaResultQuery : IQuery<GetLeaveMsgReferenceNoCommand>
    {
        public decimal RESULTID { get; set; }
        public string TRANSACTIONID { get; set; }
        public string ASSET_NUMBER { get; set; }
        public string CASE_ID { get; set; }
        public string REFERENCE_NO_STATUS { get; set; }
    }
}
