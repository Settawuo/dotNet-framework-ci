using System.Collections.Generic;

namespace WBBContract.Queries.ExWebServices.ATN
{
    public class CustomerProfileSubscriptionHolder
    {
        public string birthday { get; set; }
        public string contactClass { get; set; }
        public string province { get; set; }
        public string title { get; set; }
        public string contactName { get; set; }
        public string contactFirstname { get; set; }
        public string contactLastname { get; set; }
        public string idCardNo { get; set; }
        public string privateCode { get; set; }
        public string privateCodeFlag { get; set; }
        //public string contactPhone { get; set; }//duplicate with list below
        public List<CustomerProfileCustomerSocialMedia> socialMedia { get; set; }
        public string nationality { get; set; }
        public string localLanguage { get; set; }
        public string urlPicture { get; set; }
        public string urlDate { get; set; }
        public string contactId { get; set; }
        public List<CustomerProfileCustomerContactPhone> contactPhone { get; set; }
    }
}
