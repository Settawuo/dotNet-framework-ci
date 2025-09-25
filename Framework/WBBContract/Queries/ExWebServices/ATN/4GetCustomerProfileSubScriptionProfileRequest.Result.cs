using System.Collections.Generic;
using System.Runtime.Serialization;

namespace WBBContract.Queries.ExWebServices.ATN
{
    [DataContract]
    public class GetCustomerProfileSubScriptionProfileRequest : IQuery<GetCustomerProfileSubScriptionProfileResult>
    {
        [DataMember(Order = 0)]
        public string transactionId { get; set; }
        [DataMember(Order = 1)]
        public string channel { get; set; }
        [DataMember(Order = 2)]
        public string username { get; set; }
        [DataMember(Order = 3)]
        public string key_name { get; set; }
        [DataMember(Order = 4)]
        public string key_value { get; set; }
        [DataMember(Order = 5)]
        public string status { get; set; }
        [DataMember(Order = 6)]
        public string filter { get; set; }
    }

    public class GetCustomerProfileSubScriptionProfileResult : ResultBaseATN
    {
        public GetCustomerProfileSubScriptionProfileData resultData { get; set; }
    }

    public class GetCustomerProfileSubScriptionProfileData
    {
        public GetCustomerProfileSubScriptionProfileItem subScriptionProfile { get; set; }
        public CustomerProfileNafaProfile nafaProfile { get; set; }
    }

    public class GetCustomerProfileSubScriptionProfileItem
    {
        public string caId { get; set; }
        public string billingSystemAccountId { get; set; }
        public string billingSystemCustomerId { get; set; }
        public string emailLanguage { get; set; }
        public string ivrLanguage { get; set; }
        public string msisdn { get; set; }
        public string publicType { get; set; }
        public string smsLanguage { get; set; }
        public string subscriptionState { get; set; }
        public string subscriptionStateDate { get; set; }
        public string ussdLanguage { get; set; }
        public string dataChargingSystem { get; set; }
        public string baId { get; set; }
        public string saId { get; set; }
        public string chargeType { get; set; }
        public string segment { get; set; }
        public string registerDate { get; set; }
        public string brandId { get; set; }
        public string servicePackageId { get; set; }
        public string classOfService { get; set; }
        public List<CustomerProfileLuckyItem> luckyLists { get; set; }
        public string networkType { get; set; }
        public string subNetworkType { get; set; }
        public string churnScore { get; set; }
        public string churnScoreReason { get; set; }
        public string paGroup { get; set; }
        public string paGroupEffectiveDate { get; set; }
        public string regionCode { get; set; }
        public string cosId { get; set; }
        public string segmentEffectiveDate { get; set; }
        public string segmentExpiryDate { get; set; }
        public string clvSegment { get; set; }
        public string remark { get; set; }
        public string brandName { get; set; }
        public string suspendType { get; set; }
        public string suspendCount { get; set; }
        public string suspendFraudCount { get; set; }
        public string firstSuspendCountDate { get; set; }
        public string blacklistStatus { get; set; }
        public string installmentFlag { get; set; }
        public string contractPhoneFlag { get; set; }
        public string contractFlag { get; set; }
        public string billingMainProductId { get; set; }
        public string simSerialNo { get; set; }
        public string authorizePersonFlag { get; set; }
        public string fbbContactNo { get; set; }
        public string billingSystem { get; set; }
        public string ocrFlag { get; set; }
        public string ocrReflag { get; set; }
        public string ocrReason { get; set; }
        public string ocrApproveBy { get; set; }
        public string ocrApproveDate { get; set; }
        public string ocrDate { get; set; }
        public string ocrErrorCode { get; set; }
        public string ocrLastUpdate { get; set; }
        public string ocrLastUpdateBy { get; set; }
        public string statusReason { get; set; }
        public string idenFaceFlag { get; set; }
        public string businessRegId { get; set; }
        public string regLocationCode { get; set; }
        public string productType { get; set; }
        public string classifyCode { get; set; }
        public string mobileNoStatus { get; set; }
        public string mobileNoStatusDate { get; set; }
        public string moiFlag { get; set; }
        public string moiDate { get; set; }
        public string businessGrp { get; set; }
        public string updateLocation { get; set; }
        public string binNo { get; set; }
        public string focMember { get; set; }
        public string maxMsg { get; set; }
        public string applyMobileNo { get; set; }
        public string applyLocation { get; set; }
        public string applyLocationName { get; set; }
        public string msgLanguage { get; set; }
        public string regUserId { get; set; }
        public string cleaningFlag { get; set; }
        public string cleaningDate { get; set; }
        public string smartCase { get; set; }
        public string cardVersion { get; set; }
        public string ocrReflagApprove { get; set; }
        public string smartCardFlag { get; set; }
        public string balance { get; set; }
        public string validity { get; set; }
        public CustomerProfileServiceYear serviceYear { get; set; }
        public string serviceDay { get; set; }
        public string channel { get; set; }
        public string applyDate { get; set; }
        public string amendmentReason { get; set; }
        public string mobileType { get; set; }
        public string reason { get; set; }
        public string effectiveServicePI { get; set; }
        public string cbsMigrateDate { get; set; }
        public string cbsMigrateFlag { get; set; }
        public string insMigrateDate { get; set; }
        public string insMigrateFlag { get; set; }
        public string vpnFlag { get; set; }
        public string vpnEffectiveDate { get; set; }
        public string vpnExpireDate { get; set; }
        public string loanFlag { get; set; }
        public string piFlag { get; set; }
        public string firstActivateDate { get; set; }
        public string reasonCode { get; set; }
        public string simStatus { get; set; }
        public string nType { get; set; }
        public string fbbContactNo2 { get; set; }
        public string fbbContactNo3 { get; set; }
        public string maxSpeedUL { get; set; }
        public string maxSpeedDL { get; set; }
        public string suspendDate { get; set; }
        public string suspendReason { get; set; }
        public CustomerProfileCredCreditAmount credCreditAmount { get; set; }
        public string changeBillingSystemDate { get; set; }
        public string familyAuth { get; set; }
        public string cimAssetRemark { get; set; }
    }
}
