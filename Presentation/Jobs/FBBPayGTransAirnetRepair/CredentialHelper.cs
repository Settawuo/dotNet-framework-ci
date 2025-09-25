using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Security.Principal;

namespace FBBPayGTransAirnetRepair
{
    using FBBPayGTransAirnetRepair.Model;
    using System.Collections.Generic;
    using System.Data;
    using System.IO;
    using System.Text;
    using WBBBusinessLayer;
    using WBBEntity.PanelModels.FBBWebConfigModels;

    public class CredentialHelper
    {

        public ILogger _logger;
        public CredentialHelper(ILogger logger)
        {
            _logger = logger;
        }

        string txtProcess = "process ->";



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



                    //File.Copy(sourceFullPathName, targetFullPathName, true);
                    //if (Directory.Exists(sourceFullPathName))
                    //{

                    //}

                    Console.WriteLine("{0} file copying.", txtProcess);
                    _logger.Info(string.Format("file copying."));

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
        public bool RemoveFile(string username, string pwd, string domin, string sourcePath)
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
                    File.Delete(sourcePath);
                    isRemoveSucess = true;
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
        [PermissionSetAttribute(SecurityAction.Demand, Name = "FullTrust")]
        public static bool TestConectionNas(string username, string pwd, string domin, string sourceFullPathName)
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
        public StringBuilder GetFileSAP(string username, string pwd, string domin, string sourceFullPathName, string archivePath, string rootFileName)
        {
            WindowsImpersonationContext wic = null;
            StringBuilder sb = new StringBuilder();
            try
            {
                var adminToken = new IntPtr();
                if (LogonUser(username, domin, pwd, LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT, ref adminToken) != 0)
                {
                    wic = new WindowsIdentity(adminToken).Impersonate();

                    if (Directory.Exists(sourceFullPathName))
                    {
                        //DateTime currDateTime = DateTime.Now;
                        //string dateNow = currDateTime.ToString("yyyyMMdd");
                        //string rootFileName = string.Format("{0}_{1}", "AIRNET_INV", dateNow);

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
                                File.Copy(files, PrepareFileCreate(archivePath, fileName), true);

                                // Temp File for DB call it
                                //string destFile = PrepareFile(tempPath, fileName);
                                //File.Copy(files, destFile, true);
                                SAPModel.copyList.Add(fileName);

                                sb = SBlist(sb, fileName, "|", c_list);
                                c_list = true;

                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("{0} sap path does not exist!", txtProcess);
                        _logger.Info("sap path does not exist!");
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
                    Console.WriteLine("{0} SAP path does not exist!", txtProcess);
                    _logger.Info("SAP path does not exist!");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("{0} Get file SAP => Exception : {1}", txtProcess, e.Message);
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
        public bool WriteFile(string username, string pwd, string domin, string pathName, string fileName, List<PAYGTransAirnetFileList> data)
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
                    _logger.Info(string.Format("Target Path : {0}", "aaa"));
                    isSuccess = GenerateFile(data, pathName, fileName);
                }
            }
            catch (Exception e)
            {
                _logger.Info("Generate File Destination Error! : " + e.Message);
                isSuccess = false;
            }

            return isSuccess;
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
            foreach (PAYGTransAirnetFileList data in datas)
            {
                string finalFileNameWithPath = PrepareFile(directoryPath, data.file_name);
                _logger.Info(string.Format("Target Path : {0}", finalFileNameWithPath));
                StreamWriter file = new StreamWriter(finalFileNameWithPath, true);
                file.WriteLine(data.file_data);
                file.Close();
            }
            return true;
        }

        private string PrepareFile(string directoryPath, string fileName)
        {
            string finalFileNameWithPath = Path.Combine(directoryPath, fileName);
            // Check Directory
            if (!Directory.Exists(directoryPath))
            {
                Console.WriteLine("{0} Directory path not found! {1}", txtProcess, directoryPath);
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

        private string PrepareFileNotDelete(string directoryPath, string fileName)
        {
            string finalFileNameWithPath = Path.Combine(directoryPath, fileName);
            // Check Directory
            if (!Directory.Exists(directoryPath))
            {
                Console.WriteLine("{0} Directory path not found! {1}", txtProcess, directoryPath);
                _logger.Info("Directory path not found -> " + directoryPath);
                return "N";
                //Directory.CreateDirectory(directoryPath)
            };

            return finalFileNameWithPath;
        }

    }
}
