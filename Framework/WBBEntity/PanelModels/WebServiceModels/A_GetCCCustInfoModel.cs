namespace WBBEntity.PanelModels.WebServiceModels
{
    public class A_GetCCCustInfoModel
    {
        public string GUIDKEY { get; set; }
        /// <summary>
        /// รอบบิล
        /// </summary>
        public long BILL_CYCLE { get; set; }
        public string ACCOUNT_NO { get; set; }
        /// <summary>
        /// Customer, Billing, Service
        /// </summary>
        public string CUST_TYPE_PC { get; set; }
        /// <summary>
        /// Ex. 19850801
        /// </summary>
        public string BIRTH_DATE { get; set; }
        /// <summary>
        /// 01 = Normal
        /// 02 = VIP
        /// 03 = VIP Plus
        /// 04 = VIP Plus Plus
        /// </summary>
        public string CUST_CLASS { get; set; }
        /// <summary>
        /// Active
        /// Disconnect
        /// Inactive
        /// Pending Disconnect
        /// Suspend
        /// Suspend - Credit Limit
        /// Suspend - Debt
        /// Suspend - Fraud
        /// Suspend - Leasing
        /// Suspend - SS
        /// Terminate
        /// </summary>
        public string MOBILE_NO_STATUS { get; set; }
        public string GROUP_CODE { get; set; }
        public string CRM_SEGMENT { get; set; }
        public string CORPORATE_TYPE { get; set; }
        public string ACCOUNT_NUM { get; set; }

        /// <summary>
        /// AA, A, B, C, D, Gold, Platinum, Silver
        /// </summary>
        public string MOBILE_SEGMENT { get; set; }
        public string PROVINCE_CODE { get; set; }
        /// <summary>
        /// 01– GSM Advanced
        /// 23 – GSM 1800
        /// 03– 3G2100
        /// </summary>
        public string NETWORK_TYPE { get; set; }
        /// <summary>
        /// Post-paid, Pre-paid
        /// </summary>
        public string SUB_NETWORK_TYPE { get; set; }
        public string NAME { get; set; }
        public string ID_CARD_NO { get; set; }
        public string PASSWORD { get; set; }
        /// <summary>
        /// High,Medium,Low,Null
        /// </summary>
        public string CHURN { get; set; }
        /// <summary>
        /// Blacklist Status
        /// 0 = Green
        /// 10 = Yellow
        /// 20 = Orange
        /// 30 = Red
        /// </summary>
        public string CA_BLACKLIST { get; set; }
        /// <summary>
        /// Mobile Register Date (Service Year)
        /// </summary>
        public string REGISTER_DATE { get; set; }

        /// <summary>
        /// CONTACT PHONE
        /// </summary>
        public string CONTRACT_PHONE { get; set; }
        /// <summary>
        /// PA Group of Asset Ex. PA1, PA2
        /// </summary>
        public string PA_GROUP { get; set; }
        /// <summary>
        /// BA Payment Method Ex. Cash, Credit Card Debit
        /// </summary>
        public string PAYMENT_TYPE { get; set; }
        public string ACCOUNT_NUM2 { get; set; }
        public string NATIONALITY_CODE { get; set; }
        public string REGION_CODE { get; set; }
        public string LOCAL_LANGUAGE { get; set; }
        public string PP_COS_ID { get; set; }
        public string CA_NAME { get; set; }
        public string CA_ID_CARD_NO { get; set; }

        public string EFFECTIVE_DATE { get; set; }
        /// <summary>
        /// Asset Lifestyle
        /// </summary>
        public string CLV_SEGMENT { get; set; }
        public string REMARK { get; set; }
        public string TITLE { get; set; }
        public string CONTACT_EMAIL { get; set; }
        /// <summary>
        /// Asset Row ID
        /// </summary>
        public string ASSET_ID { get; set; }
        /// <summary>
        /// Picture_URL
        /// </summary>
        public string DESCRIPTION { get; set; }
        /// <summary>
        /// URL_Date Ex. MM/DD/YYYY HH24:MI:SS
        /// </summary>
        public string CREATE_DATE { get; set; }
        /// <summary>
        /// ContactId
        /// </summary>
        public string CONTACT_DESC { get; set; }
        /// <summary>
        /// Pre-paid
        /// 01– GSM Advanced
        /// 03– 3G2100
        /// </summary>
        public string NEW_MB_NETWORK_TYPE { get; set; }

        /// <summary>
        /// Billing_System (Project BOS)
        /// </summary>
        public string BILL_DESC { get; set; }
        public string BRAND_NAME { get; set; }
        public string INFO_TYPE { get; set; }
        public string REC_TYPE { get; set; }
        public string SYSTEM_TYPE { get; set; }
        public string AMENDMENT { get; set; }
        public string WAIVE_REMARKS { get; set; }
        public long TUX_CODE { get; set; }
    }

    public class Athena_IntraModel
    {
        public string resultCode { get; set; }
        public Athena_IntraData resultData { get; set; }

    }

    public class Athena_IntraData
    {
        public Athena_IntraSubData subScriptionProfile { get; set; }
    }

    public class Athena_IntraSubData
    {
        public string subscriptionState { get; set; }
        public string chargeType { get; set; }
        public string segment { get; set; }
        public string registerDate { get; set; }
        public string networkType { get; set; }
    }


}
