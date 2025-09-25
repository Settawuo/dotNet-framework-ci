using System.Collections.Generic;
using System.Runtime.Serialization;

namespace WBBContract.Queries.ExWebServices.ATN
{
    [DataContract]
    public class GetCustomerProfileServiceProfileRequest : IQuery<GetCustomerProfileServiceProfileResult>
    {
        [DataMember(Order = 0)]
        public string transactionId { get; set; }
        [DataMember(Order = 1)]
        public string channel { get; set; }
        [DataMember(Order = 2)]
        public string username { get; set; }
        [DataMember(Order = 3)]
        public string msisdn { get; set; }
    }

    public class GetCustomerProfileServiceProfileResult : ResultBaseATN
    {
        public GetCustomerProfileServiceProfileData resultData { get; set; }
    }

    public class GetCustomerProfileServiceProfileData
    {
        public List<GetCustomerProfileServiceProfileItem> serviceProfile { get; set; }
        public string serviceIddStatus { get; set; }
    }

    public class GetCustomerProfileServiceProfileItem
    {
        public string cbsProductId { get; set; }
        public string cbsProductSequenceId { get; set; }
        public string cbsProductName { get; set; }
        public string cbsNotificationNameEng { get; set; }
        public string cbsNotificationNameThai { get; set; }
        public string cbsNotificationNameAseanLang { get; set; }
        public string cbsProductEffectiveTime { get; set; }
        public string cbsProductExpireTime { get; set; }
        public string cbsProductActivationTime { get; set; }
        public string cbsNextBillDate { get; set; }
        public string extProductStatus { get; set; }
        public string extProductId { get; set; }
        public string extProductSequenceId { get; set; }
        public string extProductName { get; set; }
        public string extNotificationNameEng { get; set; }
        public string extNotificationNameThai { get; set; }
        public string extNotificationNameAseanLang { get; set; }
        public string extProductEffectiveTime { get; set; }
        public string extProductExpireTime { get; set; }
        public string extProductActivationTime { get; set; }
        public string extNextBillDate { get; set; }
        public string name { get; set; }
        public string shortenedNameTH { get; set; }
        public string shortenedNameEN { get; set; }
        public string descriptionTH { get; set; }
        public string descriptionEN { get; set; }
        public string billItemDescriptionTH { get; set; }
        public string billItemDescriptionEN { get; set; }
        public string serviceName { get; set; }
        public string startDateTime { get; set; }
        public string endDateTime { get; set; }
        public string featureCode { get; set; }
        public string productCd { get; set; }
        public string productClass { get; set; }
        public List<CustomerProfileAttribute> attribute { get; set; }
        public string productOfferPriceId { get; set; }
        public string groupFeature { get; set; }
        public string featureName { get; set; }
        public string moreInfoFlag { get; set; }
        public string priceType { get; set; }
        public string offeringName { get; set; }
        public string freeFeatureName { get; set; }
        public string IntegrationNameTH { get; set; }
        public string IntegrationNameEN { get; set; }
        public string serviceStatus { get; set; }
        public string serviceStatusDate { get; set; }
        public string priceIncludeVat { get; set; }
        public string ivrCancelFlag { get; set; }
        public string ivrQueryFlag { get; set; }
        public string commercialType { get; set; }
        public string productId { get; set; }
        public string productSequenceId { get; set; }
        public string productName { get; set; }
        public string holdFlag { get; set; }
        public string productPackage { get; set; }
        public string offeringGroup { get; set; }
        public string technology { get; set; }
        public string installAddress { get; set; }
        public string createUser { get; set; }
        public string createDate { get; set; }
        public string duration { get; set; }
        public string durationType { get; set; }
        public string chargeAmount { get; set; }
        public string openDate { get; set; }
        public string priceExcludeVat { get; set; }
        public string pro5gflg { get; set; }
        public string cbsProductStatus { get; set; }
        public string crmFlg { get; set; }
        public string netFlexiFl { get; set; }
        public string paymentMode { get; set; }
        public string productAcctnCat { get; set; }
        public string url { get; set; }
    }
}
