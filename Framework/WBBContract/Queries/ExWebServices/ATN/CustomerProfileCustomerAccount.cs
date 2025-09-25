using System.Collections.Generic;

namespace WBBContract.Queries.ExWebServices.ATN
{
    public class CustomerProfileCustomerAccount
    {
        public string accountState { get; set; }
        public string accountStateDate { get; set; }
        public string accountSegment { get; set; }
        public string accountCategory { get; set; }
        public string accountSubCategory { get; set; }
        public string accountGroupCode { get; set; }
        public string accountGroupName { get; set; }
        public string accountSpecialGroup { get; set; }
        public string title { get; set; }
        public string customerName { get; set; }
        public string idCardNo { get; set; }
        public string idCardType { get; set; }
        public string idCardTypeDesc { get; set; }
        public string idCardTypeNo { get; set; }
        public string birthday { get; set; }
        public string email { get; set; }
        public List<CustomerProfileCustomerAddress> address { get; set; }
        public string caId { get; set; }
        public string nationality { get; set; }
        public string billCycle { get; set; }
        public string blacklistStatus { get; set; }
        public string serviceLevel { get; set; }
        public string gender { get; set; }
        public string cardIssueDate { get; set; }
        public string cardExpired { get; set; }
        public List<CustomerProfileCustomerHobby> hobby { get; set; }
        public string titleEng { get; set; }
        public string customerNameEng { get; set; }
        public string serviceSubtype { get; set; }
        public string titleCode { get; set; }
        public string creditLimit { get; set; }
        public string urlPicture { get; set; }
        public string rsmeFlag { get; set; }
    }
}
