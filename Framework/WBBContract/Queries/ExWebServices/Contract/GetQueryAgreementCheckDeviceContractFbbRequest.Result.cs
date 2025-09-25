using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace WBBContract.Queries.ExWebServices.Contract
{
    [DataContract]
    public class GetQueryAgreementCheckDeviceContractFbbRequest : IQuery<GetQueryAgreementCheckDeviceContractFbbResult>
    {
        [DataMember(Order = 0)]
        public string transactionId { get; set; }
        [DataMember(Order = 1)]
        public string idCardNo { get; set; }
        [DataMember(Order = 2)]
        public string fibrenetId { get; set; }
        [DataMember(Order = 3)]
        public string idCardType { get; set; }
        [DataMember(Order = 4)]
        public string contractProfileFlg { get; set; }
        [DataMember(Order = 5)]
        public string sourceSystem { get; set; }
    }

    public class GetQueryAgreementCheckDeviceContractFbbResult
    {
        public string countContractFbb { get; set; }
        public string contractExpireDtFbb { get; set; }
        public string contractFlagFbb { get; set; }
        public string sameFbbNumber { get; set; }
        public string returnCode { get; set; }
        public string errorMessage { get; set; }
        public string fbbLimitContract { get; set; }
        public string contractProfileCountFbb { get; set; }
        public string idCardNo { get; set; }
        public string resultFlag { get; set; }
        public List<GetQueryAgreementCheckDeviceContractFbbItem> contractList { get; set; }

        public bool IsSuccess()
        {
            return contractList != null && contractList?.Any() == true;
        }
    }

    public class GetQueryAgreementCheckDeviceContractFbbItem
    {
        public string mobileNo { get; set; }
        public string projectName { get; set; }
        public string brand { get; set; }
        public string model { get; set; }
        public string imei { get; set; }
        public string receiptNo { get; set; }
        public string receiptDt { get; set; }
        public string expireDt { get; set; }
        public List<QueryAgreementContractRuleItem> contractRuleList { get; set; }
    }
}
