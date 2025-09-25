using System.Collections.Generic;

namespace WBBContract.Queries.ExWebServices.ATN
{
    public class CustomerProfileMobilePackageCurrentMain
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
        public string cbsProductStatus { get; set; }
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
        public string extProductStatus { get; set; }
        public string name { get; set; }
        public string shortenedNameTH { get; set; }
        public string shortenedNameEN { get; set; }
        public string descriptionTH { get; set; }
        public string descriptionEN { get; set; }
        public string billItemDescriptionTH { get; set; }
        public string billItemDescriptionEN { get; set; }
        public string productCd { get; set; }
        public string productClass { get; set; }
        public List<CustomerProfileAttribute> attribute { get; set; }
        public string maxFnNo { get; set; }
        public string productOfferPriceId { get; set; }
        public string packageId { get; set; }
        public string monthlyFee { get; set; }
        public string othersFeeRate { get; set; }
        public string productType { get; set; }
        public string groupPromo { get; set; }
        public string productEffectiveTime { get; set; }
        public string productExpireTime { get; set; }
        public string nextBillDate { get; set; }
        public string IntegrationNameTH { get; set; }
        public string IntegrationNameEN { get; set; }
        public string priceIncludeVat { get; set; }
        public string commercialType { get; set; }
        public string priceType { get; set; }
        public string offeringName { get; set; }
        public string productId { get; set; }
        public string productSequenceId { get; set; }
        public string productName { get; set; }
        public List<CustomerProfileFreeUnitItem> freeUnitItemList { get; set; }
        public string productStatus { get; set; }
        public string productPackage { get; set; }
        public string offeringGroup { get; set; }
        public string prorateFlag { get; set; }
        public string createUser { get; set; }
        public string createDate { get; set; }
        public string duration { get; set; }
        public string durationType { get; set; }
        public string chargeAmount { get; set; }
        public string openDate { get; set; }
        public string gvProductSequenceId { get; set; }
        public string upLoadSpeed { get; set; }
        public string downLoadSpeed { get; set; }
        public List<CustomerProfileNextPackage> nextPackage { get; set; }
        public string priceExcludeVat { get; set; }
        public string pro5gflg { get; set; }
        public string crmFlag { get; set; }
        public string paymentMode { get; set; }
        public string bundlingServiceName { get; set; }
        public string deviceContractFlg { get; set; }
        public string netFlexiFlg { get; set; }
        public string productAcctnCat { get; set; }
    }
}
