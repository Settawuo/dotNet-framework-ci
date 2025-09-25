using System;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Security.Principal;
using System.Collections.Generic;
using System.IO;
using WBBBusinessLayer;
using WBBEntity.Extensions;
using WBBEntity.PanelModels.FBBWebConfigModels;
using WBBContract;
using WBBContract.Commands;

namespace FBBPAYG_GenFile_DisneyPB
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

        public CredentialHelper(ILogger logger)
        {
            this._logger = logger;
        }

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

        //private List<AuthenticationMethod> SetAuthenticationMethod(string UserName, string KeyFile)
        //{
        //    var privateKey = new PrivateKeyFile(KeyFile);
        //    List<AuthenticationMethod> _methods = new List<AuthenticationMethod>();
        //    var keyFiles = new[] { privateKey };
        //    _methods.Add(new PrivateKeyAuthenticationMethod(UserName, keyFiles));

        //    return _methods;
        //}


        public List<string> UploadFileToServer(string username, string pwd, string domin, string destinationFilePath)
        {
            // ReSharper disable once UnusedVariable
            var widCurrent = WindowsIdentity.GetCurrent();
            // ReSharper disable once NotAccessedVariable
            InterfaceLogCommand log = new InterfaceLogCommand();
            WindowsImpersonationContext wic = null;
            List<string> resp = new List<string>();
            bool isRemoveSucess;


            try
            {
                var adminToken = new IntPtr();
                if (LogonUser(username, domin, pwd, 9, 0, ref adminToken) != 0)
                {
                    wic = new WindowsIdentity(adminToken).Impersonate();
                    string pathNewDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Temp/");

                    if (Directory.Exists(pathNewDirectory))
                    {
                        foreach (string filePath in Directory.GetFiles(pathNewDirectory))
                        {
                            string fileName = Path.GetFileName(filePath);

                            string destinationFileName = Path.Combine(destinationFilePath, fileName);

                            try
                            {
                                //this._logger.Info((object)("Upload Process Start"));
                                File.Copy(filePath, destinationFileName, true);
                                this._logger.Info((object)($"Upload file: " + Path.GetFileName(filePath) + ""));
                                File.Delete(filePath);
                            }
                            catch (Exception ex)
                            {
                                this._logger.Info((object)($"Upload failed for file: " + Path.GetFileName(filePath) + " . Error: {ex.Message}"));
                            }
                        }
                    }
                }
                else
                {
                    this._logger.Info("UPLOAD PROCESS FAIL (CONNECT NAS TEMP)");
                    isRemoveSucess = false;
                }
                return resp;
            }
            catch (Exception ex)
            {
                this._logger.Info((object)("UPLOAD PROCESS " + ex.ToString()));
                resp.Add(ex.ToSafeString());
            }
            finally
            {
                if (wic != null)
                    wic.Undo();
            }
            return resp;
        }



    }
}
