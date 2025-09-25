using System.Collections.Generic;

namespace WBBContract.Queries.ExWebServices.Contract
{
    public class QueryAgreementProfileTypeItem
    {
        public string profileType { get; set; }
        public List<QueryAgreementContractItem> contractList { get; set; }
    }
}
