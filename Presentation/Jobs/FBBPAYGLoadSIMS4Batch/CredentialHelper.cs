using System;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Security.Principal;

namespace FBBPAYGLoadSIMS4Batch
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Text;
    using WBBBusinessLayer;
    using WBBEntity.Extensions;
    using WBBEntity.PanelModels.FBBWebConfigModels;

    public class CredentialHelper
    {

        public ILogger _logger;
        public CredentialHelper(ILogger logger)
        {
            _logger = logger;
        }

        protected int _dayCallback = ConfigurationManager.AppSettings["DayCallback"].ToSafeInteger();
        protected int _dayArchiveForOrigin = ConfigurationManager.AppSettings["DayArchiveForOrigin"].ToSafeInteger();
        protected int _dayArchiveForDestination = ConfigurationManager.AppSettings["DayArchiveForDestination"].ToSafeInteger();
        protected string _archivePath = ConfigurationManager.AppSettings["archive"].ToSafeString();
        protected string _archiveLogPath = ConfigurationManager.AppSettings["archiveLog"].ToSafeString();
        protected string _archiveLocalPath = ConfigurationManager.AppSettings["archiveLocal"].ToSafeString();

        const int LOGON32_PROVIDER_DEFAULT = 0;
        // This parameter causes LogonUser to create a primary token.
        const int LOGON32_LOGON_INTERACTIVE = 9;

        [DllImport("advapi32.DLL", SetLastError = true)]
        public static extern int LogonUser(string lpszUsername, string lpszDomain, string lpszPassword, int dwLogonType,
            int dwLogonProvider, ref IntPtr phToken);

        public bool WriteFileLocal(string pathName, string fileName, List<PAYGTransAirnetFileList> data, bool writeNewFile = true)
        {
            bool isSuccess;

            try
            {
                if (writeNewFile)
                {
                    isSuccess = WriteFileLocal(pathName, fileName, data);
                }
                else
                {
                    isSuccess = PrepareFileContinue(pathName, fileName, data);
                }

            }
            catch (Exception e)
            {
                _logger.Info("Write file => Exception : " + e.Message);
                isSuccess = false;
            }

            return isSuccess;
        }

        public bool WriteFileLocal(string pathName, string fileName, List<PAYGTransAirnetFileList> data)
        {
            bool isSuccess;

            try
            {
                isSuccess = GenerateFile(data, pathName, fileName);
            }
            catch (Exception e)
            {
                _logger.Info("Write file => Exception : " + e.Message);
                isSuccess = false;
            }

            return isSuccess;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="pwd"></param>
        /// <param name="domin"></param>
        /// <param name="sourceFullPathName"></param>
        /// <returns></returns>
        [PermissionSetAttribute(SecurityAction.Demand, Name = "FullTrust")]
        public static byte[] ReadFile(string username, string pwd, string domin, string sourceFullPathName)
        {
            // ReSharper disable once UnusedVariable
            var widCurrent = WindowsIdentity.GetCurrent();
            WindowsImpersonationContext wic = null;
            byte[] byteData = null;

            try
            {
                var adminToken = new IntPtr();

                //if (LogonUser("nas_staffpacks", "corp-ais900", "/O9|}yPS.1[t", 9, 0, ref admin_token) != 0)
                if (LogonUser(username, domin, pwd, 9, 0, ref adminToken) != 0)
                {

                    wic = new WindowsIdentity(adminToken).Impersonate();
                    if (File.Exists(sourceFullPathName))
                    {
                        byteData = File.ReadAllBytes(sourceFullPathName);
                    }
                }
            }
            // ReSharper disable once RedundantCatchClause
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (wic != null)
                {
                    wic.Undo();
                }
            }

            return byteData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="pwd"></param>
        /// <param name="domin"></param>
        /// <param name="sourceFullPathName"></param>
        /// <returns></returns>
        [PermissionSetAttribute(SecurityAction.Demand, Name = "FullTrust")]
        public bool TestConectionNas(string username, string pwd, string domin, string sourceFullPathName)
        {
            // ReSharper disable once UnusedVariable
            var widCurrent = WindowsIdentity.GetCurrent();
            WindowsImpersonationContext wic = null;
            var isSuccess = false;

            try
            {
                var adminToken = new IntPtr();
                if (LogonUser(username, domin, pwd, 9, 0, ref adminToken) != 0)
                {
                    wic = new WindowsIdentity(adminToken).Impersonate();
                    if (File.Exists(sourceFullPathName))
                    {
                    }
                    isSuccess = true;
                }
            }
            catch (Exception)
            {
                isSuccess = false;
            }
            finally
            {
                if (wic != null)
                {
                    wic.Undo();
                }
            }

            return isSuccess;
        }

        [PermissionSetAttribute(SecurityAction.Demand, Name = "FullTrust")]
        public bool WriteFile(string username, string pwd, string domin, string pathName, List<PAYGTransAirnetFileList> data, bool writeNewFile = true)
        {
            // ReSharper disable once UnusedVariable
            var widCurrent = WindowsIdentity.GetCurrent();
            WindowsImpersonationContext wic = null;
            var isSuccess = false;

            try
            {
                var adminToken = new IntPtr();
                if (LogonUser(username, domin, pwd, LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT, ref adminToken) != 0)
                {
                    wic = new WindowsIdentity(adminToken).Impersonate();

                    //string[] filelist = Directory.GetFiles(PrepareFile(pathName, ""));
                    //foreach (string files in filelist)
                    //{
                    //    File.Delete(PrepareFile(pathName, files));
                    //}

                    //isSuccess = GenerateFile(data, pathName, null);
                    if (writeNewFile)
                    {
                        isSuccess = GenerateFile(data, pathName, null);
                    }
                    else
                    {
                        isSuccess = PrepareFileContinue(pathName, null, data);
                    }
                    //this.RemoveTargetFileOverWeek(_archiveLocalPath);
                }
            }
            catch (Exception e)
            {
                _logger.Info("Generate File Destination Error! : " + e.Message);
                _logger.Info("Generate File Destination Error! : " + e.StackTrace);
                isSuccess = false;
            }

            return isSuccess;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="pwd"></param>
        /// <param name="domin"></param>
        /// <param name="sourceFullPathName"></param>
        /// <returns></returns>
        [PermissionSetAttribute(SecurityAction.Demand, Name = "FullTrust")]
        public static string ReadFileText(string username, string pwd, string domin, string sourceFullPathName)
        {
            // ReSharper disable once UnusedVariable
            var widCurrent = WindowsIdentity.GetCurrent();
            WindowsImpersonationContext wic = null;
            string textData = null;

            try
            {
                var adminToken = new IntPtr();

                //if (LogonUser("nas_staffpacks", "corp-ais900", "/O9|}yPS.1[t", 9, 0, ref admin_token) != 0)
                if (LogonUser(username, domin, pwd, 9, 0, ref adminToken) != 0)
                {

                    wic = new WindowsIdentity(adminToken).Impersonate();
                    if (File.Exists(sourceFullPathName))
                    {
                        textData = File.ReadAllText(sourceFullPathName);
                    }
                }
            }
            // ReSharper disable once RedundantCatchClause
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (wic != null)
                {
                    wic.Undo();
                }
            }

            return textData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="pwd"></param>
        /// <param name="domin"></param>
        /// <param name="sourceFullPathName"></param>
        /// <returns></returns>
        [PermissionSetAttribute(SecurityAction.Demand, Name = "FullTrust")]
        public static string[] GetAllFile(string username, string pwd, string domin, string sourceFullPathName)
        {
            WindowsImpersonationContext wic = null;
            string[] filelist = null;

            try
            {
                var adminToken = new IntPtr();
                if (LogonUser(username, domin, pwd, 9, 0, ref adminToken) != 0)
                {
                    wic = new WindowsIdentity(adminToken).Impersonate();

                    if (Directory.Exists(sourceFullPathName))
                    {
                        filelist = Directory.GetFiles(sourceFullPathName);
                    }
                }
            }
            // ReSharper disable once RedundantCatchClause
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (wic != null)
                {
                    wic.Undo();
                }
            }

            return filelist;
        }

        private bool GenerateFile(List<PAYGTransAirnetFileList> datas, string directoryPath, string fileName)
        {
            DateTime currDateTime = DateTime.Now;
            //string fileLogPath = PrepareFileCreateNotDelete(_archiveLogPath, currDateTime.ToString("yyyyMMdd"));
            //StreamWriter fileLog = new StreamWriter(fileLogPath, true);

            foreach (PAYGTransAirnetFileList data in datas)
            {
                // write file to target
                string finalFileNameWithPath = PrepareFile(directoryPath, data.file_name);
                //_logger.Info(string.Format("Target Path : {0}", finalFileNameWithPath));
                StreamWriter file = new StreamWriter(finalFileNameWithPath, true);
                file.WriteLine(data.file_data);
                file.Close();

                // write file to archive local
                //finalFileNameWithPath = PrepareFileCreate(_archiveLocalPath, data.file_name);
                //_logger.Info(string.Format("Local Path : {0}", finalFileNameWithPath));
                //file = new StreamWriter(finalFileNameWithPath, true);
                //file.WriteLine(data.file_data);
                //file.Close();

                // write file for check run file duplicate 
                //fileLog.WriteLine(data.file_name);
            }

            //fileLog.Close();

            return true;
        }

        private bool PrepareFileContinue(string pathName, string fileName, List<PAYGTransAirnetFileList> datas)
        {
            DateTime currDateTime = DateTime.Now;

            foreach (PAYGTransAirnetFileList data in datas)
            {
                string finalFileNameWithPath = PrepareFileCreateNotDelete(pathName, data.file_name);  
                //_logger.Info(string.Format("Target Path : {0}", pathName));
                using (StreamWriter fileAppend = File.AppendText(finalFileNameWithPath))
                {
                    fileAppend.WriteLine(data.file_data);
                    fileAppend.Close();
                }
            }
            return true;
        }

        private string PrepareFile(string directoryPath, string fileName)
        {
            string finalFileNameWithPath = Path.Combine(directoryPath, fileName);
            // Check Directory
            if (!Directory.Exists(directoryPath))
            {
                _logger.Info("Directory path not found -> " + directoryPath);
                return "N";
                //Directory.CreateDirectory(directoryPath)
            };
            // Check File Duplicate
            if (File.Exists(finalFileNameWithPath)) File.Delete(finalFileNameWithPath);

            return finalFileNameWithPath;
        }

        public string PrepareFileCreateNotDelete(string directoryPath, string fileName)
        {
            string finalFileNameWithPath = Path.Combine(directoryPath, fileName);
            _logger.Info($"PrepareFileCreateNotDelete path :{finalFileNameWithPath}");
            // Check Directory
            if (!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);

            if (File.Exists(finalFileNameWithPath))
            {
                return finalFileNameWithPath;
            }
            else
            {
                return finalFileNameWithPath = "";
            }

            //return finalFileNameWithPath;
        }

        protected void RemoveTargetFileOverWeek(string pathName)
        {
            DateTime currDateTime = DateTime.Now.AddDays(-_dayArchiveForDestination);
            string[] filelist = Directory.GetFiles(PrepareFile(pathName, ""));
            foreach (string files in filelist)
            {
                string fileName = Path.GetFileName(files);
                string[] strTemp = fileName.Split('_');
                string tempFileName = strTemp[3];
                var preDateTime = new DateTime(tempFileName.Substring(0, 4).ToSafeInteger(), tempFileName.Substring(4, 2).ToSafeInteger(), tempFileName.Substring(6, 2).ToSafeInteger());

                if (preDateTime < currDateTime)
                {
                    File.Delete(PrepareFile(pathName, files));
                }
            }

        }

    }
}
