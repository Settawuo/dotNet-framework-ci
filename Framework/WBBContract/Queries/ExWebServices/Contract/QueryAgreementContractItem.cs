using System.Collections.Generic;

namespace WBBContract.Queries.ExWebServices.Contract
{
    public class QueryAgreementContractItem
    {
        public string idCard { get; set; }
        public string countContract { get; set; }
        public string countContractExc { get; set; }
        public string countContractProfileId { get; set; }
        public string countContractProfileIdExc { get; set; }
        public string countContractMobile { get; set; }
        public string countContractMobileExc { get; set; }
        public List<QueryAgreementContractDetailItem> contractDetailList { get; set; }
    }
}
