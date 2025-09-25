using System.Globalization;

namespace AIRNETEntity.Extensions
{
    public static class Constants
    {
        public static class Databases
        {
            public static string DbSchema { get; set; }
        }

        public static class DisplayFormats
        {
            public const string DateFormat = "dd/MM/yyyy";
            public const string DateTimeFormat = "dd/MM/yyyy HH:mm:ss";
            public static CultureInfo DefaultCultureInfo { get { return CultureInfo.GetCultureInfo("en-US"); } }

        }

        public static class AuthenDBReturnStatus
        {
            public static string Success = "0000";
        }

        public static class DbConstants
        {
            public static string ActiveStatus = "A";
        }

        public static class SSOReturnStatus
        {
            public const string AlreadyLoggedOut = "BAV007";
            public const string DataOrStatusHasChanged = "BAV008";
            public const string FunctionIsRedundant = "BAV015";
            public const string IncorectTransaction = "BAV017";
            public const string IncorrctedUserStatus = "BAV014";
            public const string IncorrectPrivilege = "BAV012";
            public const string InvalidSession = "BAV016";
            public const string InvalidUserNameOrPassword = "BAV001";
            public const string LogoutSuccess = "BAV005";
            public const string MultipleSessions = "BAV011";
            public const string NeverLoggedIn = "BAV013";
            public const string NotFound = "BAV004";
            public const string OnProcessing = "BAV002";
            public const string SessionExpiredOrAlreadyLoggedOut = "BAV010";
            public const string Success = "BAV000";
            public const string TemporaryNotAvailable = "BAV009";
            public const string TemporarySuspended = "BAV003";
            public const string UnAuthorized = "BAV006";
        }
    }
}
