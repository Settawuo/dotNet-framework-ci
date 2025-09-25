using System.Collections.Generic;

namespace WBBEntity.PanelModels.WebServiceModels
{
    public class RelatedMobileServiceATNModel
    {
        public string returnCode { get; set; }
        public string returnMessage { get; set; }
        public string returnRelated { get; set; }
        public List<resultDataATN> resultData { get; set; }
    }

    public class resultDataATN
    {
        public string BaNo { get; set; }
        public string MobileNo { get; set; } // Best MobileNo 0000000000
        public string MobileNoDisplay { get; set; } // Best MobileNo 000-XXX-0000
        public string MobileNoGroup { get; set; } // 0000000000, 0000000001
        public string MobileNoGroupDisplay { get; set; } // 000-XXX-0000, 000-XXX-0001
        public string HomeId { get; set; }
        public string Moo { get; set; }
        public string Mooban { get; set; }
        public string Building { get; set; }
        public string Floor { get; set; }
        public string Room { get; set; }
        public string Soi { get; set; }
        public string Street { get; set; }
        public string Tumbon { get; set; }
        public string Ampur { get; set; }
        public string Province { get; set; }
        public string ZipCode { get; set; }

        //Bill Convert from Lov
        public string ChannelViewBill { get; set; }
        public string BillCycleInfo { get; set; }

        public string BillCycle { get; set; }
        public string BillMedia { get; set; }
    }

    #region MockUp Test
    //public class MockUpServiceATNIntraModel
    //{
    //    public string resultCode { get; set; }
    //    public string resultDescription { get; set; }
    //    public string developerMessage { get; set; }
    //    public MockUpResultData resultData { get; set; }
    //}

    //public class MockUpResultData
    //{
    //    public MockUpSubscriptionAccount subscriptionAccount { get; set; }
    //    public dynamic nafaProfile { get; set; }
    //}

    //public class MockUpSubscriptionAccount
    //{
    //    public string msisdn { get; set; }
    //    public string subscriptionState { get; set; }
    //    public string subscriptionStateDate { get; set; }
    //    public List<customerAccount> customerAccount { get; set; }
    //    public List<dynamic> billingAccount { get; set; }
    //    public List<dynamic> serviceAccount { get; set; }
    //    public List<subscriptionHolder> subscriptionHolder { get; set; }
    //}

    //public class customerAccount
    //{
    //    public string accountState { get; set; }
    //    public string accountStateDate { get; set; }
    //    public string accountSegment { get; set; }
    //    public string accountCategory { get; set; }
    //    public string accountSubCategory { get; set; }
    //    public string accountGroupCode { get; set; }
    //    public string accountGroupName { get; set; }
    //    public string accountSpecialGroup { get; set; }
    //    public string title { get; set; }
    //    public string customerName { get; set; }
    //    public string idCardNo { get; set; }
    //    public string idCardType { get; set; }
    //    public string birthday { get; set; }
    //    public string email { get; set; }
    //    public List<address> address { get; set; }
    //    public string caId { get; set; }
    //    public string nationality { get; set; }
    //    public string billCycle { get; set; }
    //    public string blacklistStatus { get; set; }
    //    public string serviceLevel { get; set; }
    //    public string gender { get; set; }
    //    public string cardIssueDate { get; set; }
    //    public string cardExpired { get; set; }
    //    public List<dynamic> hobby { get; set; }
    //    public string titleEng { get; set; }
    //    public string customerNameEng { get; set; }
    //    public string serviceSubtype { get; set; }
    //    public string titleCode { get; set; }
    //    public string creditLimit { get; set; }
    //    public string idCardTypeDesc { get; set; }
    //    public string idCardTypeNo { get; set; }
    //    public string urlPicture { get; set; }
    //    public string rsmeFlag { get; set; }
    //}

    //public class address
    //{
    //    public string engFlag { get; set; }
    //    public string houseNo { get; set; }
    //    public string moo { get; set; }
    //    public string mooban { get; set; }
    //    public string building { get; set; }
    //    public string floor { get; set; }
    //    public string room { get; set; }
    //    public string soi { get; set; }
    //    public string street { get; set; }
    //    public string amphur { get; set; }
    //    public string tumbol { get; set; }
    //    public string province { get; set; }
    //    public string zipCode { get; set; }
    //}

    //public class subscriptionHolder
    //{
    //    public string birthday { get; set; }
    //    public string contactClass { get; set; }
    //    public string province { get; set; }
    //    public string title { get; set; }
    //    public string contactName { get; set; }
    //    public string contactFirstname { get; set; }
    //    public string contactLastname { get; set; }
    //    public string idCardType { get; set; }
    //    public string idCardNo { get; set; }
    //    public string privateCode { get; set; }
    //    public string privateCodeFlag { get; set; }
    //    public List<dynamic> socialMedia { get; set; }
    //    public string nationality { get; set; }
    //    public string localLanguage { get; set; }
    //    public string urlPicture { get; set; }
    //    public string urlDate { get; set; }
    //    public string contactId { get; set; }
    //    public List<dynamic> contactPhone { get; set; }
    //}

    #endregion MockUp Test


    //Reality
    public class ServiceATNIntraModel
    {
        public string resultCode { get; set; }
        public string resultDescription { get; set; }
        public string developerMessage { get; set; }
        public serviceResultData resultData { get; set; }
    }

    public class serviceResultData
    {
        public List<listDefaultBa> listDefaultBa { get; set; }
    }

    public class listDefaultBa
    {
        public string baNo { get; set; }
        public string billCycle { get; set; }
        public string billDisplay { get; set; }
        public string creditLimit { get; set; }
        public string paymentMethod { get; set; }
        public string billMedia { get; set; }
        public string billStyle { get; set; }
        public List<itemized> itemized { get; set; }
        public List<productPackage> productPackage { get; set; }
        public List<billAddress> billAddress { get; set; }
        public List<mobile> mobile { get; set; }
        public string negoFlag { get; set; }
        public string defaultBaFlag { get; set; }
    }

    public class itemized
    {
        public string gprsFlag { get; set; }
        public string localFlag { get; set; }
        public string mediaFlag { get; set; }
        public string nrFlag { get; set; }
        public string smsFlag { get; set; }
        public string startFlag { get; set; }
        public string endFlag { get; set; }
        public string transFlag { get; set; }
        public string vasFlag { get; set; }
        public string waiveFlag { get; set; }
    }

    public class productPackage
    {
        public string productPkg { get; set; }
        public string productCd { get; set; }
        public string productName { get; set; }
        public string productGroup { get; set; }
    }

    public class billAddress
    {
        public string engFlag { get; set; }
        public string houseNo { get; set; }
        public string moo { get; set; }
        public string mooban { get; set; }
        public string building { get; set; }
        public string floor { get; set; }
        public string room { get; set; }
        public string soi { get; set; }
        public string street { get; set; }
        public string amphur { get; set; }
        public string tumbol { get; set; }
        public string zipCode { get; set; }
        public string province { get; set; }
    }

    public class mobile
    {
        public string mobileNo { get; set; }
        public string mobileStatus { get; set; }
        public string mobileRegisterDate { get; set; }
        public string mobileSegment { get; set; }
        public List<serviceYear> serviceYear { get; set; }
    }

    public class serviceYear
    {
        public string year { get; set; }
        public string month { get; set; }
        public string day { get; set; }
    }
}
