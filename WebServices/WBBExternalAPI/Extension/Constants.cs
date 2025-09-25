namespace WBBExternalAPI.Extension
{
    internal class Constants
    {
        public enum AppSettingKeys
        {
            DBSCHEMA,
            DBSCHEMAFBSS,
            DBSCHEMAAIRNET,
            PROJECTCODEDB,
            PROJECTCODEDBAIRNET,
            CONNECTIONSTRINGTEMPLATE,
            ONDEVENVIRONMENT,
            PROJECTCODEFBBSHAREPLEX
        }

        public static class AuthenDBReturnStatus
        {
            public const string Success = "0000";
            public const string InvalidInputParameter = "0008";
            public const string ConnotConnectDBServer = "9004";
            public const string IPisNoPermission = "9005";
            public const string ProjectIsNotStart = "9006";
            public const string ProjectStatusIsNotReady = "9007";
            public const string ServiceStatusIsNotReady = "9008";
            public const string ServiceIsNotStart = "9009";
            public const string ServiceExpired = "9010";
            public const string DatabaseStatusIsNotReady = "9011";
            public const string ServerStatusIsNotReady = "9012";
            public const string UserStatusIsNotReady = "9013";
            public const string ServerDoesNotRegisterOrNoProjectCode = "8003";
        }

        public static class LDAPWSAuthenReturnStatus
        {
            public const string InvalidPW = "0112";
            public const string InvalidProjectCode = "0111";
            public const string Success = "0000";
        }

        public static class DBDReturnStatus
        {
            public const string Success = "0000";
        }
    }
}