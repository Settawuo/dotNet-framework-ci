using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace WBBWebService.Extension
{
    internal class Configurations
    {
        public static bool OnDev
        {
            get { return Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings[Constants.AppSettingKeys.ONDEVENVIRONMENT.ToString()]); }
        }

        public static string DbSchema
        {
            get { return System.Configuration.ConfigurationManager.AppSettings[Constants.AppSettingKeys.DBSCHEMA.ToString()]; }
        }

        public static string DbSchemaFBSS
        {
            get { return System.Configuration.ConfigurationManager.AppSettings[Constants.AppSettingKeys.DBSCHEMAFBSS.ToString()]; }
        }

        public static string DBSchemaAIRNET
        {
            get { return System.Configuration.ConfigurationManager.AppSettings[Constants.AppSettingKeys.DBSCHEMAAIRNET.ToString()]; }
        }

        public static string DbProjectCode
        {
            get { return System.Configuration.ConfigurationManager.AppSettings[Constants.AppSettingKeys.PROJECTCODEDB.ToString()]; }
        }

        public static string DbProjectCodeAirNet
        {
            get { return System.Configuration.ConfigurationManager.AppSettings[Constants.AppSettingKeys.PROJECTCODEDBAIRNET.ToString()]; }
        }

        public static string DbConnectionStringTemplate
        {
            get { return System.Configuration.ConfigurationManager.AppSettings[Constants.AppSettingKeys.CONNECTIONSTRINGTEMPLATE.ToString()]; }
        }

        public static string DbProjectCodeFBBShareplex
        {
            get { return System.Configuration.ConfigurationManager.AppSettings[Constants.AppSettingKeys.PROJECTCODEFBBSHAREPLEX.ToString()]; }
        }

        public static string DbProjectCodeFBBHVR
        {
            get { return System.Configuration.ConfigurationManager.AppSettings[Constants.AppSettingKeys.PROJECTCODEFBBHVR.ToString()]; }
        }

        public static IConfigurationRoot GetAppsetting
        {

            get
            {
                string usePathLocal = string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["UsePathLocal"]) ? "N" : System.Configuration.ConfigurationManager.AppSettings["UsePathLocal"];
                string appPath = System.Configuration.ConfigurationManager.AppSettings["PathAppsetting"];
                if (usePathLocal == "Y")
                {
                    appPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PathConnection");
                }
                var builder = new Microsoft.Extensions.Configuration.ConfigurationBuilder()
                    .SetBasePath(appPath)
                               .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                var _iconfiguration = builder.Build();

                return _iconfiguration;
            }
        }
        public static string GetContext
        {
            get
            {
                return GetAppsetting.GetConnectionString("Context");
            }
        }

        public static string GetAirNetContext
        {
            get
            {
                return GetAppsetting.GetConnectionString("AirNetContext");
            }
        }

        public static string GetFBBHVRContext
        {
            get
            {
                return GetAppsetting.GetConnectionString("FBBHVRContext");
            }
        }

        public static string GetFBBShareplexContext
        {
            get
            {
                return GetAppsetting.GetConnectionString("FBBShareplexContext");
            }
        }
    }
}