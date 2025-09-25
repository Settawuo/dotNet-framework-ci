using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Security.Principal;

namespace FBBPAYG_LMD_GENFILE_SUBPAYMENT
{
    using FBBPAYG_LMD_GENFILE_SUBPAYMENT.Model;
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
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="pwd"></param>
        /// <param name="domin"></param>
        /// <param name="sourcePath"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public StringBuilder RemoveFile(string username, string pwd, string domin, string sourcePath,
            int v_date_archive, int v_date_archive_div, string v_output_file_name, string v_output_file_name_dat_format, 
            string v_output_file_name_sync_format, string v_output_file_name_log_format)
        {
            StringBuilder sb = new StringBuilder();
            // ReSharper disable once UnusedVariable
            var widCurrent = WindowsIdentity.GetCurrent();
            // ReSharper disable once NotAccessedVariable
            WindowsImpersonationContext wic = null;
            var isRemoveSucess = false;
            List<string> errList = new List<string>();

            try
            {
                var adminToken = new IntPtr();
                if (LogonUser(username, domin, pwd, 9, 0, ref adminToken) != 0)
                {
                    // ReSharper disable once RedundantAssignment
                    wic = new WindowsIdentity(adminToken).Impersonate();

                    string v_file_name_arc = string.Empty;
                    //8.5 LOOP DELETE BY RECORD
                    for (int i = v_date_archive; i <= v_date_archive + v_date_archive_div; i++)
                    {
                        //8.6.SET ARCHIVE FILE NAME
                        try
                        {
                            v_file_name_arc = v_output_file_name + "_" + (DateTime.Now.AddDays(-i)).ToString("yyyyMMdd");
                            var removeDatFile = v_file_name_arc + v_output_file_name_dat_format;
                            var removeSyncFile = v_file_name_arc + v_output_file_name_sync_format;
                            var removeLogFile = v_file_name_arc + v_output_file_name_log_format;

                            var pathDot = Path.Combine(sourcePath, removeDatFile);
                            var pathSync = Path.Combine(sourcePath, removeSyncFile);
                            var pathLog = Path.Combine(sourcePath, removeLogFile);
                            if (File.Exists(pathDot))
                            {
                                File.Delete(pathDot);
                                _logger.Info("Remove File : " + removeDatFile);
                            }
                            if (File.Exists(pathSync))
                            {
                                File.Delete(pathSync);
                                _logger.Info("Remove File : " + removeSyncFile);
                            }
                            if (File.Exists(pathLog))
                            {
                                File.Delete(pathLog);
                                _logger.Info("Remove File : " + removeLogFile);
                            }
                        }
                        catch (Exception ex)
                        {
                            errList.Add(v_file_name_arc + ex.Message);
                            _logger.Error("Remove File : " + v_file_name_arc + ex.Message);
                        }
                    }
                }
            }
            // ReSharper disable once RedundantCatchClause
            catch (Exception ex)
            {
                _logger.Error("Remove File Error Exception: " + ex.Message.ToString());
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
        public StringBuilder placeFileTargetNas(string username, string pwd, string domin, string sourceFullPathName)
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
                    if (Directory.Exists(sourceFullPathName))
                    {

                        var files = Directory.GetFiles(pathTempFile);

                        _logger.Info($"Destination Path: {sourceFullPathName}");
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


    }
}
