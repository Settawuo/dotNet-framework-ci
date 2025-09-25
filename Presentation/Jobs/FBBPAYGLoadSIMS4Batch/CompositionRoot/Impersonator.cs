using System;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace FBBPAYGLoadSIMS4Batch.CompositionRoot
{
    public class Impersonator : IDisposable
    {
        private WindowsImpersonationContext impersonatedUser = null;
        private IntPtr userToken;

        public Impersonator(string username, string domainOrServerName, string password, bool useDomain = true)
        {
            userToken = new IntPtr(0);

            bool logonResult = false;

            if (useDomain)
            {
                logonResult = LogonUser(username, domainOrServerName, password, LOGON32_LOGON_NETWORK, LOGON32_PROVIDER_DEFAULT, ref userToken);
            }
            else
            {
                logonResult = LogonUser(username, domainOrServerName, password, LOGON32_LOGON_NEW_CREDENTIALS, LOGON32_PROVIDER_DEFAULT, ref userToken);
            }

            if (!logonResult)
            {
                if (string.IsNullOrEmpty(domainOrServerName))
                {
                    throw new InvalidOperationException(string.Format("Failed to impersonate to {0}", username));
                }
                else
                {
                    throw new InvalidOperationException(string.Format("Failed to impersonate to {0}@{1}", username, domainOrServerName));
                }
            }

            impersonatedUser = new WindowsIdentity(userToken).Impersonate();
        }

        #region IDisposable Members

        public void Dispose()
        {
            try
            {
                if (impersonatedUser != null)
                {
                    impersonatedUser.Undo();
                    CloseHandle(userToken);
                }
            }
            catch { }
        }

        #endregion IDisposable Members

        #region external dll functions

        public const int LOGON32_LOGON_INTERACTIVE = 2;
        public const int LOGON32_LOGON_NETWORK = 3;
        public const int LOGON32_LOGON_BATCH = 4;
        public const int LOGON32_LOGON_SERVICE = 5;
        public const int LOGON32_LOGON_UNLOCK = 7;
        public const int LOGON32_LOGON_NETWORK_CLEARTEXT = 8;
        public const int LOGON32_LOGON_NEW_CREDENTIALS = 9;

        public const int LOGON32_PROVIDER_DEFAULT = 0;
        public const int LOGON32_PROVIDER_WINNT35 = 1;
        public const int LOGON32_PROVIDER_WINNT40 = 2;
        public const int LOGON32_PROVIDER_WINNT50 = 3;

        [DllImport("advapi32.dll", CharSet = CharSet.Auto)]
        public static extern bool LogonUser(String lpszUserName, String lpszDomain, String lpszPassword, int dwLogonType, int dwLogonProvider, ref IntPtr phToken);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public extern static bool CloseHandle(IntPtr handle);

        #endregion external dll functions
    }
}
