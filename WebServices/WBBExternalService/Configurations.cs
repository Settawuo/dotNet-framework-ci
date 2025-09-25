using System.Configuration;

namespace WBBExternalService
{
    public class Configurations
    {
        public static bool UploadFileByVirtualDir
        {
            get { return bool.Parse(ConfigurationManager.AppSettings[Constants.AppSettingKeys.UploadFileByVirtualDir.ToString()]); }
        }

        public static bool UploadFileByImpersonate
        {
            get { return bool.Parse(ConfigurationManager.AppSettings[Constants.AppSettingKeys.UploadFileByImpersonate.ToString()]); }
        }

        public static string UploadFileTempPath
        {
            get { return ConfigurationManager.AppSettings[Constants.AppSettingKeys.UploadFileTempPath.ToString()].ToString(); }
        }

        public static string UploadFilePath
        {
            get { return ConfigurationManager.AppSettings[Constants.AppSettingKeys.UploadFilePath.ToString()].ToString(); }
        }

        public static string ProjectCodeNAS
        {
            get { return ConfigurationManager.AppSettings[Constants.AppSettingKeys.ProjectCodeNAS.ToString()].ToString(); }
        }

        public static string FontFolder
        {
            get { return ConfigurationManager.AppSettings[Constants.AppSettingKeys.FontFolder.ToString()].ToString(); }
        }

        public static string ImageFolder
        {
            get { return ConfigurationManager.AppSettings[Constants.AppSettingKeys.ImageFolder.ToString()].ToString(); }
        }
    }
}