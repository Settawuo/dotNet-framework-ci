using System.Configuration;

namespace FBBConfig.Extensions
{
    public static class Configurations
    {
        public static bool UseLDAP
        {
            get
            {
                var useLDAP = false;
                bool.TryParse(ConfigurationManager.AppSettings[WebConstants.AppSettingKeys.UseLDAP.ToString()], out useLDAP);
                return useLDAP;
            }
        }

        public static string ProjectCodeLdapFBB
        {
            get { return ConfigurationManager.AppSettings[WebConstants.AppSettingKeys.ProjectCodeLdapFBB.ToString()]; }
        }

        public static string dowloadReportPath
        {
            get { return ConfigurationManager.AppSettings[WebConstants.AppSettingKeys.dowloadReportPath.ToString()]; }
        }

        public static string _key
        {
            get { return ConfigurationManager.AppSettings[WebConstants.AppSettingKeys.KEY.ToString()]; }
        }

        public static string _keydatapower
        {
            get { return ConfigurationManager.AppSettings[WebConstants.AppSettingKeys.KEYDATAPOWER.ToString()]; }
        }
        public static string NAS_HOST
        {
            get { return ConfigurationManager.AppSettings[WebConstants.AppSettingKeys.NAS_HOST.ToString()]; }
        }
        public static string IDS_URL
        {
            get { return ConfigurationManager.AppSettings[WebConstants.AppSettingKeys.IDS_URL.ToString()]; }
        }
      
        public static bool SKIP_CERT_IDS
        {
            get
            {
                var skipCert = false;
                bool.TryParse(ConfigurationManager.AppSettings[WebConstants.AppSettingKeys.SKIP_CERT_IDS.ToString()], out skipCert);
                return skipCert;
            }
        }

    }
}