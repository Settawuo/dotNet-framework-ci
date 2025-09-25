using System.Collections.Generic;

namespace WBBEntity.PanelModels.WebServiceModels
{
    public class ChangePackageBundlingData
    {
        public string return_code { get; set; }
        public string return_message { get; set; }

        private List<ChangePackageBundlingList> _ChangePackageBundlingList;
        public List<ChangePackageBundlingList> ChangePackageBundlingList
        {
            get { return _ChangePackageBundlingList; }
            set { _ChangePackageBundlingList = value; }
        }
    }

    public class ChangePackageBundlingList
    {
        public string orderType { get; set; }
        public string mobileNo { get; set; }
        public string orderReson { get; set; }
        public string orderChannel { get; set; }
        public string userName { get; set; }
        public string chargeFeeFlag { get; set; }
        public string ascCode { get; set; }
        public string locationCd { get; set; }
        public string club900Mobile { get; set; }

        public string promotionCode { get; set; }
        public string actionStatus { get; set; }
        public string productSeq { get; set; }
        public string promotionStartDt { get; set; }
        public string overRuleStartDate { get; set; }
        public string waiveFlag { get; set; }
        public string chargeType { get; set; }
        public string orderItemReason { get; set; }
        public string promotionClass { get; set; }

        public string promotionCode_1 { get; set; }
        public string actionStatus_1 { get; set; }
        public string productSeq_1 { get; set; }
        public string promotionStartDt_1 { get; set; }
        public string overRuleStartDate_1 { get; set; }
        public string waiveFlag_1 { get; set; }
        public string chargeType_1 { get; set; }
        public string orderItemReason_1 { get; set; }
        public string promotionClass_1 { get; set; }

        public string projectName { get; set; }
        public string actionRelateMobile { get; set; }
        public string relateMobileNo { get; set; }
        public string oldRelateMobile { get; set; }
        public string referenceNo { get; set; }
        public string sourceSystem { get; set; }
        public string mobileNumberContact { get; set; }
    }
}
