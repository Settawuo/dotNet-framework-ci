using System;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Security.Principal;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;
using WBBBusinessLayer;
using WBBEntity.Extensions;
using WBBEntity.PanelModels.FBBWebConfigModels;
using WBBContract.Queries.FBBWebConfigQueries.SAPOnline;
using WBBContract;
using WBBContract.Commands;
using Renci.SshNet;
using WBBContract.Commands.FBBWebConfigCommands;

namespace FBBInventoryReconcile
{

    public class CredentialHelper
    {

        public ILogger _logger;
        private readonly ICommandHandler<InterfaceLogPayGCommand> _intfLog;
        public CredentialHelper(ILogger logger, ICommandHandler<InterfaceLogPayGCommand> intfLog)
        {
            this._logger = logger;
            _intfLog = intfLog;
        }

        protected int _dayCallback = ConfigurationManager.AppSettings["DayCallback"].ToSafeInteger();
        protected int _dayArchiveForOrigin = ConfigurationManager.AppSettings["DayArchiveForOrigin"].ToSafeInteger();
        protected int _dayArchiveForDestination = ConfigurationManager.AppSettings["DayArchiveForDestination"].ToSafeInteger();
        protected string _archivePath = ConfigurationManager.AppSettings["archive"].ToSafeString();
        protected string _archiveLogPath = ConfigurationManager.AppSettings["archiveLog"].ToSafeString();
        protected string _archiveLocalPath = ConfigurationManager.AppSettings["archiveLocal"].ToSafeString();
        public string Host { get; set; }
        public int Port { get; set; }


        private InterfaceLogPayGCommand StartInterfaceLog<T>(T inXmlParam, InterfaceLogPayGCommand log)
        {
            var dbIntfCmd = new InterfaceLogPayGCommand
            {
                ActionType = ActionType.Insert,
                IN_TRANSACTION_ID = log.IN_TRANSACTION_ID,
                METHOD_NAME = log.METHOD_NAME,
                SERVICE_NAME = "Batch FBBPayG_LoadFile_3BBReport",
                IN_ID_CARD_NO = "",
                IN_XML_PARAM = log.IN_XML_PARAM.DumpToXml(),
                INTERFACE_NODE = log.INTERFACE_NODE,
                CREATED_BY = "FBBPAYG_BATCH",
                CREATED_DATE = DateTime.Now
            };
            _intfLog.Handle(dbIntfCmd);

            return dbIntfCmd;
        }
        private void EndInterfaceLog<T>(T outXmlParam, InterfaceLogPayGCommand log)
        {
            var dbEndIntfCmd = new InterfaceLogPayGCommand
            {
                ActionType = ActionType.Update,
                IN_TRANSACTION_ID = "",
                OutInterfaceLogId = log.OutInterfaceLogId,
                REQUEST_STATUS = log.REQUEST_STATUS,
                OUT_RESULT = log.OUT_RESULT,
                OUT_ERROR_RESULT = log.OUT_ERROR_RESULT,
                OUT_XML_PARAM = log.OUT_XML_PARAM.DumpToXml(),
                UPDATED_BY = "FBBPAYG_Batch",
                INTERFACE_NODE = log.INTERFACE_NODE,
                UPDATED_DATE = DateTime.Now
            };
            _intfLog.Handle(dbEndIntfCmd);
        }

        const int LOGON32_PROVIDER_DEFAULT = 0;
        // This parameter causes LogonUser to create a primary token.
        const int LOGON32_LOGON_INTERACTIVE = 9;

        [DllImport("advapi32.DLL", SetLastError = true)]
        public static extern int LogonUser(string lpszUsername, string lpszDomain, string lpszPassword, int dwLogonType,
            int dwLogonProvider, ref IntPtr phToken);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public extern static bool CloseHandle(IntPtr handle);

        private List<AuthenticationMethod> SetAuthenticationMethod(string UserName, string KeyFile)
        {
            var privateKey = new PrivateKeyFile(KeyFile);
            List<AuthenticationMethod> _methods = new List<AuthenticationMethod>();
            var keyFiles = new[] { privateKey };
            _methods.Add(new PrivateKeyAuthenticationMethod(UserName, keyFiles));

            return _methods;
        }




        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="pwd"></param>
        /// <param name="domin"></param>
        /// <param name="nas_path"></param>
        /// <param name="date_delete"></param>
        /// <returns></returns>
        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public string RemoveFile(string username, string pwd, string domin, string nas_path, List<FbssConfigTBL> date_delete, string config_filename)
        {
            _logger.Info((object)("--Process Delete Start--"));

            string returnmsg = "";
            string inParam = "";

            var widCurrent = WindowsIdentity.GetCurrent();
            WindowsImpersonationContext wic = null;
            var isCopySucess = false;
            try
            {
                var adminToken = new IntPtr();
                if (LogonUser(username, domin, pwd, LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT, ref adminToken) != 0)
                {
                    wic = new WindowsIdentity(adminToken).Impersonate();
                    foreach (var item in date_delete)
                    {

                        int val1 = int.Parse(item.VAL1);
                        DateTime resultDate = DateTime.Now.AddDays(-val1);
                        string con_nas_path = nas_path.Replace("\\\\", "\\");
                        var files_delete = (from file in Directory.EnumerateFiles(nas_path) select file).ToList();
                        foreach (var file in files_delete)// วนไฟล์ใน path
                        {
                            DateTime firstModified = System.IO.File.GetLastWriteTime(file);// get date modified ของ file
                            if (firstModified < resultDate)// เช็คว่าไฟล์ไหนมาก่อน resultDate
                            {

                                
                                string fileName = Path.GetFileName(file);

                                int lastUnderscoreIndex = fileName.LastIndexOf('_');
                                int secondToLastUnderscoreIndex = fileName.LastIndexOf('_', lastUnderscoreIndex - 1);
                                string result_fileName = fileName.Substring(0, secondToLastUnderscoreIndex + 1);
                                if (result_fileName == config_filename)
                                {
                                    _logger.Info((object)(" : " + result_fileName));
                                    _logger.Info((object)(" : " + firstModified));
                                    if (File.Exists(file))
                                    {
                                        File.Delete(file);
                                        _logger.Info((object)(" : " + "File Name: " + fileName + " Deleted"));
                                    }
                                    else
                                    {
                                        _logger.Info((object)("File Name: " + fileName + " Not Exist"));
                                    }
                                }
                            }

                        }
                    }


                }
                else
                {
                    this._logger.Info((object)("Login failed"));
                }
            }
            catch (Exception ex)
            {
                this._logger.Info((object)("UPLOAD PROCESS " + ex.ToString()));

            }
            finally
            {
                if (wic != null)
                    wic.Undo();
            }
            return returnmsg;
        }

        [PermissionSetAttribute(SecurityAction.Demand, Name = "FullTrust")]
        public bool WriteFile(string username, string pwd, string domin, string pathName, List<ReconcileCPEHVRReturn> command)
        {
            // ReSharper disable once UnusedVariable
            var widCurrent = WindowsIdentity.GetCurrent();
            WindowsImpersonationContext wic = null;
            var isSuccess = false;
            _logger.Info((object)("--Process Write File Start--"));

            string output_path = pathName;
            try
            {
                var adminToken = new IntPtr();
                if (LogonUser(username, domin, pwd, LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT, ref adminToken) != 0)
                {
                    wic = new WindowsIdentity(adminToken).Impersonate();

                    foreach (var item in command)
                    {

                        try
                        {
                            string output_filename = item.output_filename;

                            string data_file = item.data_file;
                            string filePath = Path.Combine(output_path, output_filename);

                            if (Directory.Exists(output_path))
                            {
                                File.WriteAllText(filePath, data_file);

                                _logger.Info("Generated file name: " + output_filename + " Success");
                            }
                            else
                            {
                                _logger.Info("Path not found : " + output_path);
                            }

                        }
                        catch (Exception ex)
                        {
                            _logger.Info("Generate File Failed");
                            _logger.Info(ex);
                        }

                    }
                }
            }
            catch (Exception e)
            {
                _logger.Info("Generate File Destination Error! : " + e.Message);
                isSuccess = false;
            }

            return isSuccess;
        }
        //private bool GenerateFile(IList<ReconcileCPEHVRReturn> datas, string directoryPath, string fileName)
        //{
        //    string output_path = @"C:\Users\User\Documents\fbb-web-register-new\Presentation\Jobs\FBBInventoryReconcile\bin\Debug\WriteFileeeeeeeeeeeeeeeeeeeee";
           
        //    foreach (var item in datas)
        //    {

        //        try
        //        {
        //            string output_filename = item.output_filename;
        //            string data_file = item.data_file;
        //            //string output_path = item.output_path;
        //            string filePath = Path.Combine(output_path, output_filename);
        //            if (Directory.Exists(output_path))
        //            {
        //                File.WriteAllText(filePath, data_file);

        //                _logger.Info("Generated file name: " + output_filename + " Success");
        //            }
        //            else
        //            {
        //                _logger.Info("Path not found : " + output_path);
        //            }
                        
        //        }
        //        catch (Exception ex)
        //        {
        //            _logger.Info("Generate File Failed");
        //            _logger.Info(ex);
        //        }

        //    }
        //    return true;
        //}
        
    }
}
