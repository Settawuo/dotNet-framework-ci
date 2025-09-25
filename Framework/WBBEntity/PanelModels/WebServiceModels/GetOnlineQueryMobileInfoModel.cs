using System.Collections.Generic;

namespace WBBEntity.PanelModels.WebServiceModels
{
    public class GetOnlineQueryMobileInfoModel
    {
        public string RESULT_CODE { get; set; }
        public string RESULT_DESC { get; set; }
        public string TRANSACTION_ID { get; set; }
        public List<OnlineQuerySpecialOfferResult> LIST_SPECIAL_OFFER { get; set; }
        public OnlineQueryPersonalInfoResult PERSONAL_INFO { get; set; }
        public OnlineQueryFibrenetInfoResult FIBRENET_INFO { get; set; }
    }

    public class OnlineQuerySpecialOfferResult
    {
        public string NAME { get; set; }
        public string VALUE { get; set; }
    }

    public class OnlineQueryPersonalInfoResult
    {
        public string PRICE_EXCL_VAT { get; set; }
        public string SERVICE_YEAR_DAY { get; set; }
        public string SUB_NETWORK_TYPE { get; set; }
        public string NETWORK_TYPE { get; set; }
        public string ID_CARD_NO { get; set; }
        public string ID_CARD_TYPE { get; set; }
        public string ID_CARD_TYPE_FOR { get; set; }
        public string MOBILE_SEGMENT { get; set; }
        public string PROJECT_NAME { get; set; }
        public string ACCOUNT_CATEGORY { get; set; }
        public string REGISTER_DATE { get; set; }
        public string SFF_SERVICE_YEAR { get; set; }
        public string MOBILE_NO_STATUS { get; set; }
        public string SERVICE_LEVEL { get; set; }
        public string PA_GROUP { get; set; }
    }

    public class GetOnlineQueryPackPenaltyModel
    {
        public string RESULT_CODE { get; set; }
        public string RESULT_DESC { get; set; }
        public string TRANSACTION_ID { get; set; }
        public List<PackPenaltyResult> LIST_PACKAGE_PENALTY { get; set; }
    }

    public class PackPenaltyResult
    {
        public string SFF_PROMOTION_CODE { get; set; }
        public string SFF_PRODUCT_NAME { get; set; }
        public string ALLOW_RELOCATE { get; set; }
        public string RELOCAT_START_DATE { get; set; }
        public string RELOCAT_END_DATE { get; set; }
        public string SFF_PRODUCT_PACKAGE { get; set; }
        public string PACKAGE_DISPLAY_THA { get; set; }
        public string PACKAGE_DISPLAY_ENG { get; set; }
        public string PACKAGE_DESC { get; set; }
        public string PACKAGE_DURATION { get; set; }
    }

    public class OnlineQueryFibrenetInfoResult
    {
        public string ID_CARD_NO { get; set; }
        public string ID_CARD_TYPE { get; set; }
    }

}
