using System.Collections.Generic;

namespace WBBContract.Queries.ExWebServices.Contract
{
    public class QueryAgreementContractDetailItem
    {
        public string mobileNo { get; set; }
        public string mobileStatus { get; set; }
        public string projectName { get; set; }
        public string contractName { get; set; }
        public string brand { get; set; }
        public string model { get; set; }
        public string color { get; set; }
        public string imei { get; set; }
        public string receiptNo { get; set; }
        public string receiptDt { get; set; }
        public string status { get; set; }
        public string startDt { get; set; }
        public string endDt { get; set; }
        public string duration { get; set; }
        public string remain { get; set; }
        public string penalty { get; set; }
        public string ussdCode { get; set; }
        public string contractNo { get; set; }
        public string contractId { get; set; }
        public string lastUpdate { get; set; }
        public string created { get; set; }
        public string discountContract { get; set; }
        public string receiptLocationCd { get; set; }
        public string salesChannel { get; set; }
        public string statusDisplay { get; set; }
        public string countFlag { get; set; }
        public string description { get; set; }
        public string contractReason { get; set; }
        public string receiptLocationName { get; set; }
        public string simUnlock { get; set; }
        public string breakDt { get; set; }
        public string disconnectDt { get; set; }
        public string breakBy { get; set; }
        public string empowermentBy { get; set; }
        public string breakReason { get; set; }
        public string breakLocationId { get; set; }
        public string breakLocationName { get; set; }
        public string breakDesc { get; set; }
        public List<QueryAgreementContractHistoryItem> contractHistoryList { get; set; }
        public List<QueryAgreementContractRuleItem> contractRuleList { get; set; }
    }
}
