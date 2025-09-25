using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace WBBContract.Queries.ExWebServices.Contract
{
    [DataContract]
    public class GetQueryAgreementContractFbbRequest : IQuery<List<GetQueryAgreementContractFbbResult>>
    {
        [DataMember(Order = 0)]
        public string transactionId { get; set; }
        [DataMember(Order = 1)]
        public string option { get; set; }
        [DataMember(Order = 2)]
        public string fibrenetId { get; set; }
        [DataMember(Order = 3)]
        public string idCardNo { get; set; }
        [DataMember(Order = 4)]
        public string profileType { get; set; }
        [DataMember(Order = 5)]
        public string contractNo { get; set; }
        [DataMember(Order = 6)]
        public string sourceSystem { get; set; }
    }

    public class GetQueryAgreementContractFbbResult
    {
        public string errorMessage { get; set; }
        public List<QueryAgreementProfileTypeItem> profileTypeList { get; set; }

        public bool IsSuccess()
        {
            return profileTypeList != null && profileTypeList?.Any() == true;
        }
    }
}
