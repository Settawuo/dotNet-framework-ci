namespace WBBEntity.PanelModels.WebServiceModels
{
    public class GetIMQueryCampaignContactModel
    {
        public string MOBILE_NUMBER { get; set; }
        public string LAST_UPDATED_DATE { get; set; }
        public string VALUE_1 { get; set; }
        public string CONTACT_LIST_INFO { get; set; }
        public string RESULT_CODE { get; set; }
        public string RESULT_DESC { get; set; }
    }

    public class IMQueryCampaignContactResponse
    {
        public string MobileNumber { get; set; }
        public decimal? NumberOfRecords { get; set; }
        public decimal? IntegerVal1 { get; set; }
        public string ServiceStatus { get; set; }
        public decimal? ServiceOption { get; set; }
        public string StringVal7 { get; set; }
        public IMQueryCampaignContactConfigParameterList ParameterList { get; set; }
        public string ChildCampaignCode { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public string StringVal1 { get; set; }
        public IMQueryCampaignContactListResponse[] CampaignContactList { get; set; }
    }

    public class IMQueryCampaignContactListResponse
    {
        public string LastUpdatedDate { get; set; }
        public string ContactPhoneNumber { get; set; }
        public string Value3 { get; set; }
        public string Value1 { get; set; }
        public string Value2 { get; set; }
        public decimal? DebtAmount { get; set; }
        public string Source { get; set; }
        public string AccountBasePriorityCall { get; set; }
        public string ReferenceAccountNumber { get; set; }
        public string ProspectContactName { get; set; }
        public string ContactStatusUpdateBy { get; set; }
        public string IDNumber { get; set; }
        public string LastUpdatedBy { get; set; }
        public string Comment { get; set; }
        public string CreatedBy { get; set; }
        public string Action1 { get; set; }
        public string Action2 { get; set; }
        public string Action3 { get; set; }
        public string CANumber { get; set; }
        public string ReferenceNumber { get; set; }
        public string DoneFlag { get; set; }
        public string ContactStatusDate { get; set; }
        public string ContactRowID { get; set; }
        public string BANumber { get; set; }
        public string CallAttempt { get; set; }
        public string CampaignCode { get; set; }
        public string MobileNumber { get; set; }
        public string ContactListInfo { get; set; }
        public IMQueryCampaignContactListDetailResponse Campaign { get; set; }
        public string Outcome { get; set; }
        public string ContactStatus { get; set; }
        public string SRNPA { get; set; }
        public string CreatedDate { get; set; }
    }

    public class IMQueryCampaignContactListDetailResponse
    {
        public string URLOutboundTemplate { get; set; }
        public string LastUpdatedDate { get; set; }
        public string LastUpdatedBy { get; set; }
        public string InternalID { get; set; }
        public string CampaignURL { get; set; }
        public string CampaignStatus { get; set; }
        public string CreatedBy { get; set; }
        public IMQueryCampaignContactListDetailChannelResponse[] Channel { get; set; }
        public string CampaignSubType { get; set; }
        public string CampaignName { get; set; }
        public string EndDate { get; set; }
        public string MNP { get; set; }
        public string CampaignLevel { get; set; }
        public string CampaignSystem { get; set; }
        public string CampaignCode { get; set; }
        public string StartDate { get; set; }
        public string ParentCampaignCode { get; set; }
        public string CampaignAddedFlag { get; set; }
        public string CreatedDate { get; set; }
        public string CampaignType { get; set; }
        public string MassCampaignFlag { get; set; }
    }

    public class IMQueryCampaignContactListDetailChannelResponse
    {
        public string Name { get; set; }
    }

    public class IMQueryCampaignContactConfigModel
    {
        public string Url { get; set; }
        public string UseSecurityProtocol { get; set; }
        public string BodyStr { get; set; }
    }

    public class IMQueryCampaignContactConfigBody
    {
        public string ServiceOption { get; set; }
        public string MobileNumber { get; set; }
        public string ChildCampaignCode { get; set; }
        public IMQueryCampaignContactConfigParameterList ParameterList { get; set; }
    }

    public class IMQueryCampaignContactConfigParameterList
    {
        public IMQueryCampaignContactConfigDetail[] Parameter { get; set; }
    }

    public class IMQueryCampaignContactConfigDetail
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
