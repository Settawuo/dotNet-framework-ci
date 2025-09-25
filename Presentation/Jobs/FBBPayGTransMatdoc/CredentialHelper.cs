using System;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Security.Principal;

namespace FBBPayGTransMatdoc
{
    using FBBPayGTransMatdoc.Model;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Text;
    using WBBBusinessLayer;
    using WBBEntity.Extensions;
    using WBBEntity.PanelModels.FBBWebConfigModels;
    using System;
    using System.Data;
    using WBBContract;
    using WBBContract.Commands;
    using System.Linq;
    using WBBContract.Queries.FBBWebConfigQueries;
    using WBBEntity.PanelModels;
    using WBBContract.Queries.Commons.Masters;
    using WBBContract.Queries.FBBWebConfigQueries.SAPOnline;

    public class CredentialHelper
    {

        public ILogger _logger;
        private readonly ICommandHandler<InsertTranMatDocLoadFileLogCommand> _addLoadFileLogCommand;
        private readonly IQueryProcessor _queryProcessor;
        public CredentialHelper(
            ILogger logger,
            IQueryProcessor queryProcessor,
            ICommandHandler<InsertTranMatDocLoadFileLogCommand> addLoadFileLogCommand)
        {
            _logger = logger;
            _addLoadFileLogCommand = addLoadFileLogCommand;
            _queryProcessor = queryProcessor;
        }


        protected int _dayCallback = ConfigurationManager.AppSettings["DayCallback"].ToSafeInteger();
        protected int _dayArchiveForOrigin = ConfigurationManager.AppSettings["DayArchiveForOrigin"].ToSafeInteger();
        //protected int _dayArchiveForDestination = ConfigurationManager.AppSettings["DayArchiveForDestination"].ToSafeInteger();
        protected string _archivePath = ConfigurationManager.AppSettings["archive"].ToSafeString();
        //protected string _archiveLogPath = ConfigurationManager.AppSettings["archiveLog"].ToSafeString();
        //protected string _archiveLocalPath = ConfigurationManager.AppSettings["archiveLocal"].ToSafeString();
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
                    this.RemoveLocalFileOverWeek(_archivePath);
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

        //[PermissionSetAttribute(SecurityAction.Demand, Name = "FullTrust")]
        //public StringBuilder GetFileSAP(string username, string pwd, string domin, string sourceFullPathName, string archivePath)
        //{
        //    WindowsImpersonationContext wic = null;
        //    StringBuilder sb = new StringBuilder();
        //    try
        //    {
        //        //cfg.Host = "10.252.160.101";
        //        //cfg.Port = 22;
        //        //cfg.UserName = "leardnk8";
        //        //cfg.KeyFile = "leardnk";

        //        var adminToken = new IntPtr();
        //        if (LogonUser(username, domin, pwd, LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT, ref adminToken) != 0)
        //        {
        //            wic = new WindowsIdentity(adminToken).Impersonate();

        //            if (Directory.Exists(sourceFullPathName))
        //            {
        //                bool c_list = false;
        //                DateTime currDateTime = DateTime.Now.AddDays(-_dayCallback);
        //                List<string> logs = fileLog(_dayCallback);
        //                string[] filelist = Directory.GetFileSystemEntries(sourceFullPathName);
        //                var newfile = false;
        //                foreach (string files in filelist)
        //                {
        //                    //1200_AIRNET_INV_20220623_210001.dat
        //                    string fileName = Path.GetFileName(files);
        //                    string[] strTemp = fileName.Split('_');
        //                    string tempFileName = strTemp[3];
        //                    var preDateTime = new DateTime(tempFileName.Substring(0, 4).ToSafeInteger(), tempFileName.Substring(4, 2).ToSafeInteger(), tempFileName.Substring(6, 2).ToSafeInteger());

        //                    if (preDateTime >= currDateTime)
        //                    {
        //                        if (duplicateLog(logs, fileName))
        //                        {
        //                            newfile = true;
        //                            // Backup File
        //                            File.Copy(files, PrepareFileCreate(archivePath, fileName), true);

        //                            SAPModel.copyList.Add(fileName);

        //                            sb = SBlist(sb, fileName, "|", c_list);
        //                            c_list = true;
        //                        }
        //                    }
        //                }
        //                if (newfile == false)
        //                    _logger.Info("File not found with condition");
        //            }
        //            else
        //            {
        //                _logger.Info("SAP path does not exist!");
        //            }
        //        }
        //        else
        //        {
        //            _logger.Info($"Can not connect SAP path :{sourceFullPathName}");
        //        }
        //    }
        //    // ReSharper disable once RedundantCatchClause
        //    catch (Exception ex)
        //    {
        //        _logger.Error($"Error GetFileSAP:{ex.Message}");
        //    }
        //    finally
        //    {
        //        if (wic != null)
        //            wic.Undo();
        //    }
        //    return sb;
        //}







        public StringBuilder GetFileSAP(string sourceFullPathName, string archivePath, string tempPath)
        {

            StringBuilder sb = new StringBuilder();
            try
            {

                var widCurrent = WindowsIdentity.GetCurrent();
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

        public StringBuilder GetFileSAPNEWs4(string username, string pwd, string domin, string sourceFullPathName, string archivePath, string tempPath)
        {
            StringBuilder sb = new StringBuilder();
            List<string> failtFilesList = new List<string>();
            List<string> exceededLimitDayFilesList = new List<string>();
            List<string> historyDownloadFileList = new List<string>();
            List<string> finallyFilesList = new List<string>();

            var widCurrent = WindowsIdentity.GetCurrent();
            // ReSharper disable once NotAccessedVariable
            InterfaceLogCommand log = new InterfaceLogCommand();
            WindowsImpersonationContext wic = null;
            List<string> resp = new List<string>();
            bool isRemoveSucess;

            string returnStatusfile = "";
            string downloadSuccess = "";
            string uploadFail = "";
            string uploadSuccess = "";


            var adminToken = new IntPtr();
            if (LogonUser(username, domin, pwd, 9, 0, ref adminToken) != 0)
            {
                wic = new WindowsIdentity(adminToken).Impersonate();

                try
                {
                    if (Directory.Exists(sourceFullPathName))
                    {
                        int comDayCallback = 0;
                        try
                        {
                            comDayCallback = ConfigurationManager.AppSettings["DayCallback"].ToSafeInteger();
                        }
                        catch (Exception ex)
                        {
                            _logger.Info("DayCallback exception: " + ex + "");
                        }

                        //getLovFilterList
                        var lovFilterList = GetLovFilterList("PAYG_TRANS_MATDOC", "");

                        //read-row-by-filter-list
                        List<string> con_name_list = new List<string>();
                        List<string> comCodeListMain = new List<string>();

                        foreach (var CON_NAME in lovFilterList.Select((value, i) => new { i, value }))
                        {
                            //CON_NAME
                            var value = CON_NAME.value.CON_NAME;
                            var index = CON_NAME.i;
                            con_name_list.Add(value);
                        }
                        //distinct
                        con_name_list = con_name_list.Distinct().ToList();

                        foreach (var DISPLAY_VAL in lovFilterList.Select((value, i) => new { i, value }))
                        {
                            //DISPLAY_VAL
                            if (DISPLAY_VAL.value.DISPLAY_VAL == "company_code")
                            {

                                string[] strVALtemp = DISPLAY_VAL.value.VAL2.Split(',');

                                for (int indexSTR = 0; indexSTR < strVALtemp.Count(); indexSTR++)
                                {
                                    //comCodeList tempcomCodeList = new comCodeList();
                                    var comcode = strVALtemp[indexSTR];
                                    //tempcomCodeList.rowCount = 0;
                                    comCodeListMain.Add(comcode);
                                }


                            }
                            //var = DISPLAY_VAL.value.VAL2;
                            //var index = DISPLAY_VAL.i;
                            //mCodeListMain.Add(value);
                        }
                        //distinct
                        comCodeListMain = comCodeListMain.Distinct().ToList();


                        //string currDateAtFile = "" + strTemp[1].Substring(0, 4) + strTemp[1].Substring(4, 2) + strTemp[1].Substring(6, 2) + "";
                        //string currTimeAtFile = "" + strTemp[1].Substring(8, 6) + "";

                        //var comDayCallback = 7;
                        DateTime currDateTime = DateTime.Now;
                        DateTime advanceCurrDateTime = DateTime.Now.AddDays(-comDayCallback).Date;

                        string dateNow = currDateTime.ToString("yyyyMMdd");
                        string timenow = DateTime.Now.ToString("hhmmss");
                        //MATDOC-20241016145930-7
                        string rootFileName = string.Format("{0}-{1}", "MATDOC", dateNow);

                        //bool c_list = false;
                        //sourceFullPathName = "C:\\MATdoc\\S4";
                        string[] filelist = Directory.GetFiles(sourceFullPathName);
                        int fileCount = filelist.Length;

                        var matdocFiles = filelist
                        .Where(f =>
                        Path.GetFileName(f).StartsWith("MATDOC-", StringComparison.OrdinalIgnoreCase) &&
                        File.GetLastWriteTime(f) >= advanceCurrDateTime
                         )
                         .ToList();


                        string newFileFlag = "N";
                        if (matdocFiles.Count == 0)
                        {
                            _logger.Info("No file in path directory");
                            //set empty status
                            newFileFlag = "E";

                        }


                        //int flag1200 = 0;
                        //int flag1300 = 0;
                        //int flag1800 = 0;
                        //int JobDoneflag = 0;
                        //string newFileFlag = "N";
                        //string Checknullflag = "0";

                        foreach (string files in matdocFiles)
                        {
                            //Checknullflag = "0";
                            string text = File.ReadAllText(files);
                            // Use static Path methods to extract only the file name from the path.
                            string fileName = Path.GetFileName(files);
                            string comDayCallbacks = comDayCallback.ToString();
                            string existFileStatus = GetExistFileStatus(fileName, comDayCallbacks);

                            string[] strTemp = fileName.Split('-');
                            string tempFileName = string.Format("{0}-{1}-{2}", strTemp[0], strTemp[1], strTemp[2]);

                            string currDateAtFile = "" + strTemp[1].Substring(0, 4) + strTemp[1].Substring(4, 2) + strTemp[1].Substring(6, 2) + "";
                            string currTimeAtFile = "" + strTemp[1].Substring(8, 6) + "";

                            //check-file-exist-status
                            if (strTemp[0] == "MATDOC" && tempFileName.ToLower().EndsWith(".dat") && existFileStatus == "NEW")
                            {
                                string tempdate = strTemp[1];
                                var preDateTime = new DateTime(strTemp[1].Substring(0, 4).ToSafeInteger(), strTemp[1].Substring(4, 2).ToSafeInteger(), strTemp[1].Substring(6, 2).ToSafeInteger());
                                //campare-date-callback
                                //     if (preDateTime >= advanceCurrDateTime)
                                //    {
                                if (fileName.ToLower().EndsWith(".dat"))
                                {
                                    //string existFileStatus = GetExistFileStatus(fileName);
                                    if (existFileStatus == "NEW")
                                    {
                                        //do_normal_step_to_process_file
                                        newFileFlag = "Y";
                                    }
                                    else if (existFileStatus == "EXIST")
                                    {
                                        InsertLoadFileGenLog(fileName, "File already Exist-Read Success", "Y");
                                    }
                                    else
                                    {
                                        _logger.Info("Get exist file status => Exception : Y? N? ");
                                    }
                                }

                                // Temp File for DB call it
                                string destFile = PrepareFile(tempPath, fileName);
                                File.Copy(files, destFile, true);
                                SAPModel.copyList.Add(destFile);

                                //sb = SBlist(sb, fileName, "|", c_list);
                                //c_list = true;

                                string sbTextResult = text;
                                int checkNulllineCount = 0;
                                //read-row
                                string[] lineResultList = sbTextResult.Split(new[] { '\n' });
                                for (int resultListi = 0; resultListi < lineResultList.Count(); resultListi++)
                                {
                                    string templineResultList = lineResultList[resultListi].Replace("\r", "");

                                    if (templineResultList != "" && templineResultList != null)
                                    {
                                        string[] MandatorylineResultList = templineResultList.Split(new[] { '|' });
                                        //int mandatoryPassFlag = 0;

                                        try
                                        {
                                            for (int indexConnameList = 0; indexConnameList < con_name_list.Count(); indexConnameList++)
                                            {
                                                //for-by-con-name
                                                for (int indexFilterList = 0; indexFilterList < lovFilterList.Count(); indexFilterList++)
                                                {
                                                    if (lovFilterList[indexFilterList].DISPLAY_VAL == "company_code")
                                                    {
                                                        //check-same-Con-name
                                                        if (lovFilterList[indexFilterList].CON_NAME == con_name_list[indexConnameList])
                                                        {
                                                            //Filter
                                                            string[] strVAL2 = lovFilterList[indexFilterList].VAL2.Split(',');

                                                            int mandorotyIndex = lovFilterList[indexFilterList].VAL1.ToSafeInteger();

                                                            for (int indexSTRval2 = 0; indexSTRval2 < strVAL2.Count(); indexSTRval2++)
                                                            {
                                                                if (strVAL2[indexSTRval2] == MandatorylineResultList[mandorotyIndex])
                                                                {
                                                                    //Pass-check-mandatory
                                                                    //mandatoryPassFlag = 1;

                                                                    try
                                                                    {

                                                                        string nameMatdocMixGen = strVAL2[indexSTRval2] + "_AIRNET_INV_" + currDateAtFile + "_" + currTimeAtFile + ".dat";
                                                                        //createfile
                                                                        List<PAYGTransAirnetFileList> DataMix = new List<PAYGTransAirnetFileList>();
                                                                        PAYGTransAirnetFileList firstRowDataTemp = new PAYGTransAirnetFileList();
                                                                        firstRowDataTemp.file_data = "01|" + nameMatdocMixGen + "";
                                                                        firstRowDataTemp.file_name = nameMatdocMixGen;

                                                                        string finalFileNameWithPath = PrepareFileCreateNotDelete(tempPath, nameMatdocMixGen);
                                                                        string existfilepathMix = finalFileNameWithPath;
                                                                        if (!File.Exists(existfilepathMix))
                                                                        {
                                                                            DataMix.Add(firstRowDataTemp);
                                                                            WriteFile(username, pwd, domin, tempPath, nameMatdocMixGen, DataMix);
                                                                            _logger.Info("Create File name: ---->" + nameMatdocMixGen + "");
                                                                        }

                                                                    }
                                                                    catch
                                                                    {
                                                                        _logger.Info("File name: " + fileName + "Failed to Create");
                                                                    }
                                                                }
                                                            }

                                                        }
                                                    }

                                                }
                                            }

                                        }
                                        catch (Exception ex)
                                        {
                                            _logger.Info("exception: " + ex + "");
                                            //Checknullflag = "1";
                                        }
                                    }
                                    else
                                    {
                                        //flag-null
                                        //Checknullflag = "1";
                                        checkNulllineCount++;

                                    }

                                }

                                //check-equal-null-line-count
                                if (checkNulllineCount == lineResultList.Count())
                                {
                                    //Checknullflag = "1";
                                    //if (rowMATDOCcount == 0 && Checknullflag == "1")
                                    //{
                                    InsertLoadFileGenLog(fileName, "Data not found", "N");
                                    //}
                                }

                                //    }
                                //    else
                                //    {

                                //    _logger.Info("File name: " + fileName + " Exception Message: Date range exceeded " + comDayCallback + " days limit");
                                //   }

                            }
                            else
                            {
                                _logger.Info("File name: " + tempFileName + ", already exist file or invalid file type");
                            }


                        }

                        if (newFileFlag == "N")
                        {
                            _logger.Info("File not found.");
                            //InsertLoadFileGenLog(fileName, DateTime.Now, "invalid data", "N");
                        }

                        //temp-Path
                        string[] filelistTemp = Directory.GetFiles(tempPath);
                        List<string> listfilelistTemp = filelistTemp.ToList();
                        //delete-file-list
                        List<string> deleteFailFileList = new List<string>();


                        //wrtieLine
                        foreach (string files in filelistTemp)
                        {
                            string fileName = Path.GetFileName(files);
                            string[] strTemp = fileName.Split('-');

                            if (strTemp.Count() > 1)
                            {
                                string tempFileName = string.Format("{0}-{1}-{2}", strTemp[0], strTemp[1], strTemp[2]);

                                string currDateAtFile = "" + strTemp[1].Substring(0, 4) + strTemp[1].Substring(4, 2) + strTemp[1].Substring(6, 2) + "";
                                string currTimeAtFile = "" + strTemp[1].Substring(8, 6) + "";

                                string text = File.ReadAllText(files);
                                // Use static Path methods to extract only the file name from the path.
                                //string fileName = Path.GetFileName(files);

                                string[] strTempMatdocCheck = fileName.Split('-');
                                //rowcount
                                //int RowMixcount = 0;

                                //int Row1300count = 0;
                                //int Row1800count = 0;
                                //existpathfile
                                //string existfilepath1200 = "";
                                //string existfilepath1300 = "";
                                //string existfilepath1800 = "";

                                if (strTempMatdocCheck[0] == "MATDOC" && fileName.ToLower().EndsWith(".dat"))
                                {
                                    int nullFlag = 1;
                                    int rowMATDOCcount = 0;
                                    //string[] strTemp = fileName.Split('-');
                                    //string tempFileName = string.Format("{0}-{1}-{2}", strTemp[0], strTemp[1], strTemp[2]);

                                    string tempdate = strTemp[1];
                                    var preDateTime = new DateTime(strTemp[1].Substring(0, 4).ToSafeInteger(), strTemp[1].Substring(4, 2).ToSafeInteger(), strTemp[1].Substring(6, 2).ToSafeInteger());
                                    //camparedatecallback
                                    if (preDateTime >= advanceCurrDateTime)
                                    {
                                        int validflag = 99;

                                        string sbTextResult = text;
                                        //var result = sbTextResult.Split(new[] { '\r', '\n' });
                                        string[] lineResultList = sbTextResult.Split(new[] { '\n' });

                                        //Loop-by-Row
                                        for (int resultListi = 0; resultListi < lineResultList.Count(); resultListi++)
                                        {
                                            string[] MandatorylineResultList = lineResultList[resultListi].Split(new[] { '|' });

                                            try
                                            {
                                                //check-lenght
                                                if (MandatorylineResultList.Length >= 88)
                                                {

                                                    string passconNumber = "";
                                                    string sbTextData = "";
                                                    int DeleteFlag = 0;
                                                    nullFlag = 1;
                                                    string ConName = "";
                                                    //filter-value

                                                    //15-Movement Type
                                                    string movementTypeTextData = "";
                                                    //87
                                                    string indexTextDataSerialNumber = "";
                                                    //5
                                                    string indexTextDataUpdateDate = "";
                                                    //16
                                                    string indexTextDataMatNumber = "";
                                                    //52-Company Code
                                                    string indexTextDataCompCode = "";
                                                    //18-Plant
                                                    string indexTextDataPlant = "";
                                                    //641-58
                                                    //642-19
                                                    string indexTextDataReciveStrg = "";
                                                    //77
                                                    string indexTextDataShipTo = "";
                                                    //22
                                                    string stockTypeTextData = "";

                                                    string debit_credit = "";

                                                    for (int indexConnameList = 0; indexConnameList < con_name_list.Count(); indexConnameList++)
                                                    {

                                                        //for-by-con-name
                                                        string flagHasMatCode = "N";


                                                        string flagdebit_credit = "N";


                                                        for (int indexFilterList = 0; indexFilterList < lovFilterList.Count(); indexFilterList++)
                                                        {
                                                            //check-same-Con-name
                                                            if (lovFilterList[indexFilterList].CON_NAME == con_name_list[indexConnameList])
                                                            {
                                                                //Filter
                                                                string[] strVAL2 = lovFilterList[indexFilterList].VAL2.Split(',');

                                                                int mandorotyIndex = lovFilterList[indexFilterList].VAL1.ToSafeInteger();

                                                                for (int indexSTRval2 = 0; indexSTRval2 < strVAL2.Count(); indexSTRval2++)
                                                                {

                                                                    if (strVAL2[indexSTRval2] == MandatorylineResultList[mandorotyIndex])
                                                                    {
                                                                        //Pass-check-mandatory
                                                                        //mandatoryPassFlag = 1;

                                                                        try
                                                                        {
                                                                            if (lovFilterList[indexFilterList].DISPLAY_VAL == "company_code")
                                                                            {
                                                                                passconNumber = strVAL2[indexSTRval2];
                                                                            }

                                                                            //keep-sb-text
                                                                            if (lovFilterList[indexFilterList].DISPLAY_VAL == "plant")
                                                                            {
                                                                                indexTextDataPlant = MandatorylineResultList[mandorotyIndex];
                                                                            }

                                                                            if (lovFilterList[indexFilterList].DISPLAY_VAL == "movement_type")
                                                                            {
                                                                                movementTypeTextData = MandatorylineResultList[mandorotyIndex];
                                                                            }

                                                                            if (lovFilterList[indexFilterList].DISPLAY_VAL == "stock_type")
                                                                            {
                                                                                stockTypeTextData = MandatorylineResultList[mandorotyIndex];
                                                                            }

                                                                        }
                                                                        catch
                                                                        {
                                                                            _logger.Info("File name: " + fileName + "Failed to Create");
                                                                        }
                                                                    }
                                                                    if (lovFilterList[indexFilterList].DISPLAY_VAL == "debit_credit")
                                                                    {
                                                                        flagdebit_credit = "Y";
                                                                        if (MandatorylineResultList[mandorotyIndex] == lovFilterList[indexFilterList].VAL2)
                                                                        {
                                                                            debit_credit = MandatorylineResultList[mandorotyIndex];
                                                                        }
                                                                    }
                                                                    if (lovFilterList[indexFilterList].DISPLAY_VAL == "material_code")
                                                                    {
                                                                        flagHasMatCode = "Y";
                                                                        bool is16NumericCheck = long.TryParse(MandatorylineResultList[mandorotyIndex], out long n);

                                                                        if (is16NumericCheck == true)
                                                                        {
                                                                            //indexTextDataMatNumber = CompanylineResultList[16];
                                                                            long _indexTextDataMatNumber = long.Parse(MandatorylineResultList[mandorotyIndex]);
                                                                            //indexTextDataMatNumber = _indexTextDataMatNumber.ToSafeString();
                                                                            if (_indexTextDataMatNumber.ToSafeString() == strVAL2[indexSTRval2])
                                                                            {
                                                                                indexTextDataMatNumber = _indexTextDataMatNumber.ToSafeString();
                                                                            }

                                                                        }
                                                                        else if (is16NumericCheck == false)
                                                                        {
                                                                            //string _indexTextDataMatNumberString = CompanylineResultList[16].Substring(Math.Max(0, CompanylineResultList[16].Length - 1));
                                                                            //int _indexTextDataMatNumber = Int32.Parse(CompanylineResultList[16]);
                                                                            //indexTextDataMatNumber = _indexTextDataMatNumber.ToSafeString() + _indexTextDataMatNumberString;

                                                                            //indexTextDataMatNumber = MandatorylineResultList[mandorotyIndex];
                                                                            if (MandatorylineResultList[mandorotyIndex] == strVAL2[indexSTRval2])
                                                                            {
                                                                                indexTextDataMatNumber = MandatorylineResultList[mandorotyIndex];
                                                                            }
                                                                        }

                                                                        //indexTextDataMatNumber = MandatorylineResultList[mandorotyIndex];
                                                                    }

                                                                    //set-con-name
                                                                    ConName = lovFilterList[indexFilterList].CON_NAME;
                                                                }

                                                            }


                                                        }


                                                        if (flagdebit_credit == "N")
                                                        {
                                                            debit_credit = MandatorylineResultList[35];

                                                        }

                                                        if (flagHasMatCode == "N")
                                                        {
                                                            bool is16NumericCheck = long.TryParse(MandatorylineResultList[16], out long n);

                                                            if (is16NumericCheck == true)
                                                            {
                                                                long _indexTextDataMatNumber = long.Parse(MandatorylineResultList[16]);
                                                                indexTextDataMatNumber = _indexTextDataMatNumber.ToSafeString();

                                                            }
                                                            else if (is16NumericCheck == false)
                                                            {
                                                                indexTextDataMatNumber = MandatorylineResultList[16];
                                                            }
                                                        }

                                                        //if (passconNumber != "" && indexTextDataMatNumber != "" && indexTextDataPlant != "" && stockTypeTextData != "" && movementTypeTextData != "")
                                                        //{
                                                        //    ConName = lovFilterList[indexFilterList].CON_NAME
                                                        //}

                                                        if (passconNumber == "" || indexTextDataMatNumber == "" || indexTextDataPlant == "" || stockTypeTextData == "" || movementTypeTextData == "" || debit_credit == "")
                                                        {
                                                            passconNumber = "";
                                                            //15-Movement Type
                                                            movementTypeTextData = "";
                                                            //87
                                                            indexTextDataSerialNumber = "";
                                                            //5
                                                            indexTextDataUpdateDate = "";
                                                            //16
                                                            indexTextDataMatNumber = "";
                                                            //52-Company Code
                                                            indexTextDataCompCode = "";
                                                            //18-Plant
                                                            indexTextDataPlant = "";
                                                            //641-58
                                                            //642-19
                                                            indexTextDataReciveStrg = "";
                                                            //77
                                                            indexTextDataShipTo = "";
                                                            //22
                                                            stockTypeTextData = "";

                                                            debit_credit = "";

                                                            ConName = "";
                                                        }
                                                        else
                                                        {
                                                            break;
                                                        }

                                                    }



                                                    //old-logic
                                                    if (MandatorylineResultList[87] != null)
                                                    {
                                                        //indexTextDataSerialNumber = CompanylineResultList[87];
                                                        bool is87NumericCheck = long.TryParse(MandatorylineResultList[87], out long num);

                                                        if (is87NumericCheck == true)
                                                        {
                                                            long _indexTextDataMatNumber = long.Parse(MandatorylineResultList[87]);
                                                            indexTextDataSerialNumber = _indexTextDataMatNumber.ToSafeString();
                                                        }
                                                        else if (is87NumericCheck == false)
                                                        {
                                                            indexTextDataSerialNumber = MandatorylineResultList[87];
                                                        }
                                                    }

                                                    if (MandatorylineResultList[5] != null)
                                                    {
                                                        //indexTextDataUpdateDate = CompanylineResultList[5];
                                                        //change-format-from-dd.mm.yyyy
                                                        string[] strDateTemp = MandatorylineResultList[5].Split('.');
                                                        string tempDateFormetnew = string.Format("{2}{1}{0}", strDateTemp[0], strDateTemp[1], strDateTemp[2]);
                                                        indexTextDataUpdateDate = tempDateFormetnew;
                                                    }

                                                    if (MandatorylineResultList[52] != null)
                                                    {
                                                        indexTextDataCompCode = MandatorylineResultList[52];
                                                    }

                                                    //CheckCode641-642
                                                    //if(indexTextDataPlant == "")
                                                    //{

                                                    //}

                                                    if (MandatorylineResultList[15] != null)
                                                    {
                                                        if (MandatorylineResultList[15] == "641")
                                                        {
                                                            if (MandatorylineResultList[57] != null && indexTextDataPlant != "")
                                                            {
                                                                indexTextDataPlant = MandatorylineResultList[57];
                                                            }

                                                            if (MandatorylineResultList[58] != null)
                                                            {
                                                                indexTextDataReciveStrg = MandatorylineResultList[58];
                                                            }
                                                        }
                                                        else if (MandatorylineResultList[15] == "642")
                                                        {

                                                            if (MandatorylineResultList[18] != null && indexTextDataPlant != "")
                                                            {
                                                                indexTextDataPlant = MandatorylineResultList[18];
                                                            }

                                                            if (MandatorylineResultList[19] != null)
                                                            {
                                                                indexTextDataReciveStrg = MandatorylineResultList[19];
                                                            }
                                                        }
                                                        else if (MandatorylineResultList[15] == "311")
                                                        {

                                                            if (MandatorylineResultList[18] != null && indexTextDataPlant != "")
                                                            {
                                                                indexTextDataPlant = MandatorylineResultList[18];
                                                            }

                                                            if (MandatorylineResultList[19] != null)
                                                            {
                                                                indexTextDataReciveStrg = MandatorylineResultList[19];
                                                            }
                                                        }
                                                    }

                                                    if (MandatorylineResultList[77] != null)
                                                    {
                                                        indexTextDataShipTo = MandatorylineResultList[77];
                                                    }


                                                    if (indexTextDataSerialNumber == "" || indexTextDataUpdateDate == "" || indexTextDataMatNumber == "" || indexTextDataCompCode == "" || indexTextDataPlant == "" || indexTextDataReciveStrg == "" || stockTypeTextData == "" || ConName == "")
                                                    {
                                                        //has-empty-value-set-true-delete-flag
                                                        DeleteFlag = 1;
                                                        //validflag = 0;
                                                        _logger.Info("invalid data at Filename : " + fileName + " - Serial Number row : " + indexTextDataSerialNumber + "");
                                                        nullFlag = 0;
                                                        //_logger.Info("invalid data filename : " + fileName + " - Serial Number : " + indexTextDataSerialNumber + "- indexTextDataSerialNumber =" + indexTextDataSerialNumber + "- indexTextDataUpdateDate =" + indexTextDataUpdateDate + "- indexTextDataMatNumber =" + indexTextDataMatNumber + "- indexTextDataCompCode =" + indexTextDataCompCode + "- indexTextDataPlant =" + indexTextDataPlant + "- indexTextDataReciveStrg =" + indexTextDataReciveStrg + "- stockTypeTextData =" + stockTypeTextData);
                                                    }
                                                    else
                                                    {
                                                        DeleteFlag = 0;
                                                        //nullFlag = 0;
                                                    }

                                                    if (DeleteFlag == 0)
                                                    {
                                                        //Serial No|Updated Date|Material Code|Company Code|Plant|Storage Location|Action|Ship to
                                                        sbTextData = "" + indexTextDataSerialNumber + "|" + indexTextDataUpdateDate + "|" + indexTextDataMatNumber + "|" + indexTextDataCompCode + "|" + indexTextDataPlant + "|" + indexTextDataReciveStrg + "||" + indexTextDataShipTo + "";

                                                        validflag = 1;
                                                        //write-line
                                                        string nameMatdocMixGen = passconNumber + "_AIRNET_INV_" + currDateAtFile + "_" + currTimeAtFile + ".dat";
                                                        string finalFileNameWithPath = PrepareFileCreateNotDelete(tempPath, nameMatdocMixGen);
                                                        string existfilepathMix = finalFileNameWithPath;
                                                        if (existfilepathMix != "" && File.Exists(existfilepathMix))
                                                        {
                                                            using (StreamWriter fileAppend = File.AppendText(finalFileNameWithPath))
                                                            {
                                                                fileAppend.WriteLine("02" + ConName + "^02|" + sbTextData + "");
                                                                fileAppend.Close();
                                                                //RowMixcount = RowMixcount + 1;
                                                            }
                                                        }

                                                        rowMATDOCcount = rowMATDOCcount + 1;
                                                    }
                                                    else if (DeleteFlag == 1)
                                                    {
                                                        //string nameMatdocMixGen = passconNumber + "_AIRNET_INV_" + currDateAtFile + "_" + currTimeAtFile + ".dat";
                                                        //string finalFileNameWithPath = PrepareFileCreateNotDelete(tempPath, nameMatdocMixGen);
                                                        //string existfilepathMix = finalFileNameWithPath;
                                                        //if (existfilepathMix != "" && File.Exists(existfilepathMix))
                                                        //{
                                                        //    File.Delete(existfilepathMix);
                                                        //    failtFilesList.Add(nameMatdocMixGen);
                                                        //    _logger.Info("delete File name: " + existfilepathMix + "Exception Message : Mandatory field is invalid ");

                                                        //}
                                                    }


                                                }
                                                else
                                                {
                                                    _logger.Info("invalid data at Filename : " + fileName + " - missing index at row number " + (resultListi + 1) + "");
                                                }


                                            }
                                            catch (Exception e)
                                            {
                                                _logger.Info("file name: " + fileName + " - Exception Message : " + e + "");
                                            }

                                        }

                                        //Log-Read-File-Success
                                        if (validflag == 1)
                                        {
                                            InsertLoadFileGenLog(fileName, "Success", "Y");
                                        }

                                        if (rowMATDOCcount == 0)
                                        {
                                            if (nullFlag == 0)
                                            {
                                                //InsertLoadFileGenLog(fileName, DateTime.Now, "invalid data","N");
                                                InsertLoadFileGenLog(fileName, "Read File Success,No data match with condition", "Y");
                                            }
                                            else
                                            {
                                                //InsertLoadFileGenLog(fileName, DateTime.Now, "invalid data","N");
                                                InsertLoadFileGenLog(fileName, "Read File Success,No data match with condition", "N");
                                            }
                                        }

                                    }

                                }
                            }
                        }

                        try
                        {
                            // Get File เฉพาะ .dat
                            var files = Directory.GetFiles(tempPath, "*.dat");
                            //_logger.Info("file count : " + files.Length + "");

                            foreach (var file in files)
                            {
                                string nameMatdocMixGen = Path.GetFileNameWithoutExtension(file);
                                //_logger.Info("file foreach : " + nameMatdocMixGen + "");
                                string[] strTempMatdocCheck = nameMatdocMixGen.Split('_');

                                if (strTempMatdocCheck.Length > 1 && strTempMatdocCheck[1] == "AIRNET")
                                {
                                    string existfilepath = PrepareFileCreateNotDelete(tempPath, file);
                                    //_logger.Info("file existfilepath : " + existfilepath + "");

                                    if (existfilepath != "")
                                    {
                                        string counrowtext = File.ReadAllText(existfilepath);
                                        //string countrowsbTextResult = text;
                                        string[] countrowsbTextResultList = counrowtext.Split(new[] { '\n' });
                                        int countAllRowLine = countrowsbTextResultList.Count() - 2;

                                        if (countAllRowLine == 0)
                                        {

                                            //InsertLoadFileGenLog(file, DateTime.Now, "No data match with condition", "N");

                                            if (existfilepath != "" && File.Exists(existfilepath))
                                            {
                                                File.Delete(existfilepath);
                                                failtFilesList.Add(file);
                                            }
                                        }

                                        if (existfilepath != "" && File.Exists(existfilepath))
                                        {
                                            using (StreamWriter fileAppend = File.AppendText(existfilepath))
                                            {
                                                fileAppend.WriteLine("09|" + countAllRowLine + "");
                                                fileAppend.Close();
                                                sb.Append("|" + file + "");
                                            }

                                            try
                                            {
                                                //string filePath = "path/to/your/file.txt";
                                                string all09text = File.ReadAllText(existfilepath);
                                                List<string> all09textResultList = all09text.Split(new[] { '\n', '\r' }).ToList();

                                                var sortedall09textResultList = all09textResultList.OrderBy(all09textResultLists => all09textResultLists).ToList();

                                                using (StreamWriter writer = new StreamWriter(existfilepath, false)) // false for overwrite
                                                {
                                                    writer.Write(string.Empty);
                                                    foreach (var all09textResultLists in sortedall09textResultList)
                                                    {

                                                        if (!String.IsNullOrEmpty(all09textResultLists))
                                                        {
                                                            if (all09textResultLists.StartsWith("02"))
                                                            {
                                                                string[] all09textResultListsLine = all09textResultLists.Split('^');
                                                                writer.WriteLine(all09textResultListsLine[1].ToString());
                                                            }
                                                            else
                                                            {
                                                                writer.WriteLine(all09textResultLists.ToString());
                                                            }
                                                        }

                                                    }
                                                    writer.Close();

                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                _logger.Info("exception new: " + ex + "");
                                            }

                                        }

                                    }
                                    else
                                    {
                                        //InsertLoadFileGenLog(file, DateTime.Now, "No data match with condition", "N");

                                        if (existfilepath != "" && File.Exists(existfilepath))
                                        {
                                            File.Delete(existfilepath);
                                            failtFilesList.Add(file);
                                        }
                                    }
                                }


                            }



                        }
                        catch (Exception e)
                        {
                            //_logger.Info("can't read data at position!");
                            //_logger.Info("File name: " + file + " - Exception Message : " + e + "");
                            _logger.Info(e);
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
                finally
                {
                    if (wic != null)
                        wic.Undo();
                }
            }
            return sb;
        }





        public StringBuilder GetFileSAPNEWS4hana(string username, string pwd, string domin, string sourceFullPathName, string archivePath, string tempPath)
        {
            StringBuilder sb = new StringBuilder();
            List<string> failtFilesList = new List<string>();
            List<string> exceededLimitDayFilesList = new List<string>();
            List<string> historyDownloadFileList = new List<string>();
            List<string> finallyFilesList = new List<string>();

            var widCurrent = WindowsIdentity.GetCurrent();
            // ReSharper disable once NotAccessedVariable
            InterfaceLogCommand log = new InterfaceLogCommand();
            WindowsImpersonationContext wic = null;
            List<string> resp = new List<string>();
            bool isRemoveSucess;

            string returnStatusfile = "";
            string downloadSuccess = "";
            string uploadFail = "";
            string uploadSuccess = "";


            var adminToken = new IntPtr();
            if (LogonUser(username, domin, pwd, 9, 0, ref adminToken) != 0)
            {
                wic = new WindowsIdentity(adminToken).Impersonate();

                try
                {
                    if (Directory.Exists(sourceFullPathName))
                    {
                        int comDayCallback = 0;
                        try
                        {
                            comDayCallback = ConfigurationManager.AppSettings["DayCallback"].ToSafeInteger();

                        }
                        catch (Exception ex)
                        {
                            _logger.Info("DayCallback exception: " + ex + "");
                        }

                        //getLovFilterList
                        var lovFilterList = GetLovFilterList("PAYG_TRANS_MATDOC", "");

                        // ดึงรายการ CON_NAME ที่ไม่ซ้ำกัน
                        List<string> con_name_list = lovFilterList
                            .Select(x => x.CON_NAME)
                            .Distinct()
                            .ToList();

                        // ดึงค่า VAL2 จาก DISPLAY_VAL ที่เป็น "company_code" แล้วแยกด้วย comma ให้ไม่ซ้ำกัน
                        List<string> comCodeListMain = lovFilterList
                            .Where(x => x.DISPLAY_VAL == "company_code" && !string.IsNullOrWhiteSpace(x.VAL2))
                            .SelectMany(x => x.VAL2.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                            .Select(code => code.Trim()) // ลบช่องว่างด้านหน้า/หลัง (ถ้ามี)
                            .Distinct()
                            .ToList();

                        //var comDayCallback = 7;
                        DateTime currDateTime = DateTime.Now;
                        DateTime advanceCurrDateTime = DateTime.Now.AddDays(-comDayCallback).Date;
                        DateTime dayArchiveForOrigin_ck = DateTime.Now.AddDays(-_dayArchiveForOrigin);

                        _logger.Info("Delete File in Archive DayArchive: " + _dayArchiveForOrigin + "");

                        _logger.Info("Get File from SAP Path DayCallBack: " + comDayCallback + "");

                        string dateNow = currDateTime.ToString("yyyyMMdd");
                        string timenow = DateTime.Now.ToString("hhmmss");
                        //MATDOC-20241016145930-7
                        string rootFileName = string.Format("{0}-{1}", "MATDOC", dateNow);


                        //sourceFullPathName = "C:\\MATdoc\\S4";
                        string[] filelist = Directory.GetFiles(sourceFullPathName);
                        int fileCount = filelist.Length;

                        //var matdocFiles = filelist
                        //.Where(f =>
                        //Path.GetFileName(f).StartsWith("MATDOC-", StringComparison.OrdinalIgnoreCase) &&
                        //File.GetLastWriteTime(f) >= advanceCurrDateTime
                        // )
                        // .ToList();

                        var matdocFiles = filelist
                        .Where(f =>
                        Path.GetFileName(f).StartsWith("MATDOC-", StringComparison.OrdinalIgnoreCase) &&
                        Path.GetExtension(f).Equals(".dat", StringComparison.OrdinalIgnoreCase) &&
                        File.GetLastWriteTime(f) >= advanceCurrDateTime)
                        .ToList();
                        int processedFilesfail = 0, processedFilesSuccess = 0, processedFilesSkipped = 0;
                        var result = new StringBuilder();
                        var readfileall = matdocFiles.Count;
                        string newFileFlag = "N";
                        if (matdocFiles.Count == 0)
                        {
                            _logger.Info("No file in path directory");
                            //set empty status
                            // newFileFlag = "E";
                            return result;
                        }

                        foreach (string files in matdocFiles)
                        {
                            try
                            {

                                //Checknullflag = "0";
                                string text = File.ReadAllText(files);
                                // Use static Path methods to extract only the file name from the path.
                                string fileName = Path.GetFileName(files);
                                string comDayCallbacks = comDayCallback.ToString();
                                string existFileStatus = GetExistFileStatus(fileName, comDayCallbacks);
                                if (existFileStatus == "NEW")
                                {
                                    _logger.Info("New File: " + fileName + "");
                                }
                                else if (existFileStatus == "EXIST")
                                {
                                    processedFilesSkipped++;

                                    if (readfileall == processedFilesSkipped)
                                    {
                                        return result;
                                    }
                                    else
                                    {

                                        continue;
                                    }

                                    // _logger.Info("Reread File: " + fileName + "");
                                }
                                else
                                {
                                    _logger.Info("Reread File  : " + fileName + "");
                                }

                                string[] strTemp = fileName.Split('-');
                                string tempFileName = string.Format("{0}-{1}-{2}", strTemp[0], strTemp[1], strTemp[2]);

                                string currDateAtFile = "" + strTemp[1].Substring(0, 4) + strTemp[1].Substring(4, 2) + strTemp[1].Substring(6, 2) + "";
                                string currTimeAtFile = "" + strTemp[1].Substring(8, 6) + "";

                                //check-file-exist-status
                                // if (strTemp[0] == "MATDOC" && tempFileName.ToLower().EndsWith(".dat") && existFileStatus == "NEW")
                                if (strTemp.Length > 0
                                && strTemp[0].Equals("MATDOC", StringComparison.OrdinalIgnoreCase)
                                && Path.GetExtension(fileName).Equals(".dat", StringComparison.OrdinalIgnoreCase)
                                && (string.Equals(existFileStatus, "NEW", StringComparison.OrdinalIgnoreCase)
                                || string.Equals(existFileStatus, "READ", StringComparison.OrdinalIgnoreCase)))
                                {
                                    string tempdate = strTemp[1];
                                    var preDateTime = new DateTime(strTemp[1].Substring(0, 4).ToSafeInteger(), strTemp[1].Substring(4, 2).ToSafeInteger(), strTemp[1].Substring(6, 2).ToSafeInteger());

                                    if (fileName.ToLower().EndsWith(".dat"))
                                    {
                                        //string existFileStatus = GetExistFileStatus(fileName);
                                        if (existFileStatus == "NEW")
                                        {
                                            //do_normal_step_to_process_file
                                            newFileFlag = "Y";
                                        }
                                        else if (existFileStatus == "READ")
                                        {
                                            newFileFlag = "Y";
                                        }
                                        //else if (existFileStatus == "EXIST")
                                        //{
                                        //    //InsertLoadFileGenLog(fileName, "File already Exist-Read Success", "Y");
                                        //}


                                    }

                                    // Temp File for DB call it
                                    string destFile = PrepareFile(tempPath, fileName);
                                    File.Copy(files, destFile, true);
                                    SAPModel.copyList.Add(destFile);

                                    string sbTextResult = text;
                                    int checkNulllineCount = 0;

                                    //read-row
                                    string[] lineResultList = sbTextResult.Split(new[] { '\n' });
                                    var companyCodeFilterList = lovFilterList
                                     .Where(x => x.DISPLAY_VAL == "company_code" && con_name_list.Contains(x.CON_NAME))
                                     .ToList();

                                    foreach (var line in lineResultList)
                                    {
                                        string templine = line.Replace("\r", "");

                                        if (string.IsNullOrWhiteSpace(templine))
                                        {
                                            checkNulllineCount++;
                                            continue;
                                        }

                                        string[] columns = templine.Split('|');

                                        try
                                        {
                                            foreach (var filter in companyCodeFilterList)
                                            {
                                                int fieldIndex = filter.VAL1.ToSafeInteger();
                                                if (fieldIndex >= columns.Length) continue;

                                                string fieldValue = columns[fieldIndex];
                                                string[] filterValues = filter.VAL2.Split(',');

                                                if (filterValues.Contains(fieldValue))
                                                {
                                                    string nameMatdocMixGen = $"{fieldValue}_AIRNET_INV_{currDateAtFile}_{currTimeAtFile}.dat";

                                                    // สร้างข้อมูลไฟล์
                                                    var fileData = new List<PAYGTransAirnetFileList>
                                                      {
                                                 new PAYGTransAirnetFileList
                                                   {
                                                 file_data = $"01|{nameMatdocMixGen}",
                                                 file_name = nameMatdocMixGen
                                                     }
                                                 };

                                                    string fullPath = PrepareFileCreateNotDelete(tempPath, nameMatdocMixGen);

                                                    if (!File.Exists(fullPath))
                                                    {
                                                        WriteFile(username, pwd, domin, tempPath, nameMatdocMixGen, fileData);
                                                        // _logger.Info("Create File name: ----> " + nameMatdocMixGen);
                                                    }
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            _logger.Info("exception: " + ex);
                                        }
                                    }

                                    //check-equal-null-line-count
                                    if (checkNulllineCount == lineResultList.Count())
                                    {

                                        InsertLoadFileGenLog(fileName, "Data not found", "N");
                                        continue;
                                    }


                                }
                                else
                                {
                                    //  _logger.Info("File name: " + tempFileName + ", already exist file or invalid file type");
                                }



                            }
                            catch (Exception ex)
                            {
                                // ถ้าเกิด Error ให้ข้ามไป Step (ไฟล์ถัดไป)
                                _logger.Error($"Exception Message : {ex.Message}");
                                InsertLoadFileGenLog(files, ex.Message, "N");
                                processedFilesfail++;
                                continue;
                            }


                        }

                        if (newFileFlag == "N")
                        {
                            _logger.Info("File not found.");
                            //InsertLoadFileGenLog(fileName, DateTime.Now, "invalid data", "N");
                        }


                        //temp-Path
                        string[] filelistTemp = Directory.GetFiles(tempPath);
                        List<string> listfilelistTemp = filelistTemp.ToList();
                        //delete-file-list
                        List<string> deleteFailFileList = new List<string>();


                        List<string> logfinalFileNameWithPathList = new List<string>();
                        List<bool> isMatchedFileList = new List<bool>();



                        //wrtieLine
                        foreach (string files in filelistTemp)
                        {

                            string fileName = Path.GetFileName(files);
                            string[] strTemp = fileName.Split('-');

                            if (strTemp.Count() > 1)
                            {
                                string tempFileName = string.Format("{0}-{1}-{2}", strTemp[0], strTemp[1], strTemp[2]);

                                string currDateAtFile = "" + strTemp[1].Substring(0, 4) + strTemp[1].Substring(4, 2) + strTemp[1].Substring(6, 2) + "";
                                string currTimeAtFile = "" + strTemp[1].Substring(8, 6) + "";

                                string text = File.ReadAllText(files);

                                string[] strTempMatdocCheck = fileName.Split('-');


                                //  if (strTempMatdocCheck[0] == "MATDOC" && fileName.ToLower().EndsWith(".dat"))
                                // {
                                processedFilesSuccess++;
                                int rowMATDOCcount = 0;
                                int validflag = 99;
                                int nullFlag = 1;
                                int matchedCount = 0;
                                int notMatchedCount = 0;
                                int totalProcesseds = 0;

                                string tempdate = strTemp[1];
                                var preDateTime = new DateTime(strTemp[1].Substring(0, 4).ToSafeInteger(), strTemp[1].Substring(4, 2).ToSafeInteger(), strTemp[1].Substring(6, 2).ToSafeInteger());
                                //camparedatecallback
                                // if (preDateTime >= advanceCurrDateTime)
                                // {
                                string sbTextResult = text;
                                //var result = sbTextResult.Split(new[] { '\r', '\n' });



                                string[] lineResultList = sbTextResult.Split(new[] { '\n' });




                                // ------------------------ _logger.Info($"validflag after setting: {validflag}");

                                var comparer = StringComparer.OrdinalIgnoreCase;

                                // เตรียม filterList ให้พร้อมใช้งาน
                                var filterList = lovFilterList
                                    .Select(x => new YourFilterClass(x, comparer)) // ใช้เวอร์ชัน precompute
                                    .Where(f => !string.IsNullOrWhiteSpace(f.CON_NAME))
                                    .ToList();

                                // ทำ Lookup และจัดลำดับ CON_NAME แค่ครั้งเดียว
                                var byCon = filterList.ToLookup(f => f.CON_NAME, comparer);
                                var conOrder = filterList
                                    .Select(f => f.CON_NAME)
                                    .Distinct(comparer)
                                    .ToList();


                                int totalLines = lineResultList.Count();


                                // ไฟล์ว่างจริง ๆ หรือมีแต่บรรทัดว่าง/ตัวคั่น
                                bool onlyBlankLines = totalLines > 0 && lineResultList.All(l =>
                                {
                                    var s = l?.Replace("\r", "").Replace("\n", "");
                                    return string.IsNullOrWhiteSpace(s) || (s.Replace("|", "").Trim().Length == 0);
                                });

                                if (totalLines == 0 || onlyBlankLines)
                                {
                                    // ไฟล์เปล่า → nullFlag =0และจบงาน
                                    nullFlag = 0;

                                }
                                else
                                {
                                    for (int resultListi = 0; resultListi < totalLines; resultListi++)
                                    {
                                        var fields = lineResultList[resultListi].Split('|');
                                        try
                                        {
                                            if (fields.Length < 88)
                                            {


                                                continue;


                                                // _logger.Info($"invalid data at Filename : {fileName} - missing index at row number {resultListi + 1}");
                                            }

                                            // หา CON_NAME ที่ match ทุกกฎ
                                            string matchedConName = conOrder.FirstOrDefault(name =>
                                                byCon.Contains(name) && byCon[name].All(f => f.Match(fields))
                                            );

                                            if (matchedConName == null)
                                            {
                                                notMatchedCount++;
                                                continue;
                                            }

                                            // ==== เก็บค่าตาม index ต่าง ๆ ====
                                            string passconNumber = GetVal(fields, 52);
                                            string indexTextDataSerial = ParseLongOrRaw(fields, 87);
                                            string indexTextDataMatNumber = ParseLongOrRaw(fields, 16);
                                            string indexTextDatadebit = GetVal(fields, 35);
                                            string indexTextDataUpdateDate = ParseDateDot(fields, 5);
                                            string indexTextDataCompCode = GetVal(fields, 52);

                                            string movement = GetVal(fields, 15);
                                            string indexTextDataPlant = "", indexTextDataReciveStrg = "";
                                            if (movement == "641")
                                            {
                                                indexTextDataPlant = GetVal(fields, 57);
                                                indexTextDataReciveStrg = GetVal(fields, 58);
                                            }
                                            else if (movement == "642" || movement == "311")
                                            {
                                                indexTextDataPlant = GetVal(fields, 18);
                                                indexTextDataReciveStrg = GetVal(fields, 19);
                                            }

                                            string indexTextDataShipTo = GetVal(fields, 77);
                                            string stockTypeTextData = GetVal(fields, 22);

                                            // ==== ตรวจ field บังคับ ====
                                            if (string.IsNullOrWhiteSpace(indexTextDataSerial) ||
                                                string.IsNullOrWhiteSpace(indexTextDataUpdateDate) ||
                                                string.IsNullOrWhiteSpace(indexTextDataMatNumber) ||
                                                string.IsNullOrWhiteSpace(indexTextDataCompCode) ||
                                                string.IsNullOrWhiteSpace(indexTextDataPlant) ||
                                                string.IsNullOrWhiteSpace(indexTextDataReciveStrg) ||
                                                string.IsNullOrWhiteSpace(stockTypeTextData))
                                            {
                                                // _logger.Info($"invalid data at Filename : {fileName} - Serial Number row : {indexTextDataSerial}");

                                                continue;
                                            }

                                            // ==== สร้างไฟล์ output ====
                                            validflag = 1;
                                            string sbTextData = $"{indexTextDataSerial}|{indexTextDataUpdateDate}|{indexTextDataMatNumber}|{indexTextDataCompCode}|{indexTextDataPlant}|{indexTextDataReciveStrg}||{indexTextDataShipTo}";

                                            string nameMatdocMixGen = passconNumber + "_AIRNET_INV_" + currDateAtFile + "_" + currTimeAtFile + ".dat";
                                            string finalFileNameWithPath = PrepareFileCreateNotDelete(tempPath, nameMatdocMixGen);

                                            if (!logfinalFileNameWithPathList.Contains(finalFileNameWithPath))
                                            {
                                                logfinalFileNameWithPathList.Add(finalFileNameWithPath);
                                                isMatchedFileList.Add(File.Exists(finalFileNameWithPath));
                                            }

                                            if (!string.IsNullOrEmpty(finalFileNameWithPath) && File.Exists(finalFileNameWithPath))
                                            {
                                                using (var fileAppend = File.AppendText(finalFileNameWithPath))
                                                {
                                                    fileAppend.WriteLine("02" + matchedConName + "^02|" + sbTextData);
                                                }
                                            }
                                            else
                                            {
                                                Console.WriteLine("rowMATDOCcount: " + rowMATDOCcount);
                                            }

                                            rowMATDOCcount++;
                                            matchedCount++;
                                        }
                                        catch (Exception e)
                                        {
                                            rowMATDOCcount = 1;
                                            _logger.Info($"file name: {fileName} - Exception Message : {e.Message}");
                                            InsertLoadFileGenLog(files, e.Message, "N");
                                            continue;
                                        }
                                    }
                                }





                                //Log-Read-File-Success
                                if (validflag == 1)
                                {
                                    totalProcesseds = matchedCount + notMatchedCount;

                                    InsertLoadFileGenLog(fileName, "Success with " + matchedCount + " Records. Total " + totalProcesseds + " Records.", "Y");
                                    _logger.Info($"File Name :{fileName} - Success with {matchedCount} Records. Total: {totalProcesseds} Records.");

                                    matchedCount = 0;
                                    notMatchedCount = 0;
                                    totalProcesseds = 0;
                                }

                                if (rowMATDOCcount == 0)
                                {
                                    if (nullFlag == 0)
                                    {
                                        InsertLoadFileGenLog(fileName, "Data not found", "N");


                                    }
                                    else
                                    {
                                        InsertLoadFileGenLog(fileName, "Read File Success,No data match with condition", "Y");

                                    }




                                }

                                //}

                                // }
                            }
                        }



                        //var distinctFileNames = logfinalFileNameWithPathList.Distinct().ToList();
                        //var distinctFileMatches = isMatchedFileList.Distinct().ToList();

                        //foreach (var fileName in distinctFileNames)
                        //{
                        //    _logger.Info($"finalFileNameWithPath: {fileName}");
                        //}

                        //foreach (var isMatched in distinctFileMatches)
                        //{
                        //    _logger.Info($"File.Exists(finalFileNameWithPath): {isMatched}");
                        //}



                        //_logger.Info($"File.Exists(FileMatched): {matchedCount}");
                        //_logger.Info($"File.Exists(FileNot Matched): {notMatchedCount}");


                        try
                        {
                            // Get File เฉพาะ .dat
                            var files = Directory.GetFiles(tempPath, "*.dat");
                            //_logger.Info("file count : " + files.Length + "");

                            foreach (var file in files)
                            {
                                string nameMatdocMixGen = Path.GetFileNameWithoutExtension(file);
                                //_logger.Info("file foreach : " + nameMatdocMixGen + "");
                                string[] strTempMatdocCheck = nameMatdocMixGen.Split('_');

                                if (strTempMatdocCheck.Length > 1 && strTempMatdocCheck[1] == "AIRNET")
                                {
                                    string existfilepath = PrepareFileCreateNotDelete(tempPath, file);
                                    //_logger.Info("file existfilepath : " + existfilepath + "");

                                    if (existfilepath != "")
                                    {
                                        string counrowtext = File.ReadAllText(existfilepath);
                                        //string countrowsbTextResult = text;
                                        string[] countrowsbTextResultList = counrowtext.Split(new[] { '\n' });
                                        int countAllRowLine = countrowsbTextResultList.Count() - 2;

                                        if (countAllRowLine == 0)
                                        {

                                            //InsertLoadFileGenLog(file, DateTime.Now, "No data match with condition", "N");

                                            if (existfilepath != "" && File.Exists(existfilepath))
                                            {
                                                File.Delete(existfilepath);
                                                failtFilesList.Add(file);
                                            }
                                        }

                                        if (existfilepath != "" && File.Exists(existfilepath))
                                        {
                                            using (StreamWriter fileAppend = File.AppendText(existfilepath))
                                            {
                                                fileAppend.WriteLine("09|" + countAllRowLine + "");
                                                fileAppend.Close();
                                                sb.Append("|" + file + "");
                                            }

                                            try
                                            {
                                                //string filePath = "path/to/your/file.txt";
                                                string all09text = File.ReadAllText(existfilepath);
                                                List<string> all09textResultList = all09text.Split(new[] { '\n', '\r' }).ToList();

                                                var sortedall09textResultList = all09textResultList.OrderBy(all09textResultLists => all09textResultLists).ToList();

                                                using (StreamWriter writer = new StreamWriter(existfilepath, false)) // false for overwrite
                                                {
                                                    writer.Write(string.Empty);
                                                    foreach (var all09textResultLists in sortedall09textResultList)
                                                    {

                                                        if (!String.IsNullOrEmpty(all09textResultLists))
                                                        {
                                                            if (all09textResultLists.StartsWith("02"))
                                                            {
                                                                string[] all09textResultListsLine = all09textResultLists.Split('^');
                                                                writer.WriteLine(all09textResultListsLine[1].ToString());
                                                            }
                                                            else
                                                            {
                                                                writer.WriteLine(all09textResultLists.ToString());
                                                            }
                                                        }

                                                    }
                                                    writer.Close();

                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                _logger.Info("exception new: " + ex + "");
                                            }

                                        }

                                    }
                                    else
                                    {
                                        //InsertLoadFileGenLog(file, DateTime.Now, "No data match with condition", "N");

                                        if (existfilepath != "" && File.Exists(existfilepath))
                                        {
                                            File.Delete(existfilepath);
                                            failtFilesList.Add(file);
                                        }
                                    }
                                }


                            }



                        }
                        catch (Exception e)
                        {
                            //_logger.Info("can't read data at position!");
                            //_logger.Info("File name: " + file + " - Exception Message : " + e + "");
                            _logger.Info(e);
                        }




                        _logger.Info($"Read File ALL: {readfileall} , Succeeded : {processedFilesSuccess} , Skipped : {processedFilesSkipped} , Fail : {processedFilesfail}");


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
                finally
                {
                    if (wic != null)
                        wic.Undo();
                }
            }
            return sb;
        }




        //public class comCodeList
        //{
        //    public int rowCount { get; set; }
        //    public string Comcode { get; set; }
        //}

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

                    //string[] filelist = Directory.GetFiles(PrepareFile(pathName, fileName));
                    //foreach (string files in filelist)
                    //{
                    //    File.Delete(PrepareFile(pathName, files));
                    //}

                    string finalFileNameWithPath = Path.Combine(pathName, fileName);
                    if (File.Exists(finalFileNameWithPath)) File.Delete(finalFileNameWithPath);

                    isSuccess = GenerateFile(data, pathName, fileName);
                    //this.RemoveTargetFileOverWeek(_archiveLocalPath);
                }
            }
            catch (Exception e)
            {
                _logger.Info("Generate File Destination Error! : " + e.Message + " ----- Stacktrace :" + e.StackTrace + "");
                isSuccess = false;
            }

            return isSuccess;
        }

        //protected void RemoveTargetFileOverWeek(string pathName)
        //{
        //    DateTime currDateTime = DateTime.Now.AddDays(-_dayArchiveForDestination);
        //    string[] filelist = Directory.GetFiles(PrepareFile(pathName, ""));
        //    foreach (string files in filelist)
        //    {
        //        string fileName = Path.GetFileName(files);
        //        string[] strTemp = fileName.Split('_');
        //        string tempFileName = strTemp[3];
        //        var preDateTime = new DateTime(tempFileName.Substring(0, 4).ToSafeInteger(), tempFileName.Substring(4, 2).ToSafeInteger(), tempFileName.Substring(6, 2).ToSafeInteger());

        //        if (preDateTime < currDateTime)
        //        {
        //            File.Delete(PrepareFile(pathName, files));
        //        }
        //    }

        //}

        //protected void RemoveLocalFileOverWeek(string pathName)
        //{
        //    DateTime currDateTime = DateTime.Now.AddDays(-_dayArchiveForOrigin);
        //    string[] filelist = Directory.GetFiles(PrepareFile(pathName, ""));
        //    foreach (string files in filelist)
        //    {
        //        string fileName = Path.GetFileName(files);
        //        string[] strTemp = fileName.Split('_');
        //        string tempFileName = strTemp[3];

        //        var preDateTime = new DateTime(tempFileName.Substring(0, 4).ToSafeInteger(), tempFileName.Substring(4, 2).ToSafeInteger(), tempFileName.Substring(6, 2).ToSafeInteger());

        //        if (preDateTime < currDateTime)
        //        {
        //            File.Delete(PrepareFile(pathName, files));
        //            _logger.Info("Archive : " + files + "");

        //        }

        //    }

        //}




        protected void RemoveLocalFileOverWeek(string pathName)
        {
            // ถ้า PrepareFile คืน path โฟลเดอร์ ให้ใช้บรรทัดนี้; 
            // ถ้า pathName คือโฟลเดอร์อยู่แล้ว ใช้ var archiveDir = pathName;
            var archiveDir = PrepareFile(pathName, "");

            if (string.IsNullOrWhiteSpace(archiveDir) || !Directory.Exists(archiveDir))
            {
                _logger.Error($"Archive path not found: {archiveDir}");
                return;
            }

            // ตัดตามจำนวนวัน (Date Modified เก่ากว่า cutoff จะถูกลบ)
            var cutoff = DateTime.Now.AddDays(-_dayArchiveForOrigin);

            int deleted = 0, skipped = 0, failed = 0;

            foreach (var fullPath in Directory.EnumerateFiles(archiveDir, "*", SearchOption.TopDirectoryOnly))
            {
                try
                {
                    var lastWrite = File.GetLastWriteTime(fullPath);

                    if (lastWrite < cutoff)
                    {
                        File.Delete(fullPath);
                        //_logger.Info($"Delete File success (Archive) : {fullPath} | LastWrite: {lastWrite:yyyy-MM-dd HH:mm:ss}");

                        _logger.Info($"Delete File success (Archive) : {fullPath}");
                        deleted++;
                    }
                    else
                    {
                        skipped++;
                    }
                }
                catch (Exception ex)
                {
                    // ถ้าเกิด Error ให้ข้ามไป Step (ไฟล์ถัดไป)
                    _logger.Error($" {fullPath} | {ex.Message}");
                    failed++;
                    continue;
                }
            }

            //_logger.Info($"Archive summary | Deleted: {deleted}, Skipped: {skipped}, Failed: {failed}, Cutoff: {cutoff:yyyy-MM-dd HH:mm:ss}");
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
            //string fileLogPath = PrepareFileCreateNotDelete(_archiveLogPath, currDateTime.ToString("yyyyMMdd"));
            //StreamWriter fileLog = new StreamWriter(fileLogPath, true);

            foreach (PAYGTransAirnetFileList data in datas)
            {
                // write file to target
                string finalFileNameWithPath = PrepareFile(directoryPath, data.file_name);
                StreamWriter file = new StreamWriter(finalFileNameWithPath, true);
                file.WriteLine(data.file_data);
                // _logger.Info($"Target : {data.file_name}");
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
            //if (File.Exists(finalFileNameWithPath)) File.Delete(finalFileNameWithPath);

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

        //private List<string> fileLog(int pre)
        //{

        //    List<string> preFile = new List<string>();

        //    DateTime currDateTime = DateTime.Now.AddDays(-pre);
        //    string[] filelist = Directory.GetFiles(PrepareFileCreate(_archiveLogPath, ""));
        //    foreach (string files in filelist)
        //    {
        //        string fileName = Path.GetFileName(files);
        //        var preDateTime = new DateTime(fileName.Substring(0, 4).ToSafeInteger(), fileName.Substring(4, 2).ToSafeInteger(), fileName.Substring(6, 2).ToSafeInteger());

        //        if (!(preDateTime < currDateTime))
        //        {
        //            foreach (var list in File.ReadAllLines(files).ToList())
        //            {
        //                preFile.Add(list.ToString());
        //            }
        //        }
        //        else File.Delete(files);
        //    }

        //    return preFile;

        //}

        private bool duplicateLog(List<string> logs, string fileName)
        {
            foreach (string log in logs)
            {
                if (log == fileName) return false;
            }
            return true;
        }


        private static string GetVal(string[] arr, int idx) =>
    (idx >= 0 && idx < arr.Length) ? arr[idx] : "";

        private static string ParseLongOrRaw(string[] arr, int idx)
        {
            var v = GetVal(arr, idx);
            return long.TryParse(v, out long num) ? num.ToString() : v;
        }

        private static string ParseDateDot(string[] arr, int idx)
        {
            var v = GetVal(arr, idx);
            var parts = v.Split('.');
            return parts.Length == 3 ? $"{parts[2]}{parts[1]}{parts[0]}" : v;
        }




        public InsertTranMatDocLoadFileLogCommand InsertLoadFileGenLog(string filename, string message, string flag_type = "Y")
        {

            string[] strTemp = filename.Split('-');

            var Intfiledate = DateTime.Now;

            if (strTemp.Length >= 2)
            {
                int yearAvatar = Convert.ToInt32(strTemp[1].Substring(0, 4));
                int monthAvatar = Convert.ToInt32(strTemp[1].Substring(4, 2));
                int dayAvatar = Convert.ToInt32(strTemp[1].Substring(6, 2));

                Intfiledate = new DateTime(yearAvatar, monthAvatar, dayAvatar);
            }

            var command = new InsertTranMatDocLoadFileLogCommand()
            {
                filename = filename,
                //filedate = filedate.Date,
                filedate = Intfiledate.Date,
                message = message,
                flag_type = flag_type,
                ret_code = "0",
                ret_msg = ""
            };
            _addLoadFileLogCommand.Handle(command);
            return command;
        }

        public string GetExistFileStatus(string filename, string comDayCallbacks)
        {
            string result_exist = "NEW";
            try
            {
                var result_existfile = Get_File_Status(filename, comDayCallbacks).FirstOrDefault();

                //_logger.Info("PROGRAM GetExpireReportData: " + result_existfile);

                if (result_existfile != null)
                {
                    if (result_existfile.flag_type == "Y" && result_existfile.file_name != "")
                    {
                        result_exist = "EXIST";
                    }
                    else if (result_existfile.flag_type == "N" && result_existfile.file_name != "")
                    {
                        result_exist = "READ";
                    }
                    else
                    {
                        result_exist = "EXIST";
                    }
                }
                else if (result_existfile == null)
                {
                    result_exist = "NEW";
                }
                else
                {
                    result_exist = "EXIST";
                }

                return result_exist;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception CheckBatchFilename : " + ex.Message);
                return result_exist;
            }

        }

        public List<GetTranMatDocGetFBSSLogQuery> Get_File_Status(string filename, string comDayCallbacks)
        {
            try
            {
                //string Sheet_query = "FBBPAYG_REPORT_EXPIRE_SIM";

                TranMatDocGetFBSSLogQuery query = new TranMatDocGetFBSSLogQuery()
                {
                    inbound_filename = filename,
                    inbound_comDayCallback = comDayCallbacks

                };
                var _CFGqueryExpireDataReportModel = _queryProcessor.Execute(query);

                return _CFGqueryExpireDataReportModel;

            }
            catch (Exception ex)
            {
                _logger.Error("Exception Get_File_Status : " + ex.Message);
                return null;
            }
        }

        public List<FbssConfigTBL> GetLovFilterList(string _CON_TYPE = "PAYG_TRANS_MATDOC", string _CON_NAME = "")
        {
            try
            {
                var query = new GetFbssConfigTBLQuery
                {
                    CON_TYPE = _CON_TYPE,
                    CON_NAME = _CON_NAME
                };

                var lov = _queryProcessor.Execute(query);
                return lov;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception Get_FBSS_CONFIG_TBL_LOV : " + ex.Message);
                return null;
            }
        }

        public bool DeleteFileTemp(string username, string pwd, string domin, string sourceFullPathName, string archivePath, string tmpPath)
        {
            var widCurrent = WindowsIdentity.GetCurrent();
            // ReSharper disable once NotAccessedVariable
            InterfaceLogCommand log = new InterfaceLogCommand();
            WindowsImpersonationContext wic = null;
            List<string> resp = new List<string>();
            bool success = false;
            //var lstTempFile = Directory.GetFiles(tmpPath);
            var adminToken = new IntPtr();
            if (LogonUser(username, domin, pwd, 9, 0, ref adminToken) != 0)
            {
                wic = new WindowsIdentity(adminToken).Impersonate();

                try
                {
                    var lstTempFile = Directory.GetFiles(tmpPath);

                    foreach (var filename in lstTempFile)
                    {
                        var fullpath = Path.Combine(tmpPath, filename);
                        if (File.Exists(fullpath))
                        {
                            // Code in STG
                            bool deleteFile = RemoveFile(
                                username,
                                pwd,
                                domin,
                                fullpath
                            );

                            //// Code in Local
                            //bool deleteFile = crd.RemoveFile(fullpath);

                            if (deleteFile)
                            {
                                success = true;
                                _logger.Info(string.Format("Delete File success (Temp) : {0}", filename));
                            }
                            else
                            {
                                _logger.Info(string.Format("Delete File unsuccess (Temp) : {0}", filename));
                            }
                        }

                    }
                }
                catch (Exception e)
                {
                    _logger.Info("Delete File Temp => Exception : " + e.Message);
                }
                finally
                {
                    if (wic != null)
                        wic.Undo();
                }

            }
            return success;
        }

        /// <summary>
        /// Insert log by matdoc file in temp nas
        /// </summary>
        /// <param name="username"></param>
        /// <param name="pwd"></param>
        /// <param name="domin"></param>
        /// <param name="tmpPath"></param>
        /// <param name="message">message error</param>
        /// <param name="flag">flag = N</param>
        /// <returns></returns>
        public bool InsertLogAccessNas(string username, string pwd, string domin, string tmpPath, string message, string flag)
        {
            var widCurrent = WindowsIdentity.GetCurrent();
            // ReSharper disable once NotAccessedVariable
            InterfaceLogCommand log = new InterfaceLogCommand();
            WindowsImpersonationContext wic = null;
            List<string> resp = new List<string>();

            bool success = false;
            //var lstTempFile = Directory.GetFiles(tmpPath);
            var adminToken = new IntPtr();
            if (LogonUser(username, domin, pwd, 9, 0, ref adminToken) != 0)
            {
                wic = new WindowsIdentity(adminToken).Impersonate();

                try
                {
                    var lstTempFile = Directory.GetFiles(tmpPath);

                    foreach (var file in lstTempFile)
                    {
                        var filename = Path.GetFileName(file);
                        if (filename.StartsWith("MATDOC"))
                        {
                            InsertLoadFileGenLog(filename, message, flag);
                        }
                    }
                }
                catch (Exception e)
                {
                    _logger.Info("Insert Log Access Nas => Exception : " + e.Message);
                }
                finally
                {
                    if (wic != null)
                        wic.Undo();
                }

            }
            return success;
        }

        public Dictionary<string, List<PAYGTransAirnetFileList>> GetFileChunkList(string username, string pwd, string domin, string sourceFullPathName, string archivePath, string tempPath, string[] lstFileName)
        {
            // Modify model to procedure
            var groupedFileList = new Dictionary<string, List<PAYGTransAirnetFileList>>();

            var widCurrent = WindowsIdentity.GetCurrent();
            // ReSharper disable once NotAccessedVariable
            InterfaceLogCommand log = new InterfaceLogCommand();
            WindowsImpersonationContext wic = null;
            List<string> resp = new List<string>();

            bool success = false;
            //var lstTempFile = Directory.GetFiles(tempPath);
            var adminToken = new IntPtr();

            if (LogonUser(username, domin, pwd, 9, 0, ref adminToken) != 0)
            {
                wic = new WindowsIdentity(adminToken).Impersonate();

                try
                {

                    if (Directory.Exists(tempPath))
                    {
                        // Get File เฉพาะ .dat
                        var files = Directory.GetFiles(tempPath, "*.dat");
                        foreach (var file in files)
                        {
                            try
                            {
                                // Get filename, file extension and content data
                                string fileName = Path.GetFileNameWithoutExtension(file);
                                string fileExtension = Path.GetExtension(file);
                                string fileData = File.ReadAllText(file);

                                // Check file name only 1200, 1300, 1800
                                if (fileName.ToUpper().StartsWith("1200_AIRNET_INV") ||
                                    fileName.ToUpper().StartsWith("1300_AIRNET_INV") ||
                                    fileName.ToUpper().StartsWith("1800_AIRNET_INV"))
                                {
                                    if (!string.IsNullOrEmpty(fileData))
                                    {
                                        // Split data into 200 row per set.
                                        var fileChunks = SplitDataByLineCount(fileData, 200);

                                        // Set first index = 1 for ordering.
                                        int chunkIndex = 1;

                                        // Declare new list file data
                                        var fileChunksList = new List<PAYGTransAirnetFileList>();

                                        // Loop for add chunk data to new list file
                                        foreach (var chunk in fileChunks)
                                        {
                                            fileChunksList.Add(new PAYGTransAirnetFileList
                                            {
                                                // Set chunk index follow filename ex. AAA_1.dat
                                                // For ordering by chunk index after called from procedure.
                                                file_name = $"{fileName}_{chunkIndex}{fileExtension}",
                                                file_data = chunk   // Content data
                                            });
                                            chunkIndex++;
                                        }
                                        groupedFileList[fileName] = fileChunksList;
                                    }
                                    else
                                    {
                                        _logger.Error($"File : {fileName} is Empty");
                                        //File.Delete(file);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.Error($"Error reading file {file}: {ex.Message}");
                            }
                        }
                    }
                    else
                    {
                        _logger.Warn("Temp directory does not exist.");
                    }

                }
                catch (Exception e)
                {
                    _logger.Info("Get file SAP => Exception : " + e.Message);
                }
                finally
                {
                    if (wic != null)
                        wic.Undo();
                }

            }

            return groupedFileList;
        }

        public List<string> SplitDataByLineCount(string data, int maxLines)
        {
            var chunks = new List<string>();
            var lines = data.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None); // Support both \r\n and \n delimiters
            int totalLines = lines.Length;

            for (int start = 0; start < totalLines; start += maxLines)
            {
                var chunk = string.Join("\n", lines.Skip(start).Take(maxLines));
                if (!chunk.EndsWith("\n"))
                {
                    chunk = chunk + "\n";
                }
                chunks.Add(chunk);
            }

            return chunks;
        }


        public class SapFileProcessor
        {
            // ใช้เก็บค่าของ flag
            private class FlagWrapper
            {
                public bool Value { get; set; } = false;
            }

            private static string ToPascalCase(string input)
            {
                if (string.IsNullOrWhiteSpace(input)) return input;

                return string.Join("",
                    input
                        .Split(new[] { '_', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(word => char.ToUpper(word[0]) + word.Substring(1).ToLower())
                );
            }

        }

    }
    internal sealed class YourFilterClass
    {
        public string CON_NAME { get; }
        public string DISPLAY_VAL { get; }
        public string VAL1 { get; }     // raw
        public string VAL2 { get; }     // raw

        // ===== precomputed =====
        public List<int> Val1Indices { get; }                 // ดึงจาก VAL1 แค่ครั้งเดียว
        public HashSet<string> AllowedSet { get; }            // ค่าอนุญาต (VAL2) แบบ trim แล้ว
        public HashSet<string> AllowedMatNorm { get; }        // ค่าอนุญาต (VAL2) หลัง normalize กรณี material
        public bool Wildcard { get; }                         // มี "*" ไหม
        public bool IsMaterial { get; }                       // DISPLAY_VAL == "material_code"

        private readonly StringComparer _cmp;

        public YourFilterClass(FbssConfigTBL s, StringComparer comparer)
        {
            _cmp = comparer ?? StringComparer.OrdinalIgnoreCase;

            CON_NAME = (s.CON_NAME ?? string.Empty).Trim();
            DISPLAY_VAL = (s.DISPLAY_VAL ?? string.Empty).Trim();
            VAL1 = (s.VAL1 ?? string.Empty).Trim();
            VAL2 = (s.VAL2 ?? string.Empty).Trim();

            IsMaterial = string.Equals(DISPLAY_VAL, "material_code", StringComparison.OrdinalIgnoreCase);

            Val1Indices = SplitIndices(VAL1).ToList();

            var allowedRaw = SplitAllowed(VAL2).ToList();
            AllowedSet = new HashSet<string>(allowedRaw, _cmp);
            Wildcard = AllowedSet.Contains("*");

            if (IsMaterial)
            {
                AllowedMatNorm = new HashSet<string>(allowedRaw.Select(NormalizeMat), _cmp);
            }
            else
            {
                // ให้เป็นชุดว่างแทน null เพื่อหลีกเลี่ยง null-check
                AllowedMatNorm = new HashSet<string>(_cmp);
            }
        }


        /// ตรวจว่าแถว (fields) “ผ่านกฎนี้” ไหม

        public bool Match(string[] fields)
        {
            if (Wildcard) return true;
            if (Val1Indices.Count == 0) return false;

            foreach (var idx in Val1Indices)
            {
                // ใช้ unsigned-compare กัน out-of-range แบบเร็ว
                if ((uint)idx >= (uint)fields.Length) continue;

                var v = fields[idx]?.Trim();
                if (string.IsNullOrEmpty(v)) continue;

                if (!IsMaterial)
                {
                    if (AllowedSet.Contains(v)) return true;
                }
                else
                {
                    if (AllowedSet.Contains(v)) return true;
                    var vNorm = NormalizeMat(v);
                    if (AllowedMatNorm.Contains(vNorm)) return true;
                }
            }
            return false;
        }

        // ================= helpers =================
        private static IEnumerable<int> SplitIndices(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) yield break;
            foreach (var tok in s.Split(new[] { ',', ';', '|' }, StringSplitOptions.RemoveEmptyEntries))
                if (int.TryParse(tok.Trim(), out var idx)) yield return idx;
        }

        private static IEnumerable<string> SplitAllowed(string s) =>
            (s ?? string.Empty)
            .Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Trim())
            .Where(x => x.Length > 0);



        //public static string NormalizeMat(string x)
        //{
        //    //if (string.IsNullOrEmpty(x)) return x;
        //    //var y = x.Trim();

        //    //int tail = y.Length - 1;
        //    //if (tail >= 0 && char.IsLetter(y[tail]))
        //    //{
        //    //    // มีตัวอักษรท้ายนามสกุล (เช่น U) -> ตัดศูนย์เฉพาะด้านหน้า
        //    //    int i = 0;
        //    //    while (i < tail && y[i] == '0') i++;
        //    //    return y.Substring(i);
        //    //}
        //    //else
        //    //{
        //    //    int i = 0;
        //    //    while (i < y.Length && y[i] == '0') i++;
        //    //    return y.Substring(i);
        //    //}

        //}

        public static string NormalizeMat(string x)
        {
            if (string.IsNullOrWhiteSpace(x)) return x;
            var y = x.Trim();

            // ถ้าตัวสุดท้ายเป็นตัวอักษร -> คืนค่าเดิม (ไม่ตัดศูนย์)
            if (char.IsLetter(y[y.Length - 1]))
                return y;

            // กรณีทั่วไป: ตัดศูนย์นำหน้า
            var trimmed = y.TrimStart('0');
            return trimmed.Length == 0 ? "0" : trimmed;
        }
    }

    //internal class YourFilterClass
    //{
    //    public string CON_NAME { get; set; }
    //    public string DISPLAY_VAL { get; set; }
    //    public string VAL1 { get; set; } // index ใน MandatorylineResultList
    //    public string VAL2 { get; set; }
    //    public List<string> Allowed { get; set; } 

    //    public YourFilterClass(FbssConfigTBL s)
    //    {
    //        CON_NAME = s.CON_NAME;
    //        DISPLAY_VAL = s.DISPLAY_VAL;
    //        VAL1 = s.VAL1;
    //        VAL2 = s.VAL2;
    //    }
    //}
}
