using System.Collections.Generic;

namespace WBBContract.Queries.ExWebServices.ATN
{
    public class CustomerProfileBillingAccount
    {
        public string accountState { get; set; }
        public string accountStateDate { get; set; }
        public string accountSpecialGroup { get; set; }
        public string customerName { get; set; }
        public string billCycle { get; set; }
        public string paymentMethod { get; set; }
        public string blacklistStatus { get; set; }
        public string smsContactNo { get; set; }
        public string billMedia { get; set; }
        public string smsBillTo { get; set; }
        public string emailBillTo { get; set; }
        public string billAccountName { get; set; }
        public string billLanguage { get; set; }
        public string creditLimit { get; set; }
        public string minCreditLimit { get; set; }
        public string maxCreditLimit { get; set; }
        public string scoreRange { get; set; }
        public string mainMobile { get; set; }
        public string billDisplay { get; set; }
        public string creditCardNo { get; set; }
        public string creditCardName { get; set; }
        public string creditCardType { get; set; }
        public string creditCardBankCd { get; set; }
        public string creditCardExpMonth { get; set; }
        public string creditCardExpYear { get; set; }
        public string creditCardRefID { get; set; }
        public string bankNameCd { get; set; }
        public string bankAccntNumber { get; set; }
        public string bankName { get; set; }
        public string wtReqFlg { get; set; }
        public string wtReqDt { get; set; }
        public string title { get; set; }
        public List<CustomerProfileCustomerAddress> address { get; set; }
    }
}
