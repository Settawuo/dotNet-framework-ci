using System.Collections.Generic;

namespace WBBEntity.PanelModels.ExWebServiceModels
{
    public class evESeServiceQueryPrivilegeByMobileNoModel
    {
        public evESeServiceQueryPrivilegeByMobileNoModel()
        {
            if (assetPromotionItemList == null)
                assetPromotionItemList = new List<PrivilegePromotionModel>();
            if (assetServiceItemList == null)
                assetServiceItemList = new List<PrivilegeServiceModel>();
        }
        public string result { get; set; }
        public string errorMessage { get; set; }
        public List<PrivilegePromotionModel> assetPromotionItemList { get; set; }
        public List<PrivilegeServiceModel> assetServiceItemList { get; set; }
    }
    public class PrivilegePromotionModel
    {
        public string accountId { get; set; }
        public string accountNumber { get; set; }
        public string accountName { get; set; }
        public string billingAccountId { get; set; }
        public string billingAccountNumber { get; set; }
        public string billingAccountName { get; set; }
        public string mobileNo { get; set; }
        public string integrationId { get; set; }
        public string productId { get; set; }
        public string productCode { get; set; }
        public string productName { get; set; }
        public string integrationName { get; set; }
        public string promotionIntegrationId { get; set; }
        public string promotionStartDt { get; set; }
        public string promotionEndDt { get; set; }
        public string promotionStatusCd { get; set; }
        public string effectiveDt { get; set; }
        public string Expired { get; set; }
        public string endDt { get; set; }
        public string remark { get; set; }
        public string reasonCode { get; set; }
        public string orderNo { get; set; }
        public string projectCd { get; set; }

    }
    public class PrivilegeServiceModel
    {
        public string accountId { get; set; }
        public string accountNumber { get; set; }
        public string accountName { get; set; }
        public string billingAccountId { get; set; }
        public string billingAccountNumber { get; set; }
        public string billingAccountName { get; set; }
        public string mobileNo { get; set; }
        public string integrationId { get; set; }
        public string productId { get; set; }
        public string productCode { get; set; }
        public string productName { get; set; }
        public string integrationName { get; set; }
        public string serviceIntegrationId { get; set; }
        public string serviceStartDt { get; set; }
        public string serviceEndDt { get; set; }
        public string serviceStatusCd { get; set; }
        public string effectiveDt { get; set; }
        public string expiredt { get; set; }
        public string endDt { get; set; }
        public string remark { get; set; }
        public string reasonCode { get; set; }
        public string orderNo { get; set; }
        public string projectCd { get; set; }

    }
}
