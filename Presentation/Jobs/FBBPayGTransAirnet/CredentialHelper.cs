using System;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Security.Principal;

namespace FBBPayGTransAirnet
{
    using FBBPayGTransAirnet.Model;
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
        //private static List<string> copyList;

        //public static List<string> CopyList
        //{
        //    get { return CredentialHelper.copyList; }
        //    set { CredentialHelper.copyList = value; }
        //}

        const int LOGON32_PROVIDER_DEFAULT = 0;
        // This parameter causes LogonUser to create a primary token.
        const int LOGON32_LOGON_INTERACTIVE = 9;

        [DllImport("advapi32.DLL", SetLastError = true)]
        public static extern int LogonUser(string lpszUsername, string lpszDomain, string lpszPassword, int dwLogonType,
            int dwLogonProvider, ref IntPtr phToken);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public extern static bool CloseHandle(IntPtr handle);


        /// <summary>
        /// copy file with remote computer
        /// </summary>
        /// <param name="username">username for authenticate</param>
        /// <param name="pwd">password for authenticate</param>
        /// <param name="domin">domain name of user for authenticate</param>
        /// <param name="sourceFullPathName">source path and file name with file extension</param>
        /// <param name="targetFullPathName">target path and file name with file extension</param>
        ///
        [PermissionSetAttribute(SecurityAction.Demand, Name = "FullTrust")]
        public bool CopyFile(string username, string pwd, string domin, string sourceFullPathName, string targetFullPathName)
        {
            // ReSharper disable once UnusedVariable
            var widCurrent = WindowsIdentity.GetCurrent();
            WindowsImpersonationContext wic = null;
            var isCopySucess = false;

            try
            {
                var adminToken = new IntPtr();

                //if (LogonUser("nas_staffpacks", "corp-ais900", "/O9|}yPS.1[t", 9, 0, ref admin_token) != 0)
                if (LogonUser(username, domin, pwd, LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT, ref adminToken) != 0)
                {
                    wic = new WindowsIdentity(adminToken).Impersonate();

                    foreach (var arr in SAPModel.copyList)
                    {

                        string originFileNameWithPath = PrepareFileNotDelete(sourceFullPathName, arr);
                        if (originFileNameWithPath.Equals("N"))
                        {
                            return false;
                        }

                        string finalFileNameWithPath = PrepareFile(targetFullPathName, arr);
                        if (finalFileNameWithPath.Equals("N"))
                        {
                            return false;
                        }

                        DesModel.copyList.Add(finalFileNameWithPath);
                        File.Copy(originFileNameWithPath, finalFileNameWithPath, true);

                        //File.Delete(arr);
                    }

                    isCopySucess = true;
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
            return isCopySucess;
        }

        /// <summary>
        /// copy file
        /// </summary>
        /// <param name="sourceFullPathName">source path and file name with file extension</param>
        /// <param name="targetFullPathName">target path and file name with file extension</param>
        public static bool CopyFile(string sourceFullPathName, string targetFullPathName)
        {
            bool isCopySucess;

            try
            {
                File.Copy(sourceFullPathName, targetFullPathName, true);
                isCopySucess = true;
            }
            catch (Exception)
            {
                isCopySucess = false;
            }

            return isCopySucess;
        }

        /// <summary>
        /// copy file
        /// </summary>
        /// <param name="fileName">filename</param>
        /// <param name="sourcePath">source path</param>
        /// <param name="targetPath">target path</param>
        public static bool CopyFile(string fileName, string sourcePath, string targetPath)
        {
            bool isCopySucess;

            try
            {
                File.Copy(sourcePath + fileName, targetPath + fileName, true);
                isCopySucess = true;
            }
            catch (Exception)
            {
                isCopySucess = false;
            }

            return isCopySucess;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <returns></returns>
        public bool RemoveFile(string sourcePath)
        {
            bool isRemoveSucess;

            try
            {
                File.Delete(sourcePath);
                isRemoveSucess = true;
            }
            catch (Exception)
            {
                isRemoveSucess = false;
            }

            return isRemoveSucess;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="pwd"></param>
        /// <param name="domin"></param>
        /// <param name="sourcePath"></param>
        /// <returns></returns>
        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public bool RemoveFile(string username, string pwd, string domin, string nasArchivePath)
        {
            // ReSharper disable once UnusedVariable
            var widCurrent = WindowsIdentity.GetCurrent();
            // ReSharper disable once NotAccessedVariable
            WindowsImpersonationContext wic = null;
            bool isRemoveSucess;

            try
            {
                var adminToken = new IntPtr();

                if (LogonUser(username, domin, pwd, 9, 0, ref adminToken) != 0)
                {
                    // ReSharper disable once RedundantAssignment
                    wic = new WindowsIdentity(adminToken).Impersonate();

                    //File.Copy(@"D:\test.wsdl", @"\\10.239.109.211\staff_upload\test.wsdl", true);
                    //File.Delete(sourcePath);
                    isRemoveSucess = true;
                    //this.RemoveLocalFileOverWeek(_archivePath);
                    this.RemoveTargetFileOverWeek(nasArchivePath);
                }
                else
                {
                    isRemoveSucess = false;
                }
            }
            // ReSharper disable once RedundantCatchClause
            catch (Exception)
            {
                throw;
            }
            return isRemoveSucess;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="pwd"></param>
        /// <param name="domin"></param>
        /// <param name="sourcePath"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public bool RemoveFile(string username, string pwd, string domin, string sourcePath, string fileName)
        {
            // ReSharper disable once UnusedVariable
            var widCurrent = WindowsIdentity.GetCurrent();
            // ReSharper disable once NotAccessedVariable
            WindowsImpersonationContext wic = null;
            var isRemoveSucess = false;

            try
            {
                var adminToken = new IntPtr();
                if (LogonUser(username, domin, pwd, 9, 0, ref adminToken) != 0)
                {
                    // ReSharper disable once RedundantAssignment
                    wic = new WindowsIdentity(adminToken).Impersonate();
                    var path = Path.Combine(sourcePath, fileName);
                    if (File.Exists(path))
                    {
                        File.Delete(path);
                    }
                    isRemoveSucess = true;
                }
            }
            // ReSharper disable once RedundantCatchClause
            catch (Exception)
            {
                throw;
            }

            return isRemoveSucess;
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
        /// <param name="targetPathName"></param>
        /// <param name="fileName"></param>
        /// <param name="inputStream"></param>
        /// <returns></returns>
        [PermissionSetAttribute(SecurityAction.Demand, Name = "FullTrust")]
        public static bool CopyFileStream(string username, string pwd, string domin, string targetPathName, string fileName, Stream inputStream)
        {
            // ReSharper disable once UnusedVariable
            var widCurrent = WindowsIdentity.GetCurrent();
            WindowsImpersonationContext wic = null;
            var isCopySucess = false;
            try
            {

                var adminToken = new IntPtr();
                //if (LogonUser("nas_staffpacks", "corp-ais900", "/O9|}yPS.1[t", 9, 0, ref admin_token) != 0)
                if (LogonUser(username, domin, pwd, 9, 0, ref adminToken) != 0)
                {
                    wic = new WindowsIdentity(adminToken).Impersonate();
                    if (!Directory.Exists(targetPathName))
                    {
                        Directory.CreateDirectory(targetPathName);
                    }
                    var stream = inputStream;
                    var path = Path.Combine(targetPathName, fileName);
                    using (var fileStream = File.Create(path))
                    {
                        if (stream != null)
                        {
                            stream.CopyTo(fileStream);
                        }
                    }

                    isCopySucess = true;
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

            return isCopySucess;
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="pwd"></param>
        /// <param name="domin"></param>
        /// <param name="sourceFullPathName"></param>
        /// <returns></returns>
        //[PermissionSetAttribute(SecurityAction.Demand, Name = "FullTrust")]
        //public static bool TestConectionNas(string username, string pwd, string domin, string sourceFullPathName)
        //{
        //    // ReSharper disable once UnusedVariable
        //    var widCurrent = WindowsIdentity.GetCurrent();
        //    WindowsImpersonationContext wic = null;
        //    var isSuccess = false;

        //    try
        //    {
        //        var adminToken = new IntPtr();
        //        if (LogonUser(username, domin, pwd, 9, 0, ref adminToken) != 0)
        //        {
        //            wic = new WindowsIdentity(adminToken).Impersonate();
        //            if (File.Exists(sourceFullPathName))
        //            {
        //            }
        //            isSuccess = true;
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        isSuccess = false;
        //    }
        //    finally
        //    {
        //        if (wic != null)
        //        {
        //            wic.Undo();
        //        }
        //    }

        //    return isSuccess;
        //}


        //[PermissionSetAttribute(SecurityAction.Demand, Name = "FullTrust")]
        //public StringBuilder TestConectionNas(string username, string pwd, string domin, string sourceFullPathName)
        //{
        //    // ReSharper disable once UnusedVariable
        //    var widCurrent = WindowsIdentity.GetCurrent();
        //    WindowsImpersonationContext wic = null;
        //    StringBuilder sb = new StringBuilder();
        //    var isSuccess = false;

        //    try
        //    {
        //        var adminToken = new IntPtr();
        //        if (LogonUser(username, domin, pwd, LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT, ref adminToken) != 0)
        //        {
        //            wic = new WindowsIdentity(adminToken).Impersonate();
        //            string pathNewDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Temp/");

        //            if (!Directory.Exists(pathNewDirectory))
        //            {
        //                Directory.CreateDirectory(pathNewDirectory);
        //            }
        //            if (File.Exists(sourceFullPathName))
        //            {
        //                string[] files = Directory.GetFiles(sourceFullPathName);

        //                if (files.Length > 0)
        //                {
        //                    foreach (string file in files)
        //                    {
        //                        try
        //                        {
        //                            string fileName = Path.GetFileName(file);
        //                            string destFile = Path.Combine(pathNewDirectory, fileName);
        //                            //File.Copy(file, destFile, overwrite: true);

        //                            _logger.Info($"File downloaded successfully: {fileName}");
        //                            //sb.AppendLine($"Downloaded: {fileName}");
        //                        }
        //                        catch (Exception fileEx)
        //                        {
        //                            _logger.Error($"Error downloading file {file}: {fileEx.Message}");
        //                            //sb.AppendLine($"Failed: {file}");
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    _logger.Info("No files found in the SAP path.");
        //                }
        //                //isSuccess = true;
        //            }
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        isSuccess = false;
        //    }
        //    finally
        //    {
        //        if (wic != null)
        //        {
        //            wic.Undo();
        //        }
        //    }

        //    return sb;
        //}


        [PermissionSetAttribute(SecurityAction.Demand, Name = "FullTrust")]
        public StringBuilder ConnectNasSap(string username, string pwd, string domin, string sourceFullPathName)
        {
            WindowsImpersonationContext wic = null;
            StringBuilder sb = new StringBuilder();
            try
            {
                DateTime currDateTime = DateTime.Now.AddDays(-_dayCallback);
                var adminToken = new IntPtr();
                if (LogonUser(username, domin, pwd, LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT, ref adminToken) != 0)
                {
                    wic = new WindowsIdentity(adminToken).Impersonate();
                    string pathNewDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Temp/");
                    if (!Directory.Exists(pathNewDirectory))
                    {
                        Directory.CreateDirectory(pathNewDirectory);
                    }
                    if (Directory.Exists(sourceFullPathName))
                    {
                        string[] files = Directory.GetFiles(sourceFullPathName, "*.dat");
                        //var newfile = false;
                        //bool c_list = false;
                        //List<string> logs = fileLog(_dayCallback);

                        if (files.Length > 0)
                        {
                            foreach (string file in files)
                            {
                                try
                                {
                                    string fileName = Path.GetFileName(file);
                                    string[] strTemp = fileName.Split('_');
                                    string tempFileName = strTemp[3];
                                    var preDateTime = new DateTime(tempFileName.Substring(0, 4).ToSafeInteger(), tempFileName.Substring(4, 2).ToSafeInteger(), tempFileName.Substring(6, 2).ToSafeInteger());

                                    if (preDateTime >= currDateTime && strTemp[1] == "AIRNET")
                                    {
                                        //if (duplicateLog(logs, fileName))
                                        //{
                                            //string fileName = Path.GetFileName(file);
                                            string destFile = Path.Combine(pathNewDirectory, fileName);
                                            File.Copy(file, destFile, overwrite: true);

                                            //sb = SBlist(sb, fileName, "|", c_list);
                                            //c_list = true;
                                            _logger.Info($"Write file successfully: {fileName}");
                                        //}
                                    }
                                }
                                catch (Exception fileEx)
                                {
                                    _logger.Error($"Error write file {file}: {fileEx.Message}");
                                }
                            }
                            //if (newfile == false)
                            //    _logger.Info("File not found with condition");
                        }
                        else
                        {
                            _logger.Info("No .dat files found in the SAP path!");
                        }
                    }
                }
                else
                {
                    _logger.Info($"Cannot connect to SAP path: {sourceFullPathName}");
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Error ConnectNasSap: {ex.Message}");
            }
            finally
            {
                if (wic != null)
                    wic.Undo();
            }
            return sb;
        }




        [PermissionSetAttribute(SecurityAction.Demand, Name = "FullTrust")]
        public StringBuilder GetFileSAP(string username, string pwd, string domin, string sourceFullPathName, string archivePath)
        {
            WindowsImpersonationContext wic = null;
            StringBuilder sb = new StringBuilder();
            try
            {
                //cfg.Host = "10.252.160.101";
                //cfg.Port = 22;
                //cfg.UserName = "leardnk8";
                //cfg.KeyFile = "leardnk";

                var adminToken = new IntPtr();
                if (LogonUser(username, domin, pwd, LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT, ref adminToken) != 0)
                {
                    wic = new WindowsIdentity(adminToken).Impersonate();

                    if (Directory.Exists(sourceFullPathName))
                    {
                        bool c_list = false;
                        DateTime currDateTime = DateTime.Now.AddDays(-_dayCallback);
                        List<string> logs = fileLog(_dayCallback);
                        string[] filelist = Directory.GetFileSystemEntries(sourceFullPathName);
                        var newfile = false;
                        foreach (string files in filelist)
                        {
                            //1200_AIRNET_INV_20220623_210001.dat
                            string fileName = Path.GetFileName(files);
                            string[] strTemp = fileName.Split('_');
                            string tempFileName = strTemp[3];
                            var preDateTime = new DateTime(tempFileName.Substring(0, 4).ToSafeInteger(), tempFileName.Substring(4, 2).ToSafeInteger(), tempFileName.Substring(6, 2).ToSafeInteger());

                            if (preDateTime >= currDateTime)
                            {
                                if (duplicateLog(logs, fileName))
                                {
                                    newfile = true;
                                    // Backup File
                                    File.Copy(files, PrepareFileCreate(archivePath, fileName), true);

                                    SAPModel.copyList.Add(fileName);

                                    sb = SBlist(sb, fileName, "|", c_list);
                                    c_list = true;
                                }
                            }
                        }
                        if (newfile == false)
                            _logger.Info("File not found with condition");
                    }
                    else
                    {
                        _logger.Info("SAP path does not exist!");
                    }
                }
                else
                {
                    _logger.Info($"Can not connect SAP path :{sourceFullPathName}");
                }
            }
            // ReSharper disable once RedundantCatchClause
            catch (Exception ex)
            {
                _logger.Error($"Error GetFileSAP:{ex.Message}");
            }
            finally
            {
                if (wic != null)
                    wic.Undo();
            }
            return sb;
        }

        public StringBuilder GetFileSAP(string sourceFullPathName, string archivePath, string tempPath)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                if (Directory.Exists(sourceFullPathName))
                {
                    DateTime currDateTime = DateTime.Now;
                    string dateNow = currDateTime.ToString("yyyyMMdd");
                    string rootFileName = string.Format("{0}_{1}", "AIRNET_INV", dateNow);

                    bool c_list = false;

                    string[] filelist = Directory.GetFiles(sourceFullPathName);
                    foreach (string files in filelist)
                    {
                        string text = File.ReadAllText(files);
                        // Use static Path methods to extract only the file name from the path.
                        string fileName = Path.GetFileName(files);

                        string[] strTemp = fileName.Split('_');
                        string tempFileName = string.Format("{0}_{1}_{2}", strTemp[1], strTemp[2], strTemp[3]);
                        if (tempFileName == rootFileName)
                        {
                            // Backup File
                            File.Copy(files, PrepareFile(archivePath, fileName), true);

                            // Temp File for DB call it
                            string destFile = PrepareFile(tempPath, fileName);
                            File.Copy(files, destFile, true);
                            SAPModel.copyList.Add(destFile);

                            sb = SBlist(sb, fileName, "|", c_list);
                            c_list = true;
                        }
                    }
                }
                else
                {
                    _logger.Info("SAP path does not exist!");
                }
            }
            catch (Exception e)
            {
                _logger.Info("Get file SAP => Exception : " + e.Message);
            }

            return sb;
        }

        private static StringBuilder SBlist(StringBuilder sb, string data, string split, bool status)
        {
            if (status)
            {
                sb.Append(split + data);
                return sb;
            }
            sb.Append(data);
            return sb;
        }

        [PermissionSetAttribute(SecurityAction.Demand, Name = "FullTrust")]
        public bool WriteFile(string username, string pwd, string domin, string pathName, List<PAYGTransAirnetFileList> data)
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

                    string[] filelist = Directory.GetFiles(PrepareFile(pathName, ""));
                    foreach (string files in filelist)
                    {
                        File.Delete(PrepareFile(pathName, files));
                    }

                    isSuccess = GenerateFile(data, pathName, null);
                    this.RemoveTargetFileOverWeek(_archiveLocalPath);
                }
            }
            catch (Exception e)
            {
                _logger.Info("Generate File Destination Error! : " + e.Message);
                isSuccess = false;
            }

            return isSuccess;
        }

        public void RemoveTargetFileOverWeek(string pathName)
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

        protected void RemoveLocalFileOverWeek(string pathName)
        {
            DateTime currDateTime = DateTime.Now.AddDays(-_dayArchiveForOrigin);
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

        public bool WriteFile(string pathName, string fileName, List<PAYGTransAirnetFileList> data)
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

        private bool GenerateDATForStandAddress(DataTable data, string directoryPath, string fileName)
        {
            string finalFileNameWithPath = PrepareFile(directoryPath, fileName);
            StreamWriter file = new StreamWriter(finalFileNameWithPath, true);
            file.WriteLine("01|{0}", fileName);

            foreach (DataRow row in data.Rows)
            {
                object[] array = row.ItemArray;
                StringBuilder sb = new StringBuilder();
                sb.Append("02");
                for (var i = 0; i < array.Length; i++)
                {
                    var asdsad = array[i].ToString();
                    sb.Append(string.Format("|{0}", array[i].ToString()));
                }
                file.WriteLine(sb.ToString());
                //file.Write(array[i].ToString());
            }
            file.WriteLine("09|{0}", data.Rows.Count);
            file.Flush();
            file.Close();

            return true;
        }

        private bool GenerateFile(List<PAYGTransAirnetFileList> datas, string directoryPath, string fileName)
        {
            DateTime currDateTime = DateTime.Now;
            string fileLogPath = PrepareFileCreateNotDelete(_archiveLogPath, currDateTime.ToString("yyyyMMdd"));
            StreamWriter fileLog = new StreamWriter(fileLogPath, true);

            foreach (PAYGTransAirnetFileList data in datas)
            {
                // write file to target
                string finalFileNameWithPath = PrepareFile(directoryPath, data.file_name);
                _logger.Info(string.Format("Target Path : {0}", finalFileNameWithPath));
                StreamWriter file = new StreamWriter(finalFileNameWithPath, true);
                file.WriteLine(data.file_data);
                file.Close();

                // write file to archive local
                finalFileNameWithPath = PrepareFileCreate(_archiveLocalPath, data.file_name);
                _logger.Info(string.Format("Local Path : {0}", finalFileNameWithPath));
                file = new StreamWriter(finalFileNameWithPath, true);
                file.WriteLine(data.file_data);
                file.Close();

                // write file for check run file duplicate 
                fileLog.WriteLine(data.file_name);
            }

            fileLog.Close();

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

        private string PrepareFileCreate(string directoryPath, string fileName)
        {
            string finalFileNameWithPath = Path.Combine(directoryPath, fileName);
            // Check Directory
            if (!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);
            // Check File Duplicate
            if (File.Exists(finalFileNameWithPath)) File.Delete(finalFileNameWithPath);

            return finalFileNameWithPath;
        }

        private string PrepareFileCreateNotDelete(string directoryPath, string fileName)
        {
            string finalFileNameWithPath = Path.Combine(directoryPath, fileName);
            // Check Directory
            if (!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);

            return finalFileNameWithPath;
        }

        private string PrepareFileNotDelete(string directoryPath, string fileName)
        {
            string finalFileNameWithPath = Path.Combine(directoryPath, fileName);
            // Check Directory
            if (!Directory.Exists(directoryPath))
            {
                _logger.Info("Directory path not found -> " + directoryPath);
                return "N";
                //Directory.CreateDirectory(directoryPath)
            };

            return finalFileNameWithPath;
        }

        private List<string> fileLog(int pre)
        {

            List<string> preFile = new List<string>();

            DateTime currDateTime = DateTime.Now.AddDays(-pre);
            string[] filelist = Directory.GetFiles(PrepareFileCreate(_archiveLogPath, ""));
            foreach (string files in filelist)
            {
                string fileName = Path.GetFileName(files);
                var preDateTime = new DateTime(fileName.Substring(0, 4).ToSafeInteger(), fileName.Substring(4, 2).ToSafeInteger(), fileName.Substring(6, 2).ToSafeInteger());

                if (!(preDateTime < currDateTime))
                {
                    foreach (var list in File.ReadAllLines(files).ToList())
                    {
                        preFile.Add(list.ToString());
                    }
                }
                else File.Delete(files);
            }

            return preFile;

        }

        private bool duplicateLog(List<string> logs, string fileName)
        {
            foreach (string log in logs)
            {
                if (log == fileName) return false;
            }
            return true;
        }

        [PermissionSetAttribute(SecurityAction.Demand, Name = "FullTrust")]
        public StringBuilder placeFileTargetNas(string username, string pwd, string domin, string sourceFullPathName, List<string> skippedFiles)
        {
            StringBuilder sb = new StringBuilder();
            // ReSharper disable once UnusedVariable
            var widCurrent = WindowsIdentity.GetCurrent();
            WindowsImpersonationContext wic = null;

            try
            {
                var adminToken = new IntPtr();
                if (LogonUser(username, domin, pwd, 9, 0, ref adminToken) != 0)
                {
                    wic = new WindowsIdentity(adminToken).Impersonate();

                    string pathTempFile = Path.Combine(Directory.GetCurrentDirectory(), "Temp/");

                    _logger.Info($"Destination Path: {sourceFullPathName}");
                    if (Directory.Exists(sourceFullPathName))
                    {

                        var files = Directory.GetFiles(pathTempFile)
                                             .Where(file => !skippedFiles.Contains(Path.GetFileName(file)))
                                             .ToArray();// เอาเฉพาะไฟล์ที่ไม่ได้ถูก skip


                        foreach (var file in files)
                        {
                            try
                            {
                                var fileName = Path.GetFileName(file);
                                var destFilePath = Path.Combine(sourceFullPathName, fileName);
                                File.Copy(file, destFilePath, true);

                                _logger.Info($"Place File: {fileName} Successfully");
                            }
                            catch (Exception plcEx)
                            {
                                _logger.Info($"Error Place File: {plcEx.ToString()}");
                            }
                        }
                    }
                    else
                    {
                        _logger.Info($"Directory does not exist: {sourceFullPathName}");
                    }

                }
            }
            catch (Exception ex)
            {
                _logger.Info($"Place File In TargetNas Error : {ex.Message}");
            }
            finally
            {
                if (wic != null)
                {
                    wic.Undo();
                }
            }
            return sb;
        }

        [PermissionSetAttribute(SecurityAction.Demand, Name = "FullTrust")]
        public StringBuilder deleteFileTemp()
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                string pathTempFile = Path.Combine(Directory.GetCurrentDirectory(), "Temp/");

                if (Directory.Exists(pathTempFile))
                {

                    var files = Directory.GetFiles(pathTempFile);

                    _logger.Info($"Delete Temp Path: {pathTempFile}");
                    foreach (var file in files)
                    {
                        try
                        {
                            var fileName = Path.GetFileName(file);
                            File.Delete(file);

                            _logger.Info($"Delete File: {fileName} Successfully");
                        }
                        catch (Exception plcEx)
                        {
                            _logger.Info($"Error Delete File: {plcEx.ToString()}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Info($"Place File In TargetNas Error : {ex.Message}");
            }
            return sb;
        }


    }
}
