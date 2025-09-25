using Microsoft.Extensions.Configuration;
using System.Configuration;

namespace WBBWeb.Extension
{
    public static class Configurations
    {
        //public static string K2DomainName
        //{
        //    get { return ConfigurationManager.AppSettings[WebConstants.AppSettingKeys.DomainName.ToString()].ToString(); }
        //}

        public static bool UploadFileByVirtualDir
        {
            get { return bool.Parse(System.Configuration.ConfigurationManager.AppSettings[WebConstants.AppSettingKeys.UploadFileByVirtualDir.ToString()]); }
        }

        public static bool UploadFileByImpersonate
        {
            get { return bool.Parse(System.Configuration.ConfigurationManager.AppSettings[WebConstants.AppSettingKeys.UploadFileByImpersonate.ToString()]); }
        }

        public static string UploadFileTempPath
        {
            get { return System.Configuration.ConfigurationManager.AppSettings[WebConstants.AppSettingKeys.UploadFileTempPath.ToString()].ToString(); }
        }

        public static string UploadFilePath
        {
            get { return System.Configuration.ConfigurationManager.AppSettings[WebConstants.AppSettingKeys.UploadFilePath.ToString()].ToString(); }
        }

        public static string ProjectCodeNAS
        {
            get { return System.Configuration.ConfigurationManager.AppSettings[WebConstants.AppSettingKeys.ProjectCodeNAS.ToString()].ToString(); }
        }

        public static string UploadImageFile
        {
            get { return System.Configuration.ConfigurationManager.AppSettings[WebConstants.AppSettingKeys.UploadImageFile.ToString()].ToString(); }
        }
        public static bool UseLDAP
        {
            get
            {
                var useLDAP = false;
                bool.TryParse(System.Configuration.ConfigurationManager.AppSettings[WebConstants.AppSettingKeys.UseLDAP.ToString()], out useLDAP);
                return useLDAP;
            }
        }

        public static string ProjectCodeLdapFBB
        {
            get { return System.Configuration.ConfigurationManager.AppSettings[WebConstants.AppSettingKeys.ProjectCodeLdapFBB.ToString()]; }
        }


        public static string TARGET
        {
            get { return System.Configuration.ConfigurationManager.AppSettings[WebConstants.AppSettingKeys.TARGET.ToString()]; }
        }

        public static string TARGET_DOMAIN
        {
            get { return System.Configuration.ConfigurationManager.AppSettings[WebConstants.AppSettingKeys.TARGET_DOMAIN.ToString()]; }
        }

        public static string TARGET_USER
        {
            get { return System.Configuration.ConfigurationManager.AppSettings[WebConstants.AppSettingKeys.TARGET_USER.ToString()]; }
        }

        public static string TARGET_PWD
        {
            get { return System.Configuration.ConfigurationManager.AppSettings[WebConstants.AppSettingKeys.TARGET_PWD.ToString()]; }
        }
        public static string GetConnectionStrings
        {
            get
            {
                var builder = new Microsoft.Extensions.Configuration.ConfigurationBuilder()
                    .SetBasePath(System.Configuration.ConfigurationManager.AppSettings["PathAppsetting"])
                               .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                var _iconfiguration = builder.Build();

                return _iconfiguration.GetConnectionString("DefaultConnection");
            }
        }
    }
}
