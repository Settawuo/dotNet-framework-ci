
namespace WBBEntity.PanelModels.WebServiceModels
{
    public class QueryDBDProfileModel
    {
        public string ResultCode { get; set; }
        public string ResultMessage { get; set; }
        public DbdProfileModel DbdProfile { get; set; }
    }

    public class DbdProfileModel
    {
        public string AMPHUR_CODE { get; set; }
        public string BUSINESS_REGIST_NUM { get; set; }
        public string BUSINESS_STATUS_CODE { get; set; }
        public string BUSINESS_STATUS_DATE { get; set; }
        public string CAPITAL_UPDATE_DATE { get; set; }
        public string CREATED_BY { get; set; }
        public string CREATED_DATE { get; set; }
        public string CUSTOMER_STATUS { get; set; }
        public string DATA_TYPE { get; set; }
        public string DBD_ABBS_NAME { get; set; }
        public string DBD_NUMBER { get; set; }
        public string DBD_PROFILE_ID { get; set; }
        public string FACEBOOK { get; set; }
        public string GROUP_COMPANY_ID { get; set; }
        public string HQ_ADDRESS { get; set; }
        public string HQ_AMPHUR_NAME { get; set; }
        public string HQ_POSTCODE { get; set; }
        public string HQ_PROVINCE_NAME { get; set; }
        public string HQ_TUMBOL_NAME { get; set; }
        public string INDUSTRY_SIZE { get; set; }
        public string LATITUDE { get; set; }
        public string LONGTITUDE { get; set; }
        public string MAIN_FAX { get; set; }
        public string MAIN_PHONE { get; set; }
        public string MODIFIED_BY { get; set; }
        public string MODIFIED_DATE { get; set; }
        public string NAME_UPDATE_DATE { get; set; }
        public string PHOTO_ID { get; set; }
        public string POTENTIAL_LOG_ID { get; set; }
        public string PREVIOUS_CAPITAL { get; set; }
        public string PREVIOUS_NAME { get; set; }
        public string PROVINCE_CODE { get; set; }
        public string RECORD_SOURCE { get; set; }
        public string REGIST_CAPITAL { get; set; }
        public string REGIST_DATE { get; set; }
        public string REGIST_NAME { get; set; }
        public string REGIST_TITLE { get; set; }
        public string REGIST_TITLE_OTHER { get; set; }
        public string REMARK_INACTIVE { get; set; }
        public string REMARK_INACTIVE_UPD_DATE { get; set; }
        public string REMARK_NAME_UPDATE { get; set; }
        public string REMARK_NAME_UPDATE_DATE { get; set; }
        public string SALE_MANAGER { get; set; }
        public string SALE_REGION { get; set; }
        public string SALE_REP_ID { get; set; }
        public string SFF_ROW_ID { get; set; }
        public string SOCIAL_URL { get; set; }
        public string TSIC_CODE { get; set; }
        public string TSIC_DETAILS { get; set; }
        public string TSIC_GROUP_CODE { get; set; }
        public string TSIC_UPDATE_DATE { get; set; }
        public string TUMBOL_CODE { get; set; }
        public string TWITTER { get; set; }
        public string UPDATE_HQ_ADDRESS_DATE { get; set; }
    }
}
