namespace WBBWeb.Extension
{
    public static class WebConstants
    {
        public enum AppSettingKeys
        {
            ProjectCodeDb,
            TestFlag,
            OnDevEnvironment,

            ProjectCodeNAS,
            UploadFileByVirtualDir,
            UploadFileTempPath,
            UploadFilePath,
            UploadFileByImpersonate,
            UploadPDFVastotals,
            UploadImageFile,
            UseLDAP,
            ProjectCodeLdapFBB,

            //<!--config path file auto mail report-->
            TARGET,
            TARGET_DOMAIN,
            TARGET_USER,
            TARGET_PWD
        }

        public static class SessionKeys
        {
            public const string AvoidSessionIDChangesIssue = "__Init";
            public const string AllLov = "AllLov";
            public const string LoadLov = "LoadLov";
            public const string CurrentUICulture = "CurrentUICulture";

            public const string ZipCodeData = "ZipCodeData";
        }

        public static class LovConfigName
        {
            public const string Screen = "SCREEN";
            public const string CoveragePageCode = "FBBOR001";
            public const string SelectType_Service = "FBBORV00";
            public const string DisplayPackagePageCode = "FBBOR002";
            public const string DisplayPopupVasConfrim = "FBBORV11";
            public const string TopupPlayBox = "FBBORV12";
            public const string CustomerRegisterPageCode = "FBBOR003";
            public const string GetTopUpFixedlinePageCode = "FBBOR023";
            public const string GetTopUpInternetPageCode = "FBBOR024";
            public const string GetTopUpMeshPageCode = "FBBOR041";
            public const string GetSelectRouterCode = "FBBOR025";
            public const string ChangePromotionPageCode = "FBBOR016";
            public const string SummaryPageCode = "FBBOR004";
            public const string Vas_Package = "FBBORV10";
            public const string TitleCodeTh = "TITLE_CODE_TH";
            public const string TitleCodeEn = "TITLE_CODE_EN";
            public const string Nationality = "NATIONALITY";
            public const string CardType = "ID_CARD_TYPE";
            public const string Gender = "GENDER";
            public const string TermAndCondition = "TERM_AND_CONDITION";
            public const string ContactTime = "CONTACT_TIME";
            public const string TimeSlot = "CONTACT_TIMES";
            public const string Document = "DOCUMENT";
            public const string FBBDORMSCREEN = "FBBDORM_SCREEN";
            public const string FbbConstant = "FBB_CONSTANT";
            public const string FbbExceptionIdCard = "FBB_EXCEPTION_ID_CARD";
            public const string NotName = "Char Not on Name";
            public const string CheckDataOnelove = "F_MOBILE_FORWARD";
            public const string CheckPrePostPaid = "FBBOR015";
            public const string Plugandplay = "TERM_AND_CONDITION_PLUGANDPLAY";
            public const string LovTypeMenu = "SCREEN_MENU";
            public const string PaymentPageCode = "FBBOR019";
            public const string ReportProblemsPageCode = "FBBOR020";
            public const string ProblemType = "PROBLEM_TYPE";
            public const string MeshConfig = "FBB_MESH_CONFIG";
            public const string GetTopUpReplacePageCode = "FBBOR050";
            public const string GetScreenPriceMonthFlag = "SCREEN_PRICE_MONTH_FLAG";
            public const string GetScreenWelcomeToAisFibre = "FBBOR051";
            public const string GetConfig = "CONFIG";
            public const string GetIpCamera = "IPCamera";
        }

        public static class FBBConfigSessionKeys
        {
            public const string User = "User";
            public const string UserPaymentPromblemReport = "PaymentPromblemReport";
            public const string PaymentLog = "PaymentLog";

        }

        public class PaymentAndReport
        {
            //Constant
            public const string PAYMENT_PAGE_LOGIN = "LOGIN_PAYMENT";
            public const string REPORT_PAGE_LOGIN = "LOGIN_REPORT_PROBLEM";
            public const string PAYMENT_AND_REPORT_PAGE_LOGIN = "Login";
            public const string PAYMENT_AND_REPORT_PAGE_LOGIN_CONCURRENT = "LoginConcurrent";
            public const string PAYMENT_AND_REPORT_PAGE_SESSION_TIMEOUT = "SessionTimeout";
            //public const string PAYMENT_AND_REPORT_SESSION = "PaymentReportProblemLogin";
            public const string LOGIN_PAYMENT_AND_REPORT_RESULT_SUCCESS = "LOGIN_RESULT_001";
            public const string LOGIN_PAYMENT_AND_REPORT_RESULT_FAIL = "LOGIN_RESULT_002";
            public const string LOGIN_PAYMENT_AND_REPORT_IN_OPTION = "2";
            public const string LOGIN_PAYMENT_AND_REPORT_ASSET_STATUS = "Active";

            public const string MPAY_STATUS_SUCCESS = "success";
            public const string MPAY_STATUS_FAILED = "fail";
            public const string MPAY_STATUS_ABORT = "abort";

            public const string LOG_EVENT_NAME_LOGIN = "LOGIN";
            public const string LOG_EVENT_NAME_CHECKBALANCE = "CHECKBALANCE";
            public const string LOG_EVENT_NAME_PAYMENTMPAY = "PAYMENTMPAY";
            public const string LOG_EVENT_NAME_PAYMENTCREDIT = "PAYMENTCREDIT";
            public const string LOG_TYPE_REQUEST = "REQUEST";
            public const string LOG_TYPE_RESPONSE = "RESPONSE";
            public const string LOG_STATUS_SUCCESS = "SUCCESS";
            public const string LOG_STATUS_FAILED = "FAILED";
            public const string LOG_DATETIME_FORMAT = "yyyyMMdd HH:mm:ss.fff";
            public const string LOG_NULL_TEXT = "NULL";

            public const string LOG_SERVICE_NAME_ESERVICE_QUERY_MASS_COMMON_ACCOUNT_INFO = "evESeServiceQueryMassCommonAccountInfo";
            public const string LOG_SERVICE_NAME_A_LSTPMMOBDTL = "A_LstPMMobDtl";


            public const string TRANSACTION_ID_DATETIME_FORMAT = "yyMMddHHmmss";



        }

        public static class NotifyKey
        {
            public const string LineNotifyFBB = "ZfDQipwn58f0w7C5Lvqk2N6xfOoZKikdhyXKE19H5by";

        }
    }
}