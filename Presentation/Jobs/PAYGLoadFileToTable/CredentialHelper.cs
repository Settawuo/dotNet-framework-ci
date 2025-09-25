using System;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Security.Principal;

namespace PAYGLoadFileToTable
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Runtime.InteropServices.ComTypes;
    using System.Text;
    using System.Text.RegularExpressions;
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

        [PermissionSetAttribute(SecurityAction.Demand, Name = "FullTrust")]
        public StringBuilder GetFileSAP(string username, string pwd, string domin, string sourceFullPathName, List<string> namePartsList)
        {
            DateTime dategetfile = DateTime.Now.Date;
            WindowsImpersonationContext wic = null;
            StringBuilder sb = new StringBuilder();
            try
            {
                var adminToken = new IntPtr();
                if (LogonUser(username, domin, pwd, LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT, ref adminToken) != 0)
                {
                    var todayFiles = namePartsList.Select(a => new {
                        todayFile = string.IsNullOrEmpty(Regex.Match(a, @"_(\d{8}_\d{6})").Groups[1].Value),
                        name = a.Replace(".dat", "").Replace(".sync", ""),
                        fileExtension = a.Substring(a.LastIndexOf('.')),
                        date = string.IsNullOrEmpty(Regex.Match(a, @"_(\d{8}_\d{6})").Groups[1].Value)
                                                ? string.Empty
                                                : a.Replace(Regex.Match(a, @"_(\d{8}_\d{6})").Groups[1].Value, "").Replace(".dat", "").Replace(".sync", "")
                    })
                                        .Where(a => a.todayFile)
                                        .ToList();

                    wic = new WindowsIdentity(adminToken).Impersonate();

                    string[] filelist = Directory.GetFileSystemEntries(sourceFullPathName);
                    _logger.Info(" sourceFullPathName " + sourceFullPathName);

                    string pathNewDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Temp/");
                    _logger.Info(" pathNewDirectory " + pathNewDirectory);
                    if (Directory.Exists(sourceFullPathName))
                    {
                        foreach (string files in filelist)
                        {
                            try
                            {

                                string fileName = Path.GetFileName(files);
                                DateTime? FilNasDate = string.IsNullOrEmpty(Regex.Match(fileName, @"_(\d{8}_\d{6})").Groups[1].Value) ? (DateTime?)null : DateTime.ParseExact(Regex.Match(fileName, @"_(\d{8}_\d{6})").Groups[1].Value, "yyyyMMdd_HHmmss", null).Date;
                                string fileExtension = fileName.Contains('.') ? fileName.Substring(fileName.LastIndexOf('.')) : string.Empty;
                                if (namePartsList.Any(part => string.Equals(fileName, part, StringComparison.OrdinalIgnoreCase))
                                    || FilNasDate != null
                                        && todayFiles.Any(a => fileName.Contains(a.name)
                                        && FilNasDate == dategetfile
                                        && fileExtension == a.fileExtension
                                        ))
                                {
                                    try
                                    {
                                        string destFile = Path.Combine(pathNewDirectory, fileName);
                                        File.Copy(files, PrepareFileCreate(pathNewDirectory, fileName), true);
                                        _logger.Info($"Copied File: {fileName}");
                                    }
                                    catch (UnauthorizedAccessException ex)
                                    {
                                        _logger.Error($"Permission error copying {fileName}: {ex.Message}");
                                    }
                                    catch (Exception ex)
                                    {
                                        _logger.Error($"Error copying {fileName}: {ex.Message}");
                                    }
                                }


                            }
                            catch (Exception ex)
                            {
                                _logger.Info(" Exception error " + ex);
                                continue;
                            }
                        }
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


                //var adminToken = new IntPtr();
                //if (LogonUser(username, domin, pwd, LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT, ref adminToken) != 0)
                //{
                //    wic = new WindowsIdentity(adminToken).Impersonate();
                //    string[] filelist = Directory.GetFileSystemEntries(sourceFullPathName);
                //    _logger.Info(" sourceFullPathName " + sourceFullPathName);

                //    string pathNewDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Temp/");
                //    _logger.Info(" pathNewDirectory " + pathNewDirectory);
                //    if (Directory.Exists(sourceFullPathName))
                //    {
                //        foreach (string files in filelist)
                //        {
                //            string fileName = Path.GetFileName(files);
                //            try
                //            {
                //                if (namePartsList.Any(part => fileName.IndexOf(part, StringComparison.OrdinalIgnoreCase) >= 0))
                //                {
                //                    try
                //                    {
                //                        string destFile = Path.Combine(pathNewDirectory, fileName);
                //                        File.Copy(files, PrepareFileCreate(pathNewDirectory, fileName), true);
                //                        _logger.Info($"Copied File: {fileName}");
                //                    }
                //                    catch (UnauthorizedAccessException ex)
                //                    {
                //                        _logger.Error($"Permission error copying {fileName}: {ex.Message}");
                //                    }
                //                    catch (Exception ex)
                //                    {
                //                        _logger.Error($"Error copying {fileName}: {ex.Message}");
                //                    }
                //                }


                //            }
                //            catch (Exception ex)
                //            {
                //                _logger.Info(" Exception error " + ex);
                //                continue;
                //            }
                //        }
                //    }
                //    else
                //    {
                //        _logger.Info("SAP path does not exist!");
                //    }
                //}
                //else
                //{
                //    _logger.Info($"Can not connect SAP path :{sourceFullPathName}");
                //}
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
